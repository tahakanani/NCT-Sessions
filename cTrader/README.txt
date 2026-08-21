# cTrader Indicators & Plugins

---

# MAP Weekly (مپ هفتگی)

اندیکاتور سطوح هفتگی بر اساس سیستم MAP محمدعلی پورصمدی — مناسب فارکس، XAUUSD و شاخص‌ها.

## نصب Indicator (MAP Weekly)
1. cTrader → Automate
2. New → Indicator
3. محتوای فایل `MAPWeekly.cs` را جایگزین کد پیش‌فرض کنید
4. Build → Add Instance روی چارت

## سطوح نمایش‌داده‌شده
- **سقف و کف هفته قبل** (PW High / PW Low)
- **اصلاحی:** 25%، 50% (Mid)، 75% داخل دامنه هفته قبل
- **اکستنشن روند:** 1.25x، 1.5x، 1.75x، 2x بالای سقف و زیر کف
- **اختیاری:** 1.125x و 1.375x (پیش‌فرض خاموش — برای هفته‌های پرنوسان)

## محاسبه
```
دامنه = سقف هفته قبل − کف هفته قبل
50%  = (سقف + کف) / 2
25%  = کف + دامنه × 0.25
75%  = کف + دامنه × 0.75
1.25x = سقف + دامنه × 0.25   (و قرینه زیر کف)
2x   = سقف + دامنه × 1.00
```

خطوط از ابتدای هفته جاری رسم می‌شوند و به سمت راست ادامه دارند.

فایل: `MAPWeekly.cs` در ریشه پروژه و پوشه `cTrader/`

---

# Session Windows Pro — cTrader

پورت `SessionWindowsPro 1.21` برای مشخص‌کردن پنجره‌های زمانی مهم روی چارت. مستقل از NCT است.

## نصب Indicator (Session Windows Pro)
1. cTrader → Automate
2. New → Indicator
3. محتوای فایل `SessionWindowsPro.cs` را جایگزین کد پیش‌فرض کنید
4. Build → Add Instance روی چارت
5. روی `XAUUSD` تایم `M1` تست کنید

## پیش‌فرض‌ها (مطابق نسخه متاتریدر)
- ۱۱ پنجره از `01:00` تا `00:00` با گرید A/B/C
- شیفت همه زمان‌ها: `-30` دقیقه
- دو روز اخیر
- حالت رسم: `Edge lines only`
- مخفی‌کردن پنجره‌های بدون کندل (آخر هفته)
- نقاط زمانی: `03:00, 04:00, 08:00, 09:00, 10:00, 16:30, 18:00, 20:00, 23:00`
- خط شروع روز `00:00`
- پنل شمارش معکوس گوشه بالا-راست، هشدار اگر نقطه بعدی کمتر از ۵ دقیقه باشد

## حالت رسم
- `Edge lines only` — فقط قاب
- `Filled` — پس‌زمینه نرم بر اساس گرید
- `Filled + edges` — هر دو

## Teaching view
- `Full map (all 11)` — هر ۱۱ پنجره (پیش‌فرض)
- `Grade A` — فقط پنجره‌های داغ
- `Grade A+B` — A و B

پنجره ۱۱ (`23:00-00:00`) از نیمه‌شب رد می‌شود. نرمی گرید A/B/C برابر 72 / 84 / 92 درصد است.

فایل: `SessionWindowsPro.cs` در ریشه پروژه و پوشه `cTrader/`

---

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
