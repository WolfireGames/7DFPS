using UnityEngine;
using System.Collections.Generic;

public enum GunType {AUTOMATIC, REVOLVER};

public enum ActionType {DOUBLE, SINGLE, PUMP, BOLT};

public enum MagazineType {MAGAZINE, CYLINDER, INTERNAL};

public enum HandedType {ONE_HANDED, TWO_HANDED};

public enum PressureState {NONE, INITIAL, CONTINUING};

public enum RoundState {EMPTY, READY, FIRED, LOADING, JAMMED};

public enum SlideStage {NOTHING, PULLBACK, HOLD};

public enum BoltStage {CLOSED, CLOSING, OPEN, OPENING};

public enum Thumb{ON_HAMMER, OFF_HAMMER, SLOW_LOWERING};

public enum Safety{OFF, ON};

public enum MagStage {OUT, INSERTING, IN, REMOVING};

public enum AutoModStage {ENABLED, DISABLED};

public enum YolkStage {CLOSED, OPENING, OPEN, CLOSING};

public enum ExtractorRodStage {CLOSED, OPENING, OPEN, CLOSING};

[System.Serializable]
public class CylinderState {
	public GameObject game_object = null;
	public bool can_fire = false;
	public float seated = 0.0f;
	public bool falling = false;
};

public class GunScript:MonoBehaviour{
    public GunType gun_type;
    public ActionType action_type;
	public MagazineType magazineType;
	public HandedType handed = HandedType.ONE_HANDED;

	public bool lockSlideWhenEmpty = true;
	public bool slideSpringsAfterRelease = true;
	public bool hasSlideLock = true;
	public bool slideInteractionNeedsHand = true;

	public float projectileVelocity = 251f;
	public int projectilePerDischarge = 1;
	public float projectileInaccuracy = 0f;
    
    public List<AudioClip> sound_gunshot_bigroom;
    public List<AudioClip> sound_gunshot_smallroom;
    public List<AudioClip> sound_gunshot_open;
    public List<AudioClip> sound_mag_eject_button;
    public List<AudioClip> sound_mag_ejection;
    public List<AudioClip> sound_mag_insertion;
    public List<AudioClip> sound_slide_back;
    public List<AudioClip> sound_slide_front;
    public List<AudioClip> sound_safety;
    public List<AudioClip> sound_bullet_eject;
    public List<AudioClip> sound_cylinder_open;
    public List<AudioClip> sound_cylinder_close;
    public List<AudioClip> sound_extractor_rod_open;
    public List<AudioClip> sound_extractor_rod_close;
    public List<AudioClip> sound_cylinder_rotate;
    public List<AudioClip> sound_hammer_cock;
    public List<AudioClip> sound_hammer_decock;
    
    float kGunMechanicVolume = 0.2f;
    [HideInInspector] public bool add_head_recoil = false;
    [HideInInspector] public float recoil_transfer_x = 0f;
    [HideInInspector] public float recoil_transfer_y = 0f;
    [HideInInspector] public float rotation_transfer_x = 0f;
    [HideInInspector] public float rotation_transfer_y = 0f;
    
    [HideInInspector] public Vector3 old_pos;
    [HideInInspector] public Vector3 velocity;
    
    public GameObject magazine_obj;
    public GameObject bullet_obj;
    public GameObject muzzle_flash;
    public GameObject shell_casing;
    public GameObject casing_with_bullet;

    [HideInInspector] public bool ready_to_remove_mag = false;
    [HideInInspector] public PressureState pressure_on_trigger = PressureState.NONE;
    [HideInInspector] public float trigger_pressed = 0f;
    
    GameObject round_in_chamber;
    RoundState round_in_chamber_state = RoundState.READY;
    GameObject magazine_instance_in_gun;
    //float mag_offset = 0f;
    
    bool slide_pressure = false;
    Vector3 slide_rel_pos;
    float slide_amount = 0f;
	float bolt_amount = 0f;
    public bool slide_lock = false;

    SlideStage slide_stage = SlideStage.NOTHING;

    Thumb thumb_on_hammer = Thumb.OFF_HAMMER;
    Vector3 hammer_rel_pos;
    Quaternion hammer_rel_rot;
    float hammer_cocked = 1.0f;
    
    Vector3 safety_rel_pos;
    Quaternion safety_rel_rot;
    
    float safety_off = 1.0f;

    Safety safety = Safety.OFF;
	bool lifted = false;
	BoltStage boltStage = BoltStage.CLOSED;
    
    float kSlideLockPosition = 0.9f;
    public float kPressCheckPosition = 0.4f;
    float kSlideLockSpeed = 20.0f;

    MagStage mag_stage = MagStage.IN;
    float mag_seated = 1.0f;

    AutoModStage auto_mod_stage = AutoModStage.DISABLED;
    float auto_mod_amount = 0.0f;
    Vector3 auto_mod_rel_pos;
    bool fired_once_this_pull = false;
    
    bool has_slide = false;
    bool has_safety = false;
    bool has_hammer = false;
    bool has_auto_mod = false;
    
    Quaternion yolk_pivot_rel_rot;
    float yolk_open = 0.0f;

    YolkStage yolk_stage = YolkStage.CLOSED;
    float cylinder_rotation = 0.0f;
    //float cylinder_rotation_vel = 0.0f;
    int active_cylinder = 0;
    int target_cylinder_offset = 0;

    ExtractorRodStage extractor_rod_stage = ExtractorRodStage.CLOSED;
    float extractor_rod_amount = 0.0f;
    bool extracted = false;
    Vector3 extractor_rod_rel_pos;

    int cylinder_capacity = 6;
    public CylinderState[] cylinders;

    LevelCreatorScript level_creator = null;
    
    public bool IsAddingRounds() {
    	return yolk_stage == YolkStage.OPEN;
    }
    
    public bool IsEjectingRounds() {
    	return extractor_rod_stage != ExtractorRodStage.CLOSED;
    }
    
