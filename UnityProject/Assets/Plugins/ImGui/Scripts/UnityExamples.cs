using System.Collections.Generic;
using UnityEngine;
using ImGuiNET;
using System;
using UnityEngine.Profiling;
using IconFonts;
using System.Reflection;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;

public class UnityExamples : MonoBehaviour
{
    class LogEntry {        
        public string condition;
        public string stack_trace;
        public LogType type;
        public DateTime time;
    }
    List<LogEntry> log_entries = new List<LogEntry>();
    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    Recorder self_recorder;
    CustomSampler sampler;
    Vector4 private_color = new Vector4(1,0,0,1);
    Vector4 inherited_color = new Vector4(0.5f,0.5f,1,1);

    const int max_frame_times = 100;
    float[] frame_times = new float[max_frame_times];
    char[] rename_obj = new char[512];
    const int kFilePathBufSize = 1024;
    char[] file_path_buf = new char[kFilePathBufSize];
    string file_browse_path;

    bool show_hidden;

    public void LogCallback(string _condition, string _stackTrace, LogType _type) {
        log_entries.Add(new LogEntry() {condition = _condition, stack_trace = _stackTrace, type = _type, time = DateTime.Now});
    }

    private void Start() {
        Application.logMessageReceived += LogCallback;
        sampler = CustomSampler.Create("UnityExamplesRecorder");
        self_recorder = Recorder.Get("UnityExamplesRecorder");
        self_recorder.enabled = true;
        file_browse_path = Directory.GetCurrentDirectory();
    }

