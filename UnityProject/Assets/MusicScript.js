#pragma strict

var music_layers : AudioClip[];
private var music_sources : AudioSource[];
private var target_gain : float[];
private var danger = 0.0;

function SetDangerLevel(val : float) {
	danger = Mathf.Max(danger, val);
}

function Start () {
	music_sources = new AudioSource[music_layers.Length];
	target_gain = new float[music_layers.Length];
	for(var i=0; i<music_layers.length; ++i){
		var source : AudioSource = gameObject.AddComponent(AudioSource);
		source.clip = music_layers[i];
		music_sources[i] = source;
		music_sources[i].volume = 0.0;
		target_gain[i] = 0.0;
	}
	music_sources[0].Play();
	music_sources[1].Play();
	music_sources[2].Play();
	music_sources[3].Play();
	target_gain[0] = 1.0;
}

function FixedUpdate () {
	target_gain[1] = danger;
	target_gain[2] = danger;
	danger *= 0.99;

	for(var i=0; i<music_layers.Length; ++i){
		music_sources[i].volume = Mathf.Lerp(target_gain[i], music_sources[i].volume, 0.99);
	}
}