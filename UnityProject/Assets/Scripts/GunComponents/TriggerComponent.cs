[GunDataAttribute(GunAspect.TRIGGER)]
public class TriggerComponent : GunComponent {
    internal Predicates trigger_pressable_predicates = new Predicates();
    internal bool trigger_pressable => trigger_pressable_predicates.AllTrue();

    internal bool is_connected = true;
    internal bool pressure_on_trigger = false;
    internal float old_trigger_pressed = 0.0f;
    internal float trigger_pressed = 0.0f;
    internal int trigger_cycle = 0; // Variable use to keep track of how often the trigger cycles for fire modes

    public FireMode fire_mode = FireMode.SINGLE;
}