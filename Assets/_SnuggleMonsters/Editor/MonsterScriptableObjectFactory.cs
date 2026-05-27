// <copyright file="MonsterScriptableObjectFactory.cs" company="SnuggleMonsters">
// Copyright (c) SnuggleMonsters. All rights reserved.
// </copyright>

using UnityEditor;
using UnityEngine;

namespace SnuggleMonsters.Editor
{
    /// <summary>
    /// Editor script that adds menu items under Assets/Create/SnuggleMonsters/
    /// for quickly creating new ScriptableObject instances in the appropriate
    /// subfolder within <c>Assets/_SnuggleMonsters/ScriptableObjects/</c>.
    /// </summary>
    public static class MonsterScriptableObjectFactory
    {
        /// <summary>
        /// Base path for SnuggleMonsters ScriptableObject assets.
        /// All new assets are created relative to this folder.
        /// </summary>
        private const string BaseFolder = "Assets/_SnuggleMonsters/ScriptableObjects";

        // ----- Menu Items -----

        /// <summary>
        /// Creates a new <see cref="MonsterPartSO"/> asset.
        /// </summary>
        [MenuItem("Assets/Create/SnuggleMonsters/Monster Part", priority = 1)]
        private static void CreateMonsterPart()
        {
            MonsterPartSO asset = ScriptableObject.CreateInstance<MonsterPartSO>();
            ProjectWindowUtil.CreateAsset(asset, GetPath("MonsterPart_New.asset"));
        }

        /// <summary>
        /// Creates a new <see cref="MonsterPersonalitySO"/> asset.
        /// </summary>
        [MenuItem("Assets/Create/SnuggleMonsters/Monster Personality", priority = 2)]
        private static void CreateMonsterPersonality()
        {
            MonsterPersonalitySO asset = ScriptableObject.CreateInstance<MonsterPersonalitySO>();
            ProjectWindowUtil.CreateAsset(asset, GetPath("MonsterPersonality_New.asset"));
        }

        /// <summary>
        /// Creates a new <see cref="DanceRecipeSO"/> asset.
        /// </summary>
        [MenuItem("Assets/Create/SnuggleMonsters/Dance Recipe", priority = 3)]
        private static void CreateDanceRecipe()
        {
            DanceRecipeSO asset = ScriptableObject.CreateInstance<DanceRecipeSO>();
            ProjectWindowUtil.CreateAsset(asset, GetPath("DanceRecipe_New.asset"));
        }

        /// <summary>
        /// Creates a new <see cref="ClothingSO"/> asset.
        /// </summary>
        [MenuItem("Assets/Create/SnuggleMonsters/Clothing Item", priority = 4)]
        private static void CreateClothingItem()
        {
            ClothingSO asset = ScriptableObject.CreateInstance<ClothingSO>();
            ProjectWindowUtil.CreateAsset(asset, GetPath("ClothingItem_New.asset"));
        }

        /// <summary>
        /// Creates a new <see cref="DecorationSO"/> asset.
        /// </summary>
        [MenuItem("Assets/Create/SnuggleMonsters/Decoration Item", priority = 5)]
        private static void CreateDecorationItem()
        {
            DecorationSO asset = ScriptableObject.CreateInstance<DecorationSO>();
            ProjectWindowUtil.CreateAsset(asset, GetPath("DecorationItem_New.asset"));
        }

        // ----- Path Helper -----

        /// <summary>
        /// Returns a full asset path for a new ScriptableObject file.
        /// Ensures the base folder exists, creating it if necessary.
        /// </summary>
        /// <param name="fileName">The filename for the new asset (e.g. "MonsterPart_New.asset").</param>
        /// <returns>A full asset-database path.</returns>
        private static string GetPath(string fileName)
        {
            // Ensure the base folder exists
            if (!AssetDatabase.IsValidFolder(BaseFolder))
            {
                // Create parent folders if needed
                string parent = "Assets/_SnuggleMonsters";
                string sub = "ScriptableObjects";

                if (!AssetDatabase.IsValidFolder(parent))
                {
                    AssetDatabase.CreateFolder("Assets", "_SnuggleMonsters");
                }

                if (!AssetDatabase.IsValidFolder($"{parent}/{sub}"))
                {
                    AssetDatabase.CreateFolder(parent, sub);
                }
            }

            return $"{BaseFolder}/{fileName}";
        }
    }
}