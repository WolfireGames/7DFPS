using UnityEngine;

namespace GunSystemsV1 {
    [GunDataAttribute(GunAspect.GRIP_SAFETY_VISUAL)]
    public class GripSafetyVisualComponent : GunComponent {
        [IsNonNull, HasTransformPath("grip safety")] public Transform grip_safety;
        [IsNonNull, HasTransformPath("point_grip_safety_off")] public Transform point_grip_safety_off;

        internal Vector3 rel_pos;
        internal Quaternion rel_rot;
    }
}