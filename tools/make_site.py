"""Generates the material for the LIVE suite website on Google Sites (site/).

- site/immagini/: header banners (1600x400) for the home and every app page, a dark
  background for sections, logo and favicon, the app icons.
- site/incorpora/: self-contained HTML blocks for Google Sites' "Incorpora > Codice".

Google Sites renders an embed inside a sandboxed iframe on its own domain, so every block
is standalone: inline CSS, icons as data URIs (no relative paths to break), no JavaScript,
links opening in a new tab (a download started inside the iframe can be blocked).
The texts of the pages live in site/pagine/*.md, written by hand.

Usage: python tools/make_site.py   (needs Pillow; uses the Segoe UI fonts of Windows)
"""
import base64
import io
from pathlib import Path

from PIL import Image, ImageDraw, ImageFilter, ImageFont

ROOT = Path(__file__).resolve().parent.parent
ASSETS = ROOT / "src" / "LiveSuite" / "Assets"
SITE = ROOT / "site"
IMG = SITE / "immagini"
EMBED = SITE / "incorpora"

OWNER = "ncrdavide-svg"
BMC = "https://buymeacoffee.com/cin8"

TOP = (40, 44, 52)
BOTTOM = (16, 18, 22)
TEXT = (235, 238, 242)
DIM = (150, 158, 172)
GREEN = (102, 222, 128)
AMBER = (255, 179, 0)

FONTS = Path("C:/Windows/Fonts")
SS = 2  # images are drawn at double size, then downscaled for smooth edges

# Taglines as in src/LiveSuite/Suite.cs; "download" is the fixed-name asset of the latest release.
APPS = [
    dict(name="LiveCue", page="livecue",
         tagline="Show control: cue audio, video e immagini con GO/PANIC, uscite multi-monitor.",
         download="LiveCue-Setup.exe", note="Installer · Windows 10/11 64 bit · .NET e VLC inclusi"),
    dict(name="LiveDeck", page="livedeck",
         tagline="PowerPoint e PDF in sequenza come un'unica presentazione, senza schermate nere.",
         download="LiveDeck-Setup.exe", note="Installer · Windows 10/11 · circa 55 MB"),
    dict(name="LiveCount", page="livecount",
         tagline="Countdown per eventi dal vivo: regia + display a schermo pieno, NDI e schede USB.",
         download="LiveCount-Setup.exe", note="Installer · Windows 10/11 64 bit",
         alt=("LiveCount.exe", "versione portatile, senza installazione")),
    dict(name="LiveCaption", page="livecaption",
         tagline="Sottotitoli dal vivo sul secondo schermo: trascrizione, un colore per relatore e traduzione in inglese.",
         download="LiveCaption-Setup.exe", note="Installer · Windows 10/11 64 bit · offline",
         alt=("LiveCaption.zip", "versione portatile, senza installazione")),
]
SUITE = dict(name="LiveSuite", page="livesuite",
             tagline="Installa e tiene aggiornate tutte le app della suite da un'unica piccola app.",
             download="LiveSuite.exe", note="Windows 10/11 · circa 200 KB · nessuna installazione")


def download_url(name, asset):
    return f"https://github.com/{OWNER}/{name}/releases/latest/download/{asset}"


# ---------------------------------------------------------------- images

def font(name, size):
    return ImageFont.truetype(str(FONTS / name), size * SS)


def background(w, h, glows=True):
    bg = Image.new("RGB", (1, h))
    for y in range(h):
        t = y / (h - 1)
        bg.putpixel((0, y), tuple(round(a + (b - a) * t) for a, b in zip(TOP, BOTTOM)))
    bg = bg.resize((w, h)).convert("RGBA")
    if not glows:
        return bg
    # Soft glows in the suite's green and amber, as on the Buy Me a Coffee cover.
    glow = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    d = ImageDraw.Draw(glow)
    d.ellipse((w * 0.05, h * 0.0, w * 0.45, h * 1.1), fill=GREEN + (50,))
    d.ellipse((w * 0.58, h * -0.1, w * 0.98, h * 1.0), fill=AMBER + (36,))
    glow = glow.filter(ImageFilter.GaussianBlur(min(w, h) * 0.3))
    return Image.alpha_composite(bg, glow)


