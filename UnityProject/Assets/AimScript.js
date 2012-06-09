#pragma strict

var bullet_hole_obj:GameObject;
var gun_obj:GameObject;
var muzzle_flash:GameObject;
var shell_casing:GameObject;
private var gun_instance:GameObject;
private var main_camera:GameObject;
private var aiming = 0.0;
private var rotation_x_leeway = 0.0;
private var rotation_y_min_leeway = 0.0;
private var rotation_y_max_leeway = 0.0;
private var kRotationXLeeway = 5.0;
private var kRotationYMinLeeway = 20.0;
private var kRotationYMaxLeeway = 10.0;
private var rotation_x = 0.0;
private var rotation_y = 0.0;
private var view_rotation_x = 0.0;
private var view_rotation_y = 0.0;
private var character_controller:CharacterController;
private var shot = false;
private var recoil = 1.0;
private var kMaxAmmoInMag = 8;
private var mag_ammo = kMaxAmmoInMag;
private var round_in_chamber = true;

public var sensitivity_x = 2.0;
public var sensitivity_y = 2.0;
public var min_angle_y = -60.0;
public var max_angle_y = 60.0;

function Start () {
	gun_instance = Instantiate(gun_obj);
	main_camera = transform.FindChild("Main Camera").gameObject;
	character_controller = GetComponent(CharacterController);
}

function AimPos() : Vector3 {
	var aim_dir = AimDir();
	return main_camera.transform.position + aim_dir;
}

function AimDir() : Vector3 {
	var aim_rot = Quaternion();
	aim_rot.SetEulerAngles(-rotation_y * Mathf.PI / 180.0, rotation_x * Mathf.PI / 180.0, 0.0);
	return aim_rot * Vector3(0.0,0.0,1.0);
}

function PullSlideBack() {
	if(round_in_chamber){
		var shell_casing_rotate = Quaternion();
		shell_casing_rotate.eulerAngles.x = -90;
		var casing:GameObject = Instantiate(shell_casing, gun_instance.transform.position + gun_instance.transform.forward * 0.2, gun_instance.transform.rotation * shell_casing_rotate);
		casing.rigidbody.velocity = gun_instance.transform.rotation * Vector3(Random.Range(2.0,4.0),Random.Range(1.0,2.0),Random.Range(-1.0,-2.0));
		casing.rigidbody.angularVelocity = Vector3(Random.Range(-40.0,40.0),Random.Range(-40.0,40.0),Random.Range(-40.0,40.0));
		round_in_chamber = false;
	}
	if(mag_ammo > 0){
		--mag_ammo;
		round_in_chamber = true;
	}
}

function Update () {
	var aim_dir = AimDir();
	var aim_pos = AimPos();
	
	if(Input.GetMouseButtonDown(0)){
		if(round_in_chamber){
			Instantiate(muzzle_flash, aim_pos + aim_dir * 0.2, gun_instance.transform.rotation);
			var hit:RaycastHit;
			var mask = 1<<8;
			mask = ~mask;
			if(Physics.Raycast(gun_instance.transform.position, gun_instance.transform.forward, hit, Mathf.Infinity, mask)){
				Instantiate(bullet_hole_obj, hit.point, gun_instance.transform.rotation);
				Instantiate(muzzle_flash, hit.point + hit.normal * 0.5, gun_instance.transform.rotation);
			}
			rotation_y += Random.Range(1.0,2.0);
			rotation_x += Random.Range(-1.0,1.0);
			recoil = Random.Range(0.8,1.2);
			PullSlideBack();
		}
	}
	
	if(Input.GetKeyDown('m')){
		mag_ammo = kMaxAmmoInMag;
	}
	if(Input.GetKeyDown('r')){
		PullSlideBack();
	}
}

function FixedUpdate() {
	var aim_dir = AimDir();
	var aim_pos = main_camera.transform.position + aim_dir;
	
	var unaimed_pos = transform.position + transform.forward;
	var unaimed_dir = (transform.forward + Vector3(0,-1,0)).normalized;
	gun_instance.transform.position = Vector3.Lerp(unaimed_pos, aim_pos, aiming);
	gun_instance.transform.forward = Vector3.Lerp(unaimed_dir, aim_dir, aiming);
	
	recoil = Mathf.Max(0.0, recoil - Time.deltaTime * 30.0);
	gun_instance.transform.rotation.eulerAngles.x += recoil * -30.0;
	gun_instance.transform.position.y -= recoil * 0.2;
	
	if(Input.GetMouseButton(1)){
		aiming = Mathf.Lerp(aiming, 1.0, 0.5);
	} else {
		aiming = Mathf.Lerp(aiming, 0.0, 0.5);
	}
	
	rotation_x += Input.GetAxis("Mouse X") * sensitivity_x;
	
	rotation_y += Input.GetAxis("Mouse Y") * sensitivity_y;
	rotation_y = Mathf.Clamp (rotation_y, min_angle_y, max_angle_y);
	
	rotation_y_min_leeway = Mathf.Lerp(0.0,kRotationYMinLeeway,aiming);
	rotation_y_max_leeway = Mathf.Lerp(0.0,kRotationYMaxLeeway,aiming);
	rotation_x_leeway = Mathf.Lerp(0.0,kRotationXLeeway,aiming);
	
	view_rotation_y = Mathf.Clamp(view_rotation_y, rotation_y - rotation_y_min_leeway, rotation_y + rotation_y_max_leeway);
	view_rotation_x = Mathf.Clamp(view_rotation_x, rotation_x - rotation_x_leeway, rotation_x + rotation_x_leeway);
	
	main_camera.transform.localEulerAngles = new Vector3(-view_rotation_y, 0, 0);
	character_controller.transform.localEulerAngles.y = view_rotation_x;
}