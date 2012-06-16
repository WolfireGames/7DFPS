#pragma strict

var sound_gunshot : AudioClip[];
var sound_damage_camera : AudioClip[];
var sound_damage_gun : AudioClip[];
var sound_damage_battery : AudioClip[];
var sound_damage_ammo : AudioClip[];
var sound_damage_motor : AudioClip[];
var sound_bump : AudioClip[];
var sound_alert : AudioClip;
var sound_unalert : AudioClip;
var sound_engine_loop : AudioClip;
var sound_damaged_engine_loop : AudioClip;

private var audiosource_taser : AudioSource;
private var audiosource_motor : AudioSource;
private var audiosource_effect : AudioSource;
private var audiosource_foley : AudioSource;

var electric_spark_obj : GameObject;
var muzzle_flash : GameObject;
var bullet_obj : GameObject;

enum RobotType {SHOCK_DRONE, STATIONARY_TURRET, MOBILE_TURRET, GUN_DRONE};
var robot_type : RobotType;

private var gun_delay = 0.0;
private var alive = true;
private var rotation_x = Spring(0.0,0.0,100.0,0.0001);
private var rotation_y = Spring(0.0,0.0,100.0,0.0001);
private var initial_turret_orientation : Quaternion;
private var initial_turret_position : Vector3;
private var gun_pivot : Transform;
enum AIState {IDLE, ALERT, ALERT_COOLDOWN, AIMING, FIRING, DEACTIVATING, DEAD};
private var ai_state : AIState = AIState.IDLE;
private var battery_alive = true;
private var motor_alive = true;
private var camera_alive = true;
private var trigger_alive = true;
private var barrel_alive = true;
private var ammo_alive = true;
private var trigger_down = false;
private var bullets = 15;
private var kAlertDelay = 0.6;
private var kAlertCooldownDelay = 2.0;
private var alert_delay = 0.0;
private var alert_cooldown_delay = 0.0;
private var kMaxRange = 10.0;
private var rotor_speed = 0.0;
private var top_rotor_rotation = 0.0;
private var bottom_rotor_rotation = 0.0;
private var initial_pos : Vector3;
private var stuck = false;
private var stuck_delay = 0.0;
private var tilt_correction : Vector3;
var target_pos : Vector3;
enum CameraPivotState {DOWN, WAIT_UP, UP, WAIT_DOWN};
var camera_pivot_state = CameraPivotState.WAIT_DOWN;
var camera_pivot_delay = 0.0;
var camera_pivot_angle = 0.0;

function PlaySoundFromGroup(group : Array, volume : float){
	if(group.length == 0){
		return;
	}
	var which_shot = Random.Range(0,group.length);
	audiosource_effect.PlayOneShot(group[which_shot], volume);
}

function GetTurretLightObject() : GameObject {
	return transform.FindChild("gun pivot").FindChild("camera").FindChild("light").gameObject;
}

function GetDroneLightObject() : GameObject {
	return transform.FindChild("camera_pivot").FindChild("camera").FindChild("light").gameObject;
}

function GetDroneLensFlareObject() : GameObject {
	return transform.FindChild("camera_pivot").FindChild("camera").FindChild("lens flare").gameObject;
}


function RandomOrientation() : Quaternion {
	return Quaternion.EulerAngles(Random.Range(0,360),Random.Range(0,360),Random.Range(0,360));
}

function Damage(obj : GameObject){
	var damage_done = false;
	if(obj.name == "battery" && battery_alive){
		battery_alive = false;
		motor_alive = false;
		camera_alive = false;
		trigger_alive = false;
		if(robot_type == RobotType.SHOCK_DRONE){
			barrel_alive = false;
		}
		PlaySoundFromGroup(sound_damage_battery,1.0);
		rotation_x.target_state = 40.0;
		damage_done = true;
	} else if((obj.name == "pivot motor" || obj.name == "motor") && motor_alive){
		motor_alive = false;
		PlaySoundFromGroup(sound_damage_motor,1.0);
		damage_done = true;
	} else if(obj.name == "power cable" && (camera_alive || trigger_alive)){
		camera_alive = false;
		damage_done = true;
		PlaySoundFromGroup(sound_damage_battery,1.0);
		trigger_alive = false;
	} else if(obj.name == "ammo box" && ammo_alive){
		ammo_alive = false;
		PlaySoundFromGroup(sound_damage_ammo,1.0);
		damage_done = true;
	} else if((obj.name == "gun" || obj.name == "shock prod") && barrel_alive){
		barrel_alive = false;
		PlaySoundFromGroup(sound_damage_gun,1.0);
		damage_done = true;
	} else if(obj.name == "camera" && camera_alive){
		camera_alive = false;
		PlaySoundFromGroup(sound_damage_camera,1.0);
		damage_done = true;
	} else if(obj.name == "camera armor" && camera_alive){
		camera_alive = false;
		PlaySoundFromGroup(sound_damage_camera,1.0);
		damage_done = true;
	}
	if(damage_done){
		Instantiate(electric_spark_obj, obj.transform.position, RandomOrientation());
	}
}

