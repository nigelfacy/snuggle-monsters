using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SnuggleMonsters
{
    /// <summary>
    /// Helper class that generates unique, fun, child-safe dance names
    /// and silly dance lines for the monster's special dance party.
    /// </summary>
    public static class SpecialDanceResolver
    {
        // ──────────────────────────────────────────────────────────────────
        //  Dance Name Generation
        // ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Generates a unique, fun dance name by combining the personality's
        /// dance style with fragments from body parts and tail/wing parts.
        /// Format: "The [Style] [PartA]-[PartB]"
        /// Example: "The Wiggly Fluff-Waggle" or "The Sleepy Hoot-Snuggle"
        /// </summary>
        /// <param name="parts">Array of selected MonsterPartSO (body, eyes, horns, wings, tail, pattern).</param>
        /// <param name="personality">The selected MonsterPersonalitySO.</param>
        /// <returns>A fun dance name string.</returns>
        public static string GenerateDanceName(MonsterPartSO[] parts, MonsterPersonalitySO personality)
        {
            // Fallback if nothing is provided
            if ((parts == null || parts.Length == 0) && personality == null)
                return "The Bouncy Fluff-Waggle";

            // Get the dance style from the personality
            string style = GetDanceStyle(personality);

            // Get fragments from body parts and tail/wing parts
            string partA = GetPartFragment(parts, GetBodyPartIndices());
            string partB = GetPartFragment(parts, GetTailWingIndices());

            // Ensure we have meaningful fragments
            if (string.IsNullOrEmpty(partA))
                partA = "Fluff";
            if (string.IsNullOrEmpty(partB))
                partB = "Waggle";

            return $"The {style} {partA}-{partB}";
        }

        // ──────────────────────────────────────────────────────────────────
        //  Funny Line Selection
        // ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns a silly dance line based on the monster's personality and parts.
        /// </summary>
        /// <param name="parts">Array of selected MonsterPartSO.</param>
        /// <param name="personality">The selected MonsterPersonalitySO.</param>
        /// <returns>A funny dance-related string.</returns>
        public static string PickFunnyLine(MonsterPartSO[] parts, MonsterPersonalitySO personality)
        {
            // Base funny lines available for any monster
            string[] genericLines = new string[]
            {
                "Wiggle wiggle, little snuggle!",
                "Shake your fluff!",
                "Boogie down with the fuzzy crew!",
                "Dance like everyone's watching... with snacks!",
                "Get your groove on, fluffy one!",
                "This is the best dance party ever!",
                "Twist and shout, let it all out!",
                "Dance party! Don't forget to wiggle your tail!",
                "Feel the beat in your fuzzy feet!",
                "Spinning, twirling, snuggling!"
            };

            // If we have a personality, use personality-specific lines
            if (personality != null)
            {
                string[] personalityLines = GetPersonalityLines(personality);
                if (personalityLines != null && personalityLines.Length > 0)
                {
                    return personalityLines[Random.Range(0, personalityLines.Length)];
                }
            }

            // Fallback to generic lines
            return genericLines[Random.Range(0, genericLines.Length)];
        }

        // ──────────────────────────────────────────────────────────────────
        //  Private Helpers
        // ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Gets the dance style string from the personality.
        /// </summary>
        private static string GetDanceStyle(MonsterPersonalitySO personality)
        {
            if (personality != null && !string.IsNullOrEmpty(personality.danceStyle))
                return personality.danceStyle;

            // Fallback styles
            string[] fallbackStyles = new string[]
            {
                "Bouncy", "Wiggly", "Sleepy", "Jumpy", "Spinning",
                "Happy", "Silly", "Floaty", "Tippy-Tappy", "Groovy"
            };

            return fallbackStyles[Random.Range(0, fallbackStyles.Length)];
        }

        /// <summary>
        /// Returns the indices in the parts array corresponding to body parts.
        /// Usually index 0 = body.
        /// </summary>
        private static int[] GetBodyPartIndices()
        {
            return new int[] { 0 }; // body
        }

        /// <summary>
        /// Returns the indices in the parts array corresponding to tail/wing parts.
        /// Index 3 = wings, index 4 = tail.
        /// </summary>
        private static int[] GetTailWingIndices()
        {
            return new int[] { 3, 4 }; // wings, tail
        }

        /// <summary>
        /// Extracts a display name fragment from one of the parts at the given indices.
        /// Picks a random matching part.
        /// </summary>
        private static string GetPartFragment(MonsterPartSO[] parts, int[] indices)
        {
            if (parts == null || indices == null)
                return null;

            List<string> candidates = new List<string>();

            foreach (int idx in indices)
            {
                if (idx >= 0 && idx < parts.Length && parts[idx] != null)
                {
                    string fragment = ExtractFragment(parts[idx].displayName);
                    if (!string.IsNullOrEmpty(fragment))
                        candidates.Add(fragment);
                }
            }

            if (candidates.Count == 0)
                return null;

            return candidates[Random.Range(0, candidates.Count)];
        }

        /// <summary>
        /// Extracts a single-word fragment from a part display name.
        /// Uses the last word or a sanitised version of the name.
        /// </summary>
        private static string ExtractFragment(string displayName)
        {
            if (string.IsNullOrEmpty(displayName))
                return null;

            // Split on spaces and take a random meaningful word
            string[] words = displayName.Split(new char[] { ' ', '-', '_', '.' },
                System.StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0)
                return null;

            // Filter out very short words and common filler words
            List<string> meaningful = new List<string>();
            foreach (string w in words)
            {
                string clean = w.Trim();
                if (clean.Length >= 3 && !IsFillerWord(clean))
                    meaningful.Add(clean);
            }

            if (meaningful.Count > 0)
                return meaningful[Random.Range(0, meaningful.Count)];

            // Fallback: return the whole display name, capitalised
            return Capitalise(displayName);
        }

        /// <summary>
        /// Checks if a word is a common filler word to skip.
        /// </summary>
        private static bool IsFillerWord(string word)
        {
            string lower = word.ToLowerInvariant();
            string[] fillers = new string[]
            {
                "the", "a", "an", "of", "in", "on", "at", "to", "for",
                "and", "or", "but", "is", "it", "be", "with"
            };

            foreach (string filler in fillers)
            {
                if (lower == filler)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Capitalises the first letter of a string.
        /// </summary>
        private static string Capitalise(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            StringBuilder sb = new StringBuilder(input);
            sb[0] = char.ToUpperInvariant(sb[0]);
            return sb.ToString();
        }

        /// <summary>
        /// Returns an array of personality-specific funny dance lines.
        /// </summary>
        private static string[] GetPersonalityLines(MonsterPersonalitySO personality)
        {
            if (personality == null)
                return null;

            string style = personality.danceStyle?.ToLowerInvariant() ?? "";

            switch (style)
            {
                case "bouncy":
                case "jumpy":
                case "happy":
                    return new string[]
                    {
                        "Boing boing! I can't stop bouncing!",
                        "This is the happiest dance ever!",
                        "Jump for joy! Wheee!",
                        "I'm so bouncy I might touch the ceiling!"
                    };

                case "sleepy":
                case "floaty":
                    return new string[]
                    {
                        "Yaaawn... dancing in my dreams...",
                        "Slow and sleepy, that's the way to be!",
                        "Just a little nap-dance... zzz...",
                        "Floating on a cloud of snuggles..."
                    };

                case "wiggly":
                case "silly":
                case "groovy":
                    return new string[]
                    {
                        "Wiggle wiggle wiggle!",
                        "I'm the wiggliest monster in town!",
                        "Do the worm! Do the wiggle!",
                        "Silly dances are the best dances!"
                    };

                case "spinning":
                case "tippy-tappy":
                    return new string[]
                    {
                        "Spin around and around! So dizzy!",
                        "Tippy tap tap tap! Dancing with my toes!",
                        "Round and round like a fluffy tornado!",
                        "Watch me spin! ...Whoa, I'm dizzy!"
                    };

                default:
                    return new string[]
                    {
                        $"Feeling {style} today! Watch me dance!",
                        $"This {style} dance is my favourite!",
                        $"Time to get {style}! Let's boogie!"
                    };
            }
        }
    }
}