﻿//using System; 
using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;

namespace TitularRoyalty
{
    public class GameComponent_TitularRoyalty : GameComponent
    {
        public GameComponent_TitularRoyalty(Game game) { } // Needs this or else Rimworld throws a fit and errors.

        private string realmTypeDefName;
        public string RealmTypeDefName
        {
            get
            {
                return realmTypeDefName ??= "RealmType_Kingdom";
            }
            set
            {
                realmTypeDefName = value;
                realmTypeDef = null;
            }
        }
        
        private RealmTypeDef realmTypeDef;
        public RealmTypeDef RealmTypeDef
        {
            get
            {
                return realmTypeDef ??= DefDatabase<RealmTypeDef>.GetNamed(RealmTypeDefName, false);
            }
        }

        // Get the Titles List in Order and Cache it
        private static List<PlayerTitleDef> titlesBySeniority;
        public static List<PlayerTitleDef> TitlesBySeniority
        {
            get
            {
                return titlesBySeniority ??= DefDatabase<PlayerTitleDef>.AllDefsListForReading.OrderBy(x => x.seniority).ToList();
            }
        }

        // Custom Titles
        private Dictionary<PlayerTitleDef, RoyalTitleOverride> customTitles;
        public Dictionary<PlayerTitleDef, RoyalTitleOverride> CustomTitles
        {
            get
            {
                return customTitles ??= TitlesBySeniority.ToDictionary(x => x, x => new RoyalTitleOverride());
            }
            private set
            {
                customTitles = value;
            }
        }

        // Required for ExposeData
        private List<PlayerTitleDef> customTitles_List1;
        private List<RoyalTitleOverride> customTitles_List2;

        /// <summary>
        /// Passes on the Realmtype 
        /// </summary>
        public void SetupTitles()
        {
            foreach (PlayerTitleDef title in TitlesBySeniority)
            {
                SetupTitle(title);
			}
        }

		public void SetupTitle(PlayerTitleDef title)
        {
			// Custom Title
			if (CustomTitles.TryGetValue(title, out RoyalTitleOverride titleOverrides))
			{
                if (titleOverrides.HasTitle())
                {
					ApplyTitleOverrides(title, titleOverrides);
					return;
				}
			}

			// Realm Type, if No Custom
			if (RealmTypeDef.TitlesWithOverrides.TryGetValue(title, out RoyalTitleOverride realmTypeOverrides))
			{
                ApplyTitleOverrides(title, realmTypeOverrides);
			}
            else
            {
				ApplyTitleOverrides(title, title.originalTitleFields);
			}
		}

		private static void ApplyTitleOverrides(PlayerTitleDef title, RoyalTitleOverride titleOverrides, bool isRealmType = false)
		{
            if (titleOverrides.HasTitle())
            {
                title.label = titleOverrides.label;
                title.labelFemale = titleOverrides.HasFemaleTitle() ? titleOverrides.labelFemale : null;
            }
            else
            {
                title.label = title.originalTitleFields.label;
				title.labelFemale = title.originalTitleFields.labelFemale;
			}

			title.titleTier = titleOverrides.titleTier ?? title.originalTitleFields.titleTier ?? TitleTiers.Lowborn;
			title.allowDignifiedMeditationFocus = titleOverrides.allowDignifiedMeditationFocus ?? title.originalTitleFields.allowDignifiedMeditationFocus ?? false;

			title.TRInheritable = titleOverrides.TRInheritable ?? title.originalTitleFields.TRInheritable ?? false;
			title.canBeInherited = TitularRoyaltyMod.Settings.inheritanceEnabled ? title.TRInheritable : false;

			title.minExpectation = titleOverrides.minExpectation ?? title.originalTitleFields.minExpectation ?? ExpectationDefOf.ExtremelyLow;

			title.ClearCachedData();
		}

		/// <summary>
		/// Resets all changed titles to their default realmType or Base Value
		/// </summary>
		public void ResetTitles()
        {
            customTitles = null;
            titlesBySeniority = null;
            customTitles_List1 = null;
            customTitles_List2 = null;

            foreach (PlayerTitleDef title in TitlesBySeniority)
            {
                title.ResetToDefaultValues();
                SetupTitle(title);
            }
        }

		public RoyalTitleOverride GetCustomTitleOverrideFor(PlayerTitleDef titleDef)
        {
            if (CustomTitles.TryGetValue(titleDef, out RoyalTitleOverride result))
            {
                return result;
            }
            Log.Error($"Titular Royalty: Could not find custom title override for {titleDef.defName} {titleDef.label}");
            return null;
        }


		public void SaveTitleChange(PlayerTitleDef title, RoyalTitleOverride newOverride)
        {
            customTitles[title] = newOverride;
            SetupTitle(title);
        }

        /// <summary>
        /// Code to be run on both loading or starting a new game
        /// </summary>
        public void OnGameStart()
        {
            SetupTitles();
			Faction.OfPlayer.SetupPlayerForTR(); // Set Permit factions and other options
			ModSettingsApplier.ApplySettings(); // Apply ModSettings Changes
        }

        public override void LoadedGame() => OnGameStart();
        public override void StartedNewGame() => OnGameStart();

        /// <summary>
        /// Saves & Loads the title data, if updating from a TR 1.1 save or the values are otherwise not found, generate new ones
        /// </summary>
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref realmTypeDefName, "realmTypeDefName", "RealmType_Kingdom");
            Scribe_Collections.Look(ref customTitles, "TRCustomTitles", LookMode.Def, LookMode.Deep, ref customTitles_List1, ref customTitles_List2);
        }

    }
}