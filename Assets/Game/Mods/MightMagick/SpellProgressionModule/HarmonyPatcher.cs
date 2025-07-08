using System;
using System.Reflection;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using Game.Mods.MightMagick.SpellProgressionModule;
using HarmonyLib;
using UnityEngine;

namespace  MightyMagick.SpellProgressionModule
{

    public static class HarmonyPatcher
    {
        public static Harmony harmony;
        public static bool TryApplyPatch()
        {
            var spellProgSettings = MightyMagickMod.Instance.MightyMagickModSettings.SpellProgressionSettings;
            var absorbSettings = MightyMagickMod.Instance.MightyMagickModSettings.AbsorbSettings;

            if (!spellProgSettings.LimitSpellCastBySkill && !absorbSettings.Enabled && !spellProgSettings.LimitSpellBuyBySkill)
            {
                Debug.Log("MightyMagick - TryApplyPatch - Harmony is not needed");
                return true;
            }
            harmony = new Harmony("MightyMagickMod");

            try
            {
                if (absorbSettings.Enabled) PatchTryAbsorb();
                if (spellProgSettings.LimitSpellCastBySkill) PatchSetReadySpell();
                if (spellProgSettings.LimitSpellBuyBySkill) PatchSpellBuy();
                if (MightyMagickMod.Instance.MightyMagickModSettings.MiscSettings.DisablePressButtonSpam) PatchUIMessage();
                if (MightyMagickMod.Instance.MightyMagickModSettings.SkillProgressionSettings.Enabled)
                {
                    PatchSkillProgress();
                    PatchSkillTally();
                }

                if (MightyMagickMod.Instance.MightyMagickModSettings.SpellCostSettings.UseCostMultiplier)
                    PatchSpellEffectCost();

                if (MightyMagickMod.Instance.MightyMagickModSettings.SpellCostSettings.EquipmentPenalty)
                    PatchTotalEffectCost();

                Debug.Log("Harmony: Applied patches successfully.");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Harmony: Error while patching => {e}");
                return false;
            }
        }

        private static void PatchTotalEffectCost()
        {
            var targetMethod = typeof(FormulaHelper)
                .GetMethod(nameof(FormulaHelper.CalculateTotalEffectCosts), BindingFlags.Public | BindingFlags.Static);

            var prefixMethod = typeof(FormulaPatches)
                .GetMethod(
                    nameof(FormulaPatches.PostFix_CalculateTotalEffectCosts),
                    BindingFlags.Public | BindingFlags.Static
                );

            harmony.Patch(
                original: targetMethod,
                postfix: new HarmonyMethod(prefixMethod));

            Debug.Log("Harmony: PatchTotalEffectCost() patched successfully.");
        }

        private static void PatchSpellEffectCost()
        {
            var targetMethod = typeof(FormulaHelper)
                .GetMethod(nameof(FormulaHelper.CalculateEffectCosts), BindingFlags.Public | BindingFlags.Static,
                    null,
                    new Type[] { typeof(IEntityEffect), typeof(EffectSettings), typeof(DaggerfallEntity) },
                    null);
            var prefixMethod = typeof(FormulaPatches)
                .GetMethod(
                    nameof(FormulaPatches.PostFix_CalculateEffectCosts),
                    BindingFlags.Public | BindingFlags.Static
                );

            harmony.Patch(
                original: targetMethod,
                postfix: new HarmonyMethod(prefixMethod));

            Debug.Log("Harmony: PatchSpellEffectCost() patched successfully.");
        }

        private static void PatchSkillTally()
        {
            var targetMethod = typeof(EntityEffectManager)
                .GetMethod("TallyPlayerReadySpellEffectSkills", BindingFlags.NonPublic | BindingFlags.Instance);

            var prefixMethod = typeof(EntityEffectManagerPatches)
                .GetMethod(
                    nameof(EntityEffectManagerPatches.Prefix_TallyPlayerReadySpellEffectSkills),
                    BindingFlags.Public | BindingFlags.Static
                );

            harmony.Patch(
                original: targetMethod,
                prefix: new HarmonyMethod(prefixMethod));

            Debug.Log("Harmony: TallyPlayerReadySpellEffectSkills() patched successfully.");
        }

