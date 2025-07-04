
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using Vector3 = UnityEngine.Vector3;

namespace MightyMagick.MagicEffects
{
    public class LightNormal : IncumbentEffect
    {
        public static readonly string EffectKey = "Light";

        bool lightStarted = false;
        MagicCandleBehaviour magicCandle = null;
        bool showCandle = false;

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(15, 255);
            properties.SupportDuration = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_Self;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Illusion;
            properties.DurationCosts = MakeEffectCosts(8, 40);
        }

        public override string GroupName => TextManager.Instance.GetLocalizedText("light");
        public override TextFile.Token[] SpellMakerDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1563);
        public override TextFile.Token[] SpellBookDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1263);

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);
            StartLight();
        }

        public override void Resume(EntityEffectManager.EffectSaveData_v1 effectData, EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Resume(effectData, manager, caster);
            StartLight();
        }

        public override void End()
        {
            base.End();
            EndLight();
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return other is LightNormal;
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            // Stack my rounds onto incumbent
            incumbent.RoundsRemaining += RoundsRemaining;
            lightStarted = true;
        }

        void StartLight()
        {
            if (lightStarted)
                return;

            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            if (entityBehaviour == GameManager.Instance.PlayerEntityBehaviour)
                StartMagicCandle();

            lightStarted = true;
        }

        // Classic "magic candle" for player object only
        void StartMagicCandle()
        {
            const float candleDistance = 1.4f;

            // Create candle position out in front of player - candle is intentionally placed closer to player than classic
            // Classic is more like 4.5 units which is often on other side of walls (especially in dungeons), reducing usefulness
            // Ideally the candle would have a spring setup to push it away from collisions and smoothly move back into place
            // Just implementing similar to classic for now but at closer range
            UnityEngine.Vector3 candlePosition = GameManager.Instance.PlayerObject.transform.position + GameManager.Instance.PlayerObject.transform.forward * candleDistance;
            candlePosition.y += GameManager.Instance.PlayerController.height * 0.25f;

            // Instantiate magic candle prefab
            UnityEngine.GameObject candleObject = UnityEngine.Object.Instantiate(
                UnityEngine.Resources.Load<UnityEngine.GameObject>("MagicCandle"),
                candlePosition,
                UnityEngine.Quaternion.identity,
                GameManager.Instance.PlayerObject.transform);


                // candleObject.transform.localScale = new Vector3(0, 0, 0);
            // Get behaviour script
            if (!candleObject) return;
            magicCandle = candleObject.GetComponent<MagicCandleBehaviour>();
            if (showCandle) return;
            magicCandle.enabled = false;
            // candleObject.transform.localScale = new Vector3(0, 0, 0);




            // magicCandle.enabled = false;
            // if (!showCandle)
            //     magicCandle.DestroyCandle();
        }

        void EndLight()
        {
            if (magicCandle != null)
                magicCandle.DestroyCandle();
        }
    }
}
