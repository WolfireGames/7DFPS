using UnityEngine;

namespace GunSystemsV1 {
    public enum MagStage { OUT, INSERTING, IN, REMOVING };

    [GunDataAttribute(GunAspect.MAGAZINE)]
    public class MagazineComponent : GunComponent {
        internal mag_script mag_script;

        internal bool ready_to_remove_mag = false;
        internal MagStage mag_stage = MagStage.IN;
        internal float mag_seated = 1.0f;
    }
}