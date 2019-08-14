using UnityEngine;
using System;

public enum MusicEvent {DEAD, WON};

public class MusicScript:MonoBehaviour{
    
    public AudioClip[] music_layers;
    AudioSource[] music_sources;
    float[] music_volume;
    AudioSource sting_source;
    public AudioClip death_sting;
    public AudioClip win_sting;
    float[] target_gain;
    float danger = 0.0f;
    float global_gain = 1.0f;
    float target_global_gain = 1.0f;
    float gain_recover_delay = 0.0f;
    float danger_level_accumulate = 0.0f;
    float mystical = 0.0f;

    public void HandleEvent(MusicEvent event_cs1){
    	switch(event_cs1){
    		case MusicEvent.DEAD:
    			target_global_gain = 0.0f;
    			gain_recover_delay = 1.0f;
    			sting_source.PlayOneShot(death_sting);
    			break;
    		case MusicEvent.WON:
    			target_global_gain = 0.0f;
    			gain_recover_delay = 4.0f;
    			sting_source.PlayOneShot(GameObject.Find("gui_skin_holder").GetComponent<GUISkinHolder>().win_sting);
    			break;
    	}
    }
    
    public void AddDangerLevel(float val) {
    	danger_level_accumulate += val;
    }
    
    public void SetMystical(float val) {
    	mystical = val;
    }
    
    public void Start() {
    	music_sources = new AudioSource[music_layers.Length];
    	music_volume = new float[music_layers.Length];
    	target_gain = new float[music_layers.Length];
    	for(int i=0; i<music_layers.Length; ++i){
    		AudioSource source = gameObject.AddComponent<AudioSource>();
    		source.clip = music_layers[i];
    		music_sources[i] = source;
    		music_sources[i].loop = true;
    		music_sources[i].volume = 0.0f;
    		music_volume[i] = 0.0f;
    		target_gain[i] = 0.0f;
    	}
    	sting_source = gameObject.AddComponent<AudioSource>();
    	music_sources[0].Play();
    	music_sources[1].Play();
    	music_sources[2].Play();
    	music_sources[3].Play();
    	music_sources[4].Play();
    	target_gain[0] = 1.0f;
    }
    
    public void Update() { 
    	danger = Mathf.Max(danger_level_accumulate, danger);
    	danger_level_accumulate = 0.0f;
    	for(int i=0; i<music_layers.Length; ++i){
    		music_sources[i].volume = music_volume[i] * PlayerPrefs.GetFloat("music_volume");
    	}
    	sting_source.volume = PlayerPrefs.GetFloat("music_volume", 1.0f);
    }
    
    public void FixedUpdate() {
    	target_gain[1] = danger;
    	target_gain[2] = danger;
    	target_gain[3] = Mathf.Max(0.0f, danger-0.5f);
    	target_gain[4] = mystical;
    	danger *= 0.99f;
    	mystical *= 0.99f;
    	global_gain = Mathf.Lerp(global_gain, target_global_gain, 0.01f);
    	if(gain_recover_delay > 0.0f){
    		gain_recover_delay -= Time.deltaTime;
    		if(gain_recover_delay <= 0.0f){
    			target_global_gain = 1.0f;
    		}
    	}
    
    	for(int i=0; i<music_layers.Length; ++i){
    		music_volume[i] = Mathf.Lerp(target_gain[i], music_volume[i], 0.99f) * global_gain;
    	}
    }
}
