#!/usr/bin/env bash
# Cloud Agent note: chat RTL requires LOCAL Cursor install — see docs/CURSOR-RTL-FA.md
set -euo pipefail

cat <<'EOF'
========================================
  Cursor RTL — Cloud Agent
========================================

Chat/Composer RTL only works on YOUR local Cursor app.
This script installs the extension on the remote VM only.

Follow docs/CURSOR-RTL-FA.md on your computer:
  1. Install @id:motcke.cursor-rtl locally
  2. Status bar: RTL: OFF → Enable RTL
  3. Quit ALL Cursor windows and reopen
========================================
EOF

VERSION="1.2.4"
VSIX_URL="https://github.com/motcke/cursor-ext-rtl/releases/download/v${VERSION}/cursor-rtl.vsix"
EXT_ID="motcke.cursor-rtl-${VERSION}"
TMP_DIR="/tmp/cursor-rtl-install"

if [[ "${INSTALL_REMOTE_RTL:-0}" == "1" ]]; then
  mkdir -p "$TMP_DIR" "${HOME}/.cursor-server/extensions" "${HOME}/.cursor/extensions"
  curl -fsSL -o "${TMP_DIR}/cursor-rtl.vsix" "$VSIX_URL"
  rm -rf "${TMP_DIR}/extracted"
  unzip -qo "${TMP_DIR}/cursor-rtl.vsix" -d "${TMP_DIR}/extracted"
  for base in "${HOME}/.cursor-server/extensions" "${HOME}/.cursor/extensions"; do
    rm -rf "${base}/${EXT_ID}"
    cp -r "${TMP_DIR}/extracted/extension" "${base}/${EXT_ID}"
  done
  echo "Remote extension installed (${EXT_ID})"
fi
