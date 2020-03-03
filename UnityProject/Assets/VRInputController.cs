using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public enum HandSide {
    Right,
    Left
}

public class VRInputController : MonoBehaviour
{
    public static VRInputController instance;

    public GameObject LeftHand,RightHand,Head,InventoryPos;

    public float TallestHead = 0.1f;

    public SteamVR_Action_Vector2 Locomotion;

    public SteamVR_Action_Boolean ActionButton, JumpButton, CollectButton, GunInteract1Btn, GunInteract2Btn, GunInteract3Btn, GunInteractLongBtn, RotateLeft, RotateRight, ChangeHandedness, PauseGame;

    public SteamVR_Action_Pose pose;

    public GameObject LHandSphere, RHandSphere;

    public Renderer cylinderRenderer;

    public SteamVR_Action_Pose ControllerPose;

    public bool isFrontGrabbing, canFrontGrab;

    Transform muzzlepos;

    float triggerDepthOffset;

    private void Awake() {
        instance = this;
    }

    IEnumerator Start() {
        yield return new WaitForSeconds(1f);
        if (VRInputBridge.instance.aimScript_ref.primaryHand == HandSide.Left) {
            LHandSphere.SetActive(false);
            RHandSphere.SetActive(true);
        }
        else {
            LHandSphere.SetActive(true);
            RHandSphere.SetActive(false);
        }

        muzzlepos = VRInputBridge.instance.aimScript_ref.gun_instance.GetComponent<FiringComponent>().point_muzzle;
        canFrontGrab = muzzlepos.localPosition.z > 0.3f;

        if (VRInputBridge.instance.aimScript_ref.gun_script.HasGunComponent(GunAspect.TRIGGER_VISUAL)) {
            Renderer triggerRenderer = VRInputBridge.instance.aimScript_ref.gun_script.GetComponent<TriggerVisualComponent>().trigger.GetComponent<MeshRenderer>();
            if(triggerRenderer == null) {
                triggerRenderer = VRInputBridge.instance.aimScript_ref.gun_script.GetComponentInChildren<TriggerVisualComponent>().trigger.GetComponent<MeshRenderer>();
            }
            if (triggerRenderer != null) {
                triggerDepthOffset = (VRInputBridge.instance.aimScript_ref.gun_script.transform.InverseTransformPoint(triggerRenderer.bounds.center).z * 0.5f) + 0.01f;
                Debug.Log("Trigger depth offset set: " + triggerDepthOffset);
            }
            else {
                triggerDepthOffset = 0.02f;
            }
        }
        else {
            triggerDepthOffset = 0.02f;
        }
    }

    public Vector3 GetControllerVel(HandSide hand) {
        return pose.GetVelocity(hand == HandSide.Left ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand);
    }

    public Vector3 GetAimPos(HandSide hand) {
        switch (hand) {
            case HandSide.Right:
                return RightHand.transform.position - GetAimDir(hand)* triggerDepthOffset;
            case HandSide.Left:
                return LeftHand.transform.position - GetAimDir(hand) * triggerDepthOffset;
            default:
                return RightHand.transform.position;
        }
    }

    public Vector3 GetAimDir(HandSide hand) {
        if (!isFrontGrabbing) { 
            switch (hand) {
                case HandSide.Right:
                    return -RightHand.transform.up * 1.5f + RightHand.transform.forward;
                case HandSide.Left:
                    return -LeftHand.transform.up * 1.5f + LeftHand.transform.forward;
                default:
                    return RightHand.transform.forward;
            }
        }else{
            switch (hand) {
                case HandSide.Right:
                    return ((LHandSphere.transform.position) - RHandSphere.transform.position).normalized;
                case HandSide.Left:
                    return ((RHandSphere.transform.position) - LHandSphere.transform.position).normalized;
                default:
                    return RightHand.transform.forward;
            }
        }
    }


    bool checkedCylinderRenderer;

    public float GetSpinSpeed(HandSide hand) {
        if(!checkedCylinderRenderer && VRInputBridge.instance.aimScript_ref != null && VRInputBridge.instance.aimScript_ref.gun_script.HasGunComponent(GunAspect.REVOLVER_CYLINDER)) {
            cylinderRenderer = VRInputBridge.instance.aimScript_ref.gun_script.GetComponent<CylinderVisualComponent>().cylinder_assembly.Find("cylinder").GetComponent<Renderer>();
            checkedCylinderRenderer = true;
        }
        if (cylinderRenderer != null) {
            switch (hand) {
                case HandSide.Right:
                    if (cylinderRenderer.bounds.Contains(RHandSphere.transform.position)) {
                        return ControllerPose.GetVelocity(SteamVR_Input_Sources.RightHand).y;
                    }
                    else {
                        return 0f;
                    }
                case HandSide.Left:
                    if (cylinderRenderer.bounds.Contains(LHandSphere.transform.position)) {
                        return -ControllerPose.GetVelocity(SteamVR_Input_Sources.LeftHand).y;
                    }
                    else {
                        return 0f;
                    }
                default:
                    return ControllerPose.GetVelocity(SteamVR_Input_Sources.RightHand).y;
            }
        }
        else {
            return 0f;
        }
    }

