using DaggerfallConnect;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using MightyMagick.Formulas;

namespace Game.Mods.MightMagick.SpellProgressionModule
{
    public static class EntityEffectManagerPatches
    {
        public static bool Prefix_TryAbsorbtion(EntityEffectManager __instance, ref bool __result, IEntityEffect effect, TargetTypes targetType,
            DaggerfallEntity casterEntity, out int absorbSpellPointsOut)
        {
            var targetEntity = __instance.EntityBehaviour.Entity;
            SpellAbsorption absorbEffect = __instance.FindIncumbentEffect<SpellAbsorption>() as SpellAbsorption;
            var resultSpellAbsorb =
                SpellAbsorb.TryAbsorption(effect, targetType, casterEntity, targetEntity, absorbEffect);

            if (resultSpellAbsorb > 0)
            {
                absorbSpellPointsOut = resultSpellAbsorb;
                __result = true;
            }
            else
            {
                absorbSpellPointsOut = 0;
                __result = false;
            }

            return false;
        }

        public static bool Prefix_SetReadySpell(EntityEffectBundle spell, bool noSpellPointCost)
        {
            if (SpellCostSkillChecker.CanSpellBeCast(spell, noSpellPointCost))
                return true;

            DaggerfallUI.AddHUDText($"Not skilled enough to cast this yet.");
            return false;
        }

        public  static bool Prefix_TallyPlayerReadySpellEffectSkills(EntityEffectBundle ___readySpell)
        {
            // Validate ready spell
            if (___readySpell == null || ___readySpell.Settings.Effects == null)
                return false;

            // Loop through effects in spell bundle and tally related magic skill
            // Normally spells will have no more than 3 effects
            for (var i = 0; i < ___readySpell.Settings.Effects.Length; i++)
            {
                var effectEntry = ___readySpell.Settings.Effects[i];
                var effect = GameManager.Instance.EntityEffectBroker.GetEffectTemplate(___readySpell.Settings.Effects[i].Key);
                if (effect == null)
                    continue;

                var (_, cost) = FormulaHelper.CalculateEffectCosts(effectEntry);
                GameManager.Instance.PlayerEntity.TallySkill((DFCareer.Skills)effect.Properties.MagicSkill, (short)cost);
            }

            return false;
        }
    }
}