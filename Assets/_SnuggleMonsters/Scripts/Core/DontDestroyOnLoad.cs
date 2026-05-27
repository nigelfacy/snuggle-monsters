// <copyright file="DontDestroyOnLoad.cs" company="SnuggleMonsters">
// Copyright (c) SnuggleMonsters. All rights reserved.
// </copyright>

using UnityEngine;

namespace SnuggleMonsters.Core
{
    /// <summary>
    /// Utility component that persists a GameObject across scene loads.
    /// Attach to GameManager and other persistent objects. On Awake,
    /// calls DontDestroyOnLoad and checks for duplicate instances,
    /// destroying any duplicates found.
    /// </summary>
    public class DontDestroyOnLoad : MonoBehaviour
    {
        /// <summary>
        /// Unique identifier tag used to detect duplicate instances of the same persistent object.
        /// Assign in the Inspector. Objects with the same tag destroy the new instance on Awake.
        /// </summary>
        [SerializeField]
        [Tooltip("Unique tag for duplicate detection. Objects sharing this tag will destroy newer instances.")]
        private string objectTag = string.Empty;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Persists this GameObject and destroys duplicates.
        /// </summary>
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            if (string.IsNullOrEmpty(objectTag))
            {
                Debug.LogWarning($"[DontDestroyOnLoad] objectTag is empty on {gameObject.name}. Duplicate detection disabled.", this);
                return;
            }

            // Find all instances of DontDestroyOnLoad with the same tag
            DontDestroyOnLoad[] allInstances = FindObjectsByType<DontDestroyOnLoad>(FindObjectsSortMode.None);
            int instanceCount = 0;

            foreach (DontDestroyOnLoad instance in allInstances)
            {
                if (instance != this && instance.objectTag == objectTag)
                {
                    instanceCount++;
                }
            }

            // If another instance already exists, destroy this duplicate
            if (instanceCount > 0)
            {
                Debug.Log($"[DontDestroyOnLoad] Duplicate detected for tag '{objectTag}'. Destroying this instance.", this);
                Destroy(gameObject);
            }
        }
    }
}