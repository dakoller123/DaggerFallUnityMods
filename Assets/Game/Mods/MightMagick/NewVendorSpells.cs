using System.Collections.Generic;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using MightyMagick.MagicEffects;
using LightNormal = DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects.LightNormal;
using MageArmor = MightyMagick.MagicEffects.MageArmor;
using Shield = DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects.Shield;

namespace MightyMagick
{
    public static class NewVendorSpells
    {
        private static void AddOffer(EffectBundleSettings spell, string key)
        {
            // Create a custom spell offer
            // This informs other systems if they can use this bundle
            EntityEffectBroker.CustomSpellBundleOffer offer = new EntityEffectBroker.CustomSpellBundleOffer()
            {
                Key = key,                           // This key is for the offer itself and must be unique
                Usage = EntityEffectBroker.CustomSpellBundleOfferUsage.SpellsForSale,              // Available in spells for sale
                BundleSetttings = spell,                         // The spell bundle created earlier
            };

            // Register the offer
            GameManager.Instance.EntityEffectBroker.RegisterCustomSpellBundleOffer(offer);
        }
        public static void RegisterSpells()
        {
            var settings = MightyMagickMod.Instance.MightyMagickModSettings.MagicEffectSettings;

            if (settings.CheaperShield) AddOffer(MinorShield(), "MinorShield-CustomOffer");
            HealMultipleStats(new List<BaseEntityEffect> {new HealIntelligence(), new HealWillpower(), new HealPersonality() }, "Heal Mental", "HealMental-CustomOffer");
            HealMultipleStats(new List<BaseEntityEffect> {new HealEndurance(), new HealStrength() }, "Heal Constitution", "HealConst-CustomOffer");
            HealMultipleStats(new List<BaseEntityEffect> {new HealAgility(), new HealSpeed(), new HealLuck() }, "Heal Nimbleness", "HealNimble-CustomOffer");
            Blink();
            if (settings.JumpingHasMagnitude) HopToad();
            if (settings.AddMageArmor) AddOffer(MinorMageArmor(), "MinorMageArmor-CustomOffer");
            if (settings.LevitateHasMagnitude) Flight();
            if (settings.AddDetectQuest) Clairvoyance();

            AddOffer(Firebolt(), "Firebolt-CustomOffer");
            AddOffer(Scrying(), "Scrying-CustomOffer");
            AddOffer(LocateObject(), "LocateObject-CustomOffer");
            AddOffer(LocateObject(), "Scrying-CustomOffer");
            AddOffer(DetectMagic(), "DetectMagic-CustomOffer");
        }

        private static EffectBundleSettings DetectMagic()
        {
            var templateEffect = new DetectMagic();

            EffectSettings effectSettings = new EffectSettings()
            {
                DurationBase = 10,
                DurationPlus = 1,
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
                Name = "Detect Magic",
                IconIndex = 12,
                Effects = new[] { effectEntry },
            };
            return spell;
        }
        private static EffectBundleSettings LocateObject()
        {
            var templateEffect = new DetectTreasure();

            EffectSettings effectSettings = new EffectSettings()
            {
                DurationBase = 10,
                DurationPlus = 1,
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
                Name = "Locate Object",
                IconIndex = 12,
                Effects = new[] { effectEntry },
            };
            return spell;
        }

        private static EffectBundleSettings Scrying()
        {
            var templateEffect = new DetectEnemy();

            EffectSettings effectSettings = new EffectSettings()
            {
                DurationBase = 10,
                DurationPlus = 1,
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
                Name = "Scrying",
                IconIndex = 12,
                Effects = new[] { effectEntry },
            };
            return spell;
        }
        private static EffectBundleSettings Firebolt()
        {
            var templateEffect = new DamageHealth();

            EffectSettings effectSettings = new EffectSettings()
            {
                MagnitudeBaseMin = 10,
                MagnitudeBaseMax = 25,
                MagnitudePerLevel = 1,
                MagnitudePlusMax = 0,
                MagnitudePlusMin = 0,
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
                TargetType = TargetTypes.SingleTargetAtRange,
                ElementType = ElementTypes.Fire,
                Name = "Firebolt",
                IconIndex = 12,
                Effects = new[] { effectEntry },
            };
            return spell;
        }

