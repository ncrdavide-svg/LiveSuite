using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LiveSuite
{
    public partial class MainWindow : Window
    {
        readonly AppItem[] _items = Suite.Apps.Select(a => new AppItem(a)).ToArray();
        GitHubRelease _selfRelease;

        public MainWindow()
        {
            InitializeComponent();
            AppList.ItemsSource = _items;
            FooterText.Text = $"LiveSuite {Versions.Short(Versions.Current)} · le installazioni si scaricano da GitHub, dalla pagina «Releases» di ogni app.";
            Loaded += async (_, __) => await RefreshAllAsync();
            // Coming back to the window (e.g. after closing an app or an installer) re-reads what's installed.
            Activated += async (_, __) =>
            {
                if (!_items.Any(i => i.Busy || i.Checking) && (DateTime.Now - _lastRefresh).TotalSeconds > 60)
                    await RefreshAllAsync();
            };
        }

        DateTime _lastRefresh;

        async Task RefreshAllAsync()
        {
            _lastRefresh = DateTime.Now;
            RefreshButton.IsEnabled = false;
            try
            {
                var self = CheckSelfAsync();
                await Task.WhenAll(_items.Where(i => !i.Busy).Select(i => i.RefreshAsync()));
                await self;
            }
            finally
            {
                RefreshButton.IsEnabled = true;
            }
        }

        async Task CheckSelfAsync()
        {
            _selfRelease = await SelfUpdate.FindNewerAsync();
            if (_selfRelease == null) return;
            SelfUpdateText.Text = $"È disponibile LiveSuite {Versions.Short(_selfRelease.Version)} (questa è la {Versions.Short(Versions.Current)}).";
            SelfUpdateBanner.Visibility = Visibility.Visible;
        }

        async void Refresh_Click(object sender, RoutedEventArgs e) => await RefreshAllAsync();

        static AppItem ItemOf(object sender) => (sender as FrameworkElement)?.DataContext as AppItem;

        async void Primary_Click(object sender, RoutedEventArgs e)
        {
            var item = ItemOf(sender);
            if (item == null) return;
            if (item.PrimaryIsDownload) await InstallAsync(item);
            else item.Open();
        }

        void Open_Click(object sender, RoutedEventArgs e) => ItemOf(sender)?.Open();

        async void Reinstall_Click(object sender, RoutedEventArgs e)
        {
            var item = ItemOf(sender);
            if (item != null) await InstallAsync(item);
        }

        async Task InstallAsync(AppItem item)
        {
            var error = await item.InstallLatestAsync();
            if (error != null) MessageBox.Show(this, error, "LiveSuite", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        void Releases_Click(object sender, RoutedEventArgs e)
        {
            var item = ItemOf(sender);
            if (item != null) OpenUrl(item.ReleasesUrl);
        }

        void SelfRepo_Click(object sender, RoutedEventArgs e) => OpenUrl($"https://github.com/{Suite.SelfRepo}");

        async void SelfUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_selfRelease == null) return;
            SelfUpdateButton.IsEnabled = false;
            try
            {
                var progress = new Progress<double>(p => SelfUpdateButton.Content = p < 0 ? "Download…" : $"Download… {p:P0}");
                if (await SelfUpdate.ApplyAsync(_selfRelease, progress))
                {
                    Application.Current.Shutdown();
                    return;
                }
                MessageBox.Show(this,
                    "Non posso sostituire LiveSuite nella cartella in cui si trova (servono permessi da amministratore).\n\n" +
                    "Si apre la pagina da cui scaricare la versione nuova: mettila al posto di questa.",
                    "LiveSuite", MessageBoxButton.OK, MessageBoxImage.Information);
                OpenUrl(_selfRelease.HtmlUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Aggiornamento di LiveSuite non riuscito: " + AppItem.Friendly(ex), "LiveSuite",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            SelfUpdateButton.Content = "Aggiorna LiveSuite";
            SelfUpdateButton.IsEnabled = true;
        }

        static void OpenUrl(string url) => Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }
}
