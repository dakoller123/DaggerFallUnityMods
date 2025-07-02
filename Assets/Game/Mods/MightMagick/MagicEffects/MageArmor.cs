using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using UnityEngine;

namespace MightyMagick.MagicEffects
{
    /// <summary>
    /// MageArmor
    /// </summary>
    public class MageArmor : IncumbentEffect
    {
        public static readonly string EffectKey = "MageArmor";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(25, 255);
            properties.SupportDuration = true;
            properties.SupportMagnitude = true;
            properties.AllowedTargets = TargetTypes.CasterOnly;
            properties.AllowedElements = ElementTypes.Magic;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Alteration;
            properties.DurationCosts = MakeEffectCosts(30, 8);
            properties.MagnitudeCosts = MakeEffectCosts(300, 120);
        }

        public override string GroupName => "Mage Armor";
        public override TextFile.Token[] SpellMakerDescription => GetSpellMakerEffectDescription();
        public override TextFile.Token[] SpellBookDescription => GetSpellBookEffectDescription();

        private TextFile.Token[] GetSpellMakerEffectDescription()
        {
            return DaggerfallUnity.Instance.TextProvider.CreateTokens(
                TextFile.Formatting.JustifyCenter,
                DisplayName,
                "Duration: Rounds the magic lasts.",
                "Chance: N/A",
                "Magnitude: Increases Protection.",
                effectDescription);
        }

        private TextFile.Token[] GetSpellBookEffectDescription()
        {
            return DaggerfallUnity.Instance.TextProvider.CreateTokens(
                TextFile.Formatting.JustifyCenter,
                DisplayName,
                "Duration: %bdr + %adr per %cld level(s)",
                "Chance: N/A",
                "Magnitude: %1bm - %2bm + %1am - %2am per %clm level(s)",
                effectDescription);
        }

        private const string effectDescription = "Conjures a magical forcefield around the caster which acts like armor, protecting against physical harm. ";
        public override void ConstantEffect()
        {
            base.ConstantEffect();
            SetArmorEffect();
        }

        public override void Resume(EntityEffectManager.EffectSaveData_v1 effectData, EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Resume(effectData, manager, caster);
            SetArmorEffect();
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return (other is MageArmor);
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            // Stack my rounds onto incumbent
            incumbent.RoundsRemaining += RoundsRemaining;
        }

        void SetArmorEffect()
        {
            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            if (!(entityBehaviour == GameManager.Instance.PlayerEntity.EntityBehaviour))
                return;

            var magnitude = GetMagnitude(entityBehaviour);
            entityBehaviour.Entity.SetIncreasedArmorValueModifier(-1 * magnitude);
        }
    }
}
