using System.Collections;
using UnityEngine;

namespace SnuggleMonsters.Bedtime
{
    /// <summary>
    /// Controls a simple night-light glow effect.  Can be attached to a Light component
    /// (point light) or an Image (UI glow).  Provides smooth colour transitions via Lerp coroutine.
    /// </summary>
    public class NightLightController : MonoBehaviour
    {
        [Header("Light References")]
        [Tooltip("Optional Light component (point light). If assigned, its colour will be animated.")]
        [SerializeField] private Light pointLight;

        [Tooltip("Optional SpriteRenderer for a glow sprite. If assigned, its colour will be animated.")]
        [SerializeField] private SpriteRenderer glowSprite;

        [Tooltip("Optional CanvasGroup for UI glow fade. If assigned, its alpha may also be controlled.")]
        [SerializeField] private CanvasGroup glowCanvasGroup;

        [Header("Settings")]
        [Tooltip("Default warm-yellow colour.")]
        public Color currentColor = new Color(1f, 0.92f, 0.6f);

        [Tooltip("Duration of the colour transition animation in seconds.")]
        [SerializeField] private float transitionDuration = 1.5f;

        // Active transition coroutine.
        private Coroutine transitionCoroutine;

        private void Start()
        {
            // Auto-find Light if not assigned.
            if (pointLight == null)
                pointLight = GetComponent<Light>();

            // Auto-find SpriteRenderer if not assigned.
            if (glowSprite == null)
                glowSprite = GetComponent<SpriteRenderer>();

            // Apply the initial colour immediately.
            ApplyColor(currentColor);
        }

        /// <summary>
        /// Smoothly transition the night-light to the specified colour.
        /// </summary>
        /// <param name="c">The target colour.</param>
        public void SetColor(Color c)
        {
            currentColor = c;

            if (transitionCoroutine != null)
                StopCoroutine(transitionCoroutine);

            transitionCoroutine = StartCoroutine(LerpColor(c));
        }

        /// <summary>
        /// Coroutine that smoothly interpolates all light components to the target colour.
        /// </summary>
        /// <param name="targetColor">The colour to transition to.</param>
        private IEnumerator LerpColor(Color targetColor)
        {
            Color startColor = GetCurrentAppliedColor();
            float elapsed = 0f;

            while (elapsed < transitionDuration)
            {
                float t = elapsed / transitionDuration;
                t = Mathf.SmoothStep(0f, 1f, t); // Smooth in/out.

                Color blended = Color.Lerp(startColor, targetColor, t);
                ApplyColor(blended);

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Ensure exact final colour.
            ApplyColor(targetColor);
            transitionCoroutine = null;
        }

        /// <summary>
        /// Sample the current colour from the active light component.
        /// </summary>
        private Color GetCurrentAppliedColor()
        {
            if (pointLight != null && pointLight.enabled)
                return pointLight.color;

            if (glowSprite != null)
                return glowSprite.color;

            return currentColor;
        }

        /// <summary>
        /// Immediately apply a colour to all assigned light components.
        /// </summary>
        /// <param name="c">The colour to apply.</param>
        private void ApplyColor(Color c)
        {
            if (pointLight != null)
                pointLight.color = c;

            if (glowSprite != null)
                glowSprite.color = c;

            if (glowCanvasGroup != null)
                glowCanvasGroup.alpha = c.a;
        }
    }
}