using UnityEngine;
using UnityEngine.UI;

public class RestartDialogScript : MonoBehaviour {
    public Dropdown weapon_selection;
    public InputField seed_selection;
    
    public void OnSeedUpdate() {
        if(seed_selection.text != "") {
            if(int.TryParse(seed_selection.text, out int result)) {
                weapon_selection.value = 0;
                weapon_selection.interactable = false;
                PlayerPrefs.SetInt("generation_seed", int.Parse(seed_selection.text));
            } else {
                seed_selection.text = seed_selection.text.Remove(seed_selection.text.Length - 1);
            }
        } else {
            weapon_selection.interactable = true;
        }
    }

    public void OnWeaponSelectionUpdate() {
        if(weapon_selection.value != 0) {
            seed_selection.text = "";
            seed_selection.interactable = false;
        } else {
            seed_selection.interactable = true;
        }
    }
}
