using DaggerfallWorkshop.Game;

namespace MightyMagick.MagicEffects
{
    public class EffectRegister
    {
        private HealSpellPoints healSpellPoints;
        private ExtraSpellPts extraSpellPts;

        public void RegisterNewMagicEffects()
        {
            healSpellPoints = new HealSpellPoints();
            extraSpellPts = new ExtraSpellPts();
            GameManager.Instance.EntityEffectBroker.RegisterEffectTemplate(healSpellPoints, true);
            GameManager.Instance.EntityEffectBroker.RegisterEffectTemplate(extraSpellPts, true);
        }
    }
}