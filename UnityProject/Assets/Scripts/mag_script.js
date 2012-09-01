#pragma strict

private var num_rounds = 8;
var kMaxRounds = 8;
private var round_pos : Vector3[];
private var round_rot : Quaternion[];
private var old_pos : Vector3;
var hold_offset : Vector3;
var hold_rotation : Vector3;
var collided = false;
var sound_add_round : AudioClip[];
var sound_mag_bounce : AudioClip[];
var life_time = 0.0;
enum MagLoadStage {NONE, PUSHING_DOWN, ADDING_ROUND, REMOVING_ROUND, PUSHING_UP};
var mag_load_stage = MagLoadStage.NONE;
var mag_load_progress = 0.0;

function RemoveRound() : boolean {
	if(num_rounds == 0){
		return false;
	}
	var round_obj = transform.FindChild("round_"+num_rounds);
	round_obj.renderer.enabled = false;
	--num_rounds;
	return true;
}

function RemoveRoundAnimated() : boolean {
	if(num_rounds == 0 || mag_load_stage != MagLoadStage.NONE){
		return false;
	}
	mag_load_stage = MagLoadStage.REMOVING_ROUND;
	mag_load_progress = 0.0;
	return true;
}

function IsFull() : boolean {
	return num_rounds == kMaxRounds;
}

function AddRound() : boolean {
	if(num_rounds >= kMaxRounds || mag_load_stage != MagLoadStage.NONE){
		return false;
	}
	mag_load_stage = MagLoadStage.PUSHING_DOWN;
	mag_load_progress = 0.0;
	PlaySoundFromGroup(sound_add_round, 0.3);
	++num_rounds;
	var round_obj = transform.FindChild("round_"+num_rounds);
	round_obj.renderer.enabled = true;
	return true;
}

function NumRounds() : int {
	return num_rounds;
}

function Start () {
	old_pos = transform.position;
	num_rounds = Random.Range(0,kMaxRounds);
	round_pos = new Vector3[kMaxRounds];
	round_rot = new Quaternion[kMaxRounds];
	for(var i=0; i<kMaxRounds; ++i){
		var round = transform.FindChild("round_"+(i+1));
		round_pos[i] = round.localPosition;
		round_rot[i] = round.localRotation;
		if(i < num_rounds){
			round.renderer.enabled = true;
		} else {
			round.renderer.enabled = false;
		}
	}
}

function PlaySoundFromGroup(group : Array, volume : float){
	if(group.length == 0){return;}
	var which_shot = Random.Range(0,group.length);
	audio.PlayOneShot(group[which_shot], volume * PlayerPrefs.GetFloat("sound_volume", 1.0));
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
		collided = false;
	}
	old_pos = transform.position;
}

function Update() {
	switch(mag_load_stage){
		case MagLoadStage.PUSHING_DOWN:
			mag_load_progress += Time.deltaTime * 20.0;
			if(mag_load_progress >= 1.0){
				mag_load_stage = MagLoadStage.ADDING_ROUND;
				mag_load_progress = 0.0;
			}
			break;
		case MagLoadStage.ADDING_ROUND:
			mag_load_progress += Time.deltaTime * 20.0;
			if(mag_load_progress >= 1.0){
				mag_load_stage = MagLoadStage.NONE;
				mag_load_progress = 0.0;
				for(var i=0; i<num_rounds; ++i){
					var obj = transform.FindChild("round_"+(i+1));
					obj.localPosition = round_pos[i];
					obj.localRotation = round_rot[i];
				}
			}
			break;
		case MagLoadStage.PUSHING_UP:
			mag_load_progress += Time.deltaTime * 20.0;
			if(mag_load_progress >= 1.0){
				mag_load_stage = MagLoadStage.NONE;
				mag_load_progress = 0.0;
				RemoveRound();
				for(i=0; i<num_rounds; ++i){
					obj = transform.FindChild("round_"+(i+1));
					obj.localPosition = round_pos[i];
					obj.localRotation = round_rot[i];
				}
			}
			break;
		case MagLoadStage.REMOVING_ROUND:
			mag_load_progress += Time.deltaTime * 20.0;
			if(mag_load_progress >= 1.0){
				mag_load_stage = MagLoadStage.PUSHING_UP;
				mag_load_progress = 0.0;
			}
			break;
	}
	switch(mag_load_stage){
		case MagLoadStage.PUSHING_DOWN:
			obj = transform.FindChild("round_1");
			obj.localPosition = Vector3.Lerp(transform.FindChild("point_start_load").localPosition, 
											 transform.FindChild("point_load").localPosition, 
											 mag_load_progress);
			obj.localRotation = Quaternion.Slerp(transform.FindChild("point_start_load").localRotation, 
												 transform.FindChild("point_load").localRotation, 
												 mag_load_progress);
			for(i=1; i<num_rounds; ++i){
				obj = transform.FindChild("round_"+(i+1));
				obj.localPosition = Vector3.Lerp(round_pos[i-1], round_pos[i], mag_load_progress);
				obj.localRotation = Quaternion.Slerp(round_rot[i-1], round_rot[i], mag_load_progress);
			}
			break;
		case MagLoadStage.ADDING_ROUND:
			obj = transform.FindChild("round_1");
			obj.localPosition = Vector3.Lerp(transform.FindChild("point_load").localPosition, 
											 round_pos[0], 
											 mag_load_progress);
			obj.localRotation = Quaternion.Slerp(transform.FindChild("point_load").localRotation, 
												 round_rot[0], 
												 mag_load_progress);
			for(i=1; i<num_rounds; ++i){
				obj = transform.FindChild("round_"+(i+1));
				obj.localPosition = round_pos[i];
			}
			break;
		case MagLoadStage.PUSHING_UP:
			obj = transform.FindChild("round_1");
			obj.localPosition = Vector3.Lerp(transform.FindChild("point_start_load").localPosition, 
											 transform.FindChild("point_load").localPosition, 
											 1.0-mag_load_progress);
			obj.localRotation = Quaternion.Slerp(transform.FindChild("point_start_load").localRotation, 
												 transform.FindChild("point_load").localRotation, 
												 1.0-mag_load_progress);
			for(i=1; i<num_rounds; ++i){
				obj = transform.FindChild("round_"+(i+1));
				obj.localPosition = Vector3.Lerp(round_pos[i-1], round_pos[i], mag_load_progress);
				obj.localRotation = Quaternion.Slerp(round_rot[i-1], round_rot[i], mag_load_progress);
			}
			break;
		case MagLoadStage.REMOVING_ROUND:
			obj = transform.FindChild("round_1");
			obj.localPosition = Vector3.Lerp(transform.FindChild("point_load").localPosition, 
											 round_pos[0], 
											 1.0-mag_load_progress);
			obj.localRotation = Quaternion.Slerp(transform.FindChild("point_load").localRotation, 
												 round_rot[0], 
												 1.0-mag_load_progress);
			for(i=1; i<num_rounds; ++i){
				obj = transform.FindChild("round_"+(i+1));
				obj.localPosition = round_pos[i];
				obj.localRotation = round_rot[i];
			}
			break;
	}
}

function OnCollisionEnter (collision : Collision) {
	CollisionSound();
}