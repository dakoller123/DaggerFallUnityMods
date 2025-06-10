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
    public static class SpellAbsorb
    {
        public static int TryAbsorption(IEntityEffect effect, TargetTypes targetType, DaggerfallEntity casterEntity, DaggerfallEntity targetEntity, SpellAbsorption absorbEffectOnTarget)
        {
            var absorbSettings = MightyMagickMod.Instance.MightyMagickModSettings.AbsorbSettings;

            var absorbSpellPointsOut = 0;

            // Effect cannot be null
            if (effect == null)
                return 0;

            if (!absorbSettings.AllowNonDestructionAbsorbs && effect.Properties.MagicSkill != DFCareer.MagicSkills.Destruction)
                return 0;

            if (!absorbSettings.AbsorbOwnSpells && casterEntity == targetEntity)
                return 0;    

            // Get casting cost for this effect
            // Costs are calculated as if target cast the spell, not the actual caster
            // Note that if player self-absorbs a spell this will be equal anyway
            var entityForCastCost = absorbSettings.CalculateSpellCostWithCaster
                ? casterEntity
                : targetEntity;

            int effectCastingCost = GetEffectCastingCost(effect, targetType, entityForCastCost);


            // The target entity must have enough spell points free to absorb incoming effect
            int availableSpellPoints = targetEntity.MaxMagicka - targetEntity.CurrentMagicka;
            if (effectCastingCost > availableSpellPoints)
                return 0;
            else
                absorbSpellPointsOut = effectCastingCost;

            var chance = GetAbsorbChance(effect, targetEntity, absorbEffectOnTarget);

            if (absorbSettings.CalculateWithResistances)
            {
                int resistances = new ResistanceAggregator(FormulaHelper.GetElementType(effect), FormulaHelper.GetEffectFlags(effect), targetEntity, 0).AggregatedResistances;
                
                chance = ApplyResistanceToAbsorbChance(chance, resistance);
            }
           
            if (chance >= 100 ||  DaggerfallWorkshop.Game.Utility.Dice100.SuccessRoll(chance))
            {
                return absorbSpellPointsOut;
            }
            return 0;
        }

        static int ApplyResistanceToAbsorbChance(int baseChance, int resistance)
        {
            baseChance = Math.Clamp(baseChance, 0, 100);

            float multiplier;
            if (resistance < 0)
            {
                // Map -100 → 0, -50 → 0.5, 0 → 1
                multiplier = 1.0f + (resistance / 100.0f);  // Linear: -100 to 0 becomes 0 to 1
            }
            else
            {
                // Map 0 → 1, 100 → 1.5
                multiplier = 1.0f + (resistance / 200.0f);  // Same as before
            }

            float modifiedChance = baseChance * multiplier;

            return (int)Math.Clamp(modifiedChance, 0, 100);
        }


        static int GetAbsorbChance(IEntityEffect effect, DaggerfallEntity targetEntity, SpellAbsorption absorbEffectOnTarget)
        {
            int chance = (CheckCareerBasedAbsorption(effect, targetEntity) || targetEntity.IsAbsorbingSpells) 
                ? 100 
                : GetEffectBasedAbsorptionChance(effect, absorbEffectOnTarget, targetEntity);
            
            return chance;
           
        }

        static int GetEffectCastingCost(IEntityEffect effect, TargetTypes targetType, DaggerfallEntity casterEntity)
        {            
            (int _, int spellPointCost) = FormulaHelper.CalculateEffectCosts(effect, effect.Settings, casterEntity);
            spellPointCost = FormulaHelper.ApplyTargetCostMultiplier(spellPointCost, targetType);

            // Spells always cost at least 5 spell points
            // Otherwise it's possible for absorbs to make spell point pool go down as spell costs 5 but caster absorbs 0
            if (spellPointCost < 5)
                spellPointCost = 5;

            //Debug.LogFormat("Calculated {0} spell point cost for effect {1}", spellPointCost, effect.Key);

            return spellPointCost;
        }

        static int GetEffectBasedAbsorptionChance(IEntityEffect effect, SpellAbsorption absorbEffect, DaggerfallEntity entity)
        {
            if (absorbEffect == null)
                return 0;

            int chance = absorbEffect.Settings.ChanceBase + absorbEffect.Settings.ChancePlus * (int)Mathf.Floor(entity.Level / absorbEffect.Settings.ChancePerLevel);

            return chance;
        }

        static bool CheckCareerBasedAbsorption(IEntityEffect effect, DaggerfallEntity entity)
        {
            // Always resists or none
            DFCareer.SpellAbsorptionFlags spellAbsorption = entity.Career.SpellAbsorption;

            if (spellAbsorption == DFCareer.SpellAbsorptionFlags.None)
                return false;

            if (spellAbsorption == DFCareer.SpellAbsorptionFlags.Always)
                return true;

            // Resist in darkness (inside building or dungeon or outside at night)
            // Use player for inside/outside context - everything is where the player is
            if (spellAbsorption == DFCareer.SpellAbsorptionFlags.InDarkness)
            {
                if (GameManager.Instance.PlayerEnterExit.IsPlayerInside)
                    return true;
                else if (DaggerfallUnity.Instance.WorldTime.Now.IsNight)
                    return true;
            }

            // Resist in light (outside during the day)
            if (spellAbsorption == DFCareer.SpellAbsorptionFlags.InLight)
            {
                if (!GameManager.Instance.PlayerEnterExit.IsPlayerInside && DaggerfallUnity.Instance.WorldTime.Now.IsDay)
                    return true;
            }

            return false;
        }
    }
}