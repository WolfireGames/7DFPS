using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeybindMenu : MonoBehaviour {
    public KeybindElement template;
    public Transform container;

    private void Awake() {
        foreach (var item in RInput.player) {
            KeybindElement element = Instantiate(template, container);
            element.input_action = item;
            element.SetContent(item.name, item.GetBindingDisplayString());
            element.gameObject.SetActive(true);
        }
    }
}
