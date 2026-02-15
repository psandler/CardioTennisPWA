# GitHub Pages Deployment Guide

## Automatic Deployment Setup

The project is configured for automatic deployment to GitHub Pages when you push to the `main` branch.

### Steps to Enable GitHub Pages:

1. **Push the workflow file to GitHub:**
   ```bash
   git add .github/workflows/deploy.yml
   git commit -m "Add GitHub Pages deployment workflow"
   git push origin main
   ```

2. **Enable GitHub Pages in repository settings:**
   - Go to your repository: https://github.com/psandler/CardioTennisPWA
   - Click **Settings** ? **Pages**
   - Under **Source**, select **GitHub Actions**
   - Click **Save**

3. **The workflow will run automatically:**
   - Every push to `main` triggers deployment
   - You can also manually trigger from **Actions** tab ? **Deploy to GitHub Pages** ? **Run workflow**

4. **Access your app:**
   - After deployment completes (2-3 minutes), visit:
   - **https://psandler.github.io/CardioTennisPWA/**

### Manual Deployment (Alternative)

If you prefer to deploy manually:

```bash
# Build and publish
dotnet publish CardioTennisPWA/CardioTennisPWA.csproj -c Release -o publish

# Update base href for GitHub Pages subdirectory
# (On Windows PowerShell)
(Get-Content publish/wwwroot/index.html) -replace '<base href="/" />', '<base href="/CardioTennisPWA/" />' | Set-Content publish/wwwroot/index.html

# Add .nojekyll file
New-Item -Path publish/wwwroot/.nojekyll -ItemType File

# Deploy the wwwroot contents to gh-pages branch
# (You'll need to create and push to gh-pages branch manually)
```

### Important Notes:

- **Base href**: The workflow automatically updates `<base href="/" />` to `<base href="/CardioTennisPWA/" />` for GitHub Pages subdirectory routing
- **.nojekyll**: Required to prevent GitHub Pages from processing files with Jekyll (which would break Blazor)
- **.NET 10 Preview**: The workflow uses `dotnet-quality: 'preview'` to get .NET 10

### Troubleshooting:

**If deployment fails:**
1. Check the **Actions** tab for error logs
2. Ensure GitHub Pages is enabled in repository settings
3. Verify repository permissions allow GitHub Actions to deploy

**If app doesn't load:**
1. Check browser console for 404 errors
2. Verify base href matches your repository name
3. Clear browser cache and hard reload (Ctrl+Shift+R)

**To test PWA installation:**
1. Visit the deployed site in Chrome/Edge
2. Look for install prompt in address bar
3. Click install to add to home screen

### Local Testing:

To test locally before deploying:
```bash
dotnet run --project CardioTennisPWA
```

Visit: http://localhost:5000

---

**Next Steps After Deployment:**
1. Test localStorage functionality on deployed site
2. Verify PWA installation works
3. Test offline functionality (disconnect network, reload page)
