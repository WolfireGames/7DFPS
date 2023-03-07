using UnityEngine;
using System.Collections;
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
        public bool enemies_enabled = false;
        public bool locked = false; // Is some process working with this tile right now?
    }

    public GameObject[] level_tiles;
    public List<Light> shadowed_lights;
    public List<TileInstance> tiles;
    public GameObject turret;
    public GameObject drone;
    public GameObject player_obj;
    public const int TILE_LOAD_DISTANCE = 3;
    Transform player_inventory_transform;
    
    public IEnumerator SpawnTile(int where_cs1, float challenge, bool player, bool instant){
    	GameObject level_obj = level_tiles[UnityEngine.Random.Range(0,level_tiles.Length)];
    	GameObject level = new GameObject(where_cs1 + "_" + level_obj.name);
        GameObject level_enemies = new GameObject("enemies");
        GameObject level_items = new GameObject("items");
        GameObject level_decals = new GameObject("decals");
        level_enemies.transform.parent = level.transform;
        level_items.transform.parent = level.transform;
        level_decals.transform.parent = level.transform;

        TileInstance tile = new TileInstance {
            state = TileInstanceState.Enabled,
            tile_position = where_cs1,
            tile_object = level,
            tile_item_parent = level_items.transform,
            tile_decals_parent = level_decals.transform,
            tile_enemy_parent = level_enemies.transform,
            enemies_enabled = false,
            locked = true,
        };
        tiles.Add(tile);

        level_enemies.SetActive(false);

        foreach(Transform child in level_obj.transform){
    		if(child.gameObject.name != "enemies" && child.gameObject.name != "player_spawn" && child.gameObject.name != "items"){
    			Instantiate(child.gameObject, new Vector3(0.0f,0.0f,(float)(where_cs1*20)) + child.localPosition, child.localRotation, level.transform);

                if(!instant) {
                    yield return null;
                }
    		}
    	}
    	Transform enemies = level_obj.transform.Find("enemies");
    	if(enemies != null){
    		foreach(Transform child in enemies){
    			if(UnityEngine.Random.Range(0.0f,1.0f) <= challenge){
                    if(child.gameObject.name.Contains("flying_shock_drone_spawn")){
                        Instantiate( drone,  new Vector3(0.0f,0.0f,(float)(where_cs1*20)) + child.localPosition + enemies.localPosition, child.localRotation, level_enemies.transform );
                    } else if(child.gameObject.name.Contains("stationary_turret_fixed_spawn")){
                        Instantiate( turret,  new Vector3(0.0f,0.0f,(float)(where_cs1*20)) + child.localPosition + enemies.localPosition, child.localRotation, level_enemies.transform );
                    } else {
                        Instantiate(child.gameObject, new Vector3(0.0f, 0.0f, (float)(where_cs1 * 20)) + child.localPosition + enemies.localPosition, child.localRotation, level_enemies.transform);
                    }

                    if(!instant) {
                        yield return null;
                    }
                }
    		}
    	}
    	Transform items = level_obj.transform.Find("items");
    	if(items != null){
    		foreach(Transform child in items){
    			if(UnityEngine.Random.Range(0.0f,1.0f) <= (player?challenge+0.3f:challenge)){
    				Instantiate(child.gameObject, new Vector3(0.0f,0.0f, where_cs1*20) + child.localPosition + items.localPosition, items.localRotation, level_items.transform);

                    if(!instant) {
                        yield return null;
                    }
    			}
            }
    	}
    	if(player){
    		Transform players = level_obj.transform.Find("player_spawn");
    		if(players != null){
                Transform child = players.GetChild(UnityEngine.Random.Range(0, players.childCount));
                GameObject player_object = Instantiate(player_obj, new Vector3(0.0f,0.7f,(float)(where_cs1*20)) + child.localPosition + players.localPosition, child.localRotation, gameObject.transform);
                player_object.name = "Player";

                GameObject player_inventory = new GameObject("PlayerInventory");
                player_inventory_transform = player_inventory.transform;
                player_inventory_transform.parent = this.gameObject.transform;
    		}
    	}
    	level.transform.parent = this.gameObject.transform;
    	
    	Light[] lights = GetComponentsInChildren<Light>();
    	foreach(Light light in lights){
    		if(light.enabled && light.shadows == LightShadows.Hard){
    			shadowed_lights.Add(light);
    		}
    	}
        tile.locked = false;
    }
    
    public void Start() {
    	shadowed_lights = new List<Light>();
    	tiles = new List<TileInstance>();
    	StartCoroutine(SpawnTile(0, 0.0f, true, true));
        for(int i=-TILE_LOAD_DISTANCE; i <= TILE_LOAD_DISTANCE; ++i){
            CreateTileIfNeeded(i, true);
        }
    }
    
    public void CreateTileIfNeeded(int which, bool instant = false) {
    	bool found = false;
    	foreach(TileInstance tile in tiles){
    		if(tile.tile_position == which){
    			found = true;
    		}
    	}
    	if(!found){
    		//Debug.Log("Spawning tile: "+which);
    		StartCoroutine(SpawnTile(which, Mathf.Min(0.6f,0.1f * Mathf.Abs(which)), false, instant));
    	}
    }

    public void DeleteTile(TileInstance tile) {
        tile.state = TileInstanceState.Destroyed;
        Destroy(tile.tile_object);
        tiles.Remove(tile);
    }

    public void DisableTile(TileInstance tile) {
        tile.state = TileInstanceState.Disabled;
        tile.tile_object.SetActive(false);
    }

    public void EnableTile(TileInstance tile) {
        tile.state = TileInstanceState.Enabled;
        tile.tile_object.SetActive(true);
    }

    public void EnableEnemies(TileInstance tile) {
        tile.enemies_enabled = true;
        tile.tile_enemy_parent.gameObject.SetActive(true);
    }

    public void DisableEnemies(TileInstance tile) {
        tile.enemies_enabled = false;
        tile.tile_enemy_parent.gameObject.SetActive(false);
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
    	Transform main_camera = Camera.main.transform;

    	int tile_x = (int)(main_camera.position.z / 20.0f + 0.5f);
        for(int i = tiles.Count-1; i >= 0; i--) {
            TileInstance tile = tiles[i];
            if(tile.locked) {
                continue;
            }

            int dist = Mathf.Abs(tile_x - tile.tile_position);

            if(tile.enemies_enabled != dist <= 2) {
                if(tile.enemies_enabled) {
                    DisableEnemies(tile);
                } else {
                    EnableEnemies(tile);
                }
            }

            switch(tile.state) {
                case TileInstanceState.Disabled:
                    if(dist <= TILE_LOAD_DISTANCE) {
                        EnableTile(tile);
                    }
                    
                    if(dist > 7) {
                        DeleteTile(tile);
                    }
                    break;
                case TileInstanceState.Enabled:
                    if(dist > TILE_LOAD_DISTANCE) {
                        DisableTile(tile);
                    }
                    break;
            }
        }

    	for(int i=-TILE_LOAD_DISTANCE; i <= TILE_LOAD_DISTANCE; ++i){
            CreateTileIfNeeded(tile_x+i);
        }

    	if(PlayerPrefs.GetInt("shadowed_lights", 1) == 0) {
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
    private void OnDestroy() {
        StopAllCoroutines();
    }
}