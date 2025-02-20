


# General Development Road Map

## Round 1 (Completed)
Phase 1: Project Setup
Issue 1: Initialize Unity Project

Description: Set up a Unity project in Unity 6000.0.32f1. Configure project settings for 3D development and import existing 3D models, assets from the Unity Asset Store, and 2D GUI components.
Acceptance Criteria:
Unity project initialized with 3D settings.
All required assets imported and organized into appropriate folders (e.g., Models, Textures, UI, Scripts).
Issue 2: Version Control Setup

Description: Integrate the Unity project with Git and set up a .gitignore file to exclude unnecessary files (e.g., Library, Temp, Logs).
Acceptance Criteria:
Git repository initialized, .gitignore configured for Unity.
Phase 2: Basic Gameplay Mechanics
Issue 3: Create Initial Terrain and Environment

Description: Design a simple terrain using Unity's Terrain tools and add environmental props from the low-poly asset pack.
Acceptance Criteria:
One functional terrain with environmental props placed.
Issue 4: Add Player Character Model

Description: Import and set up a humanoid character model in the scene.
Acceptance Criteria:
Character model added to the scene and positioned correctly.
Issue 5: Implement Basic Movement

Description: Add WASD movement and spacebar jumping functionality to the character.
Acceptance Criteria:
Character moves using WASD keys.
Character jumps with the spacebar.
Issue 6: Implement Camera Controller

Description: Add a third-person camera that follows the player.
Acceptance Criteria:
Camera follows player with a smooth motion.
Player remains centered in view.
Issue 7: Test Gameplay Functionality

Description: Test the terrain, character, and movement mechanics.
Acceptance Criteria:
All movement and camera mechanics function as expected.
Phase 3: Character Rigging and Animation
Issue 8: Set Up Character Rig and Animations

Description: Configure the humanoid rig and add simple animations (e.g., idle, walking, jumping).
Acceptance Criteria:
Rig maps correctly to the character.
Basic animations play in response to player input.

## Round 2

### Phase 1 (Completed)
1. Refactor Current Work for Scalability
Before adding new features, ensure your existing implementation is scalable for multiplayer and extendable for future updates. Start by:

Prefab-Driven Design: Convert your player character into a prefab. This prefab should include:
Humanoid model
Animator controller
Player scripts (movement, animation, and input)
Placeholder components for future additions (e.g., inventory, powers, networking logic)
Script Architecture:
Separate input handling from movement logic to facilitate compatibility with networking (e.g., Unity's Input System).
Use interfaces or inheritance for character controllers, allowing you to extend functionality for AI or custom characters.
Environment Setup:
Ensure your scene is modular (prefabs for environment objects) to facilitate loading/unloading of assets.
2. Implement Multiplayer Framework
This step will introduce core multiplayer functionality that the rest of the systems will integrate with:
- Networking Solution:
  - Use Unity's Netcode for GameObjects (NGO), Photon, or Mirror for multiplayer.
  - Set up basic functionality for a host and client system.
- Player Spawning:
  - Dynamically spawn player prefabs with unique identifiers.
  - Ensure movement, animations, and cameras work correctly across the network.
- Scene Management:
  - Add basic support for loading scenes (e.g., the main menu, game maps) across clients.

### Phase 2 (Currently In Progress)
3. Expand Player Functionality
Enhance your player mechanics, making them multiplayer-compatible:

Basic Combat:
Add simple melee combat (e.g., sword slashes).
Implement health and damage systems using modular components (e.g., health script, damage script).
Inventory System:
Create a modular inventory system with the ability to equip items and manage inventory slots.
Sync inventory changes across the network.
Mistborn Powers Framework:
Set up a power system framework, allowing powers to be modular (e.g., abstract base class for powers with specific implementations).
Create placeholders for powers but delay specific implementations until the framework is stable.
4. Create the Main Menu and UI
Develop a functional main menu to host/join games and set up player options:

Main Menu:
Host/join game functionality linked to your networking framework.
Add a "Learn to Play" section with basic instructions and controls.
Character Customization:
Implement a character selection screen with power presets and skins.
Sync selected presets and skins to clients in multiplayer sessions.
5. Game Modes and Core Features
Introduce game modes and core mechanics:

Game Modes:
Start with a simple mode (e.g., Free-for-All) and expand.
Use a game manager script to handle objectives, scoring, and player spawns.
Advanced Combat:
Introduce ranged attacks (e.g., coin pushes, bows).
Implement hit detection and syncing for ranged attacks in multiplayer.
Physics Interaction:
Use Unity's physics system for dynamic coin pushes and environmental interaction.
6. Expand Mistborn Powers
Begin implementing specific Mistborn powers:

Allomantic Powers:
Choose simple, iconic powers first (e.g., Steelpush and Ironpull).
Use Unity's physics engine to calculate forces and interactions with objects.
Power Framework:
Allow multiple powers to be assigned per player but manage balance via presets.
Implement cooldowns, energy costs, and levels for each power.
Sync Across Clients:
Ensure powers are functional and synced in multiplayer before expanding further.
7. Polish and Optimize
Focus on performance, usability, and presentation:

Animations:
Refine animations for smoother transitions and additional combat moves.
Add Mistborn-specific animations (e.g., metal flaring or coin-pushing effects).
Visual Effects:
Add particle effects for Mistborn powers (e.g., metal flares, coin impacts).
Optimization:
Optimize network performance, object pooling, and LOD for assets.
Playtesting:
Conduct iterative playtesting to refine balance, mechanics, and bugs.