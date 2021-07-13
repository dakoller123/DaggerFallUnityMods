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
        private HealSpellPoints templateEffect;
        public static MightyMagickMod Instance;
        ModSettings settings;

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
            InitMod();
            mod.IsReady = true;
            //StartGameBehaviour.OnStartGame += MightyMagick_OnStartGame; 
        }

        float CalculateBonusSpellPointForAllLevelups(double bonusForSingleLevelup, int playerLevel)
        {
            return (float)(Math.Pow(bonusForSingleLevelup, (double)playerLevel));
        }

        void RaiseSpellPoints()
        {
            var playerEntity = GameManager.Instance.PlayerEntity;
            double multiplier = (double) settings.GetValue<int>("SpellPoints", "SpellPointsLevelupMultiplier");
            playerEntity.Career.SpellPointMultiplierValue *= CalculateBonusSpellPointForAllLevelups(multiplier, playerEntity.Level);
        }

        private void MightyMagick_OnStartGame(object sender, EventArgs e) 
        {
            RaiseSpellPoints();
        }

            public void InitMod()
        {
            Debug.Log("Begin mod init: MightyMagickMod");
            settings = mod.GetSettings();
            templateEffect = new HealSpellPoints();
            GameManager.Instance.EntityEffectBroker.RegisterEffectTemplate(templateEffect, true);
            FormulaOverrides.RegisterFormulaOverrides(mod);       
            Debug.Log("Finished mod init: MightyMagickMod");
        }
      
    }
}