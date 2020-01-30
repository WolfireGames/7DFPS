using UnityEngine;

[GunDataAttribute(GunAspect.EXTRACTOR_ROD_VISUAL)]
public class ExtractorRodVisualComponent : GunComponent {
    [IsNonNull, HasTransformPath("extractor_rod")] public Transform extractor_rod;
    [IsNonNull, HasTransformPath("point_extractor_rod_extended")] public Transform point_extractor_rod_extended;
    
    internal Vector3 extractor_rod_rel_pos;
    internal Quaternion extractor_rod_rel_rot;
}