using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(LevelScript))]
public class LevelScriptEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Ground Player spawns")) {
            var player_spawn = ((LevelScript)target).transform.Find("player_spawn");
            var scene = player_spawn.gameObject.scene.GetPhysicsScene();

            Undo.RegisterCompleteObjectUndo(player_spawn.Cast<Transform>().ToArray(), "Undo player spawn grounding");
            foreach (Transform spawnPoint in player_spawn) {
                RaycastHit hit;
                if(scene.Raycast(spawnPoint.position + Vector3.up * 0.001f, Vector3.down, out hit, 10))
                    spawnPoint.position = new Vector3(spawnPoint.position.x, hit.point.y, spawnPoint.position.z);
                else
                    Debug.LogAssertion("No ground found under player object, skipped it while grounding");
            }
        }

        if(GUILayout.Button("Fix spawn objects")) {
            LevelScript ls = (LevelScript)target;

            Transform enemies = ls.transform.Find("enemies");
            Transform player_spawn = ls.transform.Find("player_spawn");
            Transform items = ls.transform.Find("items");

            List<Transform> to_destroy = new List<Transform>();

            GameObject drone_prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Objects/flying_shock_drone_spawn.prefab");
            GameObject turret_prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Objects/stationary_turret_fixed_spawn.prefab");
            GameObject player_start_prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Objects/player_start.prefab");
            GameObject item_pile_prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Objects/item_pile.prefab");

            for(int i = 0; i < enemies.childCount; i++) {
                Transform child = enemies.GetChild(i);
                if(child.name == "flying_shock_drone_object") {
                    if(drone_prefab != null) {
                        GameObject replacement_drone = (GameObject)PrefabUtility.InstantiatePrefab(drone_prefab);
                        Undo.RegisterCreatedObjectUndo(replacement_drone, "Create drone spawn replacement");
                        Transform replacement_transform = replacement_drone.transform;

                        replacement_transform.position = child.position;
                        replacement_transform.rotation = child.rotation;
                        replacement_transform.parent = enemies;

                        to_destroy.Add(child);
                    } else {
                        Debug.LogError("Can't replace flying shock drone, missing prefab reference");
                    }
                } else if(child.name =="stationary_turret_fixed_object") {
                    if(turret_prefab != null) {
                        GameObject replacement_turret = (GameObject)PrefabUtility.InstantiatePrefab(turret_prefab);
                        Undo.RegisterCreatedObjectUndo(replacement_turret, "Create turret spawn replacement");
                        Transform replacement_transform = replacement_turret.transform;

                        replacement_transform.position = child.position;
                        replacement_transform.rotation = child.rotation;
                        replacement_transform.parent = enemies;

                        to_destroy.Add(child);
                    } else {
                        Debug.LogError("Can't replace stationary turret, missing prefab reference");
                    }
                }
            }

            if(player_start_prefab != null) {
                for(int i = 0; i < player_spawn.childCount; i++) {
                    Transform child = player_spawn.GetChild(i);

                    if(child.name == "Player") {
                        GameObject replacement_spawn = (GameObject)PrefabUtility.InstantiatePrefab(player_start_prefab);
                        Undo.RegisterCreatedObjectUndo(replacement_spawn, "Create player spawn replacement");

                        Transform replacement_transform = replacement_spawn.transform;

                        replacement_transform.position = child.position;
                        replacement_transform.rotation = child.rotation;
                        replacement_transform.parent = player_spawn;

                        to_destroy.Add(child);
                    }
                }
            } else {
                Debug.LogError("Unable to replace player, there is no player spawn prefab set");
            }

            if(item_pile_prefab != null) {
                for(int i = 0; i < items.childCount; i++) {
                    Transform child = items.GetChild(i);

                    if(child.name == "bullet pile") {
                        GameObject replacement_item = (GameObject)PrefabUtility.InstantiatePrefab(item_pile_prefab);
                        Undo.RegisterCreatedObjectUndo(replacement_item, "Create item pile replacement");

                        Transform replacement_transform = replacement_item.transform;

                        replacement_transform.position = child.position;
                        replacement_transform.rotation = child.rotation;
                        replacement_transform.parent = items;

                        to_destroy.Add(child);
                    }
                }
            } else {
                Debug.LogError("Unable to replace bullet pile, there is no item pile prefab set");
            }

            foreach(Transform des in to_destroy) {
                Undo.DestroyObjectImmediate(des.gameObject);
            }
        }
    }
}
#endif


public class LevelScript : MonoBehaviour{
    public void Start() {
    	Transform enemies = transform.Find("enemies");
    	if(enemies != null){
    		foreach(Transform child in enemies){
    			if(UnityEngine.Random.Range(0.0f,1.0f) < 0.9f){
    				GameObject.Destroy(child.gameObject);
    			}
    		}
    	}
    	Transform players = transform.Find("player_spawn");
    	if(players != null){
    		int num = 0;
    		foreach(Transform child in players){
    			++num;
    		}
    		int save = UnityEngine.Random.Range(0,num);
    		int j=0;
    		foreach(Transform child in players){
    			if(j != save){
    				GameObject.Destroy(child.gameObject);
    			}
    			++j;
    		}
    	}
    	Transform items = transform.Find("items");
    	if(items != null){
    		foreach(Transform child in items){
    			if(UnityEngine.Random.Range(0.0f,1.0f) < 0.9f){
    				GameObject.Destroy(child.gameObject);
    			}
    		}
    	}
    }
    
    public void Update() {

    }
}