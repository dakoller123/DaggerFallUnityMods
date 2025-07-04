// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:
//
// Notes:
//

using UnityEngine;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Entity;
using System.Collections.Generic;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;

namespace MightyMagick.MagicEffects
{
    /// <summary>
    /// Creates a soft glow around player based on variant of effect.
    /// This is a primarily a demo class for creating a custom effect.
    /// Effect is intended to be a simple example without being too trivial.
    /// Does not require any serialization. See other effects for examples of how to save effect state.
    /// </summary>
    public class MageLight : IncumbentEffect
    {
        #region Fields

        string effectKey = "MageLightInfernoMag";

        private Color32 effectColor =
            new Color32(154, 24, 8, 255);

        Dictionary<string, string> stringTable = null;
        Light myLight = null;

        #endregion

        #region Properties
        //
        // // Must override Properties to return correct properties for any variant
        // // The currentVariant value is set by magic framework - each variant gets enumerated to its own effect template
        // public override EffectProperties Properties
        // {
        //     get { return variantProperties[currentVariant].effectProperties; }
        // }

        #endregion

        #region Construtors

        public MageLight()
        {
            // Attempt to import replacement text from CSV
            LoadText();
        }

        #endregion

        #region Overrides

        public override void SetProperties()
        {
            // Set properties shared by all variants
            properties.Key = effectKey;
            properties.SupportDuration = true;
            properties.SupportMagnitude = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_Self;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.MagicSkill = DFCareer.MagicSkills.Illusion;
            properties.DurationCosts = MakeEffectCosts(8, 40);
            properties.MagnitudeCosts = MakeEffectCosts(8, 40);
            properties.DisableReflectiveEnumeration = true;
        }

        public override string GroupName => groupName;
        public override string SubGroupName => subGroupName;
        public override TextFile.Token[] SpellMakerDescription => GetSpellMakerDescription();
        public override TextFile.Token[] SpellBookDescription => GetSpellBookDescription();

        // Start is called when the effect is first initialised - do any setup work here
        // Note: Start is called even if effect is never assigned to entity
        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);
            StartLight();
        }

        // Resume is called when effect is restored from save
        // Note: Start is not called when effect is restored from save, you may need to do some setup again
        //       The setup you do in resume may be different than first-time setup, which is why these are separated
        public override void Resume(EntityEffectManager.EffectSaveData_v1 effectData, EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Resume(effectData, manager, caster);
            StartLight();
        }

        // End is called when effect is finished - do any cleanup work here
        public override void End()
        {
            base.End();
            EndLight();
        }

        // MagicRound is called immediately after effect is assigned to an entity and once per game minute
        // If an effect is resisted by target, it will never receive a single MagicRound
        public override void MagicRound()
        {
            base.MagicRound();
        }

        // ConstantEffect is called once per frame - do any work here that needs to be performed more than once per round
        public override void ConstantEffect()
        {
            base.ConstantEffect();

            // Keep light positioned on top of player
            if (myLight)
            {
                myLight.transform.position = GameManager.Instance.PlayerObject.transform.position;
            }
        }

        // IsLikeKind checks if another incumbent effect is equal to this effect
        // Can use class type, key, or whatever conditions you need to test equivalency
        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return other is MageLight;
        }

        // AddState is used to stack something onto an incumbent for this effect - e.g. top-up rounds remaining
        protected override void AddState(IncumbentEffect incumbent)
        {
            // Stack my rounds onto incumbent
            incumbent.RoundsRemaining += RoundsRemaining;
        }

        #endregion

        #region Effect Payload

        void StartLight()
        {
            // Do nothing if light already started
            if (myLight)
                return;

            // Using a static helper in FoeSpawner to find best scene parent
            // We want the light to be assigned to one of the scene parents so it will be cleaned up on scene reset
            Transform parent = GameObjectHelper.GetBestParent();

            // Create the light object
            GameObject go = new GameObject(effectKey);
            go.transform.parent = parent;
            myLight = go.AddComponent<Light>();
            myLight.type = LightType.Point;
            myLight.color = effectColor;
            var magnitude = GetMagnitude();
            myLight.range = 18.0f * magnitude;
            myLight.intensity = 1.1f;
        }

        void EndLight()
        {
            // Destroy the light gameobject when done
            if (myLight)
                GameObject.Destroy(myLight.gameObject);
        }

        #endregion

        #region Text

        // Default strings
        string groupName = "Mage Light";
        string subGroupName = "InfernoMag";
        string effectDescription = "Creates a soft light around caster.";
        string durationSpellMaker = "Duration: Rounds you will be illuminated.";
        string durationSpellBook = "Duration: %bdr + %adr per %cld level(s)";
        string chanceText = "Chance: N/A";
        string magnitudeText = "Magnitude: %1bm - %2bm + %1am - %2am per %clm level(s)";

        /// <summary>
        /// Try to load replacement text from a CSV in StreamingAssets/Text if one is provided.
        /// Spell templates are read and stored in multiple places so CSV could be loaded multiple times.
        /// CSV will also be loaded anytime spell is instantiated or text required.
        /// For a more sophisticated setup use a centralised text database and only load text once.
        /// </summary>
        void LoadText()
        {
            const string csvFilename = "Example_MageLight.csv";

            if (stringTable != null)
                return;

            stringTable = StringTableCSVParser.LoadDictionary(csvFilename);
            if (stringTable == null || stringTable.Count == 0)
                return;

            if (stringTable.ContainsKey("groupName"))
                groupName = stringTable["groupName"];

            // if (stringTable.ContainsKey("subGroupNames"))
            //     subGroupNames = TextManager.Instance.SplitTextList(stringTable["subGroupNames"]);

            if (stringTable.ContainsKey("effectDescription"))
                effectDescription = stringTable["effectDescription"];

            if (stringTable.ContainsKey("durationSpellMaker"))
                durationSpellMaker = stringTable["durationSpellMaker"];

            if (stringTable.ContainsKey("durationSpellBook"))
                durationSpellBook = stringTable["durationSpellBook"];

            if (stringTable.ContainsKey("chance"))
                chanceText = stringTable["chance"];

            if (stringTable.ContainsKey("magnitude"))
                magnitudeText = stringTable["magnitude"];
        }

        TextFile.Token[] GetSpellMakerDescription()
        {
            return DaggerfallUnity.Instance.TextProvider.CreateTokens(
                TextFile.Formatting.JustifyCenter,
                groupName,
                effectDescription,
                durationSpellMaker,
                chanceText,
                magnitudeText);
        }

        TextFile.Token[] GetSpellBookDescription()
        {
            return DaggerfallUnity.Instance.TextProvider.CreateTokens(
                TextFile.Formatting.JustifyCenter,
                groupName,
                durationSpellBook,
                chanceText,
                magnitudeText,
                effectDescription);
        }

        #endregion
    }
}
