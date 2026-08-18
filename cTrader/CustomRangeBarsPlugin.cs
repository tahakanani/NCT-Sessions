// Custom Range / Tick Bars for cTrader
// Always shows TargetChartBars (4000) custom candles on the current timeframe.
using System;
using System.Collections.Generic;
using System.Globalization;
using cAlgo.API;
using cAlgo.API.Internals;

namespace cAlgo.Plugins
{
    [Plugin(AccessRights = AccessRights.None)]
    public class CustomRangeBarsPlugin : Plugin
    {
        private const int TargetChartBars = 5000; // هم‌تراز با StartPoint پیش‌فرض NCT
        private const int MaxM1Bars = 80000;      // سقف ایمنی برای لود M1

        private ComboBox _modeBox;
        private TextBlock _sizeLabel;
        private TextBox _sizeBox;
        private TextBlock _statusText;

        private readonly Dictionary<string, CustomFrame> _frames =
            new Dictionary<string, CustomFrame>(StringComparer.OrdinalIgnoreCase);

        protected override void OnStart()
        {
            BuildUi();
            Timer.Start(TimeSpan.FromSeconds(1));
        }

        protected override void OnTimer()
        {
            foreach (var f in _frames.Values)
                f.Pulse();
        }

        protected override void OnStop()
        {
            try { Timer.Stop(); } catch { }
            _frames.Clear();
        }

        // ───────────────────────── UI ─────────────────────────

        private void BuildUi()
        {
            var panel = new StackPanel { Orientation = Orientation.Vertical, Margin = 8 };

            panel.AddChild(new TextBlock
            {
                Text = "Custom Bars",
                FontWeight = FontWeight.Bold,
                FontSize = 14,
                Margin = 4
            });

            _modeBox = new ComboBox { Width = 170, Margin = 4 };
            _modeBox.AddItem("Range (pips)");
            _modeBox.AddItem("Tick (count)");
            _modeBox.SelectedItem = "Range (pips)";
            _modeBox.SelectedItemChanged += _ =>
            {
                bool tick = IsTick();
                _sizeLabel.Text = tick ? "Size (ticks)" : "Size (pips)";
                _sizeBox.Text = tick ? "100" : "7";
            };
            panel.AddChild(_modeBox);

            _sizeLabel = new TextBlock { Text = "Size (pips)", Margin = 4 };
            panel.AddChild(_sizeLabel);

            _sizeBox = new TextBox { Text = "7", Width = 120, Margin = 4 };
            panel.AddChild(_sizeBox);

            var btn = new Button { Text = "Add Timeframe", Margin = 4 };
            btn.Click += OnAdd;
            panel.AddChild(btn);

            _statusText = new TextBlock
            {
                Text = "Add → Period",
                TextWrapping = TextWrapping.Wrap,
                Margin = 4,
                Width = 210
            };
            panel.AddChild(_statusText);

            Asp.SymbolTab.AddBlock("Custom Range / Tick Bars").Child = panel;
        }

        private bool IsTick()
        {
            return (_modeBox.SelectedItem ?? "").StartsWith("Tick", StringComparison.OrdinalIgnoreCase);
        }

        private void OnAdd(ButtonClickEventArgs args)
        {
            bool tick = IsTick();
            double size;

            if (tick)
            {
                if (!TryParse(_sizeBox.Text, out size) || size < 1 || size != Math.Floor(size))
                {
                    Status("عدد تیک نامعتبر (مثلاً 100)");
                    return;
                }
            }
            else
            {
                if (!TryParse(_sizeBox.Text, out size))
                {
                    Status("عدد رنج نامعتبر (مثلاً 7)");
                    return;
                }
            }

            string name = (tick ? "Tick " : "Range ") + Fmt(size);
            if (_frames.ContainsKey(name))
            {
                Status("قبلاً هست: " + name);
                return;
            }

            var tf = TimeFrameManager.Custom.Add(name);
            tf.Description = name + " bars";
            var frame = new CustomFrame(this, tf, tick, size);
            tf.BarsNeeded = frame.OnBarsNeeded;
            tf.BarsUnloaded += frame.OnBarsUnloaded;
            _frames[name] = frame;
            Status("OK: " + name + " → Period");
        }

        internal void Status(string s)
        {
            if (_statusText != null)
                _statusText.Text = s;
            Print(s);
        }

        private static bool TryParse(string s, out double v)
        {
            v = 0;
            return !string.IsNullOrWhiteSpace(s)
                && double.TryParse(s.Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out v)
                && v > 0 && !double.IsNaN(v) && !double.IsInfinity(v);
        }

        private static string Fmt(double v)
        {
            return v == Math.Floor(v)
                ? ((long)v).ToString(CultureInfo.InvariantCulture)
                : v.ToString("0.##", CultureInfo.InvariantCulture);
        }

        // ───────────────────────── Frame ─────────────────────────

