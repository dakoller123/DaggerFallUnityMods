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

namespace PotionOfPowerMod
{
    public class PotionOfPowerMod : MonoBehaviour
    {
        private static Mod mod;
        private HealSpellPoints templateEffect;
        public static PotionOfPowerMod Instance;

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;

            var go = new GameObject(mod.Title);
            go.AddComponent<PotionOfPowerMod>();
        }


        void Awake()
        {
            Instance = this;
            InitMod();
            mod.IsReady = true;
        }


        public void InitMod()
        {
            Debug.Log("Begin mod init: PotionOfPowerMod");

            templateEffect = new HealSpellPoints();
            GameManager.Instance.EntityEffectBroker.RegisterEffectTemplate(templateEffect, true);

            Debug.Log("Finished mod init: PotionOfPowerMod");
        }
      
    }
}