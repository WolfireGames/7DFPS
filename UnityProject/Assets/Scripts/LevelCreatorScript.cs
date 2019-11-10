using UnityEngine;
using System;
using System.Collections.Generic;


public class LevelCreatorScript:MonoBehaviour{
    public enum TileInstanceState {
        Destroyed,
        Disabled,
        Enabled,
    }
    public class TileInstance {
        public TileInstanceState state = TileInstanceState.Destroyed;
        public int tile_position;
        public GameObject tile_object;
        public Transform tile_enemy_parent;
        public Transform tile_item_parent;
        public Transform tile_decals_parent;
    }

    public GameObject[] level_tiles;
    public List<Light> shadowed_lights;
    public List<TileInstance> tiles;
    public GameObject turret;
    public GameObject drone;
    public GameObject player_obj;
    Transform player_inventory_transform;
    
    public void SpawnTile(int where_cs1,float challenge,bool player){
    	GameObject level_obj = level_tiles[UnityEngine.Random.Range(0,level_tiles.Length)];
    	GameObject level = new GameObject(where_cs1 + "_" + level_obj.name);
        GameObject level_enemies = new GameObject("enemies");
        GameObject level_items = new GameObject("items");
        GameObject level_decals = new GameObject("decals");
        level_enemies.transform.parent = level.transform;
        level_items.transform.parent = level.transform;
        level_decals.transform.parent = level.transform;

    	GameObject child_obj = null;
        foreach(Transform child in level_obj.transform){
    		if(child.gameObject.name != "enemies" && child.gameObject.name != "player_spawn" && child.gameObject.name != "items"){
    			child_obj = (GameObject)Instantiate(child.gameObject, new Vector3(0.0f,0.0f,(float)(where_cs1*20)) + child.localPosition, child.localRotation);
    			child_obj.transform.parent = level.transform;
    		}
    	}
    	Transform enemies = level_obj.transform.Find("enemies");
    	if(enemies != null){
    		foreach(Transform child in enemies){
    			if(UnityEngine.Random.Range(0.0f,1.0f) <= challenge){
                     GameObject go = null;
                     if(child.gameObject.name == "flying_shock_drone_spawn"){
                        go = (GameObject)Instantiate( drone,  new Vector3(0.0f,0.0f,(float)(where_cs1*20)) + child.localPosition + enemies.localPosition, child.localRotation );
                        go.transform.parent = level_enemies.transform;
                     } else if(child.gameObject.name == "stationary_turret_fixed_spawn"){
                        go = (GameObject)Instantiate( turret,  new Vector3(0.0f,0.0f,(float)(where_cs1*20)) + child.localPosition + enemies.localPosition, child.localRotation );
                        go.transform.parent = level_enemies.transform;
                     }
    			}
    		}
    	}
    	Transform items = level_obj.transform.Find("items");
    	if(items != null){
    		foreach(Transform child in items){
    			if(UnityEngine.Random.Range(0.0f,1.0f) <= (player?challenge+0.3f:challenge)){
    				child_obj = (GameObject)Instantiate(child.gameObject, new Vector3(0.0f,0.0f,(float)(where_cs1*20)) + child.localPosition + items.localPosition, items.localRotation);
    				child_obj.transform.parent = level_items.transform;
    			}
    		}
    	}
    	if(player){
    		Transform players = level_obj.transform.Find("player_spawn");
    		if(players != null){
    			int num = 0;
    			foreach(Transform child in players){
    				++num;
    			}
    			int save = UnityEngine.Random.Range(0,num);
    			int j=0;
    			foreach(Transform child in players){
    				if(j == save){
    					child_obj = (GameObject)Instantiate(player_obj, new Vector3(0.0f,0.7f,(float)(where_cs1*20)) + child.localPosition + players.localPosition, child.localRotation);
    					child_obj.transform.parent = this.gameObject.transform;
    					child_obj.name = "Player";

                        GameObject player_inventory = new GameObject("PlayerInventory");
                        player_inventory_transform = player_inventory.transform;
                        player_inventory_transform.parent = this.gameObject.transform;
    				}
    				++j;
    			}
    		}
    	}
    	level.transform.parent = this.gameObject.transform;
    	
    	Light[] lights = GetComponentsInChildren<Light>();
    	foreach(Light light in lights){
    		if(light.enabled && light.shadows == LightShadows.Hard){
    			shadowed_lights.Add(light);
    		}
    	}
    	tiles.Add(new TileInstance {state = TileInstanceState.Enabled,
            tile_position = where_cs1,
            tile_object = level,
            tile_item_parent = level_items.transform,
            tile_decals_parent = level_decals.transform,
            tile_enemy_parent = level_enemies.transform,
        } );
    }
    
