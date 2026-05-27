// <copyright file="SnuggleMonstersBuildSetup.cs" company="SnuggleMonsters">
// Copyright (c) SnuggleMonsters. All rights reserved.
// </copyright>
//
// ONE-CLICK BUILD CONFIGURATION
// Tools -> Snuggle Monsters -> Configure Build Settings
// Configures Player Settings for a child-safe Android build.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SnuggleMonsters.Editor
{
    public static class SnuggleMonstersBuildSetup
    {
        [MenuItem("Tools/Snuggle Monsters/Configure Build Settings", priority = 102)]
        private static void ConfigureBuild()
        {
            PlayerSettings.companyName = "SnuggleMonsters";
            PlayerSettings.productName = "Snuggle Monsters";
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.snugglesoft.monsters");
            PlayerSettings.bundleVersion = "0.1.0";
            PlayerSettings.Android.bundleVersionCode = 1;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = true;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel33;
            PlayerSettings.colorSpace = ColorSpace.Linear;
            PlayerSettings.graphicsJobs = false;
            QualitySettings.SetQualityLevel(2);
            QualitySettings.vSyncCount = 0;
            QualitySettings.antiAliasing = 2;
            QualitySettings.shadows = ShadowQuality.Disable;
            QualitySettings.masterTextureLimit = 1;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android, ApiCompatibilityLevel.NET_Unity_4_8);
            PlayerSettings.SplashScreen.showUnityLogo = true;
            Debug.Log("[BuildSetup] Android build settings configured. Go to File -> Build Settings -> Build.");

            AddScenesToBuild();
        }

        private static void AddScenesToBuild()
        {
            string[] requiredScenes = {
                "Assets/_SnuggleMonsters/Scenes/Boot.unity",
                "Assets/_SnuggleMonsters/Scenes/MonsterCreator.unity",
                "Assets/_SnuggleMonsters/Scenes/Bedroom.unity",
                "Assets/_SnuggleMonsters/Scenes/VillageHub.unity",
                "Assets/_SnuggleMonsters/Scenes/TinyAdventure.unity"
            };

            EditorBuildSettingsScene[] existingScenes = EditorBuildSettings.scenes;
            List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();

            foreach (EditorBuildSettingsScene s in existingScenes)
            {
                if (System.Array.Exists(requiredScenes, r => r == s.path))
                    scenes.Add(s);
            }

            foreach (string scene in requiredScenes)
            {
                if (!scenes.Exists(s => s.path == scene))
                    scenes.Add(new EditorBuildSettingsScene(scene, true));
            }

            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log($"[BuildSetup] {scenes.Count} scenes in Build Settings.");
        }
    }
}