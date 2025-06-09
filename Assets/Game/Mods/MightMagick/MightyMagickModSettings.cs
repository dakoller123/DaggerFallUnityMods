namespace MightyMagick
{
    public class MightyMagickModSettings
    {
        public RegenSettings RegenSettings { get; set; } = new RegenSettings();
    }

    public class RegenSettings
    {
        public bool Enabled { get; set; }
        public int RegenRateTavern {get;set;}
        public int RegenRateOutdoor {get;set;}
        public int RegenRateDungeon {get;set;}
    }
}
