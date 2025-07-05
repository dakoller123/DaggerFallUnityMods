namespace MightyMagick
{
    public class MightyMagickModSettings
    {
        public MiscSettings MiscSettings { get; } = new MiscSettings();
        public RegenSettings RegenSettings { get; } = new RegenSettings();
        public SpellCostSettings SpellCostSettings { get; } = new SpellCostSettings();
        public PotionSettings PotionSettings {get; } = new PotionSettings();
        public SavingThrowSettings SavingThrowSettings {get; } = new SavingThrowSettings();
        public MagickaPoolSettings MagickaPoolSettings {get; } = new MagickaPoolSettings();
        public MagickaEnchantSettings MagickaEnchantSettings {get; } = new MagickaEnchantSettings();
        public AbsorbSettings AbsorbSettings {get; } = new AbsorbSettings();

        public MagicEffectSettings MagicEffectSettings { get; } = new MagicEffectSettings();
        public SpellProgressionSettings SpellProgressionSettings { get; } = new SpellProgressionSettings();
    }

    public class MiscSettings
    {
        public bool DisablePressButtonSpam { get; set; }
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

        public bool ArmorPenalty { get; set; }

        public bool WeaponPenalty { get; set; }

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
