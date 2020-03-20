using UnityEngine;
using System;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class ProjectileScript : MonoBehaviour {
    public List<AudioClip> sound_hit_concrete;
    public List<AudioClip> sound_hit_metal;
    public List<AudioClip> sound_hit_glass;
    public List<AudioClip> sound_hit_body;
    public List<AudioClip> sound_hit_ricochet;
    public List<AudioClip> sound_glass_break;

    public GameObject bullet_hole_obj;
    public GameObject glass_bullet_hole_obj;
    public GameObject metal_bullet_hole_obj;
    public GameObject spark_effect;
    public GameObject puff_effect;

    public float damageMultiplier = 1f;

    [Tooltip("List of all colliders that should be completely ignored")]
    public Collider[] ignored_colliders = new Collider[0];
    [Tooltip("List of all colliders that activate this ProjectileScript's collision handeling")]
    public Collider[] own_colliders = new Collider[0];

    public bool attach_on_hit = false;
    public UnityEvent on_collision = new UnityEvent();
    [Tooltip("Minimum speed at which damage effects happen")]
    public float lethal_speed = 20f;

    [Tooltip("Log collision information in the console")]
    public bool log_collision_data = false;

    private LevelCreatorScript level_creator;

    void Awake() {
        GameObject level_object = GameObject.Find("LevelObject");
        if(level_object) {
            level_creator = level_object.GetComponent<LevelCreatorScript>();
            if(!level_creator) {
                Debug.LogWarning("We're missing a LevelCreatorScript in GunScript, this might mean that some world-interactions don't work correctly.");
            }
        }

        // Fallback to all colliders
        if(own_colliders.Length <= 0) {
            own_colliders = GetComponents<Collider>();
        }

        // Ignore own colliders
        foreach(Collider a in own_colliders) {
            foreach(Collider b in own_colliders) {
                Physics.IgnoreCollision(a, b);
            }
        }

        // Ignore colliders
        foreach (Collider own_collider in own_colliders) {
            foreach (Collider ignored_collider in ignored_colliders) {
                Physics.IgnoreCollision(own_collider, ignored_collider);
            }
        }
    }

    public static Component RecursiveHasScript(GameObject obj, Type script, int depth) {
        if(obj.GetComponent(script) != null) {
            return obj.GetComponent(script);
        } else if(depth > 0 && obj.transform.parent) {
            return RecursiveHasScript(obj.transform.parent.gameObject, script, depth-1);
        } else {
            return null;
        }
    }
    
    public static Quaternion RandomOrientation() {
        return Quaternion.Euler((float)UnityEngine.Random.Range(0,360),(float)UnityEngine.Random.Range(0,360),(float)UnityEngine.Random.Range(0,360));
    }
    
    public void PlaySoundFromGroup(List<AudioClip> group,float volume) {
        if(group.Count == 0) {
            return;
        }

        int which_shot = UnityEngine.Random.Range(0, group.Count);
        GetComponent<AudioSource>().PlayOneShot(group[which_shot], volume * Preferences.sound_volume);
    }

    private GameObject TryInstantiate(GameObject game_object, Vector3 position, Quaternion rotation) {
        if(game_object)
            return Instantiate(game_object, position, rotation);
        return null;
    }

    void OnCollisionEnter(Collision col) {
        GameObject other = col.gameObject;
        ContactPoint contact = col.GetContact(0);
        Vector3 velocity = col.relativeVelocity;

        if(!own_colliders.Contains(contact.thisCollider)) {
            return;
        }

#if UNITY_EDITOR
        if(log_collision_data) {
            Debug.Log($"COLLISION @ vel: {velocity.magnitude} ({this.gameObject.name} -> {other.name})");
        }
#endif

        ShootableLight light_script = RecursiveHasScript(other, typeof(ShootableLight), 1) as ShootableLight;
        AimScript aim_script = RecursiveHasScript(other, typeof(AimScript), 1) as AimScript;
        RobotScript robot_script = RecursiveHasScript(other, typeof(RobotScript), 3) as RobotScript;
        Rigidbody rigidbody = RecursiveHasScript(other, typeof(Rigidbody), 3) as Rigidbody;

        if(rigidbody) {
            rigidbody.AddForceAtPosition(velocity * 0.01f, contact.point, ForceMode.Impulse);
        }


        if(velocity.magnitude > lethal_speed) {
            bool broke_glass = false;
            if(light_script){
                broke_glass = light_script.WasShot(other, contact.point, velocity);
                if(col.collider.material.name.Contains("glass")){
                    PlaySoundFromGroup(sound_glass_break, 1.0f);
                }
            }

            GameObject hole = null;
            GameObject effect;
            if(robot_script) {
                PlaySoundFromGroup(sound_hit_metal, 0.8f);
                hole = TryInstantiate(metal_bullet_hole_obj, contact.point, RandomOrientation());
                effect = TryInstantiate(spark_effect, contact.point, RandomOrientation());
                robot_script.WasShot(other, contact.point, velocity, damageMultiplier);
            } else if(aim_script) {
                hole = TryInstantiate(bullet_hole_obj, contact.point, RandomOrientation());
                effect = TryInstantiate(puff_effect, contact.point, RandomOrientation());
                PlaySoundFromGroup(sound_hit_body, 1.0f);
                aim_script.WasShot();
            } else if(col.collider.material.name.Contains("metal")) {
                PlaySoundFromGroup(sound_hit_metal, 0.4f);
                hole = TryInstantiate(metal_bullet_hole_obj, contact.point, Quaternion.FromToRotation(new Vector3(0,0,-1), contact.normal) * Quaternion.AngleAxis(UnityEngine.Random.Range(0,360), new Vector3(0,0,1)));
                effect = TryInstantiate(spark_effect, contact.point, RandomOrientation());
            } else if(col.collider.material.name.Contains("glass")){
                PlaySoundFromGroup(sound_hit_glass, 0.4f);
                if(!broke_glass){ // Don't make bullet hole if glass is no longer there
                    hole = TryInstantiate(glass_bullet_hole_obj, contact.point, RandomOrientation());
                }
                effect = TryInstantiate(spark_effect, contact.point, RandomOrientation());
            } else {
                PlaySoundFromGroup(sound_hit_concrete, 0.4f);
                hole = TryInstantiate(bullet_hole_obj, contact.point, Quaternion.FromToRotation(new Vector3(0,0,-1), contact.normal) * Quaternion.AngleAxis(UnityEngine.Random.Range(0,360), new Vector3(0,0,1)));
                effect = TryInstantiate(puff_effect, contact.point, RandomOrientation());
            }

            // Offset effect
            if(effect) {
                effect.transform.position += contact.normal * 0.05f;
            }

            // Attach hole
            if(hole){
                if(aim_script) {
                    hole.transform.parent = aim_script.main_camera.transform;
                } else if(robot_script) {
                    hole.transform.parent = robot_script.transform;
                    robot_script.AttachHole(hole.transform, col.transform);
                } else if (rigidbody) {
                    hole.transform.parent = col.transform;
                } else if(level_creator) {
                    hole.transform.parent = level_creator.GetPositionTileDecalsParent(hole.transform.position);
                }
            }
        }

        if(velocity.magnitude > lethal_speed || other.layer != LayerMask.NameToLayer("Character")) {
            if(attach_on_hit) {
                if(aim_script) {
                    transform.parent = aim_script.main_camera.transform;
                } else if(robot_script) {
                    transform.parent = robot_script.transform;
                    robot_script.AttachHole(transform, col.transform);
                } else if(level_creator) {
                    transform.parent = level_creator.GetPositionTileDecalsParent(transform.position);
                } else {
                    transform.parent = col.transform;
                }
            }

            on_collision.Invoke();
        }
    }

    /// <summary> Call Physics.IgnoreCollision for every collider in the parent to prevent unintended collision behaviour </summary>
    public void PreventIntercollisionWithParent() {
        if(!transform.parent) { // No parent
            return;
        }

        Collider[] colliders = transform.parent.gameObject.GetComponents<Collider>();
        foreach (Collider collider in colliders) {
            if(own_colliders.Contains(collider)) {
                continue;
            }

            foreach (Collider own_collider in own_colliders) {
                Physics.IgnoreCollision(collider, own_collider);
            }
        }
    }

    public void DestroyColliders() {
        foreach (var collider in own_colliders) {
            GameObject.Destroy(collider);
        }
    }

    public void DestoryRigidbody() {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if(rigidbody) {
            GameObject.Destroy(rigidbody);
        }
    }
}