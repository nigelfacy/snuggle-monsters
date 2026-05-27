# Snuggle Monsters 🧸👾

A warm, child-safe Unity 2D prototype where players create a monster friend, dress them up, decorate their room, dance together, explore a friendly village, go on a tiny adventure, and tuck them into bed.

**Core promise:** Within 30 seconds of meeting, the player should love their monster.

---

## Art Style

**Scrapbook Monster + Monsters Inc**
- Vibrant saturated colours (hot pink, lime green, electric cyan, royal purple, sunny yellow)
- Bold 3px black outlines on every sprite
- Big expressive eyes (Mike-style single eye, Sulley-style big round pair, or 3-in-a-row)
- Rounded soft shapes — short stubby limbs, exaggerated proportions
- Friendly triangle teeth, tiny blush circles, star/dot motifs
- Non-threatening, huggable, child-safe — absolutely nothing scary
- *50 SVG placeholder sprites included in `Art/Placeholder/` matching this exact style*

---

## Project at a Glance

| Category | Count | Details |
|---|---|---|
| **C# Scripts** | 35 | 6,835 lines, all namespaced `SnuggleMonsters.*` |
| **SVG Sprites** | 50 | Full scrapbook/Monsters Inc art style |
| **Editor Tools** | 3 | One-click asset generator, build config, SO factory |
| **Scenes** | 5 planned | Boot → MonsterCreator → Bedroom → VillageHub → TinyAdventure |
| **Systems** | 11 complete | Creator, DressUp, Decoration, Dance, Village, Adventure, Bedtime, SaveLoad, UI, FirstEncounter, Bootstrapper |

## Project Structure

```
Assets/_SnuggleMonsters/
├── Art/Placeholder/       ← 50 SVG sprites
│   ├── Bodies/            ← 6 body shapes (Squishy, Cyclops, Stretch, Fluff, Tiny, Star)
│   ├── Eyes/              ← 5 eye types (BigRound, Single, Triple, Sleepy, Starry)
│   ├── Horns/             ← 5 horn types (Curly, Stubby, Antenna, Zigzag, Flower)
│   ├── Wings/             ← 4 wing types (Bat, Butterfly, Angel, Dragon)
│   ├── Tails/             ← 4 tail types (Curly, Heart, Spiky, Fluffy)
│   ├── Patterns/          ← 4 pattern types (Stripes, Polka, Zigzag, Stars)
│   ├── Clothes/           ← 6 clothing sprites
│   ├── Decorations/       ← 9 decoration sprites
│   └── UI/                ← 4 UI element sprites
├── Prefabs/               ← Create in Unity
├── Scenes/                ← Create 5 scenes in Unity
├── Scripts/               ← 35 C# files
│   ├── Core/              ← GameManager, SceneController, Bootstrapper, FirstEncounter
│   ├── Monster/           ← RuntimeModel, AnimatorController
│   ├── Creator/           ← CreatorController, PartSelector
│   ├── DressUp/           ← DressUpController
│   ├── House/             ← DecorationController, SnapPoint
│   ├── Dance/             ← DancePartyController, SpecialDanceResolver
│   ├── Village/           ← VillageHubController, NPCInteractable
│   ├── Adventure/         ← TinyAdventureController, GlowingObject
│   ├── Bedtime/           ← RoutineController, NightLight
│   ├── SaveLoad/          ← SaveLoadService, MonsterSaveData
│   └── UI/                ← UIController, PortraitDisplay
├── ScriptableObjects/     ← 5 SO definitions
├── Resources/             ← SO instances go here (for runtime lookup)
├── Editor/                ← 3 editor tools
└── Tests/
```

## Core Loop

```
[Boot] → [MonsterCreator] → [Bedroom] → [VillageHub] → [TinyAdventure] → [Bedtime] → Save
                                 ↑                                                    │
                                 └──────────────── Load Game ─────────────────────────┘
```

**First-play moment** (in Bedroom, plays once):
> Monster **blinks** → **bounces** happily → **waves** → *"Hi! I'm [Name]! Can I live with you?"* → **Unlocks Special Dance** 🎉

---

## 📱 Android Build & Phone Testing Guide

