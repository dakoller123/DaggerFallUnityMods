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
    public static class MagickaCostFormula
    {

        // Just makes formulas more readable
        static int trunc(double value) { return (int)Math.Truncate(value); }

        static int GetEffectComponentCosts(
            EffectCosts costs,
            int starting,
            int increase,
            int perLevel,
            int skillValue)
        {
            //Calculate effect gold cost, spellpoint cost is calculated from gold cost after adding up for duration, chance and magnitude
            return trunc(costs.OffsetGold + costs.CostA * starting + costs.CostB * trunc(increase / perLevel));
        }


        /// <summary>
        /// Calculates effect costs from an IEntityEffect and custom settings.
        /// </summary>
        public static FormulaHelper.SpellCost CalculateEffectCosts(IEntityEffect effect, EffectSettings settings, DaggerfallEntity casterEntity = null)
        {
            bool activeComponents = false;

            // Get related skill
            int skillValue = 0;
            if (casterEntity == null)
            {
                // From player
                skillValue = GameManager.Instance.PlayerEntity.Skills.GetLiveSkillValue((DFCareer.Skills)effect.Properties.MagicSkill);
            }
            else
            {
                // From another entity
                skillValue = casterEntity.Skills.GetLiveSkillValue((DFCareer.Skills)effect.Properties.MagicSkill);
            }

            // Duration costs
            int durationGoldCost = 0;
            if (effect.Properties.SupportDuration)
            {
                activeComponents = true;
                durationGoldCost = GetEffectComponentCosts(
                    effect.Properties.DurationCosts,
                    settings.DurationBase,
                    settings.DurationPlus,
                    settings.DurationPerLevel,
                    skillValue);

                //Debug.LogFormat("Duration: gold {0} spellpoints {1}", durationGoldCost, durationSpellPointCost);
            }

            // Chance costs
            int chanceGoldCost = 0;
            if (effect.Properties.SupportChance)
            {
                activeComponents = true;
                chanceGoldCost = GetEffectComponentCosts(
                    effect.Properties.ChanceCosts,
                    settings.ChanceBase,
                    settings.ChancePlus,
                    settings.ChancePerLevel,
                    skillValue);

                //Debug.LogFormat("Chance: gold {0} spellpoints {1}", chanceGoldCost, chanceSpellPointCost);
            }

            // Magnitude costs
            int magnitudeGoldCost = 0;
            if (effect.Properties.SupportMagnitude)
            {
                activeComponents = true;
                int magnitudeBase = (settings.MagnitudeBaseMax + settings.MagnitudeBaseMin) / 2;
                int magnitudePlus = (settings.MagnitudePlusMax + settings.MagnitudePlusMin) / 2;
                magnitudeGoldCost = GetEffectComponentCosts(
                    effect.Properties.MagnitudeCosts,
                    magnitudeBase,
                    magnitudePlus,
                    settings.MagnitudePerLevel,
                    skillValue);

                //Debug.LogFormat("Magnitude: gold {0} spellpoints {1}", magnitudeGoldCost, magnitudeSpellPointCost);
            }

            // If there are no active components (e.g. Teleport) then fudge some costs
            // This gives the same casting cost outcome as classic and supplies a reasonable gold cost
            // Note: Classic does not assign a gold cost when a zero-component effect is the only effect present, which seems like a bug
            int fudgeGoldCost = 0;
            if (!activeComponents)
                fudgeGoldCost = GetEffectComponentCosts(BaseEntityEffect.MakeEffectCosts(60, 100, 160), 1, 1, 1, skillValue);

            // Add gold costs together and calculate spellpoint cost from the result
            FormulaHelper.SpellCost effectCost;
            effectCost.goldCost = durationGoldCost + chanceGoldCost + magnitudeGoldCost + fudgeGoldCost;

            if (skillValue <= 100)
            {
                effectCost.spellPointCost = effectCost.goldCost * (110 - skillValue) / 400;
            }
            else
            {
                double spellPointCost = (effectCost.goldCost * (10) / 400) * (Math.Pow((99.0f / 100.0f),(skillValue - 100)));
                effectCost.spellPointCost = trunc(spellPointCost);
            }


            //Debug.LogFormat("Costs: gold {0} spellpoints {1}", finalGoldCost, finalSpellPointCost);
            return effectCost;
        }


    }
}