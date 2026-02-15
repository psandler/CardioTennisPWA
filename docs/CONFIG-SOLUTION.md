# Config-Based Environment Configuration ? (Pre-Publish Approach)

## Solution Overview

This is a **true config-based approach** that uses environment-specific configuration files that are copied **BEFORE** Blazor processes assets. This ensures integrity hashes are generated for the actual deployed files.

**Key Difference from Post-Publish Approach:**
- ? Config files copied **BEFORE** service worker manifest generation
- ? Integrity hashes calculated for the **correct** (Release) files
- ? No need to remove files from cache - hashes are valid
- ? Cleaner, simpler build process

## How It Works

### 1. Environment-Specific Configuration Files

**Development (Debug):**
- `wwwroot/app-config.Debug.js` - Base URL: `/`
- `wwwroot/manifest.Debug.webmanifest` - Relative paths
- These get copied to `app-config.js` and `manifest.webmanifest` for Debug builds

**Production/Release (GitHub Pages):**
- `wwwroot/app-config.Release.js` - Base URL: `/CardioTennisPWA/`
- `wwwroot/manifest.Release.webmanifest` - Absolute paths for GitHub Pages
- These get copied to `app-config.js` and `manifest.webmanifest` for Release builds

**Default Files** (in source control):
- `wwwroot/app-config.js` - Dev version (baseUrl: "/")
- `wwwroot/manifest.webmanifest` - Dev version (relative paths)
- These are **overwritten during non-Debug builds** and restored manually
- **Added to .gitignore** to prevent committing build-generated versions

### 2. MSBuild Target (in CardioTennisPWA.csproj)

```xml
<Target Name="CopyEnvironmentConfigBeforeBuild" BeforeTargets="ResolveStaticWebAssetsInputs">
  <!-- Only copy for non-Debug configurations -->
  <PropertyGroup>
    <ShouldCopyConfig Condition="'$(Configuration)' != 'Debug'">true</ShouldCopyConfig>
  </PropertyGroup>
  
  <!-- Copy app-config.$(Configuration).js ? app-config.js -->
  <!-- Copy manifest.$(Configuration).webmanifest ? manifest.webmanifest -->
  <!-- This happens BEFORE Blazor processes assets -->
</Target>
```

**Key Points:**
- Runs `BeforeTargets="ResolveStaticWebAssetsInputs"` (early in build process)
- Copies to **source** `wwwroot/` (not publish output)
- Only runs for non-Debug configurations (Release, etc.)
- Files are in place when Blazor generates service worker assets manifest

### 3. Service Worker Assets Manifest Generation

**What Happens:**
1. MSBuild target copies Release config files to `wwwroot/`
2. Blazor's `GenerateServiceWorkerAssetsManifest` task runs
3. Calculates SHA-256 hash for `app-config.js` (**Release version**)
4. Adds to `service-worker-assets.js` with **valid hash**
5. Service worker can cache it without SRI errors!

### 4. Runtime Configuration Loading

(Same as before - no changes needed to index.html or service worker)

## Files Created/Modified

### New Files Created
1. `wwwroot/app-config.Debug.js` - Dev config (baseUrl: "/")
2. `wwwroot/app-config.Release.js` - Prod config (baseUrl: "/CardioTennisPWA/")
3. `wwwroot/appsettings.json` - Blazor config (Dev)
4. `wwwroot/appsettings.Release.json` - Blazor config (Release)
5. `wwwroot/manifest.Debug.webmanifest` - Dev manifest
6. `wwwroot/manifest.Release.webmanifest` - Prod manifest with absolute paths
7. `Models/AppSettings.cs` - Config model (optional)

### Modified Files
1. `CardioTennisPWA.csproj` - Added `CopyEnvironmentConfigBeforeBuild` MSBuild target
2. `wwwroot/index.html` - Loads app-config.js, uses document.write() to inject base href
3. `wwwroot/service-worker.published.js` - Imports app-config.js, uses baseUrl (NO exclusion needed!)
4. `.github/workflows/deploy.yml` - Simplified (just publish, no sed)
5. `.gitignore` - Added `app-config.js` and `manifest.webmanifest` to ignore list

### Removed Files
1. ~~`build-scripts/remove-from-sw-assets.py`~~ - No longer needed!

