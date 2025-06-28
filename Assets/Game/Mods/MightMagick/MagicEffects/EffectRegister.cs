using DaggerfallWorkshop.Game;

namespace MightyMagick.MagicEffects
{
    public class EffectRegister
    {
        public void RegisterNewMagicEffects()
        {
            var settings = MightyMagickMod.Instance.MightyMagickModSettings;

            if (settings.PotionSettings.Enabled)
            {
                GameManager.Instance.EntityEffectBroker.RegisterEffectTemplate(new HealSpellPoints(), true);
            }

            if (settings.MagickaEnchantSettings.Enabled)
            {
                GameManager.Instance.EntityEffectBroker.RegisterEffectTemplate(new ExtraSpellPts(), true);
            }

            if (settings.MagicEffectSettings.HideMagicCandle)
            {
                GameManager.Instance.EntityEffectBroker.RegisterEffectTemplate(new LightNormal(), true);
            }

            if (settings.MagicEffectSettings.JumpingHasMagnitude)
            {
                GameManager.Instance.EntityEffectBroker.RegisterEffectTemplate(new MightyJumping(), true);
            }

            if (settings.MagicEffectSettings.AddMageArmor)
            {
                GameManager.Instance.EntityEffectBroker.RegisterEffectTemplate(new MageArmor(), true);
            }

            if (settings.MagicEffectSettings.CheaperShield)
            {
                GameManager.Instance.EntityEffectBroker.RegisterEffectTemplate(new Shield(), true);
            }

            if (settings.MagicEffectSettings.LevitateHasMagnitude)
            {
                GameManager.Instance.EntityEffectBroker.RegisterEffectTemplate(new MightyLevitate(), true);
            }

            if (settings.MagicEffectSettings.AddDetectQuest)
            {
                GameManager.Instance.EntityEffectBroker.RegisterEffectTemplate(new DetectQuest(), true);
            }
        }
    }
}