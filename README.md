# Game_Project

A game for the Game Project -course.

This is a 2D action platformer built using the Unity game engine. The basic idea of the game is to fight your way through the level using your sword and "demonic" telekinesis powers.

The game has two scenes: a main menu and a playable demo level.
The game scene consists of a player, some enemies, interactable objects, the environment, a camera, a particle system, as well as its own Pause- and Game Over -menu screens.

The architecture of the game is sort of a mix between component-based and object-oriented. For example, the MovableObject-script is common between all objects that the player may move via telekinesis.
Similarly, the player and all enemies share the same ParticleSystem-script that is used to create an effect the character is hit. On the other hand there are some "stand-alone" scripts that contain all logic of a specific object.
Examples being the enemy scripts and the player script that share a lot of the same functionality. This was not always done on purpose necessarily and if I had more time I'd like to refactor the code and extract some of the shared functionality into separate scipts (I tried to do this but faced issues that I couldn't solve in time).
Especially the WarriorEnemyBehavior, which was added in a hurry at the end, is an ugly copy of the original BasicEnemyBehavior-script and should be refactored before continuing development.

Perhaps the most interesting feature of the game is the telekinesis. This was achieved with the combination of two scripts (Drag and MovableObject) and an invisible game object (DragPoint) the acts as the "anchor point" for grabbing things.
The Movable object is a simple interface that determines which objects in the scene can be dragged around. The DragPoint consists of a rigid body, a circle collider and a hinge joint, as well as the movable object script (so that it can also be moved).
In addition it contains the Drag-script component that makes the drag point follow the player's mouse position. When the player clicks on a movable object, the script creates a new FixedJoint-component, that together with the existing hinge joint in the DragPoint-object make the attached target object follow the drag point, and therefore the mouse position, in a "smooth" way.
Another interesting thing was the implementation of the flying enemies. The main "trick" here was freezing their Y-position to keep them in air instead of them falling straight down. I've since learned that giving them a gravity scale of 0 would have likely been a better and simpler solution.

The total work time is hard to estimate since I didn't keep track of it, but I can say for certain that I've spent more time on this course than most 5 credit University courses. I'm guessing the total time is somewhere between 120 and 180 hours, even though the game doesn't look like much.
Coming up with the concept, battling with all kinds of bugs and weird issues, designing/implementing the graphics and all the research on how to use Unity and implement some core functionality was more time consuming than I initially thought, and in the end the project ended up being more like a proof of concept than a game.


**Some known issues**
<ul>
<li>After completing the level and choosing to play again the game might freeze, so you might need to restart to play again.</li>
<li>Enemy movement is sometimes "buggy", e.g. fast movements, appearing inside colliders (ground), "sliding" on the ground or moving back and forth around the player</li>
<li>Small graphical bugs</li>
<li>Moving against a wall while jumping can make the player "levitate" on that spot</li>
</ul>
