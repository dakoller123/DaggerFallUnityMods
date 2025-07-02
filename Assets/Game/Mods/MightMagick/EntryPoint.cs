using System;
using System.Linq;
using DaggerfallConnect;
using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using MightyMagick.MagicEffects;
using MightyMagick.Formulas;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;
using MightyMagick.SpellProgressionModule;

namespace MightyMagick
{
    public class MightyMagickMod : MonoBehaviour
    {
        private static Mod mod;
        private EffectRegister effectRegister;
        private HealSpellPoints templateEffect;
        public static MightyMagickMod Instance;

        public MightyMagickModSettings MightyMagickModSettings { get; set; } = new MightyMagickModSettings();

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;

            var go = new GameObject(mod.Title);
            go.AddComponent<MightyMagickMod>();
        }

        void Awake()
        {
            Instance = this;
            this.MightyMagickModSettings = ParseSettings();
            InitMod();
            mod.IsReady = true;
        }

        MightyMagickModSettings ParseSettings()
        {
            var result = new MightyMagickModSettings();
            ModSettings settings = mod.GetSettings();

            result.RegenSettings.Enabled = settings.GetValue<bool>("MagickaRegenModule", "Enabled");
            result.RegenSettings.RegenRateTavern = settings.GetValue<int>("MagickaRegenModule", "RegenRateTavern");
            result.RegenSettings.RegenRateOutdoor  = settings.GetValue<int>("MagickaRegenModule", "RegenRateOutdoor");
            result.RegenSettings.RegenRateDungeon  = settings.GetValue<int>("MagickaRegenModule", "RegenRateDungeon");

            result.SpellCostSettings.Enabled = settings.GetValue<bool>("SpellCostModule", "Enabled");
            result.SpellCostSettings.Multiplier  = settings.GetValue<float>("SpellCostModule", "Multiplier");

            result.PotionSettings.Enabled = settings.GetValue<bool>("PotionModule", "Enabled");

            result.PotionSettings.MagnitudeCalculation =
                (settings.GetValue<int>("PotionModule", "MagnitudeCalculation") == 1)
                ? PotionMagnitudeCalculationTypes.Flat
                : PotionMagnitudeCalculationTypes.Percentage;

            result.PotionSettings.PotionMagnitude = settings.GetValue<int>("PotionModule", "PotionMagnitude");
            result.PotionSettings.PotionsAtStart =  settings.GetValue<int>("PotionModule", "PotionsAtStart");

            result.MagickaPoolSettings.Enabled = settings.GetValue<bool>("MagickaPoolModule", "Enabled");
            result.MagickaPoolSettings.LevelUpFlatIncrease = settings.GetValue<int>("MagickaPoolModule", "LevelUpFlatIncrease");
            result.MagickaPoolSettings.LevelUpPercentageIncrease = settings.GetValue<int>("MagickaPoolModule", "LevelUpPercentageIncrease");
            result.MagickaPoolSettings.Multiplier  = settings.GetValue<float>("MagickaPoolModule", "Multiplier");

            result.MagickaEnchantSettings.Enabled = settings.GetValue<bool>("MagickaEnchantModule", "Enabled");
            result.MagickaEnchantSettings.EnchantMagnitude = settings.GetValue<int>("MagickaEnchantModule", "EnchantMagnitude");

            result.SavingThrowSettings.Enabled = settings.GetValue<bool>("SavingThrowModule", "Enabled");
            result.SavingThrowSettings.Multiplier  = settings.GetValue<float>("SavingThrowModule", "Multiplier");

            result.AbsorbSettings.Enabled = settings.GetValue<bool>("SpellAbsorbModule", "Enabled");
            result.AbsorbSettings.AllowNonDestructionAbsorbs = settings.GetValue<bool>("SpellAbsorbModule", "AllowNonDestructionAbsorbs");
            result.AbsorbSettings.AllowOwnSpellAbsorbs = settings.GetValue<bool>("SpellAbsorbModule", "AllowOwnSpellAbsorbs");
            result.AbsorbSettings.CalculateSpellCostWithCaster = settings.GetValue<bool>("SpellAbsorbModule", "CalculateSpellCostWithCaster");
            result.AbsorbSettings.CalculateWithResistances = settings.GetValue<bool>("SpellAbsorbModule", "CalculateWithResistances");
            result.AbsorbSettings.SpellCostRegenMultiplier = settings.GetValue<float>("SpellAbsorbModule", "SpellCostRegenMultiplier");
            result.AbsorbSettings.CareerAbsorbChance = settings.GetValue<int>("SpellAbsorbModule", "CareerAbsorbChance");

            result.SpellProgressionSettings.LimitSpellCastBySkill = settings.GetValue<bool>("SpellProgressionModule", "LimitSpellCastBySkill");
            result.SpellProgressionSettings.LimitSpellBuyBySkill = settings.GetValue<bool>("SpellProgressionModule", "LimitSpellBuyBySkill");
            result.SpellProgressionSettings.LimitSpellMakerToKnownEffects = settings.GetValue<bool>("SpellProgressionModule", "LimitSpellMakerToKnownEffects");
            result.SpellProgressionSettings.SpellCostCheckMultiplier = settings.GetValue<float>("SpellProgressionModule", "SpellCostCheckMultiplier");

            result.MagicEffectSettings.LevitateHasMagnitude = settings.GetValue<bool>("MagicEffectOverridesModule", "LevitateHasMagnitude");
            result.MagicEffectSettings.HideMagicCandle = settings.GetValue<bool>("MagicEffectOverridesModule", "HideMagicCandle");
            result.MagicEffectSettings.AddMoreVendorSpells =  settings.GetValue<bool>("MagicEffectOverridesModule", "AddMoreVendorSpells");
            result.MagicEffectSettings.AddMageArmor =  settings.GetValue<bool>("MagicEffectOverridesModule", "AddMageArmor");
            result.MagicEffectSettings.JumpingHasMagnitude =  settings.GetValue<bool>("MagicEffectOverridesModule", "JumpingHasMagnitude");
            result.MagicEffectSettings.AddDetectQuest =  settings.GetValue<bool>("MagicEffectOverridesModule", "AddDetectQuest");
            result.MagicEffectSettings.AddStartingSpells =  settings.GetValue<bool>("MagicEffectOverridesModule", "AddStartingSpells");
            return result;
        }

