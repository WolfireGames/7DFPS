using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Cheats : MonoBehaviour {
    public static bool hasCheated = false;
    public static bool god_mode = false;
    public static bool slomo_mode = false;
    public static bool infinite_ammo = false;
    float cheat_delay = 0.0f;

    public AudioClip[] sound_cheat_toggle;

    private int[] progress = new int[cheats.Count];
    private static Dictionary<string, Action> cheats = new Dictionary<string, Action> {
        {"iddqd", () => { god_mode =! god_mode; } },
        {"slomo", () => { slomo_mode =! slomo_mode; ToggleSlomo(); } },
        {"idinf", () => { infinite_ammo =! infinite_ammo; } },
        {"idkfa", () => { GiveBullets(); } }
    };

    public static void ResetCheats() {
        Cheats.hasCheated = false;
        Cheats.god_mode = false;
        Cheats.slomo_mode = false;
        Cheats.infinite_ammo = false;
    }

    // Cheat functionality

    public static void GiveBullets() {
        AimScript aimScript = GameObject.FindObjectOfType<AimScript>();
        
        if(aimScript.loose_bullets.Count < 30) {
            aimScript.PlaySoundFromGroup(aimScript.sound_bullet_grab, 0.2f);
        }

        while(aimScript.loose_bullets.Count < 30) {
            aimScript.AddLooseBullet(true);
        }
    }

    public static void ToggleSlomo() {
        if(Time.timeScale == 1.0f){
            Time.timeScale = 0.1f;
        } else {
            Time.timeScale = 1.0f;
        }
    }

    // Cheat Management

    public void PlaySoundFromGroup(AudioClip[] group, float volume){
        int which_shot = UnityEngine.Random.Range(0, group.Length);
        GetComponent<AudioSource>().PlayOneShot(group[which_shot], volume * Preferences.sound_volume);
    }

    void Update() {
        // Update timer
        if(cheat_delay > 0f) {
            cheat_delay -= Time.unscaledDeltaTime;
            if(cheat_delay <= 0) { // Time is up, reset progress
                progress = new int[progress.Length];
            }
        }

        // Update Cheats
        for (int i = 0; i < progress.Length; i++) {
            KeyValuePair<string, Action> cheat = cheats.ElementAt(i);
            if(Input.GetKeyDown(cheat.Key[progress[i]].ToString())) {
                progress[i]++;
                if(progress[i] >= cheat.Key.Length) { // Cheat used
                    cheat.Value.Invoke();
                    hasCheated = true;
                    progress[i] = 0;
                    PlaySoundFromGroup(sound_cheat_toggle, 1.0f);
                }
                cheat_delay = 1f;
            }
        }
    }

    void Awake() {
        ResetCheats(); // Remove all cheats when a new round starts
    }
}
