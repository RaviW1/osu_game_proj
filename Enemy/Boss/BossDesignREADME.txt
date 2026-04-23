Design Notes on the flow of the boss states
Boss starts in idle state

Jump:
 - Boss moves around by jumping in the air and landing in a slam
Run:
 - Boss will dash from one end of the room to the other

Attack:
Before boss attacks, there is a anticipation state that can 
transition to three seperate attacks
Has three attacks(we can choose which ones to implement)
 - there is a simple slam with the hammer
 - there is a slam with the the hammer on both sides repeatedly
	- this attack  spawns a bunch of projectiles
 - there is a jump attack 
	- the boss jumps, moves a distance, and comes down with a slam

Attacks will also result in a recovery animation that leaves the boss vulnerable to counter attacks

Vulnerable State 
 - after a certain amount of damage has been taken by the boss,
 the boss reveals its head that can be damaged, 
 - this flow repeats until the boss dies

NOTES:
 - boss should have hitbox that allows player to dash underneath 
 - touching the boss at all during any state ( except the vulnerable state) causes the player to take damage
