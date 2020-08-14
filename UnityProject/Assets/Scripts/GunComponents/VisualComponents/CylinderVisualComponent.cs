using UnityEngine;

[GunDataAttribute(GunAspect.CYLINDER_VISUAL)]
public class CylinderVisualComponent : GunComponent {
    [IsNonNull, HasTransformPath("cylinder_assembly")] public Transform cylinder_assembly;
}