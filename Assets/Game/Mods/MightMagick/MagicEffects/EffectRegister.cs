using DaggerfallWorkshop.Game;

namespace MightyMagick.MagicEffects
{
    public class EffectRegister
    {
        private HealSpellPoints healSpellPoints;
        private ExtraSpellPts extraSpellPts;
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
            
        }
    }
}