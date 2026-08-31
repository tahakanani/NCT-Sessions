// NCT Final1 — cTrader Automate
// Port of NCT-Final1.pine: Dual Symmetry engine, node targets, sessions, MAP Weekly, Day OC, time targets
using System;
using System.Collections.Generic;
using System.Globalization;
using cAlgo.API;
using cAlgo.API.Internals;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class NCTFinal1 : Indicator
    {
        // ───────────────────────── Nodal / Strategy ─────────────────────────

        [Parameter("Starting Point Candles", DefaultValue = 4000, MinValue = 1, MaxValue = 5000, Group = "Nodal Calculation Settings")]
        public int StartPoint { get; set; }

        [Parameter("End Point (Final)", DefaultValue = 0, MinValue = 0, MaxValue = 5000, Group = "Nodal Calculation Settings")]
        public int EndPoint { get; set; }

        [Parameter("Calc Nodes with Logarithm", DefaultValue = true, Group = "Strategy Config")]
        public bool SwCalcLogarithm { get; set; }

        [Parameter("Node Display Mode", DefaultValue = "Starred", Group = "Strategy Config")]
        public string NodeDisplayMode { get; set; }

        [Parameter("Vertical Gap Lines", DefaultValue = 0, MinValue = 0, MaxValue = 15, Group = "Strategy Config")]
        public int VerticalGapLines { get; set; }

        [Parameter("Show Regular Nodes", DefaultValue = true, Group = "Strategy Config")]
        public bool ShowRegularNodes { get; set; }

        [Parameter("Show Symmetry Nodes", DefaultValue = true, Group = "Strategy Config")]
        public bool ShowDoubleStarNodes { get; set; }

        // ───────────────────────── Node Targets — Master ─────────────────────────

        [Parameter("Show Node 1 Targets", DefaultValue = true, Group = "Node Targets — Master Switches")]
        public bool EnableSingleNode1Targets { get; set; }

        [Parameter("Show Node 2 Target", DefaultValue = true, Group = "Node Targets — Master Switches")]
        public bool EnableSingleNode2Targets { get; set; }

        [Parameter("Show Node 1+2 Pair Targets", DefaultValue = true, Group = "Node Targets — Master Switches")]
        public bool EnablePairNode12Targets { get; set; }

        [Parameter("Apply to Up Trend", DefaultValue = true, Group = "Node Targets — Master Switches")]
        public bool ShowTargetUp { get; set; }

        [Parameter("Apply to Down Trend", DefaultValue = true, Group = "Node Targets — Master Switches")]
        public bool ShowTargetDown { get; set; }

        [Parameter("Target Line Length (bars)", DefaultValue = 150, MinValue = 3, MaxValue = 490, Group = "Node Targets — Master Switches")]
        public int TargetGapBars { get; set; }

        [Parameter("Delete Hit Targets", DefaultValue = true, Group = "Node Targets — Master Switches")]
        public bool DeleteHitTargets { get; set; }

        [Parameter("Hit Grace Bars", DefaultValue = 2, MinValue = 0, MaxValue = 500, Group = "Node Targets — Master Switches")]
        public int HitGraceBars { get; set; }

        [Parameter("Target Lines Transparency", DefaultValue = 60, MinValue = 0, MaxValue = 100, Group = "Node Targets — Master Switches")]
        public int TargetTransparency { get; set; }

        // ───────────────────────── Round Numbers ─────────────────────────

        [Parameter("Show Round Number Targets", DefaultValue = false, Group = "Round Number Targets")]
        public bool EnableRoundTargets { get; set; }

        [Parameter("Round Number Base Price", DefaultValue = 10.0, MinValue = 0.0001, Group = "Round Number Targets")]
        public double RoundBasePrice { get; set; }

        [Parameter("Apply Round to Up", DefaultValue = true, Group = "Round Number Targets")]
        public bool RoundApplyUp { get; set; }

        [Parameter("Apply Round to Down", DefaultValue = true, Group = "Round Number Targets")]
        public bool RoundApplyDown { get; set; }

        [Parameter("Round Line Width", DefaultValue = 1, MinValue = 1, MaxValue = 4, Group = "Round Number Targets")]
        public int RoundLineWidth { get; set; }

        [Parameter("Round Line Style", DefaultValue = "Dotted", Group = "Round Number Targets")]
        public string RoundLineStyle { get; set; }

        [Parameter("Round Line Transparency", DefaultValue = 60, MinValue = 0, MaxValue = 100, Group = "Round Number Targets")]
        public int RoundLineTransparency { get; set; }

        [Parameter("Round Line Color", DefaultValue = "#B3B3B3", Group = "Round Number Targets")]
        public string RoundLineColorName { get; set; }

        [Parameter("Round Min Visible Price", DefaultValue = 3000.0, MinValue = 0.0, Group = "Round Number Targets")]
        public double RoundMinVisiblePrice { get; set; }

        // ───────────────────────── Node 1 Targets ─────────────────────────

        [Parameter("Show Target Double.1", DefaultValue = true, Group = "Node 1 Targets (Single)")]
        public bool ShowDouble { get; set; }

        [Parameter("Show Target 1.5DL.1", DefaultValue = true, Group = "Node 1 Targets (Single)")]
        public bool ShowDouble15 { get; set; }

        [Parameter("1.5DL.1 Ratio", DefaultValue = 1.5, MinValue = 0.01, MaxValue = 5.0, Group = "Node 1 Targets (Single)")]
        public double Double15Ratio { get; set; }

        [Parameter("Show Target 0.8DL.1", DefaultValue = true, Group = "Node 1 Targets (Single)")]
        public bool ShowDouble086 { get; set; }

        [Parameter("0.8DL.1 Ratio", DefaultValue = 0.85, MinValue = 0.01, MaxValue = 2.0, Group = "Node 1 Targets (Single)")]
        public double Double086Ratio { get; set; }

        [Parameter("Show Target Min 1", DefaultValue = true, Group = "Node 1 Targets (Single)")]
        public bool ShowMin { get; set; }

        [Parameter("0.8Min.1 Ratio", DefaultValue = 0.85, MinValue = 0.01, MaxValue = 2.0, Group = "Node 1 Targets (Single)")]
        public double Min085Ratio { get; set; }

        [Parameter("Show 1.3MIN1", DefaultValue = true, Group = "Node 1 Targets (Single)")]
        public bool ShowBasedMin13 { get; set; }

        [Parameter("Based Retrace Ratio", DefaultValue = 0.85, MinValue = 0.01, MaxValue = 2.0, Group = "Node 1 Targets (Single)")]
        public double BasedRetraceRatio { get; set; }

        [Parameter("Based Min Extension", DefaultValue = 1.3, MinValue = 0.01, MaxValue = 5.0, Group = "Node 1 Targets (Single)")]
        public double BasedMinExtRatio { get; set; }

        [Parameter("Proximity Tolerance (%)", DefaultValue = 0.15, MinValue = 0.01, MaxValue = 5.0, Group = "Node 1 Targets (Single)")]
        public double MinDblProximityTolPct { get; set; }

        [Parameter("Show Target Correction", DefaultValue = false, Group = "Node 1 Targets (Single)")]
        public bool ShowCorrection { get; set; }

        [Parameter("Double Line Width", DefaultValue = 1, MinValue = 1, MaxValue = 4, Group = "Node 1 Targets (Single)")]
        public int DoubleLineWidth { get; set; }

        [Parameter("1.5DL.1 Line Width", DefaultValue = 1, MinValue = 1, MaxValue = 4, Group = "Node 1 Targets (Single)")]
        public int Double15LineWidth { get; set; }

        [Parameter("0.8DL.1 Line Width", DefaultValue = 1, MinValue = 1, MaxValue = 4, Group = "Node 1 Targets (Single)")]
        public int Double086LineWidth { get; set; }

        [Parameter("Min Line Width", DefaultValue = 1, MinValue = 1, MaxValue = 4, Group = "Node 1 Targets (Single)")]
        public int MinLineWidth { get; set; }

        [Parameter("Correction Line Width", DefaultValue = 1, MinValue = 1, MaxValue = 4, Group = "Node 1 Targets (Single)")]
        public int CorrectionLineWidth { get; set; }

        [Parameter("Double Line Style", DefaultValue = "Solid", Group = "Node 1 Targets (Single)")]
        public string DoubleLineStyle { get; set; }

        [Parameter("1.5DL.1 Line Style", DefaultValue = "Dotted", Group = "Node 1 Targets (Single)")]
        public string Double15LineStyle { get; set; }

        [Parameter("0.8DL.1 Line Style", DefaultValue = "Dotted", Group = "Node 1 Targets (Single)")]
        public string Double086LineStyle { get; set; }

        [Parameter("Min Line Style", DefaultValue = "Solid", Group = "Node 1 Targets (Single)")]
        public string MinLineStyle { get; set; }

        [Parameter("Correction Line Style", DefaultValue = "Dotted", Group = "Node 1 Targets (Single)")]
        public string CorrectionLineStyle { get; set; }

        [Parameter("Last Nodes to Target", DefaultValue = 30, MinValue = 1, MaxValue = 100, Group = "Node 1 Targets (Single)")]
        public int TargetNode1MaxCount { get; set; }

        // ───────────────────────── Node 2 Target ─────────────────────────

        [Parameter("Show Target Node 2", DefaultValue = true, Group = "Node 2 Target (Single)")]
        public bool ShowTargetNode2 { get; set; }

        [Parameter("Node 2 Line Width", DefaultValue = 1, MinValue = 1, MaxValue = 4, Group = "Node 2 Target (Single)")]
        public int Node2LineWidth { get; set; }

        [Parameter("Node 2 Line Style", DefaultValue = "Solid", Group = "Node 2 Target (Single)")]
        public string Node2LineStyle { get; set; }

        [Parameter("Last Node 2 to Target", DefaultValue = 30, MinValue = 1, MaxValue = 100, Group = "Node 2 Target (Single)")]
        public int TargetNode2MaxCount { get; set; }

        // ───────────────────────── Pair Targets ─────────────────────────

        [Parameter("Show Pair Min 12", DefaultValue = true, Group = "Node 1+2 Pair Targets")]
        public bool ShowPairMin { get; set; }

        [Parameter("Show Pair Max 12", DefaultValue = true, Group = "Node 1+2 Pair Targets")]
        public bool ShowPairMax { get; set; }

        [Parameter("Show Pair Double 12", DefaultValue = true, Group = "Node 1+2 Pair Targets")]
        public bool ShowPairDouble { get; set; }

        [Parameter("Show Pair Correction 2", DefaultValue = false, Group = "Node 1+2 Pair Targets")]
        public bool ShowPairCorrection { get; set; }

        [Parameter("Pair Min Width", DefaultValue = 1, MinValue = 1, MaxValue = 4, Group = "Node 1+2 Pair Targets")]
        public int PairMinWidth { get; set; }

        [Parameter("Pair Max Width", DefaultValue = 1, MinValue = 1, MaxValue = 4, Group = "Node 1+2 Pair Targets")]
        public int PairMaxWidth { get; set; }

        [Parameter("Pair Double Width", DefaultValue = 1, MinValue = 1, MaxValue = 4, Group = "Node 1+2 Pair Targets")]
        public int PairDoubleWidth { get; set; }

        [Parameter("Pair Correction Width", DefaultValue = 1, MinValue = 1, MaxValue = 4, Group = "Node 1+2 Pair Targets")]
        public int PairCorrectionWidth { get; set; }

        [Parameter("Pair Min Style", DefaultValue = "Solid", Group = "Node 1+2 Pair Targets")]
        public string PairMinStyle { get; set; }

        [Parameter("Pair Max Style", DefaultValue = "Solid", Group = "Node 1+2 Pair Targets")]
        public string PairMaxStyle { get; set; }

        [Parameter("Pair Double Style", DefaultValue = "Solid", Group = "Node 1+2 Pair Targets")]
        public string PairDoubleStyle { get; set; }

        [Parameter("Pair Correction Style", DefaultValue = "Dotted", Group = "Node 1+2 Pair Targets")]
        public string PairCorrectionStyle { get; set; }

        [Parameter("Last Pairs to Target", DefaultValue = 30, MinValue = 1, MaxValue = 100, Group = "Node 1+2 Pair Targets")]
        public int PairTargetMaxCount { get; set; }

        // ───────────────────────── Visual ─────────────────────────

        [Parameter("Font Size (With Symmetry)", DefaultValue = "Large", Group = "Visual Customization")]
        public string TextNodeSizeSym { get; set; }

        [Parameter("Font Size (Without Symmetry)", DefaultValue = "Normal", Group = "Visual Customization")]
        public string TextNodeSizeNoSym { get; set; }

        [Parameter("Node Label Distance Up", DefaultValue = 0, MinValue = 0, MaxValue = 8, Group = "Visual Customization")]
        public int NodeLabelPadUp { get; set; }

        [Parameter("Node Label Distance Down", DefaultValue = 0, MinValue = 0, MaxValue = 8, Group = "Visual Customization")]
        public int NodeLabelPadDown { get; set; }

        [Parameter("Incomplete N2 Circle Radius (%)", DefaultValue = 0.004, MinValue = 0.001, MaxValue = 5.0, Group = "Visual Customization")]
        public double IncompleteNode2RadiusPct { get; set; }

        [Parameter("Incomplete N2 Circle Radius (Bars)", DefaultValue = 1, MinValue = 1, MaxValue = 50, Group = "Visual Customization")]
        public int IncompleteNode2RadiusBars { get; set; }

        [Parameter("Incomplete N2 Circle Color", DefaultValue = "Red", Group = "Visual Customization")]
        public string IncompleteNode2CircleColorName { get; set; }

        [Parameter("Incomplete N2 Fill Transparency", DefaultValue = 40, MinValue = 0, MaxValue = 100, Group = "Visual Customization")]
        public int IncompleteNode2FillTransparency { get; set; }

        [Parameter("Proximity Circle Radius (%)", DefaultValue = 0.002, MinValue = 0.001, MaxValue = 5.0, Group = "Visual Customization")]
        public double ProximityMarkerRadiusPct { get; set; }

        [Parameter("Proximity Circle Radius (Bars)", DefaultValue = 1, MinValue = 1, MaxValue = 50, Group = "Visual Customization")]
        public int ProximityMarkerRadiusBars { get; set; }

        [Parameter("Proximity Circle Color", DefaultValue = "Green", Group = "Visual Customization")]
        public string ProximityMarkerColorName { get; set; }

        [Parameter("Proximity Fill Transparency", DefaultValue = 40, MinValue = 0, MaxValue = 100, Group = "Visual Customization")]
        public int ProximityMarkerFillTransparency { get; set; }

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

        [Parameter("NoSym Color 1", DefaultValue = "White", Group = "No-Symmetry Colors")]
        public string N1Name { get; set; }

        [Parameter("NoSym Color 2", DefaultValue = "Yellow", Group = "No-Symmetry Colors")]
        public string N2Name { get; set; }

        [Parameter("NoSym Color 3", DefaultValue = "Aqua", Group = "No-Symmetry Colors")]
        public string N3Name { get; set; }

        [Parameter("NoSym Color 4", DefaultValue = "Lime", Group = "No-Symmetry Colors")]
        public string N4Name { get; set; }

        [Parameter("NoSym Color 5", DefaultValue = "Orange", Group = "No-Symmetry Colors")]
        public string N5Name { get; set; }

        [Parameter("NoSym Color 6", DefaultValue = "Fuchsia", Group = "No-Symmetry Colors")]
        public string N6Name { get; set; }

        [Parameter("NoSym Color 7", DefaultValue = "Blue", Group = "No-Symmetry Colors")]
        public string N7Name { get; set; }

        [Parameter("NoSym Color 8", DefaultValue = "Red", Group = "No-Symmetry Colors")]
        public string N8Name { get; set; }

        // ───────────────────────── Label Anti-Overlap ─────────────────────────

        [Parameter("Label Collision Tolerance (%)", DefaultValue = 0.005, MinValue = 0.001, MaxValue = 10.0, Group = "Target Label Anti-Overlap")]
        public double TargetLabelPriceTolerancePct { get; set; }

        [Parameter("Label Columns", DefaultValue = 2, MinValue = 1, MaxValue = 6, Group = "Target Label Anti-Overlap")]
        public int TargetLabelColumns { get; set; }

        [Parameter("Extra Row Offset (%)", DefaultValue = 0.02, MinValue = 0.0, MaxValue = 2.0, Group = "Target Label Anti-Overlap")]
        public double TargetLabelRowStepPct { get; set; }

        [Parameter("Label Stagger Step (bars)", DefaultValue = 13, MinValue = 4, MaxValue = 200, Group = "Target Label Anti-Overlap")]
        public int TargetLabelStaggerStep { get; set; }

        [Parameter("Label Height Above Line (%)", DefaultValue = 0.0, MinValue = 0.0, MaxValue = 2.0, Group = "Target Label Anti-Overlap")]
        public double TargetLabelYOffsetPct { get; set; }

        [Parameter("Target Label Font Size", DefaultValue = "Tiny", Group = "Target Label Anti-Overlap")]
        public string TargetLabelSize { get; set; }

        // ───────────────────────── Sessions ─────────────────────────

        [Parameter("Session Display Days", DefaultValue = 2, MinValue = 1, MaxValue = 10, Group = "Session Display")]
        public int SessionDisplayDays { get; set; }

        [Parameter("Show New York", DefaultValue = false, Group = "Session A")]
        public bool ShowSesA { get; set; }

        [Parameter("NY Name", DefaultValue = "New York", Group = "Session A")]
        public string SesAName { get; set; }

        [Parameter("NY Session (HHmm-HHmm)", DefaultValue = "1300-2200", Group = "Session A")]
        public string SesASession { get; set; }

        [Parameter("NY Color", DefaultValue = "#FF5D00", Group = "Session A")]
        public string SesAColorName { get; set; }

        [Parameter("NY Range", DefaultValue = true, Group = "Session A")]
        public bool SesARange { get; set; }

        [Parameter("NY Middle Line", DefaultValue = true, Group = "Session A")]
        public bool SesAMidline { get; set; }

        [Parameter("NY Mid Extension (Bars)", DefaultValue = 10, MinValue = 0, Group = "Session A")]
        public int SesAMidlineLength { get; set; }

        [Parameter("Show London", DefaultValue = false, Group = "Session B")]
        public bool ShowSesB { get; set; }

        [Parameter("London Name", DefaultValue = "London", Group = "Session B")]
        public string SesBName { get; set; }

        [Parameter("London Session (HHmm-HHmm)", DefaultValue = "0700-1530", Group = "Session B")]
        public string SesBSession { get; set; }

        [Parameter("London Color", DefaultValue = "#2157F3", Group = "Session B")]
        public string SesBColorName { get; set; }

        [Parameter("London Range", DefaultValue = true, Group = "Session B")]
        public bool SesBRange { get; set; }

        [Parameter("London Middle Line", DefaultValue = true, Group = "Session B")]
        public bool SesBMidline { get; set; }

        [Parameter("London Mid Extension (Bars)", DefaultValue = 10, MinValue = 0, Group = "Session B")]
        public int SesBMidlineLength { get; set; }

        [Parameter("Show Tokyo", DefaultValue = true, Group = "Session C")]
        public bool ShowSesC { get; set; }

        [Parameter("Tokyo Name", DefaultValue = "Tokyo", Group = "Session C")]
        public string SesCName { get; set; }

        [Parameter("Tokyo Session (HHmm-HHmm)", DefaultValue = "0000-0600", Group = "Session C")]
        public string SesCSession { get; set; }

        [Parameter("Tokyo Color", DefaultValue = "#FFEB3B", Group = "Session C")]
        public string SesCColorName { get; set; }

        [Parameter("Tokyo Range", DefaultValue = true, Group = "Session C")]
        public bool SesCRange { get; set; }

        [Parameter("Tokyo Middle Line", DefaultValue = true, Group = "Session C")]
        public bool SesCMidline { get; set; }

        [Parameter("Tokyo Mid Extension (Bars)", DefaultValue = 100, MinValue = 0, Group = "Session C")]
        public int SesCMidlineLength { get; set; }

        [Parameter("Show Sydney", DefaultValue = false, Group = "Session D")]
        public bool ShowSesD { get; set; }

        [Parameter("Sydney Name", DefaultValue = "Sydney", Group = "Session D")]
        public string SesDName { get; set; }

        [Parameter("Sydney Session (HHmm-HHmm)", DefaultValue = "2100-0600", Group = "Session D")]
        public string SesDSession { get; set; }

        [Parameter("Sydney Color", DefaultValue = "#FFEB3B", Group = "Session D")]
        public string SesDColorName { get; set; }

        [Parameter("Sydney Range", DefaultValue = true, Group = "Session D")]
        public bool SesDRange { get; set; }

        [Parameter("Sydney Middle Line", DefaultValue = true, Group = "Session D")]
        public bool SesDMidline { get; set; }

        [Parameter("Sydney Mid Extension (Bars)", DefaultValue = 10, MinValue = 0, Group = "Session D")]
        public int SesDMidlineLength { get; set; }

        [Parameter("UTC (+/-)", DefaultValue = 0, MinValue = -12, MaxValue = 14, Group = "Timezone")]
        public int TzOffsetHours { get; set; }

        [Parameter("Use Exchange Timezone", DefaultValue = false, Group = "Timezone")]
        public bool UseExchangeTimezone { get; set; }

        [Parameter("Range Area Transparency", DefaultValue = 90, MinValue = 0, MaxValue = 100, Group = "Ranges Settings")]
        public double RangeBgTransparency { get; set; }

        [Parameter("Range Outline", DefaultValue = true, Group = "Ranges Settings")]
        public bool ShowRangeOutline { get; set; }

        [Parameter("Outline Width", DefaultValue = 1, MinValue = 1, MaxValue = 5, Group = "Ranges Settings")]
        public int OutlineWidth { get; set; }

        [Parameter("Outline Style", DefaultValue = "Dotted", Group = "Ranges Settings")]
        public string OutlineStyleName { get; set; }

        [Parameter("Outline Transparency", DefaultValue = 0, MinValue = 0, MaxValue = 100, Group = "Ranges Settings")]
        public int OutlineTransparency { get; set; }

        [Parameter("Range Label", DefaultValue = true, Group = "Ranges Settings")]
        public bool ShowRangeLabel { get; set; }

        [Parameter("Middle Line Style", DefaultValue = "Dashed", Group = "Ranges Settings")]
        public string MidlineStyleName { get; set; }

        [Parameter("Middle Line Width", DefaultValue = 1, MinValue = 1, MaxValue = 5, Group = "Ranges Settings")]
        public int MidlineWidth { get; set; }

        [Parameter("Middle Line Transparency", DefaultValue = 0, MinValue = 0, MaxValue = 100, Group = "Ranges Settings")]
        public int MidlineTransparency { get; set; }

        [Parameter("Show Session High/Low/Mid Targets", DefaultValue = true, Group = "Session Targets")]
        public bool EnableSessionTargets { get; set; }

        [Parameter("Include Session High", DefaultValue = true, Group = "Session Targets")]
        public bool ApplySessionHigh { get; set; }

        [Parameter("Include Session Low", DefaultValue = true, Group = "Session Targets")]
        public bool ApplySessionLow { get; set; }

        [Parameter("Include Session Midline", DefaultValue = true, Group = "Session Targets")]
        public bool ApplySessionMid { get; set; }

        [Parameter("Session High Line Width", DefaultValue = 1, MinValue = 1, MaxValue = 4, Group = "Session Targets")]
        public int SessionHighLineWidth { get; set; }

        [Parameter("Session Low Line Width", DefaultValue = 1, MinValue = 1, MaxValue = 4, Group = "Session Targets")]
        public int SessionLowLineWidth { get; set; }

        [Parameter("Session Mid Line Width", DefaultValue = 1, MinValue = 1, MaxValue = 4, Group = "Session Targets")]
        public int SessionMidLineWidth { get; set; }

        [Parameter("Session Target Line Style", DefaultValue = "Dotted", Group = "Session Targets")]
        public string SessionTargetLineStyle { get; set; }

        [Parameter("Session Target Transparency", DefaultValue = 60, MinValue = 0, MaxValue = 100, Group = "Session Targets")]
        public int SessionTargetTransparency { get; set; }

        // ───────────────────────── Day Open / Close ─────────────────────────

        [Parameter("Show Day Open Line", DefaultValue = true, Group = "Day Open/Close")]
        public bool ShowDayOpenLine { get; set; }

        [Parameter("Show Day Close Line", DefaultValue = true, Group = "Day Open/Close")]
        public bool ShowDayCloseLine { get; set; }

        [Parameter("Open Line Color", DefaultValue = "Green", Group = "Day Open/Close")]
        public string OpenLineColorName { get; set; }

        [Parameter("Close Line Color", DefaultValue = "Red", Group = "Day Open/Close")]
        public string CloseLineColorName { get; set; }

        [Parameter("Open Line Width", DefaultValue = 2, MinValue = 1, MaxValue = 5, Group = "Day Open/Close")]
        public int OpenLineWidth { get; set; }

        [Parameter("Close Line Width", DefaultValue = 2, MinValue = 1, MaxValue = 5, Group = "Day Open/Close")]
        public int CloseLineWidth { get; set; }

        [Parameter("Open Line Style", DefaultValue = "Solid", Group = "Day Open/Close")]
        public string OpenLineStyle { get; set; }

        [Parameter("Close Line Style", DefaultValue = "Dotted", Group = "Day Open/Close")]
        public string CloseLineStyle { get; set; }

        [Parameter("Open Line Transparency", DefaultValue = 0, MinValue = 0, MaxValue = 100, Group = "Day Open/Close")]
        public int OpenLineTransparency { get; set; }

        [Parameter("Close Line Transparency", DefaultValue = 0, MinValue = 0, MaxValue = 100, Group = "Day Open/Close")]
        public int CloseLineTransparency { get; set; }

        [Parameter("Show Day Open/Close Panel", DefaultValue = true, Group = "Day Open/Close")]
        public bool ShowDayOCBox { get; set; }

        [Parameter("Panel Background Color", DefaultValue = "#26FFFFFF", Group = "Day Open/Close")]
        public string DayOCBoxBgColorName { get; set; }

        // ───────────────────────── Time Targets ─────────────────────────

        [Parameter("Show Time Targets", DefaultValue = true, Group = "Time Targets (Vertical)")]
        public bool EnableTimeTargets { get; set; }

        [Parameter("Timezone", DefaultValue = "Custom UTC", Group = "Time Targets (Vertical)")]
        public string TimeTargetTzMode { get; set; }

        [Parameter("UTC Offset (hours)", DefaultValue = 3.5, MinValue = -12.0, MaxValue = 14.0, Group = "Time Targets (Vertical)")]
        public double TimeTargetUtcOffset { get; set; }

        [Parameter("Display Days", DefaultValue = 1, MinValue = 1, MaxValue = 10, Group = "Time Targets (Vertical)")]
        public int TimeTargetDisplayDays { get; set; }

        [Parameter("Times (HH:MM, comma-separated)", DefaultValue = "03:30,04:30,08:00,09:00,09:30,10:30,14:30,15:00,15:30,16:00,16:30,17:00,17:30,18:00,20:00,21:30,23:00,23:30", Group = "Time Targets (Vertical)")]
        public string TimeTargetsCsv { get; set; }

        [Parameter("Time Line Color", DefaultValue = "#78909C", Group = "Time Targets (Vertical)")]
        public string TimeTargetColorName { get; set; }

        [Parameter("Time Line Width", DefaultValue = 1, MinValue = 1, MaxValue = 5, Group = "Time Targets (Vertical)")]
        public int TimeTargetWidth { get; set; }

        [Parameter("Time Line Style", DefaultValue = "Dotted", Group = "Time Targets (Vertical)")]
        public string TimeTargetStyle { get; set; }

        [Parameter("Time Line Transparency", DefaultValue = 50, MinValue = 0, MaxValue = 100, Group = "Time Targets (Vertical)")]
        public int TimeTargetTransparency { get; set; }

        [Parameter("Show Time Labels", DefaultValue = true, Group = "Time Targets (Vertical)")]
        public bool ShowTimeTargetLabels { get; set; }

        [Parameter("Time Label Size", DefaultValue = "Tiny", Group = "Time Targets (Vertical)")]
        public string TimeTargetLabelSize { get; set; }

        // ───────────────────────── MAP Weekly ─────────────────────────

        [Parameter("Show MAP Weekly Targets", DefaultValue = true, Group = "MAP Weekly Targets")]
        public bool EnableMapWeekly { get; set; }

        [Parameter("Show Weekly High", DefaultValue = true, Group = "MAP Weekly Targets")]
        public bool MapShowHigh { get; set; }

        [Parameter("Show Weekly Low", DefaultValue = true, Group = "MAP Weekly Targets")]
        public bool MapShowLow { get; set; }

        [Parameter("Show 50% Mid", DefaultValue = true, Group = "MAP Weekly Targets")]
        public bool MapShowMid { get; set; }

        [Parameter("Show 25%", DefaultValue = true, Group = "MAP Weekly Targets")]
        public bool MapShow25 { get; set; }

        [Parameter("Show 75%", DefaultValue = true, Group = "MAP Weekly Targets")]
        public bool MapShow75 { get; set; }

        [Parameter("Extensions Above High", DefaultValue = true, Group = "MAP Weekly Targets")]
        public bool MapShowExtAbove { get; set; }

        [Parameter("Extensions Below Low", DefaultValue = true, Group = "MAP Weekly Targets")]
        public bool MapShowExtBelow { get; set; }

        [Parameter("1.125x", DefaultValue = false, Group = "MAP Weekly Targets")]
        public bool MapShow1125 { get; set; }

        [Parameter("1.25x", DefaultValue = true, Group = "MAP Weekly Targets")]
        public bool MapShow125 { get; set; }

        [Parameter("1.375x", DefaultValue = false, Group = "MAP Weekly Targets")]
        public bool MapShow1375 { get; set; }

        [Parameter("1.5x", DefaultValue = true, Group = "MAP Weekly Targets")]
        public bool MapShow150 { get; set; }

        [Parameter("1.75x", DefaultValue = true, Group = "MAP Weekly Targets")]
        public bool MapShow175 { get; set; }

        [Parameter("2x", DefaultValue = true, Group = "MAP Weekly Targets")]
        public bool MapShow200 { get; set; }

        [Parameter("Extend Lines (Bars)", DefaultValue = 150, MinValue = 0, MaxValue = 490, Group = "MAP Weekly Targets")]
        public int MapExtendBars { get; set; }

        [Parameter("Show MAP Labels", DefaultValue = true, Group = "MAP Weekly Targets")]
        public bool MapShowLabels { get; set; }

        [Parameter("MAP Label Font Size", DefaultValue = "Small", Group = "MAP Weekly Targets")]
        public string MapLabelSize { get; set; }

        [Parameter("MAP Line Transparency", DefaultValue = 30, MinValue = 0, MaxValue = 100, Group = "MAP Weekly Targets")]
        public int MapLineTransparency { get; set; }

        [Parameter("MAP Key Line Width", DefaultValue = 1, MinValue = 1, MaxValue = 5, Group = "MAP Weekly Targets")]
        public int MapKeyLineWidth { get; set; }

        [Parameter("MAP Mid/Retrace Width", DefaultValue = 1, MinValue = 1, MaxValue = 5, Group = "MAP Weekly Targets")]
        public int MapMidLineWidth { get; set; }

        [Parameter("MAP Extension Width", DefaultValue = 1, MinValue = 1, MaxValue = 5, Group = "MAP Weekly Targets")]
        public int MapExtLineWidth { get; set; }

        [Parameter("MAP Key Line Style", DefaultValue = "Dotted", Group = "MAP Weekly Targets")]
        public string MapKeyLineStyle { get; set; }

        [Parameter("MAP Mid Line Style", DefaultValue = "Dotted", Group = "MAP Weekly Targets")]
        public string MapMidLineStyle { get; set; }

        [Parameter("MAP Extension Line Style", DefaultValue = "Dotted", Group = "MAP Weekly Targets")]
        public string MapExtLineStyle { get; set; }

        [Parameter("MAP Line Color", DefaultValue = "#C084FC", Group = "MAP Weekly Targets")]
        public string MapLineColorName { get; set; }

        // ───────────────────────── State ─────────────────────────

        private bool UseSymmetry
        {
            get { return !string.Equals(NodeDisplayMode, "Starless", StringComparison.OrdinalIgnoreCase); }
        }

        private readonly List<Node> _nodesUp = new List<Node>();
        private readonly List<Node> _nodesDown = new List<Node>();
        private readonly List<Color> _colors = new List<Color>();
        private readonly List<double> _stagUpPrice = new List<double>();
        private readonly List<int> _stagUpX = new List<int>();
        private readonly List<double> _stagDnPrice = new List<double>();
        private readonly List<int> _stagDnX = new List<int>();
        private readonly List<double> _stagOtPrice = new List<double>();
        private readonly List<int> _stagOtX = new List<int>();

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

        private Bars _dailyBars;
        private Bars _weeklyBars;
        private Bars _m1Bars;
        private SessionBox _lastSesA;
        private SessionBox _lastSesB;
        private SessionBox _lastSesC;
        private SessionBox _lastSesD;

        private const string ObjectPrefix = "NCTF1_";
        private const int MaxRoundLevels = 40;
        private const int StaggerScanCap = 48;
        private bool _rebuilding;

        // ───────────────────────── Lifecycle ─────────────────────────

        protected override void Initialize()
        {
            ResetState();
            try { _dailyBars = MarketData.GetBars(TimeFrame.Daily); } catch { }
            try { _weeklyBars = MarketData.GetBars(TimeFrame.Weekly); } catch { }
            try { _m1Bars = MarketData.GetBars(TimeFrame.Minute); } catch { }
            try { Timer.Start(TimeSpan.FromSeconds(1)); } catch { }
        }

        protected override void OnDestroy()
        {
            try { Timer.Stop(); } catch { }
            try { RemoveDrawings(); } catch { }
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
                ResetLabelStagger();
                _objSeq = 0;

                bool underline = !UseSymmetry;
                int extraGap = UseSymmetry ? VerticalGapLines : 0;
                int padUp = NodeLabelPadUp;
                int padDown = NodeLabelPadDown;

                if (_nodesUp.Count > 0)
                    DrawingNumberNodes(true, underline, extraGap, padUp);
                if (_nodesDown.Count > 0)
                    DrawingNumberNodes(false, underline, extraGap, padDown);

                if (ShowTargetUp)
                {
                    if (EnableSingleNode1Targets)
                        DrawingTargetsNode1(true);
                    if (EnableSingleNode2Targets && ShowTargetNode2)
                        DrawingTargetsNode2(true);
                    if (EnablePairNode12Targets)
                        DrawingPairTargetsNode12(true);
                }

                if (ShowTargetDown)
                {
                    if (EnableSingleNode1Targets)
                        DrawingTargetsNode1(false);
                    if (EnableSingleNode2Targets && ShowTargetNode2)
                        DrawingTargetsNode2(false);
                    if (EnablePairNode12Targets)
                        DrawingPairTargetsNode12(false);
                }

                if (EnableRoundTargets)
                {
                    if (RoundApplyUp)
                        DrawRoundNumberTargets(true, true);
                    if (RoundApplyDown)
                        DrawRoundNumberTargets(false, !RoundApplyUp);
                }

                DrawSessions();

                if (ShowDayOpenLine || ShowDayCloseLine || ShowDayOCBox)
                    DrawingDayOpenClose();

                if (EnableMapWeekly)
                    DrawMapWeekly();

                if (EnableTimeTargets)
                    DrawTimeTargets();

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
            _objSeq = 0;

            _priceLowestUp = 999999.0;
            _indexLowestUp = 0;
            _priceHighestUp = 0.0;
            _indexHighestUp = 0;

            _priceLowestDown = 999999.0;
            _indexLowestDown = 0;
            _priceHighestDown = 0.0;
            _indexHighestDown = 0;

            _lastSesA = null;
            _lastSesB = null;
            _lastSesC = null;
            _lastSesD = null;
        }

        private void InitColors()
        {
            _colors.Clear();
            if (UseSymmetry)
            {
                _colors.Add(ParseColor(Color1Name, Color.White));
                _colors.Add(ParseColor(Color2Name, Color.Yellow));
                _colors.Add(ParseColor(Color3Name, Color.Aqua));
                _colors.Add(ParseColor(Color4Name, Color.Lime));
                _colors.Add(ParseColor(Color5Name, Color.Orange));
                _colors.Add(ParseColor(Color6Name, Color.Fuchsia));
                _colors.Add(ParseColor(Color7Name, Color.Blue));
                _colors.Add(ParseColor(Color8Name, Color.Red));
            }
            else
            {
                _colors.Add(ParseColor(N1Name, Color.White));
                _colors.Add(ParseColor(N2Name, Color.Yellow));
                _colors.Add(ParseColor(N3Name, Color.Aqua));
                _colors.Add(ParseColor(N4Name, Color.Lime));
                _colors.Add(ParseColor(N5Name, Color.Orange));
                _colors.Add(ParseColor(N6Name, Color.Fuchsia));
                _colors.Add(ParseColor(N7Name, Color.Blue));
                _colors.Add(ParseColor(N8Name, Color.Red));
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
            return WithAlpha(baseColor, TargetTransparency);
        }

        private static Color WithAlpha(Color baseColor, int transparency)
        {
            int t = transparency;
            if (t < 0) t = 0;
            if (t > 100) t = 100;
            int alpha = (100 - t) * 255 / 100;
            return Color.FromArgb(alpha, baseColor.R, baseColor.G, baseColor.B);
        }

        // تارگت‌ها همیشه لگاریتمی‌اند (مثل Pine)
        private static double TgtMove(double from, double to)
        {
            return Math.Abs(SafeLog(to) - SafeLog(from));
        }

        private static double TgtProject(double origin, double move, bool isUp)
        {
            return isUp
                ? Math.Exp(SafeLog(origin) + move)
                : Math.Exp(SafeLog(origin) - move);
        }

        private static double TgtAlong(double start, double end, double ratio)
        {
            return Math.Exp(SafeLog(start) + ratio * (SafeLog(end) - SafeLog(start)));
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

        private double AlongPath(double start, double end, double ratio)
        {
            if (SwCalcLogarithm)
                return Math.Exp(SafeLog(start) + ratio * (SafeLog(end) - SafeLog(start)));
            return start + ratio * (end - start);
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
                            if (UseSymmetry && CalcSymmetryNodesDown(i))
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
                            if (UseSymmetry && CalcSymmetryNodesDownModeLog(i))
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
                            if (UseSymmetry && CalcSymmetryNodesUp(i))
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
                            if (UseSymmetry && CalcSymmetryNodesUpModeLog(i))
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
            return ParseFontSize(UseSymmetry ? TextNodeSizeSym : TextNodeSizeNoSym, UseSymmetry ? 14 : 12);
        }

        private int TargetFontSizeValue()
        {
            return ParseFontSize(TargetLabelSize, 8);
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
            var sb = new System.Text.StringBuilder(256);
            sb.Append(lastIndex).Append('|')
              .Append(Bars.HighPrices[lastIndex].ToString("R")).Append('|')
              .Append(Bars.LowPrices[lastIndex].ToString("R")).Append('|')
              .Append(_nodesUp.Count).Append('|')
              .Append(_nodesDown.Count).Append('|')
              .Append(NodeDisplayMode).Append('|')
              .Append(EnableSingleNode1Targets).Append('|')
              .Append(EnableSingleNode2Targets).Append('|')
              .Append(EnablePairNode12Targets).Append('|')
              .Append(TargetGapBars).Append('|')
              .Append(DeleteHitTargets).Append('|')
              .Append(HitGraceBars).Append('|')
              .Append(SwCalcLogarithm).Append('|')
              .Append(UseSymmetry).Append('|')
              .Append(ShowRegularNodes).Append('|')
              .Append(ShowDoubleStarNodes).Append('|')
              .Append(ShowSesA).Append('|').Append(ShowSesB).Append('|')
              .Append(ShowSesC).Append('|').Append(ShowSesD).Append('|')
              .Append(EnableMapWeekly).Append('|')
              .Append(ShowDayOpenLine).Append('|')
              .Append(ShowDayCloseLine).Append('|')
              .Append(EnableTimeTargets);

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

        // ───────────────────────── Drawing ─────────────────────────

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

        private void ResetLabelStagger()
        {
            _stagUpPrice.Clear();
            _stagUpX.Clear();
            _stagDnPrice.Clear();
            _stagDnX.Clear();
            _stagOtPrice.Clear();
            _stagOtX.Clear();
        }

        private static bool PriceNearLabel(double a, double b, double tolPct)
        {
            double refP = Math.Max(Math.Max(Math.Abs(a), Math.Abs(b)), 1e-7);
            return Math.Abs(a - b) / refP * 100.0 <= tolPct;
        }

        private static bool PricesWithinTolPct(double a, double b, double tolPct)
        {
            if (double.IsNaN(a) || double.IsNaN(b) || Math.Abs(a) <= 0)
                return false;
            return (Math.Abs(a - b) / Math.Abs(a)) * 100.0 <= tolPct;
        }

        private double LabelYForRow(double price, int row)
        {
            double y = price + Math.Abs(price) * TargetLabelYOffsetPct / 100.0;
            if (row > 0)
                y += Math.Abs(price) * row * TargetLabelRowStepPct / 100.0;
            return y;
        }

        // lane 0 up, 1 down, 2 other.
        // skipStagger: انتهای خط راست | forceX: نقطهٔ اجباری (مثلاً ابتدای خط راست)
        private int GetStaggerX(double price, int baseX, int lane, int lineEndX, bool skipStagger, int forceX = int.MinValue)
        {
            List<double> prices = lane == 0 ? _stagUpPrice : lane == 1 ? _stagDnPrice : _stagOtPrice;
            List<int> xs = lane == 0 ? _stagUpX : lane == 1 ? _stagDnX : _stagOtX;
            int lastIndex = Bars.Count - 1;
            int lineLen = Math.Max(lineEndX - lastIndex, 10);
            int lo = lane == 2 ? lastIndex : Math.Max(baseX, lastIndex + (int)(lineLen * 0.45));
            int hi = lastIndex + (int)Math.Round(lineLen * 0.85);
            hi = Math.Max(lo + 1, Math.Min(hi, lineEndX - 1));

            int slot = 0;
            if (!skipStagger && forceX == int.MinValue && xs.Count > 0)
            {
                int from = Math.Max(0, xs.Count - StaggerScanCap);
                for (int i = from; i < xs.Count; i++)
                {
                    if (PriceNearLabel(prices[i], price, TargetLabelPriceTolerancePct))
                        slot++;
                }
            }

            int x = lo;
            if (forceX != int.MinValue)
                x = forceX;
            else if (skipStagger)
                x = lineEndX;
            else if (TargetLabelColumns > 1)
            {
                int cols = Math.Max(TargetLabelColumns, 1);
                double colGap = (hi - lo) / (double)(cols - 1);
                int perBand = 2 * cols - 1;
                int p = slot % perBand;
                x = lo + (int)Math.Round(p < cols ? p * colGap : (p - cols) * colGap + colGap / 2.0);
                x = Math.Min(Math.Max(x, lo), hi);
            }
            else
            {
                int step = Math.Max(TargetLabelStaggerStep, 4);
                int span = Math.Max(hi - lo, step);
                x = lo + slot * step;
                if (x > hi)
                    x = lo + (slot * step) % span;
                x = Math.Min(Math.Max(x, lo), hi);
            }

            prices.Add(price);
            xs.Add(x);
            return x;
        }

        private int GetStaggerRow(double price, int lane, bool skipStagger)
        {
            if (skipStagger || TargetLabelColumns <= 1)
                return 0;
            List<double> prices = lane == 0 ? _stagUpPrice : lane == 1 ? _stagDnPrice : _stagOtPrice;
            int slot = 0;
            int n = prices.Count - 1;
            int from = Math.Max(0, n - StaggerScanCap);
            for (int i = from; i < n; i++)
            {
                if (PriceNearLabel(prices[i], price, TargetLabelPriceTolerancePct))
                    slot++;
            }
            int cols = Math.Max(TargetLabelColumns, 1);
            int perBand = 2 * cols - 1;
            return slot / perBand;
        }

        private void DrawTargetLineLabel(int startIdx, double price, Color lineCol, int width, string styleStr,
            int transp, string txt, int gapBars, int lane, bool pinToStart, bool labelBeforeLine = false)
        {
            if (double.IsNaN(price) || double.IsInfinity(price))
                return;

            int lastIndex = Bars.Count - 1;
            int lineEnd = lastIndex + Math.Max(gapBars, 10);
            int lineStart = Math.Max(startIdx, lastIndex - 490);
            lineStart = Math.Max(0, Math.Min(lineStart, lineEnd - 1));
            int baseX = lane == 2 ? lastIndex : lastIndex + Math.Max(gapBars, 10) / 2;
            int lblX = labelBeforeLine
                ? GetStaggerX(price, baseX, lane, lineEnd, true, lastIndex)
                : GetStaggerX(price, baseX, lane, lineEnd, pinToStart);
            int row = GetStaggerRow(price, lane, pinToStart || labelBeforeLine);

            Color drawCol = WithAlpha(lineCol, transp);
            Chart.DrawTrendLine(NextName("Tgt"), TimeAtIndex(lineStart), price, TimeAtIndex(lineEnd), price,
                drawCol, width, ParseLineStyle(styleStr));

            var text = Chart.DrawText(NextName("TgtLbl"), txt, TimeAtIndex(lblX), LabelYForRow(price, row), lineCol);
            text.FontSize = TargetFontSizeValue();
            text.VerticalAlignment = VerticalAlignment.Center;
            text.HorizontalAlignment = labelBeforeLine ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        private TimeSpan BarDurationAt(int barIdx)
        {
            barIdx = ClampIndex(barIdx);
            if (Bars.Count >= 2)
            {
                if (barIdx + 1 < Bars.Count)
                {
                    TimeSpan d = Bars.OpenTimes[barIdx + 1] - Bars.OpenTimes[barIdx];
                    if (d > TimeSpan.Zero)
                        return d;
                }
                if (barIdx > 0)
                {
                    TimeSpan d = Bars.OpenTimes[barIdx] - Bars.OpenTimes[barIdx - 1];
                    if (d > TimeSpan.Zero)
                        return d;
                }
            }
            return TimeSpan.FromMinutes(1);
        }

        private void DrawCircleMarker(int barIdx, double price, double radiusPct, int radiusBars, Color lineCol, int fillTransp)
        {
            if (double.IsNaN(price) || radiusPct <= 0 || radiusBars <= 0)
                return;

            double rPrice = Math.Abs(price) * radiusPct / 100.0;
            if (rPrice <= 0)
                return;

            // روی رنج، barIdx±N در زمان می‌تواند ساعت‌ها خالی باشد و بیضی در هوا کشیده شود.
            // عرض دایره = مدت همان کندل × شعاع کندلی؛ مرکز = وسط بدنهٔ همان کندل.
            barIdx = ClampIndex(barIdx);
            TimeSpan barDur = BarDurationAt(barIdx);
            DateTime tOpen = Bars.OpenTimes[barIdx];
            DateTime tCenter = tOpen + TimeSpan.FromTicks(barDur.Ticks / 2);
            long halfTicks = barDur.Ticks * Math.Max(radiusBars, 1);
            DateTime t1 = tCenter - TimeSpan.FromTicks(halfTicks);
            DateTime t2 = tCenter + TimeSpan.FromTicks(halfTicks);
            if (t2 <= t1)
                t2 = t1.AddMinutes(1);

            Color fill = WithAlpha(lineCol, fillTransp);
            var el = Chart.DrawEllipse(NextName("Circ"), t1, price + rPrice, t2, price - rPrice, fill);
            el.IsFilled = true;
            el.Color = fill;
            el.Thickness = 1;
        }

        private void DrawingNumberNodes(bool swUptrendType, bool underline, int extraGapLines, int labelPadLines)
        {
            var nodes = swUptrendType ? _nodesUp : _nodesDown;
            if (nodes.Count == 0)
                return;

            int indexColor = 0;
            int fontSize = FontSizeValue();
            Color incColor = ParseColor(IncompleteNode2CircleColorName, Color.Red);

            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                bool shouldDraw = node.IsSymmetrySetup ? ShowDoubleStarNodes : ShowRegularNodes;
                if (shouldDraw)
                {
                    int barIdx = ClampIndex(node.IndexNode);
                    Color textColor = ColorAt(indexColor);
                    string newText = node.NumberNode.ToString();
                    if (underline)
                        newText += "\n―";
                    if (i > 0 && nodes[i - 1].IndexNode == node.IndexNode)
                        newText = "     " + newText;

                    int padN = Math.Max(labelPadLines, 0);
                    if (node.IsSymmetrySetup)
                        padN += Math.Max(extraGapLines, 0);
                    if (padN > 0)
                    {
                        string pad = new string('\n', padN);
                        newText = swUptrendType ? newText + pad : pad + newText;
                    }

                    var text = Chart.DrawText(NextName(swUptrendType ? "UpNum" : "DnNum"),
                        newText, Bars.OpenTimes[barIdx], node.HighNode, textColor);
                    text.FontSize = fontSize;
                    text.VerticalAlignment = swUptrendType ? VerticalAlignment.Top : VerticalAlignment.Bottom;
                    text.HorizontalAlignment = HorizontalAlignment.Center;
                }

                if (IsIncompleteNode2(nodes, i, swUptrendType))
                    DrawCircleMarker(ClampIndex(node.IndexNode), node.HighNode, IncompleteNode2RadiusPct,
                        IncompleteNode2RadiusBars, incColor, IncompleteNode2FillTransparency);

                AdvanceColor(ref indexColor, nodes, i);
            }
        }

        private bool IsIncompleteNode2(List<Node> nodes, int i, bool swUptrendType)
        {
            if (i <= 0 || nodes[i].NumberNode != 2 || nodes[i - 1].NumberNode != 1)
                return false;

            var n1 = nodes[i - 1];
            var n2 = nodes[i];
            double move = TgtMove(n1.LowPreNode, n1.HighNode);
            double minPrice = TgtProject(n1.LowCorrection, move, swUptrendType);
            if (double.IsNaN(minPrice) || double.IsInfinity(minPrice))
                return false;

            return swUptrendType ? n2.HighNode < minPrice : n2.HighNode > minPrice;
        }

        private Color[] ComputeNodeColors(List<Node> nodes)
        {
            var result = new Color[nodes.Count];
            int indexColor = 0;
            for (int i = 0; i < nodes.Count; i++)
            {
                result[i] = ColorAt(indexColor);
                AdvanceColor(ref indexColor, nodes, i);
            }
            return result;
        }

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

            int skip = Math.Max(qualifies.Count - TargetNode1MaxCount, 0);
            string trendPrefix = swUptrendType ? "H " : "L ";
            int lane = swUptrendType ? 0 : 1;
            Color[] colorByNode = ComputeNodeColors(nodes);
            int lastIndex = Bars.Count - 1;
            Color proxColor = ParseColor(ProximityMarkerColorName, Color.Green);

            for (int q = skip; q < qualifies.Count; q++)
            {
                int i = qualifies[q];
                var node = nodes[i];
                Color lineColor = colorByNode[i];
                double moveSizeLog = TgtMove(node.LowPreNode, node.HighNode);
                double correctionSizeLog = TgtMove(node.LowCorrection, node.HighNode);
                double targetDoublePrice = TgtProject(node.HighNode, moveSizeLog, swUptrendType);

                if (ShowDouble && !ShouldDeleteHitTarget(targetDoublePrice, node.IndexNode, swUptrendType))
                    DrawTargetLineLabel(node.IndexNode, targetDoublePrice, lineColor, DoubleLineWidth, DoubleLineStyle,
                        TargetTransparency, trendPrefix + "Double.1", TargetGapBars, lane, false);

                bool hasNode2 = i + 1 < nodes.Count && nodes[i + 1].NumberNode == 2;
                bool node2Incomplete = hasNode2 && IsIncompleteNode2(nodes, i + 1, swUptrendType);
                bool node2NotComplete = !hasNode2 || node2Incomplete;

                if (ShowDouble15 && !hasNode2)
                {
                    double p15 = TgtProject(node.HighNode, moveSizeLog * Double15Ratio, swUptrendType);
                    if (!ShouldDeleteHitTarget(p15, node.IndexNode, swUptrendType))
                        DrawTargetLineLabel(node.IndexNode, p15, lineColor, Double15LineWidth, Double15LineStyle,
                            TargetTransparency, trendPrefix + "1.5DL.1", TargetGapBars, lane, false);
                }

                double proxMin = double.NaN;
                double prox086 = double.NaN;

                if (ShowDouble086 && !hasNode2)
                {
                    double p086 = TgtAlong(node.LowPreNode, targetDoublePrice, Double086Ratio);
                    if (!ShouldDeleteHitTarget(p086, node.IndexNode, swUptrendType))
                    {
                        prox086 = p086;
                        DrawTargetLineLabel(node.IndexNode, p086, lineColor, Double086LineWidth, Double086LineStyle,
                            TargetTransparency, trendPrefix + "0.8DL.1", TargetGapBars, lane, false);
                    }
                }

                if (ShowMin)
                {
                    double minPrice = TgtProject(node.LowCorrection, moveSizeLog, swUptrendType);
                    if (!ShouldDeleteHitTarget(minPrice, node.IndexCorrection, swUptrendType))
                    {
                        proxMin = minPrice;
                        DrawTargetLineLabel(node.IndexNode, minPrice, lineColor, MinLineWidth, MinLineStyle,
                            TargetTransparency, trendPrefix + "Min 1", TargetGapBars, lane, false);
                    }

                    if (node2Incomplete)
                    {
                        double min085 = TgtProject(node.LowCorrection, moveSizeLog * Min085Ratio, swUptrendType);
                        if (!ShouldDeleteHitTarget(min085, node.IndexCorrection, swUptrendType))
                            DrawTargetLineLabel(node.IndexNode, min085, lineColor, MinLineWidth, MinLineStyle,
                                TargetTransparency, trendPrefix + "0.8Min.1", TargetGapBars, lane, false);
                    }
                }

                if (ShowMin && ShowDouble086 && !hasNode2 && PricesWithinTolPct(proxMin, prox086, MinDblProximityTolPct))
                    DrawCircleMarker(lastIndex, (proxMin + prox086) / 2.0, ProximityMarkerRadiusPct,
                        ProximityMarkerRadiusBars, proxColor, ProximityMarkerFillTransparency);

                if (ShowDouble && ShowTargetNode2 && hasNode2)
                {
                    var n2 = nodes[i + 1];
                    double n2Price = TgtProject(n2.LowCorrection, TgtMove(n2.LowPreNode, n2.HighNode), swUptrendType);
                    bool delDbl = ShouldDeleteHitTarget(targetDoublePrice, node.IndexNode, swUptrendType);
                    bool delN2 = ShouldDeleteHitTarget(n2Price, n2.IndexCorrection, swUptrendType);
                    if (!delDbl && !delN2 && PricesWithinTolPct(targetDoublePrice, n2Price, MinDblProximityTolPct))
                        DrawCircleMarker(lastIndex, (targetDoublePrice + n2Price) / 2.0, ProximityMarkerRadiusPct,
                            ProximityMarkerRadiusBars, proxColor, ProximityMarkerFillTransparency);
                }

                if (ShowBasedMin13 && moveSizeLog > 0 && correctionSizeLog >= moveSizeLog * BasedRetraceRatio && node2NotComplete)
                {
                    double based = TgtProject(node.LowCorrection, moveSizeLog * BasedMinExtRatio, swUptrendType);
                    if (!ShouldDeleteHitTarget(based, node.IndexCorrection, swUptrendType))
                        DrawTargetLineLabel(node.IndexNode, based, lineColor, MinLineWidth, MinLineStyle,
                            TargetTransparency, trendPrefix + "1.3MIN1", TargetGapBars, lane, false);
                }

                if (ShowCorrection)
                {
                    double corr = TgtProject(node.HighNode, correctionSizeLog, swUptrendType);
                    if (!ShouldDeleteHitTarget(corr, node.IndexNode, swUptrendType))
                        DrawTargetLineLabel(node.IndexNode, corr, lineColor, CorrectionLineWidth, CorrectionLineStyle,
                            TargetTransparency, trendPrefix + "Cr.1", TargetGapBars, lane, false);
                }
            }
        }

        private void DrawingTargetsNode2(bool swUptrendType)
        {
            var nodes = swUptrendType ? _nodesUp : _nodesDown;
            if (nodes.Count == 0)
                return;

            var qualifies = new List<int>();
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].NumberNode == 2)
                    qualifies.Add(i);
            }
            if (qualifies.Count == 0)
                return;

            int skip = Math.Max(qualifies.Count - TargetNode2MaxCount, 0);
            string trendPrefix = swUptrendType ? "H " : "L ";
            int lane = swUptrendType ? 0 : 1;
            Color[] colorByNode = ComputeNodeColors(nodes);

            for (int q = skip; q < qualifies.Count; q++)
            {
                int i = qualifies[q];
                var node = nodes[i];
                double price = TgtProject(node.LowCorrection, TgtMove(node.LowPreNode, node.HighNode), swUptrendType);
                if (ShouldDeleteHitTarget(price, node.IndexCorrection, swUptrendType))
                    continue;
                DrawTargetLineLabel(node.IndexNode, price, colorByNode[i], Node2LineWidth, Node2LineStyle,
                    TargetTransparency, trendPrefix + "Node 2", TargetGapBars, lane, false);
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

            int skip = Math.Max(pairStarts.Count - PairTargetMaxCount, 0);
            string trendPrefix = swUptrendType ? "H " : "L ";
            int lane = swUptrendType ? 0 : 1;
            Color[] colorByNode = ComputeNodeColors(nodes);

            for (int p = skip; p < pairStarts.Count; p++)
            {
                int i = pairStarts[p];
                var node1 = nodes[i];
                var node2 = nodes[i + 1];
                Color lineColor = colorByNode[i];
                double totalMove = TgtMove(node1.LowPreNode, node2.HighNode);

                if (ShowPairMin)
                {
                    double price = TgtProject(node1.LowCorrection, totalMove, swUptrendType);
                    if (!ShouldDeleteHitTarget(price, node1.IndexCorrection, swUptrendType))
                        DrawTargetLineLabel(node1.IndexNode, price, lineColor, PairMinWidth, PairMinStyle,
                            TargetTransparency, trendPrefix + "Min 12", TargetGapBars, lane, false);
                }

                if (ShowPairMax)
                {
                    double price = TgtProject(node2.LowCorrection, totalMove, swUptrendType);
                    if (!ShouldDeleteHitTarget(price, node2.IndexCorrection, swUptrendType))
                        DrawTargetLineLabel(node2.IndexNode, price, lineColor, PairMaxWidth, PairMaxStyle,
                            TargetTransparency, trendPrefix + "Max 12", TargetGapBars, lane, false);
                }

                if (ShowPairDouble)
                {
                    double price = TgtProject(node2.HighNode, totalMove, swUptrendType);
                    if (!ShouldDeleteHitTarget(price, node2.IndexNode, swUptrendType))
                        DrawTargetLineLabel(node2.IndexNode, price, lineColor, PairDoubleWidth, PairDoubleStyle,
                            TargetTransparency, trendPrefix + "Double 12", TargetGapBars, lane, false);
                }

                if (ShowPairCorrection)
                {
                    double corr = TgtMove(node2.LowCorrection, node2.HighNode);
                    double price = TgtProject(node2.HighNode, corr, swUptrendType);
                    if (!ShouldDeleteHitTarget(price, node2.IndexNode, swUptrendType))
                        DrawTargetLineLabel(node2.IndexNode, price, lineColor, PairCorrectionWidth, PairCorrectionStyle,
                            TargetTransparency, trendPrefix + "Correction 2", TargetGapBars, lane, false);
                }
            }
        }

        private void DrawRoundNumberTargets(bool isUp, bool drawVisual)
        {
            if (RoundBasePrice <= 0 || !drawVisual)
                return;

            int lastIndex = Bars.Count - 1;
            int lookback = Math.Min(StartPoint, lastIndex);
            double chartLow = Bars.LowPrices[lastIndex];
            double chartHigh = Bars.HighPrices[lastIndex];
            int from = Math.Max(0, lastIndex - lookback);
            for (int i = from; i <= lastIndex; i++)
            {
                if (Bars.LowPrices[i] < chartLow) chartLow = Bars.LowPrices[i];
                if (Bars.HighPrices[i] > chartHigh) chartHigh = Bars.HighPrices[i];
            }

            double effectiveMin = RoundMinVisiblePrice > 0 ? Math.Max(RoundMinVisiblePrice, chartLow) : chartLow;
            int startMult = (int)Math.Floor(effectiveMin / RoundBasePrice);
            int endMult = (int)Math.Ceiling(chartHigh / RoundBasePrice);
            if (startMult < 1) startMult = 1;
            if (endMult < startMult) return;

            int total = endMult - startMult + 1;
            if (total > MaxRoundLevels)
            {
                int mid = (startMult + endMult) / 2;
                int half = MaxRoundLevels / 2;
                startMult = Math.Max(1, mid - half);
                endMult = startMult + MaxRoundLevels - 1;
            }

            Color col = ParseColor(RoundLineColorName, Color.Gray);
            string prefix = isUp ? "H " : "L ";
            int lane = isUp ? 0 : 1;

            for (int mult = startMult; mult <= endMult; mult++)
            {
                double roundPrice = RoundBasePrice * mult;
                DrawTargetLineLabel(lastIndex, roundPrice, col, RoundLineWidth, RoundLineStyle,
                    RoundLineTransparency, prefix + "RN " + roundPrice.ToString("0.####", CultureInfo.InvariantCulture),
                    TargetGapBars, lane, false);
            }
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
            double openToday = _dailyBars.LastBar.Open;
            double closeYesterday = _dailyBars.Count >= 2 ? _dailyBars[_dailyBars.Count - 2].Close : double.NaN;
            Color openCol = ParseColor(OpenLineColorName, Color.Lime);
            Color closeCol = ParseColor(CloseLineColorName, Color.Red);

            if (ShowDayOpenLine && openToday > 0 && !double.IsNaN(openToday))
                DrawTargetLineLabel(lastIndex, openToday, openCol, OpenLineWidth, OpenLineStyle,
                    OpenLineTransparency, "H Day Open", TargetGapBars, 2, true);

            if (ShowDayCloseLine && closeYesterday > 0 && !double.IsNaN(closeYesterday))
                DrawTargetLineLabel(lastIndex, closeYesterday, closeCol, CloseLineWidth, CloseLineStyle,
                    CloseLineTransparency, "L Day Close", TargetGapBars, 2, true);

            if (ShowDayOCBox && openToday > 0 && closeYesterday > 0 && !double.IsNaN(openToday) && !double.IsNaN(closeYesterday))
            {
                double boxTop = Math.Max(openToday, closeYesterday);
                double boxBot = Math.Min(openToday, closeYesterday);
                Color bg = ParseColor(DayOCBoxBgColorName, Color.FromArgb(40, 0, 0, 255));
                var rect = Chart.DrawRectangle(NextName("DayOC"), TimeAtIndex(lastIndex), boxTop,
                    TimeAtIndex(lastIndex + TargetGapBars), boxBot, bg);
                rect.IsFilled = true;
                rect.Color = bg;
                rect.Thickness = 1;
                rect.LineStyle = LineStyle.Solid;
            }
        }

        private sealed class SessionBox
        {
            public DateTime StartTime;
            public DateTime EndTime;
            public double High;
            public double Low;
        }

        private static bool TryParseSession(string raw, out int startMin, out int endMin)
        {
            startMin = 0;
            endMin = 0;
            if (string.IsNullOrWhiteSpace(raw))
                return false;
            string s = raw.Trim().Replace(":", "");
            int dash = s.IndexOf('-');
            if (dash < 4 || s.Length < dash + 5)
                return false;
            int sh, sm, eh, em;
            if (!int.TryParse(s.Substring(0, 2), out sh) || !int.TryParse(s.Substring(2, 2), out sm))
                return false;
            if (!int.TryParse(s.Substring(dash + 1, 2), out eh) || !int.TryParse(s.Substring(dash + 3, 2), out em))
                return false;
            startMin = sh * 60 + sm;
            endMin = eh * 60 + em;
            return startMin != endMin;
        }

        private DateTime SessionLocalTime(DateTime barTime)
        {
            if (UseExchangeTimezone)
                return barTime;
            return barTime.AddHours(TzOffsetHours);
        }

        private DateTime ToBarTime(DateTime local)
        {
            if (UseExchangeTimezone)
                return local;
            return local.AddHours(-TzOffsetHours);
        }

        private void EnsureM1Bars()
        {
            if (_m1Bars != null && _m1Bars.Count > 0)
                return;
            try { _m1Bars = MarketData.GetBars(TimeFrame.Minute); } catch { }
        }

        private int FindFirstM1AtOrAfter(DateTime t)
        {
            if (_m1Bars == null || _m1Bars.Count == 0)
                return -1;
            int lo = 0;
            int hi = _m1Bars.Count - 1;
            int ans = _m1Bars.Count;
            while (lo <= hi)
            {
                int mid = lo + (hi - lo) / 2;
                if (_m1Bars.OpenTimes[mid] >= t)
                {
                    ans = mid;
                    hi = mid - 1;
                }
                else
                    lo = mid + 1;
            }
            return ans;
        }

        private bool FillSessionHlFromM1(DateTime start, DateTime endExclusive, out double high, out double low)
        {
            high = double.NaN;
            low = double.NaN;
            EnsureM1Bars();
            if (_m1Bars == null || _m1Bars.Count == 0 || endExclusive <= start)
                return false;

            int i0 = FindFirstM1AtOrAfter(start);
            if (i0 < 0 || i0 >= _m1Bars.Count)
                return false;

            for (int i = i0; i < _m1Bars.Count; i++)
            {
                DateTime ot = _m1Bars.OpenTimes[i];
                if (ot >= endExclusive)
                    break;
                double h = _m1Bars.HighPrices[i];
                double l = _m1Bars.LowPrices[i];
                high = double.IsNaN(high) ? h : Math.Max(high, h);
                low = double.IsNaN(low) ? l : Math.Min(low, l);
            }

            return !double.IsNaN(high) && !double.IsNaN(low) && high >= low;
        }

        private bool FillSessionHlFromChartOverlap(DateTime start, DateTime endExclusive, out double high, out double low)
        {
            high = double.NaN;
            low = double.NaN;
            int lastIndex = Bars.Count - 1;
            for (int i = 0; i <= lastIndex; i++)
            {
                DateTime t0 = Bars.OpenTimes[i];
                DateTime t1 = i + 1 <= lastIndex ? Bars.OpenTimes[i + 1] : t0 + BarDurationAt(i);
                if (t0 >= endExclusive || t1 <= start)
                    continue;
                double h = Bars.HighPrices[i];
                double l = Bars.LowPrices[i];
                high = double.IsNaN(high) ? h : Math.Max(high, h);
                low = double.IsNaN(low) ? l : Math.Min(low, l);
            }
            return !double.IsNaN(high) && !double.IsNaN(low) && high >= low;
        }

        private List<SessionBox> CollectClockSessionBoxes(int startMin, int endMin)
        {
            var boxes = new List<SessionBox>();
            int lastIndex = Bars.Count - 1;
            DateTime nowBar = Bars.OpenTimes[lastIndex];
            if (Server != null && Server.Time > nowBar)
                nowBar = Server.Time;

            DateTime nowLocal = SessionLocalTime(nowBar);
            int keep = Math.Max(1, Math.Min(10, SessionDisplayDays));

            for (int d = 0; d <= keep + 2; d++)
            {
                DateTime date = nowLocal.Date.AddDays(-d);
                DateTime startLocal;
                DateTime endLocal;
                if (startMin < endMin)
                {
                    startLocal = date.AddMinutes(startMin);
                    endLocal = date.AddMinutes(endMin);
                }
                else
                {
                    startLocal = date.AddMinutes(startMin);
                    endLocal = date.AddDays(1).AddMinutes(endMin);
                }

                DateTime start = ToBarTime(startLocal);
                DateTime end = ToBarTime(endLocal);
                if (end <= start || start > nowBar)
                    continue;

                DateTime hlEnd = nowBar < end ? nowBar.AddMinutes(1) : end;
                double high, low;
                if (!FillSessionHlFromM1(start, hlEnd, out high, out low))
                {
                    if (!FillSessionHlFromChartOverlap(start, hlEnd, out high, out low))
                        continue;
                }

                boxes.Add(new SessionBox
                {
                    StartTime = start,
                    EndTime = end,
                    High = high,
                    Low = low
                });
            }

            boxes.Sort((a, b) => a.StartTime.CompareTo(b.StartTime));
            if (boxes.Count > keep)
                boxes.RemoveRange(0, boxes.Count - keep);
            return boxes;
        }

        private void DrawOneSession(bool show, bool showRange, string name, string session, string colorName,
            bool showMid, int midExt, bool extendToLast, out SessionBox latest)
        {
            latest = null;
            if (!show || !showRange)
                return;
            int startMin, endMin;
            if (!TryParseSession(session, out startMin, out endMin))
                return;

            var boxes = CollectClockSessionBoxes(startMin, endMin);
            if (boxes.Count == 0)
                return;

            Color css = ParseColor(colorName, Color.Yellow);
            Color fill = WithAlpha(css, (int)Math.Round(RangeBgTransparency));
            Color outline = WithAlpha(css, OutlineTransparency);
            Color midCol = WithAlpha(css, MidlineTransparency);
            LineStyle outlineStyle = ParseLineStyle(OutlineStyleName);
            LineStyle midStyle = ParseLineStyle(MidlineStyleName);
            int lastIndex = Bars.Count - 1;
            DateTime nowBar = Bars.OpenTimes[lastIndex];
            if (Server != null && Server.Time > nowBar)
                nowBar = Server.Time;
            string label = string.IsNullOrWhiteSpace(name) ? "Session" : name.Trim();

            for (int b = 0; b < boxes.Count; b++)
            {
                var box = boxes[b];
                DateTime t1 = box.StartTime;
                bool sessionEnded = nowBar >= box.EndTime;
                DateTime t2 = sessionEnded ? box.EndTime : nowBar;
                if (t2 <= t1)
                    t2 = t1.AddMinutes(1);

                DateTime extendEnd = t2 + TimeSpan.FromTicks(BarDurationAt(lastIndex).Ticks * Math.Max(0, midExt));
                if (extendToLast && sessionEnded)
                    extendEnd = Bars.OpenTimes[lastIndex];
                if (extendEnd <= t1)
                    extendEnd = t2;

                var rect = Chart.DrawRectangle(NextName("SesBox"), t1, box.High, t2, box.Low, fill);
                rect.IsFilled = true;
                rect.Color = fill;
                rect.Thickness = ShowRangeOutline && !extendToLast ? OutlineWidth : 0;
                rect.LineStyle = outlineStyle;

                if (ShowRangeOutline)
                {
                    Chart.DrawTrendLine(NextName("SesHi"), t1, box.High, extendToLast ? extendEnd : t2, box.High, outline, OutlineWidth, outlineStyle);
                    Chart.DrawTrendLine(NextName("SesLo"), t1, box.Low, extendToLast ? extendEnd : t2, box.Low, outline, OutlineWidth, outlineStyle);
                    Chart.DrawTrendLine(NextName("SesEdge"), t1, box.High, t1, box.Low, outline, OutlineWidth, outlineStyle);
                    if (!extendToLast)
                        Chart.DrawTrendLine(NextName("SesEdge"), t2, box.High, t2, box.Low, outline, OutlineWidth, outlineStyle);
                }

                if (showMid)
                {
                    double mid = (box.High + box.Low) / 2.0;
                    Chart.DrawTrendLine(NextName("SesMid"), t1, mid, extendEnd, mid, midCol, MidlineWidth, midStyle);
                }

                if (ShowRangeLabel)
                {
                    DateTime midT = new DateTime(t1.Ticks + (t2.Ticks - t1.Ticks) / 2, t1.Kind);
                    var txt = Chart.DrawText(NextName("SesLbl"), label, midT, box.High, css);
                    txt.FontSize = 8;
                    txt.VerticalAlignment = VerticalAlignment.Top;
                    txt.HorizontalAlignment = HorizontalAlignment.Center;
                }
            }

            latest = boxes[boxes.Count - 1];
        }

        private void DrawSessions()
        {
            DrawOneSession(ShowSesA, SesARange, SesAName, SesASession, SesAColorName, SesAMidline, SesAMidlineLength, false, out _lastSesA);
            DrawOneSession(ShowSesB, SesBRange, SesBName, SesBSession, SesBColorName, SesBMidline, SesBMidlineLength, false, out _lastSesB);
            DrawOneSession(ShowSesC, SesCRange, SesCName, SesCSession, SesCColorName, SesCMidline, SesCMidlineLength, true, out _lastSesC);
            DrawOneSession(ShowSesD, SesDRange, SesDName, SesDSession, SesDColorName, SesDMidline, SesDMidlineLength, false, out _lastSesD);

            if (!EnableSessionTargets)
                return;

            RegisterSessionTargets(_lastSesA, SesAName, SesAColorName);
            RegisterSessionTargets(_lastSesB, SesBName, SesBColorName);
            RegisterSessionTargets(_lastSesC, SesCName, SesCColorName);
            RegisterSessionTargets(_lastSesD, SesDName, SesDColorName);
        }

        private void RegisterSessionTargets(SessionBox box, string name, string colorName)
        {
            if (box == null)
                return;
            int lastIndex = Bars.Count - 1;
            Color col = ParseColor(colorName, Color.Yellow);
            string label = string.IsNullOrWhiteSpace(name) ? "Session" : name.Trim();

            if (ApplySessionHigh)
                DrawTargetLineLabel(lastIndex, box.High, col, SessionHighLineWidth, SessionTargetLineStyle,
                    SessionTargetTransparency, label + " High", TargetGapBars, 2, true);
            if (ApplySessionLow)
                DrawTargetLineLabel(lastIndex, box.Low, col, SessionLowLineWidth, SessionTargetLineStyle,
                    SessionTargetTransparency, label + " Low", TargetGapBars, 2, true);
            if (ApplySessionMid)
            {
                double mid = (box.High + box.Low) / 2.0;
                DrawTargetLineLabel(lastIndex, mid, col, SessionMidLineWidth, SessionTargetLineStyle,
                    SessionTargetTransparency, label + " Mid", TargetGapBars, 2, true);
            }
        }

        private void DrawMapWeekly()
        {
            if (_weeklyBars == null)
            {
                try { _weeklyBars = MarketData.GetBars(TimeFrame.Weekly); } catch { return; }
            }
            if (_weeklyBars == null || _weeklyBars.Count < 2)
                return;

            var prev = _weeklyBars[_weeklyBars.Count - 2];
            double pwHigh = prev.High;
            double pwLow = prev.Low;
            if (!(pwHigh > pwLow))
                return;

            double range = pwHigh - pwLow;
            int lastIndex = Bars.Count - 1;
            DateTime weekStart = prev.OpenTime.AddDays(7);
            int startIdx = lastIndex;
            for (int i = lastIndex; i >= 0; i--)
            {
                if (Bars.OpenTimes[i] < weekStart)
                    break;
                startIdx = i;
            }
            startIdx = Math.Max(0, Math.Min(startIdx, lastIndex));
            int endIdx = lastIndex + Math.Max(MapExtendBars, 10);
            Color col = WithAlpha(ParseColor(MapLineColorName, Color.FromArgb(255, 192, 132, 252)), MapLineTransparency);
            int mapFont = ParseFontSize(MapLabelSize, 10);

            var levels = new List<Tuple<double, int, string>>();
            Action<double, int, string> add = (p, w, st) => levels.Add(Tuple.Create(p, w, st));
            if (MapShowHigh) add(pwHigh, MapKeyLineWidth, MapKeyLineStyle);
            if (MapShowLow) add(pwLow, MapKeyLineWidth, MapKeyLineStyle);
            if (MapShowMid) add((pwHigh + pwLow) / 2.0, MapMidLineWidth, MapMidLineStyle);
            if (MapShow25) add(pwLow + range * 0.25, MapMidLineWidth, MapMidLineStyle);
            if (MapShow75) add(pwLow + range * 0.75, MapMidLineWidth, MapMidLineStyle);
            if (MapShowExtAbove)
            {
                if (MapShow1125) add(pwHigh + range * 0.125, MapExtLineWidth, MapExtLineStyle);
                if (MapShow125) add(pwHigh + range * 0.25, MapExtLineWidth, MapExtLineStyle);
                if (MapShow1375) add(pwHigh + range * 0.375, MapExtLineWidth, MapExtLineStyle);
                if (MapShow150) add(pwHigh + range * 0.50, MapExtLineWidth, MapExtLineStyle);
                if (MapShow175) add(pwHigh + range * 0.75, MapExtLineWidth, MapExtLineStyle);
                if (MapShow200) add(pwHigh + range * 1.00, MapExtLineWidth, MapExtLineStyle);
            }
            if (MapShowExtBelow)
            {
                if (MapShow1125) add(pwLow - range * 0.125, MapExtLineWidth, MapExtLineStyle);
                if (MapShow125) add(pwLow - range * 0.25, MapExtLineWidth, MapExtLineStyle);
                if (MapShow1375) add(pwLow - range * 0.375, MapExtLineWidth, MapExtLineStyle);
                if (MapShow150) add(pwLow - range * 0.50, MapExtLineWidth, MapExtLineStyle);
                if (MapShow175) add(pwLow - range * 0.75, MapExtLineWidth, MapExtLineStyle);
                if (MapShow200) add(pwLow - range * 1.00, MapExtLineWidth, MapExtLineStyle);
            }

            double close = Bars.ClosePrices[lastIndex];
            levels.Sort((a, b) => Math.Abs(a.Item1 - close).CompareTo(Math.Abs(b.Item1 - close)));

            DateTime t1 = TimeAtIndex(startIdx);
            DateTime t2 = TimeAtIndex(endIdx);
            for (int i = 0; i < levels.Count; i++)
            {
                double price = levels[i].Item1;
                Chart.DrawTrendLine(NextName("Map"), t1, price, t2, price, col, levels[i].Item2, ParseLineStyle(levels[i].Item3));
                if (MapShowLabels)
                {
                    var txt = Chart.DrawText(NextName("MapLbl"), "W.L", t2, LabelYForRow(price, 0), col);
                    txt.FontSize = mapFont;
                    txt.VerticalAlignment = VerticalAlignment.Center;
                    txt.HorizontalAlignment = HorizontalAlignment.Left;
                }
            }
        }

        private TimeSpan TimeTargetOffset()
        {
            if (string.Equals(TimeTargetTzMode, "Exchange", StringComparison.OrdinalIgnoreCase))
                return TimeSpan.Zero;
            if (string.Equals(TimeTargetTzMode, "Same as Sessions", StringComparison.OrdinalIgnoreCase))
                return TimeSpan.FromHours(TzOffsetHours);
            return TimeSpan.FromHours(TimeTargetUtcOffset);
        }

        private void DrawTimeTargets()
        {
            if (string.IsNullOrWhiteSpace(TimeTargetsCsv))
                return;

            var times = new List<Tuple<int, int, string>>();
            string[] parts = TimeTargetsCsv.Split(',');
            for (int i = 0; i < parts.Length; i++)
            {
                string raw = parts[i].Trim().Replace(" ", "");
                string[] hm = raw.Split(':');
                int hh, mm;
                if (hm.Length < 2 || !int.TryParse(hm[0], out hh) || !int.TryParse(hm[1], out mm))
                    continue;
                if (hh < 0 || hh > 23 || mm < 0 || mm > 59)
                    continue;
                times.Add(Tuple.Create(hh, mm, hh.ToString("00") + ":" + mm.ToString("00")));
            }
            if (times.Count == 0)
                return;

            TimeSpan offset = TimeTargetOffset();
            DateTime nowLocal = Server.Time.Add(offset);
            Color col = WithAlpha(ParseColor(TimeTargetColorName, Color.FromArgb(255, 120, 144, 156)), TimeTargetTransparency);
            int font = ParseFontSize(TimeTargetLabelSize, 8);
            LineStyle style = ParseLineStyle(TimeTargetStyle);
            int lastIndex = Bars.Count - 1;
            double labelY = Bars.HighPrices[lastIndex];

            int days = Math.Max(1, TimeTargetDisplayDays);
            for (int d = 0; d < days; d++)
            {
                DateTime day = nowLocal.Date.AddDays(-d);
                for (int t = 0; t < times.Count; t++)
                {
                    DateTime localStamp = day.AddHours(times[t].Item1).AddMinutes(times[t].Item2);
                    DateTime utcStamp = localStamp - offset;
                    Chart.DrawVerticalLine(NextName("TT"), utcStamp, col, TimeTargetWidth, style);
                    if (ShowTimeTargetLabels)
                    {
                        var txt = Chart.DrawText(NextName("TTLbl"), times[t].Item3, utcStamp, labelY, col);
                        txt.FontSize = font;
                        txt.VerticalAlignment = VerticalAlignment.Top;
                        txt.HorizontalAlignment = HorizontalAlignment.Center;
                    }
                }
            }
        }

    }
}
