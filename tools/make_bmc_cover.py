"""Generates docs/bmc-cover.png, the cover image for the Buy Me a Coffee page (buymeacoffee.com/cin8).

1500x500, dark tile background of the suite's icons, the four icons in a row, title and tagline.
Everything important stays in the central band, so it survives the page cropping the cover
differently on phone and desktop.

Usage: python tools/make_bmc_cover.py   (needs Pillow; uses the Segoe UI fonts of Windows)
"""
from pathlib import Path

from PIL import Image, ImageDraw, ImageFilter, ImageFont

ROOT = Path(__file__).resolve().parent.parent
ASSETS = ROOT / "src" / "LiveSuite" / "Assets"
OUT = ROOT / "docs" / "bmc-cover.png"

W, H = 1500, 500
SS = 2  # drawn at double size, then downscaled for smooth edges

TOP = (40, 44, 52)
BOTTOM = (16, 18, 22)
TEXT = (235, 238, 242)
DIM = (150, 158, 172)
GREEN = (102, 222, 128)
AMBER = (255, 179, 0)
BMC_YELLOW = (255, 221, 0)

FONTS = Path("C:/Windows/Fonts")


def font(name, size):
    return ImageFont.truetype(str(FONTS / name), size * SS)


def background():
    w, h = W * SS, H * SS
    bg = Image.new("RGB", (1, h))
    for y in range(h):
        t = y / (h - 1)
        bg.putpixel((0, y), tuple(round(a + (b - a) * t) for a, b in zip(TOP, BOTTOM)))
    bg = bg.resize((w, h)).convert("RGBA")

    # Soft coloured glows behind the icons: the suite's green and amber.
    glow = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    d = ImageDraw.Draw(glow)
    d.ellipse((w * 0.08, h * 0.05, w * 0.48, h * 1.05), fill=GREEN + (55,))
    d.ellipse((w * 0.55, h * 0.0, w * 0.95, h * 0.95), fill=AMBER + (40,))
    glow = glow.filter(ImageFilter.GaussianBlur(160 * SS))
    return Image.alpha_composite(bg, glow)


def centered(draw, text, y, fnt, fill):
    width = draw.textlength(text, font=fnt)
    draw.text(((W * SS - width) / 2, y), text, font=fnt, fill=fill)


def main():
    img = background()
    d = ImageDraw.Draw(img)

    # Icons row: LiveSuite, then the three apps.
    names = ["LiveSuite", "LiveCue", "LiveSlides", "LiveCount"]
    size, gap = 104 * SS, 34 * SS
    total = len(names) * size + (len(names) - 1) * gap
    x = (W * SS - total) // 2
    y = 58 * SS
    for n in names:
        icon = Image.open(ASSETS / f"{n}.png").convert("RGBA").resize((size, size), Image.LANCZOS)
        shadow = Image.new("RGBA", img.size, (0, 0, 0, 0))
        ImageDraw.Draw(shadow).rounded_rectangle((x + 6 * SS, y + 10 * SS, x + size + 6 * SS, y + size + 10 * SS),
                                                 radius=24 * SS, fill=(0, 0, 0, 120))
        img = Image.alpha_composite(img, shadow.filter(ImageFilter.GaussianBlur(10 * SS)))
        img.alpha_composite(icon, (x, y))
        x += size + gap
    d = ImageDraw.Draw(img)

    centered(d, "Suite LIVE", 196 * SS, font("segoeuib.ttf", 74), TEXT)
    centered(d, "LiveCue  ·  LiveSlides  ·  LiveCount", 292 * SS, font("seguisb.ttf", 30), GREEN)
    centered(d, "App gratuite per Windows per chi lavora negli eventi dal vivo", 342 * SS, font("segoeui.ttf", 26), DIM)

    # Small yellow pill, in Buy Me a Coffee's colour.
    # Segoe UI has no coffee cup: the emoji is drawn with Windows' colour emoji font.
    pill_font = font("segoeuib.ttf", 22)
    emoji_font = font("seguiemj.ttf", 22)
    label = "Un caffè tiene vivo il progetto"
    ew = d.textlength("☕", font=emoji_font)
    tw = ew + 12 * SS + d.textlength(label, font=pill_font)
    px, py, ph = (W * SS - tw) / 2 - 26 * SS, 398 * SS, 46 * SS
    d.rounded_rectangle((px, py, px + tw + 52 * SS, py + ph), radius=ph // 2, fill=BMC_YELLOW)
    d.text((px + 26 * SS, py + 8 * SS), "☕", font=emoji_font, embedded_color=True)
    d.text((px + 26 * SS + ew + 12 * SS, py + 8 * SS), label, font=pill_font, fill=(13, 12, 34))

    img.convert("RGB").resize((W, H), Image.LANCZOS).save(OUT, optimize=True)
    print("wrote", OUT)


if __name__ == "__main__":
    main()
