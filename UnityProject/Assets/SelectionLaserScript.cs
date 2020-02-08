using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class SelectionLaserScript : MonoBehaviour
{
    public static SelectionLaserScript instance;

    public Transform LeftHandLaser, RightHandLaser;

    RaycastHit HitL, HitR;

    public LayerMask mask;

    private void Awake() {
        instance = this;
    }

    Coroutine UpdateLoop;

    private void OnEnable() {
        StartCoroutine(UpdateLoopEnumerator());
    }

    private void OnDisable() {
        StopAllCoroutines();
    }

    IEnumerator UpdateLoopEnumerator()
    {
        LeftHandLaser.transform.position = VRInputController.instance.LHandSphere.transform.position;
        RightHandLaser.transform.position = VRInputController.instance.RHandSphere.transform.position;

        LeftHandLaser.transform.rotation = VRInputController.instance.LHandSphere.transform.rotation;
        RightHandLaser.transform.rotation = VRInputController.instance.RHandSphere.transform.rotation;

        if (Physics.Raycast(LeftHandLaser.position, LeftHandLaser.forward, out HitL, 10f, mask)) {
            if (HitL.collider.GetComponent<UGUIVRButton>() != null) {
                if (HitL.collider.GetComponent<UGUIVRButton>().slider != null || HitL.collider.GetComponent<UGUIVRButton>().scrollbar != null) {
                    if (VRInputController.instance.ActionPress(HandSide.Left)) {
                        HitL.collider.GetComponent<UGUIVRButton>().PressButton(HitL.point);
                    }
                }
                else {
                    if (VRInputController.instance.ActionPressDown(HandSide.Left)) {
                        HitL.collider.GetComponent<UGUIVRButton>().PressButton(HitL.point);
                    }
                }
            }
        }

        if (Physics.Raycast(RightHandLaser.position, RightHandLaser.forward, out HitR, 10f, mask)) {
            if (HitR.collider.GetComponent<UGUIVRButton>() != null) {
                if (HitR.collider.GetComponent<UGUIVRButton>().slider != null || HitR.collider.GetComponent<UGUIVRButton>().scrollbar != null) {
                    if (VRInputController.instance.ActionPress(HandSide.Right)) {
                        HitR.collider.GetComponent<UGUIVRButton>().PressButton(HitR.point);
                    }
                }
                else {
                    if (VRInputController.instance.ActionPressDown(HandSide.Right)) {
                        HitR.collider.GetComponent<UGUIVRButton>().PressButton(HitR.point);
                    }
                }
            }
        }
        yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        StartCoroutine(UpdateLoopEnumerator());
    }
}
