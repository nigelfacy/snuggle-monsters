// <copyright file="ClothingSO.cs" company="SnuggleMonsters">
// Copyright (c) SnuggleMonsters. All rights reserved.
// </copyright>

using UnityEngine;

namespace SnuggleMonsters
{
    /// <summary>
    /// ScriptableObject representing a clothing item that can be equipped on the monster.
    /// </summary>
    [CreateAssetMenu(fileName = "NewClothingItem", menuName = "SnuggleMonsters/Clothing Item", order = 4)]
    public class ClothingSO : ScriptableObject
    {
        /// <summary>Unique identifier for save/load serialization.</summary>
        [SerializeField] private string id;
        /// <summary>Human-readable display name.</summary>
        [SerializeField] private string displayName;
        /// <summary>
        /// The sprite rendered on the monster when equipped.
        /// TODO: Replace with final art asset.
        /// </summary>
        [SerializeField] private Sprite sprite;
        /// <summary>
        /// Optional colour tint override. White means use the sprite's original colours.
        /// </summary>
        [SerializeField] private Color colourTint = Color.white;

        /// <summary>Gets the unique identifier.</summary>
        public string Id => id;
        /// <summary>Gets the display name.</summary>
        public string DisplayName => displayName;
        /// <summary>Gets the sprite (may be null if art not yet assigned).</summary>
        public Sprite Sprite => sprite;
        /// <summary>Gets the colour tint.</summary>
        public Color ColourTint => colourTint;
    }
}