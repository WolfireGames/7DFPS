using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ExtentionUtil;
using GunSystemInterfaces;

[CustomEditor(typeof(GunScript))]
public class GunScriptEditor : Editor {
    GunScript gun_script;
    GunSystemsContainer systems;

    private GunAspect excluded_aspects = new GunAspect();
    private GunAspect loaded_aspects = new GunAspect();
    private Type[] required_components = new Type[0];

    GunAspect expanded_aspects = GunAspect.ALL;
    public bool force_show_aspecs = false;
    public static bool autodelete_components = false;

    private Dictionary<string, GunAspect> aspect_groups = GatherGunAspectGroups();
    private GunAspect possible_aspects = GunAspect.ALL;
    private List<Type> possible_components;
    private List<Type> possible_systems;

    private Dictionary<Type, bool> validation_cache_components = new Dictionary<Type, bool>();
    private Dictionary<FieldInfo, bool> validation_cache_fields = new Dictionary<FieldInfo, bool>();

    private static readonly Color ASPECT_LOADED = Color.green;
    private static readonly Color ASPECT_SELECTED = Color.HSVToRGB(.4f, .3f, .9f);

    private static readonly Dictionary<string, GunAspect> gun_presets = new Dictionary<string, GunAspect> {
        {"Select Preset", null},
        {"M1911 Preset", new GunAspect(GunAspect.CHAMBER, GunAspect.MAGAZINE, GunAspect.EXTERNAL_MAGAZINE, GunAspect.SLIDE, GunAspect.SLIDE_LOCK, GunAspect.SLIDE_LOCK_VISUAL, GunAspect.SLIDE_SPRING, GunAspect.SLIDE_VISUAL, GunAspect.HAMMER, GunAspect.THUMB_COCKING, GunAspect.HAMMER_VISUAL, GunAspect.SLIDE_COCKING, GunAspect.TRIGGER, GunAspect.RECOIL, GunAspect.FIRING, GunAspect.THUMB_SAFETY, GunAspect.GRIP_SAFETY, GunAspect.SLIDE_RELEASE_BUTTON, GunAspect.THUMB_SAFETY_VISUAL, GunAspect.GRIP_SAFETY_VISUAL)},
        {"Glock Preset", new GunAspect(GunAspect.CHAMBER, GunAspect.MAGAZINE, GunAspect.EXTERNAL_MAGAZINE, GunAspect.SLIDE, GunAspect.SLIDE_LOCK, GunAspect.SLIDE_LOCK_VISUAL, GunAspect.SLIDE_SPRING, GunAspect.SLIDE_VISUAL, GunAspect.HAMMER, GunAspect.THUMB_COCKING, GunAspect.FIRE_MODE, GunAspect.TRIGGER_COCKING, GunAspect.SLIDE_COCKING, GunAspect.TRIGGER, GunAspect.RECOIL, GunAspect.FIRING, GunAspect.SLIDE_RELEASE_BUTTON, GunAspect.FIRE_MODE_VISUAL)},
        {"Revolver Preset", new GunAspect(GunAspect.MANUAL_LOADING, GunAspect.REVOLVER_CYLINDER, GunAspect.EXTRACTOR_ROD, GunAspect.EXTRACTOR_ROD_VISUAL, GunAspect.CYLINDER_VISUAL, GunAspect.YOKE, GunAspect.YOKE_VISUAL, GunAspect.HAMMER, GunAspect.HAMMER_VISUAL, GunAspect.THUMB_COCKING, GunAspect.TRIGGER_COCKING, GunAspect.TRIGGER, GunAspect.RECOIL, GunAspect.FIRING)},
    };
    private static readonly string[] gun_preset_labels = gun_presets.Keys.ToArray();

    private void Update() {
        loaded_aspects = LoadedAspects();
        required_components = GetRequiredComponents();
        UpdateComponents();
        excluded_aspects = ExcludedAspects();

        UpdateValidationCache();

        // Hide all GunComponents
        foreach(GunComponent gun_component in gun_script.gameObject.GetComponents<GunComponent>()) {
            gun_component.hideFlags = HideFlags.HideInInspector;
        }
    }

    private void OnEnable() {
        gun_script = (GunScript)target;
        systems = gun_script.GetGunSystems();

        possible_aspects = GetAllAspectsUsedInSystem(systems);
        possible_components = typeof(GunComponent).GetAllDerivedTypes(systems.GetType());
        possible_systems = typeof(GunSystemBase).GetAllDerivedTypes(systems.GetType());
        Update();
    }

