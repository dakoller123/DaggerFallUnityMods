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

namespace LevitateRedoneMod
{
    public class LevitateRedoneMod : MonoBehaviour
    {
        private static Mod mod;
        private LevitateRedoneMagicEffect templateEffect;
        public static LevitateRedoneMod Instance;

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;

            var go = new GameObject(mod.Title);
            go.AddComponent<LevitateRedoneMod>();
        }


        void Awake()
        {
            Instance = this;
            InitMod();
            mod.IsReady = true;
        }


        public void InitMod()
        {
            Debug.Log("Begin mod init: LevitateRedoneMod");

            templateEffect = new LevitateRedoneMagicEffect();
            GameManager.Instance.EntityEffectBroker.RegisterEffectTemplate(templateEffect, true);

            Debug.Log("Finished mod init: LevitateRedoneMod");
        }
      
    }
}