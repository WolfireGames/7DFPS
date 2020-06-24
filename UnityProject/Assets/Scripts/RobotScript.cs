using UnityEngine;
using System.Collections.Generic;

public enum RobotType {SHOCK_DRONE, STATIONARY_TURRET, MOBILE_TURRET, GUN_DRONE};

public enum AIState {IDLE, ALERT, ALERT_COOLDOWN, AIMING, FIRING, DEACTIVATING, DEAD};

public enum CameraPivotState {DOWN, WAIT_UP, UP, WAIT_DOWN};

public class RobotScript:MonoBehaviour{
    
    private static Transform _target;

    public List<AudioClip> sound_gunshot;
    public List<AudioClip> sound_damage_camera;
    public List<AudioClip> sound_damage_gun;
    public List<AudioClip> sound_damage_battery;
    public List<AudioClip> sound_damage_ammo;
    public List<AudioClip> sound_damage_motor;
    public List<AudioClip> sound_bump;
    public AudioClip sound_alert;
    public AudioClip sound_unalert;
    public AudioClip sound_engine_loop;
    public AudioClip sound_damaged_engine_loop;

    public Transform point_pivot;
    public Transform top_rotor;
    public Transform bottom_rotor;
    public Transform point_spark;
    public Transform camera_pivot;
    public Transform battery;
    public Transform gun_pivot;
    public Transform point_muzzle_flash;
    public Transform gun_camera;
    public Transform drone_camera;
    public Transform motor;

    // Track bullet holes attached to robot
    // We do this instead of parenting the transform to avoid scaling issues
    class HoleAttachment {
        public Transform hole_transform;
        public Transform attached_part_transform;
        public Vector3 local_position;
        public Quaternion local_rotation;
    }
    List<HoleAttachment> attached_holes = new List<HoleAttachment>();
    
    AudioSource audiosource_taser;
    AudioSource audiosource_motor;
    GameObject object_audiosource_motor;
    AudioSource audiosource_effect;
    AudioSource audiosource_foley;
    float sound_line_of_sight = 0.0f;
    
    public GameObject electric_spark_obj;
    public GameObject muzzle_flash;
    public GameObject bullet_obj;

    public RobotType robot_type;
    
    float gun_delay = 0.0f;
    //bool alive = true;
    Spring rotation_x = new Spring(0.0f,0.0f,100.0f,0.0001f);
    Spring rotation_y = new Spring(0.0f,0.0f,100.0f,0.0001f);
    Quaternion initial_turret_orientation;
    Vector3 initial_turret_position;

    AIState ai_state = AIState.IDLE;
    bool battery_alive = true;
    bool motor_alive = true;
    bool camera_alive = true;
    bool trigger_alive = true;
    bool barrel_alive = true;
    bool ammo_alive = true;
    bool trigger_down = false;
    int bullets = 15;
    float kAlertDelay = 0.6f;
    float kAlertCooldownDelay = 2.0f;
    float alert_delay = 0.0f;
    float alert_cooldown_delay = 0.0f;
    float kMaxRange = 20.0f;
    float rotor_speed = 0.0f;
    float top_rotor_rotation = 0.0f;
    float bottom_rotor_rotation = 0.0f;
    Vector3 initial_pos;
    //bool stuck = false;
    float stuck_delay = 0.0f;
    Vector3 tilt_correction;
    bool distance_sleep = false;
    float kSleepDistance = 20.0f;
    
    public Vector3 target_pos;

    public bool LimitedRotation;
    bool FlipRotate, Waiting;
    public float RotationRange, EndWaitTime = 2f;
    float WaitTime;

    public CameraPivotState camera_pivot_state = CameraPivotState.WAIT_DOWN;
    public float camera_pivot_delay = 0.0f;
    public float camera_pivot_angle = 0.0f;

    LevelCreatorScript level_creator = null;
    Light lightObject;
    LensFlare lensFlareObject;

    int tile_parent_position = 0;
    
    public void PlaySoundFromGroup(List<AudioClip> group,float volume){
    	if(group.Count == 0){
    		return;
    	}
    	int which_shot = UnityEngine.Random.Range(0,group.Count);
    	audiosource_effect.PlayOneShot(group[which_shot], volume * Preferences.sound_volume);
    }
    
    public Quaternion RandomOrientation() {
    	return Quaternion.Euler((float)UnityEngine.Random.Range(0,360),(float)UnityEngine.Random.Range(0,360),(float)UnityEngine.Random.Range(0,360));
    }
    
