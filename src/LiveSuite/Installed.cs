using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace LiveSuite
{
    public sealed class InstalledInfo
    {
        public string Version { get; set; }
        public string ExePath { get; set; }
    }

    /// <summary>Finds an app installed by its Inno Setup installer, which registers
    /// "&lt;AppId&gt;_is1" under Uninstall, for all users (HKLM) or only the current one (HKCU).</summary>
    public static class Installed
    {
        public static InstalledInfo Find(SuiteApp app)
        {
            var subKey = $@"Software\Microsoft\Windows\CurrentVersion\Uninstall\{app.InnoAppId}_is1";
            foreach (var hive in new[] { RegistryHive.CurrentUser, RegistryHive.LocalMachine })
            foreach (var view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
            {
                try
                {
                    using (var root = RegistryKey.OpenBaseKey(hive, view))
                    using (var key = root.OpenSubKey(subKey))
                    {
                        if (key == null) continue;
                        var dir = key.GetValue("InstallLocation") as string;
                        if (string.IsNullOrEmpty(dir)) continue;
                        // An entry whose program is gone (deleted by hand) doesn't count as installed.
                        var exe = new[] { app.ExeName }.Concat(app.LegacyExeNames)
                            .Select(name => Path.Combine(dir, name))
                            .FirstOrDefault(File.Exists);
                        if (exe == null) continue;
                        return new InstalledInfo { Version = key.GetValue("DisplayVersion") as string, ExePath = exe };
                    }
                }
                catch
                {
                    // A hive/view we can't read is just "not there".
                }
            }
            return null;
        }
    }
}
