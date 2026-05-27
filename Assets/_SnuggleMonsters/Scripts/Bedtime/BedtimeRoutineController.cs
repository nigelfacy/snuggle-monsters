using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SnuggleMonsters.Monster;

namespace SnuggleMonsters.Bedtime
{
    /// <summary>
    /// Controls the bedtime routine sequence — a linear, locked-step process
    /// consisting of Brush Fluff, Choose Night-Light, Tuck In, and Lullaby.
    /// Automatically saves when the routine completes.
    /// </summary>
    public class BedtimeRoutineController : MonoBehaviour
    {
        [Header("Monster References")]
        [Tooltip("The monster's animation controller for gesture playback.")]
        public MonsterAnimatorController monsterAnim;

        [Tooltip("The persistent monster runtime model (for name, favourite colour, etc.).")]
        public MonsterRuntimeModel monsterModel;

        [Header("Routine Buttons")]
        [Tooltip("Button to start Brush Fluff step.")]
        public Button brushButton;

        [Tooltip("Button to start Choose Night-Light step.")]
        public Button nightLightButton;

        [Tooltip("Button to start Tuck In step.")]
        public Button tuckInButton;

        [Tooltip("Button to start Lullaby step.")]
        public Button lullabyButton;

        [Header("Night-Light")]
        [Tooltip("The night-light controller (changes colour, glow, etc.).")]
        public NightLightController nightLightCtrl;

        [Tooltip("Colour swatch picker Image — contains child swatch buttons.")]
        public Image nightLightColorPicker;

        [Header("Tuck-In")]
        [Tooltip("Blanket GameObject — shown when tuck-in step activates.")]
        public GameObject blanketObject;

        [Header("UI Text")]
        [Tooltip("Text that displays the current step's description.")]
        public TextMeshProUGUI routineText;

        [Tooltip("Text that displays the final goodnight message.")]
        public TextMeshProUGUI goodnightText;

        [Header("Colour Swatches")]
        [Tooltip("Available night-light colours (pink, blue, green, yellow, purple).")]
        public Color[] availableColors = new Color[]
        {
            new Color(1f, 0.41f, 0.71f), // Pink
            new Color(0.53f, 0.81f, 1f), // Blue
            new Color(0.56f, 0.93f, 0.56f), // Green
            new Color(1f, 1f, 0.4f), // Yellow
            new Color(0.82f, 0.63f, 1f) // Purple
        };

        [Tooltip("Colour name labels matching availableColors order.")]
        public string[] colorNames = new string[] { "Pink", "Blue", "Green", "Yellow", "Purple" };

        // Routine step enum.
        public enum RoutineStep
        {
            NotStarted,
            BrushFluff,
            ChooseLight,
            TuckIn,
            Lullaby,
            Complete
        }

        private RoutineStep currentStep = RoutineStep.NotStarted;
        private bool awaitingColorChoice = false;

        // Cached colour swatch buttons.
        private Button[] swatchButtons;

        private void Start()
        {
            // Start with only the first step enabled.
            UpdateButtonInteractability();

            // Wire up step buttons.
            if (brushButton != null)
                brushButton.onClick.AddListener(DoBrushFluff);

            if (nightLightButton != null)
                nightLightButton.onClick.AddListener(DoChooseNightLight);

            if (tuckInButton != null)
                tuckInButton.onClick.AddListener(DoTuckIn);

            if (lullabyButton != null)
                lullabyButton.onClick.AddListener(DoLullaby);

            // Hide blanket initially.
            if (blanketObject != null)
                blanketObject.SetActive(false);

            // Hide goodnight text initially.
            if (goodnightText != null)
                goodnightText.gameObject.SetActive(false);

            // Find colour swatch buttons inside the night-light color picker.
            if (nightLightColorPicker != null)
            {
                swatchButtons = nightLightColorPicker.GetComponentsInChildren<Button>();
                nightLightColorPicker.gameObject.SetActive(false);

                // Wire up swatch listeners.
                for (int i = 0; i < swatchButtons.Length; i++)
                {
                    int index = i; // Capture closure.
                    swatchButtons[i].onClick.AddListener(() => OnColorSwatchClicked(index));
                }
            }

            // Set initial routine text.
            if (routineText != null)
            {
                routineText.text = "Time for bed! Let's get cozy...";
            }

            // Auto-start the first step.
            currentStep = RoutineStep.BrushFluff;
            UpdateButtonInteractability();
        }