        public void InitMod()
        {
            Debug.Log("Begin mod init: MightyMagickMod");
            effectRegister = new EffectRegister();
            effectRegister.RegisterNewMagicEffects();
            FormulaOverrides.RegisterFormulaOverrides(mod);

            if (MightyMagickModSettings.SpellProgressionSettings.LimitSpellMakerToKnownEffects)
            {
                DaggerfallWorkshop.Game.UserInterfaceWindows.UIWindowFactory.RegisterCustomUIWindow(DaggerfallWorkshop.Game.UserInterfaceWindows.UIWindowType.SpellMaker, typeof(MightyMagickSpellMakerWindow));
            }



            HarmonyPatcher.TryApplyPatch();

            if (MightyMagickModSettings.MagicEffectSettings.AddMoreVendorSpells)
            {
                NewVendorSpells.RegisterSpells();
            }

            StartGameBehaviour.OnStartGame += OnNewGameStarted;

            Debug.Log("Finished mod init: MightyMagickMod");
        }

        static void OnNewGameStarted(object sender, EventArgs e)
        {
            Debug.Log("MightyMagickMod - OnNewGameStarted");
            var player = GameManager.Instance.PlayerEntity;
            if (MightyMagickMod.Instance.MightyMagickModSettings.PotionSettings.PotionsAtStart > 0)
            {

                Debug.Log("MightyMagickMod - Adding potions");
                var potion = ItemBuilder.CreatePotion(5188896, MightyMagickMod.Instance.MightyMagickModSettings.PotionSettings.PotionsAtStart);
                player.Items.AddItem(potion);

            }

            var magicSettings = Instance.MightyMagickModSettings.MagicEffectSettings;

            if (magicSettings.AddStartingSpells)
            {
                if (magicSettings.AddMoreVendorSpells)
                {
                    Debug.Log("MightyMagickMod - Checking starting skills");
                    if (player.Career.PrimarySkill1 == DFCareer.Skills.Alteration ||
                        player.Career.PrimarySkill2 == DFCareer.Skills.Alteration ||
                        player.Career.PrimarySkill3 == DFCareer.Skills.Alteration)
                    {
                        if (magicSettings.AddMageArmor)
                        {
                            Debug.Log("MightyMagickMod - Adding Minor Mage Armor to player");
                            var spell =
                                GameManager.Instance.EntityEffectBroker
                                    .GetCustomSpellBundleOffer("MinorMageArmor-CustomOffer");
                            // player.AddSpell(NewVendorSpells.MinorMageArmor());
                        }
                        player.AddSpell(NewVendorSpells.MinorShield());
                        // if (magicSettings.CheaperShield)
                        // {
                        //     Debug.Log("MightyMagickMod - Adding Minor Shield to player");
                        //     player.AddSpell(NewVendorSpells.MinorShield());
                        // }
                    }
                }
            }
        }
    }
}
