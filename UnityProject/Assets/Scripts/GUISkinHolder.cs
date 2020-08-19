using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GUISkinHolder : MonoBehaviour {
    public GUISkin gui_skin;
    public LevelCreatorScript levelCreatorScript;
    public List<AudioClip> sound_scream;
	public List<AudioClip> sound_tape_content;
    public AudioClip sound_tape_start;
    public AudioClip sound_tape_end;
    public AudioClip sound_tape_background;
    public GameObject tape_object;
    public AudioClip win_sting;
    private GameObject[] weapons_origin;
    public GameObject[] weapons;
    public GameObject weapon;
    public GameObject flashlight_object;
    public bool has_flashlight = false;

    public GameObject pause_menu;
    
    public void Awake() {
        weapons_origin = weapons;
        if(ModManager.IsModsEnabled()) {
            InsertMods();
        }

        weapon = GetGunHolder();
        weapon.GetComponent<WeaponHolder>().Load();
    }

    private void InsertMods() {
        InsertGunMods();
        InsertTapeMods();
        InsertLevelMods();
    }

    public void InsertGunMods() {
        ModLoadType gun_load_type = (ModLoadType)PlayerPrefs.GetInt("mod_gun_loading", 0);
        if(gun_load_type != ModLoadType.DISABLED) {
            var gunMods = ModManager.GetAvailableMods(ModType.Gun);
            var guns = new List<GameObject>(weapons_origin);
            if(gunMods.Count() > 0 && gun_load_type == ModLoadType.EXCLUSIVE)
                guns.Clear();

            foreach (var mod in gunMods) {
                WeaponHolder placeholder = new GameObject().AddComponent<WeaponHolder>();
                placeholder.gameObject.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
                placeholder.mod = mod;
                placeholder.display_name = mod.steamworksItem.GetName();
                guns.Add(placeholder.gameObject);
            }
            weapons = guns.ToArray();
        }
    }

    private void InsertTapeMods() {
        ModLoadType tape_load_type = (ModLoadType)PlayerPrefs.GetInt("mod_tape_loading", 0);
        if(tape_load_type != ModLoadType.DISABLED) {
            var tapeMods = ModManager.GetAvailableMods(ModType.Tapes);
            if(tapeMods.Count() > 0 && tape_load_type == ModLoadType.EXCLUSIVE)
                sound_tape_content.Clear();

            foreach (var mod in tapeMods) {
                foreach(AudioClip tape in mod.mainAsset.GetComponent<ModTapesHolder>().tapes) {
                    sound_tape_content.Add(tape);
                }
            }
        }
    }

    private void InsertLevelMods() {
        if(levelCreatorScript) {
            ModLoadType tile_load_type = (ModLoadType)PlayerPrefs.GetInt("mod_tile_loading", 0);
            if(tile_load_type != ModLoadType.DISABLED) {
                var tileMods = ModManager.GetAvailableMods(ModType.LevelTile);
                var tiles = new List<GameObject>(levelCreatorScript.level_tiles);
                if(tileMods.Count() > 0 && tile_load_type == ModLoadType.EXCLUSIVE)
                    tiles.Clear();

                foreach (var mod in tileMods) {
                    foreach(GameObject tile in mod.mainAsset.GetComponent<ModTilesHolder>().tile_prefabs) {
                        tiles.Add(tile);
                    }
                }
                levelCreatorScript.level_tiles = tiles.ToArray();
            }
        }
    }

    private GameObject GetGunHolder() {
        int selected_gun = PlayerPrefs.GetInt("selected_gun_index", -1);
        PlayerPrefs.DeleteKey("selected_gun_index");

        if(selected_gun < 0 || selected_gun >= weapons.Length)
            return weapons[Random.Range(0, weapons.Length)];
        return weapons[selected_gun];
    } 
    
    public void Start() {
        Instantiate(pause_menu);
    }
}