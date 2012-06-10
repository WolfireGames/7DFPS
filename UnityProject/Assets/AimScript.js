#pragma strict

var magazine_obj:GameObject;
var bullet_hole_obj:GameObject;
var gun_obj:GameObject;
var muzzle_flash:GameObject;
var shell_casing:GameObject;
var casing_with_bullet:GameObject;
private var gun_instance:GameObject;
private var main_camera:GameObject;
private var aiming = 0.0;
private var aim_vel = 0.0;
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
private var x_recoil_offset = 0.0;
private var y_recoil_offset = 0.0;
private var x_recoil_offset_vel = 0.0;
private var y_recoil_offset_vel = 0.0;
private var kRecoilSpringStrength = 800.0;
private var kRecoilSpringDamping = 0.000001;
private var head_x_recoil = 0.0;
private var head_y_recoil = 0.0;
private var head_x_recoil_vel = 0.0;
private var head_y_recoil_vel = 0.0;
private var kHeadRecoilSpringStrength = 800.0;
private var kHeadRecoilSpringDamping = 0.000001;
private var round_in_chamber = true;
private var round_in_chamber_fired = false;
private var magazine_instance_in_gun:GameObject;
private var mag_offset = 0.0;
private var kGunDistance = 0.3;
private var kAimSpringStrength = 100.0;
private var kAimSpringDamping = 0.00001;
private var slide_rel_pos : Vector3;
private var slide_amount = 0.0;
private var slide_lock = false;
private var hammer_rel_pos : Vector3;
private var hammer_rel_rot : Quaternion;
private var hammer_cocked = 1.0;
private var target_gun_rotate : Quaternion;
private var aim_toggle = false;
private var slide_pull_pose = 0.0;
private var slide_pull_pose_vel = 0.0;
private var target_slide_pull_pose = 0.0;
private var reload_pose = 0.0;
private var reload_pose_vel = 0.0;
private var target_reload_pose = 0.0;
private var kSlideLockPosition = 0.8;
private var kSlideLockSpeed = 20.0;
private var left_hand_occupied = false;
private var kMaxHeadRecoil = 10;
private var head_recoil_delay : float[] = new float[kMaxHeadRecoil];
private var next_head_recoil_delay = 0;
enum GunTilt {LEFT, CENTER, RIGHT};
private var gun_tilt : GunTilt = GunTilt.CENTER;
enum SlideStage {NOTHING, PULLBACK, HOLD};
private var slide_stage : SlideStage = SlideStage.NOTHING;
enum MagStage {OUT, INSERTING, IN, REMOVING};
private var mag_stage : MagStage = MagStage.IN;
private var mag_seated = 1.0;

public var sensitivity_x = 2.0;
public var sensitivity_y = 2.0;
public var min_angle_y = -60.0;
public var max_angle_y = 60.0;

function Start () {
	gun_instance = Instantiate(gun_obj);
	slide_rel_pos = gun_instance.transform.FindChild("slide").localPosition;
	hammer_rel_pos = gun_instance.transform.FindChild("hammer").localPosition;
	hammer_rel_rot = gun_instance.transform.FindChild("hammer").localRotation;
	magazine_instance_in_gun = Instantiate(magazine_obj);
	main_camera = GameObject.Find("Main Camera").gameObject;
	character_controller = GetComponent(CharacterController);
	for(var i=0; i<kMaxHeadRecoil; ++i){
		head_recoil_delay[i] = -1.0;
	}
}

function AimPos() : Vector3 {
	var aim_dir = AimDir();
	return main_camera.transform.position + aim_dir*kGunDistance;
}

function AimDir() : Vector3 {
	var aim_rot = Quaternion();
	aim_rot.SetEulerAngles(-rotation_y * Mathf.PI / 180.0, rotation_x * Mathf.PI / 180.0, 0.0);
	return aim_rot * Vector3(0.0,0.0,1.0);
}

function MagScript() : mag_script {
	return magazine_instance_in_gun.GetComponent("mag_script");
}

