# Project 2 — El Con Ring 2.0's Game Documentation

- Developed by Duong Banh
- Course: CPSC 386-05 / Unity Project 2
- Github Link: https://github.com/DuongBanh2022/ElConRing2.1

## Project Overview

This project is a 2D Metroivania platformer inspired by with a mix of Umamusume, Sonic 2D Action Platformer, and Elden Ring/Soulslike in general. The player explores procedurally generated rooms, moves between platforms, fights enemies, breaks objects, eats flowers, and activates hidden switches to unlock the portal; the ultimate goal is to reach the finish line.

The game uses Unity's Input System for player controls, the Audio Mixer, Rigidbody2D for movement, Animator Controllers for character animations, StateMachineBehaviour scripts, ScriptableObjects for enemy properties, particle effects for collecting and destroying enemies, and procedural generation to create different room layouts.

The player can use melee attacks and ranged projectiles to defeat enemies and break pots. Some pots contain hidden switches that help complete the room puzzle. Once all required switches are activated, the portal becomes available, allowing the player to reach the destination.

## Script Summary

The project contains scripts for player control, procedural level generation, puzzle tracking, combat, enemies, collectibles, audio, UI/Menu Controller, and gameplay effects.

| Script | Purpose |
|---|---|
| `PlayerController.cs` | Controls player movement, jumping, double jumping, attacking, shooting, health, and player interactions using the Unity Input System. |
| `PlayerControls.cs` | Generated C# wrapper used to access the configured player input actions. |
| `ProceduralLevelGenerator.cs` | Generates rooms, platforms, tilemaps, hazards, decorations, enemies, flowers, pots, switches, and portal placement. |
| `PuzzleController.cs` | Tracks activated switches and determines when the puzzle is completed. |
| `Switch.cs` | Detects player interaction and reports switch activation to the PuzzleController. |
| `SwitchDoor.cs` | Similar to ' Switch.cs but specifically used for the Room Scene only. |
| `Door.cs` | Controls the door-opening behavior for the separate ordered-switch puzzle implementation in the Room scene. |
| `Pot.cs` | Handles breakable pots and reveals a hidden switch when one is assigned. |
| `Projectile.cs` | Controls projectile movement and damage to enemies or other valid targets. |
| `Enemy.cs` | Controls on-ground movement left and right over a short distance; can damage the player via a collider trigger. |
| `EnemyStats.cs` | ScriptableObject containing configurable properties for ground enemies. |
| `FlyEnemyStats.cs` | ScriptableObject containing configurable properties for flying enemies. |
| `FlyEnemy.cs` | Controls flying enemy movement, including circular movement behavior, using its assigned stats. |
| `Flower.cs` | Handles collectible objects, such as Flowers, and updates the player's collected amount. |
| `SoundManager.cs` | Manages game audio and provides persistent sound management across scenes. |
| `GameSession.cs` | Stores gameplay state such as the player's collected Flower. |
| `CameraFolllowScript.cs` | Controls camera following behavior during gameplay. |
| `AttackHitBox.cs` | Detects enemies or breakable objects inside the player's attack area and applies damage when the player attacks. |
| `DropFlower.cs` | Handles the flower drop created when an enemy is defeated. |
| `ExplosionEffect.cs` | Plays an explosion particle effect when an object, such as a pot or enemy, is destroyed. |
| `FinishTrigger.cs` | Detects when the player reaches the finish area and triggers the game completion sequence. |
| `Hazard.cs` | Detects when the player touches a hazard and applies damage. |
| `HudController.cs` | Updates the in-game HUD to display gameplay information, such as the player's health or collected items. |
| `InstructionBoard.cs` | Allows the player to interact with an instruction board to view the game's controls and instructions. |
| `LevelPortal.cs` | Handles player interaction with the level portal to move to another level or scene. |
| `PlayerHealthBar.cs` | Updates the health bar to visually represent the player's current health. |
| `PressAnyButton.cs` | Waits for player input before continuing from the title or introductory screen. |
| `SceneLoader.cs` | Handles loading and transitioning between game scenes. |



