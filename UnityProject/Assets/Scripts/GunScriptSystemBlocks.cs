using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using ExtentionUtil;

namespace GunSystemsV1 {
    [InclusiveAspects(GunAspect.TRIGGER, GunAspect.THUMB_SAFETY)]
    public class ThumbSafetyTriggerBlockSystem : GunSystemBase {
        ThumbSafetyComponent tsc;
        TriggerComponent tc;

        public override void Initialize() {
            if(tsc.block_trigger) {
                tc.trigger_pressable_predicates.Add(() => !tsc.is_safe);
            }
        }
    }

    [InclusiveAspects(GunAspect.SLIDE, GunAspect.THUMB_SAFETY)]
    public class ThumbSafetySlideBlockSystem : GunSystemBase {
        ThumbSafetyComponent tsc;
        SlideComponent sc;

        public override void Initialize() {
            if(tsc.block_slide) {
                sc.block_slide_pull_predicates.Add(() => tsc.is_safe);
            }
        }
    }

    [InclusiveAspects(GunAspect.TRIGGER, GunAspect.GRIP_SAFETY)]
    public class GripSafetyTriggerBlockSystem : GunSystemBase {
        GripSafetyComponent gsc;
        TriggerComponent tc;

        public override void Initialize() {
            if(gsc.block_trigger) {
                tc.trigger_pressable_predicates.Add(() => !gsc.is_safe);
            }
        }
    }

    [InclusiveAspects(GunAspect.SLIDE, GunAspect.GRIP_SAFETY)]
    public class GripSafetySlideBlockSystem : GunSystemBase {
        GripSafetyComponent gsc;
        SlideComponent sc;

        public override void Initialize() {
            if(gsc.block_slide) {
                sc.block_slide_pull_predicates.Add(() => gsc.is_safe);
            }
        }
    }

    [InclusiveAspects(GunAspect.ALTERNATIVE_STANCE, GunAspect.TRIGGER)]
    public class StanceTriggerBlockSystem : GunSystemBase {
        AlternativeStanceComponent asc;
        TriggerComponent tc;

        public override void Initialize() {
            if(asc.alt_stance_blocks_trigger)
                tc.trigger_pressable_predicates.Add( () => !asc.is_alternative);

            if(asc.stance_blocks_trigger)
                tc.trigger_pressable_predicates.Add( () => asc.is_alternative);
        }
    }

    [InclusiveAspects(GunAspect.ALTERNATIVE_STANCE, GunAspect.SLIDE)]
    public class StanceSlideBlockSystem : GunSystemBase {
        AlternativeStanceComponent asc;
        SlideComponent sc;

        public override void Initialize() {
            if(asc.alt_stance_blocks_slide)
                sc.block_slide_pull_predicates.Add( () => asc.is_alternative);

            if(asc.stance_blocks_slide)
                sc.block_slide_pull_predicates.Add( () => !asc.is_alternative);
        }
    }

    [InclusiveAspects(GunAspect.ALTERNATIVE_STANCE, GunAspect.LOCKABLE_BOLT)]
    public class StanceBoltBlockSystem : GunSystemBase {
        AlternativeStanceComponent asc;
        LockableBoltComponent lbc;

        public override void Initialize() {
            if(asc.alt_stance_blocks_bolt)
                lbc.block_toggle_predicates.Add( () => asc.is_alternative);

            if(asc.stance_blocks_bolt)
                lbc.block_toggle_predicates.Add( () => !asc.is_alternative);
        }
    }

    [InclusiveAspects(GunAspect.ALTERNATIVE_STANCE, GunAspect.EXTERNAL_MAGAZINE)]
    public class StanceMagazineBlockSystem : GunSystemBase {
        AlternativeStanceComponent asc;
        ExternalMagazineComponent emc;

        public override void Initialize() {
            if(asc.alt_stance_blocks_mag)
                emc.can_eject_predicates.Add( () => !asc.is_alternative);

            if(asc.stance_blocks_mag)
                emc.can_eject_predicates.Add( () => asc.is_alternative);
        }
    }

    [InclusiveAspects(GunAspect.ALTERNATIVE_STANCE, GunAspect.MANUAL_LOADING)]
    public class StanceManualLoadingBlockSystem : GunSystemBase {
        AlternativeStanceComponent asc;
        ManualLoadingComponent mlc;

