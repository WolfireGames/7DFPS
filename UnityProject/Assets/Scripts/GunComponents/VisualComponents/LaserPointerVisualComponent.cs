using UnityEngine;

[GunDataAttribute(GunAspect.LASER_POINTER_VISUAL)]
public class LaserPointerVisualComponent : GunComponent {
    [IsNonNull, HasTransformPath("point_laser_origin")] public Transform point_laser_origin;
    public GameObject laser_point;
}