    public Vector3 GetAimHandlePos(HandSide hand) {

            switch (hand) {
                case HandSide.Right:
                    return RightHand.transform.position - RightHand.transform.forward * 0.06f;
                case HandSide.Left:
                    return LeftHand.transform.position - LeftHand.transform.forward * 0.06f;
                default:
                    return RightHand.transform.position;
            }
    }

    bool FlipFlashlightDirection;

    public Vector3 GetAimHandleDir(HandSide hand) {
        if (!isFrontGrabbing && Vector3.Distance(LeftHand.transform.localPosition,RightHand.transform.localPosition) > 0.175f) {
            switch (hand) {
                case HandSide.Right:
                    if (Vector3.Angle(RightHand.transform.forward, Head.transform.forward) > 90) {
                        if (GunInteractDown(hand)) {
                            FlipFlashlightDirection = true;
                        }
                    }
                    else if (Vector3.Angle(RightHand.transform.forward, Head.transform.forward) < 90) {
                        if (GunInteractDown(hand)) {
                            FlipFlashlightDirection = false;
                        }
                    }
                    return RightHand.transform.forward * (FlipFlashlightDirection ? -1 : 1);
                case HandSide.Left:
                    if (Vector3.Angle(LeftHand.transform.forward, Head.transform.forward) > 90) {
                        if (GunInteractDown(hand)) {
                            FlipFlashlightDirection = true;
                        }
                    }
                    else if (Vector3.Angle(LeftHand.transform.forward, Head.transform.forward) < 90) {
                        if (GunInteractDown(hand)) {
                            FlipFlashlightDirection = false;
                        }
                    }
                    return LeftHand.transform.forward * (FlipFlashlightDirection ? -1 : 1);
                default:
                    return RightHand.transform.forward;
            }
        }
        else {
            switch (hand) {
                case HandSide.Right:
                    return GetAimDir(HandSide.Left);
                case HandSide.Left:
                    return GetAimDir(HandSide.Right);
                default:
                    return RightHand.transform.position;
            }
        }
    }

   
    public Vector3 GetAimUp(HandSide hand) {
        switch (hand) {
            case HandSide.Right:
                return RightHand.transform.forward + (RightHand.transform.up*1.5f);
            case HandSide.Left:
                return LeftHand.transform.forward + (LeftHand.transform.up*1.5f);
            default:
                return RightHand.transform.up;
        }
    }

    public Vector2 GetWalkVector(HandSide hand) {
        SteamVR_Input_Sources source = (hand == HandSide.Left ? SteamVR_Input_Sources.RightHand : SteamVR_Input_Sources.LeftHand);
        Vector3 rawAxis = new Vector3(Locomotion.GetAxis(hand == HandSide.Left ? SteamVR_Input_Sources.RightHand : SteamVR_Input_Sources.LeftHand).x, 0, Locomotion.GetAxis(hand == HandSide.Left ? SteamVR_Input_Sources.RightHand : SteamVR_Input_Sources.LeftHand).y);
        rawAxis = Head.transform.localRotation * rawAxis;
        return new Vector2(rawAxis.x,rawAxis.z);
    }

    public bool GetPauseGame(HandSide hand) {
        return PauseGame.GetStateDown(hand == HandSide.Left ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand);
    }

    public bool GetRotateLeft(HandSide hand) {
        return RotateLeft.GetStateDown(hand == HandSide.Left?SteamVR_Input_Sources.LeftHand: SteamVR_Input_Sources.RightHand);
    }

    public bool GetRotateRight(HandSide hand) {
        return RotateRight.GetStateDown(hand == HandSide.Left ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand);
    }

    public bool JumpPress(HandSide hand) {
        return JumpButton.GetState(hand == HandSide.Right ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand);
    }

    public bool CollectPress(HandSide hand) {
        return CollectButton.GetState(hand == HandSide.Left ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand);
    }

    public bool ActionPress(HandSide hand) {
        return ActionButton.GetState(hand == HandSide.Left ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand);
    }

    public bool GunInteract(HandSide hand) {
        return GunInteract1Btn.GetState(hand == HandSide.Left ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand);
    }

    public bool GunInteractLongPress(HandSide hand) {
        return GunInteractLongBtn.GetState(hand == HandSide.Left ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand);
    }

    public bool GunInteract2(HandSide hand) {
        return GunInteract2Btn.GetState(hand == HandSide.Left ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand);
    }

    public bool GunInteract3(HandSide hand) {
        return GunInteract3Btn.GetState(hand == HandSide.Left ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand);
    }

