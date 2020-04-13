using UnityEngine;
using ExtentionUtil;
using System.Linq;

namespace GunSystemsV1 {
    [InclusiveAspects(GunAspect.SLIDE_LOCK_VISUAL)]
    public class SlideLockVisualSystem : GunSystemBase {
        SlideLockVisualComponent slvc;

        public override void Initialize() {
            slvc = gs.GetComponent<SlideLockVisualComponent>();
        
            slvc.rel_pos = slvc.slide_lock.localPosition;
            slvc.rel_rot = slvc.slide_lock.localRotation;
        }

        public override void Update() {
            slvc.state = Mathf.Max(gs.IsSlideLocked() ? 1f : 0f, slvc.state - Time.deltaTime * 10.0f);

            slvc.slide_lock.LerpPosition(slvc.rel_pos, slvc.point_slide_locked, slvc.state);
            slvc.slide_lock.LerpRotation(slvc.rel_rot, slvc.point_slide_locked, slvc.state);
        }
    }

    [InclusiveAspects(GunAspect.HAMMER_VISUAL, GunAspect.HAMMER)]
    public class HammerVisualSystem : GunSystemBase {
        HammerVisualComponent hvc;
        HammerComponent hc;

        public override void Initialize() {
            hc = gs.GetComponent<HammerComponent>();
            hvc = gs.GetComponent<HammerVisualComponent>();

            hvc.hammer_rel_pos = hvc.hammer.localPosition;
            hvc.hammer_rel_rot = hvc.hammer.localRotation;
        }

        public override void Update() {
            hvc.hammer.LerpPosition(hvc.hammer_rel_pos, hvc.point_hammer_cocked, hc.hammer_cocked);
            hvc.hammer.LerpRotation(hvc.hammer_rel_rot, hvc.point_hammer_cocked, hc.hammer_cocked);
        }
    }

    [InclusiveAspects(GunAspect.TRIGGER_VISUAL, GunAspect.TRIGGER)]
    public class TriggerVisualSystem : GunSystemBase {
        TriggerVisualComponent tvc;
        TriggerComponent tc;

        public override void Initialize() {
            tc = gs.GetComponent<TriggerComponent>();
            tvc = gs.GetComponent<TriggerVisualComponent>();

            tvc.trigger_rel_pos = tvc.trigger.localPosition;
            tvc.trigger_rel_rot = tvc.trigger.localRotation;
        }

        public override void Update() {
            tvc.trigger.LerpPosition(tvc.trigger_rel_pos, tvc.point_trigger_pulled, tc.trigger_pressed);
            tvc.trigger.LerpRotation(tvc.trigger_rel_rot, tvc.point_trigger_pulled, tc.trigger_pressed);
        }
    }

    [InclusiveAspects(GunAspect.SLIDE, GunAspect.SLIDE_VISUAL)]
    public class SlideVisualSystem : GunSystemBase {
        SlideComponent psc;
        SlideVisualComponent svc;

        public override void Initialize() {
            psc = gs.GetComponent<SlideComponent>();
            svc = gs.GetComponent<SlideVisualComponent>();
        }

        public override void Update() {
            svc.slide.LerpPosition(svc.point_slide_start, svc.point_slide_end, psc.slide_amount);
        }
    }

    [InclusiveAspects(GunAspect.LASER_POINTER_VISUAL)]
    public class LaserPointerVisualSystem : GunSystemBase {
        LaserPointerVisualComponent lpvc;
        GameObject point_object;

        public override void Initialize() {
            lpvc = gs.GetComponent<LaserPointerVisualComponent>();
            point_object = GameObject.Instantiate(lpvc.laser_point, lpvc.point_laser_origin.transform);
        }

        public override void Update() {
            RaycastHit hit;
            if(Physics.Raycast(lpvc.point_laser_origin.position, lpvc.point_laser_origin.transform.forward, out hit, 50, 1<<0 | 1<<8 | 1<<11)) {
                point_object.SetActive(true);
                point_object.transform.position = hit.point;
                point_object.transform.rotation = Quaternion.LookRotation(hit.normal);
            } else {
                point_object.SetActive(false);
            }
        }
    }

    [InclusiveAspects(GunAspect.SLIDE, GunAspect.SLIDE_SPRING_VISUAL)]
    public class SlideSpringVisualSystem : GunSystemBase {
        SlideComponent psc;
        SlideSpringVisualComponent ssvc;

