using UnityEngine;

namespace GunSystemsV1 {
    [GunDataAttribute(GunAspect.YOKE_VISUAL)]
    public class YokeVisualComponent : GunComponent {
        [IsNonNull, HasTransformPath("yoke")] public Transform yoke;
        [IsNonNull, HasTransformPath("yoke_pivot")] public Transform yoke_pivot;
        [IsNonNull, HasTransformPath("point_yoke_pivot_open")] public Transform point_yoke_pivot_open;
        
        internal Vector3 yoke_pivot_rel_pos;
        internal Quaternion yoke_pivot_rel_rot;
    }
}