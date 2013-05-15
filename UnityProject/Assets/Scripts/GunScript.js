#pragma strict


enum GunType {AUTOMATIC, REVOLVER};
var gun_type : GunType;

enum ActionType {DOUBLE, SINGLE};
var action_type : ActionType;

var sound_gunshot_bigroom : AudioClip[];
var sound_gunshot_smallroom : AudioClip[];
var sound_gunshot_open : AudioClip[];
var sound_mag_eject_button : AudioClip[];
var sound_mag_ejection : AudioClip[];
var sound_mag_insertion : AudioClip[];
var sound_slide_back : AudioClip[];
var sound_slide_front : AudioClip[];
var sound_safety : AudioClip[];
var sound_bullet_eject : AudioClip[];
var sound_cylinder_open : AudioClip[];
var sound_cylinder_close : AudioClip[];
var sound_extractor_rod_open : AudioClip[];
var sound_extractor_rod_close : AudioClip[];
var sound_cylinder_rotate : AudioClip[];
var sound_hammer_cock : AudioClip[];
var sound_hammer_decock : AudioClip[];

private var kGunMechanicVolume = 0.2;

var add_head_recoil = false;
var recoil_transfer_x = 0.0;
var recoil_transfer_y = 0.0;
var rotation_transfer_x = 0.0;
var rotation_transfer_y = 0.0;

var old_pos : Vector3;
var velocity : Vector3;

var magazine_obj:GameObject;

var bullet_obj:GameObject;
var muzzle_flash:GameObject;

var shell_casing:GameObject;
var casing_with_bullet:GameObject;
var ready_to_remove_mag = false;

enum PressureState {NONE, INITIAL, CONTINUING};
var pressure_on_trigger = PressureState.NONE;
var trigger_pressed = 0.0;

private var active_round:GameObject;
enum RoundState {EMPTY, READY, FIRED, LOADING, JAMMED};
private var active_round_state = RoundState.READY;

private var magazine_instance_in_gun:GameObject;
private var mag_offset = 0.0;

private var slide_pressure = false;
private var slide_rel_pos : Vector3;
private var slide_amount = 0.0;
private var slide_lock = false;
enum SlideStage {NOTHING, PULLBACK, HOLD};
private var slide_stage : SlideStage = SlideStage.NOTHING;

enum Thumb{ON_HAMMER, OFF_HAMMER, SLOW_LOWERING};
private var thumb_on_hammer = Thumb.OFF_HAMMER;
private var hammer_rel_pos : Vector3;
private var hammer_rel_rot : Quaternion;
private var hammer_cocked = 1.0;

private var safety_rel_pos : Vector3;
private var safety_rel_rot : Quaternion;

private var safety_off = 1.0;
enum Safety{OFF, ON};
private var safety = Safety.OFF;

private var kSlideLockPosition = 0.9;
private var kPressCheckPosition = 0.4;
private var kSlideLockSpeed = 20.0;

enum MagStage {OUT, INSERTING, IN, REMOVING};
private var mag_stage : MagStage = MagStage.IN;
private var mag_seated = 1.0;

enum AutoModStage {ENABLED, DISABLED};
private var auto_mod_stage : AutoModStage = AutoModStage.DISABLED;
private var auto_mod_amount = 0.0;
private var auto_mod_rel_pos : Vector3;
private var fired_once_this_pull = false;

private var has_slide = false;
private var has_safety = false;
private var has_hammer = false;
private var has_auto_mod = false;

private var yolk_pivot_rel_rot : Quaternion;
private var yolk_open = 0.0;
enum YolkStage {CLOSED, OPENING, OPEN, CLOSING};
private var yolk_stage : YolkStage = YolkStage.CLOSED;
private var cylinder_rotation = 0.0;
private var cylinder_rotation_vel = 0.0;
private var active_cylinder = 0;
private var target_cylinder_offset = 0;
enum ExtractorRodStage {CLOSED, OPENING, OPEN, CLOSING};
private var extractor_rod_stage = ExtractorRodStage.CLOSED;
private var extractor_rod_amount = 0.0;
private var extracted = false;
private var extractor_rod_rel_pos : Vector3;

class CylinderState {
	var object : GameObject = null;
	var can_fire : boolean = false;
	var seated : float = 0.0;
	var falling : boolean = false;
};

private var cylinder_capacity = 6;
var cylinders : CylinderState[];

function IsAddingRounds() : boolean {
	if(yolk_stage == YolkStage.OPEN){
		return true;
	} else {
		return false;
	}
}

function IsEjectingRounds() : boolean {
	if(extractor_rod_stage != ExtractorRodStage.CLOSED){
		return true;
	} else {
		return false;
	}
}

function GetHammer() : Transform {
	var hammer = transform.FindChild("hammer");
	if(!hammer){
		hammer = transform.FindChild("hammer_pivot");
	}
	return hammer;
}

function GetHammerCocked() : Transform {
	var hammer = transform.FindChild("point_hammer_cocked");
	if(!hammer){
		hammer = transform.FindChild("hammer_pivot");
	}
	return hammer;
}

