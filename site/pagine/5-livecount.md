# Pagina «LiveCount»

**Intestazione:** immagine `immagini/banner-livecount.png` (titolo pagina vuoto).

---

## Sezione 1 — Incorpora → Codice

Incolla `incorpora/scarica-livecount.html` (installer + versione portatile `LiveCount.exe`).

---

## Sezione 2 — Testo

### Countdown per eventi dal vivo

LiveCount tiene i tempi degli interventi: la **regia** sul monitor principale, il **display a schermo pieno** sul secondo schermo, per il relatore o per il pubblico.

### Cosa fa

- **Countdown con barra di avanzamento** che cambia colore: verde, arancio sotto il 10% del tempo, rosso sotto il 5% (soglie e colori personalizzabili).
- **Conteggio extra** dopo lo zero (+00:01, +00:02…), sempre in rosso.
- **Preset** con nome e durata, più impostazione manuale di ore, minuti e secondi.
- **Velocità di scorrimento** regolabile da 0.5x a 2x, anche mentre il timer corre.
- **Messaggio sotto il timer** da mostrare o nascondere con un clic (F9), con font, colore e grandezza a scelta.
- **Video di sfondo** da una sorgente **NDI®** o da una **scheda di acquisizione USB** (Magewell USB Capture, AJA U-TAP, webcam…), con il timer in un riquadro sovrapposto.
- **Monitor dal vivo** del secondo schermo nella regia: trascini il riquadro del timer per spostarlo e lo ridimensioni.
- **Eventi salvati** in un file `.livecount` (timer, preset, aspetto, messaggio, video), da riaprire anche su un altro PC.
- **Stream Deck con Bitfocus Companion**: LiveCount esporta da solo la pagina di tasti (AVVIA, PAUSA, RESET, ±1/5/10 MIN e un tasto per preset). C'è anche un'API HTTP sulla porta 8787 per altri sistemi di controllo.

**Scorciatoie:** F5 avvia/pausa · F8 reset · F9 mostra/nascondi messaggio.

### Installazione

- **Versione portatile**: scarica `LiveCount.exe`, doppio clic e parte. Nessuna installazione.
- **Installer**: `LiveCount-Setup.exe` aggiunge LiveCount al menu Start e si disinstalla da *Impostazioni → App*. Puoi installarlo per tutti gli utenti o solo per te.

Funziona su Windows 10 e 11 a 64 bit. Al primo avvio, se compare «Windows ha protetto il PC»: *Ulteriori informazioni* → *Esegui comunque*.

*NDI® è un marchio registrato di Vizrt NDI AB.*