    public bool ActionPressDown(HandSide hand) {
        return ActionButton.GetStateDown(hand == HandSide.Left ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand);
    }

    public bool GunInteractDown(HandSide hand) {
        return GunInteract1Btn.GetStateDown(hand == HandSide.Left ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand);
    }

    public bool GunInteractLongPressDown(HandSide hand) {
        return GunInteractLongBtn.GetStateDown(hand == HandSide.Left ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand);
    }

    public bool GunInteract2Down(HandSide hand) {
        return GunInteract2Btn.GetStateDown(hand == HandSide.Left ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand);
    }

    public bool GunInteract3Down(HandSide hand) {
        return GunInteract3Btn.GetStateDown(hand == HandSide.Left ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand);
    }

    public bool ActionPressUp(HandSide hand) {
        return ActionButton.GetStateUp(hand == HandSide.Left ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand);
    }

    public bool GunInteractUp(HandSide hand) {
        return GunInteract1Btn.GetStateUp(hand == HandSide.Left ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand);
    }

    public bool GunInteractLongPressUp(HandSide hand) {
        return GunInteractLongBtn.GetStateUp(hand == HandSide.Left ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand);
    }

    public bool GunInteract2Up(HandSide hand) {
        return GunInteract2Btn.GetStateUp(hand == HandSide.Left ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand);
    }

    public bool GunInteract3Up(HandSide hand) {
        return GunInteract3Btn.GetStateUp(hand == HandSide.Left ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand);
    }

    void Update()
    {
        if(TallestHead < Head.transform.localPosition.y) {
            TallestHead = Head.transform.localPosition.y+0.1f;
        }

        if (canFrontGrab) {
            if (LHandSphere.activeSelf) {

                if (Vector3.Distance(Vector3.Lerp(RightHand.transform.position, muzzlepos.position, 0.65f), LHandSphere.transform.position) < 0.1f) {
                    isFrontGrabbing = ActionPress(HandSide.Left);
                }

                if (isFrontGrabbing) {
                    LHandSphere.GetComponent<Renderer>().enabled = false;
                    isFrontGrabbing = ActionPress(HandSide.Left);
                }
                if (ActionPressUp(HandSide.Left)) {
                    LHandSphere.GetComponent<Renderer>().enabled = true;
                }
            }

            if (RHandSphere.activeSelf) {

                if (Vector3.Distance(Vector3.Lerp(LeftHand.transform.position, muzzlepos.position, 0.65f), RHandSphere.transform.position) < 0.1f) {
                    isFrontGrabbing = ActionPress(HandSide.Right);
                }

                if (isFrontGrabbing) {
                    RHandSphere.GetComponent<Renderer>().enabled = false;
                    isFrontGrabbing = ActionPress(HandSide.Right);
                }
                if (ActionPressUp(HandSide.Right)) {
                    RHandSphere.GetComponent<Renderer>().enabled = true;
                }
            }
        }

        InventoryPos.transform.localPosition = (new Vector3(Head.transform.localPosition.x - (transform.InverseTransformDirection(Head.transform.forward).x * 0.15f), Head.transform.localPosition.y, Head.transform.localPosition.z - (transform.InverseTransformDirection(Head.transform.forward).z * 0.15f)) * Head.transform.localScale.x) - (Vector3.up * TallestHead / 3f);
        InventoryPos.transform.rotation = transform.rotation;

        if (ChangeHandedness.GetStateDown(SteamVR_Input_Sources.Any)) {
            if(VRInputBridge.instance.aimScript_ref.primaryHand == HandSide.Right) {
                VRInputBridge.instance.aimScript_ref.primaryHand = HandSide.Left;
                VRInputBridge.instance.aimScript_ref.secondaryHand = HandSide.Right;
                LHandSphere.SetActive(false);
                RHandSphere.SetActive(true);
            }
            else {
                VRInputBridge.instance.aimScript_ref.primaryHand = HandSide.Right;
                VRInputBridge.instance.aimScript_ref.secondaryHand = HandSide.Left;
                LHandSphere.SetActive(true);
                RHandSphere.SetActive(false);
            }
        }
        //InventoryPos.transform.LookAt(Head.transform.forward - new Vector3(0, Head.transform.forward.y, 0));
        //InventoryPos.transform.position += InventoryPos.transform.forward * 0.5f;
    }

    private void FixedUpdate() {
        /*LeftHand.transform.localPosition = ControllerPose.GetLocalPosition(SteamVR_Input_Sources.LeftHand);
        RightHand.transform.localPosition = ControllerPose.GetLocalPosition(SteamVR_Input_Sources.RightHand);

        LeftHand.transform.localRotation = ControllerPose.GetLocalRotation(SteamVR_Input_Sources.LeftHand);
        RightHand.transform.localRotation = ControllerPose.GetLocalRotation(SteamVR_Input_Sources.RightHand);*/
    }
}
