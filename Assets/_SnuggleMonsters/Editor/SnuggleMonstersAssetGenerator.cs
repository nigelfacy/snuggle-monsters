// <copyright file="SnuggleMonstersAssetGenerator.cs" company="SnuggleMonsters">
// Copyright (c) SnuggleMonsters. All rights reserved.
// </copyright>

using UnityEditor;
using UnityEngine;

namespace SnuggleMonsters.Editor
{
    /// <summary>
    /// One-click generator that creates every MonsterPartSO, MonsterPersonalitySO,
    /// ClothingSO, DecorationSO, and DanceRecipeSO asset needed for the prototype.
    /// Run from Tools → Snuggle Monsters → Generate All Assets after importing into Unity.
    /// </summary>
    public static class SnuggleMonstersAssetGenerator
    {
        private const string ResourcesFolder = "Assets/_SnuggleMonsters/Resources";
        private const string PartsFolder = "Assets/_SnuggleMonsters/Resources/MonsterParts";
        private const string PersonalitiesFolder = "Assets/_SnuggleMonsters/Resources/Personalities";
        private const string ClothesFolder = "Assets/_SnuggleMonsters/Resources/Clothes";
        private const string DecorationsFolder = "Assets/_SnuggleMonsters/Resources/Decorations";
        private const string DancesFolder = "Assets/_SnuggleMonsters/Resources/Dances";

        // ──────────────────────────────────────────────────────────────
        //  Menu Item
        // ──────────────────────────────────────────────────────────────

        [MenuItem("Tools/Snuggle Monsters/Generate All Assets", priority = 100)]
        public static void GenerateAllAssets()
        {
            GenerateAllAssetsInternal();
        }

        /// <summary>
        /// Internal implementation. Called by menu item AND by CloudBuildScript.
        /// </summary>
        public static void GenerateAllAssetsInternal()
        {
            Debug.Log("[AssetGenerator] Starting full asset generation...");

            EnsureFoldersExist();
            CreateAllMonsterParts();
            CreateAllPersonalities();
            CreateAllClothing();
            CreateAllDecorations();
            CreateAllDanceRecipes();
            MovePlaceholderSprites();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[AssetGenerator] ✅ All assets generated! Open Boot scene and press Play.");
        }

