using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class CharacterInput {
	public bool GetButtonDown(string input_str) {
		return Input.GetButtonDown(input_str);
	}
	public bool GetButton(string input_str) {
		return Input.GetButton(input_str);
	}
	public bool GetButtonUp(string input_str) {
		return Input.GetButtonUp(input_str);
	}
	public float GetAxis(string input_str) {
		return Input.GetAxis(input_str);
	}
};

public enum GunTilt {NONE, LEFT, CENTER, RIGHT};

public enum HandMagStage {HOLD, HOLD_TO_INSERT, EMPTY};

public enum WeaponSlotType {GUN, MAGAZINE, FLASHLIGHT, EMPTY, EMPTYING};

[System.Serializable]
public class WeaponSlot {
	public GameObject obj = null;
	public WeaponSlotType type = WeaponSlotType.EMPTY;
	public Vector3 start_pos = new Vector3(0.0f,0.0f,0.0f);
	public Quaternion start_rot = Quaternion.identity;
	public Spring spring = new Spring(1.0f,1.0f,100.0f,0.000001f);
};

[System.Serializable]
public class CharacterMotorMovement {
	// The maximum horizontal speed when moving
	public float maxForwardSpeed = 10.0f;
	public float maxSidewaysSpeed = 10.0f;
	public float maxBackwardsSpeed = 10.0f;
	
	// Curve for multiplying speed based on slope (negative = downwards)
	public AnimationCurve slopeSpeedMultiplier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
	
	// How fast does the character change speeds?  Higher is faster.
	public float maxGroundAcceleration = 30.0f;
	public float maxAirAcceleration = 20.0f;

	// The gravity for the character
	public float gravity = 10.0f;
	public float maxFallSpeed = 20.0f;
	
	// For the next variables, @System.NonSerialized tells Unity to not serialize the variable or show it in the inspector view.
	// Very handy for organization!

	// The last collision flags returned from controller.Move
	[System.NonSerialized]
	public CollisionFlags collisionFlags; 

	// We will keep track of the character's current velocity,
	[System.NonSerialized]
	public Vector3 velocity;
	
	// This keeps track of our current velocity while we're not grounded
	[System.NonSerialized]
	public Vector3 frameVelocity = Vector3.zero;
	
	[System.NonSerialized]
	public Vector3 hitPoint = Vector3.zero;
	
	[System.NonSerialized]
	public Vector3 lastHitPoint = new Vector3(Mathf.Infinity, 0.0f, 0.0f);
}

public enum MovementTransferOnJump {
	None, // The jump is not affected by velocity of floor at all.
	InitTransfer, // Jump gets its initial velocity from the floor, then gradualy comes to a stop.
	PermaTransfer, // Jump gets its initial velocity from the floor, and keeps that velocity until landing.
	PermaLocked // Jump is relative to the movement of the last touched floor and will move together with that floor.
}

// We will contain all the jumping related variables in one helper class for clarity.
[System.Serializable]
public class CharacterMotorJumping {
	// Can the character jump?
	public bool enabled = true;

	// How high do we jump when pressing jump and letting go immediately
	public float baseHeight = 1.0f;
	
	// We add extraHeight units (meters) on top when holding the button down longer while jumping
	public float extraHeight = 4.1f;
	
	// How much does the character jump out perpendicular to the surface on walkable surfaces?
	// 0 means a fully vertical jump and 1 means fully perpendicular.
	public float perpAmount = 0.0f;
	
	// How much does the character jump out perpendicular to the surface on too steep surfaces?
	// 0 means a fully vertical jump and 1 means fully perpendicular.
	public float steepPerpAmount = 0.5f;
	
	// For the next variables, @System.NonSerialized tells Unity to not serialize the variable or show it in the inspector view.
	// Very handy for organization!

	// Are we jumping? (Initiated with jump button and not grounded yet)
	// To see if we are just in the air (initiated by jumping OR falling) see the grounded variable.
	[System.NonSerialized]
	public bool jumping = false;
	
	[System.NonSerialized]
	public bool holdingJumpButton = false;

	// the time we jumped at (Used to determine for how long to apply extra jump power after jumping.)
	[System.NonSerialized]
	public float lastStartTime = 0.0f;
	
	[System.NonSerialized]
	public float lastButtonDownTime = -100.0f;
	
	[System.NonSerialized]
	public Vector3 jumpDir = Vector3.up;
}

[System.Serializable]
public class CharacterMotorMovingPlatform {
	public bool enabled = true;
	
	public MovementTransferOnJump movementTransfer = MovementTransferOnJump.PermaTransfer;
	
	[System.NonSerialized]
	public Transform hitPlatform;
	
	[System.NonSerialized]
	public Transform activePlatform;
	
	[System.NonSerialized]
	public Vector3 activeLocalPoint;
	
	[System.NonSerialized]
	public Vector3 activeGlobalPoint;
	
	[System.NonSerialized]
	public Quaternion activeLocalRotation;
	
	[System.NonSerialized]
	public Quaternion activeGlobalRotation;
	
	[System.NonSerialized]
	public Matrix4x4 lastMatrix;
	
	[System.NonSerialized]
	public Vector3 platformVelocity;
	
	[System.NonSerialized]
	public bool newPlatform;
}

[System.Serializable]
public class CharacterMotorSliding {
	// Does the character slide on too steep surfaces?
	public bool enabled = true;
	
	// How fast does the character slide on steep surfaces?
	public float slidingSpeed = 15.0f;
	
	// How much can the player control the sliding direction?
	// If the value is 0.5 the player can slide sideways with half the speed of the downwards sliding speed.
	public float sidewaysControl = 1.0f;
	
	// How much can the player influence the sliding speed?
	// If the value is 0.5 the player can speed the sliding up to 150% or slow it down to 50%.
	public float speedControl = 0.4f;
}

[RequireComponent(typeof(CharacterController))]

public class AimScript:MonoBehaviour{
    
    // Networking
    public bool main_client_control = true;
    
    // Assets and prefabs
    GameObject magazine_obj;
    GameObject gun_obj;
    GameObject casing_with_bullet;
    public Texture texture_death_screen;
    
    public List<AudioClip> sound_bullet_grab;
    public List<AudioClip> sound_body_fall;
    public List<AudioClip> sound_electrocute;
    
    AudioSource audiosource_tape_background;
    AudioSource audiosource_audio_content;

    CharacterInput character_input;
    
    // Links to other objects in scene
    [HideInInspector]
    public GameObject main_camera;
    CharacterController character_controller;
    GUISkinHolder holder;
    WeaponHolder weapon_holder;
    
    // Help
    bool show_help = false;
    bool show_advanced_help = false;
    float help_hold_time = 0.0f;
    bool help_ever_shown = false;
    bool just_started_help = false;
    GUIStyle help_text_style = null;
    float help_text_offset = 0f;
    Color help_normal_color = new Color(.7f, .7f, .7f);
    
    // Aim down sights info
    bool aim_toggle = false;
    const float kAimSpringStrength = 100.0f;
	const float kAimSpringDamping = 0.00001f;
    Spring aim_spring = new Spring(0.0f,0.0f,kAimSpringStrength,kAimSpringDamping);
    
    // Flashlight positioning
    GameObject held_flashlight = null;
    Vector3 flashlight_aim_pos;
    Quaternion flashlight_aim_rot;
    Spring flashlight_mouth_spring = new Spring(0.0f,0.0f,kAimSpringStrength,kAimSpringDamping);
    Spring flash_ground_pose_spring = new Spring(0.0f,0.0f,kAimSpringStrength, kAimSpringDamping);
    Vector3 flash_ground_pos;
    Quaternion flash_ground_rot;
    
    // Allows gun to move independently of camera while aiming, within a small box
    float rotation_x_leeway = 0.0f;
    float rotation_y_min_leeway = 0.0f;
    float rotation_y_max_leeway = 0.0f;
    float kRotationXLeeway = 5.0f;
    float kRotationYMinLeeway = 20.0f;
    float kRotationYMaxLeeway = 10.0f;
    
    // Camera and gun rotations
    float rotation_x = 0.0f;
    float rotation_y = 0.0f;
    float view_rotation_x = 0.0f;
    float view_rotation_y = 0.0f;
    float sensitivity_x = 2.0f;
    float sensitivity_y = 2.0f;
    float min_angle_y = -89.0f;
    float max_angle_y = 89.0f;
    
    // Recoil
	const float kRecoilSpringStrength = 800.0f;
    const float kRecoilSpringDamping = 0.000001f;
    Spring x_recoil_spring = new Spring(0.0f,0.0f,kRecoilSpringStrength,kRecoilSpringDamping);
    Spring y_recoil_spring = new Spring(0.0f,0.0f,kRecoilSpringStrength,kRecoilSpringDamping);
    Spring head_recoil_spring_x = new Spring(0.0f,0.0f,kRecoilSpringStrength,kRecoilSpringDamping);
    Spring head_recoil_spring_y = new Spring(0.0f,0.0f,kRecoilSpringStrength,kRecoilSpringDamping);
    const int kMaxHeadRecoil = 10;
    float[] head_recoil_delay = new float[kMaxHeadRecoil];
    int next_head_recoil_delay = 0;
    
    // Actual instance of gun prefab			
    GameObject gun_instance;
    
    // Springs for different gun poses (how it's held)
    Spring slide_pose_spring = new Spring(0.0f,0.0f,kAimSpringStrength, kAimSpringDamping);
    Spring reload_pose_spring = new Spring(0.0f,0.0f,kAimSpringStrength, kAimSpringDamping);
    Spring press_check_pose_spring = new Spring(0.0f,0.0f,kAimSpringStrength, kAimSpringDamping);
    Spring inspect_cylinder_pose_spring = new Spring(0.0f,0.0f,kAimSpringStrength, kAimSpringDamping);
    Spring add_rounds_pose_spring = new Spring(0.0f,0.0f,kAimSpringStrength, kAimSpringDamping);
    Spring eject_rounds_pose_spring = new Spring(0.0f,0.0f,kAimSpringStrength, kAimSpringDamping);
    Spring alternative_stance_pose_spring = new Spring(0.0f,0.0f,kAimSpringStrength, kAimSpringDamping);
    float kGunDistance = 0.3f;

    GunTilt gun_tilt = GunTilt.CENTER;
    
    // Magazine posing
    Spring hold_pose_spring = new Spring(0.0f,0.0f,kAimSpringStrength, kAimSpringDamping);
    Spring mag_ground_pose_spring = new Spring(0.0f,0.0f,kAimSpringStrength, kAimSpringDamping);
    Vector3 mag_ground_pos;
    Quaternion mag_ground_rot;
    Vector3 mag_pos;
    Quaternion mag_rot;
    GameObject magazine_instance_in_hand;

    HandMagStage mag_stage = HandMagStage.EMPTY;
    bool queue_drop = false; // In case player pressed 'drop' again while mag is ejecting
    
    // Bullets
    List<GameObject> items_being_picked_up = new List<GameObject>();
    List<GameObject> loose_bullets;
    List<Spring> loose_bullet_spring;
    Spring show_bullet_spring = new Spring(0.0f,0.0f,kAimSpringStrength, kAimSpringDamping);
    float picked_up_bullet_delay = 0.0f;
    
    // Death effects
    float head_fall = 0.0f;
    float head_fall_vel = 0.0f;
    float head_tilt = 0.0f;
    float head_tilt_vel = 0.0f;
    float head_tilt_x_vel = 0.0f;
    float head_tilt_y_vel = 0.0f;
    float dead_fade = 1.0f;
    float win_fade = 0.0f;
    float dead_volume_fade = 0.0f;
    bool dead_body_fell = false;
    
    // Tape player
    float start_tape_delay = 0.0f;
    float stop_tape_delay = 0.0f;
    List<AudioClip> tapes_heard = new List<AudioClip>();
    List<AudioClip> tapes_remaining = new List<AudioClip>();
    List<AudioClip> total_tapes = new List<AudioClip>();
    bool tape_in_progress = false;
    int unplayed_tapes = 0;
    int tape_count = 11;
    
    // Cheats
    bool hasCheated = false;
    bool god_mode = false;
    bool slomo_mode = false;
    int iddqd_progress = 0;
    int idkfa_progress = 0;
    int slomo_progress = 0;
    float cheat_delay = 0.0f;
    float level_reset_hold = 0.0f;
    float slomoWarningDuration = 0f;
    
    // Inventory slots
    int target_weapon_slot = -2;

    WeaponSlot[] weapon_slots = new WeaponSlot[10];
    
    // Player state
    float health = 1.0f;
    bool dying = false;
    bool dead = false;
    bool won = false;

    //Level Creator
    LevelCreatorScript level_creator = null;
    
    public bool IsAiming() {
    	return (gun_instance != null && aim_spring.target_state == 1.0f);
    }
    
    public bool IsDead() {
    	return dead;
    }
    
    public void StepRecoil(float amount) {
    	x_recoil_spring.vel += UnityEngine.Random.Range(100,400) * amount;
    	y_recoil_spring.vel += UnityEngine.Random.Range(-200,200) * amount;
    }
    
    public void WasShot(){
    	head_recoil_spring_x.vel += (float)UnityEngine.Random.Range(-400,400);
    	head_recoil_spring_y.vel += (float)UnityEngine.Random.Range(-400,400);
    	x_recoil_spring.vel += (float)UnityEngine.Random.Range(-400,400);
    	y_recoil_spring.vel += (float)UnityEngine.Random.Range(-400,400);
    	rotation_x += (float)UnityEngine.Random.Range(-4,4);
    	rotation_y += (float)UnityEngine.Random.Range(-4,4);
    	if(!god_mode && !won){
    		dying = true;
    		if(UnityEngine.Random.Range(0.0f,1.0f) < 0.3f){
    			SetDead(true);
    		}
    		if(dead && UnityEngine.Random.Range(0.0f,1.0f) < 0.3f){
    			dead_fade += 0.3f;
    		}
    	}
    }
    
    public void FallDeath(Vector3 vel) {
    	if(!god_mode && !won){
    		SetDead(true);
    		head_fall_vel = vel.y;
    		dead_fade = Mathf.Max(dead_fade, 0.5f);
    		head_recoil_spring_x.vel += (float)UnityEngine.Random.Range(-400,400);
    		head_recoil_spring_y.vel += (float)UnityEngine.Random.Range(-400,400);
    	}
    }
    
    public void InstaKill() {
    	SetDead(true);
    	dead_fade = 1.0f;
    }
    
    public void Shock() {
    	if(!god_mode && !won){
    		if(!dead){
    			PlaySoundFromGroup(sound_electrocute, 1.0f);
    		}
    		SetDead(true);
    	}
    	head_recoil_spring_x.vel += (float)UnityEngine.Random.Range(-400,400);
    	head_recoil_spring_y.vel += (float)UnityEngine.Random.Range(-400,400);
    }
    