    public Transform GetHammer() {
    	Transform hammer = transform.Find("hammer");
    	if(hammer == null){
    		hammer = transform.Find("hammer_pivot");
    	}
    	return hammer;
    }
    
    public Transform GetHammerCocked() {
    	Transform hammer = transform.Find("point_hammer_cocked");
    	if(hammer == null){
    		hammer = transform.Find("hammer_pivot");
    	}
    	return hammer;
    }
    
	public void RemoveChildrenShadows(GameObject parent) {
		foreach(var renderer in parent.GetComponentsInChildren<Renderer>())
			renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
	}

    public void Start() {
        GameObject level_object = GameObject.Find("LevelObject");

        if(level_object != null) {
            level_creator = level_object.GetComponent<LevelCreatorScript>();
        }

        if(level_creator == null) {
            Debug.LogWarning("We're missing a LevelCreatorScript in GunScript, this might mean that some world-interactions don't work correctly.");
        }

    	if(transform.Find("slide") != null){
    		Transform slide = transform.Find("slide");
    		has_slide = true;
    		slide_rel_pos = slide.localPosition;
    		if(slide.Find("auto mod toggle") != null){
    			has_auto_mod = true;
    			auto_mod_rel_pos = slide.Find("auto mod toggle").localPosition;
    			if(UnityEngine.Random.Range(0,2) == 0){
    				auto_mod_amount = 1.0f;
    				auto_mod_stage = AutoModStage.ENABLED;
    			}
    		}
    	}
    	Transform hammer = GetHammer();
    	if(hammer != null){
    		has_hammer = true;
    		hammer_rel_pos = hammer.localPosition;
    		hammer_rel_rot = hammer.localRotation;
    	}
    	Transform yolk_pivot = transform.Find("yolk_pivot");
    	Transform extractor_rod = null;
        if(yolk_pivot != null){
    		yolk_pivot_rel_rot = yolk_pivot.localRotation;
    		Transform yolk = yolk_pivot.Find("yolk");
    		if(yolk != null){
    			Transform cylinder_assembly = yolk.Find("cylinder_assembly");
    			if(cylinder_assembly != null){
    				extractor_rod = cylinder_assembly.Find("extractor_rod");
    				if(extractor_rod != null){
    					extractor_rod_rel_pos = extractor_rod.localPosition;
    				}
    			}
    		}
    	}
    	
    	if(magazineType == MagazineType.MAGAZINE || magazineType == MagazineType.INTERNAL) {
    		if(magazineType == MagazineType.INTERNAL)
				magazine_instance_in_gun = transform.Find("internal_magazine").gameObject;
			else
				magazine_instance_in_gun = Instantiate(magazine_obj);
			
    		magazine_instance_in_gun.transform.parent = transform;
    	
			RemoveChildrenShadows(magazine_instance_in_gun);
    		
    		if(UnityEngine.Random.Range(0,2) == 0){
    			round_in_chamber = (GameObject)Instantiate(casing_with_bullet, transform.Find("point_chambered_round").position, transform.Find("point_chambered_round").rotation);
    			round_in_chamber.transform.parent = transform;
    			round_in_chamber.transform.localScale = Vector3.one;
    			
				RemoveChildrenShadows(round_in_chamber);
    		}
    		
    		if(hasSlideLock && UnityEngine.Random.Range(0,2) == 0){
    			if(MagScript() && MagScript().NumRounds() > 0) {
    				round_in_chamber_state = RoundState.LOADING;
    			}
    			slide_amount = kSlideLockPosition;
    			slide_lock = true;
    		}
    	}
    	
    	if(magazineType == MagazineType.CYLINDER) {
    		cylinders = new CylinderState[cylinder_capacity];
    		for(int i=0; i<cylinder_capacity; ++i){
    			cylinders[i] = new CylinderState();
    			if(UnityEngine.Random.Range(0,2) == 0){
    				continue;
    			}
    			string name = "point_chamber_"+(i+1);
                Transform chamber_transform = extractor_rod.Find(name);
    			cylinders[i].game_object = (GameObject)Instantiate(casing_with_bullet, chamber_transform.position, chamber_transform.rotation);
    			cylinders[i].game_object.transform.localScale = Vector3.one;
                cylinders[i].game_object.transform.parent = chamber_transform;
    			cylinders[i].can_fire = true;
    			cylinders[i].seated = Random.Range(0f, 0.5f);
				RemoveChildrenShadows(cylinders[i].game_object);
    		}
    	}
    	
    	if(UnityEngine.Random.Range(0,2) == 0 && has_hammer){
    		hammer_cocked = 0.0f;
    	}
    	
		var safetyTransform = transform.Find("safety");
    	if(safetyTransform != null){
    		has_safety = true;
    		safety_rel_pos = safetyTransform.localPosition;
    		safety_rel_rot = safetyTransform.localRotation;
    		if(UnityEngine.Random.Range(0,4) == 0){
    			safety_off = 0.0f;
    			safety = Safety.ON;
    			slide_amount = 0.0f;
    			slide_lock = false;
    		}
    	}
    }
    
    public mag_script MagScript() {
    	return magazine_instance_in_gun.GetComponent<mag_script>();
    }
    
    public bool ShouldPullSlide() {
    	if(magazineType == MagazineType.CYLINDER)
    		return false;
    
    	return (!round_in_chamber || round_in_chamber_state == RoundState.FIRED) && magazine_instance_in_gun && magazine_instance_in_gun.GetComponent<mag_script>().NumRounds() > 0;
    }
    
    public bool ShouldReleaseSlideLock() {
    	return hasSlideLock && round_in_chamber && slide_lock;
    }
    
    public bool ShouldEjectMag() {
    	if(magazineType != MagazineType.MAGAZINE)
    		return false;

    	return magazine_instance_in_gun && magazine_instance_in_gun.GetComponent<mag_script>().NumRounds() == 0;
    }
    
