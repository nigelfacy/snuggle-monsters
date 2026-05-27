// <copyright file="GameManager.cs" company="SnuggleMonsters">
// Copyright (c) SnuggleMonsters. All rights reserved.
// </copyright>

using UnityEngine;

namespace SnuggleMonsters.Core
{
    /// <summary>
    /// Scene-singleton that persists across scenes. Holds a reference to the
    /// single <see cref="MonsterRuntimeModel"/> and a <see cref="SceneController"/>
    /// for transitioning between game scenes.
    /// On the Boot scene, auto-transitions to MonsterCreator if no save exists,
    /// or to Bedroom if a monster has been saved.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // ----- Scenes -----

        /// <summary>
        /// Enumeration of all game scenes used in SnuggleMonsters.
        /// Scene names must match the names in Unity's Build Settings.
        /// </summary>
        public enum GameScene
        {
            /// <summary>Launch/boot screen. Determines initial transition destination.</summary>
            Boot,

            /// <summary>Monster creation/customisation screen.</summary>
            MonsterCreator,

            /// <summary>The monster's bedroom, where tucking-in and night-light happen.</summary>
            Bedroom,

            /// <summary>The central village hub scene.</summary>
            VillageHub,

            /// <summary>A tiny adventure game mode.</summary>
            TinyAdventure,
        }

        // ----- Serialised Fields -----

        [Header("References")]

        /// <summary>
        /// Reference to the SceneController for handling transitions.
        /// If null, one is created on the same GameObject at runtime.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the SceneController. Created automatically if null.")]
        private SceneController sceneController;

        /// <summary>
        /// Reference to the MonsterRuntimeModel singleton.
        /// Found automatically at runtime if null.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the MonsterRuntimeModel. Found automatically if null.")]
        private MonsterRuntimeModel monsterRuntimeModel;

        [Header("Settings")]

        /// <summary>
        /// If true, the Boot scene will always transition to MonsterCreator
        /// regardless of save state (useful for testing).
        /// </summary>
        [SerializeField]
        [Tooltip("If true, Boot always goes to MonsterCreator (for testing).")]
        private bool forceNewGame = false;

        // ----- Singleton -----

        /// <summary>Lazy singleton instance.</summary>
        private static GameManager instance;

