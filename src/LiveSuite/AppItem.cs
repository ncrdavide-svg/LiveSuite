using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LiveSuite
{
    public enum AppState { Checking, NotInstalled, UpdateAvailable, UpToDate, Unknown }

    /// <summary>One card in the window: an app of the suite, what's installed and what's on GitHub.</summary>
    public sealed class AppItem : INotifyPropertyChanged
    {
        static readonly CultureInfo Italian = new CultureInfo("it-IT");

        public AppItem(SuiteApp app)
        {
            App = app;
            Icon = new BitmapImage(new Uri($"pack://application:,,,/Assets/{app.IconFile}"));
        }

        public SuiteApp App { get; }
        public string Name => App.Name;
        public string Tagline => App.Tagline;
        public ImageSource Icon { get; }

        GitHubRelease _release;
        InstalledInfo _installed;
        string _error;
        bool _checking = true, _busy;
        double _progress;
        string _busyText;

        public GitHubRelease Release { get => _release; private set { _release = value; Changed(); } }
        public InstalledInfo Installed { get => _installed; private set { _installed = value; Changed(); } }
        public string Error { get => _error; private set { _error = value; Changed(); } }
        public bool Checking { get => _checking; private set { _checking = value; Changed(); } }
        public bool Busy { get => _busy; private set { _busy = value; Changed(); } }
        public double Progress { get => _progress; private set { _progress = value; OnChanged(nameof(Progress)); OnChanged(nameof(ProgressIndeterminate)); } }
        public bool ProgressIndeterminate => _progress < 0;
        public string BusyText { get => _busyText; private set { _busyText = value; OnChanged(nameof(BusyText)); } }

        public Version InstalledVersion => Versions.Parse(Installed?.Version);
        public Version LatestVersion => Release?.Version;

        public AppState State
        {
            get
            {
                if (Checking) return AppState.Checking;
                if (Installed == null) return AppState.NotInstalled;
                if (LatestVersion == null || InstalledVersion == null) return AppState.Unknown;
                return LatestVersion > InstalledVersion ? AppState.UpdateAvailable : AppState.UpToDate;
            }
        }

        public string BadgeText
        {
            get
            {
                switch (State)
                {
                    case AppState.Checking: return "CONTROLLO…";
                    case AppState.NotInstalled: return "NON INSTALLATA";
                    case AppState.UpdateAvailable: return "AGGIORNAMENTO";
                    case AppState.UpToDate: return "AGGIORNATA";
                    default: return "INSTALLATA";
                }
            }
        }

        public string InstalledText => Installed == null
            ? "Non installata su questo PC"
            : "Installata: " + (InstalledVersion != null ? Versions.Short(InstalledVersion) : Installed.Version ?? "versione sconosciuta");

        public string LatestText
        {
            get
            {
                if (Checking) return "Controllo dell'ultima versione…";
                if (Release == null) return "Ultima versione non disponibile" + (string.IsNullOrEmpty(Error) ? "" : " — " + Error);
                var text = "Ultima versione: " + Versions.Short(LatestVersion);
                if (DateTime.TryParse(Release.PublishedAt, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var date))
                    text += " del " + date.ToLocalTime().ToString("d MMMM yyyy", Italian);
                return text;
            }
        }

        public string PrimaryText
        {
            get
            {
                switch (State)
                {
                    case AppState.Checking: return "…";
                    case AppState.NotInstalled: return "Installa";
                    case AppState.UpdateAvailable: return "Aggiorna a " + Versions.Short(LatestVersion);
                    default: return "Apri";
                }
            }
        }

        /// <summary>Primary button downloads (green) rather than opens (neutral).</summary>
        public bool PrimaryIsDownload => State == AppState.NotInstalled || State == AppState.UpdateAvailable;
        public bool CanPrimary => !Busy && State != AppState.Checking;
        public bool ShowOpenSecondary => !Busy && State == AppState.UpdateAvailable;
        public bool ShowReinstall => !Busy && (State == AppState.UpToDate || State == AppState.Unknown);

        public string ReleasesUrl => Release?.HtmlUrl ?? $"https://github.com/{App.Repo}/releases/latest";

        public async Task RefreshAsync()
        {
            Checking = true;
            Error = null;
            Installed = await Task.Run(() => LiveSuite.Installed.Find(App));
            try
            {
                Release = await GitHub.GetLatestReleaseAsync(App.Repo);
            }
            catch (Exception ex)
            {
                Release = null;
                Error = Friendly(ex);
            }
            Checking = false;
        }

        public void Open()
        {
            if (Installed?.ExePath == null) return;
            Process.Start(new ProcessStartInfo(Installed.ExePath) { UseShellExecute = true, WorkingDirectory = Path.GetDirectoryName(Installed.ExePath) });
        }

        /// <summary>Downloads the installer of the latest release and runs it; returns an error message or null.</summary>
        public async Task<string> InstallLatestAsync()
        {
            if (Busy) return null;
            Busy = true;
            Progress = 0;
            BusyText = "Download in corso…";
            var file = Path.Combine(Path.GetTempPath(), "LiveSuite",
                $"{App.Name}-Setup-{(LatestVersion != null ? Versions.Short(LatestVersion) : "latest")}.exe");
            try
            {
                // The fixed-name asset is in every release; if the API didn't answer, GitHub's
                // "latest/download" link still points at it.
                var url = Release?.FindAsset(App.SetupAssetName)?.DownloadUrl
                          ?? $"https://github.com/{App.Repo}/releases/latest/download/{App.SetupAssetName}";
                var progress = new Progress<double>(p =>
                {
                    Progress = p;
                    BusyText = p < 0 ? "Download in corso…" : $"Download in corso… {p:P0}";
                });
                await GitHub.DownloadAsync(url, file, progress);

                BusyText = "Installazione in corso: segui la finestra dell'installer…";
                Progress = -1;
                Process setup;
                try
                {
                    setup = Process.Start(new ProcessStartInfo(file) { UseShellExecute = true });
                }
                catch (System.ComponentModel.Win32Exception ex) when (ex.NativeErrorCode == 1223)
                {
                    return null; // the user said no to the Windows permission prompt
                }
                if (setup != null) await Task.Run(() => setup.WaitForExit());
                return null;
            }
            catch (Exception ex)
            {
                return $"Non sono riuscito a installare {App.Name}: {Friendly(ex)}";
            }
            finally
            {
                TryDelete(file);
                Busy = false;
                BusyText = null;
                await RefreshAsync();
            }
        }

        static void TryDelete(string file)
        {
            try { if (File.Exists(file)) File.Delete(file); } catch { /* still in use: the temp folder is cleaned later */ }
        }

        public static string Friendly(Exception ex)
        {
            if (ex is AggregateException agg && agg.InnerException != null) ex = agg.InnerException;
            if (ex is TaskCanceledException) return "la connessione è troppo lenta o assente.";
            if (ex is System.Net.Http.HttpRequestException) return "connessione a GitHub non riuscita (sei collegato a internet?).";
            return ex.Message;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        /// <summary>Any state change can affect every derived property: raise them all (it's three cards).</summary>
        void Changed([CallerMemberName] string name = null) => OnChanged(string.Empty);
    }
}
