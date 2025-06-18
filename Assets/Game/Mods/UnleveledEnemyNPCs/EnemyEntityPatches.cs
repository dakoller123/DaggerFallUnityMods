using UnityEngine;
using DaggerfallConnect;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Items;
using HarmonyLib;

namespace Game.Mods.UnleveledEnemyNPCs
{
    public static class EnemyEntityPatches
    {
        static byte[] ImpSpells            = { 0x07, 0x0A, 0x1D, 0x2C };
        static byte[] GhostSpells          = { 0x22 };
        static byte[] OrcShamanSpells      = { 0x06, 0x07, 0x16, 0x19, 0x1F };
        static byte[] WraithSpells         = { 0x1C, 0x1F };
        static byte[] FrostDaedraSpells    = { 0x10, 0x14 };
        static byte[] FireDaedraSpells     = { 0x0E, 0x19 };
        static byte[] DaedrothSpells       = { 0x16, 0x17, 0x1F };
        static byte[] VampireSpells        = { 0x33 };
        static byte[] SeducerSpells        = { 0x34, 0x43 };
        static byte[] VampireAncientSpells = { 0x08, 0x32 };
        static byte[] DaedraLordSpells     = { 0x08, 0x0A, 0x0E, 0x3C, 0x43 };
        static byte[] LichSpells           = { 0x08, 0x0A, 0x0E, 0x22, 0x3C };
        static byte[] AncientLichSpells    = { 0x08, 0x0A, 0x0E, 0x1D, 0x1F, 0x22, 0x3C };
        static byte[][] EnemyClassSpells   = { FrostDaedraSpells, DaedrothSpells, OrcShamanSpells, VampireAncientSpells, DaedraLordSpells, LichSpells, AncientLichSpells };


