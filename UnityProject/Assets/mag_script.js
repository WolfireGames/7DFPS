#pragma strict

private var num_rounds = 8;
private var max_rounds = 8;
private var old_pos : Vector3;
var collided = false;
var sound_add_round : AudioClip[];
var sound_mag_bounce : AudioClip[];
var life_time = 0.0;

function RemoveRound() {
	var round_obj = transform.FindChild("round_"+num_rounds);
	round_obj.renderer.enabled = false;
	--num_rounds;
}

function AddRound() {
	if(num_rounds >= max_rounds){
		return;
	}
	PlaySoundFromGroup(sound_add_round, 0.3);
	++num_rounds;
	var round_obj = transform.FindChild("round_"+num_rounds);
	round_obj.renderer.enabled = true;
}

function NumRounds() : int {
	return num_rounds;
}

function Start () {
	old_pos = transform.position;
}

function PlaySoundFromGroup(group : Array, volume : float){
	if(group.length == 0){return;}
	var which_shot = Random.Range(0,group.length-1);
	audio.PlayOneShot(group[which_shot], volume);
}

function CollisionSound() {
	if(!collided){
		collided = true;
		PlaySoundFromGroup(sound_mag_bounce, 0.3);
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
	} else if(!rigidbody){
		life_time = 0.0;
	}
	old_pos = transform.position;
}

function OnCollisionEnter (collision : Collision) {
	CollisionSound();
}