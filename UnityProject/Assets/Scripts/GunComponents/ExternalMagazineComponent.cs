using UnityEngine;

namespace GunSystemsV1 {
    [GunDataAttribute(GunAspect.EXTERNAL_MAGAZINE)]
    public class ExternalMagazineComponent : GunComponent {
        public AudioClip[] sound_mag_eject_button = new AudioClip[0];
        public AudioClip[] sound_mag_ejection = new AudioClip[0];
        public AudioClip[] sound_mag_insertion = new AudioClip[0];

        [IsNonNull, HasTransformPath("point_mag_inserted")] public Transform point_mag_inserted;
        [IsNonNull, HasTransformPath("point_mag_to_insert")] public Transform point_mag_to_insert;

        [IsNonNull] public GameObject magazine_obj;

        internal Predicates can_eject_predicates = new Predicates();
        public bool can_eject => can_eject_predicates.AllTrue();
    }
}