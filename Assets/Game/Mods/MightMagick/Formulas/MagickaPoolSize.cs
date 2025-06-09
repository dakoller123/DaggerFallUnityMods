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
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallConnect;

namespace MightyMagick.Formulas
{
    public static class MagickaPoolSize
    {
        static float CalculatePercentage(float percentageIncrease, int playerLevel)
        {
            return (float)(Math.Pow((double)percentageIncrease, (double)playerLevel));
        }

        static float CalculateRaw(int intelligence, float multiplier)
        {
            return multiplier * intelligence;
        }

        static int CalculateFlat(int flatIncrease, int playerLevel)
        {
            return flatIncrease * playerLevel;
        }

        public static int SpellPoints(int intelligence, float multiplier)
        {
            var settings = MightyMagickMod.Instance.MightyMagickModSettings.MagickaPoolSettings;            
            var level = GameManager.Instance.PlayerEntity.Level;
            var flat = settings.LevelUpFlatIncrease;
            var percent = 1.0f + (((float)settings.LevelUpPercentageIncrease) / 100);
            
            var baseValue = CalculateRaw(intelligence, multiplier) + CalculateFlat(flat, level);
            var result = baseValue * CalculatePercentage(percent, level);

            return (int)Mathf.Floor(result);
        }

    }
}