# ecosystem-game
This is very much a work in progress still. It's a pet project, mainly as a means to become more familiar with the Unity game engine and coding in C#.
The aim is to make this a simulation-based party game, inspired by *Spore* (creating your own creature in an ecosystem of other creatures) and *The Really Nasty Horse Racing Game* (bet on the animal you think will do best in the current round).

## Gameplay
Imagine the setting: You and some friends are having a good time together in John's living room. John says "Hey I heard about this really cool game named ecosystem-game" and whips it up on the TV.
The gameplay is:
1. Players all create their creature in private (using their phones), which is ends up with stats such as reproductive rate, speed, perception, diet (carnivore/herbivore) among others. The creature also has a goal which determines how it scores points, which might be based off "animals killed", "animals spawned", "longevity" (etc).
2. All the creatures are displayed on the TV for all the players to see. Players take their bets on which creature will end up with the most points at the end of the simulation (might or might not be your own creature)
3. The creatures are placed in an ecosystem, 100 of each creature, and they live their life for *x* years. The simulation then ends
4. Players end up with more/less points based on how many points they bet on the winning/losing creature.
5. Repeat until you're sick of the game

## Roadmap
- [x] Hex grid coordinate system
- [] Units moving on grid
- [] Procedural terrain generation
- [] Units sensing on grid
- [] Units reacting to senses
- [] Creature creation

## Resources
Since this is my first time making a game, I started off following [catlike coding's hex grid tutorial](https://catlikecoding.com/unity/tutorials/hex-map/). I'll probably also be using [this Unity ecosystem simulation project](https://github.com/SebLague/Ecosystem-2) like a crutch too.
