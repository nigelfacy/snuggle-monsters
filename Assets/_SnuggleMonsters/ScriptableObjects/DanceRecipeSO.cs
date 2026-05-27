// <copyright file="DanceRecipeSO.cs" company="SnuggleMonsters">
// Copyright (c) SnuggleMonsters. All rights reserved.
// </copyright>

using System.Text;
using UnityEngine;

namespace SnuggleMonsters
{
    /// <summary>
    /// ScriptableObject defining an optional unlockable dance recipe.
    /// When a monster equips all required part types, the recipe becomes available.
    /// Also provides a static helper to generate a runtime dance name from
    /// the monster's current parts and personality.
    /// </summary>
    [CreateAssetMenu(fileName = "NewDanceRecipe", menuName = "SnuggleMonsters/Dance Recipe", order = 3)]
    public class DanceRecipeSO : ScriptableObject
    {
        // ----- Identification -----

        /// <summary>Unique string identifier for save/load and lookups.</summary>
        [SerializeField]
        [Tooltip("Unique identifier used for save/load and lookups.")]
        private string id;

        /// <summary>Human-readable name for the recipe asset.</summary>
        [SerializeField]
        [Tooltip("Display name for this recipe.")]
        private string displayName;

        // ----- Requirements -----

        /// <summary>
        /// Set of body part types the monster must have equipped to unlock this dance.
        /// Example: [Wings, Tail] unlocks a "Flutter-Waggle" dance.
        /// </summary>
        [SerializeField]
        [Tooltip("Body part types required to unlock this dance. E.g. [Wings, Tail].")]
        private BodyPartType[] requiredPartTypes;

        // ----- Dance Data -----

        /// <summary>
        /// Name of the animation trigger or coroutine to play.
        /// Example: "FlutterWaggleDance".
        /// </summary>
        [SerializeField]
        [Tooltip("Animation name/coroutine identifier for this dance.")]
        private string animationName;

        /// <summary>
        /// Fun, display-friendly name for the dance.
        /// Example: "The Flutter-Waggle".
        /// </summary>
        [SerializeField]
        [Tooltip("Fun display name (e.g. 'The Flutter-Waggle').")]
        private string funName;

        /// <summary>
        /// Lines the monster says while performing this dance.
        /// Example: "Wheee! Look at me!".
        /// </summary>
        [SerializeField]
        [Tooltip("Funny lines the monster says during this dance.")]
        private string[] funnyLines;

        // ----- Effects -----

        /// <summary>
        /// Intensity of the sparkle/particle effect during the dance (0.0 – 1.0).
        /// Passed to <see cref="MonsterAnimatorController.SparkleEffect"/>.
        /// </summary>
        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("Intensity of sparkle/particle effects during the dance.")]
        private float sparkleEffectIntensity = 0.5f;

        /// <summary>
        /// Speed multiplier applied to dance animation timing.
        /// 1.0 = normal speed, 0.5 = half speed, 2.0 = double speed.
        /// </summary>
        [SerializeField]
        [Tooltip("Speed multiplier for the dance animation. 1.0 = normal.")]
        private float danceSpeedMultiplier = 1f;

        // ----- Public Properties -----

        /// <summary>Gets the unique identifier.</summary>
        public string Id => id;

        /// <summary>Gets the display name.</summary>
        public string DisplayName => displayName;

        /// <summary>Gets the required body part types.</summary>
        public BodyPartType[] RequiredPartTypes => requiredPartTypes;

        /// <summary>Gets the animation identifier.</summary>
        public string AnimationName => animationName;

        /// <summary>Gets the fun display name.</summary>
        public string FunName => funName;

        /// <summary>Gets the funny lines.</summary>
        public string[] FunnyLines => funnyLines;

        /// <summary>Gets the sparkle effect intensity.</summary>
        public float SparkleEffectIntensity => sparkleEffectIntensity;

        /// <summary>Gets the dance speed multiplier.</summary>
        public float DanceSpeedMultiplier => danceSpeedMultiplier;