function WasShotInternal(obj : GameObject) {
	Damage(obj);
}

function WasShot(obj : GameObject, pos : Vector3, vel : Vector3) {
	if(transform.parent && transform.parent.gameObject.name == "gun pivot"){
		var x_axis = transform.FindChild("point_pivot").rotation * Vector3(1,0,0);
		var y_axis = transform.FindChild("point_pivot").rotation * Vector3(0,1,0);
		var z_axis = transform.FindChild("point_pivot").rotation * Vector3(0,0,1);
		
		var y_plane_vel = Vector3(Vector3.Dot(vel, x_axis), 0.0, Vector3.Dot(vel, z_axis));
		var rel_pos = pos - transform.FindChild("point_pivot").position;
		var y_plane_pos = Vector3(Vector3.Dot(rel_pos, z_axis), 0.0, -Vector3.Dot(rel_pos, x_axis));
		rotation_y.vel += Vector3.Dot(y_plane_vel, y_plane_pos) * 10.0;
		
		var x_plane_vel = Vector3(Vector3.Dot(vel, y_axis), 0.0, Vector3.Dot(vel, z_axis));
		rel_pos = pos - transform.FindChild("point_pivot").position;
		var x_plane_pos = Vector3(-Vector3.Dot(rel_pos, z_axis), 0.0, Vector3.Dot(rel_pos, y_axis));
		rotation_x.vel += Vector3.Dot(x_plane_vel, x_plane_pos) * 10.0;
	}
	if(robot_type == RobotType.SHOCK_DRONE){
		if(Random.Range(0.0,1.0) < 0.3){
			Damage(transform.FindChild("battery").gameObject);
		}
	}
	Damage(obj);
}

function Start () {
	audiosource_effect = gameObject.AddComponent(AudioSource);
	audiosource_effect.rolloffMode = AudioRolloffMode.Linear;
	audiosource_effect.maxDistance = 30;

	audiosource_motor = gameObject.AddComponent(AudioSource);
	audiosource_motor.loop = true;
	audiosource_motor.volume = 0.4;
	audiosource_motor.clip = sound_engine_loop;
	audiosource_motor.Play();
	
	switch(robot_type){
		case RobotType.STATIONARY_TURRET:
			gun_pivot = transform.FindChild("gun pivot");
			initial_turret_orientation = gun_pivot.transform.localRotation;
			initial_turret_position = gun_pivot.transform.localPosition;
			audiosource_motor.rolloffMode = AudioRolloffMode.Linear;
			audiosource_motor.maxDistance = 4;
			break;
		case RobotType.SHOCK_DRONE:
			audiosource_motor.maxDistance = 8;
			audiosource_foley = gameObject.AddComponent(AudioSource);
			audiosource_taser = gameObject.AddComponent(AudioSource);
			audiosource_taser.rolloffMode = AudioRolloffMode.Linear;
			audiosource_taser.loop = true;
			audiosource_taser.clip = sound_gunshot[0];
			break;
	}
	
	initial_pos = transform.position;	
	target_pos = initial_pos;
}

