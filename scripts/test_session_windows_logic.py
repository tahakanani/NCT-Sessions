#!/usr/bin/env python3
"""Verify SessionWindowsLogic against SessionWindowsPro 1.21 defaults.

Ports the C# time helpers so they can run without the cTrader SDK,
then checks the indicator source still contains the MT default map.
"""
from __future__ import annotations

import re
import sys
from datetime import datetime, timedelta
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
INDICATOR = ROOT / "cTrader" / "SessionWindowsPro.cs"
ROOT_COPY = ROOT / "SessionWindowsPro.cs"
MINUTES_PER_DAY = 1440


def parse_hhmm(text: str) -> int:
    if text is None or not str(text).strip():
        return -1
    s = str(text).strip()
    if ":" not in s:
        return -1
    hour_s, minute_s = s.split(":", 1)
    try:
        hour = int(hour_s)
        minute = int(minute_s)
    except ValueError:
        return -1
    if hour < 0 or hour > 23 or minute < 0 or minute > 59:
        return -1
    return hour * 60 + minute


def shift_wrap(minutes: int, shift: int) -> int:
    m = (minutes + shift) % MINUTES_PER_DAY
    if m < 0:
        m += MINUTES_PER_DAY
    return m


def is_in_window(bar_minutes: int, start_minutes: int, end_minutes: int, shift: int) -> bool:
    if start_minutes < 0 or end_minutes < 0 or start_minutes == end_minutes:
        return False
    s = shift_wrap(start_minutes, shift)
    e = shift_wrap(end_minutes, shift)
    m = shift_wrap(bar_minutes, 0)
    if s == e:
        return False
    if s < e:
        return s <= m < e
    return m >= s or m < e


def window_on_day(day: datetime, start_minutes: int, end_minutes: int, shift: int):
    d = day.replace(hour=0, minute=0, second=0, microsecond=0)
    s = shift_wrap(start_minutes, shift)
    e = shift_wrap(end_minutes, shift)
    start = d + timedelta(minutes=s)
    if s < e:
        end = d + timedelta(minutes=e)
    else:
        end = d + timedelta(minutes=e + MINUTES_PER_DAY)
    return start, end


def parse_time_points(text: str) -> list[int]:
    if not text or not text.strip():
        return []
    seen = set()
    out = []
    for part in re.split(r"[,;|]", text):
        minutes = parse_hhmm(part)
        if minutes < 0 or minutes in seen:
            continue
        seen.add(minutes)
        out.append(minutes)
    return sorted(out)


def alpha_from_softness(softness: int) -> int:
    s = max(0, min(100, softness))
    return int(round(255.0 * (100 - s) / 100.0))


def max_grade_for_view(teaching_view: str) -> int:
    if not teaching_view or not teaching_view.strip():
        return 3
    s = teaching_view.strip().lower()
    if "a+b" in s or "a + b" in s:
        return 2
    if (("grade a" in s or s == "a" or "a only" in s) and "b" not in s and "full" not in s):
        return 1
    if "only a" in s or "a-only" in s or "a hot" in s:
        return 1
    return 3


def minutes_until_end(now_minutes: int, start_minutes: int, end_minutes: int, shift: int) -> int:
    if not is_in_window(now_minutes, start_minutes, end_minutes, shift):
        return -1
    e = shift_wrap(end_minutes, shift)
    m = shift_wrap(now_minutes, 0)
    delta = e - m
    if delta <= 0:
        delta += MINUTES_PER_DAY
    return delta


def minutes_until_next_time_point(now_minutes: int, time_points: list[int], shift: int) -> int:
    if not time_points:
        return -1
    m = shift_wrap(now_minutes, 0)
    best = None
    for raw in time_points:
        p = shift_wrap(raw, shift)
        delta = p - m
        if delta <= 0:
            delta += MINUTES_PER_DAY
        if best is None or delta < best:
            best = delta
    return -1 if best is None else best


def try_parse_rgb(spec: str):
    if spec is None or not str(spec).strip():
        return None
    s = str(spec).strip()
    if s.startswith("#") and len(s) in (7, 9):
        h = s[1:]
        if len(h) == 6:
            return int(h[0:2], 16), int(h[2:4], 16), int(h[4:6], 16)
        return int(h[2:4], 16), int(h[4:6], 16), int(h[6:8], 16)
    parts = [p for p in re.split(r"[,\s]+", s) if p]
    if len(parts) != 3:
        return None
    try:
        r, g, b = (int(parts[0]), int(parts[1]), int(parts[2]))
    except ValueError:
        return None
    if min(r, g, b) < 0 or max(r, g, b) > 255:
        return None
    return r, g, b


