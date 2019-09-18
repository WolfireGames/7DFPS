using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;


public class optionsmenuscript:MonoBehaviour{
    public bool show_menu = false;

	public GameObject menu;
	public GameObject menuOptions;

	public Component[] settings = new Component[0];
    
    public void OnApplicationPause() {  
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void OnApplicationFocus() {
    	if(!show_menu){
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
    	}
    }
    
    public void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if(PlayerPrefs.GetInt("set_defaults", 1) == 1)
            RestoreDefaults();

        UpdateUIValues();
    }

    public void Update() {
        if(Input.GetKeyDown ("escape") && !show_menu) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            ShowMenu();
        } else if(Input.GetKeyDown ("escape") && show_menu) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            HideMenu();
        }
        
		if(Input.GetMouseButtonDown(0) && !show_menu) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if(show_menu)
        	Time.timeScale = 0.0f;
        else if(Time.timeScale == 0.0f)
        	Time.timeScale = 1.0f;
    }
    
    public void ShowMenu() {
    	show_menu = true;
		menu.SetActive(true);
    }
    
    public void HideMenu() {
    	show_menu = false;
		menuOptions.SetActive(false);
		menu.SetActive(false);
    }

    public bool IsMenuShown() {
    	return show_menu;
    }
    
	public void UpdateInt(Toggle toggle) {
		PlayerPrefs.SetInt(toggle.name, toggle.isOn ? 1 : 0);
	}

	public void UpdateFloat(Slider slider) {
		PlayerPrefs.SetFloat(slider.name, slider.value);
	}

    public void UpdateUIValues() {
		//Set UI values from preferences
		foreach (var component in settings) {
            if(component.GetType() == typeof(Slider))
                ((Slider)component).value = PlayerPrefs.GetFloat(component.name, 1f);
            else if(component.GetType() == typeof(Toggle))
                ((Toggle)component).isOn = (PlayerPrefs.GetInt(component.name, 0) == 1);
		}
    }

    public void RestoreDefaults() {
    	PlayerPrefs.SetInt("set_defaults", 0);
        
        PlayerPrefs.SetFloat("master_volume", 1.0f);
    	PlayerPrefs.SetFloat("sound_volume", 1.0f);
    	PlayerPrefs.SetFloat("music_volume", 1.0f);
    	PlayerPrefs.SetFloat("voice_volume", 1.0f);
    	PlayerPrefs.SetFloat("mouse_sensitivity", 0.2f);
    	PlayerPrefs.SetInt("lock_gun_to_center", 0);
    	PlayerPrefs.SetInt("mouse_invert", 0);
    	PlayerPrefs.SetInt("toggle_crouch", 1);

        PlayerPrefs.SetFloat("fov", 60f);
        PlayerPrefs.SetFloat("post_processing", 1f);
        PlayerPrefs.SetInt("auto_exposure", 1);
    }

    // Functionality
	public void ExitGame() {
		UnityEngine.Application.Quit();
	}

    public PostProcessVolume postProcessVolume;
    public void SetPostProcessingWeight(float weight) {
        postProcessVolume.weight = weight;
    }

    public void SetAutoExposure(bool autoExposure) {
        postProcessVolume.profile.GetSetting<AutoExposure>().enabled.Override(autoExposure);
    }
}
