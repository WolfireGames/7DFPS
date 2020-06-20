using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class RoundStackerUtility : EditorWindow {
    private GameObject originObject;
    private Space space;
    private float extraOffset;
    private Vector3 offset;
    private Vector3 angularOffset;
    private float extraAngularOffset;
    private int roundCount;
    private int step = 1;

    public void OnGUI() {
        // Settings
        originObject = (GameObject) EditorGUILayout.ObjectField(new GUIContent("Origin round", "The round to start generating from, this round will be duplicated according to the offsets set below."), originObject, typeof(GameObject), true);
        roundCount = EditorGUILayout.IntField(new GUIContent("Round Count", "Amount of rounds generated"), roundCount);
        step = EditorGUILayout.IntSlider(new GUIContent("Round Step", "How much round index increments per round. (Useful for double stacking)"), step, 1, 5);
        space = (Space)EditorGUILayout.EnumPopup(new GUIContent("Offset Space", "World: applied offset is World space\nSelf: applied offset is relative to round's rotation. (useful for curved mags)"), space);
        EditorGUILayout.BeginVertical("box");
        offset = EditorGUILayout.Vector3Field(new GUIContent("Round Offset", "Translation per round"), offset);
        extraOffset = EditorGUILayout.FloatField(new GUIContent("Extra Offset", "Decreases/Increases round density over the stack length"), extraOffset);
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical("box");
        angularOffset = EditorGUILayout.Vector3Field(new GUIContent("Round Angular Offset", "Rotation per round in degrees"), angularOffset);
        extraAngularOffset = EditorGUILayout.FloatField(new GUIContent("Extra Angular Offset", "Decreases/Increases rotation amount over the stack length"), extraAngularOffset);
        EditorGUILayout.EndVertical();

        // Buttons
        EditorGUI.BeginDisabledGroup(!originObject || roundCount < 1); // Don't allow stacking if no reference object is provided
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Stack Bullets")) {
            StackRounds();
        }
        if(GUILayout.Button("Clear Bullets")) {
            ClearRounds();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();
    }

    private void ClearRounds() {
        Undo.SetCurrentGroupName("Undo bullet clearing");
        int undoGroup = Undo.GetCurrentGroup();

        // Find all rounds other than originObject
        List<Transform> cuttingBoard = new List<Transform>();
        foreach (Transform child in originObject.transform.parent)
            if(child && child.name.StartsWith($"round_") && child != originObject.transform)
                cuttingBoard.Add(child);

        // Delete every gathered round
        for (int i = cuttingBoard.Count - 1; i >= 0 ; i--)
            Undo.DestroyObjectImmediate(cuttingBoard[i].gameObject);
        
        Undo.CollapseUndoOperations(undoGroup);
    }

    private void StackRounds() {
        Undo.SetCurrentGroupName("Undo bullet stacking");
        int undoGroup = Undo.GetCurrentGroup();
        int startIndex = GetStartingIndex();

        // Selection
        GameObject[] rounds = new GameObject[roundCount];
        rounds[0] = originObject;
        Selection.activeGameObject = null;

        // Stacking
        for (int i = 1; i < roundCount; i++) {
            // Clear previous round
            Transform prevRound = originObject.transform.parent.Find($"round_{i * step + startIndex}");
            if(prevRound) {
                Undo.DestroyObjectImmediate(prevRound.gameObject);
            }

            // Instantiate new round
            GameObject round = Instantiate(originObject, rounds[i-1].transform.position, rounds[i-1].transform.rotation, originObject.transform.parent);
            round.transform.Translate(offset + offset * extraOffset * i, space);
            round.transform.Rotate(angularOffset + angularOffset * extraAngularOffset * i, space);

            round.name = $"round_{i * step + startIndex}";
            rounds[i] = round;
            Undo.RegisterCreatedObjectUndo(round, "Undo round creation");
        }
        Selection.objects = rounds;
        Undo.CollapseUndoOperations(undoGroup);
    }

    private int GetStartingIndex() {
        if(!originObject.name.StartsWith("round_"))
            throw new System.ArgumentException("Valid rounds inside a magazine need to be called \"round_\" followed by an index starting from 1!");

        int index;
        if(!int.TryParse(originObject.name.Substring(6), out index))
            throw new System.ArgumentException("Valid rounds inside a magazine need to be called \"round_\" followed by an index starting from 1!");

        if(index < 0)
            throw new System.ArgumentException("Round index can not be negative!");

        return index;
    }
}

[CustomEditor(typeof(mag_script))]
public class MagScriptEditor : Editor { // Might want to move this to its own file
    public override void OnInspectorGUI() {
        // Draw MagScript
        base.OnInspectorGUI();

        // Buttons
        if(GUILayout.Button("Open Bullet Stacker Utility")) {
            EditorWindow.GetWindow(typeof(RoundStackerUtility), false, "Bullet Stacker");
            //EditorWindow.GetWindowWithRect(typeof(RoundStackerUtility), new Rect(0, 0, 300, 200), false, "Bullet Stacker"); // Window varient to force a specific size
        }
    }
}
