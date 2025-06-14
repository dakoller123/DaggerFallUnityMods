using System;
using System.Collections.Generic;
using System.Linq;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using UnityEngine;

namespace MightyMagick.SpellProgressionModule
{
    public class MightyMagickSpellMakerWindow : DaggerfallSpellMakerWindow
    {
        private Dictionary<string, List<IEntityEffect>> groupedKnownEffectTemplates;
        public MightyMagickSpellMakerWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null) : base(uiManager, previous)
        {
        }

        private Dictionary<string, List<IEntityEffect>> GenerateKnownDictionary()
        {
            var result = new Dictionary<string, List<IEntityEffect>>();
            EffectBundleSettings[] spellbook = GameManager.Instance.PlayerEntity.GetSpells();

            foreach (EffectBundleSettings settings in spellbook)
            {
                foreach (var effectEntry in settings.Effects)
                {
                    var effectTemplate = GameManager.Instance.EntityEffectBroker.GetEffectTemplate(effectEntry.Key);
                    var groupName = effectTemplate.GroupName;

                    if (!result.ContainsKey(groupName))
                        result[groupName] = new List<IEntityEffect>();

                    if (!result[groupName].Contains(effectTemplate))
                        result[groupName].Add(effectTemplate);
                }
            }

            return result;
        }

        private string[] GetKnownGroupNames()
        {
            groupedKnownEffectTemplates = GenerateKnownDictionary();

            var groupNames =  groupedKnownEffectTemplates.Keys.ToList();
            groupNames.Sort();
            return groupNames.ToArray();
        }

        List<IEntityEffect> enumeratedEffectTemplates = new List<IEntityEffect>();
        protected override void AddEffectGroupListBox_OnUseSelectedItem()
        {
            // Clear existing
            effectSubGroupPicker.ListBox.ClearItems();
            enumeratedEffectTemplates.Clear();

            // Enumerate subgroup effect key name pairs
            enumeratedEffectTemplates = groupedKnownEffectTemplates[effectGroupPicker.ListBox.SelectedItem];
            if (enumeratedEffectTemplates.Count < 1)
                throw new Exception(string.Format("Could not find any effect templates for group {0}", effectGroupPicker.ListBox.SelectedItem));

            // If this is a solo effect without any subgroups names defined (e.g. "Regenerate") then go straight to effect editor
            if (enumeratedEffectTemplates.Count == 1 && string.IsNullOrEmpty(enumeratedEffectTemplates[0].SubGroupName))
            {
                effectGroupPicker.CloseWindow();
                AddAndEditSlot(enumeratedEffectTemplates[0]);
                //uiManager.PushWindow(effectEditor);
                return;
            }

            // Sort list by subgroup name
            enumeratedEffectTemplates.Sort((s1, s2) => s1.SubGroupName.CompareTo(s2.SubGroupName));

            // Populate subgroup names in list box
            foreach (IEntityEffect effect in enumeratedEffectTemplates)
            {
                effectSubGroupPicker.ListBox.AddItem(effect.SubGroupName);
            }
            effectSubGroupPicker.ListBox.SelectedIndex = 0;

            // Show effect subgroup picker
            // Note: In classic the group name is now shown (and mostly obscured) behind the picker at first available effect slot
            // This is not easily visible and not sure if this really communicates anything useful to user
            // Daggerfall Unity also allows user to cancel via escape back to previous dialog, so changing this beheaviour intentionally
            uiManager.PushWindow(effectSubGroupPicker);
        }

        protected override void AddEffectButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            const int noMoreThan3Effects = 1707;

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);

            // Must have a free effect slot
            if (GetFirstFreeEffectSlotIndex() == -1)
            {
                DaggerfallMessageBox mb = new DaggerfallMessageBox(
                    uiManager,
                    DaggerfallMessageBox.CommonMessageBoxButtons.Nothing,
                    DaggerfallUnity.Instance.TextProvider.GetRSCTokens(noMoreThan3Effects),
                    this);
                mb.ClickAnywhereToClose = true;
                mb.Show();
                return;
            }

            // Clear existing
            effectGroupPicker.ListBox.ClearItems();
            tipLabel.Text = string.Empty;

            // TODO: Filter out effects incompatible with any effects already added (e.g. incompatible target types)

            // Populate group names
            string[] groupNames = GetKnownGroupNames();
            effectGroupPicker.ListBox.AddItems(groupNames);
            effectGroupPicker.ListBox.SelectedIndex = 0;

            // Show effect group picker
            uiManager.PushWindow(effectGroupPicker);
        }
    }
}