        [MenuItem("Tools/Snuggle Monsters/Clear All Generated Assets", priority = 101)]
        private static void ClearAllAssets()
        {
            if (!EditorUtility.DisplayDialog("Clear Assets?",
                "Delete ALL generated ScriptableObject assets? This cannot be undone.",
                "Yes, delete", "Cancel"))
                return;

            string[] folders = { PartsFolder, PersonalitiesFolder, ClothesFolder, DecorationsFolder, DancesFolder };
            foreach (string folder in folders)
            {
                if (AssetDatabase.IsValidFolder(folder))
                {
                    string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { folder });
                    foreach (string guid in guids)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guid);
                        AssetDatabase.DeleteAsset(path);
                    }
                }
            }

            AssetDatabase.Refresh();
            Debug.Log("[AssetGenerator] All assets cleared.");
        }

        // ──────────────────────────────────────────────────────────────
        //  Folder Setup
        // ──────────────────────────────────────────────────────────────

        private static void EnsureFoldersExist()
        {
            EnsureFolder("Assets/_SnuggleMonsters");
            EnsureFolder("Assets/_SnuggleMonsters/Resources");
            string[] folders = {
                PartsFolder, PersonalitiesFolder, ClothesFolder,
                DecorationsFolder, DancesFolder
            };
            foreach (string f in folders) EnsureFolder(f);
        }

        private static void EnsureFolder(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parent = System.IO.Path.GetDirectoryName(path);
                string name = System.IO.Path.GetFileName(path);
                if (!AssetDatabase.IsValidFolder(parent))
                    EnsureFolder(parent);
                AssetDatabase.CreateFolder(parent, name);
            }
        }

        // ──────────────────────────────────────────────────────────────
        //  MONSTER PARTS (28 total)
        // ──────────────────────────────────────────────────────────────

        private static void CreateAllMonsterParts()
        {
            // ---- BODIES (6) ----
            CreatePart("body_squishy",   "Squishy",   BodyPartType.Body, new Color(0f, 0.9f, 1f),    0.7f, 0.6f, new[]{"playful","curious"}, new[]{0.3f,0.2f});
            CreatePart("body_cyclops",   "Cyclops",   BodyPartType.Body, new Color(0.22f,1f,0.08f),   0.8f, 0.5f, new[]{"cheeky","playful"},  new[]{0.3f,0.2f});
            CreatePart("body_stretch",   "Stretch",   BodyPartType.Body, new Color(0.6f,0.35f,0.71f), 0.4f, 0.7f, new[]{"curious","shy"},     new[]{0.3f,0.1f});
            CreatePart("body_fluff",     "Fluff",     BodyPartType.Body, new Color(1f,0.41f,0.71f),   0.3f, 0.4f, new[]{"sleepy","shy"},      new[]{0.4f,0.2f});
            CreatePart("body_tiny",      "Tiny",      BodyPartType.Body, new Color(1f,0.84f,0f),      0.9f, 0.3f, new[]{"playful","cheeky"},  new[]{0.2f,0.3f});
            CreatePart("body_star",      "Star",      BodyPartType.Body, new Color(1f,0.27f,0f),      0.8f, 0.5f, new[]{"adventurous","curious"}, new[]{0.3f,0.2f});

            // ---- EYES (5) ----
            CreatePart("eyes_biground", "Big Round", BodyPartType.Eyes, Color.white,            0.5f, 0.5f, null, null);
            CreatePart("eyes_single",   "Single",    BodyPartType.Eyes, Color.white,            0.3f, 0.6f, null, null);
            CreatePart("eyes_triple",   "Triple",    BodyPartType.Eyes, Color.white,            0.4f, 0.7f, null, null);
            CreatePart("eyes_sleepy",   "Sleepy",    BodyPartType.Eyes, Color.white,            0.2f, 0.3f, null, null);
            CreatePart("eyes_starry",   "Starry",    BodyPartType.Eyes, Color.white,            0.6f, 0.4f, null, null);

            // ---- HORNS (5) ----
            CreatePart("horns_curly",   "Curly",     BodyPartType.Horns, new Color(0.54f,0.27f,0.07f), 0.4f, 0.3f, null, null);
            CreatePart("horns_stubby",  "Stubby",    BodyPartType.Horns, new Color(0.6f,0.35f,0.71f),  0.3f, 0.2f, null, null);
            CreatePart("horns_antenna", "Antenna",   BodyPartType.Horns, new Color(1f,0.84f,0f),       0.5f, 0.5f, null, null);
            CreatePart("horns_zigzag",  "Zigzag",    BodyPartType.Horns, new Color(1f,0.42f,0.21f),    0.7f, 0.4f, null, null);
            CreatePart("horns_flower",  "Flower",    BodyPartType.Horns, new Color(1f,0.41f,0.71f),    0.6f, 0.6f, null, null);

            // ---- WINGS (4) ----
            CreatePart("wings_bat",      "Bat",       BodyPartType.Wings, new Color(0.29f,0f,0.5f),   0.5f, 0.4f, null, null);
            CreatePart("wings_butterfly","Butterfly", BodyPartType.Wings, new Color(1f,0.41f,0.71f),  0.7f, 0.3f, null, null);
            CreatePart("wings_angel",    "Angel",     BodyPartType.Wings, new Color(0.69f,0.88f,0.9f),0.3f, 0.5f, null, null);
            CreatePart("wings_dragon",   "Dragon",    BodyPartType.Wings, new Color(0f,0.9f,1f),      0.8f, 0.6f, null, null);

            // ---- TAILS (4) ----
            CreatePart("tail_curly",  "Curly",   BodyPartType.Tail, new Color(0.6f,0.35f,0.71f), 0.4f, 0.3f, null, null);
            CreatePart("tail_heart",  "Heart",   BodyPartType.Tail, new Color(1f,0.08f,0.08f),   0.6f, 0.5f, null, null);
            CreatePart("tail_spiky",  "Spiky",   BodyPartType.Tail, new Color(0.82f,0.71f,0.55f),0.5f, 0.4f, null, null);
            CreatePart("tail_fluffy", "Fluffy",  BodyPartType.Tail, new Color(0.83f,0.83f,0.83f),0.3f, 0.4f, null, null);

            // ---- PATTERNS (4) ----
            CreatePart("pattern_stripes", "Stripes", BodyPartType.Pattern, new Color(1f,0.22f,0.55f), 0.3f, 0.2f, null, null);
            CreatePart("pattern_polka",   "Polka",   BodyPartType.Pattern, new Color(1f,0.08f,0.58f), 0.4f, 0.3f, null, null);
            CreatePart("pattern_zigzag",  "Zigzag",  BodyPartType.Pattern, new Color(0.6f,0.35f,0.71f),0.5f, 0.4f, null, null);
            CreatePart("pattern_stars",   "Stars",   BodyPartType.Pattern, new Color(0.29f,0f,0.5f),  0.6f, 0.5f, null, null);
        }

        private static void CreatePart(string id, string displayName, BodyPartType partType,
            Color color, float danceMod, float personalityMod,
            string[] weightKeys, float[] weightValues)
        {
            MonsterPartSO part = ScriptableObject.CreateInstance<MonsterPartSO>();
            part.name = id;
            // Use SerializedObject to set private fields
            SerializedObject so = new SerializedObject(part);
            so.FindProperty("id").stringValue = id;
            so.FindProperty("displayName").stringValue = displayName;
            so.FindProperty("partType").enumValueIndex = (int)partType;
            so.FindProperty("partColor").colorValue = color;
            so.FindProperty("danceModifier").floatValue = danceMod;
            so.FindProperty("personalityModifier").floatValue = personalityMod;
            if (weightKeys != null) so.FindProperty("personalityWeightKeys").arraySize = weightKeys.Length;
            if (weightValues != null) so.FindProperty("personalityWeightValues").arraySize = weightValues.Length;
            so.ApplyModifiedProperties();

            string fullPath = $"{PartsFolder}/mp_{id}.asset";
            AssetDatabase.CreateAsset(part, fullPath);
            Debug.Log($"[AssetGenerator] Created part: {displayName} ({id})");
        }

        // ──────────────────────────────────────────────────────────────
        //  PERSONALITIES (4)
        // ──────────────────────────────────────────────────────────────

        private static void CreateAllPersonalities()
        {
            CreatePersonality("bouncy", "Bouncy",
                1f, 0.1f, 0.8f, 0.2f, 0.6f, 0.7f,
                new[]{"Hi! I'm {0}! Wanna play?","Hey {0}! Let's do something FUN!","{0}! I'm so happy to see you!"},
                new[]{"Boing boing!","What should we do next?","I like your smile!","Wanna bounce with me?"},
                "Bouncy", "Wiggly", new Color(1f,0.84f,0f));

            CreatePersonality("snuggly", "Snuggly",
                0.3f, 1f, 0.2f, 0.8f, 0.1f, 0.2f,
                new[]{"Hi {0}... *yawn* can we cuddle?","Hello {0}, you're warm...","{0}... I like you..."},
                new[]{"Mmm... cozy...","Can we nap now?","*soft purring*","This is nice..."},
                "Sleepy", "Sway", new Color(1f,0.41f,0.71f));

            CreatePersonality("curious", "Curious",
                0.7f, 0.2f, 1f, 0.3f, 0.9f, 0.5f,
                new[]{"Ooh! {0}! What's that thing?","{0}! I found something!","Hey {0}, come look at this!"},
                new[]{"What does this button do?","I wonder where that goes...","Ooh! Shiny!","Let's explore!"},
                "Curious", "Spinny", new Color(0f,0.9f,1f));

            CreatePersonality("cheeky", "Cheeky",
                0.9f, 0.1f, 0.5f, 0.2f, 0.8f, 1f,
                new[]{"Hey {0}! Watch this!","{0}! I'm gonna get you!","Pssst {0}! Over here!"},
                new[]{"Hehehe!","Did you see that?","I'm the silliest!","Your turn!"},
                "Mischievous", "Spinny", new Color(1f,0.22f,0.55f));
        }

        private static void CreatePersonality(string id, string displayName,
            float playful, float sleepy, float curious, float shy,
            float adventurous, float cheeky,
            string[] greetings, string[] idleLines,
            string idleStyle, string danceStyle, Color favColour)
        {
            MonsterPersonalitySO p = ScriptableObject.CreateInstance<MonsterPersonalitySO>();
            p.name = id;
            SerializedObject so = new SerializedObject(p);
            so.FindProperty("id").stringValue = id;
            so.FindProperty("displayName").stringValue = displayName;
            so.FindProperty("playful").floatValue = playful;
            so.FindProperty("sleepy").floatValue = sleepy;
            so.FindProperty("curious").floatValue = curious;
            so.FindProperty("shy").floatValue = shy;
            so.FindProperty("adventurous").floatValue = adventurous;
            so.FindProperty("cheeky").floatValue = cheeky;
            so.FindProperty("greetingLines").arraySize = greetings.Length;
            for (int i = 0; i < greetings.Length; i++)
                so.FindProperty("greetingLines").GetArrayElementAtIndex(i).stringValue = greetings[i];
            so.FindProperty("idleLines").arraySize = idleLines.Length;
            for (int i = 0; i < idleLines.Length; i++)
                so.FindProperty("idleLines").GetArrayElementAtIndex(i).stringValue = idleLines[i];
            so.FindProperty("idleAnimationStyle").stringValue = idleStyle;
            so.FindProperty("danceStyle").stringValue = danceStyle;
            so.FindProperty("favouriteColour").colorValue = favColour;
            so.ApplyModifiedProperties();

            AssetDatabase.CreateAsset(p, $"{PersonalitiesFolder}/pers_{id}.asset");
            Debug.Log($"[AssetGenerator] Created personality: {displayName}");
        }

        // ──────────────────────────────────────────────────────────────
        //  CLOTHES (6)
        // ──────────────────────────────────────────────────────────────

        private static void CreateAllClothing()
        {
            CreateClothing("hat_party", "Party Hat", ClothingType.Hat, new Color(1f,0.22f,0.55f), new Vector2(0, 40));
            CreateClothing("hat_tophat", "Top Hat", ClothingType.Hat, new Color(0.6f,0.35f,0.71f), new Vector2(0, 35));
            CreateClothing("hat_crown", "Crown", ClothingType.Hat, new Color(1f,0.84f,0f), new Vector2(0, 30));
            CreateClothing("scarf", "Scarf", ClothingType.Scarf, new Color(0.22f,1f,0.08f), new Vector2(0, -10));
            CreateClothing("glasses_round", "Round Glasses", ClothingType.Glasses, new Color(0f,0.9f,1f), new Vector2(0, 5));
            CreateClothing("bowtie", "Bow Tie", ClothingType.Accessory, new Color(1f,0.08f,0.58f), new Vector2(0, -15));
        }

        private static void CreateClothing(string id, string displayName, ClothingType type, Color tint, Vector2 offset)
        {
            ClothingSO c = ScriptableObject.CreateInstance<ClothingSO>();
            c.name = id;
            SerializedObject so = new SerializedObject(c);
            so.FindProperty("id").stringValue = id;
            so.FindProperty("displayName").stringValue = displayName;
            so.FindProperty("clothingType").enumValueIndex = (int)type;
            so.FindProperty("tintColor").colorValue = tint;
            so.FindProperty("attachOffset").vector2Value = offset;
            so.ApplyModifiedProperties();

            AssetDatabase.CreateAsset(c, $"{ClothesFolder}/{id}.asset");
            Debug.Log($"[AssetGenerator] Created clothing: {displayName}");
        }

        // ──────────────────────────────────────────────────────────────
        //  DECORATIONS (9)
        // ──────────────────────────────────────────────────────────────

        private static void CreateAllDecorations()
        {
            CreateDecoration("deco_rug", "Fluffy Rug", DecorationType.Rug, new Color(0f,0.9f,1f));
            CreateDecoration("deco_lamp", "Eye Lamp", DecorationType.Lamp, new Color(1f,0.84f,0f));
            CreateDecoration("deco_poster", "Star Poster", DecorationType.Poster, new Color(0.68f,0.85f,0.9f));
            CreateDecoration("deco_toy", "Block Tower", DecorationType.Toy, new Color(1f,0.42f,0.21f));
            CreateDecoration("deco_beddeco", "Monster Blanket", DecorationType.BedDeco, new Color(1f,0.41f,0.71f));
            CreateDecoration("deco_pillow", "Heart Pillow", DecorationType.Toy, new Color(1f,0.08f,0.58f));
            CreateDecoration("deco_books", "Book Stack", DecorationType.Toy, new Color(0.6f,0.35f,0.71f));
            CreateDecoration("deco_plant", "Potted Plant", DecorationType.Toy, new Color(0.22f,1f,0.08f));
            CreateDecoration("deco_nightlight", "Star Night Light", DecorationType.Lamp, new Color(1f,0.84f,0f));
        }

        private static void CreateDecoration(string id, string displayName, DecorationType type, Color color)
        {
            DecorationSO d = ScriptableObject.CreateInstance<DecorationSO>();
            d.name = id;
            SerializedObject so = new SerializedObject(d);
            so.FindProperty("id").stringValue = id;
            so.FindProperty("displayName").stringValue = displayName;
            so.FindProperty("decorationType").enumValueIndex = (int)type;
            so.FindProperty("defaultColor").colorValue = color;
            so.ApplyModifiedProperties();

            AssetDatabase.CreateAsset(d, $"{DecorationsFolder}/{id}.asset");
            Debug.Log($"[AssetGenerator] Created decoration: {displayName}");
        }

        // ──────────────────────────────────────────────────────────────
        //  DANCE RECIPES (5)
        // ──────────────────────────────────────────────────────────────

        private static void CreateAllDanceRecipes()
        {
            CreateDance("flutter_waggle", "The Flutter-Waggle",
                new BodyPartType[]{BodyPartType.Wings, BodyPartType.Tail},
                "FlutterWaggle", 0.8f, 1.2f,
                new[]{"Wheee! Look at me!","Flutter flutter waggle waggle!","I'm flying!"});

            CreateDance("bouncy_bounce", "The Super Bounce",
                new BodyPartType[]{BodyPartType.Body},
                "SuperBounce", 0.6f, 1.5f,
                new[]{"Boing boing boing!","Up and down and up!","This is the best!"});

            CreateDance("spiral_spin", "The Spiral Spin",
                new BodyPartType[]{BodyPartType.Wings},
                "SpiralSpin", 1f, 1.8f,
                new[]{"Wheee! I'm dizzy!","Round and round!","The room is spinning!"});

            CreateDance("wiggle_shake", "The Wiggle-Shake",
                new BodyPartType[]{BodyPartType.Tail},
                "WiggleShake", 0.7f, 1.3f,
                new[]{"Shake shake shake!","Wiggle wiggle wiggle!","Look at me go!"});

            CreateDance("starry_dance", "The Starry Night Dance",
                new BodyPartType[]{BodyPartType.Pattern, BodyPartType.Eyes},
                "StarryNight", 0.9f, 1f,
                new[]{"Twinkle twinkle!","I'm a dancing star!","So sparkly!"});
        }

        private static void CreateDance(string id, string displayName,
            BodyPartType[] requiredParts, string animName,
            float sparkleIntensity, float speedMultiplier, string[] funnyLines)
        {
            DanceRecipeSO d = ScriptableObject.CreateInstance<DanceRecipeSO>();
            d.name = id;
            SerializedObject so = new SerializedObject(d);
            so.FindProperty("id").stringValue = id;
            so.FindProperty("displayName").stringValue = displayName;
            so.FindProperty("requiredPartTypes").arraySize = requiredParts.Length;
            for (int i = 0; i < requiredParts.Length; i++)
                so.FindProperty("requiredPartTypes").GetArrayElementAtIndex(i).enumValueIndex = (int)requiredParts[i];
            so.FindProperty("animationName").stringValue = animName;
            so.FindProperty("funName").stringValue = displayName;
            so.FindProperty("sparkleEffectIntensity").floatValue = sparkleIntensity;
            so.FindProperty("danceSpeedMultiplier").floatValue = speedMultiplier;
            so.FindProperty("funnyLines").arraySize = funnyLines.Length;
            for (int i = 0; i < funnyLines.Length; i++)
                so.FindProperty("funnyLines").GetArrayElementAtIndex(i).stringValue = funnyLines[i];
            so.ApplyModifiedProperties();

            AssetDatabase.CreateAsset(d, $"{DancesFolder}/{id}.asset");
            Debug.Log($"[AssetGenerator] Created dance recipe: {displayName}");
        }

        // ──────────────────────────────────────────────────────────────
        //  Sprite Mover (placeholder — assign SVGs later)
        // ──────────────────────────────────────────────────────────────

        private static void MovePlaceholderSprites()
        {
            // The SVG files in Art/Placeholder/ are already in the right place.
            // After importing, select them all and set:
            //   Texture Type = Sprite (2D and UI)
            //   Pixels Per Unit = 100
            // Then drag them onto the MonsterPartSO.sprite fields in each asset.
            Debug.Log("[AssetGenerator] 💡 Remember: Assign SVG sprites to each MonsterPartSO.sprite field in the Inspector!");
        }
    }
}