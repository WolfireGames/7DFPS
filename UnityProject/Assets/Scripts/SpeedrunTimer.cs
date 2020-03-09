using UnityEngine;
using System;

public class SpeedrunTimer : MonoBehaviour {
    private float startTime = 0;
    private float endTime = -1;
    private bool isLocked = false;
    private GUIStyle style;

    public Color runningColor = Color.white;
    public Color lockedColor = Color.blue;

    void Start() {
        style = GameObject.Find("gui_skin_holder").GetComponent<GUISkinHolder>().gui_skin.label;
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 18;

        StartTimer();
    }

    public string GetTimeString() {
        float time = Time.time;
        if(isLocked) { // Override with locked in time
            time = endTime;
        }

        return TimeSpan.FromSeconds(time - startTime).ToString(@"mm\:ss\:fff");
    }

    public void StopTimer() {
        if(isLocked) { // Don't update if we already locked it
            return;
        }

        endTime = Time.time;
        isLocked = true;
    }

    public void StartTimer() {
        startTime = Time.time;
        isLocked = false;
    }

    public void OnGUI() {
        if(PlayerPrefs.GetInt("speedrun_timer", 1) != 1) {
            return;
        }

        float width = Screen.width * 0.5f;

        style.normal.textColor = new Color(0.0f,0.0f,0.0f);
        GUI.Label(new Rect(width - 4f, Screen.height - 25f, width + 0.5f, 20 + 0.5f), GetTimeString(), style);
        
        style.normal.textColor = isLocked ? lockedColor : runningColor;
        GUI.Label(new Rect(width - 4.5f, Screen.height - 25f, width, 30f), GetTimeString(), style);
    }
}
