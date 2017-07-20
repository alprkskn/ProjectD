# ProjectD

My take of a simple top down RPG game.

This is supposed to be a base for creating top-down 2D games. I plan this project to bring together a tile editor, pathfinding, a simple state machine, a basic quest system, events/triggers, basic inventory and equipment system. It can later be used to create games for various genres.

## Current WIP
* Loading levels from Tiled exports is supported.
* Working A* on loaded levels with layer support for obstacles.
* A simple inventory system.
* Event/Trigger system. (Currently maintained from kind of an unorthodox pipeline. Might be managed from a different editor later.)
* Quests/Objectives. (Do not really support branching at the moment.)
* A basic state machine which is used in the AI implementation of agents.

## Future Work
* Items/Equipment with passive/active abilities and character stat boosts.
* Animations/Cutscenes
* A better character controller.
* An editor to create events/triggers and quests in a more intuitive way.
* Utilizing an existing dialogue editor or creating a simple one from scratch.