        // ----- Runtime Dance Name Generation -----

        /// <summary>
        /// Generates a unique dance name string at runtime by combining the names
        /// of the monster's equipped parts with their personality's dance style.
        /// This is used to populate <see cref="MonsterRuntimeModel.SpecialDanceName"/>
        /// and is NOT loaded from a recipe — recipes are extra optional dances.
        /// </summary>
        /// <param name="parts">Array of currently selected monster parts. May contain null slots.</param>
        /// <param name="personality">The monster's assigned personality. May be null.</param>
        /// <returns>A procedurally generated dance name string, or a default if inputs are empty/null.</returns>
        /// <example>
        /// Parts: [FluffyBody, RoundEyes, SpiralHorns, null, null, null] + personality "Bouncy"
        /// Returns: "The Fluffy-Bouncy Spiral Hop"
        /// </example>
        public static string GenerateSpecialDanceName(MonsterPartSO[] parts, MonsterPersonalitySO personality)
        {
            StringBuilder sb = new StringBuilder("The ");

            if (parts == null || parts.Length == 0)
            {
                sb.Append("Default Wiggle");
                return sb.ToString();
            }

            // Collect non-null part display names
            StringBuilder partsSb = new StringBuilder();
            int partCount = 0;

            foreach (MonsterPartSO part in parts)
            {
                if (part != null && !string.IsNullOrEmpty(part.DisplayName))
                {
                    if (partCount > 0)
                    {
                        partsSb.Append("-");
                    }

                    // Take a short portion of the part name for the dance name
                    string shortName = GetShortPartName(part.DisplayName);
                    partsSb.Append(shortName);
                    partCount++;
                }
            }

            if (partCount == 0)
            {
                sb.Append("Mystery");
            }
            else
            {
                sb.Append(partsSb);
            }

            // Append personality-inspired suffix
            string suffix = GetDanceSuffix(personality);
            if (!string.IsNullOrEmpty(suffix))
            {
                sb.Append(" ");
                sb.Append(suffix);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Extracts a short, punchy portion of a part name suitable for dance naming.
        /// Uses the first word or first 8 characters, whichever is shorter.
        /// </summary>
        /// <param name="fullName">The full display name of the part.</param>
        /// <returns>A shortened version of the name.</returns>
        private static string GetShortPartName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                return string.Empty;
            }

            // Use first word
            int spaceIndex = fullName.IndexOf(' ');
            if (spaceIndex > 0 && spaceIndex <= 8)
            {
                return fullName.Substring(0, spaceIndex);
            }

            // Truncate to 8 characters max
            return fullName.Length > 8 ? fullName.Substring(0, 8) : fullName;
        }

        /// <summary>
        /// Returns a dance suffix based on the personality's dance style.
        /// </summary>
        /// <param name="personality">The monster's personality.</param>
        /// <returns>A dance-style word or phrase.</returns>
        private static string GetDanceSuffix(MonsterPersonalitySO personality)
        {
            if (personality == null || string.IsNullOrEmpty(personality.DanceStyle))
            {
                return "Shuffle";
            }

            switch (personality.DanceStyle.ToLowerInvariant())
            {
                case "wiggly":
                    return "Wiggle";
                case "spinny":
                    return "Spin";
                case "bouncy":
                    return "Hop";
                case "sleepy":
                    return "Sway";
                case "shy":
                    return "Twirl";
                default:
                    return personality.DanceStyle;
            }
        }

        // ----- Editor Validation -----

        /// <summary>
        /// Validates the asset's fields when edited in the Inspector.
        /// Logs warnings for missing or inconsistent data.
        /// </summary>
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning($"[DanceRecipeSO] '{name}' has no id set.", this);
            }

            if (requiredPartTypes == null || requiredPartTypes.Length == 0)
            {
                Debug.LogWarning($"[DanceRecipeSO] '{name}' has no required part types. It will always be unlocked.", this);
            }
        }
    }
}