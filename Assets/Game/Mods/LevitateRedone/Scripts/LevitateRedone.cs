// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2021 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using UnityEngine;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Items;

namespace LevitateRedoneMod
{
    /// <summary>
    /// Levitate spell, overhauled in this mod. Replaces the original.
    /// </summary>
    public class LevitateRedoneMagicEffect : IncumbentEffect
    {
        public static readonly string EffectKey = "Levitate";
        public override void SetProperties()
        {
            properties.Key = EffectKey;    
            properties.SupportDuration = true;
            properties.SupportMagnitude = true;
            properties.AllowedTargets = TargetTypes.CasterOnly;
            properties.AllowedElements = ElementTypes.Magic;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Thaumaturgy;
            var modsettings = LevitateRedoneMod.Instance.LevitateRedoneModSettings;
            properties.DurationCosts = MakeEffectCosts(modsettings.DurationBaseCost, modsettings.DurationLevelCost);
            properties.MagnitudeCosts = MakeEffectCosts(modsettings.MagnitudeBaseCost, modsettings.MagnitudeLevelCost);
            properties.ClassicKey = MakeClassicKey(14, 255);
        }
        public override string GroupName => TextManager.Instance.GetLocalizedText("levitate");
        public override TextFile.Token[] SpellMakerDescription => GetSpellMakerEffectDescription();
        public override TextFile.Token[] SpellBookDescription => GetSpellBookEffectDescription();

        public override void SetPotionProperties()
        {
            PotionRecipe levitation = new PotionRecipe(
                "levitation",
                125,
                DefaultEffectSettings(),
                (int)DaggerfallWorkshop.Game.Items.MiscellaneousIngredients1.Pure_water,
                (int)DaggerfallWorkshop.Game.Items.MiscellaneousIngredients1.Nectar,
                (int)DaggerfallWorkshop.Game.Items.CreatureIngredients1.Ectoplasm);

            AssignPotionRecipes(levitation);
        }

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);
            StartLevitating();
        }

        public override void Resume(EntityEffectManager.EffectSaveData_v1 effectData, EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Resume(effectData, manager, caster);
            StartLevitating();
        }


        public override void End()
        {
            base.End();
            StopLevitating();
        }


        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return (other is LevitateRedoneMagicEffect);
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            // Stack my rounds onto incumbent
            incumbent.RoundsRemaining += RoundsRemaining;
        }

        float CalculateLeviationSpeed()
        {
            var spellMagnitude = GetMagnitude(caster);

            return ((0.25f * spellMagnitude) + 1);
        }

        void StartLevitating()
        {
            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;
            // Enable levitation for player or enemies
            if (entityBehaviour.EntityType == EntityTypes.Player)
            {
                GameManager.Instance.PlayerMotor.GetComponent<LevitateMotor>().IsLevitating = true;
                GameManager.Instance.PlayerMotor.GetComponent<LevitateMotor>().LevitateMoveSpeed = CalculateLeviationSpeed();
            }
            else if (entityBehaviour.EntityType == EntityTypes.EnemyMonster || entityBehaviour.EntityType == EntityTypes.EnemyClass)
            {
                EnemyMotor enemyMotor = entityBehaviour.GetComponent<EnemyMotor>();
                if (enemyMotor)
                {
                    //Todo: increase levitation speed for enemies as well.
                    enemyMotor.IsLevitating = true;
                }
            }
        }

        void StopLevitating()
        {
            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Disable levitation for player or enemies
            if (entityBehaviour.EntityType == EntityTypes.Player)
            {
                GameManager.Instance.PlayerMotor.GetComponent<LevitateMotor>().IsLevitating = false;
            }
            else if (entityBehaviour.EntityType == EntityTypes.EnemyMonster || entityBehaviour.EntityType == EntityTypes.EnemyClass)
            {
                EnemyMotor enemyMotor = entityBehaviour.GetComponent<EnemyMotor>();
                if (enemyMotor)
                    enemyMotor.IsLevitating = false;
            }
        }

        private TextFile.Token[] GetSpellMakerEffectDescription()
        {
            return DaggerfallUnity.Instance.TextProvider.CreateTokens(
                TextFile.Formatting.JustifyCenter,
                DisplayName,
                "Duration: Rounds the magic lasts.",
                "Chance: N/A",
                "Magnitude: Increases Speed.",
                effectDescription);
        }

        private TextFile.Token[] GetSpellBookEffectDescription()
        {
            return DaggerfallUnity.Instance.TextProvider.CreateTokens(
                TextFile.Formatting.JustifyCenter,
                DisplayName,
                "Duration: Spell lasts %bdr + %adr per %cld level(s)",
                "Chance: N/A",
                "Magnitude: %1bm - %2bm + %1am - %2am per %clm level(s)",
                effectDescription);
        }

        private const string effectDescription = "Target is able for float above the ground. Higher magnitude speeds up movement.";
    }
}
