using UnityEngine;
using UnityEngine.UI;

public class OptionLabelUtil : MonoBehaviour {
    private Text textBox;

    void Awake() {
        this.textBox = GetComponent<Text>();
    }

    public void ChangeValue(float value) {
        textBox.text = $"{value:0.00}";
    }
}
