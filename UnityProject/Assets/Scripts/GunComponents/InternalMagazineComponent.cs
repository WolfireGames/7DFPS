using UnityEngine;

[GunDataAttribute(GunAspect.INTERNAL_MAGAZINE)]
public class InternalMagazineComponent : GunComponent {
    [IsNonNull] public mag_script magazine_script;
}