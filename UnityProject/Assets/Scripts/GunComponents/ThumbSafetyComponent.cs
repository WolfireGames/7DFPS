using UnityEngine;

[GunDataAttribute(GunAspect.THUMB_SAFETY)]
public class ThumbSafetyComponent : GunComponent {
    public AudioClip[] sound_safety = new AudioClip[0];

    [IsNonNull, HasTransformPath("safety")] public Transform safety;
    [IsNonNull, HasTransformPath("point_safety_off")] public Transform point_safety_off;

    public bool block_trigger = true;
    public bool block_slide = true;

    internal bool is_safe = false;
    internal Vector3 safety_rel_pos;
    internal Quaternion safety_rel_rot;
    internal float safety_off = 1.0f;
}