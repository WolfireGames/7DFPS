#pragma strict

// Prefabs 

var magazine_obj:GameObject;
var gun_obj:GameObject;
var casing_with_bullet:GameObject;

// Shortcuts to components

private var main_camera:GameObject;
private var character_controller:CharacterController;

// Instances

private var gun_instance:GameObject;

// Public parameters

public var sensitivity_x = 2.0;
public var sensitivity_y = 2.0;
public var min_angle_y = -60.0;
public var max_angle_y = 60.0;

// Private variables

class Spring {
	var state : float;
	var target_state : float;
	var vel : float;
	var strength : float;
	var damping : float;
	function Spring(state : float, target_state : float, strength : float, damping : float){
		this.Set(state, target_state, strength, damping);
	}
	function Set(state : float, target_state : float, strength : float, damping : float){
		this.state = state;
		this.target_state = target_state;
		this.strength = strength;
		this.damping = damping;
		this.vel = 0.0;		
	}
	function Update() {
		this.vel += (this.target_state - this.state) * this.strength * Time.deltaTime;
		this.vel *= Mathf.Pow(this.damping, Time.deltaTime);
		this.state += this.vel * Time.deltaTime;	
	}
};

private var aim_toggle = false;
private var kAimSpringStrength = 100.0;
private var kAimSpringDamping = 0.00001;
private var aim_spring = new Spring(0,0,kAimSpringStrength,kAimSpringDamping);

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

private var kRecoilSpringStrength = 800.0;
private var kRecoilSpringDamping = 0.000001;
private var x_recoil_spring = new Spring(0,0,kRecoilSpringStrength,kRecoilSpringDamping);
private var y_recoil_spring = new Spring(0,0,kRecoilSpringStrength,kRecoilSpringDamping);
private var head_recoil_spring_x = new Spring(0,0,kRecoilSpringStrength,kRecoilSpringDamping);
private var head_recoil_spring_y = new Spring(0,0,kRecoilSpringStrength,kRecoilSpringDamping);

private var magazine_instance_in_hand:GameObject;
private var kGunDistance = 0.3;

enum Holster{NOT_HOLSTERED, HOLSTERED};
private var holstered = Holster.NOT_HOLSTERED;
private var holster_spring = new Spring(0,0,kAimSpringStrength, kAimSpringDamping);

private var slide_pose_spring = new Spring(0,0,kAimSpringStrength, kAimSpringDamping);
private var reload_pose_spring = new Spring(0,0,kAimSpringStrength, kAimSpringDamping);

enum GunTilt {LEFT, CENTER, RIGHT};
private var gun_tilt : GunTilt = GunTilt.CENTER;

private var hold_pose_spring = new Spring(0,0,kAimSpringStrength, kAimSpringDamping);
private var mag_ground_pose_spring = new Spring(0,0,kAimSpringStrength, kAimSpringDamping);

private var left_hand_occupied = false;
private var kMaxHeadRecoil = 10;
private var head_recoil_delay : float[] = new float[kMaxHeadRecoil];
private var next_head_recoil_delay = 0;
private var mag_ground_pos : Vector3;
private var mag_ground_rot : Quaternion;

enum HandMagStage {HOLD, HOLD_TO_INSERT, EMPTY};
private var mag_stage = HandMagStage.EMPTY;

private var collected_rounds = new Array();

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