## Scene Descriptions

### TitleScreen

The TitleScreen scene provides the game's starting interface.

**Key GameObjects**
- Main Camera
- TitleScreen (Canvas)
  - Menu
    - Resume (Button)
    - Quit (Button)
  - HUDCanvas
    - BestLabel
- PressAnybutton
  - Panel
- EventSystem
- MenuManager
- SoundManager

**Interaction**

When the game starts, the TitleScreen displays the main menu and waits for the player to press a button to continue. The player can select "Find El" (resume the game) or "Quit to Desktop." The HUDCanvas displays the best score through the BestLabel. The MenuManager handles menu interactions, while the SoundManager manages the game's audio. And lastly, the EventSystem handles UI input.

### Room

The Room scene contains a playable room that demonstrates the switch and door puzzle system.

**Key GameObjects**
- Main Camera
- Grass (Player)
  - AttackHitboxF
  - AttackHitboxR
  - GraceSpark (Particle System)
  - GroundCheck
  - BulletSpawn
- Grid
  - WallBackground
  - Ground
  - Wall
  - Decoration
- Switch 
- Door
- Portal
- InstructionCanvas
- UICanvas
  - PausePanel
    - Resume
    - Quit
  - FlowerScore
- Mambo (FlyEnemy)
- BlackKnight (GroundEnemy)
- Pot (Breakable)
- Dandelion (Collectible)
- PuzzleController
- HUDController
- EventSystem

**Interaction**

Playable Character name: Grass Wonder, wielding her main weapon, Naginata, with her basic movement and jump inputs to explore the level. The GroundCheck detects whether she is standing on the ground, while AttackHitboxF and AttackHitboxR handle attacks in different directions. GraceSpark provides a particle effect during gameplay, and BulletSpawn determines where projectiles are created from her Rifle.

The Grid contains the level's tilemaps, including the ground, walls, background, and decorations (A picture of a Cow Squirrel Hybrid). The player can activate the Switch objects to complete the puzzle and unlock the Door to move on to the next scene via the Portal. The InstructionCanvas displays gameplay instructions, while the UICanvas displays the game's HUD.

Mambo (FlyEnemy) and BlackKnight (GroundEnemy) act as enemies. Grass can attack BlackKnight to drop Dandelion, Mambo drops non-collectible RigidBody ChickenLeg, both enemies have explosion particles when destroyed, and break the Pot to reveal to interact hidden Switch. Dandelion acts as a collectible, which is Grass's favorite food. The PuzzleController tracks switch activation and determines when the puzzle is completed. 

The UI Canvas displays a working Pause Panel that pauses mid-game, allowing you to quit to the main menu scene and reload from the start.

### Game1

The Game scene contains the main procedural platformer gameplay.

**Key GameObjects**
- Main Camera
- Grass
- LevelGenerator
- Grid
  - Ground
  - Wall 
  - Decoration 
  - Hazard 
  - Background 
- PuzzleController
- UICanvas
  - FlowerScore
  - PausePanel
    - Resume
    - Quit
- PlayerUI
  - PlayerHP
    - Background
    - Fill Area
    - Handle Slide Area
  - SwitchCounter
  - PortalNotification
- Pot Prefabs
- Portal Prefab
- BlackKnight Prefabs
- Mambo Prefabs
- Flower Prefabs
- DropFlower Prefabs
- Bullet Prefabs
- ChickenLeg Prefabs
- Switch Prefabs
- EventSystem

**Interaction**

The player controls Grass to explore the procedurally generated level. The LevelGenerator creates rooms and places platforms, hazards, decorations, enemies, collectibles, switches, pots, and portals throughout the level. The PuzzleController tracks switch activation and unlocks the portal once the required switches are activated.

