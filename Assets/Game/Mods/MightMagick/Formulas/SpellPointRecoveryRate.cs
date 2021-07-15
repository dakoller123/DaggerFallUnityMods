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

    public static class SpellPointRecoveryRate
    {
        static int CalculateRecovery(PlayerEntity player, int ratio)
        {
            return Mathf.Max((int)Mathf.Floor(player.MaxMagicka / ratio), 1);
        }


        // Calculate how many spell points the player should recover per hour of rest
        public static int CalculateSpellPointRecoveryRate(PlayerEntity player)
        {
            if (player.Career.NoRegenSpellPoints)
                return 0;

            if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon)
                return CalculateRecovery(player, 100);

            if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideTavern)
                return CalculateRecovery(player, 8);

            return CalculateRecovery(player, 24);
        }
    }
}