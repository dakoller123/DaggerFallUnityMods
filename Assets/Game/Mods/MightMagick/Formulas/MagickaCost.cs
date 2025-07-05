// Project:   Clairvoyance for Daggerfall Unity
// Author:    kiskoller
// Based on code from:    DunnyOfPenwick

using System;
using System.Linq;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Items;
using UnityEngine;

namespace MightyMagick.Formulas
{

    public static class MagickaCost
    {
        private const int ShieldPenalty = 10;
        private const int LeatherPenalty = 5;
        private const int ChainPenalty = 10;
        private const int PlatePenalty = 20;
        private const int WeaponPenalty = 30;
        private const int StaffPenalty = 0;

        private static bool IsShield(DaggerfallUnityItem item)
        {
            return (item.TemplateIndex == (int)Armor.Kite_Shield ||
                    item.TemplateIndex == (int)Armor.Round_Shield ||
                    item.TemplateIndex == (int)Armor.Tower_Shield ||
                    item.TemplateIndex == (int)Armor.Buckler);
        }

        private static int GetItemPenalty(DaggerfallUnityItem item)
        {
            if (item == null) return 0;
            var spellCostSettings = MightyMagickMod.Instance.MightyMagickModSettings.SpellCostSettings;

            if (IsShield(item) && spellCostSettings.ArmorPenalty) return ShieldPenalty;

            if (item.ItemGroup == ItemGroups.Armor && spellCostSettings.ArmorPenalty)
            {
                switch (item.NativeMaterialValue)
                {
                    case (int)ArmorMaterialTypes.Leather:
                        return LeatherPenalty;
                    case (int)ArmorMaterialTypes.Chain:
                        return ChainPenalty;
                    default:
                        return PlatePenalty;
                }
            }

            if (item.ItemGroup == ItemGroups.Weapons && spellCostSettings.WeaponPenalty)
                return item.TemplateIndex == (int)Weapons.Staff ? StaffPenalty : WeaponPenalty;

            return 0;
        }

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
            var increaseRatio = perLevel == 0 ? 1 : trunc(increase / perLevel);
            return trunc(costs.OffsetGold + costs.CostA * starting + costs.CostB * increaseRatio);
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

            var spellCostSettings = MightyMagickMod.Instance.MightyMagickModSettings.SpellCostSettings;

            //caster entity is null == it's the player.
            if ((spellCostSettings.ArmorPenalty || spellCostSettings.WeaponPenalty) && casterEntity == null)
            {
                var armorPenalty =  GameManager.Instance.PlayerEntity.ItemEquipTable.EquipTable.Sum(GetItemPenalty);
                effectCost.spellPointCost = Mathf.RoundToInt(effectCost.spellPointCost * (1.0f + armorPenalty / 100.0f));
            }

            var modsettingsMultiplier = MightyMagickMod.Instance.MightyMagickModSettings.SpellCostSettings.Multiplier;
            effectCost.spellPointCost = Mathf.RoundToInt(effectCost.spellPointCost * modsettingsMultiplier);

            return effectCost;
        }
    }
}