// Project:         Clairvoyance Mod
// Mod author:      kiskoller
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:
//
// Notes:
//  Still WiP.

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
    public class DetectResident : DetectEffect
    {
        public static readonly string EffectKey = "Detect-Resident";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.SupportDuration = false;
            properties.AllowedTargets = TargetTypes.CasterOnly;
            properties.AllowedElements = ElementTypes.Magic;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Thaumaturgy;
            properties.DurationCosts = MakeEffectCosts(40, 200);
            properties.ShowSpellIcon = true;
            properties.DisableReflectiveEnumeration = true;
        }
        protected override void AddState(IncumbentEffect incumbent)
        {
            // Stack my rounds onto incumbent
            incumbent.RoundsRemaining += RoundsRemaining;
        }

        public override string GroupName => TextManager.Instance.GetLocalizedText("detect");

        //Todo: localisation
        public override string SubGroupName => TextManager.Instance.GetLocalizedText("resident");
        public override TextFile.Token[] SpellMakerDescription => GetSpellMakerEffectDescription();
        public override TextFile.Token[] SpellBookDescription => GetSpellBookEffectDescription();

        private void RevealBuildings()
        {
            Debug.Log($"[ClairvoyanceMod] {nameof(RevealBuildings)} called");
            Debug.Log($"[ClairvoyanceMod] Marking key subject location on map");
            TalkManager.Instance.MarkKeySubjectLocationOnMap();

            var mapId = GameManager.Instance.PlayerGPS.CurrentMapID;
            Debug.Log($"[ClairvoyanceMod] Current MapID: {mapId}");

            var saveData = TalkManager.Instance.GetConversationSaveData();
            Debug.Log($"[ClairvoyanceMod] Loaded save data: {saveData}");

            var dictQuestInfo = saveData.dictQuestInfo;
            Debug.Log($"[ClairvoyanceMod] Found {dictQuestInfo.Count} quest entries in save data");

            foreach (ulong questId in GameManager.Instance.QuestMachine.GetAllActiveQuests())
            {
                Debug.Log($"[ClairvoyanceMod] Processing quest: {questId}");
                Quest quest = GameManager.Instance.QuestMachine.GetQuest(questId);

                if (dictQuestInfo.ContainsKey(questId))
                {
                    var questInfo = dictQuestInfo[questId];
                    Debug.Log($"[ClairvoyanceMod] Found quest info for {questId}");

                    QuestResource[] questResources = quest.GetAllResources(typeof(Place));
                    Debug.Log($"[ClairvoyanceMod] Found {questResources.Length} place resources");

                    for (int i = 0; i < questResources.Length; i++)
                    {
                        Place place = (Place)questResources[i];
                        string key = place.Symbol.Name;
                        Debug.Log($"[ClairvoyanceMod] Checking place: {key} (MapID: {place.SiteDetails.mapId})");

                        if (place.SiteDetails.mapId != mapId)
                        {
                            Debug.Log($"[ClairvoyanceMod] Skipping place {key} - MapID mismatch");
                            continue;
                        }

                        if (questInfo.resourceInfo.ContainsKey(key))
                        {
                            Debug.Log($"[ClairvoyanceMod] Found matching resource key: {key}");
                            Debug.Log($"[ClairvoyanceMod] Setting hint type for {key} to: {TalkManager.QuestResourceInfo.BuildingLocationHintTypeGiven.LocationWasMarkedOnMap}");

                            questInfo.resourceInfo[key].questPlaceResourceHintTypeReceived =
                                TalkManager.QuestResourceInfo.BuildingLocationHintTypeGiven.LocationWasMarkedOnMap;

                            Debug.Log($"[ClairvoyanceMod] Marking building {buildingInfo.buildingKey} as discovered");
                            GameManager.Instance.PlayerGPS.DiscoverBuilding(buildingInfo.buildingKey);
                        }
                    }
                }
            }

            Debug.Log($"[ClairvoyanceMod] Restoring quest data: {saveData}");
            TalkManager.Instance.RestoreConversationData(saveData);
            Debug.Log($"[ClairvoyanceMod] Final map location mark");
            TalkManager.Instance.MarkKeySubjectLocationOnMap();
        }


        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);
            RevealBuildings();
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
            RevealBuildings();
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

        private const string effectDescription1 = "Caster learns the names of residental buildings in settlements.";
        private const string effectDescription2 = "Spell only works in towns and cities.";
    }
}
