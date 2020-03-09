using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using ExtentionUtil;
using System.Linq;

namespace GunSystemsV1 {
    [InclusiveAspects(GunAspect.MANUAL_LOADING, GunAspect.MAGAZINE)]
    [ExclusiveAspects(GunAspect.REVOLVER_CYLINDER)]
    public class ManualLoadingMagazineSystem : GunSystemBase {
        MagazineComponent mc;
        ManualLoadingComponent mlc;

        private bool InputAddRound() {
            if(!mlc.can_insert) {
                return false;
            }

            // Handel Mag insertion
            if(mlc.mag_insert) {
                if(mc.mag_script.AddRound())
                    return true;
                return false;
            }

            // Attempt to put a round in the chamber
            if(gs.Request(GunSystemRequests.PUT_ROUND_IN_CHAMBER)) {
                gs.PlaySound(mlc.sound_round_insertion);
                return true;
            }

            return false;
        }

        private bool IsAddingRounds() {
            return mlc.can_insert;
        }

        public override Dictionary<GunSystemRequests, GunSystemRequest> GetPossibleRequests() {
            return new Dictionary<GunSystemRequests, GunSystemRequest>() {
                {GunSystemRequests.INPUT_ADD_ROUND, InputAddRound},
            };
        }

        public override Dictionary<GunSystemQueries, GunSystemQuery> GetPossibleQuestions() {
            return new Dictionary<GunSystemQueries, GunSystemQuery>() {
                {GunSystemQueries.IS_ADDING_ROUNDS, IsAddingRounds},
            };
        }

        public override void Initialize() {
            mc = gs.GetComponent<MagazineComponent>();
            mlc = gs.GetComponent<ManualLoadingComponent>();
        }
    }

    [InclusiveAspects(GunAspect.MANUAL_LOADING)]
    [ExclusiveAspects(GunAspect.MAGAZINE, GunAspect.REVOLVER_CYLINDER)]
    public class ManualLoadingSystem : GunSystemBase {
        ManualLoadingComponent mlc;

        public bool AddRound() {
            if(!mlc.can_insert)
                return false;
            return gs.Request(GunSystemRequests.PUT_ROUND_IN_CHAMBER);
        }

        public bool InputAddRound() {
            if(AddRound()) {
                gs.PlaySound(mlc.sound_round_insertion);
                return true;
            }
            return false;
        }

        public override Dictionary<GunSystemRequests, GunSystemRequest> GetPossibleRequests() {
            return new Dictionary<GunSystemRequests, GunSystemRequest>() {
                {GunSystemRequests.INPUT_ADD_ROUND, InputAddRound},
            };
        }

        private bool IsAddingRounds() {
            return mlc.can_insert;
        }

        public override Dictionary<GunSystemQueries, GunSystemQuery> GetPossibleQuestions() {
            return new Dictionary<GunSystemQueries, GunSystemQuery>() {
                {GunSystemQueries.IS_ADDING_ROUNDS, IsAddingRounds},
            };
        }

        public override void Initialize() {
            mlc = gs.GetComponent<ManualLoadingComponent>();
        }
    }

    [InclusiveAspects(GunAspect.CHAMBER)]
    public class ChamberSystem : GunSystemBase {
        ChamberComponent cc;

        public bool PutRoundInChamber() {
            if (cc.active_round_state == RoundState.EMPTY) {
                cc.active_round = (GameObject)GameObject.Instantiate(gs.full_casing, cc.point_load_round.position, cc.point_load_round.rotation);
                RemoveChildrenShadows(cc.active_round);

                cc.active_round.transform.parent = gs.transform;
                cc.active_round.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                cc.active_round_state = RoundState.LOADING;
                return true;
            }
            return false;
        }

        public override Dictionary<GunSystemRequests, GunSystemRequest> GetPossibleRequests() {
            return new Dictionary<GunSystemRequests, GunSystemRequest>() {
                {GunSystemRequests.PUT_ROUND_IN_CHAMBER, PutRoundInChamber}
            };
        }

        public override void Initialize() {
            cc = gs.GetComponent<ChamberComponent>();
        }
    }

    [InclusiveAspects(GunAspect.MAGAZINE, GunAspect.CHAMBER)]
    public class MagazineChamberingSystem : GunSystemBase {
        MagazineComponent mc;

        public bool ChamberRoundFromMag() {
            if (mc.mag_stage == MagStage.IN && mc.mag_script && mc.mag_script.NumRounds() > 0) {
                if(gs.Request(GunSystemRequests.PUT_ROUND_IN_CHAMBER)) {
                    mc.mag_script.RemoveRound();
                    return true;
                }
            }
            return false;
        }

        public override Dictionary<GunSystemRequests, GunSystemRequest> GetPossibleRequests() {
            return new Dictionary<GunSystemRequests, GunSystemRequest>() {
                {GunSystemRequests.CHAMBER_ROUND_FROM_MAG, ChamberRoundFromMag},
            };
        }

        public override void Initialize() {
            mc = gs.GetComponent<MagazineComponent>();
        }
    }

    [InclusiveAspects(GunAspect.GRIP_SAFETY)]
    public class GripSafetySystem : GunSystemBase {
        GripSafetyComponent gsc;

        public bool RequestInputStartAim() {
            gsc.is_safe = false;
            return true;
        }

        public bool RequestInputStopAim() {
            gsc.is_safe = true;
            return true;
        }

        public override Dictionary<GunSystemRequests, GunSystemRequest> GetPossibleRequests() {
            return new Dictionary<GunSystemRequests, GunSystemRequest>() {
                {GunSystemRequests.INPUT_START_AIM, RequestInputStartAim},
                {GunSystemRequests.INPUT_STOP_AIM, RequestInputStopAim},
            };
        }

        public bool IsSafe() {
            return gsc.is_safe;
        }

        public override void Initialize() {
            gsc = gs.GetComponent<GripSafetyComponent>();
        }

        public override void Update() {
            if (gsc.is_safe) {
                gsc.safety_off = Mathf.Max(0.0f, gsc.safety_off - Time.deltaTime * 10.0f);
            } else {
                gsc.safety_off = Mathf.Min(1.0f, gsc.safety_off + Time.deltaTime * 10.0f);
            }
        }
    }

    [InclusiveAspects(GunAspect.THUMB_SAFETY, GunAspect.SLIDE)]
    public class ThumbSafetySystem : GunSystemBase {
        SlideComponent sc; // TODO Thumb safety requires Pistol Slide, move that out somehow
        ThumbSafetyComponent tsc;

        bool IsSafetyOn() {
            return tsc.is_safe;
        }

        bool RequestToggleSafety() {
            if (tsc.is_safe) {
                tsc.is_safe = false;
                gs.PlaySound(tsc.sound_safety);
            } else if (sc.slide_amount == 0.0f) {
                tsc.is_safe = true;
                gs.PlaySound(tsc.sound_safety);
            }
            return true;
        }

        public override Dictionary<GunSystemRequests, GunSystemRequest> GetPossibleRequests() {
            return new Dictionary<GunSystemRequests, GunSystemRequest>() {
                {GunSystemRequests.TOGGLE_SAFETY, RequestToggleSafety}
            };
        }

        public override Dictionary<GunSystemQueries, GunSystemQuery> GetPossibleQuestions() {
            return new Dictionary<GunSystemQueries, GunSystemQuery>() {
                {GunSystemQueries.IS_SAFETY_ON,IsSafetyOn}
            };
        }

        public override void Initialize() {
            sc = gs.GetComponent<SlideComponent>();
            tsc = gs.GetComponent<ThumbSafetyComponent>();
        }

        public override void Update() {
            if (tsc.is_safe) {
                tsc.safety_off = Mathf.Max(0.0f, tsc.safety_off - Time.deltaTime * 10.0f);
            } else {
                tsc.safety_off = Mathf.Min(1.0f, tsc.safety_off + Time.deltaTime * 10.0f);
            }
        }
    }

    [InclusiveAspects(GunAspect.MAGAZINE, GunAspect.INTERNAL_MAGAZINE)]
    public class InternalMagazineSystem : GunSystemBase {
        MagazineComponent mc;
        InternalMagazineComponent imc;

        bool IsMagazineInGun() {
            return true;
        }

        public override Dictionary<GunSystemQueries, GunSystemQuery> GetPossibleQuestions() {
            return new Dictionary<GunSystemQueries, GunSystemQuery>() {
                {GunSystemQueries.IS_MAGAZINE_IN_GUN, IsMagazineInGun},
            };
        }

        public override void Initialize() {
            mc = gs.GetComponent<MagazineComponent>();
            imc = gs.GetComponent<InternalMagazineComponent>();

            mc.mag_script = imc.magazine_script;
        }
    }

    [InclusiveAspects(GunAspect.MAGAZINE, GunAspect.EXTERNAL_MAGAZINE, GunAspect.RECOIL)]
    public class ExternalMagazineSystem : GunSystemBase {
        MagazineComponent mc;
        ExternalMagazineComponent emc;
        RecoilComponent rc;