        private sealed class CustomFrame
        {
            private readonly CustomRangeBarsPlugin _p;
            private readonly CustomTimeFrame _tf;
            private readonly bool _isTick;
            private readonly double _size;

            private CustomBars _cb;
            private List<CustomBar> _cache;   // completed bars (rebuilt after each unload)
            private bool _building;

            // forming bar
            private bool _forming;
            private double _o, _h, _l, _c;
            private long _v;
            private DateTime _lastT;

            public CustomFrame(CustomRangeBarsPlugin p, CustomTimeFrame tf, bool isTick, double size)
            {
                _p = p;
                _tf = tf;
                _isTick = isTick;
                _size = size;
            }

            // وقتی از تایم‌فریم خارج می‌شوی: کش و همهٔ وضعیت پاک می‌شود
            public void OnBarsUnloaded(CustomTimeFrameBarsUnloadedEventArgs args)
            {
                try
                {
                    _cb = null;
                    _cache = null;
                    _forming = false;
                    _v = 0;
                    _lastT = DateTime.MinValue;

                    try { _tf.RemoveBars(args.CustomBars.Symbol); } catch { }
                    try { _tf.RemoveAllUnloadedBars(); } catch { }

                    _p.Status(_tf.Name + ": کش پاک شد");
                }
                catch { }
            }

            // BarsNeeded runs on its own thread in plugins → sync loading is OK (official sample even uses Sleep)
            public void OnBarsNeeded(CustomTimeFrameBarsNeededArgs args)
            {
                var cb = args.CustomBars;
                if (cb.Bars.Count > 0)
                    return;

                if (_building)
                    return;
                _building = true;

                try
                {
                    _cb = cb;

                    if (_cache == null)
                    {
                        var m1 = _p.MarketData.GetBars(TimeFrame.Minute, cb.Symbol.Name);
                        if (m1 == null || m1.Count == 0)
                        {
                            _p.Status("دیتای M1 نیست");
                            return;
                        }

                        double range = _size * cb.Symbol.PipSize;
                        var candles = BuildExactCount(m1, range);
                        _cache = Timestamp(candles, m1.LastBar.OpenTime);
                    }

                    if (_cache.Count == 0)
                    {
                        _p.Status("کندلی ساخته نشد — سایز را کوچکتر کن");
                        return;
                    }

                    cb.AppendBars(_cache);
                    _lastT = _cache[_cache.Count - 1].Time;

                    if (_forming)
                    {
                        _lastT = _lastT.AddMinutes(1);
                        cb.AppendBar(new CustomBar(_lastT, _o, _h, _l, _c, _v));
                    }

                    _p.Status(_cache.Count.ToString("N0") + " کندل روی چارت");
                }
                catch (Exception ex)
                {
                    _p.Status("خطا: " + ex.Message);
                }
                finally
                {
                    _building = false;
                }
            }

            // live update from timer (1s) — lightweight
            public void Pulse()
            {
                var cb = _cb;
                if (cb == null || _building || cb.Bars.Count == 0)
                    return;

                try
                {
                    if (!cb.IsLoaded)
                        return;

                    double price = cb.Symbol.Bid;
                    if (price <= 0)
                        return;

                    if (!_forming)
                    {
                        _forming = true;
                        _o = _h = _l = _c = price;
                        _v = 1;
                        _lastT = _lastT == DateTime.MinValue ? cb.Bars.LastBar.OpenTime.AddMinutes(1) : _lastT.AddMinutes(1);
                        cb.AppendBar(new CustomBar(_lastT, _o, _h, _l, _c, _v));
                        return;
                    }

                    _v++;
                    _c = price;
                    if (price > _h) _h = price;
                    if (price < _l) _l = price;

                    bool closeBar;
                    double closePrice = price;

                    if (_isTick)
                    {
                        closeBar = _v >= (long)_size;
                    }
                    else
                    {
                        double range = _size * cb.Symbol.PipSize;
                        closeBar = _h - _l >= range - 1e-12;
                        if (closeBar)
                            closePrice = _c >= _o ? _l + range : _h - range;
                    }

                    if (!closeBar)
                    {
                        cb.UpdateLastBar(_o, _h, _l, _c, _v);
                        return;
                    }

                    // close current bar & start a new forming bar
                    var closed = new CustomBar(_lastT, _o,
                        _isTick ? _h : Math.Max(_h, closePrice),
                        _isTick ? _l : Math.Min(_l, closePrice),
                        _isTick ? _c : closePrice, _v);
                    cb.UpdateLastBar(closed.Open, closed.High, closed.Low, closed.Close, closed.Volume);
                    _cache.Add(closed);

                    _forming = true;
                    _o = _h = _l = _c = _isTick ? price : closed.Close;
                    _v = 1;
                    _lastT = _lastT.AddMinutes(1);
                    cb.AppendBar(new CustomBar(_lastT, _o, _h, _l, _c, _v));
                }
                catch { }
            }

