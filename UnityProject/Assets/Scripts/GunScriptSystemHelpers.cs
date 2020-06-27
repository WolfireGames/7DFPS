using System.Linq;
using GunSystemInterfaces;

namespace GunSystemsV1 {
    [InclusiveAspects(GunAspect.MAGAZINE, GunAspect.EXTERNAL_MAGAZINE)]
    public class ExternalMagazineHelperSystem : GunSystemBase {
        MagazineComponent mc = null;
        ExternalMagazineComponent emc = null;

        [GunSystemQuery(GunSystemQueries.SHOULD_EJECT_MAGAZINE)]
        public bool ShouldEjectMagazine() {
            return mc.mag_script && mc.mag_script.NumRounds() == 0 && emc.can_eject;
        }
    }

    [InclusiveAspects(GunAspect.SLIDE, GunAspect.CHAMBER, GunAspect.SLIDE_PUSHING)]
    [ExclusiveAspects(GunAspect.SLIDE_SPRING)]
    public class SlidePushingHelperSystem : GunSystemBase {
        SlideComponent sc = null;
        ChamberComponent cc = null;

        // Optional
        MagazineComponent mc = null;
        ManualLoadingComponent mlc = null;

        [GunSystemQuery(GunSystemQueries.SHOULD_PUSH_SLIDE)]
        bool ShouldPushSlide() {
            if(sc.block_slide_pull)
                return false; // Don't push if blocked

            if(sc.slide_amount <= 0f)
                return false; // Don't don't push if already pushed

            // Push if: (Round is waiting) OR (We got a mag with rounds) OR (Manual Loading requires a closed chamber)
            return (cc.active_round_state == RoundState.LOADING || cc.active_round_state == RoundState.READY) || (mc && mc.mag_script && mc.mag_script.NumRounds() > 0) || (mlc && mlc.load_when_closed);
        }
    }

    [InclusiveAspects(GunAspect.SLIDE, GunAspect.SLIDE_LOCK)]
    public class SlideLockHelperSystem : GunSystemBase {
        SlideComponent sc = null;

        [GunSystemQuery(GunSystemQueries.SHOULD_RELEASE_SLIDE_LOCK)]
        public bool ShouldReleaseSlideLock() {
            return sc.slide_lock;
        }
    }

    [InclusiveAspects(GunAspect.SLIDE, GunAspect.CHAMBER)]
    public class SlideHelperSystem : GunSystemBase {
        SlideComponent sc = null;
        ChamberComponent cc = null;

        // Optional
        MagazineComponent mc = null;
        ManualLoadingComponent mlc = null;

        [GunSystemQuery(GunSystemQueries.SHOULD_PULL_SLIDE)]
        bool ShouldPullSlide() {
            if(sc.block_slide_pull)
                return false; // Don't pull if blocked

            if(cc.active_round_state == RoundState.READY || cc.active_round_state == RoundState.LOADING)
                return false; // Don't pull if round ready

            // Pull if: (We got a mag with rounds) OR (Manual Loading requires an open chamber)
            return (mc && mc.mag_script && mc.mag_script.NumRounds() > 0) || (mlc && !mlc.load_when_closed);
        }
    }

    [InclusiveAspects(GunAspect.ALTERNATIVE_STANCE)]
    public class StanceHelperSystem : GunSystemBase {
        AlternativeStanceComponent asc = null;

        // Optional
        SlideComponent sc = null;
        MagazineComponent mc = null;
        ChamberComponent cc = null;
        LockableBoltComponent lbc = null;
        ManualLoadingComponent mlc = null;
        RevolverCylinderComponent rcc = null;

        [GunSystemQuery(GunSystemQueries.SHOULD_TOGGLE_STANCE)]
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
    }

    [InclusiveAspects(GunAspect.LOCKABLE_BOLT, GunAspect.CHAMBER)]
    public class BoltHelperSystem : GunSystemBase {
        LockableBoltComponent lbc = null;
        ChamberComponent cc = null;

        [GunSystemQuery(GunSystemQueries.SHOULD_TOGGLE_BOLT)]
        bool ShouldToggleBolt() {
            return !lbc.block_toggle && (
                cc.active_round_state != RoundState.READY && lbc.bolt_stage == BoltActionStage.LOCKED ||
                cc.active_round_state == RoundState.READY && lbc.bolt_stage == BoltActionStage.UNLOCKED
            );
        }
    }

    [InclusiveAspects(GunAspect.FIRE_MODE)]
    public class FireModeHelperSystem : GunSystemBase {
        FireModeComponent fmc = null;

        [GunSystemQuery(GunSystemQueries.SHOULD_TOGGLE_FIRE_MODE)]
        bool ShouldToggleFireMode() {
            return fmc.current_fire_mode == FireMode.AUTOMATIC || fmc.current_fire_mode == FireMode.DISABLED;
        }
    }

    [InclusiveAspects(GunAspect.MANUAL_LOADING, GunAspect.CHAMBER)]
    public class ManualLoadingHelperSystem : GunSystemBase {
        ManualLoadingComponent mlc = null;
        ChamberComponent cc = null;

        // Optional
        MagazineComponent mc = null;

        [GunSystemQuery(GunSystemQueries.SHOULD_INSERT_BULLET)]
        bool ShouldInsertBullet() {
            if(!mlc.can_insert)
                return false;

            if(!mlc.mag_insert)
                return cc.active_round_state == RoundState.EMPTY;

            return (!mc || mc.mag_script && mc.mag_script.NumRounds() <= 0) && cc.active_round_state == RoundState.EMPTY;
        }
    }

    [InclusiveAspects(GunAspect.REVOLVER_CYLINDER)]
    public class CylinderHelperSystem : GunSystemBase {
        RevolverCylinderComponent rcc = null;

        [GunSystemQuery(GunSystemQueries.SHOULD_INSERT_BULLET)]
        bool ShouldInsertBullet() {
            return rcc.cylinders.Any((cylinder) => !cylinder.game_object);
        }

        [GunSystemQuery(GunSystemQueries.SHOULD_EXTRACT_CASINGS)]
        bool ShouldExtractCasings() {
            return rcc.cylinders.Any((cylinder) => cylinder.game_object && !cylinder.can_fire);
        }

        [GunSystemQuery(GunSystemQueries.SHOULD_CLOSE_CYLINDER)]
        bool ShouldCloseCylinder() {
            return !rcc.cylinders.Any((cylinder) => !cylinder.can_fire);
        }

        [GunSystemQuery(GunSystemQueries.SHOULD_OPEN_CYLINDER)]
        bool ShouldOpenCylinder() {
            return rcc.cylinders.Any((cylinder) => !cylinder.can_fire);
        }
    }

    [InclusiveAspects(GunAspect.HAMMER, GunAspect.THUMB_COCKING)]
    public class HammerHelperSystem : GunSystemBase {
        HammerComponent hc = null;

        [GunSystemQuery(GunSystemQueries.SHOULD_PULL_BACK_HAMMER)]
        bool ShouldPullBackHammer() {
            return hc.hammer_cocked != 1.0f;
        }
    }
}