using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace LiveSuite
{
    /// <summary>
    /// The same "support the app" window as LiveCue, LiveDeck, LiveCount and LiveCaption, shown once per launch
    /// (see MainWindow's Loaded handler): a Buy Me a Coffee QR code/link in BMC's own brand colors.
    /// "No, grazie" stays disabled for a few seconds, long enough to actually notice the QR code.
    /// </summary>
    public partial class SupportWindow : Window
    {
        public const string SupportUrl = "https://buymeacoffee.com/cin8";
        const int WaitSeconds = 3;

        readonly DispatcherTimer _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        int _secondsLeft = WaitSeconds;

        public SupportWindow()
        {
            InitializeComponent();
            UpdateButtonText();
            _timer.Tick += (_, __) =>
            {
                _secondsLeft--;
                if (_secondsLeft <= 0)
                {
                    _timer.Stop();
                    NoThanksButton.IsEnabled = true;
                    NoThanksButton.Content = "No, grazie";
                }
                else
                {
                    UpdateButtonText();
                }
            };
            _timer.Start();
        }

        void UpdateButtonText() => NoThanksButton.Content = $"No, grazie ({_secondsLeft})";

        void NoThanksButton_Click(object sender, RoutedEventArgs e) => Close();

        void CodeLink_Click(object sender, RoutedEventArgs e) { if (SupporterCodeDialog.Ask(this)) Close(); }

        void Link_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try { Process.Start(new ProcessStartInfo(SupportUrl) { UseShellExecute = true }); }
            catch { /* no browser associated: nothing useful to do */ }
        }
    }
}
