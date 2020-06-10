using UnityEngine;

namespace GunSystemsV1 {
    /// <summary> Use this class to determine initial states for a gun components </summary>
    class Random {
        private static System.Random random = new System.Random();

        public static bool previous_bool;
        public static bool Bool() {
            return previous_bool = random.Next(0, 2) == 0;
        }

        public static float previous_float;
        public static float Float(float min = 0f, float max = 1f) {
            return previous_float = (float)random.NextDouble() * (max - min) + min;
        }

        public static int previous_int;
        public static int Int(int min = 0, int max = 1) {
            return previous_int = random.Next(min, max);
        }
    }

    [InclusiveAspects(GunAspect.ALTERNATIVE_STANCE)]
    [Priority(PriorityAttribute.VERY_EARLY)]
    public class StanceStartSystem : GunSystemBase {
        AlternativeStanceComponent asc;

        public override void Initialize() {
            asc = gs.GetComponent<AlternativeStanceComponent>();

            asc.is_alternative = Random.Bool();
        }
    }

    [InclusiveAspects(GunAspect.YOKE)]
    [Priority(PriorityAttribute.VERY_EARLY)]
    public class YokeStartSystem : GunSystemBase {
        YokeComponent yc;

        public override void Initialize() {
            yc = gs.GetComponent<YokeComponent>();

            // Determine if the cylinder should be open or not
            if(Random.Bool()) {
                yc.yoke_stage = YokeStage.OPEN;
                yc.yoke_open = 1f;
            } else {
                yc.yoke_stage = YokeStage.CLOSED;
                yc.yoke_open = 0f;
            }
        }
    }

    [InclusiveAspects(GunAspect.REVOLVER_CYLINDER)]
    [Priority(PriorityAttribute.VERY_EARLY)]
    public class CylinderStartSystem : GunSystemBase {
        RevolverCylinderComponent rcc;

        public override void Initialize() {
            rcc = gs.GetComponent<RevolverCylinderComponent>();

            // Initialize Cylinder
            rcc.chambers = new Transform[rcc.cylinder_capacity];
            rcc.cylinders = new CylinderState[rcc.cylinder_capacity];
            for (int i = 0; i < rcc.cylinder_capacity; ++i) {
                string name = "point_chamber_" + (i+ 1);
                rcc.chambers[i] = rcc.chamber_parent.Find(name);
                rcc.cylinders[i] = new CylinderState();
            }

            // Load Rounds into cylinder
            for(int i = 0; i < rcc.cylinder_capacity; i++) {
                if(Random.Bool())
                    continue;

                Transform chamber = rcc.chambers[i];
                rcc.cylinders[i].game_object = (GameObject)GameObject.Instantiate(rcc.full_casing,chamber.position, chamber.rotation, chamber);
                rcc.cylinders[i].game_object.transform.localScale = Vector3.one;
                rcc.cylinders[i].can_fire = true;
                rcc.cylinders[i].seated = UnityEngine.Random.Range(rcc.seating_min, rcc.seating_max);
                RemoveChildrenShadows(rcc.cylinders[i].game_object);
            }
        }
    }

    [InclusiveAspects(GunAspect.SLIDE, GunAspect.LOCKABLE_BOLT)]
    [Priority(PriorityAttribute.VERY_EARLY)]
    public class BoltSlideStartSystem : GunSystemBase {
        LockableBoltComponent bc;
        SlideComponent sc;

        public override void Initialize() {
            bc = gs.GetComponent<LockableBoltComponent>();
            sc = gs.GetComponent<SlideComponent>();

            if(Random.Bool()) {
                sc.slide_amount = 1f;
                sc.old_slide_amount = 1f;
                sc.slide_stage = SlideStage.HOLD;

                bc.bolt_stage = BoltActionStage.UNLOCKED;
                bc.bolt_rotation_lock_amount = 0f;
            } else {
                sc.slide_amount = 0f;
                sc.old_slide_amount = 0f;
                sc.slide_stage = SlideStage.NOTHING;

                bc.bolt_stage = BoltActionStage.LOCKED;
                bc.bolt_rotation_lock_amount = 1f;
            }
        }
    }

    [InclusiveAspects(GunAspect.CHAMBER, GunAspect.SLIDE)]
    [ExclusiveAspects(GunAspect.OPEN_BOLT_FIRING)]
    [Priority(PriorityAttribute.VERY_EARLY + 1)]

    public class ChamberWithSliderStartSystem : GunSystemBase {
        ChamberComponent cc;
        SlideComponent sc;

