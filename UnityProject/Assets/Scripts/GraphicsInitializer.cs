using UnityEngine;

public class GraphicsInitializer : MonoBehaviour {
    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad() {
        ApplyGraphics();
    }

    public static void ApplyGraphics() {
        ApplyResolution();
        ApplyQuality();
    }

    private static void ApplyResolution() {
        var resolution_id = PlayerPrefs.GetInt("resolution_setting", -1);
        var fullscreen_mode = (FullScreenMode) PlayerPrefs.GetInt("screen_mode_setting", 0);

        if(Screen.resolutions.Length <= resolution_id || resolution_id < 0) { // Just select the highest one if we selected an invalid one
            resolution_id = Screen.resolutions.Length - 1;
        }

        Resolution new_res = Screen.resolutions[resolution_id];
        if(!Screen.Equals(new_res, Screen.currentResolution) || Screen.fullScreenMode != fullscreen_mode || !Screen.fullScreen) {
            Screen.SetResolution(new_res.width, new_res.height, fullscreen_mode, new_res.refreshRate);
        }
    }

    private static void ApplyQuality() {
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("quality_setting", 5), true);
    }
}
