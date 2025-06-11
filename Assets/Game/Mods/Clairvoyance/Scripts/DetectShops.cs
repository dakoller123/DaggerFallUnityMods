using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility;
using UnityEngine;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ClairvoyanceMod
{
    public class DetectShop : DetectEffect
    {
        public static readonly string EffectKey = "Detect-Shop";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.SupportDuration = true;
            properties.AllowedTargets = TargetTypes.CasterOnly;
            properties.AllowedElements = ElementTypes.Magic;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Thaumaturgy;
            properties.DurationCosts = MakeEffectCosts(40, 200);
            properties.ShowSpellIcon = true;
            properties.DisableReflectiveEnumeration = true;
        }

        public override string GroupName => TextManager.Instance.GetLocalizedText("detect");

        //Todo: localisation
        public override string SubGroupName => TextManager.Instance.GetLocalizedText("shop");
        public override TextFile.Token[] SpellMakerDescription => GetSpellMakerEffectDescription();
        public override TextFile.Token[] SpellBookDescription => GetSpellBookEffectDescription();

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return (other is DetectShops);
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            incumbent.RoundsRemaining += RoundsRemaining;
        }

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);
        }

        public override void End()
        {
            base.End(); 
            if (ExteriorAutomap.instance != null)           
            {
                ExteriorAutomap.instance.RevealUndiscoveredBuildings = false;
            }
        }

        public override void MagicRound()
        {
            base.MagicRound();
            if (ExteriorAutomap.instance != null && !GameManager.Instance.IsPlayerInside)           
            {
                ExteriorAutomap.instance.RevealUndiscoveredBuildings = true;
            }   
        }


        private TextFile.Token[] GetSpellMakerEffectDescription()
        {
            return DaggerfallUnity.Instance.TextProvider.CreateTokens(
                TextFile.Formatting.JustifyCenter,
                DisplayName,
                "Duration: Rounds the magic lasts.",
                "Chance: N/A",
                "Magnitude: N/A",
                effectDescription1,
                effectDescription2);
        }

        private TextFile.Token[] GetSpellBookEffectDescription()
        {
            return DaggerfallUnity.Instance.TextProvider.CreateTokens(
                TextFile.Formatting.JustifyCenter,
                DisplayName,
                "Duration: %bdr + %adr per %cld level(s)",
                "Chance: N/A",
                "Magnitude: N/A",
                effectDescription1,
                effectDescription2);
        }

        private const string effectDescription1 = "Caster learns the names and locations of shops in settlements.";
        private const string effectDescription2 = "Spell only works in towns and cities.";
    }
}
