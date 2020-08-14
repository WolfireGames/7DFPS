using UnityEngine;

[GunDataAttribute(GunAspect.THUMB_SAFETY_VISUAL)]
public class ThumbSafetyVisualComponent : GunComponent {
    [IsNonNull, HasTransformPath("safety")] public Transform safety;
    [IsNonNull, HasTransformPath("point_safety_off")] public Transform point_safety_off;
    
    internal Vector3 rel_pos;
    internal Quaternion rel_rot;
}