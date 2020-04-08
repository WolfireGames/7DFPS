using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRInventoryManager : MonoBehaviour
{
    public static VRInventoryManager instance;
    public Transform[] slots;
    public Transform TapePL, HolstTrans, PlayerRotate;

    public Material DefaultMat, HighlightMat;

    public int ActiveSlot;

    public int HighlightSlot = -1;

    int holsterMult = 1;

    public bool TapePlayer, Holster;

    float DefaultX;

    private void Awake() {
        instance = this;
        DefaultX = HolstTrans.localPosition.x;
    }

    void Start()
    {
        float slotWidth = 360 / slots.Length-1;
        for (int i = 0; i <= slots.Length-1; i++) {
            float maxAngle = slotWidth * i;
            float minAngle;
            if (i == 0) {
                minAngle = -slotWidth;
            }
            else if (i == slots.Length) {
                minAngle = 360 - slotWidth;
            }
            else {
                minAngle = slotWidth * (i - 1);
            }
            float slotAngle = (minAngle + maxAngle) / 2;
            slots[i].localPosition = positionSlot(slotAngle, 0.25f);
        }
    }

    Vector3 positionSlot(float angle, float radius) {
        angle = Mathf.Deg2Rad * angle;
        Vector3 slotPos = (radius * transform.forward * Mathf.Cos(angle)) + (radius * transform.right * Mathf.Sin(angle));
        return slotPos;
    }

    public void SetHighlightSlot(int num) {
        HighlightSlot = num;
        if(num != -1) {
            slots[num].GetComponent<Renderer>().material = HighlightMat;
        }
        else {
            for (int i = 0; i <= slots.Length - 1; i++) {
                slots[i].GetComponent<Renderer>().material = DefaultMat;
            }
        }
    }

    void Update()
    {
        if(VRInputBridge.instance.aimScript_ref == null) {
            return;
        }

        Vector3 flattenedForward = VRInputController.instance.Head.transform.forward;
        flattenedForward.y = 0f;
        PlayerRotate.rotation = Quaternion.LookRotation(flattenedForward, Vector3.up);

        for (int i = 0; i < slots.Length-1; i++) {
            if (VRInputBridge.instance.aimScript_ref.primaryHand == HandSide.Right) {
                if (Vector3.Distance(VRInputController.instance.LeftHand.transform.position, slots[i].position) < 0.05f) {
                    ActiveSlot = i;
                    break;
                }
                else {
                    ActiveSlot = -3;
                }
            }
            else {
                if (Vector3.Distance(VRInputController.instance.RightHand.transform.position, slots[i].position) < 0.05f) {
                    ActiveSlot = i;
                    break;
                }
                else {
                    ActiveSlot = -3;
                }
            }
        }

        if (VRInputBridge.instance.aimScript_ref.primaryHand == HandSide.Right) {
            if (Vector3.Distance(VRInputController.instance.LeftHand.transform.position, TapePL.position) < 0.05f) {
                TapePlayer = true;
            }
            else {
                TapePlayer = false;
            }
        }
        else {
            if (Vector3.Distance(VRInputController.instance.RightHand.transform.position, TapePL.position) < 0.05f) {
                TapePlayer = true;
            }
            else {
                TapePlayer = false;
            }
        }

        if (PlayerPrefs.GetInt("alt_holster", 0) == 1) {
            holsterMult = -1;
        }
        else {
            holsterMult = 1;
        }

        if (VRInputBridge.instance.aimScript_ref.primaryHand == HandSide.Right) {
            if (Vector3.Distance(VRInputController.instance.RightHand.transform.position, HolstTrans.position) < 0.075f) {
                Holster = true;
            }
            else {
                Holster = false;
            }

            HolstTrans.localPosition = new Vector3(DefaultX * holsterMult, 0.13f, 0);
        }
        else {
            if (Vector3.Distance(VRInputController.instance.LeftHand.transform.position, HolstTrans.position) < 0.075f) {
                Holster = true;
            }
            else {
                Holster = false;
            }

            HolstTrans.localPosition = new Vector3(-DefaultX * holsterMult, 0.13f, 0);
        }
    }
}