def assert_eq(actual, expected, label: str) -> None:
    if actual != expected:
        raise AssertionError(f"{label}: expected {expected!r}, got {actual!r}")


def test_parse_and_shift() -> None:
    assert_eq(parse_hhmm("07:00"), 420, "parse 07:00")
    assert_eq(parse_hhmm("15:30"), 930, "parse 15:30")
    assert_eq(parse_hhmm("00:00"), 0, "parse 00:00")
    assert_eq(parse_hhmm("23:00"), 1380, "parse 23:00")
    assert_eq(parse_hhmm("24:00"), -1, "reject 24:00")
    assert_eq(parse_hhmm("7:60"), -1, "reject 7:60")
    assert_eq(shift_wrap(60, -30), 30, "01:00 shift -30")
    assert_eq(shift_wrap(10, -30), 1420, "00:10 shift -30 wraps")
    assert_eq(shift_wrap(0, -30), 1410, "00:00 shift -30 -> 23:30")
    assert_eq(shift_wrap(1380, -30), 1350, "23:00 shift -30 -> 22:30")


def test_window_11_wrap() -> None:
    # 23:00-00:00 is a midnight wrap: 23:00 until next 00:00.
    assert_eq(is_in_window(1380, 1380, 0, 0), True, "23:00 inside W11")
    assert_eq(is_in_window(1439, 1380, 0, 0), True, "23:59 inside W11")
    assert_eq(is_in_window(0, 1380, 0, 0), False, "00:00 exclusive end of W11")
    assert_eq(is_in_window(1379, 1380, 0, 0), False, "22:59 outside W11")

    # Shift -30 moves W11 to 22:30-23:30.
    assert_eq(is_in_window(1350, 1380, 0, -30), True, "22:30 inside shifted W11")
    assert_eq(is_in_window(1409, 1380, 0, -30), True, "23:29 inside shifted W11")
    assert_eq(is_in_window(1410, 1380, 0, -30), False, "23:30 exclusive end")
    assert_eq(is_in_window(0, 1380, 0, -30), False, "midnight outside shifted W11")

    day = datetime(2026, 8, 21)
    start, end = window_on_day(day, 1380, 0, -30)
    assert_eq(start, datetime(2026, 8, 21, 22, 30), "W11 shifted start")
    assert_eq(end, datetime(2026, 8, 21, 23, 30), "W11 shifted end")
    assert_eq((end - start).seconds, 3600, "W11 duration stays 60m")


def test_same_day_and_asia_style_wrap() -> None:
    # Window 01 01:00-02:30 with -30 -> 00:30-02:00
    assert_eq(is_in_window(30, 60, 150, -30), True, "00:30 in W01 shifted")
    assert_eq(is_in_window(119, 60, 150, -30), True, "01:59 in W01 shifted")
    assert_eq(is_in_window(120, 60, 150, -30), False, "02:00 out of W01 shifted")
    assert_eq(is_in_window(0, 60, 150, -30), False, "00:00 out of W01 shifted")

    # Overnight 21:00-06:00 with -30 -> 20:30-05:30
    assert_eq(is_in_window(1230, 1260, 360, -30), True, "20:30 in overnight")
    assert_eq(is_in_window(0, 1260, 360, -30), True, "00:00 in overnight")
    assert_eq(is_in_window(329, 1260, 360, -30), True, "05:29 in overnight")
    assert_eq(is_in_window(330, 1260, 360, -30), False, "05:30 out of overnight")

    start, end = window_on_day(datetime(2026, 8, 21), 1260, 360, -30)
    assert_eq(start, datetime(2026, 8, 21, 20, 30), "overnight start")
    assert_eq(end, datetime(2026, 8, 22, 5, 30), "overnight end next day")


def test_time_points_panel_and_grades() -> None:
    points = parse_time_points("03:00, 04:00, 08:00, 09:00, 10:00, 16:30, 18:00, 20:00, 23:00")
    assert_eq(points, [180, 240, 480, 540, 600, 990, 1080, 1200, 1380], "default TP list")
    assert_eq(parse_time_points("10:00, 10:00, bad, 09:00"), [540, 600], "dedupe + skip bad")

    # 16:00 with shift -30: next TP 16:30 becomes 16:00 chart time...
    # now=16:00 (960), TP 16:30 (990) shifted -30 = 960, delta wraps to 1440 if <= 0.
    # minutes_until treats delta <= 0 as next day, so "at the TP" is 24h. That's OK
    # as long as 15:31 (931) to shifted 16:30 (960) is 29 minutes.
    assert_eq(minutes_until_next_time_point(931, points, -30), 29, "29m to shifted 16:30")
    assert_eq(minutes_until_next_time_point(959, points, -30), 1, "1m to shifted 16:30")
    assert_eq(minutes_until_end(1350, 1380, 0, -30), 60, "60m left in shifted W11 at 22:30")
    assert_eq(minutes_until_end(1409, 1380, 0, -30), 1, "1m left in shifted W11")

    assert_eq(alpha_from_softness(72), 71, "grade A alpha")
    assert_eq(alpha_from_softness(84), 41, "grade B alpha")
    assert_eq(alpha_from_softness(92), 20, "grade C alpha")
    assert_eq(max_grade_for_view("Full map (all 11)"), 3, "full map")
    assert_eq(max_grade_for_view("Grade A"), 1, "grade A view")
    assert_eq(max_grade_for_view("Grade A+B"), 2, "grade A+B view")
    assert_eq(try_parse_rgb("82,88,105"), (82, 88, 105), "rgb csv")
    assert_eq(try_parse_rgb("#00B2BE"), (0, 178, 190), "hex W02")


