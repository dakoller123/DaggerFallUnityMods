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
    public static class SavingThrowOverride
    {
        public static int SavingThrow(IEntityEffect sourceEffect, DaggerfallEntity target)
        {
            if (sourceEffect == null || sourceEffect.ParentBundle == null)
                return 100;
            
            DFCareer.EffectFlags effectFlags = FormulaHelper.GetEffectFlags(sourceEffect);
            DFCareer.Elements elementType = FormulaHelper.GetElementType(sourceEffect);
            int modifier = FormulaHelper.GetResistanceModifier(effectFlags, target);
            if (sourceEffect.Properties.SupportMagnitude)
            {                
                int resistances = new ResistanceAggregator(elementType, effectFlags, target, modifier*2).AggregateResistances();
                return 100-resistances;
            }
            else
            {                
                return FormulaHelper.SavingThrow(elementType, effectFlags, target, modifier);
            }
        }
    }
}