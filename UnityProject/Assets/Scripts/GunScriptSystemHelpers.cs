using System.Collections.Generic;
using System.Linq;

namespace GunSystemsV1 {
    [InclusiveAspects(GunAspect.MAGAZINE, GunAspect.EXTERNAL_MAGAZINE)]
    public class ExternalMagazineHelperSystem : GunSystemBase {
        MagazineComponent mc;
        ExternalMagazineComponent emc;

        public bool ShouldEjectMagazine() {
            return mc.mag_script && mc.mag_script.NumRounds() == 0 && emc.can_eject;
        }

        public override Dictionary<GunSystemQueries, GunSystemQuery> GetPossibleQuestions() {
            return new Dictionary<GunSystemQueries, GunSystemQuery>() {
                {GunSystemQueries.SHOULD_EJECT_MAGAZINE, ShouldEjectMagazine},
            };
        }

        public override void Initialize() {
            mc = gs.GetComponent<MagazineComponent>();
            emc = gs.GetComponent<ExternalMagazineComponent>();
        }
    }

    [InclusiveAspects(GunAspect.SLIDE, GunAspect.SLIDE_PUSHING)]
    [ExclusiveAspects(GunAspect.SLIDE_SPRING)]
    public class SlidePushingHelperSystem : GunSystemBase {
        SlideComponent slide_c;

        bool ShouldPushSlide() {
            return !slide_c.block_slide_pull && slide_c.slide_amount > 0f;
        }

        public override Dictionary<GunSystemQueries, GunSystemQuery> GetPossibleQuestions() {
            return new Dictionary<GunSystemQueries, GunSystemQuery>() {
                {GunSystemQueries.SHOULD_PUSH_SLIDE, ShouldPushSlide},
            };
        }

        public override void Initialize() {
            slide_c = gs.GetComponent<SlideComponent>();
        }
    }

    [InclusiveAspects(GunAspect.SLIDE, GunAspect.SLIDE_LOCK)]
    public class SlideLockHelperSystem : GunSystemBase {
        SlideComponent sc;

        public bool ShouldReleaseSlideLock() {
            return sc.slide_lock;
        }

        public override void Initialize() {
            sc = gs.GetComponent<SlideComponent>();
        }

        public override Dictionary<GunSystemQueries, GunSystemQuery> GetPossibleQuestions() {
            return new Dictionary<GunSystemQueries, GunSystemQuery>() {
                {GunSystemQueries.SHOULD_RELEASE_SLIDE_LOCK, ShouldReleaseSlideLock},
            };
        }
    }

    [InclusiveAspects(GunAspect.SLIDE, GunAspect.MAGAZINE, GunAspect.CHAMBER)]
    public class SlideHelperSystem : GunSystemBase {
        SlideComponent sc;
        MagazineComponent mc;
        ChamberComponent cc;

        bool ShouldPullSlide() {
            return !sc.block_slide_pull && (cc.active_round_state == RoundState.EMPTY || cc.active_round_state == RoundState.FIRED) && mc.mag_script && mc.mag_script.NumRounds() > 0;
        }

        public override Dictionary<GunSystemQueries, GunSystemQuery> GetPossibleQuestions() {
            return new Dictionary<GunSystemQueries, GunSystemQuery>() {
                {GunSystemQueries.SHOULD_PULL_SLIDE, ShouldPullSlide},
            };
        }

        public override void Initialize() {
            sc = gs.GetComponent<SlideComponent>();
            mc = gs.GetComponent<MagazineComponent>();
            cc = gs.GetComponent<ChamberComponent>();
        }
    }

    [InclusiveAspects(GunAspect.ALTERNATIVE_STANCE)]
    public class StanceHelperSystem : GunSystemBase {
        AlternativeStanceComponent asc;

        // Optional
        SlideComponent sc;
        MagazineComponent mc;
        ChamberComponent cc;
        LockableBoltComponent lbc;
        ManualLoadingComponent mlc;
        RevolverCylinderComponent rcc;