function UpdateStationaryTurret() {
	if(motor_alive){
		switch(ai_state){
			case AIState.IDLE:
				rotation_y.target_state += Time.deltaTime * 100.0;
				break;				
			case AIState.AIMING:
			case AIState.ALERT:
			case AIState.ALERT_COOLDOWN:
			case AIState.FIRING:
				var rel_pos = target_pos - transform.FindChild("point_pivot").position;
				var x_axis = transform.FindChild("point_pivot").rotation * Vector3(1,0,0);
				var y_axis = transform.FindChild("point_pivot").rotation * Vector3(0,1,0);
				var z_axis = transform.FindChild("point_pivot").rotation * Vector3(0,0,1);
				var y_plane_pos = Vector3(Vector3.Dot(rel_pos, z_axis), 0.0, -Vector3.Dot(rel_pos, x_axis)).normalized;
				var target_y = Mathf.Atan2(y_plane_pos.x, y_plane_pos.z)/Mathf.PI*180-90;
				while(target_y > rotation_y.state + 180){
					rotation_y.state += 360.0;
				}
				while(target_y < rotation_y.state - 180){
					rotation_y.state -= 360.0;
				}
				rotation_y.target_state = target_y;
				var y_height = Vector3.Dot(y_axis, rel_pos.normalized);
				var target_x = -Mathf.Asin(y_height)/Mathf.PI*180;
				rotation_x.target_state = target_x;
				rotation_x.target_state = Mathf.Min(40,Mathf.Max(-40,target_x));
				break;
		}
	}
	if(battery_alive){
		switch(ai_state){
			case AIState.FIRING:
				trigger_down = true;
				break;
			default:
				trigger_down = false;
				break;
		}
	}
	if(barrel_alive){
		if(trigger_down){
			if(gun_delay <= 0.0){
				gun_delay += 0.1;
				var point_muzzle_flash = gun_pivot.FindChild("gun").FindChild("point_muzzleflash");
				Instantiate(muzzle_flash, point_muzzle_flash.position, point_muzzle_flash.rotation);
				PlaySoundFromGroup(sound_gunshot, 1.0);
				
				var bullet = Instantiate(bullet_obj, point_muzzle_flash.position, point_muzzle_flash.rotation);
				bullet.GetComponent(BulletScript).SetVelocity(point_muzzle_flash.forward * 300.0);
				bullet.GetComponent(BulletScript).SetHostile();				
				rotation_x.vel += Random.Range(-50,50);
				rotation_y.vel += Random.Range(-50,50);
				--bullets;
			}
		}
		if(ammo_alive && bullets > 0){
			gun_delay = Mathf.Max(0.0, gun_delay - Time.deltaTime);
		}
	}
	if(camera_alive){
		var player = GameObject.Find("Player");
		var dist = Vector3.Distance(player.transform.position, transform.position);
		var danger = Mathf.Max(0.0, 1.0 - dist/kMaxRange);
		if(danger > 0.0){
			danger = Mathf.Min(0.2, danger);
		}
		if(ai_state == AIState.AIMING || ai_state == AIState.FIRING){
			danger = 1.0;
		}
		if(ai_state == AIState.ALERT || ai_state == AIState.ALERT_COOLDOWN){
			danger += 0.5;
		}
		player.GetComponent(MusicScript).SetDangerLevel(danger);
		
		var camera = transform.FindChild("gun pivot").FindChild("camera");
		rel_pos = player.transform.position - camera.position;
		var sees_target = false;
		if(dist < kMaxRange && Vector3.Dot(camera.rotation*Vector3(0,-1,0), rel_pos.normalized) > 0.7){
			var hit:RaycastHit;
			if(!Physics.Linecast(camera.position, player.transform.position, hit, 1<<0)){
				sees_target = true;
			}
		}
		if(sees_target){
			switch(ai_state){
				case AIState.IDLE:
					ai_state = AIState.ALERT;
					audiosource_effect.PlayOneShot(sound_alert, 0.3);
					alert_delay = kAlertDelay;
					break;
				case AIState.AIMING:
					if(Vector3.Dot(camera.rotation*Vector3(0,-1,0), rel_pos.normalized) > 0.9){
						ai_state = AIState.FIRING;
					}
					target_pos = player.transform.position;
					break;					
				case AIState.FIRING:
					target_pos = player.transform.position;
					break;
				case AIState.ALERT:
					alert_delay -= Time.deltaTime;
					if(alert_delay <= 0.0){
						ai_state = AIState.AIMING;
					}
					target_pos = player.transform.position;
					break;
				case AIState.ALERT_COOLDOWN:
					ai_state = AIState.ALERT;
					alert_delay = kAlertDelay;
					break;
			}
		} else {
			switch(ai_state){
				case AIState.AIMING:
				case AIState.FIRING:
				case AIState.ALERT:
					ai_state = AIState.ALERT_COOLDOWN;
					alert_cooldown_delay = kAlertCooldownDelay;
					break;
				case AIState.ALERT_COOLDOWN:
					alert_cooldown_delay -= Time.deltaTime;
					if(alert_cooldown_delay <= 0.0){
						ai_state = AIState.IDLE;
						audiosource_effect.PlayOneShot(sound_unalert, 0.3);
					}
					break;
			}
		}
		switch(ai_state){
			case AIState.IDLE:
				GetTurretLightObject().light.color = Color(0,0,1);
				break;
			case AIState.AIMING:
				GetTurretLightObject().light.color = Color(1,0,0);
				break;
			case AIState.ALERT:
			case AIState.ALERT_COOLDOWN:
				GetTurretLightObject().light.color = Color(1,1,0);
				break;
		}
	}
	if(!camera_alive){
		GetTurretLightObject().light.intensity *= Mathf.Pow(0.01, Time.deltaTime);
	}
	var target_pitch = (Mathf.Abs(rotation_y.vel) + Mathf.Abs(rotation_x.vel)) * 0.01;
	target_pitch = Mathf.Clamp(target_pitch, 0.2, 2.0);
	audiosource_motor.pitch = Mathf.Lerp(audiosource_motor.pitch, target_pitch, Mathf.Pow(0.0001, Time.deltaTime));
	
	rotation_x.Update();
	rotation_y.Update();
	gun_pivot.localRotation = initial_turret_orientation;
	gun_pivot.localPosition = initial_turret_position;
	gun_pivot.RotateAround(
		transform.FindChild("point_pivot").position, 
		transform.FindChild("point_pivot").rotation * Vector3(1,0,0),
		rotation_x.state);
	gun_pivot.RotateAround(
		transform.FindChild("point_pivot").position, 
		transform.FindChild("point_pivot").rotation * Vector3(0,1,0),
		rotation_y.state);
}

