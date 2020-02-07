using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class OptionLabelUtil : MonoBehaviour {
    private Text textBox;

    [TextArea] public string prefix = "";
    public bool is_mod_path_label = false;

    public void Start() {
        if(is_mod_path_label) {
            ChangeValue(ModManager.GetModsfolderPath());
        }
    }

    public void ChangeValue(float value) {
        if(!textBox)
            textBox = GetComponent<Text>();
        textBox.text = $"{prefix}{value:0.00}";
    }

    public void ChangeValue(string value) {
        if(!textBox)
            textBox = GetComponent<Text>();
        textBox.text = $"{prefix}{value}";
    }
}
