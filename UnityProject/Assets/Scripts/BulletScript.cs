using UnityEngine;
using System;
using System.Collections.Generic;


public class BulletScript:MonoBehaviour{
    
    public List<AudioClip> sound_hit_concrete;
    public List<AudioClip> sound_hit_metal;
    public List<AudioClip> sound_hit_glass;
    public List<AudioClip> sound_hit_body;
    public List<AudioClip> sound_hit_ricochet;
    public List<AudioClip> sound_glass_break;
    public List<AudioClip> sound_flyby;
    public GameObject bullet_obj;
    public GameObject bullet_hole_decal_obj;
    public GameObject bullet_hole_obj;
    public GameObject glass_bullet_hole_obj;
    public GameObject metal_bullet_hole_obj;
    public GameObject glass_bullet_hole_decal_obj;
    public GameObject metal_bullet_hole_decal_obj;
    public GameObject spark_effect;
    public GameObject puff_effect;
    Vector3 old_pos = Vector3.zero;
    bool hit_something = false;
    LineRenderer line_renderer; 
    Vector3 velocity;
    float life_time = 0.0f;
    float death_time = 0.0f;
    int segment = 1;
    bool hostile = false;

    LevelCreatorScript level_creator;
    
    public void SetVelocity(Vector3 vel){
    	this.velocity = vel;
    }
    
    public void SetHostile(){
    	GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Logarithmic;
    	PlaySoundFromGroup(sound_flyby, 0.4f);
    	hostile = true;
    }
    		
    public void Start() {
        GameObject level_object = GameObject.Find("LevelObject");

        if(level_object != null) {
            level_creator = level_object.GetComponent<LevelCreatorScript>();
        }

        if(level_creator == null) {
            Debug.LogWarning("We're missing a LevelCreatorScript in GunScript, this might mean that some world-interactions don't work correctly.");
        }
        
    	line_renderer = GetComponent<LineRenderer>();
    	line_renderer.SetPosition(0, transform.position);
    	line_renderer.SetPosition(1, transform.position);
    }
    
    public MonoBehaviour RecursiveHasScript(GameObject obj,Type script,int depth) {
    	if(obj.GetComponent(script) != null){
    		return (MonoBehaviour)obj.GetComponent(script);
    	} else if(depth > 0 && (obj.transform.parent != null)){
    		return RecursiveHasScript(obj.transform.parent.gameObject, script, depth-1);
    	} else {
    		return null;
    	}
    }
    
    public static Quaternion RandomOrientation() {
    	return Quaternion.Euler((float)UnityEngine.Random.Range(0,360),(float)UnityEngine.Random.Range(0,360),(float)UnityEngine.Random.Range(0,360));
    }
    
    public void PlaySoundFromGroup(List<AudioClip> group,float volume){
    	int which_shot = UnityEngine.Random.Range(0,group.Count);
    	GetComponent<AudioSource>().PlayOneShot(group[which_shot], volume * PlayerPrefs.GetFloat("sound_volume", 1.0f));
    }
    
