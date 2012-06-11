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
private var round_in_chamber:GameObject;
enum RoundState {EMPTY, READY, FIRED, LOADING, JAMMED};
private var round_in_chamber_state = RoundState.READY;
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
private var safety_rel_pos : Vector3;
private var safety_rel_rot : Quaternion;
private var safety_off = 1.0;
enum Safety{OFF, ON};
private var safety = Safety.OFF;
enum Holster{NOT_HOLSTERED, HOLSTERED};
private var holstered = Holster.NOT_HOLSTERED;
private var holstered_amount = 0.0;
private var holstered_amount_vel = 0.0;
private var hammer_cocked = 1.0;
private var target_gun_rotate : Quaternion;
private var aim_toggle = false;
private var slide_pull_pose = 0.0;
private var slide_pull_pose_vel = 0.0;
private var target_slide_pull_pose = 0.0;
private var reload_pose = 0.0;
private var reload_pose_vel = 0.0;
private var target_reload_pose = 0.0;
private var hold_pose = 0.0;
private var hold_pose_vel = 0.0;
private var target_hold_pose = 0.0;
private var mag_ground_pose = 0.0;
private var mag_ground_pose_vel = 0.0;
private var kSlideLockPosition = 0.8;
private var kSlideLockSpeed = 20.0;
private var left_hand_occupied = false;
private var kMaxHeadRecoil = 10;
private var head_recoil_delay : float[] = new float[kMaxHeadRecoil];
private var next_head_recoil_delay = 0;
private var mag_ground_pos : Vector3;
private var mag_ground_rot : Quaternion;
enum Thumb{ON_HAMMER, OFF_HAMMER, SLOW_LOWERING};
private var thumb_on_hammer = Thumb.OFF_HAMMER;
enum GunTilt {LEFT, CENTER, RIGHT};
private var gun_tilt : GunTilt = GunTilt.CENTER;
enum SlideStage {NOTHING, PULLBACK, HOLD};
private var slide_stage : SlideStage = SlideStage.NOTHING;
enum MagStage {HOLD, HOLD_TO_INSERT, OUT, INSERTING, IN, REMOVING};
private var mag_stage : MagStage = MagStage.IN;
private var mag_seated = 1.0;

public var sensitivity_x = 2.0;
public var sensitivity_y = 2.0;
public var min_angle_y = -60.0;
public var max_angle_y = 60.0;

private var active_weapon_slot = 0;
private var target_weapon_slot = 0;
private var num_loose_bullets = 30;

enum WeaponSlotType {GUN, MAGAZINE, EMPTY};

class WeaponSlot {
	var obj:GameObject = null;
	var type:WeaponSlotType = WeaponSlotType.EMPTY;
	var in_use = false;
};

private var weapon_slots : WeaponSlot[] = new WeaponSlot[10];

