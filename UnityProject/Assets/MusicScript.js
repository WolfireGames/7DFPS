#pragma strict

var music_layers : AudioClip[];
private var music_sources : AudioSource[];

function Start () {
	music_sources = new AudioSource[music_layers.Length];
	for(var i=0; i<music_layers.length; ++i){
		var source : AudioSource = gameObject.AddComponent(AudioSource);
		source.clip = music_layers[i];
		music_sources[i] = source;
	}
	/*for(i=0; i<music_layers.length-1; ++i){
		music_sources[i].Play();
	}*/
	music_sources[0].Play();
}

function Update () {

}