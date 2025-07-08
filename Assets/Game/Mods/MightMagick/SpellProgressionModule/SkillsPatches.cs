using System;
using System.Collections.Generic;
using DaggerfallConnect;
using MightyMagick;
using UnityEngine;

namespace Game.Mods.MightMagick.SpellProgressionModule
{
    public static class SkillsPatches
    {
        static readonly List<DFCareer.Skills> MagicSkills = new List<DFCareer.Skills>{
            DFCareer.Skills.Alteration,
            DFCareer.Skills.Destruction,
            DFCareer.Skills.Restoration,
            DFCareer.Skills.Illusion,
            DFCareer.Skills.Mysticism,
            DFCareer.Skills.Thaumaturgy
        };

        public static void Postfix_GetAdvancementMultiplier(DFCareer.Skills skill, ref int __result)
        {
            if (!MagicSkills.Contains(skill))
                return;

            __result = Mathf.RoundToInt(__result * MightyMagickMod.Instance.MightyMagickModSettings.SkillProgressionSettings.BaseCost * MightyMagickMod.Instance.MightyMagickModSettings.SpellCostSettings.Multiplier);
        }
    }
}
