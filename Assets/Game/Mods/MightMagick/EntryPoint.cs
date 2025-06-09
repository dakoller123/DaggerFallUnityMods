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
            ModSettings settings = mod.GetSettings();

            MightyMagickModSettings.RegenSettings.Enabled = settings.GetValue<bool>("RegenModule", "Enabled");
            MightyMagickModSettings.RegenSettings.RegenRateTavern = settings.GetValue<int>("RegenModule", "RegenRateTavern");
            MightyMagickModSettings.RegenSettings.RegenRateOutdoor  = settings.GetValue<int>("RegenModule", "RegenRateOutdoor");
            MightyMagickModSettings.RegenSettings.RegenRateDungeon  = settings.GetValue<int>("RegenModule", "RegenRateDungeon");
            Instance = this;

            InitMod();
            mod.IsReady = true;


            Instance = this;
            InitMod();
            mod.IsReady = true;
           
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