def test_source_defaults() -> None:
    src = INDICATOR.read_text(encoding="utf-8")
    root = ROOT_COPY.read_text(encoding="utf-8")
    assert_eq(src, root, "root copy matches cTrader/SessionWindowsPro.cs")

    windows = [
        ("01", "01:00", "02:30", "82,88,105", "3"),
        ("02", "02:30", "03:00", "0,178,190", "1"),
        ("03", "03:00", "07:00", "52,120,100", "2"),
        ("04", "07:00", "10:00", "128,116,52", "2"),
        ("05", "10:00", "13:00", "255,178,44", "1"),
        ("06", "13:00", "15:30", "98,88,74", "3"),
        ("07", "15:30", "16:30", "236,110,60", "3"),
        ("08", "16:30", "18:00", "244,70,96", "1"),
        ("09", "18:00", "21:00", "74,134,224", "1"),
        ("10", "21:00", "23:00", "118,96,208", "2"),
        ("11", "23:00", "00:00", "96,66,124", "2"),
    ]
    for index, start, end, colour, grade in windows:
        group = f'Group = "Window {index}"'
        if group not in src:
            raise AssertionError(f"missing {group}")
        if f'DefaultValue = "{start}"' not in src:
            raise AssertionError(f"W{index} start {start} missing")
        if f'DefaultValue = "{end}"' not in src:
            raise AssertionError(f"W{index} end {end} missing")
        if f'DefaultValue = "{colour}"' not in src:
            raise AssertionError(f"W{index} colour {colour} missing")

    required = [
        'DefaultValue = -30',
        'DefaultValue = 2',
        'DefaultValue = "Edge lines only"',
        'DefaultValue = "SWP_"',
        "03:00, 04:00, 08:00, 09:00, 10:00, 16:30, 18:00, 20:00, 23:00",
        "public static class SessionWindowsLogic",
        "ShiftWrap",
        "IsInWindow",
        "WindowOnDay",
    ]
    for token in required:
        if token not in src:
            raise AssertionError(f"missing source token: {token}")

    if "class NCT" in src or "Asia Session" in src:
        raise AssertionError("SessionWindowsPro should stay independent of NCT")


def extract_csharp_logic() -> str:
    src = INDICATOR.read_text(encoding="utf-8")
    start = src.find("//#region SESSION_WINDOWS_LOGIC")
    end = src.find("//#endregion SESSION_WINDOWS_LOGIC")
    if start < 0 or end < 0 or end <= start:
        raise AssertionError("SESSION_WINDOWS_LOGIC region missing")
    return src[start:end]


def test_csharp_region_compiles_shape() -> None:
    region = extract_csharp_logic()
    for name in (
        "ParseHhMm",
        "ShiftWrap",
        "IsInWindow",
        "WindowOnDay",
        "ParseTimePoints",
        "MinutesUntilEnd",
        "TryParseRgb",
    ):
        if f"public static" not in region or name not in region:
            raise AssertionError(f"logic region missing {name}")
    if "cAlgo" in region:
        raise AssertionError("SessionWindowsLogic should not depend on cAlgo")


def main() -> int:
    tests = [
        test_parse_and_shift,
        test_window_11_wrap,
        test_same_day_and_asia_style_wrap,
        test_time_points_panel_and_grades,
        test_source_defaults,
        test_csharp_region_compiles_shape,
    ]
    failed = 0
    for fn in tests:
        try:
            fn()
            print(f"PASS  {fn.__name__}")
        except Exception as exc:
            failed += 1
            print(f"FAIL  {fn.__name__}: {exc}")
    print(f"{len(tests) - failed}/{len(tests)} passed")
    return 1 if failed else 0


if __name__ == "__main__":
    sys.exit(main())
