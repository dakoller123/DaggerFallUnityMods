// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2021 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    
// 
// Notes:
//

using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game;
using System.Collections.Generic;
using System;

namespace MightyMagick.MagicEffects
{
    /// <summary>
    /// Heal - Magicka
    /// </summary>
    public class HealSpellPoints : BaseEntityEffect
    {
        public static readonly string EffectKey = "Heal-SpellPoints";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.SupportMagnitude = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_Self;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.PotionMaker;
        }

        public override string GroupName => TextManager.Instance.GetLocalizedText("heal");
        public override string SubGroupName => TextManager.Instance.GetLocalizedText("spellPoints");

        public override void SetPotionProperties()
        {
            // Magnitude 5-5 + 4-4 per 1 levels
            var modPotionSettings = MightyMagickMod.Instance.MightyMagickModSettings.PotionSettings;

            EffectSettings restorePowerSettings = SetEffectMagnitude(DefaultEffectSettings(), modPotionSettings.PotionMagnitude, modPotionSettings.PotionMagnitude, 0, 0, 1);
            PotionRecipe restorePower = new PotionRecipe(
                "restorePower",
                75,
                DefaultEffectSettings(),
                (int)DaggerfallWorkshop.Game.Items.MiscellaneousIngredients1.Nectar,
                (int)DaggerfallWorkshop.Game.Items.MetalIngredients.Silver,
                (int)DaggerfallWorkshop.Game.Items.CreatureIngredients1.Werewolfs_blood,
                (int)DaggerfallWorkshop.Game.Items.CreatureIngredients1.Saints_hair);

            // Assign recipe
            restorePower.TextureRecord = 12;
            AssignPotionRecipes(restorePower);
        }

        public override void MagicRound()
        {
            base.MagicRound();

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Implement effect
            int magnitude = GetMagnitude(caster);
            var modPotionSettings = MightyMagickMod.Instance.MightyMagickModSettings.PotionSettings;

            var result = CalculateResultMagicka(magnitude, entityBehaviour.Entity.MaxMagicka, modPotionSettings);
            
            entityBehaviour.Entity.IncreaseMagicka(result);
        }

        private int CalculateResultMagicka(int magnitude,int maxMagicka, PotionSettings settings)
        {
            int flatValue = (settings.MagnitudeCalculation ==  PotionMagnitudeCalculationTypes.Percentage)
                ? (maxMagicka * magnitude / 100)
                : magnitude;

            return Math.Max((currentMagicka + flatValue, maxMagicka));
        }
    }
}
