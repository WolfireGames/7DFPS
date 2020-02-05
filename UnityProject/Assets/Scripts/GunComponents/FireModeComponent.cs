using UnityEngine;

public enum AutoModStage { ENABLED, DISABLED };

[GunDataAttribute(GunAspect.FIRE_MODE)]
public class FireModeComponent : GunComponent {
    public AudioClip[] sound_firemode_toggle = new AudioClip[0];

    internal AutoModStage auto_mod_stage = AutoModStage.DISABLED;
    internal float auto_mod_amount = 0.0f;
}