        bool ShouldToggleStance() {
            // Find out what we want to do, this isn't a pretty approach, but it doesn't lock up other components
            bool wants_bolt_toggled = cc && lbc && (
                (cc.active_round_state == RoundState.READY) == (lbc.bolt_stage == BoltActionStage.UNLOCKED)
            );

            bool wants_slide_moved = cc && sc && (
                cc.active_round_state != RoundState.READY ||
                (gs.HasGunComponent(GunAspect.SLIDE_PUSHING) && sc.slide_amount > 0f)
            );

            bool wants_mag_ejected = mc && cc && (
                cc.active_round_state == RoundState.EMPTY && mc.mag_script && mc.mag_script.NumRounds() == 0
            );

            bool wants_round_inserted = mlc && (
                !mlc.mag_insert && cc.active_round_state == RoundState.EMPTY ||
                rcc && rcc.cylinders.Any((cylinder) => !cylinder.game_object)
            );

            // Find out if we need to toggle for the things we want to do
            if(asc.is_alternative) {
                if(asc.alt_stance_blocks_bolt && wants_bolt_toggled)
                    return true;
                else if(asc.alt_stance_blocks_slide && wants_slide_moved)
                    return true;
                else if(asc.alt_stance_blocks_mag && (wants_mag_ejected || wants_round_inserted))
                    return true;
                else if(asc.alt_stance_blocks_trigger && !(wants_slide_moved || wants_bolt_toggled || wants_mag_ejected || wants_round_inserted))
                    return true;
            } else {
                if(asc.stance_blocks_bolt && wants_bolt_toggled)
                    return true;
                else if(asc.stance_blocks_slide && wants_slide_moved)
                    return true;
                else if(asc.stance_blocks_mag && (wants_mag_ejected || wants_round_inserted))
                    return true;
                else if(asc.stance_blocks_trigger && !(wants_slide_moved || wants_bolt_toggled || wants_mag_ejected || wants_round_inserted))
                    return true;
            }
            return false;
        }

        public override Dictionary<GunSystemQueries, GunSystemQuery> GetPossibleQuestions() {
            return new Dictionary<GunSystemQueries, GunSystemQuery>() {
                {GunSystemQueries.SHOULD_TOGGLE_STANCE, ShouldToggleStance},
            };
        }

        public override void Initialize() {
            asc = gs.GetComponent<AlternativeStanceComponent>();
            sc = gs.GetComponent<SlideComponent>();
            mc = gs.GetComponent<MagazineComponent>();
            cc = gs.GetComponent<ChamberComponent>();
            lbc = gs.GetComponent<LockableBoltComponent>();
            mlc = gs.GetComponent<ManualLoadingComponent>();
            rcc = gs.GetComponent<RevolverCylinderComponent>();
        }
    }

    [InclusiveAspects(GunAspect.LOCKABLE_BOLT, GunAspect.CHAMBER)]
    public class BoltHelperSystem : GunSystemBase {
        LockableBoltComponent lbc;
        ChamberComponent cc;

        bool ShouldToggleBolt() {
            return !lbc.block_toggle && (
                cc.active_round_state != RoundState.READY && lbc.bolt_stage == BoltActionStage.LOCKED ||
                cc.active_round_state == RoundState.READY && lbc.bolt_stage == BoltActionStage.UNLOCKED
            );
        }

        public override Dictionary<GunSystemQueries, GunSystemQuery> GetPossibleQuestions() {
            return new Dictionary<GunSystemQueries, GunSystemQuery>() {
                {GunSystemQueries.SHOULD_TOGGLE_BOLT, ShouldToggleBolt},
            };
        }

        public override void Initialize() {
            lbc = gs.GetComponent<LockableBoltComponent>();
            cc = gs.GetComponent<ChamberComponent>();
        }
    }

