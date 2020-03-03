using UnityEngine;

[System.Serializable]
public class CylinderState {
    public GameObject game_object = null;
    public bool can_fire = false;
    public float seated = 0.0f;
    public bool falling = false;
};

[GunDataAttribute(GunAspect.REVOLVER_CYLINDER)]
public class RevolverCylinderComponent : GunComponent {
    public AudioClip[] sound_cylinder_rotate = new AudioClip[0];
    
    internal Predicates is_closed_predicates = new Predicates();
    public bool is_closed => is_closed_predicates.AllTrue();

    internal Predicates can_manual_rotate_predicates = new Predicates();
    public bool can_manual_rotate => can_manual_rotate_predicates.AllTrue();
    public bool rotateable = true;

    public bool slide_cycling = false;
    public bool hammer_cycling = true;

    [IsNonNull, HasTransformPath("extractor_rod")] public Transform chamber_parent; // We look for the point_rounds in here

    [IsNonNull] public GameObject empty_casing;
    [IsNonNull] public GameObject full_casing;

    public int cylinder_capacity = 6;
    [Range(0f, 1f)] public float seating_min = 0;
    [Range(0f, 1f)] public float seating_max = 1f;
    [Range(0f, 1f)] public float seating_firebonus_min = 0;
    [Range(0f, 1f)] public float seating_firebonus_max = 0.5f;

    internal Transform[] chambers;

    internal float cylinder_rotation = 0.0f;
    internal float cylinder_rotation_vel = 0.0f;
    internal int active_cylinder = 0;
    internal int target_cylinder_offset = 0;

    internal CylinderState[] cylinders;
}