function Start () {
	gun_instance = Instantiate(gun_obj);
	slide_rel_pos = gun_instance.transform.FindChild("slide").localPosition;
	hammer_rel_pos = gun_instance.transform.FindChild("hammer").localPosition;
	hammer_rel_rot = gun_instance.transform.FindChild("hammer").localRotation;
	safety_rel_pos = gun_instance.transform.FindChild("safety").localPosition;
	safety_rel_rot = gun_instance.transform.FindChild("safety").localRotation;
	magazine_instance_in_gun = Instantiate(magazine_obj);
	magazine_instance_in_gun.transform.parent = gun_instance.transform;
	round_in_chamber = Instantiate(casing_with_bullet, gun_instance.transform.FindChild("point_chambered_round").position, gun_instance.transform.FindChild("point_chambered_round").rotation);
	round_in_chamber.transform.parent = gun_instance.transform;
	main_camera = GameObject.Find("Main Camera").gameObject;
	character_controller = GetComponent(CharacterController);
	for(var i=0; i<kMaxHeadRecoil; ++i){
		head_recoil_delay[i] = -1.0;
	}
	for(i=0; i<10; ++i){
		weapon_slots[i] = new WeaponSlot();
	}
	weapon_slots[0].type = WeaponSlotType.GUN;
	weapon_slots[0].obj = gun_instance;
	weapon_slots[0].in_use = true;
	weapon_slots[1].type = WeaponSlotType.MAGAZINE;
	weapon_slots[1].obj = Instantiate(magazine_obj);
	weapon_slots[1].in_use = false;
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

function ChamberRoundFromMag() : boolean {
	if(magazine_instance_in_gun && MagScript().NumRounds() > 0 && mag_stage == MagStage.IN){
		if(!round_in_chamber){
			MagScript().RemoveRound();
			round_in_chamber = Instantiate(casing_with_bullet, gun_instance.transform.FindChild("point_load_round").position, gun_instance.transform.FindChild("point_load_round").rotation);
			round_in_chamber.transform.parent = gun_instance.transform;
			round_in_chamber_state = RoundState.LOADING;
		}
		return true;
	} else {
		return false;
	}
}

function PullSlideBack() {
	slide_amount = 1.0;
	if(slide_lock && mag_stage == MagStage.IN && (!magazine_instance_in_gun || MagScript().NumRounds() == 0)){
		return;
	}
	slide_lock = false;
	if(round_in_chamber && (round_in_chamber_state == RoundState.FIRED || round_in_chamber_state == RoundState.READY)){
		round_in_chamber.AddComponent(Rigidbody);
		round_in_chamber.transform.parent = null;
		round_in_chamber.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		round_in_chamber.rigidbody.velocity = character_controller.velocity;
		round_in_chamber.rigidbody.velocity += gun_instance.transform.rotation * Vector3(Random.Range(2.0,4.0),Random.Range(1.0,2.0),Random.Range(-1.0,-3.0));
		round_in_chamber.rigidbody.angularVelocity = Vector3(Random.Range(-40.0,40.0),Random.Range(-40.0,40.0),Random.Range(-40.0,40.0));
		round_in_chamber = null;
	}
	if(!ChamberRoundFromMag() && mag_stage == MagStage.IN){
		slide_lock = true;
	}
}

function ReleaseSlideLock() {
	slide_lock = false;
}

function mix( a:Vector3, b:Vector3, val:float ) : Vector3{
	return a + (b-a) * val;
}

function mix( a:Quaternion, b:Quaternion, val:float ) : Quaternion{
	var angle = 0.0;
	var axis = Vector3();
	(Quaternion.Inverse(b)*a).ToAngleAxis(angle, axis);
	if(angle > 180){
		angle -= 360;
	}
	if(angle < -180){
		angle += 360;
	}
	return a * Quaternion.AngleAxis(angle * -val, axis);
}

function HolsterGun() : boolean {
	if(!left_hand_occupied){
		holstered = Holster.HOLSTERED;
		return true;
	} else {
		return false;
	}
}

function Update () {
	main_camera.transform.position = transform.position;
	
	var aim_dir = AimDir();
	var aim_pos = AimPos();	
	
	var unaimed_dir = (transform.forward + Vector3(0,-1,0)).normalized;
	var unaimed_pos = main_camera.transform.position + unaimed_dir*kGunDistance;
	
	var holstered_dir = (transform.forward*0.1 + Vector3(0,-1,0)).normalized;
	var holstered_pos = transform.position + transform.rotation * Vector3(0.5,-character_controller.height * 0.3,0.0);
		
	gun_instance.transform.position = mix(unaimed_pos, aim_pos, aiming);
	gun_instance.transform.forward = mix(unaimed_dir, aim_dir, aiming);
	
	gun_instance.transform.position = mix(gun_instance.transform.position, holstered_pos, holstered_amount);
	gun_instance.transform.forward = mix(gun_instance.transform.forward, holstered_dir, holstered_amount);
	
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
		
	gun_instance.transform.RotateAround(
		gun_instance.transform.FindChild("point_recoil_rotate").position,
		gun_instance.transform.rotation * Vector3(1,0,0),
		x_recoil_offset);
		
	gun_instance.transform.RotateAround(
		gun_instance.transform.FindChild("point_recoil_rotate").position,
		Vector3(0,1,0),
		y_recoil_offset);
	
	if(magazine_instance_in_gun){
		var mag_pos = gun_instance.transform.position;
		var mag_rot = gun_instance.transform.rotation;
		mag_pos += (gun_instance.transform.FindChild("point_mag_to_insert").position - 
				    gun_instance.transform.FindChild("point_mag_inserted").position) * 
				   (1.0 - mag_seated);
	   if(mag_stage == MagStage.HOLD || mag_stage == MagStage.HOLD_TO_INSERT){
			var hold_pos = main_camera.transform.position + main_camera.transform.rotation*Vector3(-0.15,0.05,0.2);
			var hold_rot = main_camera.transform.rotation * Quaternion.AngleAxis(45, Vector3(0,1,0)) * Quaternion.AngleAxis(-55, Vector3(1,0,0));
	   		hold_pos = mix(hold_pos, mag_ground_pos, mag_ground_pose);
	   		hold_rot = mix(hold_rot, mag_ground_rot, mag_ground_pose);
	   		magazine_instance_in_gun.transform.position = mix(mag_pos, hold_pos, hold_pose);
			magazine_instance_in_gun.transform.rotation = mix(mag_rot, hold_rot, hold_pose);
		} else {
			magazine_instance_in_gun.transform.position = mag_pos;
			magazine_instance_in_gun.transform.rotation = mag_rot;
		}
	}

	if(round_in_chamber){
		switch(round_in_chamber_state){
			case RoundState.READY:
			case RoundState.FIRED:
				round_in_chamber.transform.position = gun_instance.transform.FindChild("point_chambered_round").position;
				round_in_chamber.transform.rotation = gun_instance.transform.FindChild("point_chambered_round").rotation;
				break;
			case RoundState.LOADING:
				round_in_chamber.transform.position = gun_instance.transform.FindChild("point_load_round").position;
				round_in_chamber.transform.rotation = gun_instance.transform.FindChild("point_load_round").rotation;
				break;
		}
	}
	
	if(Input.GetMouseButton(1) || aim_toggle){
		aim_vel += (1.0 - aiming) * kAimSpringStrength * Time.deltaTime;
	} else {
		aim_vel += (0.0 - aiming) * kAimSpringStrength * Time.deltaTime;
	}
	aim_vel *= Mathf.Pow(kAimSpringDamping, Time.deltaTime);
	aiming += aim_vel * Time.deltaTime;	
	
	if(Input.GetKeyDown('g')){
		if(mag_stage == MagStage.OUT){
			var colliders = Physics.OverlapSphere(main_camera.transform.position, 2.0, 1 << 8);
			for(var collider in colliders){
				if(collider.gameObject.name == "colt_1911_magazine_object_filled(Clone)"){
					if(collider.gameObject.rigidbody){
						magazine_instance_in_gun = collider.gameObject;
						Destroy(magazine_instance_in_gun.rigidbody);
						hold_pose = 0.0;
						hold_pose_vel = 0.0;
						target_hold_pose = 1.0;
						mag_ground_pos = magazine_instance_in_gun.transform.position;
						mag_ground_rot = magazine_instance_in_gun.transform.rotation;
						mag_ground_pose = 1.0;
						mag_ground_pose_vel = 1.0;
						hold_pose = 1.0;
						hold_pose_vel = 0.0;
						target_hold_pose = 1.0;
						mag_stage = MagStage.HOLD;
						break;
					}
				}
			}
		}
	}
	if(Input.GetKeyDown('`') && active_weapon_slot != -1){
		target_weapon_slot = -1;
	}
	if(Input.GetKeyDown('1')){
		target_weapon_slot = 0;
	}
	if(Input.GetKeyDown('2')){
		target_weapon_slot = 1;
	}
	if(Input.GetKeyDown('3')){
		target_weapon_slot = 2;
	}
	if(Input.GetKeyDown('4')){
		target_weapon_slot = 3;
	}
	if(Input.GetKeyDown('5')){
		target_weapon_slot = 4;
	}
	if(Input.GetKeyDown('6')){
		target_weapon_slot = 5;
	}
	if(Input.GetKeyDown('7')){
		target_weapon_slot = 6;
	}
	if(Input.GetKeyDown('8')){
		target_weapon_slot = 7;
	}
	if(Input.GetKeyDown('9')){
		target_weapon_slot = 8;
	}
	if(Input.GetKeyDown('0')){
		target_weapon_slot = 9;
	}
	if(active_weapon_slot != target_weapon_slot){
		if(mag_stage == MagStage.HOLD && target_weapon_slot != -1 && weapon_slots[target_weapon_slot].type == WeaponSlotType.EMPTY){
			weapon_slots[target_weapon_slot].type = WeaponSlotType.MAGAZINE;
			weapon_slots[target_weapon_slot].in_use = false;
			weapon_slots[target_weapon_slot].obj = magazine_instance_in_gun;
			magazine_instance_in_gun = null;
			mag_stage = MagStage.OUT;
			target_weapon_slot = active_weapon_slot;
		} else if(mag_stage == MagStage.HOLD && target_weapon_slot != -1 && weapon_slots[target_weapon_slot].type == WeaponSlotType.MAGAZINE){
			var temp = weapon_slots[target_weapon_slot].obj;
			weapon_slots[target_weapon_slot].obj = magazine_instance_in_gun;
			magazine_instance_in_gun = temp;
			magazine_instance_in_gun.transform.localScale = Vector3(1.0,1.0,1.0);
			target_weapon_slot = active_weapon_slot;
		} else if(target_weapon_slot != -1 && mag_stage == MagStage.OUT && weapon_slots[target_weapon_slot].type == WeaponSlotType.MAGAZINE){
			magazine_instance_in_gun = weapon_slots[target_weapon_slot].obj;
			magazine_instance_in_gun.transform.localScale = Vector3(1.0,1.0,1.0);
			mag_stage = MagStage.HOLD;
			weapon_slots[target_weapon_slot].type = WeaponSlotType.EMPTY;
			weapon_slots[target_weapon_slot].obj = null;
			weapon_slots[target_weapon_slot].in_use = false;
			target_weapon_slot = active_weapon_slot;
		} else if(active_weapon_slot == 0){
			HolsterGun();
		} else if(active_weapon_slot == -1){
			if(weapon_slots[target_weapon_slot].type == WeaponSlotType.EMPTY){
				target_weapon_slot = -1;
			} else {
				if(target_weapon_slot == 0){
					holstered = Holster.NOT_HOLSTERED;
					weapon_slots[target_weapon_slot].in_use = true;
					active_weapon_slot = target_weapon_slot;
				} else if(weapon_slots[target_weapon_slot].type == WeaponSlotType.MAGAZINE){
					magazine_instance_in_gun = weapon_slots[target_weapon_slot].obj;
					magazine_instance_in_gun.transform.localScale = Vector3(1.0,1.0,1.0);
					mag_stage = MagStage.HOLD;
					weapon_slots[target_weapon_slot].type = WeaponSlotType.EMPTY;
					weapon_slots[target_weapon_slot].obj = null;
					weapon_slots[target_weapon_slot].in_use = false;
					target_weapon_slot = active_weapon_slot;
				}
			}
		}
	}
	
	if(holstered == Holster.HOLSTERED){
		holstered_amount_vel += (1.0 - holstered_amount) * kAimSpringStrength * Time.deltaTime;
		if(holstered_amount >= 1.0){
			weapon_slots[0].in_use = false;
			active_weapon_slot = -1;
			holstered_amount = 1.0;
			holstered_amount_vel = 1.0;
		}
	} else {
		holstered_amount_vel += (0.0 - holstered_amount) * kAimSpringStrength * Time.deltaTime;
	}
	
	holstered_amount_vel *= Mathf.Pow(kAimSpringDamping, Time.deltaTime);
	holstered_amount += holstered_amount_vel * Time.deltaTime;	
	
	rotation_y_min_leeway = Mathf.Lerp(0.0,kRotationYMinLeeway,aiming);
	rotation_y_max_leeway = Mathf.Lerp(0.0,kRotationYMaxLeeway,aiming);
	rotation_x_leeway = Mathf.Lerp(0.0,kRotationXLeeway,aiming);
	
	rotation_x += Input.GetAxis("Mouse X") * sensitivity_x;
	rotation_y += Input.GetAxis("Mouse Y") * sensitivity_y;
	rotation_y = Mathf.Clamp (rotation_y, min_angle_y, max_angle_y);
		
	if((Input.GetMouseButton(1) || aim_toggle) && holstered == Holster.NOT_HOLSTERED){
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
	
	for(var i=0; i<10; ++i){
		if(weapon_slots[i].type == WeaponSlotType.EMPTY || weapon_slots[i].in_use){
			if(weapon_slots[i].in_use){
				weapon_slots[i].obj.transform.localScale = Vector3(1.0,1.0,1.0);
				var renderers = weapon_slots[i].obj.GetComponentsInChildren(Renderer);
				for(var renderer : Renderer in renderers){
					renderer.castShadows = true; 
				}
			}
			continue;
		}
		weapon_slots[i].obj.transform.position = main_camera.transform.position + main_camera.camera.ScreenPointToRay(Vector3(main_camera.camera.pixelWidth * (0.05 + i*0.15), main_camera.camera.pixelHeight * 0.17,0)).direction * 0.3;
		weapon_slots[i].obj.transform.localScale = Vector3(0.3,0.3,0.3); 
		weapon_slots[i].obj.transform.rotation = main_camera.transform.rotation * Quaternion.AngleAxis(90, Vector3(0,1,0));
		renderers = weapon_slots[i].obj.GetComponentsInChildren(Renderer);
		for(var renderer : Renderer in renderers){
			renderer.castShadows = false; 
		}
	}
	
	if(Input.GetMouseButtonDown(0) && holstered == Holster.NOT_HOLSTERED && !slide_lock && thumb_on_hammer == Thumb.OFF_HAMMER && hammer_cocked == 1.0 && safety_off == 1.0){
		hammer_cocked = 0.0;
		if(round_in_chamber && slide_amount == 0.0 && round_in_chamber_state == RoundState.READY){
			round_in_chamber_state = RoundState.FIRED;
			GameObject.Destroy(round_in_chamber);
			round_in_chamber = Instantiate(shell_casing, gun_instance.transform.FindChild("point_chambered_round").position, gun_instance.transform.rotation);
			round_in_chamber.transform.parent = gun_instance.transform;
	
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
	
	for(i = 0; i < kMaxHeadRecoil; ++i){
		if(head_recoil_delay[i] != -1.0){
			head_recoil_delay[i] -= Time.deltaTime;
			if(head_recoil_delay[i] <= 0.0){
				head_x_recoil_vel += Random.Range(-30.0,30.0);
				head_y_recoil_vel += Random.Range(-30.0,30.0);
				head_recoil_delay[i] = -1.0;
			}
		}
	}
	
	if(holstered == Holster.NOT_HOLSTERED){
	if(Input.GetKeyDown('m')){
		if(mag_stage == MagStage.HOLD){
			target_hold_pose = 0.0;
			mag_stage = MagStage.HOLD_TO_INSERT;
			//magazine_instance_in_gun.transform.parent = gun_instance.transform;
			
			//magazine_instance_in_gun = Instantiate(magazine_obj);
			//mag_offset = -2.0;
			//mag_seated = 0.0;
		}
	}
	
	if(mag_stage == MagStage.HOLD_TO_INSERT){
		if(hold_pose < 0.01){
			mag_stage = MagStage.INSERTING;
		}
	}
	
	if(Input.GetKeyDown('e')){
		if(mag_stage == MagStage.IN){
			mag_stage = MagStage.REMOVING;
			y_recoil_offset_vel += Random.Range(-40.0,40.0);
			x_recoil_offset_vel += Random.Range(-40.0,100.0);
		} else if(mag_stage == MagStage.HOLD){
			mag_stage = MagStage.OUT;
			magazine_instance_in_gun.AddComponent(Rigidbody);
			magazine_instance_in_gun.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
			magazine_instance_in_gun.rigidbody.velocity = character_controller.velocity;
			magazine_instance_in_gun = null;
		} else if(mag_stage == MagStage.HOLD_TO_INSERT){
			mag_stage = MagStage.HOLD;
			target_hold_pose = 1.0;
		}
	}
	
	if(mag_stage == MagStage.INSERTING){
		mag_seated += Time.deltaTime * 5.0;
		if(mag_seated >= 1.0){
			mag_seated = 1.0;
			mag_stage = MagStage.IN;
			if(slide_lock){
				ChamberRoundFromMag();
			}
			y_recoil_offset_vel += Random.Range(-40.0,40.0);
			x_recoil_offset_vel += Random.Range(50.0,300.0);
			rotation_x += Random.Range(-0.4,0.4);
			rotation_y += Random.Range(0.0,1.0);
			left_hand_occupied = false;
		}
	}
	if(mag_stage == MagStage.REMOVING){
		mag_seated -= Time.deltaTime * 5.0;
		if(mag_seated <= 0.0){
			mag_seated = 0.0;
			mag_stage = MagStage.HOLD;
			magazine_instance_in_gun.transform.parent = null;
			target_hold_pose = 1.0;
			hold_pose = 0.0;
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
	if(Input.GetKeyDown('v')){
		if(safety == Safety.OFF){
			safety = Safety.ON;
		} else if(safety == Safety.ON){
			safety = Safety.OFF;
		}
	}
	if(safety == Safety.OFF){
		safety_off = Mathf.Min(1.0, safety_off + Time.deltaTime * 10.0);
	} else if(safety == Safety.ON){
		safety_off = Mathf.Max(0.0, safety_off - Time.deltaTime * 10.0);
	}
	
	if(Input.GetKeyDown('r')){
		if(left_hand_occupied == false && slide_stage == SlideStage.NOTHING){
			left_hand_occupied = true;
			slide_stage = SlideStage.PULLBACK;
		}
	}
	if(Input.GetKey('f')){
		thumb_on_hammer = Thumb.ON_HAMMER;
		hammer_cocked = Mathf.Min(1.0, hammer_cocked + Time.deltaTime * 10.0f);
	}
	if(Input.GetKeyUp('f')){
		if((Input.GetMouseButton(0) && safety_off == 1.0) || hammer_cocked != 1.0){
			thumb_on_hammer = Thumb.SLOW_LOWERING;
		} else {
			thumb_on_hammer = Thumb.OFF_HAMMER;
		}
	}
	if(thumb_on_hammer == Thumb.SLOW_LOWERING){
		hammer_cocked -= Time.deltaTime * 10.0f;
		if(hammer_cocked <= 0.0){
			hammer_cocked = 0.0;
			thumb_on_hammer = Thumb.OFF_HAMMER;
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
	}
	
	target_slide_pull_pose = 0.0;
	target_reload_pose = 0.0;
	
	if(holstered == Holster.NOT_HOLSTERED){
		if(safety == Safety.ON){
			target_reload_pose = 0.2;
			target_slide_pull_pose = 0.0;
			gun_tilt = GunTilt.RIGHT;
		}
		
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
	
	if(mag_stage == MagStage.HOLD || mag_stage == MagStage.HOLD_TO_INSERT){
		hold_pose_vel += (target_hold_pose - hold_pose) * kAimSpringStrength * Time.deltaTime;
		hold_pose_vel *= Mathf.Pow(kAimSpringDamping, Time.deltaTime);
		hold_pose += hold_pose_vel * Time.deltaTime;	
		
		mag_ground_pose_vel -= mag_ground_pose * kAimSpringStrength * Time.deltaTime;
		mag_ground_pose_vel *= Mathf.Pow(kAimSpringDamping, Time.deltaTime);
		mag_ground_pose += mag_ground_pose_vel * Time.deltaTime;	
	}
	
	gun_instance.transform.FindChild("slide").localPosition = 
		slide_rel_pos + 
		(gun_instance.transform.FindChild("point_slide_end").localPosition - 
		 gun_instance.transform.FindChild("point_slide_start").localPosition) * slide_amount;
	
	
	gun_instance.transform.FindChild("hammer").localPosition = 
		Vector3.Lerp(hammer_rel_pos, gun_instance.transform.FindChild("point_hammer_cocked").localPosition, hammer_cocked);
	gun_instance.transform.FindChild("hammer").localRotation = 
		Quaternion.Slerp(hammer_rel_rot, gun_instance.transform.FindChild("point_hammer_cocked").localRotation, hammer_cocked);
	
	gun_instance.transform.FindChild("safety").localPosition = 
		Vector3.Lerp(safety_rel_pos, gun_instance.transform.FindChild("point_safety_off").localPosition, safety_off);
	gun_instance.transform.FindChild("safety").localRotation = 
		Quaternion.Slerp(safety_rel_rot, gun_instance.transform.FindChild("point_safety_off").localRotation, safety_off);
		
	hammer_cocked = Mathf.Max(hammer_cocked, slide_amount);

	if(slide_stage == SlideStage.NOTHING){
		slide_amount = Mathf.Max(0.0, slide_amount - Time.deltaTime * kSlideLockSpeed);
		if(slide_amount == 0.0 && round_in_chamber_state == RoundState.LOADING){
			round_in_chamber_state = RoundState.READY;
		}
	}
	if(slide_lock){
		slide_amount = Mathf.Max(kSlideLockPosition, slide_amount);
	}
}

function FixedUpdate() {
}