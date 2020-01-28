using UnityEngine;

public enum AutoModStage { ENABLED, DISABLED };

[GunDataAttribute(GunAspect.FIRE_MODE)]
public class FireModeComponent : GunComponent {
    public AudioClip[] sound_firemode_toggle = new AudioClip[0];

    [IsNonNull, HasTransformPath("auto mod toggle")] public Transform auto_mod_toggle;
    [IsNonNull, HasTransformPath("point_auto_mod_enabled")] public Transform point_auto_mod_enabled;

    internal AutoModStage auto_mod_stage = AutoModStage.DISABLED;
    internal float auto_mod_amount = 0.0f;
    internal Vector3 auto_mod_rel_pos;
    internal Quaternion auto_mod_rel_rot;
}