        public override void Initialize() {
            cc = gs.GetComponent<ChamberComponent>();
            sc = gs.GetComponent<SlideComponent>();

            //if(Random.Bool() && !gs.IsSlidePulledBack() && !gs.IsSlideLocked()) { // IsSlidePulledBack and IsSlideLocked are part of uninitialized GunSystems and can't be used here
            if(Random.Bool() && sc.slide_stage == SlideStage.NOTHING && !sc.slide_lock) {
                cc.active_round = GameObject.Instantiate(gs.full_casing, cc.point_chambered_round.position, cc.point_chambered_round.rotation, gs.transform);
                cc.active_round.transform.localScale = Vector3.one;
                cc.active_round_state = RoundState.READY;

                RemoveChildrenShadows(cc.active_round);
            }
        }
    }

    [InclusiveAspects(GunAspect.CHAMBER)]
    [ExclusiveAspects(GunAspect.OPEN_BOLT_FIRING, GunAspect.SLIDE)]
    [Priority(PriorityAttribute.VERY_EARLY + 1)]

    public class ChamberStartSystem : GunSystemBase {
        ChamberComponent cc;

        public override void Initialize() {
            cc = gs.GetComponent<ChamberComponent>();

            if(Random.Bool()) {
                cc.active_round = GameObject.Instantiate(gs.full_casing, cc.point_chambered_round.position, cc.point_chambered_round.rotation, gs.transform);
                cc.active_round.transform.localScale = Vector3.one;
                cc.active_round_state = RoundState.READY;

                RemoveChildrenShadows(cc.active_round);
            }
        }
    }

    [InclusiveAspects(GunAspect.SLIDE, GunAspect.SLIDE_LOCK)]
    [ExclusiveAspects(GunAspect.LOCKABLE_BOLT)]
    [Priority(PriorityAttribute.VERY_EARLY)]
    public class SlideLockStartSystem : GunSystemBase {
        SlideComponent sc;

        public override void Initialize() {
            sc = gs.GetComponent<SlideComponent>();

            if(Random.Bool()) {
                sc.slide_amount = sc.slide_lock_position;
                sc.old_slide_amount = sc.slide_lock_position;
                //sc.slide_stage = SlideStage.HOLD;

                sc.slide_lock = true;
                gs.preferred_tilt = Random.Bool() ? GunTilt.LEFT : GunTilt.RIGHT;
            }
        }
    }

    [InclusiveAspects(GunAspect.HAMMER, GunAspect.INTERNAL_MAGAZINE)]
    [Priority(PriorityAttribute.VERY_EARLY)]
    public class HammerIntMagStartSystem : GunSystemBase {
        HammerComponent hc;

        public override void Initialize() {
            hc = gs.GetComponent<HammerComponent>();

            hc.prev_hammer_cocked = 1f;
            hc.hammer_cocked = 1f;
        }
    }

    [InclusiveAspects(GunAspect.HAMMER)]
    [ExclusiveAspects(GunAspect.INTERNAL_MAGAZINE)]
    [Priority(PriorityAttribute.VERY_EARLY)]
    public class HammerStartSystem : GunSystemBase {
        HammerComponent hc;

        public override void Initialize() {
            hc = gs.GetComponent<HammerComponent>();

            if(Random.Bool()) {
                hc.prev_hammer_cocked = 1f;
                hc.hammer_cocked = 1f;
            }
        }
    }

    [InclusiveAspects(GunAspect.THUMB_SAFETY, GunAspect.SLIDE)]
    [Priority(PriorityAttribute.VERY_EARLY + 1)]
    public class ThumbSafetySlideStartSystem : GunSystemBase {
        ThumbSafetyComponent tsc;
        SlideComponent sc;

        public override void Initialize() {
            tsc = gs.GetComponent<ThumbSafetyComponent>();
            sc = gs.GetComponent<SlideComponent>();

            if(!tsc.block_slide || sc.slide_amount == 0f) {
                if(Random.Bool()) {
                    tsc.is_safe = true;
                    tsc.safety_off = 0f;
                }
            }
        }
    }

    [InclusiveAspects(GunAspect.THUMB_SAFETY)]
    [ExclusiveAspects(GunAspect.SLIDE)]
    [Priority(PriorityAttribute.VERY_EARLY)]
    public class ThumbSafetyStartSystem : GunSystemBase {
        ThumbSafetyComponent tsc;

        public override void Initialize() {
            tsc = gs.GetComponent<ThumbSafetyComponent>();

            if(Random.Bool()) {
                tsc.is_safe = true;
                tsc.safety_off = 0f;
            }
        }
    }

    [InclusiveAspects(GunAspect.FIRE_MODE)]
    [Priority(PriorityAttribute.VERY_EARLY)]
    public class FireModeStartSystem : GunSystemBase {
        FireModeComponent fmc;

        public override void Initialize() {
            fmc = gs.GetComponent<FireModeComponent>();
            fmc.current_fire_mode_index = Random.Int(0, fmc.fire_modes.Length);
        }
    }
}