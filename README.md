# MathWorld: Digit Quest

**A Unity 2D educational RPG that replaces combat with arithmetic problem-solving.**  
Designed to help children (ages 7–11) engage in maths practice through game mechanics that tie quiz performance to progression and success.

> A Unity 2D maths-quiz RPG serious game for children (ages 7–11), replacing traditional combat with problem-solving mechanics. :contentReference[oaicite:2]{index=2}

## Playable Demo

A playable version of *MathWorld: Digit Quest* is hosted on itch.io:

https://betapbo.itch.io/mathworld  
**Password:** `1234`

This demo reflects the core gameplay and educational features described in this repository. Reviews, feedback, and gameplay insights from this demo can be found on the itch.io page.

## Academic Context
This project was completed as my **graduate thesis project** at **University College Cork (UCC)**.  
**Supervisor:** Dr John O’Mullane.

Development spanned approximately **five months** and was completed as a solo developer.  
Visual and audio resources used in this project are from free/public assets and were integrated appropriately.

## Game Overview
In *MathWorld: Digit Quest*, the player explores a 2D RPG world, interacts with NPCs, accepts quests, and engages in “combat” by solving mathematics problems.

### Core Gameplay
- **Math quiz replaces traditional combat:** maths performance translates to in-game success.  
- **Difficulty progression:** questions scale with player level and are categorised by arithmetic operation types.  
- **Mini-games:** implementations such as Sudoku and 2048 help reinforce arithmetic skills and offer variation.  
- **RPG systems:** quests, character progression, inventory, and item usage support exploratory and learning gameplay.

## Educational Design
The game focuses on four arithmetic operations—addition, subtraction, multiplication, and division—with content structured to support incremental learning and prevent player fatigue.

## Technical Highlights
- **Unity (C#)**: Game logic, UI, and systems implemented in a modular, component-based architecture.
- **Data Persistence:** JSON-based save/load system preserving player progress, inventory, and quest status.
- **ScriptableObjects:** Used for configurable game data and balancing.
- **Event-Driven Architecture:** Loose coupling between systems for readability and maintainability.
- **Dynamic Question Generation:** Algorithmic creation of quiz content with constraints to avoid trivial or invalid combinations.

## Project Structure
- Assets/Scripts/ # Primary game logic
- Assets/Scripts/MathsQuiz/ # Maths quiz system
- Assets/Scripts/RPGSystems/ # Quest, inventory, progression systems
- Assets/Scripts/ UI/ # UI and HUD management
- Assets/Scripts/Persistence/ # Save/load systems
- Assets/Scripts/Utilities/ # Helpers, event management, data containers
- Assets/Scenes/ # Main game scenes
- Art # Free/public visual assets
- Audio # Free/public sound/music assets
- Packages/
- ProjectSettings/
.gitignore

*(Rename or reorganise this snippet if your folders differ slightly.)*

## How to Run
1. Clone the repository.  
2. Open in **Unity Hub** with the version specified in `ProjectSettings`.  
3. Load the main game scene.  
4. Press **Play** to run in the editor.

## Learning & Evaluation Insights
Testing and evaluation included informal playtesting sessions demonstrating:
- intuitive navigation,
- appropriate difficulty scaling,
- positive correlation between maths performance and in-game progression.

Due to scope constraints, this does not constitute comprehensive educational research, but indicates feasibility of this design approach.

## Acknowledgements
The project integrates free visual and audio assets. Credits for these assets can be found within the project’s asset folders.

## Contact
- GitHub: https://github.com/RuiqingCHEN  

## Citation (Academic)
If referencing this work:

**Ruiqing Chen (2025).** *MathWorld: Digit Quest – Educational Serious Game for Children*. MSc Thesis, University College Cork.

