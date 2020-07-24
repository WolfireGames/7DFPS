using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.XR;
public enum HandSide {
    Right,
    Left
}

public class VRInputController : MonoBehaviour
{
    public static VRInputController instance;

    public GameObject LeftHand,RightHand,Head,InventoryPos, RightMeleeGrip,LeftMeleeGrip;

    public float TallestHead = 0.1f;

    public SteamVR_Action_Vector2 Locomotion;

    public SteamVR_Action_Boolean ActionButton, JumpButton, CollectButton, GunInteract1Btn, GunInteract2Btn, GunInteract3Btn, GunInteractLongBtn, RotateLeft, RotateRight, PauseGame;

    public SteamVR_Action_Pose pose;

    public GameObject LHandSphere, RHandSphere;

    public Renderer cylinderRenderer;

    public SteamVR_Action_Pose ControllerPose;

    public bool isFrontGrabbing, canFrontGrab;

    Transform muzzlepos;

    Vector3 localTriggerCenter, controllerTriggerCenter, relativeTriggerOffset;

    bool readyInit;

    MeleeWeaponInfo meleeWeapon;

    public Material gunMat;

    private void Awake() {
        instance = this;
    }

    public string GetBindingString(ISteamVR_Action_In action, bool offhand = false) {
        if (!offhand) {
            return action.GetLocalizedOrigin(VRInputBridge.instance.aimScript_ref.primaryHand == HandSide.Left ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand);
        }
        else {
            return action.GetLocalizedOrigin(VRInputBridge.instance.aimScript_ref.primaryHand == HandSide.Left ? SteamVR_Input_Sources.RightHand : SteamVR_Input_Sources.LeftHand);
        }
    }

    IEnumerator Start() {
        MaxRenderScale = SteamVR_Camera.sceneResolutionScale;
        MinRenderScale = SteamVR_Camera.sceneResolutionScale * 0.1f;
        //if (PlayerPrefs.GetInt("dynamic_resolution", 0) == 1) {
            StartCoroutine(FPS());
        //}
        yield return new WaitForSeconds(1f);
        while (RightHand.transform.localPosition == Vector3.zero) {
            yield return null;
        }

        if (VRInputBridge.instance.aimScript_ref.primaryHand == HandSide.Left) {
            LHandSphere.SetActive(false);
            RHandSphere.SetActive(true);
        }
        else {
            LHandSphere.SetActive(true);
            RHandSphere.SetActive(false);
        }

        if(VRInputBridge.instance.aimScript_ref.gun_script.HasGunComponent(GunAspect.FIRING)){
            muzzlepos = VRInputBridge.instance.aimScript_ref.gun_instance.GetComponent<FiringComponent>().point_muzzle;
            canFrontGrab = muzzlepos.localPosition.z > 0.3f;
        }else{
            canFrontGrab = false;
        }
        

        

        if (VRInputBridge.instance.aimScript_ref.gun_script.HasGunComponent(GunAspect.TRIGGER_VISUAL)) {
            Renderer triggerRenderer = VRInputBridge.instance.aimScript_ref.gun_script.GetComponent<TriggerVisualComponent>().trigger.GetComponent<MeshRenderer>();
            if(triggerRenderer == null) {
                triggerRenderer = VRInputBridge.instance.aimScript_ref.gun_script.GetComponentInChildren<TriggerVisualComponent>().trigger.GetComponent<MeshRenderer>();
            }
            if (triggerRenderer != null) {
                localTriggerCenter = triggerRenderer.transform.parent.InverseTransformPoint(triggerRenderer.bounds.center);
                if (RightHand.transform.GetChild(0).Find("trigger").GetChild(0) != null) {
                    controllerTriggerCenter = RightHand.transform.GetChild(0).Find("trigger").GetChild(0).localPosition;
                }
                relativeTriggerOffset = (localTriggerCenter - controllerTriggerCenter) - (Vector3.forward * 0.035f);
                readyInit = true;
            }
        }

        meleeWeapon = VRInputBridge.instance.aimScript_ref.gun_script.gameObject.GetComponentInChildren<MeleeWeaponInfo>();
        if (meleeWeapon != null) {
            MeleeWeaponInfo info = meleeWeapon;
            localTriggerCenter = VRInputBridge.instance.aimScript_ref.gun_script.transform.InverseTransformPoint(info.MainGrip.position);
            meleeWeapon.MainGrip.GetComponentInParent<Animator>().SetBool("VRPose",true);
            controllerTriggerCenter = RightMeleeGrip.transform.localPosition;
            relativeTriggerOffset = (localTriggerCenter - controllerTriggerCenter);
            readyInit = true;
        }

        /*Renderer[] gunRenderers = VRInputBridge.instance.aimScript_ref.gun_script.GetComponentsInChildren<Renderer>();

        bool WasGunShader = gunRenderers[0].material.shader.name.ToLower().Contains("gunshader");

        if (WasGunShader) {
            float oldDeet = gunRenderers[0].material.GetFloat(0);
            float oldScale = gunRenderers[0].material.GetFloat(1);
            float oldMetallic = gunRenderers[0].material.GetFloat(2);
            float oldSmoothness = gunRenderers[0].material.GetFloat(3);

            gunMat.SetFloat(0, oldDeet);
            gunMat.SetFloat(1, oldScale);
            gunMat.SetFloat(2, oldMetallic);
            gunMat.SetFloat(3, oldSmoothness);

            if(gunRenderers[0].material.mainTexture != null) {
                gunMat.SetTexture(0, gunRenderers[0].material.mainTexture);
            }

            foreach (Renderer rend in gunRenderers) {
                rend.material = gunMat;
            }
        }
        else {
            float oldMetallic = gunRenderers[0].material.GetFloat(0);
            float oldSmoothness = gunRenderers[0].material.GetFloat(1);

            gunMat.SetFloat(0, oldMetallic);
            gunMat.SetFloat(1, oldSmoothness);

            if (gunRenderers[0].material.mainTexture != null) {
                gunMat.SetTexture(0, gunRenderers[0].material.mainTexture);
            }

            foreach (Renderer rend in gunRenderers) {
                rend.material = gunMat;
            }
        }*/
    }

