#pragma strict

var bullet_hole_obj:GameObject;
var gun_obj:GameObject;
private var gun_instance:GameObject;
private var main_camera:GameObject;
private var aiming = 0.0;

function Start () {
	gun_instance = Instantiate(gun_obj);
	main_camera = transform.FindChild("Main Camera").gameObject;
}

function Update () {
}

function FixedUpdate() {
	var aim_pos = main_camera.transform.position + main_camera.transform.forward;
	var aim_dir = main_camera.transform.forward;
	var unaimed_pos = transform.position + transform.forward;
	var unaimed_dir = (transform.forward + Vector3(0,-1,0)).normalized;
	gun_instance.transform.position = Vector3.Lerp(unaimed_pos, aim_pos, aiming);
	gun_instance.transform.forward = Vector3.Lerp(unaimed_dir, aim_dir, aiming);
	
	if(Input.GetMouseButton(1)){
		aiming = Mathf.Lerp(aiming, 1.0, 0.5);
	} else {
		aiming = Mathf.Lerp(aiming, 0.0, 0.5);
	}
	
	if(Input.GetMouseButtonDown(0)){
		var hit:RaycastHit;
		if(Physics.Raycast(gun_instance.transform.position, gun_instance.transform.forward, hit)){
			Instantiate(bullet_hole_obj, hit.point, gun_instance.transform.rotation);
		}
	}
}