function PullSlideBack() {
	slide_amount = 1.0;
	if(slide_lock && mag_stage == MagStage.IN && (!magazine_instance_in_gun || MagScript().NumRounds() == 0)){
		return;
	}
	slide_lock = false;
	if(round_in_chamber){
		var shell_casing_rotate = Quaternion();
		shell_casing_rotate.eulerAngles.x = 0;
		var casing:GameObject;
		if(round_in_chamber_fired){
			casing = Instantiate(shell_casing, gun_instance.transform.FindChild("point_chambered_round").position, gun_instance.transform.rotation * shell_casing_rotate);
		} else {
			casing = Instantiate(casing_with_bullet, gun_instance.transform.FindChild("point_chambered_round").position, gun_instance.transform.rotation * shell_casing_rotate);
		}
		casing.rigidbody.velocity = character_controller.velocity;
		casing.rigidbody.velocity += gun_instance.transform.rotation * Vector3(Random.Range(2.0,4.0),Random.Range(1.0,2.0),Random.Range(-1.0,-3.0));
		casing.rigidbody.angularVelocity = Vector3(Random.Range(-40.0,40.0),Random.Range(-40.0,40.0),Random.Range(-40.0,40.0));
		round_in_chamber = false;
	}
	if(magazine_instance_in_gun && MagScript().NumRounds() > 0){
		MagScript().RemoveRound();
		round_in_chamber = true;
		round_in_chamber_fired = false;
	} else if(mag_stage == MagStage.IN){
		slide_lock = true;
	}
}

function ReleaseSlideLock() {
	slide_lock = false;
	if(magazine_instance_in_gun && MagScript().NumRounds() > 0){
		MagScript().RemoveRound();
		round_in_chamber = true;
		round_in_chamber_fired = false;
	}
}

function mix( a:Vector3, b:Vector3, val:float ) : Vector3{
	return a + (b-a) * val;
}

function mix( a:Quaternion, b:Quaternion, val:float ) : Quaternion{
	var angle = 0.0;
	var axis = Vector3();
	(Quaternion.Inverse(b)*a).ToAngleAxis(angle, axis);
	return a * Quaternion.AngleAxis(angle * -val, axis);
}

