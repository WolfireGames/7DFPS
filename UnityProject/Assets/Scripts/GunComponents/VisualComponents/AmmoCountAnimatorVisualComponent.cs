using UnityEngine;

[GunDataAttribute(GunAspect.AMMO_COUNT_ANIMATOR_VISUAL)]
public class AmmoCountAnimatorVisualComponent : GunComponent {
    [IsNonNull] public Animator animator;
}