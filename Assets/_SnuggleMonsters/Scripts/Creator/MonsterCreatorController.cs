using System.Collections.Generic;
using SnuggleMonsters.Core;
using SnuggleMonsters.Monster;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SnuggleMonsters
{
    /// <summary>
    /// Main controller for the Monster Creator scene.
    /// Manages body part selection, preview rendering, personality assignment,
    /// and transitions to the Bedroom scene after confirmation.
    /// </summary>
    public class MonsterCreatorController : MonoBehaviour
    {
        // ──────────────────────────────────────────────────────────────────
        //  Serialized Fields — Part Options
        // ──────────────────────────────────────────────────────────────────

        [Header("Body Part Options")]
        [Tooltip("All available body options for the monster.")]
        public MonsterPartSO[] allBodyOptions;

        [Tooltip("All available eye options for the monster.")]
        public MonsterPartSO[] allEyeOptions;

        [Tooltip("All available horn options for the monster.")]
        public MonsterPartSO[] allHornOptions;

        [Tooltip("All available wing options for the monster.")]
        public MonsterPartSO[] allWingOptions;

        [Tooltip("All available tail options for the monster.")]
        public MonsterPartSO[] allTailOptions;

        [Tooltip("All available pattern options for the monster.")]
        public MonsterPartSO[] allPatternOptions;

        [Header("Personality Options")]
        [Tooltip("All available personality types.")]
        public MonsterPersonalitySO[] allPersonalities;

        // ──────────────────────────────────────────────────────────────────
        //  Serialized Fields — UI References
        // ──────────────────────────────────────────────────────────────────

        [Header("UI References")]
        [Tooltip("Input field for the monster's name.")]
        public TMP_InputField nameInput;

        [Tooltip("Button to randomise all selections.")]
        public Button randomiseButton;

        [Tooltip("Button to confirm and proceed to the bedroom.")]
        public Button confirmButton;

        [Header("Preview")]
        [Tooltip("Parent transform under which the preview monster is instantiated.")]
        public Transform monsterPreviewParent;

        [Tooltip("Backdrop/glow Image that shows the personality's favourite colour.")]
        public Image personalityGlow;

        [Header("Part Selector Panels")]
        [Tooltip("PartSelectorUI for body parts.")]
        public PartSelectorUI bodySelector;

        [Tooltip("PartSelectorUI for eye parts.")]
        public PartSelectorUI eyeSelector;

        [Tooltip("PartSelectorUI for horn parts.")]
        public PartSelectorUI hornSelector;

        [Tooltip("PartSelectorUI for wing parts.")]
        public PartSelectorUI wingSelector;

        [Tooltip("PartSelectorUI for tail parts.")]
        public PartSelectorUI tailSelector;

        [Tooltip("PartSelectorUI for pattern parts.")]
        public PartSelectorUI patternSelector;

        [Tooltip("PartSelectorUI for personalities.")]
        public PartSelectorUI personalitySelector;

        [Header("Personality Info")]
        [Tooltip("Text field for displaying the currently selected personality description.")]
        public TMP_Text personalityDescriptionText;

        [Header("Preview Monster Parts")]
        [Tooltip("Child GameObject for the body part preview.")]
        public GameObject previewBody;

        [Tooltip("Child GameObject for the eyes part preview.")]
        public GameObject previewEyes;

        [Tooltip("Child GameObject for the horns part preview.")]
        public GameObject previewHorns;

        [Tooltip("Child GameObject for the wings part preview.")]
        public GameObject previewWings;

        [Tooltip("Child GameObject for the tail part preview.")]
        public GameObject previewTail;

        [Tooltip("Child GameObject for the pattern part preview.")]
        public GameObject previewPattern;

        // ──────────────────────────────────────────────────────────────────
        //  Serialized Fields — Model & Animation
        // ──────────────────────────────────────────────────────────────────

        [Header("Model & Animation")]
        [Tooltip("The runtime monster model. If not assigned, searched via FindObjectOfType.")]
        public MonsterRuntimeModel monsterModel;

        [Tooltip("Animator controller for the preview monster.")]
        public MonsterAnimatorController previewAnimator;

        // ──────────────────────────────────────────────────────────────────
        //  Private State
        // ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Currently selected body part.
        /// </summary>
        private MonsterPartSO _selectedBody;

        /// <summary>
        /// Currently selected eyes part.
        /// </summary>
        private MonsterPartSO _selectedEyes;

        /// <summary>
        /// Currently selected horns part.
        /// </summary>
        private MonsterPartSO _selectedHorns;

        /// <summary>
        /// Currently selected wings part.
        /// </summary>
        private MonsterPartSO _selectedWings;

        /// <summary>
        /// Currently selected tail part.
        /// </summary>
        private MonsterPartSO _selectedTail;

        /// <summary>
        /// Currently selected pattern part.
        /// </summary>
        private MonsterPartSO _selectedPattern;

        /// <summary>
        /// Currently selected personality.
        /// </summary>
        private MonsterPersonalitySO _selectedPersonality;

        // ──────────────────────────────────────────────────────────────────
        //  Unity Lifecycle
        // ──────────────────────────────────────────────────────────────────

        private void Awake()
        {
            // Resolve monsterModel if not assigned
            if (monsterModel == null)
                monsterModel = FindObjectOfType<MonsterRuntimeModel>();

            if (monsterModel == null)
                Debug.LogError("[MonsterCreatorController] No MonsterRuntimeModel found in scene.");
        }

        private void OnEnable()
        {
            // Wire up button listeners
            if (randomiseButton != null)
                randomiseButton.onClick.AddListener(RandomiseButton);

            if (confirmButton != null)
                confirmButton.onClick.AddListener(ConfirmButton);

            // Populate all selection panels
            PopulatePartSelectors();

            // Trigger initial preview
            RefreshPreview();
        }

        private void OnDisable()
        {
            if (randomiseButton != null)
                randomiseButton.onClick.RemoveListener(RandomiseButton);

            if (confirmButton != null)
                confirmButton.onClick.RemoveListener(ConfirmButton);
        }

        // ──────────────────────────────────────────────────────────────────
        //  Initialisation
        // ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Populates all PartSelectorUI panels with their respective options.
        /// </summary>
        private void PopulatePartSelectors()
        {
            if (bodySelector != null)
                bodySelector.Setup(allBodyOptions, part => OnPartSelected(ref _selectedBody, part, previewBody));

            if (eyeSelector != null)
                eyeSelector.Setup(allEyeOptions, part => OnPartSelected(ref _selectedEyes, part, previewEyes));

            if (hornSelector != null)
                hornSelector.Setup(allHornOptions, part => OnPartSelected(ref _selectedHorns, part, previewHorns));

            if (wingSelector != null)
                wingSelector.Setup(allWingOptions, part => OnPartSelected(ref _selectedWings, part, previewWings));

            if (tailSelector != null)
                tailSelector.Setup(allTailOptions, part => OnPartSelected(ref _selectedTail, part, previewTail));

            if (patternSelector != null)
                patternSelector.Setup(allPatternOptions, part => OnPartSelected(ref _selectedPattern, part, previewPattern));

            // Personality selector — wrap MonsterPersonalitySO[] for display
            if (personalitySelector != null && allPersonalities != null)
            {
                // Create a simplified wrapper: we can show personalities as if they were parts
                // but we handle the selection differently
                personalitySelector.Setup(
                    ConvertPersonalitiesToParts(allPersonalities),
                    part => OnPersonalitySelected(FindPersonalityByPart(part))
                );
            }
        }

        /// <summary>
        /// Converts MonsterPersonalitySO array to a display-friendly MonsterPartSO[] for the selector.
        /// We clone key properties so the PartSelectorUI can render them (colour swatch, name).
        /// </summary>
        private MonsterPartSO[] ConvertPersonalitiesToParts(MonsterPersonalitySO[] personalities)
        {
            if (personalities == null || personalities.Length == 0)
                return null;

            MonsterPartSO[] parts = new MonsterPartSO[personalities.Length];
            for (int i = 0; i < personalities.Length; i++)
            {
                MonsterPersonalitySO p = personalities[i];
                if (p == null) continue;

                // Create a temporary ScriptableObject-like wrapper
                MonsterPartSO part = ScriptableObject.CreateInstance<MonsterPartSO>();
                part.name = p.name;
                part.displayName = p.personalityName;
                part.color = p.favouriteColor;
                parts[i] = part;
            }

            return parts;
        }

        /// <summary>
        /// Finds the MonsterPersonalitySO that matches the given display part.
        /// Uses index-based matching since we created parts from personalities.
        /// </summary>
        private MonsterPersonalitySO FindPersonalityByPart(MonsterPartSO part)
        {
            if (part == null || allPersonalities == null)
                return null;

            for (int i = 0; i < allPersonalities.Length; i++)
            {
                if (allPersonalities[i] != null && allPersonalities[i].personalityName == part.displayName)
                    return allPersonalities[i];
            }

            return null;
        }

        // ──────────────────────────────────────────────────────────────────
        //  Part Selection
        // ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Called when any body part is selected from the UI.
        /// Updates the stored reference, refreshes the preview sprite and colour.
        /// </summary>
        /// <param name="selectedField">Reference to the field storing the selected part.</param>
        /// <param name="part">The newly selected MonsterPartSO.</param>
        /// <param name="previewObject">The preview child GameObject to update.</param>
        private void OnPartSelected(ref MonsterPartSO selectedField, MonsterPartSO part, GameObject previewObject)
        {
            if (part == null)
                return;

            selectedField = part;

            // Update the preview
            if (previewObject != null)
            {
                // Enable the preview object
                previewObject.SetActive(true);

                // Update the sprite renderer
                SpriteRenderer renderer = previewObject.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.sprite = part.sprite;
                    renderer.color = part.color;
                }
            }

            // Update the monster model if available
            UpdateMonsterModel();
        }

        // ──────────────────────────────────────────────────────────────────
        //  Personality Selection
        // ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Called when a personality is selected. Updates the model and shows
        /// the personality's description and favourite colour as a glow backdrop.
        /// </summary>
        /// <param name="p">The selected MonsterPersonalitySO.</param>
        private void OnPersonalitySelected(MonsterPersonalitySO p)
        {
            if (p == null)
                return;

            _selectedPersonality = p;

            // Show personality description
            if (personalityDescriptionText != null)
                personalityDescriptionText.text = p.description;

            // Update backdrop glow colour
            if (personalityGlow != null)
            {
                personalityGlow.color = p.favouriteColor;
                personalityGlow.enabled = true;
            }

            // Update the monster model
            UpdateMonsterModel();
        }

        // ──────────────────────────────────────────────────────────────────
        //  Preview Refresh
        // ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Refreshes the entire preview monster based on current selections.
        /// Enables/disables child GameObjects and applies colours.
        /// </summary>
        private void RefreshPreview()
        {
            ApplyPreviewPart(previewBody, _selectedBody);
            ApplyPreviewPart(previewEyes, _selectedEyes);
            ApplyPreviewPart(previewHorns, _selectedHorns);
            ApplyPreviewPart(previewWings, _selectedWings);
            ApplyPreviewPart(previewTail, _selectedTail);
            ApplyPreviewPart(previewPattern, _selectedPattern);

            // Update glow
            if (personalityGlow != null && _selectedPersonality != null)
            {
                personalityGlow.color = _selectedPersonality.favouriteColor;
                personalityGlow.enabled = true;
            }
            else if (personalityGlow != null)
            {
                personalityGlow.enabled = false;
            }
        }

        /// <summary>
        /// Applies the given part data to a preview child GameObject.
        /// </summary>
        private void ApplyPreviewPart(GameObject previewObj, MonsterPartSO part)
        {
            if (previewObj == null)
                return;

            if (part == null)
            {
                previewObj.SetActive(false);
                return;
            }

            previewObj.SetActive(true);
            SpriteRenderer renderer = previewObj.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sprite = part.sprite;
                renderer.color = part.color;
            }
        }

        // ──────────────────────────────────────────────────────────────────
        //  Model Sync
        // ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Writes the currently selected parts and personality into the
        /// MonsterRuntimeModel so other systems can read them.
        /// Uses <see cref="MonsterRuntimeModel.SetPart"/> and <see cref="MonsterRuntimeModel.SetPersonality"/>.
        /// </summary>
        private void UpdateMonsterModel()
        {
            if (monsterModel == null)
                return;

            monsterModel.SetPart(_selectedBody);
            monsterModel.SetPart(_selectedEyes);
            monsterModel.SetPart(_selectedHorns);
            monsterModel.SetPart(_selectedWings);
            monsterModel.SetPart(_selectedTail);
            monsterModel.SetPart(_selectedPattern);
            monsterModel.SetPersonality(_selectedPersonality);
        }

        // ──────────────────────────────────────────────────────────────────
        //  Button Actions
        // ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Randomises all part and personality selections, then refreshes the preview.
        /// </summary>
        public void RandomiseButton()
        {
            // Pick random parts
            _selectedBody = GetRandom(allBodyOptions);
            _selectedEyes = GetRandom(allEyeOptions);
            _selectedHorns = GetRandom(allHornOptions);
            _selectedWings = GetRandom(allWingOptions);
            _selectedTail = GetRandom(allTailOptions);
            _selectedPattern = GetRandom(allPatternOptions);
            _selectedPersonality = GetRandom(allPersonalities);

            // Update selector highlights
            if (bodySelector != null)
                bodySelector.Setup(allBodyOptions, part => OnPartSelected(ref _selectedBody, part, previewBody));
            if (eyeSelector != null)
                eyeSelector.Setup(allEyeOptions, part => OnPartSelected(ref _selectedEyes, part, previewEyes));
            if (hornSelector != null)
                hornSelector.Setup(allHornOptions, part => OnPartSelected(ref _selectedHorns, part, previewHorns));
            if (wingSelector != null)
                wingSelector.Setup(allWingOptions, part => OnPartSelected(ref _selectedWings, part, previewWings));
            if (tailSelector != null)
                tailSelector.Setup(allTailOptions, part => OnPartSelected(ref _selectedTail, part, previewTail));
            if (patternSelector != null)
                patternSelector.Setup(allPatternOptions, part => OnPartSelected(ref _selectedPattern, part, previewPattern));
            if (personalitySelector != null && allPersonalities != null)
                personalitySelector.Setup(
                    ConvertPersonalitiesToParts(allPersonalities),
                    part => OnPersonalitySelected(FindPersonalityByPart(part))
                );

            // Refresh the preview visuals
            RefreshPreview();
            UpdateMonsterModel();

            // Update personality description
            if (personalityDescriptionText != null && _selectedPersonality != null)
                personalityDescriptionText.text = _selectedPersonality.description;

            Debug.Log("[MonsterCreatorController] Randomised all selections.");
        }

        /// <summary>
        /// Confirms the current selection, saves everything to the MonsterRuntimeModel,
        /// generates a special dance name, and transitions to the Bedroom scene.
        /// </summary>
        public void ConfirmButton()
        {
            // Validate / default the name
            string monsterName = nameInput != null ? nameInput.text.Trim() : string.Empty;
            if (string.IsNullOrEmpty(monsterName))
            {
                monsterName = "Fluffles";
                if (nameInput != null)
                    nameInput.text = monsterName;
            }

            if (monsterModel == null)
            {
                Debug.LogError("[MonsterCreatorController] Cannot confirm — no MonsterRuntimeModel available.");
                return;
            }

            // Save name and parts to model
            monsterModel.MonsterName = monsterName;
            UpdateMonsterModel();

            // Generate a special dance name from parts + personality
            MonsterPartSO[] allParts = new MonsterPartSO[]
            {
                _selectedBody, _selectedEyes, _selectedHorns,
                _selectedWings, _selectedTail, _selectedPattern
            };
            monsterModel.SpecialDanceName = SpecialDanceResolver.GenerateDanceName(allParts, _selectedPersonality);

            Debug.Log($"[MonsterCreatorController] Confirmed! Monster '{monsterName}' — Dance: '{monsterModel.SpecialDanceName}'");

            // Transition to the Bedroom scene via GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GoToScene(GameManager.GameScene.Bedroom);
            }
            else
            {
                Debug.LogError("[MonsterCreatorController] No GameManager found — cannot transition.");
            }
        }

        // ──────────────────────────────────────────────────────────────────
        //  Helpers
        // ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns a random element from an array, or null if the array is null/empty.
        /// </summary>
        private T GetRandom<T>(T[] array) where T : class
        {
            if (array == null || array.Length == 0)
                return null;

            return array[Random.Range(0, array.Length)];
        }
    }
}