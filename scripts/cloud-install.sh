#!/usr/bin/env bash
# Idempotent Cloud Agent setup for the nct-sessions repo.
#   * installs the .NET SDK (needed to compile the cTrader Automate C# algos),
#   * restores + builds every cTrader indicator/plugin into an .algo file,
#   * runs the Pine Script sanity checker.
# Safe to run repeatedly and against a warm (snapshotted) VM.
set -euo pipefail

DOTNET_CHANNEL="8.0"
DOTNET_DIR="${DOTNET_ROOT:-$HOME/.dotnet}"
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

log() { printf '\n\033[1;34m==> %s\033[0m\n' "$*"; }

# 1. .NET SDK ---------------------------------------------------------------
if [ -x "$DOTNET_DIR/dotnet" ] && "$DOTNET_DIR/dotnet" --list-sdks 2>/dev/null | grep -q "^${DOTNET_CHANNEL%.*}\."; then
  log ".NET SDK already present: $("$DOTNET_DIR/dotnet" --version)"
else
  log "Installing .NET SDK ${DOTNET_CHANNEL} into ${DOTNET_DIR}"
  tmp="$(mktemp -d)"
  curl -fsSL https://dot.net/v1/dotnet-install.sh -o "$tmp/dotnet-install.sh"
  bash "$tmp/dotnet-install.sh" --channel "$DOTNET_CHANNEL" --install-dir "$DOTNET_DIR"
  rm -rf "$tmp"
fi

export DOTNET_ROOT="$DOTNET_DIR"
export PATH="$DOTNET_DIR:$PATH"

# 2. Make `dotnet` visible to future shells without editing shell profiles.
if command -v sudo >/dev/null 2>&1; then
  sudo ln -sf "$DOTNET_DIR/dotnet" /usr/local/bin/dotnet 2>/dev/null || true
fi

# 3. Build all cTrader Automate algos --------------------------------------
log "Building cTrader Automate algos (Release)"
dotnet build "$REPO_ROOT/build/nct-sessions.sln" --configuration Release

# 4. Pine Script sanity check ----------------------------------------------
log "Checking Pine Script files"
python3 "$REPO_ROOT/scripts/check_pine.py"

log "Environment ready. Built .algo files live in build/*/bin/Release/."