    public void Update() {
    	if(!hit_something){
    		life_time += Time.deltaTime;
    		if(life_time > 1.5f){
    			hit_something = true;
    		}
    		old_pos = transform.position;
    		transform.position += velocity * Time.deltaTime;
    		velocity += Physics.gravity * Time.deltaTime;
    		RaycastHit hit = new RaycastHit();
    		if(Physics.Linecast(old_pos, transform.position, out hit, 1<<0 | 1<<9 | 1<<11)){
                GameObject hit_obj = hit.collider.gameObject;
                GameObject hit_transform_obj = hit.transform.gameObject;
                ShootableLight light_script = RecursiveHasScript(hit_obj, typeof(ShootableLight), 1) as ShootableLight;
                AimScript aim_script = RecursiveHasScript(hit_obj, typeof(AimScript), 1) as AimScript;
                RobotScript turret_script = RecursiveHasScript(hit_obj, typeof(RobotScript), 3) as RobotScript;
    			transform.position = hit.point;
    			float ricochet_amount = Vector3.Dot(velocity.normalized, hit.normal) * -1.0f;
    			if(UnityEngine.Random.Range(0.0f, 1.0f) > ricochet_amount && Vector3.Magnitude(velocity) * (1.0-ricochet_amount) > 10.0){
    				var ricochet = Instantiate(bullet_obj, hit.point, transform.rotation);
    				var ricochet_vel = velocity * 0.3f * (1.0f-ricochet_amount);
    				velocity -= ricochet_vel;
    				ricochet_vel = Vector3.Reflect(ricochet_vel, hit.normal);
    				ricochet.GetComponent<BulletScript>().SetVelocity(ricochet_vel);
    				PlaySoundFromGroup(sound_hit_ricochet, hostile ? 1.0f : 0.6f);
    			} else if(turret_script && velocity.magnitude > 100.0f){
                    RaycastHit new_hit;
    				if(Physics.Linecast(hit.point + velocity.normalized * 0.001f, hit.point + velocity.normalized, out new_hit, 1<<11 | 1<<12)){
    					if(new_hit.collider.gameObject.layer == 12){
    						turret_script.WasShotInternal(new_hit.collider.gameObject);
    					}
    				}					
    			}
    			if(turret_script && turret_script.GetComponent<Rigidbody>()){
    				turret_script.GetComponent<Rigidbody>().AddForceAtPosition(velocity * 0.01f, hit.point, ForceMode.Impulse);
    			}
                bool broke_glass = false;
    			if(light_script){
    				broke_glass = light_script.WasShot(hit_obj, hit.point, velocity);
    				if(hit.collider.material.name == "glass (Instance)"){
    					PlaySoundFromGroup(sound_glass_break, 1.0f);
    				}
    			}
    			if(Vector3.Magnitude(velocity) > 50){
                    GameObject hole = null;
                    GameObject effect;
    				if(turret_script){
    					PlaySoundFromGroup(sound_hit_metal, hostile ? 1.0f : 0.8f);                        
					    hole = Instantiate(metal_bullet_hole_obj, hit.point, RandomOrientation());
    					effect = Instantiate(spark_effect, hit.point, RandomOrientation());
    					turret_script.WasShot(hit_obj, hit.point, velocity);
    				} else if(aim_script){
    					hole = Instantiate(bullet_hole_obj, hit.point, RandomOrientation());
    					effect = Instantiate(puff_effect, hit.point, RandomOrientation());
    					PlaySoundFromGroup(sound_hit_body, 1.0f);
    					aim_script.WasShot();
    				} else if(hit.collider.material.name == "metal (Instance)"){
    					PlaySoundFromGroup(sound_hit_metal, hostile ? 1.0f : 0.4f);					
    					hole = Instantiate(metal_bullet_hole_decal_obj, hit.point, Quaternion.FromToRotation(new Vector3(0,0,-1), hit.normal) * Quaternion.AngleAxis(UnityEngine.Random.Range(0,360), new Vector3(0,0,1)));
    					effect = Instantiate(spark_effect, hit.point, RandomOrientation());
    				} else if(hit.collider.material.name == "glass (Instance)"){
    					PlaySoundFromGroup(sound_hit_glass, hostile ? 1.0f : 0.4f);
                        if(!broke_glass){ // Don't make bullet hole if glass is no longer there
        					hole = Instantiate(glass_bullet_hole_obj, hit.point, RandomOrientation());
                        }
    					effect = Instantiate(spark_effect, hit.point, RandomOrientation());
    				} else {
    					PlaySoundFromGroup(sound_hit_concrete, hostile ? 1.0f : 0.4f);
    					hole = Instantiate(bullet_hole_decal_obj, hit.point, Quaternion.FromToRotation(new Vector3(0,0,-1), hit.normal) * Quaternion.AngleAxis(UnityEngine.Random.Range(0,360), new Vector3(0,0,1)));
    					effect = Instantiate(puff_effect, hit.point, RandomOrientation());
    				}
    				effect.transform.position += hit.normal * 0.05f;
                    if(hole != null){
    				    if(aim_script){
    					    hole.transform.parent = aim_script.main_camera.transform;
                        } else if(turret_script){
                            hole.transform.parent = turret_script.transform;
                            turret_script.AttachHole(hole.transform, hit.transform);
    				    } else if(level_creator != null) {
                            hole.transform.parent = level_creator.GetPositionTileDecalsParent(hole.transform.position);
                        } 
                    }
    			}
    			hit_something = true;
    		}
            line_renderer.positionCount = segment+1;
    		line_renderer.SetPosition(segment, transform.position);
    		++segment;
    	} else {
    		life_time += Time.deltaTime;
    		death_time += Time.deltaTime;
    		//Destroy(this.gameObject);
    	}
    	for(int i=0; i<segment; ++i){
    		Color start_color = new Color(1.0f,1.0f,1.0f,(1.0f - life_time * 5.0f)*0.05f);
    		Color end_color = new Color(1.0f,1.0f,1.0f,(1.0f - death_time * 5.0f)*0.05f);
            line_renderer.startColor = start_color;
            line_renderer.endColor = end_color;
    		if(death_time > 1.0f){
    			Destroy(this.gameObject);
    		}
    	}
    }
}