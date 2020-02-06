using UnityEngine;

[GunDataAttribute(GunAspect.THUMB_SAFETY)]
public class ThumbSafetyComponent : GunComponent {
    public AudioClip[] sound_safety = new AudioClip[0];

    public bool block_trigger = true;
    public bool block_slide = true;

    internal bool is_safe = false;
    internal float safety_off = 1.0f;
}