def paste_icon(img, name, x, y, size):
    shadow = Image.new("RGBA", img.size, (0, 0, 0, 0))
    r = size * 0.23
    ImageDraw.Draw(shadow).rounded_rectangle(
        (x + size * 0.05, y + size * 0.09, x + size * 1.05, y + size * 1.09), radius=r, fill=(0, 0, 0, 120))
    img = Image.alpha_composite(img, shadow.filter(ImageFilter.GaussianBlur(size * 0.09)))
    icon = Image.open(ASSETS / f"{name}.png").convert("RGBA").resize((int(size), int(size)), Image.LANCZOS)
    img.alpha_composite(icon, (int(x), int(y)))
    return img


def centered(draw, text, y, fnt, fill, w):
    draw.text(((w - draw.textlength(text, font=fnt)) / 2, y), text, font=fnt, fill=fill)


def save(img, path, w, h):
    path.parent.mkdir(parents=True, exist_ok=True)
    img.convert("RGB").resize((w, h), Image.LANCZOS).save(path, optimize=True)
    print("wrote", path.relative_to(ROOT))


def home_banner():
    """1600x400 with icons, title and subtitle. Content kept in the central band: Google Sites
    crops the header image differently on phones."""
    W, H = 1600, 400
    w, h = W * SS, H * SS
    img = background(w, h)
    names = [SUITE["name"]] + [a["name"] for a in APPS]
    size, gap = 84 * SS, 26 * SS
    x = (w - (len(names) * size + (len(names) - 1) * gap)) // 2
    for n in names:
        img = paste_icon(img, n, x, 40 * SS, size)
        x += size + gap
    d = ImageDraw.Draw(img)
    centered(d, "Suite LIVE", 150 * SS, font("segoeuib.ttf", 72), TEXT, w)
    centered(d, "LiveCue  ·  LiveDeck  ·  LiveCount  ·  LiveCaption", 248 * SS, font("seguisb.ttf", 28), GREEN, w)
    centered(d, "App gratuite per Windows per chi lavora negli eventi dal vivo", 296 * SS,
             font("segoeui.ttf", 24), DIM, w)
    save(img, IMG / "banner-home.png", W, H)


def plain_banner():
    """Same background without text, for when the page title is written by Google Sites itself."""
    save(background(1600 * SS, 400 * SS), IMG / "banner-vuoto.png", 1600, 400)


def section_background():
    """Dark background for Google Sites sections (Sfondo sezione > Immagine): gives the dark
    LIVE look whatever theme is chosen."""
    save(background(1920, 1080, glows=False), IMG / "sfondo-sezione.png", 1920, 1080)


def wrap(draw, text, fnt, max_w):
    lines, line = [], ""
    for word in text.split():
        test = (line + " " + word).strip()
        if draw.textlength(test, font=fnt) <= max_w:
            line = test
        else:
            lines.append(line)
            line = word
    return lines + [line]


