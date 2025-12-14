import json
import re
import sys

if len(sys.argv) < 2:
    print("Usage: python remove-from-sw-assets.py <path-to-service-worker-assets.js>")
    sys.exit(1)

assets_file = sys.argv[1]

try:
    with open(assets_file, 'r', encoding='utf-8') as f:
        content = f.read()
    
    match = re.search(r'self\.assetsManifest = ({.*});', content, re.DOTALL)
    if not match:
        print(f"Could not find assetsManifest in {assets_file}")
        sys.exit(1)
    
    manifest = json.loads(match.group(1))
    
    # Remove files that were replaced during build (integrity hashes are invalid)
    files_to_remove = ['app-config.js', 'manifest.webmanifest']
    original_count = len(manifest['assets'])
    manifest['assets'] = [
        asset for asset in manifest['assets'] 
        if asset.get('url') not in files_to_remove
    ]
    removed_count = original_count - len(manifest['assets'])
    
    new_content = f'self.assetsManifest = {json.dumps(manifest, indent=2)};'
    
    with open(assets_file, 'w', encoding='utf-8') as f:
        f.write(new_content)
    
    print(f"Removed {removed_count} asset(s) from service worker manifest")
    sys.exit(0)
    
except Exception as e:
    print(f"Error: {e}")
    sys.exit(1)
