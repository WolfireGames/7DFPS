using UnityEngine;
using System;


public class BulletPileScript : MonoBehaviour {
    public void Start() {
    	GUISkinHolder holder = GameObject.Find("gui_skin_holder").GetComponent<GUISkinHolder>();
    	WeaponHolder weapon_holder = holder.weapon.GetComponent<WeaponHolder>();
        Transform tile_parent = null;
        GameObject level_object = GameObject.Find("LevelObject");
        if(level_object != null) {
            LevelCreatorScript level_creator = level_object.GetComponent<LevelCreatorScript>();

            tile_parent = level_creator.GetPositionTileItemParent(transform.position);
        } else {
            tile_parent = transform.parent;
        }

    	int num_bullets = UnityEngine.Random.Range(1,6);
        if (UnityEngine.Random.Range(0,4) == 0 && weapon_holder.mag_object != null && PlayerPrefs.GetInt("modifier_spawn_magazines") == 1) {
    	    if (weapon_holder.mag_object.TryGetComponent(out mag_script magazinePrefab)) {
                // Give each round individually a chance to be inside the magazine
                int rounds_in_mag = 0;
                for (int i = 0; i < Mathf.Min(num_bullets, magazinePrefab.kMaxRounds); i++) {
                    if (UnityEngine.Random.value > 0.1f) {
                        rounds_in_mag++;
                    }
                }
                num_bullets -= rounds_in_mag;

                mag_script magazine = Instantiate(magazinePrefab);
                magazine.transform.parent = tile_parent;
                magazine.transform.position = transform.position + new Vector3(UnityEngine.Random.Range(-0.1f,0.1f), UnityEngine.Random.Range(0.2f,0.4f), UnityEngine.Random.Range(-0.1f,0.1f));
                magazine.transform.rotation = BulletScript.RandomOrientation();
                magazine.gameObject.AddComponent<Rigidbody>();
                magazine.SetRoundCount(rounds_in_mag);
            }
        }

    	for(int i=0; i<num_bullets; ++i){
    		GameObject bullet = (GameObject)Instantiate(weapon_holder.bullet_object);
            bullet.transform.parent = tile_parent;
    		bullet.transform.position = transform.position + 
    			new Vector3(UnityEngine.Random.Range(-0.1f,0.1f),
    					UnityEngine.Random.Range(0.0f,0.2f),
    					UnityEngine.Random.Range(-0.1f,0.1f));
    		bullet.transform.rotation = BulletScript.RandomOrientation();
    		bullet.AddComponent<Rigidbody>();
    		bullet.GetComponent<ShellCasingScript>().collided = true;
    	}
    	if(UnityEngine.Random.Range(0,4) == 0){
    		GameObject tape = (GameObject)Instantiate(holder.tape_object);
            tape.transform.parent = tile_parent;
    		tape.transform.position = transform.position + 
    			new Vector3(UnityEngine.Random.Range(-0.1f,0.1f),
    					UnityEngine.Random.Range(0.0f,0.2f),
    					UnityEngine.Random.Range(-0.1f,0.1f));
    		tape.transform.rotation = BulletScript.RandomOrientation();		
    	}
    	if(UnityEngine.Random.Range(0,4) == 0 && !holder.has_flashlight){
    		GameObject flashlight = (GameObject)Instantiate(holder.flashlight_object);
            flashlight.transform.parent = tile_parent;
    		flashlight.transform.position = transform.position + 
    			new Vector3(UnityEngine.Random.Range(-0.1f,0.1f),
    					UnityEngine.Random.Range(0.2f,0.4f),
    					UnityEngine.Random.Range(-0.1f,0.1f));
    		flashlight.transform.rotation = BulletScript.RandomOrientation();
    	}
    }
    
    public void Update() {
    
    }
}
