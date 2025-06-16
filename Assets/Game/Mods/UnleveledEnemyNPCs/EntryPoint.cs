using System.Reflection;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;
using HarmonyLib;
using UnityEngine;

namespace Game.Mods.UnleveledEnemyNPCs
{
    public class UnleveledEnemyNPCsMod : MonoBehaviour
    {
        private static Mod mod;
        public static UnleveledEnemyNPCsMod Instance;
        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }
        public int CommonLevel { get; set; }

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;

            var go = new GameObject(mod.Title);
            go.AddComponent<UnleveledEnemyNPCsMod>();
        }

        void Awake()
        {
            Instance = this;
            ParseSettings();
            InitMod();
            mod.IsReady = true;
        }

        void ParseSettings()
        {
            ModSettings settings = mod.GetSettings();
            MinLevel = settings.GetValue<int>("MainSection", "MinimumLevel");
            MaxLevel = settings.GetValue<int>("MainSection", "MaximumLevel");
            CommonLevel = settings.GetValue<int>("MainSection", "CommonLevel");
            Debug.Log($"{nameof(UnleveledEnemyNPCsMod)} - {nameof(ParseSettings)} - {nameof(MinLevel)} {MinLevel} - {nameof(MaxLevel)} {MaxLevel} - {nameof(CommonLevel)} {CommonLevel}");
        }

        public void InitMod()
        {
            Debug.Log("Begin mod init: UnleveledEnemyNPCsMod");
            var harmony = new Harmony("UnleveledEnemyNPCs");

            // Target method to patch
            MethodInfo targetMethod = typeof(EnemyEntity)
                .GetMethod(nameof(EnemyEntity.SetEnemyCareer), BindingFlags.Instance | BindingFlags.Public);

            // Prefix method
            MethodInfo prefixMethod = typeof(EnemyEntityPatches)
                .GetMethod(nameof(EnemyEntityPatches.Prefix_SetEnemyCareer), BindingFlags.Static | BindingFlags.Public);
            harmony.Patch(
                original: targetMethod,
                prefix: new HarmonyMethod(prefixMethod)
            );
            Debug.Log("Finished mod init: UnleveledEnemyNPCsMod");
        }
    }
}
