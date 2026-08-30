#!/usr/bin/env python3
"""Lightweight sanity checker for TradingView Pine Script files.

Pine only truly compiles inside TradingView (there is no official offline
compiler), so this performs a fast structural lint instead:

  * a ``//@version=`` pragma is present,
  * exactly one ``indicator(``/``strategy(``/``library(`` declaration exists,
  * brackets ``() [] {}`` are balanced once strings and comments are stripped.

It is a smoke test, not a substitute for compiling on TradingView.
"""
from __future__ import annotations

import glob
import os
import re
import sys

VERSION_RE = re.compile(r"^\s*//@version\s*=\s*(\d+)", re.MULTILINE)
DECL_RE = re.compile(r"^\s*(indicator|strategy|library)\s*\(", re.MULTILINE)


def strip_strings_and_comments(src: str) -> str:
    """Replace string literals and line comments with spaces of equal length."""
    out = []
    i, n = 0, len(src)
    quote = None
    while i < n:
        c = src[i]
        if quote:
            if c == "\\" and i + 1 < n:
                out.append("  ")
                i += 2
                continue
            if c == quote:
                quote = None
            out.append(" " if c != "\n" else "\n")
            i += 1
            continue
        if c in ("'", '"'):
            quote = c
            out.append(" ")
            i += 1
            continue
        if c == "/" and i + 1 < n and src[i + 1] == "/":
            while i < n and src[i] != "\n":
                out.append(" ")
                i += 1
            continue
        out.append(c)
        i += 1
    return "".join(out)


def check_brackets(code: str) -> str | None:
    pairs = {")": "(", "]": "[", "}": "{"}
    opens = set(pairs.values())
    stack = []
    line = 1
    for ch in code:
        if ch == "\n":
            line += 1
        elif ch in opens:
            stack.append((ch, line))
        elif ch in pairs:
            if not stack or stack[-1][0] != pairs[ch]:
                return f"unbalanced '{ch}' at line {line}"
            stack.pop()
    if stack:
        ch, ln = stack[-1]
        return f"unclosed '{ch}' opened at line {ln}"
    return None


def check_file(path: str) -> list[str]:
    errors = []
    with open(path, "r", encoding="utf-8", errors="replace") as fh:
        src = fh.read()

    m = VERSION_RE.search(src)
    if not m:
        errors.append("missing //@version pragma")
    decls = DECL_RE.findall(src)
    if not decls:
        errors.append("no indicator()/strategy()/library() declaration")

    bracket_err = check_brackets(strip_strings_and_comments(src))
    if bracket_err:
        errors.append(bracket_err)
    return errors


def main(argv: list[str]) -> int:
    repo_root = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
    targets = argv[1:] or sorted(glob.glob(os.path.join(repo_root, "*.pine")))
    if not targets:
        print("No .pine files found.")
        return 0

    failed = False
    for path in targets:
        rel = os.path.relpath(path, repo_root)
        errors = check_file(path)
        if errors:
            failed = True
            print(f"FAIL {rel}")
            for e in errors:
                print(f"      - {e}")
        else:
            m = VERSION_RE.search(open(path, encoding="utf-8", errors="replace").read())
            ver = m.group(1) if m else "?"
            print(f"OK   {rel}  (v{ver})")

    print()
    print("Pine check FAILED." if failed else "Pine check passed.")
    return 1 if failed else 0


if __name__ == "__main__":
    raise SystemExit(main(sys.argv))
