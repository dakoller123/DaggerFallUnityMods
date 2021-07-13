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
            FormulaHelper.RegisterOverride(mod, "CalculateEffectCosts", (Func<IEntityEffect, EffectSettings, DaggerfallEntity, FormulaHelper.SpellCost>)MagickaCostFormula.CalculateEffectCosts);
        }
    }
}