        public override void Initialize() {
            psc = gs.GetComponent<SlideComponent>();
            ssvc = gs.GetComponent<SlideSpringVisualComponent>();

            ssvc.rel_pos = ssvc.recoil_spring.localPosition;
            ssvc.rel_rot = ssvc.recoil_spring.localRotation;
            ssvc.rel_scale = ssvc.recoil_spring.localScale;
        }
        
        public override void Update() {
            ssvc.recoil_spring.LerpPosition(ssvc.rel_pos, ssvc.point_recoil_spring_compressed, psc.slide_amount);
            ssvc.recoil_spring.LerpRotation(ssvc.rel_rot, ssvc.point_recoil_spring_compressed, psc.slide_amount);
            ssvc.recoil_spring.LerpScale(ssvc.rel_scale, ssvc.point_recoil_spring_compressed, psc.slide_amount);
        }
    }

    [InclusiveAspects(GunAspect.EXTRACTOR_ROD, GunAspect.EXTRACTOR_ROD_VISUAL)]
    public class ExtractorRodSystem : GunSystemBase {
        ExtractorRodVisualComponent ervc;
        ExtractorRodComponent erc;

        public override void Initialize() {
            ervc = gs.GetComponent<ExtractorRodVisualComponent>();
            erc = gs.GetComponent<ExtractorRodComponent>();

            ervc.extractor_rod_rel_pos = ervc.extractor_rod.localPosition;
            ervc.extractor_rod_rel_rot = ervc.extractor_rod.localRotation;
        }

        public override void Update() {
            ervc.extractor_rod.LerpPosition(ervc.extractor_rod_rel_pos, ervc.point_extractor_rod_extended, erc.extractor_rod_amount);
            ervc.extractor_rod.LerpRotation(ervc.extractor_rod_rel_rot, ervc.point_extractor_rod_extended, erc.extractor_rod_amount);
        }
    }

    [InclusiveAspects(GunAspect.REVOLVER_CYLINDER, GunAspect.CYLINDER_VISUAL)]
    public class CylinderVisualSystem : GunSystemBase {
        CylinderVisualComponent cvc;
        RevolverCylinderComponent rcc;

        public override void Initialize() {
            cvc = gs.GetComponent<CylinderVisualComponent>();
            rcc = gs.GetComponent<RevolverCylinderComponent>();
        }

        public override void Update() {
            if(rcc.rotateable) {
                var tmp_cs1 = cvc.cylinder_assembly.localRotation;
                var tmp_cs2 = tmp_cs1.eulerAngles;
                tmp_cs2.z = rcc.cylinder_rotation;

                tmp_cs1.eulerAngles = tmp_cs2;
                cvc.cylinder_assembly.localRotation = tmp_cs1;
            }
        }
    }

    [InclusiveAspects(GunAspect.YOKE, GunAspect.YOKE_VISUAL)]
    public class YokeVisualSystem : GunSystemBase {
        YokeVisualComponent yvc;
        YokeComponent yc;

        public override void Initialize() {
            yvc = gs.GetComponent<YokeVisualComponent>();
            yc = gs.GetComponent<YokeComponent>();

            yvc.yoke_pivot_rel_pos = yvc.yoke_pivot.localPosition;
            yvc.yoke_pivot_rel_rot = yvc.yoke_pivot.localRotation;
        }

        public override void Update() {
            yvc.yoke_pivot.LerpPosition(yvc.yoke_pivot_rel_pos, yvc.point_yoke_pivot_open, yc.yoke_open);
            yvc.yoke_pivot.LerpRotation(yvc.yoke_pivot_rel_rot, yvc.point_yoke_pivot_open, yc.yoke_open);
        }
    }

    [InclusiveAspects(GunAspect.FIRE_MODE, GunAspect.FIRE_MODE_VISUAL)]
    public class FireModeVisualSystem : GunSystemBase {
        FireModeComponent fmc;
        FireModeVisualComponent fmvc;

        public override void Initialize() {
            fmc = gs.GetComponent<FireModeComponent>();
            fmvc = gs.GetComponent<FireModeVisualComponent>();

            fmvc.rel_pos = fmvc.fire_mode_toggle.localPosition;
            fmvc.rel_rot = fmvc.fire_mode_toggle.localRotation;
        }

