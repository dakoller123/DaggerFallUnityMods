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
    public static class FormulaOverrides
    {
        public static void RegisterFormulaOverrides(Mod mod)
        {
            var settings = MightyMagickMod.Instance.MightyMagickModSettings;

            if (settings.RegenSettings.Enabled)
            {
                FormulaHelper.RegisterOverride(mod, "CalculateSpellPointRecoveryRate", (Func< PlayerEntity, int>)SpellPointRecoveryRate.CalculateSpellPointRecoveryRate);
            }

            if (settings.SpellCostSettings.Enabled)
            {
                FormulaHelper.RegisterOverride(mod, "CalculateEffectCosts", (Func<IEntityEffect, EffectSettings, DaggerfallEntity, FormulaHelper.SpellCost>)MagickaCost.CalculateEffectCosts);
            }
            
            if (settings.MagickaPoolSettings.Enabled)
            {
                FormulaHelper.RegisterOverride(mod, "SpellPoints", (Func<int, float, int>)MagickaPoolSize.SpellPoints);
            }

            if (settings.SavingThrowSettings.Enabled)
            {
                FormulaHelper.RegisterOverride(mod, "SavingThrowSpellEffect", (Func<IEntityEffect, DaggerfallEntity, int>)SavingThrowOverride.SavingThrow);
            }           
            
            if (settings.AbsorbSettings.Enabled)
            {
                FormulaHelper.RegisterOverride(mod, "TryAbsorption", (Func<IEntityEffect , TargetTypes, DaggerfallEntity, DaggerfallEntity, SpellAbsorption, int>)SpellAbsorb.TryAbsorption);
            }
        }
    }
}