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

namespace MightyMagickMod
{
    public class MightyMagickMod : MonoBehaviour
    {
        private static Mod mod;
        private HealSpellPoints templateEffect;
        public static MightyMagickMod Instance;

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
        }


        public void InitMod()
        {
            Debug.Log("Begin mod init: MightyMagickMod");

            templateEffect = new HealSpellPoints();
            GameManager.Instance.EntityEffectBroker.RegisterEffectTemplate(templateEffect, true);

            Debug.Log("Finished mod init: MightyMagickMod");
        }
      
    }
}