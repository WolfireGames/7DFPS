using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class OptionLabelUtil : MonoBehaviour {
    private Text textBox;

    [TextArea] public string prefix = "";

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
