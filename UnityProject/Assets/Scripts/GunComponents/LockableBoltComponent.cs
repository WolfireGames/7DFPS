using UnityEngine;

public enum BoltActionStage {LOCKING, UNLOCKING, UNLOCKED, LOCKED}

[GunData(GunAspect.LOCKABLE_BOLT)]
public class LockableBoltComponent : GunComponent {
    public AudioClip[] sound_rifle_bolt_unlock = new AudioClip[0];
    public AudioClip[] sound_rifle_bolt_lock = new AudioClip[0];

    [IsNonNull, HasTransformPath("bolt")] public Transform bolt;
    [IsNonNull, HasTransformPath("point_bolt_unlocked")] public Transform point_bolt_unlocked;

    public float toggle_speed = 7f;

    internal Predicates block_toggle_predicates = new Predicates();
    internal bool block_toggle => !block_toggle_predicates.AllFalse();

    internal BoltActionStage bolt_stage = BoltActionStage.LOCKED;
    internal float bolt_rotation_lock_amount = 1.0f;
    internal Quaternion bolt_unlocked_rot;
    internal Quaternion bolt_locked_rot;
}