    /// <summary> Gets every GunAspect that is referenced by a system version </summary>
    public GunAspect GetAllAspectsUsedInSystem(GunSystemsContainer systems) {
        GunAspect usedAspects = new GunAspect();
        foreach(Type system in typeof(GunSystemBase).GetAllDerivedTypes(systems.GetType())) {
            GunAspect inclusive = system.GetCustomAttribute<InclusiveAspectsAttribute>(false)?.inclusive_aspects;
            GunAspect exclusive = system.GetCustomAttribute<ExclusiveAspectsAttribute>(false)?.exclusive_aspects;

            if(inclusive == null && exclusive == null)
                continue;
            
            if(inclusive == null) {
                usedAspects = usedAspects | exclusive;
            } else if(exclusive == null) {
                usedAspects = usedAspects | inclusive;
            } else {
                usedAspects = usedAspects | exclusive | inclusive;
            }
        }
        return usedAspects;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        serializedObject.Update();

        EditorGUILayout.LabelField("Gun Aspects", EditorStyles.boldLabel);
        autodelete_components = EditorGUILayout.Toggle("Autodelete components", autodelete_components);
        int selected_preset = EditorGUILayout.Popup(0, gun_preset_labels);
        if(selected_preset != 0) {
            gun_script.aspect = gun_presets.Values.ElementAt(selected_preset);
            EditorUtility.SetDirty(gun_script);
        }

        if(!Application.isPlaying || force_show_aspecs) {
            DrawAspectButtons();
            DrawAspects();
        } else {
            EditorGUILayout.HelpBox("Aspects hidden during play mode to prevent performance drops.\n\nDepending on the underlying systems, many changes made during play mode won't change anyways.", MessageType.Info);
            force_show_aspecs = GUILayout.Button("Show aspects anyways");
        }

        if(GUILayout.Button("DEBUG: VALIDATE SYSTEMS"))
            ValidateSystems();

        serializedObject.ApplyModifiedProperties();
    }

    private GunAspect ExcludedAspects() {
        GunAspect excluded_aspects = new GunAspect();
        foreach (Type type in possible_systems) {
            ExclusiveAspectsAttribute exclusives = type.GetCustomAttribute<ExclusiveAspectsAttribute>();
            if (exclusives != null && systems.ShouldLoadSystem(type, gun_script.aspect, true)) {
                excluded_aspects = excluded_aspects | exclusives.exclusive_aspects;
            }
        }
        return excluded_aspects;
    }

    private GunAspect LoadedAspects() {
        GunAspect loaded_aspects = new GunAspect();
        foreach (Type type in possible_systems) {
            if (systems.ShouldLoadSystem(type, gun_script.aspect)) {
                loaded_aspects = loaded_aspects | type.GetCustomAttribute<InclusiveAspectsAttribute>().inclusive_aspects;
            }
        }
        return loaded_aspects;
    }

    private Type[] GetRequiredComponents() {
        List<Type> types = new List<Type>();
        foreach (Type type in possible_components) {
            GunAspect component_aspect = type.GetCustomAttribute<GunDataAttribute>().gun_aspect;
            if(loaded_aspects.HasFlag(component_aspect)) {
                types.Add(type);
            }
        }

        return types.ToArray();
    }

    private void UpdateComponents() {
        foreach(Type possible_component in possible_components) {
            bool should_have = required_components.Contains(possible_component);
            bool has_component = gun_script.GetComponent(possible_component) != null;

            if(should_have == has_component)
                continue; // Component is fine, skip to next

            // Component is NOT fine, see what it should have been
            if(should_have) {
                AddGunComponent(possible_component);
            } else if (autodelete_components) {
                RemoveGunComponent(possible_component);
            }
        }
    }

    private void RemoveGunComponent(Type gun_component) {
        Debug.Log($"REMOVING {gun_component}");

        GameObject.DestroyImmediate(gun_script.gameObject.GetComponent(gun_component), true);
        EditorUtility.SetDirty(gun_script);
    }

    private void AddGunComponent(Type gun_component) {
        Debug.Log($"ADDING {gun_component}");

        Component component = gun_script.gameObject.AddComponent(gun_component);
        component.hideFlags = HideFlags.HideInInspector;
        EditorUtility.SetDirty(gun_script);
    }

    public GunComponent GetComponentFromAspect(GunAspect aspect) {
        foreach (Type type in possible_components) {
            GunDataAttribute gun_data = type.GetCustomAttribute<GunDataAttribute>();
            if(gun_data != null && gun_data.gun_aspect.HasFlag(aspect)) {
                return (GunComponent) gun_script.GetComponent(type);
            }
        }
        return null;
    }

