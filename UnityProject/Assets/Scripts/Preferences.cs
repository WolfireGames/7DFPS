using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to store static references to often used PlayerPrefs
/// </summary>
/// <remarks>
/// This class helps to reduces the amount of calls to PlayerPrefs.Get by calling it once and storing the value
/// </remarks>
public class Preferences : MonoBehaviour {
    public static float master_volume = 1f;
    public static float sound_volume = 1f;
    public static float music_volume = 1f;
    public static float voice_volume = 1f;
    public static float mouse_sensitivity = 1f;

    /// <summary>
    /// Retrieve PlayerPref values and update static values
    /// </summary>
    public static void UpdatePreferences() {
        master_volume = PlayerPrefs.GetFloat("master_volume", 0f);
        sound_volume = PlayerPrefs.GetFloat("sound_volume", 0f);
        music_volume = PlayerPrefs.GetFloat("music_volume", 0f);
        voice_volume = PlayerPrefs.GetFloat("voice_volume", 0f);
        mouse_sensitivity = PlayerPrefs.GetFloat("mouse_sensitivity", 0f);
    }
}