    public void SetDead(bool new_dead) {
    	if(new_dead == dead){
    		return;
    	}
    	dead = new_dead;
    	if(!dead){
    		head_tilt_vel = 0.0f;
    		head_tilt_x_vel = 0.0f;
    		head_tilt_y_vel = 0.0f;
    		head_tilt = 0.0f;
    		head_fall = 0.0f;
    	} else {
    		if(main_client_control){
    			GetComponent<MusicScript>().HandleEvent(MusicEvent.DEAD);
    		}
    		head_tilt_vel = (float)UnityEngine.Random.Range(-100,100);
    		head_tilt_x_vel = (float)UnityEngine.Random.Range(-100,100);
    		head_tilt_y_vel = (float)UnityEngine.Random.Range(-100,100);
    		head_fall = 0.0f;
    		head_fall_vel = 0.0f;
    		dead_body_fell = false;
    	}
    }
    
    public void PlaySoundFromGroup(List<AudioClip> group,float volume){
    	int which_shot = UnityEngine.Random.Range(0,group.Count);
    	GetComponent<AudioSource>().PlayOneShot(group[which_shot], volume * Preferences.sound_volume);
    }
    
    public void AddLooseBullet(bool spring) {
        GameObject round = Instantiate(casing_with_bullet);

        if(level_creator != null) {
            round.transform.parent = level_creator.GetPlayerInventoryTransform();
        }
           
    	loose_bullets.Add(round);
    	Spring new_spring = new Spring(0.3f,0.3f,kAimSpringStrength,kAimSpringDamping);
    	loose_bullet_spring.Add(new_spring);
    	if(spring){
    		new_spring.vel = 3.0f;
    		picked_up_bullet_delay = 2.0f;
    	}
    }
    
    public void Start() { 
        GameObject level_object = GameObject.Find("LevelObject");

        if(level_object != null) {
            level_creator = level_object.GetComponent<LevelCreatorScript>();
        }

        if(level_creator == null) {
            Debug.LogWarning("We're missing a LevelCreatorScript in AimScript, this might mean that some world-interactions don't work correctly.");
        }

    	holder = GameObject.Find("gui_skin_holder").GetComponent<GUISkinHolder>();
    	help_text_style = holder.gui_skin.label;
    	weapon_holder = holder.weapon.GetComponent<WeaponHolder>();
    	magazine_obj = weapon_holder.mag_object;
    	gun_obj = weapon_holder.gun_object;
    	casing_with_bullet = weapon_holder.bullet_object;
    	character_input = new CharacterInput();
    
    	if(UnityEngine.Random.Range(0f, 1f) < 0.35f) {
    		held_flashlight = (GameObject)Instantiate(holder.flashlight_object);
    		Destroy(held_flashlight.GetComponent<Rigidbody>());
    		held_flashlight.GetComponent<FlashlightScript>().TurnOn();
            if(level_creator != null) {
                held_flashlight.transform.parent = level_creator.GetPlayerInventoryTransform();
            }
    		holder.has_flashlight = true;
    	}
    	
    	rotation_x = transform.rotation.eulerAngles.y;
    	view_rotation_x = transform.rotation.eulerAngles.y;
    	gun_instance = (GameObject)Instantiate(gun_obj);
        if(level_creator != null) {
            gun_instance.transform.parent = level_creator.GetPlayerInventoryTransform();
        }
    	Renderer[] renderers = gun_instance.GetComponentsInChildren<Renderer>();
    	foreach(Renderer renderer in renderers){
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    	}
    	if(main_client_control){
    		main_camera = GameObject.Find("Main Camera").gameObject;
    	} else {
    		main_camera = new GameObject();
    		GetComponent<MusicScript>().enabled = false;
    	}
    	character_controller = GetComponent<CharacterController>();
    	for(int i=0; i<kMaxHeadRecoil; ++i){
    		head_recoil_delay[i] = -1.0f;
    	}
    	for(int i=0; i<10; ++i){
    		weapon_slots[i] = new WeaponSlot();
    	}
    	int num_start_bullets = UnityEngine.Random.Range(0,10);
    	if(magazine_obj != null) {
    		int num_start_mags = UnityEngine.Random.Range(0,3);
    		for(int i=1; i<num_start_mags+1; ++i){
    			weapon_slots[i].type = WeaponSlotType.MAGAZINE;
    			weapon_slots[i].obj = (GameObject)Instantiate(magazine_obj);
                if(level_creator != null) {
                    weapon_slots[i].obj.transform.parent = level_creator.GetPlayerInventoryTransform();
                }
    		}
    	} else {
    		num_start_bullets += UnityEngine.Random.Range(0,20);
    	}
    	loose_bullets = new List<GameObject>();
    	loose_bullet_spring = new List<Spring>();
    	for(int i=0; i<num_start_bullets; ++i){
    		AddLooseBullet(false);
    	}
    	audiosource_tape_background = gameObject.AddComponent<AudioSource>();
    	audiosource_tape_background.loop = true;
    	audiosource_tape_background.clip = holder.sound_tape_background;
    	audiosource_audio_content = gameObject.AddComponent<AudioSource>();
    	audiosource_audio_content.loop = false;
    	
    	List<AudioClip> temp_total_tapes = new List<AudioClip>(holder.sound_tape_content);
    	while(tapes_remaining.Count < tape_count) {
    		if(temp_total_tapes.Count <= 0) {
    			temp_total_tapes.AddRange(holder.sound_tape_content); // We have run out of tapes, but we need more => Allow for duplicates
    		}
    
    		int rand_tape_id = UnityEngine.Random.Range(0, temp_total_tapes.Count);
    		tapes_remaining.Add(temp_total_tapes[rand_tape_id]);
    		temp_total_tapes.RemoveAt(rand_tape_id);
    	}
    
    	total_tapes = new List<AudioClip>(tapes_remaining);
    }
    
    public float GunDist() {
    	return kGunDistance * (0.5f + PlayerPrefs.GetFloat("gun_distance", 1.0f)*0.5f);
    }
    
    public Vector3 AimPos() {
    	Vector3 aim_dir = AimDir();
    	return main_camera.transform.position + aim_dir*GunDist();
    }
    
    public Vector3 AimDir() {
    	Quaternion aim_rot = new Quaternion();
        aim_rot = Quaternion.Euler(-rotation_y, rotation_x, 0.0f);
    	return aim_rot * new Vector3(0.0f,0.0f,1.0f);
    }
    
    public GunScript GetGunScript() {
    	return gun_instance.GetComponent<GunScript>();
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
    	if(angle == 0){
    		return a;
    	}
    	return a * Quaternion.AngleAxis(angle * -val, axis);
    }
    
    public bool ShouldPickUpNearby() {
    	//object nearest_mag = null;
    	//float nearest_mag_dist = 0.0f;
    	Collider[] colliders = Physics.OverlapSphere(main_camera.transform.position, 2.0f, 1 << 8);
    	foreach(Collider collider in colliders){
    		if((magazine_obj != null) && collider.gameObject.name == magazine_obj.name+"(Clone)" && (collider.gameObject.GetComponent<Rigidbody>() != null)){
    			if(mag_stage == HandMagStage.EMPTY){
    				return true;
    			}	
    		} else if((collider.gameObject.name == casing_with_bullet.name || collider.gameObject.name == casing_with_bullet.name+"(Clone)") && (collider.gameObject.GetComponent<Rigidbody>() != null)){
    			return true;
    		}
    	}
    	return false;
    }
    
    // Pick up nearby objects
    public void HandleGetControl(){
    	// Only pick up one magazine at a time
    	GameObject nearest_mag = null;
    	float nearest_mag_dist = 0.0f;
    	// Detect all nearby colliders on correct physics layer
    	Collider[] colliders = Physics.OverlapSphere(main_camera.transform.position, 2.0f, 1 << 8);
    	foreach(Collider collider in colliders){
    		if((magazine_obj != null) && collider.gameObject.name == magazine_obj.name+"(Clone)" && (collider.gameObject.GetComponent<Rigidbody>() != null)){
    			// Magazine
    			float dist = Vector3.Distance(collider.transform.position, main_camera.transform.position);
    			if(nearest_mag == null || dist < nearest_mag_dist){	
    				nearest_mag_dist = dist;
    				nearest_mag = collider.gameObject;
    			}					
    		} else if((collider.gameObject.name == casing_with_bullet.name || collider.gameObject.name == casing_with_bullet.name+"(Clone)") && (collider.gameObject.GetComponent<Rigidbody>() != null)){
    			// Unfired bullet
    			items_being_picked_up.Add(collider.gameObject);			
    			collider.gameObject.GetComponent<Rigidbody>().useGravity = false;
    			collider.gameObject.GetComponent<Rigidbody>().WakeUp();
    			collider.enabled = false;
    		} else if(collider.gameObject.name == "cassette_tape(Clone)" && (collider.gameObject.GetComponent<Rigidbody>() != null)){
    			// Cassette tape
    			items_being_picked_up.Add(collider.gameObject);			
    			collider.gameObject.GetComponent<Rigidbody>().useGravity = false;
    			collider.gameObject.GetComponent<Rigidbody>().WakeUp();
    			collider.enabled = false;
    		} else if(collider.gameObject.name == "flashlight_object(Clone)" && (collider.gameObject.GetComponent<Rigidbody>() != null) && !holder.has_flashlight){
    			// Flashlight
    			held_flashlight = collider.gameObject;
    			Destroy(held_flashlight.GetComponent<Rigidbody>());
                if(level_creator != null) {
                    held_flashlight.transform.parent = level_creator.GetPlayerInventoryTransform();
                } else {
                    held_flashlight.transform.parent = null; //Move flashlight out of tile
                }
    			held_flashlight.GetComponent<FlashlightScript>().TurnOn();
    			holder.has_flashlight = true;
    			flash_ground_pos = held_flashlight.transform.position;
    			flash_ground_rot = held_flashlight.transform.rotation;
    			flash_ground_pose_spring.state = 1.0f;
    			flash_ground_pose_spring.vel = 1.0f;
    		}
    	}
    	// Picking up magazine
    	if((nearest_mag != null) && mag_stage == HandMagStage.EMPTY){
    		magazine_instance_in_hand = nearest_mag;
    		Destroy(magazine_instance_in_hand.GetComponent<Rigidbody>());
            if(level_creator != null) { 
                magazine_instance_in_hand.transform.parent = level_creator.GetPlayerInventoryTransform(); //Move item out of tile into player inventory
            } else {
                magazine_instance_in_hand.transform.parent = null; //Move item out of tile
            }
    		mag_ground_pos = magazine_instance_in_hand.transform.position;
    		mag_ground_rot = magazine_instance_in_hand.transform.rotation;
    		mag_ground_pose_spring.state = 1.0f;
    		mag_ground_pose_spring.vel = 1.0f;
    		hold_pose_spring.state = 1.0f;
    		hold_pose_spring.vel = 0.0f;
    		hold_pose_spring.target_state = 1.0f;
    		mag_stage = HandMagStage.HOLD;
    	}
    }
    