        bool IsReadyToRemoveMagazine() {
            return mc.ready_to_remove_mag;
        }

        bool IsMagazineEjecting() {
            return mc.mag_stage == MagStage.REMOVING;
        }

        bool IsMagazineInGun() {
            return mc.mag_script != null;
        }

        public bool InputEjectMagazine() {
            gs.PlaySound(emc.sound_mag_eject_button);
            if (emc.can_eject && mc.mag_stage != MagStage.OUT) {
                mc.mag_stage = MagStage.REMOVING;
                gs.PlaySound(emc.sound_mag_ejection);
                return true;
            }
            return false;
        }

        public bool InputInsertMagazine() {
            if(!mc.mag_script) {
                return false; // No mag to push in
            }

            mc.mag_stage = MagStage.INSERTING;
            gs.PlaySound(emc.sound_mag_insertion);
            mc.mag_seated = 0.0f;
            return true;
        }

        public override Dictionary<GunSystemQueries, GunSystemQuery> GetPossibleQuestions() {
            return new Dictionary<GunSystemQueries, GunSystemQuery>() {
                {GunSystemQueries.IS_MAGAZINE_IN_GUN, IsMagazineInGun},
                {GunSystemQueries.IS_MAGAZINE_EJECTING, IsMagazineEjecting},
                {GunSystemQueries.IS_READY_TO_REMOVE_MAGAZINE, IsReadyToRemoveMagazine}
            };
        }

        public override Dictionary<GunSystemRequests, GunSystemRequest> GetPossibleRequests() {
            return new Dictionary<GunSystemRequests, GunSystemRequest>() {
                {GunSystemRequests.INPUT_EJECT_MAGAZINE, InputEjectMagazine},
                {GunSystemRequests.INPUT_INSERT_MAGAZINE, InputInsertMagazine},
            };
        }

        public override void Initialize() {
            mc = gs.GetComponent<MagazineComponent>();
            emc = gs.GetComponent<ExternalMagazineComponent>();
            rc = gs.GetComponent<RecoilComponent>();

            mc.mag_script = GameObject.Instantiate(emc.magazine_obj, gs.transform).GetComponent<mag_script>();
            RemoveChildrenShadows(mc.mag_script.gameObject);
        }

        public override void Update() {
            if (mc.mag_script) {
                Vector3 mag_pos = emc.point_mag_inserted.position;
                Quaternion mag_rot = gs.transform.rotation;
                float mag_seated_display = mc.mag_seated;
                mag_pos += (emc.point_mag_to_insert.position -
                            emc.point_mag_inserted.position) *
                            (1.0f - mag_seated_display);
                mc.mag_script.transform.position = mag_pos;
                mc.mag_script.transform.rotation = mag_rot;
            }

            if (mc.mag_stage == MagStage.INSERTING) {
                mc.mag_seated += Time.deltaTime * 5.0f;
                if (mc.mag_seated >= 1.0f) {
                    mc.mag_seated = 1.0f;
                    mc.mag_stage = MagStage.IN;
                    rc.recoil_transfer_y += UnityEngine.Random.Range(-40.0f, 40.0f);
                    rc.recoil_transfer_x += UnityEngine.Random.Range(50.0f, 300.0f);
                    rc.rotation_transfer_x += UnityEngine.Random.Range(-0.4f, 0.4f);
                    rc.rotation_transfer_y += UnityEngine.Random.Range(0.0f, 1.0f);
                }
            }

            if (mc.mag_stage == MagStage.REMOVING) {
                mc.mag_seated -= Time.deltaTime * 5.0f;
                if (mc.mag_seated <= 0.0f) {
                    mc.mag_seated = 0.0f;
                    mc.ready_to_remove_mag = true;
                    mc.mag_stage = MagStage.OUT;
                }
            }
        }
    }

    /*
        System to handle slide pushing mechanics if we have a slide, but no slide spring
    */
    [InclusiveAspects(GunAspect.SLIDE, GunAspect.SLIDE_PUSHING)]
    [ExclusiveAspects(GunAspect.SLIDE_SPRING)]
    public class SlidePushingSystem : GunSystemBase {
        SlideComponent slide_c;
        private bool pushing = false;

        bool PushSlide() {
            slide_c.slide_stage = SlideStage.NOTHING;
            pushing = true;
            return true;
        }

        bool IsWaitingForSlidePush() {
            return !slide_c.block_slide_pull && slide_c.slide_amount > 0f;
        }

        public override Dictionary<GunSystemRequests, GunSystemRequest> GetPossibleRequests() {
            return new Dictionary<GunSystemRequests, GunSystemRequest>() {
                {GunSystemRequests.INPUT_PUSH_SLIDE_FORWARD, PushSlide},
            };
        }

        public override Dictionary<GunSystemQueries, GunSystemQuery> GetPossibleQuestions() {
            return new Dictionary<GunSystemQueries, GunSystemQuery>() {
                {GunSystemQueries.IS_WAITING_FOR_SLIDE_PUSH, IsWaitingForSlidePush},
            };
        }

        public override void Initialize() {
            slide_c = gs.GetComponent<SlideComponent>();
        }

        public override void Update() {
            slide_c.old_slide_amount = slide_c.slide_amount;
            if (pushing) {
                slide_c.slide_amount = Mathf.Max(0.0f, slide_c.slide_amount - Time.deltaTime * slide_c.slide_lock_speed);
                if(slide_c.slide_amount == 0f)
                    pushing = false;
            }
        }
    }

    [InclusiveAspects(GunAspect.SLIDE, GunAspect.SLIDE_SPRING)]
    [ExclusiveAspects(GunAspect.SLIDE_PUSHING)]
    [Priority(PriorityAttribute.EARLY)]
    public class SlideSpringSystem : GunSystemBase {
        SlideComponent slide_c;

        bool InputReleaseSlide() {
            slide_c.slide_stage = SlideStage.NOTHING;
            return true;
        }

        public override void Initialize() {
            slide_c = gs.GetComponent<SlideComponent>();
        }

        public override Dictionary<GunSystemRequests, GunSystemRequest> GetPossibleRequests() {
            return new Dictionary<GunSystemRequests, GunSystemRequest>() {
                {GunSystemRequests.INPUT_RELEASE_SLIDE, InputReleaseSlide},
            };
        }

        public override void Update() {
            slide_c.old_slide_amount = slide_c.slide_amount;
            if (slide_c.slide_stage == SlideStage.NOTHING) {
                slide_c.slide_amount = Mathf.Max(0.0f, slide_c.slide_amount - Time.deltaTime * slide_c.slide_lock_speed);
            }
        }
    }

    [InclusiveAspects(GunAspect.SLIDE, GunAspect.CHAMBER)]
    public class SlideEjectingSystem : GunSystemBase {
        SlideComponent slide_c;
        ChamberComponent cc;

        public override void Initialize() {
            slide_c = gs.GetComponent<SlideComponent>();
            cc = gs.GetComponent<ChamberComponent>();

            if(!slide_c.eject_round)
                gs.gun_systems.UnloadSystem(this);
        }

        public override void Update() {
            if(cc.active_round_state != RoundState.EMPTY && cc.active_round_state != RoundState.LOADING && slide_c.old_slide_amount >= 0.9f) {
                cc.active_round.AddComponent<Rigidbody>();
                gs.PlaySound(slide_c.sound_bullet_eject);
                cc.active_round.transform.parent = null;
                cc.active_round.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
                cc.active_round.GetComponent<Rigidbody>().velocity = gs.velocity;
                cc.active_round.GetComponent<Rigidbody>().velocity += gs.transform.rotation * new Vector3(UnityEngine.Random.Range(2.0f, 4.0f), UnityEngine.Random.Range(1.0f, 2.0f), UnityEngine.Random.Range(-1.0f, -3.0f));
                cc.active_round.GetComponent<Rigidbody>().angularVelocity = new Vector3(UnityEngine.Random.Range(-40.0f, 40.0f), UnityEngine.Random.Range(-40.0f, 40.0f), UnityEngine.Random.Range(-40.0f, 40.0f));
                cc.active_round = null;
                cc.active_round_state = RoundState.EMPTY;
            }
        }
    }

    [InclusiveAspects(GunAspect.SLIDE, GunAspect.CHAMBER, GunAspect.MAGAZINE, GunAspect.SLIDE_LOCK)]
    public class EmptyInternalMagSlideLockSystem : GunSystemBase {
        SlideComponent sc;
        ChamberComponent cc;
        MagazineComponent mc;

        public override void Initialize() {
            sc = gs.GetComponent<SlideComponent>();
            cc = gs.GetComponent<ChamberComponent>();
            mc = gs.GetComponent<MagazineComponent>();

            sc.should_slide_lock_predicates.Add(ShouldSlideLock);
        }

