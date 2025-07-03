using System.Collections.Generic;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using MightyMagick.MagicEffects;
using LightNormal = DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects.LightNormal;

namespace MightyMagick
{
    public static class AnducarsGrimoire
    {
        private static void AddOffer(EffectBundleSettings spell, string key)
        {
            // Create a custom spell offer
            // This informs other systems if they can use this bundle
            EntityEffectBroker.CustomSpellBundleOffer offer = new EntityEffectBroker.CustomSpellBundleOffer()
            {
                Key = $"Anducar-{key}-CustomOffer",                           // This key is for the offer itself and must be unique
                Usage = EntityEffectBroker.CustomSpellBundleOfferUsage.SpellsForSale,              // Available in spells for sale
                BundleSetttings = spell,                         // The spell bundle created earlier
            };

            // Register the offer
            GameManager.Instance.EntityEffectBroker.RegisterCustomSpellBundleOffer(offer);
        }
        public static void RegisterSpells()
        {
            var settings = MightyMagickMod.Instance.MightyMagickModSettings.MagicEffectSettings;

            if (!settings.AnducarsGrimoire) return;

            HealMultipleStats(new List<BaseEntityEffect> {new HealIntelligence(), new HealWillpower(), new HealPersonality() }, "Anducar's Heal Mental", "HealMental");
            HealMultipleStats(new List<BaseEntityEffect> {new HealEndurance(), new HealStrength() }, "Anducar's Heal Constitution", "HealConst");
            HealMultipleStats(new List<BaseEntityEffect> {new HealAgility(), new HealSpeed(), new HealLuck() }, "Anducar's Heal Nimbleness", "HealNimble");

            AddOffer(Firebolt(), nameof(Firebolt));
            AddOffer(FieryTouch(), nameof(FieryTouch));
            AddOffer(FireNova(), nameof(FireNova));
            AddOffer(Blink(), nameof(Blink));
            AddOffer(CurePoison(), nameof(CurePoison));
            AddOffer(CureDisease(), nameof(CureDisease));
            AddOffer(Rise(), nameof(Rise));
            AddOffer(Regenerate(), nameof(Regenerate));
            AddOffer(Light(), nameof(Light));
            AddOffer(Buoyancy(), nameof(Buoyancy));
            AddOffer(Frostbolt(), nameof(Frostbolt));
            AddOffer(Stamina(), nameof(Stamina));
            AddOffer(Teleport(), nameof(Teleport));
            AddOffer(FreeAction(), nameof(FreeAction));
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
                        MagnitudeBaseMax = 2,
                        MagnitudePerLevel = 2,
                        MagnitudePlusMin = 1,
                        MagnitudePlusMax = 2,
                    },
                });
            }

            var offer = new EntityEffectBroker.CustomSpellBundleOffer()
            {
                Key = key,                           // This key is for the offer itself and must be unique
                Usage = EntityEffectBroker.CustomSpellBundleOfferUsage.SpellsForSale,              // Available in spells for sale
                BundleSetttings = new EffectBundleSettings()
                {
                    Version = 1,
                    BundleType = BundleTypes.Spell,
                    TargetType = TargetTypes.CasterOnly,
                    ElementType = ElementTypes.Magic,
                    Name = name,
                    IconIndex = 12,
                    Effects = effectEntries.ToArray(),
                },                         // The spell bundle created earlier
            };

            GameManager.Instance.EntityEffectBroker.RegisterCustomSpellBundleOffer(offer);
        }

        public static EffectBundleSettings Teleport()
        {
            return new EffectBundleSettings()
            {
                Version = 1,
                BundleType = BundleTypes.Spell,
                TargetType = TargetTypes.CasterOnly,
                ElementType = ElementTypes.Magic,
                Name = "Anducar's Teleport",
                IconIndex = 12,
                Effects = new[] { new EffectEntry()
                {
                    Key = new Teleport().Properties.Key,
                    Settings = new EffectSettings()
                    {
                    },
                } },
            };
        }
        public static EffectBundleSettings FreeAction()
        {
            return new EffectBundleSettings()
            {
                Version = 1,
                BundleType = BundleTypes.Spell,
                TargetType = TargetTypes.CasterOnly,
                ElementType = ElementTypes.Magic,
                Name = "Anducar's Free Action",
                IconIndex = 12,
                Effects = new[] { new EffectEntry()
                {
                    Key = new FreeAction().Properties.Key,
                    Settings = new EffectSettings()
                    {
                        DurationBase = 1,
                        DurationPerLevel = 1,
                        DurationPlus = 2
                    },
                } },
            };
        }
        public static EffectBundleSettings Stamina()
        {
            return new EffectBundleSettings()
            {
                Version = 1,
                BundleType = BundleTypes.Spell,
                TargetType = TargetTypes.CasterOnly,
                ElementType = ElementTypes.Magic,
                Name = "Anducar's Stamina",
                IconIndex = 12,
                Effects = new[] { new EffectEntry()
                {
                    Key = new HealFatigue().Properties.Key,
                    Settings = new EffectSettings()
                    {
                        MagnitudeBaseMin = 10,
                        MagnitudeBaseMax = 11,
                        MagnitudePerLevel = 2,
                        MagnitudePlusMax = 1,
                        MagnitudePlusMin = 2,
                    },
                } },
            };
        }
        public static EffectBundleSettings Frostbolt()
        {
            return new EffectBundleSettings()
            {
                Version = 1,
                BundleType = BundleTypes.Spell,
                TargetType = TargetTypes.SingleTargetAtRange,
                ElementType = ElementTypes.Cold,
                Name = "Anducar's Frostbolt",
                IconIndex = 12,
                Effects = new[] { new EffectEntry()
                {
                    Key = new DamageHealth().Properties.Key,
                    Settings = new EffectSettings()
                    {
                        MagnitudeBaseMin = 1,
                        MagnitudeBaseMax = 2,
                        MagnitudePerLevel = 1,
                        MagnitudePlusMax = 2,
                        MagnitudePlusMin = 3,
                    },
                } },
            };
        }
        public static EffectBundleSettings FieryTouch()
        {
            return new EffectBundleSettings()
            {
                Version = 1,
                BundleType = BundleTypes.Spell,
                TargetType = TargetTypes.ByTouch,
                ElementType = ElementTypes.Fire,
                Name = "Anducar's Fiery Touch",
                IconIndex = 12,
                Effects = new[] { new EffectEntry()
                {
                    Key = new DamageHealth().Properties.Key,
                    Settings = new EffectSettings()
                    {
                        MagnitudeBaseMin = 1,
                        MagnitudeBaseMax = 2,
                        MagnitudePerLevel = 1,
                        MagnitudePlusMax = 2,
                        MagnitudePlusMin = 3,
                    },
                } },
            };
        }

        public static EffectBundleSettings FireNova()
        {
            return new EffectBundleSettings()
            {
                Version = 1,
                BundleType = BundleTypes.Spell,
                TargetType = TargetTypes.AreaAroundCaster,
                ElementType = ElementTypes.Fire,
                Name = "Anducar's Fire Nova",
                IconIndex = 12,
                Effects = new[] { new EffectEntry()
                {
                    Key = new DamageHealth().Properties.Key,
                    Settings = new EffectSettings()
                    {
                        MagnitudeBaseMin = 1,
                        MagnitudeBaseMax = 2,
                        MagnitudePerLevel = 1,
                        MagnitudePlusMax = 2,
                        MagnitudePlusMin = 3,
                    },
                } },
            };
        }

        public static EffectBundleSettings Blink()
        {
            return new EffectBundleSettings()
            {
                Version = 1,
                BundleType = BundleTypes.Spell,
                TargetType = TargetTypes.CasterOnly,
                ElementType = ElementTypes.Magic,
                Name = "Anducar's Blink",
                IconIndex = 12,
                Effects = new[] { new EffectEntry()
                {
                    Key = new InvisibilityTrue().Properties.Key,
                    Settings = new EffectSettings()
                    {
                        DurationBase = 1,
                        DurationPerLevel = 2,
                        DurationPlus = 1
                    },
                } },
            };
        }

        public static EffectBundleSettings CurePoison()
        {
            return new EffectBundleSettings()
            {
                Version = 1,
                BundleType = BundleTypes.Spell,
                TargetType = TargetTypes.CasterOnly,
                ElementType = ElementTypes.Magic,
                Name = "Anducar's Cleanse",
                IconIndex = 12,
                Effects = new[] { new EffectEntry()
                {
                    Key = new CurePoison().Properties.Key,
                    Settings = new EffectSettings()
                    {
                        ChanceBase = 20,
                        ChancePerLevel = 2,
                        ChancePlus = 1
                    }
                }}
            };
        }

        public static EffectBundleSettings CureDisease()
        {
            return new EffectBundleSettings()
            {
                Version = 1,
                BundleType = BundleTypes.Spell,
                TargetType = TargetTypes.CasterOnly,
                ElementType = ElementTypes.Magic,
                Name = "Anducar's Cure",
                IconIndex = 12,
                Effects = new[] { new EffectEntry()
                {
                    Key = new CureDisease().Properties.Key,
                    Settings = new EffectSettings()
                    {
                        ChanceBase = 20,
                        ChancePerLevel = 2,
                        ChancePlus = 1
                    }
                }}
            };
        }

        public static EffectBundleSettings Firebolt()
        {
            return new EffectBundleSettings()
            {
                Version = 1,
                BundleType = BundleTypes.Spell,
                TargetType = TargetTypes.SingleTargetAtRange,
                ElementType = ElementTypes.Fire,
                Name = "Anducar's Firebolt",
                IconIndex = 12,
                Effects = new[] { new EffectEntry()
                {
                    Key = new DamageHealth().Properties.Key,
                    Settings = new EffectSettings()
                    {
                        MagnitudeBaseMin = 1,
                        MagnitudeBaseMax = 2,
                        MagnitudePerLevel = 1,
                        MagnitudePlusMax = 2,
                        MagnitudePlusMin = 3,
                    }
                }}
            };
        }

        public static EffectBundleSettings Rise()
        {
            return new EffectBundleSettings()
            {
                Version = 1,
                BundleType = BundleTypes.Spell,
                TargetType = TargetTypes.CasterOnly,
                ElementType = ElementTypes.Magic,
                Name = "Anducar's Rise",
                IconIndex = 12,
                Effects = new[] { new EffectEntry()
                {
                    Key = new MightyLevitate().Properties.Key,
                    Settings = new EffectSettings()
                    {
                        MagnitudeBaseMin = 1,
                        MagnitudeBaseMax = 2,
                        MagnitudePerLevel = 2,
                        MagnitudePlusMax = 2,
                        MagnitudePlusMin = 1,
                        DurationPerLevel = 2,
                        DurationBase = 1,
                        DurationPlus = 1,
                    }
                }}
            };
        }

        public static EffectBundleSettings Regenerate()
        {
            return new EffectBundleSettings()
            {
                Version = 1,
                BundleType = BundleTypes.Spell,
                TargetType = TargetTypes.CasterOnly,
                ElementType = ElementTypes.Magic,
                Name = "Anducar's Regenerate",
                IconIndex = 12,
                Effects = new[] { new EffectEntry()
                {
                    Key = new Regenerate().Properties.Key,
                    Settings = new EffectSettings()
                    {
                        MagnitudeBaseMin = 1,
                        MagnitudeBaseMax = 2,
                        MagnitudePerLevel = 1,
                        MagnitudePlusMax = 11,
                        MagnitudePlusMin = 10,
                        DurationPerLevel = 1,
                        DurationBase = 1,
                        DurationPlus = 2,
                    }
                }}
            };
        }

        public static EffectBundleSettings Light()
        {
            var spell = new EffectBundleSettings()
            {
                Version = 1,
                BundleType = BundleTypes.Spell,
                TargetType = TargetTypes.CasterOnly,
                ElementType = ElementTypes.Magic,
                Name = "Anducar's Candelight",
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

        public static EffectBundleSettings Buoyancy()
        {
            var spell = new EffectBundleSettings()
            {
                Version = 1,
                BundleType = BundleTypes.Spell,
                TargetType = TargetTypes.CasterOnly,
                ElementType = ElementTypes.Magic,
                Name = "Anducar's Buoyancy",
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