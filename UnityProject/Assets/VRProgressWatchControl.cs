using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRProgressWatchControl : MonoBehaviour
{
    public Text TapeProgress, Timer, Milliseconds;

    Renderer rend;

    AimScript asRef;
    SpeedrunTimer srRef;

    string TapeProgressString, SpeedrunTimerString, millisecondString;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);
        asRef = VRInputBridge.instance.aimScript_ref;
        srRef = asRef.GetComponent<SpeedrunTimer>();
        rend = GetComponent<Renderer>();
        UpdateWatchRotation();
    }

    public void UpdateWatchRotation() {
        if (PlayerPrefs.GetInt("watch_visible", 1) == 1) {
            transform.localEulerAngles = new Vector3(-PlayerPrefs.GetFloat("watch_rotation"), 0, 0);
            rend.enabled = true;
            TapeProgress.transform.parent.gameObject.SetActive(true);
        }
        else {
            rend.enabled = false;
            TapeProgress.transform.parent.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (rend != null && PlayerPrefs.GetInt("watch_visible", 1) == 0) {
            if (rend.enabled) {
                rend.enabled = false;
                TapeProgress.transform.parent.gameObject.SetActive(false);
            }
            return;
        }
        if(asRef == null) {
            return;
        }
        TapeProgressString = asRef.tapes_heard.Count + "/" + asRef.total_tapes.Count + " tapes absorbed";
        TapeProgressString = TapeProgressString.Replace("1", " 1");

        SpeedrunTimerString = srRef.GetTimeString();

        millisecondString = SpeedrunTimerString.Split(':')[2];
        millisecondString = millisecondString.Replace("1", " 1");

        SpeedrunTimerString = SpeedrunTimerString.Remove(5);

        SpeedrunTimerString = SpeedrunTimerString.Replace("1", " 1");

        TapeProgress.text = TapeProgressString;
        Timer.text = SpeedrunTimerString;
        Milliseconds.text = millisecondString;
    }
}
