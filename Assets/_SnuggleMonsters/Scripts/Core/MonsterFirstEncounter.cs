using System.Collections;
using UnityEngine;
using TMPro;
using SnuggleMonsters.Monster;

namespace SnuggleMonsters.Core
{
    /// <summary>
    /// CRITICAL: Handles the first-play moment when the player meets the monster
    /// for the first time in the Bedroom scene.  Plays a sequence of animations
    /// and dialogue, then unlocks the special dance and sets a flag so it only
    /// plays once.
    /// 
    /// Attach this script to the monster GameObject in the Bedroom scene.
    /// </summary>
    [RequireComponent(typeof(MonsterAnimatorController))]
    public class MonsterFirstEncounter : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("The monster's animation controller.")]
        [SerializeField] private MonsterAnimatorController monsterAnim;

        [Tooltip("The persistent monster runtime model.")]
        [SerializeField] private MonsterRuntimeModel monsterModel;

        [Header("Speech Bubble")]
        [Tooltip("Speech bubble GameObject (child of monster). Must contain a TextMeshProUGUI component.")]
        public GameObject speechBubble;

        [Tooltip("Text component inside the speech bubble.")]
        public TextMeshProUGUI speechText;

        [Header("Settings")]
        [Tooltip("Name of the special dance to unlock (if not already set).")]
        public string danceToUnlock = "HappyDance";

        [Tooltip("Delay between each step in the encounter sequence.")]
        public float stepDelay = 0.5f;

        [Tooltip("How long to show the speech bubble before it fades.")]
        public float bubbleDisplayDuration = 3f;

        [Tooltip("PlayerPrefs key used to persist the encounter-completed flag (fallback).")]
        public string encounterPrefsKey = "SnuggleMonsters_FirstEncounterDone";

        // Internal state.
        private bool hasPlayed = false;
        private Coroutine encounterCoroutine;

        private void Awake()
        {
            // Auto-assign animator.
            if (monsterAnim == null)
                monsterAnim = GetComponent<MonsterAnimatorController>();

            // Find speech bubble components.
            if (speechBubble != null && speechText == null)
                speechText = speechBubble.GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Start()
        {
            // Check if this encounter has already happened.
            if (monsterModel != null && monsterModel.HasCompletedFirstEncounter)
            {
                hasPlayed = true;
                return;
            }

            // Fallback: check PlayerPrefs if model is not available.
            if (monsterModel == null && PlayerPrefs.GetInt(encounterPrefsKey, 0) == 1)
            {
                hasPlayed = true;
                return;
            }

            // Hide speech bubble initially.
            if (speechBubble != null)
                speechBubble.SetActive(false);

            // Begin the encounter sequence.
            encounterCoroutine = StartCoroutine(PlayEncounterSequence());
        }

        /// <summary>
        /// The main encounter sequence: blink -> bounce -> wave -> speech -> unlock dance.
        /// </summary>
        private IEnumerator PlayEncounterSequence()
        {
            // 1. Blink.
            if (monsterAnim != null)
                yield return monsterAnim.Blink();

            yield return new WaitForSeconds(stepDelay);

            // 2. Bounce happily.
            if (monsterAnim != null)
                yield return monsterAnim.BounceHappy();

            yield return new WaitForSeconds(stepDelay);

            // 3. Wave.
            if (monsterAnim != null)
                yield return monsterAnim.Wave();

            yield return new WaitForSeconds(stepDelay * 0.5f);

            // 4. Show speech bubble.
            string monsterName = monsterModel != null ? monsterModel.MonsterName : "Fluff";
            string greeting = $"Hi! I'm {monsterName}! Can I live with you?";

            if (speechBubble != null)
                speechBubble.SetActive(true);

            if (speechText != null)
                speechText.text = greeting;

            // Display for the configured duration, then hide.
            yield return new WaitForSeconds(bubbleDisplayDuration);

            if (speechBubble != null)
                speechBubble.SetActive(false);

            // 5. Unlock special dance.
            if (monsterModel != null && string.IsNullOrEmpty(monsterModel.SpecialDanceName))
            {
                monsterModel.SpecialDanceName = danceToUnlock;
                Debug.Log($"[MonsterFirstEncounter] Special dance '{danceToUnlock}' unlocked!");

                // Play the dance as a treat.
                if (monsterAnim != null)
                    yield return monsterAnim.Dance(monsterModel.Personality != null ? monsterModel.Personality.DanceStyle : "Bouncy", 1f);
            }

            // 6. Mark as completed.
            MarkEncounterComplete();
        }

        /// <summary>
        /// Persist the first-encounter flag so it never plays again.
        /// </summary>
        private void MarkEncounterComplete()
        {
            if (monsterModel != null)
            {
                monsterModel.HasCompletedFirstEncounter = true;
            }

            // Fallback PlayerPrefs persistence.
            PlayerPrefs.SetInt(encounterPrefsKey, 1);
            PlayerPrefs.Save();

            hasPlayed = true;
            Debug.Log("[MonsterFirstEncounter] First encounter marked as complete.");
        }

        /// <summary>
        /// Public accessor for other scripts to query whether the encounter has played.
        /// </summary>
        public bool HasEncounterPlayed => hasPlayed;

        /// <summary>
        /// Manually trigger the encounter (e.g. from a debug menu or re-run).
        /// Only works if the encounter has not yet played.
        /// </summary>
        public void PlayEncounter()
        {
            if (hasPlayed)
                return;

            if (encounterCoroutine != null)
                StopCoroutine(encounterCoroutine);

            encounterCoroutine = StartCoroutine(PlayEncounterSequence());
        }
    }
}