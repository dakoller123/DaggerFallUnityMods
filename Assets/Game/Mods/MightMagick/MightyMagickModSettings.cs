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
        public AbsorbSettings AbsorbSettings {get;set;} = new AbsorbSettings();

        public MagicEffectSettings MagicEffectSettings { get; set; } = new MagicEffectSettings();
        public SpellProgressionSettings SpellProgressionSettings { get; set; } = new SpellProgressionSettings();
    }

    public class MagicEffectSettings
    {
        public bool HideMagicCandle { get; set; }
        public bool LevitateHasMagnitude { get; set; }

        public bool AddUnleveledVendorSpells { get; set; }
        public bool CheaperShield { get; set; } = false;
        public bool AddMageArmor { get; set; }
        public bool JumpingHasMagnitude { get; set; }
        public bool AddDetectQuest { get; set; }

        public bool AddStartingSpells { get; set; }
        public bool AnducarsGrimoire { get; set; }
    }

    public class SpellProgressionSettings
    {
        public bool LimitSpellCastBySkill { get; set; }
        public bool LimitSpellBuyBySkill { get; set; }
        public bool LimitSpellMakerToKnownEffects { get; set; }
        public float SpellCostCheckMultiplier { get; set; }
    }

    public class AbsorbSettings
    {
        public bool Enabled { get; set; }
        public bool AllowNonDestructionAbsorbs { get; set; }
        public bool AllowOwnSpellAbsorbs { get; set; }
        public bool CalculateSpellCostWithCaster { get; set; }
        public bool CalculateWithResistances {get;set; }
        public int CareerAbsorbChance { get; set; }
        public float SpellCostRegenMultiplier { get; set; }
    }

    public class MagickaEnchantSettings
    {
        public bool Enabled {get;set;}
        public int EnchantMagnitude { get; set; }
    }

    public class SpellCostSettings
    {
        public bool Enabled { get; set; }
        public float Multiplier { get; set; }

    }

    public class SavingThrowSettings
    {
        public bool Enabled { get; set; }
        public float Multiplier { get; set; }
    }

    public class MagickaPoolSettings
    {
        public bool Enabled { get; set; }
        public int LevelUpPercentageIncrease { get; set; }
        public int LevelUpFlatIncrease { get; set; }
        public float Multiplier { get; set; }
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
        public int PotionsAtStart { get; set; }
    }

    public class RegenSettings
    {
        public bool Enabled { get; set; }
        public int RegenRateTavern {get;set;}
        public int RegenRateOutdoor {get;set;}
        public int RegenRateDungeon {get;set;}
    }
}
