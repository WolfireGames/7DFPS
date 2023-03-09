using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour {

    public Texture splash_back;
    public Texture splash_text;
    public Texture splash_play;
    public Texture splash_credits;

    enum SplashState { FADE_IN, WAIT, FADE_OUT, LOADING };
    SplashState state = SplashState.FADE_IN;

    private float fade_in = 0.0f;
    private float fade_out = 0.0f;
    private float fade_out_delay = 0.0f;

    public AudioClip music_a;
    public AudioClip music_b;
    public AudioClip play_sound;
    public AudioClip stop_sound;

    private AudioSource audiosource_music_a;
    private AudioSource audiosource_music_b;
    private AudioSource audiosource_effect;

	// Use this for initialization
	void Start () {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
	    Time.timeScale = 1;
        audiosource_music_a = gameObject.AddComponent<AudioSource>();
        audiosource_music_a.loop = true;
        audiosource_music_a.clip = music_a;
        audiosource_music_a.volume = Preferences.music_volume;
        audiosource_music_b = gameObject.AddComponent<AudioSource>();
        audiosource_music_b.loop = true;
        audiosource_music_b.clip = music_b;
        audiosource_music_b.volume = Preferences.music_volume;
        audiosource_music_b.Play();
        audiosource_music_a.Play();
        audiosource_effect = gameObject.AddComponent<AudioSource>();
        audiosource_effect.PlayOneShot(play_sound, Preferences.sound_volume);
        audiosource_effect.volume = Preferences.sound_volume;

        // Reset modifiers here. This code is only run once at the start of the game
        PlayerPrefs.SetInt("modifier_green_demon", 0);
        PlayerPrefs.SetInt("modifier_fog", 0);
        PlayerPrefs.SetInt("modifier_spawn_magazines", 0);
	}

	// Update is called once per frame
	void Update () {
        fade_in = Mathf.Min(5.0f, fade_in + Time.deltaTime);
        if(state == SplashState.FADE_OUT){
            fade_out = Mathf.Min(1.0f, fade_in + Time.deltaTime * 2.0f);
            fade_out_delay += Time.deltaTime;
        }
        AudioListener.volume = Preferences.master_volume;
	}

    private void OnGUI() {
        Color color = new Color(fade_in,fade_in,fade_in,1);
        color.r *= 1.0f - fade_out;
        color.g *= 1.0f - fade_out;
        color.b *= 1.0f - fade_out;
        GUI.color = color;
        //GUI.DrawTexture(Rect(0,0,Screen.width, Screen.height),splash_back, ScaleMode.ScaleAndCrop, false);
        float scale = 1.0f + (Mathf.Sin(Time.time*0.5f)+1.0f)*0.1f;
        GUI.DrawTexture(new Rect(Screen.width*(1-scale)*0.5f,Screen.height*(1-scale)*0.5f,Screen.width*scale,Screen.height*scale), splash_back, ScaleMode.ScaleAndCrop, true);
        GUI.DrawTexture(new Rect(Screen.width*0.125f, Screen.height * 0.3f - splash_text.height*0.5f, Screen.width*0.75f,splash_text.height), splash_text, ScaleMode.ScaleToFit, true);
        color.a = color.a * (fade_in-1.6f) * 4.0f;
        GUI.color = color;
        GUI.DrawTexture(new Rect(Screen.width*0.5f - splash_play.width, 
                             Screen.height * 0.7f - splash_play.height*0.5f, 
                             splash_play.width*2.0f,
                             splash_play.height),
                        splash_play, ScaleMode.ScaleToFit, true);
        GUI.color = new Color(color.r, color.g, color.b, 0.35f * (fade_in-3.6f)*4.0f);
        GUI.DrawTexture(new Rect(Screen.width*0.125f, Screen.height * 0.9f - splash_credits.height*0.5f, Screen.width*0.75f,splash_credits.height), splash_credits, ScaleMode.ScaleToFit, true);
        
        if(state == SplashState.FADE_IN || state == SplashState.WAIT){
            if (Event.current.type == EventType.KeyDown ||
                Event.current.type == EventType.MouseDown)
            {
                state = SplashState.FADE_OUT;
                audiosource_effect.PlayOneShot(stop_sound, Preferences.sound_volume);
                audiosource_music_a.Stop();
                audiosource_music_b.Stop();
            }
        }
        if(fade_out_delay >= 0.2 && state == SplashState.FADE_OUT){
            //UnityEngine.SceneManagement.SceneManager.LoadScene("scene");
            //UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneLoaded; 
            //UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("scene",LoadSceneMode.Additive);
            UnityEngine.SceneManagement.SceneManager.LoadScene("scene");
            state = SplashState.LOADING;
        }	
    }

    private void SceneLoaded(Scene arg0, LoadSceneMode arg1) {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= SceneLoaded;
        //UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("splashscreen");
    }
}
