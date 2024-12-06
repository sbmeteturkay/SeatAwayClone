## Seat Away Clone Case Study

Time: 4 days
This case study consists of a mock-game project.

**External Packages**:
- Odin Inspector
- Dotween
- Heart Package
- Init(args)

## Features:

### 1.Grid System

##### Grid system
-  using unity's Grid component for data and using a tiled sprite for rendering tiles. Creates grid cells for pathfinding. returns if an object can move nearsides.

##### Pathfinding
- used a* algorithm 

##### Grid Objects Controller 
- Spawns grid objects into given grid system. 
- You can get grid objects with given tile data.
-  Gives input from GridInput to selected grid object.
- has an event which fires when a object move into another cell

##### Grid Object
- Draggable object on grid with animation.

##### Grid Input
- casts a ray from camera to mouse position and fires input down and cancel events

### 2.Level design

I needed to create a basic level data to get defined materials from one place and create multiple levels easily to test.

##### Level design data
- passenger and seat prefabs to spawn
- colored material dictionary to reference from all over IColorable users and others.

##### Level data
- has level design data
- has passenger list 
- has matrix to create basic level which also sets the Grid systems size
  
![LevelData](https://github.com/user-attachments/assets/3f6d9864-9ca5-491b-a60e-3779d5e82afb)

##### Level manager
- Has level data
- Creates grid with grid system and init passenger and grid objects with object pooling using level data.



### 3. Gameplay

##### ColoredGridObject
- Grid object inheritance with IColorable implementation.

##### ColoredGridObjectController
- Has methods to return colored grid object 

##### Passenger
- has IColorable implementation 
- used state machine pattern which has 3 states
- onQueue
- MovingTowardsTarget
- Seated
- also has PassengerManager Observer implementation to observe when first one on the queue

##### PassengerManager
- Passenger manager holds the passenger which was created from the level manager. 
- Listens GridObjectControllers OnObjectMove to notify observers
- Line up on queue passengers 1 cell right from grid and 1 cell down from front of them
- Return observer to if target cell has sitting passenger on it
- fires an event when queue changes for ui


## Demo gif
![Movie_003](https://github.com/user-attachments/assets/c65c796f-9eec-4f51-9cb1-88410689808c)



