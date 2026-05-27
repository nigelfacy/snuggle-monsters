using System.IO;
using UnityEngine;
using SnuggleMonsters.Core;

namespace SnuggleMonsters.SaveLoad
{
    /// <summary>
    /// JSON-based persistence service for saving and loading MonsterRuntimeModel data.
    /// Uses Application.persistentDataPath as the storage root.
    /// All ScriptableObjects referenced via IDs must reside in a Resources/ folder
    /// for runtime lookup via Resources.LoadAll.
    /// </summary>
    public class SaveLoadService : MonoBehaviour
    {
        // Singleton instance.
        private static SaveLoadService _instance;

        /// <summary>
        /// Singleton accessor.  Creates a new GameObject if none exists.
        /// </summary>
        public static SaveLoadService Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject(nameof(SaveLoadService));
                    _instance = go.AddComponent<SaveLoadService>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        /// <summary>
        /// Full path to the save file on disk.
        /// </summary>
        public string SaveFilePath => Path.Combine(Application.persistentDataPath, "snugglemonster_save.json");

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Serialise the current monster model to JSON and write to disk.
        /// </summary>
        /// <param name="model">The runtime model to persist.</param>
        public void SaveMonster(MonsterRuntimeModel model)
        {
            MonsterSaveData data = new MonsterSaveData(model);
            string json = JsonUtility.ToJson(data, prettyPrint: true);
            File.WriteAllText(SaveFilePath, json);
            Debug.Log($"[SaveLoadService] Save written to {SaveFilePath}");
        }

        /// <summary>
        /// Read the save file from disk and deserialise to MonsterSaveData.
        /// Returns null if the file does not exist or deserialisation fails.
        /// </summary>
        /// <returns>Deserialised save data, or null.</returns>
        public MonsterSaveData LoadMonster()
        {
            if (!SaveExists())
            {
                Debug.LogWarning("[SaveLoadService] No save file found.");
                return null;
            }

            try
            {
                string json = File.ReadAllText(SaveFilePath);
                MonsterSaveData data = JsonUtility.FromJson<MonsterSaveData>(json);
                return data;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[SaveLoadService] Failed to load save: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Check whether a save file exists on disk.
        /// </summary>
        /// <returns>True if the save file exists.</returns>
        public bool SaveExists()
        {
            return File.Exists(SaveFilePath);
        }

        /// <summary>
        /// Delete the save file from disk.  No-op if it does not exist.
        /// </summary>
        public void DeleteSave()
        {
            if (SaveExists())
            {
                File.Delete(SaveFilePath);
                Debug.Log("[SaveLoadService] Save file deleted.");
            }
        }

        /// <summary>
        /// Load save data from disk and populate a MonsterRuntimeModel's fields.
        /// Also resolves any ScriptableObject references via Resources.Load using
        /// stored ID strings.
        /// </summary>
        /// <param name="model">The runtime model to populate.</param>
        public void LoadIntoModel(MonsterRuntimeModel model)
        {
            MonsterSaveData data = LoadMonster();
            if (data != null)
            {
                data.PopulateModel(model);
                Debug.Log("[SaveLoadService] Model populated from save.");
            }
        }

        /// <summary>
        /// Generic helper to load a ScriptableObject by its ID string from Resources.
        /// Searches all Resources folders for any ScriptableObject of type T whose
        /// name (or custom id field) matches the given ID.
        /// </summary>
        /// <typeparam name="T">Type of ScriptableObject to find (must inherit ScriptableObject).</typeparam>
        /// <param name="id">The ID string to match against object names.</param>
        /// <returns>The first matching ScriptableObject, or null if none found.</returns>
        public static T LoadSOById<T>(string id) where T : ScriptableObject
        {
            T[] allOfType = Resources.LoadAll<T>("");

            foreach (T obj in allOfType)
            {
                // Match by asset name.
                if (obj.name == id)
                    return obj;
            }

            Debug.LogWarning($"[SaveLoadService] No ScriptableObject of type {typeof(T).Name} found with ID '{id}'.");
            return null;
        }
    }
}