function Start () {
	if(transform.FindChild("slide")){
		var slide = transform.FindChild("slide");
		has_slide = true;
		slide_rel_pos = slide.localPosition;
		if(slide.FindChild("auto mod toggle")){
			has_auto_mod = true;
			auto_mod_rel_pos = slide.FindChild("auto mod toggle").localPosition;
			if(Random.Range(0,2) == 0){
				auto_mod_amount = 1.0;
				auto_mod_stage = AutoModStage.ENABLED;
			}
		}
	}
	var hammer = GetHammer();
	if(hammer){
		has_hammer = true;
		hammer_rel_pos = hammer.localPosition;
		hammer_rel_rot = hammer.localRotation;
	}
	var yolk_pivot = transform.FindChild("yolk_pivot");
	if(yolk_pivot){
		yolk_pivot_rel_rot = yolk_pivot.localRotation;
		var yolk = yolk_pivot.FindChild("yolk");
		if(yolk){
			var cylinder_assembly = yolk.FindChild("cylinder_assembly");
			if(cylinder_assembly){
				var extractor_rod = cylinder_assembly.FindChild("extractor_rod");
				if(extractor_rod){
					extractor_rod_rel_pos = extractor_rod.localPosition;
				}
			}
		}
	}
	
	if(gun_type == GunType.AUTOMATIC){
		magazine_instance_in_gun = Instantiate(magazine_obj);
		magazine_instance_in_gun.transform.parent = transform;
	
		var renderers = magazine_instance_in_gun.GetComponentsInChildren(Renderer);
		for(var renderer : Renderer in renderers){
			renderer.castShadows = false; 
		}
		
		if(Random.Range(0,2) == 0){
			active_round = Instantiate(casing_with_bullet, transform.FindChild("point_chambered_round").position, transform.FindChild("point_chambered_round").rotation);
			active_round.transform.parent = transform;
			active_round.transform.localScale = Vector3(1.0,1.0,1.0);
			renderers = active_round.GetComponentsInChildren(Renderer);
			for(var renderer : Renderer in renderers){
				renderer.castShadows = false; 
			}
		}
		
		if(Random.Range(0,2) == 0 && magazine_instance_in_gun.GetComponent(mag_script).NumRounds()>0){
			active_round_state = RoundState.LOADING;
			slide_amount = kSlideLockPosition;
			slide_lock = true;
		}
	}
	
	if(gun_type == GunType.REVOLVER){
		cylinders = new CylinderState[cylinder_capacity];
		for(var i=0; i<cylinder_capacity; ++i){
			cylinders[i] = new CylinderState();
			if(Random.Range(0,2) == 0){
				continue;
			}
			var name = "point_chamber_"+(i+1);
			cylinders[i].object = Instantiate(casing_with_bullet, extractor_rod.FindChild(name).position, extractor_rod.FindChild(name).rotation);
			cylinders[i].object.transform.localScale = Vector3(1.0,1.0,1.0);
			cylinders[i].can_fire = true;
			cylinders[i].seated = Random.Range(0.0,0.5);
			renderers = cylinders[i].object.GetComponentsInChildren(Renderer);
			for(var renderer : Renderer in renderers){
				renderer.castShadows = false; 
			}
		}
	}
	
	if(Random.Range(0,2) == 0 && has_hammer){
		hammer_cocked = 0.0;
	}
	
	if(transform.FindChild("safety")){
		has_safety = true;
		safety_rel_pos = transform.FindChild("safety").localPosition;
		safety_rel_rot = transform.FindChild("safety").localRotation;
		if(Random.Range(0,4) == 0){
			safety_off = 0.0;
			safety = Safety.ON;
			slide_amount = 0.0;
			slide_lock = false;
		}
	}
	
}

function MagScript() : mag_script {
	return magazine_instance_in_gun.GetComponent("mag_script");
}

function ShouldPullSlide() {
	if(gun_type != GunType.AUTOMATIC){
		return false;
	}
	return (!active_round && magazine_instance_in_gun && magazine_instance_in_gun.GetComponent(mag_script).NumRounds()>0);
}

function ShouldReleaseSlideLock() {
	return (active_round && slide_lock);
}

function ShouldEjectMag() {
	if(gun_type != GunType.AUTOMATIC){
		return false;
	}
	return (magazine_instance_in_gun && magazine_instance_in_gun.GetComponent(mag_script).NumRounds() == 0);
}

function ChamberRoundFromMag() : boolean {
	if(gun_type != GunType.AUTOMATIC){
		return false;
	}
	if(magazine_instance_in_gun && MagScript().NumRounds() > 0 && mag_stage == MagStage.IN){
		if(!active_round){
			active_round = Instantiate(casing_with_bullet, transform.FindChild("point_load_round").position, transform.FindChild("point_load_round").rotation);
			var renderers = active_round.GetComponentsInChildren(Renderer);
			for(var renderer : Renderer in renderers){
				renderer.castShadows = false; 
			}
			active_round.transform.parent = transform;
			active_round.transform.localScale = Vector3(1.0,1.0,1.0);
			active_round_state = RoundState.LOADING;
		}
		return true;
	} else {
		return false;
	}
}

