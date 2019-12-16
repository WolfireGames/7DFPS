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

    public SteamVR_Action_Boolean ActionButton, JumpButton, CollectButton, GunInteract1Btn, GunInteract2Btn, GunInteract3Btn, GunInteractLongBtn, RotateLeft, RotateRight, ChangeHandedness;

    public SteamVR_Action_Pose ControllerPose;

    private void Awake() {
        instance = this;
    }

    public Vector3 GetAimPos(HandSide hand) {
        switch (hand) {
            case HandSide.Right:
                return RightHand.transform.position - GetAimDir(hand)*0.02f;
            case HandSide.Left:
                return LeftHand.transform.position - GetAimDir(hand) * 0.02f;
            default:
                return RightHand.transform.position;
        }
    }

    public float GetSpinSpeed(HandSide hand) {
        switch (hand) {
            case HandSide.Right:
                if (Vector3.Distance(RightHand.transform.position, LeftHand.transform.position) < 0.15f) {
                    return -ControllerPose.GetVelocity(SteamVR_Input_Sources.RightHand).y;
                }
                else {
                    return 0f;
                }
            case HandSide.Left:
                if (Vector3.Distance(RightHand.transform.position, LeftHand.transform.position) < 0.15f) {
                    return -ControllerPose.GetVelocity(SteamVR_Input_Sources.LeftHand).y;
                }
                else {
                    return 0f;
                }
            default:
                return ControllerPose.GetVelocity(SteamVR_Input_Sources.RightHand).y;
        }
    }

    public Vector3 GetAimDir(HandSide hand) {
        switch (hand) {
            case HandSide.Right:
                return -RightHand.transform.up*1.5f + RightHand.transform.forward;
            case HandSide.Left:
                return -LeftHand.transform.up*1.5f + LeftHand.transform.forward;
            default:
                return RightHand.transform.forward;
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
        InventoryPos.transform.localPosition = Head.transform.localPosition - (Vector3.up * TallestHead / 3f);
        InventoryPos.transform.rotation = transform.rotation;

        if (ChangeHandedness.GetStateDown(SteamVR_Input_Sources.Any)) {
            if(VRInputBridge.instance.aimScript_ref.primaryHand == HandSide.Right) {
                VRInputBridge.instance.aimScript_ref.primaryHand = HandSide.Left;
                VRInputBridge.instance.aimScript_ref.secondaryHand = HandSide.Right;
            }
            else {
                VRInputBridge.instance.aimScript_ref.primaryHand = HandSide.Right;
                VRInputBridge.instance.aimScript_ref.secondaryHand = HandSide.Left;
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
