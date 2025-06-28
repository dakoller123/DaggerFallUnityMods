using System.Collections.Generic;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using MightyMagick.MagicEffects;
using Shield = MightyMagick.MagicEffects.Shield;
using MageArmor = MightyMagick.MagicEffects.MageArmor;

namespace MightyMagick
{
    public static class NewVendorSpells
    {
        public static void RegisterSpells()
        {
            var settings = MightyMagickMod.Instance.MightyMagickModSettings.MagicEffectSettings;

            if (settings.CheaperShield) MinorShield();
            HealMultipleStats(new List<BaseEntityEffect> {new HealIntelligence(), new HealWillpower(), new HealPersonality() }, "Heal Mental", "HealMental-CustomOffer");
            HealMultipleStats(new List<BaseEntityEffect> {new HealEndurance(), new HealStrength() }, "Heal Constitution", "HealConst-CustomOffer");
            HealMultipleStats(new List<BaseEntityEffect> {new HealAgility(), new HealSpeed(), new HealLuck() }, "Heal Nimbleness", "HealNimble-CustomOffer");

            if (settings.JumpingHasMagnitude) HopToad();
            if (settings.AddMageArmor) MinorMageArmor();
            if (settings.LevitateHasMagnitude) Flight();
            if (settings.AddDetectQuest) Clairvoyance();
        }

        private static void Clairvoyance()
        {
            var templateEffect = new DetectQuest();

            EffectSettings effectSettings = new EffectSettings()
            {
                DurationBase = 10,
                DurationPerLevel = 1
            };

            EffectEntry effectEntry = new EffectEntry()
            {
                Key = templateEffect.Properties.Key,
                Settings = effectSettings,
            };

            EffectBundleSettings spell = new EffectBundleSettings()
            {
                Version = 1,
                BundleType = BundleTypes.Spell,
                TargetType = TargetTypes.CasterOnly,
                ElementType = ElementTypes.Magic,
                Name = "Clairvoyance",
                IconIndex = 12,
                Effects = new[] { effectEntry },
            };

            EntityEffectBroker.CustomSpellBundleOffer offer = new EntityEffectBroker.CustomSpellBundleOffer()
            {
                Key = "Clairvoyance-CustomOffer",                           // This key is for the offer itself and must be unique
                Usage = EntityEffectBroker.CustomSpellBundleOfferUsage.SpellsForSale,              // Available in spells for sale
                BundleSetttings = spell,                         // The spell bundle created earlier
            };

            // Register the offer
            GameManager.Instance.EntityEffectBroker.RegisterCustomSpellBundleOffer(offer);
        }
        private static void Flight()
        {
            var templateEffect = new MightyLevitate();

            EffectSettings effectSettings = new EffectSettings()
            {
                DurationBase = 2,
                MagnitudeBaseMin = 40,
                MagnitudeBaseMax = 40,
                MagnitudePerLevel = 1,
                DurationPerLevel = 1
            };

            EffectEntry effectEntry = new EffectEntry()
            {
                Key = templateEffect.Properties.Key,
                Settings = effectSettings,
            };

            EffectBundleSettings spell = new EffectBundleSettings()
            {
                Version = 1,
                BundleType = BundleTypes.Spell,
                TargetType = TargetTypes.CasterOnly,
                ElementType = ElementTypes.Magic,
                Name = "Flight",
                IconIndex = 12,
                Effects = new[] { effectEntry },
            };

            // Create a custom spell offer
            // This informs other systems if they can use this bundle
            EntityEffectBroker.CustomSpellBundleOffer offer = new EntityEffectBroker.CustomSpellBundleOffer()
            {
                Key = "Flight-CustomOffer",                           // This key is for the offer itself and must be unique
                Usage = EntityEffectBroker.CustomSpellBundleOfferUsage.SpellsForSale,              // Available in spells for sale
                BundleSetttings = spell,                         // The spell bundle created earlier
            };

            // Register the offer
            GameManager.Instance.EntityEffectBroker.RegisterCustomSpellBundleOffer(offer);
        }

