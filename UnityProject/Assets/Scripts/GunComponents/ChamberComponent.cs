using UnityEngine;

namespace GunSystemsV1 {
    public enum RoundState { EMPTY, READY, FIRED, LOADING, JAMMED };

    [GunDataAttribute(GunAspect.CHAMBER)]
    public class ChamberComponent : GunComponent {
        [IsNonNull, HasTransformPath("point_chambered_round")] public Transform point_chambered_round;
        [IsNonNull, HasTransformPath("point_load_round")] public Transform point_load_round;

        internal Predicates is_closed_predicates = new Predicates();
        internal bool is_closed => is_closed_predicates.AllTrue();

        internal GameObject active_round = null;
        internal RoundState active_round_state = RoundState.EMPTY;
    }
}