using UnityEngine;

namespace GunSystemsV1 {
    [GunDataAttribute(GunAspect.SLIDE_SPRING_VISUAL)]
    public class SlideSpringVisualComponent : GunComponent {
        [IsNonNull, HasTransformPath("point_recoil_spring_compressed")] public Transform point_recoil_spring_compressed;
        [IsNonNull, HasTransformPath("recoil_spring")] public Transform recoil_spring;

        internal Vector3 rel_pos;
        internal Quaternion rel_rot;
        internal Vector3 rel_scale;
    }
}