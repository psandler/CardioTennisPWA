# PWA Troubleshooting Guide

## How to Check PWA Status on Deployed Site

### 1. Open DevTools (F12)

Visit: https://psandler.github.io/CardioTennisPWA/

### 2. Check Console for Errors
- Look for service worker registration errors
- Look for manifest loading errors

### 3. Check Application Tab

**Service Workers:**
- Application ? Service Workers
- Should show: `https://psandler.github.io/CardioTennisPWA/service-worker.js`
- Status should be: **Activated and is running**

**Manifest:**
- Application ? Manifest
- Should load `manifest.webmanifest`
- Check that `start_url` and `id` are correct: `/CardioTennisPWA/`

### 4. Test Installation

**Desktop (Chrome/Edge):**
- Look for install icon in address bar (? or computer icon)
- Click to install
- App should open in standalone window

**Mobile (Chrome/Safari):**
- Menu ? "Add to Home Screen" or "Install App"
- Should show app name and icon
- Tap to install

## Common Issues and Fixes

### Issue: "Service Worker registration failed"
**Symptoms:** Console error about service worker registration
**Cause:** Incorrect service worker path
**Fix:** The deployment workflow now fixes this automatically (updates `navigator.serviceWorker.register` path)

### Issue: Service worker loads but doesn't activate
**Symptoms:** Service worker shows in DevTools but status is "waiting" or "installing"
**Cause:** Old service worker cached
**Fix:** 
1. Application ? Service Workers ? Check "Update on reload"
2. Click "Unregister"
3. Hard refresh (Ctrl+Shift+R)

### Issue: Manifest doesn't load (404)
**Symptoms:** Console shows 404 for `manifest.webmanifest`
**Cause:** Base href not applied to manifest link
**Fix:** Manifest link is relative, should work automatically with base href

### Issue: Install prompt doesn't appear
**Symptoms:** No install icon in browser
**Possible Causes:**
1. **Already installed** - Check chrome://apps or edge://apps
2. **Not HTTPS** - GitHub Pages uses HTTPS, so this shouldn't be an issue
3. **Manifest invalid** - Check Application ? Manifest for errors
4. **Service worker not registered** - Check Application ? Service Workers

**Fixes:**
1. Uninstall existing app (if installed)
2. Check DevTools ? Application ? Manifest for validation errors
3. Ensure service worker is active
4. Try different browser

### Issue: App works locally but not on GitHub Pages
**Symptoms:** PWA installs locally but not on deployed site
**Cause:** Base path configuration
**Fix:** The deployment workflow now handles:
- `index.html` base href ? `/CardioTennisPWA/`
- Service worker registration ? `/CardioTennisPWA/service-worker.js`
- Service worker base path ? `/CardioTennisPWA/`
- Manifest `start_url` and `id` ? `/CardioTennisPWA/`

## Testing Checklist

After deploying, verify:

- [ ] Site loads at https://psandler.github.io/CardioTennisPWA/
- [ ] No console errors
- [ ] Service worker registers (check DevTools ? Application ? Service Workers)
- [ ] Service worker shows "activated and running"
- [ ] Manifest loads (check DevTools ? Application ? Manifest)
- [ ] Install prompt appears (or already installed)
- [ ] Can install app
- [ ] Installed app opens in standalone window
- [ ] Offline mode works:
  - [ ] Install app
  - [ ] Disconnect network
  - [ ] Close and reopen app
  - [ ] App still loads

## Deployment Workflow Changes (Applied)

The `.github/workflows/deploy.yml` now includes these PWA fixes:

```yaml
- name: Change base-tag in index.html
  run: |
    sed -i 's/<base href="\/" \/>/<base href="\/CardioTennisPWA\/" \/>/g' publish/wwwroot/index.html

- name: Fix service worker registration for GitHub Pages
  run: |
    sed -i "s/navigator.serviceWorker.register('service-worker.js'/navigator.serviceWorker.register('\/CardioTennisPWA\/service-worker.js'/g" publish/wwwroot/index.html

- name: Fix service worker base path
  run: |
    sed -i 's/const base = "\/";/const base = "\/CardioTennisPWA\/";/g' publish/wwwroot/service-worker.published.js
    sed -i 's/const base = "\/";/const base = "\/CardioTennisPWA\/";/g' publish/wwwroot/service-worker.js

- name: Fix manifest scope and start_url
  run: |
    sed -i 's/"start_url": ".\/"/"start_url": "\/CardioTennisPWA\/"/g' publish/wwwroot/manifest.webmanifest
    sed -i 's/"id": ".\/"/"id": "\/CardioTennisPWA\/"/g' publish/wwwroot/manifest.webmanifest
```

## Next Steps

1. **Commit and push the updated workflow:**
   ```bash
   git add .github/workflows/deploy.yml
   git commit -m "Fix PWA paths for GitHub Pages deployment"
   git push origin main
   ```

2. **Wait for deployment** (2-3 minutes)

3. **Test PWA installation:**
   - Visit https://psandler.github.io/CardioTennisPWA/
   - Open DevTools and verify service worker
   - Look for install prompt
   - Install and test

4. **Test offline:**
   - After installing, disconnect network
   - Close and reopen app
   - Should still work

---

**Last Updated**: 2025-12-06
