using System.Collections.Generic;
using System.Linq;

namespace GunSystemsV1 {
    [InclusiveAspects(GunAspect.MAGAZINE, GunAspect.EXTERNAL_MAGAZINE)]
    public class ExternalMagazineHelperSystem : GunSystemBase {
        MagazineComponent mc;

        public bool ShouldEjectMagazine() {
            return (mc.mag_script && mc.mag_script.NumRounds() == 0);
        }

        public override Dictionary<GunSystemQueries, GunSystemQuery> GetPossibleQuestions() {
            return new Dictionary<GunSystemQueries, GunSystemQuery>() {
                {GunSystemQueries.SHOULD_EJECT_MAGAZINE, ShouldEjectMagazine},
            };
        }

        public override void Initialize() {
            mc = gs.GetComponent<MagazineComponent>();
        }
    }

    [InclusiveAspects(GunAspect.SLIDE, GunAspect.SLIDE_PUSHING)]
    [ExclusiveAspects(GunAspect.SLIDE_SPRING)]
    public class SlidePushingHelperSystem : GunSystemBase {
        SlideComponent slide_c;

        bool ShouldPushSlide() {
            return slide_c.slide_amount > 0f;
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
        SlideComponent slide_c; //unused, but closely related
        MagazineComponent mc;
        ChamberComponent cc;

        bool ShouldPullSlide() {
            return (cc.active_round_state == RoundState.EMPTY || cc.active_round_state == RoundState.FIRED) && mc.mag_script && mc.mag_script.NumRounds() > 0;
        }

        public override Dictionary<GunSystemQueries, GunSystemQuery> GetPossibleQuestions() {
            return new Dictionary<GunSystemQueries, GunSystemQuery>() {
                {GunSystemQueries.SHOULD_PULL_SLIDE, ShouldPullSlide},
            };
        }

        public override void Initialize() {
            mc = gs.GetComponent<MagazineComponent>();
            cc = gs.GetComponent<ChamberComponent>();
        }
    }

    [InclusiveAspects(GunAspect.ALTERNATIVE_STANCE)]
    public class StanceHelperSystem : GunSystemBase {
        AlternativeStanceComponent asc;

        bool ShouldToggleStance() { // This function can probably be optimized a bit
            if(asc.is_alternative) {
                if(asc.alt_stance_blocks_bolt && gs.ShouldToggleBolt())
                    return true;
                else if(asc.alt_stance_blocks_slide && (gs.ShouldPullSlide() || gs.ShouldPushSlideForward()))
                    return true;
                else if(asc.alt_stance_blocks_mag && (gs.ShouldEjectMag() || gs.ShouldInsertBullet()))
                    return true;
                else if(asc.alt_stance_blocks_trigger && !(gs.ShouldPullSlide() || gs.ShouldPushSlideForward() || gs.ShouldToggleBolt() || gs.ShouldEjectMag() || gs.ShouldInsertBullet()))
                    return true;
            } else {
                if(asc.stance_blocks_bolt && gs.ShouldToggleBolt())
                    return true;
                else if(asc.stance_blocks_slide && (gs.ShouldPullSlide() || gs.ShouldPushSlideForward()))
                    return true;
                else if(asc.stance_blocks_mag && (gs.ShouldEjectMag() || gs.ShouldInsertBullet()))
                    return true;
                else if(asc.stance_blocks_trigger && !(gs.ShouldPullSlide() || gs.ShouldPushSlideForward() || gs.ShouldToggleBolt() || gs.ShouldEjectMag() || gs.ShouldInsertBullet()))
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
        }
    }

    [InclusiveAspects(GunAspect.LOCKABLE_BOLT)]
    public class BoltHelperSystem : GunSystemBase {
        LockableBoltComponent lbc;

        bool ShouldToggleBolt() {
            return lbc.bolt_stage != BoltActionStage.UNLOCKED && gs.ShouldPullSlide() || lbc.bolt_stage == BoltActionStage.UNLOCKED && (!gs.ShouldPullSlide() && !gs.ShouldPushSlideForward());
        }

        public override Dictionary<GunSystemQueries, GunSystemQuery> GetPossibleQuestions() {
            return new Dictionary<GunSystemQueries, GunSystemQuery>() {
                {GunSystemQueries.SHOULD_TOGGLE_BOLT, ShouldToggleBolt},
            };
        }

        public override void Initialize() {
            lbc = gs.GetComponent<LockableBoltComponent>();
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
            if(mlc.load_when_closed != cc.is_closed)
                return false;

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