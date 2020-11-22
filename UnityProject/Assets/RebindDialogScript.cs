using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class RebindDialogScript : MonoBehaviour {
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private string new_binding;
    private InputAction input_action;
    public Text text;

    public Button cancel_button;
    public Button confirm_button;

    public void Rebind(InputAction input_action) {
        text.text = "Press the key you want to rebind to.\nCancel with \"ESC\"";
        this.input_action = input_action;
        SetButtons(false);
        SetWindow(true);

        input_action.Disable();
        rebindingOperation = input_action.PerformInteractiveRebinding()
            .WithCancelingThrough("<Keyboard>/escape")
            .OnApplyBinding( (x, y) => { OnApplyRebinding(x, y); } )
            .OnCancel( (x) => Cancel(x) )
            .Start();

    }

    public void Confirm() {
        input_action.ApplyBindingOverride(new_binding);
        //SetContent(label.text, input_action.GetBindingDisplayString());
        RInput.SaveOverrides();
        input_action.Enable();
        SetWindow(false);
        CleanUp();
    }

    public void Cancel(InputActionRebindingExtensions.RebindingOperation operation) {
        input_action.Enable();
        SetWindow(false);
    }

    private void CleanUp() {
        if(rebindingOperation != null) {
            rebindingOperation.Cancel();
            rebindingOperation.Dispose();
            rebindingOperation = null;
        }
    }

    private void OnDisable() {
        CleanUp();
    }

    private void OnApplyRebinding(InputActionRebindingExtensions.RebindingOperation operation, string new_binding) {
        SetButtons(true);
        this.new_binding = new_binding;
        text.text = $"Do you really want to use {new_binding}?";
    }

    private void SetButtons(bool is_active) {
        cancel_button.gameObject.SetActive(is_active);
        confirm_button.gameObject.SetActive(is_active);
    }

    private void SetWindow(bool is_active) {
        gameObject.SetActive(is_active);
    }
}
