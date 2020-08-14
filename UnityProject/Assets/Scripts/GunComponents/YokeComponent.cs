using UnityEngine;

namespace GunSystemsV1 {
    public enum YokeStage { CLOSED, OPENING, OPEN, CLOSING };

    [GunDataAttribute(GunAspect.YOKE)]
    public class YokeComponent : GunComponent {
        public AudioClip[] sound_cylinder_open = new AudioClip[0];
        public AudioClip[] sound_cylinder_close = new AudioClip[0];
        
        public bool open_yoke_blocks_trigger = true;
        public bool open_yoke_blocks_hammer = true;
        public bool closed_yoke_blocks_extractor = true;

        internal float yoke_open = 0.0f;
        internal YokeStage yoke_stage = YokeStage.CLOSED;
    }
}