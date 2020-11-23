using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeybindElement : MonoBehaviour {
    [HideInInspector] public InputAction input_action;
    public RebindDialogScript rebind_dialog_script;
    public Text label;
    public Text current_binding;

    public void OnClick () {
        rebind_dialog_script.Rebind(input_action);
    }

    public void OnEnable() {
        label.text = input_action.name;
        current_binding.text = input_action.GetBindingDisplayString();
    }
}
