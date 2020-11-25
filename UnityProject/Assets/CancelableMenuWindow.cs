using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelableMenuWindow : MonoBehaviour {
    private void OnEnable() {
        optionsmenuscript.window_stack.Push( gameObject );
    }
}
