using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[RequireComponent(typeof(Dropdown))]
public class ScreenModeMenuScript : OptionInitializerBase {
    private Dropdown dropdown;

    public override void Initialize() {
        UpdateDropdown();
    }

    private void Awake() {
        UpdateDropdown();
    }

    private void OnEnable() {
        UpdateDropdown();
    }

    private void UpdateDropdown() {
        dropdown = GetComponent<Dropdown>();
        dropdown.SetValueWithoutNotify((int) Screen.fullScreenMode);
    }
}
