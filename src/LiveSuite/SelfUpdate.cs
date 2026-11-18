using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace LiveSuite
{
    /// <summary>LiveSuite is a single portable .exe: to update it, download the new one, rename the
    /// running file (Windows allows renaming, not overwriting, a running exe), put the new one in its
    /// place and restart. The leftover ".old" is deleted at the next start.</summary>
    public static class SelfUpdate
    {
        static string ExePath => Process.GetCurrentProcess().MainModule.FileName;
        static string OldPath => ExePath + ".old";

        public static void CleanUpPrevious()
        {
            try { if (File.Exists(OldPath)) File.Delete(OldPath); } catch { /* old process still closing: next time */ }
        }

        /// <summary>The newer release of LiveSuite, or null if this one is the latest (or GitHub didn't answer).</summary>
        public static async Task<GitHubRelease> FindNewerAsync()
        {
            try
            {
                var release = await GitHub.GetLatestReleaseAsync(Suite.SelfRepo);
                var asset = release.FindAsset(Suite.SelfAssetName);
                return asset != null && release.Version != null && release.Version > Versions.Current ? release : null;
            }
            catch
            {
                return null; // not being able to update the updater is never worth an error message
            }
        }

        /// <summary>Replaces this exe with the release's one and restarts. Returns false if the
        /// folder isn't writable (e.g. LiveSuite was copied into Program Files).</summary>
        public static async Task<bool> ApplyAsync(GitHubRelease release, IProgress<double> progress)
        {
            var exe = ExePath;
            var downloaded = Path.Combine(Path.GetTempPath(), "LiveSuite", "LiveSuite-" + Versions.Short(release.Version) + ".exe");
            await GitHub.DownloadAsync(release.FindAsset(Suite.SelfAssetName).DownloadUrl, downloaded, progress);
            try
            {
                if (File.Exists(OldPath)) File.Delete(OldPath);
                File.Move(exe, OldPath);
                try
                {
                    File.Copy(downloaded, exe);
                }
                catch
                {
                    File.Move(OldPath, exe); // put the working one back
                    throw;
                }
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (IOException)
            {
                return false;
            }
            finally
            {
                try { File.Delete(downloaded); } catch { }
            }
            Process.Start(new ProcessStartInfo(exe) { UseShellExecute = true });
            return true;
        }
    }
}
