using System.Collections.Generic;
using UnityEngine;
using System;

namespace GunSystemInterfaces {
    public delegate bool ConnectMagazine(GameObject mag);
    public delegate GameObject DisconnectMagazine();
    public delegate void SpinCylinder(int amount);
    public delegate Vector2 GetRecoilTransfer();
    public delegate Vector2 GetRecoilRotation();
    public delegate bool AddHeadRecoil();

    public enum GunSystemQueries {
        SHOULD_PULL_SLIDE,
        IS_ADDING_ROUNDS,
        IS_EJECTING_ROUNDS,
        SHOULD_EJECT_MAGAZINE,
        SHOULD_RELEASE_SLIDE_LOCK,
        IS_SAFETY_ON,
        IS_SLIDE_LOCKED,
        IS_SLIDE_PULLED_BACK,
        SHOULD_PUSH_SLIDE,
        IS_WAITING_FOR_SLIDE_PUSH,
        IS_MAGAZINE_IN_GUN,
        IS_MAGAZINE_EJECTING,
        SHOULD_OPEN_CYLINDER,
        SHOULD_CLOSE_CYLINDER,
        SHOULD_EXTRACT_CASINGS,
        SHOULD_INSERT_BULLET,
        IS_CYLINDER_OPEN,
        IS_IN_ALTERNATIVE_STANCE,
        SHOULD_TOGGLE_FIRE_MODE,
        IS_HAMMER_COCKED,
        SHOULD_PULL_BACK_HAMMER,
        IS_PRESS_CHECK,
        IS_READY_TO_REMOVE_MAGAZINE,
        SHOULD_TOGGLE_STANCE,
        SHOULD_TOGGLE_BOLT,
    };

    public enum GunSystemRequests {
        CHAMBER_ROUND_FROM_MAG,
        PULL_SLIDE_BACK,
        APPLY_TRIGGER_PRESSURE,
        RELEASE_TRIGGER_PRESSURE,
        INPUT_EJECT_MAGAZINE,
        INPUT_INSERT_MAGAZINE,
        APPLY_PRESSURE_ON_SLIDE_LOCK,
        RELEASE_PRESSURE_ON_SLIDE_LOCK,
        TOGGLE_SAFETY,
        INPUT_RELEASE_SLIDE_LOCK,
        RELEASE_SLIDE_LOCK,
        TOGGLE_FIRE_MODE,
        INPUT_PULL_SLIDE_BACK,
        INPUT_PULL_SLIDE_PRESS_CHECK,
        INPUT_RELEASE_SLIDE,
        INPUT_PUSH_SLIDE_FORWARD,
        INPUT_TOGGLE_BOLT_LOCK,
        COCK_HAMMER,
        INPUT_PRESSURE_ON_HAMMER,
        INPUT_RELEASE_HAMMER,
        INPUT_ADD_ROUND,
        INPUT_SWING_OUT_CYLINDER,
        INPUT_CLOSE_CYLINDER,
        INPUT_USE_EXTRACTOR_ROD,
        INPUT_TOGGLE_STANCE,
        PUT_ROUND_IN_CHAMBER,
        INPUT_START_AIM,
        INPUT_STOP_AIM,
        DISCHARGE,
        SPEND_ROUND,
        DESTROY_ROUND,
        RESET_RECOIL,
    };

    /// <summary> Base class for every Gun System </summary>
    public abstract class GunSystemBase {
        public delegate bool GunSystemRequest();
        public delegate bool GunSystemQuery();

        public GunScript gs;
        public virtual void Initialize() {}
        public virtual void Update() { gs.gun_systems.UnloadSystem(this); }

        protected void RemoveChildrenShadows(GameObject parent) {
            foreach(var renderer in parent.GetComponentsInChildren<Renderer>()) {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }
    }

    /// <summary> Container class for a Gun System version, this class is extended in a separate namespace and connects GunScript logic with GunSystem logic </summary>
    public abstract class GunSystemsContainer {
        public Dictionary<GunSystemRequests, GunSystemBase.GunSystemRequest> requests;
        public Dictionary<GunSystemQueries, GunSystemBase.GunSystemQuery> queries;

        public ConnectMagazine connectMagazine;
        public DisconnectMagazine disconnectMagazine;
        public SpinCylinder spinCylinder;
        public GetRecoilTransfer getRecoilTransfer;
        public GetRecoilRotation getRecoilRotation;
        public AddHeadRecoil addHeadRecoil;

        public abstract void LoadSystems(GunScript gs, GunAspect aspects);
        public abstract bool ShouldLoadSystem(Type system, GunAspect aspects, bool ignore_exclusives = false, bool ignore_inclusives = false);
        public abstract void UnloadSystem(GunSystemBase system);
        public abstract void Initialize();
        public abstract void Update();
    }
}