        public override void Update() {
            fmvc.fire_mode_toggle.LerpPosition(fmvc.rel_pos, fmvc.point_fire_mode_enabled, fmc.fire_mode_amount);
            fmvc.fire_mode_toggle.LerpRotation(fmvc.rel_rot, fmvc.point_fire_mode_enabled, fmc.fire_mode_amount);
        }
    }

    [InclusiveAspects(GunAspect.THUMB_SAFETY, GunAspect.THUMB_SAFETY_VISUAL)]
    public class ThumbSafetyVisualSystem : GunSystemBase {
        ThumbSafetyComponent tsc;
        ThumbSafetyVisualComponent tsvc;

        public override void Initialize() {
            tsc = gs.GetComponent<ThumbSafetyComponent>();
            tsvc = gs.GetComponent<ThumbSafetyVisualComponent>();

            tsvc.rel_pos = tsvc.safety.localPosition;
            tsvc.rel_rot = tsvc.safety.localRotation;
        }

        public override void Update() {
            tsvc.safety.LerpPosition(tsvc.rel_pos, tsvc.point_safety_off, tsc.safety_off);
            tsvc.safety.LerpRotation(tsvc.rel_rot, tsvc.point_safety_off, tsc.safety_off);
        }
    }

    [InclusiveAspects(GunAspect.GRIP_SAFETY, GunAspect.GRIP_SAFETY_VISUAL)]
    public class GripSafetyVisualSystem : GunSystemBase {
        GripSafetyComponent gsc;
        GripSafetyVisualComponent gsvc;

        public override void Initialize() {
            gsc = gs.GetComponent<GripSafetyComponent>();
            gsvc = gs.GetComponent<GripSafetyVisualComponent>();

            gsvc.rel_pos = gsvc.grip_safety.localPosition;
            gsvc.rel_rot = gsvc.grip_safety.localRotation;
        }

        public override void Update() {
            gsvc.grip_safety.LerpPosition(gsvc.rel_pos, gsvc.point_grip_safety_off, gsc.safety_off);
            gsvc.grip_safety.LerpRotation(gsvc.rel_rot, gsvc.point_grip_safety_off, gsc.safety_off);
        }
    }

    [InclusiveAspects(GunAspect.AMMO_COUNT_ANIMATOR_VISUAL, GunAspect.MAGAZINE)]
    public class AmmoCounterMagazineVisualSystem : GunSystemBase {
        AmmoCountAnimatorVisualComponent acavc;
        MagazineComponent mc;

        public override void Initialize() {
            acavc = gs.GetComponent<AmmoCountAnimatorVisualComponent>();
            mc = gs.GetComponent<MagazineComponent>();
        }

        public override void Update() {
            acavc.animator.SetInteger("rounds_in_mag", mc.mag_script ? mc.mag_script.NumRounds() : -1);
        }
    }

    [InclusiveAspects(GunAspect.AMMO_COUNT_ANIMATOR_VISUAL, GunAspect.REVOLVER_CYLINDER)]
    public class AmmoCounterCylinderVisualSystem : GunSystemBase {
        AmmoCountAnimatorVisualComponent acavc;
        RevolverCylinderComponent rcc;

        public override void Initialize() {
            acavc = gs.GetComponent<AmmoCountAnimatorVisualComponent>();
            rcc = gs.GetComponent<RevolverCylinderComponent>();
        }

        public override void Update() {
            acavc.animator.SetInteger("rounds_in_mag", rcc.cylinders.Count( (cylinder) => cylinder.can_fire ));
        }
    }

    [InclusiveAspects(GunAspect.AMMO_COUNT_ANIMATOR_VISUAL, GunAspect.CHAMBER)]
    public class AmmoCounterChamberVisualSystem : GunSystemBase {
        AmmoCountAnimatorVisualComponent acavc;
        ChamberComponent cc;

        public override void Initialize() {
            acavc = gs.GetComponent<AmmoCountAnimatorVisualComponent>();
            cc = gs.GetComponent<ChamberComponent>();
        }

        public override void Update() {
            acavc.animator.SetBool("round_chambered", cc.active_round_state != RoundState.EMPTY);
        }
    }
}