    public void Damage(GameObject obj){
    	bool damage_done = false;
    	if(obj.name == "battery" && battery_alive){
    		battery_alive = false;
    		motor_alive = false;
    		camera_alive = false;
    		trigger_alive = false;
    		if(robot_type == RobotType.SHOCK_DRONE){
    			barrel_alive = false;
    		}
    		PlaySoundFromGroup(sound_damage_battery,1.0f);
    		rotation_x.target_state = 40.0f;
    		damage_done = true;
    	} else if((obj.name == "pivot motor" || obj.name == "motor") && motor_alive){
    		motor_alive = false;
    		PlaySoundFromGroup(sound_damage_motor,1.0f);
    		damage_done = true;
    	} else if(obj.name == "power cable" && (camera_alive || trigger_alive)){
    		camera_alive = false;
    		damage_done = true;
    		PlaySoundFromGroup(sound_damage_battery,1.0f);
    		trigger_alive = false;
    	} else if(obj.name == "ammo box" && ammo_alive){
    		ammo_alive = false;
    		PlaySoundFromGroup(sound_damage_ammo,1.0f);
    		damage_done = true;
    	} else if((obj.name == "gun" || obj.name == "shock prod") && barrel_alive){
    		barrel_alive = false;
    		PlaySoundFromGroup(sound_damage_gun,1.0f);
    		damage_done = true;
    	} else if(obj.name == "camera" && camera_alive){
    		camera_alive = false;
    		PlaySoundFromGroup(sound_damage_camera,1.0f);
    		damage_done = true;
    	} else if(obj.name == "camera armor" && camera_alive){
    		camera_alive = false;
    		PlaySoundFromGroup(sound_damage_camera,1.0f);
    		damage_done = true;
    	}
    	if(damage_done){
    		Instantiate(electric_spark_obj, obj.transform.position, RandomOrientation());
    	}
    }
    
    public void WasShotInternal(GameObject obj) {
    	Damage(obj);
    }
    
    public void WasShot(GameObject obj, Vector3 pos, Vector3 vel, float damage = 1f) {
    	if((transform.parent != null) && transform.parent.gameObject.name == "gun pivot"){
    		Vector3 x_axis = point_pivot.rotation * new Vector3(1.0f,0.0f,0.0f);
    		Vector3 y_axis = point_pivot.rotation * new Vector3(0.0f,1.0f,0.0f);
    		Vector3 z_axis = point_pivot.rotation * new Vector3(0.0f,0.0f,1.0f);
    		
    		Vector3 y_plane_vel = new Vector3(Vector3.Dot(vel, x_axis), 0.0f, Vector3.Dot(vel, z_axis));
    		Vector3 rel_pos = pos - point_pivot.position;
    		Vector3 y_plane_pos = new Vector3(Vector3.Dot(rel_pos, z_axis), 0.0f, -Vector3.Dot(rel_pos, x_axis));
    		rotation_y.vel += Vector3.Dot(y_plane_vel, y_plane_pos) * 10.0f;
    		
    		Vector3 x_plane_vel = new Vector3(Vector3.Dot(vel, y_axis), 0.0f, Vector3.Dot(vel, z_axis));
    		rel_pos = pos - point_pivot.position;
    		Vector3 x_plane_pos = new Vector3(-Vector3.Dot(rel_pos, z_axis), 0.0f, Vector3.Dot(rel_pos, y_axis));
    		rotation_x.vel += Vector3.Dot(x_plane_vel, x_plane_pos) * 10.0f;
    	}
    	
    	if(robot_type == RobotType.SHOCK_DRONE) {
    		if(Random.Range(0f, 1f) < 1 - Mathf.Pow(0.5f, damage))
    			Damage(battery.gameObject);
    	} else {
    		if(Random.Range(0f, 1f) < 1 - Mathf.Pow(0.75f, damage))
    			Damage(battery.gameObject);
    	}
    	Damage(obj);
    }
    
