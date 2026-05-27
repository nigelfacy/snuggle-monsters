using System.Collections.Generic;
using SnuggleMonsters.Core;
using UnityEngine;
using UnityEngine.UI;

namespace SnuggleMonsters
{
    /// <summary>
    /// Manages room decoration in the Bedroom scene.
    /// Places decoration prefabs at snap points, tracks placement state,
    /// and persists the state to the MonsterRuntimeModel.
    /// </summary>
    public class HouseDecorationController : MonoBehaviour
    {
        // ──────────────────────────────────────────────────────────────────
        //  Serialized Fields
        // ──────────────────────────────────────────────────────────────────

        [Header("Decorations")]
        [Tooltip("All decorations available for placement.")]
        public DecorationSO[] availableDecorations;

        [Header("Snap Points")]
        [Tooltip("All snap points in the room where decorations can be placed.")]
        public SnapPoint[] snapPoints;

        [Header("Containers")]
        [Tooltip("Parent transform under which decoration preview UI/buttons are organised.")]
        public Transform decorationsContainer;

        [Header("UI")]
        [Tooltip("Button to clear all placed decorations.")]
        public Button clearDecosButton;

        // ──────────────────────────────────────────────────────────────────
        //  Private State
        // ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Toggles for each decoration, keyed by decoration ID.
        /// </summary>
        private Dictionary<string, bool> _decorationToggles;

        /// <summary>
        /// Which snap points currently have which decoration placed.
        /// Maps snapId -> decorationId.
        /// </summary>
        private Dictionary<string, string> _placedDecorations;

        /// <summary>
        /// Reference to the runtime model for persisting state.
        /// </summary>
        private MonsterRuntimeModel _model;

        // ──────────────────────────────────────────────────────────────────
        //  Unity Lifecycle
        // ──────────────────────────────────────────────────────────────────

        private void Awake()
        {
            _decorationToggles = new Dictionary<string, bool>();
            _placedDecorations = new Dictionary<string, string>();
            _model = FindObjectOfType<MonsterRuntimeModel>();

            // Initialise toggles (all off by default)
            if (availableDecorations != null)
            {
                foreach (DecorationSO deco in availableDecorations)
                {
                    if (deco != null)
                        _decorationToggles[deco.id] = false;
                }
            }
        }

        private void OnEnable()
        {
            // Wire up clear button
            if (clearDecosButton != null)
            {
                clearDecosButton.onClick.RemoveAllListeners();
                clearDecosButton.onClick.AddListener(ClearAllDecorations);
            }

            // Restore any previously placed decorations from the model
            RestoreFromModel();
        }

        private void OnDisable()
        {
            if (clearDecosButton != null)
                clearDecosButton.onClick.RemoveListener(ClearAllDecorations);
        }

        // ──────────────────────────────────────────────────────────────────
        //  Public Methods
        // ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Places a decoration at the first available snap point that accepts its type.
        /// If a match is found, spawns the prefab and updates state.
        /// </summary>
        /// <param name="deco">The decoration to place.</param>
        /// <returns>True if the decoration was successfully placed.</returns>
        public bool PlaceDecoration(DecorationSO deco)
        {
            if (deco == null || deco.prefab == null)
            {
                Debug.LogWarning("[HouseDecorationController] Cannot place null decoration.");
                return false;
            }

            // Find an available snap point for this decoration type
            SnapPoint target = FindAvailableSnapPoint(deco.decorationType);
            if (target == null)
            {
                Debug.Log($"[HouseDecorationController] No available snap point for decoration type '{deco.decorationType}'.");
                return false;
            }

            // Place it
            target.PlaceDecoration(deco);
            _placedDecorations[target.snapId] = deco.id;
            _decorationToggles[deco.id] = true;

            // Persist to model
            SaveToModel();

            Debug.Log($"[HouseDecorationController] Placed '{deco.displayName}' at snap point '{target.snapId}'.");
            return true;
        }

        /// <summary>
        /// Removes a decoration from a specific snap point by its snap ID.
        /// </summary>
        /// <param name="snapId">The identifier of the snap point to clear.</param>
        public void RemoveDecorationFromSnap(string snapId)
        {
            SnapPoint snap = FindSnapPoint(snapId);
            if (snap == null)
            {
                Debug.LogWarning($"[HouseDecorationController] No snap point found with ID '{snapId}'.");
                return;
            }

            if (!snap.isOccupied || snap.currentDecoration == null)
            {
                Debug.Log($"[HouseDecorationController] Snap point '{snapId}' is already empty.");
                return;
            }

            string decoId = snap.currentDecoration.id;
            snap.RemoveDecoration();
            _placedDecorations.Remove(snapId);
            _decorationToggles[decoId] = false;

            // Persist to model
            SaveToModel();

            Debug.Log($"[HouseDecorationController] Removed decoration from snap point '{snapId}'.");
        }