function GetGunScript() : GunScript {
	return gun_instance.GetComponent(GunScript);
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
	
	//var holstered_dir = (transform.forward*0.1 + Vector3(0,-1,0)).normalized;
	//var holstered_pos = transform.position + transform.rotation * Vector3(0.5,-character_controller.height * 0.3,0.0);
		
	var i = 0;
	var holstered_pos = main_camera.transform.position + main_camera.camera.ScreenPointToRay(Vector3(main_camera.camera.pixelWidth * (0.05 + i*0.15), main_camera.camera.pixelHeight * 0.17,0)).direction * 0.3;
	var holstered_scale = Vector3(0.3,0.3,0.3); 
	var holstered_rot = main_camera.transform.rotation * Quaternion.AngleAxis(90, Vector3(0,1,0));
		
	gun_instance.transform.position = mix(unaimed_pos, aim_pos, aim_spring.state);
	gun_instance.transform.forward = mix(unaimed_dir, aim_dir, aim_spring.state);
	
	gun_instance.transform.position = mix(gun_instance.transform.position, holstered_pos, holster_spring.state);
	gun_instance.transform.rotation = mix(gun_instance.transform.rotation, holstered_rot, holster_spring.state);
	gun_instance.transform.localScale = mix(Vector3(1.0,1.0,1.0), holstered_scale, holster_spring.state);
	
	gun_instance.transform.position = mix(gun_instance.transform.position,
									      gun_instance.transform.FindChild("pose_slide_pull").position,
									      slide_pose_spring.state);
									      
	gun_instance.transform.rotation = mix(
		gun_instance.transform.rotation,
		gun_instance.transform.FindChild("pose_slide_pull").rotation,
		slide_pose_spring.state);
	gun_instance.transform.position = mix(gun_instance.transform.position,
									      gun_instance.transform.FindChild("pose_reload").position,
									      reload_pose_spring.state);
	gun_instance.transform.rotation = mix(
		gun_instance.transform.rotation,
		gun_instance.transform.FindChild("pose_reload").rotation,
		reload_pose_spring.state);
		
	gun_instance.transform.RotateAround(
		gun_instance.transform.FindChild("point_recoil_rotate").position,
		gun_instance.transform.rotation * Vector3(1,0,0),
		x_recoil_spring.state);
		
	gun_instance.transform.RotateAround(
		gun_instance.transform.FindChild("point_recoil_rotate").position,
		Vector3(0,1,0),
		y_recoil_spring.state);
	
	if(magazine_instance_in_hand){
		var mag_pos = gun_instance.transform.position;
		var mag_rot = gun_instance.transform.rotation;
		mag_pos += (gun_instance.transform.FindChild("point_mag_to_insert").position - 
				    gun_instance.transform.FindChild("point_mag_inserted").position);
	   if(mag_stage == HandMagStage.HOLD || mag_stage == HandMagStage.HOLD_TO_INSERT){
			var hold_pos = main_camera.transform.position + main_camera.transform.rotation*Vector3(-0.15,0.05,0.2);
			var hold_rot = main_camera.transform.rotation * Quaternion.AngleAxis(45, Vector3(0,1,0)) * Quaternion.AngleAxis(-55, Vector3(1,0,0));
	   		hold_pos = mix(hold_pos, mag_ground_pos, mag_ground_pose_spring.state);
	   		hold_rot = mix(hold_rot, mag_ground_rot, mag_ground_pose_spring.state);
	   		magazine_instance_in_hand.transform.position = mix(mag_pos, hold_pos, hold_pose_spring.state);
			magazine_instance_in_hand.transform.rotation = mix(mag_rot, hold_rot, hold_pose_spring.state);
		} else {
			magazine_instance_in_hand.transform.position = mag_pos;
			magazine_instance_in_hand.transform.rotation = mag_rot;
		}
	}
	
	if(Input.GetMouseButton(1) || aim_toggle){
		aim_spring.target_state = 1.0;
	} else {
		aim_spring.target_state = 0.0;
	}
	aim_spring.Update();
	
	if(Input.GetKeyDown('g')){
		var nearest_mag = null;
		var nearest_mag_dist = 0.0;
		var colliders = Physics.OverlapSphere(main_camera.transform.position, 2.0, 1 << 8);
		for(var collider in colliders){
			if(collider.gameObject.name == magazine_obj.name+"(Clone)" && collider.gameObject.rigidbody){
				var dist = Vector3.Distance(collider.transform.position, main_camera.transform.position);
				if(!nearest_mag || dist < nearest_mag_dist){	
					nearest_mag_dist = dist;
					nearest_mag = collider.gameObject;
				}					
			} else if(collider.gameObject.name == casing_with_bullet.name+"(Clone)" && collider.gameObject.rigidbody){
				collected_rounds.push(collider.gameObject);			
				collider.gameObject.rigidbody.useGravity = false;
				collider.gameObject.rigidbody.WakeUp();
				collider.enabled = false;
			}
		}
		if(nearest_mag && mag_stage == HandMagStage.EMPTY){
			magazine_instance_in_hand = nearest_mag;
			Destroy(magazine_instance_in_hand.rigidbody);
			mag_ground_pos = magazine_instance_in_hand.transform.position;
			mag_ground_rot = magazine_instance_in_hand.transform.rotation;
			mag_ground_pose_spring.state = 1.0;
			mag_ground_pose_spring.vel = 1.0;
			hold_pose_spring.state = 1.0;
			hold_pose_spring.vel = 0.0;
			hold_pose_spring.target_state = 1.0;
			mag_stage = HandMagStage.HOLD;
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
		if(mag_stage == HandMagStage.HOLD && target_weapon_slot != -1 && weapon_slots[target_weapon_slot].type == WeaponSlotType.EMPTY){
			weapon_slots[target_weapon_slot].type = WeaponSlotType.MAGAZINE;
			weapon_slots[target_weapon_slot].in_use = false;
			weapon_slots[target_weapon_slot].obj = magazine_instance_in_hand;
			magazine_instance_in_hand = null;
			mag_stage = HandMagStage.EMPTY;
			target_weapon_slot = active_weapon_slot;
		} else if(mag_stage == HandMagStage.HOLD && target_weapon_slot != -1 && weapon_slots[target_weapon_slot].type == WeaponSlotType.MAGAZINE){
			var temp = weapon_slots[target_weapon_slot].obj;
			weapon_slots[target_weapon_slot].obj = magazine_instance_in_hand;
			magazine_instance_in_hand = temp;
			magazine_instance_in_hand.transform.localScale = Vector3(1.0,1.0,1.0);
			target_weapon_slot = active_weapon_slot;
		} else if(target_weapon_slot != -1 && mag_stage == HandMagStage.EMPTY && weapon_slots[target_weapon_slot].type == WeaponSlotType.MAGAZINE){
			magazine_instance_in_hand = weapon_slots[target_weapon_slot].obj;
			magazine_instance_in_hand.transform.localScale = Vector3(1.0,1.0,1.0);
			mag_stage = HandMagStage.HOLD;
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
					magazine_instance_in_hand = weapon_slots[target_weapon_slot].obj;
					magazine_instance_in_hand.transform.localScale = Vector3(1.0,1.0,1.0);
					mag_stage = HandMagStage.HOLD;
					weapon_slots[target_weapon_slot].type = WeaponSlotType.EMPTY;
					weapon_slots[target_weapon_slot].obj = null;
					weapon_slots[target_weapon_slot].in_use = false;
					target_weapon_slot = active_weapon_slot;
				}
			}
		}
	}
	
	if(holstered == Holster.HOLSTERED){
		holster_spring.target_state = 1.0;
		if(holster_spring.state >= 1.0){
			weapon_slots[0].in_use = false;
			active_weapon_slot = -1;
		}
	} else {
		holster_spring.target_state = 0.0;
	}
	
	holster_spring.Update();
	
	rotation_y_min_leeway = Mathf.Lerp(0.0,kRotationYMinLeeway,aim_spring.state);
	rotation_y_max_leeway = Mathf.Lerp(0.0,kRotationYMaxLeeway,aim_spring.state);
	rotation_x_leeway = Mathf.Lerp(0.0,kRotationXLeeway,aim_spring.state);
	
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
	main_camera.transform.localEulerAngles += Vector3(head_recoil_spring_y.state, head_recoil_spring_x.state, 0);
	character_controller.transform.localEulerAngles.y = view_rotation_x;
	
	for(i=0; i<10; ++i){
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
	
	for(i = 0; i < kMaxHeadRecoil; ++i){
		if(head_recoil_delay[i] != -1.0){
			head_recoil_delay[i] -= Time.deltaTime;
			if(head_recoil_delay[i] <= 0.0){
				head_recoil_spring_x.vel += Random.Range(-30.0,30.0);
				head_recoil_spring_y.vel += Random.Range(-30.0,30.0);
				head_recoil_delay[i] = -1.0;
			}
		}
	}
	
	if(Input.GetKeyDown('e')){
		if(mag_stage == HandMagStage.HOLD){
			mag_stage = HandMagStage.EMPTY;
			magazine_instance_in_hand.AddComponent(Rigidbody);
			magazine_instance_in_hand.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
			magazine_instance_in_hand.rigidbody.velocity = character_controller.velocity;
			magazine_instance_in_hand = null;
		}
	}
	
	if(Input.GetKeyDown('e')){
		if(mag_stage == HandMagStage.EMPTY && gun_instance){
			gun_instance.GetComponent(GunScript).MagEject();
		} else if(mag_stage == HandMagStage.HOLD_TO_INSERT){
			mag_stage = HandMagStage.HOLD;
			hold_pose_spring.target_state = 1.0;
		}
	}
	/*
	if(mag_stage == MagStage.REMOVING){
		mag_seated -= Time.deltaTime * 5.0;
		if(mag_seated <= 0.0){
			mag_seated = 0.0;
			mag_stage = MagStage.HOLD;
			magazine_instance_in_hand.transform.parent = null;
			hold_pose_spring.target_state = 1.0;
			hold_pose_spring.state = 0.0;
		}
	}
	*/
	
	if(gun_instance){
		var gun_script = GetGunScript();
		if(Input.GetMouseButton(0)){
			gun_script.ApplyPressureToTrigger();
		} else {
			gun_script.ReleasePressureFromTrigger();
		}
		if(Input.GetKeyDown('t')){
			gun_script.ReleaseSlideLock();
		}
		if(Input.GetKey('t')){
			gun_script.PressureOnSlideLock();
		}
		if(Input.GetKeyDown('v')){
			gun_script.ToggleSafety();			
		}	
		if(Input.GetKeyDown('r')){
			gun_script.PullBackSlide();
		}
		if(Input.GetKeyUp('r')){
			gun_script.ReleaseSlide();
		}
		if(Input.GetKey('f')){
			gun_script.PressureOnHammer();
		}
		if(Input.GetKeyUp('f')){
			gun_script.ReleaseHammer();
		}		
		
		if(slide_pose_spring.target_state < 0.1 && reload_pose_spring.target_state < 0.1){
			gun_tilt = GunTilt.CENTER;
		} else if(slide_pose_spring.target_state > reload_pose_spring.target_state){
			gun_tilt = GunTilt.LEFT;
		} else {
			gun_tilt = GunTilt.RIGHT;
		}
		
		slide_pose_spring.target_state = 0.0;
		reload_pose_spring.target_state = 0.0;
		
		if(gun_script.IsSafetyOn()){
			reload_pose_spring.target_state = 0.2;
			slide_pose_spring.target_state = 0.0;
			gun_tilt = GunTilt.RIGHT;
		}
		
		if(gun_script.IsSlideLocked()){
			if(gun_tilt != GunTilt.LEFT){
				reload_pose_spring.target_state = 0.7;
			} else {
				slide_pose_spring.target_state = 0.7;
			}
		}
		if(gun_script.IsSlidePulledBack()){
			if(gun_tilt != GunTilt.RIGHT){
				slide_pose_spring.target_state = 1.0;
			} else {
				reload_pose_spring.target_state = 1.0;
			}
		}
		x_recoil_spring.vel += gun_script.recoil_transfer_x;
		y_recoil_spring.vel += gun_script.recoil_transfer_y;
		rotation_x += gun_script.rotation_transfer_x;
		rotation_y += gun_script.rotation_transfer_y;
		gun_script.recoil_transfer_x = 0.0;
		gun_script.recoil_transfer_y = 0.0;
		gun_script.rotation_transfer_x = 0.0;
		gun_script.rotation_transfer_y = 0.0;
		if(gun_script.add_head_recoil){
			head_recoil_delay[next_head_recoil_delay] = 0.1;
			next_head_recoil_delay = (next_head_recoil_delay + 1)%kMaxHeadRecoil;
			gun_script.add_head_recoil = false;
		}
		
		if(gun_script.ready_to_remove_mag && !magazine_instance_in_hand){
			magazine_instance_in_hand = gun_script.RemoveMag();
			mag_stage = HandMagStage.HOLD;
			hold_pose_spring.state = 0.0;
			hold_pose_spring.vel = 0.0;
			hold_pose_spring.target_state = 1.0;
		}
		if(Input.GetKeyDown('m')){
			if(mag_stage == HandMagStage.HOLD){
				hold_pose_spring.target_state = 0.0;
				mag_stage = HandMagStage.HOLD_TO_INSERT;
			}
		}
		if(mag_stage == HandMagStage.HOLD_TO_INSERT){
			if(hold_pose_spring.state < 0.01){
				gun_script.InsertMag(magazine_instance_in_hand);
				magazine_instance_in_hand = null;
				mag_stage = HandMagStage.EMPTY;
			}
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
	
	slide_pose_spring.Update();
	reload_pose_spring.Update();
	x_recoil_spring.Update();
	y_recoil_spring.Update();
	head_recoil_spring_x.Update();
	head_recoil_spring_y.Update();

	if(mag_stage == HandMagStage.HOLD || mag_stage == HandMagStage.HOLD_TO_INSERT){
		hold_pose_spring.Update();
		mag_ground_pose_spring.Update();
	}
	
	var attract_pos = transform.position - Vector3(0,character_controller.height * 0.2,0);
	for(i=0; i<collected_rounds.length; ++i){
		var round = collected_rounds[i] as GameObject;
		round.rigidbody.velocity += (attract_pos - round.transform.position) * Time.deltaTime * 20.0;
		round.rigidbody.velocity *= Mathf.Pow(0.1, Time.deltaTime);;
		//round.rigidbody.position += round.rigidbody.velocity * Time.deltaTime;
		if(Vector3.Distance(round.transform.position, attract_pos) < 0.5){
			GameObject.Destroy(round);
			++num_loose_bullets;
			collected_rounds.splice(i,1);
		}
	}
	collected_rounds.remove(null);
}

function FixedUpdate() {
}