    public void Start() {
    	shadowed_lights = new List<Light>();
    	tiles = new List<TileInstance>();
    	SpawnTile(0,0.0f,true);
    	for(int i=-6; i <= 6; ++i){
    		CreateTileIfNeeded(i);
    	}
    }
    
    public void CreateTileIfNeeded(int which){
    	bool found = false;
    	foreach(TileInstance tile in tiles){
    		if(tile.tile_position == which){
    			found = true;
    		}
    	}
    	if(!found){
    		//Debug.Log("Spawning tile: "+which);
    		SpawnTile(which, Mathf.Min(0.6f,0.1f * Mathf.Abs(which)), false);
    	}
    }

    public void DeleteTile(int tile_position) {
        for(int i = tiles.Count-1; i >= 0; i--) {
            if(tiles[i].tile_position == tile_position) {
                tiles[i].state = TileInstanceState.Destroyed;
                Destroy(tiles[i].tile_object);
                tiles.RemoveAt(i);
            }
        }
    }

    public void DisableTile(int tile_position) {
        for(int i = 0; i < tiles.Count; i++) {
            if(tiles[i].tile_position == tile_position) {
                tiles[i].state = TileInstanceState.Disabled;
                tiles[i].tile_object.SetActive(false);
            }
        }
    }

    public void EnableTile(int tile_position) {
        for(int i = 0; i < tiles.Count; i++) {
            if(tiles[i].tile_position == tile_position) {
                tiles[i].state = TileInstanceState.Enabled;
                tiles[i].tile_object.SetActive(true);
            }
        }
    }

    public TileInstance GetTileAtPosition(int pos) {
        for(int i = 0; i < tiles.Count; i++) {
            if(tiles[i].tile_position == pos) {
                return tiles[i]; 
            }
        }
        return null;
    }

    public int GetTilePosition(Vector3 tile_position) {
        return Mathf.FloorToInt((tile_position.z+10.0f)/20.0f);
    }

    public TileInstance GetTileAtPosition(Vector3 tile_position) {
        int pos = GetTilePosition(tile_position);
        //Debug.Log("position " + tile_position.z + " gives tile id " + pos);

        return GetTileAtPosition(pos);
    }

    public Transform GetPositionTileItemParent(Vector3 tile_position) {
        TileInstance tile_instance = GetTileAtPosition(tile_position);

        if(tile_instance != null) {
            return tile_instance.tile_item_parent;
        } else {
            return null;
        }
    }

    public Transform GetPositionTileDecalsParent(Vector3 tile_position) {
        TileInstance tile_instance = GetTileAtPosition(tile_position);

        if(tile_instance != null) {
            return tile_instance.tile_decals_parent;
        } else {
            return null;
        }
    }

    public Transform GetPositionTileEnemiesParent(Vector3 tile_position) {
        TileInstance tile_instance = GetTileAtPosition(tile_position);

        if(tile_instance != null) {
            return tile_instance.tile_enemy_parent;
        } else {
            return null;
        }
    }

    public Transform GetPlayerInventoryTransform() {
        return player_inventory_transform;
    }
    
    public void Update() {
    	Transform main_camera = GameObject.Find("Main Camera").transform;
    	int tile_x = (int)(main_camera.position.z / 20.0f + 0.5f);

        for(int i = tiles.Count-1; i >= 0; i--) {
            int dist = Math.Abs(tile_x - tiles[i].tile_position);
            if(dist < 3) {
                if(tiles[i].state == TileInstanceState.Disabled) {
                    EnableTile(tiles[i].tile_position);
                }
            } else if(dist > 4) {
                if(tiles[i].state == TileInstanceState.Enabled) {
                    DisableTile(tiles[i].tile_position);
                }
            } else if(dist > 7) {
                if(tiles[i].state == TileInstanceState.Disabled) {
                    DeleteTile(tiles[i].tile_position);
                }
            }
        }

    	for(int i=-4; i <= 4; ++i){
    		CreateTileIfNeeded(tile_x+i);
    	}


    	foreach(Light light in shadowed_lights){
    		if(light == null){
    			Debug.Log("LIGHT IS MISSING");
    		}
    		if(light != null){
    			float shadowed_amount = Vector3.Distance(main_camera.position, light.gameObject.transform.position);
    			float shadow_threshold = Mathf.Min(30.0f,light.range*2.0f);
    			float fade_threshold = shadow_threshold * 0.75f;
    			if(shadowed_amount < shadow_threshold){
    				light.shadows = LightShadows.Hard;
    				light.shadowStrength = Mathf.Min(1.0f, 1.0f-(fade_threshold - shadowed_amount) / (fade_threshold - shadow_threshold));
    			} else {
    				light.shadows = LightShadows.None;
    			}
    		}
    	}
    }
}