        /// <summary>
        /// Update button interactability based on the current routine step.
        /// Only the current step's button is enabled.
        /// </summary>
        private void UpdateButtonInteractability()
        {
            if (brushButton != null)
                brushButton.interactable = (currentStep == RoutineStep.BrushFluff);

            if (nightLightButton != null)
                nightLightButton.interactable = (currentStep == RoutineStep.ChooseLight);

            if (tuckInButton != null)
                tuckInButton.interactable = (currentStep == RoutineStep.TuckIn);

            if (lullabyButton != null)
                lullabyButton.interactable = (currentStep == RoutineStep.Lullaby);
        }

        // ---- Step 1: Brush Fluff ----

        private void DoBrushFluff()
        {
            if (currentStep != RoutineStep.BrushFluff)
                return;

            // Animate brushing.
            if (monsterAnim != null)
                monsterAnim.BrushFluff();

            // Update text.
            if (routineText != null)
                routineText.text = "Time to brush your fluff! So soft!";

            // Advance to next step.
            currentStep = RoutineStep.ChooseLight;
            UpdateButtonInteractability();
        }

        // ---- Step 2: Choose Night-Light ----

        private void DoChooseNightLight()
        {
            if (currentStep != RoutineStep.ChooseLight)
                return;

            // Show colour swatches.
            if (nightLightColorPicker != null)
                nightLightColorPicker.gameObject.SetActive(true);

            // Monster says its favourite colour.
            string favColorName = GetColorName(monsterModel?.favouriteColor ?? availableColors[0]);
            if (routineText != null && monsterModel != null)
            {
                routineText.text = $"{monsterModel.monsterName}'s favourite colour is {favColorName}! Pick a night-light colour!";
            }

            awaitingColorChoice = true;
        }

        /// <summary>
        /// Called when a colour swatch is clicked.
        /// </summary>
        /// <param name="swatchIndex">Index into availableColors.</param>
        private void OnColorSwatchClicked(int swatchIndex)
        {
            if (!awaitingColorChoice || currentStep != RoutineStep.ChooseLight)
                return;

            if (swatchIndex < 0 || swatchIndex >= availableColors.Length)
                return;

            // Change night-light colour.
            if (nightLightCtrl != null)
            {
                Color chosen = availableColors[swatchIndex];
                nightLightCtrl.SetColor(chosen);
            }

            // Hide the picker.
            if (nightLightColorPicker != null)
                nightLightColorPicker.gameObject.SetActive(false);

            awaitingColorChoice = false;

            // Advance to next step.
            currentStep = RoutineStep.TuckIn;
            UpdateButtonInteractability();
        }

        // ---- Step 3: Tuck In ----

        private void DoTuckIn()
        {
            if (currentStep != RoutineStep.TuckIn)
                return;

            // Show blanket.
            if (blanketObject != null)
                blanketObject.SetActive(true);

            // Update text.
            if (routineText != null)
                routineText.text = "Nighty-night, little one!";

            // Advance to next step.
            currentStep = RoutineStep.Lullaby;
            UpdateButtonInteractability();
        }

        // ---- Step 4: Lullaby ----

        private void DoLullaby()
        {
            if (currentStep != RoutineStep.Lullaby)
                return;

            // Play sway animation.
            if (monsterAnim != null)
                monsterAnim.Sway();

            // Show goodnight message.
            string monsterName = monsterModel != null ? monsterModel.monsterName : "Fluff";
            if (routineText != null)
                routineText.text = $"Goodnight, {monsterName}! Sweet dreams!";

            if (goodnightText != null)
            {
                goodnightText.text = $"Goodnight, {monsterName}! Sweet dreams!";
                goodnightText.gameObject.SetActive(true);
            }

            // Mark complete.
            currentStep = RoutineStep.Complete;
            UpdateButtonInteractability();

            // Save game.
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SaveGame();
            }
        }

        /// <summary>
        /// Get a human-readable name for a colour (best match).
        /// </summary>
        private string GetColorName(Color color)
        {
            float bestDistance = float.MaxValue;
            int bestIndex = 0;

            for (int i = 0; i < availableColors.Length; i++)
            {
                float dist = ColorDistance(color, availableColors[i]);
                if (dist < bestDistance)
                {
                    bestDistance = dist;
                    bestIndex = i;
                }
            }

            return colorNames != null && bestIndex < colorNames.Length ? colorNames[bestIndex] : "Pretty";
        }

        /// <summary>
        /// Simple Euclidean distance between two colours in RGB space.
        /// </summary>
        private static float ColorDistance(Color a, Color b)
        {
            float dr = a.r - b.r;
            float dg = a.g - b.g;
            float db = a.b - b.b;
            return dr * dr + dg * dg + db * db;
        }
    }
}