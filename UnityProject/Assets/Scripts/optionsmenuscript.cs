using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class optionsmenuscript:MonoBehaviour{
    public static bool show_menu = false;

    public GameObject menu;
    public GameObject menuOptions;
    public GameObject optionsContent;

    private PostProcessVolume postProcessVolume;

    public void OnApplicationPause() {  
        UnlockCursor();
    }

    public void OnApplicationFocus() {
        if(!show_menu) {
            LockCursor();
        }
    }

    public void Start() {
        LockCursor();

        postProcessVolume = Camera.main.GetComponent<PostProcessVolume>();

        if(PlayerPrefs.GetInt("set_defaults", 1) == 1) {
            RestoreDefaults();
        }

        UpdateUIValues();
    }

    public void Update() {
        if(Input.GetKeyDown("escape") && !show_menu) {
            ShowMenu();
        } else if(Input.GetKeyDown("escape") && show_menu) {
            HideMenu();
        }

        if(Input.GetMouseButtonDown(0) && !show_menu) {
            LockCursor();
        }
    }

    private void LockCursor() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowMenu() {
        show_menu = true;
        menu.SetActive(true);
        UnlockCursor();
        Time.timeScale = 0.0f;
    }
    
    public void HideMenu() {
        show_menu = false;
        menu.SetActive(false);
        LockCursor();
        Time.timeScale = 1.0f;
    }

    public static bool IsMenuShown() {
        return show_menu;
    }
    
    public void UpdateInt(Toggle toggle) {
        PlayerPrefs.SetInt(toggle.name, toggle.isOn ? 1 : 0);
    }

    public void UpdateFloat(Slider slider) {
        PlayerPrefs.SetFloat(slider.name, slider.value);
    }

    public void UpdateUIValues() {
        foreach(Transform transform in optionsContent.transform) {

            // Update Sliders
            Slider slider = transform.GetComponent<Slider>();
            if(slider != null) {
                slider.value = PlayerPrefs.GetFloat(slider.name, 1f);
                continue; // Don't need to check for other Setting types
            }

            // Update toggles
            Toggle toggle = transform.GetComponent<Toggle>();
            if(toggle != null) {
                toggle.isOn = (PlayerPrefs.GetInt(toggle.name, 0) == 1);
            }
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
    public void ToggleOptions() {
        menuOptions.SetActive(!menuOptions.activeSelf);
    }

    public void ExitGame() {
        UnityEngine.Application.Quit();
    }

    public void SetPostProcessingWeight(float weight) {
        postProcessVolume.weight = weight;
    }

    public void SetAutoExposure(bool autoExposure) {
        postProcessVolume.profile.GetSetting<AutoExposure>().enabled.Override(autoExposure);
    }
}