    public bool ChamberRoundFromMag() {
    	if(magazineType == MagazineType.CYLINDER){
    		return false;
    	}
    	if((magazine_instance_in_gun != null) && MagScript().NumRounds() > 0 && mag_stage == MagStage.IN){
    		if(round_in_chamber == null){
    			round_in_chamber = (GameObject)Instantiate(casing_with_bullet, transform.Find("point_load_round").position, transform.Find("point_load_round").rotation);
    			round_in_chamber.transform.parent = transform;
    			round_in_chamber.transform.localScale = Vector3.one;
    			round_in_chamber_state = RoundState.LOADING;

    			RemoveChildrenShadows(round_in_chamber);
    		}
    		return true;
    	}
    	return false;
    }
    
		public bool CanInteractWithSlide() {
		return handed == HandedType.ONE_HANDED || !slideInteractionNeedsHand || lifted;
	}

    public bool PullSlideBack() {
    	if(gun_type == GunType.REVOLVER || !CanInteractWithSlide()){
    		return false;
    	}
    	slide_amount = 1.0f;
    	if(slide_lock && mag_stage == MagStage.IN && (!magazine_instance_in_gun || MagScript().NumRounds() == 0)){
    		return false;
    	}
    	slide_lock = false;
    	if(round_in_chamber && (round_in_chamber_state == RoundState.FIRED || round_in_chamber_state == RoundState.READY)) {
    		round_in_chamber.AddComponent<Rigidbody>();
    		PlaySoundFromGroup(sound_bullet_eject, kGunMechanicVolume);
            if(level_creator != null){
                round_in_chamber.transform.parent = level_creator.GetPositionTileItemParent(round_in_chamber.transform.position);
            }
    		round_in_chamber.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
    		round_in_chamber.GetComponent<Rigidbody>().velocity = velocity;
    		round_in_chamber.GetComponent<Rigidbody>().velocity += transform.rotation * new Vector3(Random.Range(2f,4f),Random.Range(1f,2f),Random.Range(-1f,-3f));
    		round_in_chamber.GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(-40f,40f),Random.Range(-40f,40f),Random.Range(-40f,40f));
    		round_in_chamber = null;
    	}