Grass can attack enemies using the Melee attack or Bullet Prefabs, defeat enemies to eat flowers, and ChickenLeg is purely a comedy gag in the game. Pots can be broken to reveal hidden switches or other rewards. BlackKnight and Mambo act as ground and flying enemies, while hazards pose a danger to Grass, as she loses her flower on contact, inspired by Sonic's hurting mechanic.

The UICanvas displays the FlowerScore and PausePanel. PlayerUI displays the Grass's health through PlayerHP, the number of activated switches through SwitchCounter, and notifications when the portal becomes available. The player can use Resume to continue playing or Quit to leave the game. 

### FinishGame

The FinishGame scene provides the game's completion interface.

**Key GameObjects**
- Main Camera
- Grid
- Zephyr (2D Animation Sprite NPC)
- ElCheckPoint (NPC Box Collider 2D FinishLine)
- Background
- Grass
- UICanvas
- CompletionCanvas
  - FinishPanel
    - Best
    - Quit to TitleScreen
- HUDController
- EventSystem

**Interaction**

The final stage of Grass catches El at the finish line. Her fat hip- I mean her Box Collider 2D detects the finish-line trigger and activates the game completion sequence. The CompletionCanvas displays the FinishPanel to indicate the game is complete, and HUDController manages the completion interface. The Best object displays the player's best result, while the Quit to TitleScreen button allows the player to return to the TitleScreen scene.


## Project Requirement Satisfaction

### Requirement 1 — Platformer Movement

The player uses a Rigidbody2D component for physics-based movement and jumping. A GroundCheck object detects whether the player is touching the ground before allowing a jump. The player also supports double jumping to reach higher platforms.

The input actions include:

- Move — controls horizontal movement. (A/D)
- Jump — allows the player to jump and double jump. (Space)
- Attack — triggers the melee attack. (Left Mouse)
- Shoot — triggers the ranged projectile attack. (E)
- Interact — handles interactions with gameplay objects when configured. (F)

The `PlayerController.cs` script processes the input actions and controls the player's movement and actions.

### Requirement 2 — Puzzle State Tracking

The project uses `PuzzleController.cs` to track the player's progress through the switch puzzle.

Each switch detects player interaction through a trigger and reports its activation to the puzzle controller. The controller tracks activated switches and checks whether the required number of switches has been activated.

Some switches are hidden inside breakable pots or randomly spawn on certain platforms. When the player destroys a pot containing a switch, the hidden switch becomes available for activation.

When all required switches are activated, the puzzle is completed, and the portal can be unlocked.

The project also contains a separate specific implementation using `SwitchDoor.cs` and `Door.cs` in the Room scene.

### Requirement 3 — Audio and Persistence

The project uses a persistent `SoundManager` to manage audio across gameplay scenes. It remains active when transitioning between scenes and prevents duplicate audio managers from being created.

Audio is used to provide feedback for gameplay events such as eating Flowers (Collectibles), breaking pots, attacking, shooting, and defeating enemies.

The audio system keeps music and sound effects consistent when changing scenes without creating duplicate persistent audio managers.

AudioMixer groups separate music and sound effects, allowing their volumes to be controlled independently. 

Player audio settings are saved so volume preferences are retained between game sessions.

### Requirement 4 — ScriptableObject and Pure C#

The project uses ScriptableObjects to store configurable enemy properties separately from enemy behavior.

The enemy stat assets include:

  - EnemyStats — stores properties used by ground enemies.

  - FlyEnemyStats — stores properties used by flying enemies.

These assets let you configure enemy properties such as movement speed, appearance, and other gameplay values through the Unity Inspector.

The project also uses GameSession to hold gameplay state such as the number of collected Flowers.

The ScriptableObject assets and pure C# gameplay state help separate data from the MonoBehaviour scripts that control objects in the scene.

### Requirement 5 — Animator and Blend Tree

The game uses Unity's Animator Controller to manage the player's animations and transitions between different states. 

