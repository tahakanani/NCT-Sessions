// SessionWindowsPro — cTrader Automate
// Port of MetaTrader SessionWindowsPro 1.21: 11 graded time windows,
// time-point lines, day-start line, and a live countdown panel.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using cAlgo.API;
using cAlgo.API.Internals;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class SessionWindowsPro : Indicator
    {
        // ───────────────────────── General ─────────────────────────

        [Parameter("Teaching view", DefaultValue = "Full map (all 11)", Group = "General")]
        public string TeachingView { get; set; }

        [Parameter("Days back to draw", DefaultValue = 2, MinValue = 1, MaxValue = 30, Group = "General")]
        public int DaysBack { get; set; }

        [Parameter("Shift all times (minutes, +/-)", DefaultValue = -30, MinValue = -720, MaxValue = 720, Group = "General")]
        public int ShiftMinutes { get; set; }

        [Parameter("Hide windows with no bars (weekends)", DefaultValue = true, Group = "General")]
        public bool HideEmptyWindows { get; set; }

        [Parameter("Draw mode", DefaultValue = "Edge lines only", Group = "General")]
        public string DrawModeName { get; set; }

        [Parameter("Frame width", DefaultValue = 1, MinValue = 1, MaxValue = 5, Group = "General")]
        public int FrameWidth { get; set; }

        [Parameter("Vertical line at each window start", DefaultValue = false, Group = "General")]
        public bool VerticalAtWindowStart { get; set; }

        [Parameter("Object name prefix", DefaultValue = "SWP_", Group = "General")]
        public string ObjectNamePrefix { get; set; }

        // ───────────────────────── Grades ─────────────────────────

        [Parameter("Softness % of grade-A windows", DefaultValue = 72, MinValue = 0, MaxValue = 100, Group = "Grades")]
        public int SoftnessA { get; set; }

        [Parameter("Softness % of grade-B windows", DefaultValue = 84, MinValue = 0, MaxValue = 100, Group = "Grades")]
        public int SoftnessB { get; set; }

        [Parameter("Softness % of grade-C windows", DefaultValue = 92, MinValue = 0, MaxValue = 100, Group = "Grades")]
        public int SoftnessC { get; set; }

        // ───────────────────────── Labels ─────────────────────────

        [Parameter("Show time-range labels", DefaultValue = false, Group = "Labels")]
        public bool ShowTimeRangeLabels { get; set; }

        [Parameter("Label position", DefaultValue = "Top of the chart", Group = "Labels")]
        public string LabelPositionName { get; set; }

        [Parameter("Label colour (when not per-zone)", DefaultValue = "170,175,185", Group = "Labels")]
        public string LabelColourName { get; set; }

        [Parameter("Label font size", DefaultValue = 8, MinValue = 6, MaxValue = 24, Group = "Labels")]
        public int LabelFontSize { get; set; }

        [Parameter("Colour label like its zone", DefaultValue = true, Group = "Labels")]
        public bool ColourLabelLikeZone { get; set; }

        [Parameter("Stagger labels (avoid overlap)", DefaultValue = true, Group = "Labels")]
        public bool StaggerLabels { get; set; }

        [Parameter("Append grade to label (A/B/C)", DefaultValue = true, Group = "Labels")]
        public bool AppendGradeToLabel { get; set; }

        // ───────────────────────── Time points ─────────────────────────

        [Parameter("Show time-point lines", DefaultValue = true, Group = "Time Points")]
        public bool ShowTimePointLines { get; set; }

        [Parameter("Time points (HH:MM, comma separated)", DefaultValue = "03:00, 04:00, 08:00, 09:00, 10:00, 13:00, 15:30, 16:30, 18:00, 20:00, 21:00, 23:00", Group = "Time Points")]
        public string TimePointsList { get; set; }

        [Parameter("Time-point line colour", DefaultValue = "232,200,110", Group = "Time Points")]
        public string TimePointColourName { get; set; }

        [Parameter("Time-point line style", DefaultValue = "Dot", Group = "Time Points")]
        public string TimePointStyleName { get; set; }

        [Parameter("Time-point line width", DefaultValue = 1, MinValue = 1, MaxValue = 5, Group = "Time Points")]
        public int TimePointWidth { get; set; }

        [Parameter("Show HH:MM tag on each time-point", DefaultValue = true, Group = "Time Points")]
        public bool ShowTimePointTags { get; set; }

        // ───────────────────────── Day start ─────────────────────────

        [Parameter("Show day-start line at 00:00", DefaultValue = true, Group = "Day Start")]
        public bool ShowDayStartLine { get; set; }

        [Parameter("Day-start line colour", DefaultValue = "100,105,122", Group = "Day Start")]
        public string DayStartColourName { get; set; }

        [Parameter("Day-start line style", DefaultValue = "Dash", Group = "Day Start")]
        public string DayStartStyleName { get; set; }

        [Parameter("Day-start line width", DefaultValue = 1, MinValue = 1, MaxValue = 5, Group = "Day Start")]
        public int DayStartWidth { get; set; }

        [Parameter("Show 00:00 tag on it", DefaultValue = true, Group = "Day Start")]
        public bool ShowDayStartTag { get; set; }

        // ───────────────────────── Live panel ─────────────────────────

        [Parameter("Show current-window / countdown panel", DefaultValue = true, Group = "Live Panel")]
        public bool ShowLivePanel { get; set; }

        [Parameter("Corner (0=TL 1=TR 2=BL 3=BR)", DefaultValue = 1, MinValue = 0, MaxValue = 3, Group = "Live Panel")]
        public int PanelCorner { get; set; }

        [Parameter("X offset (px)", DefaultValue = 12, MinValue = 0, MaxValue = 200, Group = "Live Panel")]
        public int PanelXOffset { get; set; }

        [Parameter("Y offset (px)", DefaultValue = 22, MinValue = 0, MaxValue = 200, Group = "Live Panel")]
        public int PanelYOffset { get; set; }

        [Parameter("Panel font size", DefaultValue = 10, MinValue = 6, MaxValue = 24, Group = "Live Panel")]
        public int PanelFontSize { get; set; }

        [Parameter("Warn when next TP closer than (minutes)", DefaultValue = 5, MinValue = 0, MaxValue = 180, Group = "Live Panel")]
        public int WarnMinutes { get; set; }

        // ───────────────────────── Window 01 ─────────────────────────

        [Parameter("Enabled", DefaultValue = true, Group = "Window 01")]
        public bool W01Enabled { get; set; }

        [Parameter("Start (HH:MM)", DefaultValue = "01:00", Group = "Window 01")]
        public string W01Start { get; set; }

        [Parameter("End (HH:MM)", DefaultValue = "02:30", Group = "Window 01")]
        public string W01End { get; set; }

        [Parameter("Colour", DefaultValue = "82,88,105", Group = "Window 01")]
        public string W01Colour { get; set; }

        [Parameter("Grade (1=A hot 2=B 3=C)", DefaultValue = 3, MinValue = 1, MaxValue = 3, Group = "Window 01")]
        public int W01Grade { get; set; }

        // ───────────────────────── Window 02 ─────────────────────────

        [Parameter("Enabled", DefaultValue = true, Group = "Window 02")]
        public bool W02Enabled { get; set; }

        [Parameter("Start (HH:MM)", DefaultValue = "02:30", Group = "Window 02")]
        public string W02Start { get; set; }

        [Parameter("End (HH:MM)", DefaultValue = "03:00", Group = "Window 02")]
        public string W02End { get; set; }

        [Parameter("Colour", DefaultValue = "0,178,190", Group = "Window 02")]
        public string W02Colour { get; set; }

        [Parameter("Grade (1=A hot 2=B 3=C)", DefaultValue = 1, MinValue = 1, MaxValue = 3, Group = "Window 02")]
        public int W02Grade { get; set; }

        // ───────────────────────── Window 03 ─────────────────────────

        [Parameter("Enabled", DefaultValue = true, Group = "Window 03")]
        public bool W03Enabled { get; set; }

        [Parameter("Start (HH:MM)", DefaultValue = "03:00", Group = "Window 03")]
        public string W03Start { get; set; }

        [Parameter("End (HH:MM)", DefaultValue = "07:00", Group = "Window 03")]
        public string W03End { get; set; }

        [Parameter("Colour", DefaultValue = "52,120,100", Group = "Window 03")]
        public string W03Colour { get; set; }

        [Parameter("Grade (1=A hot 2=B 3=C)", DefaultValue = 2, MinValue = 1, MaxValue = 3, Group = "Window 03")]
        public int W03Grade { get; set; }

        // ───────────────────────── Window 04 ─────────────────────────

        [Parameter("Enabled", DefaultValue = true, Group = "Window 04")]
        public bool W04Enabled { get; set; }

        [Parameter("Start (HH:MM)", DefaultValue = "07:00", Group = "Window 04")]
        public string W04Start { get; set; }

        [Parameter("End (HH:MM)", DefaultValue = "10:00", Group = "Window 04")]
        public string W04End { get; set; }

        [Parameter("Colour", DefaultValue = "128,116,52", Group = "Window 04")]
        public string W04Colour { get; set; }

        [Parameter("Grade (1=A hot 2=B 3=C)", DefaultValue = 2, MinValue = 1, MaxValue = 3, Group = "Window 04")]
        public int W04Grade { get; set; }

        // ───────────────────────── Window 05 ─────────────────────────

        [Parameter("Enabled", DefaultValue = true, Group = "Window 05")]
        public bool W05Enabled { get; set; }

        [Parameter("Start (HH:MM)", DefaultValue = "10:00", Group = "Window 05")]
        public string W05Start { get; set; }

        [Parameter("End (HH:MM)", DefaultValue = "13:00", Group = "Window 05")]
        public string W05End { get; set; }

        [Parameter("Colour", DefaultValue = "255,178,44", Group = "Window 05")]
        public string W05Colour { get; set; }

        [Parameter("Grade (1=A hot 2=B 3=C)", DefaultValue = 1, MinValue = 1, MaxValue = 3, Group = "Window 05")]
        public int W05Grade { get; set; }

        // ───────────────────────── Window 06 ─────────────────────────

        [Parameter("Enabled", DefaultValue = true, Group = "Window 06")]
        public bool W06Enabled { get; set; }

        [Parameter("Start (HH:MM)", DefaultValue = "13:00", Group = "Window 06")]
        public string W06Start { get; set; }

        [Parameter("End (HH:MM)", DefaultValue = "15:30", Group = "Window 06")]
        public string W06End { get; set; }

        [Parameter("Colour", DefaultValue = "98,88,74", Group = "Window 06")]
        public string W06Colour { get; set; }

        [Parameter("Grade (1=A hot 2=B 3=C)", DefaultValue = 3, MinValue = 1, MaxValue = 3, Group = "Window 06")]
        public int W06Grade { get; set; }

        // ───────────────────────── Window 07 ─────────────────────────

        [Parameter("Enabled", DefaultValue = true, Group = "Window 07")]
        public bool W07Enabled { get; set; }

        [Parameter("Start (HH:MM)", DefaultValue = "15:30", Group = "Window 07")]
        public string W07Start { get; set; }

        [Parameter("End (HH:MM)", DefaultValue = "16:30", Group = "Window 07")]
        public string W07End { get; set; }

        [Parameter("Colour", DefaultValue = "236,110,60", Group = "Window 07")]
        public string W07Colour { get; set; }

        [Parameter("Grade (1=A hot 2=B 3=C)", DefaultValue = 3, MinValue = 1, MaxValue = 3, Group = "Window 07")]
        public int W07Grade { get; set; }

        // ───────────────────────── Window 08 ─────────────────────────

        [Parameter("Enabled", DefaultValue = true, Group = "Window 08")]
        public bool W08Enabled { get; set; }

        [Parameter("Start (HH:MM)", DefaultValue = "16:30", Group = "Window 08")]
        public string W08Start { get; set; }

        [Parameter("End (HH:MM)", DefaultValue = "18:00", Group = "Window 08")]
        public string W08End { get; set; }

        [Parameter("Colour", DefaultValue = "244,70,96", Group = "Window 08")]
        public string W08Colour { get; set; }

        [Parameter("Grade (1=A hot 2=B 3=C)", DefaultValue = 1, MinValue = 1, MaxValue = 3, Group = "Window 08")]
        public int W08Grade { get; set; }

        // ───────────────────────── Window 09 ─────────────────────────

        [Parameter("Enabled", DefaultValue = true, Group = "Window 09")]
        public bool W09Enabled { get; set; }

        [Parameter("Start (HH:MM)", DefaultValue = "18:00", Group = "Window 09")]
        public string W09Start { get; set; }

        [Parameter("End (HH:MM)", DefaultValue = "21:00", Group = "Window 09")]
        public string W09End { get; set; }

        [Parameter("Colour", DefaultValue = "74,134,224", Group = "Window 09")]
        public string W09Colour { get; set; }

        [Parameter("Grade (1=A hot 2=B 3=C)", DefaultValue = 1, MinValue = 1, MaxValue = 3, Group = "Window 09")]
        public int W09Grade { get; set; }

        // ───────────────────────── Window 10 ─────────────────────────

        [Parameter("Enabled", DefaultValue = true, Group = "Window 10")]
        public bool W10Enabled { get; set; }

        [Parameter("Start (HH:MM)", DefaultValue = "21:00", Group = "Window 10")]
        public string W10Start { get; set; }

        [Parameter("End (HH:MM)", DefaultValue = "23:00", Group = "Window 10")]
        public string W10End { get; set; }

        [Parameter("Colour", DefaultValue = "118,96,208", Group = "Window 10")]
        public string W10Colour { get; set; }

        [Parameter("Grade (1=A hot 2=B 3=C)", DefaultValue = 2, MinValue = 1, MaxValue = 3, Group = "Window 10")]
        public int W10Grade { get; set; }

        // ───────────────────────── Window 11 ─────────────────────────

        [Parameter("Enabled", DefaultValue = true, Group = "Window 11")]
        public bool W11Enabled { get; set; }

        [Parameter("Start (HH:MM)", DefaultValue = "23:00", Group = "Window 11")]
        public string W11Start { get; set; }

        [Parameter("End (HH:MM)", DefaultValue = "00:00", Group = "Window 11")]
        public string W11End { get; set; }

        [Parameter("Colour", DefaultValue = "96,66,124", Group = "Window 11")]
        public string W11Colour { get; set; }

        [Parameter("Grade (1=A hot 2=B 3=C)", DefaultValue = 2, MinValue = 1, MaxValue = 3, Group = "Window 11")]
        public int W11Grade { get; set; }

        // ───────────────────────── State ─────────────────────────

        private int _objSeq;
        private string _lastDrawSignature;
        private int _lastDrawBarIndex = -1;
        private bool _rebuilding;
        private string _activePrefix = "SWP_";

        private const string PanelObjectName = "Panel";

        // ───────────────────────── Lifecycle ─────────────────────────

        protected override void Initialize()
        {
            _activePrefix = NormalizePrefix(ObjectNamePrefix);
            try { Timer.Start(TimeSpan.FromSeconds(1)); } catch { }
        }

        protected override void OnDestroy()
        {
            try { Timer.Stop(); } catch { }
            RemoveDrawings(true);
        }

        protected override void OnTimer()
        {
            try
            {
                if (Bars == null || Bars.Count < 2)
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

                _activePrefix = NormalizePrefix(ObjectNamePrefix);

                WindowDef[] windows = CollectWindows();
                int[] timePoints = SessionWindowsLogic.ParseTimePoints(TimePointsList);
                string signature = BuildDrawSignature(lastIndex, windows, timePoints);

                if (_lastDrawBarIndex != lastIndex || signature != _lastDrawSignature)
                {
                    RemoveDrawings(false);
                    _objSeq = 0;
                    DrawWindowsAndLines(lastIndex, windows, timePoints);
                    _lastDrawSignature = signature;
                    _lastDrawBarIndex = lastIndex;
                }

                DrawLivePanel(windows, timePoints);
            }
            finally
            {
                _rebuilding = false;
            }
        }

        // ───────────────────────── Collect / filter ─────────────────────────

        private WindowDef[] CollectWindows()
        {
            return new[]
            {
                new WindowDef(1, W01Enabled, W01Start, W01End, W01Colour, W01Grade),
                new WindowDef(2, W02Enabled, W02Start, W02End, W02Colour, W02Grade),
                new WindowDef(3, W03Enabled, W03Start, W03End, W03Colour, W03Grade),
                new WindowDef(4, W04Enabled, W04Start, W04End, W04Colour, W04Grade),
                new WindowDef(5, W05Enabled, W05Start, W05End, W05Colour, W05Grade),
                new WindowDef(6, W06Enabled, W06Start, W06End, W06Colour, W06Grade),
                new WindowDef(7, W07Enabled, W07Start, W07End, W07Colour, W07Grade),
                new WindowDef(8, W08Enabled, W08Start, W08End, W08Colour, W08Grade),
                new WindowDef(9, W09Enabled, W09Start, W09End, W09Colour, W09Grade),
                new WindowDef(10, W10Enabled, W10Start, W10End, W10Colour, W10Grade),
                new WindowDef(11, W11Enabled, W11Start, W11End, W11Colour, W11Grade)
            };
        }

        private bool WindowVisible(WindowDef window)
        {
            if (window == null || !window.Enabled)
                return false;
            if (window.StartMinutes < 0 || window.EndMinutes < 0)
                return false;
            if (window.StartMinutes == window.EndMinutes)
                return false;

            int maxGrade = SessionWindowsLogic.MaxGradeForView(TeachingView);
            return window.Grade <= maxGrade;
        }

        // ───────────────────────── Drawing ─────────────────────────

        private void DrawWindowsAndLines(int lastIndex, WindowDef[] windows, int[] timePoints)
        {
            double top;
            double bottom;
            GetChartBounds(lastIndex, out top, out bottom);
            if (top <= bottom)
                return;

            DateTime lastOpen = Bars.OpenTimes[lastIndex];
            DateTime lastDay = lastOpen.Date;
            int days = Math.Max(1, Math.Min(30, DaysBack));
            DateTime firstDay = lastDay.AddDays(1 - days);

            DrawMode mode = ParseDrawMode(DrawModeName);
            Color labelFallback = ParseRgbColor(LabelColourName, Color.FromArgb(255, 170, 175, 185));
            bool labelAtTop = IsTopLabel(LabelPositionName);
            int labelSize = Math.Max(6, Math.Min(24, LabelFontSize));
            int frameW = Math.Max(1, Math.Min(5, FrameWidth));

            int staggerSlot = 0;
            for (DateTime day = firstDay; day <= lastDay; day = day.AddDays(1))
            {
                if (HideEmptyWindows && !DayHasBars(day))
                    continue;

                for (int w = 0; w < windows.Length; w++)
                {
                    WindowDef window = windows[w];
                    if (!WindowVisible(window))
                        continue;

                    DateTime startTime;
                    DateTime endTime;
                    SessionWindowsLogic.WindowOnDay(day, window.StartMinutes, window.EndMinutes, ShiftMinutes,
                        out startTime, out endTime);

                    if (HideEmptyWindows && !HasBarsInRange(startTime, endTime))
                        continue;

                    int r, g, b;
                    if (!SessionWindowsLogic.TryParseRgb(window.ColourName, out r, out g, out b))
                    {
                        r = 128;
                        g = 128;
                        b = 128;
                    }
                    int alpha = SessionWindowsLogic.AlphaFromSoftness(SoftnessForGrade(window.Grade));
                    Color fillColor = Color.FromArgb(alpha, r, g, b);
                    Color edgeColor = Color.FromArgb(Math.Max(alpha, 70), r, g, b);

                    if (mode == DrawMode.Filled || mode == DrawMode.FilledAndEdges)
                    {
                        var rect = Chart.DrawRectangle(NextName("Box"), startTime, top, endTime, bottom, fillColor);
                        rect.IsFilled = true;
                        rect.Color = fillColor;
                        rect.Thickness = mode == DrawMode.FilledAndEdges ? frameW : 0;
                    }

                    if (mode == DrawMode.EdgesOnly || mode == DrawMode.FilledAndEdges)
                    {
                        Chart.DrawTrendLine(NextName("L"), startTime, bottom, startTime, top, edgeColor, frameW, LineStyle.Solid);
                        Chart.DrawTrendLine(NextName("R"), endTime, bottom, endTime, top, edgeColor, frameW, LineStyle.Solid);
                        Chart.DrawTrendLine(NextName("T"), startTime, top, endTime, top, edgeColor, frameW, LineStyle.Solid);
                        Chart.DrawTrendLine(NextName("B"), startTime, bottom, endTime, bottom, edgeColor, frameW, LineStyle.Solid);
                    }

                    if (VerticalAtWindowStart)
                    {
                        Chart.DrawTrendLine(NextName("VS"), startTime, bottom, startTime, top, edgeColor, frameW, LineStyle.Solid);
                    }

                    if (ShowTimeRangeLabels)
                    {
                        string label = SessionWindowsLogic.FormatWindowLabel(
                            window.StartMinutes, window.EndMinutes, ShiftMinutes,
                            window.Grade, AppendGradeToLabel);

                        Color labelColor = ColourLabelLikeZone ? edgeColor : labelFallback;
                        double labelPrice = labelAtTop ? top : bottom;
                        if (StaggerLabels)
                        {
                            double span = top - bottom;
                            int slot = staggerSlot % 4;
                            double offset = span * (0.012 + slot * 0.018);
                            labelPrice = labelAtTop ? top - offset : bottom + offset;
                            staggerSlot++;
                        }

                        DateTime mid = SessionWindowsLogic.MidTime(startTime, endTime);
                        var txt = Chart.DrawText(NextName("Lbl"), label, mid, labelPrice, labelColor);
                        txt.FontSize = labelSize;
                        txt.HorizontalAlignment = HorizontalAlignment.Center;
                        txt.VerticalAlignment = labelAtTop ? VerticalAlignment.Top : VerticalAlignment.Bottom;
                    }
                }

                DrawDayLines(day, lastDay, timePoints, top, bottom);
            }
        }

        private void DrawDayLines(DateTime day, DateTime lastDay, int[] timePoints, double top, double bottom)
        {
            Color tpColor = ParseRgbColor(TimePointColourName, Color.FromArgb(255, 232, 200, 110));
            Color dsColor = ParseRgbColor(DayStartColourName, Color.FromArgb(255, 100, 105, 122));
            LineStyle tpStyle = ParseLineStyle(TimePointStyleName, LineStyle.DotsRare);
            LineStyle dsStyle = ParseLineStyle(DayStartStyleName, LineStyle.Dots);
            int tpWidth = Math.Max(1, Math.Min(5, TimePointWidth));
            int dsWidth = Math.Max(1, Math.Min(5, DayStartWidth));
            int tagSize = Math.Max(6, Math.Min(24, LabelFontSize));

            if (ShowDayStartLine)
            {
                DateTime midnight = SessionWindowsLogic.TimeOnDay(day, 0, ShiftMinutes);
                if (midnight.Date >= day.AddDays(-1) && midnight <= lastDay.AddDays(1))
                {
                    Chart.DrawTrendLine(NextName("DS"), midnight, bottom, midnight, top, dsColor, dsWidth, dsStyle);
                    if (ShowDayStartTag)
                    {
                        string tag = SessionWindowsLogic.FormatHhMm(SessionWindowsLogic.ShiftWrap(0, ShiftMinutes));
                        var txt = Chart.DrawText(NextName("DSTag"), tag, midnight, top, dsColor);
                        txt.FontSize = tagSize;
                        txt.HorizontalAlignment = HorizontalAlignment.Center;
                        txt.VerticalAlignment = VerticalAlignment.Top;
                    }
                }
            }

            if (!ShowTimePointLines || timePoints == null)
                return;

            for (int i = 0; i < timePoints.Length; i++)
            {
                DateTime when = SessionWindowsLogic.TimeOnDay(day, timePoints[i], ShiftMinutes);
                Chart.DrawTrendLine(NextName("TP"), when, bottom, when, top, tpColor, tpWidth, tpStyle);
                if (ShowTimePointTags)
                {
                    string tag = SessionWindowsLogic.FormatHhMm(SessionWindowsLogic.ShiftWrap(timePoints[i], ShiftMinutes));
                    var txt = Chart.DrawText(NextName("TPTag"), tag, when, top, tpColor);
                    txt.FontSize = tagSize;
                    txt.HorizontalAlignment = HorizontalAlignment.Center;
                    txt.VerticalAlignment = VerticalAlignment.Top;
                }
            }
        }

        private void DrawLivePanel(WindowDef[] windows, int[] timePoints)
        {
            string name = _activePrefix + PanelObjectName;
            if (!ShowLivePanel)
            {
                Chart.RemoveObject(name);
                return;
            }

            DateTime now = Server != null ? Server.Time : Bars.OpenTimes[Bars.Count - 1];
            int nowMin = now.Hour * 60 + now.Minute;
            int shift = ShiftMinutes;

            WindowDef current = null;
            int remain = -1;
            for (int i = 0; i < windows.Length; i++)
            {
                WindowDef window = windows[i];
                if (!WindowVisible(window))
                    continue;
                if (!SessionWindowsLogic.IsInWindow(nowMin, window.StartMinutes, window.EndMinutes, shift))
                    continue;

                current = window;
                remain = SessionWindowsLogic.MinutesUntilEnd(nowMin, window.StartMinutes, window.EndMinutes, shift);
                break;
            }

            int nextTp = SessionWindowsLogic.MinutesUntilNextTimePoint(nowMin, timePoints, shift);
            bool warn = WarnMinutes > 0 && nextTp >= 0 && nextTp <= WarnMinutes;

            var sb = new StringBuilder();
            int yPad = Math.Max(0, PanelYOffset / 14);
            for (int i = 0; i < yPad; i++)
                sb.Append('\n');
            int xPad = Math.Max(0, PanelXOffset / 8);
            string indent = xPad > 0 ? new string(' ', xPad) : "";

            if (current != null)
            {
                string range = SessionWindowsLogic.FormatWindowLabel(
                    current.StartMinutes, current.EndMinutes, shift, current.Grade, AppendGradeToLabel);
                sb.Append(indent).Append("NOW  W").Append(current.Index.ToString("00")).Append("  ").Append(range).Append('\n');
                sb.Append(indent).Append("ends in  ").Append(SessionWindowsLogic.FormatDuration(remain)).Append('\n');
            }
            else
            {
                sb.Append(indent).Append("NOW  (between windows)").Append('\n');
            }

            if (nextTp >= 0)
            {
                int nextAbs = SessionWindowsLogic.NextTimePointAbsolute(nowMin, timePoints, shift);
                sb.Append(indent);
                if (warn)
                    sb.Append("WARN  ");
                sb.Append("next TP  ").Append(SessionWindowsLogic.FormatHhMm(nextAbs));
                sb.Append("  in  ").Append(SessionWindowsLogic.FormatDuration(nextTp));
            }
            else
            {
                sb.Append(indent).Append("next TP  —");
            }

            VerticalAlignment vAlign;
            HorizontalAlignment hAlign;
            CornerAlign(PanelCorner, out vAlign, out hAlign);
            Color panelColor = warn
                ? Color.FromArgb(255, 255, 196, 72)
                : Color.FromArgb(255, 230, 232, 238);

            Chart.DrawStaticText(name, sb.ToString(), vAlign, hAlign, panelColor);
        }

        private bool DayHasBars(DateTime day)
        {
            DateTime start = day.Date;
            DateTime next = start.AddDays(1);
            for (int i = 0; i < Bars.Count; i++)
            {
                DateTime t = Bars.OpenTimes[i];
                if (t >= next)
                    break;
                if (t >= start)
                    return true;
            }

            return false;
        }

        private bool HasBarsInRange(DateTime startTime, DateTime endTime)
        {
            if (Bars == null || Bars.Count == 0)
                return false;

            for (int i = 0; i < Bars.Count; i++)
            {
                DateTime t = Bars.OpenTimes[i];
                if (t < startTime)
                    continue;
                if (t >= endTime)
                    break;
                return true;
            }

            return false;
        }

        private void GetChartBounds(int lastIndex, out double top, out double bottom)
        {
            top = 0;
            bottom = 0;
            try
            {
                top = Chart.TopY;
                bottom = Chart.BottomY;
            }
            catch
            {
                top = 0;
                bottom = 0;
            }

            if (top > bottom && !double.IsNaN(top) && !double.IsNaN(bottom))
                return;

            int from = Math.Max(0, lastIndex - 500);
            top = Bars.HighPrices[from];
            bottom = Bars.LowPrices[from];
            for (int i = from + 1; i <= lastIndex; i++)
            {
                if (Bars.HighPrices[i] > top)
                    top = Bars.HighPrices[i];
                if (Bars.LowPrices[i] < bottom)
                    bottom = Bars.LowPrices[i];
            }

            if (top <= bottom)
            {
                top = bottom + Symbol.PipSize * 50;
            }
        }

        // ───────────────────────── Helpers ─────────────────────────

        private int SoftnessForGrade(int grade)
        {
            if (grade <= 1)
                return SoftnessA;
            if (grade == 2)
                return SoftnessB;
            return SoftnessC;
        }

        private string BuildDrawSignature(int lastIndex, WindowDef[] windows, int[] timePoints)
        {
            var sb = new StringBuilder(512);
            sb.Append(lastIndex).Append('|')
              .Append(DaysBack).Append('|')
              .Append(ShiftMinutes).Append('|')
              .Append(HideEmptyWindows).Append('|')
              .Append(DrawModeName).Append('|')
              .Append(FrameWidth).Append('|')
              .Append(VerticalAtWindowStart).Append('|')
              .Append(TeachingView).Append('|')
              .Append(SoftnessA).Append('|').Append(SoftnessB).Append('|').Append(SoftnessC).Append('|')
              .Append(ShowTimeRangeLabels).Append('|')
              .Append(LabelPositionName).Append('|')
              .Append(StaggerLabels).Append('|')
              .Append(AppendGradeToLabel).Append('|')
              .Append(ColourLabelLikeZone).Append('|')
              .Append(ShowTimePointLines).Append('|')
              .Append(TimePointsList).Append('|')
              .Append(ShowTimePointTags).Append('|')
              .Append(ShowDayStartLine).Append('|')
              .Append(ShowDayStartTag).Append('|')
              .Append(_activePrefix).Append('|');

            try
            {
                sb.Append(Math.Round(Chart.TopY, 6)).Append('|').Append(Math.Round(Chart.BottomY, 6)).Append('|');
            }
            catch
            {
                sb.Append("noy|");
            }

            for (int i = 0; i < windows.Length; i++)
            {
                WindowDef w = windows[i];
                sb.Append(w.Enabled).Append(',')
                  .Append(w.StartMinutes).Append(',')
                  .Append(w.EndMinutes).Append(',')
                  .Append(w.ColourName).Append(',')
                  .Append(w.Grade).Append(';');
            }

            if (timePoints != null)
            {
                for (int i = 0; i < timePoints.Length; i++)
                    sb.Append(timePoints[i]).Append(',');
            }

            DateTime lastOpen = Bars.OpenTimes[lastIndex];
            sb.Append(lastOpen.Date.Ticks);
            return sb.ToString();
        }

        private void RemoveDrawings(bool includePanel)
        {
            var names = new List<string>();
            foreach (var obj in Chart.Objects)
            {
                if (obj.Name == null)
                    continue;
                if (!obj.Name.StartsWith(_activePrefix, StringComparison.Ordinal)
                    && !obj.Name.StartsWith("SWP_", StringComparison.Ordinal))
                    continue;
                if (!includePanel && (obj.Name == _activePrefix + PanelObjectName || obj.Name == "SWP_" + PanelObjectName))
                    continue;
                names.Add(obj.Name);
            }

            for (int i = 0; i < names.Count; i++)
                Chart.RemoveObject(names[i]);

            if (includePanel)
            {
                Chart.RemoveObject(_activePrefix + PanelObjectName);
                Chart.RemoveObject("SWP_" + PanelObjectName);
            }
        }

        private string NextName(string kind)
        {
            _objSeq++;
            return _activePrefix + kind + "_" + _objSeq;
        }

        private static string NormalizePrefix(string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                return "SWP_";
            string p = prefix.Trim();
            return p.EndsWith("_", StringComparison.Ordinal) ? p : p + "_";
        }

        private static void CornerAlign(int corner, out VerticalAlignment vAlign, out HorizontalAlignment hAlign)
        {
            switch (corner)
            {
                case 0:
                    vAlign = VerticalAlignment.Top;
                    hAlign = HorizontalAlignment.Left;
                    return;
                case 2:
                    vAlign = VerticalAlignment.Bottom;
                    hAlign = HorizontalAlignment.Left;
                    return;
                case 3:
                    vAlign = VerticalAlignment.Bottom;
                    hAlign = HorizontalAlignment.Right;
                    return;
                default:
                    vAlign = VerticalAlignment.Top;
                    hAlign = HorizontalAlignment.Right;
                    return;
            }
        }

        private static bool IsTopLabel(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return true;
            string s = name.Trim().ToLowerInvariant();
            return !(s.Contains("bottom") || s.Contains("low"));
        }

        private static DrawMode ParseDrawMode(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return DrawMode.EdgesOnly;

            string s = name.Trim().ToLowerInvariant();
            if (s.Contains("filled") && (s.Contains("edge") || s.Contains("+")))
                return DrawMode.FilledAndEdges;
            if (s.Contains("fill"))
                return DrawMode.Filled;
            return DrawMode.EdgesOnly;
        }

        private static LineStyle ParseLineStyle(string name, LineStyle fallback)
        {
            if (string.IsNullOrWhiteSpace(name))
                return fallback;

            switch (name.Trim().ToLowerInvariant())
            {
                case "solid": return LineStyle.Solid;
                case "dash":
                case "dashed":
                case "dots": return LineStyle.Dots;
                case "dot":
                case "dotted":
                case "dotsrare":
                case "dots rare": return LineStyle.DotsRare;
                default: return fallback;
            }
        }

        private static Color ParseRgbColor(string name, Color fallback)
        {
            int r, g, b;
            if (!SessionWindowsLogic.TryParseRgb(name, out r, out g, out b))
                return fallback;
            return Color.FromArgb(255, r, g, b);
        }

        private enum DrawMode
        {
            EdgesOnly,
            Filled,
            FilledAndEdges
        }

        private sealed class WindowDef
        {
            public readonly int Index;
            public readonly bool Enabled;
            public readonly int StartMinutes;
            public readonly int EndMinutes;
            public readonly string ColourName;
            public readonly int Grade;

            public WindowDef(int index, bool enabled, string start, string end, string colour, int grade)
            {
                Index = index;
                Enabled = enabled;
                StartMinutes = SessionWindowsLogic.ParseHhMm(start);
                EndMinutes = SessionWindowsLogic.ParseHhMm(end);
                ColourName = colour;
                Grade = grade < 1 ? 1 : (grade > 3 ? 3 : grade);
            }
        }
    }

    //#region SESSION_WINDOWS_LOGIC
    public static class SessionWindowsLogic
    {
        public const int MinutesPerDay = 1440;

        public static int ParseHhMm(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return -1;

            string s = text.Trim();
            int colon = s.IndexOf(':');
            if (colon <= 0 || colon >= s.Length - 1)
                return -1;

            int hour;
            int minute;
            if (!int.TryParse(s.Substring(0, colon), NumberStyles.Integer, CultureInfo.InvariantCulture, out hour))
                return -1;
            if (!int.TryParse(s.Substring(colon + 1), NumberStyles.Integer, CultureInfo.InvariantCulture, out minute))
                return -1;
            if (hour < 0 || hour > 23 || minute < 0 || minute > 59)
                return -1;
            return hour * 60 + minute;
        }

        public static int ShiftWrap(int minutes, int shift)
        {
            int m = minutes + shift;
            m %= MinutesPerDay;
            if (m < 0)
                m += MinutesPerDay;
            return m;
        }

        public static bool IsInWindow(int barMinutes, int startMinutes, int endMinutes, int shift)
        {
            if (startMinutes < 0 || endMinutes < 0 || startMinutes == endMinutes)
                return false;

            int s = ShiftWrap(startMinutes, shift);
            int e = ShiftWrap(endMinutes, shift);
            int m = ShiftWrap(barMinutes, 0);
            if (s == e)
                return false;
            if (s < e)
                return m >= s && m < e;
            return m >= s || m < e;
        }

        public static void WindowOnDay(DateTime day, int startMinutes, int endMinutes, int shift,
            out DateTime startTime, out DateTime endTime)
        {
            DateTime d = day.Date;
            int s = ShiftWrap(startMinutes, shift);
            int e = ShiftWrap(endMinutes, shift);
            startTime = d.AddMinutes(s);
            if (s < e)
                endTime = d.AddMinutes(e);
            else
                endTime = d.AddMinutes(e + MinutesPerDay);
        }

        public static DateTime TimeOnDay(DateTime day, int minutes, int shift)
        {
            return day.Date.AddMinutes(ShiftWrap(minutes, shift));
        }

        public static DateTime MidTime(DateTime start, DateTime end)
        {
            long ticks = start.Ticks + (end.Ticks - start.Ticks) / 2;
            return new DateTime(ticks, start.Kind);
        }

        public static int[] ParseTimePoints(string text)
        {
            var list = new List<int>();
            if (string.IsNullOrWhiteSpace(text))
                return list.ToArray();

            string[] parts = text.Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
            var seen = new HashSet<int>();
            for (int i = 0; i < parts.Length; i++)
            {
                int minutes = ParseHhMm(parts[i]);
                if (minutes < 0 || seen.Contains(minutes))
                    continue;
                seen.Add(minutes);
                list.Add(minutes);
            }

            list.Sort();
            return list.ToArray();
        }

        public static int AlphaFromSoftness(int softnessPercent)
        {
            int s = softnessPercent;
            if (s < 0) s = 0;
            if (s > 100) s = 100;
            return (int)Math.Round(255.0 * (100 - s) / 100.0);
        }

        public static int MaxGradeForView(string teachingView)
        {
            if (string.IsNullOrWhiteSpace(teachingView))
                return 3;

            string s = teachingView.Trim().ToLowerInvariant();
            if (s.Contains("a+b") || s.Contains("a + b") || s.Contains("grade a+b"))
                return 2;
            if ((s.Contains("grade a") || s == "a" || s.Contains("a only")) && !s.Contains("b") && !s.Contains("full"))
                return 1;
            if (s.Contains("only a") || s.Contains("a-only") || s.Contains("a hot"))
                return 1;
            return 3;
        }

        public static string FormatHhMm(int minutes)
        {
            int m = ShiftWrap(minutes, 0);
            return (m / 60).ToString("00") + ":" + (m % 60).ToString("00");
        }

        public static string FormatWindowLabel(int startMinutes, int endMinutes, int shift, int grade, bool appendGrade)
        {
            string range = FormatHhMm(ShiftWrap(startMinutes, shift)) + "-" + FormatHhMm(ShiftWrap(endMinutes, shift));
            if (!appendGrade)
                return range;
            return range + " " + GradeLetter(grade);
        }

        public static string GradeLetter(int grade)
        {
            if (grade <= 1) return "A";
            if (grade == 2) return "B";
            return "C";
        }

        public static int MinutesUntilEnd(int nowMinutes, int startMinutes, int endMinutes, int shift)
        {
            if (!IsInWindow(nowMinutes, startMinutes, endMinutes, shift))
                return -1;

            int e = ShiftWrap(endMinutes, shift);
            int m = ShiftWrap(nowMinutes, 0);
            int delta = e - m;
            if (delta <= 0)
                delta += MinutesPerDay;
            return delta;
        }

        public static int MinutesUntilNextTimePoint(int nowMinutes, int[] timePoints, int shift)
        {
            if (timePoints == null || timePoints.Length == 0)
                return -1;

            int m = ShiftWrap(nowMinutes, 0);
            int best = int.MaxValue;
            for (int i = 0; i < timePoints.Length; i++)
            {
                int p = ShiftWrap(timePoints[i], shift);
                int delta = p - m;
                if (delta <= 0)
                    delta += MinutesPerDay;
                if (delta < best)
                    best = delta;
            }

            return best == int.MaxValue ? -1 : best;
        }

        public static int NextTimePointAbsolute(int nowMinutes, int[] timePoints, int shift)
        {
            if (timePoints == null || timePoints.Length == 0)
                return -1;

            int m = ShiftWrap(nowMinutes, 0);
            int bestDelta = int.MaxValue;
            int bestAbs = -1;
            for (int i = 0; i < timePoints.Length; i++)
            {
                int p = ShiftWrap(timePoints[i], shift);
                int delta = p - m;
                if (delta <= 0)
                    delta += MinutesPerDay;
                if (delta < bestDelta)
                {
                    bestDelta = delta;
                    bestAbs = p;
                }
            }

            return bestAbs;
        }

        public static string FormatDuration(int minutes)
        {
            if (minutes < 0)
                return "—";
            if (minutes < 60)
                return minutes + "m";
            int h = minutes / 60;
            int m = minutes % 60;
            if (m == 0)
                return h + "h";
            return h + "h " + m + "m";
        }

        public static bool TryParseRgb(string spec, out int r, out int g, out int b)
        {
            r = 0;
            g = 0;
            b = 0;
            if (string.IsNullOrWhiteSpace(spec))
                return false;

            string s = spec.Trim();
            if (s.Length > 0 && s[0] == '#' && (s.Length == 7 || s.Length == 9))
            {
                try
                {
                    string h = s.Substring(1);
                    if (h.Length == 6)
                    {
                        r = Convert.ToInt32(h.Substring(0, 2), 16);
                        g = Convert.ToInt32(h.Substring(2, 2), 16);
                        b = Convert.ToInt32(h.Substring(4, 2), 16);
                        return true;
                    }

                    r = Convert.ToInt32(h.Substring(2, 2), 16);
                    g = Convert.ToInt32(h.Substring(4, 2), 16);
                    b = Convert.ToInt32(h.Substring(6, 2), 16);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            string[] parts = s.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3)
                return false;

            if (!int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out r))
                return false;
            if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out g))
                return false;
            if (!int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out b))
                return false;
            if (r < 0 || r > 255 || g < 0 || g > 255 || b < 0 || b > 255)
                return false;
            return true;
        }
    }
    //#endregion SESSION_WINDOWS_LOGIC
}
