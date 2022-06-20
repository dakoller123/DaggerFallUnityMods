// Project:   Clairvoyance for Daggerfall Unity
// Author:    kiskoller
// Based on code from:    DunnyOfPenwick

using System;
using UnityEngine;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Formulas;

namespace MightyMagick.Formulas
{
    public static class FormulaOverrides
    {
        public static void RegisterFormulaOverrides(Mod mod)
        {

            FormulaHelper.RegisterOverride(mod, "CalculateEffectCosts", (Func<IEntityEffect, EffectSettings, DaggerfallEntity, FormulaHelper.SpellCost>)MagickaCost.CalculateEffectCosts);
            FormulaHelper.RegisterOverride(mod, "SpellPoints", (Func<int, float, int>)MagickaPoolSize.SpellPoints);
            FormulaHelper.RegisterOverride(mod, "CalculateSpellPointRecoveryRate", (Func< PlayerEntity, int>)SpellPointRecoveryRate.CalculateSpellPointRecoveryRate);
            FormulaHelper.RegisterOverride(mod, "SavingThrow", (Func<DFCareer.Elements, DFCareer.EffectFlags, DaggerfallEntity, int, int>)SavingThrow);
        }


        
        public static int SavingThrow(DFCareer.Elements elementType, DFCareer.EffectFlags effectFlags, DaggerfallEntity target, int modifier)
        {
            //This changes from chance based magic defense to percentage based one.


            int resistPercentage = 0;

            // Handle resistances granted by magical effects
            if (target.HasResistanceFlag(elementType))
            {
                resistPercentage += target.GetResistanceChance(elementType);
            }

            DFCareer.ToleranceFlags toleranceFlags = DFCareer.ToleranceFlags.Normal;
            int biographyMod = 0;

            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            if ((effectFlags & DFCareer.EffectFlags.Paralysis) != 0)
            {
                toleranceFlags |= FormulaHelper.GetToleranceFlag(target.Career.Paralysis);
                // Innate immunity if high elf. Start with 100 saving throw, but can be modified by
                // tolerance flags. Note this differs from classic, where high elves have 100% immunity
                // regardless of tolerance flags.
                if (target == playerEntity && playerEntity.Race == Races.HighElf)
                    resistPercentage = 100;
            }
            if ((effectFlags & DFCareer.EffectFlags.Magic) != 0)
            {
                toleranceFlags |= FormulaHelper.GetToleranceFlag(target.Career.Magic);
                if (target == playerEntity)
                    biographyMod += playerEntity.BiographyResistMagicMod;
            }
            if ((effectFlags & DFCareer.EffectFlags.Poison) != 0)
            {
                toleranceFlags |= FormulaHelper.GetToleranceFlag(target.Career.Poison);
                if (target == playerEntity)
                    biographyMod += playerEntity.BiographyResistPoisonMod;
            }
            if ((effectFlags & DFCareer.EffectFlags.Fire) != 0)
                toleranceFlags |= FormulaHelper.GetToleranceFlag(target.Career.Fire);
            if ((effectFlags & DFCareer.EffectFlags.Frost) != 0)
                toleranceFlags |= FormulaHelper.GetToleranceFlag(target.Career.Frost);
            if ((effectFlags & DFCareer.EffectFlags.Shock) != 0)
                toleranceFlags |= FormulaHelper.GetToleranceFlag(target.Career.Shock);
            if ((effectFlags & DFCareer.EffectFlags.Disease) != 0)
            {
                toleranceFlags |= FormulaHelper.GetToleranceFlag(target.Career.Disease);
                if (target == playerEntity)
                    biographyMod += playerEntity.BiographyResistDiseaseMod;
            }

            // Note: Differing from classic implementation here. In classic
            // immune grants always 100% resistance and critical weakness is
            // always 0% resistance if there is no immunity. Here we are using
            // a method that allows mixing different tolerance flags, getting
            // rid of related exploits when creating a character class.
            if ((toleranceFlags & DFCareer.ToleranceFlags.Immune) != 0)
                resistPercentage += 100;
            if ((toleranceFlags & DFCareer.ToleranceFlags.CriticalWeakness) != 0)
                resistPercentage -= 100;
            if ((toleranceFlags & DFCareer.ToleranceFlags.LowTolerance) != 0)
                resistPercentage -= 50;
            if ((toleranceFlags & DFCareer.ToleranceFlags.Resistant) != 0)
                resistPercentage += 50;

            resistPercentage += biographyMod + modifier;
            if (elementType == DFCareer.Elements.Frost && target == playerEntity && playerEntity.Race == Races.Nord)
                resistPercentage += 30;
            else if (elementType == DFCareer.Elements.Magic && target == playerEntity && playerEntity.Race == Races.Breton)
                resistPercentage += 30;

            // Handle perfect immunity of 100% or greater
            // Otherwise clamping to 5-95 allows a perfectly immune character to sometimes receive incoming payload
            // This doesn't seem to match immunity intent or player expectations from classic
            if (resistPercentage >= 100)
                return 0;

            resistPercentage = Mathf.Clamp(resistPercentage, 5, 95);

            int percentDamageOrDuration = 100 - resistPercentage;

            return percentDamageOrDuration;
        }
    } 


  
}