        private bool ShouldSlideLock() {
            return cc.active_round_state == RoundState.EMPTY && mc.mag_stage == MagStage.IN && mc.mag_script && mc.mag_script.NumRounds() <= 0;
        }
    }

    [InclusiveAspects(GunAspect.SLIDE, GunAspect.CHAMBER)]
    public class SlideChamberingSystem : GunSystemBase {
        SlideComponent slide_c;
        ChamberComponent chamber_c;

        public override void Initialize() {
            slide_c = gs.GetComponent<SlideComponent>();
            chamber_c = gs.GetComponent<ChamberComponent>();

            if(!slide_c.chamber_round)
                gs.gun_systems.UnloadSystem(this);
        }

        public override void Update() {
            if (!slide_c.slide_lock) {
                if(slide_c.old_slide_amount > slide_c.slide_chambering_position && ((!slide_c.chamber_on_pull && slide_c.slide_stage == SlideStage.NOTHING) || (slide_c.chamber_on_pull && slide_c.slide_stage != SlideStage.NOTHING))) {
                    gs.ChamberRoundFromMag();
                }
                
                if(slide_c.slide_stage == SlideStage.NOTHING) {
                    if (chamber_c.is_closed && slide_c.old_slide_amount != 0.0f) {
                        gs.PlaySound(slide_c.sound_slide_front, gs.base_volume * 1.5f);
                        if (chamber_c.active_round != null) {
                            chamber_c.active_round.transform.position = chamber_c.point_chambered_round.position;
                            chamber_c.active_round.transform.rotation = chamber_c.point_chambered_round.rotation;
                        }
                    }

                    if (chamber_c.is_closed && chamber_c.active_round_state == RoundState.LOADING) {
                        chamber_c.active_round_state = RoundState.READY;
                    }
                }
            }
        }
    }

    [InclusiveAspects(GunAspect.SLIDE, GunAspect.SLIDE_LOCK, GunAspect.SLIDE_RELEASE_BUTTON)]
    public class SlideLockSwichSystem : GunSystemBase {
        SlideComponent sc;
        bool pressure_on_switch = false;

        bool InputReleaseSlideLock() {
            return gs.Request(GunSystemRequests.RELEASE_SLIDE_LOCK);
        }

        bool ApplyPressureToSlideLock() {
            pressure_on_switch = true;
            return true;
        }

        bool ReleasePressureToSlideLock() {
            pressure_on_switch = false;
            return true;
        }

        public override void Initialize() {
            sc = gs.GetComponent<SlideComponent>();

            sc.should_slide_lock_predicates.Add(() => pressure_on_switch);
        }

        public override Dictionary<GunSystemRequests, GunSystemRequest> GetPossibleRequests() {
            return new Dictionary<GunSystemRequests, GunSystemRequest>() {
                {GunSystemRequests.APPLY_PRESSURE_ON_SLIDE_LOCK, ApplyPressureToSlideLock},
                {GunSystemRequests.RELEASE_PRESSURE_ON_SLIDE_LOCK, ReleasePressureToSlideLock},
                {GunSystemRequests.INPUT_RELEASE_SLIDE_LOCK, InputReleaseSlideLock},
            };
        }
    }

    [InclusiveAspects(GunAspect.SLIDE, GunAspect.SLIDE_LOCK)]
    public class SlideLockSystem : GunSystemBase {
        SlideComponent sc;

        bool IsSlideLocked() {
            return sc.slide_lock;
        }

        bool ReleaseSlideLock() {
            sc.slide_lock = false;
            return true;
        }

        public override void Initialize() {
            sc = gs.GetComponent<SlideComponent>();
        }

        public override Dictionary<GunSystemQueries, GunSystemQuery> GetPossibleQuestions() {
            return new Dictionary<GunSystemQueries, GunSystemQuery>() {
                {GunSystemQueries.IS_SLIDE_LOCKED, IsSlideLocked},
            };
        }

        public override Dictionary<GunSystemRequests, GunSystemRequest> GetPossibleRequests() {
            return new Dictionary<GunSystemRequests, GunSystemRequest>() {
                {GunSystemRequests.RELEASE_SLIDE_LOCK, ReleaseSlideLock},
            };
        }

        public override void Update() {
            if(sc.old_slide_amount > sc.slide_lock_position) {
                sc.slide_lock = false; // Assume the gun is no longer locked

                if(sc.slide_amount < sc.slide_lock_position && sc.should_slide_lock) {
                    sc.slide_lock = true; // We went over the lock notch and some system wants to push it in
                }
            }
        
            // Catch Slide
            if (sc.slide_lock && sc.slide_stage == SlideStage.NOTHING && sc.old_slide_amount >= sc.slide_lock_position) {
                sc.slide_amount = Mathf.Max(sc.slide_lock_position, sc.slide_amount);
                if (sc.old_slide_amount != sc.slide_lock_position && sc.slide_amount == sc.slide_lock_position) {
                    gs.PlaySound(sc.sound_slide_front);
                }
            }
        }
    }

    [InclusiveAspects(GunAspect.SLIDE)]
    [ExclusiveAspects(GunAspect.OPEN_BOLT_FIRING)]
    public class PressCheckSystem : GunSystemBase {
        SlideComponent slide_c;

        bool IsPressCheck() {
            return slide_c.slide_stage == SlideStage.HOLD && slide_c.slide_amount == slide_c.press_check_position;
        }

        bool InputPressCheck() {
            if (slide_c.slide_stage == SlideStage.NOTHING && !slide_c.block_slide_pull) {
                slide_c.slide_stage = SlideStage.PULLBACK;
            }

            if (slide_c.slide_stage == SlideStage.PULLBACK && slide_c.slide_amount >= slide_c.press_check_position) {
                slide_c.slide_amount = slide_c.press_check_position;
                slide_c.slide_stage = SlideStage.HOLD;
                gs.PlaySound(slide_c.sound_slide_back);
            }
            return true;
        }

        public override Dictionary<GunSystemQueries, GunSystemQuery> GetPossibleQuestions() {
            return new Dictionary<GunSystemQueries, GunSystemQuery>() {
                {GunSystemQueries.IS_PRESS_CHECK, IsPressCheck}
            };
        }

        public override Dictionary<GunSystemRequests, GunSystemRequest> GetPossibleRequests() {
            return new Dictionary<GunSystemRequests, GunSystemRequest>() {
                {GunSystemRequests.INPUT_PULL_SLIDE_PRESS_CHECK, InputPressCheck},
            };
        }

        public override void Initialize() {
            slide_c = gs.GetComponent<SlideComponent>();
        }
    }

    [InclusiveAspects(GunAspect.SLIDE, GunAspect.FIRING)]
    public class SlideKickSystem : GunSystemBase {
        SlideComponent emc;
        FiringComponent fc;

        public override void Initialize() {
            emc = gs.GetComponent<SlideComponent>();
            fc = gs.GetComponent<FiringComponent>();

            if(!emc.kick_slide_back)
                gs.gun_systems.UnloadSystem(this);
        }

        public override void Update() {
            //If we fired a round, load another one, we assume it's a semi.
            if (emc.prev_fire_count != fc.fire_count) {
                gs.PullSlideBack();
                //The state can be SlideStage.PULLING if we don't set.
                //This state handling should be looked over and simplified.
                emc.slide_stage = SlideStage.NOTHING;
                emc.prev_fire_count = fc.fire_count;
                gs.preferred_tilt = GunTilt.RIGHT;
            }
        }
    }

    [InclusiveAspects(GunAspect.SLIDE, GunAspect.CHAMBER)]
    public class SlideChamberClosingSystem : GunSystemBase {
        SlideComponent sc;
        ChamberComponent cc;

        public override void Initialize() {
            sc = gs.GetComponent<SlideComponent>();
            cc = gs.GetComponent<ChamberComponent>();

            cc.is_closed_predicates.Add(() => sc.slide_amount == 0);
        }
    }

    [InclusiveAspects(GunAspect.SLIDE)]
    public class SlideSystem : GunSystemBase {
        SlideComponent slide_c;

        bool IsSlidePulledBack() {
            return slide_c.slide_stage != SlideStage.NOTHING;
        }

        bool RequestInputPullSlideBack() {
            if (slide_c.block_slide_pull == false) {
                slide_c.slide_stage = SlideStage.PULLBACK;
                
                // Tilt gun left if we aren't already prefering something else
                if(gs.preferred_tilt == GunTilt.NONE) {
                    gs.preferred_tilt = GunTilt.LEFT;
                }
            }
            return true;
        }

        bool RequestPullSlideBack() {
            slide_c.slide_amount = 1.0f;
            return true;
        }

        public override Dictionary<GunSystemQueries, GunSystemQuery> GetPossibleQuestions() {
            return new Dictionary<GunSystemQueries, GunSystemQuery>() {
                {GunSystemQueries.IS_SLIDE_PULLED_BACK,IsSlidePulledBack},
            };
        }