    [InclusiveAspects(GunAspect.FIRE_MODE)]
    public class FireModeHelperSystem : GunSystemBase {
        FireModeComponent fmc;

        bool ShouldToggleAutoMode() {
            return fmc.auto_mod_stage == AutoModStage.ENABLED;
        }

        public override Dictionary<GunSystemQueries, GunSystemQuery> GetPossibleQuestions() {
            return new Dictionary<GunSystemQueries, GunSystemQuery>() {
                {GunSystemQueries.SHOULD_TOGGLE_AUTO_MODE, ShouldToggleAutoMode}
            };
        }

        public override void Initialize() {
            fmc = gs.GetComponent<FireModeComponent>();
        }
    }

    [InclusiveAspects(GunAspect.MANUAL_LOADING, GunAspect.MAGAZINE, GunAspect.CHAMBER)]
    public class ManualLoadingHelperSystem : GunSystemBase {
        ManualLoadingComponent mlc;
        MagazineComponent mc;
        ChamberComponent cc;

        bool ShouldInsertBullet() {
            if(!mlc.can_insert)
                return false;

            if(!mlc.mag_insert)
                return cc.active_round_state == RoundState.EMPTY;

            return mc.mag_script && mc.mag_script.NumRounds() <= 0;
        }

        public override Dictionary<GunSystemQueries, GunSystemQuery> GetPossibleQuestions() {
            return new Dictionary<GunSystemQueries, GunSystemQuery>() {
                {GunSystemQueries.SHOULD_INSERT_BULLET, ShouldInsertBullet},
            };
        }

        public override void Initialize() {
            mlc = gs.GetComponent<ManualLoadingComponent>();
            mc = gs.GetComponent<MagazineComponent>();
            cc = gs.GetComponent<ChamberComponent>();
        }
    }

    [InclusiveAspects(GunAspect.REVOLVER_CYLINDER)]
    public class CylinderHelperSystem : GunSystemBase {
        RevolverCylinderComponent rcc;

        bool ShouldInsertBullet() {
            return rcc.cylinders.Any((cylinder) => !cylinder.game_object);
        }

        bool ShouldExtractCasings() {
            return rcc.cylinders.Any((cylinder) => cylinder.game_object && !cylinder.can_fire);
        }

        bool ShouldCloseCylinder() {
            return !rcc.cylinders.Any((cylinder) => !cylinder.can_fire);
        }

        bool ShouldOpenCylinder() {
            return rcc.cylinders.Any((cylinder) => !cylinder.can_fire);
        }

        public override Dictionary<GunSystemQueries, GunSystemQuery> GetPossibleQuestions() {
            return new Dictionary<GunSystemQueries, GunSystemQuery>() {
                {GunSystemQueries.SHOULD_OPEN_CYLINDER, ShouldOpenCylinder},
                {GunSystemQueries.SHOULD_CLOSE_CYLINDER, ShouldCloseCylinder},
                {GunSystemQueries.SHOULD_EXTRACT_CASINGS, ShouldExtractCasings},
                {GunSystemQueries.SHOULD_INSERT_BULLET, ShouldInsertBullet},
            };
        }

        public override void Initialize() {
            rcc = gs.GetComponent<RevolverCylinderComponent>();
        }
    }

    [InclusiveAspects(GunAspect.HAMMER, GunAspect.THUMB_COCKING)]
    public class HammerHelperSystem : GunSystemBase {
        HammerComponent hc;

        bool ShouldPullBackHammer() {
            return hc.hammer_cocked != 1.0f;
        }

        public override Dictionary<GunSystemQueries, GunSystemQuery> GetPossibleQuestions() {
            return new Dictionary<GunSystemQueries, GunSystemQuery>() {
                {GunSystemQueries.SHOULD_PULL_BACK_HAMMER, ShouldPullBackHammer}
            };
        }

        public override void Initialize() {
            hc = gs.GetComponent<HammerComponent>();
        }
    }
}