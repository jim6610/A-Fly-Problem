/****************************************************/
A Fly Problem
By: 
	Jahrel Stewart 40115728, Jimmy Wong 26649815, 
	Kevin Brousseau 40060549, Lorenzo Monge 40045872, 
	Steven Labelle 40005118, Xavier Knoll 40132134
/****************************************************/


----- Game Objectives -----
- To exterminate all the flies in a level within the time limit. Doing so will grant the player money that can be used to purchase new weapons in the shop.


----- Rules -----
- Level is over if timer runs out or if the player kills all the flies. 


----- Mechanics -----
- Destructible Objects
	- Destructible objects can be picked up and thrown as a weapon. It is possible to kill enemies by doing this, however the player will lose money due to damages.
	- Every destructible object has a value, and that value is substracted from the contract value if it is destroyed.
- Stealth
	- Crouching will reduce the player detection range on the flies as well as turning off the lights. This will allow the player to get closer to the fly before it
	  detects the player and flies away.
	- Turning off the lights will also reduce the player's visibilty but will also reduce the speed of the flies.
- Shop 
	- The shop menu allows the player to purchase new weapons and decide what to equip for the level.


----- Enemy Types -----
- Fly
	- Will fly around the map and can land for a few seconds.
	- Will fly away if player approaches
- Spider
	- Will wander around the map and spawn webs after a set amount of time.
	- If the player walks over the web, their movement and look speed will be reduced momentarily. 
- Scorpion
	- Will target the player and poison them. Poison will cause player's movements to be inverted for a few seconds.
	- Scorpion will attack again when its attack cooldown is over. 


----- Controls -----
- Strafe Forward                   W
- Strafe Left                      A
- Strafe Backward                  S
- Strafe Left                      D
- Pick up/throw object/interact    E
- Run                              hold L-SHIFT
- Toggle Crouch                    L-CTRL
- Attack                           LMB
- Switch weapons                   mouse wheel or number keys [1-4]


----- Sources -----
Asset packs
	3Dfrk
	- https://assetstore.unity.com/publishers/33751
	9t5 Low poly insects
	- https://assetstore.unity.com/packages/3d/characters/animals/insects/9t5-low-poly-insects-202139
	Destructible props pack 
	- https://assetstore.unity.com/packages/3d/props/destructible-props-pack-27379
	Bar Stool
	- https://assetstore.unity.com/packages/3d/props/bar-stool-98790
	Wine Glass and Bottle
	- https://assetstore.unity.com/packages/3d/props/wine-glass-bottle-124055



Models
	Breakable glass
	- http://peerplay.nl/unity/breaking-glass/

	Furniture Models
	- https://free3d.com/3d-model/small-tv-table-708240.html
	- https://free3d.com/3d-model/casual-sofa-denim-v1--415793.html
	- https://free3d.com/3d-model/wooden-chair-709228.html
	- https://free3d.com/3d-model/-office-desk-v3--821728.html
	- https://free3d.com/3d-model/master-bed-king-size-v2--968638.html
	- https://free3d.com/3d-model/dresser-old-65315.html
	- https://free3d.com/3d-model/night-stand-v2--465221.html
	- https://free3d.com/3d-model/jacuzzi-30573.html
	- https://free3d.com/3d-model/rubbish-bin-83371.html
	- https://free3d.com/3d-model/wine-shelves-v2-537113.html


Audio
- Music
- SFX
	"Glass Smash, Bottle, D.wav" by InspectorJ (www.jshaw.co.uk) of Freesound.org
	- https://freesound.org/people/InspectorJ/sounds/344265/
	Wine glass breaking
	- https://freesound.org/people/Pablobd/sounds/502999/
	Gun shot
	- https://freesound.org/people/schots/sounds/382735/
	Rife Reload
	- https://freesound.org/people/GFL7/sounds/276963/
	Pistol Reload
	- https://freesound.org/people/nioczkus/sounds/396331/
	Rifle Clip Empty
	- https://freesound.org/people/michorvath/sounds/427603/
	Pistol Clip Empty
	- https://freesound.org/people/Sophia_C/sounds/467183/


Tutorials
SHATTER / DESTRUCTION
- https://www.youtube.com/watch?v=EgNV0PWVaS8
Shooting with Raycasts
- https://www.youtube.com/watch?v=THnivyG0Mvo
Weapon switching
- https://www.youtube.com/watch?v=Dn_BUIVdAPg
Weapon Reloading
- https://www.youtube.com/watch?v=kAx5g9V5bcM