        public override Dictionary<GunSystemRequests, GunSystemRequest> GetPossibleRequests() {
            return new Dictionary<GunSystemRequests, GunSystemRequest>() {
                {GunSystemRequests.PULL_SLIDE_BACK, RequestPullSlideBack},
                {GunSystemRequests.INPUT_PULL_SLIDE_BACK, RequestInputPullSlideBack},
            };
        }

        public override void Initialize() {
            slide_c = gs.GetComponent<SlideComponent>();
        }

        public override void Update() {
            // Slide pulling
            if (slide_c.slide_stage == SlideStage.PULLBACK) {
                slide_c.slide_amount += Time.deltaTime * 10.0f;
                if (slide_c.slide_amount >= 1.0f) {
                    gs.PullSlideBack();
                    slide_c.slide_amount = 1.0f;
                    slide_c.slide_stage = SlideStage.HOLD;
                    gs.PlaySound(slide_c.sound_slide_back);

                    if(slide_c.release_slide_automatically) {
                        gs.ReleaseSlide();
                    }
                }
            }

            // Override every tilt when slide is closed
            if (slide_c.slide_amount == 0f) {
                gs.preferred_tilt = GunTilt.NONE;
            }
        }
    }

    [InclusiveAspects(GunAspect.ALTERNATIVE_STANCE)]
    public class AlternativeStanceSystem : GunSystemBase {
        AlternativeStanceComponent asc;

        private bool InputToggleStance() {
            asc.is_alternative = !asc.is_alternative;
            return true;
        }

        private bool IsInAlternativeStance() {
            return asc.is_alternative;
        }

        public override Dictionary<GunSystemQueries, GunSystemQuery> GetPossibleQuestions() {
            return new Dictionary<GunSystemQueries, GunSystemQuery>() {
                {GunSystemQueries.IS_IN_ALTERNATIVE_STANCE, IsInAlternativeStance},
            };
        }

        public override Dictionary<GunSystemRequests, GunSystemRequest> GetPossibleRequests() {
            return new Dictionary<GunSystemRequests, GunSystemRequest>() {
                {GunSystemRequests.INPUT_TOGGLE_STANCE, InputToggleStance},
            };
        }

        public override void Initialize() {
            asc = gs.GetComponent<AlternativeStanceComponent>();
        }
    }

    [InclusiveAspects(GunAspect.FIRE_MODE, GunAspect.TRIGGER)]
    public class FireModeSystem : GunSystemBase {
        FireModeComponent fmc;
        TriggerComponent tc;

        bool RequestToggleFireMode() {
            gs.PlaySound(fmc.sound_firemode_toggle);
            if (fmc.auto_mod_stage == AutoModStage.DISABLED) {
                fmc.auto_mod_stage = AutoModStage.ENABLED;
            } else if (fmc.auto_mod_stage == AutoModStage.ENABLED) {
                fmc.auto_mod_stage = AutoModStage.DISABLED;
            }
            return true;
        }

        public override Dictionary<GunSystemRequests, GunSystemRequest> GetPossibleRequests() {
            return new Dictionary<GunSystemRequests, GunSystemRequest>() {
                {GunSystemRequests.TOGGLE_FIRE_MODE, RequestToggleFireMode}
            };
        }

        public override void Initialize() {
            fmc = gs.GetComponent<FireModeComponent>();
            tc = gs.GetComponent<TriggerComponent>();
        }

        public override void Update() {
            if (fmc.auto_mod_stage == AutoModStage.ENABLED) {
                fmc.auto_mod_amount = Mathf.Min(1.0f, fmc.auto_mod_amount + Time.deltaTime * 10.0f);
            } else if (fmc.auto_mod_stage == AutoModStage.DISABLED) {
                fmc.auto_mod_amount = Mathf.Max(0.0f, fmc.auto_mod_amount - Time.deltaTime * 10.0f);
            }

            if (fmc.auto_mod_amount == 1.0f) {
                tc.fire_mode = FireMode.AUTOMATIC;
            } else if (fmc.auto_mod_amount == 0.0f) {
                tc.fire_mode = FireMode.SINGLE;
            } else {
                //Fall back to disabled while changing, because you can't shoot in an undefined setting 
                //(Or can you? We should test on a real glock)
                tc.fire_mode = FireMode.DISABLED;
            }
        }
    }

    [InclusiveAspects(GunAspect.TRIGGER, GunAspect.HAMMER, GunAspect.CHAMBER)]
    [ExclusiveAspects(GunAspect.OPEN_BOLT_FIRING)]
    public class AutomaticFireSystem : GunSystemBase {
        TriggerComponent tc;
        ChamberComponent cc;
        HammerComponent hc;

        public override void Initialize() {
            tc = gs.GetComponent<TriggerComponent>();
            cc = gs.GetComponent<ChamberComponent>();
            hc = gs.GetComponent<HammerComponent>();
        }

        public override void Update() {
            if (!tc.is_connected && hc.thumb_on_hammer == Thumb.OFF_HAMMER && hc.hammer_cocked == 1.0f) {
                if (cc.is_closed) {
                    if(tc.fire_mode == FireMode.SINGLE || cc.active_round_state != RoundState.READY) {
                        tc.is_connected = true;
                    }
                    hc.hammer_cocked = 0.0f;
                    gs.PlaySound(hc.sound_hammer_strike);
                    if(cc.active_round_state == RoundState.READY) {
                        gs.Request(GunSystemRequests.DISCHARGE);
                    }
                }
            }
        }
    }

    [InclusiveAspects(GunAspect.TRIGGER, GunAspect.SLIDE, GunAspect.CHAMBER, GunAspect.OPEN_BOLT_FIRING)]
    [Priority(PriorityAttribute.LATE)]
    public class OpenBoltFireSystem : GunSystemBase {
        TriggerComponent tc;
        ChamberComponent cc;
        SlideComponent sc;

        public override void Initialize() {
            tc = gs.GetComponent<TriggerComponent>();
            cc = gs.GetComponent<ChamberComponent>();
            sc = gs.GetComponent<SlideComponent>();
        }

        public override void Update() {
            // Slide release
            if (!tc.is_connected) {
                if(tc.fire_mode == FireMode.SINGLE) {
                    tc.is_connected = true;
                }

                gs.Request(GunSystemRequests.RELEASE_SLIDE_LOCK);
            }

            // Slide priming
            if (sc.slide_amount == 0 && sc.old_slide_amount != 0 && cc.is_closed) {
                if(cc.active_round_state == RoundState.READY) {
                    gs.Request(GunSystemRequests.DISCHARGE);
                }
            }
        }
    }

    /// <summary> This system makes sure we always try to push the slide into the slide lock </summary>
    [InclusiveAspects(GunAspect.SLIDE, GunAspect.OPEN_BOLT_FIRING)]
    [Priority(PriorityAttribute.VERY_EARLY)]
    public class OpenBoltSlideLockSystem : GunSystemBase {
        SlideComponent sc;

        public override void Initialize() {
            sc = gs.GetComponent<SlideComponent>();

            sc.should_slide_lock_predicates.Add(() => true);
        }
    }

    [InclusiveAspects(GunAspect.REVOLVER_CYLINDER, GunAspect.MANUAL_LOADING)]
    public class CylinderLoadingSystem : GunSystemBase {
        RevolverCylinderComponent rcc;
        ManualLoadingComponent mlc;

        public override Dictionary<GunSystemRequests, GunSystemRequest> GetPossibleRequests() {
            return new Dictionary<GunSystemRequests, GunSystemRequest>() {
                {GunSystemRequests.INPUT_ADD_ROUND, InputAddRoundToCylinder},
            };
        }

        public override void Initialize() {
            rcc = gs.GetComponent<RevolverCylinderComponent>();
            mlc = gs.GetComponent<ManualLoadingComponent>();
        }

        bool InputAddRoundToCylinder() {
            if (rcc.is_closed == mlc.load_when_closed) {
                int best_chamber = -1;
                int next_shot = rcc.active_cylinder;
                if (!gs.IsHammerCocked()) {
                    next_shot = (next_shot + 1) % rcc.cylinder_capacity;
                }
                for (int i = 0; i < rcc.cylinder_capacity; ++i) {
                    int check = (next_shot + i) % rcc.cylinder_capacity;
                    if (check < 0) {
                        check += rcc.cylinder_capacity;
                    }
                    if (rcc.cylinders[check].game_object == null) {
                        best_chamber = check;
                        break;
                    }
                }
                if (best_chamber == -1) {
                    return false;
                }

                PutRoundInChamber(best_chamber);
                gs.PlaySound(mlc.sound_round_insertion);
                return true;

            }
            return false;
        }

