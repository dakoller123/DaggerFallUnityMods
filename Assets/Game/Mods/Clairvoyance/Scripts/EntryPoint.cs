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

namespace ClairvoyanceMod
{
    public class ClairvoyanceMod : MonoBehaviour
    {
        private static Mod mod;
        private DetectQuest templateEffect;
        public static ClairvoyanceMod Instance;

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;

            var go = new GameObject(mod.Title);
            go.AddComponent<ClairvoyanceMod>();
        }


        void Awake()
        {
            Instance = this;
            InitMod();
            mod.IsReady = true;
        }


        public void InitMod()
        {
            Debug.Log("Begin mod init: ClairvoyanceMod");

            templateEffect = new DetectQuest();
            GameManager.Instance.EntityEffectBroker.RegisterEffectTemplate(templateEffect);

            Debug.Log("Finished mod init: ClairvoyanceMod");
        }
      
    }
}