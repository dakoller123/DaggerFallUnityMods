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
        //Todo: somehow put this into mod setting. 
        const float bonusMagickaPerLevel = 1.10f;

        static float CalculateBonusSpellPointForAllLevelups(double bonusForSingleLevelup, int playerLevel)
        {
            return (float)(Math.Pow(bonusForSingleLevelup, (double)playerLevel));
        }

        public static int SpellPoints(int intelligence, float multiplier)
        {
            float baseMagickaWithoutLevelupBonus = (float)intelligence * multiplier;
            var playerEntity = GameManager.Instance.PlayerEntity;
            float result = baseMagickaWithoutLevelupBonus * CalculateBonusSpellPointForAllLevelups(bonusMagickaPerLevel, playerEntity.Level);

            return (int)Mathf.Floor(result);
        }

    }
}