using UnityEngine;

namespace GunSystemsV1 {
    [GunDataAttribute(GunAspect.GRIP_SAFETY)]
    public class GripSafetyComponent : GunComponent {
        public bool block_trigger = true;
        public bool block_slide = false;

        internal bool is_safe = false;
        internal float safety_off = 1.0f;
    }
}