The Animator includes Idle, Running, Jumping, Attacking, Shooting, and Hurt animations to represent the player's actions during gameplay. 

Animator parameters such as IsRunnin (Bool), IsAttacking (Trigger), IsShooting (Trigger), and IsHurt (Trigger) are controlled through the PlayerController script. 

A Blend Tree uses movement parameters to smoothly control the player's movement animations based on the player's movement speed.

### Requirement 6 — Animation Event or StateMachineBehaviour

The project uses Animation Events to connect animation playback with gameplay actions.

For example, the shooting animation can call `FireBullet()` at the appropriate point in the animation. This lets the projectile appear when the character performs the firing motion, rather than immediately when the input is received.

The attack animation can also use an animation event to activate the attack hitbox at the correct moment.

This connects character animations with actual gameplay behavior. 

The player also has a hurt animation specifically for interacting with a spike hazard tile.

### Requirement 7 — Particle Effect from Code

The project uses Particle Systems to provide visual feedback during gameplay.

Scripts trigger particle effects when specific gameplay events occur.

Examples include:

- Flower or collectible pickup effects.
- Pot destruction explosions.
- Enemy destruction explosions.

The scripts instantiate or play particle effects when the corresponding gameplay event occurs. These effects make collection, combat, and object destruction more visually noticeable.

### Requirement 8 — Procedural Generation

The generator creates tilemap sections, platforms, hazards, decorations, enemies, flowers, pots, switches, and portal locations.

The generator exposes configurable Inspector settings, including:

- Room width and height.
- Number of initial rooms.
- Number of rooms generated ahead.
- Platform count and platform dimensions.
- Hazard count.
- Enemy count.
- Flower count.
- Puzzle switch requirements.
- Random seed.

The random seed lets you control the generated layout for testing and replaying different room arrangements.

The generator also includes placement rules to avoid placing important gameplay objects too close to the player's starting position or hazards.

The goal is to create different room layouts while keeping platforms and puzzle objects accessible so the player can complete the game.

## Third-Party Assets, Packages, and Tutorials

The project uses Unity's built-in systems and packages, including:

- Unity Input System for keyboard and mouse input.
- Unity 2D Tilemap system for procedural level construction.
- Unity Physics 2D for Rigidbody2D, Collider2D, and trigger interactions.
- Unity Animator for player and enemy animations.
- Unity Particle System for gameplay effects.
- Unity Audio System for music and sound effects.
- ScriptableObjects for configurable enemy data.

### Third-Party Assets

The project uses external visual, audio, and other resources where applicable.

The following information should be completed with the actual assets used in the Project 2 build:

- Player character sprites and animations.
- Enemy sprites and animations.
- Environment and platform tilesets.
- Collectible and puzzle sprites.
- Sound effects and background music.
- Particle effect textures or prefabs.

The following third-party assets are:

- (Characters)

  - Grass Wonder: 
    - She is from the popular Japanese gacha game franchise called Umamusume: Pretty Derby. 
      - https://umamusume.com/characters/grasswonder/
    - This specific design for this game is a fan-designed character called Tanuki, a deformed chibi in the Japanese 2chan thread "深夜のションボリたぬきスレ". 
    - The Tanuki artwork is mostly made by an anonymous user, but her Tanuki design is partially made by a user called Zzhao/Hakuhan (Their social media is impossible to find).

  - Black Knight (Enemy): 
    - Part of the Tanuki thread "深夜のションボリたぬきスレ".

  - Mambo (Enemy): 
    - El Condor Pasa's pet bird, an official character from Umamusume, and the artwork design is part of "深夜のションボリたぬきスレ".

  - El Condor Pasa (NPC): 
    - Official character from Umamusume.
      - https://umamusume.com/characters/elcondorpasa/
    - Her Tanuki design is from "深夜のションボリたぬきスレ".

  - Yamanin Zephyr (NPC): 
    - Official character from Umamusume.
      - https://umamusume.com/characters/yamaninzephyr/
    - Her Tanuki design is from "深夜のションボリたぬきスレ".

  - Maurice (Decoration Painting):
    - Part of the Tanuki thread "深夜のションボリたぬきスレ".

