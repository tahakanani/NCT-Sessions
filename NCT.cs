// NCT Dual Symmetry Indicator for cTrader Automate
// Port of Pine core engine + node targets + density (no sessions / POV)
using System;
using System.Collections.Generic;
using cAlgo.API;
using cAlgo.API.Internals;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class NCT : Indicator
    {
        // ───────────────────────── Nodal / Strategy ─────────────────────────

        [Parameter("Starting Point Candles", DefaultValue = 5000, MinValue = 1, MaxValue = 5000, Group = "Nodal Calculation Settings")]
        public int StartPoint { get; set; }

        [Parameter("End Point (Final)", DefaultValue = 0, MinValue = 0, MaxValue = 5000, Group = "Nodal Calculation Settings")]
        public int EndPoint { get; set; }

        [Parameter("Calc Nodes with Symmetry", DefaultValue = true, Group = "Strategy Config")]
        public bool SwCalcSymmetry { get; set; }

        // Logarithm is always ON for node calc + targets (requested).
        // Kept for compatibility; value is ignored at runtime.
        [Parameter("Calc Nodes with Logarithm (Always On)", DefaultValue = true, Group = "Strategy Config")]
        public bool SwCalcLogarithm { get; set; }

        [Parameter("Show Regular Nodes", DefaultValue = true, Group = "Strategy Config")]
        public bool ShowRegularNodes { get; set; }

        [Parameter("Show Double-Star Nodes (**)", DefaultValue = true, Group = "Strategy Config")]
        public bool ShowDoubleStarNodes { get; set; }

        [Parameter("Show Star Suffix (*)", DefaultValue = true, Group = "Strategy Config")]
        public bool ShowStarSuffix { get; set; }

        [Parameter("Show Node Connecting Lines", DefaultValue = false, Group = "Strategy Config")]
        public bool ShowNodeConnectingLines { get; set; }

        // ───────────────────────── Visual ─────────────────────────

        [Parameter("Font Size (Node Numbers)", DefaultValue = "Normal", Group = "Visual Customization")]
        public string TextNodeSize { get; set; }

        [Parameter("Font Size (Target Labels)", DefaultValue = "Small", Group = "Visual Customization")]
        public string TargetLabelFontSize { get; set; }

        [Parameter("Color 1", DefaultValue = "White", Group = "Visual Customization")]
        public string Color1Name { get; set; }

        [Parameter("Color 2", DefaultValue = "Yellow", Group = "Visual Customization")]
        public string Color2Name { get; set; }

        [Parameter("Color 3", DefaultValue = "Aqua", Group = "Visual Customization")]
        public string Color3Name { get; set; }

        [Parameter("Color 4", DefaultValue = "Lime", Group = "Visual Customization")]
        public string Color4Name { get; set; }

        [Parameter("Color 5", DefaultValue = "Orange", Group = "Visual Customization")]
        public string Color5Name { get; set; }

        [Parameter("Color 6", DefaultValue = "Fuchsia", Group = "Visual Customization")]
        public string Color6Name { get; set; }

        [Parameter("Color 7", DefaultValue = "Blue", Group = "Visual Customization")]
        public string Color7Name { get; set; }

        [Parameter("Color 8", DefaultValue = "Red", Group = "Visual Customization")]
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

        [Parameter("Show Double (Single)", DefaultValue = true, Group = "Node Targets")]
        public bool ShowDouble { get; set; }

        [Parameter("Show Min (Single)", DefaultValue = true, Group = "Node Targets")]
        public bool ShowMin { get; set; }

        [Parameter("Show Correction (Single)", DefaultValue = true, Group = "Node Targets")]
        public bool ShowCorrection { get; set; }

        [Parameter("Show Pair Min", DefaultValue = true, Group = "Node Targets")]
        public bool ShowPairMin { get; set; }

        [Parameter("Show Pair Max", DefaultValue = true, Group = "Node Targets")]
        public bool ShowPairMax { get; set; }

        [Parameter("Show Pair Double", DefaultValue = true, Group = "Node Targets")]
        public bool ShowPairDouble { get; set; }

        [Parameter("Show Pair Correction", DefaultValue = true, Group = "Node Targets")]
        public bool ShowPairCorrection { get; set; }

        [Parameter("Target Gap Bars", DefaultValue = 50, MinValue = 3, MaxValue = 500, Group = "Node Targets")]
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

        // ───────────────────────── Density ─────────────────────────

        [Parameter("Enable Density", DefaultValue = true, Group = "Close Target Detection")]
        public bool EnableDensity { get; set; }

        [Parameter("Red Tolerance (%)", DefaultValue = 0.01, MinValue = 0.001, MaxValue = 10, Group = "Close Target Detection")]
        public double RedTolerancePct { get; set; }

        [Parameter("Yellow Tolerance (%)", DefaultValue = 0.05, MinValue = 0.001, MaxValue = 10, Group = "Close Target Detection")]
        public double YellowTolerancePct { get; set; }

        // ───────────────────────── State ─────────────────────────

        private readonly List<Node> _nodesUp = new List<Node>();
        private readonly List<Node> _nodesDown = new List<Node>();
        private readonly List<Color> _colors = new List<Color>();
        private readonly List<double> _densityPrices = new List<double>();
        private readonly List<bool> _densityIsUp = new List<bool>();
        private readonly List<DateTime> _densityLabelTimes = new List<DateTime>();

        private bool _initialized;
        private int _objSeq;
        private string _lastDrawSignature;
        private int _lastDrawBarIndex = -1;

        private double _priceLowestUp = 999999.0;
        private int _indexLowestUp;
        private double _priceHighestUp;
        private int _indexHighestUp;

        private double _priceLowestDown = 999999.0;
        private int _indexLowestDown;
        private double _priceHighestDown;
        private int _indexHighestDown;

        private int _redDensityCount;
        private int _yellowDensityCount;

        private const string ObjectPrefix = "NCT_";
        private const string StatsName = "NCT_DensityStats";

        // ───────────────────────── Lifecycle ─────────────────────────

        protected override void Initialize()
        {
            ResetState();
        }

        public override void Calculate(int index)
        {
            try
            {
                if (index == 0)
                    ResetState();

                int lastIndex = Bars.Count - 1;
                if (lastIndex < 2)
                    return;

                int min = lastIndex - StartPoint;
                int max = lastIndex - EndPoint;

                if (index > min && index <= max && index > 1)
                {
                    if (!_initialized)
                        InitFirst();

                    CalcNodeUpTrend(index);
                    CalcNodeDownTrend(index);
                }

                if (index == lastIndex)
                {
                    // Avoid delete/redraw flicker on every tick while scrolling or live updating.
                    // Redraw only when node structure changed or a new bar arrived.
                    string signature = BuildDrawSignature(lastIndex);
                    if (signature == _lastDrawSignature && _lastDrawBarIndex == lastIndex)
                        return;

                    RemoveDrawings();
                    _densityPrices.Clear();
                    _densityIsUp.Clear();
                    _densityLabelTimes.Clear();
                    _redDensityCount = 0;
                    _yellowDensityCount = 0;
                    _objSeq = 0;

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

                    if (EnableDensity)
                        DrawDensityClusters();

                    DrawStatsOverlay();

                    _lastDrawSignature = signature;
                    _lastDrawBarIndex = lastIndex;
                }
            }
            catch
            {
                // Avoid breaking the chart on unexpected data edge cases
            }
        }

        // ───────────────────────── Init / Reset ─────────────────────────

        private void ResetState()
        {
            _nodesUp.Clear();
            _nodesDown.Clear();
            _colors.Clear();
            _densityPrices.Clear();
            _densityIsUp.Clear();
            _densityLabelTimes.Clear();
            _initialized = false;
            _objSeq = 0;
            _redDensityCount = 0;
            _yellowDensityCount = 0;
            _lastDrawSignature = null;
            _lastDrawBarIndex = -1;

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

        private void InitFirst()
        {
            InitColors();
            _initialized = true;
        }

        private void InitColors()
        {
            _colors.Clear();
            _colors.Add(ParseColor(Color1Name, Color.White));
            _colors.Add(ParseColor(Color2Name, Color.Yellow));
            _colors.Add(ParseColor(Color3Name, Color.Aqua));
            _colors.Add(ParseColor(Color4Name, Color.Lime));
            _colors.Add(ParseColor(Color5Name, Color.Orange));
            _colors.Add(ParseColor(Color6Name, Color.Fuchsia));
            _colors.Add(ParseColor(Color7Name, Color.Blue));
            _colors.Add(ParseColor(Color8Name, Color.Red));
        }

        private static Color ParseColor(string name, Color fallback)
        {
            if (string.IsNullOrWhiteSpace(name))
                return fallback;

            switch (name.Trim().ToLowerInvariant())
            {
                case "white": return Color.White;
                case "yellow": return Color.Yellow;
                case "aqua":
                case "cyan": return Color.Aqua;
                case "lime": return Color.Lime;
                case "green": return Color.Green;
                case "orange": return Color.Orange;
                case "fuchsia":
                case "magenta": return Color.Fuchsia;
                case "blue": return Color.Blue;
                case "red": return Color.Red;
                case "gray":
                case "grey": return Color.Gray;
                case "black": return Color.Black;
                case "dimgray":
                case "dimgrey": return Color.DimGray;
                case "lightgray":
                case "lightgrey": return Color.LightGray;
                case "darkorange": return Color.DarkOrange;
                case "darkblue": return Color.DarkBlue;
                case "darkred": return Color.DarkRed;
                case "darkgreen": return Color.DarkGreen;
                case "deepskyblue": return Color.DeepSkyBlue;
                case "dodgerblue": return Color.DodgerBlue;
                case "gold": return Color.Gold;
                case "khaki": return Color.Khaki;
                case "violet": return Color.Violet;
                case "purple": return Color.Purple;
                case "pink": return Color.Pink;
                case "brown": return Color.Brown;
                case "coral": return Color.Coral;
                case "tomato": return Color.Tomato;
                case "teal": return Color.Teal;
                case "navy": return Color.Navy;
                case "maroon": return Color.Maroon;
                case "olive": return Color.Olive;
                case "silver": return Color.Silver;
                default: return fallback;
            }
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
            // Always logarithmic (targets + nodal calc must stay log-based)
            SortNodesUpTrendModeLog();
        }

        private void SortDown()
        {
            // Always logarithmic (targets + nodal calc must stay log-based)
            SortNodesDownTrendModeLog();
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
            return ParseFontSize(TargetLabelFontSize, 10);
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
              .Append(_nodesUp.Count).Append('|')
              .Append(_nodesDown.Count).Append('|')
              .Append(ShowTargetUp).Append('|')
              .Append(ShowTargetDown).Append('|')
              .Append(EnableSingleNode1Targets).Append('|')
              .Append(EnablePairNode12Targets).Append('|')
              .Append(TargetMaxCount).Append('|')
              .Append(PairMaxCount).Append('|')
              .Append(TargetLabelFontSize).Append('|')
              .Append(TextNodeSize).Append('|')
              .Append(TargetGapBars).Append('|')
              .Append(DeleteHitTargets).Append('|')
              .Append(EnableDensity);

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

        private void RegisterDensity(double price, bool isUp, DateTime endTime)
        {
            if (!EnableDensity || double.IsNaN(price) || double.IsInfinity(price) || price <= 0)
                return;
            _densityPrices.Add(price);
            _densityIsUp.Add(isUp);
            _densityLabelTimes.Add(endTime);
        }

        private DateTime TargetLabelTime(DateTime startTime, DateTime endTime)
        {
            // Keep target text in the middle of the line.
            long ticks = startTime.Ticks + (endTime.Ticks - startTime.Ticks) / 2;
            return new DateTime(ticks, startTime.Kind);
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
            string suffix = ShowStarSuffix ? "*" : "";

            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                bool shouldDraw = node.IsSymmetrySetup ? ShowDoubleStarNodes : ShowRegularNodes;
                if (shouldDraw)
                {
                    int barIdx = ClampIndex(node.IndexNode);
                    Color textColor = ColorAt(indexColor);

                    string newText = node.IsSymmetrySetup
                        ? "**" + node.NumberNode + "**"
                        : node.NumberNode + suffix;

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

                double moveSizeLog = Math.Abs(SafeLog(node.HighNode) - SafeLog(node.LowPreNode));
                double correctionSizeLog = Math.Abs(SafeLog(node.HighNode) - SafeLog(node.LowCorrection));
                DateTime startTime = TimeAtIndex(node.IndexNode);

                if (ShowDouble)
                {
                    double price = swUptrendType
                        ? Math.Exp(SafeLog(node.HighNode) + moveSizeLog)
                        : Math.Exp(SafeLog(node.HighNode) - moveSizeLog);

                    if (!ShouldDeleteHitTarget(price, node.IndexNode, swUptrendType))
                    {
                        DrawTargetLine(startTime, endTime, price, lineColor, DoubleLineWidth, styleDash,
                            trendPrefix + "Double 1", labelColor);
                        RegisterDensity(price, swUptrendType, endTime);
                    }
                }

                if (ShowMin)
                {
                    double price = swUptrendType
                        ? Math.Exp(SafeLog(node.LowCorrection) + moveSizeLog)
                        : Math.Exp(SafeLog(node.LowCorrection) - moveSizeLog);

                    if (!ShouldDeleteHitTarget(price, node.IndexCorrection, swUptrendType))
                    {
                        DrawTargetLine(startTime, endTime, price, lineColor, MinLineWidth, styleMin,
                            trendPrefix + "Min 1", labelColor);
                        RegisterDensity(price, swUptrendType, endTime);
                    }
                }

                if (ShowCorrection)
                {
                    double price = swUptrendType
                        ? Math.Exp(SafeLog(node.HighNode) + correctionSizeLog)
                        : Math.Exp(SafeLog(node.HighNode) - correctionSizeLog);

                    if (!ShouldDeleteHitTarget(price, node.IndexNode, swUptrendType))
                    {
                        DrawTargetLine(startTime, endTime, price, lineColor, CorrectionLineWidth, styleDash,
                            trendPrefix + "Correction 1", labelColor);
                        RegisterDensity(price, swUptrendType, endTime);
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

                double totalMoveLog = Math.Abs(SafeLog(node2.HighNode) - SafeLog(node1.LowPreNode));

                if (ShowPairMin)
                {
                    double price = swUptrendType
                        ? Math.Exp(SafeLog(node1.LowCorrection) + totalMoveLog)
                        : Math.Exp(SafeLog(node1.LowCorrection) - totalMoveLog);

                    if (!ShouldDeleteHitTarget(price, node1.IndexCorrection, swUptrendType))
                    {
                        DrawTargetLine(TimeAtIndex(node1.IndexNode), endTime, price, lineColor, MinLineWidth, styleMin,
                            trendPrefix + "Min 12", labelColor);
                        RegisterDensity(price, swUptrendType, endTime);
                    }
                }

                if (ShowPairMax)
                {
                    double price = swUptrendType
                        ? Math.Exp(SafeLog(node2.LowCorrection) + totalMoveLog)
                        : Math.Exp(SafeLog(node2.LowCorrection) - totalMoveLog);

                    if (!ShouldDeleteHitTarget(price, node2.IndexCorrection, swUptrendType))
                    {
                        DrawTargetLine(TimeAtIndex(node2.IndexNode), endTime, price, lineColor, PairMaxLineWidth, styleMin,
                            trendPrefix + "Max 12", labelColor);
                        RegisterDensity(price, swUptrendType, endTime);
                    }
                }

                if (ShowPairDouble)
                {
                    double price = swUptrendType
                        ? Math.Exp(SafeLog(node2.HighNode) + totalMoveLog)
                        : Math.Exp(SafeLog(node2.HighNode) - totalMoveLog);

                    if (!ShouldDeleteHitTarget(price, node2.IndexNode, swUptrendType))
                    {
                        DrawTargetLine(TimeAtIndex(node2.IndexNode), endTime, price, lineColor, DoubleLineWidth, styleDash,
                            trendPrefix + "Double 12", labelColor);
                        RegisterDensity(price, swUptrendType, endTime);
                    }
                }

                if (ShowPairCorrection)
                {
                    double correctionSizeLog2 = Math.Abs(SafeLog(node2.HighNode) - SafeLog(node2.LowCorrection));
                    double price = swUptrendType
                        ? Math.Exp(SafeLog(node2.HighNode) + correctionSizeLog2)
                        : Math.Exp(SafeLog(node2.HighNode) - correctionSizeLog2);

                    if (!ShouldDeleteHitTarget(price, node2.IndexNode, swUptrendType))
                    {
                        DrawTargetLine(TimeAtIndex(node2.IndexNode), endTime, price, lineColor, CorrectionLineWidth, styleDash,
                            trendPrefix + "Correction 2", labelColor);
                        RegisterDensity(price, swUptrendType, endTime);
                    }
                }
            }
        }

        private void DrawTargetLine(DateTime startTime, DateTime endTime, double price, Color lineColor,
            int width, LineStyle style, string label, Color labelColor)
        {
            Chart.DrawTrendLine(NextName("Tgt"), startTime, price, endTime, price, lineColor, width, style);

            DateTime labelTime = TargetLabelTime(startTime, endTime);
            var txt = Chart.DrawText(NextName("TgtLbl"), label, labelTime, price, labelColor);
            txt.FontSize = TargetFontSizeValue();
            txt.VerticalAlignment = VerticalAlignment.Center;
            txt.HorizontalAlignment = HorizontalAlignment.Center;
        }

        // ───────────────────────── Density ─────────────────────────

        private void DrawDensityClusters()
        {
            int n = _densityPrices.Count;
            if (n < 2)
                return;

            var order = new List<int>(n);
            for (int i = 0; i < n; i++)
                order.Add(i);
            order.Sort((a, b) => _densityPrices[a].CompareTo(_densityPrices[b]));

            var used = new bool[n];

            for (int oi = 0; oi < n; oi++)
            {
                int i = order[oi];
                if (used[i])
                    continue;

                bool trendI = _densityIsUp[i];
                double runMin = _densityPrices[i];
                double runMax = runMin;
                DateTime runEndTime = _densityLabelTimes[i];
                var cluster = new List<int> { i };

                for (int oj = oi + 1; oj < n; oj++)
                {
                    int j = order[oj];
                    if (used[j])
                        continue;

                    double priceJ = _densityPrices[j];
                    double refP = Math.Max(Math.Abs(runMin), 1e-7);
                    double widthProbe = (Math.Max(runMax, priceJ) - Math.Min(runMin, priceJ)) / refP * 100.0;
                    if (widthProbe > YellowTolerancePct)
                        break;

                    if (_densityIsUp[j] != trendI)
                        continue;

                    cluster.Add(j);
                    runMin = Math.Min(runMin, priceJ);
                    runMax = Math.Max(runMax, priceJ);
                    if (_densityLabelTimes[j] > runEndTime)
                        runEndTime = _densityLabelTimes[j];
                }

                if (cluster.Count < 2)
                {
                    used[i] = true;
                    continue;
                }

                for (int k = 0; k < cluster.Count; k++)
                    used[cluster[k]] = true;

                double refW = Math.Max(Math.Abs(runMin), 1e-7);
                double widthPct = (runMax - runMin) / refW * 100.0;
                double midPrice = (runMin + runMax) / 2.0;

                bool asRed = widthPct <= RedTolerancePct;
                bool asYellow = !asRed && widthPct <= YellowTolerancePct;
                if (!asRed && !asYellow)
                    continue;

                if (asRed)
                    _redDensityCount++;
                else
                    _yellowDensityCount++;

                // Density dots at the right end of the target line.
                Color dotColor = asRed ? Color.Red : Color.Yellow;
                var circ = Chart.DrawText(NextName("Dens"), "●", runEndTime, midPrice, dotColor);
                circ.FontSize = 14;
                circ.VerticalAlignment = VerticalAlignment.Center;
                circ.HorizontalAlignment = HorizontalAlignment.Center;
            }
        }

        private void DrawStatsOverlay()
        {
            string text =
                "Red ● " + _redDensityCount +
                "\nYellow ● " + _yellowDensityCount +
                "\nUp Nodes " + _nodesUp.Count +
                "\nDn Nodes " + _nodesDown.Count;

            Chart.DrawStaticText(StatsName, text, VerticalAlignment.Top, HorizontalAlignment.Right, Color.White);
        }
    }
}
