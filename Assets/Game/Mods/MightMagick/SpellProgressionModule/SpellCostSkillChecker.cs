using DaggerfallConnect;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.MagicAndEffects;
using MightyMagick;
using UnityEngine;

namespace Game.Mods.MightMagick.SpellProgressionModule
{
    public static class SpellCostSkillChecker
    {
        public static bool CanSpellBeCast(EffectBundleSettings bundleSettings, DaggerfallEntity casterEntity)
        {
            if (GameManager.Instance.PlayerEntity == null || casterEntity != GameManager.Instance.PlayerEntity) return true;

            var spellCostMultiplier = MightyMagickMod.Instance.MightyMagickModSettings.SpellProgressionSettings
                .SpellCostCheckMultiplier;

            foreach (var effect in bundleSettings.Effects)
            {
                var spellCostRecord = FormulaHelper.CalculateEffectCosts(effect, casterEntity);
                var spellPointCost = spellCostRecord.spellPointCost;
                var effectTemplate = GameManager.Instance.EntityEffectBroker.GetEffectTemplate(effect.Key);
                var skillValue = GameManager.Instance.PlayerEntity.Skills.GetLiveSkillValue((DFCareer.Skills)effectTemplate.Properties.MagicSkill);
                if (skillValue >= 100) continue;
                var checkedSpellPointCost = (int)Mathf.Round(spellPointCost * spellCostMultiplier);
                if (skillValue >= checkedSpellPointCost) continue;
                return false;
            }

            return true;
        }

        public static bool CanSpellBeCast(EntityEffectBundle spell, bool noSpellPointCost)
        {
            if (noSpellPointCost) return true;

            var casterEntity = spell.CasterEntityBehaviour.Entity;
            return CanSpellBeCast(spell.Settings, casterEntity);
        }
    }
}