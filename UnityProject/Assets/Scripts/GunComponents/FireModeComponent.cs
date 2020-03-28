using UnityEngine;

public enum FireMode { AUTOMATIC, SINGLE, DISABLED, BURST_THREE };

[GunDataAttribute(GunAspect.FIRE_MODE)]
public class FireModeComponent : GunComponent {
    public AudioClip[] sound_firemode_toggle = new AudioClip[0];
    public FireMode[] fire_modes = new FireMode[] {FireMode.SINGLE, FireMode.AUTOMATIC};

    internal FireMode current_fire_mode => fire_modes[current_fire_mode_index];
    internal int current_fire_mode_index = 0;
    internal int target_fire_mode_index = 0;

    internal float fire_mode_amount = 0;
}