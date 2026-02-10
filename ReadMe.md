# Match-3 Game (Unity)

A small match-3 game implemented as part of a technical assignment.  
The project was intentionally kept as simple and readable as possible, without using third-party libraries or frameworks.

apk: https://drive.google.com/file/d/1RNBHJLPJ145X72-d8o94KktxChgk8qRa/view?usp=sharing

---

## Technologies

- **Engine:** Unity 2022.3.62f2 (LTS)
- **Language:** C#
- **Scenes:** 1 (`Main`)

**No third-party plugins are used:**
- DoTween
- UniTask
- Zenject

All logic is implemented using standard Unity features only.

---

## Gameplay Description

- Classic match-3 mechanics
- The game board consists of blocks of different types
- Blocks are moved using swipe gestures
- After each move, the board is normalized:
  - gravity is applied
  - matches are searched
  - matched blocks are destroyed
- A level is considered completed when **all blocks are destroyed**

---

## Save System

Game progress is fully saved between sessions:

- current level
- complete state of the game board

### Save File Location
Application.persistentDataPath/game_save.json

- Save format: **JSON**
- Saving is performed:
  - when the application is minimized
  - when the application is closed
  - after any change to the game board state

---

## Project Architecture

The project is built with a clear separation of responsibilities:

- **GameManager** — loads configurations and initializes core systems
- **GamePlayController** — manages gameplay flow and scenarios
- **BoardModel** — data model of the game board
- **BoardSystem** — game rules (moves, gravity, matches)
- **BoardVisual** — visual representation of the board
- **BoardController** — gameplay flow (coroutines, action sequencing)
- **UIManager** — UI window management
- **GameStats** — current game state (level, events)
- **SaveSystem** — save/load system

UI and gameplay logic communicate via events, without tight coupling.

---

## Running the Project

1. Open the project in **Unity 2022.3.62f2**
2. Open the `Main` scene
3. Press **Play**

---

## Notes

- The project is focused on simplicity and maintainability
- The architecture is easy to extend (new levels, UI, win conditions)
- The code is written without over-engineering

---