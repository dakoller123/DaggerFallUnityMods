namespace MightyMagick
{
    public class MightyMagickModSettings
    {
        public RegenSettings RegenSettings { get; set; } = new RegenSettings();
        public SpellCostSettings SpellCostSettings { get; set; } = new SpellCostSettings();
        public PotionSettings PotionSettings {get;set; } = new PotionSettings();
        public SavingThrowSettings SavingThrowSettings {get;set;} = new SavingThrowSettings();
        public MagickaPoolSettings MagickaPoolSettings {get;set;} = new MagickaPoolSettings(); 

        public MagickaEnchantSettings MagickaEnchantSettings {get;set;} = new MagickaEnchantSettings();
    }
    public class MagickaEnchantSettings
    {
        public bool Enabled {get;set;}
        public int EnchantMagnitude { get; set; }
    }

    public class SpellCostSettings 
    {
        public bool Enabled { get; set; }

    }

    public class SavingThrowSettings 
    {
        public bool Enabled { get; set; }

    }

    public class MagickaPoolSettings 
    {
        public bool Enabled { get; set; }

    }

    public enum PotionMagnitudeCalculationTypes
    {
        Percentage,
        Flat
    }


    public class PotionSettings
    {
        public bool Enabled { get; set; }
        public int PotionMagnitude {get;set;}
        public PotionMagnitudeCalculationTypes  MagnitudeCalculation { get; set; }
    }

    public class RegenSettings
    {
        public bool Enabled { get; set; }
        public int RegenRateTavern {get;set;}
        public int RegenRateOutdoor {get;set;}
        public int RegenRateDungeon {get;set;}
    }
}
