using System;
using System.Collections;
using UnityEngine;

namespace SnuggleMonsters.Adventure
{
    /// <summary>
    /// An interactable object in the adventure scene that glows with a pulsing
    /// animation and can be clicked to "find" it.  Reports discovery back to the
    /// TinyAdventureController via the OnFound event.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
    public class GlowingObject : MonoBehaviour
    {
        [Header("Object Identity")]
        [Tooltip("Name of this glowing object (for logging / UI).")]
        public string objectName = "Glowing Object";

        [Tooltip("Message shown when the player finds this object.")]
        public string foundMessage = "You found a sparkly acorn!";

        [Header("State")]
        [Tooltip("Has this object already been found?")]
        public bool isFound = false;

        [Header("Glow Animation")]
        [Tooltip("Speed of the alpha-pulse glow effect.")]
        public float glowSpeed = 2f;

        [Header("References")]
        [Tooltip("SpriteRenderer for this object (auto-assigned if left empty).")]
        public SpriteRenderer spriteRenderer;

        [Tooltip("Optional particle system that plays when the object is found.")]
        public ParticleSystem sparkleOnFind;

        [Header("UI Reference")]
        [Tooltip("Optional Canvas / TextMeshPro object to display the found message.")]
        public GameObject foundMessagePopup;

        /// <summary>
        /// Fires when this object is found (clicked while not yet found).
        /// The TinyAdventureController subscribes to this event.
        /// </summary>
        public event Action OnFound;

        // Cached components.
        private Collider2D objectCollider;
        private Color originalColor;
        private Coroutine glowCoroutine;

        private void Awake()
        {
            // Auto-assign components.
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            objectCollider = GetComponent<Collider2D>();

            if (spriteRenderer != null)
                originalColor = spriteRenderer.color;
        }

        private void Start()
        {
            // Begin the pulsing glow animation.
            if (spriteRenderer != null)
            {
                glowCoroutine = StartCoroutine(PulseGlow());
            }
        }

        /// <summary>
        /// Handles click / tap input.  Requires a Collider2D on this GameObject.
        /// </summary>
        private void OnMouseDown()
        {
            if (!isFound)
            {
                MarkAsFound();
            }
        }

        /// <summary>
        /// Mark the object as found, play effects, show message, and notify the controller.
        /// </summary>
        public void MarkAsFound()
        {
            if (isFound)
                return;

            isFound = true;

            // Stop glow coroutine.
            if (glowCoroutine != null)
            {
                StopCoroutine(glowCoroutine);
                glowCoroutine = null;
            }

            // Set sprite to full brightness.
            if (spriteRenderer != null)
            {
                Color fullColor = originalColor;
                fullColor.a = 1f;
                spriteRenderer.color = fullColor;
            }

            // Play sparkle particle effect.
            if (sparkleOnFind != null)
            {
                sparkleOnFind.Play();
            }

            // Show the found message popup.
            if (foundMessagePopup != null)
            {
                foundMessagePopup.SetActive(true);
                var popupText = foundMessagePopup.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (popupText != null)
                    popupText.text = foundMessage;
            }

            // Disable collider so it can't be clicked again.
            if (objectCollider != null)
                objectCollider.enabled = false;

            Debug.Log($"[GlowingObject] Found: {objectName} — {foundMessage}");

            // Notify the controller.
            OnFound?.Invoke();
        }

        /// <summary>
        /// Coroutine that smoothly pulses the alpha value of the sprite.
        /// </summary>
        private IEnumerator PulseGlow()
        {
            if (spriteRenderer == null)
                yield break;

            while (!isFound)
            {
                // Oscillate alpha between 0.4 and 1.0 using a sine wave.
                float alpha = 0.4f + (Mathf.Sin(Time.time * glowSpeed) + 1f) * 0.3f;
                Color c = spriteRenderer.color;
                c.a = Mathf.Clamp01(alpha);
                spriteRenderer.color = c;
                yield return null;
            }
        }

        private void OnDestroy()
        {
            // Clean up coroutine.
            if (glowCoroutine != null)
            {
                StopCoroutine(glowCoroutine);
                glowCoroutine = null;
            }
        }
    }
}