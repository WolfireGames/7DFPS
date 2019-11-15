using UnityEngine;
using System;
using System.Collections.Generic;

public class GUISkinHolder:MonoBehaviour{
    
    public GUISkin gui_skin;
    public List<AudioClip> sound_scream;
	public List<AudioClip> sound_tape_content;
    public AudioClip sound_tape_start;
    public AudioClip sound_tape_end;
    public AudioClip sound_tape_background;
    public GameObject tape_object;
    public AudioClip win_sting;
    public GameObject[] weapons;
    public GameObject weapon;
    public GameObject flashlight_object;
    public bool has_flashlight = false;

    public GameObject pause_menu;
    
    public void Awake() {
    	weapon = weapons[0];
    	//weapon = weapons[UnityEngine.Random.Range(0,weapons.Length)];
    }
    
    public void Start() {
        Instantiate(pause_menu);
    }
    
    public void Update() {
    }
}