using UnityEngine;

namespace SnuggleMonsters
{
    /// <summary>
    /// MonoBehaviour representing a position in the room where a decoration can be placed.
    /// </summary>
    public class SnapPoint : MonoBehaviour
    {
        /// <summary>
        /// Unique identifier for this snap point (e.g. "rug_01", "wall_left").
        /// </summary>
        [Header("Identification")]
        [Tooltip("Unique identifier for this snap point.")]
        public string snapId;

        /// <summary>
        /// The type of decoration allowed at this snap point.
        /// </summary>
        [Tooltip("The type of decoration allowed at this snap point.")]
        public DecorationType allowedType;

        /// <summary>
        /// Whether this snap point currently has a decoration placed on it.
        /// </summary>
        [Header("State")]
        [Tooltip("Whether this snap point currently has a decoration placed on it.")]
        public bool isOccupied;

        /// <summary>
        /// Reference to the DecorationSO placed here, or null if empty.
        /// </summary>
        [Tooltip("Reference to the DecorationSO placed here, or null if empty.")]
        public DecorationSO currentDecoration;

        /// <summary>
        /// The transform used as the attachment point (defaults to this.transform).
        /// </summary>
        [Header("Attachment")]
        [Tooltip("The transform used as the attachment point.")]
        public Transform snapTransform;

        /// <summary>
        /// The spawned decoration instance currently parented to this snap point.
        /// </summary>
        private GameObject _placedInstance;

        // ──────────────────────────────────────────────────────────────────
        //  Unity Lifecycle
        // ──────────────────────────────────────────────────────────────────

        private void Awake()
        {
            // Default snapTransform to self if not assigned
            if (snapTransform == null)
                snapTransform = transform;
        }

        // ──────────────────────────────────────────────────────────────────
        //  Public Methods
        // ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Places the given decoration at this snap point. Instantiates its prefab
        /// as a child of <see cref="snapTransform"/> and marks the point as occupied.
        /// If the point is already occupied, the previous decoration is removed first.
        /// </summary>
        /// <param name="deco">The DecorationSO to place.</param>
        public void PlaceDecoration(DecorationSO deco)
        {
            // Remove any existing decoration first
            if (isOccupied)
                RemoveDecoration();

            // Validate prefab
            if (deco == null || deco.prefab == null)
            {
                Debug.LogWarning($"[SnapPoint:{snapId}] Cannot place decoration — SO or prefab is null.");
                return;
            }

            // Instantiate the prefab as a child of the snap transform
            _placedInstance = Instantiate(deco.prefab, snapTransform);
            _placedInstance.transform.localPosition = Vector3.zero;
            _placedInstance.transform.localRotation = Quaternion.identity;

            // Apply tint colour if renderer exists
            SpriteRenderer renderer = _placedInstance.GetComponent<SpriteRenderer>();
            if (renderer != null)
                renderer.color = deco.defaultColor;

            currentDecoration = deco;
            isOccupied = true;

            Debug.Log($"[SnapPoint:{snapId}] Placed decoration '{deco.displayName}'.");
        }

        /// <summary>
        /// Removes the decoration from this snap point. Destroys the spawned
        /// child GameObject and clears the occupied state.
        /// </summary>
        public void RemoveDecoration()
        {
            if (_placedInstance != null)
            {
                Destroy(_placedInstance);
                _placedInstance = null;
            }

            currentDecoration = null;
            isOccupied = false;

            Debug.Log($"[SnapPoint:{snapId}] Decoration removed.");
        }

        /// <summary>
        /// Returns the currently placed decoration instance, if any.
        /// </summary>
        /// <returns>The instantiated decoration GameObject, or null.</returns>
        public GameObject GetPlacedInstance()
        {
            return _placedInstance;
        }

        /// <summary>
        /// Checks whether the given DecorationType is allowed at this snap point.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type matches <see cref="allowedType"/>.</returns>
        public bool CanAccept(DecorationType type)
        {
            return allowedType == type;
        }
    }
}