        public void PutRoundInChamber(int index) {
            Transform chamber = rcc.chambers[index];
            rcc.cylinders[index].game_object = (GameObject)GameObject.Instantiate(rcc.full_casing,chamber.position, chamber.rotation, chamber);
            rcc.cylinders[index].game_object.transform.localScale = Vector3.one;
            rcc.cylinders[index].can_fire = true;
            rcc.cylinders[index].seated = UnityEngine.Random.Range(rcc.seating_min, rcc.seating_max);
            RemoveChildrenShadows(rcc.cylinders[index].game_object);
        }
    }

    [InclusiveAspects(GunAspect.YOKE)]
    public class YokeSystem : GunSystemBase {
        YokeComponent yc;

        public override void Initialize() {
            yc = gs.GetComponent<YokeComponent>();
        }

        bool InputCloseCylinder() {
            if (yc.yoke_stage == YokeStage.OPEN || yc.yoke_stage == YokeStage.OPENING) { // TODO add erc.extractor_rod_stage == ExtractorRodStage.CLOSED
                yc.yoke_stage = YokeStage.CLOSING;
                return true;
            }
            return false;
        }

        bool InputSwingOutCylinder() {
            if (yc.yoke_stage == YokeStage.CLOSED || yc.yoke_stage == YokeStage.CLOSING) {
                yc.yoke_stage = YokeStage.OPENING;
                return true;
            }
            return false;
        }

        bool IsCylinderOpen() {
            return yc.yoke_stage == YokeStage.OPEN || yc.yoke_stage == YokeStage.OPENING;
        }

        public bool IsAddingRounds() {
            if (yc.yoke_stage == YokeStage.OPEN) {
                return true;
            } else {
                return false;
            }
        }

        public override Dictionary<GunSystemQueries, GunSystemQuery> GetPossibleQuestions() {
            return new Dictionary<GunSystemQueries, GunSystemQuery>() {
                {GunSystemQueries.IS_ADDING_ROUNDS, IsAddingRounds},
                {GunSystemQueries.IS_CYLINDER_OPEN, IsCylinderOpen},
            };
        }

        public override Dictionary<GunSystemRequests, GunSystemRequest> GetPossibleRequests() {
            return new Dictionary<GunSystemRequests, GunSystemRequest>() {
                {GunSystemRequests.INPUT_SWING_OUT_CYLINDER, InputSwingOutCylinder},
                {GunSystemRequests.INPUT_CLOSE_CYLINDER, InputCloseCylinder},
            };
        }

        public override void Update() {
            if (yc.yoke_stage == YokeStage.CLOSING) {
                yc.yoke_open -= Time.deltaTime * 5.0f;
                if (yc.yoke_open <= 0.0f) {
                    yc.yoke_open = 0.0f;
                    yc.yoke_stage = YokeStage.CLOSED;
                    gs.PlaySound(yc.sound_cylinder_close, gs.base_volume * 2.0f);
                    //rcc.target_cylinder_offset = 0; // TODO reimplement
                }
            }

            if (yc.yoke_stage == YokeStage.OPENING) {
                yc.yoke_open += Time.deltaTime * 5.0f;
                if (yc.yoke_open >= 1.0f) {
                    yc.yoke_open = 1.0f;
                    yc.yoke_stage = YokeStage.OPEN;
                    gs.PlaySound(yc.sound_cylinder_open, gs.base_volume * 2.0f);
                }
            }
        }
    }

    [InclusiveAspects(GunAspect.REVOLVER_CYLINDER, GunAspect.HAMMER)]
    public class CylinderHammerCycleSystem : GunSystemBase {
        RevolverCylinderComponent rcc;
        HammerComponent hc;

        public override void Initialize() {
            rcc = gs.GetComponent<RevolverCylinderComponent>();
            hc = gs.GetComponent<HammerComponent>();

            if(!rcc.hammer_cycling)
                gs.gun_systems.UnloadSystem(this);
        }

        public override void Update() {
            if (rcc.hammer_cycling && hc.thumb_on_hammer != Thumb.SLOW_LOWERING) {
                if (hc.hammer_cocked > 0.0f) {
                    if (rcc.is_closed && hc.hammer_cocked == 1.0f && hc.prev_hammer_cocked != 1.0f) {
                        ++rcc.active_cylinder;
                        rcc.cylinder_rotation = rcc.active_cylinder * 360.0f / rcc.cylinder_capacity;
                    }
                    if (rcc.is_closed && hc.hammer_cocked < 1.0f) {
                        rcc.cylinder_rotation = (rcc.active_cylinder + hc.hammer_cocked) * 360.0f / rcc.cylinder_capacity;
                        rcc.target_cylinder_offset = (int)0.0f;
                    }
                }
            }
        }
    }

    /// <summary> A system to cycle cylinders, it does half the motion when the slide pulls back, and does the other half on the way back </summary>
    [InclusiveAspects(GunAspect.REVOLVER_CYLINDER, GunAspect.SLIDE)]
    public class CylinderSlideCycleSystem : GunSystemBase {
        RevolverCylinderComponent rcc;
        SlideComponent sc;

        private bool reverse_direction = false;

        public override void Initialize() {
            rcc = gs.GetComponent<RevolverCylinderComponent>();
            sc = gs.GetComponent<SlideComponent>();

            if(!rcc.slide_cycling)
                gs.gun_systems.UnloadSystem(this);
        }

        public override void Update() {
            if (sc.slide_amount != sc.old_slide_amount) {
                if(sc.slide_amount > sc.old_slide_amount) {
                    rcc.cylinder_rotation = (rcc.active_cylinder + sc.slide_amount / 2f) * 360.0f / rcc.cylinder_capacity;
                    if(sc.slide_amount == 1f) {
                        reverse_direction = true;
                    }
                }

                if(sc.slide_amount < sc.old_slide_amount) {
                    if(reverse_direction) {
                        rcc.cylinder_rotation = (rcc.active_cylinder + 1 - sc.slide_amount / 2f) * 360.0f / rcc.cylinder_capacity;
                        if(sc.slide_amount == 0f) {
                            rcc.active_cylinder++;
                            reverse_direction = false;
                        }
                    } else {
                        rcc.cylinder_rotation = (rcc.active_cylinder + sc.slide_amount / 2f) * 360.0f / rcc.cylinder_capacity;
                    }
                }
            }
        }
    }

    [InclusiveAspects(GunAspect.REVOLVER_CYLINDER, GunAspect.HAMMER)]
    public class RevolverCylinderSystem : GunSystemBase {
        RevolverCylinderComponent rcc;
        HammerComponent hc;

        public override void Initialize() {
            rcc = gs.GetComponent<RevolverCylinderComponent>();
            hc = gs.GetComponent<HammerComponent>();

            rcc.chambers = new Transform[rcc.cylinder_capacity];
            rcc.cylinders = new CylinderState[rcc.cylinder_capacity];
            for (int i = 0; i < rcc.cylinder_capacity; ++i) {
                string name = "point_chamber_" + (i+ 1);
                rcc.chambers[i] = rcc.chamber_parent.Find(name);
                rcc.cylinders[i] = new CylinderState();
            }
        }

        public override void Update() {
            if (rcc.is_closed && hc.hammer_cocked == 1.0f) {
                rcc.target_cylinder_offset = 0;
            }

            if(!rcc.can_manual_rotate) {
                rcc.target_cylinder_offset = 0;
            }

            if (rcc.target_cylinder_offset != 0.0f) {
                float target_cylinder_rotation = ((rcc.active_cylinder + rcc.target_cylinder_offset) * 360.0f / rcc.cylinder_capacity);
                rcc.cylinder_rotation = Mathf.Lerp(target_cylinder_rotation, rcc.cylinder_rotation, Mathf.Pow(0.2f, Time.deltaTime));
                if (rcc.cylinder_rotation > (rcc.active_cylinder + 0.5f) * 360.0f / rcc.cylinder_capacity) {
                    ++rcc.active_cylinder;
                    --rcc.target_cylinder_offset;
                    if (rcc.is_closed) {
                        gs.PlaySound(rcc.sound_cylinder_rotate);
                    }
                }
                if (rcc.cylinder_rotation < (rcc.active_cylinder - 0.5f) * 360.0f / rcc.cylinder_capacity) {
                    --rcc.active_cylinder;
                    ++rcc.target_cylinder_offset;
                    if (rcc.is_closed) {
                        gs.PlaySound(rcc.sound_cylinder_rotate);
                    }
                }
            }
        }
    }

    [InclusiveAspects(GunAspect.REVOLVER_CYLINDER, GunAspect.TRIGGER, GunAspect.HAMMER)]
    public class RevolverFireSystem : GunSystemBase {
        RevolverCylinderComponent rcc;
        TriggerComponent tc;
        HammerComponent hc;

        public override void Initialize() {
            rcc = gs.GetComponent<RevolverCylinderComponent>();
            tc = gs.GetComponent<TriggerComponent>();
            hc = gs.GetComponent<HammerComponent>();
        }