    public bool HandleInventoryControls() {	
    	if(character_input.GetButtonDown("Holster")){
    		target_weapon_slot = -1;
    	}
    	if(character_input.GetButtonDown("Inventory 1")){
    		target_weapon_slot = 0;
    	}
    	if(character_input.GetButtonDown("Inventory 2")){
    		target_weapon_slot = 1;
    	}
    	if(character_input.GetButtonDown("Inventory 3")){
    		target_weapon_slot = 2;
    	}
    	if(character_input.GetButtonDown("Inventory 4")){
    		target_weapon_slot = 3;
    	}
    	if(character_input.GetButtonDown("Inventory 5")){
    		target_weapon_slot = 4;
    	}
    	if(character_input.GetButtonDown("Inventory 6")){
    		target_weapon_slot = 5;
    	}
    	if(character_input.GetButtonDown("Inventory 7")){
    		target_weapon_slot = 6;
    	}
    	if(character_input.GetButtonDown("Inventory 8")){
    		target_weapon_slot = 7;
    	}
    	if(character_input.GetButtonDown("Inventory 9")){
    		target_weapon_slot = 8;
    	}
    	if(character_input.GetButtonDown("Inventory 10")){
    		target_weapon_slot = 9;
    	}
    	
    	bool mag_ejecting = false;
		GunScript gunScript = null;
		if(gun_instance)
			gunScript = gun_instance.GetComponent<GunScript>();

    	if(gun_instance && (gunScript.IsMagCurrentlyEjecting() || gunScript.IsReadyToRemoveMagazine()))
    		mag_ejecting = true;
    
    	bool insert_mag_with_number_key = false;
    	
    	if(target_weapon_slot != -2 && !mag_ejecting && (mag_stage == HandMagStage.EMPTY || mag_stage == HandMagStage.HOLD)){
    		if(target_weapon_slot == -1 && (gun_instance == null)){
    			for(int i=0; i<10; ++i){
    				if(weapon_slots[i].type == WeaponSlotType.GUN){
    					target_weapon_slot = i;
    					break;
    				}
    			}
    		}
    		if(mag_stage == HandMagStage.HOLD && target_weapon_slot != -1 && weapon_slots[target_weapon_slot].type == WeaponSlotType.EMPTY){
    			// Put held mag in empty slot
    			for(int i=0; i<10; ++i){
    				if(weapon_slots[target_weapon_slot].type != WeaponSlotType.EMPTY && weapon_slots[target_weapon_slot].obj == magazine_instance_in_hand){
    					weapon_slots[target_weapon_slot].type = WeaponSlotType.EMPTY;
    				}
    			}
    			weapon_slots[target_weapon_slot].type = WeaponSlotType.MAGAZINE;
    			weapon_slots[target_weapon_slot].obj = magazine_instance_in_hand;
    			weapon_slots[target_weapon_slot].spring.state = 0.0f;
    			weapon_slots[target_weapon_slot].spring.target_state = 1.0f;
    			weapon_slots[target_weapon_slot].start_pos = magazine_instance_in_hand.transform.position - main_camera.transform.position;
    			weapon_slots[target_weapon_slot].start_rot = Quaternion.Inverse(main_camera.transform.rotation) * magazine_instance_in_hand.transform.rotation;
    			magazine_instance_in_hand = null;
    			mag_stage = HandMagStage.EMPTY;
    			target_weapon_slot = -2;
    		} else if(mag_stage == HandMagStage.HOLD && target_weapon_slot != -1 && weapon_slots[target_weapon_slot].type == WeaponSlotType.EMPTYING && weapon_slots[target_weapon_slot].obj == magazine_instance_in_hand && (gun_instance != null) && !gunScript.IsThereAMagInGun()){
    			insert_mag_with_number_key = true;
    			target_weapon_slot = -2;
    		} else if (target_weapon_slot != -1 && mag_stage == HandMagStage.EMPTY && weapon_slots[target_weapon_slot].type == WeaponSlotType.MAGAZINE){
    			// Take mag from inventory
    			magazine_instance_in_hand = weapon_slots[target_weapon_slot].obj;
    			mag_stage = HandMagStage.HOLD;
    			hold_pose_spring.state = 1.0f;
    			hold_pose_spring.target_state = 1.0f;
    			weapon_slots[target_weapon_slot].type = WeaponSlotType.EMPTYING;
    			weapon_slots[target_weapon_slot].spring.target_state = 0.0f;
    			weapon_slots[target_weapon_slot].spring.state = 1.0f;
    			target_weapon_slot = -2;
    		} else if (target_weapon_slot != -1 && mag_stage == HandMagStage.EMPTY && weapon_slots[target_weapon_slot].type == WeaponSlotType.EMPTY && (held_flashlight != null)){
    			// Put flashlight away
    			held_flashlight.GetComponent<FlashlightScript>().TurnOff();
    			weapon_slots[target_weapon_slot].type = WeaponSlotType.FLASHLIGHT;
    			weapon_slots[target_weapon_slot].obj = held_flashlight;
    			weapon_slots[target_weapon_slot].spring.state = 0.0f;
    			weapon_slots[target_weapon_slot].spring.target_state = 1.0f;
    			weapon_slots[target_weapon_slot].start_pos = held_flashlight.transform.position - main_camera.transform.position;
    			weapon_slots[target_weapon_slot].start_rot = Quaternion.Inverse(main_camera.transform.rotation) * held_flashlight.transform.rotation;
    			held_flashlight = null;
    			target_weapon_slot = -2;
    		}  else if (target_weapon_slot != -1 && (held_flashlight == null) && weapon_slots[target_weapon_slot].type == WeaponSlotType.FLASHLIGHT) {// && (!gun_instance || gunScript.handed == HandedType.ONE_HANDED)) {
    			// Take flashlight from inventory
    			held_flashlight = weapon_slots[target_weapon_slot].obj;
    			held_flashlight.GetComponent<FlashlightScript>().TurnOn();
    			weapon_slots[target_weapon_slot].type = WeaponSlotType.EMPTYING;
    			weapon_slots[target_weapon_slot].spring.target_state = 0.0f;
    			weapon_slots[target_weapon_slot].spring.state = 1.0f;
    			target_weapon_slot = -2;
    		} else if((gun_instance != null) && target_weapon_slot == -1){
    			// Put gun away
    			if(target_weapon_slot == -1){
    				for(int i=0; i<10; ++i){
    					if(weapon_slots[i].type == WeaponSlotType.EMPTY){
    						target_weapon_slot = i;
    						break;
    					}
    				}
    			}
    			if(target_weapon_slot != -1 && weapon_slots[target_weapon_slot].type == WeaponSlotType.EMPTY){
    				for(int i=0; i<10; ++i){
    					if(weapon_slots[target_weapon_slot].type != WeaponSlotType.EMPTY && weapon_slots[target_weapon_slot].obj == gun_instance){
    						weapon_slots[target_weapon_slot].type = WeaponSlotType.EMPTY;
    					}
    				}
    				weapon_slots[target_weapon_slot].type = WeaponSlotType.GUN;
    				weapon_slots[target_weapon_slot].obj = gun_instance;
    				weapon_slots[target_weapon_slot].spring.state = 0.0f;
    				weapon_slots[target_weapon_slot].spring.target_state = 1.0f;
    				weapon_slots[target_weapon_slot].start_pos = gun_instance.transform.position - main_camera.transform.position;
    				weapon_slots[target_weapon_slot].start_rot = Quaternion.Inverse(main_camera.transform.rotation) * gun_instance.transform.rotation;
    				gun_instance = null;
    				target_weapon_slot = -2;
    			}
    		} else if(target_weapon_slot >= 0 && (gun_instance == null)){
    			if(weapon_slots[target_weapon_slot].type == WeaponSlotType.EMPTY){
    				target_weapon_slot = -2;
    			} else {
    				if(weapon_slots[target_weapon_slot].type == WeaponSlotType.GUN){
    					gun_instance = weapon_slots[target_weapon_slot].obj;
    					weapon_slots[target_weapon_slot].type = WeaponSlotType.EMPTYING;
    					weapon_slots[target_weapon_slot].spring.target_state = 0.0f;
    					weapon_slots[target_weapon_slot].spring.state = 1.0f;
    					target_weapon_slot = -2;
    				} else if(weapon_slots[target_weapon_slot].type == WeaponSlotType.MAGAZINE && mag_stage == HandMagStage.EMPTY){
    					magazine_instance_in_hand = weapon_slots[target_weapon_slot].obj;
    					mag_stage = HandMagStage.HOLD;
    					weapon_slots[target_weapon_slot].type = WeaponSlotType.EMPTYING;
    					weapon_slots[target_weapon_slot].spring.target_state = 0.0f;
    					weapon_slots[target_weapon_slot].spring.state = 1.0f;
    					target_weapon_slot = -2;
    				}
    			}
    		}
    	}
    	return insert_mag_with_number_key;
    }
    
    public void HandleGunControls(bool insert_mag_with_number_key) {
    	GunScript gun_script = GetGunScript();
    	if(character_input.GetButton("Trigger")){
    		gun_script.ApplyPressureToTrigger();
    	} else {
    		gun_script.ReleasePressureFromTrigger();
    	}
    	if(character_input.GetButtonDown("Slide Lock")){
    		gun_script.ReleaseSlideLock();
    	}
    	if(character_input.GetButtonUp("Slide Lock")){
    		gun_script.ReleasePressureOnSlideLock();
    	}

    	// Pull slide or do press check
    	if(character_input.GetButton("Pull Back Slide")) {
    		if(character_input.GetButton("Slide Lock")) {
    			gun_script.Request(GunSystemRequests.INPUT_PULL_SLIDE_PRESS_CHECK);
    		} else if(character_input.GetButtonUp("Slide Lock")) {
				gun_script.InputPullSlideBack();
    		}
    	}

		if(character_input.GetButtonDown("Pull Back Slide")){
			if(gun_script.Query(GunSystemQueries.IS_WAITING_FOR_SLIDE_PUSH)) { // Slide input should push slide forward
				gun_script.PushSlideForward();
			} else {
				gun_script.InputPullSlideBack();
			}
		}
		if(character_input.GetButtonUp("Pull Back Slide")){
			gun_script.ReleaseSlide();
		}
    	if(character_input.GetButton("Slide Lock")) {
    		gun_script.PressureOnSlideLock();
    	}
    	if(character_input.GetButtonDown("Toggle Bolt Lock")) {
    		gun_script.ToggleBoltLock();
    	}
    	if(character_input.GetButtonDown("Safety")){
    		gun_script.ToggleSafety();			
    	}	
    	if(character_input.GetButtonDown("Auto Mod Toggle")){
    		gun_script.ToggleAutoMod();			
    	}
    	if(character_input.GetButtonDown("Swing Out Cylinder")){
    		gun_script.SwingOutCylinder();
    	}	
    	if(character_input.GetButtonDown("Close Cylinder")){
    		gun_script.CloseCylinder();
    	}	
    	if(character_input.GetButton("Extractor Rod")){
    		gun_script.ExtractorRod();
    	}
    	if(character_input.GetButton("Hammer")){
    		gun_script.PressureOnHammer();
    	}
    	if(character_input.GetButtonUp("Hammer")){
    		gun_script.ReleaseHammer();
    	}
    	if(character_input.GetButtonDown("Toggle Stance")){
    		gun_script.InputToggleStance();
    	}
    	if(character_input.GetAxis("Mouse ScrollWheel") != 0.0f){
    		gun_script.RotateCylinder((int)Input.GetAxis("Mouse ScrollWheel"));
    	}		
    	if(character_input.GetButtonDown("Insert")){
    		if(loose_bullets.Count > 0){
    			if(GetGunScript().AddRoundToCylinder()){
    				GameObject.Destroy(loose_bullets[loose_bullets.Count-1]);
    				loose_bullets.RemoveAt(loose_bullets.Count-1);
    				loose_bullet_spring.RemoveAt(loose_bullet_spring.Count-1);
    			}
    		}
    	}
    	// Aiming notification
    	if(!aim_toggle) {
    		if(character_input.GetButtonDown("Hold To Aim")) {
    			gun_script.InputStartAim();
    		} else if(character_input.GetButtonUp("Hold To Aim")) {
    			gun_script.InputStopAim();
    		}
    	}
		if(character_input.GetButtonDown("Aim Toggle") && !character_input.GetButton("Hold To Aim")) {
			if(aim_toggle) {
				gun_script.InputStopAim();
			} else {
    			gun_script.InputStartAim();
			}
		}

    	if(gun_script.preferred_tilt != GunTilt.NONE) { // Allow guncomponents to choose how the gun should be tilted
    		gun_tilt = gun_script.preferred_tilt;
    	} else if(slide_pose_spring.target_state < 0.1f && reload_pose_spring.target_state < 0.1f){
    		gun_tilt = GunTilt.CENTER;
    	} else if(slide_pose_spring.target_state > reload_pose_spring.target_state){
    		gun_tilt = GunTilt.LEFT;
    	} else {
    		gun_tilt = GunTilt.RIGHT;
    	}
    	
    	slide_pose_spring.target_state = 0.0f;
    	reload_pose_spring.target_state = 0.0f;
    	press_check_pose_spring.target_state = 0.0f;
    	
    	if(gun_script.IsSafetyOn()){
    		reload_pose_spring.target_state = 0.2f;
    		slide_pose_spring.target_state = 0.0f;
    		gun_tilt = GunTilt.RIGHT;
    	}
    	
    	if(gun_script.IsSlideLocked() && !gun_script.HasGunComponent(GunAspect.OPEN_BOLT_FIRING)){
    		if(gun_tilt != GunTilt.LEFT){
    			reload_pose_spring.target_state = 0.7f;
    		} else {
    			slide_pose_spring.target_state = 0.7f;
    		}
    	}
    	if(gun_script.IsSlidePulledBack()){
    		if(gun_tilt != GunTilt.RIGHT){
    			slide_pose_spring.target_state = 1.0f;
    		} else {
    			reload_pose_spring.target_state = 1.0f;
    		}
    	}
    	
    	alternative_stance_pose_spring.target_state = gun_script.IsInAlternativeStance() ? 1f : 0f;
    	
    	if(gun_script.IsPressCheck()){
    		slide_pose_spring.target_state = 0.0f;
    		reload_pose_spring.target_state = 0.0f;
    		press_check_pose_spring.target_state = 0.6f;
    	}
    	
    	add_rounds_pose_spring.target_state = 0.0f;
    	eject_rounds_pose_spring.target_state = 0.0f;
    	inspect_cylinder_pose_spring.target_state = 0.0f;
    	if(gun_script.IsEjectingRounds()){
    		eject_rounds_pose_spring.target_state = 1.0f;
    	//} else if(gun_script.IsAddingRounds()){
    	//	add_rounds_pose_spring.target_state = 1.0;
    	} else if(gun_script.IsCylinderOpen()){
    		inspect_cylinder_pose_spring.target_state = 1.0f;
    	}
    	
        var recoil_data = gun_script.GetRecoilData();
        if(recoil_data != null) {
            x_recoil_spring.vel += recoil_data.recoil_transfer_x;
            y_recoil_spring.vel += recoil_data.recoil_transfer_y;
            rotation_x += recoil_data.rotation_transfer_x;
            rotation_y += recoil_data.rotation_transfer_y;
            recoil_data.recoil_transfer_x = 0.0f;
            recoil_data.recoil_transfer_y = 0.0f;
            recoil_data.rotation_transfer_x = 0.0f;
            recoil_data.rotation_transfer_y = 0.0f;
            if(recoil_data.add_head_recoil){
                head_recoil_delay[next_head_recoil_delay] = 0.1f;
                next_head_recoil_delay = (next_head_recoil_delay + 1)%kMaxHeadRecoil;
                recoil_data.add_head_recoil = false;
            }
        }
    	
    	if(gun_script.IsReadyToRemoveMagazine() && (magazine_instance_in_hand == null)){
    		magazine_instance_in_hand = gun_script.GrabMag();
    		mag_stage = HandMagStage.HOLD;
    		hold_pose_spring.state = 0.0f;
    		hold_pose_spring.vel = 0.0f;
    		hold_pose_spring.target_state = 1.0f;
    	}
    	if((character_input.GetButtonDown("Insert")/* && aim_spring.state > 0.5*/) || insert_mag_with_number_key){
    		if(mag_stage == HandMagStage.HOLD && !gun_script.IsThereAMagInGun() || insert_mag_with_number_key){
    			hold_pose_spring.target_state = 0.0f;
    			mag_stage = HandMagStage.HOLD_TO_INSERT;
    		}
    	}
    	if(mag_stage == HandMagStage.HOLD_TO_INSERT){
    		if(hold_pose_spring.state < 0.01f){
    			gun_script.InsertMag(magazine_instance_in_hand);
    			magazine_instance_in_hand = null;
    			mag_stage = HandMagStage.EMPTY;
    		}
    	}
    }
    
