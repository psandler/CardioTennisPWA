# Config-Based Environment Configuration ?

## Solution Overview

This is a **true config-based approach** that uses environment-specific configuration files that are swapped at build time via MSBuild. **No post-publish file modifications**, **no integrity hash issues**.

## How It Works

### 1. Environment-Specific Configuration Files

**Development (Default):**
- `wwwroot/app-config.js` - Base URL: `/`
- `wwwroot/manifest.webmanifest` - Relative paths

**Production/Release (GitHub Pages):**
- `wwwroot/app-config.Release.js` - Base URL: `/CardioTennisPWA/`
- `wwwroot/manifest.Release.webmanifest` - Absolute paths for GitHub Pages

### 2. MSBuild Target (in CardioTennisPWA.csproj)

```xml
<Target Name="CopyEnvironmentConfig" AfterTargets="Publish">
  <!-- Copies app-config.Release.js ? app-config.js -->
  <!-- Copies manifest.Release.webmanifest ? manifest.webmanifest -->
</Target>
```

### 3. Runtime Configuration Loading

**index.html:**
- Loads `app-config.js` **first** in `<head>`
- Uses `document.write()` to inject `<base href>` **before** HTML parser encounters any `<link>` tags
- This ensures all resources (CSS, manifest, icons) load from the correct base path
- Dynamically sets service worker registration path

**Why `document.write()`?**
- The browser parses HTML sequentially
- Once it sees a `<link>` tag, it immediately requests that resource
- We must set `<base href>` **before** the first `<link>` tag
- `document.write()` executes synchronously during parsing, allowing us to inject the base tag in time

**service-worker.published.js:**
- Imports `app-config.js`
- Uses `self.appConfig.baseUrl` for cache base path

## Files Created

### Configuration Files
1. `wwwroot/app-config.js` - Dev config (baseUrl: "/")
2. `wwwroot/app-config.Release.js` - Prod config (baseUrl: "/CardioTennisPWA/")
3. `wwwroot/appsettings.json` - Blazor config (Dev)
4. `wwwroot/appsettings.Release.json` - Blazor config (Release)
5. `wwwroot/manifest.webmanifest` - Dev manifest
6. `wwwroot/manifest.Release.webmanifest` - Prod manifest
7. `Models/AppSettings.cs` - Config model (optional, for future use)

### Modified Files
1. `CardioTennisPWA.csproj` - Added `CopyEnvironmentConfig` MSBuild target
2. `wwwroot/index.html` - Loads app-config.js, sets base href dynamically
3. `wwwroot/service-worker.published.js` - Imports app-config.js, uses baseUrl
4. `.github/workflows/deploy.yml` - Simplified (no sed commands)

## Benefits

? **No File Modifications** - Files published exactly as built  
? **No SRI Errors** - Integrity hashes remain valid  
? **Clean Separation** - Dev uses `/`, Prod uses `/CardioTennisPWA/`  
? **MSBuild Native** - Uses standard .NET build system  
? **Maintainable** - Change repo name? Update one config file  
? **Testable** - Can publish locally with Release config to verify  

## How to Use

### Local Development
```bash
dotnet run --project CardioTennisPWA
# Uses app-config.js (baseUrl: "/")
# Visit http://localhost:5263
```

### GitHub Pages Deployment
```bash
git push origin main
# Workflow publishes with -c Release
# MSBuild copies app-config.Release.js ? app-config.js
# MSBuild copies manifest.Release.webmanifest ? manifest.webmanifest
# Deployed app uses baseUrl: "/CardioTennisPWA/"
```

### Test Production Build Locally
```bash
dotnet publish -c Release -o publish
# Check publish/wwwroot/app-config.js
# Should have baseUrl: "/CardioTennisPWA/"
```

## Configuration Flow

```
Development:
  wwwroot/app-config.js ????????????? Published as-is
                                       baseUrl: "/"

Release:
  wwwroot/app-config.Release.js ???
                                   ???? Copied to app-config.js
                                   ?    baseUrl: "/CardioTennisPWA/"
  (original app-config.js ignored) ?
```

## Manifest Flow

```
Development:
  wwwroot/manifest.webmanifest ?????? Published as-is
                                       start_url: "./"

Release:
  wwwroot/manifest.Release.webmanifest ???
                                          ???? Copied to manifest.webmanifest
                                          ?    start_url: "/CardioTennisPWA/"
  (original manifest.webmanifest ignored)?
```

## Why This is Better Than Sed Approach

| Aspect | Sed Approach ? | Config Approach ? |
|--------|----------------|-------------------|
| **File Modification** | Post-publish | Build-time |
| **Integrity Hashes** | Broken, need workaround | Remain valid |
| **Workflow Complexity** | Multiple sed/Python steps | Simple publish |
| **Maintainability** | Hardcoded in workflow | Config files |
| **Testability** | Must deploy to test | Can test locally |
| **.NET Integration** | Shell scripts | Native MSBuild |

## Troubleshooting

### Config not copying during publish
Check that files exist:
- `wwwroot/app-config.Release.js`
- `wwwroot/manifest.Release.webmanifest`

Configuration name must match (Release, Debug, etc.)

### Base href not updating
Check browser console - `app-config.js` should load before other resources.
Verify `self.appConfig.baseUrl` is set correctly.

### Service worker errors
Check that service worker imports `app-config.js`:
```javascript
self.importScripts('./app-config.js');
```

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

**Status**: Clean config-based solution ?  
**Approach**: Build-time configuration file swapping via MSBuild  
**SRI Issues**: None - files not modified after publish  
**Complexity**: Low - standard .NET build patterns
