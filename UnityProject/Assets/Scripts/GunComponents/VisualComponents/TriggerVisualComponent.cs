using UnityEngine;

[GunDataAttribute(GunAspect.TRIGGER_VISUAL)]
public class TriggerVisualComponent : GunComponent {
    [IsNonNull, HasTransformPath("point_trigger_pulled")] public Transform point_trigger_pulled;
    [IsNonNull, HasTransformPath("trigger")] public Transform trigger;

    internal Vector3 trigger_rel_pos;
    internal Quaternion trigger_rel_rot;
}