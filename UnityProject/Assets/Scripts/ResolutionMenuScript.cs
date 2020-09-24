using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[RequireComponent(typeof(Dropdown))]
public class ResolutionMenuScript : OptionInitializerBase {
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

        int index_override = dropdown.value;

        // Clear previous options
        dropdown.ClearOptions();

        string[] strings = new string[Screen.resolutions.Length];
        for (int i = 0; i < Screen.resolutions.Length; i++) {
            var screen = Screen.resolutions[i];

            strings[i] = Screen.resolutions[i].ToString();
            if(Screen.fullScreen && Screen.Equals(screen, Screen.currentResolution)) {
                index_override = i;
            }
        }
        // Add new options
        dropdown.AddOptions(strings.ToList());
        if(index_override != -1) {
            dropdown.SetValueWithoutNotify(index_override);
        }
    }
}
