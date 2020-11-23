using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeybindMenu : MonoBehaviour {
    public KeybindElement template;
    public UnityEngine.UI.Text label_template;
    public RebindDialogScript rebind_dialog_script;
    public Transform container;

    private string last_label = "";

    private void Awake() {
        foreach (InputAction action in RInput.player) {
            AddKeybindElement(action);
        }

        foreach (InputAction action in RInput.gun) {
            AddKeybindElement(action);
        }
    }

    private void AddKeybindElement(InputAction action) {
        if(last_label != action.actionMap.name) {
            Instantiate(label_template, container).text = action.actionMap.name;
            last_label = action.actionMap.name;
        }

        KeybindElement element = Instantiate(template, container);
        element.input_action = action;
        element.rebind_dialog_script = rebind_dialog_script;
        element.gameObject.SetActive(true);
    }
}
