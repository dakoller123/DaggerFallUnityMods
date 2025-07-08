using System;
using System.Collections.Generic;
using System.Linq;
using DaggerfallConnect;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.MagicAndEffects;
using MightyMagick;
using UnityEngine;

namespace Game.Mods.MightMagick.SpellProgressionModule
{
    public static class FormulaPatches
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

            if (IsShield(item)) return ShieldPenalty;

            if (item.ItemGroup == ItemGroups.Armor && spellCostSettings.EquipmentPenalty)
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

            if (item.ItemGroup == ItemGroups.Weapons)
                return item.TemplateIndex == (int)Weapons.Staff ? StaffPenalty : WeaponPenalty;

            return 0;
        }


        public static void PostFix_CalculateEffectCosts(ref FormulaHelper.SpellCost __result, IEntityEffect effect, EffectSettings settings, DaggerfallEntity casterEntity = null)
        {
            __result.spellPointCost = Mathf.RoundToInt(__result.spellPointCost * MightyMagickMod.Instance.MightyMagickModSettings.SpellCostSettings.Multiplier);
        }

        public static void PostFix_CalculateTotalEffectCosts(ref FormulaHelper.SpellCost __result, EffectEntry[] effectEntries, TargetTypes targetType, DaggerfallEntity casterEntity = null, bool minimumCastingCost = false)
        {
            //caster entity is null == it's the player.
            if (casterEntity != null) return;

            var armorPenalty =  GameManager.Instance.PlayerEntity.ItemEquipTable.EquipTable.Sum(GetItemPenalty);
            __result.spellPointCost = Mathf.RoundToInt(__result.spellPointCost * (1.0f + armorPenalty / 100.0f));
        }
    }
}
