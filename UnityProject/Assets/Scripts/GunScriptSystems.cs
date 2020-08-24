using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using ExtentionUtil;
using System.Linq;
using GunSystemInterfaces;

namespace GunSystemsV1 {
    [InclusiveAspects(GunAspect.MANUAL_LOADING, GunAspect.MAGAZINE)]
    [ExclusiveAspects(GunAspect.REVOLVER_CYLINDER)]
    public class ManualLoadingMagazineSystem : GunSystemBase {
        MagazineComponent mc = null;
        ManualLoadingComponent mlc = null;

        [GunSystemRequest(GunSystemRequests.INPUT_ADD_ROUND)]
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

        [GunSystemQuery(GunSystemQueries.IS_ADDING_ROUNDS)]
        private bool IsAddingRounds() {
            return mlc.can_insert;
        }
    }

    [InclusiveAspects(GunAspect.MANUAL_LOADING)]
    [ExclusiveAspects(GunAspect.MAGAZINE, GunAspect.REVOLVER_CYLINDER)]
    public class ManualLoadingSystem : GunSystemBase {
        ManualLoadingComponent mlc = null;

        public bool AddRound() {
            if(!mlc.can_insert)
                return false;
            return gs.Request(GunSystemRequests.PUT_ROUND_IN_CHAMBER);
        }

        [GunSystemRequest(GunSystemRequests.INPUT_ADD_ROUND)]
        public bool InputAddRound() {
            if(AddRound()) {
                gs.PlaySound(mlc.sound_round_insertion);
                return true;
            }
            return false;
        }

        [GunSystemQuery(GunSystemQueries.IS_ADDING_ROUNDS)]
        private bool IsAddingRounds() {
            return mlc.can_insert;
        }
    }

    [InclusiveAspects(GunAspect.CHAMBER)]
    public class ChamberSystem : GunSystemBase {
        ChamberComponent cc = null;

        [GunSystemRequest(GunSystemRequests.PUT_ROUND_IN_CHAMBER)]
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
    }

    [InclusiveAspects(GunAspect.MAGAZINE, GunAspect.CHAMBER)]
    public class MagazineChamberingSystem : GunSystemBase {
        MagazineComponent mc = null;

        [GunSystemRequest(GunSystemRequests.CHAMBER_ROUND_FROM_MAG)]
        public bool ChamberRoundFromMag() {
            if (mc.mag_stage == MagStage.IN && mc.mag_script && mc.mag_script.NumRounds() > 0) {
                if(gs.Request(GunSystemRequests.PUT_ROUND_IN_CHAMBER)) {
                    if(Cheats.infinite_ammo)
                        return true;
                    
                    mc.mag_script.RemoveRound();
                    return true;
                }
            }
            return false;
        }
    }

    [InclusiveAspects(GunAspect.MAGAZINE)]
    public class MagazineSystem : GunSystemBase {
        MagazineComponent mc = null;

        public GameObject DisconnectMag () {
            if(!mc.mag_script) {
                return null;
            }

            // Grag mag reference
            GameObject mag = mc.mag_script.gameObject;

            // Disconnect mag from systems
            mc.mag_script = null;
            mc.ready_to_remove_mag = false;
            mag.transform.parent = null;

            return mag;
        }

        public bool ConnectMag(GameObject mag) {
            // Set this mag as mag to insert
            mc.mag_script = mag.GetComponent<mag_script>();
            mag.transform.parent = gs.transform;

            // Tell the systems to push the mag in
            return gs.Request(GunSystemRequests.INPUT_INSERT_MAGAZINE);
        }

        public override void Initialize() {
            gs.gun_systems.disconnectMagazine = DisconnectMag;
            gs.gun_systems.connectMagazine = ConnectMag;
        }
    }

    [InclusiveAspects(GunAspect.GRIP_SAFETY)]
    public class GripSafetySystem : GunSystemBase {
        GripSafetyComponent gsc = null;

        [GunSystemRequest(GunSystemRequests.INPUT_START_AIM)]
        public bool RequestInputStartAim() {
            gsc.is_safe = false;
            return true;
        }