            // ─────────── builders (history) ───────────

            private struct Candle { public double O, H, L, C; public long V; }

            // آنقدر M1 اخیر را بزرگ می‌کند تا دقیقاً TargetChartBars کندل ساخته شود
            private List<Candle> BuildExactCount(Bars m1, double range)
            {
                var candles = new List<Candle>();
                if (m1 == null || m1.Count == 0)
                    return candles;

                int window = 500;

                for (int attempt = 0; attempt < 60; attempt++)
                {
                    while (m1.Count < Math.Min(window, MaxM1Bars))
                    {
                        int before = m1.Count;
                        _p.Status("لود M1… " + m1.Count.ToString("N0"));
                        if (m1.LoadMoreHistory() <= 0 || m1.Count <= before)
                            break;
                    }

                    int used = Math.Min(window, m1.Count);
                    int start = m1.Count - used;
                    candles = _isTick
                        ? BuildTick(m1, (long)_size, start)
                        : BuildRange(m1, range, start);

                    _p.Status(candles.Count.ToString("N0") + " / " + TargetChartBars);

                    if (candles.Count >= TargetChartBars)
                        break;
                    if (used < window)
                        break;
                    if (window >= MaxM1Bars)
                        break;

                    window = Math.Min(window * 2, MaxM1Bars);
                }

                if (candles.Count > TargetChartBars)
                    candles = candles.GetRange(candles.Count - TargetChartBars, TargetChartBars);

                return candles;
            }

            private static List<CustomBar> Timestamp(List<Candle> candles, DateTime lastTime)
            {
                var list = new List<CustomBar>(candles.Count);
                int n = candles.Count;
                for (int i = 0; i < n; i++)
                {
                    var c = candles[i];
                    list.Add(new CustomBar(lastTime.AddMinutes(-(n - i)), c.O, c.H, c.L, c.C, c.V));
                }
                return list;
            }

            private static List<Candle> BuildRange(Bars m1, double range, int start)
            {
                var outBars = new List<Candle>();
                if (range <= 0 || m1.Count == 0)
                    return outBars;

                bool forming = false;
                double o = 0, h = 0, l = 0, c = 0;
                long v = 0;
                start = Math.Max(0, start);

                for (int i = start; i < m1.Count; i++)
                {
                    var b = m1[i];
                    double[] path = b.Close >= b.Open
                        ? new[] { b.Open, b.Low, b.High, b.Close }
                        : new[] { b.Open, b.High, b.Low, b.Close };

                    foreach (double px in path)
                    {
                        if (!forming)
                        {
                            forming = true;
                            o = h = l = c = px;
                            v = 1;
                            continue;
                        }

                        v++;
                        c = px;

                        if (px > h)
                        {
                            h = px;
                            while (h - l >= range - 1e-12)
                            {
                                double hi = l + range;
                                outBars.Add(new Candle { O = o, H = hi, L = l, C = hi, V = v });
                                o = l = hi;
                                h = Math.Max(hi, px);
                                c = px;
                                v = 1;
                                if (h - l < range - 1e-12) break;
                            }
                        }

                        if (px < l)
                        {
                            l = px;
                            while (h - l >= range - 1e-12)
                            {
                                double lo = h - range;
                                outBars.Add(new Candle { O = o, H = h, L = lo, C = lo, V = v });
                                o = h = lo;
                                l = Math.Min(lo, px);
                                c = px;
                                v = 1;
                                if (h - l < range - 1e-12) break;
                            }
                        }
                    }
                }

                return outBars;
            }

            private static List<Candle> BuildTick(Bars m1, long tickSize, int start)
            {
                var outBars = new List<Candle>();
                if (tickSize <= 0 || m1.Count == 0)
                    return outBars;

                bool forming = false;
                double o = 0, h = 0, l = 0, c = 0;
                long v = 0;
                start = Math.Max(0, start);

                for (int i = start; i < m1.Count; i++)
                {
                    var b = m1[i];
                    double[] path = b.Close >= b.Open
                        ? new[] { b.Open, b.Low, b.High, b.Close }
                        : new[] { b.Open, b.High, b.Low, b.Close };
                    long each = Math.Max(1L, (long)b.TickVolume / 4);

                    foreach (double px in path)
                    {
                        long left = each;
                        while (left > 0)
                        {
                            if (!forming)
                            {
                                forming = true;
                                o = h = l = c = px;
                                v = 0;
                            }

                            long take = Math.Min(tickSize - v, left);
                            if (px > h) h = px;
                            if (px < l) l = px;
                            c = px;
                            v += take;
                            left -= take;

                            if (v >= tickSize)
                            {
                                outBars.Add(new Candle { O = o, H = h, L = l, C = c, V = v });
                                forming = false;
                                v = 0;
                            }
                        }
                    }
                }

                return outBars;
            }
        }
    }
}
