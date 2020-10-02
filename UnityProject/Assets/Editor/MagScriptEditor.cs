using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(mag_script))]
public class MagScriptEditor : Editor {
    public override void OnInspectorGUI() {
        // Draw MagScript
        base.OnInspectorGUI();

        if(!HasLoadPoints()) {
            EditorGUILayout.HelpBox("You need to set up two point objects:\n - point_load\n - point_start_load", MessageType.Error);
        }

        if(GUILayout.Button("Open Bullet Stacker Utility")) {
            EditorWindow.GetWindow(typeof(RoundStackerUtility), false, "Bullet Stacker");
            //EditorWindow.GetWindowWithRect(typeof(RoundStackerUtility), new Rect(0, 0, 300, 200), false, "Bullet Stacker"); // Window variant to force a specific size
        }

        if(!HasRoundPositions()) {
            EditorGUILayout.HelpBox($"Round positions are not set up correctly!\nMake sure you have enough round objects for {((mag_script) target).kMaxRounds} rounds.\n\nThey need to be called \"round_1\", \"round_2\" ... \"round{((mag_script) target).kMaxRounds}\"!", MessageType.Error);
        }
    }

    private bool HasLoadPoints() {
        mag_script mag = (mag_script)target;
        return mag.transform.Find($"point_load") && mag.transform.Find($"point_start_load");
    }

    private bool HasRoundPositions() {
        mag_script mag = (mag_script)target;

        for (int i = 1; i <= mag.kMaxRounds; i++) {
            if(mag.transform.Find($"round_{i}") == null) {
                return false;
            }
        }
        return true;
    }
}
