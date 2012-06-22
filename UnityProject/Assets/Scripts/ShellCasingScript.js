#pragma strict

var sound_shell_bounce : AudioClip[];
var collided = false;
var old_pos : Vector3;
var life_time = 0.0;
var glint_delay = 0.0;
var glint_progress = 0.0;
private var glint_light:Light;

function PlaySoundFromGroup(group : Array, volume : float){
	var which_shot = Random.Range(0,group.length);
	audio.PlayOneShot(group[which_shot], volume * PlayerPrefs.GetFloat("sound_volume", 1.0));
}

function Start () {
	old_pos = transform.position;
	if(transform.FindChild("light_pos")){
		glint_light = transform.FindChild("light_pos").light;
		glint_light.enabled = false;
	}
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
	if(rigidbody && rigidbody.IsSleeping() && glint_light){
		if(glint_delay == 0.0){
			glint_delay = Random.Range(1.0,5.0);
		}
		glint_delay = Mathf.Max(0.0, glint_delay - Time.deltaTime);
		if(glint_delay == 0.0){
			glint_progress = 1.0;
		}
		if(glint_progress > 0.0){
			glint_light.enabled = true;
			glint_light.intensity = Mathf.Sin(glint_progress * Mathf.PI);
			glint_progress = Mathf.Max(0.0, glint_progress - Time.deltaTime * 2.0);
		} else {
			glint_light.enabled = false;
		}
	}
	old_pos = transform.position;
}

function OnCollisionEnter (collision : Collision) {
	CollisionSound();
}