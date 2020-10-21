using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

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

    private void LateUpdate() {
        actions_started.Clear();
        actions_canceled.Clear();
    }
}
