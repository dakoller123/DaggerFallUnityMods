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
        bool castByItem = false;
        public static readonly string EffectKey = "Levitate";
        int forcedRoundsRemaining = 1;
        public override void SetProperties()
        {
            properties.Key = EffectKey;    
            properties.SupportDuration = true;
            properties.SupportMagnitude = true;
            properties.SupportChance = true;
            properties.AllowedTargets = TargetTypes.CasterOnly;
            properties.AllowedElements = ElementTypes.Magic;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Thaumaturgy;
            properties.DurationCosts = MakeEffectCosts(100, 300);
            properties.MagnitudeCosts = MakeEffectCosts(200, 400);
            properties.ChanceCosts = MakeEffectCosts(40, 300);
            properties.ClassicKey = MakeClassicKey(14, 255);
            properties.ChanceFunction = ChanceFunction.Custom;
        }
        public override string GroupName => TextManager.Instance.GetLocalizedText("levitate");
        public override TextFile.Token[] SpellMakerDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1562);
        public override TextFile.Token[] SpellBookDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1262);

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

        void CheckCastByItem()
        {
            castByItem = ParentBundle.castByItem != null;
        }

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);
            StartOrKeepLevitatingIfRollChanceSuccess();
        }

        public override void Resume(EntityEffectManager.EffectSaveData_v1 effectData, EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Resume(effectData, manager, caster);
            StartOrKeepLevitatingIfRollChanceSuccess();
        }


        public override void End()
        {
            base.End();
            StopLevitating();
        }

        public override int RoundsRemaining
        {
            get { return forcedRoundsRemaining; }
        }


        protected override int RemoveRound()
        {
            return forcedRoundsRemaining;
        }


        public override void MagicRound()
        {
            base.MagicRound();
            forcedRoundsRemaining--;
            StartOrKeepLevitatingIfRollChanceSuccess();
        }

        /// <summary>
        /// Cancel effect.
        /// </summary>
        public void CancelEffect()
        {
            forcedRoundsRemaining = 0;
            ResignAsIncumbent();
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

            return (0.25f * spellMagnitude);
        }

        void StartOrKeepLevitatingIfRollChanceSuccess()
        {
            // Do nothing if spell chance fails
            // Always succeeds chance roll when cast by item but still subject to level vs. door requirement
            if (!RollChance() && !castByItem || forcedRoundsRemaining < 1)
            {
                DaggerfallUI.AddHUDText(TextManager.Instance.GetLocalizedText("spellEffectFailed"));
                CancelEffect();
                StopLevitating();
                return;
            }
            else
            {
                StartLevitating();
            }
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
    }
}