    public void HandleControls() {
    	if(character_input.GetButton("Get")){
    		HandleGetControl();
    	}
    	
    	for(int i = 0; i < kMaxHeadRecoil; ++i){
    		if(head_recoil_delay[i] != -1.0f){
    			head_recoil_delay[i] -= Time.deltaTime;
    			if(head_recoil_delay[i] <= 0.0f){
    				head_recoil_spring_x.vel += UnityEngine.Random.Range(-30.0f,30.0f);
    				head_recoil_spring_y.vel += UnityEngine.Random.Range(-30.0f,30.0f);
    				head_recoil_delay[i] = -1.0f;
    			}
    		}
    	}
    	
    	bool insert_mag_with_number_key = HandleInventoryControls();
    	
    	if(character_input.GetButtonDown("Eject/Drop") || queue_drop){
    		if(mag_stage == HandMagStage.HOLD){
    			mag_stage = HandMagStage.EMPTY;
    			magazine_instance_in_hand.AddComponent<Rigidbody>();
    			magazine_instance_in_hand.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
    			magazine_instance_in_hand.GetComponent<Rigidbody>().velocity = character_controller.velocity;
                magazine_instance_in_hand.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;

                if(level_creator != null) {
                    magazine_instance_in_hand.transform.parent = level_creator.GetPositionTileItemParent(magazine_instance_in_hand.transform.position);
                }

    			magazine_instance_in_hand = null;
    			queue_drop = false;
    		} else if(held_flashlight != null && mag_stage == HandMagStage.EMPTY && gun_instance == null){
                held_flashlight.AddComponent<Rigidbody>();
                held_flashlight.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
                held_flashlight.GetComponent<Rigidbody>().velocity = character_controller.velocity;
                held_flashlight.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;

                if(level_creator != null){
                    held_flashlight.transform.parent = level_creator.GetPositionTileItemParent(held_flashlight.transform.position);
                }

                held_flashlight = null;
                holder.has_flashlight = false;
                queue_drop = false;
            }
    	}
    	
    	if(character_input.GetButtonDown("Eject/Drop")){
    		if(mag_stage == HandMagStage.EMPTY && (gun_instance != null)){
    			if(gun_instance.GetComponent<GunScript>().IsMagCurrentlyEjecting()){
    				queue_drop = true;
    			} else {
    				gun_instance.GetComponent<GunScript>().MagEject();
    			}
    		} else if(mag_stage == HandMagStage.HOLD_TO_INSERT){
    			mag_stage = HandMagStage.HOLD;
    			hold_pose_spring.target_state = 1.0f;
    		}
    	}
    	
    	if(gun_instance != null){
    		HandleGunControls(insert_mag_with_number_key);
    	} else if(mag_stage == HandMagStage.HOLD){
    		if(character_input.GetButtonDown("Insert")){
    			if(loose_bullets.Count > 0){
    				if(magazine_instance_in_hand.GetComponent<mag_script>().AddRound()){
    					GameObject.Destroy(loose_bullets[loose_bullets.Count-1]);
    					loose_bullets.RemoveAt(loose_bullets.Count-1);
    					loose_bullet_spring.RemoveAt(loose_bullet_spring.Count-1);
    				}
    			}
    		}
    		if(character_input.GetButtonDown("Pull Back Slide")){
    			if(magazine_instance_in_hand.GetComponent<mag_script>().RemoveRoundAnimated()){
    				AddLooseBullet(true);
    				PlaySoundFromGroup(sound_bullet_grab, 0.2f);
    			}
    		}
    	}
    	if(character_input.GetButtonDown("Aim Toggle")){
    		aim_toggle = !aim_toggle;
    	}
    	if(character_input.GetButtonDown("Slow Motion Toggle")){
    		if(slomo_mode) {
    			if(Time.timeScale == 1.0f) {
    				Time.timeScale = 0.1f;
    			} else {
    				Time.timeScale = 1.0f;
    			}
    		} else {
    			slomoWarningDuration = 1f;
    		}
    	}
        if(character_input.GetButtonDown("Flashlight Toggle")){
            if(held_flashlight != null && mag_stage == HandMagStage.EMPTY && gun_instance == null){
                held_flashlight.GetComponent<FlashlightScript>().ToggleSwitch();
            }
        }
    }
    
    public void StartTapePlay() {
    	GetComponent<AudioSource>().PlayOneShot(holder.sound_tape_start, 1.0f * Preferences.voice_volume);
    	audiosource_tape_background.Play();
    	if(tape_in_progress && start_tape_delay == 0.0f){ 
    		audiosource_audio_content.Play();
    	}
    	if(!tape_in_progress && tapes_remaining.Count > 0){
    		audiosource_audio_content.clip = tapes_remaining[0];
    		tapes_remaining.RemoveAt(0);
    		//audiosource_audio_content.pitch = 10.0;
    		//audiosource_audio_content.clip = holder.sound_scream[Random.Range(0,holder.sound_scream.Count)];
    		start_tape_delay = UnityEngine.Random.Range(0.5f,3.0f);
    		stop_tape_delay = 0.0f;
    		tape_in_progress = true;
    	}
    	audiosource_tape_background.pitch = 0.1f;
    	audiosource_audio_content.pitch = 0.1f;
    }
    
    public void StopTapePlay() {
    	GetComponent<AudioSource>().PlayOneShot(holder.sound_tape_end, 1.0f * Preferences.voice_volume);
    	if(tape_in_progress){
    		audiosource_tape_background.Pause();
    		audiosource_audio_content.Pause();
    	} else {
    		audiosource_tape_background.Stop();
    		audiosource_audio_content.Stop();
    	}
    }
    
    public void StartWin() {
    	if(main_client_control){
    		GetComponent<MusicScript>().HandleEvent(MusicEvent.WON);
    	}
    	won = true;
    }
    
    public void ApplyPose(string name,float amount){
    	Transform pose = gun_instance.transform.Find(name);
    	if(amount == 0.0f || (pose == null)){
    		return;
    	}
    	gun_instance.transform.position = mix(gun_instance.transform.position,
    									      pose.position,
    									      amount);
    	gun_instance.transform.rotation = mix(
    		gun_instance.transform.rotation,
    		pose.rotation,
    		amount);
    }
    
    public void UpdateCheats() {
    	if(iddqd_progress == 0 && Input.GetKeyDown("i")){
    		++iddqd_progress; cheat_delay = 1.0f;
    	} else if(iddqd_progress == 1 && Input.GetKeyDown("d")){
    		++iddqd_progress; cheat_delay = 1.0f;
    	} else if(iddqd_progress == 2 && Input.GetKeyDown("d")){
    		++iddqd_progress; cheat_delay = 1.0f;
    	} else if(iddqd_progress == 3 && Input.GetKeyDown("q")){
    		++iddqd_progress; cheat_delay = 1.0f;
    	} else if(iddqd_progress == 4 && Input.GetKeyDown("d")){
    		iddqd_progress = 0;
    		god_mode = !god_mode;
    		hasCheated = true;
    		PlaySoundFromGroup(holder.sound_scream, 1.0f);
    	}
    	if(idkfa_progress == 0 && Input.GetKeyDown("i")){
    		++idkfa_progress; cheat_delay = 1.0f;
    	} else if(idkfa_progress == 1 && Input.GetKeyDown("d")){
    		++idkfa_progress; cheat_delay = 1.0f;
    	} else if(idkfa_progress == 2 && Input.GetKeyDown("k")){
    		++idkfa_progress; cheat_delay = 1.0f;
    	} else if(idkfa_progress == 3 && Input.GetKeyDown("f")){
    		++idkfa_progress; cheat_delay = 1.0f;
    	} else if(idkfa_progress == 4 && Input.GetKeyDown("a")){
    		idkfa_progress = 0;
    		hasCheated = true;
    		if(loose_bullets.Count < 30){
    			PlaySoundFromGroup(sound_bullet_grab, 0.2f);
    		}
    		while(loose_bullets.Count < 30){
    			AddLooseBullet(true);
    		}
    		PlaySoundFromGroup(holder.sound_scream, 1.0f);
    	}
    	if(slomo_progress == 0 && Input.GetKeyDown("s")){
    		++slomo_progress; cheat_delay = 1.0f;
    	} else if(slomo_progress == 1 && Input.GetKeyDown("l")){
    		++slomo_progress; cheat_delay = 1.0f;
    	} else if(slomo_progress == 2 && Input.GetKeyDown("o")){
    		++slomo_progress; cheat_delay = 1.0f;
    	} else if(slomo_progress == 3 && Input.GetKeyDown("m")){
    		++slomo_progress; cheat_delay = 1.0f;
    	} else if(slomo_progress == 4 && Input.GetKeyDown("o")){
    		slomo_progress = 0;
    		slomo_mode = true;
    		hasCheated = true;
    		if(Time.timeScale == 1.0f){
    			Time.timeScale = 0.1f;
    		} else {
    			Time.timeScale = 1.0f;
    		}
    		PlaySoundFromGroup(holder.sound_scream, 1.0f);
    	}
    	if(cheat_delay > 0.0f){
    		cheat_delay -= Time.deltaTime;
    		if(cheat_delay <= 0.0f){
    			cheat_delay = 0.0f;
    			iddqd_progress = 0;
    			idkfa_progress = 0;
    			slomo_progress = 0;
    		}
    	}
    }
    
    public void UpdateTape() {
    	if(tapes_heard.Count + unplayed_tapes + (tape_in_progress ? 1 : 0) >= total_tapes.Count) {
    		GetComponent<SpeedrunTimer>().StopTimer();
    	}
    
    	if(!tape_in_progress && unplayed_tapes > 0){
    		--unplayed_tapes;
    		StartTapePlay();
    	}
    	if(character_input.GetButtonDown("Tape Player") && tape_in_progress){
    		if(!audiosource_tape_background.isPlaying){
    			StartTapePlay();
    		} else {
    			StopTapePlay();
    		}
    	}
    	if(tape_in_progress && audiosource_tape_background.isPlaying){ 
    		GetComponent<MusicScript>().SetMystical((tapes_heard.Count+1.0f)/total_tapes.Count);
    		audiosource_tape_background.volume = Preferences.voice_volume;
    		audiosource_tape_background.pitch = Mathf.Min(1.0f,audiosource_audio_content.pitch + Time.deltaTime * 3.0f);
    		audiosource_audio_content.volume = Preferences.voice_volume;
    		audiosource_audio_content.pitch = Mathf.Min(1.0f,audiosource_audio_content.pitch + Time.deltaTime * 3.0f);
    		//audiosource_audio_content.pitch = 10.0;
    		//audiosource_audio_content.volume = 0.1;
    		if(start_tape_delay > 0.0f){
    			if(!audiosource_audio_content.isPlaying){
    				start_tape_delay = Mathf.Max(0.0f, start_tape_delay - Time.deltaTime);
    				if(start_tape_delay == 0.0f){
    					audiosource_audio_content.Play();
    				}
    			}
    		} else if(stop_tape_delay > 0.0f){
    			stop_tape_delay = Mathf.Max(0.0f, stop_tape_delay - Time.deltaTime);
    			if(stop_tape_delay == 0.0f){
    				tape_in_progress = false;
    				tapes_heard.Add(audiosource_audio_content.clip);
    				StopTapePlay();
    				if(tapes_heard.Count == total_tapes.Count){
    					StartWin();
    				}
    			}
    		} else if(!audiosource_audio_content.isPlaying){
    			stop_tape_delay = UnityEngine.Random.Range(0.5f,3.0f);
    		}
    	}
    }
    
    public void UpdateHealth() {
    	if(dying){
    		health -= Time.deltaTime;
    	}
    	if(health <= 0.0f){
    		health = 0.0f;
    		SetDead(true);
    		dying = false;
    	}
    }
    
    public void UpdateHelpToggle() {
    	if(character_input.GetButton("Help Toggle")){
    		help_hold_time += Time.deltaTime;
    		if(show_help && help_hold_time >= 1.0f){
    			show_advanced_help = true;
    		}
    	}
    	if(character_input.GetButtonDown("Help Toggle")){
    		if(!show_help){
    			show_help = true;
    			help_ever_shown = true;
    			just_started_help = true;
    		}
    		help_hold_time = 0.0f;
    	}
    	if(character_input.GetButtonUp("Help Toggle")){
    		if(show_help && help_hold_time < 1.0f && !just_started_help){
    			show_help = false;
    			show_advanced_help = false;
    		}
    		just_started_help = false;
    	}
    }
    
    public void UpdateLevelResetButton() {
    	if(character_input.GetButtonDown("Level Reset")){
    		level_reset_hold = 0.01f;
    	}
    	if(level_reset_hold != 0.0f && Input.GetButton("Level Reset")){
    		level_reset_hold += Time.deltaTime; 
    		dead_volume_fade = Mathf.Min(1.0f-level_reset_hold * 0.5f, dead_volume_fade);
    		dead_fade = level_reset_hold * 0.5f;
    		if(level_reset_hold >= 2.0f){
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    			level_reset_hold = 0.0f;
    		}
    	} else {
    		level_reset_hold = 0.0f;
    	}
    }
    
    public void UpdateLevelEndEffects() {
    	if(won){
    		win_fade = Mathf.Min(1.0f, win_fade + Time.deltaTime * 0.1f);
    		dead_volume_fade = Mathf.Max(0.0f, dead_volume_fade - Time.deltaTime * 0.1f);
    	} else if(dead){
    		dead_fade = Mathf.Min(1.0f, dead_fade + Time.deltaTime * 0.3f);
    		dead_volume_fade = Mathf.Max(0.0f, dead_volume_fade - Time.deltaTime * 0.23f);
    		head_fall_vel -= 9.8f * Time.deltaTime;
    		head_fall += head_fall_vel * Time.deltaTime;
    		head_tilt += head_tilt_vel * Time.deltaTime;
    		view_rotation_x += head_tilt_x_vel * Time.deltaTime;
    		view_rotation_y += head_tilt_y_vel * Time.deltaTime;
    		float min_fall = character_controller.height * character_controller.transform.localScale.y * -1.0f;
    		if(head_fall < min_fall && head_fall_vel < 0.0f){			
    			if(Mathf.Abs(head_fall_vel) > 0.5f){
    				head_recoil_spring_x.vel += UnityEngine.Random.Range(-10,10) * Mathf.Abs(head_fall_vel);
    				head_recoil_spring_y.vel += UnityEngine.Random.Range(-10,10) * Mathf.Abs(head_fall_vel);
    				head_tilt_vel = 0.0f;
    				head_tilt_x_vel = 0.0f;
    				head_tilt_y_vel = 0.0f;
    				if(!dead_body_fell){
    					PlaySoundFromGroup(sound_body_fall, 1.0f);
    					dead_body_fell = true;
    				}
    			}
    			head_fall_vel *= -0.3f;
    		}
    		head_fall = Mathf.Max(min_fall,head_fall);
    	} else {
    		dead_fade = Mathf.Max(0.0f, dead_fade - Time.deltaTime * 1.5f);
    		dead_volume_fade = Mathf.Min(1.0f, dead_volume_fade + Time.deltaTime * 1.5f);
    	}
    }
    
    public void UpdateLevelChange() {
    	if((dead && dead_volume_fade <= 0.0f)){ 
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    	}
    	if(won && dead_volume_fade <= 0.0f){ 
            UnityEngine.SceneManagement.SceneManager.LoadScene("winscene");
    	}
    }
    
    public void UpdateFallOffMapDeath() {
    	if(transform.position.y < -1){
    		InstaKill();
    	}
    }
    
    public void UpdateAimSpring() {
    	bool offset_aim_target = false;
    	if((character_input.GetButton("Hold To Aim") || aim_toggle) && !dead && (gun_instance != null)){
    		aim_spring.target_state = 1.0f;
    		RaycastHit hit = new RaycastHit();
    		if(Physics.Linecast(main_camera.transform.position, AimPos() + AimDir() * 0.2f, out hit, 1 << 0)){
    			aim_spring.target_state = Mathf.Clamp(
    				1.0f - (Vector3.Distance(hit.point, main_camera.transform.position)/(GunDist() + 0.2f)),
    				0.0f,
    				1.0f);
    			offset_aim_target = true;
    		}
    	} else {
    		aim_spring.target_state = 0.0f;
    	}
    	aim_spring.Update();
    	if(offset_aim_target){
    		aim_spring.target_state = 1.0f;
    	}
    }
    
