# NCT Dual Symmetry — cTrader

## نصب Indicator (NCT)
1. cTrader را باز کنید → Automate
2. New → Indicator
3. محتوای فایل NCT.cs را جایگزین کد پیش‌فرض کنید
4. Build → اگر خطا نبود، روی چارت Add Instance

## شامل چیست (NCT)
- موتور Dual Symmetry (Up/Down) + Log/Linear
- شماره نودها (**N** / N*) و خطوط زیگ‌زاگ
- تارگت‌های Node1 و Pair 1+2 (با حذف hit)
- Density قرمز/زرد + پنل آمار

## هنوز نیست (نسخه Pine)
- Session boxes (NY/London/Tokyo)
- Pivot Volume Profile (PoC/VAH/VAL)
- Confluence zones کامل

فایل Indicator: NCT.cs در ریشه پروژه و پوشه cTrader

---

# Custom Range / Tick Bars — Plugin

سایزهای Range و Tick پیش‌فرض سی‌تریدر محدودند. این Plugin هر عددی که وارد کنی را به‌عنوان تایم‌فریم واقعی در لیست Period اضافه می‌کند.

## نصب Plugin
1. cTrader → Automate
2. New → **Plugin** (نه Indicator)
3. محتوای فایل `CustomRangeBarsPlugin.cs` را جایگزین کد پیش‌فرض کنید
4. Build
5. Plugin را **Start** کنید (باید روشن بماند)

## استفاده
1. بعد از Start، در پنل **Active Symbol Panel (Asp)** سمت راست چارت، بلاک **Custom Range / Tick Bars** را باز کن
2. Type را انتخاب کن (Range یا Tick)
3. عدد را وارد کن → **Add Timeframe**
4. از لیست Period چارت، `Range 7` یا `Tick 100` را انتخاب کن

پنجره جدا باز نمی‌شود — همه چیز داخل همان پنل Asp است.
اگر Asp را نمی‌بینی: راست‌کلیک روی چارت → Active Symbol Panel را روشن کن.

## نکات
- Range: واحد **پیپ** (`N × PipSize`)
- Tick: هر N تیک یک کندل جدید
- تاریخچه از M1 ساخته می‌شود تا روی همان تایم‌فریم سفارشی **۵۰۰۰ کندل** نشان داده شود (هم‌تراز با Start Point اندیکاتور NCT)
- لایو با تیک آپدیت می‌شود
- اندیکاتور NCT روی همین تایم‌فریم سفارشی کار می‌کند؛ اگر نودها نیامدند NCT را یک‌بار Remove و دوباره Add کنید
- Plugin باید Start باشد تا تایم‌فریم‌های سفارشی در دسترس بمانند

فایل Plugin: cTrader/CustomRangeBarsPlugin.cs
