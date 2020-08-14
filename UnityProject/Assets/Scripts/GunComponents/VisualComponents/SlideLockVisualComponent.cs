using UnityEngine;

[GunDataAttribute(GunAspect.SLIDE_LOCK_VISUAL)]
public class SlideLockVisualComponent : GunComponent {
    internal Vector3 rel_pos;
    internal Quaternion rel_rot;
    internal float state = 0f;

    [IsNonNull, HasTransformPath("slide lock", "slide stop")] public Transform slide_lock;
    [IsNonNull, HasTransformPath("point_slide_stop_locked")] public Transform point_slide_locked;
}