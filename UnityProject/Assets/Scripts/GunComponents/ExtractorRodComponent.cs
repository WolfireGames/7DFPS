using UnityEngine;

public enum ExtractorRodStage { CLOSED, OPENING, OPEN, CLOSING };

[GunDataAttribute(GunAspect.EXTRACTOR_ROD)]
public class ExtractorRodComponent : GunComponent {
    public AudioClip[] sound_extractor_rod_open = new AudioClip[0];
    public AudioClip[] sound_extractor_rod_close = new AudioClip[0];

    [Tooltip("Which chamber should be ejected? Negative values cause every chamber to eject!")]
    public int chamber_offset = -1;

    internal Predicates can_extract_predicates = new Predicates();
    public bool can_extract => can_extract_predicates.AllTrue();

    internal ExtractorRodStage extractor_rod_stage = ExtractorRodStage.CLOSED;
    internal float extractor_rod_amount = 0.0f;
    internal bool extracted = false;
}