        public override void Update() {
            if (!tc.is_connected && hc.thumb_on_hammer == Thumb.OFF_HAMMER && hc.hammer_cocked == 1.0f) {
                hc.hammer_cocked = 0.0f;
                if(tc.fire_mode == FireMode.SINGLE) {
                    tc.is_connected = true;
                }

                if (rcc.is_closed) {
                    int which_chamber = rcc.active_cylinder % rcc.cylinder_capacity;
                    if (which_chamber < 0) {
                        which_chamber += rcc.cylinder_capacity;
                    }
                    GameObject round = rcc.cylinders[which_chamber].game_object;
                    if ((round != null) && rcc.cylinders[which_chamber].can_fire) {
                        gs.Request(GunSystemRequests.DISCHARGE);
                    } else {
                        gs.PlaySound(hc.sound_hammer_strike, 0.5f);
                    }
                } else {
                    gs.PlaySound(hc.sound_hammer_strike, 0.5f);
                }
            }
        }
    }

    [InclusiveAspects(GunAspect.TRIGGER)]
    public class GunTriggerSystem : GunSystemBase {
        TriggerComponent tc;
        public bool ApplyTriggerPressure() {
            if(tc.trigger_pressable) {
                tc.pressure_on_trigger = true;
                return true;
            }
            return false;
        }

        public bool ReleaseTriggerPressure() {
            tc.pressure_on_trigger = false;
            return true;
        }

        public override Dictionary<GunSystemRequests, GunSystemRequest> GetPossibleRequests() {
            return new Dictionary<GunSystemRequests, GunSystemRequest>() {
                {GunSystemRequests.APPLY_TRIGGER_PRESSURE, ApplyTriggerPressure},
                {GunSystemRequests.RELEASE_TRIGGER_PRESSURE, ReleaseTriggerPressure}
            };
        }

        public override void Initialize() {
            tc = gs.GetComponent<TriggerComponent>();
        }

        public override void Update() {
            tc.old_trigger_pressed = tc.trigger_pressed;
            if (tc.pressure_on_trigger) {
                tc.trigger_pressed = Mathf.Min(1.0f, tc.trigger_pressed + Time.deltaTime * 20.0f);
            } else {
                tc.trigger_pressed = Mathf.Max(0.0f, tc.trigger_pressed - Time.deltaTime * 20.0f);
            }

            if(tc.trigger_pressed != 1f) {
                tc.is_connected = true; // Guns usually need to cycle before connecting again
            } else if(tc.old_trigger_pressed != 1f) {
                tc.is_connected = false; // We just pressed the trigger fully
            }
        }
    }

    [InclusiveAspects(GunAspect.REVOLVER_CYLINDER, GunAspect.YOKE, GunAspect.YOKE_AUTO_EJECTOR)]
    public class YokeAutoEjectSystem : GunSystemBase {
        RevolverCylinderComponent rcc;
        YokeComponent yc;

        private bool triggered = true;

        public override void Initialize() {
            rcc = gs.GetComponent<RevolverCylinderComponent>();
            yc = gs.GetComponent<YokeComponent>();
        }

        public override void Update() {
            if(triggered && yc.yoke_stage == YokeStage.CLOSED) {
                triggered = false;
            }

            if(!triggered && yc.yoke_stage == YokeStage.OPEN) {
                foreach (CylinderState cylinder in rcc.cylinders.Where((cylinder) => cylinder.game_object)) {
                    CylinderEjectionSystem.EjectRound(cylinder, gs.velocity * 0.75f - cylinder.game_object.transform.forward);
                }
                triggered = true;
            }
        }
    }

    [InclusiveAspects(GunAspect.EXTRACTOR_ROD, GunAspect.REVOLVER_CYLINDER)]
    public class RevolverExtractorRodSystem : GunSystemBase {
        RevolverCylinderComponent rcc;
        ExtractorRodComponent erc;

        public bool RequestInputUseExtractorRod() {
            if (erc.can_extract) {
                erc.extractor_rod_stage = ExtractorRodStage.OPENING;
                if (erc.extractor_rod_amount < 1.0f) {
                    erc.extracted = false;
                }
                return true;
            }

            return false;
        }

        public bool IsEjectingRounds() {
            if (erc.extractor_rod_stage != ExtractorRodStage.CLOSED) {
                return true;
            }
            return false;
        }

        public override Dictionary<GunSystemQueries, GunSystemQuery> GetPossibleQuestions() {
            return new Dictionary<GunSystemQueries, GunSystemQuery>() {
                {GunSystemQueries.IS_EJECTING_ROUNDS, IsEjectingRounds}
            };
        }

        public override Dictionary<GunSystemRequests, GunSystemRequest> GetPossibleRequests() {
            return new Dictionary<GunSystemRequests, GunSystemRequest>() {
                {GunSystemRequests.INPUT_USE_EXTRACTOR_ROD,RequestInputUseExtractorRod}
            };
        }

        public override void Initialize() {
            rcc = gs.GetComponent<RevolverCylinderComponent>();
            erc = gs.GetComponent<ExtractorRodComponent>();
        }

        public override void Update() {
            if (erc.extractor_rod_stage == ExtractorRodStage.CLOSING) {
                erc.extractor_rod_amount -= Time.deltaTime * 10.0f;
                if (erc.extractor_rod_amount <= 0.0f) {
                    erc.extractor_rod_amount = 0.0f;
                    erc.extractor_rod_stage = ExtractorRodStage.CLOSED;
                    gs.PlaySound(erc.sound_extractor_rod_close);
                }
                for (int i = 0; i < rcc.cylinder_capacity; ++i) {
                    if (rcc.cylinders[i].game_object != null) {
                        rcc.cylinders[i].falling = false;
                    }
                }
            }

            if (erc.extractor_rod_stage == ExtractorRodStage.OPENING) {
                float old_extractor_rod_amount = erc.extractor_rod_amount;
                erc.extractor_rod_amount += Time.deltaTime * 10.0f;
                if (erc.extractor_rod_amount >= 1.0f) {
                    if(erc.chamber_offset < 0) { // Extract in all chambers
                        for (int i = 0; i < rcc.cylinder_capacity; ++i) {
                            ExtractCylinder(rcc.cylinders[i]);
                        }
                    } else { // Extract in specific chamber
                        int which_chamber = (rcc.active_cylinder + erc.chamber_offset) % rcc.cylinder_capacity;
                        if (which_chamber < 0) {
                            which_chamber += rcc.cylinder_capacity;
                        }
                        ExtractCylinder(rcc.cylinders[which_chamber]);
                    }
                    erc.extractor_rod_amount = 1.0f;
                    erc.extractor_rod_stage = ExtractorRodStage.OPEN;
                    if (old_extractor_rod_amount < 1.0f) {
                        gs.PlaySound(erc.sound_extractor_rod_open);
                    }
                }

                if(!erc.extracted) {
                    erc.extracted = true;
                }
            }

            if (erc.extractor_rod_stage == ExtractorRodStage.OPENING || erc.extractor_rod_stage == ExtractorRodStage.OPEN) {
                erc.extractor_rod_stage = ExtractorRodStage.CLOSING;
            }
        }

        private void ExtractCylinder(CylinderState cylinder) {
            if(!erc.extracted && cylinder.game_object != null) {
                if (UnityEngine.Random.Range(0.0f, 3.0f) > cylinder.seated) {
                    cylinder.falling = true;
                    cylinder.seated -= UnityEngine.Random.Range(0.0f, 0.5f);
                } else {
                    cylinder.falling = false;
                }
            }

            if ((cylinder.game_object != null) && cylinder.falling) {
                cylinder.seated -= Time.deltaTime * 5.0f;
                if (cylinder.seated <= 0.0f) {
                    CylinderEjectionSystem.EjectRound(cylinder, gs.velocity);
                }
            }
        }
    }

    /// <summary> This system holds the static method "EjectRound" to be used from other systems that extract rounds from cylinders
    [InclusiveAspects(GunAspect.REVOLVER_CYLINDER)]
    public class CylinderEjectionSystem : GunSystemBase {

        /// <summary> Eject a round if a gameobject is inside the cylinder </summary>
        public static void EjectRound(CylinderState cylinder, Vector3 velocity) {
            if(cylinder.game_object) {
                GameObject bullet = cylinder.game_object;
                bullet.AddComponent<Rigidbody>();
                bullet.transform.parent = null;
                bullet.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
                bullet.GetComponent<Rigidbody>().velocity = velocity;
                bullet.GetComponent<Rigidbody>().angularVelocity = new Vector3(UnityEngine.Random.Range(-40.0f, 40.0f), UnityEngine.Random.Range(-40.0f, 40.0f), UnityEngine.Random.Range(-40.0f, 40.0f));
                cylinder.game_object = null;
                cylinder.can_fire = false;
            }
        }
    }

    [InclusiveAspects(GunAspect.HAMMER, GunAspect.TRIGGER, GunAspect.THUMB_COCKING)]
    public class HammerDecockSystem : GunSystemBase {
        TriggerComponent tc;
        HammerComponent hc;

