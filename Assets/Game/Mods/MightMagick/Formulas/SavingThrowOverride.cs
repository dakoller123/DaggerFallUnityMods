using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Formulas;
using UnityEngine;

namespace MightyMagick.Formulas
{
    public static class SavingThrowOverride
    {
        public static int SavingThrow(IEntityEffect sourceEffect, DaggerfallEntity target)
        {

            if (sourceEffect?.ParentBundle == null)
                return 100;

            var effectFlags = FormulaHelper.GetEffectFlags(sourceEffect);
            var elementType = FormulaHelper.GetElementType(sourceEffect);
            var modifier = FormulaHelper.GetResistanceModifier(effectFlags, target);

            if (!sourceEffect.Properties.SupportMagnitude)
                return FormulaHelper.SavingThrow(elementType, effectFlags, target, modifier);

            var resistance = new ResistanceAggregator(elementType, effectFlags, target, modifier*2).AggregateResistances();

            var resistancePercent = 100 - resistance;
            var willpowerPercent = CalculateWithWillPower(sourceEffect, target);

            return resistancePercent * willpowerPercent / 100;
        }

        private static int CalculateWithWillPower(IEntityEffect sourceEffect, DaggerfallEntity target)
        {
            var sourceCaster = sourceEffect.Caster.Entity;
            var casterWillPower = sourceCaster.Stats.LiveWillpower;
            var targetWillpower = target.Stats.LiveWillpower;

            // Normalize around 50: so 50 = 0, 100 = +50, 0 = -50
            var casterBonus = casterWillPower - 50;
            var targetPenalty = targetWillpower - 50;

            // Net effect on modifier: caster bonus - target resistance
            var netEffect = casterBonus - targetPenalty;

            // Apply the net effect to 100%
            var finalPercent = 100 + netEffect;

            return finalPercent;
        }
    }
}