        private static void Blink()
        {
            var templateEffect = new InvisibilityTrue();

            EffectSettings effectSettings = new EffectSettings()
            {
                DurationBase = 2,
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
                Name = "Blink",
                IconIndex = 12,
                Effects = new[] { effectEntry },
            };

            // Create a custom spell offer
            // This informs other systems if they can use this bundle
            EntityEffectBroker.CustomSpellBundleOffer offer = new EntityEffectBroker.CustomSpellBundleOffer()
            {
                Key = "Blink-CustomOffer",                           // This key is for the offer itself and must be unique
                Usage = EntityEffectBroker.CustomSpellBundleOfferUsage.SpellsForSale,              // Available in spells for sale
                BundleSetttings = spell,                         // The spell bundle created earlier
            };

            // Register the offer
            GameManager.Instance.EntityEffectBroker.RegisterCustomSpellBundleOffer(offer);
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

        public static EffectBundleSettings MinorMageArmor()
        {
            var templateEffect = new MageArmor();

            EffectSettings effectSettings = new EffectSettings()
            {
                DurationBase = 5,
                MagnitudeBaseMin = 1,
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
                Effects = new[] { effectEntry },
            };
            return spell;
        }
        public static void HopToad()
        {
            var templateEffect = new MightyJumping();

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

            var effectEntry = new EffectEntry()
            {
                Key = templateEffect.Properties.Key,
                Settings = effectSettings,
            };

            var spell = new EffectBundleSettings()
            {
                Version = 1,
                BundleType = BundleTypes.Spell,
                TargetType = TargetTypes.CasterOnly,
                ElementType = ElementTypes.Magic,
                Name = "Tinur's Hoptoad",
                IconIndex = 12,
                Effects = new[] { effectEntry },
            };

            EntityEffectBroker.CustomSpellBundleOffer offer = new EntityEffectBroker.CustomSpellBundleOffer()
            {
                Key = "HopToad-CustomOffer",                           // This key is for the offer itself and must be unique
                Usage = EntityEffectBroker.CustomSpellBundleOfferUsage.SpellsForSale,              // Available in spells for sale
                BundleSetttings = spell,                         // The spell bundle created earlier
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
                        MagnitudeBaseMax = 5,
                        MagnitudePerLevel = 1,
                        MagnitudePlusMin = 1,
                        MagnitudePlusMax = 1,
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

        public static EffectBundleSettings MinorShield()
        {
            var templateEffect = new Shield();

            EffectSettings effectSettings = new EffectSettings()
            {
                DurationBase = 5,
                MagnitudeBaseMin = 10,
                MagnitudeBaseMax = 10,
                MagnitudePerLevel = 1,
                DurationPerLevel = 1
            };

            EffectEntry effectEntry = new EffectEntry()
            {
                Key = templateEffect.Properties.Key,
                Settings = effectSettings,
            };

            EffectBundleSettings minorShieldSpell = new EffectBundleSettings()
            {
                Version = 1,
                BundleType = BundleTypes.Spell,
                TargetType = TargetTypes.CasterOnly,
                ElementType = ElementTypes.Magic,
                Name = "Minor Shield",
                IconIndex = 12,
                Effects = new[] { effectEntry },
            };

            return minorShieldSpell;
        }

        public static EffectBundleSettings FieryTouch()
        {
            var templateEffect = new DamageHealth();

            EffectSettings effectSettings = new EffectSettings()
            {
                MagnitudeBaseMin = 1,
                MagnitudeBaseMax = 2,
                MagnitudePerLevel = 1,
                MagnitudePlusMax = 2,
                MagnitudePlusMin = 3,
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
                TargetType = TargetTypes.ByTouch,
                ElementType = ElementTypes.Fire,
                Name = "Fire Touch I",
                IconIndex = 12,
                Effects = new[] { effectEntry },
            };

            return spell;
        }

        public static EffectBundleSettings FireBoltUnleveled()
        {
            var templateEffect = new DamageHealth();

            EffectSettings effectSettings = new EffectSettings()
            {
                MagnitudeBaseMin = 1,
                MagnitudeBaseMax = 2,
                MagnitudePerLevel = 1,
                MagnitudePlusMax = 2,
                MagnitudePlusMin = 3,
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
                TargetType = TargetTypes.SingleTargetAtRange,
                ElementType = ElementTypes.Fire,
                Name = "Firebolt",
                IconIndex = 12,
                Effects = new[] { effectEntry },
            };

            return spell;
        }

        public static EffectBundleSettings Rise()
        {
            var templateEffect = new Levitate();

            EffectSettings effectSettings = new EffectSettings()
            {
                MagnitudeBaseMin = 1,
                MagnitudeBaseMax = 2,
                MagnitudePerLevel = 2,
                MagnitudePlusMax = 2,
                MagnitudePlusMin = 1,
                DurationPerLevel = 2,
                DurationBase = 1,
                DurationPlus = 1,
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
                Name = "Rise",
                IconIndex = 12,
                Effects = new[] { effectEntry },
            };

            return spell;
        }

        public static EffectBundleSettings Regenerate()
        {
            var templateEffect = new Regenerate();

            EffectSettings effectSettings = new EffectSettings()
            {
                MagnitudeBaseMin = 1,
                MagnitudeBaseMax = 2,
                MagnitudePerLevel = 1,
                MagnitudePlusMax = 11,
                MagnitudePlusMin = 10,
                DurationPerLevel = 1,
                DurationBase = 1,
                DurationPlus = 2,
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
                Name = "Regenerate",
                IconIndex = 12,
                Effects = new[] { effectEntry },
            };

            return spell;
        }

        public static EffectBundleSettings Light()
        {
            var spell = new EffectBundleSettings()
            {
                Version = 1,
                BundleType = BundleTypes.Spell,
                TargetType = TargetTypes.CasterOnly,
                ElementType = ElementTypes.Magic,
                Name = "Candlelight",
                IconIndex = 12,
                Effects = new[] { new EffectEntry()
                {
                    Key = LightNormal.EffectKey,
                    Settings = new EffectSettings()
                    {
                        DurationPerLevel = 2,
                        DurationBase = 3,
                        DurationPlus = 1,
                    }
                } },
            };
            return spell;
        }

        public static EffectBundleSettings Buyoancy()
        {
            var spell = new EffectBundleSettings()
            {
                Version = 1,
                BundleType = BundleTypes.Spell,
                TargetType = TargetTypes.CasterOnly,
                ElementType = ElementTypes.Magic,
                Name = "Buyoancy",
                IconIndex = 12,
                Effects = new[] { new EffectEntry()
                {
                    Key = WaterBreathing.EffectKey,
                    Settings = new EffectSettings()
                    {
                        DurationPerLevel = 1,
                        DurationBase = 1,
                        DurationPlus = 1,
                    }
                },new EffectEntry()
                {
                    Key = WaterWalking.EffectKey,
                    Settings = new EffectSettings()
                    {
                        DurationPerLevel = 1,
                        DurationBase = 1,
                        DurationPlus = 1,
                    }
                } },
            };
            return spell;
        }


    }
}