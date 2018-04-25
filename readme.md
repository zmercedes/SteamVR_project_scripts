Rube Goldberg Challenge:

 0. [x] 0. set up hand control scripts
 1. [x] 1. add teleportation
 	- reworked arc renderer serveral times, now uses parabolic raycaster to determine time to target,
 	  then renders an arc.
 2. [ ] 2. add object grabbing/throwing -> _**added, needs polish**_
 3. [ ] 3. create rube goldberg objects
 	- try different geometries, colors, physics functions
 4. [ ] 4. create object menu -> _**added, needs polish**_
 	- attaches to right hand, appears on touchpad press
 5. [ ] 5. set special grab rules for rube goldberg objects
 	- can grab but cannot throw. on release, must stay in place
 6. [ ] 6. gameplay!
 	- create collectible that ball must touch in order to win
 	- reenable collectible on ball touching floor
 	- create goal that loads next level on ball hitting it after 
 	  having collected all collectibles
 	- create anti cheating mechanics: the ball must be thrown from initial
 	  platform area in order to not trigger this mechanic
 	- 4 different levels
 	- can limit the number of objects that can be placed in a 
 	   level, can vary per level
 7. [ ] 7. final polish!
 	- rework the aimer object to show position above ground
 	- create instruction UI
 	- make environment nice
 	- runs at 90fps

 Both Hand Actions:
    - Grab objects

 Right Hand Actions:
    - rube goldberg item picker/generator (touchpad)
 
 Left Hand Actions:
     touchpad activates teleportation