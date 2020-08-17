using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class optionsmenuscript : MonoBehaviour {
    public static bool show_menu = false;
    public static bool show_mod_ui = false;

    public GameObject menu;
    public GameObject menuOptions;
    public GameObject optionsContent;
    public Camera uiCamera;
    private GraphicRaycaster raycaster;

    private PostProcessLayer postProcessLayer;
    private PostProcessVolume postProcessVolume;
    private AutoExposure autoExposure;
    private Bloom bloom;
    private Vignette vignette;
    private AmbientOcclusion ambientOcclusion;

    public void OnApplicationPause() {  
        UnlockCursor();
    }

    public void OnApplicationFocus() {
        if(!show_menu) {
            LockCursor();
        }
    }

    private void Awake() {
        raycaster = GetComponent<GraphicRaycaster>();

        postProcessVolume = Camera.main.GetComponent<PostProcessVolume>();
        postProcessLayer = Camera.main.GetComponent<PostProcessLayer>();
        autoExposure = postProcessVolume.profile.GetSetting<AutoExposure>();
        bloom = postProcessVolume.profile.GetSetting<Bloom>();
        vignette = postProcessVolume.profile.GetSetting<Vignette>();
        ambientOcclusion = postProcessVolume.profile.GetSetting<AmbientOcclusion>();
    }

    public void Start() {
        LockCursor();

        if(PlayerPrefs.GetInt("set_defaults", 1) == 1) {
            RestoreDefaults();
        }

        Preferences.UpdatePreferences();
        UpdateUIValuesAndApplyDefaults();
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

        raycaster.enabled = !ImGuiUnity.instance.MouseGrabbed;
    }

    public void OnGUI() {
        GUI.depth = -20;
        if(show_menu && Event.current.type == EventType.Repaint) {
            uiCamera.Render();
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
        show_mod_ui = false;
        menu.SetActive(false);
        LockCursor();
        Time.timeScale = 1.0f;

        Preferences.UpdatePreferences();
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

    public void UpdateSwitch(Dropdown dropdown) {
        PlayerPrefs.SetInt(dropdown.name, dropdown.value);
    }

    public void UpdateUIValuesAndApplyDefaults() {
        foreach(Transform transform in optionsContent.transform) {
            if(transform.name.StartsWith("_")) // Don't default settings that start with _
                continue;

            // Update Sliders
            Slider slider = transform.GetComponent<Slider>();
            if(slider != null) {
                if(PlayerPrefs.HasKey(slider.name))
                    slider.SetValueWithoutNotify(PlayerPrefs.GetFloat(slider.name));
                slider.onValueChanged.Invoke(slider.value);
                continue; // Don't need to check for other Setting types
            }

            // Update Toggles
            Toggle toggle = transform.GetComponent<Toggle>();
            if(toggle != null) {
                if(PlayerPrefs.HasKey(toggle.name))
                    toggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt(toggle.name) == 1);
                toggle.onValueChanged.Invoke(toggle.isOn);
                continue;
            }

            // Update Dropdowns
            Dropdown dropdown = transform.GetComponent<Dropdown>();
            if(dropdown != null) {
                if(PlayerPrefs.HasKey(dropdown.name))
                    dropdown.SetValueWithoutNotify(PlayerPrefs.GetInt(dropdown.name));
                dropdown.onValueChanged.Invoke(dropdown.value);
                continue;
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
        PlayerPrefs.SetFloat("gun_distance", 1f);
        PlayerPrefs.SetInt("lock_gun_to_center", 0);
        PlayerPrefs.SetInt("mouse_invert", 0);
        PlayerPrefs.SetInt("toggle_crouch", 1);

        PlayerPrefs.SetFloat("post_processing", 1f);
        PlayerPrefs.SetFloat("ambient_intensity", 0.44f);
        PlayerPrefs.SetFloat("bloom_intensity", 1f);

        PlayerPrefs.SetFloat("auto_exposure_min_luminance", -3.3f);
        PlayerPrefs.SetFloat("auto_exposure_max_luminance", 2.8f);
        PlayerPrefs.SetFloat("auto_exposure_exposure_compensation", 0.93f);
        
        PlayerPrefs.SetInt("antialiasing_mode", 3);
        PlayerPrefs.SetInt("vignette", 0);

        PlayerPrefs.SetInt("ignore_vanilla_guns", 0);
        PlayerPrefs.SetInt("ignore_vanilla_tiles", 0);
    }

    // Functionality
    public void ToggleOptions() {
        menuOptions.SetActive(!menuOptions.activeSelf);
    }

    public void ExitGame() {
        UnityEngine.Application.Quit();
    }

    public void SetPostProcessingEnabled(Toggle toggle) {
        postProcessLayer.enabled = toggle.isOn;
    }
    
    public void SetPostProcessingWeight(float weight) {
        postProcessVolume.weight = weight;
    }

    public void SetAmbientIntensity(float intensity) {
        ambientOcclusion.intensity.Override(intensity);
    }

    public void SetBloomIntensity(float intensity) {
        bloom.intensity.Override(intensity);
    }

    public void SetAutoExposureMinEV(float autoExposureMinLuminance) {
        autoExposure.minLuminance.Override(autoExposureMinLuminance);
    }

    public void SetAutoExposureMaxEV(float autoExposureMaxLuminance) {
        autoExposure.maxLuminance.Override(autoExposureMaxLuminance);
    }

    public void SetAutoExposureExposureCompensation(float autoExposureExposureCompensation) {
        autoExposure.keyValue.Override(autoExposureExposureCompensation);
    }
 
    public void SetAAMode(int mode) {
        postProcessLayer.antialiasingMode = (PostProcessLayer.Antialiasing) mode;
    }

    public void SetVignette(bool enabled) {
        vignette.active = enabled;
    }

    public void SetVSync(bool enabled) {
        QualitySettings.vSyncCount = enabled ? 1 : 0;
    }

    public void OpenModsFolder() {
        Application.OpenURL($"\"{ModManager.GetModsfolderPath()}\"");
    }

    public void ToggleModUI() {
        show_mod_ui = !show_mod_ui;
    }

    public void ReopenCurrentScene() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        HideMenu();
    }

    public void OpenScene(string scene_name) {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene_name);
        HideMenu();
    }
}