- (Platformer)

  - https://assetstore.unity.com/packages/2d/environments/pixel-art-platformer-village-props-166114
  - https://assetstore.unity.com/packages/2d/environments/asset-fttgr-free-pixel-art-platformer-222174
  - https://assetstore.unity.com/packages/p/pixel-2d-castle-tileset-135397
  - https://assetstore.unity.com/packages/2d/environments/platformer-set-150023

- (Object)

  - Justaway (Pot):
    - Based on Gintama's Justaway in-universe merchandise.
      - https://gintama.fandom.com/wiki/Justaway
    - Artwork design comes from "深夜のションボリたぬきスレ"

  - ChickenLeg:
    - Artwork design comes from "深夜のションボリたぬきスレ"

  - Flower:
    - https://btl-games.itch.io/springtime-dend

  - Portal:
    - https://benmusick.artstation.com/projects/xGr14

  - Switch:
    - https://assetstore.unity.com/packages/p/pixel-2d-castle-tileset-135397

- (UI)

  - Player's HP Handle Slider is custom-made by Duong Banh.

  - TitleScreen Background is hand-drawn and was used previously from Project 1.

- (Music)

  - (Elden Ring 8-bit) https://www.youtube.com/watch?v=Kl4-HAepMtM&list=RDKl4-HAepMtM&start_radio=1
  - (RERoom) https://youtu.be/8Hq0l4kd8tQ?si=wLZWULTJVwA3xopL
  - (CityCrisis8bit) https://youtu.be/KwuKYBMrlVg?si=AwLsuiXmqzIMHLF7
  - (DeathSound) https://youtu.be/VUfvRciny_Y?si=igGfiGjy3zyyyzTA
  - (Breaking Glass Sound) https://www.myinstants.com/en/search/?name=break%20glass
  - (Sekiro Parry Sound) https://www.youtube.com/watch?v=qtNDctR2iMM
  - (Fortnite Gun Sound) https://www.myinstants.com/en/search/?name=fortnite+gun
  - (GemChime) From Midterm Zippack
  - (Portal) Elden Ring Confirmation Sound https://www.zedge.net/notification-sounds/c1592250-a288-4475-9fb4-040d72881616
  - (Portal Appears) Elden Ring Traverse Mist Sound https://www.zedge.net/notification-sounds/3e0b9d30-aaf2-47a8-a51e-c0d29b3f90bf
  - (Result) https://www.youtube.com/watch?v=X4iYFVZwZoI&list=RDX4iYFVZwZoI&start_radio=1

Some scripts are from the Midterm Review/Exam with custom modifications, and some were previously used in Project 1.

### Tutorials and Learning References

Hurt Animation is guided and consulted by classmate Hector Mojica Arevalo, with approval to create a script for Hurt Animation and Drop Prefabs.

Hector's GitHub with approval: https://github.com/TamtungOS/Terminal-Dogma

Procedural Generations are varied sources, such as:
  - https://youtube.com/playlist?list=PLcRSafycjWFenI87z7uZHFv6cUG2Tzu9v&si=ZU5vuYeIKEVmnIwb
  - https://youtu.be/-5OS1s-NWRw?si=BrLE0oANMEuKeObY
  - https://youtu.be/NtY_R0g8L8E?si=8U2Od8okTQ29HJRA

Any tutorial code or examples used are from Project 1, and this project is a continuation that builds on newer concepts and lecture materials. 

The following tutorial references for the project are:
  - https://youtu.be/FXXc0hTWIMs?si=8yds-00jGFSc2YD7 
  - https://ezgif.com/gif-to-sprite (EzGif Sprite Sheet Converter)
  - https://www.youtube.com/shorts/bCzxNCvEtc0

