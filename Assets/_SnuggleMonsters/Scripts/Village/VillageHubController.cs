using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SnuggleMonsters.Village
{
    /// <summary>
    /// Simple village hub scene controller managing three clickable locations
    /// (GiggleGarden, PillowPond, Button Bakery) each with its own description
    /// and an interactable NPC.  Back button returns to the Bedroom scene.
    /// </summary>
    public class VillageHubController : MonoBehaviour
    {
        [Header("Location Buttons")]
        [Tooltip("Button to jump to GiggleGarden panel.")]
        public Button giggleGardenButton;

        [Tooltip("Button to jump to PillowPond panel.")]
        public Button pillowPondButton;

        [Tooltip("Button to jump to Button Bakery panel.")]
        public Button buttonBakeryButton;

        [Tooltip("Button to return to the Bedroom scene.")]
        public Button backToBedroomButton;

        [Header("UI Elements")]
        [Tooltip("Array of panel GameObjects corresponding to each location (must match order: Garden, Pond, Bakery).")]
        public GameObject[] locationPanels = new GameObject[3];

        [Tooltip("Text element displaying the current location's title.")]
        public TextMeshProUGUI locationTitleText;

        [Tooltip("Text element displaying the current location's description.")]
        public TextMeshProUGUI locationDescText;

        [Header("NPCs")]
        [Tooltip("NPC interactables for each location (same order as panels).")]
        public NPCInteractable[] npcs = new NPCInteractable[3];

        // Location data.
        private struct LocationData
        {
            public string title;
            public string description;
        }

        private readonly LocationData[] locations = new LocationData[]
        {
            new LocationData
            {
                title = "Giggle Garden",
                description = "Where the flowers giggle when you tickle them"
            },
            new LocationData
            {
                title = "Pillow Pond",
                description = "The softest pond in the whole world — you can nap on lily pads!"
            },
            new LocationData
            {
                title = "Button Bakery",
                description = "The baker makes the warmest, cuddliest button biscuits"
            }
        };

        private int currentLocationIndex = -1;

        private void Start()
        {
            // Wire up button listeners.
            if (giggleGardenButton != null)
                giggleGardenButton.onClick.AddListener(() => ShowLocation(0));

            if (pillowPondButton != null)
                pillowPondButton.onClick.AddListener(() => ShowLocation(1));

            if (buttonBakeryButton != null)
                buttonBakeryButton.onClick.AddListener(() => ShowLocation(2));

            if (backToBedroomButton != null)
                backToBedroomButton.onClick.AddListener(GoBackToBedroom);

            // Start with all panels hidden.
            HideAllPanels();
        }

        /// <summary>
        /// Display the location panel, title, description, and NPC for the given index.
        /// </summary>
        /// <param name="index">0 = GiggleGarden, 1 = PillowPond, 2 = Button Bakery.</param>
        private void ShowLocation(int index)
        {
            if (index < 0 || index >= locations.Length)
                return;

            currentLocationIndex = index;

            // Show panel.
            HideAllPanels();

            if (locationPanels != null && index < locationPanels.Length && locationPanels[index] != null)
            {
                locationPanels[index].SetActive(true);
            }

            // Update text.
            if (locationTitleText != null)
                locationTitleText.text = locations[index].title;

            if (locationDescText != null)
                locationDescText.text = locations[index].description;

            // Enable the corresponding NPC interactable.
            for (int i = 0; i < (npcs?.Length ?? 0); i++)
            {
                if (npcs[i] != null)
                    npcs[i].gameObject.SetActive(i == index);
            }
        }

        /// <summary>
        /// Deactivate all location panels.
        /// </summary>
        private void HideAllPanels()
        {
            if (locationPanels == null)
                return;

            foreach (GameObject panel in locationPanels)
            {
                if (panel != null)
                    panel.SetActive(false);
            }
        }

        /// <summary>
        /// Return to the Bedroom scene via GameManager.
        /// </summary>
        private void GoBackToBedroom()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GoToScene(GameManager.GameScene.Bedroom);
            }
        }
    }
}