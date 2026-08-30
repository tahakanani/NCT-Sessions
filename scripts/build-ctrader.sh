#!/usr/bin/env bash
# Build the cTrader Automate indicators/plugin into .algo files.
# Usage: scripts/build-ctrader.sh [Release|Debug]
set -euo pipefail
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
CONFIG="${1:-Release}"

if command -v dotnet >/dev/null 2>&1; then
  DOTNET_BIN="dotnet"
elif [ -x "$HOME/.dotnet/dotnet" ]; then
  DOTNET_BIN="$HOME/.dotnet/dotnet"
else
  echo "dotnet not found. Run scripts/cloud-install.sh first." >&2
  exit 1
fi

exec "$DOTNET_BIN" build "$REPO_ROOT/build/nct-sessions.sln" --configuration "$CONFIG"