    private static Dictionary<string, GunAspect> GatherGunAspectGroups() {
        Dictionary<string, GunAspect> output = new Dictionary<string, GunAspect> {{"None", new GunAspect()}};
        Type type = typeof(GunAspect);
        foreach(GunAspect value in GunAspect.ALL) {
            string key = value.GetGUIGroup();
            if(!output.ContainsKey(key)) {
                output.Add(key, new GunAspect());
            }
            output[key] = output[key] | value;
        }
        return output;
    }

    private string GetFieldLabel(string base_label) {
        return base_label.Replace('_', ' ');
    }

    private bool ShouldShowComponent(Type component) {
        foreach (FieldInfo field in component.GetFields()) {
            if(field.GetCustomAttribute<HideInInspector>() != null) {
                continue;
            }
            return true;
        }
        return false;
    }

    // Drawing
    public void DrawAspectData(GunComponent component) {
        // Init
        var obj = new SerializedObject(component); 
        var prop = obj.GetIterator();
        prop.Next(true);

        // Display properties
        do {
            if(prop.displayName == "Script" || prop.displayName == "Object Hide Flags") {
                continue;
            }

            if(!IsValid(prop, component)) {
                Color temp_color = GUI.color;
                GUI.color = Color.red;
                EditorGUILayout.PropertyField(prop, new GUIContent(GetFieldLabel(prop.displayName)), true);
                GUI.color = temp_color;
            } else {
                EditorGUILayout.PropertyField(prop, new GUIContent(GetFieldLabel(prop.displayName)), true);
            }
        } while (prop.NextVisible(false));

        // Finalize
        obj.ApplyModifiedProperties();
    }

    public void DrawAspects() {
        // Draw Header
        EditorGUI.BeginChangeCheck();
        foreach (GunAspect aspect in possible_aspects) {
            if(aspect.IsEmpty() || !loaded_aspects.HasFlag(aspect))
                continue;

            GunComponent component = GetComponentFromAspect(aspect);
            if(!component || !ShouldShowComponent(component.GetType())) {
                continue;
            }

            bool show_error = !IsValid(component);
            if(show_error)
                BeginError();

            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel++;

            // Show Aspect Header
            bool unfold = EditorGUILayout.Foldout(expanded_aspects.HasFlag(aspect), aspect.GetGUIContent(), true, EditorStyles.label);

            // Display Aspect Data
            if(unfold) {
                DrawAspectData(component);
            }
            
            // Update folded Aspects
            if(unfold != expanded_aspects.HasFlag(aspect)) {
                if(unfold) {
                    expanded_aspects = expanded_aspects | aspect;
                } else {
                    expanded_aspects = ~(~expanded_aspects | aspect);
                }
            }

            // Endgroup
            EditorGUI.indentLevel--;
            GUILayout.EndVertical();
            if(show_error)
                EndError();
        }

        if(EditorGUI.EndChangeCheck())
            UpdateValidationCache();
    }

