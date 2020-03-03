using UnityEngine;

public enum FireMode { AUTOMATIC, SINGLE, DISABLED };

[GunDataAttribute(GunAspect.TRIGGER)]
public class TriggerComponent : GunComponent {
    internal Predicates trigger_pressable_predicates = new Predicates();
    internal bool trigger_pressable => trigger_pressable_predicates.AllTrue();

    internal bool is_connected = true;
    internal bool pressure_on_trigger = false;
    internal float old_trigger_pressed = 0.0f;
    internal float trigger_pressed = 0.0f;

    public FireMode fire_mode = FireMode.SINGLE;
}