"""Generates src/LiveSuite/Assets/LiveSuite.ico (and a PNG preview).

Same family as LiveCue's, LiveCount's and LiveSlides' icons (same dark tile and palette): three
rounded blocks, one per app of the suite - green (LiveCue's GO), amber (the cue on air) and light
(LiveCount's stopwatch hand) - with a downward arrow on the big one, because LiveSuite is the
place where you download the others.

Usage: python tools/make_icon.py   (needs Pillow)
"""
from pathlib import Path

from PIL import Image, ImageDraw

ROOT = Path(__file__).resolve().parent.parent
OUT_ICO = ROOT / "src" / "LiveSuite" / "Assets" / "LiveSuite.ico"
OUT_PNG = ROOT / "src" / "LiveSuite" / "Assets" / "LiveSuite.png"

S = 1024  # drawn at this size, then downscaled with Lanczos for smooth edges

TILE_TOP = (52, 57, 66)
TILE_BOTTOM = (22, 24, 28)
AMBER = (255, 179, 0)
GO_TOP = (102, 222, 128)
GO_BOTTOM = (34, 160, 70)
LIGHT = (235, 238, 242)
DARK = (22, 24, 28)


def vertical_gradient(size, top, bottom):
    g = Image.new("RGBA", (1, size))
    for y in range(size):
        t = y / (size - 1)
        g.putpixel((0, y), tuple(round(a + (b - a) * t) for a, b in zip(top, bottom)) + (255,))
    return g.resize((size, size))


def rounded_mask(box, radius):
    m = Image.new("L", (S, S), 0)
    ImageDraw.Draw(m).rounded_rectangle(box, radius=radius, fill=255)
    return m


def draw():
    img = Image.new("RGBA", (S, S), (0, 0, 0, 0))
    img.paste(vertical_gradient(S, TILE_TOP, TILE_BOTTOM), (0, 0), rounded_mask((0, 0, S - 1, S - 1), 200))

    # Big green block on the left (full height), two small ones stacked on the right.
    m, gap = 170, 56
    left = (m, m, 560, S - m)
    img.paste(vertical_gradient(S, GO_TOP, GO_BOTTOM), (0, 0), rounded_mask(left, 70))
    d = ImageDraw.Draw(img)
    top_right = (560 + gap, m, S - m, (S - gap) // 2)
    bottom_right = (560 + gap, (S + gap) // 2, S - m, S - m)
    d.rounded_rectangle(top_right, radius=60, fill=AMBER)
    d.rounded_rectangle(bottom_right, radius=60, fill=LIGHT)

    # Download arrow on the green block.
    cx = (left[0] + left[2]) // 2
    d.rectangle((cx - 42, 330, cx + 42, 560), fill=DARK)
    d.polygon([(cx - 130, 540), (cx + 130, 540), (cx, 690)], fill=DARK)
    d.rounded_rectangle((cx - 140, 730, cx + 140, 778), radius=20, fill=DARK)
    return img


def main():
    big = draw()
    big.resize((512, 512), Image.LANCZOS).save(OUT_PNG)
    sizes = [16, 24, 32, 48, 64, 128, 256]
    big.resize((256, 256), Image.LANCZOS).save(OUT_ICO, sizes=[(s, s) for s in sizes])
    print("wrote", OUT_ICO, OUT_PNG)


if __name__ == "__main__":
    main()
