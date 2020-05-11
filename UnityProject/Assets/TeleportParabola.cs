using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportParabola : MonoBehaviour
{
    public HandSide side;

    public LineRenderer line;

    public float startVelocity = 10f;

    public Vector3 gravity = new Vector3(0,-9.81f,0);

    public int parabolaResolution;

    public List<Vector3> parabolaPoints = new List<Vector3>();

    public bool VRTeleportEnabled;

    bool Teleported, CanTeleport;

    float LastHitPoint = 0f;

    public TeleportCircleColor circle;

    public AudioSource StepPlay;

    public void Start()
    {
        Teleported = true;
       if (PlayerPrefs.GetInt("vr_teleport") == 1) {
            VRTeleportEnabled = true;
        }
    }

    private static float ParabolicCurve2D(float p0, float v0, float a, float t) {
        return p0 + v0 * t + 0.5f * a * t * t;
    }

    private static Vector3 ParabolicCurve3D(Vector3 p0, Vector3 v0, Vector3 a, float t) {
        Vector3 ret = new Vector3();
        for (int x = 0; x < 3; x++)
            ret[x] = ParabolicCurve2D(p0[x], v0[x], a[x], t);
        return ret;
    }

    void FixedUpdate()
    {
        if (VRTeleportEnabled) {
            Vector3 clampedYVel = transform.forward;

            clampedYVel.y = Mathf.Clamp(clampedYVel.y, -1f, 0.5f);

            if (VRInputBridge.instance.aimScript_ref.secondaryHand == side) {
                if (VRInputController.instance.GetWalkVector(VRInputBridge.instance.aimScript_ref.primaryHand).magnitude > 0.5f && VRInputBridge.instance.aimScript_ref.grounded) {
                    Teleported = false;
                    bool HitSomething = false;
                    for (int i = 0; i < parabolaResolution; i++) {
                        if (!HitSomething) {
                            Vector3 currentParabolaPoint = ParabolicCurve3D(transform.position, clampedYVel * startVelocity, gravity, (i + 0f) / parabolaResolution);

                            if (i > 0) {
                                Vector3 lastParabolaPoint = parabolaPoints[parabolaPoints.Count - 1];

                                RaycastHit hit = new RaycastHit();

                                if (Physics.Linecast(lastParabolaPoint, currentParabolaPoint, out hit)) {
                                    currentParabolaPoint = hit.point;
                                    circle.transform.position = hit.point;
                                    circle.transform.rotation = Quaternion.LookRotation(hit.normal);

                                    float heightDifference = transform.position.y - hit.point.y;

                                    CanTeleport = (hit.normal.y > 0.6f) && Mathf.Abs(heightDifference) < 2.5f && !Physics.Raycast(circle.transform.position,circle.transform.forward,VRInputController.instance.Head.transform.localPosition.y*0.8f);
                                    circle.Teleportable(CanTeleport);
                                    
                                    HitSomething = true;
                                    circle.gameObject.SetActive(true);
                                }
                                else {
                                    CanTeleport = false;
                                    circle.gameObject.SetActive(false);
                                }
                            }

                            parabolaPoints.Add(currentParabolaPoint);
                            LastHitPoint = (i + 0f) / parabolaResolution;
                        }
                        else {
                            parabolaPoints.Add(parabolaPoints[parabolaPoints.Count - 1]);
                        }
                        if(i == parabolaResolution && !HitSomething) {
                            circle.gameObject.SetActive(false);
                        }
                    }
                    line.SetPositions(parabolaPoints.ToArray());
                    parabolaPoints.Clear();
                    line.enabled = true;
                    
                }
                else {
                    parabolaPoints.Clear();
                    line.enabled = false;
                    circle.gameObject.SetActive(false);
                    if (!Teleported && CanTeleport) {
                        VRInputBridge.instance.aimScript_ref.TeleportTo(ParabolicCurve3D(transform.position, clampedYVel * startVelocity, gravity, LastHitPoint) - VRInputBridge.instance.aimScript_ref.transform.position);
                        Teleported = true;
                        StepPlay.Play();
                    } 
                }
            }
            
            if (PlayerPrefs.GetInt("vr_teleport") == 0) {
                VRTeleportEnabled = false;
                VRInputBridge.instance.aimScript_ref.RecheckTeleportEnabled();
            }
        }
        else {
            circle.gameObject.SetActive(false);
            line.enabled = false;
            if (VRInputBridge.instance.aimScript_ref.secondaryHand == side && PlayerPrefs.GetInt("vr_teleport") == 1) {
                VRTeleportEnabled = true;
                VRInputBridge.instance.aimScript_ref.RecheckTeleportEnabled();
            }
        }
    }
}
