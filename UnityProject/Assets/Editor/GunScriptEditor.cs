using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GunScript))]
[CanEditMultipleObjects]
public class GunScriptEditor : Editor {
    private bool list_sounds = false;
    private GunScript gun_script; // target

    // Tooltip overrides
    private Dictionary<string, string> tooltips = new Dictionary<string, string> {
        {"camera_nearplane_override", "This changes the clipping distance. If you are using a normal sidearm, you probably don't want to touch this.\nLower this if parts of your stock are clipping on a close gun distance setting."},
        {"handed", "Determines if flashlights or other objects can be held while holding the gun in default position:\n - ONE_HANDED: other objects can be held in the second hand\n - TWO_HANDED: The gun needs to be holstered or the grip must be shifted before other objects can be used."},
        {"gun_type", "Determines what general kind of gun we are looking for:\n - AUTOMATIC: The chamber is cycled by a pulled back slide\n - REVOLVER: The chamber needs to cycle by rotating the cylinder"},
        {"magazineType", "Determines how the Gun is loaded:\n - MAGAZINE: Your typical weapon, bullets are stored inside an external magazine\n - CYLINDER: typical for revolvers\n - INTERNAL: typical for shotguns or breach loading guns, rounds are stored inside the gun without a detachable container. (Requires a Magazine inside the Prefab)"},
        {"slideInteractionNeedsHand", "Can we interact with the slide even if we don't have a free hand?\n - TRUE: Grip needs to be changed before interacting with the slide\n - FALSE: We can interact with the slide without a free hand. (Useful for pump action shotguns, as you hold the \"slide\" in one of your hands)"},
    };

    // These *must not* be null
    private List<string> non_null = new List<string> {"bullet_obj", "shell_casing", "casing_with_bullet"};

    // Hide certain properties if gun_scrip doesn't meet requirements
    private Dictionary<string, System.Predicate<GunScript>> predicates = new Dictionary<string, System.Predicate<GunScript>> {
        {"magazine_obj", new System.Predicate<GunScript>((gun_script) => { return ((GunScript)gun_script).magazineType == MagazineType.MAGAZINE;})},
        {"cylinders", new System.Predicate<GunScript>((gun_script) => { return ((GunScript)gun_script).magazineType == MagazineType.CYLINDER;})},
    };

    public override void OnInspectorGUI() {
        // Init
        serializedObject.Update();
        gun_script = target as GunScript;

        // General properties
        SerializedProperty property = serializedObject.GetIterator();
        if(property.NextVisible(true)) {
            do {
                if(IsSoundArray(property))
                    continue;

                if(!ShouldDraw(property))
                    continue;

                if(tooltips.ContainsKey(property.name)) // Is there a custom tooltip provided?
                    EditorGUILayout.PropertyField(property, new GUIContent(property.displayName, tooltips[property.name]), true);
                else
                    EditorGUILayout.PropertyField(property, true);

                if(non_null.Contains(property.name) && property.objectReferenceInstanceIDValue == 0) 
                    DrawWarning($"\"{property.displayName}\" can not be None!");
            } while (property.NextVisible(false));
        }

        // Header for sounds
        GUILayout.Space(7);
        EditorGUILayout.LabelField("Sound Effects", EditorStyles.boldLabel);

        // Sound properties
        if(list_sounds = EditorGUILayout.Foldout(list_sounds, "Sound Options", true)) {
            EditorGUI.indentLevel++;
            DrawSoundOptions();
            EditorGUI.indentLevel--;
        }
        
        // Show Contact data via buttons
        DrawContactOptions();

        // Apply changed
        serializedObject.ApplyModifiedProperties();
    }

    private bool ShouldDraw(SerializedProperty property) {
        if(!predicates.ContainsKey(property.name))
            return true;
        return predicates[property.name].Invoke(gun_script);
    }

    private void DrawWarning(string warning) {
        GUIStyle error_style = new GUIStyle(EditorStyles.label);
        error_style.normal.textColor = Color.red;
        error_style.padding.bottom = 5;
        error_style.padding.left = 10;

        GUILayout.Label(warning, error_style);
    }

    private void DrawContactOptions() {
        GUILayout.BeginVertical("Box");

        GUILayout.Label("Can't find what you want?");

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if(GUILayout.Button("Discord"))
            Application.OpenURL("https://discordapp.com/invite/wCntgVQ");

        if(GUILayout.Button("Github"))
            Application.OpenURL("https://github.com/David20321/7DFPS/issues");

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    // Sound specifics
    private bool IsSoundArray(SerializedProperty property) {
        return property.isArray && property.arrayElementType == "PPtr<$AudioClip>";
    }

    private string CapitalizeFirst(string str) {
        if(str.Length <= 1)
            return str.ToUpper();
        return str[0].ToString().ToUpper() + str.Substring(1);
    }

    private GUIContent SoundPropertyToContentLabel(SerializedProperty property) {
        string label = "";
        string[] parts = property.name.Split('_');

        for (int i = 1; i < parts.Length; i++)
            label += CapitalizeFirst(parts[i]) + " ";

        return new GUIContent(label, property.tooltip);
    }

    private void DrawSoundOptions() {
        SerializedProperty property = serializedObject.GetIterator();

        GUILayout.BeginVertical("box");

        // Controlls
        GUILayout.BeginHorizontal();
        bool force_state = false;
        bool expand = false;
        if(GUILayout.Button("Expand All")) {
            force_state = true;
            expand = true;
        } else if (GUILayout.Button("Shrink All")) {
            force_state = true;
        }
        GUILayout.EndHorizontal();

        // Draw properties
        if(property.NextVisible(true)) {
            do {
                if(!IsSoundArray(property))
                    continue;

                if(force_state)
                    property.isExpanded = expand;
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(property, SoundPropertyToContentLabel(property), true);
                if(!property.isExpanded) { // Display arraysize if not expanded
                    GUILayout.FlexibleSpace();
                    GUILayout.Label($"({property.arraySize})");
                }
                EditorGUILayout.EndHorizontal();
            } while (property.NextVisible(false));
        }
        GUILayout.EndVertical();
    }
}
