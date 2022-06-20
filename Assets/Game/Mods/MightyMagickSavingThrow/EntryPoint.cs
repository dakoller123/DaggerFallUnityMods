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
using MightyMagick.Formulas;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;

namespace MightyMagick
{
    public class MightyMagickSavingThrowMod : MonoBehaviour
    {
        private static Mod mod;
        public static MightyMagickSavingThrowMod Instance;

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;

            var go = new GameObject(mod.Title);
            go.AddComponent<MightyMagickSavingThrowMod>();
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
            FormulaOverrides.RegisterFormulaOverrides(mod);          
            Debug.Log("Finished mod init: MightyMagickMod");
        }
      
    }
}