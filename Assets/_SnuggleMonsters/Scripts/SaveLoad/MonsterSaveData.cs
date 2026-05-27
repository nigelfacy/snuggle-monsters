using System;
using System.Collections.Generic;
using SnuggleMonsters.Core;
using UnityEngine;

namespace SnuggleMonsters
{
    /// <summary>
    /// Serializable snapshot of MonsterRuntimeModel data used for JSON persistence.
    /// Contains only primitive / serializable fields — no direct UnityEngine.Object references.
    /// ScriptableObject references are stored as ID strings and resolved at load-time
    /// via Resources.LoadAll.
    /// 
    /// TODO: For full part/clothing/deco persistence, store all selected part IDs,
    /// clothing IDs, and decoration IDs as string arrays. Currently stores colors
    /// as a minimal working prototype.
    /// </summary>
    [Serializable]
    public class MonsterSaveData
    {
        /// <summary>The monster's given name.</summary>
        public string monsterName = "Fluff";

        /// <summary>Body colour stored as RGBA floats.</summary>
        public float bodyColorR = 1f;
        public float bodyColorG = 1f;
        public float bodyColorB = 1f;
        public float bodyColorA = 1f;

        /// <summary>Favourite colour RGBA floats.</summary>
        public float favColorR = 1f;
        public float favColorG = 0.41f;
        public float favColorB = 0.71f;
        public float favColorA = 1f;

        /// <summary>Personality border colour RGBA floats.</summary>
        public float borderColorR = 1f;
        public float borderColorG = 0.84f;
        public float borderColorB = 0f;
        public float borderColorA = 1f;

        /// <summary>List of unlocked sticker IDs.</summary>
        public List<string> unlockedStickers = new List<string>();

        /// <summary>Name of the special dance animation (empty if not unlocked).</summary>
        public string specialDanceName = string.Empty;

        /// <summary>Whether the first-encounter sequence has played.</summary>
        public bool hasCompletedFirstEncounter = false;

        /// <summary>Whether the monster has been tucked into bed.</summary>
        public bool hasBeenTuckedIn = false;

        /// <summary>Night light colour RGBA.</summary>
        public float nightLightColorR = 1f;
        public float nightLightColorG = 1f;
        public float nightLightColorB = 1f;
        public float nightLightColorA = 1f;

        /// <summary>ISO timestamp of the last save.</summary>
        public string lastSaveTimestamp = string.Empty;

        /// <summary>
        /// Default constructor required for JSON deserialisation.
        /// </summary>
        public MonsterSaveData() { }

        /// <summary>
        /// Construct a save-data snapshot from a MonsterRuntimeModel instance.
        /// </summary>
        /// <param name="model">The runtime model to snapshot.</param>
        public MonsterSaveData(MonsterRuntimeModel model)
        {
            // Name
            monsterName = model.MonsterName;

            // Body colour from selected body part (or default if none)
            MonsterPartSO bodyPart = model.SelectedParts[(int)BodyPartType.Body];
            Color bodyCol = (bodyPart != null) ? bodyPart.PartColor : Color.white;
            bodyColorR = bodyCol.r;
            bodyColorG = bodyCol.g;
            bodyColorB = bodyCol.b;
            bodyColorA = bodyCol.a;

            // Personality-derived colours
            MonsterPersonalitySO pers = model.Personality;
            if (pers != null)
            {
                Color favCol = pers.FavouriteColour;
                favColorR = favCol.r;
                favColorG = favCol.g;
                favColorB = favCol.b;
                favColorA = favCol.a;
            }

            // Border colour: derived from personality ID hash for now (simple deterministic)
            borderColorR = 1f;
            borderColorG = 0.84f;
            borderColorB = 0f;
            borderColorA = 1f;

            unlockedStickers = new List<string>(model.UnlockedStickers);
            specialDanceName = model.SpecialDanceName;
            hasCompletedFirstEncounter = model.HasCompletedFirstEncounter;
            hasBeenTuckedIn = model.HasBeenTuckedIn;

            Color nightColor = model.NightLightColor;
            nightLightColorR = nightColor.r;
            nightLightColorG = nightColor.g;
            nightLightColorB = nightColor.b;
            nightLightColorA = nightColor.a;

            lastSaveTimestamp = DateTime.UtcNow.ToString("o");
        }

        /// <summary>
        /// Apply this saved data back into a MonsterRuntimeModel, overwriting current values.
        /// </summary>
        /// <param name="model">The runtime model to populate.</param>
        public void PopulateModel(MonsterRuntimeModel model)
        {
            model.MonsterName = monsterName;
            model.SpecialDanceName = specialDanceName;
            model.HasCompletedFirstEncounter = hasCompletedFirstEncounter;
            model.HasBeenTuckedIn = hasBeenTuckedIn;
            model.NightLightColor = new Color(nightLightColorR, nightLightColorG, nightLightColorB, nightLightColorA);

            // Stickers
            model.UnlockedStickers.Clear();
            foreach (string sticker in unlockedStickers)
                model.UnlockedStickers.Add(sticker);
        }
    }
}