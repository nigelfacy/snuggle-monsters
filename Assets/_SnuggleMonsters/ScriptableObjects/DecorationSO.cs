// <copyright file="DecorationSO.cs" company="SnuggleMonsters">
// Copyright (c) SnuggleMonsters. All rights reserved.
// </copyright>

using UnityEngine;

namespace SnuggleMonsters
{
    /// <summary>
    /// ScriptableObject representing a decoration item that can be placed in the monster's room.
    /// </summary>
    [CreateAssetMenu(fileName = "NewDecorationItem", menuName = "SnuggleMonsters/Decoration Item", order = 5)]
    public class DecorationSO : ScriptableObject
    {
        /// <summary>Unique identifier for save/load serialization.</summary>
        [SerializeField] private string id;
        /// <summary>Human-readable display name.</summary>
        [SerializeField] private string displayName;
        /// <summary>
        /// The sprite/art for this decoration.
        /// TODO: Replace with final art asset.
        /// </summary>
        [SerializeField] private Sprite sprite;
        /// <summary>Optional colour tint override.</summary>
        [SerializeField] private Color colourTint = Color.white;
        /// <summary>Whether this decoration can be placed on the wall.</summary>
        [SerializeField] private bool isWallDecoration;

        /// <summary>Gets the unique identifier.</summary>
        public string Id => id;
        /// <summary>Gets the display name.</summary>
        public string DisplayName => displayName;
        /// <summary>Gets the sprite (may be null if art not yet assigned).</summary>
        public Sprite Sprite => sprite;
        /// <summary>Gets the colour tint.</summary>
        public Color ColourTint => colourTint;
        /// <summary>Gets whether this is a wall decoration.</summary>
        public bool IsWallDecoration => isWallDecoration;
    }
}