    	if(!ChamberRoundFromMag() && mag_stage == MagStage.IN && lockSlideWhenEmpty) {
    		slide_lock = true;
    	}
		return true;
    }
    
    public bool ReleaseSlideLock() {
    	if(!hasSlideLock)
    		return false;

    	slide_lock = false;
		return true;
    }
    
    public Vector3 mix(Vector3 a,Vector3 b,float val){
    	return a + (b-a) * val;
    }
    
    public Quaternion mix(Quaternion a,Quaternion b,float val){
    	float angle = 0.0f;
    	Vector3 axis = new Vector3();
    	(Quaternion.Inverse(b)*a).ToAngleAxis(out angle, out axis);
    	if(angle > 180){
    		angle -= 360.0f;
    	}
    	if(angle < -180){
    		angle += 360.0f;
    	}
    	return a * Quaternion.AngleAxis(angle * -val, axis);
    }
    
    public void PlaySoundFromGroup(List<AudioClip> group,float volume){
    	if(group.Count == 0){
    		return;
    	}
    	int which_shot = Random.Range(0,group.Count);
    	GetComponent<AudioSource>().PlayOneShot(group[which_shot], volume * PlayerPrefs.GetFloat("sound_volume", 1.0f));
    }

    public void Discharge() {
		Instantiate(muzzle_flash, transform.Find("point_muzzleflash").position, transform.Find("point_muzzleflash").rotation);
		
		for(var i = 0; i < projectilePerDischarge; i++) {
			var bullet = Instantiate(bullet_obj, transform.Find("point_muzzle").position, transform.Find("point_muzzle").rotation);
			
			if(projectileInaccuracy != 0f) {
				var angle = Random.Range(0, 6.283f);
				var radius = Random.Range(0, projectileInaccuracy);
				bullet.transform.Rotate(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
			}
			bullet.GetComponent<BulletScript>().SetVelocity(bullet.transform.forward * projectileVelocity);
		}
		
		rotation_transfer_y += Random.Range(1f,2f);
		rotation_transfer_x += Random.Range(-1f,1f);
		recoil_transfer_x -= Random.Range(150f,300f);
		recoil_transfer_y += Random.Range(-200f,200f);
		add_head_recoil = true;
	}
    
    public bool ApplyPressureToTrigger() {
    	if(pressure_on_trigger == PressureState.NONE){
    		pressure_on_trigger = PressureState.INITIAL;
    		fired_once_this_pull = false;
    	} else {
    		pressure_on_trigger = PressureState.CONTINUING;
    	}

    	if(yolk_stage != YolkStage.CLOSED){
    		return false;
    	}

			if(action_type == ActionType.BOLT && IsBoltOpen()) {
				return false;
			}

    	if((pressure_on_trigger == PressureState.INITIAL || action_type == ActionType.DOUBLE) && !slide_lock && thumb_on_hammer == Thumb.OFF_HAMMER && hammer_cocked == 1f && safety_off == 1f && (auto_mod_stage == AutoModStage.ENABLED || !fired_once_this_pull)){
    		trigger_pressed = 1.0f;

            if(gun_type == GunType.AUTOMATIC && slide_amount == 0.0f){
    			hammer_cocked = 0.0f;
    			if(round_in_chamber && round_in_chamber_state == RoundState.READY){
    				fired_once_this_pull = true;
    				PlaySoundFromGroup(sound_gunshot_smallroom, 1.0f);
    				round_in_chamber_state = RoundState.FIRED;

    				GameObject.Destroy(round_in_chamber);
    				round_in_chamber = Instantiate(shell_casing, transform.Find("point_chambered_round").position, transform.rotation);
    				round_in_chamber.transform.parent = transform;
    				RemoveChildrenShadows(round_in_chamber);
    				
    				Discharge();
					
					if(action_type == ActionType.SINGLE || action_type == ActionType.DOUBLE) {
						PullSlideBack();
					}

    				return true;
    			} else {
    				PlaySoundFromGroup(sound_mag_eject_button, 0.5f);
    			}
    		} else if(gun_type == GunType.REVOLVER){
    			hammer_cocked = 0.0f;

    			int which_chamber = active_cylinder % cylinder_capacity;
    			if(which_chamber < 0){
    				which_chamber += cylinder_capacity;
    			}

    			GameObject round = cylinders[which_chamber].game_object;
    			if(round && cylinders[which_chamber].can_fire){
    				PlaySoundFromGroup(sound_gunshot_smallroom, 1.0f);
    				round_in_chamber_state = RoundState.FIRED;
    				cylinders[which_chamber].can_fire = false;
    				cylinders[which_chamber].seated += Random.Range(0.0f,0.5f);
    				cylinders[which_chamber].game_object = Instantiate(shell_casing, round.transform.position, round.transform.rotation);
                    cylinders[which_chamber].game_object.transform.parent = round.transform.parent;
    				GameObject.Destroy(round);

					RemoveChildrenShadows(cylinders[which_chamber].game_object);
    				Discharge();
    				return true;
    			} else {
    				PlaySoundFromGroup(sound_mag_eject_button, 0.5f);
    			}
    		}
    	}
    	
    	if(action_type == ActionType.DOUBLE && trigger_pressed < 1.0f && thumb_on_hammer == Thumb.OFF_HAMMER){
    		CockHammer();
    		CockHammer();
    	}
    	
    	return false;
    }
    
    public void ReleasePressureFromTrigger() {
    	pressure_on_trigger = PressureState.NONE;
    	trigger_pressed = 0.0f;
    }
    
    public bool MagEject() {
    	if(magazineType != MagazineType.MAGAZINE){
    		return false;
    	}
    	PlaySoundFromGroup(sound_mag_eject_button, kGunMechanicVolume);
    	if(round_in_chamber_state == RoundState.LOADING) {
    		GameObject.Destroy(round_in_chamber);
    		round_in_chamber_state = RoundState.EMPTY;
    	}
    	if(mag_stage != MagStage.OUT){
    		mag_stage = MagStage.REMOVING;
    		PlaySoundFromGroup(sound_mag_ejection, kGunMechanicVolume);
    		return true;
    	}
    	return false;
    }
    
    public bool PressureOnSlideLock() {
    	if(slide_amount > kPressCheckPosition && slide_stage == SlideStage.PULLBACK || (hasSlideLock && slide_amount > kSlideLockPosition))
    		slide_lock = true;
    	return true;
    }
    
    public void ReleasePressureOnSlideLock() {
    	if(slide_amount == kPressCheckPosition){
    		slide_lock = false;
    		if(slide_pressure){
    			slide_stage = SlideStage.PULLBACK;
    		}
    	} else if(slide_amount == 1.0f){
    		slide_lock = false;
    	}
    }
    
    public bool ToggleSafety() {
    	if(!has_safety){
    		return false;
    	}
    	if(safety == Safety.OFF){
    		if(slide_amount == 0.0f && hammer_cocked == 1.0f){
    			safety = Safety.ON;
    			PlaySoundFromGroup(sound_safety, kGunMechanicVolume);
    		}
    	} else if(safety == Safety.ON){
    		safety = Safety.OFF;
    		PlaySoundFromGroup(sound_safety, kGunMechanicVolume);
		}
		return true;
    }
    
    public bool ToggleAutoMod() {
    	if(!has_auto_mod){
    		return false;
    	}
    	PlaySoundFromGroup(sound_safety, kGunMechanicVolume);
    	if(auto_mod_stage == AutoModStage.DISABLED){
    		auto_mod_stage = AutoModStage.ENABLED;
    	} else if(auto_mod_stage == AutoModStage.ENABLED){
    		auto_mod_stage = AutoModStage.DISABLED;
			}
			return true;
    }
    
		public void ToggleLiftedStage(){
		if(handed == HandedType.ONE_HANDED)
			return;

		lifted = !lifted;
	}

	public bool IsLifted() {
		return lifted;
	}
    
    public bool PullBackSlide() {
    	if(gun_type == GunType.REVOLVER || !CanInteractWithSlide())
    		return false;

		if(action_type == ActionType.BOLT && !IsBoltOpen())
			return false;
    	
    	slide_pressure = true;
		if(!slideSpringsAfterRelease && slide_stage == SlideStage.HOLD) { // Push the slide back manually
			slide_stage = SlideStage.NOTHING;
			return true;
		}

		switch (action_type) {
			case ActionType.SINGLE:
			case ActionType.DOUBLE:
				if(slide_stage != SlideStage.PULLBACK && safety == Safety.OFF) {
					slide_stage = SlideStage.PULLBACK;
					slide_lock = false;
				}
				return true;
			case ActionType.BOLT:
			case ActionType.PUMP:
				if(slide_stage != SlideStage.PULLBACK)
					slide_stage = SlideStage.PULLBACK;
				return true;
			default:
				return true;
		}
    }
    
	public bool ToggleBolt() {
		if(action_type != ActionType.BOLT || !CanInteractWithSlide())
			return false;

		if(IsSlidePulledBack())
			return false;

		switch (boltStage) {
			case BoltStage.OPENING:
			case BoltStage.OPEN:
				boltStage = BoltStage.CLOSING;
				return true;
			case BoltStage.CLOSING:
			case BoltStage.CLOSED:
				boltStage = BoltStage.OPENING;
				return true;
		}
		return false;
	}

	public bool IsBoltOpen() {
		if(action_type != ActionType.BOLT)
			return false;

		return boltStage == BoltStage.OPEN;
	}

    public bool ReleaseSlide() {
    	if(gun_type == GunType.REVOLVER){
    		return false;
    	}

		if(slideSpringsAfterRelease || slide_amount == kPressCheckPosition)
    		slide_stage = SlideStage.NOTHING;

		slide_pressure = false;
		return true;
    }
    
    public void CockHammer(){
    	float old_hammer_cocked = hammer_cocked;
    	hammer_cocked = Mathf.Min(1.0f, hammer_cocked + Time.deltaTime * 10.0f);
    	if(hammer_cocked == 1.0f && old_hammer_cocked != 1.0f){
    		if(thumb_on_hammer == Thumb.ON_HAMMER){
    			PlaySoundFromGroup(sound_hammer_cock, kGunMechanicVolume);
    		}
    		++active_cylinder;
    		cylinder_rotation = active_cylinder * 360.0f / cylinder_capacity;
    	}
    	if(hammer_cocked < 1.0f){
    		cylinder_rotation = (active_cylinder + hammer_cocked) * 360.0f / cylinder_capacity;
    		target_cylinder_offset = (int)0.0f;
    	}
    }
    
    public void PressureOnHammer() {
    	if(!has_hammer){
    		return;
    	}
    	thumb_on_hammer = Thumb.ON_HAMMER;
    	if(gun_type == GunType.REVOLVER && yolk_stage != YolkStage.CLOSED){
    		return;
    	}
    	CockHammer();
    }
    
    public void ReleaseHammer() {
    	if(!has_hammer){
    		return;
    	}
    	if((pressure_on_trigger != PressureState.NONE && safety_off == 1.0f) || hammer_cocked != 1.0f){
    		thumb_on_hammer = Thumb.SLOW_LOWERING;
    		trigger_pressed = 1.0f;
    	} else {
    		thumb_on_hammer = Thumb.OFF_HAMMER;
    	}
    }
    
    public bool IsSafetyOn() {
    	return safety == Safety.ON;
    }
    
    public bool IsSlideLocked() {
    	if(!hasSlideLock)
    		return false;

    	return slide_lock;
    }
    
    public bool IsSlidePulledBack() {
    	if(gun_type == GunType.REVOLVER)
    		return false;

    	return slide_stage != SlideStage.NOTHING;
    }
    
    public GameObject RemoveMag() {
    	if(magazineType != MagazineType.MAGAZINE){
    		return null;
    	}
    	GameObject mag = magazine_instance_in_gun;
    	magazine_instance_in_gun = null;
    	mag.transform.parent = null;
    	ready_to_remove_mag = false;
    	return mag;
    }
    
    public bool IsThereAMagInGun() {
    	if(magazineType == MagazineType.CYLINDER)
    		return false;

    	return magazine_instance_in_gun;
    }
    
    public bool IsMagCurrentlyEjecting() {
    	if(magazineType != MagazineType.MAGAZINE){
    		return false;
    	}
    	return mag_stage == MagStage.REMOVING;
    }
    
    public void InsertMag(GameObject mag) {
    	if(magazineType != MagazineType.MAGAZINE){
    		return;
    	}
    	if(magazine_instance_in_gun != null){
    		return;
    	}
    	magazine_instance_in_gun = mag;
    	mag.transform.parent = transform;
    	mag_stage = MagStage.INSERTING;
    	PlaySoundFromGroup(sound_mag_insertion, kGunMechanicVolume);
    	mag_seated = 0.0f;
    }
    
    public bool IsCylinderOpen(){
    	return yolk_stage == YolkStage.OPEN || yolk_stage == YolkStage.OPENING;
    }
    
    public bool AddRound() {
    	if(magazineType == MagazineType.MAGAZINE)
    		return false;

		if(action_type == ActionType.BOLT && (!IsBoltOpen() || !IsSlidePulledBack()))
			return false;

		if(magazineType == MagazineType.INTERNAL) {
			if(IsLifted())
				return magazine_instance_in_gun.GetComponent<mag_script>().AddRound();
			else
				return false;
		}

		if(yolk_stage != YolkStage.OPEN) // Cylinder closed
			return false;

    	int best_chamber = -1;
    	int next_shot = active_cylinder;
    	if(!IsHammerCocked()){
    		next_shot = (next_shot + 1) % cylinder_capacity;
    	}
    	for(int i=0; i<cylinder_capacity; ++i){
    		int check = (next_shot + i)%cylinder_capacity;
    		if(check < 0){
    			check += cylinder_capacity;
    		}
    		if(!cylinders[check].game_object){
    			best_chamber = check;
    			break;
    		}
    	}
    	if(best_chamber == -1){
    		return false;
    	}
    	Transform yolk_pivot = transform.Find("yolk_pivot");
    	if(yolk_pivot != null){
    		Transform yolk = yolk_pivot.Find("yolk");
    		if(yolk != null){
    			Transform cylinder_assembly = yolk.Find("cylinder_assembly");
    			if(cylinder_assembly != null){
    				Transform extractor_rod = cylinder_assembly.Find("extractor_rod");
    				if(extractor_rod != null){
    					string name = "point_chamber_"+(best_chamber+1);
                        Transform chamber_transform = extractor_rod.Find(name);
    					cylinders[best_chamber].game_object = (GameObject)Instantiate(casing_with_bullet, chamber_transform.position, chamber_transform.rotation);
    					cylinders[best_chamber].game_object.transform.localScale = Vector3.one;
                        cylinders[best_chamber].game_object.transform.parent = chamber_transform;
    					cylinders[best_chamber].can_fire = true;
    					cylinders[best_chamber].seated = Random.Range(0f,1f);

						RemoveChildrenShadows(cylinders[best_chamber].game_object);
    					PlaySoundFromGroup(sound_bullet_eject, kGunMechanicVolume);
    					return true;
    				}
    			}
    		}
    	}
    	return false;
    }
    
    public bool ShouldOpenCylinder() {
    	int num_firable_bullets = 0;
    	for(int i=0; i<cylinder_capacity; ++i){
    		if(cylinders[i].can_fire){
    			++num_firable_bullets;
    		}
    	}
    	return num_firable_bullets != cylinder_capacity;
    }
    
    public bool ShouldCloseCylinder() {
    	int num_firable_bullets = 0;
    	for(int i=0; i<cylinder_capacity; ++i){
    		if(cylinders[i].can_fire){
    			++num_firable_bullets;
    		}
    	}
    	return num_firable_bullets == cylinder_capacity;
    }
    
    public bool ShouldExtractCasings() {
    	int num_fired_bullets = 0;
    	for(int i=0; i<cylinder_capacity; ++i){
    		if(cylinders[i].game_object && !cylinders[i].can_fire){
    			++num_fired_bullets;
    		}
    	}
    	return num_fired_bullets > 0;
    }
    
    public bool ShouldInsertBullet() {
		switch(magazineType) {
			case MagazineType.CYLINDER:
				var num_empty_chambers = 0;
				for(var i=0; i<cylinder_capacity; ++i)
					if(!cylinders[i].game_object)
						++num_empty_chambers;
				return num_empty_chambers > 0;
			case MagazineType.INTERNAL: 
				return !magazine_instance_in_gun.GetComponent<mag_script>().IsFull();
			default: 
				return false;
		}
    }
    
    public bool HasSlide() {
    	return has_slide;
    }
    
    public bool HasSafety() {
    	return has_safety;
    }
    
    public bool HasHammer() {
    	return has_hammer;
    }
    
    public bool HasAutoMod() {
    	return has_auto_mod;
    }
    
    public bool ShouldToggleAutoMod() {
    	return auto_mod_stage == AutoModStage.ENABLED;
    }
    
    public bool IsHammerCocked() {
    	return hammer_cocked == 1.0f;
    }
    
    public bool ShouldPullBackHammer() {
    	return hammer_cocked != 1.0f && has_hammer && action_type == ActionType.SINGLE;
    }
    
    public bool SwingOutCylinder() {
    	if(magazineType == MagazineType.CYLINDER && (yolk_stage == YolkStage.CLOSED || yolk_stage == YolkStage.CLOSING)) {
    		yolk_stage = YolkStage.OPENING;
    		return true;
    	}

    	return false;
    }
    
    public bool CloseCylinder() {
    	if(magazineType == MagazineType.CYLINDER && (extractor_rod_stage == ExtractorRodStage.CLOSED && yolk_stage == YolkStage.OPEN || yolk_stage == YolkStage.OPENING)) {
    		yolk_stage = YolkStage.CLOSING;
    		return true;
    	}

    	return false;
    }
    
    public bool ExtractorRod() {
    	if(magazineType == MagazineType.CYLINDER && (yolk_stage == YolkStage.OPEN && extractor_rod_stage == ExtractorRodStage.CLOSED || extractor_rod_stage == ExtractorRodStage.CLOSING)) {
    		extractor_rod_stage = ExtractorRodStage.OPENING;

    		if(extractor_rod_amount < 1.0f){
    			extracted = false;
    		}
    		return true;
    	}

		return false;
    }
    
    public void RotateCylinder(int how_many) {
    	target_cylinder_offset += how_many * (Mathf.Max(1,Mathf.Abs(target_cylinder_offset)));
    	target_cylinder_offset = Mathf.Max(-12, Mathf.Min(12, target_cylinder_offset));
    }
    
    public bool IsPressCheck() {
    	if(gun_type == GunType.REVOLVER) {
    		return false;
    	}
    	return slide_amount <= kPressCheckPosition && 
    		((slide_stage == SlideStage.PULLBACK && slide_lock) || (slide_stage == SlideStage.HOLD));
    }
    
    public void Update() {
    	if(magazineType != MagazineType.CYLINDER) {
    		if(magazine_instance_in_gun && magazineType != MagazineType.INTERNAL) {
    			Vector3 mag_pos = transform.Find("point_mag_inserted").position;
    			Quaternion mag_rot = transform.rotation;
    			float mag_seated_display = mag_seated;
    			mag_pos += (transform.Find("point_mag_to_insert").position - 
    					    transform.Find("point_mag_inserted").position) * 
    					   (1.0f - mag_seated_display);
    		   magazine_instance_in_gun.transform.position = mag_pos;
    		   magazine_instance_in_gun.transform.rotation = mag_rot;
    		}
    		
    		if(mag_stage == MagStage.INSERTING) {
    			mag_seated += Time.deltaTime * 5.0f;
    			if(mag_seated >= 1.0f){
    				mag_seated = 1.0f;
    				mag_stage = MagStage.IN;
    				if(slide_amount > 0.7f) {
    					ChamberRoundFromMag();
    				}
    				recoil_transfer_y += Random.Range(-40.0f,40.0f);
    				recoil_transfer_x += Random.Range(50.0f,300.0f);
    				rotation_transfer_x += Random.Range(-0.4f,0.4f);
    				rotation_transfer_y += Random.Range(0.0f,1.0f);
    			}
    		}

    		if(mag_stage == MagStage.REMOVING) {
    			mag_seated -= Time.deltaTime * 5.0f;
    			if(mag_seated <= 0.0f) {
    				mag_seated = 0.0f;
					if(magazineType != MagazineType.INTERNAL) {
    					ready_to_remove_mag = true;
					}
    				mag_stage = MagStage.OUT;
    			}
    		}
    	}
    	
    	if(has_safety) {
    		if(safety == Safety.OFF) {
    			safety_off = Mathf.Min(1.0f, safety_off + Time.deltaTime * 10.0f);
    		} else if(safety == Safety.ON) {
    			safety_off = Mathf.Max(0.0f, safety_off - Time.deltaTime * 10.0f);
    		}
    	}
    	
    	if(has_auto_mod) {
    		if(auto_mod_stage == AutoModStage.ENABLED) {
    			auto_mod_amount = Mathf.Min(1.0f, auto_mod_amount + Time.deltaTime * 10.0f);
    		} else if(auto_mod_stage == AutoModStage.DISABLED) {
    			auto_mod_amount = Mathf.Max(0.0f, auto_mod_amount - Time.deltaTime * 10.0f);
    		}
    	}
    	
    	if(thumb_on_hammer == Thumb.SLOW_LOWERING) {
    		hammer_cocked -= Time.deltaTime * 10.0f;
    		if(hammer_cocked <= 0.0f) {
    			hammer_cocked = 0.0f;
    			thumb_on_hammer = Thumb.OFF_HAMMER;
    			PlaySoundFromGroup(sound_hammer_decock, kGunMechanicVolume);
    			//PlaySoundFromGroup(sound_mag_eject_button, kGunMechanicVolume);
    		}
    	}
    
    	if(has_slide) {
    		if(slide_stage == SlideStage.PULLBACK) {
				slide_amount += Time.deltaTime * 10f;

				if(slide_amount >= kSlideLockPosition && slide_lock) {
					slide_amount = kSlideLockPosition;
					slide_stage = SlideStage.HOLD;
					PlaySoundFromGroup(sound_slide_back, kGunMechanicVolume);
				}

				if(slide_amount >= kPressCheckPosition && slide_lock) {
					slide_amount = kPressCheckPosition;
					slide_stage = SlideStage.HOLD;
					slide_lock = false;
					PlaySoundFromGroup(sound_slide_back, kGunMechanicVolume);
				}

				if(slide_amount >= 1f) {
					PullSlideBack();
					slide_amount = 1f;
					slide_stage = SlideStage.HOLD;
					PlaySoundFromGroup(sound_slide_back, kGunMechanicVolume);
				}
    		}

			if(boltStage == BoltStage.OPENING) {
				bolt_amount += Time.deltaTime * 10f;
				if(bolt_amount >= 1f) {
					boltStage = BoltStage.OPEN;
					PlaySoundFromGroup(sound_cylinder_open, kGunMechanicVolume);
				}
			} else if(boltStage == BoltStage.CLOSING) {
				if(!round_in_chamber)
					ChamberRoundFromMag();

				bolt_amount -= Time.deltaTime * 10f;
				if(bolt_amount <= 0f) {
					boltStage = BoltStage.CLOSED;
					PlaySoundFromGroup(sound_cylinder_close, kGunMechanicVolume);
				}
			}
    		
    		float slide_amount_display = slide_amount;
    		transform.Find("slide").localPosition = 
    			slide_rel_pos + 
    			(transform.Find("point_slide_end").localPosition - 
    			 transform.Find("point_slide_start").localPosition) * slide_amount_display;

				if(action_type == ActionType.BOLT) //Update Bolt Rotation
				transform.Find("slide").localRotation = Quaternion.Lerp(transform.Find("point_slide_start").localRotation, transform.Find("point_slide_end").localRotation, bolt_amount);
    	}
    	
    	if(has_hammer) {
    		Transform hammer = GetHammer();
    		Transform point_hammer_cocked = transform.Find("point_hammer_cocked");
    		float hammer_cocked_display = hammer_cocked;
    		hammer.localPosition = Vector3.Lerp(hammer_rel_pos, point_hammer_cocked.localPosition, hammer_cocked_display);
    		hammer.localRotation = Quaternion.Slerp(hammer_rel_rot, point_hammer_cocked.localRotation, hammer_cocked_display);
    	}
    		
    	if(has_safety) {
    		float safety_off_display = safety_off;
    		transform.Find("safety").localPosition = Vector3.Lerp(safety_rel_pos, transform.Find("point_safety_off").localPosition, safety_off_display);
    		transform.Find("safety").localRotation = Quaternion.Slerp(safety_rel_rot, transform.Find("point_safety_off").localRotation, safety_off_display);
    	}
    	
    	if(has_auto_mod) {
    		float auto_mod_amount_display = auto_mod_amount;
    		Transform slide = transform.Find("slide");
    		slide.Find("auto mod toggle").localPosition = Vector3.Lerp(auto_mod_rel_pos, slide.Find("point_auto_mod_enabled").localPosition, auto_mod_amount_display);
    	}
    			
    	if(gun_type != GunType.REVOLVER) {
    		hammer_cocked = Mathf.Max(hammer_cocked, slide_amount);
    		if(hammer_cocked != 1f && thumb_on_hammer == Thumb.OFF_HAMMER  && (pressure_on_trigger == PressureState.NONE || action_type == ActionType.SINGLE)){
    			hammer_cocked = Mathf.Min(hammer_cocked, slide_amount);
    		}
    	} else {
    		if(hammer_cocked != 1.0f && thumb_on_hammer == Thumb.OFF_HAMMER && (pressure_on_trigger == PressureState.NONE || action_type == ActionType.SINGLE)){
    			hammer_cocked = 0.0f;
    		}
    	}
    	
    	if(has_slide) {
    		if(slide_stage == SlideStage.NOTHING) {
    			float old_slide_amount = slide_amount;
    			slide_amount = Mathf.Max(0.0f, slide_amount - Time.deltaTime * kSlideLockSpeed);

    			if(!slide_lock && slide_amount == 0.0f && old_slide_amount != 0.0f) {
    				PlaySoundFromGroup(sound_slide_front, kGunMechanicVolume*1.5f);
    				if(round_in_chamber) {
    					round_in_chamber.transform.position = transform.Find("point_chambered_round").position;
    					round_in_chamber.transform.rotation = transform.Find("point_chambered_round").rotation;
    				}
    			}
    			if(slide_amount == 0.0f && round_in_chamber_state == RoundState.LOADING){
    				MagScript().RemoveRound();
    				round_in_chamber_state = RoundState.READY;
    			}

    			if(slide_lock && old_slide_amount >= kSlideLockPosition) {
    				slide_amount = Mathf.Max(kSlideLockPosition, slide_amount);
    				if(old_slide_amount != kSlideLockPosition && slide_amount == kSlideLockPosition) {
    					PlaySoundFromGroup(sound_slide_front, kGunMechanicVolume);
    				}
    			}
    		}
    	}
    	
    	if(gun_type == GunType.REVOLVER) {
    		if(yolk_stage == YolkStage.CLOSED && hammer_cocked == 1.0f) {
    			target_cylinder_offset = 0;
    		}

    		if(target_cylinder_offset != 0.0f) {
    			float target_cylinder_rotation = ((active_cylinder + target_cylinder_offset) * 360.0f / cylinder_capacity);
    			cylinder_rotation = Mathf.Lerp(target_cylinder_rotation, cylinder_rotation, Mathf.Pow(0.2f, Time.deltaTime));
    			if(cylinder_rotation > (active_cylinder + 0.5f)  * 360.0f / cylinder_capacity) {
    				++active_cylinder;
    				--target_cylinder_offset;
    				if(yolk_stage == YolkStage.CLOSED) {
    					PlaySoundFromGroup(sound_cylinder_rotate, kGunMechanicVolume);
    				}
    			}
    			if(cylinder_rotation < (active_cylinder - 0.5f)  * 360.0f / cylinder_capacity) {
    				--active_cylinder;
    				++target_cylinder_offset;
    				if(yolk_stage == YolkStage.CLOSED) {
    					PlaySoundFromGroup(sound_cylinder_rotate, kGunMechanicVolume);
    				}
    			}
    		}

    		if(yolk_stage == YolkStage.CLOSING) {
    			yolk_open -= Time.deltaTime * 5.0f;
    			if(yolk_open <= 0.0f) {
    				yolk_open = 0.0f;
    				yolk_stage = YolkStage.CLOSED;
    				PlaySoundFromGroup(sound_cylinder_close, kGunMechanicVolume * 2.0f);
    				target_cylinder_offset = 0;
    			}
    		}

    		if(yolk_stage == YolkStage.OPENING) {
    			yolk_open += Time.deltaTime * 5.0f;
    			if(yolk_open >= 1.0f) {
    				yolk_open = 1.0f;
    				yolk_stage = YolkStage.OPEN;
    				PlaySoundFromGroup(sound_cylinder_open, kGunMechanicVolume * 2.0f);
    			}
    		}

    		if(extractor_rod_stage == ExtractorRodStage.CLOSING) {
    			extractor_rod_amount -= Time.deltaTime * 10.0f;
    			if(extractor_rod_amount <= 0.0f) {
    				extractor_rod_amount = 0.0f;
    				extractor_rod_stage = ExtractorRodStage.CLOSED;
    				PlaySoundFromGroup(sound_extractor_rod_close, kGunMechanicVolume);
    			}

    			for(int i=0; i<cylinder_capacity; ++i) {
    				if(cylinders[i].game_object) {
    					cylinders[i].falling = false;
    				}
    			}
    		}

    		if(extractor_rod_stage == ExtractorRodStage.OPENING) {
    			float old_extractor_rod_amount = extractor_rod_amount;
    			extractor_rod_amount += Time.deltaTime * 10.0f;
    			if(extractor_rod_amount >= 1.0f) {
    				if(!extracted) {
    					for(int i=0; i<cylinder_capacity; ++i) {
    						if(cylinders[i].game_object) {
    							if(Random.Range(0.0f,3.0f) > cylinders[i].seated) {
    								cylinders[i].falling = true;
    								cylinders[i].seated -= Random.Range(0.0f,0.5f);
    							} else {
    								cylinders[i].falling = false;
    							}
    						}
    					}
    					extracted = true;
    				}

    				for(int i=0; i<cylinder_capacity; ++i) {
    					if(cylinders[i].game_object && cylinders[i].falling) {
    						cylinders[i].seated -= Time.deltaTime * 5.0f;
    						if(cylinders[i].seated <= 0.0f) {
    							GameObject bullet = cylinders[i].game_object;
    							bullet.AddComponent<Rigidbody>();
    							bullet.transform.parent = null;
    							bullet.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
    							bullet.GetComponent<Rigidbody>().velocity = velocity;
    							bullet.GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(-40.0f, 40.0f), Random.Range(-40.0f, 40.0f), Random.Range(-40.0f, 40.0f));

                                if(level_creator != null) {
                                    bullet.transform.parent = level_creator.GetPositionTileItemParent(bullet.transform.position);
                                }

    							cylinders[i].game_object = null;
    							cylinders[i].can_fire = false;
    						}
    					}
    				}

    				extractor_rod_amount = 1.0f;
    				extractor_rod_stage = ExtractorRodStage.OPEN;
    				if(old_extractor_rod_amount < 1.0f) {
    					PlaySoundFromGroup(sound_extractor_rod_open, kGunMechanicVolume);
    				}
    			}
    		}
    		if(extractor_rod_stage == ExtractorRodStage.OPENING || extractor_rod_stage == ExtractorRodStage.OPEN) {
    			extractor_rod_stage = ExtractorRodStage.CLOSING;
    		}
    			
    		float yolk_open_display = yolk_open;
    		float extractor_rod_amount_display = extractor_rod_amount;
    		Transform yolk_pivot = transform.Find("yolk_pivot");
    		yolk_pivot.localRotation = Quaternion.Slerp(yolk_pivot_rel_rot, transform.Find("point_yolk_pivot_open").localRotation, yolk_open_display);
    		Transform cylinder_assembly = yolk_pivot.Find("yolk").Find("cylinder_assembly");
    		var tmp_cs1 = cylinder_assembly.localRotation;
            var tmp_cs2 = tmp_cs1.eulerAngles;
            tmp_cs2.z = cylinder_rotation;
            tmp_cs1.eulerAngles = tmp_cs2;
            cylinder_assembly.localRotation = tmp_cs1;	
    		Transform extractor_rod = cylinder_assembly.Find("extractor_rod");
    		extractor_rod.localPosition = Vector3.Lerp(extractor_rod_rel_pos, cylinder_assembly.Find("point_extractor_rod_extended").localPosition,extractor_rod_amount_display);	
    	}
    }
    
    public void FixedUpdate() {
    	velocity = (transform.position - old_pos) / Time.deltaTime;
    	old_pos = transform.position;
    }
}

