using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
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
    }
}