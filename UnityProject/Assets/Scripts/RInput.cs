﻿using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class RInput : MonoBehaviour {
    public static MovementInputs inputs_player;
    public static GunInputs inputs_gun;

    private static HashSet<int> actions_started = new HashSet<int>();
    private static HashSet<int> actions_canceled = new HashSet<int>();

    private void AddStartedAction(InputAction action) {
        actions_started.Add(action.GetHashCode());
    }

    private void AddCanceledAction(InputAction action) {
        actions_started.Add(action.GetHashCode());
    }

    [RuntimeInitializeOnLoadMethod]
    private void Init() {
        inputs_player = new MovementInputs();
        inputs_gun = new GunInputs();

        SetupInputActionMap(inputs_gun.main); // TODO can we do this in a loop somehow?
        SetupInputActionMap(inputs_player.main);
        SetupInputActionMap(inputs_player.Magazine);
        SetupInputActionMap(inputs_player.Inventory);
    }

    private void SetupInputActionMap(InputActionMap map) {
        foreach (InputAction item in map) {
            item.started += ctx => AddStartedAction(ctx.action);
            item.canceled += ctx => AddCanceledAction(ctx.action);
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
