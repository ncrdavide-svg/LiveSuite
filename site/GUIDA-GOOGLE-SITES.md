# Il sito della suite LIVE su Google Sites — guida passo passo

Google Sites non ha un'API per costruire i siti: i clic nell'editor vanno fatti a mano. Qui però è tutto pronto: testi da copiare, immagini da caricare e blocchi HTML da incollare. Tempo previsto: circa un'ora la prima volta.

## Cosa c'è in questa cartella

| Cartella / file | Contenuto |
|---|---|
| `pagine/` | Il testo di ogni pagina, sezione per sezione, già in italiano. Ogni file dice anche quale immagine e quale blocco HTML usare. |
| `immagini/banner-home.png` | Intestazione della Home (1600×400, con titolo e icone). |
| `immagini/banner-<app>.png` | Intestazione di ogni pagina app (icona, nome, descrizione). |
| `immagini/banner-vuoto.png` | Intestazione scura senza testo, per Domande frequenti e Supporto. |
| `immagini/sfondo-sezione.png` | Sfondo scuro da usare dietro le sezioni (dà lo stile LIVE con qualsiasi tema). |
| `immagini/logo.png`, `favicon.png` | Logo nella barra in alto e icona della scheda del browser. |
| `immagini/icone/` | Le icone delle app a 512 px, se servono altrove. |
| `incorpora/*.html` | Blocchi da incollare con **Inserisci → Incorpora → Incorpora codice**. |

I blocchi HTML sono autonomi (icone incluse nel codice, nessun JavaScript): si aprono anche con doppio clic per vederli nel browser. Il numero di versione nei riquadri «Scarica» è un badge di shields.io che legge l'ultima Release da GitHub, quindi si aggiorna da solo.