    private void DrawAspectButtons() {
        GUIStyle style = new GUIStyle(EditorStyles.miniButton);
        style.fontSize = 10;
        style.clipping = TextClipping.Clip;
        style.margin = new RectOffset();

        foreach(var group in aspect_groups) {
            GUILayout.BeginHorizontal();

            GunAspect group_aspect = GunAspect.ALL.Where((value) => { return possible_aspects.HasFlag(value) && group.Value.HasFlag(value);}).ToArray();
            for (int i = 0; i < group_aspect.value.Length; i++) {
                GunAspect current_aspect = group_aspect.value[i];
                if(i % 2 == 0) {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
                
                // Prepare color
                Color color = GUI.color;
                if(loaded_aspects.HasFlag(current_aspect))
                    GUI.color = ASPECT_LOADED;
                else if(gun_script.aspect.HasFlag(current_aspect))
                    GUI.color = ASPECT_SELECTED;

                // Draw Button
                if(GUILayout.Button(current_aspect.GetGUIContent(), style, GUILayout.MinWidth(20))) {
                    gun_script.aspect = gun_script.aspect ^ current_aspect;
                    EditorUtility.SetDirty(gun_script);
                }

                // Restore color
                GUI.color = color;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        if(validation_cache_components.ContainsValue(false))
            EditorGUILayout.HelpBox("One or more Gun Components report an issue!\n\nMake sure every required field is set below!", MessageType.Warning);
    }

    private Color pre_error_color;
    private void BeginError() {
        GUILayout.BeginHorizontal();
        Color temp_color = GUI.color;
        GUI.color = Color.red;
        GUILayout.Box("", GUILayout.MinWidth(1f), GUILayout.MaxWidth(2f), GUILayout.ExpandHeight(true));
        GUI.color = temp_color;
    }

    private void EndError() {
        GUILayout.EndHorizontal();
    }

    // Validation
    /// <summary> Reset validation cache for components and fields, and revalidate each individualy </summary>
    private void UpdateValidationCache() {
        validation_cache_components.Clear();
        validation_cache_fields.Clear();

        // Validate every component
        foreach (GunComponent component in gun_script.GetComponents<GunComponent>()) {
            if(!required_components.Contains(component.GetType())) {
                continue; // This component is attached but not actually used, don't list errors!
            }
            bool valid = true;

            // Validate every field
            foreach (FieldInfo field in component.GetType().GetFields()) {
                bool valid_field = ValidateField(field, component);
                if(!valid_field) {
                    valid = false; // Component is not valid
                }

                // Store valid status of the checked field
                validation_cache_fields.Add(field, valid_field);
            }

            // Store valid status of the checked component
            validation_cache_components.Add(component.GetType(), valid);
        }
    }

    /// <summary> Validate a single component's field and handle Custom Attributes </summary>
    private bool ValidateField(FieldInfo field, GunComponent component) {
        bool valid = true;
        bool is_null = (component == null || field.GetValue(component) == null || field.GetValue(component).Equals(null));

        foreach(object attribute in field.GetCustomAttributes(true)) {
            if(is_null && attribute is HasTransformPathAttribute path_attribute) { // Try to restore a reference if it is missing
                if(RestoreReference(field, component, path_attribute.paths)) {
                    valid = ValidateField(field, component); // Revalidate when we assigned something
                }
            }

            if(attribute is IsNonNull) // This field needs to be set
                if(is_null)
                    valid = false;
        }
        return valid;
    }

    /// <summary> Check if a component is cached as valid </summary>
    private bool IsValid(GunComponent component) {
        if(!validation_cache_components.ContainsKey(component.GetType()))
            return false;
        return validation_cache_components[component.GetType()];
    }

    /// <summary> Check if a property is cached as valid </summary>
    private bool IsValid(SerializedProperty property, GunComponent component) {
        FieldInfo field = component.GetType().GetField(property.name);

        if(!validation_cache_fields.ContainsKey(field))
            return false;
        return validation_cache_fields[field];
    }

    /// <summary> Try to find a transform inside the gun with any of the specified names, and set the field to it. </summary>
    private bool RestoreReference(FieldInfo field, GunComponent component, string[] names) {
        foreach (string name in names) {
            Transform transform = gun_script.transform.Find(name, true);
            if(transform != null) {
                field.SetValue(component, transform);
                return true;
            }
        }
        return false;
    }

    /// <summary> Use Reflection to check if all systems have proper attributes defined and Debug.Log everything is used, but not specified </summary>
    public void ValidateSystems() {
        // Validate Gun Components, map them to GunAspects for later use
        Dictionary<ushort, Type> component_map = new Dictionary<ushort, Type>();
        foreach (Type component_type in typeof(GunComponent).GetAllDerivedTypes()) {
            GunDataAttribute att = component_type.GetCustomAttribute<GunDataAttribute>();
            if(att == null) {
                Debug.LogError($"{component_type} doesn't have a GunDataAttribute assigned. Every GunComponent needs to specify one!");
                continue;
            }

            if(component_map.ContainsKey((ushort)att.gun_aspect)) { // TODO might need to override GunAspect operator==
                Debug.LogError($"{component_type} links to {att.gun_aspect}, but {component_map[(ushort)att.gun_aspect]} has already linked to the same aspect!");
            } else {
                component_map.Add((ushort)att.gun_aspect, component_type);
            }
        }

        // Go over every system
        foreach (Type system_type in typeof(GunSystemBase).GetAllDerivedTypes(systems.GetType())) {
            List<Type> registered = new List<Type>();

            // Gather types the system says it wants to use
            InclusiveAspectsAttribute inc_att = system_type.GetCustomAttribute<InclusiveAspectsAttribute>();
            if(inc_att != null)
                foreach (GunAspect gun_aspect in inc_att.inclusive_aspects.value)
                    if(component_map.ContainsKey((ushort)gun_aspect))
                        registered.Add(component_map[(ushort)gun_aspect]);

            // Gather types the system actually uses as fields
            foreach(FieldInfo field in system_type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                if(field.FieldType.IsSubclassOf(typeof(GunComponent)) && !registered.Contains(field.FieldType))
                    Debug.LogWarning($"{system_type} uses {field.FieldType} but doesn't include it as inclusive aspect!");
        }

        Debug.Log("System validation completed!");
    }
}