        private static void PatchSkillProgress()
        {
            MethodInfo targetMethod = typeof(DaggerfallSkills)
                .GetMethod(nameof(DaggerfallSkills.GetAdvancementMultiplier), BindingFlags.Public | BindingFlags.Static);

            MethodInfo postFixMethod = typeof(SkillsPatches)
                .GetMethod(
                    nameof(SkillsPatches.Postfix_GetAdvancementMultiplier),
                    BindingFlags.Public | BindingFlags.Static
                );

            harmony.Patch(
                original: targetMethod,
                postfix: new HarmonyMethod(postFixMethod));

            Debug.Log("Harmony: GetAdvancementMultiplier() patched successfully.");
        }
        private static void PatchUIMessage()
        {
            MethodInfo targetMethod = typeof(DaggerfallWorkshop.Game.DaggerfallUI)
                .GetMethod("AddHUDText", BindingFlags.Public | BindingFlags.Static,
                    null,
                    new Type[] { typeof(string), typeof(float)},
                    null);

            MethodInfo prefixMethod = typeof(UIPatches)
                .GetMethod(
                    "Prefix_AddHUDText",
                    BindingFlags.Public | BindingFlags.Static
                );

            harmony.Patch(
                original: targetMethod,
                prefix: new HarmonyMethod(prefixMethod));

            Debug.Log("Harmony: AddHUDText() patched successfully.");
        }

        private static void PatchSetReadySpell()
        {
            MethodInfo targetMethod = typeof(DaggerfallWorkshop.Game.MagicAndEffects.EntityEffectManager)
                .GetMethod("SetReadySpell", BindingFlags.Public | BindingFlags.Instance,
                    null,
                    new Type[] { typeof(EntityEffectBundle), typeof(bool)},
                    null);

            MethodInfo prefixMethod = typeof(EntityEffectManagerPatches)
                .GetMethod(
                    "Prefix_SetReadySpell",
                    BindingFlags.Public | BindingFlags.Static
                    );

            harmony.Patch(
                original: targetMethod,
                prefix: new HarmonyMethod(prefixMethod));

            Debug.Log("Harmony: SetReadySpell() patched successfully.");
        }

        private static void PatchTryAbsorb()
        {
            MethodInfo targetMethod = typeof(DaggerfallWorkshop.Game.MagicAndEffects.EntityEffectManager)
                .GetMethod("TryAbsorption", BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new Type[] { typeof(IEntityEffect), typeof(TargetTypes), typeof(DaggerfallEntity),  typeof(int).MakeByRefType()},
                    null);

            MethodInfo prefixMethod = typeof(EntityEffectManagerPatches)
                .GetMethod(
                    "Prefix_TryAbsorbtion",
                    BindingFlags.Public | BindingFlags.Static
                );

            harmony.Patch(
                original: targetMethod,
                prefix: new HarmonyMethod(prefixMethod));

            Debug.Log("Harmony: TryAbsorption() patched successfully.");
        }

        private static void PatchSpellBuy()
        {
            MethodInfo targetMethod = typeof(DaggerfallSpellBookWindow).GetMethod(
                "BuyButton_OnMouseClick",
                BindingFlags.Instance | BindingFlags.NonPublic
            );

            MethodInfo prefixMethod = typeof(SpellBookPatches)
                .GetMethod(
                    "Prefix_BuyButton_OnMouseClick",
                    BindingFlags.Public | BindingFlags.Static
                );

            if (prefixMethod == null)
            {
                Debug.LogError("Harmony: Failed to find Prefix_BuyButton_OnMouseClick");
            }

            if (targetMethod == null)
            {
                Debug.LogError("Harmony: Failed to find BuyButton_OnMouseClick");
            }

            harmony.Patch(
                original: targetMethod,
                prefix: new HarmonyMethod(prefixMethod));

            Debug.Log("Harmony: BuyButton_OnMouseClick() patched successfully.");
        }
    }
}
