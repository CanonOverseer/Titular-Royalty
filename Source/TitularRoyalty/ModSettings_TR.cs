﻿using System.Collections.Generic;
using UnityEngine;
using Verse;
using SettingsHelper;

namespace TitularRoyalty
{
    public class TRSettings : ModSettings
    {
        public string realmType;
        public override void ExposeData()
        {
            Scribe_Values.Look(ref realmType, "realmType", "Kingdom");
            base.ExposeData();
        }
    }

    public class TitularRoyaltyMod : Mod
    {
        // A reference to our settings.
        TRSettings settings;

        // A mandatory constructor which resolves the reference to our settings.
        public TitularRoyaltyMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<TRSettings>();
        }

        public static string[] realmTypes = { "Kingdom", "Empire" };

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            listingStandard.AddLabeledRadioList($"Realm Type, default is Kingdom, all Empire does is change the King title to Emperor,\nMore Coming Soon!",
                                                 realmTypes, ref settings.realmType);
            //listingStandard.CheckboxLabeled("exampleBoolExplanation", ref settings.exampleBool, "exampleBoolToolTip");
            //listingStandard.Label("exampleFloatExplanation");
            //settings.exampleFloat = listingStandard.Slider(settings.exampleFloat, 100f, 300f);
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Titular Royalty"; //.Translate()
        }
    }
}
