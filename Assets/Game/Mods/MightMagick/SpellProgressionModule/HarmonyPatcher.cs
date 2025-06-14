using System;
using System.IO;
using System.Reflection;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using Game.Mods.MightMagick.SpellProgressionModule;
using UnityEngine;

namespace  MightyMagick.SpellProgressionModule
{
    public static class HarmonyPatcher
    {
        private const string HarmonyAssemblyPath = "Mods/0Harmony.dll";
        private const string HarmonyPatchId = "mightymagickmod.entityeffectmanager.patch";

        private static object harmonyInstance;
        private static MethodInfo patchMethod;
        private static ConstructorInfo harmonyMethodCtor;
        private static Type harmonyMethodType;

        public static bool TryApplyPatch()
        {
            var spellProgSettings = MightyMagickMod.Instance.MightyMagickModSettings.SpellProgressionSettings;
            var absorbSettings = MightyMagickMod.Instance.MightyMagickModSettings.AbsorbSettings;

            if (!spellProgSettings.LimitSpellCastBySkill && !absorbSettings.Enabled && !spellProgSettings.LimitSpellBuyBySkill)
            {
                Debug.Log("MightyMagick - TryApplyPatch - Harmony is not needed");
                return true;
            }

            string harmonyPath = Path.Combine(Application.streamingAssetsPath, HarmonyAssemblyPath);

            if (!File.Exists(harmonyPath))
            {
                Debug.LogError($"Harmony: {harmonyPath} not found");
                return false;
            }

            try
            {
                Setup(harmonyPath);

                if (absorbSettings.Enabled) PatchTryAbsorb();
                if (spellProgSettings.LimitSpellCastBySkill) PatchSetReadySpell();
                if (spellProgSettings.LimitSpellBuyBySkill) PatchSpellBuy();

                Debug.Log("Harmony: Applied patches successfully.");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Harmony: Error while patching => {e}");
                return false;
            }
        }

        private static void Setup(string harmonyPath)
        {
            Assembly harmonyAssembly = GetHarmonyAssembly(harmonyPath);
            Type harmonyType = harmonyAssembly.GetType("HarmonyLib.Harmony");
            harmonyMethodType = harmonyAssembly.GetType("HarmonyLib.HarmonyMethod");

            harmonyInstance = Activator.CreateInstance(harmonyType, new object[] { HarmonyPatchId });

            patchMethod = harmonyType.GetMethod("Patch");
            harmonyMethodCtor = harmonyMethodType.GetConstructor(new Type[] { typeof(MethodInfo) });
        }

        private static Assembly GetHarmonyAssembly(string harmonyPath)
        {
            byte[] dllData = File.ReadAllBytes(harmonyPath);
            return Assembly.Load(dllData);
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

            object harmonyPrefix = harmonyMethodCtor.Invoke(new object[] { prefixMethod });

            patchMethod.Invoke(harmonyInstance, new object[]
            {
                targetMethod,
                harmonyPrefix,
                null, null, null
            });

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

            object harmonyPrefix = harmonyMethodCtor.Invoke(new object[] { prefixMethod });

            patchMethod.Invoke(harmonyInstance, new object[]
            {
                targetMethod,
                harmonyPrefix,
                null, null, null
            });

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

            object harmonyPrefix = harmonyMethodCtor.Invoke(new object[] { prefixMethod });

            patchMethod.Invoke(harmonyInstance, new object[]
            {
                targetMethod,
                harmonyPrefix,
                null, null, null
            });

            Debug.Log("Harmony: BuyButton_OnMouseClick() patched successfully.");
        }
    }
}
