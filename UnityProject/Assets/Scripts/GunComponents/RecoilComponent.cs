using UnityEngine;

[GunDataAttribute(GunAspect.RECOIL)]
public class RecoilComponent : GunComponent {
    internal int old_fire_count = 0;
    internal bool add_head_recoil = false;
    internal float recoil_transfer_x = 0.0f;
    internal float recoil_transfer_y = 0.0f;
    internal float rotation_transfer_x = 0.0f;
    internal float rotation_transfer_y = 0.0f;
}