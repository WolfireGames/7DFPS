#pragma strict

var splash_back : Texture;
var splash_text : Texture;
var splash_play : Texture;
var splash_credits : Texture;
enum SplashState{FADE_IN, WAIT, FADE_OUT};
var state = SplashState.FADE_IN;
var GameScene : GameObject;
private var fade_in = 0.0;
private var fade_out = 0.0;
private var fade_out_delay = 0.0;

var music_a : AudioClip;
var music_b : AudioClip;
var play_sound : AudioClip;
var stop_sound : AudioClip;

private var audiosource_music_a : AudioSource;
private var audiosource_music_b : AudioSource;
private var audiosource_effect : AudioSource;

function Awake () {
    Application.targetFrameRate = 60;
}

function Start () {
	audiosource_music_a = gameObject.AddComponent(AudioSource);
	audiosource_music_a.loop = true;
	audiosource_music_a.clip = music_a;
	audiosource_music_a.volume = PlayerPrefs.GetFloat("music_volume");
	audiosource_music_b = gameObject.AddComponent(AudioSource);
	audiosource_music_b.loop = true;
	audiosource_music_b.clip = music_b;
	audiosource_music_b.volume = PlayerPrefs.GetFloat("music_volume");
	audiosource_music_b.Play();
	audiosource_music_a.Play();
	audiosource_effect = gameObject.AddComponent(AudioSource);
    audiosource_effect.PlayOneShot(play_sound, PlayerPrefs.GetFloat("sound_volume", 1.0));
	audiosource_effect.volume = PlayerPrefs.GetFloat("sound_volume");
}

function Update () {
	fade_in = Mathf.Min(5.0, fade_in + Time.deltaTime);
	if(state == SplashState.FADE_OUT){
		fade_out = Mathf.Min(1.0, fade_in + Time.deltaTime * 2.0);
		fade_out_delay += Time.deltaTime;
	}
	AudioListener.volume = PlayerPrefs.GetFloat("master_volume", 1.0);
}

function OnGUI(){
	var color = Color(fade_in,fade_in,fade_in,1);
	color.r *= 1.0 - fade_out;
	color.g *= 1.0 - fade_out;
	color.b *= 1.0 - fade_out;
	GUI.color = color;
	//GUI.DrawTexture(Rect(0,0,Screen.width, Screen.height),splash_back, ScaleMode.ScaleAndCrop, false);
	var scale = 1.0 + (Mathf.Sin(Time.time*0.5)+1.0)*0.1;
	GUI.DrawTexture(Rect(Screen.width*(1-scale)*0.5,Screen.height*(1-scale)*0.5,Screen.width*scale,Screen.height*scale), splash_back, ScaleMode.ScaleAndCrop, true);
	GUI.DrawTexture(Rect(Screen.width*0.125, Screen.height * 0.3 - splash_text.height*0.5, Screen.width*0.75,splash_text.height), splash_text, ScaleMode.ScaleToFit, true);
	GUI.color.a *= (fade_in-1.6)*4.0;
	GUI.DrawTexture(Rect(Screen.width*0.5 - splash_play.width, 
						 Screen.height * 0.7 - splash_play.height*0.5, 
						 splash_play.width*2.0,
						 splash_play.height),
				    splash_play, ScaleMode.ScaleToFit, true);
	GUI.color = Color(color.r, color.g, color.b, 0.35 * (fade_in-3.6)*4.0);
	GUI.DrawTexture(Rect(Screen.width*0.125, Screen.height * 0.9 - splash_credits.height*0.5, Screen.width*0.75,splash_credits.height), splash_credits, ScaleMode.ScaleToFit, true);
	
	if(state != SplashState.FADE_OUT){
	    if (Event.current.type == EventType.KeyDown ||
	        Event.current.type == EventType.MouseDown)
	    {
	        state = SplashState.FADE_OUT;
   			audiosource_effect.PlayOneShot(stop_sound, PlayerPrefs.GetFloat("sound_volume", 1.0));
	        audiosource_music_a.Stop();
	        audiosource_music_b.Stop();
	    }
    }
    if(fade_out_delay >= 0.2){
		Application.LoadLevel("scene");
	}	
}