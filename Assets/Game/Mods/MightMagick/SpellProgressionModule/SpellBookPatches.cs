using System;
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
            if (!SpellCostSkillChecker.CanSpellBeCast(___offeredSpells[___spellsListBox.SelectedIndex], GameManager.Instance.PlayerEntity))
            {
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(___uiManager, __instance);
                TextFile.Token token = new TextFile.Token();
                token.formatting = TextFile.Formatting.Text;
                token.text = "You are not skilled enough to learn this spell yet.";
                messageBox.SetTextTokens(new[] { token }, __instance);
                messageBox.ClickAnywhereToClose = true;
                ___uiManager.PushWindow(messageBox);
                return false;
            }

            return true;
        }
    }
}