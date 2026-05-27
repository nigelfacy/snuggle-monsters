// <copyright file="SceneController.cs" company="SnuggleMonsters">
// Copyright (c) SnuggleMonsters. All rights reserved.
// </copyright>

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SnuggleMonsters.Core
{
    /// <summary>
    /// Handles scene loading with a fade-to-black transition effect.
    /// Uses a simple Canvas overlay whose alpha is tweened via a coroutine.
    /// No external tweening library required — uses <see cref="Mathf.Lerp"/>.
    /// </summary>
    public class SceneController : MonoBehaviour
    {
        // ----- Fade Configuration -----

        [Header("Fade Settings")]

        /// <summary>Duration of the fade-out transition in seconds.</summary>
        [SerializeField]
        [Tooltip("Duration of the fade-out (scene exit) in seconds.")]
        private float fadeOutDuration = 0.5f;

        /// <summary>Duration of the fade-in transition in seconds.</summary>
        [SerializeField]
        [Tooltip("Duration of the fade-in (scene enter) in seconds.")]
        private float fadeInDuration = 0.5f;

        /// <summary>
        /// Reference to the Canvas Group controlling the fade overlay.
        /// If null, a default overlay is created at runtime.
        /// </summary>
        [SerializeField]
        [Tooltip("Canvas Group for the fade overlay. Created automatically if null.")]
        private CanvasGroup fadeCanvasGroup;

        // ----- Private State -----

        /// <summary>Whether a scene transition is currently in progress.</summary>
        private bool isTransitioning = false;

        /// <summary>The alpha tween coroutine, stored so it can be stopped if needed.</summary>
        private Coroutine activeFadeCoroutine;

        // ----- Public Methods -----

        /// <summary>
        /// Loads a scene by name with a fade-to-black effect.
        /// The onComplete callback is invoked after the fade-in finishes.
        /// </summary>
        /// <param name="sceneName">Name of the scene to load (must be in Build Settings).</param>
        /// <param name="onComplete">Optional callback invoked after the fade-in completes.</param>
        public void LoadScene(string sceneName, Action onComplete = null)
        {
            if (isTransitioning)
            {
                Debug.LogWarning($"[SceneController] A scene transition is already in progress. Ignoring LoadScene('{sceneName}').");
                return;
            }

            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("[SceneController] Cannot load scene: sceneName is null or empty.");
                return;
            }

            // Initialise the fade overlay if needed
            if (fadeCanvasGroup == null)
            {
                CreateDefaultFadeOverlay();
            }

            // Ensure the overlay blocks raycasts during the transition
            fadeCanvasGroup.blocksRaycasts = true;

            StartCoroutine(TransitionCoroutine(sceneName, onComplete));
        }

        /// <summary>
        /// Immediately sets the fade overlay alpha without any transition.
        /// Useful for instant-start scenarios (e.g. Boot scene).
        /// </summary>
        /// <param name="alpha">Target alpha (0 = transparent, 1 = fully opaque).</param>
        public void SetFadeAlpha(float alpha)
        {
            if (fadeCanvasGroup == null)
            {
                CreateDefaultFadeOverlay();
            }

            fadeCanvasGroup.alpha = Mathf.Clamp01(alpha);
        }

        // ----- Coroutine -----

        /// <summary>
        /// Coroutine that fades out, loads the new scene, then fades in.
        /// </summary>
        private IEnumerator TransitionCoroutine(string sceneName, Action onComplete)
        {
            isTransitioning = true;

            // 1. Fade out
            yield return FadeAlpha(1f, fadeOutDuration);
            fadeCanvasGroup.blocksRaycasts = true;

            // 2. Load the scene
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName);
            while (!loadOp.isDone)
            {
                yield return null;
            }

            // 3. Re-establish references after scene load if needed
            yield return null;

            // 4. Fade in
            yield return FadeAlpha(0f, fadeInDuration);
            fadeCanvasGroup.blocksRaycasts = false;

            isTransitioning = false;

            // 5. Invoke callback
            onComplete?.Invoke();
        }

        /// <summary>
        /// Smoothly interpolates the fade overlay alpha from current to target.
        /// </summary>
        private IEnumerator FadeAlpha(float targetAlpha, float duration)
        {
            if (fadeCanvasGroup == null)
            {
                yield break;
            }

            float startAlpha = fadeCanvasGroup.alpha;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                yield return null;
            }

            fadeCanvasGroup.alpha = targetAlpha;
        }

        // ----- Overlay Creation -----

        /// <summary>
        /// Creates a default full-screen fade overlay GameObject at runtime.
        /// Uses a Canvas with an Image child set to black.
        /// </summary>
        private void CreateDefaultFadeOverlay()
        {
            GameObject overlayGO = new GameObject("FadeOverlay");

            // Ensure it persists across scene loads
            DontDestroyOnLoad(overlayGO);

            // Canvas setup
            Canvas canvas = overlayGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 9999;

            // Canvas scaler for consistent sizing
            CanvasScaler scaler = overlayGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            // Image (full screen black)
            GameObject imageGO = new GameObject("FadeImage");
            imageGO.transform.SetParent(overlayGO.transform, false);

            RectTransform rectTransform = imageGO.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;

            Image image = imageGO.AddComponent<Image>();
            image.color = Color.black;

            // Canvas Group on the overlay root
            fadeCanvasGroup = overlayGO.AddComponent<CanvasGroup>();
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
            fadeCanvasGroup.interactable = false;

            Debug.Log("[SceneController] Created default fade overlay.");
        }

        // ----- Gizmos / Editor -----

        private void OnValidate()
        {
            fadeOutDuration = Mathf.Max(0.01f, fadeOutDuration);
            fadeInDuration = Mathf.Max(0.01f, fadeInDuration);
        }
    }
}