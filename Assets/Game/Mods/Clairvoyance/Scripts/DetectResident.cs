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
    /// <summary>
    /// Detect Quest
    /// </summary>
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

        public override string GroupName => TextManager.Instance.GetLocalizedText("detect");

        //Todo: localisation
        public override string SubGroupName => TextManager.Instance.GetLocalizedText("resident");
        public override TextFile.Token[] SpellMakerDescription => GetSpellMakerEffectDescription();
        public override TextFile.Token[] SpellBookDescription => GetSpellBookEffectDescription();

        private void RevealBuildings()
        {
            Debug.Log("ClairvoyanceMod: RevealBuildings called");
            TalkManager.Instance.MarkKeySubjectLocationOnMap();
            var mapId = GameManager.Instance.PlayerGPS.CurrentMapID;
            Debug.Log($"ClairvoyanceMod: MapId: {mapId}");
            var saveData = TalkManager.Instance.GetConversationSaveData();
            var dictQuestInfo = saveData.dictQuestInfo;
            foreach (ulong questId in GameManager.Instance.QuestMachine.GetAllActiveQuests())
            {
                Debug.Log($"ClairvoyanceMod: Checking quest: {questId}");
                Quest quest = GameManager.Instance.QuestMachine.GetQuest(questId);

                if (dictQuestInfo.ContainsKey(questId))
                {
                    var questInfo = dictQuestInfo[questId]; // Get questInfo containing orphaned list of quest resources

                    QuestResource[] questResources = quest.GetAllResources(typeof(Place)); // Get list of place quest resources
                    for (int i = 0; i < questResources.Length; i++)
                    {
                        Place place = (Place)questResources[i];
                        string key = place.Symbol.Name;
                        Debug.Log($"ClairvoyanceMod: Checking place: {key}");

                        // Always ensure we are locating building key in current location, not just same building key in another location within same quest
                        if (place.SiteDetails.mapId != mapId )
                            continue;

                        if (questInfo.resourceInfo.ContainsKey(key))
                        {
                            Debug.Log($"ClairvoyanceMod: Setting place {key} BuildingLocationHintTypeGiven to true");
                            questInfo.resourceInfo[key].questPlaceResourceHintTypeReceived = TalkManager.QuestResourceInfo.BuildingLocationHintTypeGiven.LocationWasMarkedOnMap;
                            GameManager.Instance.PlayerGPS.DiscoverBuilding(buildingInfo.buildingKey);
                        }
                    }
                }
            }
            Debug.Log($"ClairvoyanceMod: Restoring questdata {saveData}");
            TalkManager.Instance.RestoreConversationData(saveData);
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
