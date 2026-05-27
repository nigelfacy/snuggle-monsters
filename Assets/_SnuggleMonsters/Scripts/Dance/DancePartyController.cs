using System.Collections;
using SnuggleMonsters.Core;
using SnuggleMonsters.Monster;
using UnityEngine;
using UnityEngine.UI;

namespace SnuggleMonsters
{
    /// <summary>
    /// Controls the dance party mode in the Bedroom scene.
    /// Handles special dance animations, confetti/sparkle effects,
    /// and funny dialogue display.
    /// </summary>
    public class DancePartyController : MonoBehaviour
    {
        // ──────────────────────────────────────────────────────────────────
        //  Serialized Fields
        // ──────────────────────────────────────────────────────────────────

        [Header("UI References")]
        [Tooltip("Button that triggers the special dance.")]
        public Button specialDanceButton;

        [Tooltip("UI panel containing all dance party UI elements.")]
        public GameObject danceUI;

        [Header("Dance Info Text")]
        [Tooltip("Text field that displays the name of the special dance.")]
        public TMPro.TextMeshProUGUI danceNameText;

        [Tooltip("Text field that displays a random funny line during the dance.")]
        public TMPro.TextMeshProUGUI funnyLineText;

        [Header("Model & Animation")]
        [Tooltip("The runtime monster model with dance name and personality data.")]
        public MonsterRuntimeModel monsterModel;

        [Tooltip("The monster's animator controller for playing dance animations.")]
        public MonsterAnimatorController animator;

        [Header("Effects (Optional)")]
        [Tooltip("Optional particle system for sparkle effects during the dance. Null is fine for prototype.")]
        public ParticleSystem sparkleEffect;

        [Tooltip("Image overlay with simple coloured dots used as confetti. Can be null for prototype.")]
        public Image confettiOverlay;

        // ──────────────────────────────────────────────────────────────────
        //  Private State
        // ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Whether a dance is currently in progress.
        /// </summary>
        private bool _isDancing;

        /// <summary>
        /// Cached original position of the dance UI for slide animation.
        /// </summary>
        private Vector2 _danceUIOriginalPosition;

        /// <summary>
        /// Whether the dance UI is currently visible.
        /// </summary>
        private bool _danceUIVisible;

        // ──────────────────────────────────────────────────────────────────
        //  Unity Lifecycle
        // ──────────────────────────────────────────────────────────────────

        private void Awake()
        {
            // Resolve monster model if not assigned
            if (monsterModel == null)
                monsterModel = FindObjectOfType<MonsterRuntimeModel>();

            // Cache the original dance UI position for slide animation
            if (danceUI != null)
            {
                RectTransform rect = danceUI.GetComponent<RectTransform>();
                if (rect != null)
                    _danceUIOriginalPosition = rect.anchoredPosition;

                // Start hidden
                danceUI.SetActive(false);
                _danceUIVisible = false;
            }
        }

        private void OnEnable()
        {
            // Wire up the special dance button
            if (specialDanceButton != null)
            {
                specialDanceButton.onClick.RemoveAllListeners();
                specialDanceButton.onClick.AddListener(OnSpecialDanceButton);
            }
        }

        private void OnDisable()
        {
            if (specialDanceButton != null)
                specialDanceButton.onClick.RemoveListener(OnSpecialDanceButton);
        }

        // ──────────────────────────────────────────────────────────────────
        //  Public Methods
        // ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Shows the dance party UI with a slide-in animation.
        /// </summary>
        public void ShowDanceUI()
        {
            if (danceUI == null || _danceUIVisible)
                return;

            danceUI.SetActive(true);
            _danceUIVisible = true;

            // Slide-in animation via coroutine
            StartCoroutine(AnimateSlideIn());
        }

        /// <summary>
        /// Hides the dance party UI with a slide-out animation.
        /// </summary>
        public void HideDanceUI()
        {
            if (danceUI == null || !_danceUIVisible)
                return;

            // Slide-out animation via coroutine
            StartCoroutine(AnimateSlideOut());
        }

        /// <summary>
        /// Toggles the dance party UI visibility.
        /// </summary>
        public void ToggleDanceUI()
        {
            if (_danceUIVisible)
                HideDanceUI();
            else
                ShowDanceUI();
        }

        // ──────────────────────────────────────────────────────────────────
        //  Dance Execution
        // ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Called when the special dance button is pressed.
        /// Retrieves the dance name from the model, displays fanfare text,
        /// plays the dance animation, shows a funny line, and triggers effects.
        /// </summary>
        public void OnSpecialDanceButton()
        {
            if (_isDancing)
            {
                Debug.Log("[DancePartyController] Already dancing!");
                return;
            }

            if (monsterModel == null)
            {
                Debug.LogError("[DancePartyController] No MonsterRuntimeModel assigned.");
                return;
            }

            // Get the special dance name from the model
            string danceName = monsterModel.SpecialDanceName;
            if (string.IsNullOrEmpty(danceName))
            {
                // Fallback: generate on the fly from parts
                MonsterPartSO[] parts = monsterModel.SelectedParts;
                danceName = SpecialDanceResolver.GenerateDanceName(parts, monsterModel.Personality);
                monsterModel.SpecialDanceName = danceName;
            }

            // Show the dance name with fanfare
            if (danceNameText != null)
                danceNameText.text = $"✨ {danceName} ✨";

            // Show a random funny line
            if (funnyLineText != null)
            {
                funnyLineText.text = SpecialDanceResolver.PickFunnyLine(monsterModel.SelectedParts, monsterModel.Personality);
            }

            // Play the dance animation via the existing SpecialDance trigger
            if (animator != null)
            {
                animator.SpecialDance();
            }

            // Start the effects coroutine
            StartCoroutine(DanceEffectsCoroutine());

            Debug.Log($"[DancePartyController] Started special dance: {danceName}");
        }

