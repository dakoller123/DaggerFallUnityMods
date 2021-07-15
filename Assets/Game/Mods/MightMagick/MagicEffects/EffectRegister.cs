using DaggerfallWorkshop.Game;

namespace MightyMagick.MagicEffects
{
    public class EffectRegister
    {
        private HealSpellPoints healSpellPoints;
        private ExtraSpellPts extraSpellPts;
        private DamageHealth damageHealth;
        public void RegisterNewMagicEffects()
        {
            healSpellPoints = new HealSpellPoints();
            extraSpellPts = new ExtraSpellPts();
            damageHealth = new DamageHealth();
            GameManager.Instance.EntityEffectBroker.RegisterEffectTemplate(healSpellPoints, true);
            GameManager.Instance.EntityEffectBroker.RegisterEffectTemplate(extraSpellPts, true);
            GameManager.Instance.EntityEffectBroker.RegisterEffectTemplate(damageHealth, true);
        }
    }
}