        [HarmonyPatch(typeof(EnemyEntity), nameof(EnemyEntity.SetEnemyCareer))]
        public static bool Prefix_SetEnemyCareer(
            EnemyEntity __instance,
            ref DFCareer ___career, ref int ___careerIndex,
            ref DaggerfallStats ___stats, ref int ___level, ref int ___maxHealth,
            ref MobileEnemy ___mobileEnemy,
            ref EntityTypes ___entityType,
            ref string ___name,
            ref WeaponMaterialTypes ___minMetalToHit,
            ref MobileTeams ___team,
            ref DaggerfallSkills ___skills,
            ref ItemCollection ___items,
            MobileEnemy mobileEnemy, EntityTypes entityType)
        {
            int desiredLevel = BellCurveRandom.GetBellCurveRandom(UnleveledEnemyNPCsMod.Instance.MinLevel, UnleveledEnemyNPCsMod.Instance.MaxLevel, UnleveledEnemyNPCsMod.Instance.Offset);

            // Try custom career first
            ___career = EnemyEntity.GetCustomCareerTemplate(mobileEnemy.ID);

            if (___career != null)
            {
                // Custom enemy
                ___careerIndex = mobileEnemy.ID;
                ___stats.SetPermanentFromCareer(___career);

                if (entityType == EntityTypes.EnemyMonster)
                {
                    // Default like a monster
                    ___level = mobileEnemy.Level;
                    ___maxHealth = Random.Range(mobileEnemy.MinHealth, mobileEnemy.MaxHealth + 1);
                    for (int i = 0; i < __instance.ArmorValues.Length; i++)
                    {
                        __instance.ArmorValues[i] = (sbyte)(mobileEnemy.ArmorValue * 5);
                    }
                }
                else
                {
                    // Default like a class enemy
                    ___level = desiredLevel;
                    ___maxHealth = FormulaHelper.RollEnemyClassMaxHealth(___level, ___career.HitPointsPerLevel);
                }
            }
            else if (entityType == EntityTypes.EnemyMonster)
            {
                ___careerIndex = mobileEnemy.ID;
                ___career = EnemyEntity.GetMonsterCareerTemplate((MonsterCareers)___careerIndex);
                ___stats.SetPermanentFromCareer(___career);

                // Enemy monster has predefined level, health and armor values.
                // Armor values can be modified below by equipment.
                ___level = mobileEnemy.Level;
                ___maxHealth = UnityEngine.Random.Range(mobileEnemy.MinHealth, mobileEnemy.MaxHealth + 1);
                for (int i = 0; i < __instance.ArmorValues.Length; i++)
                {
                    __instance.ArmorValues[i] = (sbyte)(mobileEnemy.ArmorValue * 5);
                }
            }
            else if (entityType == EntityTypes.EnemyClass)
            {
                ___careerIndex = mobileEnemy.ID - 128;
                ___career = EnemyEntity.GetClassCareerTemplate((ClassCareers)___careerIndex);
                ___stats.SetPermanentFromCareer(___career);

                // Enemy class is levelled to player and uses similar health rules
                // City guards are 3 to 6 levels above the player

                ___level = desiredLevel;
                if (___careerIndex == (int)MobileTypes.Knight_CityWatch - 128)
                {
                    ___level += UnityEngine.Random.Range(3, 7);
                }

                ___maxHealth = FormulaHelper.RollEnemyClassMaxHealth(___level, ___career.HitPointsPerLevel);
            }
            else
            {
                ___career = new DFCareer();
                ___careerIndex = -1;
                return false;
            }

            ___mobileEnemy = mobileEnemy;
            ___entityType = entityType;
            ___name = ___career.Name;
            ___minMetalToHit = mobileEnemy.MinMetalToHit;
            ___team = mobileEnemy.Team;

            short skillsLevel = (short)((___level * 5) + 30);
            if (skillsLevel > 100)
            {
                skillsLevel = 100;
            }

            for (int i = 0; i <= DaggerfallSkills.Count; i++)
            {
                ___skills.SetPermanentSkillValue(i, skillsLevel);
            }

            // Generate loot table items
            DaggerfallLoot.GenerateItems(mobileEnemy.LootTableKey, ___items);

            // Enemy classes and some monsters use equipment
            if (___careerIndex == (int)MonsterCareers.Orc || ___careerIndex == (int)MonsterCareers.OrcShaman)
            {
                __instance.SetEnemyEquipment(0);
            }
            else if (___careerIndex == (int)MonsterCareers.Centaur || ___careerIndex == (int)MonsterCareers.OrcSergeant)
            {
                __instance.SetEnemyEquipment(1);
            }
            else if (___careerIndex == (int)MonsterCareers.OrcWarlord)
            {
                __instance.SetEnemyEquipment(2);
            }
            else if (entityType == EntityTypes.EnemyClass)
            {
                __instance.SetEnemyEquipment(UnityEngine.Random.Range(0, 2)); // 0 or 1
            }

            // Assign spell lists
            if (entityType == EntityTypes.EnemyMonster)
            {
                if (___careerIndex == (int)MonsterCareers.Imp)
                    __instance.SetEnemySpells(ImpSpells);
                else if (___careerIndex == (int)MonsterCareers.Ghost)
                    __instance.SetEnemySpells(GhostSpells);
                else if (___careerIndex == (int)MonsterCareers.OrcShaman)
                    __instance.SetEnemySpells(OrcShamanSpells);
                else if (___careerIndex == (int)MonsterCareers.Wraith)
                    __instance.SetEnemySpells(WraithSpells);
                else if (___careerIndex == (int)MonsterCareers.FrostDaedra)
                    __instance.SetEnemySpells(FrostDaedraSpells);
                else if (___careerIndex == (int)MonsterCareers.FireDaedra)
                    __instance.SetEnemySpells(FireDaedraSpells);
                else if (___careerIndex == (int)MonsterCareers.Daedroth)
                    __instance.SetEnemySpells(DaedrothSpells);
                else if (___careerIndex == (int)MonsterCareers.Vampire)
                    __instance.SetEnemySpells(VampireSpells);
                else if (___careerIndex == (int)MonsterCareers.DaedraSeducer)
                    __instance.SetEnemySpells(SeducerSpells);
                else if (___careerIndex == (int)MonsterCareers.VampireAncient)
                    __instance.SetEnemySpells(VampireAncientSpells);
                else if (___careerIndex == (int)MonsterCareers.DaedraLord)
                    __instance.SetEnemySpells(DaedraLordSpells);
                else if (___careerIndex == (int)MonsterCareers.Lich)
                    __instance.SetEnemySpells(LichSpells);
                else if (___careerIndex == (int)MonsterCareers.AncientLich)
                    __instance.SetEnemySpells(AncientLichSpells);
            }
            else if (entityType == EntityTypes.EnemyClass && (mobileEnemy.CastsMagic))
            {
                int spellListLevel = ___level / 3;
                if (spellListLevel > 6)
                    spellListLevel = 6;
                __instance.SetEnemySpells(EnemyClassSpells[spellListLevel]);
            }

            // Chance of adding map
            DaggerfallLoot.RandomlyAddMap(mobileEnemy.MapChance, ___items);

            if (!string.IsNullOrEmpty(mobileEnemy.LootTableKey))
            {
                // Chance of adding potion
                DaggerfallLoot.RandomlyAddPotion(3, ___items);
                // Chance of adding potion recipe
                DaggerfallLoot.RandomlyAddPotionRecipe(2, ___items);
            }

            EnemyEntity.OnLootSpawned?.Invoke(__instance, new EnemyLootSpawnedEventArgs { MobileEnemy = mobileEnemy, EnemyCareer = ___career, Items = ___items });

            __instance.FillVitalSigns();

            return false;
        }
    }
}