    public void UpdateCameraRotationControls() {
    	rotation_y_min_leeway = Mathf.Lerp(0.0f,kRotationYMinLeeway,aim_spring.state);
    	rotation_y_max_leeway = Mathf.Lerp(0.0f,kRotationYMaxLeeway,aim_spring.state);
    	rotation_x_leeway = Mathf.Lerp(0.0f,kRotationXLeeway,aim_spring.state);
    	
    	if(PlayerPrefs.GetInt("lock_gun_to_center", 0)==1){
    		rotation_y_min_leeway = 0.0f;
    		rotation_y_max_leeway = 0.0f;
    		rotation_x_leeway = 0.0f;
    	}
    	
    	sensitivity_x = Preferences.mouse_sensitivity * 10.0f;
    	sensitivity_y = Preferences.mouse_sensitivity * 10.0f;
    	if(PlayerPrefs.GetInt("mouse_invert", 0) == 1){
    		sensitivity_y = -Mathf.Abs(sensitivity_y);
    	} else {
    		sensitivity_y = Mathf.Abs(sensitivity_y);
    	}
    	
    	bool in_menu = optionsmenuscript.IsMenuShown();
    	if(!dead && !in_menu){
    		rotation_x += character_input.GetAxis("Mouse X") * sensitivity_x;
    		rotation_y += character_input.GetAxis("Mouse Y") * sensitivity_y;
    		rotation_y = Mathf.Clamp (rotation_y, min_angle_y, max_angle_y);
    	
    		if((character_input.GetButton("Hold To Aim") || aim_toggle) && (gun_instance != null)){
    			view_rotation_y = Mathf.Clamp(view_rotation_y, rotation_y - rotation_y_min_leeway, rotation_y + rotation_y_max_leeway);
    			view_rotation_x = Mathf.Clamp(view_rotation_x, rotation_x - rotation_x_leeway, rotation_x + rotation_x_leeway);
    		} else {
    			view_rotation_x += character_input.GetAxis("Mouse X") * sensitivity_x;
    			view_rotation_y += character_input.GetAxis("Mouse Y") * sensitivity_y;
    			view_rotation_y = Mathf.Clamp (view_rotation_y, min_angle_y, max_angle_y);
    			
    			rotation_y = Mathf.Clamp(rotation_y, view_rotation_y - rotation_y_max_leeway, view_rotation_y + rotation_y_min_leeway);
    			rotation_x = Mathf.Clamp(rotation_x, view_rotation_x - rotation_x_leeway, view_rotation_x + rotation_x_leeway);
    		}
    	}
    }
    
    public void UpdateCameraAndPlayerTransformation() {
    	main_camera.transform.localEulerAngles = new Vector3(-view_rotation_y, view_rotation_x, head_tilt);
    	main_camera.transform.localEulerAngles += new Vector3(head_recoil_spring_y.state, head_recoil_spring_x.state, 0.0f); 
    	var tmp_cs1 = character_controller.transform.localEulerAngles;
        tmp_cs1.y = view_rotation_x;
        character_controller.transform.localEulerAngles = tmp_cs1;
    	main_camera.transform.position = transform.position;
    	var tmp_cs2 = main_camera.transform.position;
        tmp_cs2.y += character_controller.height * character_controller.transform.localScale.y - 0.1f;
        tmp_cs2.y += head_fall;
        main_camera.transform.position = tmp_cs2;
    }
    
    public void UpdateGunTransformation() {
    	Vector3 aim_dir = AimDir();
    	Vector3 aim_pos = AimPos();	
    	
    	Vector3 unaimed_dir = (transform.forward + new Vector3(0.0f,-1.0f,0.0f)).normalized;
    	Vector3 unaimed_pos = main_camera.transform.position + unaimed_dir*GunDist();
    	 
    	gun_instance.transform.position = mix(unaimed_pos, aim_pos, aim_spring.state);
    	gun_instance.transform.forward = mix(unaimed_dir, aim_dir, aim_spring.state);
      	
    	ApplyPose("pose_slide_pull", slide_pose_spring.state);
    	ApplyPose("pose_reload", reload_pose_spring.state);
    	ApplyPose("pose_press_check", press_check_pose_spring.state);
    	ApplyPose("pose_inspect_cylinder", inspect_cylinder_pose_spring.state);
    	ApplyPose("pose_add_rounds", add_rounds_pose_spring.state);
    	ApplyPose("pose_eject_rounds", eject_rounds_pose_spring.state);
    	ApplyPose("pose_alternative_stance", alternative_stance_pose_spring.state);
    
    	gun_instance.transform.RotateAround(
    		gun_instance.transform.Find("point_recoil_rotate").position,
    		gun_instance.transform.rotation * new Vector3(1.0f,0.0f,0.0f),
    		x_recoil_spring.state);
    		
    	gun_instance.transform.RotateAround(
    		gun_instance.transform.Find("point_recoil_rotate").position,
    		new Vector3(0.0f,1.0f,0.0f),
    		y_recoil_spring.state); 
    }
    
    public void UpdateFlashlightTransformation() {
    	Vector3 flashlight_hold_pos = main_camera.transform.position + main_camera.transform.rotation*new Vector3(-0.15f,-0.01f,0.15f);
    	Quaternion flashlight_hold_rot = main_camera.transform.rotation;
    	
    	Vector3 flashlight_pos = flashlight_hold_pos;
    	Quaternion flashlight_rot = flashlight_hold_rot;
    
    	held_flashlight.transform.position = flashlight_pos;
    	held_flashlight.transform.rotation = flashlight_rot;
    	
    	held_flashlight.transform.RotateAround(
    		held_flashlight.transform.Find("point_recoil_rotate").position,
    		held_flashlight.transform.rotation * new Vector3(1.0f,0.0f,0.0f),
    		x_recoil_spring.state * 0.3f);
    		
    	held_flashlight.transform.RotateAround(
    		held_flashlight.transform.Find("point_recoil_rotate").position,
    		new Vector3(0.0f,1.0f,0.0f),
    		y_recoil_spring.state * 0.3f);
    
    	flashlight_pos = held_flashlight.transform.position;
    	flashlight_rot = held_flashlight.transform.rotation;
    		
    	if(gun_instance != null){
    		flashlight_aim_pos = gun_instance.transform.position + gun_instance.transform.rotation*new Vector3(0.07f,-0.03f,0.0f);
    		flashlight_aim_rot = gun_instance.transform.rotation;
    		
    		flashlight_aim_pos -= main_camera.transform.position;
    		flashlight_aim_pos = Quaternion.Inverse(main_camera.transform.rotation) * flashlight_aim_pos;
    		flashlight_aim_rot = Quaternion.Inverse(main_camera.transform.rotation) * flashlight_aim_rot;
    	}
    
    	flashlight_pos = mix(flashlight_pos, main_camera.transform.rotation * flashlight_aim_pos + main_camera.transform.position, aim_spring.state);
    	flashlight_rot = mix(flashlight_rot, main_camera.transform.rotation * flashlight_aim_rot, aim_spring.state);
    	 
    	Vector3 flashlight_mouth_pos = main_camera.transform.position + main_camera.transform.rotation*new Vector3(0.0f,-0.08f,0.05f);
    	Quaternion flashlight_mouth_rot = main_camera.transform.rotation;
    	
    	flashlight_mouth_spring.target_state = 0.0f;
    	if(magazine_instance_in_hand != null){
    		flashlight_mouth_spring.target_state = 1.0f;
    	}
    	flashlight_mouth_spring.target_state = Mathf.Max(flashlight_mouth_spring.target_state,
    		(inspect_cylinder_pose_spring.state + eject_rounds_pose_spring.state + (press_check_pose_spring.state/0.6f) + (reload_pose_spring.state/0.7f) + slide_pose_spring.state) * aim_spring.state);
    	
    	flashlight_mouth_spring.Update();
    	 
    	flashlight_pos = mix(flashlight_pos, flashlight_mouth_pos, flashlight_mouth_spring.state);
    	flashlight_rot = mix(flashlight_rot, flashlight_mouth_rot, flashlight_mouth_spring.state);
    	
    	flashlight_pos = mix(flashlight_pos, flash_ground_pos, flash_ground_pose_spring.state);
       	flashlight_rot = mix(flashlight_rot, flash_ground_rot, flash_ground_pose_spring.state);
       		
    	held_flashlight.transform.position = flashlight_pos;
    	held_flashlight.transform.rotation = flashlight_rot;
    }
    
    public void UpdateMagazineTransformation() {
    	if(gun_instance != null){
    		mag_pos = gun_instance.transform.position;
    		mag_rot = gun_instance.transform.rotation;
    		mag_pos += (gun_instance.transform.Find("point_mag_to_insert").position - 
    				    gun_instance.transform.Find("point_mag_inserted").position);
        }
       if(mag_stage == HandMagStage.HOLD || mag_stage == HandMagStage.HOLD_TO_INSERT){
       		mag_script mag_script = magazine_instance_in_hand.GetComponent<mag_script>();
       		Vector3 hold_pos = main_camera.transform.position + main_camera.transform.rotation*mag_script.hold_offset;
    		Quaternion hold_rot = main_camera.transform.rotation * Quaternion.AngleAxis(mag_script.hold_rotation.x, new Vector3(0.0f,1.0f,0.0f)) * Quaternion.AngleAxis(mag_script.hold_rotation.y, new Vector3(1.0f,0.0f,0.0f));
       		hold_pos = mix(hold_pos, mag_ground_pos, mag_ground_pose_spring.state);
    	   	hold_rot = mix(hold_rot, mag_ground_rot, mag_ground_pose_spring.state);
       		if(hold_pose_spring.state != 1.0f){ 
       			float amount = hold_pose_spring.state;
    	   		magazine_instance_in_hand.transform.position = mix(mag_pos, hold_pos, amount);
    			magazine_instance_in_hand.transform.rotation = mix(mag_rot, hold_rot, amount);
    		} else {
    	   		magazine_instance_in_hand.transform.position = hold_pos;
    			magazine_instance_in_hand.transform.rotation = hold_rot;
    		}
    	} else {
    		magazine_instance_in_hand.transform.position = mag_pos;
    		magazine_instance_in_hand.transform.rotation = mag_rot;
    	} 
    }
    
    public void UpdateInventoryTransformation() {
    	int i = 0;
    	WeaponSlot slot = null;
        for(i=0; i<10; ++i){
    		slot = weapon_slots[i];
    		if(slot.type == WeaponSlotType.EMPTY){
    			continue;
    		}
    		slot.obj.transform.localScale = new Vector3(1.0f,1.0f,1.0f); 
    	}
    	for(i=0; i<10; ++i){
    		slot = weapon_slots[i];
    		if(slot.type == WeaponSlotType.EMPTY){
    			continue;
    		}
    		Vector3 start_pos = main_camera.transform.position + slot.start_pos;
    		Quaternion start_rot = main_camera.transform.rotation * slot.start_rot;
    		if(slot.type == WeaponSlotType.EMPTYING){
    			start_pos = slot.obj.transform.position;
    			start_rot = slot.obj.transform.rotation;
    			if(Mathf.Abs(slot.spring.vel) <= 0.01f && slot.spring.state <= 0.01f){
    				slot.type = WeaponSlotType.EMPTY;
    				slot.spring.state = 0.0f;
    			}
    		} 
    		float scale = 0.0f;
    		Vector3 target_pos = main_camera.transform.position;
    		if(main_camera.GetComponent<Camera>() != null){
    			target_pos += main_camera.GetComponent<Camera>().ScreenPointToRay(new Vector3(main_camera.GetComponent<Camera>().pixelWidth * (0.05f + i*0.15f), main_camera.GetComponent<Camera>().pixelHeight * 0.17f,0.0f)).direction * 0.3f;
    		}
    		slot.obj.transform.position = mix(
    			start_pos, 
    			target_pos, 
    			slot.spring.state);
    		scale = 0.3f * slot.spring.state + (1.0f - slot.spring.state);
    		var tmp_cs3 = slot.obj.transform.localScale;
            tmp_cs3.x *= scale;
            tmp_cs3.y *= scale;
            tmp_cs3.z *= scale;
            slot.obj.transform.localScale = tmp_cs3; 
    		slot.obj.transform.rotation = mix(
    			start_rot, 
    			main_camera.transform.rotation * Quaternion.AngleAxis(90.0f, new Vector3(0.0f,1.0f,0.0f)), 
    			slot.spring.state);
    		Renderer[] renderers = slot.obj.GetComponentsInChildren<Renderer>();
    		foreach(Renderer renderer in renderers){
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    		}
    		slot.spring.Update();
    	}
    }
    
    public void UpdateLooseBulletDisplay() {
    	bool can_add_rounds = gun_instance && gun_instance.GetComponent<GunScript>().IsAddingRounds();
    	if((mag_stage == HandMagStage.HOLD && (gun_instance == null)) || picked_up_bullet_delay > 0.0f || can_add_rounds){
    		show_bullet_spring.target_state = 1.0f;
    		picked_up_bullet_delay = Mathf.Max(0.0f, picked_up_bullet_delay - Time.deltaTime);
    	} else {	
    		show_bullet_spring.target_state = 0.0f;
    	}
    	show_bullet_spring.Update();
    	
    	for(int i=0; i<loose_bullets.Count; ++i){
    		Spring spring = loose_bullet_spring[i];
    		spring.Update();
    		GameObject bullet = loose_bullets[i];
    		bullet.transform.position = main_camera.transform.position;
    		if(main_camera.GetComponent<Camera>() != null){
    			bullet.transform.position += main_camera.GetComponent<Camera>().ScreenPointToRay(new Vector3(0.0f, (float)main_camera.GetComponent<Camera>().pixelHeight,0.0f)).direction * 0.3f;
    		}
    		bullet.transform.position += main_camera.transform.rotation * new Vector3(0.02f,-0.01f,0.0f);
    		bullet.transform.position += main_camera.transform.rotation * new Vector3(0.006f * i,0.0f,0.0f);
    		bullet.transform.position += main_camera.transform.rotation * new Vector3(-0.03f,0.03f,0.0f) * (1.0f - show_bullet_spring.state);
    		var tmp_cs4 = bullet.transform.localScale;
            tmp_cs4.x = spring.state;
            tmp_cs4.y = spring.state;
            tmp_cs4.z = spring.state;
            bullet.transform.localScale = tmp_cs4;
    		bullet.transform.rotation = main_camera.transform.rotation * Quaternion.AngleAxis(90.0f, new Vector3(-1.0f,0.0f,0.0f));
    		Renderer[] renderers = bullet.GetComponentsInChildren<Renderer>();
    		foreach(Renderer renderer in renderers){
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    		}
    	}
    }
    
