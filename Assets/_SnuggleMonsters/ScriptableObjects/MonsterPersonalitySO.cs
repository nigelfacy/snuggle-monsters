// <copyright file="MonsterPersonalitySO.cs" company="SnuggleMonsters">
// Copyright (c) SnuggleMonsters. All rights reserved.
// </copyright>

using UnityEngine;

namespace SnuggleMonsters
{
    /// <summary>
    /// ScriptableObject defining a monster's personality archetype.
    /// Each personality has weighted traits (playful, sleepy, etc.),
    /// a collection of greeting and idle dialogue lines, and preferred
    /// animation and colour styles.
    /// </summary>
    [CreateAssetMenu(fileName = "NewMonsterPersonality", menuName = "SnuggleMonsters/Monster Personality", order = 2)]
    public class MonsterPersonalitySO : ScriptableObject
    {
        // ----- Identification -----

        /// <summary>Unique string identifier for save/load serialization.</summary>
        [SerializeField]
        [Tooltip("Unique identifier used for save/load and lookups.")]
        private string id;

        /// <summary>Human-readable name shown in UI.</summary>
        [SerializeField]
        [Tooltip("Display name shown in menus and tooltips.")]
        private string displayName;

        // ----- Personality Traits -----

        /// <summary>How playful the monster is. Higher = more energetic and bouncy.</summary>
        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("How playful the monster is. Higher = more energetic.")]
        private float playful = 0.5f;

        /// <summary>How sleepy the monster is. Higher = more prone to yawning and napping.</summary>
        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("How sleepy the monster is. Higher = more prone to napping.")]
        private float sleepy = 0.5f;

        /// <summary>How curious the monster is. Higher = more likely to investigate new things.</summary>
        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("How curious the monster is. Higher = more investigative.")]
        private float curious = 0.5f;

        /// <summary>How shy the monster is. Higher = more timid and reserved.</summary>
        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("How shy the monster is. Higher = more timid.")]
        private float shy = 0.5f;

        /// <summary>How adventurous the monster is. Higher = more daring and bold.</summary>
        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("How adventurous the monster is. Higher = more bold.")]
        private float adventurous = 0.5f;

        /// <summary>How cheeky the monster is. Higher = more mischievous and playful-impish.</summary>
        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("How cheeky the monster is. Higher = more mischievous.")]
        private float cheeky = 0.5f;

        // ----- Dialogue -----

        /// <summary>
        /// Lines the monster may say upon greeting the player.
        /// Use "{0}" as a placeholder for the monster's name.
        /// Example: "Hi! I'm {0}! Can I live with you?"
        /// </summary>
        [SerializeField]
        [Tooltip("Greeting lines. Use {0} as placeholder for the monster's name.")]
        private string[] greetingLines;

        /// <summary>
        /// Lines the monster may say during idle moments.
        /// </summary>
        [SerializeField]
        [Tooltip("Idle dialogue lines.")]
        private string[] idleLines;

        // ----- Animation ----≠

        /// <summary>
        /// Name of the idle animation style.
        /// Examples: "Bouncy", "Sleepy", "ShyWave".
        /// Used to select the appropriate animation coroutine in <see cref="MonsterAnimatorController"/>.
        /// </summary>
        [SerializeField]
        [Tooltip("Idle animation style name (e.g. Bouncy, Sleepy, ShyWave).")]
        private string idleAnimationStyle = "Bouncy";

        /// <summary>
        /// Name of the dance animation style.
        /// Examples: "Wiggly", "Spinny", "Bouncy".
        /// Used by <see cref="MonsterAnimatorController.Dance"/> to pick the dance sequence.
        /// </summary>
        [SerializeField]
        [Tooltip("Dance animation style name (e.g. Wiggly, Spinny, Bouncy).")]
        private string danceStyle = "Wiggly";

        // ----- Visual Preference -----

        /// <summary>The monster's favourite colour, used for accent lighting and decoration.</summary>
        [SerializeField]
        [Tooltip("The monster's favourite colour.")]
        private Color favouriteColour = Color.white;

        // ----- Public Properties -----

        /// <summary>Gets the unique identifier.</summary>
        public string Id => id;

        /// <summary>Gets the display name.</summary>
        public string DisplayName => displayName;

        /// <summary>Gets the playful trait value (0.0 – 1.0).</summary>
        public float Playful => playful;

        /// <summary>Gets the sleepy trait value (0.0 – 1.0).</summary>
        public float Sleepy => sleepy;

        /// <summary>Gets the curious trait value (0.0 – 1.0).</summary>
        public float Curious => curious;

        /// <summary>Gets the shy trait value (0.0 – 1.0).</summary>
        public float Shy => shy;

        /// <summary>Gets the adventurous trait value (0.0 – 1.0).</summary>
        public float Adventurous => adventurous;

        /// <summary>Gets the cheeky trait value (0.0 – 1.0).</summary>
        public float Cheeky => cheeky;

        /// <summary>Gets the greeting dialogue lines.</summary>
        public string[] GreetingLines => greetingLines;

        /// <summary>Gets the idle dialogue lines.</summary>
        public string[] IdleLines => idleLines;

        /// <summary>Gets the idle animation style name.</summary>
        public string IdleAnimationStyle => idleAnimationStyle;

        /// <summary>Gets the dance animation style name.</summary>
        public string DanceStyle => danceStyle;

        /// <summary>Gets the monster's favourite colour.</summary>
        public Color FavouriteColour => favouriteColour;
    }
}