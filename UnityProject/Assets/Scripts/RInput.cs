using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System.Linq;

public class RInput : MonoBehaviour {
    public static MovementInputs player;
    public static GunInputs gun;

    private static HashSet<int> actions_started = new HashSet<int>();
    private static HashSet<int> actions_canceled = new HashSet<int>();

    private static void AddStartedAction(InputAction action) {
        actions_started.Add(action.GetHashCode());
    }

    private static void AddCanceledAction(InputAction action) {
        actions_canceled.Add(action.GetHashCode());
    }

    [RuntimeInitializeOnLoadMethod]
    public static void Init() {
        player = new MovementInputs();
        gun = new GunInputs();
        
        SetupInputActionMap(gun.main); // TODO can we do this in a loop somehow?
        SetupInputActionMap(player.main);
        SetupInputActionMap(player.Magazine);
        SetupInputActionMap(player.Inventory);

        gun.Enable();
        player.Enable();

        LoadOverrides();
        SaveOverrides();

        // Create an RInput object to hook into Unity's update cycle
        RInput instance = new GameObject().AddComponent<RInput>();
        GameObject.DontDestroyOnLoad(instance);
        instance.gameObject.hideFlags = HideFlags.HideAndDontSave;
    }

    private static void SetupInputActionMap(InputActionMap map) {
        foreach (InputAction item in map) {
            if(item.type == InputActionType.Button) {
                item.started += ctx => RInput.AddStartedAction(ctx.action);
                item.canceled += ctx => RInput.AddCanceledAction(ctx.action);
            }
        }
    }

    public static bool GetButton(InputAction action) {
        return action.ReadValue<float>() > 0.5f;
    }

    public static bool GetButtonDown(InputAction action) {
        return actions_started.Contains(action.GetHashCode());
    }

    public static bool GetButtonUp(InputAction action) {
        return actions_canceled.Contains(action.GetHashCode());
    }

    public static float GetAxis(InputAction action) {
        return action.ReadValue<float>();
    }

    public static Vector2 GetAxis2D(InputAction action) {
        return action.ReadValue<Vector2>();
    }

    private void LateUpdate() {
        actions_started.Clear();
        actions_canceled.Clear();
    }

    public static void SaveOverrides() {
        PlayerPrefs.SetString("player_input", Save(player.asset).ToJSON());
        PlayerPrefs.SetString("gun_input", Save(gun.asset).ToJSON());
    }

    public static void LoadOverrides() {
        if(PlayerPrefs.HasKey("player_input")) {
            Load(player.asset, Overrides.FromJSON(PlayerPrefs.GetString("player_input")));
        }

        if(PlayerPrefs.HasKey("gun_input")) {
            Load(gun.asset, Overrides.FromJSON(PlayerPrefs.GetString("gun_input")));
        }
    }


    // Serialization // TODO Replace with "SaveBindingOverridesAsJson" framework in InputSystem 1.1, once it comes out... And never look back

    // Serializeable wrapper for action overrides
    private class Overrides : Dictionary<System.Guid, string> {
        private struct Serializeable {
            public string[] keys;
            public string[] values;
        }

        public static Overrides FromJSON(string json) {
            Serializeable serializeable = JsonUtility.FromJson<Serializeable>(json);
            
            if(serializeable.keys == null || serializeable.values == null) {
                return new Overrides();
            }

            if(serializeable.keys.Length != serializeable.values.Length) {
                Debug.LogError("Corrupt keybind dictionary! Key and Value count don't match! Falling back to empty!");
                return new Overrides();
            }

            Overrides overrides = new Overrides();
            for (int i = 0; i < serializeable.keys.Length; i++) {
                if(System.Guid.TryParse(serializeable.keys[i], out System.Guid guid)) {
                    overrides.Add(guid, serializeable.values[i]);
                } else {
                    Debug.LogWarning($"Failed to parse guid: {serializeable.keys[i]}, mapped to {serializeable.values[i]}. Resetting to default!");
                }
            }
            return overrides;
        }

        public string ToJSON() {
            Serializeable serializeable = new Serializeable() {
                keys = Keys.Select( (x) => x.ToString() ).ToArray(),
                values = Values.ToArray(),
            };
            return JsonUtility.ToJson(serializeable);
        }
    }

    // Modified from https://forum.unity.com/threads/saving-user-bindings.805722/#post-5384310
    private static Overrides Save(InputActionAsset asset) {
        Overrides overrides = new Overrides();
        foreach (var map in asset.actionMaps) {
            foreach (var binding in map.bindings) {
                if (!string.IsNullOrEmpty(binding.overridePath)) {
                    overrides[binding.id] = binding.overridePath;
                }
            }
        }
        return overrides;
    }

    // Modified from https://forum.unity.com/threads/saving-user-bindings.805722/#post-5384310 
    private static void Load(InputActionAsset asset, Overrides overrides) {
        foreach (var map in asset.actionMaps) {
            var bindings = map.bindings;
            for (var i = 0; i < bindings.Count; ++i) {
                if (overrides.TryGetValue(bindings[i].id, out var overridePath)) {
                    map.ApplyBindingOverride(i, new InputBinding { overridePath = overridePath });
                }
            }
        }
    }
}