    public void UpdateSprings() {	
    	slide_pose_spring.Update();
    	reload_pose_spring.Update();
    	press_check_pose_spring.Update();
    	inspect_cylinder_pose_spring.Update();
    	add_rounds_pose_spring.Update();
    	eject_rounds_pose_spring.Update();
    	alternative_stance_pose_spring.Update();
    	x_recoil_spring.Update();
    	y_recoil_spring.Update();
    	head_recoil_spring_x.Update();
    	head_recoil_spring_y.Update();
    	if(mag_stage == HandMagStage.HOLD || mag_stage == HandMagStage.HOLD_TO_INSERT){
    		hold_pose_spring.Update();
    		mag_ground_pose_spring.Update();
    	}
    	flash_ground_pose_spring.Update();
    }
    
    public void UpdatePickupMagnet() {
    	Vector3 attract_pos = transform.position - new Vector3(0.0f,character_controller.height * 0.2f,0.0f);
    	for(int i=0; i<items_being_picked_up.Count; ++i){
    		GameObject round = items_being_picked_up[i];
    		if(round == null){
    			continue;
    		}
    		round.GetComponent<Rigidbody>().velocity += (attract_pos - round.transform.position) * Time.deltaTime * 20.0f;
    		round.GetComponent<Rigidbody>().velocity *= Mathf.Pow(0.1f, Time.deltaTime);
    		//round.rigidbody.position += round.rigidbody.velocity * Time.deltaTime;
    		if(Vector3.Distance(round.transform.position, attract_pos) < 0.5f){
    			if(round.gameObject.name == "cassette_tape(Clone)"){
    				++unplayed_tapes;
    			} else {
    				AddLooseBullet(true);
    				items_being_picked_up.Remove(round);
    				PlaySoundFromGroup(sound_bullet_grab, 0.2f);
    			}
    			GameObject.Destroy(round);
    		}
    	}
    	items_being_picked_up.Remove(null);
    }
    
    public void Update() {
    	if(main_client_control){
    		UpdateTape();
    		UpdateCheats();
    	}
    	UpdateFallOffMapDeath();
    	UpdateHealth();
    	if(main_client_control){
    		UpdateHelpToggle();	
    		UpdateLevelResetButton();
    		UpdateLevelChange();
    	}
    	UpdateLevelEndEffects();
    	if(main_client_control){
    		AudioListener.volume = dead_volume_fade * Preferences.master_volume;
    	}
    	FPSInputControllerUpdate();
    	PlatformInputControllerUpdate();
    	CharacterMotorUpdate();
    	UpdateAimSpring();
    	UpdateCameraRotationControls();
    	UpdateCameraAndPlayerTransformation();	
    	if(gun_instance != null){
    		UpdateGunTransformation();
    	}
    	if(held_flashlight != null){
    		UpdateFlashlightTransformation();
    	}				
    	if(magazine_instance_in_hand != null){
    		UpdateMagazineTransformation();
    	}
    	UpdateInventoryTransformation();
    	UpdateLooseBulletDisplay();
    	bool in_menu = optionsmenuscript.IsMenuShown();
    	if(!dead && !in_menu){
    		HandleControls();
    	}
    	UpdateSprings();	
    	UpdatePickupMagnet();
    }
    
    public void FixedUpdate() {
    	CharacterMotorFixedUpdate();
}

    public bool ShouldHolsterGun() {
    	if(loose_bullets == null){
    		return false;
    	}
    	if(loose_bullets.Count > 0){
    	} else return false;
    	if(magazine_instance_in_hand != null){
    	} else return false;
    	if(magazine_instance_in_hand.GetComponent<mag_script>().NumRounds() == 0){
    	} else return false;
    	return true;
    }
    
    public bool CanLoadBulletsInMag() {
    	return (gun_instance == null) && mag_stage == HandMagStage.HOLD && loose_bullets.Count > 0 && !magazine_instance_in_hand.GetComponent<mag_script>().IsFull();
    }
    
    public bool CanRemoveBulletFromMag() {
    	return (gun_instance == null) && mag_stage == HandMagStage.HOLD && magazine_instance_in_hand.GetComponent<mag_script>().NumRounds() > 0;
    }
    
    public bool ShouldDrawWeapon() {
    	return (gun_instance == null) && !CanLoadBulletsInMag();
    }
    
    public int GetMostLoadedMag() {
    	int max_rounds = 0;
    	int max_rounds_slot = -1;
    	for(int i=0; i<10; ++i){
    		if(weapon_slots[i].type == WeaponSlotType.MAGAZINE){
    			int rounds = weapon_slots[i].obj.GetComponent<mag_script>().NumRounds();
    			if(rounds > max_rounds){
    				max_rounds_slot = i+1;
    				max_rounds = rounds;
    			}
    		}
    	}
    	return max_rounds_slot;
    }
    
    public bool ShouldPutMagInInventory() {
    	int rounds = magazine_instance_in_hand.GetComponent<mag_script>().NumRounds();
    	int most_loaded = GetMostLoadedMag();
    	if(most_loaded == -1){
    		return false;
    	}
    	if(weapon_slots[most_loaded-1].obj.GetComponent<mag_script>().NumRounds() > rounds){
    		return true;
    	}
    	return false;
    }
    
    public int GetEmptySlot() {
    	int empty_slot = -1;
    	for(int i=0; i<10; ++i){
    		if(weapon_slots[i].type == WeaponSlotType.EMPTY){
    			empty_slot = i+1;
    			break;
    		}
    	}
    	return empty_slot;
    }
    
    public int GetFlashlightSlot() {
    	int flashlight_slot = -1;
    	for(int i=0; i<10; ++i){
    		if(weapon_slots[i].type == WeaponSlotType.FLASHLIGHT){
    			flashlight_slot = i+1;
    			break;
    		}
    	}
    	return flashlight_slot;
    }
    
    /// <summary> Draws a line in the help menu, can only be called from within OnGUI </summary>
    private void DrawHelpLine(string text, bool bold = false) {
        float width = Screen.width * 0.5f;
        help_text_style.fontStyle = bold ? FontStyle.Bold : FontStyle.Normal;
        help_text_style.fontSize = 18;

        help_text_style.normal.textColor = Color.black;
        GUI.Label(new Rect(width - 4f, help_text_offset + 0.5f, width + 0.5f, help_text_offset + 20 + 0.5f), text, help_text_style);
    
        help_text_style.normal.textColor = bold ? Color.white : help_normal_color;
        GUI.Label(new Rect(width - 4.5f, help_text_offset, width, help_text_offset + 30), text, help_text_style);
        help_text_offset += 20;
    }

    public void OnGUI() {
    	if(main_client_control && Event.current.type == EventType.Repaint){
    		GunScript gun_script = null;
    		if(gun_instance != null){
    			gun_script = gun_instance.GetComponent<GunScript>();
    		}

    		help_text_offset = 0f;

    		DrawHelpLine($"{tapes_heard.Count} tapes absorbed out of {total_tapes.Count}", true);
    		if(!show_help){
    			DrawHelpLine("View help: Press [ ? ]", !help_ever_shown);
    		} else {
    			DrawHelpLine("Hide help: Press [ ? ]");
    			DrawHelpLine("");
    			if(tape_in_progress){
    				DrawHelpLine("Pause/Resume tape player: [ x ]");
    			}
    			
    			DrawHelpLine("Look: [ move mouse ]");
    			DrawHelpLine("Move: [ WASD ]");
    			DrawHelpLine("Jump: [ space ]");
    			DrawHelpLine("Pick up nearby: hold [ g ]", ShouldPickUpNearby());
                if(held_flashlight != null){
    				int empty_slot = GetEmptySlot();
    				if(empty_slot != -1){
    					DrawHelpLine($"Put flashlight in inventory: tap [ {empty_slot} ]");
    				}
                    if(gun_instance == null && mag_stage == HandMagStage.EMPTY){
                        DrawHelpLine("Drop flashlight: tap [ e ]");
                        if(held_flashlight.GetComponent<FlashlightScript>().switch_on){
                            DrawHelpLine("Turn off flashlight: tap [ v ]");
                        } else {
                            DrawHelpLine("Turn on flashlight: tap [ v ]");
                        }
                    }
    			} else {
    				int flashlight_slot = GetFlashlightSlot();
    				if(flashlight_slot != -1){
    					DrawHelpLine($"Equip flashlight: tap [ {flashlight_slot} ]", true);
    				}
    			}
    			if(gun_instance != null){
    				DrawHelpLine("Fire weapon: tap [ left mouse button ]");
    				bool should_aim = (aim_spring.state < 0.5f);
    				DrawHelpLine("Aim weapon: hold [ right mouse button ]", should_aim);
    				DrawHelpLine("Aim weapon: tap [ q ]", should_aim);
    				DrawHelpLine("Holster weapon: tap [ ~ ]", ShouldHolsterGun());
    			} else {
    				DrawHelpLine("Draw weapon: tap [ ~ ]", ShouldDrawWeapon());
    			}
                if(gun_instance != null){
    				if(gun_script.HasSlide()){
						if(gun_script.Query(GunSystemQueries.IS_WAITING_FOR_SLIDE_PUSH)) {
    						DrawHelpLine("Push forward slide: tap [ r ]",  gun_script.ShouldPushSlideForward());
						} else {
    						DrawHelpLine("Pull back slide: hold [ r ]", gun_script.ShouldPullSlide());
						}
    					if(gun_script.HasGunComponent(GunAspect.SLIDE_RELEASE_BUTTON)) {
    						DrawHelpLine("Release slide lock: tap [ t ]", gun_script.ShouldReleaseSlideLock());
    					}
    				}
    				if(gun_script.HasSafety()){
    					DrawHelpLine("Toggle safety: tap [ v ]", gun_script.IsSafetyOn());
    				}
    				if(gun_script.HasAutoMod()){
    					DrawHelpLine("Toggle full-automatic: tap [ v ]", gun_script.ShouldToggleAutoMod());
    				}
    				if(gun_script.HasHammer()){
    					DrawHelpLine("Pull back hammer: hold [ f ]", gun_script.ShouldPullBackHammer());
    				}
    				if(gun_script.HasGunComponent(GunAspect.LOCKABLE_BOLT)){
    					DrawHelpLine("Toggle Bolt: tap [ t ]", gun_script.ShouldToggleBolt());
    				}
    				if(gun_script.HasGunComponent(GunAspect.ALTERNATIVE_STANCE)){
    					DrawHelpLine("Switch holdingstyle: tap [ f ]", gun_script.ShouldToggleStance());
    				}
    				if(gun_script.HasGunComponent(GunAspect.REVOLVER_CYLINDER)){
    					if(!gun_script.IsCylinderOpen()){
    						DrawHelpLine("Open cylinder: tap [ e ]", (gun_script.ShouldOpenCylinder() && loose_bullets.Count!=0));
    					} else {
    						DrawHelpLine("Close cylinder: tap [ r ]", (gun_script.ShouldCloseCylinder() || loose_bullets.Count==0));
    						DrawHelpLine("Extract casings: hold [ v ]", gun_script.ShouldExtractCasings());
    						DrawHelpLine("Insert bullet: tap [ z ]", (gun_script.ShouldInsertBullet() && loose_bullets.Count!=0));
    					}
    					DrawHelpLine("Spin cylinder: [ mousewheel ]");
    				} else if(gun_script.HasGunComponent(GunAspect.MANUAL_LOADING)) {
    					DrawHelpLine("Insert bullet: tap [ z ]", (gun_script.ShouldInsertBullet() && loose_bullets.Count!=0));
    				}
    				if(gun_script.HasGunComponent(GunAspect.EXTERNAL_MAGAZINE)) {
    					if(mag_stage == HandMagStage.HOLD && !gun_script.IsThereAMagInGun()){
    						bool should_insert_mag = (magazine_instance_in_hand.GetComponent<mag_script>().NumRounds() >= 1);
    						DrawHelpLine("Insert magazine: tap [ z ]", should_insert_mag);
    					} else if(mag_stage == HandMagStage.EMPTY && gun_script.IsThereAMagInGun()){
    						DrawHelpLine("Eject magazine: tap [ e ]", gun_script.ShouldEjectMag());
    					} else if(mag_stage == HandMagStage.EMPTY && !gun_script.IsThereAMagInGun()){
    						int max_rounds_slot = GetMostLoadedMag();
    						if(max_rounds_slot != -1){
    							DrawHelpLine($"Equip magazine: tap [ {max_rounds_slot} ]", true);
    						}
    					}
    				}
    			} else {
    				if(CanLoadBulletsInMag()){
    					DrawHelpLine("Insert bullet in magazine: tap [ z ]", true);
    				}
    				if(CanRemoveBulletFromMag()){
    					DrawHelpLine("Remove bullet from magazine: tap [ r ]");
    				}
    			}
    			if(mag_stage == HandMagStage.HOLD){
    				int empty_slot = GetEmptySlot();
    				if(empty_slot != -1){
    					DrawHelpLine($"Put magazine in inventory: tap [ {empty_slot} ]", ShouldPutMagInInventory());
    				}
    				DrawHelpLine("Drop magazine: tap [ e ]");
    			}
    			
    			DrawHelpLine("");
    			if(show_advanced_help){
    				DrawHelpLine("Advanced help:");
    				DrawHelpLine("Toggle crouch: [ c ]");
    				if(aim_spring.state < 0.5f){
    					DrawHelpLine("Run: tap repeatedly [ w ]");
    				}
    				if(gun_instance != null){
    					if(!gun_script.IsSafetyOn() && gun_script.IsHammerCocked()){
    						DrawHelpLine("Decock: Hold [f], hold [LMB], release [f]", ShouldPickUpNearby());
    					}
    					if(!gun_script.IsSlideLocked() && !gun_script.IsSafetyOn()){
    						DrawHelpLine("Inspect chamber: hold [ t ] and then [ r ]");
    					}
    					if(mag_stage == HandMagStage.EMPTY && !gun_script.IsThereAMagInGun()){
    						int max_rounds_slot = GetMostLoadedMag();
    						if(max_rounds_slot != -1){
    							DrawHelpLine($"Quick load magazine: double tap [ {max_rounds_slot} ]");
    						}
    					}
    				}
    				DrawHelpLine("Reset game: hold [ l ]");
    			} else {
    				DrawHelpLine("Advanced help: Hold [ ? ]");
    			}
    		}

    		if(hasCheated) {
    			DrawHelpLine("");
    			DrawHelpLine("Cheats used", true);

    			if(god_mode)
    				DrawHelpLine("God Mode enabled", true);
    			if(slomo_mode)
    				DrawHelpLine("Slomo Mode enabled", Time.timeScale == 0.1f);
    		}

    		if(slomoWarningDuration > 0) {
    			DrawHelpLine("Slomo button requires slomo cheat!");
    			slomoWarningDuration -= Time.deltaTime * 0.2f;
    		}

    		if(dead_fade > 0.0f){
    		    if(texture_death_screen == null){
    		        Debug.LogError("Assign a Texture in the inspector.");
    		        return;
    		    }
    		    GUI.color = new Color(0.0f,0.0f,0.0f,dead_fade);
    		    GUI.DrawTexture(new Rect(0.0f,0.0f,(float)Screen.width,(float)Screen.height), texture_death_screen, ScaleMode.StretchToFill, true);
    		}
    		if(win_fade > 0.0f){
    		    GUI.color = new Color(1.0f,1.0f,1.0f,win_fade);
    		    GUI.DrawTexture(new Rect(0.0f,0.0f,(float)Screen.width,(float)Screen.height), texture_death_screen, ScaleMode.StretchToFill, true);
    		}
    	}
    }
    
