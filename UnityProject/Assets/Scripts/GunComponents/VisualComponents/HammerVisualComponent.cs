using UnityEngine;

namespace GunSystemsV1 {
    [GunDataAttribute(GunAspect.HAMMER_VISUAL)]
    public class HammerVisualComponent : GunComponent {
        internal Vector3 hammer_rel_pos;
        internal Quaternion hammer_rel_rot;

        [IsNonNull, HasTransformPath("hammer_pivot")] public Transform hammer;
        [IsNonNull, HasTransformPath("point_hammer_cocked")] public Transform point_hammer_cocked;
    }
}