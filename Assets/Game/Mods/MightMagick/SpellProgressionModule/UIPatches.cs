using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using MightyMagick.Formulas;

namespace Game.Mods.MightMagick.SpellProgressionModule
{
    public static class UIPatches
    {
        public static bool Prefix_AddHUDText(string message, float delay)
        {
            return message != "Press button to fire spell.";
        }
    }
}