        // ──────────────────────────────────────────────────────────────────
        //  Coroutines
        // ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Coroutine that manages confetti/sparkle effects during the dance.
        /// Runs for the duration of the dance animation.
        /// </summary>
        private IEnumerator DanceEffectsCoroutine()
        {
            _isDancing = true;

            // --- Sparkle Effect ---
            if (sparkleEffect != null)
            {
                sparkleEffect.Play();
            }

            // --- Confetti Overlay ---
            if (confettiOverlay != null)
            {
                confettiOverlay.gameObject.SetActive(true);
                confettiOverlay.color = GetConfettiColor();
                StartCoroutine(AnimateConfetti(confettiOverlay));
            }

            // Wait for the dance animation to finish (approximate duration)
            // If animator is null, just show for a short time
            float danceDuration = animator != null ? 3f : 2f;
            yield return new WaitForSeconds(danceDuration);

            // --- Stop Effects ---
            if (sparkleEffect != null)
            {
                sparkleEffect.Stop();
            }

            if (confettiOverlay != null)
            {
                confettiOverlay.gameObject.SetActive(false);
            }

            _isDancing = false;

            Debug.Log("[DancePartyController] Dance finished!");
        }

        /// <summary>
        /// Animates the confetti overlay with colour cycling and pulsing alpha.
        /// Runs continuously on the overlay Image until the coroutine is stopped
        /// or the GameObject is deactivated.
        /// </summary>
        private IEnumerator AnimateConfetti(Image overlay)
        {
            if (overlay == null)
                yield break;

            float elapsed = 0f;
            while (overlay.gameObject.activeInHierarchy)
            {
                elapsed += Time.deltaTime;

                // Pulse alpha between 0.3 and 0.8
                float alpha = 0.3f + (Mathf.Sin(elapsed * 3f) * 0.25f + 0.25f);
                Color c = overlay.color;
                c.a = alpha;
                overlay.color = c;

                yield return null;
            }
        }

        /// <summary>
        /// Slide-in animation for the dance UI.
        /// Slides the panel up from below the screen.
        /// </summary>
        private IEnumerator AnimateSlideIn()
        {
            if (danceUI == null)
                yield break;

            RectTransform rect = danceUI.GetComponent<RectTransform>();
            if (rect == null)
                yield break;

            // Start position: off-screen below
            Vector2 startPos = _danceUIOriginalPosition + Vector2.down * 200f;
            Vector2 endPos = _danceUIOriginalPosition;

            float duration = 0.35f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                rect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
                yield return null;
            }

            rect.anchoredPosition = endPos;
        }

        /// <summary>
        /// Slide-out animation for the dance UI.
        /// Slides the panel back down below the screen, then deactivates it.
        /// </summary>
        private IEnumerator AnimateSlideOut()
        {
            if (danceUI == null)
                yield break;

            RectTransform rect = danceUI.GetComponent<RectTransform>();
            if (rect == null)
            {
                danceUI.SetActive(false);
                _danceUIVisible = false;
                yield break;
            }

            Vector2 startPos = rect.anchoredPosition;
            Vector2 endPos = _danceUIOriginalPosition + Vector2.down * 200f;

            float duration = 0.3f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                rect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
                yield return null;
            }

            rect.anchoredPosition = startPos; // Reset
            danceUI.SetActive(false);
            _danceUIVisible = false;
        }

        // ──────────────────────────────────────────────────────────────────
        //  Helpers
        // ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns a confetti colour based on the monster's personality favourite colour,
        /// or defaults to a bright rainbow colour.
        /// </summary>
        private Color GetConfettiColor()
        {
            if (monsterModel != null && monsterModel.Personality != null)
                return monsterModel.Personality.favouriteColor;

            // Default bright confetti colours
            Color[] confettiColors = new Color[]
            {
                Color.red, Color.blue, Color.green, Color.yellow,
                Color.magenta, new Color(1f, 0.5f, 0f) // orange
            };

            return confettiColors[Random.Range(0, confettiColors.Length)];
        }

        /// <summary>
        /// Static method that combines part names + personality style into a unique dance name.
        /// Convenience wrapper around SpecialDanceResolver.GenerateDanceName.
        /// </summary>
        /// <param name="parts">Array of selected MonsterPartSO.</param>
        /// <param name="personality">The selected MonsterPersonalitySO.</param>
        /// <returns>A fun dance name string.</returns>
        public static string GenerateDanceName(MonsterPartSO[] parts, MonsterPersonalitySO personality)
        {
            return SpecialDanceResolver.GenerateDanceName(parts, personality);
        }
    }
}