using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class OptionLabelUtil : MonoBehaviour {
    private Text textBox;

    public void ChangeValue(float value) {
        if(textBox == null) {
            textBox = GetComponent<Text>();
        }

        textBox.text = $"{value:0.00}";
    }
}
