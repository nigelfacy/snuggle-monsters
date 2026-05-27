using System;
using UnityEngine;
using UnityEngine.UI;

namespace SnuggleMonsters
{
    /// <summary>
    /// Helper class that manages a single category's selection panel in the Monster Creator.
    /// Creates a horizontal row of buttons, one per part option, and highlights the selected one.
    /// </summary>
    public class PartSelectorUI : MonoBehaviour
    {
        // ──────────────────────────────────────────────────────────────────
        //  Serialized Fields
        // ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// The label text shown above this part category (e.g. "Body", "Eyes").
        /// </summary>
        [Header("UI References")]
        [Tooltip("Label text shown above this part category.")]
        public TMPro.TextMeshProUGUI categoryLabel;

        /// <summary>
        /// The RectTransform that serves as the parent for the option buttons.
        /// Should use a HorizontalLayoutGroup for automatic spacing.
        /// </summary>
        [Tooltip("Parent RectTransform for the option buttons.")]
        public RectTransform buttonContainer;

        /// <summary>
        /// Prefab used for each option button. Must have a Button component and
        /// at least an Image (for the colour swatch) and a TextMeshPro label child.
        /// </summary>
        [Tooltip("Prefab for each option button.")]
        public GameObject buttonPrefab;

        // ──────────────────────────────────────────────────────────────────
        //  Private State
        // ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// The currently selected MonsterPartSO, or null if nothing selected.
        /// </summary>
        private MonsterPartSO _selectedPart;

        /// <summary>
        /// Callback invoked when a part is selected.
        /// </summary>
        private Action<MonsterPartSO> _onSelected;

        /// <summary>
        /// Array of dynamically created option buttons.
        /// </summary>
        private PartOptionUI[] _optionButtons;

        // ──────────────────────────────────────────────────────────────────
        //  Public Methods
        // ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Sets up this panel with the given part options.
        /// Destroys any previously created buttons and rebuilds the layout.
        /// </summary>
        /// <param name="options">Array of MonsterPartSO options to display.</param>
        /// <param name="onSelected">Callback invoked when the user selects a part.</param>
        public void Setup(MonsterPartSO[] options, Action<MonsterPartSO> onSelected)
        {
            _onSelected = onSelected;
            _selectedPart = null;

            // Clear existing buttons
            ClearButtons();

            if (options == null || options.Length == 0)
            {
                Debug.LogWarning("[PartSelectorUI] No options provided — panel left empty.");
                return;
            }

            // Create one button per option
            _optionButtons = new PartOptionUI[options.Length];
            for (int i = 0; i < options.Length; i++)
            {
                MonsterPartSO part = options[i];
                if (part == null)
                    continue;

                // Instantiate button
                GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
                PartOptionUI optionUI = btnObj.GetComponent<PartOptionUI>();
                if (optionUI == null)
                    optionUI = btnObj.AddComponent<PartOptionUI>();

                // Configure the button
                optionUI.Setup(part, OnOptionClicked);

                _optionButtons[i] = optionUI;

                // Auto-select the first option
                if (i == 0)
                {
                    OnOptionClicked(part);
                }
            }

            // Force layout rebuild so the HorizontalLayoutGroup recalculates
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(buttonContainer);
        }

        // ──────────────────────────────────────────────────────────────────
        //  Private Methods
        // ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Handles a click on one of the option buttons.
        /// Updates the selection highlight and invokes the callback.
        /// </summary>
        /// <param name="part">The part that was clicked.</param>
        private void OnOptionClicked(MonsterPartSO part)
        {
            if (part == null)
                return;

            _selectedPart = part;

            // Update highlight on all buttons
            if (_optionButtons != null)
            {
                foreach (PartOptionUI opt in _optionButtons)
                {
                    if (opt != null)
                        opt.SetSelected(opt.Part == part);
                }
            }

            // Invoke the callback
            _onSelected?.Invoke(part);
        }

        /// <summary>
        /// Destroys all previously created option buttons and clears the array.
        /// </summary>
        private void ClearButtons()
        {
            if (_optionButtons != null)
            {
                foreach (PartOptionUI opt in _optionButtons)
                {
                    if (opt != null && opt.gameObject != null)
                        Destroy(opt.gameObject);
                }
            }

            _optionButtons = null;

            // Also destroy any leftover children (safety net)
            for (int i = buttonContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(buttonContainer.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// Returns the currently selected MonsterPartSO.
        /// </summary>
        public MonsterPartSO GetSelectedPart()
        {
            return _selectedPart;
        }
    }

    // ──────────────────────────────────────────────────────────────────────
    //  Helper: PartOptionUI
    // ──────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Internal helper component attached to each option button.
    /// Manages the colour swatch, label, and selection highlight.
    /// </summary>
    internal class PartOptionUI : MonoBehaviour
    {
        /// <summary>
        /// The MonsterPartSO this button represents.
        /// </summary>
        public MonsterPartSO Part { get; private set; }

        [Header("References")]
        [SerializeField] private Image _swatchImage;
        [SerializeField] private TMPro.TextMeshProUGUI _labelText;
        [SerializeField] private Image _highlightBorder;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();

            // Auto-find children if not assigned
            if (_swatchImage == null)
                _swatchImage = GetComponentInChildren<Image>();
            if (_labelText == null)
                _labelText = GetComponentInChildren<TMPro.TextMeshProUGUI>();

            // Try to find a highlight border
            if (_highlightBorder == null && transform.childCount > 0)
            {
                foreach (Transform child in transform)
                {
                    Image img = child.GetComponent<Image>();
                    if (img != null && img != _swatchImage)
                    {
                        _highlightBorder = img;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Configures this button to represent the given part,
        /// and sets the click handler.
        /// </summary>
        public void Setup(MonsterPartSO part, Action<MonsterPartSO> onClick)
        {
            Part = part;

            if (_labelText != null)
                _labelText.text = part != null ? part.displayName : "???";

            if (_swatchImage != null && part != null)
                _swatchImage.color = part.color;

            if (_button != null)
            {
                _button.onClick.RemoveAllListeners();
                _button.onClick.AddListener(() => onClick?.Invoke(part));
            }

            SetSelected(false);
        }

        /// <summary>
        /// Toggles the selection highlight on this button.
        /// </summary>
        public void SetSelected(bool selected)
        {
            if (_highlightBorder != null)
                _highlightBorder.enabled = selected;

            // Also bump the scale slightly when selected for visual feedback
            transform.localScale = selected ? Vector3.one * 1.1f : Vector3.one;
        }
    }
}