    float forward_input_delay = 10.0f;
    float old_vert_axis = 0.0f;
    bool bool_running = false;
    
    // Update is called once per frame
    public void PlatformInputControllerUpdate() {
    	// Get the input vector from kayboard or analog stick
    	Vector3 directionVector = new Vector3(character_input.GetAxis("Horizontal"), 0.0f, character_input.GetAxis("Vertical"));
    	
    	if(old_vert_axis < 0.9f && character_input.GetAxis("Vertical") >= 0.9f){
    		if(!crouching && forward_input_delay < 0.4f && !GetComponent<AimScript>().IsAiming()){
    			SetRunning(Mathf.Clamp((0.4f-forward_input_delay)/0.2f,0.01f,1.0f));
    			bool_running = true;			
    		}
    		forward_input_delay = 0.0f;
    	}
    	forward_input_delay += Time.deltaTime;
    	if(forward_input_delay > 0.4f || GetComponent<AimScript>().IsAiming()){
    		SetRunning(0.0f);
    		bool_running = false;
    	}
    	if(bool_running){
    		directionVector.z = 1.0f;
    	}
    	old_vert_axis = character_input.GetAxis("Vertical");
    	
    	if (directionVector != Vector3.zero) {
    		// Get the length of the directon vector and then normalize it
    		// Dividing by the length is cheaper than normalizing when we already have the length anyway
    		float directionLength = directionVector.magnitude;
    		directionVector = directionVector / directionLength;
    		
    		// Make sure the length is no bigger than 1
    		directionLength = Mathf.Min(1.0f, directionLength);
    		
    		// Make the input vector more sensitive towards the extremes and less sensitive in the middle
    		// This makes it easier to control slow speeds when using analog sticks
    		directionLength = directionLength * directionLength;
    		
    		// Multiply the normalized direction vector by the modified length
    		directionVector = directionVector * directionLength;
    	}
    	
    	// Apply the direction to the CharacterMotor
    	inputMoveDirection = transform.rotation * directionVector;
    	inputJump = character_input.GetButton("Jump");	
    }
    
    // This makes the character turn to face the current movement speed per default.
    bool autoRotate = false;
    const float maxRotationSpeed = 360.0f;
    
    // Update is called once per frame
    public void FPSInputControllerUpdate() {
    	// Get the input vector from kayboard or analog stick
    	Vector3 directionVector = new Vector3(character_input.GetAxis("Horizontal"), character_input.GetAxis("Vertical"), 0.0f);
    	
    	if (directionVector != Vector3.zero) {
    		// Get the length of the directon vector and then normalize it
    		// Dividing by the length is cheaper than normalizing when we already have the length anyway
    		float directionLength = directionVector.magnitude;
    		directionVector = directionVector / directionLength;
    		
    		// Make sure the length is no bigger than 1
    		directionLength = Mathf.Min(1.0f, directionLength);
    		
    		// Make the input vector more sensitive towards the extremes and less sensitive in the middle
    		// This makes it easier to control slow speeds when using analog sticks
    		directionLength = directionLength * directionLength;
    		
    		// Multiply the normalized direction vector by the modified length
    		directionVector = directionVector * directionLength;
    	}
    	
    	// Rotate the input vector into camera space so up is camera's up and right is camera's right
    	directionVector = Camera.main.transform.rotation * directionVector;
    	
    	// Rotate input vector to be perpendicular to character's up vector
    	Quaternion camToCharacterSpace = Quaternion.FromToRotation(-Camera.main.transform.forward, transform.up);
    	directionVector = (camToCharacterSpace * directionVector);
    	
    	// Apply the direction to the CharacterMotor
    	inputMoveDirection = directionVector;
    	inputJump = character_input.GetButton("Jump");
    	
    	// Set rotation to the move direction	
    	if (autoRotate && directionVector.sqrMagnitude > 0.01f) {
    		Vector3 newForward = ConstantSlerp(
    			transform.forward,
    			directionVector,
    			maxRotationSpeed * Time.deltaTime
    		);
    		newForward = ProjectOntoPlane(newForward, transform.up);
    		transform.rotation = Quaternion.LookRotation(newForward, transform.up);
    	}
    }
    
    public Vector3 ProjectOntoPlane(Vector3 v,Vector3 normal) {
    	return v - Vector3.Project(v, normal);
    }
    
    public Vector3 ConstantSlerp(Vector3 from,Vector3 to,float angle) {
    	float value = Mathf.Min(1.0f, angle / Vector3.Angle(from, to));
    	return Vector3.Slerp(from, to, value);
    }
    
    //float kStandHeight = 2.0f;
    //float kCrouchHeight = 1.0f;
    bool crouching = false;
    float step_timer = 0.0f;
    float head_bob = 0.0f;
    
    public List<AudioClip> sound_footstep_jump_concrete;
    public List<AudioClip> sound_footstep_run_concrete;
    public List<AudioClip> sound_footstep_walk_concrete;
    public List<AudioClip> sound_footstep_crouchwalk_concrete;
    
    float running = 0.0f;
    
    Spring height_spring = new Spring(0.0f,0.0f,100.0f,0.00001f);
    Vector3 die_dir;
    
    // Does this script currently respond to input?
    bool canControl = true;
    
    bool useFixedUpdate = true;
    
    // For the next variables, @System.NonSerialized tells Unity to not serialize the variable or show it in the inspector view.
    // Very handy for organization!
    
    // The current global direction we want the character to move in.
    [System.NonSerialized]
    public Vector3 inputMoveDirection = Vector3.zero;
    
    // Is the jump button held down? We use this interface instead of checking
    // for the jump button directly so this script can also be used by AIs.
    [System.NonSerialized]
    public bool inputJump = false;
    
    public void SetRunning(float val){
    	running = val;
    }
    
    public float GetRunning(){
    	return running;
}

    public CharacterMotorMovement movement = new CharacterMotorMovement();

    public CharacterMotorJumping jumping = new CharacterMotorJumping();

    public CharacterMotorMovingPlatform movingPlatform = new CharacterMotorMovingPlatform();

    public CharacterMotorSliding sliding = new CharacterMotorSliding();
    
    [System.NonSerialized]
    public bool grounded = true;
    
    [System.NonSerialized]
    public Vector3 groundNormal = Vector3.zero;
    
    Vector3 lastGroundNormal = Vector3.zero;
    
    Transform tr;
    
    CharacterController controller;
    
    public void Awake() {
    	controller = GetComponent<CharacterController>();
    	tr = transform;
    }
    
    public Vector3 GetVelocity() {
    	return movement.velocity;
    }
    
    void UpdateFunction() {
    	// We copy the actual velocity into a temporary variable that we can manipulate.
    	Vector3 velocity = movement.velocity;
    	
    	// Update velocity based on input
    	velocity = ApplyInputVelocityChange(velocity);
    	
    	// Apply gravity and jumping force
    	velocity = ApplyGravityAndJumping (velocity);
    	
    	// Moving platform support
    	Vector3 moveDistance = Vector3.zero;
    	if (MoveWithPlatform()) {
    		Vector3 newGlobalPoint = movingPlatform.activePlatform.TransformPoint(movingPlatform.activeLocalPoint);
    		moveDistance = (newGlobalPoint - movingPlatform.activeGlobalPoint);
    		if (moveDistance != Vector3.zero)
    			controller.Move(moveDistance);
    		
    		// Support moving platform rotation as well:
            Quaternion newGlobalRotation = movingPlatform.activePlatform.rotation * movingPlatform.activeLocalRotation;
            Quaternion rotationDiff = newGlobalRotation * Quaternion.Inverse(movingPlatform.activeGlobalRotation);
            
            float yRotation = rotationDiff.eulerAngles.y;
            if (yRotation != 0) {
    	        // Prevent rotation of the local up vector
    	        tr.Rotate(0.0f, yRotation, 0.0f);
            }
    	}
    	
    	// Save lastPosition for velocity calculation.
    	Vector3 lastPosition = tr.position;
    	
    	// We always want the movement to be framerate independent.  Multiplying by Time.deltaTime does this.
    	Vector3 currentMovementOffset = velocity * Time.deltaTime;
    	
    	// Find out how much we need to push towards the ground to avoid loosing grouning
    	// when walking down a step or over a sharp change in slope.
    	float pushDownOffset = Mathf.Max(controller.stepOffset, (new Vector3(currentMovementOffset.x, 0.0f, currentMovementOffset.z)).magnitude);
    	if (grounded)
    		currentMovementOffset -= pushDownOffset * Vector3.up;
    	
    	// Reset variables that will be set by collision function
    	movingPlatform.hitPlatform = null;
    	groundNormal = Vector3.zero;
    	
       	// Move our character!
    	movement.collisionFlags = controller.Move (currentMovementOffset);
    	
    	movement.lastHitPoint = movement.hitPoint;
    	lastGroundNormal = groundNormal;
    	
    	if (movingPlatform.enabled && movingPlatform.activePlatform != movingPlatform.hitPlatform) {
    		if (movingPlatform.hitPlatform != null) {
    			movingPlatform.activePlatform = movingPlatform.hitPlatform;
    			movingPlatform.lastMatrix = movingPlatform.hitPlatform.localToWorldMatrix;
    			movingPlatform.newPlatform = true;
    		}
    	}
    	
    	Vector3 old_vel = movement.velocity;
    	
    	// Calculate the velocity based on the current and previous position.  
    	// This means our velocity will only be the amount the character actually moved as a result of collisions.
    	Vector3 oldHVelocity = new Vector3(velocity.x, 0.0f, velocity.z);
    	movement.velocity = (tr.position - lastPosition) / Time.deltaTime;
    	Vector3 newHVelocity = new Vector3(movement.velocity.x, 0.0f, movement.velocity.z);
    	
    	// The CharacterController can be moved in unwanted directions when colliding with things.
    	// We want to prevent this from influencing the recorded velocity.
    	if (oldHVelocity == Vector3.zero) {
    		movement.velocity = new Vector3(0.0f, movement.velocity.y, 0.0f);
    	}
    	else {
    		float projectedNewVelocity = Vector3.Dot(newHVelocity, oldHVelocity) / oldHVelocity.sqrMagnitude;
    		movement.velocity = oldHVelocity * Mathf.Clamp01(projectedNewVelocity) + movement.velocity.y * Vector3.up;
    	}
    	
    	if (movement.velocity.y < velocity.y - 0.001f) {
    		if (movement.velocity.y < 0) {
    			// Something is forcing the CharacterController down faster than it should.
    			// Ignore this
    			var tmp_cs5 = movement.velocity;
                tmp_cs5.y = velocity.y;
                movement.velocity = tmp_cs5;
    		}
    		else {
    			// The upwards movement of the CharacterController has been blocked.
    			// This is treated like a ceiling collision - stop further jumping here.
    			jumping.holdingJumpButton = false;
    		}
    	}
    	
    	// We were grounded but just loosed grounding
    	if (grounded && !IsGroundedTest()) {
    		grounded = false;
    		
    		// Apply inertia from platform
    		if (movingPlatform.enabled &&
    			(movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer ||
    			movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)
    		) {
    			movement.frameVelocity = movingPlatform.platformVelocity;
    			movement.velocity += movingPlatform.platformVelocity;
    		}
    		
    		SendMessage("OnFall", SendMessageOptions.DontRequireReceiver);
    		// We pushed the character down to ensure it would stay on the ground if there was any.
    		// But there wasn't so now we cancel the downwards offset to make the fall smoother.
    		tr.position += pushDownOffset * Vector3.up;
    	}
    	// We were not grounded but just landed on something
    	else if (!grounded && IsGroundedTest()) {
    		if(old_vel.y < -8.0f){
    			GetComponent<AimScript>().FallDeath(old_vel);
    		} else if(old_vel.y < 0.0f){
    			PlaySoundFromGroup(sound_footstep_jump_concrete, Mathf.Min(old_vel.y / -4.0f, 1.0f));
    			GetComponent<AimScript>().StepRecoil(-old_vel.y * 0.1f);
    		}
    		height_spring.vel = old_vel.y;
    		grounded = true;
    		jumping.jumping = false;
    		StartCoroutine(SubtractNewPlatformVelocity());
    		
    		SendMessage("OnLand", SendMessageOptions.DontRequireReceiver);
    	}
    	
    	// Moving platforms support
    	if (MoveWithPlatform()) {
    		// Use the center of the lower half sphere of the capsule as reference point.
    		// This works best when the character is standing on moving tilting platforms. 
    		movingPlatform.activeGlobalPoint = tr.position + Vector3.up * (controller.center.y - controller.height*0.5f + controller.radius);
    		movingPlatform.activeLocalPoint = movingPlatform.activePlatform.InverseTransformPoint(movingPlatform.activeGlobalPoint);
    		
    		// Support moving platform rotation as well:
            movingPlatform.activeGlobalRotation = tr.rotation;
            movingPlatform.activeLocalRotation = Quaternion.Inverse(movingPlatform.activePlatform.rotation) * movingPlatform.activeGlobalRotation; 
    	}
    }
    