### Step 1: Install Unity (15 min)
1. Download **Unity Hub** from [unity.com/download](https://unity.com/download)
2. Install Unity Hub → Open it → Go to **Installs** → **Install Editor**
3. Choose **Unity 2022.3 LTS** (e.g. `2022.3.55f1`)
4. **On the install modules screen, tick:**
   - ☑ **Android Build Support**
     - ☑ OpenJDK
     - ☑ Android SDK & NDK
   - ☑ **Windows Build Support (IL2CPP)**
5. Let it download and install (~5-10 GB)

### Step 2: Open the Project
1. In Unity Hub, click **Open** → **Add project from disk**
2. Navigate to `C:\Users\nigel\Documents\SnuggleMonsters\` and select it
3. Click the project to open it in Unity
4. Wait for compilation (progress bar bottom-right)

### Step 3: Generate All Assets (30 seconds)
1. Wait for Unity to finish compiling
2. **Tools** → **Snuggle Monsters** → **Configure Build Settings**
3. **Tools** → **Snuggle Monsters** → **Generate All Assets**
4. Creates 28 Monster Parts, 4 Personalities, 6 Clothes, 9 Decorations, 5 Dances

### Step 4: Create the 5 Scenes
1. Right-click Project window → Create → Scene → name it `Boot.unity` in `Assets/_SnuggleMonsters/Scenes/`
2. Repeat for: `MonsterCreator.unity`, `Bedroom.unity`, `VillageHub.unity`, `TinyAdventure.unity`
3. Open `Boot.unity` → empty GameObject → add **SceneBootstrapper** component → Save

### Step 5: Build APK for Phone
1. **File** → **Build Settings**
2. Platform: **Android** → **Switch Platform**
3. Click **Build** → choose folder → `SnuggleMonsters.apk` (takes 2-5 min first time)

### Step 6: Install on Phone
**Cable:** Connect phone via USB → enable USB Debugging → copy APK → tap to install
**Cloud:** Upload APK to Google Drive → download on phone → install
**Alt (WebGL):** Switch to WebGL platform → Build → upload `index.html` to any static host → play instantly in browser

---

## Quick-Start Checklist

- [ ] Unity 2022.3 LTS installed with Android Build Support
- [ ] Project opened in Unity Hub
- [ ] TextMeshPro imported (Window → TextMeshPro → Import TMP Essentials)
- [ ] All 5 scenes created in Scenes/ folder
- [ ] Tools → Configure Build Settings run
- [ ] Tools → Generate All Assets run
- [ ] Boot scene has SceneBootstrapper
- [ ] 5 scenes added to Build Settings
- [ ] Build APK → install on phone → play!

---

## Design Principles

| Principle | How It's Applied |
|---|---|
| **No combat, no losing** | Every interaction is positive. No timers, no failure states, no wrong answers |
| **30-second love** | Monster immediately blinks, bounces, waves, speaks. Dance unlocked on first meeting |
| **Child-safe text** | Warm, funny, gentle. "You found a sparkly acorn!" — not "You died" |
| **No dark/scary** | Bright colours, rounded shapes, smiling faces everywhere |
| **Modular architecture** | All parts, personalities, dances, clothes, decorations are ScriptableObjects |
| **Console-friendly** | All logs use `[ClassName]` prefix for easy filtering |

## Where Real Art/Audio Goes

Every file has `// TODO` comments. Key upgrade points:
- **MonsterPartSO.cs** — `sprite` field: assign real drawn sprites
- **MonsterAnimatorController.cs** — swap coroutines for Animator state machines
- **DancePartyController.cs** — add dance music
- **BedtimeRoutineController.cs** — add lullaby audio
- **GameManager.cs** — audio manager, analytics setup
- **SaveLoadService.cs** — upgrade to Addressables for better SO management

## Troubleshooting

| Problem | Fix |
|---|---|
| SDK not found | Unity Hub → Installs → Add modules → Android SDK |
| Scene list empty | Tools → Snuggle Monsters → Configure Build Settings |
| Build slow first time | IL2CPP first build is slow. Subsequent builds faster |
| Missing TextMeshPro | Window → TextMeshPro → Import TMP Essentials |
| Compile errors | Wait for Unity to finish background compilation (bottom-right) |