        public override void Initialize() {
            if(asc.alt_stance_blocks_mag)
                mlc.can_insert_predicates.Add( () => !asc.is_alternative);

            if(asc.stance_blocks_mag)
                mlc.can_insert_predicates.Add( () => asc.is_alternative);
        }
    }

    [InclusiveAspects(GunAspect.TRIGGER, GunAspect.YOKE)]
    public class OpenYokeTriggerBlockSystem : GunSystemBase {
        YokeComponent yc;
        TriggerComponent tc;

        public override void Initialize() {
            if(yc.open_yoke_blocks_trigger) {
                tc.trigger_pressable_predicates.Add(() => yc.yoke_stage == YokeStage.CLOSED);
            }
        }
    }

    [InclusiveAspects(GunAspect.HAMMER, GunAspect.YOKE)]
    public class OpenYokeHammerBlockSystem : GunSystemBase {
        YokeComponent yc;
        HammerComponent hc;

        public override void Initialize() {
            if(yc.open_yoke_blocks_hammer) {
                hc.is_blocked_predicates.Add( () => yc.yoke_stage != YokeStage.CLOSED );
            }
        }
    }

    [InclusiveAspects(GunAspect.EXTRACTOR_ROD, GunAspect.YOKE)]
    public class YokeExtractorRodBlockSystem : GunSystemBase {
        YokeComponent yc;
        ExtractorRodComponent erc;

        public override void Initialize() {
            if(yc.closed_yoke_blocks_extractor) {
                erc.can_extract_predicates.Add( () => yc.yoke_stage == YokeStage.OPEN );
            }
        }
    }

    [InclusiveAspects(GunAspect.YOKE, GunAspect.REVOLVER_CYLINDER)]
    public class YokeCylinderClosingSystem : GunSystemBase {
        RevolverCylinderComponent rcc;
        YokeComponent yc;

        public override void Initialize() {
            rcc.is_closed_predicates.Add( () => yc.yoke_stage == YokeStage.CLOSED );
        }
    }

    [InclusiveAspects(GunAspect.SLIDE, GunAspect.LOCKABLE_BOLT)]
    public class BoltSlideBlockSystem : GunSystemBase {
        LockableBoltComponent bc;
        SlideComponent sc;

        public override void Initialize() {
            sc.block_slide_pull_predicates.Add( () => bc.bolt_stage != BoltActionStage.UNLOCKED);
        }
    }

    [InclusiveAspects(GunAspect.TRIGGER, GunAspect.LOCKABLE_BOLT)]
    public class BoltTriggerBlockSystem : GunSystemBase {
        LockableBoltComponent bc;
        TriggerComponent tc;

        public override void Initialize() {
            tc.trigger_pressable_predicates.Add( () => bc.bolt_stage == BoltActionStage.LOCKED);
        }
    }

    [InclusiveAspects(GunAspect.CHAMBER, GunAspect.MANUAL_LOADING)]
    public class ManualLoadingChamberBlockSystem : GunSystemBase {
        ManualLoadingComponent mlc;
        ChamberComponent cc;

        public override void Initialize() {
            mlc.can_insert_predicates.Add( () => cc.is_closed == mlc.load_when_closed);
        }
    }

    /// <summary> System to block cylinder rotation when an extractor rod is inside a chamber </summary>
    [InclusiveAspects(GunAspect.REVOLVER_CYLINDER, GunAspect.EXTRACTOR_ROD)]
    public class CylinderExtractorRotationBlockSystem : GunSystemBase {
        RevolverCylinderComponent rcc;
        ExtractorRodComponent erc;

        public override void Initialize() {
            if(erc.chamber_offset >= 0)
                rcc.can_manual_rotate_predicates.Add( () => erc.extractor_rod_stage == ExtractorRodStage.CLOSED);
        }
    }

    /// <summary> System to apply the constant cylinder rotation block caused by "!rotateable" </summary>
    [InclusiveAspects(GunAspect.REVOLVER_CYLINDER)]
    [Priority(PriorityAttribute.EARLY)]
    public class CylinderRotationMainBlockSystem : GunSystemBase {
        RevolverCylinderComponent rcc;

        public override void Initialize() {
            if(!rcc.rotateable)
                rcc.can_manual_rotate_predicates.Add( () => false);
        }
    }
}