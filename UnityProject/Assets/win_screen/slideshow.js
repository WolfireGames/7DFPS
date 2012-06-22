#pragma strict

var slides : Texture[];

var cur_slide = 0;
var state = SplashState.FADE_IN;
private var fade_in = 0.0;
private var fade_out = 0.0;

var music_a : AudioClip;
var music_b : AudioClip;
var play_sound : AudioClip;
var stop_sound : AudioClip;

private var audiosource_music_a : AudioSource;
private var audiosource_music_b : AudioSource;
private var audiosource_effect : AudioSource;

function Start () {
	audiosource_music_a = gameObject.AddComponent(AudioSource);
	audiosource_music_a.loop = true;
	audiosource_music_a.clip = music_a;
	audiosource_music_b = gameObject.AddComponent(AudioSource);
	audiosource_music_b.loop = true;
	audiosource_music_b.clip = music_b;
	audiosource_music_b.Play();
	audiosource_music_a.Play();
	audiosource_effect = gameObject.AddComponent(AudioSource);
    //audiosource_effect.PlayOneShot(play_sound);
}

function Update () {
	AudioListener.volume = Mathf.Min(1.0 * PlayerPrefs.GetFloat("master_volume", 1.0), AudioListener.volume + Time.deltaTime * 2.0 * PlayerPrefs.GetFloat("master_volume", 1.0));
	if(state == SplashState.FADE_IN){
		fade_in = Mathf.Min(1.0, fade_in + Time.deltaTime * 2.0);
	    if(fade_in == 1.0){
	    	state = SplashState.WAIT;
		}
	}
	if(state == SplashState.FADE_OUT){
		fade_out = Mathf.Min(1.0, fade_out + Time.deltaTime * 2.0);
	    if(fade_out == 1.0){
	    	++cur_slide;
	    	if(cur_slide == slides.Length){
	    		Application.Quit();
	    		Application.LoadLevel("splashscreen");
	    	}
	    	state = SplashState.FADE_IN;
	    	fade_out = 0.0;
	    	fade_in = 0.0;
		}
	}
}

function OnGUI(){
	if(cur_slide >= slides.Length){
		return;
	}
	var color = Color(1,1,1,fade_in * (1.0 - fade_out));
	GUI.color = color;
	var tex = slides[cur_slide];
	var max_fit = Mathf.Min(Screen.width / (tex.width + 0.0), Screen.height / (tex.height + 0.0));
	Debug.Log(max_fit);
	var scale = 1.0 + (Mathf.Sin(Time.time*0.5)+1.0)*0.1;
	scale = max_fit;
	GUI.DrawTexture(Rect(Screen.width*0.5-tex.width*scale*0.5,
						 Screen.height*0.5-tex.height*scale*0.5,
						 tex.width*scale,
						 tex.height*scale),
					slides[cur_slide], ScaleMode.ScaleToFit, true);
	
	if(state == SplashState.WAIT){
	    if (Event.current.type == EventType.KeyDown ||
	        Event.current.type == EventType.MouseDown)
	    {
	        state = SplashState.FADE_OUT;
   			audiosource_effect.PlayOneShot(stop_sound, PlayerPrefs.GetFloat("sound_volume", 1.0));
	        //audiosource_music_a.Stop();
	        //audiosource_music_b.Stop();
	    }
    }
    /*if(fade_out == 1.0){
		Application.LoadLevel("splash");
	}*/
}