        bool RequestInputReleaseHammer() {
            if (tc.pressure_on_trigger || hc.hammer_cocked != 1.0f) {
                hc.thumb_on_hammer = Thumb.SLOW_LOWERING;
                tc.is_connected = true;
            } else {
                hc.thumb_on_hammer = Thumb.OFF_HAMMER;
            }
            return true;
        }

        public override Dictionary<GunSystemRequests, GunSystemRequest> GetPossibleRequests() {
            return new Dictionary<GunSystemRequests, GunSystemRequest>() {
                {GunSystemRequests.INPUT_RELEASE_HAMMER,RequestInputReleaseHammer}
            };
        }

        public override void Initialize() {
            tc = gs.GetComponent<TriggerComponent>();
            hc = gs.GetComponent<HammerComponent>();
        }

        public override void Update() {
            if (hc.thumb_on_hammer == Thumb.SLOW_LOWERING) {
                hc.hammer_cocked -= Time.deltaTime * 10.0f;
                if (hc.hammer_cocked <= 0.0f) {
                    hc.hammer_cocked = 0.0f;
                    hc.thumb_on_hammer = Thumb.OFF_HAMMER;
                    gs.PlaySound(hc.sound_hammer_decock);
                }
            }
        }
    }

    [InclusiveAspects(GunAspect.HAMMER)]
    public class HammerSystem : GunSystemBase {
        HammerComponent hc;

        bool IsHammerCocked() {
            return hc.hammer_cocked == 1.0f;
        }

        public override Dictionary<GunSystemQueries, GunSystemQuery> GetPossibleQuestions() {
            return new Dictionary<GunSystemQueries, GunSystemQuery>() {
                {GunSystemQueries.IS_HAMMER_COCKED, IsHammerCocked},
            };
        }


        public override void Initialize() {
            hc = gs.GetComponent<HammerComponent>();
        }

        public override void Update() {
            hc.prev_hammer_cocked = hc.hammer_cocked;

            if (hc.thumb_on_hammer == Thumb.OFF_HAMMER) {
                if (hc.hammer_cocked > 0.0f && hc.hammer_cocked != 1.0f) {
                    hc.hammer_cocked = 0.0f;
                }
            } else if (hc.thumb_on_hammer == Thumb.ON_HAMMER || hc.thumb_on_hammer == Thumb.TRIGGER_PULLED) {
                hc.hammer_cocked = Mathf.Min(1.0f, hc.hammer_cocked + Time.deltaTime * 10.0f);
                if (hc.hammer_cocked == 1.0f && hc.prev_hammer_cocked != 1.0f) {
                    gs.PlaySound(hc.sound_hammer_cock);
                }
            }
        }
    }

    /*
        * System that cocks the hammer by a mechanism where the trigger cocks.
    */
    [InclusiveAspects(GunAspect.HAMMER, GunAspect.TRIGGER, GunAspect.TRIGGER_COCKING)]
    public class TriggerCockingSystem : GunSystemBase {
        TriggerComponent tc;
        HammerComponent hc;

        public override void Initialize() {
            tc = gs.GetComponent<TriggerComponent>();
            hc = gs.GetComponent<HammerComponent>();
        }

        public override void Update() {
            if(!hc.is_blocked && !tc.is_connected) {
                if (hc.thumb_on_hammer == Thumb.OFF_HAMMER && tc.trigger_pressed == 1f) {
                    hc.thumb_on_hammer = Thumb.TRIGGER_PULLED;
                } else if (hc.thumb_on_hammer == Thumb.TRIGGER_PULLED && hc.hammer_cocked == 1.0f) {
                    hc.thumb_on_hammer = Thumb.OFF_HAMMER;
                }
            }
        }
    }

    [InclusiveAspects(GunAspect.HAMMER, GunAspect.THUMB_COCKING)]
    public class ThumbCockingSystem : GunSystemBase {
        HammerComponent hc;

        bool RequestInputPressureOnHammer() {
            if(hc.is_blocked)
                return false;

            hc.thumb_on_hammer = Thumb.ON_HAMMER;
            return true;
        }

        public override Dictionary<GunSystemRequests, GunSystemRequest> GetPossibleRequests() {
            return new Dictionary<GunSystemRequests, GunSystemRequest>() {
                {GunSystemRequests.INPUT_PRESSURE_ON_HAMMER,RequestInputPressureOnHammer}
            };
        }

        public override void Initialize() {
            hc = gs.GetComponent<HammerComponent>();
        }
    }

    [InclusiveAspects(GunAspect.HAMMER, GunAspect.SLIDE, GunAspect.SLIDE_COCKING)]
    public class SlideCockingSystem : GunSystemBase {
        SlideComponent sc;
        HammerComponent hc;

        public override void Initialize() {
            sc = gs.GetComponent<SlideComponent>();
            hc = gs.GetComponent<HammerComponent>();
        }

        public override void Update() {
            if(!hc.is_blocked) {
                hc.hammer_cocked = Mathf.Max(hc.hammer_cocked, sc.slide_amount);
            }
        }
    }

    [InclusiveAspects(GunAspect.SLIDE, GunAspect.LOCKABLE_BOLT)]
    public class SlideBoltBlockSystem : GunSystemBase {
        LockableBoltComponent bc;
        SlideComponent sc;

        public override void Initialize() {
            bc = gs.GetComponent<LockableBoltComponent>();
            sc = gs.GetComponent<SlideComponent>();

            bc.block_toggle_predicates.Add(() => sc.slide_amount > 0f || sc.slide_stage == SlideStage.PULLBACK);
        }
    }

    [InclusiveAspects(GunAspect.LOCKABLE_BOLT)]
    public class LockableBoltSystem : GunSystemBase {
        LockableBoltComponent bc;

        private bool ToggleBolt() {
            if(bc.block_toggle) 
                return false;

            switch (bc.bolt_stage) {
                case BoltActionStage.LOCKED:
                    bc.bolt_stage = BoltActionStage.UNLOCKING;
                    gs.PlaySound(bc.sound_rifle_bolt_unlock);
                    return true;
                case BoltActionStage.UNLOCKED:
                    bc.bolt_stage = BoltActionStage.LOCKING;
                    gs.PlaySound(bc.sound_rifle_bolt_lock);
                    return true;
            }
            return false;
        }

        public override Dictionary<GunSystemRequests, GunSystemRequest> GetPossibleRequests() {
            return new Dictionary<GunSystemRequests, GunSystemRequest>() {
                {GunSystemRequests.INPUT_TOGGLE_BOLT_LOCK, ToggleBolt},
            };
        }

        public override void Initialize() {
            bc = gs.GetComponent<LockableBoltComponent>();

            bc.bolt_unlocked_rot = bc.point_bolt_unlocked.localRotation;
            bc.bolt_locked_rot = bc.bolt.localRotation;
        }

        public override void Update() {
            switch (bc.bolt_stage) {
                case BoltActionStage.LOCKING:
                    bc.bolt_rotation_lock_amount += Time.deltaTime * bc.toggle_speed;
                    if( bc.bolt_rotation_lock_amount >= 1.0f) {
                        bc.bolt_rotation_lock_amount = 1.0f;
                        bc.bolt_stage = BoltActionStage.LOCKED;
                    }
                    break;
                case BoltActionStage.UNLOCKING:
                    bc.bolt_rotation_lock_amount -= Time.deltaTime * bc.toggle_speed;
                    if (bc.bolt_rotation_lock_amount <= 0.0f) {
                        bc.bolt_rotation_lock_amount = 0.0f;
                        bc.bolt_stage = BoltActionStage.UNLOCKED;
                    }
                    break;
            }

            bc.bolt.localRotation = Quaternion.Slerp(
                bc.bolt_unlocked_rot,
                bc.bolt_locked_rot,
                bc.bolt_rotation_lock_amount
            );
        }
    }

    [InclusiveAspects(GunAspect.REVOLVER_CYLINDER)]
    public class RoundSpendingCylinderSystem : GunSystemBase {
        RevolverCylinderComponent rcc;

        public bool SpendRound() {
            int which_chamber = rcc.active_cylinder % rcc.cylinder_capacity;
            if (which_chamber < 0) {
                which_chamber += rcc.cylinder_capacity;
            }

            GameObject round = rcc.cylinders[which_chamber].game_object;
            if ((round != null) && rcc.cylinders[which_chamber].can_fire) {
                rcc.cylinders[which_chamber].can_fire = false;
                rcc.cylinders[which_chamber].seated += UnityEngine.Random.Range(rcc.seating_firebonus_min, rcc.seating_firebonus_max);
                rcc.cylinders[which_chamber].game_object = GameObject.Instantiate(gs.empty_casing, round.transform.position, round.transform.rotation, round.transform.parent);
                GameObject.Destroy(round);

                RemoveChildrenShadows(rcc.cylinders[which_chamber].game_object);
                return true;
            }
            return false;
        }

