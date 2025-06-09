// Project:   Clairvoyance for Daggerfall Unity
// Author:    kiskoller
// Based on code from:    DunnyOfPenwick

using System;
using UnityEngine;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Formulas;
using MightyMagick.MagicEffects;
using MightyMagick.Formulas;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;

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

            result.PotionSettings.Enabled = settings.GetValue<bool>("PotionModule", "Enabled");
            var calculationType = settings.GetValue<string>("PotionModule", "MagnitudeCalculation");
            result.PotionSettings.MagnitudeCalculation = Enum.Parse<PotionMagnitudeCalculationTypes>(calculationType);
            result.PotionSettings.PotionMagnitude = settings..GetValue<int>("PotionModule", "PotionMagnitude");

            result.MagickaPoolSettings.Enabled = settings.GetValue<bool>("MagickaPoolModule", "Enabled");

            result.MagickaEnchantSettings.Enabled = settings.GetValue<bool>("MagickaEnchantModule", "Enabled");
            result.MagickaEnchantSettings.EnchantMagnitude = settings.GetValue<int>("MagickaEnchantModule", "EnchantMagnitude");

            result.SavingThrowSettings.Enabled = settings.GetValue<bool>("SavingThrowModule", "Enabled");

            return result;
        }

        public void InitMod()
        {
            Debug.Log("Begin mod init: MightyMagickMod");
            effectRegister = new EffectRegister();
            effectRegister.RegisterNewMagicEffects();
            FormulaOverrides.RegisterFormulaOverrides(mod);          
            Debug.Log("Finished mod init: MightyMagickMod");
        }
      
    }
}