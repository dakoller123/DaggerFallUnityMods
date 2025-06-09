using DaggerfallWorkshop.Game.UserInterfaceWindows;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility;
using DaggerfallConnect.Save;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game;
using System.Linq;

namespace MightyMagick
{
    public class MightyMagicSpellBookWindow : DaggerfallSpellBookWindow
    {
        public MightyMagicSpellBookWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null, bool buyMode = false)
                : base(uiManager, previous, buyMode) {}
            
        protected override void LoadSpellsForSale()
        {
            // Load spells for sale
            offeredSpells.Clear();

            var effectBroker = GameManager.Instance.EntityEffectBroker;

            IEnumerable<SpellRecord.SpellRecordData> standardSpells = effectBroker.StandardSpells;
            if (standardSpells == null || standardSpells.Count() == 0)
            {
                Debug.LogError("Failed to load SPELLS.STD for spellbook in buy mode.");
                return;
            }

            // Add standard spell bundles to offer
            foreach(SpellRecord.SpellRecordData standardSpell in standardSpells)
            {
                // Filter internal spells starting with exclamation point '!'
                if (standardSpell.spellName.StartsWith("!"))
                    continue;

                // NOTE: Classic allows purchase of duplicate spells
                // If ever changing this, must ensure spell is an *exact* duplicate (i.e. not a custom spell with same name)
                // Just allowing duplicates for now as per classic and let user manage preference

                // Get effect bundle settings from classic spell
                EffectBundleSettings bundle;
                if (!effectBroker.ClassicSpellRecordDataToEffectBundleSettings(standardSpell, BundleTypes.Spell, out bundle))
                    continue;

                // Store offered spell and add to list box
                offeredSpells.Add(bundle);
            }

            // Add custom spells for sale bundles to list of offered spells
            offeredSpells.AddRange(effectBroker.GetCustomSpellBundles(EntityEffectBroker.CustomSpellBundleOfferUsage.SpellsForSale));

            // Sort spells for easier finding
            offeredSpells = offeredSpells.Where(x => x.Name.Equals("Recall")).OrderBy(x => x.Name).ToList();
        }
    }

    
}
