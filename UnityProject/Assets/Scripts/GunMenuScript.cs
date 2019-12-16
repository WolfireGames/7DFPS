using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class GunMenuScript : MonoBehaviour {
    private Dropdown dropdown;
    private GUISkinHolder gui_skin_holder;

    public void Awake() {
        dropdown = GetComponent<Dropdown>();
        gui_skin_holder = GameObject.Find("gui_skin_holder").GetComponent<GUISkinHolder>();
    }

    public void Start() {
        // Clear previous options
        dropdown.ClearOptions();

        // Extract names from WeaponHolder array
        List<string> strings = new List<string>();
        strings.Add("Random");
        foreach (var weapon in gui_skin_holder.weapons)
            strings.Add(weapon.GetComponent<WeaponHolder>().display_name);

        // Add new options
        dropdown.AddOptions(strings);

        // Set current value as active
        dropdown.SetValueWithoutNotify(PlayerPrefs.GetInt("selected_gun_index", -1) + 1);
    }

    public void OnValueChanged(int new_value) {
        PlayerPrefs.SetInt("selected_gun_index", new_value - 1);
    }
}
