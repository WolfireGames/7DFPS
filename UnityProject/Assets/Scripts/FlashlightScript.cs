using UnityEngine;
using System;


public class FlashlightScript:MonoBehaviour{
    
    public AnimationCurve battery_curve;
    public AudioClip sound_turn_on;
    public AudioClip sound_turn_off;
    float kSoundVolume = 0.3f;
    public bool switch_on = false;
    const float max_battery_life = 60*60*5.5f;
    float battery_life_remaining = max_battery_life;
    
    float initial_pointlight_intensity;
    float initial_spotlight_intensity;

    Light pointlight;
    Light spotlight;

    public void Awake() {
    	switch_on = false;// Random.Range(0.0,1.0) < 0.5;
    	pointlight = transform.Find("Pointlight").GetComponent<Light>();
    	spotlight = transform.Find("Spotlight").GetComponent<Light>();
    }
    
    public void Start() {
    	initial_pointlight_intensity = pointlight.intensity;
    	initial_spotlight_intensity = spotlight.intensity;
    	battery_life_remaining = UnityEngine.Random.Range(max_battery_life*0.2f, max_battery_life);
    }
    
    public void TurnOn(){
    	if(!switch_on){
    		switch_on = true;
    		GetComponent<AudioSource>().PlayOneShot(sound_turn_on, kSoundVolume * Preferences.sound_volume);
    	}
    }
    
    public void TurnOff(){
    	if(switch_on){
    		switch_on = false;
    		GetComponent<AudioSource>().PlayOneShot(sound_turn_off, kSoundVolume * Preferences.sound_volume);
    	}
    }
    
    public void ToggleSwitch(){
        if(switch_on){
            TurnOff();
        } else {
            TurnOn();
        }
    }
    public void Update() {
    	if(switch_on){
    		battery_life_remaining -= Time.deltaTime;
    		if(battery_life_remaining <= 0.0f){
    			battery_life_remaining = 0.0f;
    		}
    		float battery_curve_eval = battery_curve.Evaluate(1.0f-battery_life_remaining/max_battery_life);
    		pointlight.intensity = initial_pointlight_intensity * battery_curve_eval * 8.0f;
    		spotlight.intensity = initial_spotlight_intensity * battery_curve_eval * 3.0f;
    		pointlight.enabled = true;
    		spotlight.enabled = true;
    	} else {
    		pointlight.enabled = false;
    		spotlight.enabled = false;
    	}
    	if(GetComponent<Rigidbody>() != null){
    		pointlight.enabled = true;
    		pointlight.intensity = 1.0f + Mathf.Sin(Time.time * 2.0f);
    		pointlight.range = 1.0f;
    	} else {
    		pointlight.range = 10.0f;
    	}
    }
}