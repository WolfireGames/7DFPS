using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeybindElement : MonoBehaviour {
    [HideInInspector] public InputAction input_action;
    public Text label;
    public Text current_binding;
    public InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    public void OnClick () {
        input_action.Disable();
        rebindingOperation = input_action.PerformInteractiveRebinding()
            .WithCancelingThrough("<Keyboard>/escape")
            .OnApplyBinding( (x, y) => { OnApplyRebinding(x, y); } )
            .OnComplete( (x) => { OnComplete(x); })
            .Start();
    }

    private void OnApplyRebinding(InputActionRebindingExtensions.RebindingOperation operation, string new_binding) {
        input_action.ApplyBindingOverride(new_binding);
        SetContent(label.text, input_action.GetBindingDisplayString());
        RInput.SaveOverrides();
    }

    private void OnComplete(InputActionRebindingExtensions.RebindingOperation operation) {
        input_action.Enable();
        CleanUp();
    }

    private void OnDisable() {
        CleanUp();
    }

    private void CleanUp() {
        if(rebindingOperation != null) {
            rebindingOperation.Cancel();
            rebindingOperation.Dispose();
        }
    }

    public void SetContent(string text, string binding) {
        label.text = text;
        current_binding.text = binding;
    }
}
