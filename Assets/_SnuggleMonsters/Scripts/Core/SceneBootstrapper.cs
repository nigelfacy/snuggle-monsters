using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using SnuggleMonsters.SaveLoad;

namespace SnuggleMonsters.Core
{
    /// <summary>
    /// Attached to the Boot scene.  On Start, this bootstrapper:
    /// 1. Initialises the GameManager singleton.
    /// 2. Checks whether a save file exists.
    /// 3. No save -> transitions to MonsterCreator scene.
    /// 4. Save exists -> transitions to Bedroom scene (loads monster from save).
    /// 5. Shows a brief "Snuggle Monsters!" title screen for 2 seconds before transitioning.
    /// 
    /// Creates its own basic UI canvas if one does not already exist in the scene.
    /// </summary>
    public class SceneBootstrapper : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("Title text (e.g. 'Snuggle Monsters!'). Auto-created if left null.")]
        public TextMeshProUGUI titleText;

        [Tooltip("Subtitle text (e.g. 'Loading...'). Auto-created if left null.")]
        public TextMeshProUGUI subtitleText;

        [Header("Boot Settings")]
        [Tooltip("Duration to show the title screen before transitioning.")]
        public float titleScreenDuration = 2f;

        [Tooltip("Optional Canvas reference. Auto-created if null.")]
        public Canvas bootCanvas;

        [Header("Scene Names")]
        [Tooltip("Name of the Monster Creator scene.")]
        public string monsterCreatorScene = "MonsterCreator";

        [Tooltip("Name of the Bedroom scene.")]
        public string bedroomScene = "Bedroom";

        private void Start()
        {
            StartCoroutine(BootSequence());
        }

        /// <summary>
        /// The full boot sequence: initialise, show title, check save, transition.
        /// </summary>
        private IEnumerator BootSequence()
        {
            // 1. Initialise GameManager singleton.
            Debug.Log("[SceneBootstrapper] Initialising GameManager...");
            GameManager.Instance.Initialise();

            // 2. Ensure the boot canvas and text exist.
            EnsureBootUI();

            // 3. Set title text.
            if (titleText != null)
                titleText.text = "Snuggle Monsters!";

            if (subtitleText != null)
                subtitleText.text = "Loading...";

            // 4. Briefly display the title screen.
            Debug.Log($"[SceneBootstrapper] Showing title screen for {titleScreenDuration}s...");
            yield return new WaitForSeconds(titleScreenDuration);

            // 5. Check for save and route accordingly.
            bool saveExists = SaveLoadService.Instance.SaveExists();
            string targetScene;

            if (saveExists)
            {
                Debug.Log("[SceneBootstrapper] Save found. Loading monster data and transitioning to Bedroom.");
                GameManager.Instance.LoadGame();
                targetScene = bedroomScene;
            }
            else
            {
                Debug.Log("[SceneBootstrapper] No save found. Transitioning to Monster Creator.");
                targetScene = monsterCreatorScene;
            }

            // 6. Transition to the target scene.
            if (subtitleText != null)
                subtitleText.text = $"Loading {targetScene}...";

            yield return new WaitForSeconds(0.25f); // Brief pause so the user sees the destination label.

            SceneManager.LoadScene(targetScene);
        }

        /// <summary>
        /// If no canvas / text objects are assigned, create them programmatically.
        /// </summary>
        private void EnsureBootUI()
        {
            if (bootCanvas == null)
            {
                GameObject canvasGO = new GameObject("BootCanvas");
                bootCanvas = canvasGO.AddComponent<Canvas>();
                bootCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<CanvasScaler>();
                canvasGO.AddComponent<GraphicRaycaster>();
            }

            if (titleText == null)
            {
                GameObject titleGO = new GameObject("BootTitleText");
                titleGO.transform.SetParent(bootCanvas.transform, false);

                titleText = titleGO.AddComponent<TextMeshProUGUI>();
                titleText.text = "Snuggle Monsters!";
                titleText.fontSize = 48;
                titleText.alignment = TextAlignmentOptions.Center;
                titleText.color = Color.white;

                RectTransform rt = titleGO.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = new Vector2(600f, 100f);
            }

            if (subtitleText == null)
            {
                GameObject subGO = new GameObject("BootSubtitleText");
                subGO.transform.SetParent(bootCanvas.transform, false);

                subtitleText = subGO.AddComponent<TextMeshProUGUI>();
                subtitleText.text = "Loading...";
                subtitleText.fontSize = 24;
                subtitleText.alignment = TextAlignmentOptions.Center;
                subtitleText.color = new Color(0.8f, 0.8f, 0.8f);

                RectTransform rt = subGO.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = new Vector2(0f, -60f);
                rt.sizeDelta = new Vector2(400f, 60f);
            }
        }
    }
}