        /// <summary>
        /// Toggles a decoration on/off. If on, places it at a suitable snap point.
        /// If off, removes it from wherever it was placed.
        /// </summary>
        /// <param name="deco">The decoration to toggle.</param>
        public void ToggleDecoration(DecorationSO deco)
        {
            if (deco == null)
                return;

            bool currentlyOn = _decorationToggles.TryGetValue(deco.id, out bool val) && val;

            if (currentlyOn)
            {
                // Find which snap point has this decoration and remove it
                foreach (var kvp in _placedDecorations)
                {
                    if (kvp.Value == deco.id)
                    {
                        RemoveDecorationFromSnap(kvp.Key);
                        return;
                    }
                }
            }
            else
            {
                PlaceDecoration(deco);
            }
        }

        /// <summary>
        /// Clears all decorations from all snap points.
        /// </summary>
        public void ClearAllDecorations()
        {
            // Iterate over a copy since we're modifying the collection
            List<string> snapIds = new List<string>(_placedDecorations.Keys);
            foreach (string snapId in snapIds)
            {
                RemoveDecorationFromSnap(snapId);
            }

            // Reset all toggles
            foreach (string key in new List<string>(_decorationToggles.Keys))
            {
                _decorationToggles[key] = false;
            }

            // Persist to model
            SaveToModel();

            Debug.Log("[HouseDecorationController] All decorations cleared.");
        }

        /// <summary>
        /// Checks whether a decoration is currently placed anywhere.
        /// </summary>
        public bool IsDecorationPlaced(DecorationSO deco)
        {
            if (deco == null)
                return false;

            return _decorationToggles.TryGetValue(deco.id, out bool val) && val;
        }

        // ──────────────────────────────────────────────────────────────────
        //  Helpers
        // ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Finds the first snap point that accepts the given decoration type
        /// and is not already occupied.
        /// </summary>
        private SnapPoint FindAvailableSnapPoint(DecorationType type)
        {
            if (snapPoints == null)
                return null;

            foreach (SnapPoint snap in snapPoints)
            {
                if (snap != null && snap.CanAccept(type) && !snap.isOccupied)
                    return snap;
            }

            return null;
        }

        /// <summary>
        /// Finds a snap point by its snap ID.
        /// </summary>
        private SnapPoint FindSnapPoint(string snapId)
        {
            if (snapPoints == null || string.IsNullOrEmpty(snapId))
                return null;

            foreach (SnapPoint snap in snapPoints)
            {
                if (snap != null && snap.snapId == snapId)
                    return snap;
            }

            return null;
        }

        // ──────────────────────────────────────────────────────────────────
        //  Persistence (Model)
        // ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Saves the current decoration placement state to the MonsterRuntimeModel.
        /// Uses <see cref="MonsterRuntimeModel.PlaceDecoration"/> for each placed decoration.
        /// </summary>
        private void SaveToModel()
        {
            if (_model == null)
                return;

            // The model tracks placed decorations as a flat list; we clear and re-add.
            // This is a simplified approach — for full snap-point persistence,
            // a dedicated save system should be used.
            foreach (var kvp in _placedDecorations)
            {
                DecorationSO deco = FindDecorationById(kvp.Value);
                if (deco != null)
                    _model.PlaceDecoration(deco);
            }
        }

        /// <summary>
        /// Restores decoration placement state from the MonsterRuntimeModel.
        /// Re-places any saved decorations at their respective snap points.
        /// </summary>
        private void RestoreFromModel()
        {
            if (_model == null || _model.PlacedDecorations == null)
                return;

            // Clear everything first
            ClearAllDecorations();

            // Re-apply each saved decoration
            foreach (DecorationSO deco in _model.PlacedDecorations)
            {
                if (deco == null)
                    continue;

                SnapPoint snap = FindAvailableSnapPoint(deco.decorationType);
                if (snap != null)
                {
                    snap.PlaceDecoration(deco);
                    _placedDecorations[snap.snapId] = deco.id;
                    _decorationToggles[deco.id] = true;
                }
            }
        }

        /// <summary>
        /// Finds a DecorationSO by its ID from the available decorations array.
        /// </summary>
        private DecorationSO FindDecorationById(string id)
        {
            if (availableDecorations == null || string.IsNullOrEmpty(id))
                return null;

            foreach (DecorationSO deco in availableDecorations)
            {
                if (deco != null && deco.id == id)
                    return deco;
            }

            return null;
        }
    }
}