Per rigenerare immagini e blocchi (per esempio dopo aver cambiato una descrizione o aggiunto un'app): `python tools/make_site.py` (serve Pillow).

## Struttura del sito

| Pagina | File testo | Intestazione | Blocco da incorporare |
|---|---|---|---|
| Home | `pagine/1-home.md` | `banner-home.png` | `schede-app.html` |
| LiveSuite | `pagine/2-livesuite.md` | `banner-livesuite.png` | `scarica-livesuite.html` |
| LiveCue | `pagine/3-livecue.md` | `banner-livecue.png` | `scarica-livecue.html` |
| LiveDeck | `pagine/4-livedeck.md` | `banner-livedeck.png` | `scarica-livedeck.html` |
| LiveCount | `pagine/5-livecount.md` | `banner-livecount.png` | `scarica-livecount.html` |
| LiveCaption | `pagine/6-livecaption.md` | `banner-livecaption.png` | `scarica-livecaption.html` |
| Domande frequenti | `pagine/7-domande.md` | `banner-vuoto.png` | — |
| Supporto | `pagine/8-supporto.md` | `banner-vuoto.png` | `caffe.html` |

---

## 1. Crea il sito

1. Vai su **https://sites.google.com** con il tuo account Google.
2. Clicca **+ (Sito vuoto)** in alto a sinistra.
3. In alto a sinistra, dove c'è «Sito senza titolo», scrivi il nome del documento: `Suite LIVE` (è solo il nome del file in Drive).
4. Nell'intestazione della pagina, dove c'è «Titolo della pagina», cancella il testo (il titolo è già nell'immagine della Home).
5. In alto a sinistra, accanto al logo, c'è «Inserisci il nome del sito»: scrivi `Suite LIVE`. È il nome che compare nella barra di navigazione.

## 2. Tema e colori nello stile LIVE

1. Nel pannello a destra apri la scheda **Temi**.
2. Scegli un tema semplice e moderno (per esempio *Vision* o *Impression*: i nomi possono cambiare; vanno bene tutti).
3. Sotto il tema, nel colore, scegli il **verde** più vicino a `#22A046` (il verde GO delle app). Se c'è il selettore personalizzato (icona **+** o tavolozza), inserisci `#22A046`.
4. Come stile del carattere scegli quello più pulito (senza grazie).

**Per avere le sezioni scure come nelle app**: passa con il mouse su una sezione, clicca l'icona della tavolozza **«Sfondo della sezione»** che compare a sinistra e scegli **Immagine** → carica `immagini/sfondo-sezione.png`. Il testo diventa bianco da solo. Fallo per le sezioni di testo; i blocchi HTML hanno già i loro riquadri scuri e stanno bene sia su fondo chiaro sia su fondo scuro.

## 3. Logo e favicon

1. Clicca l'ingranaggio **Impostazioni** in alto.
2. Apri **Immagini del brand** (*Brand images*).
3. **Logo**: carica `immagini/logo.png`.
4. **Favicon**: carica `immagini/favicon.png`.
5. Chiudi le impostazioni.

## 4. Crea le pagine

1. Pannello a destra → scheda **Pagine**.
2. Clicca **+** in basso → **Nuova pagina** e crea, in quest'ordine: `LiveSuite`, `LiveCue`, `LiveDeck`, `LiveCount`, `LiveCaption`, `Domande frequenti`, `Supporto`. La «Home» c'è già.
3. Trascina le pagine per metterle in questo ordine: compaiono così nel menu in alto.
4. Facoltativo: trascina le quattro app sotto «LiveSuite» per fare un sottomenu. Con sette voci il menu in alto ci sta comunque.

## 5. Riempi ogni pagina

Per ogni pagina apri il file corrispondente in `pagine/` e segui le sezioni nell'ordine.

**Intestazione (banner)**
1. Passa con il mouse sull'intestazione → **Cambia immagine** → **Carica** → scegli il banner indicato.
2. Sempre lì, **Tipo di intestazione** → **Banner**. Se il banner appare tagliato in alto o in basso, prova **Banner grande**.
3. Nelle pagine delle app e nella Home lascia vuoto il titolo della pagina (il nome è già nell'immagine). In Domande frequenti e Supporto scrivi il titolo.

**Testo**
1. Pannello a destra → **Inserisci** → **Casella di testo**.
2. Copia il testo dal file `.md` e incollalo. Gli asterischi (`**grassetto**`) e i `###` non servono: usa i pulsanti di formattazione della casella (stile **Titolo** / **Sottotitolo** per le righe con `###`, **elenco puntato** per le righe che iniziano con `-`).
   Suggerimento: se apri i file `.md` in un visualizzatore Markdown (per esempio l'anteprima di VS Code o la pagina del file su GitHub) e copi da lì, grassetti ed elenchi arrivano già formattati.
3. Per le tre colonne della Home: **Inserisci → Layout** e scegli quello a tre caselle.

**Blocco HTML (download, schede delle app, caffè)**
1. Apri il file `.html` indicato con il Blocco note (tasto destro → Apri con → Blocco note), **Ctrl+A**, **Ctrl+C**.
2. In Google Sites: **Inserisci → Incorpora** → scheda **Incorpora codice** → incolla → **Avanti** → **Inserisci**.
3. Allarga il riquadro a tutta la larghezza e trascina il bordo inferiore finché non c'è più la barra di scorrimento interna. Altezze di partenza indicative:
   - `schede-app.html`: circa 640 px;
   - `scarica-*.html`: circa 240 px (LiveCount e LiveCaption un po' di più, hanno anche la versione portatile);
   - `caffe.html`: circa 200 px.

**Pulsanti** (Home, sezione «Inizia da LiveSuite»): **Inserisci → Pulsante**, nome e link sono scritti nel file della pagina.

**Domande frequenti**: ogni domanda può diventare un **Testo comprimibile** (Inserisci → Testo comprimibile): si apre con un clic, comodo su telefono.

## 6. Controlla su telefono

In alto c'è l'icona **Anteprima** (lo schermo con l'occhio). In basso a destra dell'anteprima scegli **telefono** e **tablet**.

Su telefono Google Sites **mantiene le proporzioni** dei blocchi incorporati: un blocco largo 950 e alto 240 sul computer diventa 360×90 sul telefono. Per questo ogni blocco ha una **versione compatta** che si attiva da sola quando il riquadro è stretto (o più basso di quanto serve alla versione completa) e riempie esattamente lo spazio, senza barre di scorrimento: righe con icona, nome e pulsante «Scarica»; per il caffè solo il pulsante giallo.

Se su computer vedi la versione compatta al posto di quella completa, il riquadro è troppo basso: allungalo un po'.

## 7. Pubblica

1. Clicca **Pubblica** in alto a destra.
2. **Indirizzo web**: `suite-live` (l'indirizzo sarà `https://sites.google.com/view/suite-live`). Se è già preso: `suitelive-app`, `live-suite-eventi` o simili.
3. In **Chi può visualizzare il mio sito** lascia **Chiunque** (con un account Google Workspace di lavoro controlla che non sia limitato all'organizzazione).
4. **Pubblica**. Apri l'indirizzo in una finestra in incognito per vederlo come un visitatore e prova un paio di pulsanti «Scarica».

Ogni modifica successiva diventa visibile solo dopo aver cliccato di nuovo **Pubblica**.

## 8. Facoltativo: un dominio tuo

Se hai (o compri) un dominio, per esempio `suitelive.it`:
1. **Impostazioni → Domini personalizzati → Inizia la configurazione**.
2. Scrivi un sottodominio, per esempio `www.suitelive.it` (Google Sites funziona con `www.`, non con il dominio «nudo»).
3. Google chiede di verificare il dominio: segui la procedura (aggiungi il record TXT che ti indica presso il provider del dominio).
4. Presso il provider aggiungi un record **CNAME** per `www` che punta a `ghs.googlehosted.com`.
5. Dal pannello del provider imposta il reindirizzamento da `suitelive.it` a `www.suitelive.it`.

La propagazione può richiedere da qualche minuto a qualche ora.

## 9. Quando esce una nuova versione di un'app

**Non devi cambiare niente**: tutti i pulsanti puntano a `releases/latest/download/<file>`, quindi scaricano sempre l'ultima Release su GitHub, e il numero di versione nei riquadri si aggiorna da solo.

L'unica regola, la stessa di LiveSuite: **ogni Release deve contenere l'asset con il nome fisso** (`LiveCue-Setup.exe`, `LiveDeck-Setup.exe`, `LiveCount-Setup.exe` e `LiveCount.exe`, `LiveCaption-Setup.exe` e `LiveCaption.zip`, `LiveSuite.exe`).

Devi toccare il sito solo se cambi le funzioni di un'app (aggiorna il testo della sua pagina e ripubblica) o se aggiungi un'app nuova:
1. aggiungi l'app in `tools/make_site.py` (lista `APPS`) e la sua icona in `src/LiveSuite/Assets/`;
2. `python tools/make_site.py`;
3. in Google Sites crea la pagina nuova, sostituisci il blocco della Home con il nuovo `schede-app.html` (clic sul blocco → icona matita → incolla il codice nuovo) e ripubblica.

## Note

- I pulsanti dei blocchi HTML aprono il download in una nuova scheda: dentro il riquadro di Google Sites un download diretto può essere bloccato dal browser.
- I pulsanti «Dettagli» nella Home portano alla pagina GitHub dell'app. Dopo la pubblicazione, se preferisci che portino alle pagine del sito, sostituisci in `tools/make_site.py` l'indirizzo GitHub con quello della pagina (per esempio `https://sites.google.com/view/suite-live/livecue`) e rigenera.
