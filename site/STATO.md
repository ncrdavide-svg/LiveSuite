# Stato del sito su Google Sites

Aggiornato il 19 novembre 2026. Sito: «Suite LIVE» nel Google Sites dell'account del progetto, **non ancora pubblicato**.

## Fatto

- Logo, favicon e le 8 pagine nel menu (Home, LiveSuite, LiveCue, LiveDeck, LiveCount, LiveCaption, Domande frequenti, Supporto).
- Ogni pagina ha banner, blocco incorporato e testo.

## Da fare

1. **Sostituire i 7 blocchi incorporati** con la versione nuova di `incorpora/` (ha la versione compatta per telefono; quelli inseriti prima su telefono appaiono tagliati). In ogni pagina: clic sul blocco → matita (Modifica) → Ctrl+A, incolla il codice nuovo → Avanti → Inserisci/Aggiorna.
   - Supporto → `caffe.html`
   - Home → `schede-app.html`
   - LiveSuite, LiveCue, LiveDeck, LiveCount, LiveCaption → `scarica-<app>.html`
2. Controllare con **Anteprima → Telefono** (icona schermo in alto) che nessun blocco abbia la barra di scorrimento interna.
3. **Pubblica** con indirizzo `suite-live` (vedi punto 7 della guida), visibile a **Chiunque**.
4. Prova in incognito un paio di pulsanti «Scarica».

Per copiare il codice di un blocco: aprire il file `.html` con il Blocco note, Ctrl+A, Ctrl+C. Su un altro computer prima `git pull` nella cartella LiveSuite.
