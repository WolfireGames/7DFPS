using UnityEngine;

[GunDataAttribute(GunAspect.GRIP_SAFETY)]
public class GripSafetyComponent : GunComponent {
    [IsNonNull, HasTransformPath("grip safety")] public Transform grip_safety;
    [IsNonNull, HasTransformPath("point_grip_safety_off")] public Transform point_grip_safety_off;

    public bool block_trigger = true;
    public bool block_slide = false;

    internal bool is_safe = false;
    internal Vector3 safety_rel_pos;
    internal Quaternion safety_rel_rot;
    internal float safety_off = 1.0f;
}