        [GunSystemRequest(GunSystemRequests.INPUT_STOP_AIM)]
        public bool RequestInputStopAim() {
            gsc.is_safe = true;
            return true;
        }

        public bool IsSafe() {
            return gsc.is_safe;
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
        SlideComponent sc = null; // TODO Thumb safety requires Pistol Slide, move that out somehow
        ThumbSafetyComponent tsc = null;

        [GunSystemQuery(GunSystemQueries.IS_SAFETY_ON)]
        bool IsSafetyOn() {
            return tsc.is_safe;
        }

        [GunSystemRequest(GunSystemRequests.TOGGLE_SAFETY)]
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
        MagazineComponent mc = null;
        InternalMagazineComponent imc = null;

        [GunSystemQuery(GunSystemQueries.IS_MAGAZINE_IN_GUN)]
        bool IsMagazineInGun() {
            return true;
        }

        public override void Initialize() {
            mc.mag_script = imc.magazine_script;
        }
    }

    [InclusiveAspects(GunAspect.MAGAZINE, GunAspect.EXTERNAL_MAGAZINE, GunAspect.RECOIL)]
    public class ExternalMagazineSystem : GunSystemBase {
        MagazineComponent mc = null;
        ExternalMagazineComponent emc = null;
        RecoilComponent rc = null;

        [GunSystemQuery(GunSystemQueries.IS_READY_TO_REMOVE_MAGAZINE)]
        bool IsReadyToRemoveMagazine() {
            return mc.ready_to_remove_mag;
        }

        [GunSystemQuery(GunSystemQueries.IS_MAGAZINE_EJECTING)]
        bool IsMagazineEjecting() {
            return mc.mag_stage == MagStage.REMOVING;
        }

        [GunSystemQuery(GunSystemQueries.IS_MAGAZINE_IN_GUN)]
        bool IsMagazineInGun() {
            return mc.mag_script != null;
        }

        [GunSystemRequest(GunSystemRequests.INPUT_EJECT_MAGAZINE)]
        public bool InputEjectMagazine() {
            gs.PlaySound(emc.sound_mag_eject_button);
            if (emc.can_eject && mc.mag_stage != MagStage.OUT) {
                mc.mag_stage = MagStage.REMOVING;
                gs.PlaySound(emc.sound_mag_ejection);
                return true;
            }
            return false;
        }

        [GunSystemRequest(GunSystemRequests.INPUT_INSERT_MAGAZINE)]
        public bool InputInsertMagazine() {
            if(!mc.mag_script) {
                return false; // No mag to push in
            }

            mc.mag_stage = MagStage.INSERTING;
            gs.PlaySound(emc.sound_mag_insertion);
            mc.mag_seated = 0.0f;
            return true;
        }

