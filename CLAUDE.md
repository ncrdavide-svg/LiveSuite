# LiveSuite — note per Claude

Launcher della suite LIVE (LiveCue, LiveSlides, LiveCount, LiveCaption): mostra versione installata e ultima versione su GitHub di ogni app, scarica ed esegue l'installer, si aggiorna da solo. L'utente parla **italiano**: rispondi in italiano.

Repository: https://github.com/ncrdavide-svg/LiveSuite (pubblico). Le app: `ncrdavide-svg/LiveCue`, `/LiveSlides`, `/LiveCount`, `/LiveCaption` (quest'ultima in Python + PyInstaller, sempre con installer Inno Setup).

## Comandi

```
dotnet build src/LiveSuite -c Release
powershell -ExecutionPolicy Bypass -File build.ps1   # → artifacts\LiveSuite.exe
gh release create vX.Y.Z artifacts\LiveSuite.exe --title "LiveSuite X.Y.Z" --notes "..."
```

## Come funziona

- **.NET Framework 4.8 di proposito** (non .NET 9 come le altre app): è dentro Windows, quindi l'exe è un solo file da ~200 KB senza runtime. Niente pacchetti NuGet a runtime: JSON con `DataContractJsonSerializer`, HTTP con `HttpClient`.
- `Suite.cs`: elenco delle app (repo GitHub, AppId di Inno Setup, icona). Aggiungere un'app = una voce lì + PNG in `Assets/`.
- `GitHub.cs`: API pubblica `releases/latest` (limite 60 richieste/ora per IP) + download con progresso. Se l'API non risponde, il download usa `releases/latest/download/<App>-Setup.exe`, quindi **ogni Release delle app deve avere l'asset col nome fisso `<App>-Setup.exe`**.
- `Installed.cs`: versione installata da `HKCU/HKLM\...\Uninstall\{AppId}_is1` (`DisplayVersion`, `InstallLocation`), viste 64 e 32 bit. Se l'exe non c'è più, l'app conta come non installata.
- `SelfUpdate.cs`: se la Release di LiveSuite ha `LiveSuite.exe` più nuovo, rinomina l'exe in esecuzione in `.old`, copia il nuovo, riavvia; `.old` si cancella all'avvio successivo. Se la cartella non è scrivibile apre la pagina della Release.
- `SupportWindow`: finestra Buy Me a Coffee (QR + buymeacoffee.com/cin8, "No, grazie" dopo 3 s) a ogni avvio, modale, identica a quella di LiveCue, LiveSlides e LiveCount; nel piè di pagina anche il link «Offrimi un caffè».

## Convenzioni (come le altre app)

- Interfaccia in **italiano**; codice, commenti e commit in **inglese**. I commenti spiegano il *perché*.
- Palette condivisa: fondo #16181C, verde GO #22A046, ambra #FFB300.
