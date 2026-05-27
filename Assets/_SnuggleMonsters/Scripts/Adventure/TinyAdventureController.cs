using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SnuggleMonsters.Adventure
{
    /// <summary>
    /// Controls the "Find the missing moon-button" mini-adventure.
    /// Players click three glowing objects to find the moon's missing button,
    /// earning a sticker reward when all are collected.
    /// </summary>
    public class TinyAdventureController : MonoBehaviour
    {
        [Header("Glowing Objects")]
        [Tooltip("The three glowing items the player must find and click.")]
        public GlowingObject[] glowingObjects = new GlowingObject[3];

        [Header("UI")]
        [Tooltip("Instruction text displayed at the top of the screen.")]
        public TextMeshProUGUI instructionText;

        [Tooltip("Button to exit the adventure and return to the village hub.")]
        public Button backButton;

        [Tooltip("Reward sticker GameObject — disabled on start, shown when all items collected.")]
        public GameObject rewardSticker;

        [Header("Reward")]
        [Tooltip("ID of the sticker to unlock when all items are found.")]
        public string stickerRewardId = "moon_button_sticker";

        [Tooltip("Reference to the persistent monster runtime model.")]
        public MonsterRuntimeModel monsterModel;

        [Header("Timing")]
        [Tooltip("Seconds to wait after each item is found before the next can be clicked.")]
        public float waitAfterEachFind = 0.5f;

        // Internal state.
        private int foundCount = 0;
        private bool allFound = false;
        private bool isProcessingClick = false;

        private void Start()
        {
            // Set initial instruction.
            if (instructionText != null)
            {
                instructionText.text = "Oh no! The moon dropped its button! Can you help find it?";
            }

            // Hide reward sticker initially.
            if (rewardSticker != null)
                rewardSticker.SetActive(false);

            // Wire up back button.
            if (backButton != null)
                backButton.onClick.AddListener(GoBackToVillage);

            // Subscribe to each glowing object's onFound event.
            if (glowingObjects != null)
            {
                for (int i = 0; i < glowingObjects.Length; i++)
                {
                    if (glowingObjects[i] != null)
                    {
                        int index = i; // Capture for closure.
                        glowingObjects[i].OnFound += () => StartCoroutine(OnObjectFound(index));
                    }
                }
            }

            // Fallback: try to find MonsterRuntimeModel if not assigned.
            if (monsterModel == null)
            {
                monsterModel = FindObjectOfType<MonsterRuntimeModel>();
                if (monsterModel == null)
                {
                    // Check GameManager.
                    if (GameManager.Instance != null)
                        monsterModel = GameManager.Instance.MonsterModel;
                }
            }
        }

        /// <summary>
        /// Called when a GlowingObject is clicked and found.
        /// Increments the count and checks for completion.
        /// </summary>
        /// <param name="objectIndex">Index of the object that was found.</param>
        private IEnumerator OnObjectFound(int objectIndex)
        {
            // Prevent overlapping click processing.
            if (isProcessingClick)
                yield break;

            isProcessingClick = true;
            foundCount++;

            Debug.Log($"[TinyAdventure] Found object {objectIndex} ({foundCount}/{glowingObjects.Length})");

            // Update instruction.
            if (instructionText != null)
            {
                int remaining = (glowingObjects?.Length ?? 0) - foundCount;
                if (remaining > 0)
                {
                    instructionText.text = $"Found one! {remaining} more to go...";
                }
            }

            yield return new WaitForSeconds(waitAfterEachFind);
            isProcessingClick = false;

            // Check for completion.
            if (foundCount >= (glowingObjects?.Length ?? 0) && !allFound)
            {
                allFound = true;
                OnAllFound();
            }
        }

        /// <summary>
        /// Called when all three glowing objects have been found.
        /// Displays the completion message, unlocks the sticker, and shows the reward.
        /// </summary>
        private void OnAllFound()
        {
            Debug.Log("[TinyAdventure] All items found!");

            // Completion message.
            if (instructionText != null)
            {
                instructionText.text = "You found the moon-button! The moon says THANK YOU!";
            }

            // Show reward popup / sticker.
            if (rewardSticker != null)
            {
                rewardSticker.SetActive(true);
            }

            // Unlock sticker in the monster model.
            if (monsterModel != null)
            {
                monsterModel.UnlockSticker(stickerRewardId);
                Debug.Log($"[TinyAdventure] Sticker '{stickerRewardId}' unlocked!");
            }
        }

        /// <summary>
        /// Return to the Village Hub scene.
        /// </summary>
        private void GoBackToVillage()
        {
            if (GameManager.Instance != null)
            {
                SceneController.LoadScene(GameManager.Instance.VillageHubSceneName);
            }
        }
    }
}