        public static void MinorMageArmor()
        {
            var templateEffect = new MageArmor();

            EffectSettings effectSettings = new EffectSettings()
            {
                DurationBase = 5,
                MagnitudeBaseMin = 3,
                MagnitudeBaseMax = 3,
                MagnitudePerLevel = 1,
                DurationPerLevel = 1
            };

            EffectEntry effectEntry = new EffectEntry()
            {
                Key = templateEffect.Properties.Key,
                Settings = effectSettings,
            };

            EffectBundleSettings spell = new EffectBundleSettings()
            {
                Version = 1,
                BundleType = BundleTypes.Spell,
                TargetType = TargetTypes.CasterOnly,
                ElementType = ElementTypes.Magic,
                Name = "Minor Mage Armor",
                IconIndex = 12,
                Effects = new EffectEntry[] { effectEntry },
            };

            // Create a custom spell offer
            // This informs other systems if they can use this bundle
            EntityEffectBroker.CustomSpellBundleOffer offer = new EntityEffectBroker.CustomSpellBundleOffer()
            {
                Key = "MinorMageArmor-CustomOffer",                           // This key is for the offer itself and must be unique
                Usage = EntityEffectBroker.CustomSpellBundleOfferUsage.SpellsForSale,              // Available in spells for sale
                BundleSetttings = spell,                         // The spell bundle created earlier
            };

            // Register the offer
            GameManager.Instance.EntityEffectBroker.RegisterCustomSpellBundleOffer(offer);
        }
        public static void HopToad()
        {
            var templateEffect = new MightyJumping();

            // Create effect settings for our custom spell
            // These are Chance, Duration, and Magnitude required by spell - usually seen in spellmaker
            // No need to define settings not used by effect
            // For our custom spell, we're using same Duration settings as Light spell: 1 + 4 per level
            // Note these settings will also control final cost of spell to buy and cast
            var effectSettings = new EffectSettings()
            {
                DurationBase = 1,
                DurationPlus = 0,
                DurationPerLevel = 1,
                MagnitudeBaseMin = 20,
                MagnitudeBaseMax = 20,
                MagnitudePerLevel = 1,
                MagnitudePlusMin = 0,
                MagnitudePlusMax = 0,
            };

            // Create an EffectEntry
            // This links the effect key with settings
            // Each effect entry in bundle needs its own settings - most spells only have a single effect
            var effectEntry = new EffectEntry()
            {
                Key = templateEffect.Properties.Key,
                Settings = effectSettings,
            };

            // Create a custom spell bundle
            // This is a portable version of the spell for other systems
            // For example, every spell in the player's spellbook is a bundle
            // Bundle target and elements settings should follow effect requirements
            var minorShieldSpell = new EffectBundleSettings()
            {
                Version = 1,
                BundleType = BundleTypes.Spell,
                TargetType = TargetTypes.CasterOnly,
                ElementType = ElementTypes.Magic,
                Name = "Tinur's Hoptoad",
                IconIndex = 12,
                Effects = new[] { effectEntry },
            };

            // Create a custom spell offer
            // This informs other systems if they can use this bundle
            EntityEffectBroker.CustomSpellBundleOffer offer = new EntityEffectBroker.CustomSpellBundleOffer()
            {
                Key = "HopToad-CustomOffer",                           // This key is for the offer itself and must be unique
                Usage = EntityEffectBroker.CustomSpellBundleOfferUsage.SpellsForSale,              // Available in spells for sale
                BundleSetttings = minorShieldSpell,                         // The spell bundle created earlier
            };

            // Register the offer
            GameManager.Instance.EntityEffectBroker.RegisterCustomSpellBundleOffer(offer);
        }

        public static void HealMultipleStats(List<BaseEntityEffect> effects, string name, string key)
        {
            var effectEntries = new List<EffectEntry>();
            foreach (var effect in effects)
            {
                effectEntries.Add(new EffectEntry()
                {
                    Key = effect.Properties.Key,
                    Settings = new EffectSettings()
                    {
                        MagnitudeBaseMin = 1,
                        MagnitudeBaseMax = 5
                    },
                });
            }

            EffectBundleSettings bundleSettings = new EffectBundleSettings()
            {
                Version = 1,
                BundleType = BundleTypes.Spell,
                TargetType = TargetTypes.CasterOnly,
                ElementType = ElementTypes.Magic,
                Name = name,
                IconIndex = 12,
                Effects = effectEntries.ToArray(),
            };

            EntityEffectBroker.CustomSpellBundleOffer offer = new EntityEffectBroker.CustomSpellBundleOffer()
            {
                Key = key,                           // This key is for the offer itself and must be unique
                Usage = EntityEffectBroker.CustomSpellBundleOfferUsage.SpellsForSale,              // Available in spells for sale
                BundleSetttings = bundleSettings,                         // The spell bundle created earlier
            };

            GameManager.Instance.EntityEffectBroker.RegisterCustomSpellBundleOffer(offer);
        }

        public static void MinorShield()
        {
            // This method is an example of how to create a fully custom spell bundle
            // and expose it to other systems like spells for sale and item enchanter
            // The process is mostly just setting up data, something that can be automated with helpers

            // First register custom effect with broker
            // This will make it available to crafting stations supported by effect
            // We're using variant 0 of this effect here (Inferno)
            var templateEffect = new Shield();

            // Create effect settings for our custom spell
            // These are Chance, Duration, and Magnitude required by spell - usually seen in spellmaker
            // No need to define settings not used by effect
            // For our custom spell, we're using same Duration settings as Light spell: 1 + 4 per level
            // Note these settings will also control final cost of spell to buy and cast
            EffectSettings effectSettings = new EffectSettings()
            {
                DurationBase = 5,
                MagnitudeBaseMin = 5,
                MagnitudeBaseMax = 5,
                MagnitudePerLevel = 1,
                DurationPerLevel = 1
            };

            // Create an EffectEntry
            // This links the effect key with settings
            // Each effect entry in bundle needs its own settings - most spells only have a single effect
            EffectEntry effectEntry = new EffectEntry()
            {
                Key = templateEffect.Properties.Key,
                Settings = effectSettings,
            };

            // Create a custom spell bundle
            // This is a portable version of the spell for other systems
            // For example, every spell in the player's spellbook is a bundle
            // Bundle target and elements settings should follow effect requirements
            EffectBundleSettings minorShieldSpell = new EffectBundleSettings()
            {
                Version = 1,
                BundleType = BundleTypes.Spell,
                TargetType = TargetTypes.CasterOnly,
                ElementType = ElementTypes.Magic,
                Name = "Minor Shield",
                IconIndex = 12,
                Effects = new EffectEntry[] { effectEntry },
            };

            // Create a custom spell offer
            // This informs other systems if they can use this bundle
            EntityEffectBroker.CustomSpellBundleOffer minorShieldOffer = new EntityEffectBroker.CustomSpellBundleOffer()
            {
                Key = "MinorShield-CustomOffer",                           // This key is for the offer itself and must be unique
                Usage = EntityEffectBroker.CustomSpellBundleOfferUsage.SpellsForSale,              // Available in spells for sale
                BundleSetttings = minorShieldSpell,                         // The spell bundle created earlier
            };

            // Register the offer
            GameManager.Instance.EntityEffectBroker.RegisterCustomSpellBundleOffer(minorShieldOffer);
        }
    }
}