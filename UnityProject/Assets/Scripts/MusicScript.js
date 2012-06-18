#pragma strict

var music_layers : AudioClip[];
private var music_sources : AudioSource[];
private var sting_source : AudioSource;
var death_sting : AudioClip;
var win_sting : AudioClip;
private var target_gain : float[];
private var danger = 0.0;
private var global_gain = 1.0;
private var target_global_gain = 1.0;
private var gain_recover_delay = 0.0;
private var danger_level_accumulate = 0.0;
private var mystical = 0.0;
enum MusicEvent {DEAD, WON};

function HandleEvent(event : MusicEvent){
	switch(event){
		case MusicEvent.DEAD:
			target_global_gain = 0.0;
			gain_recover_delay = 1.0;
			sting_source.PlayOneShot(death_sting);
			break;
		case MusicEvent.WON:
			target_global_gain = 0.0;
			gain_recover_delay = 4.0;
			sting_source.PlayOneShot(GameObject.Find("gui_skin_holder").GetComponent(GUISkinHolder).win_sting);
			break;
	}
}

function AddDangerLevel(val : float) {
	danger_level_accumulate += val;
}

function SetMystical(val : float) {
	mystical = val;
}

function Start () {
	music_sources = new AudioSource[music_layers.Length];
	target_gain = new float[music_layers.Length];
	for(var i=0; i<music_layers.length; ++i){
		var source : AudioSource = gameObject.AddComponent(AudioSource);
		source.clip = music_layers[i];
		music_sources[i] = source;
		music_sources[i].loop = true;
		music_sources[i].volume = 0.0;
		target_gain[i] = 0.0;
	}
	sting_source = gameObject.AddComponent(AudioSource);
	music_sources[0].Play();
	music_sources[1].Play();
	music_sources[2].Play();
	music_sources[3].Play();
	music_sources[4].Play();
	target_gain[0] = 1.0;
}

function Update() { 
	danger = Mathf.Max(danger_level_accumulate, danger);
	danger_level_accumulate = 0.0;
}

function FixedUpdate () {
	target_gain[1] = danger;
	target_gain[2] = danger;
	target_gain[3] = Mathf.Max(0.0, danger-0.5);
	target_gain[4] = mystical;
	danger *= 0.99;
	mystical *= 0.99;
	global_gain = Mathf.Lerp(global_gain, target_global_gain, 0.01);
	if(gain_recover_delay > 0.0){
		gain_recover_delay -= Time.deltaTime;
		if(gain_recover_delay <= 0.0){
			target_global_gain = 1.0;
		}
	}

	for(var i=0; i<music_layers.Length; ++i){
		music_sources[i].volume = Mathf.Lerp(target_gain[i], music_sources[i].volume, 0.99) * global_gain;
	}
}