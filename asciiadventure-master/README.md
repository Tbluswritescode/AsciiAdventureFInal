asciiadventure
===
Ascii adventure in C#

## Game files
* [screen.cs](screen.cs)
* [adventure.cs](adventure.cs)
* [gameobjects.cs](gameobjects.cs)
* [player.cs](player.cs)
* [mob.cs](mob.cs)



### Challenges
* Not the greatest class hierarchy
* Mobs can only move when the player moves. There's no timer in the background.
* Not possible for moving gameobjs to move over objects with replacing them
* Really clunky menu options
* Not great MVC for display; the Screen is essentially everything
    * Need to draw moving gameobjs over the top of the underlying stuff
    * Two layers? Basic stuff, plus moving things?
* Not well set up to have multiple screens connected by portals, or something

### Other notes
* Screen is one screen, with numRows and numCols
    * 
* GameObject is an object in the game.
    * Owned by Screen, and does not need to know about screen
    * Some GameObjects include Wall and Treasure
    * Is passable? If it's passable, you can walk over it.
* MovingGameObject extends GameObject, but adds movement as a possibility.
    * Knows about Screen
    * needs to ask Screen about legal moves
