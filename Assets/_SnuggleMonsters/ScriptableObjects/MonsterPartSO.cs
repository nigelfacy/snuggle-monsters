// <copyright file="MonsterPartSO.cs" company="SnuggleMonsters">
// Copyright (c) SnuggleMonsters. All rights reserved.
// </copyright>

using UnityEngine;

namespace SnuggleMonsters
{
    /// <summary>
    /// ScriptableObject representing a single selectable monster body part.
    /// Each instance defines the visual identity, colour tint, and personality
    /// modifiers for one part slot (Body, Eyes, Horns, Wings, Tail, or Pattern).
    /// </summary>
    [CreateAssetMenu(fileName = "NewMonsterPart", menuName = "SnuggleMonsters/Monster Part", order = 1)]
    public class MonsterPartSO : ScriptableObject
    {
        // ----- Identification -----

        /// <summary>Unique string identifier used for save/load serialization and lookups.</summary>
        [SerializeField]
        [Tooltip("Unique identifier used for save/load and lookups.")]
        private string id;

        /// <summary>Human-readable name shown in UI.</summary>
        [SerializeField]
        [Tooltip("Display name shown in menus and tooltips.")]
        private string displayName;

        // ----- Visual -----

        /// <summary>Which body slot this part occupies.</summary>
        [SerializeField]
        [Tooltip("The body slot this part belongs to.")]
        private BodyPartType partType;

        /// <summary>Colour tint applied to the part sprite. Defaults to white (no tint).</summary>
        [SerializeField]
        [Tooltip("Colour tint applied to the part. White means no tint.")]
        private Color partColor = Color.white;

        /// <summary>
        /// The sprite rendered for this part.
        /// TODO: Replace with final art asset when available.
        /// </summary>
        [SerializeField]
        [Tooltip("Sprite rendered for this part. TODO: Replace with final art asset.")]
        private Sprite sprite;

        // ----- Gameplay Modifiers -----

        /// <summary>
        /// Modifier affecting dance animation blend (0.0 – 1.0).
        /// Higher values produce more exaggerated dance motion.
        /// </summary>
        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("Affects dance animation blend. Higher = more exaggerated motion.")]
        private float danceModifier = 0.5f;

        /// <summary>
        /// Modifier that pushes the monster's personality weights (0.0 – 1.0).
        /// Combined with <see cref="personalityWeightKeys"/> and <see cref="personalityWeightValues"/>.
        /// </summary>
        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("Overall personality weight modifier. Combined with key-value pairs.")]
        private float personalityModifier = 0.5f;

        // ----- Personality Weight Overrides -----

        /// <summary>
        /// Trait names whose weights are adjusted by this part.
        /// Must correspond one-to-one with <see cref="personalityWeightValues"/>.
        /// Example: "playful", "sleepy", "curious".
        /// </summary>
        [SerializeField]
        [Tooltip("Trait names affected by this part. One-to-one with weight values below.")]
        private string[] personalityWeightKeys;

        /// <summary>
        /// Weight values for each corresponding key in <see cref="personalityWeightKeys"/>.
        /// Values are added to the base personality traits during generation.
        /// </summary>
        [SerializeField]
        [Tooltip("Weight values for each trait key above. Added to base personality traits.")]
        private float[] personalityWeightValues;

        // ----- Public Properties -----

        /// <summary>Gets the unique identifier for this part.</summary>
        public string Id => id;

        /// <summary>Gets the human-readable display name.</summary>
        public string DisplayName => displayName;

        /// <summary>Gets the body slot this part occupies.</summary>
        public BodyPartType PartType => partType;

        /// <summary>Gets or sets the colour tint applied to the part sprite.</summary>
        public Color PartColor { get => partColor; set => partColor = value; }

        /// <summary>
        /// Gets the sprite rendered for this part.
        /// May be null if art has not been assigned yet.
        /// </summary>
        public Sprite Sprite => sprite;

        /// <summary>Gets the dance animation modifier (0.0 – 1.0).</summary>
        public float DanceModifier => danceModifier;

        /// <summary>Gets the overall personality modifier (0.0 – 1.0).</summary>
        public float PersonalityModifier => personalityModifier;

        /// <summary>Gets the trait names affected by this part's personality weight overrides.</summary>
        public string[] PersonalityWeightKeys => personalityWeightKeys;

        /// <summary>Gets the weight values for each corresponding trait key.</summary>
        public float[] PersonalityWeightValues => personalityWeightValues;

        // ----- Editor Validation -----

        /// <summary>
        /// Validates field consistency when the asset is modified in the Inspector.
        /// Ensures <see cref="personalityWeightKeys"/> and <see cref="personalityWeightValues"/>
        /// arrays have matching lengths.
        /// </summary>
        private void OnValidate()
        {
            if (personalityWeightKeys != null && personalityWeightValues != null &&
                personalityWeightKeys.Length != personalityWeightValues.Length)
            {
                Debug.LogWarning(
                    $"[MonsterPartSO] '{name}' has mismatched personalityWeightKeys ({personalityWeightKeys.Length}) " +
                    $"and personalityWeightValues ({personalityWeightValues.Length}). They must be the same length.",
                    this);
            }
        }
    }
}