    public void CharacterMotorFixedUpdate() {
    	if (movingPlatform.enabled) {
    		if (movingPlatform.activePlatform != null) {
    			if (!movingPlatform.newPlatform) {
    				//Vector3 lastVelocity = movingPlatform.platformVelocity;
    				
    				movingPlatform.platformVelocity = (
    					movingPlatform.activePlatform.localToWorldMatrix.MultiplyPoint3x4(movingPlatform.activeLocalPoint)
    					- movingPlatform.lastMatrix.MultiplyPoint3x4(movingPlatform.activeLocalPoint)
    				) / Time.deltaTime;
    			}
    			movingPlatform.lastMatrix = movingPlatform.activePlatform.localToWorldMatrix;
    			movingPlatform.newPlatform = false;
    		}
    		else {
    			movingPlatform.platformVelocity = Vector3.zero;	
    		}
    	}
    	
    	CharacterController controller = GetComponent<CharacterController>();
    	if(crouching && running == 0.0f){
    		height_spring.target_state = 0.5f + head_bob;
    	} else {
    		height_spring.target_state = 1.0f + head_bob;
    	}
    	height_spring.Update();
    	float old_height = controller.transform.localScale.y * controller.height;
    	var tmp_cs6 = controller.transform.localScale;
        tmp_cs6.y = height_spring.state;
        controller.transform.localScale = tmp_cs6;
    	float height = controller.transform.localScale.y * controller.height;
    	if(height > old_height){
    		var tmp_cs7 = controller.transform.position;
            tmp_cs7.y += height - old_height;
            controller.transform.position = tmp_cs7;
    	}
    	die_dir *= 0.93f;
    	
    	if (useFixedUpdate)
    		UpdateFunction();
    }
    
    public void CharacterMotorUpdate() {
    	if(PlayerPrefs.GetInt("toggle_crouch", 1)==1){
    		if(!GetComponent<AimScript>().IsDead() && character_input.GetButtonDown("Crouch Toggle")){
    			crouching = !crouching;
    		}
    	} else {
    		if(!GetComponent<AimScript>().IsDead()){
    			crouching = character_input.GetButton("Crouch Toggle");
    		}
    	}	
    	if(running > 0.0f){
    		crouching = false;
    	}
    	if (!useFixedUpdate)
    		UpdateFunction();
    }
    
    Vector3 ApplyInputVelocityChange(Vector3 velocity) {	
    	if (!canControl)
    		inputMoveDirection = Vector3.zero;
    	
    	// Find desired velocity
    	Vector3 desiredVelocity = Vector3.zero;
    	if (grounded && TooSteep()) {
    		// The direction we're sliding in
    		desiredVelocity = (new Vector3(groundNormal.x, 0.0f, groundNormal.z)).normalized;
    		// Find the input movement direction projected onto the sliding direction
    		Vector3 projectedMoveDir = Vector3.Project(inputMoveDirection, desiredVelocity);
    		// Add the sliding direction, the spped control, and the sideways control vectors
    		desiredVelocity = desiredVelocity + projectedMoveDir * sliding.speedControl + (inputMoveDirection - projectedMoveDir) * sliding.sidewaysControl;
    		// Multiply with the sliding speed
    		desiredVelocity *= sliding.slidingSpeed;
    	}
    	else {
    		desiredVelocity = GetDesiredHorizontalVelocity();
    	}
    	
    	if(grounded){
    		float kSoundVolumeMult = 0.8f;
    		float step_volume = movement.velocity.magnitude * 0.15f * kSoundVolumeMult;
    		step_volume = Mathf.Clamp(step_volume, 0.0f,1.0f);
    		head_bob = (Mathf.Sin(step_timer * Mathf.PI) * 0.1f - 0.05f) * movement.velocity.magnitude * 0.5f;
    		if(running > 0.0f){
    			head_bob *= 2.0f;
    		}
    		if(velocity.magnitude > 0.01f){
    			float step_speed = movement.velocity.magnitude * 0.75f;
    			if(movement.velocity.normalized.y > 0.1f){
    				step_speed += movement.velocity.normalized.y * 3.0f;
    			} else if(movement.velocity.normalized.y < -0.1f){
    				step_speed -= movement.velocity.normalized.y * 2.0f;
    			}
    			if(crouching){
    				step_speed *= 1.5f;
    			}
    			if(running == 0.0f){
    				step_speed = Mathf.Clamp(step_speed,1.0f,4.0f);
    			} else {
    				step_speed = running * 2.5f + 2.5f;
    			}
    			step_timer -= Time.deltaTime * step_speed;
    			if(step_timer < 0.0f){
    				if(crouching){
    					PlaySoundFromGroup(sound_footstep_crouchwalk_concrete, step_volume);
    				} else if(running > 0.0f){
    					PlaySoundFromGroup(sound_footstep_run_concrete, step_volume);
    				} else {
    					PlaySoundFromGroup(sound_footstep_walk_concrete, step_volume);					
    				}
    				GetComponent<AimScript>().StepRecoil(step_volume/kSoundVolumeMult);
    				step_timer = 1.0f;
    			}
    		} else if(desiredVelocity.magnitude == 0.0f && velocity.magnitude < 0.01f){
    			if(step_timer < 0.8f && step_timer != 0.5f){
    				if(crouching){
    					PlaySoundFromGroup(sound_footstep_crouchwalk_concrete, step_volume);
    				} else {
    					PlaySoundFromGroup(sound_footstep_walk_concrete, step_volume);					
    				}
    				GetComponent<AimScript>().StepRecoil(step_volume/kSoundVolumeMult);
    			}
    			step_timer = 0.5f;
    		}
    	}
    	
    	if (movingPlatform.enabled && movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer) {
    		desiredVelocity += movement.frameVelocity;
    		desiredVelocity.y = 0.0f;
    	}
    	
    	if (grounded)
    		desiredVelocity = AdjustGroundVelocityToNormal(desiredVelocity, groundNormal);
    	else
    		velocity.y = 0.0f;
    	
    	// Enforce max velocity change
    	float maxVelocityChange = GetMaxAcceleration(grounded) * Time.deltaTime;
    	Vector3 velocityChangeVector = (desiredVelocity - velocity);
    	if (velocityChangeVector.sqrMagnitude > maxVelocityChange * maxVelocityChange) {
    		velocityChangeVector = velocityChangeVector.normalized * maxVelocityChange;
    	}
    	// If we're in the air and don't have control, don't apply any velocity change at all.
    	// If we're on the ground and don't have control we do apply it - it will correspond to friction.
    	if (grounded)// || canControl)
    		velocity += velocityChangeVector;
    	
    	if (grounded) {
    		// When going uphill, the CharacterController will automatically move up by the needed amount.
    		// Not moving it upwards manually prevent risk of lifting off from the ground.
    		// When going downhill, DO move down manually, as gravity is not enough on steep hills.
    		velocity.y = Mathf.Min(velocity.y, 0.0f);
    	}
    	
    	return velocity;
    }
    
    Vector3 ApplyGravityAndJumping(Vector3 velocity) {
    	
    	if (!inputJump || !canControl) {
    		jumping.holdingJumpButton = false;
    		jumping.lastButtonDownTime = -100.0f;
    	}
    	
    	if (inputJump && jumping.lastButtonDownTime < 0 && canControl)
    		jumping.lastButtonDownTime = Time.time;
    	
    	if (grounded)
    		velocity.y = Mathf.Min(0.0f, velocity.y) - movement.gravity * Time.deltaTime;
    	else {
    		velocity.y = movement.velocity.y - movement.gravity * Time.deltaTime;
    		
    		// When jumping up we don't apply gravity for some time when the user is holding the jump button.
    		// This gives more control over jump height by pressing the button longer.
    		if (jumping.jumping && jumping.holdingJumpButton) {
    			// Calculate the duration that the extra jump force should have effect.
    			// If we're still less than that duration after the jumping time, apply the force.
    			if (Time.time < jumping.lastStartTime + jumping.extraHeight / CalculateJumpVerticalSpeed(jumping.baseHeight)) {
    				// Negate the gravity we just applied, except we push in jumpDir rather than jump upwards.
    				velocity += jumping.jumpDir * movement.gravity * Time.deltaTime;
    			}
    		}
    		
    		// Make sure we don't fall any faster than maxFallSpeed. This gives our character a terminal velocity.
    		velocity.y = Mathf.Max (velocity.y, -movement.maxFallSpeed);
    	}
    		
    	if (grounded) {
    		// Jump only if the jump button was pressed down in the last 0.2 seconds.
    		// We use this check instead of checking if it's pressed down right now
    		// because players will often try to jump in the exact moment when hitting the ground after a jump
    		// and if they hit the button a fraction of a second too soon and no new jump happens as a consequence,
    		// it's confusing and it feels like the game is buggy.
    		if (jumping.enabled && canControl && (Time.time - jumping.lastButtonDownTime < 0.2f)) {
    			PlaySoundFromGroup(sound_footstep_run_concrete, 1.0f);
    			step_timer = 0.0f;
    			crouching = false;
    			grounded = false;
    			jumping.jumping = true;
    			jumping.lastStartTime = Time.time;
    			jumping.lastButtonDownTime = -100.0f;
    			jumping.holdingJumpButton = true;
    			
    			// Calculate the jumping direction
    			if (TooSteep())
    				jumping.jumpDir = Vector3.Slerp(Vector3.up, groundNormal, jumping.steepPerpAmount);
    			else
    				jumping.jumpDir = Vector3.Slerp(Vector3.up, groundNormal, jumping.perpAmount);
    			
    			// Apply the jumping force to the velocity. Cancel any vertical velocity first.
    			velocity.y = 0.0f;
    			velocity += jumping.jumpDir * CalculateJumpVerticalSpeed (jumping.baseHeight);
    			
    			// Apply inertia from platform
    			if (movingPlatform.enabled &&
    				(movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer ||
    				movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)
    			) {
    				movement.frameVelocity = movingPlatform.platformVelocity;
    				velocity += movingPlatform.platformVelocity;
    			}
    			
    			SendMessage("OnJump", SendMessageOptions.DontRequireReceiver);
    		}
    		else {
    			jumping.holdingJumpButton = false;
    		}
    	}
    	
    	return velocity;
    }
    
    public void OnControllerColliderHit(ControllerColliderHit hit) {
    	if (hit.normal.y > 0 && hit.normal.y > groundNormal.y && hit.moveDirection.y < 0) {
    		if ((hit.point - movement.lastHitPoint).sqrMagnitude > 0.001f || lastGroundNormal == Vector3.zero)
    			groundNormal = hit.normal;
    		else
    			groundNormal = lastGroundNormal;
    		
    		movingPlatform.hitPlatform = hit.collider.transform;
    		movement.hitPoint = hit.point;
    		movement.frameVelocity = Vector3.zero;
    	}
    }
    
    IEnumerator SubtractNewPlatformVelocity() {
    	// When landing, subtract the velocity of the new ground from the character's velocity
    	// since movement in ground is relative to the movement of the ground.
    	if (movingPlatform.enabled &&
    		(movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer ||
    		movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)
    	) {
    		// If we landed on a new platform, we have to wait for two FixedUpdates
    		// before we know the velocity of the platform under the character
    		if (movingPlatform.newPlatform) {
    			Transform platform = movingPlatform.activePlatform;
    			yield return new WaitForFixedUpdate();
    			yield return new WaitForFixedUpdate();
    			if (grounded && platform == movingPlatform.activePlatform)
    				yield return 1;
    		}
    		movement.velocity -= movingPlatform.platformVelocity;
    	}
    }
    
    bool MoveWithPlatform() {
    	return (
    		movingPlatform.enabled
    		&& (grounded || movingPlatform.movementTransfer == MovementTransferOnJump.PermaLocked)
    		&& movingPlatform.activePlatform != null
    	);
    }
    
    Vector3 GetDesiredHorizontalVelocity() {
    	if(GetComponent<AimScript>().IsDead()){
    		return die_dir;
    	}
    	
    	// Find desired velocity
    	Vector3 desiredLocalDirection = tr.InverseTransformDirection(inputMoveDirection);
    	float maxSpeed = MaxSpeedInDirection(desiredLocalDirection);
    	if (grounded) {
    		// Modify max speed on slopes based on slope speed multiplier curve
    		float movementSlopeAngle = Mathf.Asin(movement.velocity.normalized.y)  * Mathf.Rad2Deg;
    		maxSpeed *= movement.slopeSpeedMultiplier.Evaluate(movementSlopeAngle);
    	}
    	die_dir = tr.TransformDirection(desiredLocalDirection * maxSpeed);
    	return die_dir;
    }
    
    Vector3 AdjustGroundVelocityToNormal(Vector3 hVelocity,Vector3 groundNormal) {
    	Vector3 sideways = Vector3.Cross(Vector3.up, hVelocity);
    	return Vector3.Cross(sideways, groundNormal).normalized * hVelocity.magnitude;
    }
    
    bool IsGroundedTest() {
    	return (groundNormal.y > 0.01f);
    }
    
    public float GetMaxAcceleration(bool grounded) {
    	// Maximum acceleration on ground and in air
    	if (grounded)
    		return movement.maxGroundAcceleration;
    	else
    		return movement.maxAirAcceleration;
    }
    
    public float CalculateJumpVerticalSpeed(float targetJumpHeight) {
    	// From the jump height and gravity we deduce the upwards speed 
    	// for the character to reach at the apex.
    	return Mathf.Sqrt (2 * targetJumpHeight * movement.gravity);
    }
    
    public bool IsJumping() {
    	return jumping.jumping;
    }
    
    public bool IsSliding() {
    	return (grounded && sliding.enabled && TooSteep());
    }
    
    public bool IsTouchingCeiling() {
    	return (int)(movement.collisionFlags & CollisionFlags.CollidedAbove) != 0;
    }
    
    public bool IsGrounded() {
    	return grounded;
    }
    
    public bool TooSteep() {
    	return (groundNormal.y <= Mathf.Cos(controller.slopeLimit * Mathf.Deg2Rad));
    }
    
    public Vector3 GetDirection() {
    	return inputMoveDirection;
    }
    
    public void SetControllable(bool controllable) {
    	canControl = controllable;
    }
    
    // Project a direction onto elliptical quater segments based on forward, sideways, and backwards speed.
    // The function returns the length of the resulting vector.
    public float MaxSpeedInDirection(Vector3 desiredMovementDirection) {
    	if (desiredMovementDirection == Vector3.zero)
    		return 0.0f;
    	else {
    		float zAxisEllipseMultiplier = (desiredMovementDirection.z > 0 ? movement.maxForwardSpeed : movement.maxBackwardsSpeed) / movement.maxSidewaysSpeed;
    		Vector3 temp = new Vector3(desiredMovementDirection.x, 0.0f, desiredMovementDirection.z / zAxisEllipseMultiplier).normalized;
    		float length = new Vector3(temp.x, 0.0f, temp.z * zAxisEllipseMultiplier).magnitude * movement.maxSidewaysSpeed;
    		return length * (crouching ? 0.5f : 1.0f) * (1.0f + running);
    	}
    }
    
    public void SetVelocity(Vector3 velocity) {
    	grounded = false;
    	movement.velocity = velocity;
    	movement.frameVelocity = Vector3.zero;
    	SendMessage("OnExternalVelocity");
    }
    
    // Require a character controller to be attached to the same game object

}

