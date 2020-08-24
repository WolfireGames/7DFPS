using UnityEngine;

public enum SlideStage { NOTHING, PULLBACK, HOLD };

[GunDataAttribute(GunAspect.SLIDE)]
public class SlideComponent : GunComponent {
    public AudioClip[] sound_slide_back = new AudioClip[0];
    public AudioClip[] sound_slide_front = new AudioClip[0];
    public AudioClip[] sound_bullet_eject = new AudioClip[0];
    
    internal Predicates block_slide_pull_predicates = new Predicates();
    internal bool block_slide_pull => !block_slide_pull_predicates.AllFalse();

    internal Predicates should_slide_lock_predicates = new Predicates();
    internal bool should_slide_lock => !should_slide_lock_predicates.AllFalse();

    public bool eject_round = true; // Should a chambered round be ejected when the slide is pulled?
    public bool chamber_round = true; // Should a round be chambered when the slide closes?

    public bool chamber_on_pull = false; // Do we start chambering the round when the slide springs back, or when it pulls back?
    public float slide_chambering_position = 0.7f; // Start chambering at this slide amount

    public float slide_lock_position = 0.9f;
    public float press_check_position = 0.4f;
    public float slide_lock_speed = 20.0f;

    public float slide_cock_position = 0.3f; // At what position does the slide fully cock the firing mechanism

    public bool kick_slide_back = true;
    public bool release_slide_automatically = false; // If we pull the slide all the way back: release it!

    internal int prev_fire_count = 0;
    internal float old_slide_amount = 0.0f;
    internal float slide_amount = 0.0f;
    internal bool slide_lock = false;
    internal SlideStage slide_stage = SlideStage.NOTHING;
}