function UpdateDrone() {
	var rel_pos = target_pos - transform.position;
	if(motor_alive){		
		var kFlyDeadZone = 0.2;
		var kFlySpeed = 10.0;
		var target_vel = (target_pos - transform.position) / kFlyDeadZone;
		if(target_vel.magnitude > 1.0){
			target_vel = target_vel.normalized;
		}
		target_vel *= kFlySpeed;
		var target_accel = (target_vel - rigidbody.velocity);
		if(ai_state == AIState.IDLE){
			target_accel *= 0.1;
		}
		target_accel.y += 9.81;
		
		rotor_speed = target_accel.magnitude;
		rotor_speed = Mathf.Clamp(rotor_speed, 0.0, 14.0);
		
		var up = transform.rotation * Vector3(0,1,0);
		var correction : Quaternion;
		correction.SetFromToRotation(up, target_accel.normalized);
		var correction_vec : Vector3;
		var correction_angle : float;
		correction.ToAngleAxis(correction_angle, correction_vec);
		tilt_correction = correction_vec * correction_angle;
		tilt_correction -= rigidbody.angularVelocity;
		
		
		var x_axis = transform.rotation * Vector3(1,0,0);
		var y_axis = transform.rotation * Vector3(0,1,0);
		var z_axis = transform.rotation * Vector3(0,0,1);
		if(ai_state != AIState.IDLE){
			var y_plane_pos = Vector3(Vector3.Dot(rel_pos, z_axis), 0.0, -Vector3.Dot(rel_pos, x_axis)).normalized;
			var target_y = Mathf.Atan2(y_plane_pos.x, y_plane_pos.z)/Mathf.PI*180-90;
			while(target_y > 180){
				target_y -= 360.0;
			}
			while(target_y < -180){
				target_y += 360.0;
			}
			tilt_correction += y_axis * target_y;	
			tilt_correction *= 5.0;
		} else {
			tilt_correction += y_axis;	
		}
		
		if(ai_state == AIState.IDLE){
			tilt_correction *= 0.1;
		}
		
		if(rigidbody.velocity.magnitude < 0.2){ 
			stuck_delay += Time.deltaTime;
			if(stuck_delay > 1.0){
				target_pos = transform.position + Vector3(Random.Range(-1.0,1.0), Random.Range(-1.0,1.0), Random.Range(-1.0,1.0));
				stuck_delay = 0.0;
			}
		} else {
			stuck_delay = 0.0;
		}
		
	} else {
		rotor_speed = Mathf.Max(0.0, rotor_speed - Time.deltaTime * 5.0);
		rigidbody.angularDrag = 0.05;
	}
	if(barrel_alive && ai_state == AIState.FIRING){
		if(!audiosource_taser.isPlaying){
			audiosource_taser.Play();
		}
		if(gun_delay <= 0.0){
			gun_delay = 0.1;	
			Instantiate(muzzle_flash, transform.FindChild("point_spark").position, RandomOrientation());
			if(Vector3.Distance(transform.FindChild("point_spark").position, GameObject.Find("Player").transform.position) < 1){;
				GameObject.Find("Player").GetComponent(AimScript).Shock();
			}
		}
	} else {
		audiosource_taser.Stop();
	}
	gun_delay = Mathf.Max(0.0, gun_delay - Time.deltaTime);
	
	top_rotor_rotation += rotor_speed * Time.deltaTime * 1000.0;
	bottom_rotor_rotation -= rotor_speed * Time.deltaTime * 1000.0;
	if(rotor_speed * Time.timeScale > 7.0){
		transform.FindChild("bottom rotor").gameObject.renderer.enabled = false;
		transform.FindChild("top rotor").gameObject.renderer.enabled = false;
	} else {
		transform.FindChild("bottom rotor").gameObject.renderer.enabled = true;
		transform.FindChild("top rotor").gameObject.renderer.enabled = true;
	}
	transform.FindChild("bottom rotor").localEulerAngles.y = bottom_rotor_rotation;
	transform.FindChild("top rotor").localEulerAngles.y = top_rotor_rotation;
	
	//rigidbody.velocity += transform.rotation * Vector3(0,1,0) * rotor_speed * Time.deltaTime;
	if(camera_alive){
		switch(camera_pivot_state) {
			case CameraPivotState.DOWN:
				camera_pivot_angle += Time.deltaTime * 25.0;
				if(camera_pivot_angle > 50){
					camera_pivot_angle = 50;
					camera_pivot_state = CameraPivotState.WAIT_UP;
					camera_pivot_delay = 0.2;
				}
				break;
			case CameraPivotState.UP:
				camera_pivot_angle -= Time.deltaTime * 25.0;
				if(camera_pivot_angle < 0){
					camera_pivot_angle = 0;
					camera_pivot_state = CameraPivotState.WAIT_DOWN;
					camera_pivot_delay = 0.2;
				}
				break;
			case CameraPivotState.WAIT_DOWN:
				camera_pivot_delay -= Time.deltaTime;
				if(camera_pivot_delay < 0){
					camera_pivot_state = CameraPivotState.DOWN;
				}
				break;
			case CameraPivotState.WAIT_UP:
				camera_pivot_delay -= Time.deltaTime;
				if(camera_pivot_delay < 0){
					camera_pivot_state = CameraPivotState.UP;
				}
				break;
		}
		var cam_pivot = transform.FindChild("camera_pivot");
		cam_pivot.localEulerAngles.x = camera_pivot_angle;
		var player = GameObject.Find("Player");
		var dist = Vector3.Distance(player.transform.position, transform.position);
		var danger = Mathf.Max(0.0, 1.0 - dist/kMaxRange);
		if(danger > 0.0){
			danger = Mathf.Min(0.2, danger);
		}
		if(ai_state == AIState.AIMING || ai_state == AIState.FIRING){
			danger = 1.0;
		}
		if(ai_state == AIState.ALERT || ai_state == AIState.ALERT_COOLDOWN){
			danger += 0.5;
		}
		player.GetComponent(MusicScript).SetDangerLevel(danger);
		
		var camera = transform.FindChild("camera_pivot").FindChild("camera");
		rel_pos = player.transform.position - camera.position;
		var sees_target = false;
		if(dist < kMaxRange && Vector3.Dot(camera.rotation*Vector3(0,-1,0), rel_pos.normalized) > 0.7){
			var hit:RaycastHit;
			if(!Physics.Linecast(camera.position, player.transform.position, hit, 1<<0)){
				sees_target = true;
			}
		}
		if(sees_target){
			var new_target = player.transform.position + player.GetComponent(CharacterMotor).GetVelocity() * 
							Mathf.Clamp(Vector3.Distance(player.transform.position, transform.position) * 0.1, 0.5, 1.0);
			switch(ai_state){
				case AIState.IDLE:
					ai_state = AIState.ALERT;
					alert_delay = kAlertDelay;
					audiosource_effect.PlayOneShot(sound_alert, 0.3);
					break;
				case AIState.AIMING:
					target_pos = new_target;
					if(Vector3.Distance(transform.position, target_pos) < 4){
						ai_state = AIState.FIRING;
					}
					target_pos.y += 1.0;
					break;					
				case AIState.FIRING:
					target_pos = new_target;
					if(Vector3.Distance(transform.position, target_pos) > 4){
						ai_state = AIState.AIMING;
					}
					break;
				case AIState.ALERT:
					alert_delay -= Time.deltaTime;
					target_pos = new_target;
					target_pos.y += 1.0;
					if(alert_delay <= 0.0){
						ai_state = AIState.AIMING;
					}
					break;
				case AIState.ALERT_COOLDOWN:
					ai_state = AIState.ALERT;
					alert_delay = kAlertDelay;
					break;
			}
		} else {
			switch(ai_state){
				case AIState.AIMING:
				case AIState.FIRING:
				case AIState.ALERT:
					ai_state = AIState.ALERT_COOLDOWN;
					alert_cooldown_delay = kAlertCooldownDelay;
					break;
				case AIState.ALERT_COOLDOWN:
					alert_cooldown_delay -= Time.deltaTime;
					if(alert_cooldown_delay <= 0.0){
						ai_state = AIState.IDLE;
						audiosource_effect.PlayOneShot(sound_unalert, 0.3);
					}
					break;
			}
		}
		switch(ai_state){
			case AIState.IDLE:
				GetDroneLightObject().light.color = Color(0,0,1);
				break;
			case AIState.AIMING:
				GetDroneLightObject().light.color = Color(1,0,0);
				break;
			case AIState.ALERT:
			case AIState.ALERT_COOLDOWN:
				GetDroneLightObject().light.color = Color(1,1,0);
				break;
		}
	}
	if(!camera_alive){
		GetDroneLightObject().light.intensity *= Mathf.Pow(0.01, Time.deltaTime);
	}
	(GetDroneLensFlareObject().GetComponent(LensFlare) as LensFlare).color = GetDroneLightObject().light.color;
	(GetDroneLensFlareObject().GetComponent(LensFlare) as LensFlare).brightness = GetDroneLightObject().light.intensity;
	var target_pitch = rotor_speed * 0.2;
	target_pitch = Mathf.Clamp(target_pitch, 0.2, 3.0);
	audiosource_motor.pitch = Mathf.Lerp(audiosource_motor.pitch, target_pitch, Mathf.Pow(0.0001, Time.deltaTime));
	audiosource_motor.volume = rotor_speed * 0.1;
}