function PullSlideBack() {
	if(gun_type != GunType.AUTOMATIC){
		return false;
	}
	slide_amount = 1.0;
	if(slide_lock && mag_stage == MagStage.IN && (!magazine_instance_in_gun || MagScript().NumRounds() == 0)){
		return;
	}
	slide_lock = false;
	if(active_round && (active_round_state == RoundState.FIRED || active_round_state == RoundState.READY)){
		active_round.AddComponent(Rigidbody);
		PlaySoundFromGroup(sound_bullet_eject, kGunMechanicVolume);
		active_round.transform.parent = null;
		active_round.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		active_round.rigidbody.velocity = velocity;
		active_round.rigidbody.velocity += transform.rotation * Vector3(Random.Range(2.0,4.0),Random.Range(1.0,2.0),Random.Range(-1.0,-3.0));
		active_round.rigidbody.angularVelocity = Vector3(Random.Range(-40.0,40.0),Random.Range(-40.0,40.0),Random.Range(-40.0,40.0));
		active_round = null;
	}
	if(!ChamberRoundFromMag() && mag_stage == MagStage.IN){
		slide_lock = true;
	}
}

function ReleaseSlideLock() {
	if(gun_type != GunType.AUTOMATIC){
		return false;
	}
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

function PlaySoundFromGroup(group : Array, volume : float){
	if(group.length == 0){
		return;
	}
	var which_shot = Random.Range(0,group.length);
	audio.PlayOneShot(group[which_shot], volume * PlayerPrefs.GetFloat("sound_volume", 1.0));
}


function ApplyPressureToTrigger() : boolean {
	if(pressure_on_trigger == PressureState.NONE){
		pressure_on_trigger = PressureState.INITIAL;
		fired_once_this_pull = false;
	} else {
		pressure_on_trigger = PressureState.CONTINUING;
	}
	if(yolk_stage != YolkStage.CLOSED){
		return;
	}
	if((pressure_on_trigger == PressureState.INITIAL || action_type == ActionType.DOUBLE) && !slide_lock && thumb_on_hammer == Thumb.OFF_HAMMER && hammer_cocked == 1.0 && safety_off == 1.0 && (auto_mod_stage == AutoModStage.ENABLED || !fired_once_this_pull)){
		trigger_pressed = 1.0;
		if(gun_type == GunType.AUTOMATIC && slide_amount == 0.0){
			hammer_cocked = 0.0;
			if(active_round && active_round_state == RoundState.READY){
				fired_once_this_pull = true;
				PlaySoundFromGroup(sound_gunshot_smallroom, 1.0);
				active_round_state = RoundState.FIRED;
				GameObject.Destroy(active_round);
				active_round = Instantiate(shell_casing, transform.FindChild("point_chambered_round").position, transform.rotation);
				active_round.transform.parent = transform;
				var renderers = active_round.GetComponentsInChildren(Renderer);
				for(var renderer : Renderer in renderers){
					renderer.castShadows = false; 
				}
				
				Instantiate(muzzle_flash, transform.FindChild("point_muzzleflash").position, transform.FindChild("point_muzzleflash").rotation);
				var bullet = Instantiate(bullet_obj, transform.FindChild("point_muzzle").position, transform.FindChild("point_muzzle").rotation);
				bullet.GetComponent(BulletScript).SetVelocity(transform.forward * 251.0);
				PullSlideBack();
				rotation_transfer_y += Random.Range(1.0,2.0);
				rotation_transfer_x += Random.Range(-1.0,1.0);
				recoil_transfer_x -= Random.Range(150.0,300.0);
				recoil_transfer_y += Random.Range(-200.0,200.0);
				add_head_recoil = true;
				return true;
			} else {
				PlaySoundFromGroup(sound_mag_eject_button, 0.5);
			}
		} else if(gun_type == GunType.REVOLVER){
			hammer_cocked = 0.0;
			var which_chamber = active_cylinder % cylinder_capacity;
			if(which_chamber < 0){
				which_chamber += cylinder_capacity;
			}
			var round = cylinders[which_chamber].object;
			if(round && cylinders[which_chamber].can_fire){
				PlaySoundFromGroup(sound_gunshot_smallroom, 1.0);
				cylinders[which_chamber].can_fire = false;
				cylinders[which_chamber].seated += Random.Range(0.0,0.5);
				cylinders[which_chamber].object = Instantiate(shell_casing, round.transform.position, round.transform.rotation);
				GameObject.Destroy(round);
				renderers = cylinders[which_chamber].object.GetComponentsInChildren(Renderer);
				for(var renderer : Renderer in renderers){
					renderer.castShadows = false; 
				}				
				Instantiate(muzzle_flash, transform.FindChild("point_muzzleflash").position, transform.FindChild("point_muzzleflash").rotation);
				bullet = Instantiate(bullet_obj, transform.FindChild("point_muzzle").position, transform.FindChild("point_muzzle").rotation);
				bullet.GetComponent(BulletScript).SetVelocity(transform.forward * 251.0);
				rotation_transfer_y += Random.Range(1.0,2.0);
				rotation_transfer_x += Random.Range(-1.0,1.0);
				recoil_transfer_x -= Random.Range(150.0,300.0);
				recoil_transfer_y += Random.Range(-200.0,200.0);
				add_head_recoil = true;
				return true;
			} else {
				PlaySoundFromGroup(sound_mag_eject_button, 0.5);
			}
		}
	}
	
	if(action_type == ActionType.DOUBLE && trigger_pressed < 1.0 && thumb_on_hammer == Thumb.OFF_HAMMER){
		CockHammer();
		CockHammer();
	}
	
	return false;
}

function ReleasePressureFromTrigger() {
	pressure_on_trigger = PressureState.NONE;
	trigger_pressed = 0.0;
}

function MagEject() : boolean {
	if(gun_type != GunType.AUTOMATIC){
		return false;
	}
	PlaySoundFromGroup(sound_mag_eject_button, kGunMechanicVolume);
	if(active_round_state == RoundState.LOADING){
		GameObject.Destroy(active_round);
		active_round_state = RoundState.EMPTY;
	}
	if(mag_stage != MagStage.OUT){
		mag_stage = MagStage.REMOVING;
		PlaySoundFromGroup(sound_mag_ejection, kGunMechanicVolume);
		return true;
	}
	return false;
}

function TryToReleaseSlideLock() {
	if(gun_type != GunType.AUTOMATIC){
		return false;
	}
	if(slide_amount == kSlideLockPosition){
		ReleaseSlideLock();
	}
}

function PressureOnSlideLock() {
	if(gun_type != GunType.AUTOMATIC){
		return false;
	}
	if(slide_amount > kPressCheckPosition && slide_stage == SlideStage.PULLBACK){
		slide_lock = true;
	} else if(slide_amount > kSlideLockPosition){// && slide_stage == SlideStage.NOTHING){
		slide_lock = true;
	}
}

function ReleasePressureOnSlideLock() {
	if(slide_amount == kPressCheckPosition){
		slide_lock = false;
		if(slide_pressure){
			slide_stage = SlideStage.PULLBACK;
		}
	} else if(slide_amount == 1.0){
		slide_lock = false;
	}
}

function ToggleSafety() {
	if(!has_safety){
		return false;
	}
	if(safety == Safety.OFF){
		if(slide_amount == 0.0 && hammer_cocked == 1.0){
			safety = Safety.ON;
			PlaySoundFromGroup(sound_safety, kGunMechanicVolume);
		}
	} else if(safety == Safety.ON){
		safety = Safety.OFF;
		PlaySoundFromGroup(sound_safety, kGunMechanicVolume);
	}
}

function ToggleAutoMod() {
	if(!has_auto_mod){
		return false;
	}
	PlaySoundFromGroup(sound_safety, kGunMechanicVolume);
	if(auto_mod_stage == AutoModStage.DISABLED){
		auto_mod_stage = AutoModStage.ENABLED;
	} else if(auto_mod_stage == AutoModStage.ENABLED){
		auto_mod_stage = AutoModStage.DISABLED;
	}
}

function PullBackSlide() {
	if(gun_type != GunType.AUTOMATIC){
		return false;
	}
	if(slide_stage != SlideStage.PULLBACK && safety == Safety.OFF){
		slide_stage = SlideStage.PULLBACK;
		slide_lock = false;
	}
	slide_pressure = true;
}

function ReleaseSlide() {
	if(gun_type != GunType.AUTOMATIC){
		return false;
	}
	slide_stage = SlideStage.NOTHING;
	slide_pressure = false;
}

function CockHammer(){
	var old_hammer_cocked = hammer_cocked;
	hammer_cocked = Mathf.Min(1.0, hammer_cocked + Time.deltaTime * 10.0f);
	if(hammer_cocked == 1.0 && old_hammer_cocked != 1.0){
		if(thumb_on_hammer == Thumb.ON_HAMMER){
			PlaySoundFromGroup(sound_hammer_cock, kGunMechanicVolume);
		}
		++active_cylinder;
		cylinder_rotation = active_cylinder * 360.0 / cylinder_capacity;
	}
	if(hammer_cocked < 1.0){
		cylinder_rotation = (active_cylinder + hammer_cocked) * 360.0 / cylinder_capacity;
		target_cylinder_offset = 0.0;
	}
}

function PressureOnHammer() {
	if(!has_hammer){
		return;
	}
	thumb_on_hammer = Thumb.ON_HAMMER;
	if(gun_type == GunType.REVOLVER && yolk_stage != YolkStage.CLOSED){
		return;
	}
	CockHammer();
}

function ReleaseHammer() {
	if(!has_hammer){
		return;
	}
	if((pressure_on_trigger != PressureState.NONE && safety_off == 1.0) || hammer_cocked != 1.0){
		thumb_on_hammer = Thumb.SLOW_LOWERING;
		trigger_pressed = 1.0;
	} else {
		thumb_on_hammer = Thumb.OFF_HAMMER;
	}
}

function IsSafetyOn() : boolean {
	return (safety == Safety.ON);
}

function IsSlideLocked() : boolean {
	if(gun_type != GunType.AUTOMATIC){
		return false;
	}
	return (slide_lock);
}

function IsSlidePulledBack() : boolean {
	if(gun_type != GunType.AUTOMATIC){
		return false;
	}
	return (slide_stage != SlideStage.NOTHING);
}

function RemoveMag() : GameObject {
	if(gun_type != GunType.AUTOMATIC){
		return null;
	}
	var mag = magazine_instance_in_gun;
	magazine_instance_in_gun = null;
	mag.transform.parent = null;
	ready_to_remove_mag = false;
	return mag;
}

function IsThereAMagInGun() : boolean {
	if(gun_type != GunType.AUTOMATIC){
		return false;
	}
	return magazine_instance_in_gun;
}

function IsMagCurrentlyEjecting() : boolean {
	if(gun_type != GunType.AUTOMATIC){
		return false;
	}
	return mag_stage == MagStage.REMOVING;
}

function InsertMag(mag : GameObject) {
	if(gun_type != GunType.AUTOMATIC){
		return;
	}
	if(magazine_instance_in_gun){
		return;
	}
	magazine_instance_in_gun = mag;
	mag.transform.parent = transform;
	mag_stage = MagStage.INSERTING;
	PlaySoundFromGroup(sound_mag_insertion, kGunMechanicVolume);
	mag_seated = 0.0;
}

function IsCylinderOpen(){
	return yolk_stage == YolkStage.OPEN || yolk_stage == YolkStage.OPENING;
}

function AddRoundToCylinder() : boolean {
	if(gun_type != GunType.REVOLVER || yolk_stage != YolkStage.OPEN){
		return false;
	}
	var best_chamber = -1;
	var next_shot = active_cylinder;
	if(!IsHammerCocked()){
		next_shot = (next_shot + 1) % cylinder_capacity;
	}
	for(var i=0; i<cylinder_capacity; ++i){
		var check = (next_shot + i)%cylinder_capacity;
		if(check < 0){
			check += cylinder_capacity;
		}
		if(!cylinders[check].object){
			best_chamber = check;
			break;
		}
	}
	if(best_chamber == -1){
		return false;
	}
	var yolk_pivot = transform.FindChild("yolk_pivot");
	if(yolk_pivot){
		var yolk = yolk_pivot.FindChild("yolk");
		if(yolk){
			var cylinder_assembly = yolk.FindChild("cylinder_assembly");
			if(cylinder_assembly){
				var extractor_rod = cylinder_assembly.FindChild("extractor_rod");
				if(extractor_rod){
					var name = "point_chamber_"+(best_chamber+1);
					cylinders[best_chamber].object = Instantiate(casing_with_bullet, extractor_rod.FindChild(name).position, extractor_rod.FindChild(name).rotation);
					cylinders[best_chamber].object.transform.localScale = Vector3(1.0,1.0,1.0);
					cylinders[best_chamber].can_fire = true;
					cylinders[best_chamber].seated = Random.Range(0.0,1.0);
					var renderers = cylinders[best_chamber].object.GetComponentsInChildren(Renderer);
					for(var renderer : Renderer in renderers){
						renderer.castShadows = false; 
					}
					PlaySoundFromGroup(sound_bullet_eject, kGunMechanicVolume);
					return true;
				}
			}
		}
	}
	return false;
}

function ShouldOpenCylinder() : boolean {
	var num_firable_bullets = 0;
	for(var i=0; i<cylinder_capacity; ++i){
		if(cylinders[i].can_fire){
			++num_firable_bullets;
		}
	}
	return num_firable_bullets != cylinder_capacity;
}

function ShouldCloseCylinder() : boolean {
	var num_firable_bullets = 0;
	for(var i=0; i<cylinder_capacity; ++i){
		if(cylinders[i].can_fire){
			++num_firable_bullets;
		}
	}
	return num_firable_bullets == cylinder_capacity;
}

function ShouldExtractCasings() : boolean {
	var num_fired_bullets = 0;
	for(var i=0; i<cylinder_capacity; ++i){
		if(cylinders[i].object && !cylinders[i].can_fire){
			++num_fired_bullets;
		}
	}
	return num_fired_bullets > 0;
}

function ShouldInsertBullet() : boolean {
	var num_empty_chambers = 0;
	for(var i=0; i<cylinder_capacity; ++i){
		if(!cylinders[i].object){
			++num_empty_chambers;
		}
	}
	return num_empty_chambers > 0;
}

function HasSlide() : boolean {
	return has_slide;
}

function HasSafety() : boolean {
	return has_safety;
}

function HasHammer() : boolean {
	return has_hammer;
}

function HasAutoMod() : boolean {
	return has_auto_mod;
}

function ShouldToggleAutoMod() : boolean {
	return auto_mod_stage == AutoModStage.ENABLED;
}

function IsHammerCocked() : boolean {
	return hammer_cocked == 1.0;
}

function ShouldPullBackHammer() : boolean {
	return hammer_cocked != 1.0 && has_hammer && action_type == ActionType.SINGLE;
}

function SwingOutCylinder() : boolean {
	if(gun_type == GunType.REVOLVER && (yolk_stage == YolkStage.CLOSED || yolk_stage == YolkStage.CLOSING)){
		yolk_stage = YolkStage.OPENING;
		return true;
	} else {
		return false;
	}
}

function CloseCylinder() : boolean {
	if(gun_type == GunType.REVOLVER && (extractor_rod_stage == ExtractorRodStage.CLOSED && yolk_stage == YolkStage.OPEN || yolk_stage == YolkStage.OPENING)){
		yolk_stage = YolkStage.CLOSING;
		return true;
	} else {
		return false;
	}
}

function ExtractorRod() : boolean {
	if(gun_type == GunType.REVOLVER && (yolk_stage == YolkStage.OPEN && extractor_rod_stage == ExtractorRodStage.CLOSED || extractor_rod_stage == ExtractorRodStage.CLOSING)){
		extractor_rod_stage = ExtractorRodStage.OPENING;
		if(extractor_rod_amount < 1.0){
			extracted = false;
		}
		return true;
	} else {
		return false;
	}
}

function RotateCylinder(how_many : int) {
	/*while(how_many != 0){
		if(how_many > 0){
			active_cylinder = (active_cylinder + 1)%cylinder_capacity;
			--how_many;
		}
		if(how_many < 0){
			active_cylinder = (cylinder_capacity + active_cylinder - 1)%cylinder_capacity;
			++how_many;
		}
	}*/
	target_cylinder_offset += how_many * (Mathf.Max(1,Mathf.Abs(target_cylinder_offset)));
	target_cylinder_offset = Mathf.Max(-12, Mathf.Min(12, target_cylinder_offset));
}

function IsPressCheck() : boolean {
	if(gun_type != GunType.AUTOMATIC){
		return false;
	}
	return slide_amount <= kPressCheckPosition && 
		((slide_stage == SlideStage.PULLBACK && slide_lock) || (slide_stage == SlideStage.HOLD));
}

function Update () {
	if(gun_type == GunType.AUTOMATIC){
		if(magazine_instance_in_gun){
			var mag_pos = transform.FindChild("point_mag_inserted").position;
			var mag_rot = transform.rotation;
			mag_pos += (transform.FindChild("point_mag_to_insert").position - 
					    transform.FindChild("point_mag_inserted").position) * 
					   (1.0 - mag_seated);
		   magazine_instance_in_gun.transform.position = mag_pos;
		   magazine_instance_in_gun.transform.rotation = mag_rot;
		}
		
		if(mag_stage == MagStage.INSERTING){
			mag_seated += Time.deltaTime * 5.0;
			if(mag_seated >= 1.0){
				mag_seated = 1.0;
				mag_stage = MagStage.IN;
				if(slide_amount > 0.7){
					ChamberRoundFromMag();
				}
				recoil_transfer_y += Random.Range(-40.0,40.0);
				recoil_transfer_x += Random.Range(50.0,300.0);
				rotation_transfer_x += Random.Range(-0.4,0.4);
				rotation_transfer_y += Random.Range(0.0,1.0);
			}
		}
		if(mag_stage == MagStage.REMOVING){
			mag_seated -= Time.deltaTime * 5.0;
			if(mag_seated <= 0.0){
				mag_seated = 0.0;
				ready_to_remove_mag = true;
				mag_stage = MagStage.OUT;
			}
		}
	}
	
	if(has_safety){
		if(safety == Safety.OFF){
			safety_off = Mathf.Min(1.0, safety_off + Time.deltaTime * 10.0);
		} else if(safety == Safety.ON){
			safety_off = Mathf.Max(0.0, safety_off - Time.deltaTime * 10.0);
		}
	}
	
	if(has_auto_mod){
		if(auto_mod_stage == AutoModStage.ENABLED){
			auto_mod_amount = Mathf.Min(1.0, auto_mod_amount + Time.deltaTime * 10.0);
		} else if(auto_mod_stage == AutoModStage.DISABLED){
			auto_mod_amount = Mathf.Max(0.0, auto_mod_amount - Time.deltaTime * 10.0);
		}
	}
	
	if(thumb_on_hammer == Thumb.SLOW_LOWERING){
		hammer_cocked -= Time.deltaTime * 10.0f;
		if(hammer_cocked <= 0.0){
			hammer_cocked = 0.0;
			thumb_on_hammer = Thumb.OFF_HAMMER;
			PlaySoundFromGroup(sound_hammer_decock, kGunMechanicVolume);
			//PlaySoundFromGroup(sound_mag_eject_button, kGunMechanicVolume);
		}
	}

	if(has_slide){
		if(slide_stage == SlideStage.PULLBACK || slide_stage == SlideStage.HOLD){
			if(slide_stage == SlideStage.PULLBACK){
				slide_amount += Time.deltaTime * 10.0;
				if(slide_amount >= kSlideLockPosition && slide_lock){
					slide_amount = kSlideLockPosition;
					slide_stage = SlideStage.HOLD;
					PlaySoundFromGroup(sound_slide_back, kGunMechanicVolume);
				}
				if(slide_amount >= kPressCheckPosition && slide_lock){
					slide_amount = kPressCheckPosition;
					slide_stage = SlideStage.HOLD;
					slide_lock = false;
					PlaySoundFromGroup(sound_slide_back, kGunMechanicVolume);
				}
				if(slide_amount >= 1.0){
					PullSlideBack();
					slide_stage = SlideStage.HOLD;
					PlaySoundFromGroup(sound_slide_back, kGunMechanicVolume);
				}
			}
		}	
		
		transform.FindChild("slide").localPosition = 
			slide_rel_pos + 
			(transform.FindChild("point_slide_end").localPosition - 
			 transform.FindChild("point_slide_start").localPosition) * slide_amount;
	}
	
	if(has_hammer){
		var hammer = GetHammer();
		var point_hammer_cocked = transform.FindChild("point_hammer_cocked");
		hammer.localPosition = 
			Vector3.Lerp(hammer_rel_pos, point_hammer_cocked.localPosition, hammer_cocked);
		hammer.localRotation = 
			Quaternion.Slerp(hammer_rel_rot, point_hammer_cocked.localRotation, hammer_cocked);
	}
		
	if(has_safety){
		transform.FindChild("safety").localPosition = 
			Vector3.Lerp(safety_rel_pos, transform.FindChild("point_safety_off").localPosition, safety_off);
		transform.FindChild("safety").localRotation = 
			Quaternion.Slerp(safety_rel_rot, transform.FindChild("point_safety_off").localRotation, safety_off);
	}
	
	if(has_auto_mod){
		var slide = transform.FindChild("slide");
		slide.FindChild("auto mod toggle").localPosition = 
			Vector3.Lerp(auto_mod_rel_pos, slide.FindChild("point_auto_mod_enabled").localPosition, auto_mod_amount);
	}
			
	if(gun_type == GunType.AUTOMATIC){
		hammer_cocked = Mathf.Max(hammer_cocked, slide_amount);
		if(hammer_cocked != 1.0 && thumb_on_hammer == Thumb.OFF_HAMMER  && (pressure_on_trigger == PressureState.NONE || action_type == ActionType.SINGLE)){
			hammer_cocked = Mathf.Min(hammer_cocked, slide_amount);
		}
	} else {
		if(hammer_cocked != 1.0 && thumb_on_hammer == Thumb.OFF_HAMMER && (pressure_on_trigger == PressureState.NONE || action_type == ActionType.SINGLE)){
			hammer_cocked = 0.0;
		}
	}
	
	if(has_slide){
		if(slide_stage == SlideStage.NOTHING){
			var old_slide_amount = slide_amount;
			slide_amount = Mathf.Max(0.0, slide_amount - Time.deltaTime * kSlideLockSpeed);
			if(!slide_lock && slide_amount == 0.0 && old_slide_amount != 0.0){
				PlaySoundFromGroup(sound_slide_front, kGunMechanicVolume*1.5);
				if(active_round){
					active_round.transform.position = transform.FindChild("point_chambered_round").position;
					active_round.transform.rotation = transform.FindChild("point_chambered_round").rotation;
				}
			}
			if(slide_amount == 0.0 && active_round_state == RoundState.LOADING){
				MagScript().RemoveRound();
				active_round_state = RoundState.READY;
			}
			if(slide_lock && old_slide_amount >= kSlideLockPosition){
				slide_amount = Mathf.Max(kSlideLockPosition, slide_amount);
				if(old_slide_amount != kSlideLockPosition && slide_amount == kSlideLockPosition){
					PlaySoundFromGroup(sound_slide_front, kGunMechanicVolume);
				}
			}
		}
	}
	
	if(gun_type == GunType.REVOLVER){
		if(yolk_stage == YolkStage.CLOSED && hammer_cocked == 1.0){
			target_cylinder_offset = 0;
		}
		if(target_cylinder_offset != 0.0){
			var target_cylinder_rotation = ((active_cylinder + target_cylinder_offset) * 360.0 / cylinder_capacity);
			cylinder_rotation = Mathf.Lerp(target_cylinder_rotation, cylinder_rotation, Mathf.Pow(0.2,Time.deltaTime));
			if(cylinder_rotation > (active_cylinder + 0.5)  * 360.0 / cylinder_capacity){
				++active_cylinder;
				--target_cylinder_offset;
				if(yolk_stage == YolkStage.CLOSED){
					PlaySoundFromGroup(sound_cylinder_rotate, kGunMechanicVolume);
				}
			}
			if(cylinder_rotation < (active_cylinder - 0.5)  * 360.0 / cylinder_capacity){
				--active_cylinder;
				++target_cylinder_offset;
				if(yolk_stage == YolkStage.CLOSED){
					PlaySoundFromGroup(sound_cylinder_rotate, kGunMechanicVolume);
				}
			}
		}
		if(yolk_stage == YolkStage.CLOSING){
			yolk_open -= Time.deltaTime * 5.0;
			if(yolk_open <= 0.0){
				yolk_open = 0.0;
				yolk_stage = YolkStage.CLOSED;
				PlaySoundFromGroup(sound_cylinder_close, kGunMechanicVolume * 2.0);
				target_cylinder_offset = 0;
			}
		}
		if(yolk_stage == YolkStage.OPENING){
			yolk_open += Time.deltaTime * 5.0;
			if(yolk_open >= 1.0){
				yolk_open = 1.0;
				yolk_stage = YolkStage.OPEN;
				PlaySoundFromGroup(sound_cylinder_open, kGunMechanicVolume * 2.0);
			}
		}
		if(extractor_rod_stage == ExtractorRodStage.CLOSING){
			extractor_rod_amount -= Time.deltaTime * 10.0;
			if(extractor_rod_amount <= 0.0){
				extractor_rod_amount = 0.0;
				extractor_rod_stage = ExtractorRodStage.CLOSED;
				PlaySoundFromGroup(sound_extractor_rod_close, kGunMechanicVolume);
			}
			for(var i=0; i<cylinder_capacity; ++i){
				if(cylinders[i].object){
					cylinders[i].falling = false;
				}
			}
		}
		if(extractor_rod_stage == ExtractorRodStage.OPENING){
			var old_extractor_rod_amount = extractor_rod_amount;
			extractor_rod_amount += Time.deltaTime * 10.0;
			if(extractor_rod_amount >= 1.0){
				if(!extracted){
					for(i=0; i<cylinder_capacity; ++i){
						if(cylinders[i].object){
							if(Random.Range(0.0,3.0) > cylinders[i].seated){
								cylinders[i].falling = true;
								cylinders[i].seated -= Random.Range(0.0,0.5);
							} else {
								cylinders[i].falling = false;
							}
						}
					}
					extracted = true;
				}
				for(i=0; i<cylinder_capacity; ++i){
					if(cylinders[i].object && cylinders[i].falling){
						cylinders[i].seated -= Time.deltaTime * 5.0;
						if(cylinders[i].seated <= 0.0){
							var bullet = cylinders[i].object;
							bullet.AddComponent(Rigidbody);
							bullet.transform.parent = null;
							bullet.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
							bullet.rigidbody.velocity = velocity;
							bullet.rigidbody.angularVelocity = Vector3(Random.Range(-40.0,40.0),Random.Range(-40.0,40.0),Random.Range(-40.0,40.0));
							cylinders[i].object = null;
							cylinders[i].can_fire = false;
						}
					}
				}
				extractor_rod_amount = 1.0;
				extractor_rod_stage = ExtractorRodStage.OPEN;
				if(old_extractor_rod_amount < 1.0){
					PlaySoundFromGroup(sound_extractor_rod_open, kGunMechanicVolume);
				}
			}
		}
		if(extractor_rod_stage == ExtractorRodStage.OPENING || extractor_rod_stage == ExtractorRodStage.OPEN){
			extractor_rod_stage = ExtractorRodStage.CLOSING;
		}
			
		var yolk_pivot = transform.FindChild("yolk_pivot");
		yolk_pivot.localRotation = Quaternion.Slerp(yolk_pivot_rel_rot, 
			transform.FindChild("point_yolk_pivot_open").localRotation,
			yolk_open);
		var cylinder_assembly = yolk_pivot.FindChild("yolk").FindChild("cylinder_assembly");
		cylinder_assembly.localRotation.eulerAngles.z = cylinder_rotation;	
		var extractor_rod = cylinder_assembly.FindChild("extractor_rod");
		extractor_rod.localPosition = Vector3.Lerp(
			extractor_rod_rel_pos, 
			cylinder_assembly.FindChild("point_extractor_rod_extended").localPosition,
		    extractor_rod_amount);	
	
		for(i=0; i<cylinder_capacity; ++i){
			if(cylinders[i].object){
				var name = "point_chamber_"+(i+1);
				var bullet_chamber = extractor_rod.FindChild(name);
				cylinders[i].object.transform.position = bullet_chamber.position;
				cylinders[i].object.transform.rotation = bullet_chamber.rotation;
				cylinders[i].object.transform.localScale = transform.localScale;
			}
		}
	}
}

function FixedUpdate() {
	velocity = (transform.position - old_pos) / Time.deltaTime;
	old_pos = transform.position;
}