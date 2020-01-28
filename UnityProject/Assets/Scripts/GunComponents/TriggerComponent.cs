using UnityEngine;

public enum PressureState { NONE, INITIAL, CONTINUING };
public enum FireMode { AUTOMATIC, SINGLE, DISABLED };

[GunDataAttribute(GunAspect.TRIGGER)]
public class TriggerComponent : GunComponent {
    internal Predicates trigger_pressable_predicates = new Predicates();
    internal bool trigger_pressable => trigger_pressable_predicates.AllTrue();

    internal PressureState pressure_on_trigger = PressureState.NONE;
    internal float trigger_pressed = 0.0f;
    internal bool fired_once_this_pull = false;
    public FireMode fire_mode = FireMode.SINGLE;
}