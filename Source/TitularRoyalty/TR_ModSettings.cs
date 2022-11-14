﻿using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;
using SettingsHelper;
using HarmonyLib;

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

        public TRSettings Settings { get; private set; }
        public static TitularRoyaltyMod Instance { get; private set; }

        // A mandatory constructor which resolves the reference to our settings.
        public TitularRoyaltyMod(ModContentPack content) : base(content)
        {
            this.Settings = GetSettings<TRSettings>();
            
            // Singleton
            Instance = this;

            // Harmony Stuff
            var harmony = new Harmony("com.TitularRoyalty.patches");
            harmony.PatchAll();
        }

        public static string[] realmTypes = { "Kingdom", "Empire", "Roman", "Roman (Alt)" };

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            //Explains more about the titlesets
            listingStandard.Label("TR_selectorexplanation".Translate());
            listingStandard.AddHorizontalLine();

            //Radio list to choose titles
            listingStandard.AddLabeledRadioList("TR_baserealmtype".Translate(),
                                                 realmTypes, ref Settings.realmType);
            listingStandard.Gap(24);
            listingStandard.AddHorizontalLine();

            //Explains that you need to reload the save for changes to apply
            listingStandard.Gap(12);
            listingStandard.Label("TR_reloadsavefix".Translate());
            listingStandard.End();

            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "TR_modname".Translate();
        }


        // END SETTINGS
        //===================================

    }

}
