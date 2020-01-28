using UnityEngine;

[GunDataAttribute(GunAspect.ALTERNATIVE_STANCE)]
public class AlternativeStanceComponent : GunComponent {
    public bool stance_blocks_trigger = false;
    public bool stance_blocks_slide = false;
    public bool stance_blocks_bolt = false;
    public bool stance_blocks_mag = false;

    public bool alt_stance_blocks_trigger = false;
    public bool alt_stance_blocks_slide = false;
    public bool alt_stance_blocks_bolt = false;
    public bool alt_stance_blocks_mag = false;

    internal bool is_alternative = false;
}