def app_banner(app):
    """1600x400: icon on the left, name and tagline on the right, all within the central 1200 px."""
    W, H = 1600, 400
    w, h = W * SS, H * SS
    img = background(w, h)
    size = 180 * SS
    left = 230 * SS
    img = paste_icon(img, app["name"], left, (h - size) // 2 - 6 * SS, size)
    d = ImageDraw.Draw(img)
    tx = left + size + 56 * SS
    title_f = font("segoeuib.ttf", 76)
    tag_f = font("segoeui.ttf", 28)
    lines = wrap(d, app["tagline"], tag_f, w - tx - 230 * SS)
    block = 100 * SS + len(lines) * 40 * SS
    y = (h - block) // 2 - 10 * SS
    d.text((tx, y), app["name"], font=title_f, fill=TEXT)
    y += 106 * SS
    for line in lines:
        d.text((tx, y), line, font=tag_f, fill=DIM)
        y += 40 * SS
    save(img, IMG / f"banner-{app['page']}.png", W, H)


def logo_and_icons():
    # Google Sites wants a square image for the favicon, and shows the logo small in the
    # navigation bar: the LiveSuite icon works for both.
    logo = Image.open(ASSETS / "LiveSuite.png").convert("RGBA")
    logo.save(IMG / "logo.png", optimize=True)
    logo.resize((256, 256), Image.LANCZOS).save(IMG / "favicon.png", optimize=True)
    print("wrote site/immagini/logo.png, favicon.png")
    (IMG / "icone").mkdir(exist_ok=True)
    for n in [SUITE["name"]] + [a["name"] for a in APPS]:
        Image.open(ASSETS / f"{n}.png").save(IMG / "icone" / f"{n}.png", optimize=True)
    print("wrote site/immagini/icone/*.png")


# ---------------------------------------------------------------- embeds

def data_uri(path, px):
    im = Image.open(path).convert("RGBA").resize((px, px), Image.LANCZOS)
    buf = io.BytesIO()
    im.save(buf, "PNG", optimize=True)
    return "data:image/png;base64," + base64.b64encode(buf.getvalue()).decode()


def icon_uri(name):
    return data_uri(ASSETS / f"{name}.png", 160)  # shown at 64-80 px, sharp on 2x screens


CSS = """
:root{--bg:#16181C;--card:#1F2228;--line:#2E333C;--text:#EBEEF2;--dim:#969EAC;--go:#22A046;--go2:#2BBF57;--amber:#FFB300}
*{box-sizing:border-box;margin:0;padding:0}
html,body{background:transparent;color:var(--text);font-family:"Segoe UI",system-ui,-apple-system,Roboto,Arial,sans-serif;line-height:1.45}
body{padding:8px}
a{color:inherit}
.card{background:var(--card);border:1px solid var(--line);border-radius:16px;padding:20px;display:flex;gap:18px;align-items:flex-start}
.card img.icon{width:72px;height:72px;border-radius:16px;flex:none}
.card h3{font-size:22px;font-weight:700;margin-bottom:4px}
.card p{color:var(--dim);font-size:15px}
.btn{display:inline-flex;align-items:center;justify-content:center;gap:8px;background:var(--go);color:#fff;text-decoration:none;font-weight:600;font-size:15px;padding:10px 18px;border-radius:10px;transition:background .15s}
.btn:hover{background:var(--go2)}
.btn.ghost{background:transparent;border:1px solid var(--line);color:var(--text)}
.btn.ghost:hover{border-color:var(--dim);background:transparent}
.actions{display:flex;flex-wrap:wrap;gap:10px;margin-top:14px;align-items:center}
.small{font-size:13px;color:var(--dim)}
.badge{height:20px;vertical-align:middle}
"""


def page(title, body, extra_css=""):
    return f"""<!doctype html>
<html lang="it">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width,initial-scale=1">
<title>{title}</title>
<base target="_blank">
<style>{CSS}{extra_css}</style>
</head>
<body>
{body}
</body>
</html>
"""


def badge(name):
    # shields.io reads the latest release: the version shown updates by itself.
    return (f'<img class="badge" src="https://img.shields.io/github/v/release/{OWNER}/{name}'
            f'?label=versione&amp;color=22A046&amp;style=flat-square" alt="">')


def write(name, html):
    EMBED.mkdir(parents=True, exist_ok=True)
    (EMBED / name).write_text(html, encoding="utf-8")
    print(f"wrote site/incorpora/{name} ({len(html.encode()) // 1024} KB)")


def embed_cards():
    """Home page: LiveSuite highlighted on top, then the four apps in a responsive grid."""
    cards = []
    for a in APPS:
        cards.append(f"""<div class="card">
  <img class="icon" src="{icon_uri(a['name'])}" alt="">
  <div>
    <h3>{a['name']}</h3>
    <p>{a['tagline']}</p>
    <div class="actions">
      <a class="btn" href="{download_url(a['name'], a['download'])}">⬇ Scarica</a>
      <a class="btn ghost" href="https://github.com/{OWNER}/{a['name']}">Dettagli</a>
    </div>
  </div>
</div>""")
    s = SUITE
    body = f"""<div class="suite card">
  <img class="icon" src="{icon_uri(s['name'])}" alt="">
  <div>
    <h3>{s['name']} <span class="pill">consigliato</span></h3>
    <p>{s['tagline']} Mostra la versione installata e l'ultima pubblicata, e aggiorna con un clic.</p>
    <div class="actions">
      <a class="btn" href="{download_url(s['name'], s['download'])}">⬇ Scarica LiveSuite.exe</a>
      <span class="small">{s['note']}</span>
    </div>
  </div>
</div>
<div class="grid">
{chr(10).join(cards)}
</div>"""
    css = """
.suite{border-color:#2f5c3c;background:linear-gradient(135deg,#1d2a22,#1F2228 60%);margin-bottom:14px}
.pill{display:inline-block;font-size:11px;font-weight:700;letter-spacing:.04em;text-transform:uppercase;color:#16181C;background:var(--amber);border-radius:999px;padding:2px 8px;vertical-align:middle;margin-left:6px}
.grid{display:grid;grid-template-columns:1fr 1fr;gap:14px}
@media (max-width:680px){.grid{grid-template-columns:1fr}}
@media (max-width:420px){.card{flex-direction:column;padding:16px}.card img.icon{width:60px;height:60px}}
"""
    write("schede-app.html", page("Le app della suite LIVE", body, css))


def embed_download(app):
    """One big download box for an app page."""
    alt = ""
    if app.get("alt"):
        asset, desc = app["alt"]
        alt = (f'<a class="btn ghost" href="{download_url(app["name"], asset)}">{asset}</a>'
               f'<span class="small">{desc}</span>')
    body = f"""<div class="card dl">
  <img class="icon" src="{icon_uri(app['name'])}" alt="">
  <div class="txt">
    <h3>Scarica {app['name']} {badge(app['name'])}</h3>
    <p>{app['note']}. Il pulsante scarica sempre l'ultima versione.</p>
    <div class="actions">
      <a class="btn big" href="{download_url(app['name'], app['download'])}">⬇ Scarica {app['download']}</a>
      {alt}
    </div>
    <p class="small more"><a href="https://github.com/{OWNER}/{app['name']}/releases">Tutte le versioni e le novità</a>
    · <a href="https://github.com/{OWNER}/{app['name']}">Pagina su GitHub</a></p>
  </div>
</div>"""
    css = """
.dl{align-items:center}
.btn.big{font-size:17px;padding:13px 22px}
.more{margin-top:12px}
@media (max-width:480px){.dl{flex-direction:column;align-items:flex-start}.btn.big{width:100%}}
"""
    write(f"scarica-{app['page']}.html", page(f"Scarica {app['name']}", body, css))


def embed_coffee():
    # The QR is pure black and white: 1-bit keeps the data URI a few KB instead of ~60.
    im = Image.open(ASSETS / "BuyMeACoffeeQR.png").convert("L").resize((240, 240), Image.LANCZOS)
    buf = io.BytesIO()
    im.point(lambda v: 255 if v > 128 else 0).convert("1").save(buf, "PNG", optimize=True)
    qr = "data:image/png;base64," + base64.b64encode(buf.getvalue()).decode()
    body = f"""<div class="card coffee">
  <img class="qr" src="{qr}" alt="QR code buymeacoffee.com/cin8">
  <div>
    <h3>☕ Offrimi un caffè</h3>
    <p>Le app della suite LIVE sono gratuite. Se ti fanno risparmiare tempo durante un evento,
    un caffè aiuta a tenerle aggiornate e a farne di nuove.</p>
    <div class="actions">
      <a class="btn bmc" href="{BMC}">☕ buymeacoffee.com/cin8</a>
    </div>
  </div>
</div>"""
    css = """
.coffee{align-items:center}
.qr{width:120px;height:120px;border-radius:12px;background:#fff;padding:6px;flex:none}
.btn.bmc{background:#FFDD00;color:#0D0C22}
.btn.bmc:hover{background:#ffe633}
@media (max-width:480px){.coffee{flex-direction:column;align-items:flex-start}}
"""
    write("caffe.html", page("Offrimi un caffè", body, css))


def main():
    IMG.mkdir(parents=True, exist_ok=True)
    home_banner()
    plain_banner()
    section_background()
    for a in APPS + [SUITE]:
        app_banner(a)
    logo_and_icons()
    embed_cards()
    for a in [SUITE] + APPS:
        embed_download(a)
    embed_coffee()


if __name__ == "__main__":
    main()
