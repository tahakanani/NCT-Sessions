// MAP Weekly (مپ هفتگی) — cTrader Automate
// Weekly levels from Mohammad Ali Poursamadi's MAP system:
// previous week High/Low, 25/50/75% retracements, and 1.25x–2x trend extensions.
using System;
using System.Collections.Generic;
using cAlgo.API;
using cAlgo.API.Internals;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class MAPWeekly : Indicator
    {
        // ───────────────────────── Key Levels ─────────────────────────

        [Parameter("Show Weekly High", DefaultValue = true, Group = "Key Levels")]
        public bool ShowHigh { get; set; }

        [Parameter("Show Weekly Low", DefaultValue = true, Group = "Key Levels")]
        public bool ShowLow { get; set; }

        [Parameter("Show 50% Mid", DefaultValue = true, Group = "Key Levels")]
        public bool ShowMid { get; set; }

        [Parameter("Show 25% Level", DefaultValue = true, Group = "Key Levels")]
        public bool Show25 { get; set; }

        [Parameter("Show 75% Level", DefaultValue = true, Group = "Key Levels")]
        public bool Show75 { get; set; }

        // ───────────────────────── Extensions ─────────────────────────

        [Parameter("Show Extensions Above High", DefaultValue = true, Group = "Trend Extensions")]
        public bool ShowExtAbove { get; set; }

        [Parameter("Show Extensions Below Low", DefaultValue = true, Group = "Trend Extensions")]
        public bool ShowExtBelow { get; set; }

        [Parameter("Show 1.25x", DefaultValue = true, Group = "Trend Extensions")]
        public bool Show125 { get; set; }

        [Parameter("Show 1.5x", DefaultValue = true, Group = "Trend Extensions")]
        public bool Show150 { get; set; }

        [Parameter("Show 1.75x", DefaultValue = true, Group = "Trend Extensions")]
        public bool Show175 { get; set; }

        [Parameter("Show 2x", DefaultValue = true, Group = "Trend Extensions")]
        public bool Show200 { get; set; }

        [Parameter("Show 1.125x (Optional)", DefaultValue = false, Group = "Trend Extensions")]
        public bool Show1125 { get; set; }

        [Parameter("Show 1.375x (Optional)", DefaultValue = false, Group = "Trend Extensions")]
        public bool Show1375 { get; set; }

        // ───────────────────────── Display ─────────────────────────

        [Parameter("Extend Lines (Bars)", DefaultValue = 150, MinValue = 0, MaxValue = 500, Group = "Display")]
        public int ExtendBars { get; set; }

        [Parameter("Show Labels", DefaultValue = true, Group = "Display")]
        public bool ShowLabels { get; set; }

        [Parameter("Label Font Size", DefaultValue = 9, MinValue = 6, MaxValue = 18, Group = "Display")]
        public int LabelFontSize { get; set; }

        [Parameter("Line Width (Key)", DefaultValue = 2, MinValue = 1, MaxValue = 5, Group = "Display")]
        public int KeyLineWidth { get; set; }

        [Parameter("Line Width (Mid/Retrace)", DefaultValue = 1, MinValue = 1, MaxValue = 5, Group = "Display")]
        public int MidLineWidth { get; set; }

        [Parameter("Line Width (Extensions)", DefaultValue = 1, MinValue = 1, MaxValue = 5, Group = "Display")]
        public int ExtLineWidth { get; set; }

        [Parameter("Key Line Style", DefaultValue = "Solid", Group = "Display")]
        public string KeyLineStyleName { get; set; }

        [Parameter("Mid Line Style", DefaultValue = "Dashed", Group = "Display")]
        public string MidLineStyleName { get; set; }

        [Parameter("Extension Line Style", DefaultValue = "Dotted", Group = "Display")]
        public string ExtLineStyleName { get; set; }

        // ───────────────────────── Colors ─────────────────────────

        [Parameter("High Color", DefaultValue = "#FF4D6D", Group = "Colors")]
        public string HighColorName { get; set; }

        [Parameter("Low Color", DefaultValue = "#7CFF47", Group = "Colors")]
        public string LowColorName { get; set; }

        [Parameter("Mid Color", DefaultValue = "#FFE566", Group = "Colors")]
        public string MidColorName { get; set; }

        [Parameter("25/75% Color", DefaultValue = "#7AA2FF", Group = "Colors")]
        public string RetraceColorName { get; set; }

        [Parameter("Extension Color", DefaultValue = "#C084FC", Group = "Colors")]
        public string ExtColorName { get; set; }

        // ───────────────────────── State ─────────────────────────

        private Bars _weeklyBars;
        private int _objSeq;
        private string _lastDrawSignature;
        private int _lastDrawBarIndex = -1;
        private bool _rebuilding;

        private const string ObjectPrefix = "MAPW_";

        protected override void Initialize()
        {
            try { _weeklyBars = MarketData.GetBars(TimeFrame.Weekly); } catch { }
            try { Timer.Start(TimeSpan.FromSeconds(1)); } catch { }
        }

        protected override void OnDestroy()
        {
            try { Timer.Stop(); } catch { }
            RemoveDrawings();
        }

        protected override void OnTimer()
        {
            try
            {
                if (Bars == null || Bars.Count < 2)
                    return;
                if (_lastDrawBarIndex == Bars.Count - 1)
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
                if (lastIndex < 1 || index != lastIndex)
                    return;
                RebuildAndDraw();
            }
            catch { }
        }

        private void RebuildAndDraw()
        {
            if (_rebuilding)
                return;
            _rebuilding = true;

            try
            {
                int lastIndex = Bars.Count - 1;
                if (lastIndex < 1)
                    return;

                if (_weeklyBars == null)
                {
                    try { _weeklyBars = MarketData.GetBars(TimeFrame.Weekly); } catch { return; }
                }

                if (_weeklyBars == null || _weeklyBars.Count < 2)
                    return;

                WeeklyData week = GetPreviousWeekData();
                if (week == null)
                    return;

                string signature = BuildDrawSignature(lastIndex, week);
                if (_lastDrawBarIndex == lastIndex && signature == _lastDrawSignature)
                    return;

                RemoveDrawings();
                _objSeq = 0;

                DateTime lineStart = FindCurrentWeekStart(week.WeekOpenTime);
                DateTime lineEnd = TimeAtIndex(lastIndex + Math.Max(0, ExtendBars));

                double range = week.High - week.Low;
                if (range <= 0 || double.IsNaN(range) || double.IsInfinity(range))
                    return;

                Color highColor = ParseColor(HighColorName, Color.FromArgb(255, 255, 77, 109));
                Color lowColor = ParseColor(LowColorName, Color.FromArgb(255, 124, 255, 71));
                Color midColor = ParseColor(MidColorName, Color.FromArgb(255, 255, 229, 102));
                Color retraceColor = ParseColor(RetraceColorName, Color.FromArgb(255, 122, 162, 255));
                Color extColor = ParseColor(ExtColorName, Color.FromArgb(255, 192, 132, 252));

                LineStyle keyStyle = ParseLineStyle(KeyLineStyleName, LineStyle.Solid);
                LineStyle midStyle = ParseLineStyle(MidLineStyleName, LineStyle.Dots);
                LineStyle extStyle = ParseLineStyle(ExtLineStyleName, LineStyle.DotsRare);

                double mid = (week.High + week.Low) / 2.0;
                double level25 = week.Low + range * 0.25;
                double level75 = week.Low + range * 0.75;

                if (ShowHigh)
                    DrawLevel(lineStart, lineEnd, week.High, "PW High", highColor, KeyLineWidth, keyStyle);

                if (ShowLow)
                    DrawLevel(lineStart, lineEnd, week.Low, "PW Low", lowColor, KeyLineWidth, keyStyle);

                if (ShowMid)
                    DrawLevel(lineStart, lineEnd, mid, "50%", midColor, MidLineWidth, midStyle);

                if (Show25)
                    DrawLevel(lineStart, lineEnd, level25, "25%", retraceColor, MidLineWidth, midStyle);

                if (Show75)
                    DrawLevel(lineStart, lineEnd, level75, "75%", retraceColor, MidLineWidth, midStyle);

                if (ShowExtAbove)
                {
                    if (Show1125)
                        DrawLevel(lineStart, lineEnd, week.High + range * 0.125, "1.125x", extColor, ExtLineWidth, extStyle);
                    if (Show125)
                        DrawLevel(lineStart, lineEnd, week.High + range * 0.25, "1.25x", extColor, ExtLineWidth, extStyle);
                    if (Show1375)
                        DrawLevel(lineStart, lineEnd, week.High + range * 0.375, "1.375x", extColor, ExtLineWidth, extStyle);
                    if (Show150)
                        DrawLevel(lineStart, lineEnd, week.High + range * 0.50, "1.5x", extColor, ExtLineWidth, extStyle);
                    if (Show175)
                        DrawLevel(lineStart, lineEnd, week.High + range * 0.75, "1.75x", extColor, ExtLineWidth, extStyle);
                    if (Show200)
                        DrawLevel(lineStart, lineEnd, week.High + range * 1.00, "2x", extColor, ExtLineWidth, extStyle);
                }

                if (ShowExtBelow)
                {
                    if (Show1125)
                        DrawLevel(lineStart, lineEnd, week.Low - range * 0.125, "1.125x", extColor, ExtLineWidth, extStyle);
                    if (Show125)
                        DrawLevel(lineStart, lineEnd, week.Low - range * 0.25, "1.25x", extColor, ExtLineWidth, extStyle);
                    if (Show1375)
                        DrawLevel(lineStart, lineEnd, week.Low - range * 0.375, "1.375x", extColor, ExtLineWidth, extStyle);
                    if (Show150)
                        DrawLevel(lineStart, lineEnd, week.Low - range * 0.50, "1.5x", extColor, ExtLineWidth, extStyle);
                    if (Show175)
                        DrawLevel(lineStart, lineEnd, week.Low - range * 0.75, "1.75x", extColor, ExtLineWidth, extStyle);
                    if (Show200)
                        DrawLevel(lineStart, lineEnd, week.Low - range * 1.00, "2x", extColor, ExtLineWidth, extStyle);
                }

                _lastDrawSignature = signature;
                _lastDrawBarIndex = lastIndex;
            }
            finally
            {
                _rebuilding = false;
            }
        }

        private sealed class WeeklyData
        {
            public double High;
            public double Low;
            public DateTime WeekOpenTime;
        }

        private WeeklyData GetPreviousWeekData()
        {
            // Last weekly bar is the current (incomplete) week; use the one before it.
            int prevIdx = _weeklyBars.Count - 2;
            if (prevIdx < 0)
                return null;

            var bar = _weeklyBars[prevIdx];
            double high = bar.High;
            double low = bar.Low;

            if (high <= 0 || low <= 0 || double.IsNaN(high) || double.IsNaN(low))
                return null;

            return new WeeklyData
            {
                High = high,
                Low = low,
                WeekOpenTime = _weeklyBars.OpenTimes[_weeklyBars.Count - 1]
            };
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

        private void DrawLevel(DateTime startTime, DateTime endTime, double price, string label,
            Color color, int width, LineStyle style)
        {
            if (double.IsNaN(price) || double.IsInfinity(price))
                return;

            Chart.DrawTrendLine(NextName("Ln"), startTime, price, endTime, price, color, width, style);

            if (!ShowLabels)
                return;

            DateTime labelTime = new DateTime(
                startTime.Ticks + (endTime.Ticks - startTime.Ticks) / 2,
                startTime.Kind);

            var txt = Chart.DrawText(NextName("Lbl"), label, labelTime, price, color);
            txt.FontSize = Math.Max(6, Math.Min(LabelFontSize, 18));
            txt.VerticalAlignment = VerticalAlignment.Center;
            txt.HorizontalAlignment = HorizontalAlignment.Center;
        }

        private string BuildDrawSignature(int lastIndex, WeeklyData week)
        {
            return lastIndex + "|" +
                   week.High.ToString("R") + "|" +
                   week.Low.ToString("R") + "|" +
                   week.WeekOpenTime.Ticks + "|" +
                   ShowHigh + ShowLow + ShowMid + Show25 + Show75 +
                   ShowExtAbove + ShowExtBelow +
                   Show1125 + Show125 + Show1375 + Show150 + Show175 + Show200 +
                   ExtendBars + ShowLabels + LabelFontSize +
                   KeyLineWidth + MidLineWidth + ExtLineWidth;
        }

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
        }

        private string NextName(string kind)
        {
            _objSeq++;
            return ObjectPrefix + kind + "_" + _objSeq;
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

        private static LineStyle ParseLineStyle(string name, LineStyle fallback)
        {
            if (string.IsNullOrWhiteSpace(name))
                return fallback;

            switch (name.Trim().ToLowerInvariant())
            {
                case "solid": return LineStyle.Solid;
                case "dashed":
                case "dots": return LineStyle.Dots;
                case "dotted":
                case "dotsrare":
                case "dots rare": return LineStyle.DotsRare;
                default: return fallback;
            }
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
                case "orange": return Color.FromArgb(255, 255, 159, 28);
                case "blue": return Color.FromArgb(255, 122, 162, 255);
                case "red":
                case "rose": return Color.FromArgb(255, 255, 77, 109);
                case "violet":
                case "purple": return Color.FromArgb(255, 192, 132, 252);
                case "gray":
                case "grey": return Color.Gray;
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
    }
}
