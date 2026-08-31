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

# NCT Final1 — cTrader (پورت NCT-Final1.pine)

نسخه کامل برای cTrader. همان گروه‌های ورودی و همان منطق نود، تارگت، سشن، MAP و Day OC.

## نصب Indicator (NCT Final1)
1. cTrader → Automate
2. New → Indicator
3. محتوای فایل NCTFinal1.cs را جایگزین کد پیش‌فرض کنید
4. Build → Add Instance روی چارت

## شامل چیست
- موتور Dual Symmetry با حالت Starred / Starless
- تارگت‌های Node 1 (Double.1, 1.5DL.1, 0.8DL.1, Min 1, 0.8Min.1, 1.3MIN1)
- تارگت Node 2 و زوج 1+2
- دایره قرمز نود ۲ ناقص و دایره سبز نزدیکی تارگت
- سشن‌های NY / London / Tokyo / Sydney
- MAP Weekly بنفش
- Day Open / Day Close از کندل روزانه
- تارگت‌های زمانی عمودی
- حذف تارگت بعد از hit و چیدمان لیبل

فایل: NCTFinal1.cs در ریشه پروژه و پوشه cTrader

---

# NCT Dual Symmetry — cTrader (نسخه قدیمی)

## نصب Indicator (NCT)
1. cTrader را باز کنید → Automate
2. New → Indicator
3. محتوای فایل NCT.cs را جایگزین کد پیش‌فرض کنید
4. Build → اگر خطا نبود، روی چارت Add Instance

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
