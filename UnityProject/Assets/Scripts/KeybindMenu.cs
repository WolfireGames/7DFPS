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
        AddKeybindElements(RInput.player);
        AddKeybindElements(RInput.gun);
    }

    public void AddKeybindElements(IEnumerable<InputAction> actions) {
        foreach (InputAction action in actions) {
            foreach (var item in action.bindings) {
                if(IsRebindableAction(action, item)) {
                    AddKeybindElement(action, item);
                }
            }
        }
    }

    private bool IsRebindableAction(InputAction action, InputBinding binding) {
        return binding.isComposite || binding.isPartOfComposite || action.expectedControlType == "Button";
    }

    public void ResetKeybinds() {
        RInput.ResetKeybinds();

        // Refresh window
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    private void AddKeybindElement(InputAction action, InputBinding binding) {
        if(last_label != action.actionMap.name) {
            var text = Instantiate(label_template, container);
            text.text = action.actionMap.name;
            text.gameObject.SetActive(true);

            last_label = action.actionMap.name;
        }

        KeybindElement element = Instantiate(template, container);
        element.input_action = action;
        element.binding = binding;
        element.rebind_dialog_script = rebind_dialog_script;
        element.gameObject.SetActive(true);
    }
}
