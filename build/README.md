# Building the cTrader Automate algos

The C# indicators/plugin in [`../cTrader`](../cTrader) are compiled into cTrader
`.algo` extensions with the official [`cTrader.Automate`](https://www.nuget.org/packages/cTrader.Automate)
NuGet package and the .NET SDK — no cTrader Desktop required.

## Layout

| Project | Source | Output |
| --- | --- | --- |
| `NCT` | `cTrader/NCT.cs` (`cAlgo.Indicators.TAHA3`) | `NCT/bin/Release/NCT.algo` |
| `MAPWeekly` | `cTrader/MAPWeekly.cs` (`cAlgo.Indicators.MAPWeekly`) | `MAPWeekly/bin/Release/MAPWeekly.algo` |
| `CustomRangeBars` | `cTrader/CustomRangeBarsPlugin.cs` (`cAlgo.Plugins.CustomRangeBarsPlugin`) | `CustomRangeBars/bin/Release/CustomRangeBars.algo` |

Each `.csproj` compiles a single source file from `../cTrader` so the canonical
sources stay in one place. Shared settings (target framework, package
reference) live in `Directory.Build.props`; `Directory.Build.targets` forces the
`.algo` file name to match the project name.

## Build

```bash
# from the repo root — installs the .NET SDK if needed, builds every algo,
# then runs the Pine sanity check
bash scripts/cloud-install.sh

# or just build the algos (SDK already installed)
bash scripts/build-ctrader.sh          # Release
bash scripts/build-ctrader.sh Debug    # Debug

# or with the .NET CLI directly
dotnet build build/nct-sessions.sln -c Release
```

The built `.algo` files (and a matching `.algo.metadata`) appear under each
project's `bin/Release/`. Copy the `.algo` into cTrader, or import the source
`.cs` via **Automate → New → Indicator/Plugin** as described in
[`../cTrader/README.txt`](../cTrader/README.txt).

## Pine Script

The `.pine` files run on TradingView (there is no offline Pine compiler).
`scripts/check_pine.py` performs a structural sanity check (version pragma,
declaration, bracket balance):

```bash
python3 scripts/check_pine.py
```
