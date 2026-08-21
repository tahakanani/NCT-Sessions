# راهنمای RTL در Cursor (فارسی)

> **مهم:** چت و Composer داخل **برنامه Cursor روی کامپیوتر شما** اجرا می‌شوند، نه روی سرور Cloud Agent.  
> نصب افزونه روی Cloud Agent **متن چت را راست‌به‌چپ نمی‌کند**.

## مراحل (حدود ۲ دقیقه)

### ۱) نصب افزونه روی Cursor محلی

1. در Cursor: `Ctrl+Shift+X` (Mac: `Cmd+Shift+X`)
2. در جستجو بنویسید:

```
@id:motcke.cursor-rtl
```

3. روی **Install** کلیک کنید.

**روش جایگزین (VSIX):**
1. فایل داخل همین مخزن:
   `scripts/cursor-rtl.vsix`
2. `Ctrl+Shift+P`
3. دستور:
   `Extensions: Install from VSIX...`
4. همان فایل را انتخاب کنید.

---

### ۲) فعال‌سازی Patch (الزامی)

نصب افزونه **کافی نیست**. باید Patch را فعال کنید:

1. پایین صفحه Cursor، در **Status Bar** (نوار وضعیت) دنبال **`RTL: OFF`** بگردید (سمت راست).
2. روی آن کلیک کنید → **Enable RTL** را بزنید.
3. اگر پیام تأیید آمد → **Allow / Approve** کنید.

**یا از Command Palette:**
- `Ctrl+Shift+P` → `Cursor RTL: Enable RTL / Fix After Update`

---

### ۳) بستن کامل Cursor

1. **همه** پنجره‌های Cursor را ببندید (فقط Reload کافی نیست).
2. دوباره Cursor را باز کنید.
3. در Status Bar باید **`RTL: ON`** را ببینید.

---

### ۴) تست

در چت بنویسید:

```
این متن باید راست‌چین باشد
```

اگر هنوز چپ‌چین است → بخش عیب‌یابی پایین.

---

## عیب‌یابی

| مشکل | راه‌حل |
|------|--------|
| Status Bar اصلاً `RTL: OFF` ندارد | افزونه نصب نشده — مرحله ۱ را دوباره انجام دهید |
| بعد از Enable هنوز `RTL: OFF` است | Cursor را **کاملاً** ببندید و دوباره باز کنید |
| Permission denied | **Linux:** Cursor را با دسترسی نوشتن روی پوشه نصب اجرا کنید. **Windows:** Run as Administrator |
| بعد از آپدیت Cursor خراب شد | `Cursor RTL: Enable RTL / Fix After Update` |
| Cursor بالا نمی‌آید | `Cursor RTL: Disable RTL Support` |

**گزارش تشخیص:**
- `Ctrl+Shift+P` → `Cursor RTL: Diagnostics`
- خروجی را برای پشتیبانی ذخیره کنید.

---

## تنظیمات پیشنهادی

`Ctrl+Shift+P` → **Preferences: Open User Settings (JSON)**

```json
{
  "cursorRtl.editorRtl": "auto",
  "cursorRtl.markdownPreview": true,
  "cursorRtl.autoReapply": true,
  "cursorRtl.showStatusBar": true
}
```

- `auto` — فایل‌های فارسی RTL، کد انگلیسی LTR
- `autoReapply` — بعد از آپدیت Cursor خودکار Patch را دوباره می‌زند

---

## چرا Cloud Agent کافی نبود؟

| محل | RTL با نصب Cloud |
|-----|------------------|
| چت / Composer / Agent | ❌ خیر — فقط Cursor محلی |
| ویرایشگر کد روی سرور | ⚠️ محدود |

افزونه `main.js` برنامه Cursor روی **سیستم شما** را Patch می‌کند.

---

## لینک‌ها

- افزونه: https://open-vsx.org/extension/motcke/cursor-rtl
- مستندات: https://motcke.github.io/cursor-ext-rtl/
