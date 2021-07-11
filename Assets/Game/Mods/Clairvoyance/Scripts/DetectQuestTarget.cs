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

namespace ClairvoyanceMod
{
    /// <summary>
    /// Detect Quest
    /// </summary>
    public class DetectQuest : DetectEffect
    {
        public static readonly string EffectKey = "Detect-Quest";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.SupportDuration = true;
            properties.AllowedTargets = TargetTypes.CasterOnly;
            properties.AllowedElements = ElementTypes.Magic;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Thaumaturgy;
            properties.DurationCosts = MakeEffectCosts(8, 40);
            properties.ShowSpellIcon = true;
            properties.DisableReflectiveEnumeration = true;
        }

        public override string GroupName => TextManager.Instance.GetLocalizedText("detect");

        //Todo: localisation
        public override string SubGroupName => "Quest";
        public override TextFile.Token[] SpellMakerDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1598);
        public override TextFile.Token[] SpellBookDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1298);

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return (other is DetectQuest);
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            // Stack my rounds onto incumbent
            incumbent.RoundsRemaining += RoundsRemaining;
        }

        private void AddQuestMarkerToTracked()
        {
            DetectedObjects.Clear();
            if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon)
            {
                QuestMarker spawnMarker;
                Vector3 buildingOrigin;
                bool result = QuestMachine.Instance.GetCurrentLocationQuestMarker(out spawnMarker, out buildingOrigin);
                if (result)
                {
                    Vector3 dungeonBlockPosition = new Vector3(spawnMarker.dungeonX * RDBLayout.RDBSide, 0, spawnMarker.dungeonZ * RDBLayout.RDBSide);
                    Vector3 questTargetPosition = dungeonBlockPosition + spawnMarker.flatPosition;
                    GameObject questTarget = new GameObject();
                    questTarget.name = "questTarget";
                    questTarget.transform.position = questTargetPosition;                    
                    PlayerGPS.NearbyObject nearbyObject;
                    nearbyObject.gameObject = questTarget;
                    nearbyObject.flags = PlayerGPS.NearbyObjectFlags.None;
                    nearbyObject.distance = 0;
                    DetectedObjects.Add(nearbyObject);
                }
            }

        }

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);
            DetectedObjects = new List<PlayerGPS.NearbyObject> { };
            AddQuestMarkerToTracked();
            DaggerfallUI.Instance.DaggerfallHUD.HUDCompass.RegisterDetector(this);

        }

        public override void End()
        {
            base.End();            
            DaggerfallUI.Instance.DaggerfallHUD.HUDCompass.DeregisterDetector(this);
            DetectedObjects.Clear();
        }

        public override void MagicRound()
        {
            base.MagicRound();
            AddQuestMarkerToTracked();
        }
    }
}
