using UnityEngine;

[GunDataAttribute(GunAspect.FIRE_MODE_VISUAL)]
public class FireModeVisualComponent : GunComponent {
    [IsNonNull, HasTransformPath("auto mod toggle")] public Transform fire_mode_toggle;
    [IsNonNull, HasTransformPath("point_auto_mod_enabled")] public Transform point_fire_mode_enabled;
    
    internal Vector3 rel_pos;
    internal Quaternion rel_rot;
}