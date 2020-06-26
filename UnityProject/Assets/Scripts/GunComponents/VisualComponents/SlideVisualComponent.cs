using UnityEngine;

namespace GunSystemsV1 {
    [GunDataAttribute(GunAspect.SLIDE_VISUAL)]
    public class SlideVisualComponent : GunComponent {
        [IsNonNull, HasTransformPath("point_slide_start")] public Transform point_slide_start;
        [IsNonNull, HasTransformPath("point_slide_end")] public Transform point_slide_end;
        [IsNonNull, HasTransformPath("slide")] public Transform slide;
    }
}