    public float MinRenderScale, MaxRenderScale;

    public float frequency = 0.5f;
    int fps;

    private IEnumerator FPS() {
        for (; ; ) {
            // Capture frame-per-second
            int lastFrameCount = Time.frameCount;
            float lastTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(frequency);
            float timeSpan = Time.realtimeSinceStartup - lastTime;
            int frameCount = Time.frameCount - lastFrameCount;

            // Display it
            fps = Mathf.RoundToInt(frameCount / timeSpan);

            if (PlayerPrefs.GetInt("dynamic_resolution", 0) == 1) {
                float framtime;

                XRStats.TryGetGPUTimeLastFrame(out framtime);

                //Debug.Log(framtime + " - " + (1f / SteamVR.instance.hmd_DisplayFrequency) * 1000f + " at " + fps);

                if (fps < SteamVR.instance.hmd_DisplayFrequency * 0.9f && framtime > (1f / SteamVR.instance.hmd_DisplayFrequency) * 1000f) {
                    SteamVR_Camera.sceneResolutionScale *= 0.95f;
                    //Debug.Log("Lowering render res! Min: " + MinRenderScale + " Max: " + MaxRenderScale);
                }
                else if (framtime < (1f / SteamVR.instance.hmd_DisplayFrequency) * 1000f) {
                    SteamVR_Camera.sceneResolutionScale *= 1.05f;
                    //Debug.Log("Raising render res! Min: " + MinRenderScale + " Max: " + MaxRenderScale);
                }

                SteamVR_Camera.sceneResolutionScale = Mathf.Clamp(SteamVR_Camera.sceneResolutionScale, MinRenderScale, MaxRenderScale);
            }
            else if (SteamVR_Camera.sceneResolutionScale != MaxRenderScale) { 
                SteamVR_Camera.sceneResolutionScale = MaxRenderScale;
            }
        }
    }

    public Vector3 GetControllerVel(HandSide hand) {
        return pose.GetVelocity(hand == HandSide.Left ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand);
    }

    public Vector3 GetAimPos(HandSide hand, bool isMag = false) {
        if (!isMag && readyInit) {
            switch (hand) {
                case HandSide.Right:
                    return RightHand.transform.position - (VRInputBridge.instance.aimScript_ref.gun_script.transform.rotation * (relativeTriggerOffset * transform.localScale.x));
                case HandSide.Left:
                    return LeftHand.transform.position - (VRInputBridge.instance.aimScript_ref.gun_script.transform.rotation * (relativeTriggerOffset * transform.localScale.x));
                default:
                    return RightHand.transform.position;
            }
        }
        else {
            switch (hand) {
                case HandSide.Right:
                    return RightHand.transform.position - GetAimDir(hand) * 0.02f;
                case HandSide.Left:
                    return LeftHand.transform.position - GetAimDir(hand) * 0.02f;
                default:
                    return RightHand.transform.position;
            }
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
            CylinderVisualComponent cylvis = VRInputBridge.instance.aimScript_ref.gun_script.GetComponent<CylinderVisualComponent>();
            if (cylvis != null) {
                if (cylvis.cylinder_assembly.Find("cylinder") != null) {
                    cylinderRenderer = cylvis.cylinder_assembly.Find("cylinder").GetComponent<Renderer>();
                }
                else {
                    cylinderRenderer = cylvis.cylinder_assembly.GetComponent<Renderer>();
                }
            }
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

    public Vector2 GetRawWalkVector(HandSide hand) {
        SteamVR_Input_Sources source = (hand == HandSide.Left ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand);
        return Locomotion.GetAxis(source);
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

    public bool TeleportPressDown(HandSide hand) {
        return JumpButton.GetStateDown(hand == HandSide.Left ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand);
    }

    public bool CollectPress(HandSide hand) {
        return CollectButton.GetState(hand == HandSide.Left ? SteamVR_Input_Sources.RightHand : SteamVR_Input_Sources.LeftHand);
    }

    public bool CollectPressDown(HandSide hand) {
        return CollectButton.GetStateDown(hand == HandSide.Left ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand);
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

    private void LateUpdate() {
        if (meleeWeapon != null) {
            if (LHandSphere.activeSelf) {
                meleeWeapon.MainGrip.rotation = RightMeleeGrip.transform.rotation;
            }
            else {
                meleeWeapon.MainGrip.rotation = LeftMeleeGrip.transform.rotation;
            }
        }
    }

    void Update()
    {
        Debug.DrawLine(RightHand.transform.position, RightHand.transform.position - (RightHand.transform.rotation * relativeTriggerOffset), Color.red);
        if (TallestHead < Head.transform.localPosition.y) {
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

        //if (ChangeHandedness.GetStateDown(SteamVR_Input_Sources.Any)) {
        //    PlayerPrefs.SetInt("left_handed", PlayerPrefs.GetInt("left_handed") == 1?0:1);
        //}

        if (PlayerPrefs.GetInt("left_handed",0) == 1) {
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
