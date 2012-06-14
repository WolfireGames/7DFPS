#pragma strict

var sound_hit_concrete : AudioClip[];
var sound_hit_metal : AudioClip[];
var sound_hit_glass : AudioClip[];
var sound_hit_body : AudioClip[];
var sound_hit_ricochet : AudioClip[];
var sound_flyby : AudioClip[];
var bullet_obj : GameObject;
var bullet_hole_obj : GameObject;
var metal_bullet_hole_obj : GameObject;
var spark_effect : GameObject;
var puff_effect : GameObject;
private var old_pos;
private var hit_something = false;
private var line_renderer : LineRenderer; 
private var velocity : Vector3;
private var life_time = 0.0;
private var death_time = 0.0;
private var segment = 1;
private var hostile = false;

function SetVelocity(vel : Vector3){
	this.velocity = vel;
}

function SetHostile(){
	audio.rolloffMode = AudioRolloffMode.Logarithmic;
	PlaySoundFromGroup(sound_flyby, 0.4);
	hostile = true;
}
		
function Start () {
	line_renderer = GetComponent(LineRenderer);
	line_renderer.SetPosition(0, transform.position);
	line_renderer.SetPosition(1, transform.position);
	old_pos = transform.position;
}

function RecursiveHasScript(obj : GameObject, script : String, depth : int) : MonoBehaviour {
	if(obj.GetComponent(script)){
		return obj.GetComponent(script);
	} else if(depth > 0 && obj.transform.parent){
		return RecursiveHasScript(obj.transform.parent.gameObject, script, depth-1);
	} else {
		return null;
	}
}

function RandomOrientation() : Quaternion {
	return Quaternion.EulerAngles(Random.Range(0,360),Random.Range(0,360),Random.Range(0,360));
}

function PlaySoundFromGroup(group : Array, volume : float){
	var which_shot = Random.Range(0,group.length-1);
	audio.PlayOneShot(group[which_shot], volume);
}

function Update () {
	if(!hit_something){
		life_time += Time.deltaTime;
		if(life_time > 1.5){
			hit_something = true;
		}
		transform.position += velocity * Time.deltaTime;
		velocity += Physics.gravity * Time.deltaTime;
		var hit:RaycastHit;
		if(Physics.Linecast(old_pos, transform.position, hit, 1<<0 | 1<<9 | 1<<11)){
			var hit_obj = hit.collider.gameObject;
			var hit_transform_obj = hit.transform.gameObject;
			var aim_script : AimScript = RecursiveHasScript(hit_obj, "AimScript", 1);
			var turret_script : StationaryTurretScript = RecursiveHasScript(hit_obj, "StationaryTurretScript", 3);
			transform.position = hit.point;
			var ricochet_amount = Vector3.Dot(velocity.normalized, hit.normal) * -1.0;
			if(Random.Range(0.0,1.0) > ricochet_amount && Vector3.Magnitude(velocity) * (1.0-ricochet_amount) > 10.0){
				var ricochet = Instantiate(bullet_obj, hit.point, transform.rotation);
				var ricochet_vel = velocity * 0.3 * (1.0-ricochet_amount);
				velocity -= ricochet_vel;
				ricochet_vel = Vector3.Reflect(ricochet_vel, hit.normal);
				ricochet.GetComponent(BulletScript).SetVelocity(ricochet_vel);
				PlaySoundFromGroup(sound_hit_ricochet, hostile ? 1.0 : 0.6);
			} else if(turret_script && velocity.magnitude > 100.0){
				var new_hit:RaycastHit;
				if(Physics.Linecast(hit.point + velocity.normalized * 0.001, hit.point + velocity.normalized, new_hit, 1<<11 | 1<<12)){
					if(new_hit.collider.gameObject.layer == 12){
						turret_script.WasShotInternal(new_hit.collider.gameObject);
					}
				}					
			}
			if(hit_transform_obj.rigidbody){
				hit_transform_obj.rigidbody.AddForceAtPosition(velocity * 0.01, hit.point, ForceMode.Impulse);
			}
			if(Vector3.Magnitude(velocity) > 50){
				var hole : GameObject;
				var effect : GameObject;
				if(turret_script){
					PlaySoundFromGroup(sound_hit_metal, hostile ? 1.0 : 0.8);
					hole = Instantiate(metal_bullet_hole_obj, hit.point, RandomOrientation());
					effect = Instantiate(spark_effect, hit.point, RandomOrientation());
					turret_script.WasShot(hit_obj, hit.point, velocity);
				} else if(aim_script){
					hole = Instantiate(bullet_hole_obj, hit.point, RandomOrientation());
					effect = Instantiate(puff_effect, hit.point, RandomOrientation());
					PlaySoundFromGroup(sound_hit_body, 1.0);
					aim_script.WasShot();
				} else {
					PlaySoundFromGroup(sound_hit_concrete, hostile ? 1.0 : 0.4);
					hole = Instantiate(bullet_hole_obj, hit.point, RandomOrientation());
					effect = Instantiate(puff_effect, hit.point, RandomOrientation());
				}
				effect.transform.position += hit.normal * 0.05;
				hole.transform.position += hit.normal * 0.01;
				if(!aim_script){
					hole.transform.parent = hit_obj.transform;
				} else {
					hole.transform.parent = GameObject.Find("Main Camera").transform;
				}
			}
			hit_something = true;
		}
		line_renderer.SetVertexCount(segment+1);
		line_renderer.SetPosition(segment, transform.position);
		++segment;
	} else {
		life_time += Time.deltaTime;
		death_time += Time.deltaTime;
		//Destroy(this.gameObject);
	}
	for(var i=0; i<segment; ++i){
		var start_color = Color(1,1,1,(1.0 - life_time * 5.0)*0.05);
		var end_color = Color(1,1,1,(1.0 - death_time * 5.0)*0.05);
		line_renderer.SetColors(start_color, end_color);
		if(death_time > 1.0){
			Destroy(this.gameObject);
		}
	}
}