// NCT Dual Symmetry Indicator for cTrader Automate
// Paste this entire file over TAHA3.cs (select all, replace). Do not merge with old Pine code.
using System;
using System.Collections.Generic;
using cAlgo.API;
using cAlgo.API.Internals;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class TAHA3 : Indicator
    {
        // ───────────────────────── Nodal / Strategy ─────────────────────────

        [Parameter("Starting Point Candles", DefaultValue = 5000, MinValue = 1, MaxValue = 20000, Group = "Nodal Calculation Settings")]
        public int StartPoint { get; set; }

        [Parameter("End Point (Final)", DefaultValue = 0, MinValue = 0, MaxValue = 20000, Group = "Nodal Calculation Settings")]
        public int EndPoint { get; set; }

        [Parameter("Calc Nodes with Symmetry", DefaultValue = true, Group = "Strategy Config")]
        public bool SwCalcSymmetry { get; set; }

        [Parameter("Calc Nodes with Logarithm", DefaultValue = true, Group = "Strategy Config")]
        public bool SwCalcLogarithm { get; set; }

        [Parameter("Show Regular Nodes", DefaultValue = true, Group = "Strategy Config")]
        public bool ShowRegularNodes { get; set; }

        [Parameter("Show Starred Symmetry Nodes (*)", DefaultValue = true, Group = "Strategy Config")]
        public bool ShowDoubleStarNodes { get; set; }

        [Parameter("Show Star Suffix (*)", DefaultValue = true, Group = "Strategy Config")]
        public bool ShowStarSuffix { get; set; }

        [Parameter("Show Node Connecting Lines", DefaultValue = false, Group = "Strategy Config")]
        public bool ShowNodeConnectingLines { get; set; }

        // ───────────────────────── Visual ─────────────────────────

        [Parameter("Font Size (Node Numbers)", DefaultValue = "Normal", Group = "Visual Customization")]
        public string TextNodeSize { get; set; }

        [Parameter("Target Font Size", DefaultValue = 10, MinValue = 1, MaxValue = 72, Group = "Visual Customization")]
        public int TargetLabelFontSize { get; set; }

        [Parameter("Incomplete 2 Circle", DefaultValue = true, Group = "Visual Customization")]
        public bool ShowIncomplete2Circle { get; set; }

        [Parameter("Incomplete 2 Circle Radius (%)", DefaultValue = 0.004, MinValue = 0.001, MaxValue = 5.0, Group = "Visual Customization")]
        public double Incomplete2RadiusPct { get; set; }

        [Parameter("Incomplete 2 Circle Radius (Bars)", DefaultValue = 1, MinValue = 1, MaxValue = 50, Group = "Visual Customization")]
        public int Incomplete2RadiusBars { get; set; }

        [Parameter("Incomplete 2 Circle Fill Transparency", DefaultValue = 40, MinValue = 0, MaxValue = 100, Group = "Visual Customization")]
        public int Incomplete2FillTransp { get; set; }

        [Parameter("Min1 ↔ 0.8DL.1 Green Circle", DefaultValue = true, Group = "Visual Customization")]
        public bool ShowProxCircle { get; set; }

        [Parameter("Min1 ↔ 0.8DL.1 Proximity (%)", DefaultValue = 0.15, MinValue = 0.01, MaxValue = 5.0, Group = "Visual Customization")]
        public double MinDblProximityTolPct { get; set; }

        [Parameter("Proximity Circle Radius (%)", DefaultValue = 0.002, MinValue = 0.001, MaxValue = 5.0, Group = "Visual Customization")]
        public double ProxRadiusPct { get; set; }

        [Parameter("Proximity Circle Radius (Bars)", DefaultValue = 1, MinValue = 1, MaxValue = 50, Group = "Visual Customization")]
        public int ProxRadiusBars { get; set; }

        [Parameter("Proximity Circle Fill Transparency", DefaultValue = 40, MinValue = 0, MaxValue = 100, Group = "Visual Customization")]
        public int ProxFillTransp { get; set; }

        [Parameter("Color 1", DefaultValue = "#FFE566", Group = "Visual Customization")]
        public string Color1Name { get; set; }

        [Parameter("Color 2", DefaultValue = "#7AA2FF", Group = "Visual Customization")]
        public string Color2Name { get; set; }

        [Parameter("Color 3", DefaultValue = "#FF9F1C", Group = "Visual Customization")]
        public string Color3Name { get; set; }

        [Parameter("Color 4", DefaultValue = "#2DE2E6", Group = "Visual Customization")]
        public string Color4Name { get; set; }

        [Parameter("Color 5", DefaultValue = "#FF5AD9", Group = "Visual Customization")]
        public string Color5Name { get; set; }

        [Parameter("Color 6", DefaultValue = "#7CFF47", Group = "Visual Customization")]
        public string Color6Name { get; set; }

        [Parameter("Color 7", DefaultValue = "#FF4D6D", Group = "Visual Customization")]
        public string Color7Name { get; set; }

        [Parameter("Color 8", DefaultValue = "#C084FC", Group = "Visual Customization")]
        public string Color8Name { get; set; }

        // ───────────────────────── Node Targets ─────────────────────────

        [Parameter("Enable Single Node1 Targets", DefaultValue = true, Group = "Node Targets")]
        public bool EnableSingleNode1Targets { get; set; }

        [Parameter("Enable Pair Node12 Targets", DefaultValue = true, Group = "Node Targets")]
        public bool EnablePairNode12Targets { get; set; }

        [Parameter("Show Target Up", DefaultValue = true, Group = "Node Targets")]
        public bool ShowTargetUp { get; set; }

        [Parameter("Show Target Down", DefaultValue = true, Group = "Node Targets")]
        public bool ShowTargetDown { get; set; }

        [Parameter("Show Double.1 (Single)", DefaultValue = true, Group = "Node Targets")]
        public bool ShowDouble { get; set; }

        [Parameter("Show 1.5DL.1 (Single)", DefaultValue = true, Group = "Node Targets")]
        public bool ShowDouble15 { get; set; }

        [Parameter("1.5DL.1 Ratio", DefaultValue = 1.5, MinValue = 1.0, MaxValue = 5.0, Group = "Node Targets")]
        public double Double15Ratio { get; set; }

        [Parameter("Show 0.8DL.1 (Single)", DefaultValue = true, Group = "Node Targets")]
        public bool ShowDouble086 { get; set; }

        [Parameter("Start→Double Ratio", DefaultValue = 0.85, MinValue = 0.01, MaxValue = 2.0, Group = "Node Targets")]
        public double Double086Ratio { get; set; }

        [Parameter("0.8DL.1 Line Width", DefaultValue = 1, MinValue = 1, MaxValue = 5, Group = "Node Targets")]
        public int Double086LineWidth { get; set; }

        [Parameter("0.8DL.1 Line Style", DefaultValue = "Dotted", Group = "Node Targets")]
        public string Double086LineStyleName { get; set; }

        [Parameter("Show Min (Single)", DefaultValue = true, Group = "Node Targets")]
        public bool ShowMin { get; set; }

        [Parameter("Show 0.8Min.1 (Incomplete 2)", DefaultValue = true, Group = "Node Targets")]
        public bool ShowMin085 { get; set; }

        [Parameter("0.8Min.1 Ratio", DefaultValue = 0.85, MinValue = 0.01, MaxValue = 2.0, Group = "Node Targets")]
        public double Min085Ratio { get; set; }

        [Parameter("Show 1.3MIN1 (Based Node 1)", DefaultValue = true, Group = "Node Targets")]
        public bool ShowMin13Based { get; set; }

        [Parameter("Based Retrace Ratio", DefaultValue = 0.85, MinValue = 0.01, MaxValue = 1.0, Group = "Node Targets")]
        public double BasedRetraceRatio { get; set; }

        [Parameter("Based Min Extension", DefaultValue = 1.3, MinValue = 1.0, MaxValue = 5.0, Group = "Node Targets")]
        public double BasedMin13Ratio { get; set; }

        [Parameter("Show Correction (Single)", DefaultValue = false, Group = "Node Targets")]
        public bool ShowCorrection { get; set; }

        [Parameter("Show Pair Min", DefaultValue = true, Group = "Node Targets")]
        public bool ShowPairMin { get; set; }

        [Parameter("Show Pair Max", DefaultValue = true, Group = "Node Targets")]
        public bool ShowPairMax { get; set; }

        [Parameter("Show Pair Double", DefaultValue = true, Group = "Node Targets")]
        public bool ShowPairDouble { get; set; }

        [Parameter("Show Pair Correction", DefaultValue = false, Group = "Node Targets")]
        public bool ShowPairCorrection { get; set; }

        [Parameter("Target Gap Bars", DefaultValue = 150, MinValue = 3, MaxValue = 500, Group = "Node Targets")]
        public int TargetGapBars { get; set; }

        [Parameter("Delete Hit Targets", DefaultValue = true, Group = "Node Targets")]
        public bool DeleteHitTargets { get; set; }

        [Parameter("Hit Grace Bars", DefaultValue = 2, MinValue = 0, MaxValue = 500, Group = "Node Targets")]
        public int HitGraceBars { get; set; }

        [Parameter("Target Max Count (0 = All)", DefaultValue = 0, MinValue = 0, MaxValue = 5000, Group = "Node Targets")]
        public int TargetMaxCount { get; set; }

        [Parameter("Pair Max Count (0 = All)", DefaultValue = 0, MinValue = 0, MaxValue = 5000, Group = "Node Targets")]
        public int PairMaxCount { get; set; }

        [Parameter("Double Line Width", DefaultValue = 1, MinValue = 1, MaxValue = 5, Group = "Node Targets")]
        public int DoubleLineWidth { get; set; }

        [Parameter("Min Line Width", DefaultValue = 3, MinValue = 1, MaxValue = 5, Group = "Node Targets")]
        public int MinLineWidth { get; set; }

        [Parameter("Correction Line Width", DefaultValue = 1, MinValue = 1, MaxValue = 5, Group = "Node Targets")]
        public int CorrectionLineWidth { get; set; }

        [Parameter("Pair Max Line Width", DefaultValue = 3, MinValue = 1, MaxValue = 5, Group = "Node Targets")]
        public int PairMaxLineWidth { get; set; }

        [Parameter("Line Style", DefaultValue = "Dashed", Group = "Node Targets")]
        public string TargetLineStyleName { get; set; }

        [Parameter("Min Line Style", DefaultValue = "Solid", Group = "Node Targets")]
        public string MinLineStyleName { get; set; }

        [Parameter("Transparency (0-100)", DefaultValue = 60, MinValue = 0, MaxValue = 100, Group = "Node Targets")]
        public int TargetTransparency { get; set; }

        // ───────────────────────── Day Open / Close ─────────────────────────

        [Parameter("Show Day Open Target", DefaultValue = true, Group = "Day Open/Close")]
        public bool ShowDayOpenTarget { get; set; }

        [Parameter("Show Day Close Target", DefaultValue = true, Group = "Day Open/Close")]
        public bool ShowDayCloseTarget { get; set; }

        // ───────────────────────── Asia Session ─────────────────────────

        [Parameter("Show Asia Session", DefaultValue = true, Group = "Asia Session")]
        public bool ShowAsiaSession { get; set; }

        [Parameter("Asia Name", DefaultValue = "Asia", Group = "Asia Session")]
        public string AsiaSessionName { get; set; }

        [Parameter("Start Hour (UTC)", DefaultValue = 0, MinValue = 0, MaxValue = 23, Group = "Asia Session")]
        public int AsiaStartHour { get; set; }

        [Parameter("Start Minute", DefaultValue = 0, MinValue = 0, MaxValue = 59, Group = "Asia Session")]
        public int AsiaStartMinute { get; set; }

        [Parameter("End Hour (UTC)", DefaultValue = 6, MinValue = 0, MaxValue = 23, Group = "Asia Session")]
        public int AsiaEndHour { get; set; }

        [Parameter("End Minute", DefaultValue = 0, MinValue = 0, MaxValue = 59, Group = "Asia Session")]
        public int AsiaEndMinute { get; set; }

        [Parameter("UTC Offset", DefaultValue = 0, MinValue = -12, MaxValue = 14, Group = "Asia Session")]
        public int AsiaUtcOffset { get; set; }

        [Parameter("Display Days", DefaultValue = 2, MinValue = 1, MaxValue = 10, Group = "Asia Session")]
        public int AsiaDisplayDays { get; set; }

        [Parameter("Show Range Box", DefaultValue = true, Group = "Asia Session")]
        public bool AsiaShowRange { get; set; }

        [Parameter("Show Midline", DefaultValue = true, Group = "Asia Session")]
        public bool AsiaShowMidline { get; set; }

        [Parameter("Midline Extension (Bars)", DefaultValue = 100, MinValue = 0, MaxValue = 500, Group = "Asia Session")]
        public int AsiaMidlineExtension { get; set; }

        [Parameter("Extend Midline To Last Bar", DefaultValue = true, Group = "Asia Session")]
        public bool AsiaExtendMidlineToLast { get; set; }

        [Parameter("Show Asia High Target", DefaultValue = true, Group = "Asia Session")]
        public bool AsiaShowHighTarget { get; set; }

        [Parameter("Show Asia Low Target", DefaultValue = true, Group = "Asia Session")]
        public bool AsiaShowLowTarget { get; set; }

        [Parameter("Show Asia Mid Target", DefaultValue = true, Group = "Asia Session")]
        public bool AsiaShowMidTarget { get; set; }

        // ───────────────────────── Day OC Panel ─────────────────────────

        [Parameter("Show Day OC Panel", DefaultValue = false, Group = "Day Open/Close")]
        public bool ShowDayOcPanel { get; set; }

        // ───────────────────────── Label Anti-Overlap ─────────────────────────

        [Parameter("Label Collision Tolerance (%)", DefaultValue = 0.2, MinValue = 0.001, MaxValue = 5, Group = "Target Label Anti-Overlap")]
        public double LabelCollisionTolerancePct { get; set; }

        [Parameter("Label Stagger Step (bars)", DefaultValue = 24, MinValue = 2, MaxValue = 200, Group = "Target Label Anti-Overlap")]
        public int LabelStaggerStep { get; set; }

        // ───────────────────────── Round Numbers ─────────────────────────

        [Parameter("Show Round Number Targets", DefaultValue = false, Group = "Round Number Targets")]
        public bool EnableRoundTargets { get; set; }

        [Parameter("Round Number Base Price", DefaultValue = 10.0, MinValue = 0.0001, Group = "Round Number Targets")]
        public double RoundBasePrice { get; set; }

        [Parameter("Round Apply Up", DefaultValue = true, Group = "Round Number Targets")]
        public bool RoundApplyUp { get; set; }

        [Parameter("Round Apply Down", DefaultValue = true, Group = "Round Number Targets")]
        public bool RoundApplyDown { get; set; }

        [Parameter("Round Line Width", DefaultValue = 1, MinValue = 1, MaxValue = 5, Group = "Round Number Targets")]
        public int RoundLineWidth { get; set; }

        [Parameter("Round Line Style", DefaultValue = "Dashed", Group = "Round Number Targets")]
        public string RoundLineStyleName { get; set; }

        [Parameter("Round Transparency (0-100)", DefaultValue = 60, MinValue = 0, MaxValue = 100, Group = "Round Number Targets")]
        public int RoundLineTransparency { get; set; }

        [Parameter("Round Line Color", DefaultValue = "#9CA3AF", Group = "Round Number Targets")]
        public string RoundLineColorName { get; set; }

        [Parameter("Round Min Visible Price", DefaultValue = 3000.0, MinValue = 0.0, Group = "Round Number Targets")]
        public double RoundMinVisiblePrice { get; set; }

        // ───────────────────────── Fair Value Gap ─────────────────────────

        [Parameter("Enable Fair Value Gap", DefaultValue = false, Group = "Fair Value Gap")]
        public bool EnableFvg { get; set; }

        [Parameter("FVG Threshold %", DefaultValue = 0.0, MinValue = 0, MaxValue = 100, Group = "Fair Value Gap")]
        public double FvgThresholdPer { get; set; }

        [Parameter("FVG Auto Threshold", DefaultValue = false, Group = "Fair Value Gap")]
        public bool FvgAutoThreshold { get; set; }

        [Parameter("FVG Unmitigated Levels", DefaultValue = 0, MinValue = 0, MaxValue = 50, Group = "Fair Value Gap")]
        public int FvgShowLast { get; set; }

        [Parameter("FVG Mitigation Levels", DefaultValue = false, Group = "Fair Value Gap")]
        public bool FvgMitigationLevels { get; set; }

        [Parameter("FVG Extend (bars)", DefaultValue = 20, MinValue = 0, MaxValue = 500, Group = "Fair Value Gap")]
        public int FvgExtend { get; set; }

        [Parameter("FVG Bull Color", DefaultValue = "#089981", Group = "Fair Value Gap")]
        public string FvgBullColorName { get; set; }

        [Parameter("FVG Bear Color", DefaultValue = "#F23645", Group = "Fair Value Gap")]
        public string FvgBearColorName { get; set; }

        [Parameter("Show FVG Dashboard", DefaultValue = false, Group = "Fair Value Gap")]
        public bool FvgShowDash { get; set; }

        // ───────────────────────── MAP Weekly ─────────────────────────

        [Parameter("Enable MAP Weekly", DefaultValue = true, Group = "MAP Weekly")]
        public bool EnableMapWeekly { get; set; }

        [Parameter("MAP Show Weekly High", DefaultValue = true, Group = "MAP Weekly")]
        public bool MapShowHigh { get; set; }

        [Parameter("MAP Show Weekly Low", DefaultValue = true, Group = "MAP Weekly")]
        public bool MapShowLow { get; set; }

        [Parameter("MAP Show 50% Mid", DefaultValue = true, Group = "MAP Weekly")]
        public bool MapShowMid { get; set; }

        [Parameter("MAP Show 25% Level", DefaultValue = true, Group = "MAP Weekly")]
        public bool MapShow25 { get; set; }

        [Parameter("MAP Show 75% Level", DefaultValue = true, Group = "MAP Weekly")]
        public bool MapShow75 { get; set; }

        [Parameter("MAP Ext Above High", DefaultValue = true, Group = "MAP Weekly")]
        public bool MapShowExtAbove { get; set; }

        [Parameter("MAP Ext Below Low", DefaultValue = true, Group = "MAP Weekly")]
        public bool MapShowExtBelow { get; set; }

        [Parameter("MAP Show 1.25x", DefaultValue = true, Group = "MAP Weekly")]
        public bool MapShow125 { get; set; }

        [Parameter("MAP Show 1.5x", DefaultValue = true, Group = "MAP Weekly")]
        public bool MapShow150 { get; set; }

        [Parameter("MAP Show 1.75x", DefaultValue = true, Group = "MAP Weekly")]
        public bool MapShow175 { get; set; }

        [Parameter("MAP Show 2x", DefaultValue = true, Group = "MAP Weekly")]
        public bool MapShow200 { get; set; }

        [Parameter("MAP Show 1.125x", DefaultValue = false, Group = "MAP Weekly")]
        public bool MapShow1125 { get; set; }

        [Parameter("MAP Show 1.375x", DefaultValue = false, Group = "MAP Weekly")]
        public bool MapShow1375 { get; set; }

        [Parameter("MAP High Color", DefaultValue = "#FF4D6D", Group = "MAP Weekly")]
        public string MapHighColorName { get; set; }

        [Parameter("MAP Low Color", DefaultValue = "#7CFF47", Group = "MAP Weekly")]
        public string MapLowColorName { get; set; }

        [Parameter("MAP Mid Color", DefaultValue = "#FFE566", Group = "MAP Weekly")]
        public string MapMidColorName { get; set; }

        [Parameter("MAP Retrace Color", DefaultValue = "#7AA2FF", Group = "MAP Weekly")]
        public string MapRetraceColorName { get; set; }

        [Parameter("MAP Extension Color", DefaultValue = "#C084FC", Group = "MAP Weekly")]
        public string MapExtColorName { get; set; }

        [Parameter("MAP Transparency (0-100)", DefaultValue = 85, MinValue = 0, MaxValue = 100, Group = "MAP Weekly")]
        public int MapTransparency { get; set; }

        // ───────────────────────── State ─────────────────────────

        private readonly List<Node> _nodesUp = new List<Node>();
        private readonly List<Node> _nodesDown = new List<Node>();
        private readonly List<Color> _colors = new List<Color>();
        private readonly List<double> _stagLabelPrices = new List<double>();
        private readonly List<DateTime> _stagLabelTimes = new List<DateTime>();
        private double _labelVisTop = 1;
        private double _labelVisBot;
        private double _labelVisHeight = 400;

        private int _objSeq;
        private string _lastDrawSignature;
        private int _lastDrawBarIndex = -1;
        private double _lastLiveHigh = double.NaN;
        private double _lastLiveLow = double.NaN;
        private double _lastLiveClose = double.NaN;
        private bool _lastLiveBull;

        private double _priceLowestUp = 999999.0;
        private int _indexLowestUp;
        private double _priceHighestUp;
        private int _indexHighestUp;

        private double _priceLowestDown = 999999.0;
        private int _indexLowestDown;
        private double _priceHighestDown;
        private int _indexHighestDown;

        private int _fvgBullCount;
        private int _fvgBearCount;
        private int _fvgBullMitigated;
        private int _fvgBearMitigated;
        private Bars _dailyBars;
        private Bars _weeklyBars;

        private const string ObjectPrefix = "NCT_";
        private const string StatsName = "NCT_Stats";
        private bool _rebuilding;

        // ───────────────────────── Lifecycle ─────────────────────────

        protected override void Initialize()
        {
            ResetState();
            try { _dailyBars = MarketData.GetBars(TimeFrame.Daily); } catch { }
            try { _weeklyBars = MarketData.GetBars(TimeFrame.Weekly); } catch { }
            // Custom plugin timeframes often never send ticks, so Calculate may not run.
            try { Timer.Start(TimeSpan.FromSeconds(1)); } catch { }
        }

        protected override void OnDestroy()
        {
            try { Timer.Stop(); } catch { }
        }

        protected override void OnTimer()
        {
            try
            {
                if (Bars == null || Bars.Count < 3)
                    return;
                RebuildAndDraw();
            }
            catch { }
        }

        public override void Calculate(int index)
        {
            try
            {
                int lastIndex = Bars.Count - 1;
                if (lastIndex < 2)
                    return;

                // Custom TFs may skip historical Calculate(0..n) and only hit the last bar
                // (or never fire at all — OnTimer covers that). Always rebuild on last bar.
                if (index != lastIndex)
                    return;

                RebuildAndDraw();
            }
            catch
            {
                // Avoid breaking the chart on unexpected data edge cases
            }
        }

        private void RebuildAndDraw()
        {
            if (_rebuilding)
                return;
            _rebuilding = true;

            try
            {
                int lastIndex = Bars.Count - 1;
                if (lastIndex < 2)
                    return;

                double liveHigh = Bars.HighPrices[lastIndex];
                double liveLow = Bars.LowPrices[lastIndex];
                double liveClose = Bars.ClosePrices[lastIndex];
                bool liveBull = liveClose >= Bars.OpenPrices[lastIndex];
                // Skip only when this bar has not moved in a way that can change nodes or hits.
                if (_lastDrawBarIndex == lastIndex
                    && _lastDrawSignature != null
                    && _lastLiveHigh == liveHigh
                    && _lastLiveLow == liveLow
                    && _lastLiveBull == liveBull)
                    return;

                string savedSig = _lastDrawSignature;
                int savedBar = _lastDrawBarIndex;

                ResetState();
                InitColors();

                int lookback = Math.Min(Math.Max(StartPoint, 1), lastIndex);
                int min = Math.Max(1, lastIndex - lookback);
                int max = lastIndex - Math.Max(EndPoint, 0);
                if (max > lastIndex)
                    max = lastIndex;
                if (max <= min)
                    return;

                for (int i = min + 1; i <= max; i++)
                {
                    CalcNodeUpTrend(i);
                    CalcNodeDownTrend(i);
                }

                string signature = BuildDrawSignature(lastIndex);
                if (signature == savedSig && savedBar == lastIndex)
                {
                    _lastDrawSignature = savedSig;
                    _lastDrawBarIndex = savedBar;
                    _lastLiveHigh = liveHigh;
                    _lastLiveLow = liveLow;
                    _lastLiveClose = liveClose;
                    _lastLiveBull = liveBull;
                    return;
                }

                RemoveDrawings();
                _stagLabelPrices.Clear();
                _stagLabelTimes.Clear();
                _objSeq = 0;
                CaptureLabelPriceRange();

                if (_nodesUp.Count > 0)
                    DrawingNumberNodes(true);
                if (_nodesDown.Count > 0)
                    DrawingNumberNodes(false);

                if (ShowNodeConnectingLines)
                    DrawingLineNodes();

                if (EnableSingleNode1Targets || EnablePairNode12Targets)
                {
                    if (ShowTargetUp)
                    {
                        if (EnableSingleNode1Targets)
                            DrawingTargetsNode1(true);
                        if (EnablePairNode12Targets)
                            DrawingPairTargetsNode12(true);
                    }

                    if (ShowTargetDown)
                    {
                        if (EnableSingleNode1Targets)
                            DrawingTargetsNode1(false);
                        if (EnablePairNode12Targets)
                            DrawingPairTargetsNode12(false);
                    }
                }

                if (ShowDayOpenTarget || ShowDayCloseTarget || ShowDayOcPanel)
                    DrawingDayOpenClose();

                if (ShowAsiaSession)
                    DrawingAsiaSession();

                if (EnableRoundTargets)
                {
                    if (RoundApplyUp)
                        DrawingRoundNumberTargets(true);
                    if (RoundApplyDown)
                        DrawingRoundNumberTargets(false);
                }

                if (EnableFvg)
                    DrawingFairValueGaps();

                if (EnableMapWeekly)
                    DrawingMapWeekly();

                DrawStatsOverlay();

                _lastDrawSignature = signature;
                _lastDrawBarIndex = lastIndex;
                _lastLiveHigh = liveHigh;
                _lastLiveLow = liveLow;
                _lastLiveClose = liveClose;
                _lastLiveBull = liveBull;
            }
            finally
            {
                _rebuilding = false;
            }
        }

        // ───────────────────────── Init / Reset ─────────────────────────

        private void ResetState()
        {
            _nodesUp.Clear();
            _nodesDown.Clear();
            _colors.Clear();
            _stagLabelPrices.Clear();
            _stagLabelTimes.Clear();
            _objSeq = 0;
            _fvgBullCount = 0;
            _fvgBearCount = 0;
            _fvgBullMitigated = 0;
            _fvgBearMitigated = 0;
            // Keep draw-cache (_lastDrawSignature / bar index / live OHLC) so a mid-rebuild
            // return does not force a full redraw on the next tick.

            _priceLowestUp = 999999.0;
            _indexLowestUp = 0;
            _priceHighestUp = 0.0;
            _indexHighestUp = 0;

            _priceLowestDown = 999999.0;
            _indexLowestDown = 0;
            _priceHighestDown = 0.0;
            _indexHighestDown = 0;

            // Do not wipe chart objects here — avoids blank/flicker during full recalc.
            // Drawings are replaced atomically at the end of Calculate on the last bar.
        }

        private void InitColors()
        {
            _colors.Clear();
            _colors.Add(ParseColor(Color1Name, Color.FromArgb(255, 255, 229, 102))); // Gold
            _colors.Add(ParseColor(Color2Name, Color.FromArgb(255, 122, 162, 255))); // Bright Blue
            _colors.Add(ParseColor(Color3Name, Color.FromArgb(255, 255, 159, 28)));  // Orange
            _colors.Add(ParseColor(Color4Name, Color.FromArgb(255, 45, 226, 230)));  // Cyan
            _colors.Add(ParseColor(Color5Name, Color.FromArgb(255, 255, 90, 217)));  // Magenta
            _colors.Add(ParseColor(Color6Name, Color.FromArgb(255, 124, 255, 71)));  // Lime
            _colors.Add(ParseColor(Color7Name, Color.FromArgb(255, 255, 77, 109)));  // Rose
            _colors.Add(ParseColor(Color8Name, Color.FromArgb(255, 192, 132, 252))); // Violet
        }

        private static Color ParseColor(string name, Color fallback)
        {
            if (string.IsNullOrWhiteSpace(name))
                return fallback;

            string s = name.Trim();
            Color hex;
            if (TryParseHexColor(s, out hex))
                return hex;

            switch (s.ToLowerInvariant())
            {
                case "white": return Color.White;
                case "yellow":
                case "gold": return Color.FromArgb(255, 255, 229, 102);
                case "aqua":
                case "cyan": return Color.FromArgb(255, 45, 226, 230);
                case "lime":
                case "green": return Color.FromArgb(255, 124, 255, 71);
                case "orange":
                case "darkorange": return Color.FromArgb(255, 255, 159, 28);
                case "fuchsia":
                case "magenta":
                case "pink":
                case "hotpink": return Color.FromArgb(255, 255, 90, 217);
                case "blue":
                case "darkblue":
                case "navy":
                case "deepskyblue":
                case "dodgerblue": return Color.FromArgb(255, 122, 162, 255);
                case "red":
                case "rose":
                case "tomato": return Color.FromArgb(255, 255, 77, 109);
                case "gray":
                case "grey": return Color.Gray;
                case "black": return Color.Black;
                case "dimgray":
                case "dimgrey": return Color.DimGray;
                case "lightgray":
                case "lightgrey": return Color.LightGray;
                case "darkred": return Color.DarkRed;
                case "darkgreen": return Color.DarkGreen;
                case "khaki": return Color.Khaki;
                case "violet":
                case "lavender":
                case "purple": return Color.FromArgb(255, 192, 132, 252);
                case "brown": return Color.Brown;
                case "coral": return Color.Coral;
                case "teal": return Color.Teal;
                case "maroon": return Color.Maroon;
                case "olive": return Color.Olive;
                case "silver": return Color.Silver;
                default: return fallback;
            }
        }

        private static bool TryParseHexColor(string s, out Color color)
        {
            color = Color.Black;
            if (s.Length < 7 || s[0] != '#')
                return false;

            string h = s.Substring(1);
            try
            {
                if (h.Length == 6)
                {
                    color = Color.FromArgb(255,
                        Convert.ToInt32(h.Substring(0, 2), 16),
                        Convert.ToInt32(h.Substring(2, 2), 16),
                        Convert.ToInt32(h.Substring(4, 2), 16));
                    return true;
                }

                if (h.Length == 8)
                {
                    color = Color.FromArgb(
                        Convert.ToInt32(h.Substring(0, 2), 16),
                        Convert.ToInt32(h.Substring(2, 2), 16),
                        Convert.ToInt32(h.Substring(4, 2), 16),
                        Convert.ToInt32(h.Substring(6, 2), 16));
                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        private Color WithTransparency(Color baseColor)
        {
            int t = TargetTransparency;
            if (t < 0) t = 0;
            if (t > 100) t = 100;
            int alpha = (100 - t) * 255 / 100;
            return Color.FromArgb(alpha, baseColor.R, baseColor.G, baseColor.B);
        }

        private static LineStyle ParseLineStyle(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return LineStyle.Dots;

            switch (name.Trim().ToLowerInvariant())
            {
                case "solid": return LineStyle.Solid;
                case "dashed":
                case "dots": return LineStyle.Dots;
                case "dotted":
                case "dotsrare":
                case "dots rare": return LineStyle.DotsRare;
                default: return LineStyle.Dots;
            }
        }

        // ───────────────────────── Node ─────────────────────────

        private sealed class Node
        {
            public int IndexPreNode;
            public int IndexNode;
            public int IndexCorrection;
            public int NumberNode;
            public double LowPreNode;
            public double HighNode;
            public double LowCorrection;
            public double AmountCorrection;
            public double LogAmountCorrection;
            public bool IsSymmetrySetup;
        }

        private static double SafeLog(double value)
        {
            return value > 0 ? Math.Log(value) : 0.0;
        }

        private double AbsMove(double from, double to)
        {
            if (SwCalcLogarithm)
                return Math.Abs(SafeLog(to) - SafeLog(from));
            return Math.Abs(to - from);
        }

        private double Project(double origin, double move, bool isUp)
        {
            if (SwCalcLogarithm)
                return isUp
                    ? Math.Exp(SafeLog(origin) + move)
                    : Math.Exp(SafeLog(origin) - move);
            return isUp ? origin + move : origin - move;
        }

        // Pine: node calc may be linear, but plotted targets are always logarithmic.
        private double TargetAbsMove(double from, double to)
        {
            return Math.Abs(SafeLog(to) - SafeLog(from));
        }

        private double TargetProject(double origin, double move, bool isUp)
        {
            return isUp
                ? Math.Exp(SafeLog(origin) + move)
                : Math.Exp(SafeLog(origin) - move);
        }

        private double AlongPath(double start, double end, double ratio)
        {
            return Math.Exp(SafeLog(start) + ratio * (SafeLog(end) - SafeLog(start)));
        }

        // ───────────────────────── Symmetry ─────────────────────────

        private bool CalcSymmetryNodesDown(int i)
        {
            if (i < 3 || i >= _nodesDown.Count)
                return false;

            var node = _nodesDown[i];
            var prePreNode = _nodesDown[i - 2];
            var prePrePreNode = _nodesDown[i - 3];

            if (prePreNode.NumberNode != 2)
                return false;
            if (node.AmountCorrection < prePreNode.AmountCorrection)
                return false;

            double amountSymmetry = prePrePreNode.LowPreNode - prePreNode.HighNode;
            double minPrice = prePreNode.LowCorrection - amountSymmetry;
            return node.HighNode <= minPrice;
        }

        private bool CalcSymmetryNodesDownModeLog(int i)
        {
            if (i < 3 || i >= _nodesDown.Count)
                return false;

            var node = _nodesDown[i];
            var prePreNode = _nodesDown[i - 2];
            var prePrePreNode = _nodesDown[i - 3];

            if (prePreNode.NumberNode != 2)
                return false;
            if (node.LogAmountCorrection < prePreNode.LogAmountCorrection)
                return false;

            double amountSymmetry = SafeLog(prePrePreNode.LowPreNode) - SafeLog(prePreNode.HighNode);
            double minPrice = SafeLog(prePreNode.LowCorrection) - amountSymmetry;
            return SafeLog(node.HighNode) <= minPrice;
        }

        private bool CalcSymmetryNodesUp(int i)
        {
            if (i < 3 || i >= _nodesUp.Count)
                return false;

            var node = _nodesUp[i];
            var prePreNode = _nodesUp[i - 2];
            var prePrePreNode = _nodesUp[i - 3];

            if (prePreNode.NumberNode != 2)
                return false;
            if (node.AmountCorrection < prePreNode.AmountCorrection)
                return false;

            double amountSymmetry = prePreNode.HighNode - prePrePreNode.LowPreNode;
            double maxPrice = prePreNode.LowCorrection + amountSymmetry;
            return node.HighNode >= maxPrice;
        }

        private bool CalcSymmetryNodesUpModeLog(int i)
        {
            if (i < 3 || i >= _nodesUp.Count)
                return false;

            var node = _nodesUp[i];
            var prePreNode = _nodesUp[i - 2];
            var prePrePreNode = _nodesUp[i - 3];

            if (prePreNode.NumberNode != 2)
                return false;
            if (node.LogAmountCorrection < prePreNode.LogAmountCorrection)
                return false;

            double amountSymmetry = SafeLog(prePreNode.HighNode) - SafeLog(prePrePreNode.LowPreNode);
            double maxPrice = SafeLog(prePreNode.LowCorrection) + amountSymmetry;
            return SafeLog(node.HighNode) >= maxPrice;
        }

        // ───────────────────────── Sort Down ─────────────────────────

        private void SortNodesDownTrend()
        {
            int i = _nodesDown.Count - 1;
            if (_nodesDown.Count == 0)
                return;

            while (true)
            {
                var node = _nodesDown[i];

                if (_nodesDown.Count <= 1)
                {
                    if (node.LowCorrection >= node.LowPreNode)
                        _nodesDown.RemoveAt(0);
                    break;
                }

                var preNode = _nodesDown[i - 1];

                if (node.LowCorrection < preNode.LowCorrection)
                {
                    if (node.AmountCorrection < preNode.AmountCorrection)
                    {
                        node.NumberNode = 1;
                        break;
                    }

                    if (node.AmountCorrection >= preNode.AmountCorrection)
                    {
                        if (preNode.NumberNode == 1)
                        {
                            if (SwCalcSymmetry && CalcSymmetryNodesDown(i))
                            {
                                var prePrePreNode = _nodesDown[i - 3];
                                node.IndexPreNode = prePrePreNode.IndexPreNode;
                                node.LowPreNode = prePrePreNode.LowPreNode;
                                node.NumberNode = 1;
                                node.IsSymmetrySetup = true;
                                _nodesDown.RemoveAt(i - 1);
                                _nodesDown.RemoveAt(i - 2);
                                _nodesDown.RemoveAt(i - 3);
                                i -= 3;
                                continue;
                            }

                            node.NumberNode = 2;
                            break;
                        }

                        if (preNode.NumberNode == 2)
                        {
                            node.IndexPreNode = _nodesDown[i - 2].IndexPreNode;
                            node.LowPreNode = _nodesDown[i - 2].LowPreNode;
                            node.IsSymmetrySetup = _nodesDown[i - 2].IsSymmetrySetup || node.IsSymmetrySetup;
                            node.NumberNode = 1;
                            _nodesDown.RemoveAt(i - 1);
                            _nodesDown.RemoveAt(i - 2);
                            i -= 2;
                            continue;
                        }
                    }
                }
                else if (node.LowCorrection >= preNode.LowCorrection)
                {
                    if (node.HighNode > preNode.HighNode)
                    {
                        node.IndexPreNode = preNode.IndexPreNode;
                        node.IndexNode = preNode.IndexNode;
                        node.HighNode = preNode.HighNode;
                        node.LowPreNode = preNode.LowPreNode;
                        node.AmountCorrection = node.LowCorrection - node.HighNode;
                        node.NumberNode = preNode.NumberNode;
                        node.IsSymmetrySetup = preNode.IsSymmetrySetup || node.IsSymmetrySetup;
                        _nodesDown.RemoveAt(i - 1);
                        i -= 1;
                        continue;
                    }

                    if (node.HighNode <= preNode.HighNode)
                    {
                        if (preNode.NumberNode == 1)
                        {
                            node.NumberNode = 1;
                            node.IndexPreNode = preNode.IndexPreNode;
                            node.LowPreNode = preNode.LowPreNode;
                            node.IsSymmetrySetup = preNode.IsSymmetrySetup || node.IsSymmetrySetup;
                            _nodesDown.RemoveAt(i - 1);
                            i -= 1;
                            continue;
                        }

                        if (preNode.NumberNode == 2)
                        {
                            node.IndexPreNode = _nodesDown[i - 2].IndexPreNode;
                            node.LowPreNode = _nodesDown[i - 2].LowPreNode;
                            node.IsSymmetrySetup = _nodesDown[i - 2].IsSymmetrySetup || node.IsSymmetrySetup;
                            node.NumberNode = 1;
                            _nodesDown.RemoveAt(i - 1);
                            _nodesDown.RemoveAt(i - 2);
                            i -= 2;
                            continue;
                        }
                    }
                }

                break;
            }
        }

        private void SortNodesDownTrendModeLog()
        {
            int i = _nodesDown.Count - 1;
            if (_nodesDown.Count == 0)
                return;

            while (true)
            {
                var node = _nodesDown[i];

                if (_nodesDown.Count <= 1)
                {
                    if (node.LowCorrection >= node.LowPreNode)
                        _nodesDown.RemoveAt(0);
                    break;
                }

                var preNode = _nodesDown[i - 1];

                if (node.LowCorrection < preNode.LowCorrection)
                {
                    if (node.LogAmountCorrection < preNode.LogAmountCorrection)
                    {
                        node.NumberNode = 1;
                        break;
                    }

                    if (node.LogAmountCorrection >= preNode.LogAmountCorrection)
                    {
                        if (preNode.NumberNode == 1)
                        {
                            if (SwCalcSymmetry && CalcSymmetryNodesDownModeLog(i))
                            {
                                var prePrePreNode = _nodesDown[i - 3];
                                node.IndexPreNode = prePrePreNode.IndexPreNode;
                                node.LowPreNode = prePrePreNode.LowPreNode;
                                node.NumberNode = 1;
                                node.IsSymmetrySetup = true;
                                _nodesDown.RemoveAt(i - 1);
                                _nodesDown.RemoveAt(i - 2);
                                _nodesDown.RemoveAt(i - 3);
                                i -= 3;
                                continue;
                            }

                            node.NumberNode = 2;
                            break;
                        }

                        if (preNode.NumberNode == 2)
                        {
                            node.IndexPreNode = _nodesDown[i - 2].IndexPreNode;
                            node.LowPreNode = _nodesDown[i - 2].LowPreNode;
                            node.IsSymmetrySetup = _nodesDown[i - 2].IsSymmetrySetup || node.IsSymmetrySetup;
                            node.NumberNode = 1;
                            _nodesDown.RemoveAt(i - 1);
                            _nodesDown.RemoveAt(i - 2);
                            i -= 2;
                            continue;
                        }
                    }
                }
                else if (node.LowCorrection >= preNode.LowCorrection)
                {
                    if (node.HighNode > preNode.HighNode)
                    {
                        node.IndexPreNode = preNode.IndexPreNode;
                        node.IndexNode = preNode.IndexNode;
                        node.HighNode = preNode.HighNode;
                        node.LowPreNode = preNode.LowPreNode;
                        node.AmountCorrection = node.LowCorrection - node.HighNode;
                        node.LogAmountCorrection = SafeLog(node.LowCorrection) - SafeLog(node.HighNode);
                        node.NumberNode = preNode.NumberNode;
                        node.IsSymmetrySetup = preNode.IsSymmetrySetup || node.IsSymmetrySetup;
                        _nodesDown.RemoveAt(i - 1);
                        i -= 1;
                        continue;
                    }

                    if (node.HighNode <= preNode.HighNode)
                    {
                        if (preNode.NumberNode == 1)
                        {
                            node.NumberNode = 1;
                            node.IndexPreNode = preNode.IndexPreNode;
                            node.LowPreNode = preNode.LowPreNode;
                            node.IsSymmetrySetup = preNode.IsSymmetrySetup || node.IsSymmetrySetup;
                            _nodesDown.RemoveAt(i - 1);
                            i -= 1;
                            continue;
                        }

                        if (preNode.NumberNode == 2)
                        {
                            node.IndexPreNode = _nodesDown[i - 2].IndexPreNode;
                            node.LowPreNode = _nodesDown[i - 2].LowPreNode;
                            node.IsSymmetrySetup = _nodesDown[i - 2].IsSymmetrySetup || node.IsSymmetrySetup;
                            node.NumberNode = 1;
                            _nodesDown.RemoveAt(i - 1);
                            _nodesDown.RemoveAt(i - 2);
                            i -= 2;
                            continue;
                        }
                    }
                }

                break;
            }
        }

        // ───────────────────────── Sort Up ─────────────────────────

        private void SortNodesUpTrend()
        {
            int i = _nodesUp.Count - 1;
            if (_nodesUp.Count == 0)
                return;

            while (true)
            {
                var node = _nodesUp[i];

                if (_nodesUp.Count <= 1)
                {
                    if (node.LowCorrection <= node.LowPreNode)
                        _nodesUp.RemoveAt(0);
                    break;
                }

                var preNode = _nodesUp[i - 1];

                if (node.LowCorrection > preNode.LowCorrection)
                {
                    if (node.AmountCorrection < preNode.AmountCorrection)
                    {
                        node.NumberNode = 1;
                        break;
                    }

                    if (node.AmountCorrection >= preNode.AmountCorrection)
                    {
                        if (preNode.NumberNode == 1)
                        {
                            if (SwCalcSymmetry && CalcSymmetryNodesUp(i))
                            {
                                var prePrePreNode = _nodesUp[i - 3];
                                node.IndexPreNode = prePrePreNode.IndexPreNode;
                                node.LowPreNode = prePrePreNode.LowPreNode;
                                node.NumberNode = 1;
                                node.IsSymmetrySetup = true;
                                _nodesUp.RemoveAt(i - 1);
                                _nodesUp.RemoveAt(i - 2);
                                _nodesUp.RemoveAt(i - 3);
                                i -= 3;
                                continue;
                            }

                            node.NumberNode = 2;
                            break;
                        }

                        if (preNode.NumberNode == 2)
                        {
                            node.IndexPreNode = _nodesUp[i - 2].IndexPreNode;
                            node.LowPreNode = _nodesUp[i - 2].LowPreNode;
                            node.IsSymmetrySetup = _nodesUp[i - 2].IsSymmetrySetup || node.IsSymmetrySetup;
                            node.NumberNode = 1;
                            _nodesUp.RemoveAt(i - 1);
                            _nodesUp.RemoveAt(i - 2);
                            i -= 2;
                            continue;
                        }
                    }
                }
                else if (node.LowCorrection <= preNode.LowCorrection)
                {
                    if (node.HighNode < preNode.HighNode)
                    {
                        node.IndexPreNode = preNode.IndexPreNode;
                        node.IndexNode = preNode.IndexNode;
                        node.HighNode = preNode.HighNode;
                        node.LowPreNode = preNode.LowPreNode;
                        node.AmountCorrection = node.HighNode - node.LowCorrection;
                        node.NumberNode = preNode.NumberNode;
                        node.IsSymmetrySetup = preNode.IsSymmetrySetup || node.IsSymmetrySetup;
                        _nodesUp.RemoveAt(i - 1);
                        i -= 1;
                        continue;
                    }

                    if (node.HighNode >= preNode.HighNode)
                    {
                        if (preNode.NumberNode == 1)
                        {
                            node.NumberNode = 1;
                            node.IndexPreNode = preNode.IndexPreNode;
                            node.LowPreNode = preNode.LowPreNode;
                            node.IsSymmetrySetup = preNode.IsSymmetrySetup || node.IsSymmetrySetup;
                            _nodesUp.RemoveAt(i - 1);
                            i -= 1;
                            continue;
                        }

                        if (preNode.NumberNode == 2)
                        {
                            node.IndexPreNode = _nodesUp[i - 2].IndexPreNode;
                            node.LowPreNode = _nodesUp[i - 2].LowPreNode;
                            node.IsSymmetrySetup = _nodesUp[i - 2].IsSymmetrySetup || node.IsSymmetrySetup;
                            node.NumberNode = 1;
                            _nodesUp.RemoveAt(i - 1);
                            _nodesUp.RemoveAt(i - 2);
                            i -= 2;
                            continue;
                        }
                    }
                }

                break;
            }
        }

        private void SortNodesUpTrendModeLog()
        {
            int i = _nodesUp.Count - 1;
            if (_nodesUp.Count == 0)
                return;

            while (true)
            {
                var node = _nodesUp[i];

                if (_nodesUp.Count <= 1)
                {
                    if (node.LowCorrection <= node.LowPreNode)
                        _nodesUp.RemoveAt(0);
                    break;
                }

                var preNode = _nodesUp[i - 1];

                if (node.LowCorrection > preNode.LowCorrection)
                {
                    if (node.LogAmountCorrection < preNode.LogAmountCorrection)
                    {
                        node.NumberNode = 1;
                        break;
                    }

                    if (node.LogAmountCorrection >= preNode.LogAmountCorrection)
                    {
                        if (preNode.NumberNode == 1)
                        {
                            if (SwCalcSymmetry && CalcSymmetryNodesUpModeLog(i))
                            {
                                var prePrePreNode = _nodesUp[i - 3];
                                node.IndexPreNode = prePrePreNode.IndexPreNode;
                                node.LowPreNode = prePrePreNode.LowPreNode;
                                node.NumberNode = 1;
                                node.IsSymmetrySetup = true;
                                _nodesUp.RemoveAt(i - 1);
                                _nodesUp.RemoveAt(i - 2);
                                _nodesUp.RemoveAt(i - 3);
                                i -= 3;
                                continue;
                            }

                            node.NumberNode = 2;
                            break;
                        }

                        if (preNode.NumberNode == 2)
                        {
                            node.IndexPreNode = _nodesUp[i - 2].IndexPreNode;
                            node.LowPreNode = _nodesUp[i - 2].LowPreNode;
                            node.IsSymmetrySetup = _nodesUp[i - 2].IsSymmetrySetup || node.IsSymmetrySetup;
                            node.NumberNode = 1;
                            _nodesUp.RemoveAt(i - 1);
                            _nodesUp.RemoveAt(i - 2);
                            i -= 2;
                            continue;
                        }
                    }
                }
                else if (node.LowCorrection <= preNode.LowCorrection)
                {
                    if (node.HighNode < preNode.HighNode)
                    {
                        node.IndexPreNode = preNode.IndexPreNode;
                        node.IndexNode = preNode.IndexNode;
                        node.HighNode = preNode.HighNode;
                        node.LowPreNode = preNode.LowPreNode;
                        node.AmountCorrection = node.HighNode - node.LowCorrection;
                        node.LogAmountCorrection = SafeLog(node.HighNode) - SafeLog(node.LowCorrection);
                        node.NumberNode = preNode.NumberNode;
                        node.IsSymmetrySetup = preNode.IsSymmetrySetup || node.IsSymmetrySetup;
                        _nodesUp.RemoveAt(i - 1);
                        i -= 1;
                        continue;
                    }

                    if (node.HighNode >= preNode.HighNode)
                    {
                        if (preNode.NumberNode == 1)
                        {
                            node.NumberNode = 1;
                            node.IndexPreNode = preNode.IndexPreNode;
                            node.LowPreNode = preNode.LowPreNode;
                            node.IsSymmetrySetup = preNode.IsSymmetrySetup || node.IsSymmetrySetup;
                            _nodesUp.RemoveAt(i - 1);
                            i -= 1;
                            continue;
                        }

                        if (preNode.NumberNode == 2)
                        {
                            node.IndexPreNode = _nodesUp[i - 2].IndexPreNode;
                            node.LowPreNode = _nodesUp[i - 2].LowPreNode;
                            node.IsSymmetrySetup = _nodesUp[i - 2].IsSymmetrySetup || node.IsSymmetrySetup;
                            node.NumberNode = 1;
                            _nodesUp.RemoveAt(i - 1);
                            _nodesUp.RemoveAt(i - 2);
                            i -= 2;
                            continue;
                        }
                    }
                }

                break;
            }
        }

        private void SortUp()
        {
            if (SwCalcLogarithm)
                SortNodesUpTrendModeLog();
            else
                SortNodesUpTrend();
        }

        private void SortDown()
        {
            if (SwCalcLogarithm)
                SortNodesDownTrendModeLog();
            else
                SortNodesDownTrend();
        }

        // ───────────────────────── Set / Calc Nodes ─────────────────────────

        private void SetNodeUpTrend(int index)
        {
            var node = new Node
            {
                NumberNode = 1,
                IndexPreNode = _indexLowestUp,
                IndexNode = _indexHighestUp,
                IndexCorrection = index,
                LowPreNode = _priceLowestUp,
                HighNode = _priceHighestUp,
                LowCorrection = Bars.LowPrices[index],
                AmountCorrection = _priceHighestUp - Bars.LowPrices[index],
                LogAmountCorrection = SafeLog(_priceHighestUp) - SafeLog(Bars.LowPrices[index]),
                IsSymmetrySetup = false
            };

            _nodesUp.Add(node);
            SortUp();

            _priceHighestUp = Bars.LowPrices[index];
            _indexHighestUp = index;
        }

        private void SetNodeDownTrend(int index)
        {
            var node = new Node
            {
                NumberNode = 1,
                IndexPreNode = _indexHighestDown,
                IndexNode = _indexLowestDown,
                IndexCorrection = index,
                LowPreNode = _priceHighestDown,
                HighNode = _priceLowestDown,
                LowCorrection = Bars.HighPrices[index],
                AmountCorrection = Bars.HighPrices[index] - _priceLowestDown,
                LogAmountCorrection = SafeLog(Bars.HighPrices[index]) - SafeLog(_priceLowestDown),
                IsSymmetrySetup = false
            };

            _nodesDown.Add(node);
            SortDown();

            _priceLowestDown = Bars.HighPrices[index];
            _indexLowestDown = index;
        }

        private void CalcNodeUpTrend(int index)
        {
            double low0 = Bars.LowPrices[index];
            double high0 = Bars.HighPrices[index];
            double open0 = Bars.OpenPrices[index];
            double close0 = Bars.ClosePrices[index];
            double open1 = Bars.OpenPrices[index - 1];
            double close1 = Bars.ClosePrices[index - 1];
            double low1 = Bars.LowPrices[index - 1];

            if (low0 < _priceLowestUp)
            {
                _priceLowestUp = low0;
                _indexLowestUp = index;
            }

            if (high0 > _priceHighestUp)
            {
                _priceHighestUp = high0;
                _indexHighestUp = index;
            }

            int sizeNodes = _nodesUp.Count;
            if (sizeNodes > 0)
            {
                var node = _nodesUp[sizeNodes - 1];

                if (low0 <= node.LowCorrection)
                {
                    node.LowCorrection = low0;
                    node.IndexCorrection = index;
                    node.AmountCorrection = node.HighNode - node.LowCorrection;
                    node.LogAmountCorrection = SafeLog(node.HighNode) - SafeLog(node.LowCorrection);
                    SortUp();
                }
                else if (open0 > close0 && open1 > close1)
                {
                    if (high0 >= node.HighNode)
                    {
                        node.HighNode = high0;
                        node.IndexNode = index;
                    }

                    if (low0 < low1)
                    {
                        node.LowCorrection = low0;
                        node.IndexCorrection = index;
                    }

                    node.AmountCorrection = node.HighNode - node.LowCorrection;
                    node.LogAmountCorrection = SafeLog(node.HighNode) - SafeLog(node.LowCorrection);
                    SortUp();

                    if (_nodesUp.Count > 0)
                    {
                        _indexLowestUp = _nodesUp[_nodesUp.Count - 1].IndexCorrection;
                        _indexLowestUp = ClampIndex(_indexLowestUp);
                        _priceLowestUp = Bars.LowPrices[_indexLowestUp];
                    }
                }
            }

            if (open0 > close0)
            {
                if (_nodesUp.Count > 0)
                {
                    _indexLowestUp = ClampIndex(_nodesUp[_nodesUp.Count - 1].IndexCorrection);
                    _priceLowestUp = Bars.LowPrices[_indexLowestUp];
                }

                SetNodeUpTrend(index);
            }
        }

        private void CalcNodeDownTrend(int index)
        {
            double low0 = Bars.LowPrices[index];
            double high0 = Bars.HighPrices[index];
            double open0 = Bars.OpenPrices[index];
            double close0 = Bars.ClosePrices[index];
            double open1 = Bars.OpenPrices[index - 1];
            double close1 = Bars.ClosePrices[index - 1];

            if (low0 < _priceLowestDown)
            {
                _priceLowestDown = low0;
                _indexLowestDown = index;
            }

            if (high0 > _priceHighestDown)
            {
                _priceHighestDown = high0;
                _indexHighestDown = index;
            }

            int sizeNodes = _nodesDown.Count;
            if (sizeNodes > 0)
            {
                var node = _nodesDown[sizeNodes - 1];

                if (high0 >= node.LowCorrection)
                {
                    node.LowCorrection = high0;
                    node.IndexCorrection = index;
                    node.AmountCorrection = node.LowCorrection - node.HighNode;
                    node.LogAmountCorrection = SafeLog(node.LowCorrection) - SafeLog(node.HighNode);
                    SortDown();
                }
                else if (open0 < close0 && open1 < close1)
                {
                    if (low0 <= node.HighNode)
                    {
                        node.HighNode = low0;
                        node.IndexNode = index;
                    }

                    node.LowCorrection = high0;
                    node.IndexCorrection = index;
                    node.AmountCorrection = node.LowCorrection - node.HighNode;
                    node.LogAmountCorrection = SafeLog(node.LowCorrection) - SafeLog(node.HighNode);
                    SortDown();
                }
            }

            if (open0 < close0)
            {
                if (_nodesDown.Count > 0)
                {
                    _indexHighestDown = ClampIndex(_nodesDown[_nodesDown.Count - 1].IndexCorrection);
                    _priceHighestDown = Bars.HighPrices[_indexHighestDown];
                }

                SetNodeDownTrend(index);
            }
        }

        // ───────────────────────── Helpers ─────────────────────────

        private int ClampIndex(int index)
        {
            if (Bars.Count <= 0)
                return 0;
            if (index < 0)
                return 0;
            if (index >= Bars.Count)
                return Bars.Count - 1;
            return index;
        }

        private DateTime TimeAtIndex(int index)
        {
            if (Bars.Count <= 0)
                return Server.Time;

            if (index < Bars.Count)
                return Bars.OpenTimes[ClampIndex(index)];

            TimeSpan barDuration = TimeSpan.FromMinutes(1);
            if (Bars.Count >= 2)
                barDuration = Bars.OpenTimes[Bars.Count - 1] - Bars.OpenTimes[Bars.Count - 2];
            if (barDuration <= TimeSpan.Zero)
                barDuration = TimeSpan.FromMinutes(1);

            int extra = index - (Bars.Count - 1);
            return Bars.OpenTimes[Bars.Count - 1] + TimeSpan.FromTicks(barDuration.Ticks * extra);
        }

        private string NextName(string kind)
        {
            _objSeq++;
            return ObjectPrefix + kind + "_" + _objSeq;
        }

        private int FontSizeValue()
        {
            return ParseFontSize(TextNodeSize, 12);
        }

        private int TargetFontSizeValue()
        {
            return Math.Max(1, Math.Min(TargetLabelFontSize, 72));
        }

        private static int ParseFontSize(string sizeName, int fallback)
        {
            if (string.IsNullOrWhiteSpace(sizeName))
                return fallback;

            switch (sizeName.Trim())
            {
                case "Huge": return 18;
                case "Large": return 14;
                case "Normal": return 12;
                case "Small": return 10;
                case "Tiny": return 8;
                case "Auto":
                default: return fallback;
            }
        }

        private string BuildDrawSignature(int lastIndex)
        {
            // Compact fingerprint of drawable state — skip redraw when unchanged on same bar.
            var sb = new System.Text.StringBuilder(256);
            sb.Append(lastIndex).Append('|')
              .Append(Bars.HighPrices[lastIndex].ToString("R")).Append('|')
              .Append(Bars.LowPrices[lastIndex].ToString("R")).Append('|')
              .Append(_nodesUp.Count).Append('|')
              .Append(_nodesDown.Count).Append('|')
              .Append(ShowTargetUp).Append('|')
              .Append(ShowTargetDown).Append('|')
              .Append(ShowDouble).Append('|')
              .Append(ShowDouble15).Append('|')
              .Append(Double15Ratio.ToString("R")).Append('|')
              .Append(ShowDouble086).Append('|')
              .Append(Double086Ratio.ToString("R")).Append('|')
              .Append(ShowMin).Append('|')
              .Append(ShowMin085).Append('|')
              .Append(Min085Ratio.ToString("R")).Append('|')
              .Append(ShowMin13Based).Append('|')
              .Append(BasedRetraceRatio.ToString("R")).Append('|')
              .Append(BasedMin13Ratio.ToString("R")).Append('|')
              .Append(EnableSingleNode1Targets).Append('|')
              .Append(EnablePairNode12Targets).Append('|')
              .Append(TargetMaxCount).Append('|')
              .Append(PairMaxCount).Append('|')
              .Append(TargetLabelFontSize).Append('|')
              .Append(ShowIncomplete2Circle).Append('|')
              .Append(Incomplete2RadiusPct.ToString("R")).Append('|')
              .Append(Incomplete2RadiusBars).Append('|')
              .Append(ShowProxCircle).Append('|')
              .Append(MinDblProximityTolPct.ToString("R")).Append('|')
              .Append(TextNodeSize).Append('|')
              .Append(TargetGapBars).Append('|')
              .Append(DeleteHitTargets).Append('|')
              .Append(HitGraceBars).Append('|')
              .Append(ShowDayOpenTarget).Append('|')
              .Append(ShowDayCloseTarget).Append('|')
              .Append(ShowDayOcPanel).Append('|')
              .Append(EnableRoundTargets).Append('|')
              .Append(RoundBasePrice.ToString("R")).Append('|')
              .Append(EnableFvg).Append('|')
              .Append(EnableMapWeekly).Append('|')
              .Append(MapTransparency).Append('|')
              .Append(MapShow150).Append('|')
              .Append(FvgExtend).Append('|')
              .Append(FvgThresholdPer.ToString("R")).Append('|')
              .Append(LabelCollisionTolerancePct.ToString("R")).Append('|')
              .Append(LabelStaggerStep).Append('|')
              .Append(SwCalcLogarithm).Append('|')
              .Append(SwCalcSymmetry).Append('|')
              .Append(ShowStarSuffix).Append('|')
              .Append(ShowRegularNodes).Append('|')
              .Append(ShowDoubleStarNodes).Append('|')
              .Append(ShowAsiaSession).Append('|')
              .Append(AsiaDisplayDays).Append('|')
              .Append(AsiaStartHour).Append('|')
              .Append(AsiaEndHour).Append('|')
              .Append(AsiaUtcOffset);

            AppendNodesFingerprint(sb, _nodesUp);
            AppendNodesFingerprint(sb, _nodesDown);
            return sb.ToString();
        }

        private static void AppendNodesFingerprint(System.Text.StringBuilder sb, List<Node> nodes)
        {
            sb.Append('#');
            for (int i = 0; i < nodes.Count; i++)
            {
                var n = nodes[i];
                sb.Append(n.NumberNode).Append(',')
                  .Append(n.IndexPreNode).Append(',')
                  .Append(n.IndexNode).Append(',')
                  .Append(n.IndexCorrection).Append(',')
                  .Append(n.HighNode.ToString("R")).Append(',')
                  .Append(n.LowPreNode.ToString("R")).Append(',')
                  .Append(n.LowCorrection.ToString("R")).Append(',')
                  .Append(n.IsSymmetrySetup ? 1 : 0).Append(';');
            }
        }

        private Color ColorAt(int indexColor)
        {
            if (_colors.Count == 0)
                return Color.White;
            int i = indexColor;
            if (i < 0) i = 0;
            if (i >= _colors.Count) i = _colors.Count - 1;
            return _colors[i];
        }

        private void AdvanceColor(ref int indexColor, List<Node> nodes, int i)
        {
            if (i >= nodes.Count - 1)
                return;
            if (nodes[i + 1].NumberNode == 1)
                indexColor++;
            if (indexColor == 8)
                indexColor = 1;
        }

        private bool ShouldDeleteHitTarget(double price, int startIndex, bool isUp)
        {
            if (!DeleteHitTargets || double.IsNaN(price) || double.IsInfinity(price))
                return false;

            int lastIndex = Bars.Count - 1;
            int from = ClampIndex(startIndex + 1);
            if (from > lastIndex)
                return false;

            int firstHitBar = -1;
            for (int b = from; b <= lastIndex; b++)
            {
                bool hit = isUp ? Bars.HighPrices[b] >= price : Bars.LowPrices[b] <= price;
                if (hit)
                {
                    firstHitBar = b;
                    break;
                }
            }

            if (firstHitBar < 0)
                return false;

            return (lastIndex - firstHitBar) >= HitGraceBars;
        }

        private DateTime TargetLabelTime(DateTime startTime, DateTime endTime)
        {
            long ticks = startTime.Ticks + (endTime.Ticks - startTime.Ticks) / 2;
            return new DateTime(ticks, startTime.Kind);
        }

        private static DateTime LerpTime(DateTime a, DateTime b, double t)
        {
            if (t <= 0)
                return a;
            if (t >= 1)
                return b;
            long ticks = a.Ticks + (long)((b.Ticks - a.Ticks) * t);
            return new DateTime(ticks, a.Kind);
        }

        private double LabelNewness(DateTime formedAt)
        {
            if (Bars == null || Bars.Count < 2)
                return 1.0;

            int last = Bars.Count - 1;
            int first = Math.Max(0, last - Math.Max(StartPoint, 50));
            DateTime t0 = Bars.OpenTimes[first];
            DateTime t1 = Bars.OpenTimes[last];
            if (formedAt <= t0)
                return 0.0;
            if (formedAt >= t1)
                return 1.0;

            double span = (t1 - t0).TotalSeconds;
            if (span < 1e-9)
                return 1.0;
            return (formedAt - t0).TotalSeconds / span;
        }

        private bool LabelSlotTaken(DateTime slotTime, double price)
        {
            TimeSpan bar = AverageBarDuration();
            int occupyBars = Math.Max(6, 8 + TargetFontSizeValue() / 2);
            long occupyTicks = bar.Ticks * occupyBars;

            for (int i = 0; i < _stagLabelPrices.Count; i++)
            {
                if (!PricesCloseForLabel(_stagLabelPrices[i], price))
                    continue;
                if (Math.Abs((_stagLabelTimes[i] - slotTime).Ticks) < occupyTicks)
                    return true;
            }
            return false;
        }

        private DateTime PlaceTargetLabelTime(DateTime startTime, DateTime endTime, double price, out bool atRight)
        {
            DateTime mid = TargetLabelTime(startTime, endTime);
            DateTime right = endTime;
            if (right <= mid)
            {
                atRight = false;
                _stagLabelPrices.Add(price);
                _stagLabelTimes.Add(mid);
                return mid;
            }

            double newness = LabelNewness(startTime);
            DateTime preferred = LerpTime(right, mid, newness);

            DateTime chosen = preferred;
            bool hasClose = false;
            bool neighborAtMid = false;
            for (int i = 0; i < _stagLabelPrices.Count; i++)
            {
                if (!PricesCloseForLabel(_stagLabelPrices[i], price))
                    continue;
                hasClose = true;
                if (Math.Abs((_stagLabelTimes[i] - mid).Ticks) <= Math.Abs((_stagLabelTimes[i] - right).Ticks))
                    neighborAtMid = true;
            }

            if (hasClose)
            {
                DateTime first = neighborAtMid ? right : mid;
                DateTime second = neighborAtMid ? mid : right;
                if (!LabelSlotTaken(first, price))
                    chosen = first;
                else if (!LabelSlotTaken(second, price))
                    chosen = second;
                else
                    chosen = LerpTime(mid, right, 0.5);
            }
            else if (LabelSlotTaken(preferred, price))
            {
                DateTime alt = newness >= 0.5 ? right : mid;
                chosen = LabelSlotTaken(alt, price) ? LerpTime(mid, right, 0.5) : alt;
            }

            atRight = Math.Abs((chosen - right).Ticks) <= Math.Abs((chosen - mid).Ticks);
            _stagLabelPrices.Add(price);
            _stagLabelTimes.Add(chosen);
            return chosen;
        }

        // ───────────────────────── Drawing: cleanup / nodes / zig-zag ─────────────────────────

        private void RemoveDrawings()
        {
            var names = new List<string>();
            foreach (var obj in Chart.Objects)
            {
                if (obj.Name != null && obj.Name.StartsWith(ObjectPrefix, StringComparison.Ordinal))
                    names.Add(obj.Name);
            }

            for (int i = 0; i < names.Count; i++)
                Chart.RemoveObject(names[i]);

            Chart.RemoveObject(StatsName);
        }

        private void DrawingNumberNodes(bool swUptrendType)
        {
            var nodes = swUptrendType ? _nodesUp : _nodesDown;
            if (nodes.Count == 0)
                return;

            int indexColor = 0;
            int fontSize = FontSizeValue();

            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                bool shouldDraw = SwCalcSymmetry
                    ? (node.IsSymmetrySetup ? ShowDoubleStarNodes : ShowRegularNodes)
                    : (ShowRegularNodes || ShowDoubleStarNodes);
                if (shouldDraw)
                {
                    int barIdx = ClampIndex(node.IndexNode);
                    Color textColor = ColorAt(indexColor);

                    string newText = SwCalcSymmetry
                        ? node.NumberNode + "*"
                        : node.NumberNode.ToString();

                    if (i > 0 && nodes[i - 1].IndexNode == node.IndexNode)
                        newText = "     " + newText;

                    double y = swUptrendType
                        ? Bars.HighPrices[barIdx]
                        : Bars.LowPrices[barIdx];

                    string name = NextName(swUptrendType ? "UpNum" : "DnNum");
                    var text = Chart.DrawText(name, newText, Bars.OpenTimes[barIdx], y, textColor);
                    text.FontSize = fontSize;
                    text.VerticalAlignment = swUptrendType ? VerticalAlignment.Top : VerticalAlignment.Bottom;
                    text.HorizontalAlignment = HorizontalAlignment.Center;

                    if (IsIncompleteNode2(nodes, i, swUptrendType))
                    {
                        DrawIncompleteNode2Underline(barIdx, y, swUptrendType, textColor);
                        if (ShowIncomplete2Circle)
                            DrawCircleMarker(node.IndexNode, node.HighNode, Incomplete2RadiusPct, Incomplete2RadiusBars, Color.Red, Incomplete2FillTransp);
                    }

                    if (node.IsSymmetrySetup)
                    {
                        int corrIdx = ClampIndex(node.IndexCorrection);
                        string sigText = swUptrendType ? "▲ POI" : "▼ POI";
                        double sigY = swUptrendType
                            ? Bars.LowPrices[corrIdx]
                            : Bars.HighPrices[corrIdx];

                        string sigName = NextName(swUptrendType ? "UpPoi" : "DnPoi");
                        var sig = Chart.DrawText(sigName, sigText, Bars.OpenTimes[corrIdx], sigY, Color.Gray);
                        sig.FontSize = 8;
                        sig.VerticalAlignment = swUptrendType ? VerticalAlignment.Bottom : VerticalAlignment.Top;
                        sig.HorizontalAlignment = HorizontalAlignment.Center;
                    }
                }

                AdvanceColor(ref indexColor, nodes, i);
            }
        }

        private bool IsIncompleteNode2(List<Node> nodes, int i, bool swUptrendType)
        {
            if (i <= 0 || nodes[i].NumberNode != 2 || nodes[i - 1].NumberNode != 1)
                return false;

            var n1 = nodes[i - 1];
            var n2 = nodes[i];
            double move = AbsMove(n1.LowPreNode, n1.HighNode);
            double minPrice = Project(n1.LowCorrection, move, swUptrendType);

            if (double.IsNaN(minPrice) || double.IsInfinity(minPrice))
                return false;

            return swUptrendType
                ? n2.HighNode < minPrice
                : n2.HighNode > minPrice;
        }

        private bool FollowingNode2IsComplete(List<Node> nodes, int node1Index, bool swUptrendType)
        {
            int i2 = node1Index + 1;
            if (i2 >= nodes.Count || nodes[i2].NumberNode != 2)
                return false;
            return !IsIncompleteNode2(nodes, i2, swUptrendType);
        }

        private bool IsActiveNode1Setup(List<Node> nodes, int node1Idx)
        {
            for (int j = node1Idx + 1; j < nodes.Count; j++)
            {
                if (nodes[j].NumberNode == 1)
                    return false;
            }
            return true;
        }

        private static bool PricesWithinTolPct(double a, double b, double tolPct)
        {
            if (double.IsNaN(a) || double.IsNaN(b) || Math.Abs(a) <= 0)
                return false;
            return Math.Abs(a - b) / Math.Abs(a) * 100.0 <= tolPct;
        }

        private void DrawCircleMarker(int barIdx, double price, double radiusPct, int radiusBars, Color lineCol, int fillTransp)
        {
            if (double.IsNaN(price) || price <= 0 || radiusPct <= 0 || radiusBars <= 0)
                return;

            double rPrice = Math.Abs(price) * radiusPct / 100.0;
            if (rPrice <= 0)
                return;

            DateTime t0 = TimeAtIndex(barIdx - radiusBars);
            DateTime t1 = TimeAtIndex(barIdx + radiusBars);
            if (t1 <= t0)
                t1 = t0.AddMinutes(1);

            int t = fillTransp;
            if (t < 0) t = 0;
            if (t > 100) t = 100;
            int alpha = (100 - t) * 255 / 100;
            Color fill = Color.FromArgb(alpha, lineCol.R, lineCol.G, lineCol.B);
            var ell = Chart.DrawEllipse(NextName("Circ"), t0, price + rPrice, t1, price - rPrice, fill);
            ell.IsFilled = true;
            ell.Color = fill;
        }

        private bool IsBasedNode1(Node node)
        {
            double impulse = TargetAbsMove(node.LowPreNode, node.HighNode);
            if (impulse <= 1e-12)
                return false;
            double retrace = TargetAbsMove(node.HighNode, node.LowCorrection);
            return retrace >= impulse * BasedRetraceRatio;
        }

        private void DrawIncompleteNode2Underline(int barIdx, double y, bool swUptrendType, Color color)
        {
            DateTime t0 = TimeAtIndex(barIdx - 1);
            DateTime t1 = TimeAtIndex(barIdx + 1);
            if (t1 <= t0)
                t1 = t0.AddMinutes(1);

            double pip = (Symbol != null && Symbol.PipSize > 0) ? Symbol.PipSize : Math.Abs(y) * 1e-4;
            double offset = pip * 3;
            double lineY = swUptrendType ? y + offset : y - offset;

            Chart.DrawTrendLine(NextName(swUptrendType ? "UpInc2" : "DnInc2"),
                t0, lineY, t1, lineY, color, 2);
        }

        private void DrawingLineNodes()
        {
            DrawZigZag(_nodesUp, "UpLine");
            DrawZigZag(_nodesDown, "DnLine");
        }

        private void DrawZigZag(List<Node> nodes, string prefix)
        {
            if (nodes == null || nodes.Count == 0)
                return;

            int indexColor = 0;
            const int lineWidth = 2;

            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                Color lineColor = ColorAt(indexColor);

                int i1 = ClampIndex(node.IndexPreNode);
                int i2 = ClampIndex(node.IndexNode);
                int i3 = ClampIndex(node.IndexCorrection);

                Chart.DrawTrendLine(NextName(prefix + "A"),
                    Bars.OpenTimes[i1], node.LowPreNode,
                    Bars.OpenTimes[i2], node.HighNode,
                    lineColor, lineWidth);

                Chart.DrawTrendLine(NextName(prefix + "B"),
                    Bars.OpenTimes[i2], node.HighNode,
                    Bars.OpenTimes[i3], node.LowCorrection,
                    lineColor, lineWidth);

                AdvanceColor(ref indexColor, nodes, i);
            }
        }

        // ───────────────────────── Targets ─────────────────────────

        private void DrawingTargetsNode1(bool swUptrendType)
        {
            var nodes = swUptrendType ? _nodesUp : _nodesDown;
            if (nodes.Count == 0)
                return;

            var qualifies = new List<int>();
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].NumberNode == 1)
                    qualifies.Add(i);
            }

            if (qualifies.Count == 0)
                return;

            // 0 = show all node targets; otherwise keep only the newest N
            int skip = TargetMaxCount <= 0 ? 0 : Math.Max(qualifies.Count - TargetMaxCount, 0);
            LineStyle styleDash = ParseLineStyle(TargetLineStyleName);
            LineStyle styleMin = ParseLineStyle(MinLineStyleName);
            int lastIndex = Bars.Count - 1;
            int endIdx = lastIndex + TargetGapBars;
            DateTime endTime = TimeAtIndex(endIdx);
            string trendPrefix = swUptrendType ? "H " : "L ";

            int indexColor = 0;
            var colorByNode = new Color[nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
            {
                colorByNode[i] = ColorAt(indexColor);
                AdvanceColor(ref indexColor, nodes, i);
            }

            for (int q = skip; q < qualifies.Count; q++)
            {
                int i = qualifies[q];
                var node = nodes[i];
                Color lineColor = WithTransparency(colorByNode[i]);
                Color labelColor = colorByNode[i];

                double moveSize = TargetAbsMove(node.LowPreNode, node.HighNode);
                double correctionSize = TargetAbsMove(node.LowCorrection, node.HighNode);
                DateTime startTime = TimeAtIndex(node.IndexNode);

                bool hasRelatedNode2 = i + 1 < nodes.Count && nodes[i + 1].NumberNode == 2;
                double proxMinPrice = double.NaN;
                double proxDbl086Price = double.NaN;

                if (ShowDouble)
                {
                    double price = TargetProject(node.HighNode, moveSize, swUptrendType);

                    if (!ShouldDeleteHitTarget(price, node.IndexNode, swUptrendType))
                    {
                        DrawTargetLine(startTime, endTime, price, lineColor, DoubleLineWidth, styleDash,
                            trendPrefix + "Double.1", labelColor);
                    }
                }

                if (ShowDouble15)
                {
                    double price = TargetProject(node.HighNode, moveSize * Double15Ratio, swUptrendType);

                    if (!ShouldDeleteHitTarget(price, node.IndexNode, swUptrendType))
                    {
                        DrawTargetLine(startTime, endTime, price, lineColor, DoubleLineWidth, styleDash,
                            trendPrefix + "1.5DL.1", labelColor);
                    }
                }

                if (ShowDouble086)
                {
                    if (!hasRelatedNode2)
                    {
                        double doublePrice = TargetProject(node.HighNode, moveSize, swUptrendType);
                        double price = AlongPath(node.LowPreNode, doublePrice, Double086Ratio);

                        if (!ShouldDeleteHitTarget(price, node.IndexNode, swUptrendType))
                        {
                            proxDbl086Price = price;
                            DrawTargetLine(startTime, endTime, price, lineColor, Double086LineWidth,
                                ParseLineStyle(Double086LineStyleName),
                                trendPrefix + "0.8DL.1", labelColor);
                        }
                    }
                }

                if (ShowMin)
                {
                    double price = TargetProject(node.LowCorrection, moveSize, swUptrendType);

                    if (!ShouldDeleteHitTarget(price, node.IndexCorrection, swUptrendType))
                    {
                        proxMinPrice = price;
                        DrawTargetLine(startTime, endTime, price, lineColor, MinLineWidth, styleMin,
                            trendPrefix + "Min 1", labelColor);
                    }
                }

                if (ShowProxCircle
                    && ShowMin
                    && ShowDouble086
                    && !hasRelatedNode2
                    && IsActiveNode1Setup(nodes, i)
                    && PricesWithinTolPct(proxMinPrice, proxDbl086Price, MinDblProximityTolPct))
                {
                    double proxY = (proxMinPrice + proxDbl086Price) / 2.0;
                    int proxBar = (node.IndexPreNode + node.IndexCorrection) / 2;
                    DrawCircleMarker(proxBar, proxY, ProxRadiusPct, ProxRadiusBars, Color.Lime, ProxFillTransp);
                }

                // 0.8Min.1 only while node 2 is incomplete; drop it when that setup's node 2 completes.
                if (ShowMin085
                    && !FollowingNode2IsComplete(nodes, i, swUptrendType)
                    && i + 1 < nodes.Count
                    && IsIncompleteNode2(nodes, i + 1, swUptrendType))
                {
                    double minPrice = TargetProject(node.LowCorrection, moveSize, swUptrendType);
                    double price = AlongPath(node.LowCorrection, minPrice, Min085Ratio);

                    if (!ShouldDeleteHitTarget(price, node.IndexCorrection, swUptrendType))
                    {
                        DrawTargetLine(startTime, endTime, price, lineColor, MinLineWidth,
                            ParseLineStyle(Double086LineStyleName),
                            trendPrefix + "0.8Min.1", labelColor);
                    }
                }

                // Based node 1 (retrace ≥ 0.85): 1.3 of Min 1 while forming node 2.
                if (ShowMin13Based
                    && IsBasedNode1(node)
                    && !FollowingNode2IsComplete(nodes, i, swUptrendType))
                {
                    double price = TargetProject(node.LowCorrection, moveSize * BasedMin13Ratio, swUptrendType);

                    if (!ShouldDeleteHitTarget(price, node.IndexCorrection, swUptrendType))
                    {
                        DrawTargetLine(startTime, endTime, price, lineColor, MinLineWidth, styleMin,
                            trendPrefix + BasedMin13Ratio.ToString("0.##") + "MIN1", labelColor);
                    }
                }

                if (ShowCorrection)
                {
                    double price = TargetProject(node.HighNode, correctionSize, swUptrendType);

                    if (!ShouldDeleteHitTarget(price, node.IndexNode, swUptrendType))
                    {
                        DrawTargetLine(startTime, endTime, price, lineColor, CorrectionLineWidth, styleDash,
                            trendPrefix + "Correction 1", labelColor);
                    }
                }
            }
        }

        private void DrawingPairTargetsNode12(bool swUptrendType)
        {
            var nodes = swUptrendType ? _nodesUp : _nodesDown;
            if (nodes.Count < 2)
                return;

            var pairStarts = new List<int>();
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                if (nodes[i].NumberNode == 1 && nodes[i + 1].NumberNode == 2)
                    pairStarts.Add(i);
            }

            if (pairStarts.Count == 0)
                return;

            // 0 = show all pair targets; otherwise keep only the newest N
            int skip = PairMaxCount <= 0 ? 0 : Math.Max(pairStarts.Count - PairMaxCount, 0);
            LineStyle styleDash = ParseLineStyle(TargetLineStyleName);
            LineStyle styleMin = ParseLineStyle(MinLineStyleName);
            int lastIndex = Bars.Count - 1;
            int endIdx = lastIndex + TargetGapBars;
            DateTime endTime = TimeAtIndex(endIdx);
            string trendPrefix = swUptrendType ? "H " : "L ";

            int indexColor = 0;
            var colorByNode = new Color[nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
            {
                colorByNode[i] = ColorAt(indexColor);
                AdvanceColor(ref indexColor, nodes, i);
            }

            for (int p = skip; p < pairStarts.Count; p++)
            {
                int i = pairStarts[p];
                var node1 = nodes[i];
                var node2 = nodes[i + 1];
                Color lineColor = WithTransparency(colorByNode[i]);
                Color labelColor = colorByNode[i];

                double totalMove = TargetAbsMove(node1.LowPreNode, node2.HighNode);

                if (ShowPairMin)
                {
                    double price = TargetProject(node1.LowCorrection, totalMove, swUptrendType);

                    if (!ShouldDeleteHitTarget(price, node1.IndexCorrection, swUptrendType))
                    {
                        DrawTargetLine(TimeAtIndex(node1.IndexNode), endTime, price, lineColor, MinLineWidth, styleMin,
                            trendPrefix + "Min 12", labelColor);
                    }
                }

                if (ShowPairMax)
                {
                    double price = TargetProject(node2.LowCorrection, totalMove, swUptrendType);

                    if (!ShouldDeleteHitTarget(price, node2.IndexCorrection, swUptrendType))
                    {
                        DrawTargetLine(TimeAtIndex(node2.IndexNode), endTime, price, lineColor, PairMaxLineWidth, styleMin,
                            trendPrefix + "Max 12", labelColor);
                    }
                }

                if (ShowPairDouble)
                {
                    double price = TargetProject(node2.HighNode, totalMove, swUptrendType);

                    if (!ShouldDeleteHitTarget(price, node2.IndexNode, swUptrendType))
                    {
                        DrawTargetLine(TimeAtIndex(node2.IndexNode), endTime, price, lineColor, DoubleLineWidth, styleDash,
                            trendPrefix + "Double 12", labelColor);
                    }
                }

                if (ShowPairCorrection)
                {
                    double correctionSize2 = TargetAbsMove(node2.LowCorrection, node2.HighNode);
                    double price = TargetProject(node2.HighNode, correctionSize2, swUptrendType);

                    if (!ShouldDeleteHitTarget(price, node2.IndexNode, swUptrendType))
                    {
                        DrawTargetLine(TimeAtIndex(node2.IndexNode), endTime, price, lineColor, CorrectionLineWidth, styleDash,
                            trendPrefix + "Correction 2", labelColor);
                    }
                }
            }
        }

        private TimeSpan AverageBarDuration()
        {
            if (Bars.Count >= 2)
            {
                TimeSpan d = Bars.OpenTimes[Bars.Count - 1] - Bars.OpenTimes[Bars.Count - 2];
                if (d > TimeSpan.Zero)
                    return d;
            }
            return TimeSpan.FromMinutes(1);
        }

        private void CaptureLabelPriceRange()
        {
            _labelVisHeight = 400;
            _labelVisTop = 1;
            _labelVisBot = 0;

            try
            {
                double h = Chart.Height;
                if (h >= 2 && !double.IsNaN(h) && !double.IsInfinity(h))
                    _labelVisHeight = h;
            }
            catch
            {
            }

            try
            {
                double top = Chart.TopY;
                double bot = Chart.BottomY;
                if (!double.IsNaN(top) && !double.IsNaN(bot) && Math.Abs(top - bot) > 1e-12)
                {
                    _labelVisTop = Math.Max(top, bot);
                    _labelVisBot = Math.Min(top, bot);
                    return;
                }
            }
            catch
            {
            }

            if (Bars == null || Bars.Count == 0)
                return;

            int last = Bars.Count - 1;
            int first = Math.Max(0, last - 250);
            double hi = double.MinValue;
            double lo = double.MaxValue;
            for (int i = first; i <= last; i++)
            {
                if (Bars.HighPrices[i] > hi)
                    hi = Bars.HighPrices[i];
                if (Bars.LowPrices[i] < lo)
                    lo = Bars.LowPrices[i];
            }
            if (hi > lo)
            {
                _labelVisTop = hi;
                _labelVisBot = lo;
            }
        }

        private bool PricesCloseForLabel(double a, double b)
        {
            double refP = Math.Max(Math.Max(Math.Abs(a), Math.Abs(b)), 1e-7);
            if (Math.Abs(a - b) / refP * 100.0 <= LabelCollisionTolerancePct)
                return true;

            double range = _labelVisTop - _labelVisBot;
            if (range <= 1e-12)
                return false;

            double dyPx = Math.Abs(a - b) / range * _labelVisHeight;
            return dyPx <= TargetFontSizeValue() * 3.2;
        }

        private DateTime StaggerLabelTime(DateTime preferred, double price)
        {
            return StaggerLabelTime(preferred, price, DateTime.MaxValue);
        }

        private DateTime StaggerLabelTime(DateTime preferred, double price, DateTime lineEnd)
        {
            DateTime t = preferred;
            TimeSpan bar = AverageBarDuration();
            int stepBars = Math.Max(4, LabelStaggerStep);
            TimeSpan step = TimeSpan.FromTicks(bar.Ticks * stepBars);
            // Centered text occupies several bars; bigger fonts need a wider exclusive slot.
            int occupyBars = Math.Max(stepBars, 8 + TargetFontSizeValue());
            long occupyTicks = bar.Ticks * occupyBars;

            DateTime maxT = lineEnd;
            if (maxT < preferred || maxT == DateTime.MaxValue)
                maxT = preferred.Add(TimeSpan.FromTicks(bar.Ticks * Math.Max(80, stepBars * 8)));

            int guard = 0;
            while (guard < 48)
            {
                DateTime pushTo = t;
                bool collision = false;
                for (int i = 0; i < _stagLabelPrices.Count; i++)
                {
                    if (!PricesCloseForLabel(_stagLabelPrices[i], price))
                        continue;
                    long dt = Math.Abs((_stagLabelTimes[i] - t).Ticks);
                    if (dt >= occupyTicks)
                        continue;
                    collision = true;
                    DateTime next = _stagLabelTimes[i].Add(step);
                    if (next > pushTo)
                        pushTo = next;
                }
                if (!collision)
                    break;
                if (pushTo <= t)
                    pushTo = t.Add(step);
                t = pushTo > maxT ? maxT : pushTo;
                if (t >= maxT)
                    break;
                guard++;
            }

            _stagLabelPrices.Add(price);
            _stagLabelTimes.Add(t);
            return t;
        }

        private void DrawTargetLine(DateTime startTime, DateTime endTime, double price, Color lineColor,
            int width, LineStyle style, string label, Color labelColor)
        {
            DrawTargetLine(startTime, endTime, price, lineColor, width, style, label, labelColor, false);
        }

        private void DrawTargetLine(DateTime startTime, DateTime endTime, double price, Color lineColor,
            int width, LineStyle style, string label, Color labelColor, bool pinLabelAtEnd)
        {
            Chart.DrawTrendLine(NextName("Tgt"), startTime, price, endTime, price, lineColor, width, style);

            DateTime labelTime;
            bool atRight;
            if (pinLabelAtEnd)
            {
                labelTime = endTime;
                atRight = true;
                _stagLabelPrices.Add(price);
                _stagLabelTimes.Add(labelTime);
            }
            else
            {
                labelTime = PlaceTargetLabelTime(startTime, endTime, price, out atRight);
            }

            var txt = Chart.DrawText(NextName("TgtLbl"), label, labelTime, price, labelColor);
            txt.FontSize = TargetFontSizeValue();
            txt.VerticalAlignment = VerticalAlignment.Center;
            txt.HorizontalAlignment = atRight ? HorizontalAlignment.Right : HorizontalAlignment.Center;
        }

        private void DrawingDayOpenClose()
        {
            if (_dailyBars == null)
            {
                try { _dailyBars = MarketData.GetBars(TimeFrame.Daily); } catch { return; }
            }

            if (_dailyBars == null || _dailyBars.Count < 1)
                return;

            int lastIndex = Bars.Count - 1;
            DateTime startTime = Bars.OpenTimes[lastIndex];
            DateTime endTime = TimeAtIndex(lastIndex + TargetGapBars);

            if (ShowDayOpenTarget)
            {
                double openToday = _dailyBars.LastBar.Open;
                if (openToday > 0 && !double.IsNaN(openToday))
                {
                    DrawTargetLine(startTime, endTime, openToday, Color.Lime, 2, LineStyle.Solid,
                        "H Day Open", Color.Lime, true);
                }
            }

            if (ShowDayCloseTarget && _dailyBars.Count >= 2)
            {
                double closeYesterday = _dailyBars[_dailyBars.Count - 2].Close;
                if (closeYesterday > 0 && !double.IsNaN(closeYesterday))
                {
                    DrawTargetLine(startTime, endTime, closeYesterday, Color.Red, 2, LineStyle.Dots,
                        "L Day Close", Color.Red, true);
                }
            }

            if (ShowDayOcPanel && _dailyBars.Count >= 2)
            {
                double openToday = _dailyBars.LastBar.Open;
                double closeYesterday = _dailyBars[_dailyBars.Count - 2].Close;
                if (openToday > 0 && closeYesterday > 0
                    && !double.IsNaN(openToday) && !double.IsNaN(closeYesterday))
                {
                    double boxTop = Math.Max(openToday, closeYesterday);
                    double boxBot = Math.Min(openToday, closeYesterday);
                    Color fill = Color.FromArgb(40, 30, 80, 180);
                    var box = Chart.DrawRectangle(NextName("OcBox"), startTime, boxTop, endTime, boxBot, fill);
                    box.IsFilled = true;
                }
            }
        }

        private sealed class AsiaBox
        {
            public int StartIdx;
            public int EndIdx;
            public double High;
            public double Low;
        }

        private bool InAsiaSession(DateTime openTime)
        {
            DateTime local = openTime.AddHours(AsiaUtcOffset);
            int minutes = local.Hour * 60 + local.Minute;
            int start = Math.Max(0, Math.Min(23, AsiaStartHour)) * 60 + Math.Max(0, Math.Min(59, AsiaStartMinute));
            int end = Math.Max(0, Math.Min(23, AsiaEndHour)) * 60 + Math.Max(0, Math.Min(59, AsiaEndMinute));
            if (start == end)
                return false;
            if (start < end)
                return minutes >= start && minutes < end;
            return minutes >= start || minutes < end;
        }

        private void DrawingAsiaSession()
        {
            if (Bars == null || Bars.Count < 2)
                return;

            int lastIndex = Bars.Count - 1;
            var boxes = new List<AsiaBox>();
            AsiaBox current = null;

            for (int i = 0; i <= lastIndex; i++)
            {
                bool inside = InAsiaSession(Bars.OpenTimes[i]);
                if (inside)
                {
                    double h = Bars.HighPrices[i];
                    double l = Bars.LowPrices[i];
                    if (current == null)
                    {
                        current = new AsiaBox
                        {
                            StartIdx = i,
                            EndIdx = i,
                            High = h,
                            Low = l
                        };
                    }
                    else
                    {
                        current.EndIdx = i;
                        if (h > current.High) current.High = h;
                        if (l < current.Low) current.Low = l;
                    }
                }
                else if (current != null)
                {
                    boxes.Add(current);
                    current = null;
                }
            }

            if (current != null)
                boxes.Add(current);

            int keep = Math.Max(1, Math.Min(10, AsiaDisplayDays));
            int startBox = Math.Max(0, boxes.Count - keep);

            Color asiaYellow = Color.FromArgb(255, 255, 235, 59);
            Color asiaFill = Color.FromArgb(25, 255, 235, 59);
            string label = string.IsNullOrWhiteSpace(AsiaSessionName) ? "Asia" : AsiaSessionName.Trim();

            for (int b = startBox; b < boxes.Count; b++)
            {
                var box = boxes[b];
                DateTime t1 = Bars.OpenTimes[ClampIndex(box.StartIdx)];
                DateTime t2 = Bars.OpenTimes[ClampIndex(box.EndIdx)];
                if (t2 <= t1)
                    t2 = t1.AddMinutes(1);

                int extendIdx = box.EndIdx + Math.Max(0, AsiaMidlineExtension);
                bool sessionEnded = box.EndIdx < lastIndex;
                if (AsiaExtendMidlineToLast && sessionEnded)
                    extendIdx = lastIndex;
                DateTime extendEnd = TimeAtIndex(extendIdx);
                if (extendEnd <= t1)
                    extendEnd = t2;

                if (AsiaShowRange)
                {
                    var rect = Chart.DrawRectangle(NextName("AsiaBox"), t1, box.High, t2, box.Low, asiaFill);
                    rect.IsFilled = true;
                    rect.Color = asiaFill;
                    rect.Thickness = 1;
                    rect.LineStyle = LineStyle.Dots;

                    Chart.DrawTrendLine(NextName("AsiaHigh"), t1, box.High, extendEnd, box.High, asiaYellow, 1, LineStyle.Dots);
                    Chart.DrawTrendLine(NextName("AsiaLow"), t1, box.Low, extendEnd, box.Low, asiaYellow, 1, LineStyle.Dots);
                    Chart.DrawTrendLine(NextName("AsiaEdge"), t1, box.High, t1, box.Low, asiaYellow, 1, LineStyle.Dots);
                    Chart.DrawTrendLine(NextName("AsiaEdge"), t2, box.High, t2, box.Low, asiaYellow, 1, LineStyle.Dots);

                    DateTime midT = new DateTime(t1.Ticks + (t2.Ticks - t1.Ticks) / 2, t1.Kind);
                    var txt = Chart.DrawText(NextName("AsiaLbl"), label, midT, box.High, asiaYellow);
                    txt.FontSize = TargetFontSizeValue();
                    txt.VerticalAlignment = VerticalAlignment.Top;
                    txt.HorizontalAlignment = HorizontalAlignment.Center;
                }

                if (AsiaShowMidline)
                {
                    double mid = (box.High + box.Low) / 2.0;
                    Chart.DrawTrendLine(NextName("AsiaMid"), t1, mid, extendEnd, mid, asiaYellow, 1, LineStyle.Dots);
                }
            }

            if (boxes.Count == 0)
                return;
            if (!AsiaShowHighTarget && !AsiaShowLowTarget && !AsiaShowMidTarget)
                return;

            var latest = boxes[boxes.Count - 1];
            DateTime tgtStart = Bars.OpenTimes[lastIndex];
            DateTime tgtEnd = TimeAtIndex(lastIndex + TargetGapBars);

            if (AsiaShowHighTarget)
            {
                DrawTargetLine(tgtStart, tgtEnd, latest.High, asiaYellow, 1, LineStyle.Dots,
                    label + " High", asiaYellow);
            }

            if (AsiaShowLowTarget)
            {
                DrawTargetLine(tgtStart, tgtEnd, latest.Low, asiaYellow, 1, LineStyle.Dots,
                    label + " Low", asiaYellow);
            }

            if (AsiaShowMidTarget)
            {
                double mid = (latest.High + latest.Low) / 2.0;
                DrawTargetLine(tgtStart, tgtEnd, mid, asiaYellow, 1, LineStyle.Dots,
                    label + " Mid", asiaYellow);
            }
        }

        private Color ColorWithAlphaPct(Color baseColor, int transparency)
        {
            int t = transparency;
            if (t < 0) t = 0;
            if (t > 100) t = 100;
            int alpha = (100 - t) * 255 / 100;
            return Color.FromArgb(alpha, baseColor.R, baseColor.G, baseColor.B);
        }

        private void DrawingRoundNumberTargets(bool isUp)
        {
            if (RoundBasePrice <= 0 || Bars == null || Bars.Count < 2)
                return;

            int lastIndex = Bars.Count - 1;
            int lookback = Math.Min(Math.Max(StartPoint, 1), lastIndex);
            int from = Math.Max(0, lastIndex - lookback);
            double chartLow = Bars.LowPrices[from];
            double chartHigh = Bars.HighPrices[from];
            for (int i = from + 1; i <= lastIndex; i++)
            {
                if (Bars.LowPrices[i] < chartLow)
                    chartLow = Bars.LowPrices[i];
                if (Bars.HighPrices[i] > chartHigh)
                    chartHigh = Bars.HighPrices[i];
            }

            double effectiveMin = RoundMinVisiblePrice > 0 ? Math.Max(RoundMinVisiblePrice, chartLow) : chartLow;
            double effectiveMax = chartHigh;
            int startMult = (int)Math.Floor(effectiveMin / RoundBasePrice);
            int endMult = (int)Math.Ceiling(effectiveMax / RoundBasePrice);
            if (startMult < 1)
                startMult = 1;
            if (endMult < startMult)
                return;

            const int maxLevels = 40;
            int totalLevels = endMult - startMult + 1;
            if (totalLevels > maxLevels)
            {
                int mid = (startMult + endMult) / 2;
                int half = maxLevels / 2;
                startMult = Math.Max(1, mid - half);
                endMult = startMult + maxLevels - 1;
            }

            DateTime startTime = Bars.OpenTimes[lastIndex];
            DateTime endTime = TimeAtIndex(lastIndex + TargetGapBars);
            Color lineColor = ColorWithAlphaPct(ParseColor(RoundLineColorName, Color.Gray), RoundLineTransparency);
            Color labelColor = ParseColor(RoundLineColorName, Color.Gray);
            LineStyle style = ParseLineStyle(RoundLineStyleName);
            string prefix = isUp ? "H RN " : "L RN ";

            for (int mult = startMult; mult <= endMult; mult++)
            {
                double roundPrice = RoundBasePrice * mult;
                DrawTargetLine(startTime, endTime, roundPrice, lineColor, RoundLineWidth, style,
                    prefix + roundPrice.ToString("0.####"), labelColor);
            }
        }

        private void DrawingFairValueGaps()
        {
            if (Bars == null || Bars.Count < 4)
                return;

            int lastIndex = Bars.Count - 1;
            double autoCum = 0;
            int autoN = 0;
            var born = new List<int>();
            var mx = new List<double>();
            var mn = new List<double>();
            var isBull = new List<bool>();

            for (int i = 2; i <= lastIndex; i++)
            {
                double hi = Bars.HighPrices[i];
                double lo = Bars.LowPrices[i];
                if (lo > 0)
                {
                    autoCum += (hi - lo) / lo;
                    autoN++;
                }

                double thr = FvgAutoThreshold
                    ? (autoN > 0 ? autoCum / autoN : 0.0)
                    : FvgThresholdPer / 100.0;

                double hi2 = Bars.HighPrices[i - 2];
                double lo2 = Bars.LowPrices[i - 2];
                double close1 = Bars.ClosePrices[i - 1];

                bool bull = lo > hi2 && close1 > hi2 && hi2 > 0 && (lo - hi2) / hi2 > thr;
                bool bear = hi < lo2 && close1 < lo2 && hi > 0 && (lo2 - hi) / hi > thr;
                if (bull)
                {
                    born.Add(i);
                    mx.Add(lo);
                    mn.Add(hi2);
                    isBull.Add(true);
                    _fvgBullCount++;
                }
                else if (bear)
                {
                    born.Add(i);
                    mx.Add(lo2);
                    mn.Add(hi);
                    isBull.Add(false);
                    _fvgBearCount++;
                }
            }

            Color bullCss = ParseColor(FvgBullColorName, Color.FromArgb(180, 8, 153, 129));
            Color bearCss = ParseColor(FvgBearColorName, Color.FromArgb(180, 242, 54, 69));
            Color bullFill = Color.FromArgb(70, bullCss.R, bullCss.G, bullCss.B);
            Color bearFill = Color.FromArgb(70, bearCss.R, bearCss.G, bearCss.B);

            var aliveIdx = new List<int>();
            for (int g = 0; g < born.Count; g++)
            {
                int b = born[g];
                double top = mx[g];
                double bot = mn[g];
                bool mitigated = false;
                int mitBar = lastIndex;
                for (int i = b + 1; i <= lastIndex; i++)
                {
                    if (Bars.HighPrices[i] >= bot && Bars.LowPrices[i] <= top)
                    {
                        mitigated = true;
                        mitBar = i;
                        break;
                    }
                }

                if (mitigated)
                {
                    if (isBull[g])
                        _fvgBullMitigated++;
                    else
                        _fvgBearMitigated++;

                    if (FvgMitigationLevels)
                    {
                        double y = isBull[g] ? bot : top;
                        Chart.DrawTrendLine(NextName("FvgMit"),
                            Bars.OpenTimes[ClampIndex(b - 2)], y,
                            Bars.OpenTimes[mitBar], y,
                            isBull[g] ? bullCss : bearCss, 1, LineStyle.Dots);
                    }
                    continue;
                }

                aliveIdx.Add(g);
                int right = Math.Max(lastIndex, b + FvgExtend);
                DateTime t1 = Bars.OpenTimes[ClampIndex(b - 2)];
                DateTime t2 = TimeAtIndex(right);
                Color fill = isBull[g] ? bullFill : bearFill;
                var rect = Chart.DrawRectangle(NextName("Fvg"), t1, top, t2, bot, fill);
                rect.IsFilled = true;
                rect.Color = fill;
                rect.Thickness = 1;
            }

            if (FvgShowLast > 0 && aliveIdx.Count > 0)
            {
                int take = Math.Min(FvgShowLast, aliveIdx.Count);
                for (int k = 0; k < take; k++)
                {
                    int g = aliveIdx[aliveIdx.Count - 1 - k];
                    double lvl = isBull[g] ? mn[g] : mx[g];
                    Color col = isBull[g] ? bullCss : bearCss;
                    Chart.DrawTrendLine(NextName("FvgLvl"),
                        Bars.OpenTimes[ClampIndex(born[g] - 2)], lvl,
                        Bars.OpenTimes[lastIndex], lvl, col, 1, LineStyle.Solid);
                }
            }
        }

        private void DrawingMapWeekly()
        {
            if (_weeklyBars == null)
            {
                try { _weeklyBars = MarketData.GetBars(TimeFrame.Weekly); } catch { return; }
            }
            if (_weeklyBars == null || _weeklyBars.Count < 2 || Bars == null || Bars.Count < 2)
                return;

            int prevIdx = _weeklyBars.Count - 2;
            var bar = _weeklyBars[prevIdx];
            double high = bar.High;
            double low = bar.Low;
            if (high <= 0 || low <= 0 || double.IsNaN(high) || double.IsNaN(low) || high <= low)
                return;

            double range = high - low;
            int lastIndex = Bars.Count - 1;
            DateTime currentWeekOpen = _weeklyBars.OpenTimes[_weeklyBars.Count - 1];
            DateTime lineStart = FindCurrentWeekStart(currentWeekOpen);
            DateTime lineEnd = TimeAtIndex(lastIndex + TargetGapBars);

            Color highColor = ColorWithAlphaPct(ParseColor(MapHighColorName, Color.FromArgb(255, 255, 77, 109)), MapTransparency);
            Color lowColor = ColorWithAlphaPct(ParseColor(MapLowColorName, Color.FromArgb(255, 124, 255, 71)), MapTransparency);
            Color midColor = ColorWithAlphaPct(ParseColor(MapMidColorName, Color.FromArgb(255, 255, 229, 102)), MapTransparency);
            Color retraceColor = ColorWithAlphaPct(ParseColor(MapRetraceColorName, Color.FromArgb(255, 122, 162, 255)), MapTransparency);
            Color extColor = ColorWithAlphaPct(ParseColor(MapExtColorName, Color.FromArgb(255, 192, 132, 252)), MapTransparency);
            LineStyle keyStyle = LineStyle.Solid;
            LineStyle midStyle = LineStyle.Dots;
            LineStyle extStyle = LineStyle.DotsRare;

            if (MapShowHigh)
                DrawTargetLine(lineStart, lineEnd, high, highColor, 2, keyStyle, "WM", highColor, true);
            if (MapShowLow)
                DrawTargetLine(lineStart, lineEnd, low, lowColor, 2, keyStyle, "WM", lowColor, true);
            if (MapShowMid)
                DrawTargetLine(lineStart, lineEnd, (high + low) / 2.0, midColor, 1, midStyle, "WM", midColor, true);
            if (MapShow25)
                DrawTargetLine(lineStart, lineEnd, low + range * 0.25, retraceColor, 1, midStyle, "WM", retraceColor, true);
            if (MapShow75)
                DrawTargetLine(lineStart, lineEnd, low + range * 0.75, retraceColor, 1, midStyle, "WM", retraceColor, true);

            if (MapShowExtAbove)
            {
                DrawMapExt(lineStart, lineEnd, high, range, true, extColor, extStyle);
            }
            if (MapShowExtBelow)
            {
                DrawMapExt(lineStart, lineEnd, low, range, false, extColor, extStyle);
            }
        }

        private void DrawMapExt(DateTime start, DateTime end, double origin, double range, bool above,
            Color color, LineStyle style)
        {
            double sign = above ? 1.0 : -1.0;
            if (MapShow1125)
                DrawMapExtLevel(start, end, origin + sign * range * 0.125, color, style);
            if (MapShow125)
                DrawMapExtLevel(start, end, origin + sign * range * 0.25, color, style);
            if (MapShow1375)
                DrawMapExtLevel(start, end, origin + sign * range * 0.375, color, style);
            if (MapShow150)
                DrawMapExtLevel(start, end, origin + sign * range * 0.50, color, style);
            if (MapShow175)
                DrawMapExtLevel(start, end, origin + sign * range * 0.75, color, style);
            if (MapShow200)
                DrawMapExtLevel(start, end, origin + sign * range * 1.00, color, style);
        }

        private void DrawMapExtLevel(DateTime start, DateTime end, double price,
            Color color, LineStyle style)
        {
            DrawTargetLine(start, end, price, color, 1, style, "WM", color, true);
        }

        private DateTime FindCurrentWeekStart(DateTime currentWeekOpen)
        {
            if (Bars == null || Bars.Count == 0)
                return Server.Time;

            for (int i = Bars.Count - 1; i >= 0; i--)
            {
                if (Bars.OpenTimes[i] < currentWeekOpen)
                    return i + 1 < Bars.Count ? Bars.OpenTimes[i + 1] : Bars.OpenTimes[i];
            }
            return Bars.OpenTimes[0];
        }

        private void DrawStatsOverlay()
        {
            string text =
                "Up Nodes " + _nodesUp.Count +
                "\nDn Nodes " + _nodesDown.Count;
            if (EnableFvg && FvgShowDash)
            {
                double bullMitPct = _fvgBullCount > 0 ? _fvgBullMitigated * 100.0 / _fvgBullCount : 0.0;
                double bearMitPct = _fvgBearCount > 0 ? _fvgBearMitigated * 100.0 / _fvgBearCount : 0.0;
                text +=
                    "\nFVG Bull " + _fvgBullCount +
                    "\nFVG Bear " + _fvgBearCount +
                    "\nFVG Bull Mit " + bullMitPct.ToString("0.##") + "%" +
                    "\nFVG Bear Mit " + bearMitPct.ToString("0.##") + "%";
            }

            Chart.DrawStaticText(StatsName, text, VerticalAlignment.Top, HorizontalAlignment.Right, Color.White);
        }
    }
}
