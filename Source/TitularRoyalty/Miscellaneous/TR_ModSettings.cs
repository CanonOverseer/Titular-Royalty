﻿using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;
using SettingsHelper;
using HarmonyLib;
using UnityEngine.UIElements;
using System.Linq;
using System;

namespace TitularRoyalty
{
    public class TRSettings : ModSettings
    {
        public bool inheritanceEnabled;
        public bool clothingQualityRequirements;
        public bool titlesGivePermitPoints;

        public bool SovietModEnabled => ModLister.HasActiveModWithName("Titular Royalty - Soviet Revolution");

        public override void ExposeData()
        {
            Scribe_Values.Look(ref inheritanceEnabled, "inheritanceEnabled", true);
            Scribe_Values.Look(ref clothingQualityRequirements, "clothingQualityRequirements", true);
            Scribe_Values.Look(ref titlesGivePermitPoints, "titlesGivePermitPoints", true);
            base.ExposeData();
        }
    }

    public class TitularRoyaltyMod : Mod
    {

        public static TRSettings Settings { get; private set; }

        // A mandatory constructor which resolves the reference to our settings.
        public TitularRoyaltyMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<TRSettings>();

            // Harmony Stuff
            var harmony = new Harmony("com.TitularRoyalty.patches");
            harmony.PatchAll();
        }

        // Realm Types
        private static RealmTypeDef[] _realmTypes;
        public static RealmTypeDef[] RealmTypes
        {
            get
            {
                return _realmTypes ??= DefDatabase<RealmTypeDef>.AllDefsListForReading.ToArray();
            }
        }
        private static Dictionary<string, string> realmTypeLabels;
        public static Dictionary<string, string> RealmTypeLabels
        {
            get
            {
                return realmTypeLabels ??= RealmTypes.ToDictionary(x => x.label, x => x.defName);
            }
        }

        //Name that shows at the top
        public override string SettingsCategory() => "TR_modname".Translate();

        //Main Rendering
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.AddHorizontalLine();

            //Miscellanous Toggles
            listingStandard.Gap(12);

            Rect miscOptionsTitleRect = listingStandard.GetRect(32);
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(miscOptionsTitleRect, "TR_miscoptionstitle".Translate());
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            listingStandard.Gap(12);

            //First row of checkbox options
            Listing_Standard Checkboxes = listingStandard.GetRect(24).BeginListingStandard(3);
            Checkboxes.CheckboxLabeled("TR_checkbox_vanillainheritance".Translate(), ref Settings.inheritanceEnabled);
            Checkboxes.CheckboxLabeled("TR_checkbox_needsclothesquality".Translate(), ref Settings.clothingQualityRequirements);
            Checkboxes.CheckboxLabeled("TR_checkbox_titlegivespermitpoints".Translate(), ref Settings.titlesGivePermitPoints);
            Checkboxes.End();

            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        // END SETTINGS
        //===================================

    }

}
