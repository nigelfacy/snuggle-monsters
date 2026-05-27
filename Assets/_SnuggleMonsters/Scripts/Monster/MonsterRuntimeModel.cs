// <copyright file="MonsterRuntimeModel.cs" company="SnuggleMonsters">
// Copyright (c) SnuggleMonsters. All rights reserved.
// </copyright>

using System.Collections.Generic;
using UnityEngine;

namespace SnuggleMonsters.Core
{
    /// <summary>
    /// Singleton MonoBehaviour that stores and manages the runtime state of the
    /// created monster throughout the game session. Persists across scenes via
    /// <see cref="DontDestroyOnLoad"/>.
    /// </summary>
    public class MonsterRuntimeModel : MonoBehaviour
    {
        // ----- Singleton -----

        /// <summary>Lazy singleton instance.</summary>
        private static MonsterRuntimeModel instance;

        /// <summary>
        /// Gets the singleton instance. Creates a new GameObject if none exists.
        /// </summary>
        public static MonsterRuntimeModel Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject(nameof(MonsterRuntimeModel));
                    instance = go.AddComponent<MonsterRuntimeModel>();
                    DontDestroyOnLoad(go);
                }

                return instance;
            }
        }

        // ----- State Fields -----

        [Header("Monster State")]

        /// <summary>Currently selected parts, one per BodyPartType index.</summary>
        [SerializeField]
        private MonsterPartSO[] selectedParts = new MonsterPartSO[6];

        /// <summary>The monster's chosen name.</summary>
        [SerializeField]
        private string monsterName = string.Empty;

        /// <summary>The monster's assigned personality.</summary>
        [SerializeField]
        private MonsterPersonalitySO personality;

        /// <summary>Currently equipped clothing items.</summary>
        [SerializeField]
        private List<ClothingSO> equippedClothes = new List<ClothingSO>();

        /// <summary>Currently placed decorations in the monster's room.</summary>
        [SerializeField]
        private List<DecorationSO> placedDecorations = new List<DecorationSO>();

        /// <summary>Sticker IDs the player has unlocked.</summary>
        [SerializeField]
        private List<string> unlockedStickers = new List<string>();

        /// <summary>
        /// Runtime-generated special dance name.
        /// Populated via <see cref="DanceRecipeSO.GenerateSpecialDanceName"/>.
        /// </summary>
        [SerializeField]
        private string specialDanceName = string.Empty;

        /// <summary>Colour of the night light in the bedroom.</summary>
        [SerializeField]
        private Color nightLightColor = Color.white;

        /// <summary>Whether the monster has been tucked into bed.</summary>
        [SerializeField]
        private bool hasBeenTuckedIn = false;

        /// <summary>Whether the first-encounter scene has played.</summary>
        [SerializeField]
        private bool hasCompletedFirstEncounter = false;

        /// <summary>Circular buffer of recent dialogue lines from the monster.</summary>
        [SerializeField]
        private List<string> dialogueLog = new List<string>();

        // ----- Public Properties -----

        /// <summary>Gets the array of selected monster parts, indexed by BodyPartType.</summary>
        public MonsterPartSO[] SelectedParts => selectedParts;

        /// <summary>Gets or sets the monster's name.</summary>
        public string MonsterName
        {
            get => monsterName;
            set => monsterName = value;
        }

        /// <summary>Gets or sets the monster's personality.</summary>
        public MonsterPersonalitySO Personality
        {
            get => personality;
            set => personality = value;
        }

        /// <summary>Gets the list of equipped clothing items.</summary>
        public IReadOnlyList<ClothingSO> EquippedClothes => equippedClothes;

        /// <summary>Gets the list of placed decorations.</summary>
        public IReadOnlyList<DecorationSO> PlacedDecorations => placedDecorations;

        /// <summary>Gets the list of unlocked sticker IDs.</summary>
        public List<string> UnlockedStickers => unlockedStickers;

        /// <summary>Gets or sets the special dance name.</summary>
        public string SpecialDanceName
        {
            get => specialDanceName;
            set => specialDanceName = value;
        }

        /// <summary>Gets or sets the night light colour.</summary>
        public Color NightLightColor
        {
            get => nightLightColor;
            set => nightLightColor = value;
        }

        /// <summary>Gets or sets whether the monster has been tucked in.</summary>
        public bool HasBeenTuckedIn
        {
            get => hasBeenTuckedIn;
            set => hasBeenTuckedIn = value;
        }

        /// <summary>Gets or sets whether the first-encounter vignette has played.</summary>
        public bool HasCompletedFirstEncounter
        {
            get => hasCompletedFirstEncounter;
            set => hasCompletedFirstEncounter = value;
        }

        /// <summary>Gets the dialogue log.</summary>
        public IReadOnlyList<string> DialogueLog => dialogueLog;

        // ----- Part Management -----

        /// <summary>
        /// Sets a monster part, replacing any existing part of the same <see cref="BodyPartType"/>.
        /// </summary>
        /// <param name="part">The part to assign.</param>
        public void SetPart(MonsterPartSO part)
        {
            if (part == null)
            {
                Debug.LogWarning("[MonsterRuntimeModel] Attempted to SetPart with null. Use RemovePart instead.");
                return;
            }

            int index = (int)part.PartType;
            if (index < 0 || index >= selectedParts.Length)
            {
                Debug.LogError($"[MonsterRuntimeModel] Part type index out of range: {part.PartType} ({index})");
                return;
            }

            selectedParts[index] = part;
            Debug.Log($"[MonsterRuntimeModel] Set part '{part.DisplayName}' in slot {part.PartType}.");
        }

        /// <summary>
        /// Removes the part in the specified slot, clearing it to null.
        /// </summary>
        /// <param name="type">The body part type slot to clear.</param>
        public void RemovePart(BodyPartType type)
        {
            int index = (int)type;
            if (index < 0 || index >= selectedParts.Length)
            {
                Debug.LogError($"[MonsterRuntimeModel] Part type index out of range: {type} ({index})");
                return;
            }

            selectedParts[index] = null;
            Debug.Log($"[MonsterRuntimeModel] Cleared part slot {type}.");
        }

        // ----- Personality -----

        /// <summary>
        /// Assigns a personality to the monster.
        /// </summary>
        /// <param name="p">The personality to assign.</param>
        public void SetPersonality(MonsterPersonalitySO p)
        {
            personality = p;

            if (p != null)
            {
                Debug.Log($"[MonsterRuntimeModel] Set personality to '{p.DisplayName}'.");
            }
            else
            {
                Debug.LogWarning("[MonsterRuntimeModel] Personality set to null.");
            }
        }

        // ----- Clothing -----

        /// <summary>
        /// Equips a clothing item. Does nothing if already equipped.
        /// </summary>
        /// <param name="item">The clothing item to equip.</param>
        public void EquipClothing(ClothingSO item)
        {
            if (item == null)
            {
                Debug.LogWarning("[MonsterRuntimeModel] Attempted to equip null clothing.");
                return;
            }

            if (equippedClothes.Contains(item))
            {
                Debug.Log($"[MonsterRuntimeModel] Clothing '{item.DisplayName}' is already equipped.");
                return;
            }

            equippedClothes.Add(item);
            Debug.Log($"[MonsterRuntimeModel] Equipped clothing '{item.DisplayName}'.");
        }

        /// <summary>
        /// Removes a clothing item from the monster.
        /// </summary>
        /// <param name="item">The clothing item to remove.</param>
        public void RemoveClothing(ClothingSO item)
        {
            if (item == null)
            {
                Debug.LogWarning("[MonsterRuntimeModel] Attempted to remove null clothing.");
                return;
            }

            if (equippedClothes.Remove(item))
            {
                Debug.Log($"[MonsterRuntimeModel] Removed clothing '{item.DisplayName}'.");
            }
            else
            {
                Debug.LogWarning($"[MonsterRuntimeModel] Clothing '{item.DisplayName}' was not equipped.");
            }
        }

        // ----- Decorations -----

        /// <summary>
        /// Places a decoration in the monster's room.
        /// </summary>
        /// <param name="deco">The decoration to place.</param>
        public void PlaceDecoration(DecorationSO deco)
        {
            if (deco == null)
            {
                Debug.LogWarning("[MonsterRuntimeModel] Attempted to place null decoration.");
                return;
            }

            if (placedDecorations.Contains(deco))
            {
                Debug.Log($"[MonsterRuntimeModel] Decoration '{deco.DisplayName}' is already placed.");
                return;
            }

            placedDecorations.Add(deco);
            Debug.Log($"[MonsterRuntimeModel] Placed decoration '{deco.DisplayName}'.");
        }

        /// <summary>
        /// Removes a decoration from the monster's room.
        /// </summary>
        /// <param name="deco">The decoration to remove.</param>
        public void RemoveDecoration(DecorationSO deco)
        {
            if (deco == null)
            {
                Debug.LogWarning("[MonsterRuntimeModel] Attempted to remove null decoration.");
                return;
            }

            if (placedDecorations.Remove(deco))
            {
                Debug.Log($"[MonsterRuntimeModel] Removed decoration '{deco.DisplayName}'.");
            }
            else
            {
                Debug.LogWarning($"[MonsterRuntimeModel] Decoration '{deco.DisplayName}' was not placed.");
            }
        }

        // ----- Stickers -----

        /// <summary>
        /// Adds a sticker by ID if it hasn't already been unlocked.
        /// </summary>
        /// <param name="stickerId">The unique sticker identifier.</param>
        public void AddSticker(string stickerId)
        {
            if (string.IsNullOrEmpty(stickerId))
            {
                Debug.LogWarning("[MonsterRuntimeModel] Attempted to add null or empty sticker ID.");
                return;
            }

            if (unlockedStickers.Contains(stickerId))
            {
                Debug.Log($"[MonsterRuntimeModel] Sticker '{stickerId}' is already unlocked.");
                return;
            }

            unlockedStickers.Add(stickerId);
            Debug.Log($"[MonsterRuntimeModel] Unlocked sticker '{stickerId}'.");
        }

        // ----- Dialogue -----

        /// <summary>
        /// Adds a line to the dialogue log. Keeps the log at a reasonable size.
        /// </summary>
        /// <param name="line">The dialogue line to add.</param>
        public void AddDialogueLine(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return;
            }

            dialogueLog.Add(line);

            // Keep the log from growing indefinitely
            const int maxLogSize = 100;
            if (dialogueLog.Count > maxLogSize)
            {
                dialogueLog.RemoveAt(0);
            }
        }

        /// <summary>
        /// Returns a greeting line from the assigned personality, with the monster's name
        /// substituted in. Falls back to a default greeting if no personality is assigned.
        /// </summary>
        /// <returns>A formatted greeting string.</returns>
        public string GetGreeting()
        {
            if (personality != null && personality.GreetingLines != null && personality.GreetingLines.Length > 0)
            {
                string template = personality.GreetingLines[Random.Range(0, personality.GreetingLines.Length)];
                string greeting = string.Format(template, monsterName);
                AddDialogueLine(greeting);
                return greeting;
            }

            string defaultGreeting = $"Hello! I'm {monsterName}!";
            AddDialogueLine(defaultGreeting);
            return defaultGreeting;
        }

        /// <summary>
        /// Returns a random idle dialogue line from the assigned personality.
        /// Falls back to a default idle line if no personality is assigned.
        /// </summary>
        /// <returns>An idle dialogue string.</returns>
        public string GetRandomIdleLine()
        {
            if (personality != null && personality.IdleLines != null && personality.IdleLines.Length > 0)
            {
                string line = personality.IdleLines[Random.Range(0, personality.IdleLines.Length)];
                AddDialogueLine(line);
                return line;
            }

            string defaultLine = "...";
            AddDialogueLine(defaultLine);
            return defaultLine;
        }

        // ----- Utility -----

        /// <summary>
        /// Clears all monster state, resetting to defaults.
        /// </summary>
        public void ResetAll()
        {
            for (int i = 0; i < selectedParts.Length; i++)
            {
                selectedParts[i] = null;
            }

            monsterName = string.Empty;
            personality = null;
            equippedClothes.Clear();
            placedDecorations.Clear();
            unlockedStickers.Clear();
            specialDanceName = string.Empty;
            nightLightColor = Color.white;
            hasBeenTuckedIn = false;
            hasCompletedFirstEncounter = false;
            dialogueLog.Clear();

            Debug.Log("[MonsterRuntimeModel] All state has been reset.");
        }

        // ----- Lifecycle -----

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogWarning("[MonsterRuntimeModel] Duplicate instance destroyed.");
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}