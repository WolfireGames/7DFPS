#pragma strict

var sound_shell_bounce : AudioClip[];
var collided = false;
var old_pos : Vector3;
var life_time = 0.0;

function PlaySoundFromGroup(group : Array, volume : float){
	var which_shot = Random.Range(0,group.length-1);
	audio.PlayOneShot(group[which_shot], volume);
}

function Start () {
	old_pos = transform.position;
}

function CollisionSound() {
	if(!collided){
		collided = true;
		PlaySoundFromGroup(sound_shell_bounce, 0.3);
	}
}

function FixedUpdate () {
	if(rigidbody && !rigidbody.IsSleeping() && collider && collider.enabled){
		life_time += Time.deltaTime;
		var hit : RaycastHit;
		if(Physics.Linecast(old_pos, transform.position, hit, 1)){
			transform.position = hit.point;
			transform.rigidbody.velocity *= -0.3;
		}
		if(life_time > 2.0){
			rigidbody.Sleep();
		}
	}
	old_pos = transform.position;
}

function OnCollisionEnter (collision : Collision) {
	CollisionSound();
}