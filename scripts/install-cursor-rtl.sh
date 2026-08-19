#!/usr/bin/env bash
set -euo pipefail

VERSION="1.2.4"
VSIX_URL="https://github.com/motcke/cursor-ext-rtl/releases/download/v${VERSION}/cursor-rtl.vsix"
EXT_ID="motcke.cursor-rtl-${VERSION}"
TMP_DIR="/tmp/cursor-rtl-install"

mkdir -p "$TMP_DIR" \
  "${HOME}/.cursor-server/extensions" \
  "${HOME}/.cursor/extensions"

curl -fsSL -o "${TMP_DIR}/cursor-rtl.vsix" "$VSIX_URL"
rm -rf "${TMP_DIR}/extracted"
unzip -qo "${TMP_DIR}/cursor-rtl.vsix" -d "${TMP_DIR}/extracted"

for base in "${HOME}/.cursor-server/extensions" "${HOME}/.cursor/extensions"; do
  rm -rf "${base}/${EXT_ID}"
  cp -r "${TMP_DIR}/extracted/extension" "${base}/${EXT_ID}"
done

echo "Installed Cursor RTL (${EXT_ID})"
