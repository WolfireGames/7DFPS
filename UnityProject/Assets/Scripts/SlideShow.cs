using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideShow : MonoBehaviour {
    enum SplashState { FADE_IN, WAIT, FADE_OUT };

    public Texture[] slides;
    private int cur_slide = 0;
    SplashState state; 
    private float fade_in = 0.0f;
    private float fade_out = 0.0f;

    public AudioClip music_a;
    public AudioClip music_b;
    public AudioClip play_sound;
    public AudioClip stop_sound;

    private AudioSource audiosource_music_a;
    private AudioSource audiosource_music_b;
    private AudioSource audiosource_effect;

	void Start () {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1.0f;
        audiosource_music_a = gameObject.AddComponent<AudioSource>();
        audiosource_music_a.loop = true;
        audiosource_music_a.clip = music_a;
        audiosource_music_b = gameObject.AddComponent<AudioSource>();
        audiosource_music_b.loop = true;
        audiosource_music_b.clip = music_b;
        audiosource_music_b.Play();
        audiosource_music_a.Play();
        audiosource_effect = gameObject.AddComponent<AudioSource>();
        //audiosource_effect.PlayOneShot(play_sound);
	}
	
	void Update () {
        AudioListener.volume = Mathf.Min(1.0f * Preferences.master_volume, AudioListener.volume + Time.deltaTime * 2.0f * Preferences.master_volume);
        if(state == SplashState.FADE_IN){
            fade_in = Mathf.Min(1.0f, fade_in + Time.deltaTime * 2.0f);
            if(fade_in == 1.0){
                state = SplashState.WAIT;
            }
        }
        if(state == SplashState.FADE_OUT){
            fade_out = Mathf.Min(1.0f, fade_out + Time.deltaTime * 2.0f);
            if(fade_out == 1.0){
                ++cur_slide;
                if(cur_slide == slides.Length){
                    Application.Quit();
                    UnityEngine.SceneManagement.SceneManager.LoadScene("splashscreen");
                }
                state = SplashState.FADE_IN;
                fade_out = 0.0f;
                fade_in = 0.0f;
            }
        }
	}

    void OnGUI(){
        if(cur_slide >= slides.Length){
            return;
        }
        Color color = new Color(1.0f,1.0f,1.0f,fade_in * (1.0f - fade_out));
        GUI.color = color;
        Texture tex = slides[cur_slide];
        float max_fit = Mathf.Min(Screen.width / (tex.width + 0.0f), Screen.height / (tex.height + 0.0f));
        Debug.Log(max_fit);
        float scale = 1.0f + (Mathf.Sin(Time.time*0.5f)+1.0f)*0.1f;
        scale = max_fit;
        GUI.DrawTexture(new Rect(Screen.width*0.5f-tex.width*scale*0.5f,
                             Screen.height*0.5f-tex.height*scale*0.5f,
                             tex.width*scale,
                             tex.height*scale),
                        slides[cur_slide], ScaleMode.ScaleToFit, true);
        
        if(state == SplashState.WAIT){
            if (Event.current.type == EventType.KeyDown ||
                Event.current.type == EventType.MouseDown)
            {
                state = SplashState.FADE_OUT;
                audiosource_effect.PlayOneShot(stop_sound, Preferences.sound_volume);
                //audiosource_music_a.Stop();
                //audiosource_music_b.Stop();
            }
        }
        /*if(fade_out == 1.0){
            Application.LoadLevel("splash");
        }*/
    }
}
