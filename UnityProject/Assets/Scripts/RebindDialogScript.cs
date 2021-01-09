using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class RebindDialogScript : MonoBehaviour {
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private string new_binding;
    private InputAction input_action;
    private InputBinding binding;
    public Text text;

    public Button cancel_button;
    public Button confirm_button;

    public void Rebind(InputAction input_action, InputBinding binding) {
        text.text = $"Press the key you want to rebind {input_action.name} to.\nCancel with \"ESC\"";
        this.input_action = input_action;
        this.binding = binding;
        SetButtons(false);
        SetWindow(true);

        input_action.Disable();
        rebindingOperation = input_action.PerformInteractiveRebinding()
            .WithCancelingThrough("<Keyboard>/escape")
            .WithExpectedControlType("Button")
            .OnApplyBinding( (x, y) => { OnApplyRebinding(x, y); } )
            .OnCancel( (x) => Cancel(x) )
            .Start();

    }

    public void Confirm() {
        if(binding.isPartOfComposite)
            input_action.ApplyBindingOverride(input_action.bindings.IndexOf((x) => x.name == binding.name), new_binding);
        else
            input_action.ApplyBindingOverride(new_binding);

        RInput.SaveOverrides();
        input_action.Enable();
        SetWindow(false);
        CleanUp();

        // Hack to refresh pause menu and reset content of all Keybind settings
        transform.parent.gameObject.SetActive(false);
        transform.parent.gameObject.SetActive(true);
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