        public override Dictionary<GunSystemRequests, GunSystemRequest> GetPossibleRequests() {
            return new Dictionary<GunSystemRequests, GunSystemRequest>() {
                {GunSystemRequests.SPEND_ROUND, SpendRound},
            };
        }

        public override void Initialize() {
            rcc = gs.GetComponent<RevolverCylinderComponent>();
        }
    }

    [InclusiveAspects(GunAspect.CHAMBER)]
    public class RoundSpendingChamberSystem : GunSystemBase {
        ChamberComponent cc;

        public bool SpendRound() {
            if(cc.active_round != null) {
                cc.active_round_state = RoundState.FIRED;

                GameObject spent_round = GameObject.Instantiate(gs.empty_casing, cc.active_round.transform.position, cc.active_round.transform.rotation, cc.active_round.transform.parent);
                GameObject.Destroy(cc.active_round);
                cc.active_round = spent_round;

                RemoveChildrenShadows(cc.active_round);
                return true;
            }
            return false;
        }

        public override Dictionary<GunSystemRequests, GunSystemRequest> GetPossibleRequests() {
            return new Dictionary<GunSystemRequests, GunSystemRequest>() {
                {GunSystemRequests.SPEND_ROUND, SpendRound},
            };
        }

        public override void Initialize() {
            cc = gs.GetComponent<ChamberComponent>();
        }
    }

    /*
        * Make the bullet fly forward, creating a logical projectile.
    */
    [InclusiveAspects(GunAspect.FIRING)]
    public class FiringSystem : GunSystemBase {
        FiringComponent fc;

        public bool Discharge() {
            GameObject bullet = null;
            gs.PlaySound(fc.sound_gunshot_smallroom, 1.0f);
            
            gs.Request(GunSystemRequests.SPEND_ROUND);

            for(var i = 0; i < fc.projectile_count; i++) {
                GameObject.Instantiate(fc.muzzle_flash, fc.point_muzzleflash.position, fc.point_muzzleflash.rotation);
                bullet = (GameObject)GameObject.Instantiate(fc.bullet_obj, fc.point_muzzle.position, fc.point_muzzle.rotation);

                if(fc.inaccuracy != 0f) {
                    float angle = UnityEngine.Random.Range(0, 6.283f);
                    float radius = UnityEngine.Random.Range(0, fc.inaccuracy);
                    bullet.transform.Rotate(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
                }
                bullet.GetComponent<BulletScript>().SetVelocity(bullet.transform.forward * fc.exit_velocity);
            }

            fc.fire_count++;
            return true;
        }

        public override Dictionary<GunSystemRequests, GunSystemRequest> GetPossibleRequests() {
            return new Dictionary<GunSystemRequests, GunSystemRequest>() {
                {GunSystemRequests.DISCHARGE, Discharge},
            };
        }

        public override void Initialize() {
            fc = gs.GetComponent<FiringComponent>();
        }
    }

    [InclusiveAspects(GunAspect.RECOIL, GunAspect.FIRING)]
    public class FiringRecoilSystem : GunSystemBase {
        FiringComponent fc;
        RecoilComponent rc;

        public override void Initialize() {
            fc = gs.GetComponent<FiringComponent>();
            rc = gs.GetComponent<RecoilComponent>();
        }

        public override void Update() {
            if(rc.old_fire_count != fc.fire_count) {
                rc.rotation_transfer_y += UnityEngine.Random.Range(1.0f, 2.0f) * fc.recoil_multiplier;
                rc.rotation_transfer_x += UnityEngine.Random.Range(-1.0f, 1.0f) * fc.recoil_multiplier;
                rc.recoil_transfer_x -= UnityEngine.Random.Range(150.0f, 300.0f) * fc.recoil_multiplier;
                rc.recoil_transfer_y += UnityEngine.Random.Range(-200.0f, 200.0f) * fc.recoil_multiplier;
                rc.add_head_recoil = true;
            }
            rc.old_fire_count = fc.fire_count;
        }
    }

    [InclusiveAspects(GunAspect.RECOIL, GunAspect.AIM_SWAY)]
    public class AimSwaySystem : GunSystemBase {
        AimSwayComponent asc;
        RecoilComponent rc;

        public override void Initialize() {
            asc = gs.GetComponent<AimSwayComponent>();
            rc = gs.GetComponent<RecoilComponent>();
        }

        public override void Update() {
            rc.recoil_transfer_x -= asc.horizontal_strength * Mathf.Sin(4 * Time.time * asc.speed + (float)Math.PI / 4f) * Time.deltaTime;
            rc.recoil_transfer_y -= asc.vertical_strength * Mathf.Sin(3 * Time.time * asc.speed) * Time.deltaTime;
        }
    }

    public class GunSystems : GunSystemsContainer {
        GunSystemBase[] systems;

        public override void LoadSystems(GunScript gs, GunAspect aspects) {
            Dictionary<GunSystemRequests, string> taken_requests = new Dictionary<GunSystemRequests, string>();
            Dictionary<GunSystemQueries, string> taken_queries = new Dictionary<GunSystemQueries, string>();

            Dictionary<GunSystemBase, int> loaded_systems = new Dictionary<GunSystemBase, int>();
            requests = new Dictionary<GunSystemRequests, GunSystemBase.GunSystemRequest>();
            queries = new Dictionary<GunSystemQueries, GunSystemBase.GunSystemQuery>();

            //Load all systems which aspects match the current gun.
            foreach (Type type in typeof(GunSystemBase).GetAllDerivedTypes(typeof(GunSystems))) {
                if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(GunSystemBase))) {
                    if(ShouldLoadSystem(type, aspects)) {
                        GunSystemBase gsb = (GunSystemBase)Activator.CreateInstance(type);
                        gsb.gs = gs;

                        Dictionary<GunSystemRequests, GunSystemBase.GunSystemRequest> requests = gsb.GetPossibleRequests();
                        Dictionary<GunSystemQueries, GunSystemBase.GunSystemQuery> queries = gsb.GetPossibleQuestions();

                        if (requests != null) {
                            foreach (KeyValuePair<GunSystemRequests, GunSystemBase.GunSystemRequest> request in requests) {
                                if (taken_requests.ContainsKey(request.Key)) {
                                    Debug.LogError("System Request() clash on: \"" + request.Key.ToString() + "\" both " + gsb.ToString() + " and " + taken_requests[request.Key] + " registers a response, making them incompatible to coexist. Check your aspects on your gun.");
                                } else {
                                    taken_requests.Add(request.Key, gsb.ToString());
                                }
                            }

                            this.requests.Merge(requests);
                        }

                        if (queries != null) {
                            foreach (KeyValuePair<GunSystemQueries, GunSystemBase.GunSystemQuery> query in queries) {
                                if (taken_queries.ContainsKey(query.Key)) {
                                    Debug.LogError("System Query() clash on: \"" + query.Key.ToString() + "\" both " + gsb.ToString() + " and " + taken_queries[query.Key] + " registers a response, making them incompatible to coexist. Check your aspects on your gun.");
                                } else {
                                    taken_queries.Add(query.Key, gsb.ToString());
                                }
                            }
                            this.queries.Merge(queries);
                        }
                        loaded_systems.Add(gsb, GetPriority(type));
                    }
                }
            }

            // Add systems in order of their priority attribute
            systems = (from item in loaded_systems orderby item.Value select item.Key).ToArray();
        }

        public int GetPriority(Type system) {
            PriorityAttribute priority = system.GetCustomAttribute<PriorityAttribute>(false);
            if(priority != null)
                return priority.priority;
            return PriorityAttribute.NORMAL;
        }

        public override bool ShouldLoadSystem(Type system, GunAspect aspects, bool ignore_exclusives = false, bool ignore_inclusives = false) {
            if(aspects.IsEmpty())
                return false;
            
            if(!ignore_exclusives) {
                ExclusiveAspectsAttribute exclusive = system.GetCustomAttribute<ExclusiveAspectsAttribute>(false);
                if(exclusive != null && exclusive.exclusive_aspects.HasAnyFlag(aspects)) {
                    return false;
                }
            }

            if(!ignore_inclusives) {
                InclusiveAspectsAttribute inclusive = system.GetCustomAttribute<InclusiveAspectsAttribute>(false);
                if(inclusive != null && !aspects.HasFlag(inclusive.inclusive_aspects)) {
                    return false;
                }
            }

            return true;
        }

        public override void UnloadSystem(GunSystemBase system) {
            List<GunSystemBase> temp_systems = new List<GunSystemBase>(systems);
            temp_systems.Remove(system);
            systems = temp_systems.ToArray();
        }

        public override void Initialize() {
            foreach (GunSystemBase system in systems) {
                system.Initialize();
            }
        }

        public override void Update() {
            foreach (GunSystemBase system in systems) {
                system.Update();
            }
        }
    }
}