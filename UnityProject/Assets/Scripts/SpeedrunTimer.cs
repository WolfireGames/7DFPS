using UnityEngine;
using System;

public class SpeedrunTimer : MonoBehaviour {
    private float startTime = 0;
    private float endTime = -1;
    private bool isLocked = false;
    private GUIStyle style;

    public Color runningColor = Color.white;
    public Color lockedColor = Color.blue;
    public Color noticeColor = Color.green;

    private int currentSplit = 0;
    private float splitTime = 0;
    private float splitDuration = 0f;
    private float splitOpacity = 0f;
    private string splitText = "";

    void Start() {
        style = GameObject.Find("gui_skin_holder").GetComponent<GUISkinHolder>().gui_skin.label;
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 18;

        StartTimer();
    }

    private void Update() {
        if(splitDuration > 0) {
            splitOpacity = Mathf.Clamp01(splitOpacity + Time.deltaTime * 4);
            splitDuration -= Time.deltaTime;
        } else {
            splitOpacity = Mathf.Clamp01(splitOpacity - Time.deltaTime);
        }
    }

    public string GetTimeString() {
        float time = Time.time;
        if(isLocked) { // Override with locked in time
            time = endTime;
        }

        return FormatDurationString(time - startTime);
    }

    public string FormatDurationString(float duration) {
        return TimeSpan.FromSeconds(duration).ToString(@"mm\:ss\:fff");
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

    public void Split() {
        currentSplit++;
        splitDuration = 5f;
        splitText = $"[{currentSplit}] {FormatDurationString(Time.time - splitTime)}";
        splitTime = Time.time;
    }

    public void OnGUI() {
        if(PlayerPrefs.GetInt("speedrun_timer", 0) != 1) {
            return;
        }

        float width = Screen.width * 0.5f;

        style.normal.textColor = new Color(0.0f,0.0f,0.0f);
        GUI.Label(new Rect(width - 4f, Screen.height - 25f, width + 0.5f, 20 + 0.5f), GetTimeString(), style);
        
        style.normal.textColor = isLocked ? lockedColor : runningColor;
        GUI.Label(new Rect(width - 4.5f, Screen.height - 25f, width, 30f), GetTimeString(), style);

        DrawSplitGUI();
    }

    private void DrawSplitGUI() {
        if(splitOpacity > 0) {
            float width = Screen.width * 0.5f;
            int prevFontSize = style.fontSize;

            style.fontSize = 12;
            style.normal.textColor = new Color(0f, 0f, 0f, splitOpacity);
            GUI.Label(new Rect(width - 4f, Screen.height - 40f, width + 0.5f, 20 + 0.5f), splitText, style);

            noticeColor.a = splitOpacity;
            style.normal.textColor = noticeColor;
            GUI.Label(new Rect(width - 4.5f, Screen.height - 40f, width, 30f), splitText, style);
            style.fontSize = prevFontSize;
        }
    }
}
