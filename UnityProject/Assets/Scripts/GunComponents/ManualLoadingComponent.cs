using UnityEngine;

[GunDataAttribute(GunAspect.MANUAL_LOADING)]
public class ManualLoadingComponent : GunComponent {
    public AudioClip[] sound_round_insertion = new AudioClip[0];

    internal Predicates can_insert_predicates = new Predicates();
    public bool can_insert => can_insert_predicates.AllTrue();

    public bool mag_insert = false;
    public bool load_when_closed = false;

    [Tooltip("Which chambers can not be accessed for loading? This is useful for revolvers with only one open space to refill rounds.")]
    public int[] inaccessabile_chamber_offsets = new int[0];
}