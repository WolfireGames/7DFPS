using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeybindElement : MonoBehaviour {
    [HideInInspector] public InputAction input_action;
    [HideInInspector] public InputBinding binding;
    public RebindDialogScript rebind_dialog_script;
    public Text label;
    public Text current_binding;
    public Button button;

    public void OnClick () {
        rebind_dialog_script.Rebind(input_action, binding);
    }

    public void OnEnable() {
        label.text = input_action.name;
        current_binding.text = input_action.GetBindingDisplayString();

        binding = input_action.bindings[input_action.bindings.IndexOf((x) => x.name == binding.name)]; // Refresh binding incase they were changed, we assume that the name stayed the same
        if(binding.isComposite) {
            button.gameObject.SetActive(false);
            current_binding.gameObject.SetActive(false);
        } else if(binding.isPartOfComposite) {
            label.text = $" - {binding.name}";
            current_binding.text = binding.ToDisplayString();
        }
    }
}
