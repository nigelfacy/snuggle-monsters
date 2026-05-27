// <copyright file="CloudBuildScript.cs" company="SnuggleMonsters">
// Copyright (c) SnuggleMonsters. All rights reserved.
// </copyright>
//
// Headless build method called by GitHub Actions (game-ci/unity-builder).
// Generates all assets, creates scenes if missing, configures build settings,
// and builds the Android APK.
//
// Called from: .github/workflows/build.yml → buildMethod

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SnuggleMonsters.Editor
{
    /// <summary>
    /// Cloud/CI build entry point. Called by game-ci/unity-builder in GitHub Actions.
    /// </summary>
    public static class CloudBuildScript
    {
        /// <summary>
        /// Main build method. Called by the GitHub Action with no arguments.
        /// </summary>
        public static void BuildApk()
        {
            Debug.Log("[CloudBuild] Starting Snuggle Monsters cloud build...");

            // 1. Generate all ScriptableObject assets
            Debug.Log("[CloudBuild] Step 1/5: Generating ScriptableObject assets...");
            SnuggleMonstersAssetGenerator.GenerateAllAssetsInternal();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // 2. Ensure all 5 scenes exist
            Debug.Log("[CloudBuild] Step 2/5: Creating scenes...");
            EnsureScenesExist();

            // 3. Add scenes to Build Settings
            Debug.Log("[CloudBuild] Step 3/5: Configuring Build Settings...");
            ConfigureBuildSettings();

            // 4. Set Player Settings
            Debug.Log("[CloudBuild] Step 4/5: Setting Player Settings...");
            ApplyPlayerSettings();

            // 5. Build APK
            Debug.Log("[CloudBuild] Step 5/5: Building Android APK...");
            PerformBuild();

            Debug.Log("[CloudBuild] ✅ Build complete!");
        }

        // ──────────────────────────────────────────────────────────────────
        //  Scene Creation
        // ──────────────────────────────────────────────────────────────────

        private static void EnsureScenesExist()
        {
            string scenesDir = "Assets/_SnuggleMonsters/Scenes";
            if (!AssetDatabase.IsValidFolder(scenesDir))
            {
                string parent = "Assets/_SnuggleMonsters";
                if (!AssetDatabase.IsValidFolder(parent))
                    AssetDatabase.CreateFolder("Assets", "_SnuggleMonsters");
                AssetDatabase.CreateFolder(parent, "Scenes");
            }

            string[] requiredScenes = {
                "Boot",
                "MonsterCreator",
                "Bedroom",
                "VillageHub",
                "TinyAdventure"
            };

            foreach (string sceneName in requiredScenes)
            {
                string path = $"{scenesDir}/{sceneName}.unity";
                if (!File.Exists(path))
                {
                    Debug.Log($"[CloudBuild] Creating scene: {sceneName}");
                    SceneManagement.Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                    EditorSceneManager.SaveScene(scene, path);
                    EditorSceneManager.CloseScene(scene, true);
                }
            }

            AssetDatabase.Refresh();
        }

        // ──────────────────────────────────────────────────────────────────
        //  Build Settings
        // ──────────────────────────────────────────────────────────────────

        private static void ConfigureBuildSettings()
        {
            string[] requiredScenes = {
                "Assets/_SnuggleMonsters/Scenes/Boot.unity",
                "Assets/_SnuggleMonsters/Scenes/MonsterCreator.unity",
                "Assets/_SnuggleMonsters/Scenes/Bedroom.unity",
                "Assets/_SnuggleMonsters/Scenes/VillageHub.unity",
                "Assets/_SnuggleMonsters/Scenes/TinyAdventure.unity"
            };

            EditorBuildSettingsScene[] existing = EditorBuildSettings.scenes;
            List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();

            foreach (EditorBuildSettingsScene s in existing)
                if (System.Array.Exists(requiredScenes, r => r == s.path))
                    scenes.Add(s);

            foreach (string scene in requiredScenes)
                if (!scenes.Exists(s => s.path == scene))
                    scenes.Add(new EditorBuildSettingsScene(scene, true));

            EditorBuildSettings.scenes = scenes.ToArray();
        }

        // ──────────────────────────────────────────────────────────────────
        //  Player Settings
        // ──────────────────────────────────────────────────────────────────

        private static void ApplyPlayerSettings()
        {
            PlayerSettings.companyName = "SnuggleMonsters";
            PlayerSettings.productName = "Snuggle Monsters";
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.snugglesoft.monsters");
            PlayerSettings.bundleVersion = "0.1.0";
            PlayerSettings.Android.bundleVersionCode = 1;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel33;
            PlayerSettings.colorSpace = ColorSpace.Linear;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android, ApiCompatibilityLevel.NET_Unity_4_8);
            QualitySettings.SetQualityLevel(2);
            QualitySettings.vSyncCount = 0;
            QualitySettings.antiAliasing = 2;
            QualitySettings.shadows = ShadowQuality.Disable;
            QualitySettings.masterTextureLimit = 1;
        }

        // ──────────────────────────────────────────────────────────────────
        //  Build
        // ──────────────────────────────────────────────────────────────────

        private static void PerformBuild()
        {
            string[] scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);

            string buildDir = "build/Android";
            Directory.CreateDirectory(buildDir);

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = $"{buildDir}/SnuggleMonsters.apk",
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);

            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.Log($"[CloudBuild] ✅ APK built: {options.locationPathName}");
                Debug.Log($"[CloudBuild]    Size: {new FileInfo(options.locationPathName).Length / 1024 / 1024} MB");
            }
            else
            {
                Debug.LogError($"[CloudBuild] ❌ Build failed: {report.summary.result}");
                foreach (var step in report.steps)
                    if (step.messages != null)
                        foreach (var msg in step.messages)
                            if (msg.type == UnityEditor.Build.Reporting.LogType.Error)
                                Debug.LogError($"[CloudBuild] {msg.content}");
                throw new System.Exception($"Unity build failed with result: {report.summary.result}");
            }
        }
    }
}