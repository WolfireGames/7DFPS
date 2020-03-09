using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRProgressWatchControl : MonoBehaviour
{
    public Text TapeProgress, Timer, Milliseconds;

    AimScript asRef;
    SpeedrunTimer srRef;

    string TapeProgressString, SpeedrunTimerString, millisecondString;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);
        asRef = VRInputBridge.instance.aimScript_ref;
        srRef = asRef.GetComponent<SpeedrunTimer>();
    }

    void Update()
    {
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
