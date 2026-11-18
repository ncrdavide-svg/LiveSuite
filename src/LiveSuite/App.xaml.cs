using System.Windows;

namespace LiveSuite
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            SelfUpdate.CleanUpPrevious();
            DispatcherUnhandledException += (_, args) =>
            {
                MessageBox.Show(AppItem.Friendly(args.Exception), "LiveSuite", MessageBoxButton.OK, MessageBoxImage.Warning);
                args.Handled = true;
            };
            base.OnStartup(e);
        }
    }
}
