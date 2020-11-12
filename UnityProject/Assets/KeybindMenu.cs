using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeybindMenu : MonoBehaviour {
    public KeybindElement template;
    public Transform container;

    private void Awake() {
        foreach (InputAction action in RInput.player) {
            AddKeybindElement(action);
        }

        foreach (InputAction action in RInput.gun) {
            AddKeybindElement(action);
        }
    }

    private void AddKeybindElement(InputAction action) {
        KeybindElement element = Instantiate(template, container);
        element.input_action = action;
        element.SetContent(action.name, action.GetBindingDisplayString());
        element.gameObject.SetActive(true);
    }
}
