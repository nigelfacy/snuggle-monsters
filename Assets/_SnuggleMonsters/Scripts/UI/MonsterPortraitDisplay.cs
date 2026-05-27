using UnityEngine;
using UnityEngine.UI;

namespace SnuggleMonsters.UI
{
    /// <summary>
    /// Displays a simple portrait representation of the monster on UI elements.
    /// For the prototype, this sets the portrait image colour to the monster's
    /// body colour and shows a personality-colour border.
    /// </summary>
    public class MonsterPortraitDisplay : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("The runtime model of the monster to display.")]
        public MonsterRuntimeModel monsterModel;

        [Tooltip("The main portrait Image (colored circle body).")]
        public Image monsterPortrait;

        [Tooltip("Optional border Image for personality colour accent.")]
        public Image personalityBorder;

        [Header("Fallback Visuals")]
        [Tooltip("Optional child Image for left eye.")]
        public Image eyeLeft;

        [Tooltip("Optional child Image for right eye.")]
        public Image eyeRight;

        [Tooltip("Colour for the eyes (default black).")]
        public Color eyeColor = Color.black;

        private void Start()
        {
            RefreshPortrait();
        }

        /// <summary>
        /// Refresh the portrait display to reflect the current monster model state.
        /// Sets body colour on the portrait image and border colour for personality.
        /// </summary>
        public void RefreshPortrait()
        {
            if (monsterModel == null)
            {
                // Try to find the model at runtime.
                if (GameManager.Instance != null)
                {
                    monsterModel = GameManager.Instance.MonsterModel;
                }

                if (monsterModel == null)
                    return;
            }

            // Set portrait body colour.
            if (monsterPortrait != null)
            {
                monsterPortrait.color = monsterModel.bodyPartColor;
            }

            // Set personality border colour.
            if (personalityBorder != null)
            {
                Color borderColor = monsterModel.personalityBorderColor;
                borderColor.a = personalityBorder.color.a; // Preserve original alpha.
                personalityBorder.color = borderColor;
            }

            // Set eye colours (if child images are wired up).
            if (eyeLeft != null)
                eyeLeft.color = eyeColor;

            if (eyeRight != null)
                eyeRight.color = eyeColor;

            Debug.Log("[MonsterPortraitDisplay] Portrait refreshed.");
        }

        /// <summary>
        /// Update the portrait with a specific model reference.
        /// </summary>
        /// <param name="model">The monster runtime model to display.</param>
        public void SetModel(MonsterRuntimeModel model)
        {
            monsterModel = model;
            RefreshPortrait();
        }
    }
}