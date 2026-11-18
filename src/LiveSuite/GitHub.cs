using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LiveSuite
{
    [DataContract]
    public sealed class GitHubRelease
    {
        [DataMember(Name = "tag_name")] public string TagName { get; set; }
        [DataMember(Name = "name")] public string Title { get; set; }
        [DataMember(Name = "html_url")] public string HtmlUrl { get; set; }
        [DataMember(Name = "published_at")] public string PublishedAt { get; set; }
        [DataMember(Name = "assets")] public GitHubAsset[] Assets { get; set; }

        public Version Version => Versions.Parse(TagName);

        public GitHubAsset FindAsset(string name) =>
            Assets?.FirstOrDefault(a => string.Equals(a.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    [DataContract]
    public sealed class GitHubAsset
    {
        [DataMember(Name = "name")] public string Name { get; set; }
        [DataMember(Name = "browser_download_url")] public string DownloadUrl { get; set; }
        [DataMember(Name = "size")] public long Size { get; set; }
    }

    public static class GitHub
    {
        static readonly HttpClient Http = CreateClient();

        static HttpClient CreateClient()
        {
            // .NET Framework only negotiates what it is told to on older setups; GitHub needs TLS 1.2+.
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            var client = new HttpClient { Timeout = TimeSpan.FromMinutes(30) };
            client.DefaultRequestHeaders.UserAgent.ParseAdd("LiveSuite/" + Versions.Current);
            return client;
        }

        /// <summary>Latest published release of "owner/repo". Uses the public API (60 requests/hour per IP, plenty here).</summary>
        public static async Task<GitHubRelease> GetLatestReleaseAsync(string repo, CancellationToken ct = default)
        {
            using (var req = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/repos/{repo}/releases/latest"))
            {
                req.Headers.Accept.ParseAdd("application/vnd.github+json");
                using (var resp = await Http.SendAsync(req, ct).ConfigureAwait(false))
                {
                    if ((int)resp.StatusCode == 403)
                        throw new InvalidOperationException("GitHub ha limitato le richieste da questa rete: riprova tra qualche minuto.");
                    if (resp.StatusCode == HttpStatusCode.NotFound)
                        throw new InvalidOperationException("Nessuna versione pubblicata.");
                    resp.EnsureSuccessStatusCode();
                    using (var stream = await resp.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        return (GitHubRelease)new DataContractJsonSerializer(typeof(GitHubRelease)).ReadObject(stream);
                }
            }
        }

        /// <summary>Downloads url to path, reporting progress 0..1 (or -1 when the size is unknown).</summary>
        public static async Task DownloadAsync(string url, string path, IProgress<double> progress, CancellationToken ct = default)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var partial = path + ".part";
            using (var resp = await Http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false))
            {
                resp.EnsureSuccessStatusCode();
                var total = resp.Content.Headers.ContentLength ?? -1;
                using (var input = await resp.Content.ReadAsStreamAsync().ConfigureAwait(false))
                using (var output = File.Create(partial))
                {
                    var buffer = new byte[81920];
                    long done = 0;
                    int read;
                    while ((read = await input.ReadAsync(buffer, 0, buffer.Length, ct).ConfigureAwait(false)) > 0)
                    {
                        await output.WriteAsync(buffer, 0, read, ct).ConfigureAwait(false);
                        done += read;
                        progress?.Report(total > 0 ? (double)done / total : -1);
                    }
                }
            }
            // Only a complete file gets the real name, so a cancelled download is never run.
            if (File.Exists(path)) File.Delete(path);
            File.Move(partial, path);
        }
    }

    public static class Versions
    {
        public static Version Current => typeof(Versions).Assembly.GetName().Version;

        /// <summary>"v1.2.0", "1.2", "1.2.0.0" → Version; null if it isn't a version.</summary>
        public static Version Parse(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            s = s.Trim().TrimStart('v', 'V');
            var cut = s.IndexOfAny(new[] { '-', '+', ' ' });
            if (cut > 0) s = s.Substring(0, cut);
            if (!s.Contains('.')) s += ".0";
            return Version.TryParse(s, out var v) ? Normalize(v) : null;
        }

        /// <summary>Missing parts count as 0, so 1.2 == 1.2.0 == 1.2.0.0.</summary>
        static Version Normalize(Version v) =>
            new Version(v.Major, v.Minor, Math.Max(v.Build, 0), Math.Max(v.Revision, 0));

        public static string Short(Version v)
        {
            if (v == null) return "";
            if (v.Revision > 0) return v.ToString(4);
            return v.ToString(3);
        }
    }
}
