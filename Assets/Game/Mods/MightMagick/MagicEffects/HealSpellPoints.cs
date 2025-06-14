using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game;

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
                restorePowerSettings,
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
            var maxMagicka = entityBehaviour.Entity.MaxMagicka;
            int increaseValue = (modPotionSettings.MagnitudeCalculation ==  PotionMagnitudeCalculationTypes.Percentage)
                ? (maxMagicka * magnitude / 100)                
                : magnitude;

            entityBehaviour.Entity.IncreaseMagicka(increaseValue);
        }
    }
}
