using System.Collections.Generic;
using System.Runtime.InteropServices;
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

namespace MightyMagick.Formulas
{
    public class ResistanceAggregator
    {
        DFCareer.Elements elementType;
        DFCareer.EffectFlags effectFlags;
        DaggerfallEntity target;
        int modifier;
        PlayerEntity playerEntity;
        public ResistanceAggregator(DFCareer.Elements elementType, DFCareer.EffectFlags effectFlags, DaggerfallEntity target, int modifier)
        {
            this.elementType = elementType;
            this.effectFlags = effectFlags;
            this.target = target;
            this.modifier = modifier;
            playerEntity = GameManager.Instance.PlayerEntity;
        }

        private int AggregateBiography()
        {
            if (target == playerEntity)
            {
               if ((effectFlags & DFCareer.EffectFlags.Disease) != 0)
               {
                   return playerEntity.BiographyResistDiseaseMod;
               }

               if ((effectFlags & DFCareer.EffectFlags.Poison) != 0)
               {
                   return playerEntity.BiographyResistPoisonMod;
               }

               if ((effectFlags & DFCareer.EffectFlags.Magic) != 0)
               {
                   return playerEntity.BiographyResistMagicMod;
               }
            }

            return 0;
        }

        private static bool SpellHasFlags(DFCareer.Elements elementType, DFCareer.EffectFlags checkFlags, DFCareer.EffectFlags spellEffectFlags)
        {
            return (elementType == DFCareer.Elements.Fire && (checkFlags & DFCareer.EffectFlags.Fire) != 0) ||
            (elementType == DFCareer.Elements.Frost && (checkFlags & DFCareer.EffectFlags.Frost) != 0) ||
            (elementType == DFCareer.Elements.DiseaseOrPoison && (checkFlags & spellEffectFlags & (DFCareer.EffectFlags.Disease | DFCareer.EffectFlags.Poison)) != 0) ||
            (elementType == DFCareer.Elements.Shock && (checkFlags & DFCareer.EffectFlags.Shock) != 0) ||
            (elementType == DFCareer.Elements.Magic && (checkFlags & DFCareer.EffectFlags.Magic) != 0) ||
            (spellEffectFlags & DFCareer.EffectFlags.Paralysis) != 0 && (checkFlags & DFCareer.EffectFlags.Paralysis) != 0;
        }

        private int AggregateRacials()
        {
            if (target == playerEntity)
            {
                var raceTemplate = playerEntity.GetLiveRaceTemplate();
                if (SpellHasFlags(elementType, raceTemplate.ResistanceFlags, effectFlags))
                    return 50;
                if (SpellHasFlags(elementType, raceTemplate.ImmunityFlags, effectFlags))
                    return 100;
                if (SpellHasFlags(elementType, raceTemplate.LowToleranceFlags, effectFlags))
                    return -50;
                if (SpellHasFlags(elementType, raceTemplate.CriticalWeaknessFlags, effectFlags))
                    return -100;
            }
            return 0;
        }

        private int AggregateCareer()
        {
            //I really don't like this code but I've copied from main game and hesitant to refactor
            //Not sure that the flags are and why these checks are written like this
            DFCareer.ToleranceFlags toleranceFlags = DFCareer.ToleranceFlags.Normal;
            if ((effectFlags & DFCareer.EffectFlags.Paralysis) != 0)
            {
                toleranceFlags |= FormulaHelper.GetToleranceFlag(target.Career.Paralysis);
            }

            if ((effectFlags & DFCareer.EffectFlags.Magic) != 0)
            {
                toleranceFlags |= FormulaHelper.GetToleranceFlag(target.Career.Magic);
            }

            if ((effectFlags & DFCareer.EffectFlags.Poison) != 0)
            {
                toleranceFlags |= FormulaHelper.GetToleranceFlag(target.Career.Poison);
            }

            if ((effectFlags & DFCareer.EffectFlags.Fire) != 0)
            {
                toleranceFlags |= FormulaHelper.GetToleranceFlag(target.Career.Fire);
            }   

            if ((effectFlags & DFCareer.EffectFlags.Frost) != 0)
            {
                toleranceFlags |= FormulaHelper.GetToleranceFlag(target.Career.Frost);
            }

            if ((effectFlags & DFCareer.EffectFlags.Shock) != 0)
            {
                toleranceFlags |= FormulaHelper.GetToleranceFlag(target.Career.Shock);
            }
                
            if ((effectFlags & DFCareer.EffectFlags.Disease) != 0)
            {
                toleranceFlags |= FormulaHelper.GetToleranceFlag(target.Career.Disease);
            }

            if ((toleranceFlags & DFCareer.ToleranceFlags.Immune) != 0)
                return 100;
            if ((toleranceFlags & DFCareer.ToleranceFlags.CriticalWeakness) != 0)
                return -100;
            if ((toleranceFlags & DFCareer.ToleranceFlags.LowTolerance) != 0)
                return -50;
            if ((toleranceFlags & DFCareer.ToleranceFlags.Resistant) != 0)
                return 50;

            return 0;
        }

        private int AggregateMagicEffect()
        {
            if (target.HasResistanceFlag(elementType))
            {
                return target.GetResistanceChance(elementType);
            }
            return 0;
        }

        public int AggregateResistances()
        { 
            return AggregateCareer() + AggregateRacials() + AggregateBiography() + AggregateMagicEffect() + modifier;
        }
    }
}