    // From https://answers.unity.com/questions/43422/how-to-implement-show-in-explorer.html
    public void ShowExplorer(string itemPath) {
        itemPath = itemPath.Replace(@"/", @"\");   // explorer doesn't like front slashes
        System.Diagnostics.Process.Start("explorer.exe", "/select,"+itemPath);
    }

    GameObject selected;

    // Recursively list all children of a Transform
    void DrawHierarchy(Transform transform){
        // Color or hide entry if it is hidden
        GameObject game_object = transform.gameObject;
        Vector4 color = ImGui.GetStyleColorVec4(ImGuiCol.Text);
        if(((transform.hideFlags & HideFlags.HideAndDontSave) != 0) || ((transform.hideFlags & HideFlags.HideInHierarchy) != 0)){
            color = new Vector4(1,0,0,1);
            if(!show_hidden){
                return;
            }
        }
        // Darken entry if it is inactive
        if(!game_object.activeInHierarchy){
            color = Vector4.Scale(color, new Vector4(.5f,.5f,.5f, 1f));
        }
        bool is_selected = (game_object == selected);
        bool is_leaf = (transform.childCount == 0);
        // Different flags for leaves and branches
        var flags = is_leaf?ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen | (is_selected ? ImGuiTreeNodeFlags.Selected : 0)
                           :ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick | (is_selected ? ImGuiTreeNodeFlags.Selected : 0);
        ImGui.PushStyleColor(ImGuiCol.Text, color);
        bool open = ImGui.TreeNodeEx(transform.name, flags);
        ImGui.PopStyleColor();
        if(ImGui.IsItemClicked()){
            selected = game_object;
            rename_obj = (game_object.name.PadRight(512, '\0')).ToCharArray();
        }
        if(open && !is_leaf){ // Draw children
            for(int i=0; i<transform.childCount; ++i){
                DrawHierarchy(transform.GetChild(i));
            }
            ImGui.TreePop();
        }
    }

    // See https://github.com/ocornut/imgui/issues/759
    void ColumnSeparator() {
        var draw_list = ImGui.GetWindowDrawList();
        var p = ImGui.GetCursorScreenPos();
        draw_list.AddLine(new Vector2(p.x - 9999, p.y), new Vector2(p.x + 9999, p.y), ImGui.GetColorU32(ImGuiCol.Border));
    }

    object GetValue(MemberInfo info, object obj, Type t){
        object value = null;
        if(t == typeof(PropertyInfo)){
            var prop_info = (PropertyInfo)info;
            if(prop_info.GetIndexParameters().Length != 0){
                return null; // Maybe figure out how to handle this sometime
            }
            try {
                value = prop_info.GetValue(obj);
            } catch (Exception exc){
                Debug.LogError($"Error getting PropertyInfo value! {exc}");
                return null;
            }
        } else if(t == typeof(FieldInfo)){
            try {
                value = ((FieldInfo)info).GetValue(obj);
            } catch (Exception exc){
                Debug.LogError($"Error getting FieldInfo value! {exc}");
                return null;
            }
        }
        return value;
    }
    
    void SetValue(MemberInfo info, object obj, Type t, object value){
        if(t == typeof(PropertyInfo)){
            obj.GetType().GetProperty(info.Name).SetValue(obj, value);
        } else if(t == typeof(FieldInfo)){
            obj.GetType().GetField(info.Name).SetValue(obj, value);
        }
    }

    bool DrawValueInspector(string name, bool disabled, ref object value, int instance_id){
        bool custom_inspector = false;
        bool changed = false;
        if(value != null){
            custom_inspector = true;
            var type = value.GetType();
            if(EditFieldTypes.ContainsKey(type)){
                var func = EditFieldTypes[type];

                float width = ImGui.GetContentRegionAvailWidth();
                float height = ImGui.GetFontSize()+ImGui.GetStyle().ItemSpacing.y;
        
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, Vector2.zero);
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.zero);
                if(type != typeof(Matrix4x4)){
                    ImGui.BeginChild($"{name}_{instance_id}_label_region", new Vector2(width / 3f, height), false, ImGuiWindowFlags.NoScrollWithMouse);
                    ImGui.Text($"{name}:");
                    ImGui.EndChild();
                } else {
                    ImGui.Text($"{name}:");
                }

                if(func(name, ref value, disabled) && !disabled){
                    changed = true;
                }
                
                ImGui.PopStyleVar(2);
            } else if (type.IsGenericType && value is IList) {
                var list = (IList)value;
                var display_str = $"{name} (List<{type.GetGenericArguments()[0]}>[{list.Count}])###{name}_{instance_id}";
                if(list.Count == 0) {
                    ImGui.Text(display_str);
                } else if (ImGui.TreeNode(display_str)){
                    foreach(var entry in (IEnumerable)value){
                        if(entry == null) {
                            ImGui.Text("null");
                        } else {
                            ImGui.Text(entry.ToString());
                        }
                    }
                    ImGui.TreePop();
                }
            } else if (type.IsArray) {
                var array = (Array)value;
                var display_str = $"{name} (Array<{type.GetElementType()}>[{array.Length}])###{name}_{instance_id}";
                if (array.Length == 0) {
                    ImGui.Text(display_str);
                } else if (ImGui.TreeNode(display_str)) {
                    foreach (var entry in array) {
                        if (entry == null) {
                            ImGui.TextColored(var_color, "null");
                        } else {
                            ImGui.TextColored(var_color, entry.ToString());
                        }
                    }
                    ImGui.TreePop();
                }
            } else if(type.IsEnum){
                var val = (Enum)value;
                if(EditEnum(name, ref val, disabled) && !disabled){
                    value = val;
                    changed = true;
                }
            } else {
                custom_inspector = false;
            }
        }
        if(!custom_inspector) {
            float width = ImGui.GetContentRegionAvailWidth();
            float height = ImGui.GetFontSize()+ImGui.GetStyle().ItemSpacing.y;
        
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, Vector2.zero);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.zero);
            ImGui.BeginChild($"{name}_{instance_id}_label_region", new Vector2(width / 3f, height), false, ImGuiWindowFlags.NoScrollWithMouse);
            ImGui.Text($"{name}:");
            ImGui.EndChild();
            ImGui.SameLine();
            ImGui.TextColored(var_color, $"{(value!=null?value.ToString():"null")}");
            ImGui.PopStyleVar(2);
        }
        return changed;
    }

    void DrawMemberInfo(MemberInfo info, UnityEngine.Object obj, Type t){
        // Skip boilerplate inherited members that nobody cares about
        if (info.DeclaringType == typeof(Component) || 
            info.DeclaringType == typeof(UnityEngine.Object) || 
            info.DeclaringType == typeof(UnityEngine.Behaviour) || 
            info.DeclaringType == typeof(UnityEngine.MonoBehaviour)) 
        {
            return;
        }
        bool deprecated = info.GetCustomAttributes(typeof(System.ObsoleteAttribute), true).Length > 0;
        if (deprecated) {
            return;
        }

        object value = null;
        try {
            value = GetValue(info, obj, t);
        } catch (Exception exc){
            Debug.LogError($"Some kind of problem! {exc}");
            return;
        }
        bool disabled = false;
        if(t == typeof(PropertyInfo)){
            disabled = !((PropertyInfo)info).CanWrite;
        }
        bool changed = DrawValueInspector(info.Name, disabled, ref value, obj.GetInstanceID());
        if(changed){
            SetValue(info, obj, t, value);
        }
    }

    void DisplayFieldsAndProperties(UnityEngine.Object obj) {
        Type t = obj.GetType();
        FieldInfo[] fieldInfo = t.GetFields();
        foreach (FieldInfo info in fieldInfo) {
            DrawMemberInfo(info, obj, typeof(FieldInfo));
        }
        PropertyInfo[] propertyInfo = t.GetProperties();
        foreach (PropertyInfo info in propertyInfo) {
            DrawMemberInfo(info, obj, typeof(PropertyInfo));
        }
    }

    static char[] vecfields = new char[] {'X', 'Y', 'Z', 'W'};
    static Vector4 var_color = new Vector4(255,204,153,255)/255f;
    delegate bool EditField(string name, ref object obj, bool disabled);
    Dictionary<Type, EditField> EditFieldTypes = new Dictionary<Type, EditField> {
        {typeof(Vector2), EditVector2},
        {typeof(Vector3), EditVector3},
        {typeof(Vector4), EditVector4},
        {typeof(Quaternion), EditQuaternion},
        {typeof(Color), EditColor},
        {typeof(Matrix4x4), EditMatrix4x4},
        {typeof(bool), EditBool},
        {typeof(int), EditInt},
        {typeof(float), EditFloat},
        //{typeof(Enum), EditEnum},
        {typeof(string), EditString},
    };

    static bool EditVector2(string name, ref object obj, bool disabled){
        Vector2 vec = (Vector2)obj;
        bool changed = false;
        float width = ImGui.GetContentRegionAvailWidth();
        float height = ImGui.GetFontSize()+ImGui.GetStyle().ItemSpacing.y;
        
        for(int i=0; i<2; ++i){
            ImGui.SameLine();
            ImGui.BeginChild($"{name}_{vecfields[i]}_region", new Vector2(width * 2f/3f/vecfields.Length, height), false);
            float val = vec[i];
            if (!disabled) {
                changed = ImGui.DragFloat($"{vecfields[i]}###{name}_{vecfields[i]}", ref val, .1f, 0f, 0f, "%g") || changed;
            } else {
                ImGui.TextColored(var_color, val.ToString());
            }
            vec[i] = val;
            ImGui.EndChild();
        }
        obj = vec;
        return changed;
    }
    
    static bool EditVector3(string name, ref object obj, bool disabled) {
        var vec = (Vector3)obj;
        bool changed = false;
        float width = ImGui.GetContentRegionAvailWidth();
        float height = ImGui.GetFontSize()+ImGui.GetStyle().ItemSpacing.y;

        for(int i=0; i<3; ++i){
            ImGui.SameLine();
            ImGui.BeginChild($"{name}_{vecfields[i]}_region", new Vector2(width * 2f/3f/vecfields.Length, height), false, ImGuiWindowFlags.NoScrollWithMouse);
            float val = vec[i];
            if(!disabled){
                changed = ImGui.DragFloat($"{vecfields[i]}###{name}_{vecfields[i]}", ref val, .1f, 0f, 0f, "%g") || changed;
            } else {
                ImGui.TextColored(var_color, val.ToString());
            }
            vec[i] = val;
            ImGui.EndChild();
        }
        obj = vec;
        return changed;
    }
    
    static bool EditQuaternion(string name, ref object obj, bool disabled) {
        var vec = (Quaternion)obj;
        bool changed = false;
        float width = ImGui.GetContentRegionAvailWidth();
        float height = ImGui.GetFontSize()+ImGui.GetStyle().ItemSpacing.y;

        for(int i=0; i<vecfields.Length; ++i){
            ImGui.SameLine();
            ImGui.BeginChild($"{name}_{vecfields[i]}_region", new Vector2(width * 2f/3f/vecfields.Length, height), false, ImGuiWindowFlags.NoScrollWithMouse);
            float val = vec[i];
            if(!disabled){
                changed = ImGui.DragFloat($"{vecfields[i]}###{name}_{vecfields[i]}", ref val, .1f, 0f, 0f, "%g") || changed;
            } else {
                ImGui.TextColored(var_color, val.ToString());
            }
            vec[i] = val;
            ImGui.EndChild();
        }
        obj = vec;
        return changed;
    }

    static bool EditVector4(string name, ref object obj, bool disabled){
        var vec = (Vector4)obj;
        bool changed = false;
        float width = ImGui.GetContentRegionAvailWidth();
        float height = ImGui.GetFontSize()+ImGui.GetStyle().ItemSpacing.y;

        for(int i=0; i<vecfields.Length; ++i){
            ImGui.SameLine();
            ImGui.BeginChild($"{name}_{vecfields[i]}_region", new Vector2(width * 2f/3f/vecfields.Length, height), false, ImGuiWindowFlags.NoScrollWithMouse);
            float val = vec[i];
            if (!disabled) {
                changed = ImGui.DragFloat($"{vecfields[i]}###{name}_{vecfields[i]}", ref val, .1f, 0f, 0f, "%g") || changed;
            } else {
                ImGui.TextColored(var_color, val.ToString());
            }
            vec[i] = val;
            ImGui.EndChild();
        }
        obj = vec;
        return changed;
    }
    
    static bool EditString(string name, ref object obj, bool disabled) {
        var vec = (string)obj;
        bool changed = false;
        float width = ImGui.GetContentRegionAvailWidth();
        float height = ImGui.GetFontSize()+ImGui.GetStyle().ItemSpacing.y;
        
        ImGui.SameLine();
        var buf = vec.PadRight(512, '\0').ToCharArray();
        if(!disabled){
            changed = ImGui.InputText($"###{name}_text", buf);
            if(changed){
                vec = new string(buf);
            }
        } else {
            ImGui.TextColored(var_color, vec.ToString());
        }
        obj = vec;
        return changed;
    }

    static bool EditBool(string name, ref object obj, bool disabled) {
        var vec = (bool)obj;
        bool changed = false;
        float width = ImGui.GetContentRegionAvailWidth();
        float height = ImGui.GetFontSize()+ImGui.GetStyle().ItemSpacing.y;
        
        ImGui.SameLine();
        if(!disabled){
            changed = ImGui.Checkbox($"###{name}_checkbox", ref vec);
        } else {
            ImGui.TextColored(var_color, vec.ToString());
        }
        obj = vec;
        return changed;
    }

    static bool EditColor(string name, ref object obj, bool disabled){
        var vec = (Color)obj;
        bool changed = false;
        float width = ImGui.GetContentRegionAvailWidth();
        float height = ImGui.GetFontSize()+ImGui.GetStyle().ItemSpacing.y;
        
        ImGui.SameLine();
        Vector4 col = (Vector4)vec;
        if (!disabled) {
            changed = ImGui.ColorEdit4($"###{name}_color", ref col, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoLabel | ImGuiColorEditFlags.HDR);
        } else {
            ImGui.ColorEdit4($"###{name}_color", ref col, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoLabel | ImGuiColorEditFlags.HDR | ImGuiColorEditFlags.NoPicker);
        }
        obj = vec;

        vec = (Color)col;
        return changed;
    }

    static bool EditEnum<T>(string name, ref T vec, bool disabled) {
        //var vec = (T)obj;
        bool changed = false;
        float width = ImGui.GetContentRegionAvailWidth();
        float height = ImGui.GetFontSize()+ImGui.GetStyle().ItemSpacing.y;
        
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, Vector2.zero);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.zero);
        ImGui.BeginChild($"{name}_label_region", new Vector2(width / 3f, height), false, ImGuiWindowFlags.NoScrollWithMouse);
        ImGui.Text($"{name}:");
        ImGui.EndChild();
       
        string curr_str = vec.ToString();
        var names = Enum.GetNames(vec.GetType());
        int curr = 0;
        for(int i=0; i<names.Length; ++i){
            if(names[i] == curr_str){
                curr = i;
            }
        }
        
        ImGui.SameLine();
        ImGui.BeginChild($"{name}_edit_region", new Vector2(width, height), false, ImGuiWindowFlags.NoScrollWithMouse);
        if(!disabled){
            changed = ImGui.Combo($"###{name}_enum", ref curr, names);
        } else {
            ImGui.TextColored(var_color, names[curr]);
        }
        ImGui.EndChild();

        var values = Enum.GetValues(vec.GetType());
        int index = 0;
        foreach(var value in values){
            if(index == curr){
                vec = (T)value;
            }
            ++index;
        }

        ImGui.PopStyleVar(2);
        return changed;
    }

    static bool EditFloat(string name, ref object obj, bool disabled){
        var vec = (float)obj;
        bool changed = false;
        float width = ImGui.GetContentRegionAvailWidth();
        float height = ImGui.GetFontSize()+ImGui.GetStyle().ItemSpacing.y;
        
        ImGui.SameLine();
        if(!disabled){
            changed = ImGui.DragFloat($"###{name}_dragfloat", ref vec, 0.1f, 0f, 0f, "%g");
         } else {
            ImGui.TextColored(var_color, vec.ToString());
        }
        obj = vec;
        return changed;
    }
    
    static bool EditInt(string name, ref object obj, bool disabled){
        var vec = (int)obj;
        bool changed = false;
        float width = ImGui.GetContentRegionAvailWidth();
        float height = ImGui.GetFontSize()+ImGui.GetStyle().ItemSpacing.y;
        
        ImGui.SameLine();
        if(!disabled){
            changed = ImGui.DragInt($"###{name}_dragint", ref vec);
        } else {
            ImGui.TextColored(var_color, vec.ToString());
        }
        obj = vec;
        return changed;
    }


    static bool EditMatrix4x4(string name, ref object obj, bool disabled){
        var vec = (Matrix4x4)obj;
        bool changed = false;
        float width = ImGui.GetContentRegionAvailWidth();
        float height = ImGui.GetFontSize()+ImGui.GetStyle().ItemSpacing.y;

        for(int i=0; i<16; ++i){
            if(i%4!=0){
                ImGui.SameLine();
            }
            ImGui.BeginChild($"{name}_{i}_region", new Vector2(width / 4f, height), false, ImGuiWindowFlags.NoScrollWithMouse);
            float val = vec[i];
            if(disabled){
                string text = val.ToString();
                var size = ImGui.CalcTextSize(text);
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + ImGui.GetContentRegionAvail().x*0.5f-size.x*0.5f);
                ImGui.TextColored(var_color, val.ToString());
            } else {
                changed = ImGui.DragFloat($"###{name}_{i}", ref val, .1f, 0f, 0f, "%g") || changed;
            }
            vec[i] = val;
            ImGui.EndChild();
        }
        obj = vec;
        return changed;
    }

    void DrawTypeHierarchy(Type base_type, List<UnityEngine.Object> objects) {
        if (ImGui.TreeNode($"{base_type} [{objects.Count}] ### {base_type} List")) {
            var types = new Dictionary<Type, List<UnityEngine.Object>>();
            foreach (var obj in objects) {
                var type = obj.GetType();
                if(type == base_type){
                    if(ImGui.TreeNode($"{obj.name}###type_hier_leaf_{obj.GetInstanceID()}")){
                        DisplayFieldsAndProperties(obj);
                        ImGui.TreePop();
                    }
                } else {
                    while (type.BaseType != null && type.BaseType != base_type) {
                        type = type.BaseType;
                    }
                    if(type.BaseType == null){
                        continue;
                    }
                    if (!types.ContainsKey(type)) {
                        types[type] = new List<UnityEngine.Object>();
                    }
                    types[type].Add(obj);
                }
            }
            foreach (var entry in types) {
                DrawTypeHierarchy(entry.Key, entry.Value);
            }
            ImGui.TreePop();
        }
    }

    void DrawHierarchyInspector() {
        // Top bar
        ImGui.Checkbox("Show Hidden", ref show_hidden);
        ImGui.Separator();
        // Hierarchy
        ImGui.Columns(2);
        // Get all transforms
        var transforms = Resources.FindObjectsOfTypeAll<Transform>();
        // Categorize transforms by which scene they belong to, create new "Editor scene" for non-scene transforms
        var default_scene = "Editor###editor_scene";
        var roots = new Dictionary<string, List<Transform>>(); // Contain list of transforms for each scene
        foreach (var entry in transforms) {
            if (entry.parent == null) { // Only look at root transforms
                var scene = entry.gameObject.scene;
                var name = default_scene;
                if (scene != null) {
                    name = scene.name;
                    if (name == null) {
                        name = default_scene;
                    }
                }
                // Add root transforms to scene dictionary
                if (!roots.ContainsKey(name)) {
                    roots[name] = new List<Transform>();
                }
                roots[name].Add(entry);
            }
        }
        // Draw the hierarchy for each scene and root transform
        foreach (var entry in roots) {
            if ((show_hidden || entry.Key != default_scene) && ImGui.TreeNode(entry.Key)) {
                foreach (var transform in entry.Value) {
                    DrawHierarchy(transform);
                }
                ImGui.TreePop();
            }
        }
        // Inspector
        ImGui.NextColumn();
        if (selected) { // Only inspect if an object is selected
            float width = ImGui.GetContentRegionAvailWidth();
            bool is_active = selected.activeInHierarchy;
            if (ImGui.Checkbox("###selected_active", ref is_active)) {
                selected.SetActive(is_active);
            }
            ImGui.SameLine();
            ImGui.PushItemWidth(width - 100f); // Leave room for static checkbox
            if (ImGui.InputText("###rename", rename_obj)) {
                selected.name = new string(rename_obj);
            }
            ImGui.PopItemWidth();
            ImGui.SameLine();
            bool is_static = selected.isStatic;
            if (ImGui.Checkbox("Static", ref is_static)) {
                selected.isStatic = is_static;
            }
            float cursor_x = ImGui.GetCursorPosX();
            ImGui.Text($"Tag: ");
            ImGui.SameLine();
            ImGui.TextColored(var_color, selected.tag);
            ImGui.SameLine();
            ImGui.SetCursorPosX(cursor_x + width * 0.5f);
            ImGui.Text($"Layer: ");
            ImGui.SameLine();
            ImGui.TextColored(var_color, $"{LayerMask.LayerToName(selected.layer)}({selected.layer})");
            ImGui.SetCursorPosX(cursor_x);
            ImGui.Text($"HideFlags: ");
            ImGui.SameLine();
            ImGui.TextColored(var_color, selected.hideFlags.ToString());
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0f, 0f));
            ColumnSeparator();
            var components = selected.GetComponents<Component>();
            foreach (var component in components) {
                ImGui.SetCursorPos(ImGui.GetCursorPos() + Vector2.up * ImGui.GetStyle().ItemSpacing.y);
                if (component == null) {
                    ImGui.Text("Missing script");
                    ColumnSeparator();
                    continue;
                } else {
                    Type t = component.GetType();
                    string display_name = t.ToString();
                    if (t.BaseType == typeof(MonoBehaviour))
                        display_name = $"{FontAwesome5.FileCode} {t} (Script)";
                    if (t == typeof(Camera))
                        display_name = $"{FontAwesome5.Video} Camera";
                    if (t == typeof(Transform))
                        display_name = $"{FontAwesome5.ArrowsAlt} Transform";
                    if (t == typeof(Light))
                        display_name = $"{FontAwesome5.Lightbulb} Light";
                    if (t == typeof(BoxCollider))
                        display_name = $"{FontAwesome5.Square} Box Collider";
                    if (t == typeof(MeshFilter))
                        display_name = $"{FontAwesome5.DrawPolygon} Mesh Filter";
                    if (t == typeof(MeshRenderer))
                        display_name = $"{FontAwesome5.Eye} Mesh Renderer";
                    if (t == typeof(AudioListener))
                        display_name = $"{FontAwesome5.Microphone} Audio Listener";

                    bool tree_open = ImGui.TreeNodeEx($"{display_name}###{display_name}_{component.GetInstanceID()}", ImGuiTreeNodeFlags.AllowItemOverlap);
                    if (t.IsSubclassOf(typeof(Behaviour))) {
                        ImGui.SameLine();
                        bool enabled = ((Behaviour)component).enabled;
                        if (ImGui.Checkbox($"###enabled{component.GetInstanceID()}", ref enabled)) {
                            ((Behaviour)component).enabled = enabled;
                        }
                    } else if (t.IsSubclassOf(typeof(Collider))) {
                        ImGui.SameLine();
                        bool enabled = ((Collider)component).enabled;
                        if (ImGui.Checkbox($"###enabled{component.GetInstanceID()}", ref enabled)) {
                            ((Collider)component).enabled = enabled;
                        }
                    } else if (t.IsSubclassOf(typeof(Renderer))) {
                        ImGui.SameLine();
                        bool enabled = ((Renderer)component).enabled;
                        if (ImGui.Checkbox($"###enabled{component.GetInstanceID()}", ref enabled)) {
                            ((Renderer)component).enabled = enabled;
                        }
                    }
                    if (tree_open) {
                        if (t == typeof(Transform)) {
                            Transform transform = (Transform)component;
                            object pos = transform.position;
                            if(DrawValueInspector("Position", false, ref pos, GetInstanceID())){
                                transform.position = (Vector3)pos;
                            }
                            object rot = transform.localEulerAngles;
                            if(DrawValueInspector("Rotation", false, ref rot, GetInstanceID())){
                                transform.position = (Vector3)rot;
                            }
                            object scale = transform.localScale;
                            if(DrawValueInspector("Scale", false, ref scale, GetInstanceID())){
                                transform.localScale = (Vector3)scale;
                            }
                            if (ImGui.TreeNode($"Advanced###advanced_{component.GetInstanceID()}")) {
                                DisplayFieldsAndProperties(component);
                                ImGui.TreePop();
                            }
                        } else {
                            DisplayFieldsAndProperties(component);
                        }
                        ImGui.TreePop();
                    }
                }
                ColumnSeparator();
            }
            ImGui.PopStyleVar();
        }
        ImGui.Columns(1);
    }

    void SpecialFolder(Environment.SpecialFolder folder, string display) {    
        try {
            string filePath = Environment.GetFolderPath(folder);
            if (filePath.Length > 0 && Directory.Exists(filePath)) {
                ImGui.Text(display);
                if (ImGui.IsItemClicked()) {
                    file_browse_path = filePath + Path.DirectorySeparatorChar;
                }
            }
        } catch (Exception) {}
    }
    
    void DrawFileBrowser() {
        if (ImGui.Begin("File Browser")) {
            {
                ImGui.Button(FontAwesome5.ArrowLeft);
                ImGui.SameLine();
                ImGui.Button(FontAwesome5.ArrowRight);
                ImGui.SameLine();
                if(ImGui.Button(FontAwesome5.ArrowUp)) {
                    // Split path
                    var path_split = file_browse_path.Split(Path.DirectorySeparatorChar);
                    // Remove "" element at the end if necessary
                    if(path_split.Length > 0 && path_split[path_split.Length-1] == ""){
                        var new_path_split = new string[path_split.Length-1];
                        for (int i = 0; i < new_path_split.Length - 1; ++i) {
                            new_path_split[i] = path_split[i];
                        }
                        path_split = new_path_split;
                    }
                    // Remove last dir in path
                    if (path_split.Length > 1) {
                        var builder = new StringBuilder();
                        for(int i=0; i<path_split.Length-1; ++i){
                            builder.Append(path_split[i]);
                            builder.Append(Path.DirectorySeparatorChar);
                        }
                        file_browse_path = builder.ToString();
                    }
                }
                ImGui.SameLine();
                var split = file_browse_path.Split(Path.DirectorySeparatorChar);
                var path = "";
                ImGui.Dummy(Vector2.zero);
                foreach (var name in split) {
                    if (name == "") {
                        continue;
                    }
                    path += name;
                    ImGui.SameLine();
                    ImGui.Text(name);
                    if (ImGui.IsItemClicked()) {
                        file_browse_path = path + Path.DirectorySeparatorChar;
                    }
                    path += Path.DirectorySeparatorChar;
                    ImGui.SameLine();
                    ImGui.TextWrapped(">");
                }
            }


            ImGui.Separator();
            // left
            ImGui.BeginChild("left pane", new Vector2(150, -ImGui.GetFrameHeightWithSpacing()*2-10), true);
            ImGui.Text($"{FontAwesome5.Folder} Application.dataPath");
            if (ImGui.IsItemClicked()) {
                file_browse_path = Application.dataPath.Replace('/', Path.DirectorySeparatorChar);
            }
            ImGui.Text($"{FontAwesome5.Folder} Persistent Data");
            if (ImGui.IsItemClicked()) {
                file_browse_path = Application.persistentDataPath.Replace('/', Path.DirectorySeparatorChar);;
            }
            SpecialFolder(Environment.SpecialFolder.Desktop, $"{FontAwesome5.Desktop} Desktop");
            SpecialFolder(Environment.SpecialFolder.MyDocuments, $"{FontAwesome5.File} Documents");
            SpecialFolder(Environment.SpecialFolder.MyMusic, $"{FontAwesome5.Music} Music");
            SpecialFolder(Environment.SpecialFolder.MyPictures, $"{FontAwesome5.Image} Pictures");
            SpecialFolder(Environment.SpecialFolder.MyVideos, $"{FontAwesome5.Video} Videos");
            try  {
                string[] drives = System.IO.Directory.GetLogicalDrives();
                foreach (string str in drives) {
                    if(Directory.Exists(str)){
                        ImGui.Text($"{FontAwesome5.Hdd} {str}");
                        if (ImGui.IsItemClicked()) {
                            file_browse_path = str;
                        }
                    }
                }
            } catch(Exception) {}
            ImGui.EndChild();
            ImGui.SameLine();
            ImGui.BeginChild("right pane", new Vector2(0, -ImGui.GetFrameHeightWithSpacing()*2-10), true);
            var dirs = Directory.EnumerateDirectories(file_browse_path);
            foreach (var dir in dirs) {
                var dir_name = dir.Split(Path.DirectorySeparatorChar).Last();
                ImGui.Text($"{FontAwesome5.Folder} {dir_name}");
                if (ImGui.IsItemClicked()) {
                    file_browse_path = dir + Path.DirectorySeparatorChar;
                }
            }
            var files = Directory.EnumerateFiles(file_browse_path);
            foreach (var file in files) {  
                var filename = file.Split(Path.DirectorySeparatorChar).Last();
                ImGui.TextWrapped(filename);
                if (ImGui.IsItemClicked()) {
                    file_path_buf = filename.PadRight(kFilePathBufSize, '\0').ToCharArray();
                }
            }
            ImGui.EndChild();
            ImGui.Separator();
            ImGui.Text("File name:");
            ImGui.SameLine();
            ImGui.InputText("###filename", file_path_buf);
            ImGui.SameLine();
            int curr = 0;
            string[] extensions = { "All Files (*.*)" };
            ImGui.Combo("###extension", ref curr, extensions, 1);
            ImGui.Button("Open");
            ImGui.SameLine();
            ImGui.Button("Cancel");
        }
        ImGui.End();
    }


    void Update() {
        sampler.Begin();
        for(int i=0; i<max_frame_times-1; ++i){
            frame_times[i] = frame_times[i+1];
        }
        frame_times[max_frame_times-1] = sw.ElapsedMilliseconds;
        sw.Restart();
        DrawFileBrowser();

        if(ImGui.Begin("Unity Examples")){
            if (ImGui.CollapsingHeader($"{FontAwesome5.Sitemap} Hierarchy | Inspector")){
                DrawHierarchyInspector();
            }
            if (ImGui.CollapsingHeader($"{FontAwesome5.Bars} Log")) {
                if(ImGui.Button("Standard")){
                    Debug.Log("Logging a standard message!");
                }
                ImGui.SameLine();
                if(ImGui.Button("Warning")){
                    Debug.LogWarning("Logging a warning message!");
                }
                ImGui.SameLine();
                if(ImGui.Button("Error")){
                    Debug.LogError("Logging an error message!");
                }
                ImGui.SameLine();
                if(ImGui.Button("Assert")){
                    Debug.Assert(false);
                }
                ImGui.SameLine();
                if(ImGui.Button("Exception")){
                    Debug.LogException(new Exception("test"));
                }
                ImGui.BeginChild("Child1", new Vector2(ImGui.GetWindowContentRegionWidth(), 300), true);
                int index=0;
                foreach(var entry in log_entries){
                    var pos = ImGui.GetCursorPos();
                    var color = Vector4.one;
                    string icon = "";
                    switch(entry.type){
                        case LogType.Log: icon = FontAwesome5.CommentDots; break;
                        case LogType.Warning: icon = FontAwesome5.ExclamationTriangle; color=new Vector4(1,1,0,1); break;
                        case LogType.Assert:
                        case LogType.Exception:
                        case LogType.Error: icon = FontAwesome5.ExclamationCircle; color=new Vector4(1,0,0,1); break;
                    }
                    ImGui.TextColored(color, $"   {icon}");
                    ImGui.SetCursorPos(pos);
                    if(ImGui.TreeNode(string.Format("  [{2}] {0}##{1}", entry.condition, index, entry.time.ToString("HH:mm:ss"), icon))){
                        ImGui.Indent();
                        ImGui.TextWrapped(entry.stack_trace);
                        ImGui.Unindent();
                        ImGui.TreePop();
                    }
                    ++index;
                }
                ImGui.EndChild();
            }
            /*if (ImGui.CollapsingHeader("Stats (only available in editor)")) {
                UnityEditor.UnityStats.
            }*/
            if (ImGui.CollapsingHeader($"{FontAwesome5.Clock} Profiler")){
                if(self_recorder.isValid){
                    ImGui.Text(string.Format("Time to execute all code needed for Unity Examples window: {0:0.00} ms", self_recorder.elapsedNanoseconds / 1000000f));
                } else {
                    ImGui.Text("Live sampler/recorder only works in development builds");
                }
                Profiler.logFile = string.Format("{0}/profile_data", Application.persistentDataPath);
                ImGui.TextWrapped(string.Format("Log file: {0}", Profiler.logFile));
                if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer) {
                    if(ImGui.Button("Show file")){
                        ShowExplorer(Profiler.logFile);
                    }
                }
                ImGui.TextWrapped("You can load the .raw file into the Unity profiler to see sample timings");
                FrameTimingManager.CaptureFrameTimings();
                ImGui.Text(string.Format("CPU Timer Freq: {0}", FrameTimingManager.GetCpuTimerFrequency()));
                ImGui.Text(string.Format("GPU Timer Freq: {0}", FrameTimingManager.GetGpuTimerFrequency()));
                ImGui.Text(string.Format("VSync: {0} hz", FrameTimingManager.GetVSyncsPerSecond()));
                var timings = new FrameTiming[1];
                uint num_timings = FrameTimingManager.GetLatestTimings(1, timings);
                if(num_timings > 0){
                    ImGui.Text(string.Format("cpuFrameTime: {0}", timings[0].cpuFrameTime));
                    ImGui.Text(string.Format("cpuTimeFrameComplete: {0}", timings[0].cpuTimeFrameComplete));
                    ImGui.Text(string.Format("cpuTimePresentCalled: {0}", timings[0].cpuTimePresentCalled));
                    ImGui.Text(string.Format("gpuFrameTime: {0}", timings[0].gpuFrameTime));
                    ImGui.Text(string.Format("syncInterval: {0}", timings[0].syncInterval));
                }
                ImGui.Text(string.Format("Profiler enabled: {0}", Profiler.enabled));
                ImGui.Text(string.Format("Graphics Driver: {0:0,0} bytes", Profiler.GetAllocatedMemoryForGraphicsDriver()));
                ImGui.Text(string.Format("Mono heap: {0:0,0} bytes", Profiler.GetMonoHeapSizeLong()));
                ImGui.Text(string.Format("Mono used: {0:0,0} bytes", Profiler.GetMonoUsedSizeLong()));
                ImGui.Text(string.Format("Temp allocator size: {0:0,0} bytes", Profiler.GetTempAllocatorSize()));
                ImGui.Text(string.Format("Total allocated: {0:0,0} bytes", Profiler.GetTotalAllocatedMemoryLong()));
                ImGui.Text(string.Format("Total reserved: {0:0,0} bytes", Profiler.GetTotalReservedMemoryLong()));
                ImGui.Text(string.Format("Total unused reserved: {0:0,0} bytes", Profiler.GetTotalUnusedReservedMemoryLong()));
                ImGui.Text("Frame times:");
                ImGui.PlotLines("##ft", frame_times);

                if(!Profiler.enabled){
                    if(ImGui.Button("Start profiling")){
                        Profiler.enableBinaryLog = true;
                        Profiler.enabled = true;      
                    }
                } else {
                    if(ImGui.Button("Stop profiling")){
                        Profiler.enableBinaryLog = false;
                        Profiler.enabled = false;      
                    }
                }
            }
            if (ImGui.CollapsingHeader($"{FontAwesome5.ClipboardList} Objects")){
                var all_objects = new List<UnityEngine.Object>(Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object)));
                DrawTypeHierarchy(typeof(UnityEngine.Object), all_objects);                
            }
            if (ImGui.CollapsingHeader($"{FontAwesome5.Cog} Settings")){
                    string[] resolutions = new string[Screen.resolutions.Length+1];
                    resolutions[0] = "Custom";
                    int curr_preset = 0;
                    int index = 1;
                    foreach(var res in Screen.resolutions){
                        resolutions[index] = string.Format("{0} x {1}", res.width, res.height);
                        if(res.width == Screen.width && res.height == Screen.height){
                            curr_preset = index;
                        }
                        ++index;
                    }
                    ImGui.Text("Resolution:");
                    if(ImGui.Combo("##Resolution Presets", ref curr_preset, resolutions, resolutions.Length)) {
                        if(curr_preset != 0){
                            var res = Screen.resolutions[curr_preset - 1];
                            Screen.SetResolution(res.width, res.height, Screen.fullScreen);
                        }
                    }
                    bool fullscreen = Screen.fullScreen;
                    if (ImGui.Checkbox("Fullscreen", ref fullscreen)) {
                        Screen.SetResolution(Screen.width, Screen.height, fullscreen);
                    }            
                    ImGui.Text("Quality level:");    
                    string[] quality_levels = QualitySettings.names;
                    int curr_quality = QualitySettings.GetQualityLevel();
                    if(ImGui.Combo("##Quality Settings", ref curr_quality, quality_levels, quality_levels.Length)) {
                        QualitySettings.SetQualityLevel(curr_quality, true);
                        var imgui_unity = FindObjectOfType<ImGuiUnity>();
                        if(imgui_unity != null){
                            imgui_unity.RecreateFontTexture();
                        }
                    }
                    var joysticks = Input.GetJoystickNames();
                    ImGui.Text(string.Format("Joysticks: {0}", joysticks.Length));
                    ImGui.Indent();
                    foreach(var joystick in joysticks){                    
                        ImGui.Text(joystick);
                    }
                    ImGui.Unindent();
            }
            if (ImGui.CollapsingHeader($"{FontAwesome5.BoxOpen} Diagnostics")){
                if (ImGui.TreeNode("Application")){
                    ImGui.TextWrapped(string.Format("productName: {0}",Application.productName));
                    ImGui.TextWrapped(string.Format("version: {0}",Application.version));
                    ImGui.TextWrapped(string.Format("companyName: {0}",Application.companyName));
                    ImGui.TextWrapped(string.Format("platform: {0}",Application.platform));
                    ImGui.TextWrapped(string.Format("dataPath: {0}",Application.dataPath));
                    ImGui.TextWrapped(string.Format("persistentDataPath: {0}",Application.persistentDataPath));
                    ImGui.TextWrapped(string.Format("temporaryCachePath: {0}",Application.temporaryCachePath));
                    ImGui.TextWrapped(string.Format("unityVersion: {0}",Application.unityVersion));
                    ImGui.TextWrapped(string.Format("systemLanguage: {0}",Application.systemLanguage));
                    ImGui.TextWrapped(string.Format("targetFrameRate: {0}",Application.targetFrameRate));
                    ImGui.TextWrapped(string.Format("internetReachability: {0}",Application.internetReachability));
                    ImGui.TextWrapped(string.Format("isEditor: {0}",Application.isEditor));
                    ImGui.TextWrapped(string.Format("isFocused: {0}",Application.isFocused));
                    ImGui.TextWrapped(string.Format("runInBackground: {0}",Application.runInBackground));
                    ImGui.TextWrapped(string.Format("sandboxType: {0}",Application.sandboxType));
                    ImGui.TreePop();
                }
                if (ImGui.TreeNode("Graphics")){
                    ImGui.TextWrapped(string.Format("activeTier: {0} out of 3", Graphics.activeTier));
                    ImGui.TextWrapped(string.Format("activeColorGamut: {0}", Graphics.activeColorGamut));
                    ImGui.TextWrapped(string.Format("dpi: {0}", Screen.dpi));
                    ImGui.TextWrapped(string.Format("currentResolution: {0}", Screen.currentResolution));
                    ImGui.TextWrapped(string.Format("orientation: {0}", Screen.orientation));
                    ImGui.TextWrapped(string.Format("safeArea: {0}", Screen.safeArea));
                    ImGui.TextWrapped(string.Format("fullScreenMode: {0}", Screen.fullScreenMode));
                    var displays = Display.displays;
                    if(ImGui.TreeNode(string.Format("displays: {0}", Display.displays.Length))){
                        int index=0;
                        foreach(var display in displays){
                            ImGui.TextWrapped(string.Format("Display {0}:", index));     
                            ImGui.Indent();
                            ImGui.TextWrapped(string.Format("active: {0}", display.active));     
                            ImGui.TextWrapped(string.Format("renderingWidth: {0}", display.renderingWidth));      
                            ImGui.TextWrapped(string.Format("renderingHeight: {0}", display.renderingHeight));   
                            ImGui.TextWrapped(string.Format("systemWidth: {0}", display.systemWidth));   
                            ImGui.TextWrapped(string.Format("systemHeight: {0}", display.systemHeight));   
                            ImGui.Unindent();
                            ++index;
                        }
                        ImGui.TreePop();
                    }
                    ImGui.TreePop();
                }
                if (ImGui.TreeNode("SystemInfo")){
                    ImGui.TextWrapped(string.Format("deviceModel: {0}", SystemInfo.deviceModel));
                    ImGui.TextWrapped(string.Format("deviceName: {0}", SystemInfo.deviceName));
                    ImGui.TextWrapped(string.Format("deviceType: {0}", SystemInfo.deviceType));
                    ImGui.TextWrapped(string.Format("graphicsDeviceName: {0}", SystemInfo.graphicsDeviceName));
                    ImGui.TextWrapped(string.Format("graphicsDeviceType: {0}", SystemInfo.graphicsDeviceType));
                    ImGui.TextWrapped(string.Format("graphicsDeviceVendor: {0}", SystemInfo.graphicsDeviceVendor));
                    ImGui.TextWrapped(string.Format("graphicsDeviceVersion: {0}", SystemInfo.graphicsDeviceVersion));
                    ImGui.TextWrapped(string.Format("graphicsMemorySize: {0}", SystemInfo.graphicsMemorySize));
                    ImGui.TextWrapped(string.Format("operatingSystem: {0}", SystemInfo.operatingSystem));
                    ImGui.TextWrapped(string.Format("processorType: {0}", SystemInfo.processorType));
                    ImGui.TextWrapped(string.Format("processorCount: {0}", SystemInfo.processorCount));
                    ImGui.TextWrapped(string.Format("processorFrequency: {0}", SystemInfo.processorFrequency));
                    ImGui.TextWrapped(string.Format("systemMemorySize: {0}", SystemInfo.systemMemorySize));
                    ImGui.TreePop();
                }
            }
        }
        ImGui.End();
        sampler.End();
    }
}