        /// <summary>
        /// Gets the singleton instance of the GameManager.
        /// </summary>
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    Debug.LogError("[GameManager] No instance found. Ensure a GameManager exists in the scene.");
                }

                return instance;
            }
        }

        /// <summary>Gets the SceneController used for scene transitions.</summary>
        public SceneController SceneController => sceneController;

        /// <summary>Gets the MonsterRuntimeModel singleton.</summary>
        public MonsterRuntimeModel MonsterRuntimeModel => monsterRuntimeModel;

        // ----- Dedicated Bootstrapper Support -----

        /// <summary>
        /// Called by SceneBootstrapper to initialise systems before the boot sequence.
        /// Currently a placeholder for future system initialisation (audio, analytics, etc.).
        /// </summary>
        public void Initialise()
        {
            Debug.Log("[GameManager] Initialising game systems...");

            // Ensure the monster runtime model is available
            if (monsterRuntimeModel == null)
                monsterRuntimeModel = MonsterRuntimeModel.Instance;

            // TODO: Initialise audio manager
            // TODO: Initialise analytics
            // TODO: Initialise any other persistent systems
        }

        /// <summary>
        /// Called by SceneBootstrapper when a save file is found.
        /// Loads monster data from the save file into the runtime model.
        /// </summary>
        public void LoadGame()
        {
            Debug.Log("[GameManager] Loading saved game...");

            if (monsterRuntimeModel == null)
                monsterRuntimeModel = MonsterRuntimeModel.Instance;

            // Delegate to SaveLoadService to populate the model
            // TODO: Uncomment once SaveLoadService is added to the scene
            // SnuggleMonsters.SaveLoad.SaveLoadService.Instance.LoadIntoModel(monsterRuntimeModel);
            Debug.Log("[GameManager] Save loaded. Monster state restored.");
        }

        // ----- Public Methods -----

        /// <summary>
        /// Transitions to the specified game scene with a fade effect.
        /// Maps the <see cref="GameScene"/> enum value to the corresponding scene name
        /// and delegates to <see cref="SceneController.LoadScene"/>.
        /// </summary>
        /// <param name="scene">The target scene to load.</param>
        public void GoToScene(GameScene scene)
        {
            string sceneName = GetSceneName(scene);

            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError($"[GameManager] No scene name mapped for GameScene.{scene}");
                return;
            }

            Debug.Log($"[GameManager] Navigating to {scene} ({sceneName})");

            if (sceneController != null)
            {
                sceneController.LoadScene(sceneName);
            }
            else
            {
                Debug.LogWarning("[GameManager] SceneController is null. Loading scene directly.");
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            }
        }

        /// <summary>
        /// Quits the application. In the Unity Editor, stops play mode.
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("[GameManager] Quitting game.");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        // ----- Boot Logic -----

        /// <summary>
        /// Evaluates whether a saved monster exists and transitions to the appropriate scene.
        /// Called automatically when the Boot scene starts.
        /// </summary>
        public void HandleBootTransition()
        {
            // TODO: Replace with actual save file check once save system is implemented.
            // For now, uses a simple PlayerPrefs check as a placeholder.
            bool hasSaveData = !forceNewGame && HasSavedMonster();

            if (hasSaveData)
            {
                Debug.Log("[GameManager] Boot: Save data found. Transitioning to Bedroom.");
                GoToScene(GameScene.Bedroom);
            }
            else
            {
                Debug.Log("[GameManager] Boot: No save data. Transitioning to MonsterCreator.");
                GoToScene(GameScene.MonsterCreator);
            }
        }

        // ----- Private Helpers -----

        /// <summary>
        /// Maps a <see cref="GameScene"/> enum value to its Unity scene name.
        /// </summary>
        /// <param name="scene">The scene enum value.</param>
        /// <returns>The corresponding Unity scene name.</returns>
        private static string GetSceneName(GameScene scene)
        {
            // TODO: If scene names differ from enum names, update this mapping.
            switch (scene)
            {
                case GameScene.Boot:
                    return "Boot";
                case GameScene.MonsterCreator:
                    return "MonsterCreator";
                case GameScene.Bedroom:
                    return "Bedroom";
                case GameScene.VillageHub:
                    return "VillageHub";
                case GameScene.TinyAdventure:
                    return "TinyAdventure";
                default:
                    Debug.LogError($"[GameManager] Unhandled GameScene value: {scene}");
                    return null;
            }
        }

        /// <summary>
        /// Placeholder check for whether a saved monster exists.
        /// TODO: Replace with proper save system (e.g. file I/O, PlayerPrefs, or JSON).
        /// </summary>
        /// <returns>True if save data exists; otherwise false.</returns>
        private static bool HasSavedMonster()
        {
            // TODO: Implement actual save data check.
            // Example: return SaveSystem.SaveExists("monster");
            return PlayerPrefs.HasKey("SnuggleMonsters_SaveExists");
        }

        // ----- Lifecycle -----

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogWarning("[GameManager] Duplicate instance destroyed.");
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            // Auto-resolve references if not set in the Inspector
            if (sceneController == null)
            {
                sceneController = GetComponent<SceneController>();

                if (sceneController == null)
                {
                    sceneController = gameObject.AddComponent<SceneController>();
                    Debug.Log("[GameManager] Created SceneController component at runtime.");
                }
            }

            if (monsterRuntimeModel == null)
            {
                monsterRuntimeModel = FindFirstObjectByType<MonsterRuntimeModel>();

                if (monsterRuntimeModel == null)
                {
                    Debug.LogWarning("[GameManager] No MonsterRuntimeModel found in scene. It will be created on demand.");
                }
            }
        }

        private void Start()
        {
            // If we're on the Boot scene, handle the initial transition
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (currentScene == GetSceneName(GameScene.Boot))
            {
                // Small delay to let everything initialise
                Invoke(nameof(HandleBootTransition), 0.1f);
            }
        }
    }
}