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
enum AIState {IDLE, ALERT, AIMING, FIRING, DEACTIVATING, DEAD};
private var ai_state : AIState = AIState.IDLE;
private var battery_alive = true;
private var motor_alive = true;

function WasShotInternal(obj : GameObject) {
	if(obj.name == "battery"){
		battery_alive = false;
		motor_alive = false;
		rotation_x.target_state = 40.0;
	} else if(obj.name == "pivot motor"){
		motor_alive = false;
	}
}

function WasShot(obj : GameObject, pos : Vector3, vel : Vector3) {
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

function Start () {
	gun_pivot = transform.FindChild("gun pivot");
	initial_turret_orientation = gun_pivot.transform.localRotation;
	initial_turret_position = gun_pivot.transform.localPosition;
}

function Update () {
	if(motor_alive){
		rotation_y.target_state += Time.deltaTime * 100.0;
		//rotation_x = Mathf.Sin(Time.time*4)*30.0;
		
		//if(gun_delay <= 0.0){
		//	gun_delay += 0.1;
		//	var point_muzzle_flash = gun_pivot.FindChild("point_muzzleflash");
		//	Instantiate(muzzle_flash, point_muzzle_flash.position, point_muzzle_flash.rotation);
		//	var bullet = Instantiate(bullet_obj, point_muzzle_flash.position, point_muzzle_flash.rotation);
		//	bullet.GetComponent(BulletScript).SetVelocity(point_muzzle_flash.forward * 300.0);
		//}
		gun_delay -= Time.deltaTime;
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