function Update () {
	switch(robot_type){
		case RobotType.STATIONARY_TURRET:
			UpdateStationaryTurret();
			break;
		case RobotType.SHOCK_DRONE:
			UpdateDrone();
			break;
	}
}

function OnCollisionEnter(collision : Collision) {
	if(robot_type == RobotType.SHOCK_DRONE){
		if(collision.impactForceSum.magnitude > 10){
			if(Random.Range(0.0,1.0)<0.5 && motor_alive){
				Damage(transform.FindChild("motor").gameObject);
			} else if(Random.Range(0.0,1.0)<0.5 && camera_alive){
				Damage(transform.FindChild("camera_pivot").FindChild("camera").gameObject);
			} else if(Random.Range(0.0,1.0)<0.5 && battery_alive){
				Damage(transform.FindChild("battery").gameObject);
			} else {
				motor_alive = true;
				Damage(transform.FindChild("motor").gameObject);
			} 
		} else {
			var which_shot = Random.Range(0,sound_bump.length);
			audiosource_foley.PlayOneShot(sound_bump[which_shot], collision.impactForceSum.magnitude * 0.15);
		}
	}
}

function FixedUpdate() {
	if(robot_type == RobotType.SHOCK_DRONE){
		rigidbody.AddForce(transform.rotation * Vector3(0,1,0) * rotor_speed, ForceMode.Force);
		if(motor_alive){
			rigidbody.AddTorque(tilt_correction, ForceMode.Force);
		}
	}
}