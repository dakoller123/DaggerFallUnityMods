// Project:   Clairvoyance for Daggerfall Unity
// Author:    kiskoller
// Based on code from:    DunnyOfPenwick

using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;

namespace MightyMagick.Formulas
{

    public static class SpellPointRecoveryRate
    {
        static int CalculateRecovery(PlayerEntity player, int ratio)
        {
            if (ratio == 0)
            {
                return 0;
            }
            return Mathf.Max((int)Mathf.Floor(player.MaxMagicka / ratio), 1);
        }


        // Calculate how many spell points the player should recover per hour of rest
        public static int CalculateSpellPointRecoveryRate(PlayerEntity player)
        {
            var regenRate = CalculateRegenRate(player);

            return CalculateRecovery(player, regenRate);
        }

        public static int CalculateRegenRate(PlayerEntity player)
        {
            var regenSettings = MightyMagickMod.Instance.MightyMagickModSettings.RegenSettings;

            if (player.Career.NoRegenSpellPoints)
            {
                return 0; 
            }

            if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon)
            {
                return regenSettings.RegenRateDungeon;
            }

            if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideTavern)
            {
                return regenSettings.RegenRateTavern;
            }

            return regenSettings.RegenRateOutdoor;
        }
    }
}