    public void Awake() {
    	// Assign light objects
    	switch(robot_type) {
    		case RobotType.MOBILE_TURRET:
    		case RobotType.STATIONARY_TURRET:
    			lightObject = gun_camera.Find("light").GetComponent<Light>();
                lensFlareObject = gun_camera.Find("lens flare").GetComponent<LensFlare>();
                break;
    		case RobotType.GUN_DRONE:
    		case RobotType.SHOCK_DRONE:
    			lightObject = drone_camera.Find("light").GetComponent<Light>();
    			lensFlareObject = drone_camera.Find("lens flare").GetComponent<LensFlare>();
    			break;
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

    	audiosource_effect = gameObject.AddComponent<AudioSource>();
    	audiosource_effect.rolloffMode = AudioRolloffMode.Linear;
    	audiosource_effect.maxDistance = 30.0f;
    	audiosource_effect.spatialBlend = 1.0f;
    
    	object_audiosource_motor = new GameObject("motor audiosource object");
    	object_audiosource_motor.transform.parent = transform;
    	object_audiosource_motor.transform.localPosition = new Vector3(0.0f,0.0f,0.0f);
    	
    	audiosource_motor = object_audiosource_motor.AddComponent<AudioSource>();
    	object_audiosource_motor.AddComponent<AudioLowPassFilter>();
    	audiosource_motor.loop = true;
    	audiosource_motor.volume = 0.4f * Preferences.sound_volume;
    	audiosource_motor.clip = sound_engine_loop;
    	audiosource_motor.spatialBlend = 1.0f;
    	
    	switch(robot_type){
    		case RobotType.STATIONARY_TURRET:
    			initial_turret_orientation = gun_pivot.transform.localRotation;
    			initial_turret_position = gun_pivot.transform.localPosition;
    			audiosource_motor.rolloffMode = AudioRolloffMode.Linear;
    			audiosource_motor.maxDistance = 4.0f;
    			break;
    		case RobotType.SHOCK_DRONE:
    			audiosource_motor.maxDistance = 8.0f;
    			audiosource_foley = gameObject.AddComponent<AudioSource>();
    	        audiosource_foley.spatialBlend = 1.0f;
    			audiosource_taser = gameObject.AddComponent<AudioSource>();
                audiosource_taser.Stop();
                audiosource_taser.playOnAwake = false;
    	        audiosource_taser.spatialBlend = 1.0f;
    			audiosource_taser.rolloffMode = AudioRolloffMode.Linear;
    			audiosource_taser.loop = true;
    			audiosource_taser.clip = sound_gunshot[0];
    			break;
    	}
    	
    	initial_pos = transform.position;	
    	target_pos = initial_pos;
    }

    internal void AttachHole(Transform hole_transform, Transform part_transform) {
        var hole_attachment = new HoleAttachment();
        hole_attachment.hole_transform = hole_transform;
        hole_attachment.attached_part_transform = part_transform;
        hole_attachment.local_position = part_transform.InverseTransformPoint(hole_transform.position);
        hole_attachment.local_rotation = Quaternion.Inverse(part_transform.transform.rotation) * hole_transform.rotation;
        attached_holes.Add(hole_attachment);
    }

    public Transform GetTarget() {
    	if(RobotScript._target == null) {
    		RobotScript._target = GameObject.Find("Player").transform;
    	}
    	return RobotScript._target;
    }

    public bool ShouldSleep() {
    	Transform target = GetTarget();
    	if(target == null) {
    		return true;
    	}
    	return Vector3.Distance(target.position, transform.position) > kSleepDistance;
    }

    public void UpdateStationaryTurret() {
    	if(ShouldSleep()){
    		lightObject.shadows = LightShadows.None;
    		if(audiosource_motor.isPlaying){
    			audiosource_motor.Stop();
    		}
    		return;
    	} else {
    		if(!audiosource_motor.isPlaying){
    			audiosource_motor.volume = Preferences.sound_volume;
    			audiosource_motor.Play();
    		}
    		audiosource_motor.volume = 0.4f * Preferences.sound_volume;
    		if(lightObject.intensity > 0.0f){
    			lightObject.shadows = LightShadows.Hard;
    		} else {
    			lightObject.shadows = LightShadows.None;
    		}
    	}
    	Vector3 rel_pos = Vector3.zero;
        if(motor_alive){
    		switch(ai_state){
    			case AIState.IDLE:
                    if (!LimitedRotation) {
                        rotation_y.target_state += Time.deltaTime * 100.0f;
                    }
                    else {
                        if (!Waiting) {
                            rotation_y.target_state += Time.deltaTime * 100.0f * (FlipRotate ? -1f : 1f);
                        }
                        else {
                            WaitTime += Time.deltaTime;
                            if (WaitTime > EndWaitTime) {
                                Waiting = false;
                                WaitTime = 0f;
                            }
                        }
                        if (Mathf.Abs(rotation_y.target_state) > RotationRange) {
                            FlipRotate = !FlipRotate;
                            Waiting = true;
                        }
                        rotation_y.target_state = Mathf.Clamp(rotation_y.target_state, -RotationRange, RotationRange);
                    }
                    break;
    			case AIState.AIMING:
    			case AIState.ALERT:
    			case AIState.ALERT_COOLDOWN:
    			case AIState.FIRING:
    				rel_pos = target_pos - point_pivot.position;
    				Vector3 x_axis = point_pivot.rotation * new Vector3(1.0f,0.0f,0.0f);
    				Vector3 y_axis = point_pivot.rotation * new Vector3(0.0f,1.0f,0.0f);
    				Vector3 z_axis = point_pivot.rotation * new Vector3(0.0f,0.0f,1.0f);
    				Vector3 y_plane_pos = (new Vector3(Vector3.Dot(rel_pos, z_axis), 0.0f, -Vector3.Dot(rel_pos, x_axis))).normalized;
    				float target_y = Mathf.Atan2(y_plane_pos.x, y_plane_pos.z)/Mathf.PI*180-90;
    				while(target_y > rotation_y.state + 180){
    					rotation_y.state += 360.0f;
    				}
    				while(target_y < rotation_y.state - 180){
    					rotation_y.state -= 360.0f;
    				}
    				rotation_y.target_state = target_y;
    				float y_height = Vector3.Dot(y_axis, rel_pos.normalized);
    				float target_x = -Mathf.Asin(y_height)/Mathf.PI*180;
    				rotation_x.target_state = target_x;
    				rotation_x.target_state = Mathf.Min(40.0f,Mathf.Max(-40.0f,target_x));
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
    			if(gun_delay <= 0.0f){
    				gun_delay += 0.1f;
    				Instantiate(muzzle_flash, point_muzzle_flash.position, point_muzzle_flash.rotation);
    				PlaySoundFromGroup(sound_gunshot, 1.0f);
    				
    				GameObject bullet = (GameObject)Instantiate(bullet_obj, point_muzzle_flash.position, point_muzzle_flash.rotation);
    				bullet.GetComponent<BulletScript>().SetVelocity(point_muzzle_flash.forward * 300.0f);
    				bullet.GetComponent<BulletScript>().SetHostile();
    				rotation_x.vel += (float)UnityEngine.Random.Range(-50,50);
    				rotation_y.vel += (float)UnityEngine.Random.Range(-50,50);
    				--bullets;
    			}
    		}
    		if(ammo_alive && bullets > 0){
    			gun_delay = Mathf.Max(0.0f, gun_delay - Time.deltaTime);
    		}
    	}

    	// Target interactions
    	Transform target = GetTarget();
    	if(target != null) {
    		float danger = 0.0f;
    		float dist = Vector3.Distance(target.position, transform.position);
    		if(battery_alive){
    			danger += Mathf.Max(0.0f, 1.0f - dist/kMaxRange);
    		}
    		if(camera_alive){
    			if(danger > 0.0f){
    				danger = Mathf.Min(0.2f, danger);
    			}
    			if(ai_state == AIState.AIMING || ai_state == AIState.FIRING){
    				danger = 1.0f;
    			}
    			if(ai_state == AIState.ALERT || ai_state == AIState.ALERT_COOLDOWN){
    				danger += 0.5f;
    			}
    			
    			rel_pos = target.position - gun_camera.position;
    			bool sees_target = false;
    			if(dist < kMaxRange && Vector3.Dot(gun_camera.rotation*new Vector3(0.0f,-1.0f,0.0f), rel_pos.normalized) > 0.7f){
    				RaycastHit hit = new RaycastHit();
    				if(!Physics.Linecast(gun_camera.position, target.position, out hit, 1<<0)){
    					sees_target = true;
    				}
    			}
    			if(sees_target){
    				switch(ai_state){
    					case AIState.IDLE:
    						ai_state = AIState.ALERT;
    						audiosource_effect.PlayOneShot(sound_alert, 0.3f * Preferences.sound_volume);
    						alert_delay = kAlertDelay;
    						break;
    					case AIState.AIMING:
    						if(Vector3.Dot(gun_camera.rotation*new Vector3(0.0f,-1.0f,0.0f), rel_pos.normalized) > 0.9f){
    							ai_state = AIState.FIRING;
    						}
    						target_pos = target.position;
    						break;					
    					case AIState.FIRING:
    						target_pos = target.position;
    						break;
    					case AIState.ALERT:
    						alert_delay -= Time.deltaTime;
    						if(alert_delay <= 0.0f){
    							ai_state = AIState.AIMING;
    						}
    						target_pos = target.position;
    						break;
    					case AIState.ALERT_COOLDOWN:
    						ai_state = AIState.ALERT;
    						alert_delay = kAlertDelay;
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
    						if(alert_cooldown_delay <= 0.0f){
    							ai_state = AIState.IDLE;
    							audiosource_effect.PlayOneShot(sound_unalert, 0.3f * Preferences.sound_volume);
    						}
    						break;
    				}
    			}
    			switch(ai_state){
    				case AIState.IDLE:
    					lightObject.color = new Color(0.0f,0.0f,1.0f);
    					break;
    				case AIState.AIMING:
    					lightObject.color = new Color(1.0f,0.0f,0.0f);
    					break;
    				case AIState.ALERT:
    				case AIState.ALERT_COOLDOWN:
    					lightObject.color = new Color(1.0f,1.0f,0.0f);
    					break;
    			}
    		}
    		target.GetComponent<MusicScript>().AddDangerLevel(danger);
    	}
    	if(!camera_alive){
    		lightObject.intensity *= Mathf.Pow(0.01f, Time.deltaTime);
    	}
        lensFlareObject.color = lightObject.color;
        lensFlareObject.brightness = lightObject.intensity;
        float target_pitch = (Mathf.Abs(rotation_y.vel) + Mathf.Abs(rotation_x.vel)) * 0.01f;
    	target_pitch = Mathf.Clamp(target_pitch, 0.2f, 2.0f);
    	audiosource_motor.pitch = Mathf.Lerp(audiosource_motor.pitch, target_pitch, Mathf.Pow(0.0001f, Time.deltaTime));
    	
    	rotation_x.Update();
    	rotation_y.Update();
    	gun_pivot.localRotation = initial_turret_orientation;
    	gun_pivot.localPosition = initial_turret_position;
    	gun_pivot.RotateAround(
    		point_pivot.position, 
    		point_pivot.rotation * new Vector3(1.0f,0.0f,0.0f),
    		rotation_x.state);
    	gun_pivot.RotateAround(
    		point_pivot.position, 
    		point_pivot.rotation * new Vector3(0.0f,1.0f,0.0f),
    		rotation_y.state);
    }
    
    public void UpdateDrone() {
    	if(ShouldSleep()){
    		lightObject.shadows = LightShadows.None;
    		if(motor_alive){
    			distance_sleep = true;
    			GetComponent<Rigidbody>().Sleep();
    		}
    		if(audiosource_motor.isPlaying){
    			audiosource_motor.Stop();
    		}
    		return;
    	} else {
    		if(lightObject.intensity > 0.0f){
    			lightObject.shadows = LightShadows.Hard;
    		} else {
    			lightObject.shadows = LightShadows.None;
    		}
    		if(motor_alive && distance_sleep){
    			GetComponent<Rigidbody>().WakeUp();
    			distance_sleep = false;
    		}
    		if(!audiosource_motor.isPlaying){
    			audiosource_motor.volume = Preferences.sound_volume;
    			audiosource_motor.Play();
    		}

            if(level_creator != null) {
                int new_tile_position = level_creator.GetTilePosition(transform.position);
                if(tile_parent_position != new_tile_position) {
                    tile_parent_position = new_tile_position;
                    transform.parent = level_creator.GetPositionTileEnemiesParent(transform.position);
                }
            }
    	}

    	if(audiosource_taser.isPlaying && (!barrel_alive || ai_state != AIState.FIRING)) { // Turn off taser if we no longer fire
    		audiosource_taser.Stop();
    	}

    	Vector3 rel_pos = target_pos - transform.position;
    	if(motor_alive){		
    		float kFlyDeadZone = 0.2f;
    		float kFlySpeed = 10.0f;
    		Vector3 target_vel = (target_pos - transform.position) / kFlyDeadZone;
    		if(target_vel.magnitude > 1.0f){
    			target_vel = target_vel.normalized;
    		}
    		target_vel *= kFlySpeed;
    		Vector3 target_accel = (target_vel - GetComponent<Rigidbody>().velocity);
    		if(ai_state == AIState.IDLE){
    			target_accel *= 0.1f;
    		}
    		target_accel.y += 9.81f;
    		
    		rotor_speed = target_accel.magnitude;
    		rotor_speed = Mathf.Clamp(rotor_speed, 0.0f, 14.0f);
    		
    		Vector3 up = transform.rotation * new Vector3(0.0f,1.0f,0.0f);
    		Quaternion correction = Quaternion.identity;
    		correction.SetFromToRotation(up, target_accel.normalized);
    		Vector3 correction_vec = Vector3.zero;
    		float correction_angle = 0.0f;
    		correction.ToAngleAxis(out correction_angle, out correction_vec);
    		tilt_correction = correction_vec * correction_angle;
    		tilt_correction -= GetComponent<Rigidbody>().angularVelocity;
    		
    		
    		Vector3 x_axis = transform.rotation * new Vector3(1.0f,0.0f,0.0f);
    		Vector3 y_axis = transform.rotation * new Vector3(0.0f,1.0f,0.0f);
    		Vector3 z_axis = transform.rotation * new Vector3(0.0f,0.0f,1.0f);
    		if(ai_state != AIState.IDLE){
    			Vector3 y_plane_pos = (new Vector3(Vector3.Dot(rel_pos, z_axis), 0.0f, -Vector3.Dot(rel_pos, x_axis))).normalized;
    			float target_y = Mathf.Atan2(y_plane_pos.x, y_plane_pos.z)/Mathf.PI*180-90;
    			while(target_y > 180){
    				target_y -= 360.0f;
    			}
    			while(target_y < -180){
    				target_y += 360.0f;
    			}
    			tilt_correction += y_axis * target_y;	
    			tilt_correction *= 5.0f;
    		} else {
    			tilt_correction += y_axis;	
    		}
    		
    		if(ai_state == AIState.IDLE){
    			tilt_correction *= 0.1f;
    		}
    		
    		if(GetComponent<Rigidbody>().velocity.magnitude < 0.2f){ 
    			stuck_delay += Time.deltaTime;
    			if(stuck_delay > 1.0f){
    				target_pos = transform.position + new Vector3(UnityEngine.Random.Range(-1.0f,1.0f), UnityEngine.Random.Range(-1.0f,1.0f), UnityEngine.Random.Range(-1.0f,1.0f));
    				stuck_delay = 0.0f;
    			}
    		} else {
    			stuck_delay = 0.0f;
    		}
    		
    	} else {
    		rotor_speed = Mathf.Max(0.0f, rotor_speed - Time.deltaTime * 5.0f);
    		GetComponent<Rigidbody>().angularDrag = 0.05f;
    	}
    	
    	top_rotor_rotation += rotor_speed * Time.deltaTime * 1000.0f;
    	bottom_rotor_rotation -= rotor_speed * Time.deltaTime * 1000.0f;
    	if(rotor_speed * Time.timeScale > 7.0f){
    		bottom_rotor.gameObject.GetComponent<Renderer>().enabled = false;
    		top_rotor.gameObject.GetComponent<Renderer>().enabled = false;
    	} else {
    		bottom_rotor.gameObject.GetComponent<Renderer>().enabled = true;
    		top_rotor.gameObject.GetComponent<Renderer>().enabled = true;
    	}
    	var tmp_cs1 = bottom_rotor.localEulerAngles;
        tmp_cs1.y = bottom_rotor_rotation;
        bottom_rotor.localEulerAngles = tmp_cs1;
    	var tmp_cs2 = top_rotor.localEulerAngles;
        tmp_cs2.y = top_rotor_rotation;
        top_rotor.localEulerAngles = tmp_cs2;
    	
    	//rigidbody.velocity += transform.rotation * Vector3(0,1,0) * rotor_speed * Time.deltaTime;
    	RaycastHit hit = new RaycastHit();
        if(camera_alive){
    		if(ai_state == AIState.IDLE){
    			switch(camera_pivot_state) {
    				case CameraPivotState.DOWN:
    					camera_pivot_angle += Time.deltaTime * 25.0f;
    					if(camera_pivot_angle > 50){
    						camera_pivot_angle = 50.0f;
    						camera_pivot_state = CameraPivotState.WAIT_UP;
    						camera_pivot_delay = 0.2f;
    					}
    					break;
    				case CameraPivotState.UP:
    					camera_pivot_angle -= Time.deltaTime * 25.0f;
    					if(camera_pivot_angle < 0){
    						camera_pivot_angle = 0.0f;
    						camera_pivot_state = CameraPivotState.WAIT_DOWN;
    						camera_pivot_delay = 0.2f;
    					}
    					break;
    				case CameraPivotState.WAIT_DOWN:
    					camera_pivot_delay -= Time.deltaTime;
    					if(camera_pivot_delay < 0){
    						camera_pivot_state = CameraPivotState.DOWN;
    					}
    					break;
    				case CameraPivotState.WAIT_UP:
    					camera_pivot_delay -= Time.deltaTime;
    					if(camera_pivot_delay < 0){
    						camera_pivot_state = CameraPivotState.UP;
    					}
    					break;
    			}
    		} else {
    			camera_pivot_angle -= Time.deltaTime * 25.0f;
    			if(camera_pivot_angle < 0){
    				camera_pivot_angle = 0.0f;
    			}
    		}

    		// Target interactions
    		var target = GetTarget();
    		if(target != null) {
    			// Taser
    			if(barrel_alive && ai_state == AIState.FIRING){
    				if(!audiosource_taser.isPlaying){
    					audiosource_taser.volume = Preferences.sound_volume;
    					audiosource_taser.Play();
    				} else {
    					audiosource_taser.volume = Preferences.sound_volume;
    				}
    				if(gun_delay <= 0.0f){
    					gun_delay = 0.1f;	
    					Instantiate(muzzle_flash, point_spark.position, RandomOrientation());
    					if(Vector3.Distance(point_spark.position, target.position) < 1){
    						target.GetComponent<AimScript>().Shock();
    					}
    				}
    			}
    			gun_delay = Mathf.Max(0.0f, gun_delay - Time.deltaTime);

    			// Danger state
    			var tmp_cs3 = camera_pivot.localEulerAngles;
    			tmp_cs3.x = camera_pivot_angle;
    			camera_pivot.localEulerAngles = tmp_cs3;
    			float dist = Vector3.Distance(target.position, transform.position);
    			float danger = Mathf.Max(0.0f, 1.0f - dist/kMaxRange);
    			if(danger > 0.0f){
    				danger = Mathf.Min(0.2f, danger);
    			}
    			if(ai_state == AIState.AIMING || ai_state == AIState.FIRING){
    				danger = 1.0f;
    			}
    			if(ai_state == AIState.ALERT || ai_state == AIState.ALERT_COOLDOWN){
    				danger += 0.5f;
    			}
    			target.GetComponent<MusicScript>().AddDangerLevel(danger);
    			
    			// Target finding
    			rel_pos = target.position - drone_camera.position;
    			bool sees_target = false;
    			if(dist < kMaxRange && Vector3.Dot(drone_camera.rotation*new Vector3(0.0f,-1.0f,0.0f), rel_pos.normalized) > 0.7f){
    				hit = new RaycastHit();
    				if(!Physics.Linecast(drone_camera.position, target.position, out hit, 1<<0)){
    					sees_target = true;
    				}
    			}

    			// Attacking
    			if(sees_target){
    				Vector3 new_target = target.position + target.GetComponent<AimScript>().GetVelocity() * 
    								Mathf.Clamp(Vector3.Distance(target.transform.position, transform.position) * 0.1f, 0.5f, 1.0f);
    				switch(ai_state){
    					case AIState.IDLE:
    						ai_state = AIState.ALERT;
    						alert_delay = kAlertDelay;
    						audiosource_effect.PlayOneShot(sound_alert, 0.3f * Preferences.sound_volume);
    						break;
    					case AIState.AIMING:
    						target_pos = new_target;
    						if(Vector3.Distance(transform.position, target_pos) < 4){
    							ai_state = AIState.FIRING;
    						}
    						target_pos.y += 1.0f;
    						break;					
    					case AIState.FIRING:
    						target_pos = new_target;
    						if(Vector3.Distance(transform.position, target_pos) > 4){
    							ai_state = AIState.AIMING;
    						}
    						break;
    					case AIState.ALERT:
    						alert_delay -= Time.deltaTime;
    						target_pos = new_target;
    						target_pos.y += 1.0f;
    						if(alert_delay <= 0.0f){
    							ai_state = AIState.AIMING;
    						}
    						break;
    					case AIState.ALERT_COOLDOWN:
    						ai_state = AIState.ALERT;
    						alert_delay = kAlertDelay;
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
    						if(alert_cooldown_delay <= 0.0f){
    							ai_state = AIState.IDLE;
    							audiosource_effect.PlayOneShot(sound_unalert, 0.3f * Preferences.sound_volume);
    						}
    						break;
    				}
    			}
    		}
    		switch(ai_state){
    			case AIState.IDLE:
    				lightObject.color = new Color(0.0f,0.0f,1.0f);
    				break;
    			case AIState.AIMING:
    				lightObject.color = new Color(1.0f,0.0f,0.0f);
    				break;
    			case AIState.ALERT:
    			case AIState.ALERT_COOLDOWN:
    				lightObject.color = new Color(1.0f,1.0f,0.0f);
    				break;
    		}
    	}
    	if(!camera_alive){
    		lightObject.intensity *= Mathf.Pow(0.01f, Time.deltaTime);
    	}
    	lensFlareObject.color = lightObject.color;
    	lensFlareObject.brightness = lightObject.intensity;
    	float target_pitch = rotor_speed * 0.2f;
    	target_pitch = Mathf.Clamp(target_pitch, 0.2f, 3.0f);
    	audiosource_motor.pitch = Mathf.Lerp(audiosource_motor.pitch, target_pitch, Mathf.Pow(0.0001f, Time.deltaTime));
    	audiosource_motor.volume = rotor_speed * 0.1f * Preferences.sound_volume;
    
    	audiosource_motor.volume -= Vector3.Distance(Camera.main.transform.position, transform.position) * 0.0125f * Preferences.sound_volume;
    
    	bool line_of_sight = true;
    	if(Physics.Linecast(transform.position, Camera.main.transform.position, out hit, 1<<0)){
    		line_of_sight = false;
    	}
    	if(line_of_sight){
    		sound_line_of_sight += Time.deltaTime * 3.0f;
    	} else {
    		sound_line_of_sight -= Time.deltaTime * 3.0f;
    	}
    	sound_line_of_sight = (float)Mathf.Clamp((int)sound_line_of_sight,0,1);
    	
    	audiosource_motor.volume *= 0.5f + sound_line_of_sight * 0.5f;
    	object_audiosource_motor.GetComponent<AudioLowPassFilter>().cutoffFrequency = 
    		Mathf.Lerp(5000.0f, 44000.0f, sound_line_of_sight);
    }
    
    
    public void Update() {
    	switch(robot_type){
    		case RobotType.STATIONARY_TURRET:
    			UpdateStationaryTurret();
    			break;
    		case RobotType.SHOCK_DRONE:
    			UpdateDrone();
    			break;
    	}
        foreach(var hole in attached_holes){
            if(hole.attached_part_transform != null && hole.hole_transform != null){
                hole.hole_transform.position = hole.attached_part_transform.TransformPoint(hole.local_position);
                hole.hole_transform.rotation = hole.attached_part_transform.rotation * hole.local_rotation;
            }
        }
    }
    
    public void OnCollisionEnter(Collision collision) {
    	if(robot_type == RobotType.SHOCK_DRONE){
    		if(collision.relativeVelocity.magnitude > 10){
    			if(UnityEngine.Random.Range(0.0f,1.0f)<0.5f && motor_alive){
    				Damage(motor.gameObject);
    			} else if(UnityEngine.Random.Range(0.0f,1.0f)<0.5f && camera_alive){
    				Damage(drone_camera.gameObject);
    			} else if(UnityEngine.Random.Range(0.0f,1.0f)<0.5f && battery_alive){
    				Damage(battery.gameObject);
    			} else {
    				motor_alive = true;
    				Damage(motor.gameObject);
    			} 
    		} else {
    			int which_shot = UnityEngine.Random.Range(0,sound_bump.Count);
    			audiosource_foley.PlayOneShot(sound_bump[which_shot], collision.relativeVelocity.magnitude * 0.15f * Preferences.sound_volume);
    		}
    	}
    }
    
    public void FixedUpdate() {
    	if(robot_type == RobotType.SHOCK_DRONE && !distance_sleep){
    		GetComponent<Rigidbody>().AddForce(transform.rotation * new Vector3(0.0f,1.0f,0.0f) * rotor_speed, ForceMode.Force);
    		if(motor_alive){
    			GetComponent<Rigidbody>().AddTorque(tilt_correction, ForceMode.Force);
    		}
    	}
    }
}
