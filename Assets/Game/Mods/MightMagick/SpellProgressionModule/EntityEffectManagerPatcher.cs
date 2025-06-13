using System;
using System.IO;
using System.Reflection;
using DaggerfallConnect;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.MagicAndEffects;
using UnityEngine;

namespace  MightyMagick.SpellProgressionModule
{
    public static class EntityEffectManagerPatcher
    {
        private const string HarmonyAssemblyPath = "Mods/0Harmony.dll";
        private const string HarmonyPatchId = "mightymagickmod.entityeffectmanager.patch";

        private static object harmonyInstance;
        private static MethodInfo patchMethod;
        private static ConstructorInfo harmonyMethodCtor;
        private static Type harmonyMethodType;
        private static float spellCostMultiplier;

        public static bool Prefix_SetReadySpell(EntityEffectBundle spell, bool noSpellPointCost)
        {
            if (GameManager.Instance.PlayerEntity == null) return true;
            if (spell.CasterEntityBehaviour?.Entity != GameManager.Instance.PlayerEntity) return true;
            var casterEntity = GameManager.Instance.PlayerEntity;

            foreach (var effect in spell.Settings.Effects)
            {
                var (_, spellPointCost) = FormulaHelper.CalculateEffectCosts(effect, casterEntity);
                var effectTemplate = GameManager.Instance.EntityEffectBroker.GetEffectTemplate(effect.Key);
                var skillValue = GameManager.Instance.PlayerEntity.Skills.GetLiveSkillValue((DFCareer.Skills)effectTemplate.Properties.MagicSkill);
                if (skillValue >= 100) continue;
                var checkedSpellPointCost = (int)Mathf.Round(spellPointCost * spellCostMultiplier);
                if (skillValue >= checkedSpellPointCost) continue;
                DaggerfallUI.AddHUDText($"Not skilled enough to cast this yet.");
                return false;
            }

            return true;
        }

        public static bool TryApplyPatch()
        {
            spellCostMultiplier = MightyMagickMod.Instance.MightyMagickModSettings.SpellProgressionSettings
                .SpellCostCheckMultiplier;
            string harmonyPath = Path.Combine(Application.streamingAssetsPath, HarmonyAssemblyPath);

            if (!File.Exists(harmonyPath))
            {
                Debug.LogError($"Harmony: {harmonyPath} not found");
                return false;
            }

            try
            {
                Setup(harmonyPath);

                PatchSetReadySpell();

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

            MethodInfo prefixMethod = typeof(EntityEffectManagerPatcher)
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


    }
}