function Update () {
	main_camera.transform.position = transform.position;
	
	var aim_dir = AimDir();
	var aim_pos = AimPos();	
	
	var unaimed_dir = (transform.forward + Vector3(0,-1,0)).normalized;
	var unaimed_pos = main_camera.transform.position + unaimed_dir*kGunDistance;
	gun_instance.transform.position = mix(unaimed_pos, aim_pos, aiming);
	gun_instance.transform.forward = mix(unaimed_dir, aim_dir, aiming);
	gun_instance.transform.position = mix(gun_instance.transform.position,
									      gun_instance.transform.FindChild("pose_slide_pull").position,
									      slide_pull_pose);
	gun_instance.transform.rotation = mix(
		gun_instance.transform.rotation,
		gun_instance.transform.FindChild("pose_slide_pull").rotation,
		slide_pull_pose);
	gun_instance.transform.position = mix(gun_instance.transform.position,
									      gun_instance.transform.FindChild("pose_reload").position,
									      reload_pose);
	gun_instance.transform.rotation = mix(
		gun_instance.transform.rotation,
		gun_instance.transform.FindChild("pose_reload").rotation,
		reload_pose);
	
	if(magazine_instance_in_gun){
		magazine_instance_in_gun.transform.position = gun_instance.transform.position;
		magazine_instance_in_gun.transform.rotation = gun_instance.transform.rotation;
		magazine_instance_in_gun.transform.position += (gun_instance.transform.FindChild("point_mag_to_insert").position - 
													    gun_instance.transform.FindChild("point_mag_inserted").position) * 
													   (1.0 - mag_seated);
		
		//magazine_instance_in_gun.transform.position += gun_instance.transform.rotation * Vector3(0.0,mag_offset,0.0);
		//mag_offset = Mathf.Min(0.0, mag_offset + Time.deltaTime * 5.0);
	}
	
	//gun_instance.transform.rotation.eulerAngles.x += recoil * -30.0;
	//gun_instance.transform.position.y -= recoil * 0.2;
	gun_instance.transform.RotateAround(
		gun_instance.transform.FindChild("point_recoil_rotate").position,
		gun_instance.transform.rotation * Vector3(1,0,0),
		x_recoil_offset);
		
	gun_instance.transform.RotateAround(
		gun_instance.transform.FindChild("point_recoil_rotate").position,
		Vector3(0,1,0),
		y_recoil_offset);
	
	if(Input.GetMouseButton(1) || aim_toggle){
		aim_vel += (1.0 - aiming) * kAimSpringStrength * Time.deltaTime;
	} else {
		aim_vel += (0.0 - aiming) * kAimSpringStrength * Time.deltaTime;
	}
	aim_vel *= Mathf.Pow(kAimSpringDamping, Time.deltaTime);
	aiming += aim_vel * Time.deltaTime;	
	
	rotation_y_min_leeway = Mathf.Lerp(0.0,kRotationYMinLeeway,aiming);
	rotation_y_max_leeway = Mathf.Lerp(0.0,kRotationYMaxLeeway,aiming);
	rotation_x_leeway = Mathf.Lerp(0.0,kRotationXLeeway,aiming);
	
	rotation_x += Input.GetAxis("Mouse X") * sensitivity_x;
	rotation_y += Input.GetAxis("Mouse Y") * sensitivity_y;
	rotation_y = Mathf.Clamp (rotation_y, min_angle_y, max_angle_y);
		
	if(Input.GetMouseButton(1)){
		view_rotation_y = Mathf.Clamp(view_rotation_y, rotation_y - rotation_y_min_leeway, rotation_y + rotation_y_max_leeway);
		view_rotation_x = Mathf.Clamp(view_rotation_x, rotation_x - rotation_x_leeway, rotation_x + rotation_x_leeway);
	} else {
		view_rotation_x += Input.GetAxis("Mouse X") * sensitivity_x;
		view_rotation_y += Input.GetAxis("Mouse Y") * sensitivity_y;
		view_rotation_y = Mathf.Clamp (view_rotation_y, min_angle_y, max_angle_y);
		
		rotation_y = Mathf.Clamp(rotation_y, view_rotation_y - rotation_y_max_leeway, view_rotation_y + rotation_y_min_leeway);
		rotation_x = Mathf.Clamp(rotation_x, view_rotation_x - rotation_x_leeway, view_rotation_x + rotation_x_leeway);
	}
	main_camera.transform.localEulerAngles = Vector3(-view_rotation_y, view_rotation_x, 0);
	main_camera.transform.localEulerAngles += Vector3(head_y_recoil, head_x_recoil, 0);
	character_controller.transform.localEulerAngles.y = view_rotation_x;
	
	if(Input.GetMouseButtonDown(0) && !slide_lock){
		hammer_cocked = slide_amount;
		if(round_in_chamber && slide_amount == 0.0){
			round_in_chamber_fired = true;
			Instantiate(muzzle_flash, gun_instance.transform.FindChild("point_muzzle").position, gun_instance.transform.rotation);
			var hit:RaycastHit;
			var mask = 1<<8;
			mask = ~mask;
			if(Physics.Raycast(gun_instance.transform.position, gun_instance.transform.forward, hit, Mathf.Infinity, mask)){
				Instantiate(bullet_hole_obj, hit.point, gun_instance.transform.rotation);
				Instantiate(muzzle_flash, hit.point + hit.normal * 0.5, gun_instance.transform.rotation);
			}
			rotation_y += Random.Range(1.0,2.0);
			rotation_x += Random.Range(-1.0,1.0);
			x_recoil_offset_vel -= Random.Range(150.0,300.0);
			y_recoil_offset_vel += Random.Range(-200.0,200.0);
			head_recoil_delay[next_head_recoil_delay] = 0.1;
			next_head_recoil_delay = (next_head_recoil_delay + 1)%kMaxHeadRecoil;
			PullSlideBack();
		}
	}
	
	for(var i = 0; i < kMaxHeadRecoil; ++i){
		if(head_recoil_delay[i] != -1.0){
			head_recoil_delay[i] -= Time.deltaTime;
			if(head_recoil_delay[i] <= 0.0){
				head_x_recoil_vel += Random.Range(-30.0,30.0);
				head_y_recoil_vel += Random.Range(-30.0,30.0);
				head_recoil_delay[i] = -1.0;
			}
		}
	}
	
	if(Input.GetKeyDown('m')){
		if(mag_stage == MagStage.OUT && !left_hand_occupied){
			left_hand_occupied = true;
			mag_stage = MagStage.INSERTING;
			magazine_instance_in_gun = Instantiate(magazine_obj);
			mag_offset = -2.0;
			mag_seated = 0.0;
		}
	}
	if(Input.GetKeyDown('e')){
		if(mag_stage == MagStage.IN){
			mag_stage = MagStage.REMOVING;
		}
	}
	
	if(mag_stage == MagStage.INSERTING){
		mag_seated += Time.deltaTime * 5.0;
		if(mag_seated >= 1.0){
			mag_seated = 1.0;
			mag_stage = MagStage.IN;
			left_hand_occupied = false;
		}
	}
	if(mag_stage == MagStage.REMOVING){
		mag_seated -= Time.deltaTime * 5.0;
		if(mag_seated <= 0.0){
			mag_seated = 0.0;
			mag_stage = MagStage.OUT;
			magazine_instance_in_gun.AddComponent(Rigidbody);
			magazine_instance_in_gun.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
			magazine_instance_in_gun.rigidbody.velocity = character_controller.velocity;
			magazine_instance_in_gun = null;
		}
	}
	
	if(Input.GetKeyDown('t')){
		if(slide_amount == kSlideLockPosition){
			ReleaseSlideLock();
		}
	}
	if(Input.GetKey('t')){
		if(slide_amount > kSlideLockPosition){
			slide_lock = true;
		}
	}
	if(Input.GetKeyDown('r')){
		if(left_hand_occupied == false && slide_stage == SlideStage.NOTHING){
			left_hand_occupied = true;
			slide_stage = SlideStage.PULLBACK;
		}
	}
	if(Input.GetKeyDown('q')){
		aim_toggle = !aim_toggle;
	}
	if(Input.GetKeyDown('tab')){
		if(Time.timeScale == 1.0){
			Time.timeScale = 0.1;
		} else {
			Time.timeScale = 1.0;
		}
	}
	
	if(slide_pull_pose < 0.1 && target_reload_pose < 0.1){
		gun_tilt = GunTilt.CENTER;
	} else if(slide_pull_pose > target_reload_pose){
		gun_tilt = GunTilt.LEFT;
	} else {
		gun_tilt = GunTilt.RIGHT;
	}
	
	target_slide_pull_pose = 0.0;
	target_reload_pose = 0.0;
	if(slide_lock){
		if(gun_tilt != GunTilt.LEFT){
			target_reload_pose = 0.7;
		} else {
			target_slide_pull_pose = 0.7;
		}
	}
	if(slide_stage == SlideStage.PULLBACK || slide_stage == SlideStage.HOLD){
		if(gun_tilt != GunTilt.RIGHT){
			target_slide_pull_pose = 1.0;
		} else {
			target_reload_pose = 1.0;
		}
		if(slide_stage == SlideStage.PULLBACK){
			slide_amount += Time.deltaTime * 10.0;
			if(slide_amount >= 1.0){
				PullSlideBack();
				slide_stage = SlideStage.HOLD;
			}
		} else {
			slide_amount = 1.0;
		}
		if(!Input.GetKey('r')){
			slide_stage = SlideStage.NOTHING;
			left_hand_occupied = false;
		}
	}
	
	
	slide_pull_pose_vel += (target_slide_pull_pose - slide_pull_pose) * kAimSpringStrength * Time.deltaTime;
	slide_pull_pose_vel *= Mathf.Pow(kAimSpringDamping, Time.deltaTime);
	slide_pull_pose += slide_pull_pose_vel * Time.deltaTime;	
	
	reload_pose_vel += (target_reload_pose - reload_pose) * kAimSpringStrength * Time.deltaTime;
	reload_pose_vel *= Mathf.Pow(kAimSpringDamping, Time.deltaTime);
	reload_pose += reload_pose_vel * Time.deltaTime;	
	
	x_recoil_offset_vel -= x_recoil_offset * kRecoilSpringStrength * Time.deltaTime;
	x_recoil_offset_vel *= Mathf.Pow(kRecoilSpringDamping, Time.deltaTime);
	x_recoil_offset += x_recoil_offset_vel * Time.deltaTime;	
	
	y_recoil_offset_vel -= y_recoil_offset * kRecoilSpringStrength * Time.deltaTime;
	y_recoil_offset_vel *= Mathf.Pow(kRecoilSpringDamping, Time.deltaTime);
	y_recoil_offset += y_recoil_offset_vel * Time.deltaTime;	
	
	head_x_recoil_vel -= head_x_recoil * kRecoilSpringStrength * Time.deltaTime;
	head_x_recoil_vel *= Mathf.Pow(kRecoilSpringDamping, Time.deltaTime);
	head_x_recoil += head_x_recoil_vel * Time.deltaTime;	
	
	head_y_recoil_vel -= head_y_recoil * kRecoilSpringStrength * Time.deltaTime;
	head_y_recoil_vel *= Mathf.Pow(kRecoilSpringDamping, Time.deltaTime);
	head_y_recoil += head_y_recoil_vel * Time.deltaTime;	
	
	gun_instance.transform.FindChild("slide").localPosition = 
		slide_rel_pos + 
		(gun_instance.transform.FindChild("point_slide_end").localPosition - 
		 gun_instance.transform.FindChild("point_slide_start").localPosition) * slide_amount;
	
	
	gun_instance.transform.FindChild("hammer").localPosition = 
		Vector3.Lerp(hammer_rel_pos, gun_instance.transform.FindChild("point_hammer_cocked").localPosition, hammer_cocked);
	gun_instance.transform.FindChild("hammer").localRotation = 
		Quaternion.Slerp(hammer_rel_rot, gun_instance.transform.FindChild("point_hammer_cocked").localRotation, hammer_cocked);
		
	hammer_cocked = Mathf.Max(hammer_cocked, slide_amount);

	if(slide_stage == SlideStage.NOTHING){
		slide_amount = Mathf.Max(0.0, slide_amount - Time.deltaTime * kSlideLockSpeed);
	}
	if(slide_lock){
		slide_amount = Mathf.Max(kSlideLockPosition, slide_amount);
	}
}

function FixedUpdate() {
}