using UnityEngine;

namespace GunSystemsV1 {
    public enum Thumb { ON_HAMMER, OFF_HAMMER, SLOW_LOWERING, TRIGGER_PULLED };

    [GunDataAttribute(GunAspect.HAMMER)]
    public class HammerComponent : GunComponent {
        public AudioClip[] sound_hammer_cock = new AudioClip[0];
        public AudioClip[] sound_hammer_decock = new AudioClip[0];
        public AudioClip[] sound_hammer_strike = new AudioClip[0];

        internal Predicates is_blocked_predicates = new Predicates();
        public bool is_blocked => !is_blocked_predicates.AllFalse();

        internal Thumb thumb_on_hammer = Thumb.OFF_HAMMER;
        internal float prev_hammer_cocked = 0.0f;
        internal float hammer_cocked = 0.0f;
    }
}