## Benefits

? **Valid Integrity Hashes** - Files cached with correct SRI checks  
? **Simpler Build** - No post-publish cleanup needed  
? **Cleaner** - No Python script, one MSBuild step  
? **Better Offline** - Config files properly cached by service worker  
? **Standard .NET** - Uses MSBuild targets correctly  
? **Easy to Maintain** - Change repo name? Update one config file  

## Tradeoffs

?? **Source files modified during build** - `app-config.js` and `manifest.webmanifest` overwritten  
?? **Added to .gitignore** - These files ignored to prevent committing build versions  
?? **Manual restore** - After Release build, dev versions need manual copy (or clean checkout)  

## How to Use

### Local Development (Debug)
```bash
dotnet run --project CardioTennisPWA
# Uses app-config.Debug.js ? app-config.js (baseUrl: "/")
# Visit http://localhost:5263
```

### GitHub Pages Deployment (Release)
```bash
git push origin main
# Workflow publishes with -c Release
# MSBuild copies app-config.Release.js ? app-config.js BEFORE asset processing
# Service worker assets manifest gets correct hash
# Deployed app uses baseUrl: "/CardioTennisPWA/"
```

### Test Production Build Locally
```bash
dotnet publish -c Release -o publish
# Check publish/wwwroot/service-worker-assets.js
# Should have app-config.js with valid hash
```

## Configuration Flow

```
Release Build:
  1. MSBuild target runs (BeforeTargets="ResolveStaticWebAssetsInputs")
  2. Copies app-config.Release.js ? wwwroot/app-config.js
  3. Copies manifest.Release.webmanifest ? wwwroot/manifest.webmanifest
  4. Blazor processes assets
  5. Generates service-worker-assets.js with hash for Release version
  6. ? Hash is valid for deployed file!

Debug Build:
  1. MSBuild target skipped (Configuration == 'Debug')
  2. Uses existing wwwroot/app-config.js (Dev version)
  3. Uses existing wwwroot/manifest.webmanifest (Dev version)
  4. ? Local development works with baseUrl: "/"
```

## Why This is Better Than Post-Publish

| Aspect | Post-Publish ? | Pre-Publish ? |
|--------|----------------|----------------|
| **Integrity Hashes** | Invalid (files modified after) | Valid (calculated for final files) |
| **Config Cached** | No (excluded from SW cache) | Yes (with valid hash) |
| **Build Steps** | 2 (copy + remove from manifest) | 1 (copy before build) |
| **Python Script** | Required | Not needed |
| **Offline Performance** | Small extra request | Fully cached |
| **Complexity** | Higher (post-process cleanup) | Lower (standard MSBuild) |

## Troubleshooting

### Config not copying during build
Check that files exist:
- `wwwroot/app-config.Release.js`
- `wwwroot/manifest.Release.webmanifest`

Configuration name must match (Release, Debug, etc.)

### Dev versions overwritten after Release build
This is expected. Restore them:
```bash
Copy-Item wwwroot/app-config.Debug.js wwwroot/app-config.js -Force
Copy-Item wwwroot/manifest.Debug.webmanifest wwwroot/manifest.webmanifest -Force
```

Or do a clean git checkout:
```bash
git checkout -- CardioTennisPWA/wwwroot/app-config.js
git checkout -- CardioTennisPWA/wwwroot/manifest.webmanifest
```

### SRI errors still happening
Check `service-worker-assets.js` - should have `app-config.js` with a hash.
If missing, MSBuild target didn't run early enough.

## Repository Name Change

To change repository name from "CardioTennisPWA" to something else:

1. Update `wwwroot/app-config.Release.js`:
   ```javascript
   self.appConfig = {
       baseUrl: "/NewRepoName/"
   };
   ```

2. Update `wwwroot/manifest.Release.webmanifest`:
   ```json
   {
     "id": "/NewRepoName/",
     "start_url": "/NewRepoName/"
   }
   ```

3. Done! No workflow changes needed.

---

**Status**: Clean pre-publish config solution ?  
**Approach**: Copy config files BEFORE asset manifest generation  
**SRI Issues**: None - integrity hashes are valid  
**Complexity**: Low - single MSBuild target, no post-processing
