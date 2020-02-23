using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRInputBridge : MonoBehaviour
{
    public static VRInputBridge instance;

    public AimScript aimScript_ref;

    Renderer SlideObject;
    Bounds SlideBounds;

    void Awake()
    {
        instance = this;
    }

    private IEnumerator Start() {
        yield return new WaitForSeconds(0.5f);
        aimScript_ref = FindObjectOfType<AimScript>();

        if (aimScript_ref.gun_script.HasGunComponent(GunAspect.SLIDE)){//gun_instance.GetComponent<GunScript>().HasSlide()) {
            SlideObject = aimScript_ref.gun_script.GetComponent<SlideVisualComponent>().slide.GetComponent<Renderer>();
            if(SlideObject == null) {
                SlideObject = aimScript_ref.gun_script.GetComponent<SlideVisualComponent>().slide.GetComponentInChildren<Renderer>();
            }
        }

        Camera.main.nearClipPlane = 0.01f;
    }

    public bool MagOut;

    public bool GetButtonDown(string input_str, HandSide hand) {
        switch (input_str) {
            case "Slide Lock":
                return VRInputController.instance.GunInteractDown(hand);
            case "Safety":
                return VRInputController.instance.GunInteract2Down(hand);
            case "Auto Mod Toggle":
                return VRInputController.instance.GunInteract3Down(hand);
            case "Pull Back Slide":
                if(SlideObject == null) {
                    return false;
                }
                if (hand == HandSide.Left) {
                    SlideBounds = SlideObject.bounds;
                    if (SlideBounds.Contains(VRInputController.instance.LHandSphere.transform.position) || MagOut) {
                        return VRInputController.instance.ActionPressDown(hand);
                    }
                    else {
                        return false;
                    }
                }
                else {
                    SlideBounds = SlideObject.bounds;
                    if (SlideBounds.Contains(VRInputController.instance.RHandSphere.transform.position) || MagOut) {
                        return VRInputController.instance.ActionPressDown(hand);
                    }
                    else {
                        return false;
                    }
                }

            case "Swing Out Cylinder":
                return VRInputController.instance.GunInteractDown(hand);

            case "Close Cylinder":
                return VRInputController.instance.GunInteractUp(hand);//Make more specific and 3D

            case "Insert":

                if (aimScript_ref != null) {
                    
                    if (aimScript_ref.primaryHand == HandSide.Right) {
                        if (hand == HandSide.Left) {
                            return VRInputController.instance.GunInteractDown(hand);//Magazine bullet insert
                        }
                        else if (aimScript_ref.gun_script.HasGunComponent(GunAspect.EXTERNAL_MAGAZINE)){//Magazine into gun insert, have to hold the mag under the gun.
                            Vector3 magInsertPos = aimScript_ref.gun_script.GetComponent<ExternalMagazineComponent>().point_mag_to_insert.position;
                            if (MagOut && Vector3.Distance(VRInputController.instance.LHandSphere.transform.position, magInsertPos) < 0.075f && VRInputController.instance.LHandSphere.transform.position.y < magInsertPos.y) {
                                MagOut = false;
                                return true;
                            }
                            else {
                                return false;
                            }
                        }
                    }
                    else {
                        if (hand == HandSide.Right) {
                            return VRInputController.instance.GunInteractDown(hand);//Lefthanded version
                        }
                        else if (aimScript_ref.gun_script.HasGunComponent(GunAspect.EXTERNAL_MAGAZINE)) {
                            Vector3 magInsertPos = aimScript_ref.gun_script.GetComponent<ExternalMagazineComponent>().point_mag_to_insert.position;
                            if (MagOut && Vector3.Distance(magInsertPos, VRInputController.instance.RHandSphere.transform.position) < 0.05f && VRInputController.instance.RHandSphere.transform.position.y < magInsertPos.y) {
                                MagOut = false;
                                return true;
                            }
                            else {
                                return false;
                            }
                        }
                    }
                }
                return false;

            case "Eject/Drop":
                if (VRInputController.instance.GunInteractLongPressDown(hand)) {
                    MagOut = true;
                }
                return VRInputController.instance.GunInteractLongPressDown(hand);
            case "Flashlight Toggle":
                return VRInputController.instance.GunInteract2Down(hand);
            case "Inventory 1":
                if (VRInventoryManager.instance.ActiveSlot == 0) {
                    return VRInputController.instance.ActionPressDown(hand);
                }
                else {
                    return false;
                }
            case "Inventory 2":
                if (VRInventoryManager.instance.ActiveSlot == 1) {
                    return VRInputController.instance.ActionPressDown(hand);
                }
                else {
                    return false;
                }
            case "Inventory 3":
                if (VRInventoryManager.instance.ActiveSlot == 2) {
                    return VRInputController.instance.ActionPressDown(hand);
                }
                else {
                    return false;
                }
            case "Inventory 4":
                if (VRInventoryManager.instance.ActiveSlot == 3) {
                    return VRInputController.instance.ActionPressDown(hand);
                }
                else {
                    return false;
                }
            case "Inventory 5":
                if (VRInventoryManager.instance.ActiveSlot == 4) {
                    return VRInputController.instance.ActionPressDown(hand);
                }
                else {
                    return false;
                }
            case "Inventory 6":
                if (VRInventoryManager.instance.ActiveSlot == 5) {
                    return VRInputController.instance.ActionPressDown(hand);
                }
                else {
                    return false;
                }
            case "Inventory 7":
                if (VRInventoryManager.instance.ActiveSlot == 6) {
                    return VRInputController.instance.ActionPressDown(hand);
                }
                else {
                    return false;
                }
            case "Inventory 8":
                if (VRInventoryManager.instance.ActiveSlot == 7) {
                    return VRInputController.instance.ActionPressDown(hand);
                }
                else {
                    return false;
                }
            case "Inventory 9":
                if (VRInventoryManager.instance.ActiveSlot == 8) {
                    return VRInputController.instance.ActionPressDown(hand);
                }
                else {
                    return false;
                }
            case "Inventory 10":
                if (VRInventoryManager.instance.ActiveSlot == 9) {
                    return VRInputController.instance.ActionPressDown(hand);
                }
                else {
                    return false;
                }
            case "Tape Player":
                if (VRInventoryManager.instance.TapePlayer) {
                    return VRInputController.instance.ActionPressDown(hand);
                }
                else {
                    return false;
                }
            default:
                return Input.GetButtonDown(input_str);
        }
        
    }
    public bool GetButton(string input_str, HandSide hand) {
        switch (input_str) {
            case "Trigger":
                return VRInputController.instance.ActionPress(hand);
            case "Slide Lock":
                return VRInputController.instance.GunInteract(hand);
            case "Extractor Rod":
                return VRInputController.instance.GunInteract2(hand);//Make more specific and 3D
            case "Hammer":
                return VRInputController.instance.GunInteract3(hand);
            case "Get":
                return VRInputController.instance.CollectPress(hand);
            case "Pull Back Slide":
                if (SlideObject == null) {
                    return false;
                }
                if (hand == HandSide.Left) {
                    SlideBounds = SlideObject.bounds;
                    if (SlideBounds.Contains(VRInputController.instance.LeftHand.transform.position) || MagOut) {
                        return VRInputController.instance.ActionPress(hand);
                    }
                    else {
                        return false;
                    }
                }
                else {
                    SlideBounds = SlideObject.bounds;
                    if (SlideBounds.Contains(VRInputController.instance.RightHand.transform.position) || MagOut) {
                        return VRInputController.instance.ActionPress(hand);
                    }
                    else {
                        return false;
                    }
                }
            default:
                return Input.GetButton(input_str);
        }
        
    }
    public bool GetButtonUp(string input_str, HandSide hand) {
        switch (input_str) {
            case "Slide Lock":
                return VRInputController.instance.GunInteractUp(hand);
            case "Pull Back Slide":
                return VRInputController.instance.ActionPressUp(hand);//Make more specific and 3D
            case "Hammer":
                return VRInputController.instance.GunInteract3Up(hand);
            default:
                return Input.GetButtonUp(input_str);
        }
        
    }
    public float GetAxis(string input_str, HandSide hand) {
        switch (input_str) {
            case "Mouse ScrollWheel":
                return VRInputController.instance.GetSpinSpeed(hand) * 10f;
            default:
                return Input.GetAxis(input_str);
        }
        
    }
}
