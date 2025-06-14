using System.Collections.Generic;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using UnityEngine;

namespace Game.Mods.MightMagick.SpellProgressionModule
{
    public class SpellBookPatches
    {
        public static bool Prefix_BuyButton_OnMouseClick(DaggerfallSpellBookWindow __instance, List<EffectBundleSettings> ___offeredSpells, IUserInterfaceManager ___uiManager, ListBox ___spellsListBox, BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.AddHUDText($"BuyButton_OnMouseClick ${___offeredSpells[___spellsListBox.SelectedIndex].Name}");
            Debug.Log($"BuyButton_OnMouseClick ${___offeredSpells[___spellsListBox.SelectedIndex].Name}");
            if (!SpellCostSkillChecker.CanSpellBeCast(___offeredSpells[___spellsListBox.SelectedIndex], GameManager.Instance.PlayerEntity))
            {
                Debug.Log($"BuyButton_OnMouseClick ${___offeredSpells[___spellsListBox.SelectedIndex].Name}");
                DaggerfallUI.AddHUDText($"BuyButton_OnMouseClick ${___offeredSpells[___spellsListBox.SelectedIndex].Name}");
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(___uiManager, __instance);
                TextFile.Token token = new TextFile.Token();
                token.text = "Not skilled enough to cast this yet.";
                messageBox.SetTextTokens(new[] { token }, __instance);
                ___uiManager.PushWindow(messageBox);
                return false;
            }

            return true;
        }
    }
}