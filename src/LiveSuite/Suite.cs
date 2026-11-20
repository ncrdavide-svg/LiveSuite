namespace LiveSuite
{
    /// <summary>One app of the suite: where its releases live and how its installer registers it.</summary>
    public sealed class SuiteApp
    {
        public string Name { get; set; }
        public string Tagline { get; set; }
        public string IconFile { get; set; }

        /// <summary>GitHub "owner/repo". Every release must carry "&lt;Name&gt;-Setup.exe" (fixed name).</summary>
        public string Repo { get; set; }

        /// <summary>Inno Setup AppId (without the doubled brace), used to find the installed version.</summary>
        public string InnoAppId { get; set; }

        /// <summary>Exe names the app had before a rename, still found in old installs.</summary>
        public string[] LegacyExeNames { get; set; } = new string[0];

        /// <summary>Not released to the public yet: the card says so and offers no download.
        /// A copy already on this PC (e.g. a tester's) can still be opened.</summary>
        public bool ComingSoon { get; set; }

        public string ExeName => Name + ".exe";
        public string SetupAssetName => Name + "-Setup.exe";
    }

    public static class Suite
    {
        public const string Owner = "ncrdavide-svg";

        /// <summary>LiveSuite's own repository, checked for self-updates.</summary>
        public const string SelfRepo = Owner + "/LiveSuite";
        public const string SelfAssetName = "LiveSuite.exe";

        // To add an app to the suite: add it here (and its 512px PNG icon in Assets\).
        public static readonly SuiteApp[] Apps =
        {
            new SuiteApp
            {
                Name = "LiveCue",
                Tagline = "Show control: cue audio, video e immagini con GO/PANIC, uscite multi-monitor.",
                IconFile = "LiveCue.png",
                Repo = Owner + "/LiveCue",
                InnoAppId = "{6C0B6F0E-7E1F-4C3B-9A51-2B8E4E7D0C11}",
            },
            new SuiteApp
            {
                Name = "LiveDeck",
                Tagline = "PowerPoint e PDF in sequenza come un'unica presentazione, senza schermate nere.",
                IconFile = "LiveDeck.png",
                Repo = Owner + "/LiveDeck",
                // Called LiveSlides up to 1.3 (same AppId): an old install still counts as installed,
                // so the card offers the update instead of a fresh install.
                LegacyExeNames = new[] { "LiveSlides.exe" },
                // Still being worked on before it goes public.
                ComingSoon = true,
                InnoAppId = "{2535BD68-0D2F-4EC7-8B40-B7CD44F43CDC}",
            },
            new SuiteApp
            {
                Name = "LiveCount",
                Tagline = "Countdown per eventi dal vivo: regia + display a schermo pieno, NDI e schede USB.",
                IconFile = "LiveCount.png",
                Repo = Owner + "/LiveCount",
                InnoAppId = "{766B9C0E-13AB-4EC5-81CF-A64E3E3C3441}",
            },
            new SuiteApp
            {
                Name = "LiveCaption",
                Tagline = "Sottotitoli dal vivo sul secondo schermo: trascrizione, un colore per relatore e traduzione in inglese.",
                IconFile = "LiveCaption.png",
                Repo = Owner + "/LiveCaption",
                InnoAppId = "{BB4CDF44-BF66-4723-A67E-C48D9F7D34C6}",
            },
        };
    }
}
