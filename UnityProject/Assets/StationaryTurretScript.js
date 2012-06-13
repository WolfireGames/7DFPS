#pragma strict

var muzzle_flash : GameObject;
var bullet_obj : GameObject;
private var gun_delay = 0.0;
private var alive = true;
private var rotation_x = Spring(0,0,100,0.0001);
private var rotation_y = Spring(0,0,100,0.0001);
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
var target_pos : Vector3;

function GetLightObject() : GameObject {
	return transform.FindChild("gun pivot").FindChild("camera").FindChild("light").gameObject;
}

function Damage(obj : GameObject){
	if(obj.name == "battery"){
		battery_alive = false;
		motor_alive = false;
		camera_alive = false;
		trigger_alive = false;
		rotation_x.target_state = 40.0;
	} else if(obj.name == "pivot motor"){
		motor_alive = false;
	} else if(obj.name == "power cable"){
		camera_alive = false;
		trigger_alive = false;
	} else if(obj.name == "ammo box"){
		ammo_alive = false;
	} else if(obj.name == "gun"){
		barrel_alive = false;
	} else if(obj.name == "camera"){
		camera_alive = false;
	} else if(obj.name == "camera armor"){
		camera_alive = false;
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
	Damage(obj);
}

function Start () {
	gun_pivot = transform.FindChild("gun pivot");
	initial_turret_orientation = gun_pivot.transform.localRotation;
	initial_turret_position = gun_pivot.transform.localPosition;
}

function Update () {
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
				var bullet = Instantiate(bullet_obj, point_muzzle_flash.position, point_muzzle_flash.rotation);
				bullet.GetComponent(BulletScript).SetVelocity(point_muzzle_flash.forward * 300.0);
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
					}
					break;
			}
		}
		switch(ai_state){
			case AIState.IDLE:
				GetLightObject().light.color = Color(0,0,1);
				break;
			case AIState.AIMING:
				GetLightObject().light.color = Color(1,0,0);
				break;
			case AIState.ALERT:
			case AIState.ALERT_COOLDOWN:
				GetLightObject().light.color = Color(1,1,0);
				break;
		}
	}
	if(!camera_alive){
		GetLightObject().light.intensity *= Mathf.Pow(0.01, Time.deltaTime);
	}
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