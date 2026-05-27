using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SnuggleMonsters.UI
{
    /// <summary>
    /// Shared UI utilities: panel management, fades, typewriter text, and button shake.
    /// Provides common UX patterns for use across scenes.
    /// </summary>
    public class UIController : MonoBehaviour
    {
        [Header("Panel Management")]
        [Tooltip("List of all panels managed by this controller. When ShowPanel is called, all others are hidden.")]
        public List<GameObject> managedPanels = new List<GameObject>();

        /// <summary>
        /// Show one panel and hide all others in the managed list.
        /// </summary>
        /// <param name="panel">The panel to show. Must be in managedPanels.</param>
        public void ShowPanel(GameObject panel)
        {
            foreach (GameObject p in managedPanels)
            {
                if (p != null)
                    p.SetActive(p == panel);
            }
        }

        /// <summary>
        /// Show one panel and hide all others in the managed list, by index.
        /// </summary>
        /// <param name="panelIndex">Index of the panel to show in managedPanels.</param>
        public void ShowPanelByIndex(int panelIndex)
        {
            if (panelIndex < 0 || panelIndex >= managedPanels.Count)
                return;

            ShowPanel(managedPanels[panelIndex]);
        }

        /// <summary>
        /// Fade a CanvasGroup from its current alpha to 1 over the given duration.
        /// </summary>
        /// <param name="cg">The CanvasGroup to fade.</param>
        /// <param name="duration">Duration in seconds.</param>
        public void FadeIn(CanvasGroup cg, float duration)
        {
            if (cg == null) return;
            StartCoroutine(FadeCoroutine(cg, 1f, duration));
        }

        /// <summary>
        /// Fade a CanvasGroup from its current alpha to 0 over the given duration.
        /// </summary>
        /// <param name="cg">The CanvasGroup to fade.</param>
        /// <param name="duration">Duration in seconds.</param>
        public void FadeOut(CanvasGroup cg, float duration)
        {
            if (cg == null) return;
            StartCoroutine(FadeCoroutine(cg, 0f, duration));
        }

        /// <summary>
        /// Core fade coroutine.
        /// </summary>
        private IEnumerator FadeCoroutine(CanvasGroup cg, float targetAlpha, float duration)
        {
            float startAlpha = cg.alpha;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            cg.alpha = targetAlpha;
        }

        /// <summary>
        /// Typewriter effect: reveal a string one character at a time in a TextMeshProUGUI field.
        /// </summary>
        /// <param name="textField">The text field to write to.</param>
        /// <param name="message">The full message to display.</param>
        /// <param name="charDelay">Seconds between each character reveal.</param>
        /// <returns>IEnumerator for coroutine usage.</returns>
        public IEnumerator TypewriterText(TMPro.TextMeshProUGUI textField, string message, float charDelay)
        {
            if (textField == null || string.IsNullOrEmpty(message))
                yield break;

            textField.text = string.Empty;

            for (int i = 0; i < message.Length; i++)
            {
                textField.text += message[i];
                yield return new WaitForSeconds(charDelay);
            }
        }

        /// <summary>
        /// Quick shake animation on a Button to indicate wrong/empty input.
        /// </summary>
        /// <param name="btn">The button to shake.</param>
        public void ShakeButton(Button btn)
        {
            if (btn == null) return;
            StartCoroutine(ShakeCoroutine(btn));
        }

        /// <summary>
        /// Core shake coroutine — rapidly oscillates the button's anchored position.
        /// </summary>
        private IEnumerator ShakeCoroutine(Button btn)
        {
            RectTransform rt = btn.GetComponent<RectTransform>();
            if (rt == null) yield break;

            Vector2 originalPos = rt.anchoredPosition;
            float shakeMagnitude = 8f;
            float shakeDuration = 0.3f;
            float elapsed = 0f;

            while (elapsed < shakeDuration)
            {
                float offsetX = Mathf.Sin(elapsed * 60f) * shakeMagnitude * (1f - elapsed / shakeDuration);
                rt.anchoredPosition = originalPos + new Vector2(offsetX, 0f);
                elapsed += Time.deltaTime;
                yield return null;
            }

            rt.anchoredPosition = originalPos;
        }
    }
}