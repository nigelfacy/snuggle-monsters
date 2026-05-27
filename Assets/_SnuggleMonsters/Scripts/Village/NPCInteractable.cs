using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SnuggleMonsters.Village
{
    /// <summary>
    /// Clickable NPC in the village hub that cycles through friendly dialogue lines
    /// when interacted with.  Displays a simple speech-bubble UI above the NPC.
    /// </summary>
    public class NPCInteractable : MonoBehaviour
    {
        [Header("NPC Identity")]
        [Tooltip("Display name of this NPC.")]
        public string npcName = "NPC";

        [Tooltip("Friendly dialogue lines — cycle through on each click.")]
        public string[] dialogueLines = new string[]
        {
            "Hello there!",
            "Nice to meet you!",
            "Have a wonderful day!"
        };

        [Tooltip("Optional sprite for the NPC.")]
        public Sprite npcSprite;

        [Header("Dialogue State")]
        [Tooltip("Has this NPC been greeted at least once?")]
        public bool hasBeenGreeted = false;

        [Header("UI References")]
        [Tooltip("Speech bubble GameObject (child of NPC). Must contain a TextMeshProUGUI component.")]
        public GameObject speechBubble;

        [Tooltip("Optional TextMeshPro field for the NPC name label (above bubble).")]
        public TextMeshProUGUI nameLabel;

        [Tooltip("Delay before hiding the speech bubble (seconds). Set 0 for persistent until next click.")]
        public float bubbleAutoHideDelay = 3f;

        // Internal state.
        private int dialogueIndex = 0;
        private TextMeshProUGUI bubbleText;
        private Coroutine hideCoroutine;

        private void Awake()
        {
            // Find TextMeshPro in speech bubble.
            if (speechBubble != null)
            {
                bubbleText = speechBubble.GetComponentInChildren<TextMeshProUGUI>();
                speechBubble.SetActive(false);
            }

            // Set name label.
            if (nameLabel != null)
                nameLabel.text = npcName;

            // Set sprite if available.
            if (npcSprite != null)
            {
                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.sprite = npcSprite;
            }
        }

        /// <summary>
        /// Called when the NPC is clicked (requires Collider2D on this GameObject).
        /// Cycles to the next dialogue line and shows the speech bubble.
        /// </summary>
        private void OnMouseDown()
        {
            Interact();
        }

        /// <summary>
        /// Public method usable by UnityEvent / Button onClick as well.
        /// </summary>
        public void Interact()
        {
            if (dialogueLines == null || dialogueLines.Length == 0)
                return;

            // Cycle dialogue index.
            dialogueIndex = (dialogueIndex + 1) % dialogueLines.Length;

            // Mark greeted.
            if (!hasBeenGreeted)
                hasBeenGreeted = true;

            // Show the current line.
            string line = dialogueLines[dialogueIndex];
            ShowBubble(line);

            // Optional greeting behaviour on first interaction.
            if (dialogueIndex == 0 && hasBeenGreeted)
            {
                // Could play a special greeting animation here.
            }
        }

        /// <summary>
        /// Show the speech bubble with the given text.
        /// Auto-hides after bubbleAutoHideDelay seconds (if > 0).
        /// </summary>
        /// <param name="text">The dialogue text to display.</param>
        private void ShowBubble(string text)
        {
            if (speechBubble == null || bubbleText == null)
                return;

            // Cancel any previous hide coroutine.
            if (hideCoroutine != null)
                StopCoroutine(hideCoroutine);

            bubbleText.text = text;
            speechBubble.SetActive(true);

            // Auto-hide if a delay is configured.
            if (bubbleAutoHideDelay > 0f)
            {
                hideCoroutine = StartCoroutine(HideBubbleAfterDelay(bubbleAutoHideDelay));
            }
        }

        /// <summary>
        /// Hide the speech bubble after a delay.
        /// </summary>
        private IEnumerator HideBubbleAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (speechBubble != null)
                speechBubble.SetActive(false);

            hideCoroutine = null;
        }

        /// <summary>
        /// Manually hide the speech bubble immediately.
        /// </summary>
        public void HideBubble()
        {
            if (hideCoroutine != null)
            {
                StopCoroutine(hideCoroutine);
                hideCoroutine = null;
            }

            if (speechBubble != null)
                speechBubble.SetActive(false);
        }
    }
}