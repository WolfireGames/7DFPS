using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpCanvasManager : MonoBehaviour
{
    public static Text helpText;
    public GameObject Backdrop;

    void Start()
    {
        helpText = GetComponentInChildren<Text>();
        transform.localScale = Vector3.one * 0.1f;
    }

    void LateUpdate()
    {
        if(PlayerPrefs.GetInt("show_help",0) == 1) {
            Backdrop.SetActive(true);
            helpText.gameObject.SetActive(true);
            Transform head = VRInputController.instance.Head.transform;
            transform.rotation = Quaternion.LookRotation(head.forward, head.up);
            transform.position = head.transform.position + head.forward*0.5f - head.up * 0.2f;// VRInputController.instance.GetAimPos(VRInputBridge.instance.aimScript_ref.primaryHand) - transform.right * 0.15f;
        }
        else {
            Backdrop.SetActive(false);
            helpText.gameObject.SetActive(false);
        }
    }
}
