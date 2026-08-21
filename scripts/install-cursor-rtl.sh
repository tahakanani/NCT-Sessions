#!/usr/bin/env bash
# Install Cursor RTL from the bundled VSIX (scripts/cursor-rtl.vsix).
# Chat/Composer RTL still requires a LOCAL Cursor install — see docs/CURSOR-RTL-FA.md
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
VSIX="${CURSOR_RTL_VSIX:-${SCRIPT_DIR}/cursor-rtl.vsix}"
VERSION="1.2.4"
EXT_ID="motcke.cursor-rtl-${VERSION}"
TMP_DIR="/tmp/cursor-rtl-install"

cat <<'EOF'
========================================
  Cursor RTL — install from VSIX
========================================

Chat/Composer RTL only works on YOUR local Cursor app.
This script installs the bundled VSIX on this machine.

Local Cursor: Ctrl+Shift+P → Extensions: Install from VSIX...
Select: scripts/cursor-rtl.vsix
Then: status bar RTL: OFF → Enable RTL → quit all windows
========================================
EOF

if [[ ! -f "$VSIX" ]]; then
  echo "VSIX not found: $VSIX" >&2
  exit 1
fi

mkdir -p "$TMP_DIR" "${HOME}/.cursor-server/extensions" "${HOME}/.cursor/extensions"
rm -rf "${TMP_DIR}/extracted"
unzip -qo "$VSIX" -d "${TMP_DIR}/extracted"

PKG_VERSION="$(python3 -c 'import json,sys; print(json.load(open(sys.argv[1])).get("version",""))' "${TMP_DIR}/extracted/extension/package.json")"
if [[ -n "$PKG_VERSION" ]]; then
  VERSION="$PKG_VERSION"
  EXT_ID="motcke.cursor-rtl-${VERSION}"
fi

for base in "${HOME}/.cursor-server/extensions" "${HOME}/.cursor/extensions"; do
  rm -rf "${base}/${EXT_ID}"
  cp -a "${TMP_DIR}/extracted/extension" "${base}/${EXT_ID}"
  python3 - "$base" "$EXT_ID" "$VERSION" <<'PY'
import json, os, sys, time
base, ext_id, version = sys.argv[1], sys.argv[2], sys.argv[3]
path = os.path.join(base, "extensions.json")
entries = []
if os.path.isfile(path):
    try:
        with open(path, encoding="utf-8") as f:
            entries = json.load(f)
        if not isinstance(entries, list):
            entries = []
    except Exception:
        entries = []
entries = [e for e in entries if (e.get("identifier") or {}).get("id") != "motcke.cursor-rtl"]
loc = os.path.join(base, ext_id)
entries.append({
    "identifier": {"id": "motcke.cursor-rtl"},
    "version": version,
    "location": {"$mid": 1, "fsPath": loc, "path": loc, "scheme": "file"},
    "relativeLocation": ext_id,
    "metadata": {
        "isApplicationScoped": False,
        "installedTimestamp": int(time.time() * 1000),
        "pinned": True,
        "source": "vsix",
        "isPreReleaseVersion": False,
    },
})
with open(path, "w", encoding="utf-8") as f:
    json.dump(entries, f)
PY
done

echo "Installed ${EXT_ID} from ${VSIX}"
echo "Locations:"
echo "  ${HOME}/.cursor-server/extensions/${EXT_ID}"
echo "  ${HOME}/.cursor/extensions/${EXT_ID}"
