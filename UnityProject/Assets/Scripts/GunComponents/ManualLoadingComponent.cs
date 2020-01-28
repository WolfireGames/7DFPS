using UnityEngine;

[GunDataAttribute(GunAspect.MANUAL_LOADING)]
public class ManualLoadingComponent : GunComponent {
    public AudioClip[] sound_round_insertion = new AudioClip[0];

    internal Predicates can_insert_predicates = new Predicates();
    public bool can_insert => can_insert_predicates.AllTrue();

    public bool mag_insert = false;
    public bool load_when_closed = false;
}