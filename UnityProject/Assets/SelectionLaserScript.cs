using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class SelectionLaserScript : MonoBehaviour
{
    public static SelectionLaserScript instance;

    public Transform LeftHandLaser, RightHandLaser;

    public Transform LeftHandSelectionCircle, RightHandSelectionCircle;

    RaycastHit HitL, HitR;

    public LayerMask mask;

    public GameObject LastUsedObjectL, LastUsedObjectR;

    private void Awake() {
        instance = this;
    }

    VRProgressWatchControl wristWatch;

    Coroutine UpdateLoop;

    private void OnEnable() {
        wristWatch = FindObjectOfType<VRProgressWatchControl>();
        StartCoroutine(UpdateLoopEnumerator());
    }

    private void OnDisable() {
        StopAllCoroutines();
    }

    IEnumerator UpdateLoopEnumerator()
    {
        wristWatch.UpdateWatchRotation();
        LeftHandLaser.transform.position = VRInputController.instance.LHandSphere.transform.position;
        RightHandLaser.transform.position = VRInputController.instance.RHandSphere.transform.position;

        LeftHandLaser.transform.rotation = VRInputController.instance.LHandSphere.transform.rotation;
        RightHandLaser.transform.rotation = VRInputController.instance.RHandSphere.transform.rotation;

        if (Physics.Raycast(LeftHandLaser.position, LeftHandLaser.forward, out HitL, 10f, mask)) {
            if (!LeftHandSelectionCircle.gameObject.activeSelf) {
                LeftHandSelectionCircle.gameObject.SetActive(true);
            }
            LeftHandSelectionCircle.position = HitL.point;

            LeftHandLaser.GetChild(0).localScale = new Vector3(1f, 1f, HitL.distance * 10f);

            if (HitL.collider.GetComponent<UGUIVRButton>() != null) {
                if (HitL.collider.GetComponent<UGUIVRButton>().slider != null || HitL.collider.GetComponent<UGUIVRButton>().scrollbar != null) {
                    if (LastUsedObjectL == null) { 
                        if (VRInputController.instance.ActionPress(HandSide.Left)) {
                            HitL.collider.GetComponent<UGUIVRButton>().PressButton(HitL.point);
                        }

                        if (VRInputController.instance.ActionPressDown(HandSide.Left)) {
                            LastUsedObjectL = HitL.collider.gameObject;
                        }
                    }
                    else {
                        if (LastUsedObjectL == HitL.collider.gameObject && VRInputController.instance.ActionPress(HandSide.Left)) {
                            HitL.collider.GetComponent<UGUIVRButton>().PressButton(HitL.point);
                        }
                    }
                }
                else {
                    if (LastUsedObjectL == null && VRInputController.instance.ActionPressDown(HandSide.Left)) {
                        HitL.collider.GetComponent<UGUIVRButton>().PressButton(HitL.point);
                        LastUsedObjectL = HitL.collider.gameObject;
                    }
                }
            }
        }
        else {
            if(LeftHandSelectionCircle.gameObject.activeSelf)
                LeftHandSelectionCircle.gameObject.SetActive(false);

            LeftHandLaser.GetChild(0).localScale = new Vector3(1f, 1f, 10f);
        }

        if (VRInputController.instance.ActionPressUp(HandSide.Left)) {
            LastUsedObjectL = null;
        }

        if (VRInputController.instance.ActionPressUp(HandSide.Right)) {
            LastUsedObjectR = null;
        }

        if (Physics.Raycast(RightHandLaser.position, RightHandLaser.forward, out HitR, 10f, mask)) {
            if (!RightHandSelectionCircle.gameObject.activeSelf) {
                RightHandSelectionCircle.gameObject.SetActive(true);
            }
            RightHandSelectionCircle.position = HitR.point;

            RightHandLaser.GetChild(0).localScale = new Vector3(1f, 1f, HitR.distance * 10f);

            if (HitR.collider.GetComponent<UGUIVRButton>() != null) {
                if (HitR.collider.GetComponent<UGUIVRButton>().slider != null || HitR.collider.GetComponent<UGUIVRButton>().scrollbar != null) {
                    if (LastUsedObjectR == null) {
                        if (VRInputController.instance.ActionPress(HandSide.Right)) {
                            HitR.collider.GetComponent<UGUIVRButton>().PressButton(HitR.point);
                        }

                        if (VRInputController.instance.ActionPressDown(HandSide.Right)) {
                            LastUsedObjectR = HitR.collider.gameObject;
                        }
                    }
                    else {
                        if (LastUsedObjectR == HitR.collider.gameObject && VRInputController.instance.ActionPress(HandSide.Right)) {
                            HitR.collider.GetComponent<UGUIVRButton>().PressButton(HitR.point);
                        }
                    }
                }
                else {
                    if (LastUsedObjectR == null && VRInputController.instance.ActionPressDown(HandSide.Right)) {
                        HitR.collider.GetComponent<UGUIVRButton>().PressButton(HitR.point);
                        LastUsedObjectR = HitR.collider.gameObject;
                    }
                }
            }
        }
        else {
            if (RightHandSelectionCircle.gameObject.activeSelf)
                RightHandSelectionCircle.gameObject.SetActive(false);

            RightHandLaser.GetChild(0).localScale = new Vector3(1f, 1f, 10f);
        }

        yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime * 0.5f);
        StartCoroutine(UpdateLoopEnumerator());
    }
}