        public override void Initialize() {
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
        SlideComponent slide_c = null;
        private bool pushing = false;

        [GunSystemRequest(GunSystemRequests.INPUT_PUSH_SLIDE_FORWARD)]
        bool PushSlide() {
            slide_c.slide_stage = SlideStage.NOTHING;
            pushing = true;
            return true;
        }

        [GunSystemQuery(GunSystemQueries.IS_WAITING_FOR_SLIDE_PUSH)]
        bool IsWaitingForSlidePush() {
            return !slide_c.block_slide_pull && slide_c.slide_amount > 0f;
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
        SlideComponent slide_c = null;

        [GunSystemRequest(GunSystemRequests.INPUT_RELEASE_SLIDE)]
        bool InputReleaseSlide() {
            slide_c.slide_stage = SlideStage.NOTHING;
            return true;
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
        SlideComponent slide_c = null;
        ChamberComponent cc = null;

        public override void Initialize() {
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
        SlideComponent sc = null;
        ChamberComponent cc = null;
        MagazineComponent mc = null;

        public override void Initialize() {
            sc.should_slide_lock_predicates.Add(ShouldSlideLock);
        }

        private bool ShouldSlideLock() {
            return cc.active_round_state == RoundState.EMPTY && mc.mag_stage == MagStage.IN && mc.mag_script && mc.mag_script.NumRounds() <= 0;
        }
    }

    [InclusiveAspects(GunAspect.SLIDE, GunAspect.CHAMBER)]
    public class SlideChamberingSystem : GunSystemBase {
        SlideComponent slide_c = null;
        ChamberComponent chamber_c = null;

        public override void Initialize() {
            if(!slide_c.chamber_round)
                gs.gun_systems.UnloadSystem(this);
        }

        public override void Update() {
            if (!slide_c.slide_lock) {
                if(slide_c.chamber_on_pull) {
                    if(slide_c.slide_amount > slide_c.slide_chambering_position && slide_c.slide_stage != SlideStage.NOTHING) {
                        gs.ChamberRoundFromMag();
                    }
                } else if(slide_c.old_slide_amount > slide_c.slide_chambering_position && slide_c.slide_amount <= slide_c.slide_chambering_position) {
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
        SlideComponent sc = null;
        bool pressure_on_switch = false;

        [GunSystemRequest(GunSystemRequests.INPUT_RELEASE_SLIDE_LOCK)]
        bool InputReleaseSlideLock() {
            return gs.Request(GunSystemRequests.RELEASE_SLIDE_LOCK);
        }

        [GunSystemRequest(GunSystemRequests.APPLY_PRESSURE_ON_SLIDE_LOCK)]
        bool ApplyPressureToSlideLock() {
            pressure_on_switch = true;
            return true;
        }

        [GunSystemRequest(GunSystemRequests.RELEASE_PRESSURE_ON_SLIDE_LOCK)]
        bool ReleasePressureToSlideLock() {
            pressure_on_switch = false;
            return true;
        }

        public override void Initialize() {
            sc.should_slide_lock_predicates.Add(() => pressure_on_switch);
        }
    }

    [InclusiveAspects(GunAspect.SLIDE, GunAspect.SLIDE_LOCK)]
    public class SlideLockSystem : GunSystemBase {
        SlideComponent sc = null;

        [GunSystemQuery(GunSystemQueries.IS_SLIDE_LOCKED)]
        bool IsSlideLocked() {
            return sc.slide_lock;
        }

        [GunSystemRequest(GunSystemRequests.RELEASE_SLIDE_LOCK)]
        bool ReleaseSlideLock() {
            sc.slide_lock = false;
            return true;
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
        SlideComponent slide_c = null;

        [GunSystemQuery(GunSystemQueries.IS_PRESS_CHECK)]
        bool IsPressCheck() {
            return slide_c.slide_stage == SlideStage.HOLD && slide_c.slide_amount == slide_c.press_check_position;
        }

        [GunSystemRequest(GunSystemRequests.INPUT_PULL_SLIDE_PRESS_CHECK)]
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
    }

    [InclusiveAspects(GunAspect.SLIDE, GunAspect.FIRING)]
    public class SlideKickSystem : GunSystemBase {
        SlideComponent emc = null;
        FiringComponent fc = null;

        public override void Initialize() {
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
        SlideComponent sc = null;
        ChamberComponent cc = null;

        public override void Initialize() {
            cc.is_closed_predicates.Add(() => sc.slide_amount == 0);
        }
    }

    [InclusiveAspects(GunAspect.SLIDE)]
    public class SlideSystem : GunSystemBase {
        SlideComponent slide_c = null;

        [GunSystemQuery(GunSystemQueries.IS_SLIDE_PULLED_BACK)]
        bool IsSlidePulledBack() {
            return slide_c.slide_stage != SlideStage.NOTHING;
        }

        [GunSystemRequest(GunSystemRequests.INPUT_PULL_SLIDE_BACK)]
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

        [GunSystemRequest(GunSystemRequests.PULL_SLIDE_BACK)]
        bool RequestPullSlideBack() {
            slide_c.slide_amount = 1.0f;
            return true;
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
        AlternativeStanceComponent asc = null;

        [GunSystemRequest(GunSystemRequests.INPUT_TOGGLE_STANCE)]
        private bool InputToggleStance() {
            asc.is_alternative = !asc.is_alternative;
            return true;
        }

        [GunSystemQuery(GunSystemQueries.IS_IN_ALTERNATIVE_STANCE)]
        private bool IsInAlternativeStance() {
            return asc.is_alternative;
        }
    }

    [InclusiveAspects(GunAspect.TRIGGER, GunAspect.FIRING)]
    public class FireModeSystem : GunSystemBase {
        TriggerComponent tc = null;
        //FiringComponent fc = null; TODO This is never used, can GunAspect.FIRING be removed without consequences?

        int current_trigger_cycle = 0;

        public override void Update() {
            if(current_trigger_cycle != tc.trigger_cycle) {
                switch (tc.fire_mode) {
                    case FireMode.SINGLE:
                        tc.is_connected = true;
                        break;
                    case FireMode.BURST_THREE:
                        if(tc.trigger_cycle % 3 == 0)
                            tc.is_connected = true;
                        break;
                }
            }
            current_trigger_cycle = tc.trigger_cycle;
        }
    }

    [InclusiveAspects(GunAspect.FIRE_MODE, GunAspect.TRIGGER)]
    public class FireModeToggleSystem : GunSystemBase {
        FireModeComponent fmc = null;
        TriggerComponent tc = null;

        [GunSystemRequest(GunSystemRequests.TOGGLE_FIRE_MODE)]
        bool RequestToggleFireMode() {
            gs.PlaySound(fmc.sound_firemode_toggle);

            fmc.target_fire_mode_index++;
            if(fmc.target_fire_mode_index >= fmc.fire_modes.Length)
                fmc.target_fire_mode_index = 0;
            return true;
        }

        public override void Update() {
            if(fmc.target_fire_mode_index == fmc.current_fire_mode_index) { // idle
                tc.fire_mode = fmc.current_fire_mode;
            } else {
                tc.fire_mode = FireMode.DISABLED;

                var target = (float)fmc.target_fire_mode_index / (fmc.fire_modes.Length - 1);
                if(fmc.target_fire_mode_index > fmc.current_fire_mode_index) {
                    fmc.fire_mode_amount = Mathf.Min(target, fmc.fire_mode_amount + Time.deltaTime * 10.0f);
                } else {
                    fmc.fire_mode_amount = Mathf.Max(target, fmc.fire_mode_amount - Time.deltaTime * 10.0f);
                }

                if(fmc.fire_mode_amount == target) {
                    fmc.current_fire_mode_index = fmc.target_fire_mode_index;
                }
            }
        }
    }

    [InclusiveAspects(GunAspect.TRIGGER, GunAspect.HAMMER, GunAspect.CHAMBER)]
    [ExclusiveAspects(GunAspect.OPEN_BOLT_FIRING)]
    public class AutomaticFireSystem : GunSystemBase {
        TriggerComponent tc = null;
        ChamberComponent cc = null;
        HammerComponent hc = null;

        public override void Update() {
            if (!tc.is_connected && hc.thumb_on_hammer == Thumb.OFF_HAMMER && hc.hammer_cocked == 1.0f) {
                if (cc.is_closed) {
                    tc.trigger_cycle++;
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
        TriggerComponent tc = null;
        ChamberComponent cc = null;
        SlideComponent sc = null;

        public override void Update() {
            // Slide release
            if (!tc.is_connected) {
                tc.trigger_cycle++;
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
        SlideComponent sc = null;

        public override void Initialize() {
            sc.should_slide_lock_predicates.Add(() => true);
        }
    }

    [InclusiveAspects(GunAspect.REVOLVER_CYLINDER, GunAspect.MANUAL_LOADING)]
    public class CylinderLoadingSystem : GunSystemBase {
        RevolverCylinderComponent rcc = null;
        ManualLoadingComponent mlc = null;

        [GunSystemRequest(GunSystemRequests.INPUT_ADD_ROUND)]
        bool InputAddRoundToCylinder() {
            if (rcc.is_closed == mlc.load_when_closed) {
                
                int best_chamber = GetBestChamber(); // TODO
                if (best_chamber == -1) {
                    return false;
                }

                PutRoundInChamber(best_chamber);
                gs.PlaySound(mlc.sound_round_insertion);
                return true;

            }
            return false;
        }

        private int GetBestChamber() {
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
                if (!rcc.cylinders[check].game_object && IsChamberAccessible(check)) {
                    best_chamber = check;
                    break;
                }
            }

            return best_chamber;
        }

        private bool IsChamberAccessible(int chamber) {
            int which_chamber = (chamber - rcc.active_cylinder) % rcc.cylinder_capacity;
            if (which_chamber < 0) {
                which_chamber += rcc.cylinder_capacity;
            }
            return !mlc.inaccessabile_chamber_offsets.Contains(which_chamber);
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
        YokeComponent yc = null;

        [GunSystemRequest(GunSystemRequests.INPUT_CLOSE_CYLINDER)]
        bool InputCloseCylinder() {
            if (yc.yoke_stage == YokeStage.OPEN || yc.yoke_stage == YokeStage.OPENING) { // TODO add erc.extractor_rod_stage == ExtractorRodStage.CLOSED
                yc.yoke_stage = YokeStage.CLOSING;
                return true;
            }
            return false;
        }

        [GunSystemRequest(GunSystemRequests.INPUT_SWING_OUT_CYLINDER)]
        bool InputSwingOutCylinder() {
            if (yc.yoke_stage == YokeStage.CLOSED || yc.yoke_stage == YokeStage.CLOSING) {
                yc.yoke_stage = YokeStage.OPENING;
                return true;
            }
            return false;
        }

        [GunSystemQuery(GunSystemQueries.IS_CYLINDER_OPEN)]
        bool IsCylinderOpen() {
            return yc.yoke_stage == YokeStage.OPEN || yc.yoke_stage == YokeStage.OPENING;
        }

        [GunSystemQuery(GunSystemQueries.IS_ADDING_ROUNDS)]
        public bool IsAddingRounds() {
            if (yc.yoke_stage == YokeStage.OPEN) {
                return true;
            } else {
                return false;
            }
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

    [InclusiveAspects(GunAspect.REVOLVER_CYLINDER)]
    public class CylinderSpinSystem : GunSystemBase {
        RevolverCylinderComponent rcc = null;

        public void SpinCylinder(int amount) {
            rcc.target_cylinder_offset += amount * (Mathf.Max(1, Mathf.Abs(rcc.target_cylinder_offset)));
            rcc.target_cylinder_offset = Mathf.Max(-12, Mathf.Min(12, rcc.target_cylinder_offset));
        }

        public override void Initialize() {
            gs.gun_systems.spinCylinder = SpinCylinder;
        }
    }

    [InclusiveAspects(GunAspect.REVOLVER_CYLINDER, GunAspect.HAMMER)]
    public class CylinderHammerCycleSystem : GunSystemBase {
        RevolverCylinderComponent rcc = null;
        HammerComponent hc = null;

        public override void Initialize() {
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
        RevolverCylinderComponent rcc = null;
        SlideComponent sc = null;

        private bool reverse_direction = false;

        public override void Initialize() {
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
        RevolverCylinderComponent rcc = null;
        HammerComponent hc = null;

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
        RevolverCylinderComponent rcc = null;
        TriggerComponent tc = null;
        HammerComponent hc = null;

        public override void Update() {
            if (!tc.is_connected && hc.thumb_on_hammer == Thumb.OFF_HAMMER && hc.hammer_cocked == 1.0f) {
                hc.hammer_cocked = 0.0f;
                tc.trigger_cycle++;
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
        /// The gun trigger works based on triggers of regular firearms, "TriggerComponent.IsConnected" is the variable holding the hammer,
        /// when it returns FALSE, then the hammer should strike, depending on the firemode, it reconnects (in a firearm it reconnects on a separate location that prevents another pull)
        /// It only disconnects again, if the user stops applying pressure to the trigger and pulls it again.
        /// TriggerComponent.IsConnected is only FALSE, when the firemode systems and trigger systems determine that the gun should be able to fire!

        TriggerComponent tc = null;
        [GunSystemRequest(GunSystemRequests.APPLY_TRIGGER_PRESSURE)]
        public bool ApplyTriggerPressure() {
            if(tc.trigger_pressable) {
                tc.pressure_on_trigger = true;
                return true;
            }
            return false;
        }

        [GunSystemRequest(GunSystemRequests.RELEASE_TRIGGER_PRESSURE)]
        public bool ReleaseTriggerPressure() {
            tc.pressure_on_trigger = false;
            return true;
        }

        public override void Update() {
            tc.old_trigger_pressed = tc.trigger_pressed;
            if (tc.pressure_on_trigger) {
                tc.trigger_pressed = Mathf.Min(1.0f, tc.trigger_pressed + Time.deltaTime * 200.0f);
            } else {
                tc.trigger_pressed = Mathf.Max(0.0f, tc.trigger_pressed - Time.deltaTime * 20.0f);
            }

            if(tc.fire_mode != FireMode.DISABLED) {
                if(tc.trigger_pressed != 1f) {
                    tc.is_connected = true; // Guns usually need to cycle before connecting again
                } else if(tc.old_trigger_pressed != 1f) {
                    tc.is_connected = false; // We just pressed the trigger fully
                }
            }
        }
    }

    [InclusiveAspects(GunAspect.REVOLVER_CYLINDER, GunAspect.YOKE, GunAspect.YOKE_AUTO_EJECTOR)]
    public class YokeAutoEjectSystem : GunSystemBase {
        RevolverCylinderComponent rcc = null;
        YokeComponent yc = null;

        private bool triggered = true;

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
        RevolverCylinderComponent rcc = null;
        ExtractorRodComponent erc = null;

        [GunSystemRequest(GunSystemRequests.INPUT_USE_EXTRACTOR_ROD)]
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

        [GunSystemQuery(GunSystemQueries.IS_EJECTING_ROUNDS)]
        public bool IsEjectingRounds() {
            if (erc.extractor_rod_stage != ExtractorRodStage.CLOSED) {
                return true;
            }
            return false;
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
        TriggerComponent tc = null;
        HammerComponent hc = null;

        [GunSystemRequest(GunSystemRequests.INPUT_RELEASE_HAMMER)]
        bool RequestInputReleaseHammer() {
            if (tc.pressure_on_trigger || hc.hammer_cocked != 1.0f) {
                hc.thumb_on_hammer = Thumb.SLOW_LOWERING;
                tc.is_connected = true;
            } else {
                hc.thumb_on_hammer = Thumb.OFF_HAMMER;
            }
            return true;
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
        HammerComponent hc = null;

        [GunSystemQuery(GunSystemQueries.IS_HAMMER_COCKED)]
        bool IsHammerCocked() {
            return hc.hammer_cocked == 1.0f;
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
        TriggerComponent tc = null;
        HammerComponent hc = null;

        int current_trigger_cycle = 0;

        public override void Update() {
            if(!hc.is_blocked && CanPull() && !tc.is_connected) {
                if (hc.thumb_on_hammer == Thumb.OFF_HAMMER && tc.trigger_pressed == 1f) {
                    hc.thumb_on_hammer = Thumb.TRIGGER_PULLED;
                } else if (hc.thumb_on_hammer == Thumb.TRIGGER_PULLED && hc.hammer_cocked == 1.0f) {
                    hc.thumb_on_hammer = Thumb.OFF_HAMMER;
                }
            }

            if(!CanPull() && tc.is_connected)
                current_trigger_cycle = tc.trigger_cycle;
        }

        private bool CanPull() {
            return current_trigger_cycle == tc.trigger_cycle;
        }
    }

    [InclusiveAspects(GunAspect.HAMMER, GunAspect.THUMB_COCKING)]
    public class ThumbCockingSystem : GunSystemBase {
        HammerComponent hc = null;

        [GunSystemRequest(GunSystemRequests.INPUT_PRESSURE_ON_HAMMER)]
        bool RequestInputPressureOnHammer() {
            if(hc.is_blocked)
                return false;

            hc.thumb_on_hammer = Thumb.ON_HAMMER;
            return true;
        }
    }

    [InclusiveAspects(GunAspect.HAMMER, GunAspect.SLIDE, GunAspect.SLIDE_COCKING)]
    public class SlideCockingSystem : GunSystemBase {
        SlideComponent sc = null;
        HammerComponent hc = null;

        public override void Initialize() {
            sc = gs.GetComponent<SlideComponent>();
            hc = gs.GetComponent<HammerComponent>();
        }

        public override void Update() {
            if(!hc.is_blocked) {
                hc.hammer_cocked = Mathf.Max(hc.hammer_cocked, Mathf.Clamp01(sc.slide_amount / sc.slide_cock_position));
            }
        }
    }

    [InclusiveAspects(GunAspect.SLIDE, GunAspect.LOCKABLE_BOLT)]
    public class SlideBoltBlockSystem : GunSystemBase {
        LockableBoltComponent bc = null;
        SlideComponent sc = null;

        public override void Initialize() {
            bc.block_toggle_predicates.Add(() => sc.slide_amount > 0f || sc.slide_stage == SlideStage.PULLBACK);
        }
    }

    [InclusiveAspects(GunAspect.LOCKABLE_BOLT)]
    public class LockableBoltSystem : GunSystemBase {
        LockableBoltComponent bc = null;

        [GunSystemRequest(GunSystemRequests.INPUT_TOGGLE_BOLT_LOCK)]
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

        public override void Initialize() {
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
        RevolverCylinderComponent rcc = null;

        [GunSystemRequest(GunSystemRequests.SPEND_ROUND)]
        public bool SpendRound() {
            if(Cheats.infinite_ammo)
                return true;

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

        [GunSystemRequest(GunSystemRequests.DESTROY_ROUND)]
        public bool DestroyRound() {
            int which_chamber = rcc.active_cylinder % rcc.cylinder_capacity;
            if (which_chamber < 0) {
                which_chamber += rcc.cylinder_capacity;
            }

            GameObject round = rcc.cylinders[which_chamber].game_object;
            if (round) {
                rcc.cylinders[which_chamber].can_fire = false;
                GameObject.Destroy(round);

                return true;
            }
            return false;
        }
    }

    [InclusiveAspects(GunAspect.CHAMBER)]
    public class RoundSpendingChamberSystem : GunSystemBase {
        ChamberComponent cc = null;

        [GunSystemRequest(GunSystemRequests.SPEND_ROUND)]
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

        [GunSystemRequest(GunSystemRequests.DESTROY_ROUND)]
        public bool DestroyRound() {
            if(cc.active_round != null) {
                cc.active_round_state = RoundState.EMPTY;

                GameObject.Destroy(cc.active_round);
                return true;
            }
            return false;
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
        FiringComponent fc = null;

        [GunSystemRequest(GunSystemRequests.DISCHARGE)]
        public bool Discharge() {
            GameObject bullet = null;
            gs.PlaySound(fc.sound_gunshot_smallroom, 1.0f);
            
            if(fc.caseless_ammunition) {
                gs.Request(GunSystemRequests.DESTROY_ROUND);
            } else {
                gs.Request(GunSystemRequests.SPEND_ROUND);
            }

            for(var i = 0; i < fc.projectile_count; i++) {
                GameObject.Instantiate(fc.muzzle_flash, fc.point_muzzleflash.position, fc.point_muzzleflash.rotation);
                bullet = (GameObject)GameObject.Instantiate(fc.bullet_obj, fc.point_muzzle.position, fc.point_muzzle.rotation);

                if(fc.inaccuracy != 0f) {
                    float angle = UnityEngine.Random.Range(0, 6.283f);
                    float radius = UnityEngine.Random.Range(0, fc.inaccuracy);
                    bullet.transform.Rotate(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
                }

                if(bullet.GetComponent<BulletScript>()) {
                    bullet.GetComponent<BulletScript>().SetVelocity(bullet.transform.forward * fc.exit_velocity);
                } else if(bullet.GetComponent<ProjectileScript>()) {
                    bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * fc.exit_velocity, ForceMode.Impulse);
                    Debug.DrawLine(bullet.transform.position, bullet.transform.position + bullet.transform.forward * 20, Color.magenta, 10f);
                }
            }

            fc.fire_count++;
            return true;
        }
    }

    [InclusiveAspects(GunAspect.RECOIL)]
    public class RecoilSystem : GunSystemBase {
        RecoilComponent rc = null;

        [GunSystemRequest(GunSystemRequests.RESET_RECOIL)]
        public bool ResetRecoil() {
            rc.recoil_transfer_x = 0;
            rc.recoil_transfer_y = 0;
            rc.rotation_transfer_x = 0;
            rc.rotation_transfer_y = 0;
            rc.add_head_recoil = false;
            return true;
        }

        public Vector2 GetRecoilTransfer() {
            return new Vector2(rc.recoil_transfer_x, rc.recoil_transfer_y);
        }

        public Vector2 GetRecoilRotation() {
            return new Vector2(rc.rotation_transfer_x, rc.rotation_transfer_y);
        }

        public bool AddHeadRecoil() {
            return rc.add_head_recoil;
        }

        public override void Initialize() {
            gs.gun_systems.getRecoilTransfer = GetRecoilTransfer;
            gs.gun_systems.getRecoilRotation = GetRecoilRotation;
            gs.gun_systems.addHeadRecoil = AddHeadRecoil;
        }
    }

    [InclusiveAspects(GunAspect.RECOIL, GunAspect.FIRING)]
    public class FiringRecoilSystem : GunSystemBase {
        FiringComponent fc = null;
        RecoilComponent rc = null;

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
        AimSwayComponent asc = null;
        RecoilComponent rc = null;

        public override void Update() {
            rc.recoil_transfer_x -= asc.horizontal_strength * Mathf.Sin(4 * Time.time * asc.speed + (float)Math.PI / 4f) * Time.deltaTime;
            rc.recoil_transfer_y -= asc.vertical_strength * Mathf.Sin(3 * Time.time * asc.speed) * Time.deltaTime;
        }
    }

    public class GunSystemRequestAttribute : Attribute {
        public GunSystemRequests request;
        public GunSystemRequestAttribute(GunSystemRequests request) {
            this.request = request;
        }
    }

    public class GunSystemQueryAttribute : Attribute {
        public GunSystemQueries query;
        public GunSystemQueryAttribute(GunSystemQueries query) {
            this.query = query;
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

                        Dictionary<GunSystemRequests, GunSystemBase.GunSystemRequest> requests = GetSystemRequests(gsb, type);
                        Dictionary<GunSystemQueries, GunSystemBase.GunSystemQuery> queries = GetSystemQueries(gsb, type);

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
                        
                        // Fill component data on GunSystems
                        foreach(FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)) {
                            if(typeof(GunComponent).IsAssignableFrom(field.FieldType)) {
                                field.SetValue(gsb, gs.GetComponent(field.FieldType));
                            }
                        }

                        loaded_systems.Add(gsb, GetPriority(type));
                    }
                }
            }

            // Add systems in order of their priority attribute
            systems = (from item in loaded_systems orderby item.Value select item.Key).ToArray();
        }
        
        /// <summary> Returns a dictionary with each request a system has specified </summary>
        public Dictionary<GunSystemRequests, GunSystemBase.GunSystemRequest> GetSystemRequests(GunSystemBase system, Type type) {
            var requests = new Dictionary<GunSystemRequests, GunSystemBase.GunSystemRequest>();
            
            foreach(MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)) {
                GunSystemRequestAttribute att = method.GetCustomAttribute<GunSystemRequestAttribute>();
                if(att != null) {
                    requests.Add(att.request, (GunSystemBase.GunSystemRequest) Delegate.CreateDelegate(typeof(GunSystemBase.GunSystemRequest), system, method));
                }
            }

            return requests;
        }

        /// <summary> Returns a dictionary with each query a system has specified </summary>
        public Dictionary<GunSystemQueries, GunSystemBase.GunSystemQuery> GetSystemQueries(GunSystemBase system, Type type) {
            var queries = new Dictionary<GunSystemQueries, GunSystemBase.GunSystemQuery>();
            
            foreach(MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)) {
                GunSystemQueryAttribute att = method.GetCustomAttribute<GunSystemQueryAttribute>();
                if(att != null) {
                    queries.Add(att.query, (GunSystemBase.GunSystemQuery) Delegate.CreateDelegate(typeof(GunSystemBase.GunSystemQuery), system, method));
                }
            }

            return queries;
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