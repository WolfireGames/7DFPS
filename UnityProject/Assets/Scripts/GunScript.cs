using UnityEngine;
using System.Collections.Generic;
using System;

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
};

/// <summary> Base class for every Gun System </summary>
public abstract class GunSystemBase {
    public delegate bool GunSystemRequest();
    public delegate bool GunSystemQuery();

    public GunScript gs;
    public virtual void Initialize() {}
    public virtual void Update() { gs.gun_systems.UnloadSystem(this); }

    public virtual Dictionary<GunSystemRequests, GunSystemRequest> GetPossibleRequests() { return null; }
    public virtual Dictionary<GunSystemQueries, GunSystemQuery> GetPossibleQuestions() { return null; }

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

    public abstract void LoadSystems(GunScript gs, GunAspect aspects);
    public abstract bool ShouldLoadSystem(Type system, GunAspect aspects, bool ignore_exclusives = false, bool ignore_inclusives = false);
    public abstract void UnloadSystem(GunSystemBase system);
    public abstract void Initialize();
    public abstract void Update();
}

[RequireComponent(typeof(AudioSource))]
public class GunScript : MonoBehaviour {
    // Versioning
    public enum GunSystemVersion { VERSION_0, VERSION_1 }
    public GunSystemVersion gun_system_version = GunSystemVersion.VERSION_0;

    // Gun Settings
    [IsNonNull] public GameObject empty_casing; // TODO make IsNonNull work for non guncomponents
    [IsNonNull] public GameObject full_casing;
    [Range(0.1f, 0.01f)] public float camera_nearplane_override = 0.1f;
    [HideInInspector, NonSerialized] public float base_volume = 0.2f;

    // Misc
    [HideInInspector] public GunAspect aspect = new GunAspect();
    [HideInInspector, NonSerialized] public Vector3 old_pos;
    [HideInInspector, NonSerialized] public Vector3 velocity;
    [HideInInspector, NonSerialized] public GunTilt preferred_tilt = GunTilt.NONE;
    [HideInInspector, NonSerialized] public GunSystemsContainer gun_systems;

    // MonoBehaviour Methods
    public void Start() {
        Camera.main.nearClipPlane = camera_nearplane_override; // Override Camera's near plane to support guns closer to the camera
    }

    public void OnEnable() {
        gun_systems = GetGunSystems();

        // Init
        aspect = aspect ^ (GunAspect.ALTERNATIVE_STANCE & aspect); // Remove ALTERNATIVE_STANCE if it is present
        gun_systems.LoadSystems(this, aspect);
        gun_systems.Initialize();
    }

    public void Update() {
        gun_systems.Update();
    }

    public void FixedUpdate() {
        velocity = (transform.position - old_pos) / Time.deltaTime;
        old_pos = transform.position;
    }

    // Utility
    public void PlaySound(AudioClip[] group, float volume = 0.2f) {
        if (group.Length > 0) {
            int which_shot = UnityEngine.Random.Range(0, group.Length);
            GetComponent<AudioSource>().PlayOneShot(group[which_shot], volume * Preferences.sound_volume);
        }
    }

    /// <summary> Disconnect the magazine from the GunSystems and returns the disconnected GameObject </summary>
    public GameObject GrabMag() {
        MagazineComponent mc = GetComponent<MagazineComponent>();
        if(!mc || !mc.mag_script) {
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

    /// <summary> Connect a magazine to the GunSystems and notifies the Systems to handle it from now on </summary>
    public bool InsertMag(GameObject mag) {
        MagazineComponent mc = GetComponent<MagazineComponent>();
        if (!mc) {
            return false;
        }

        // Set this mag as mag to insert
        mc.mag_script = mag.GetComponent<mag_script>();
        mag.transform.parent = transform;

        // Tell the systems to push the mag in
        return Request(GunSystemRequests.INPUT_INSERT_MAGAZINE);
    }

    public void RotateCylinder(int how_many) {
        RevolverCylinderComponent rcc = GetComponent<RevolverCylinderComponent>();
        if(!rcc) {
            return;
        }
        
        rcc.target_cylinder_offset += how_many * (Mathf.Max(1, Mathf.Abs(rcc.target_cylinder_offset)));
        rcc.target_cylinder_offset = Mathf.Max(-12, Mathf.Min(12, rcc.target_cylinder_offset));
    }

    public RecoilComponent GetRecoilData() {
        return GetComponent<RecoilComponent>();
    }

    /// <summary> Return a gun system instance of the selected gun systems version </summary>
    public GunSystemsContainer GetGunSystems() {
        switch (gun_system_version) {
            case GunSystemVersion.VERSION_0:
            case GunSystemVersion.VERSION_1:
                return new GunSystemsV1.GunSystems();
            default:
                throw new ArgumentOutOfRangeException($"There is no gun system version {gun_system_version}!");
        }
    }

    // Has component methods
    public bool HasGunComponent(GunAspect aspect) {
        return this.aspect.HasFlag(aspect);
    }

    public bool HasSlide() {
        return HasGunComponent((GunAspect)GunAspect.SLIDE);
    }

    public bool HasSafety() {
        return HasGunComponent(GunAspect.THUMB_SAFETY);
    }

    public bool HasHammer() {
        return HasGunComponent(GunAspect.HAMMER);
    }

    public bool HasAutoMod() {
        return HasGunComponent(GunAspect.FIRE_MODE);
    }

    // Request / Query logic
    public bool Request(GunSystemRequests gsr) {
        if(gun_systems != null && gun_systems.requests.ContainsKey(gsr)) {
            return gun_systems.requests[gsr]();
        }
        return false;
    }

    public bool Query(GunSystemQueries gsq) {
        if(gun_systems != null && gun_systems.queries.ContainsKey(gsq)) {
            return gun_systems.queries[gsq]();
        }
        return false;
    }

    public bool IsAddingRounds() {
        return Query(GunSystemQueries.IS_ADDING_ROUNDS);
    }

    public bool IsEjectingRounds() {
        return Query(GunSystemQueries.IS_EJECTING_ROUNDS);
    }

    public bool ShouldPullSlide() {
        return Query(GunSystemQueries.SHOULD_PULL_SLIDE);
    }

    public bool ShouldReleaseSlideLock() {
        return Query(GunSystemQueries.SHOULD_RELEASE_SLIDE_LOCK);
    }

    public bool ShouldEjectMag() {
        return Query(GunSystemQueries.SHOULD_EJECT_MAGAZINE);
    }

    public bool ChamberRoundFromMag() {
        return Request(GunSystemRequests.CHAMBER_ROUND_FROM_MAG);
    }

    public bool PullSlideBack() {
        return Request(GunSystemRequests.PULL_SLIDE_BACK);
    }

    public bool PushSlideForward() {
        return Request(GunSystemRequests.INPUT_PUSH_SLIDE_FORWARD);
    }

    public bool ShouldPushSlideForward() {
        return Query(GunSystemQueries.SHOULD_PUSH_SLIDE);
    }

    public bool ReleaseSlideLock() {
        return Request(GunSystemRequests.INPUT_RELEASE_SLIDE_LOCK);
    }

    public bool ToggleBoltLock() {
        return Request(GunSystemRequests.INPUT_TOGGLE_BOLT_LOCK);
    }

    public bool ApplyPressureToTrigger() {
        return Request(GunSystemRequests.APPLY_TRIGGER_PRESSURE);
    }

    public bool ReleasePressureFromTrigger() {
        return Request(GunSystemRequests.RELEASE_TRIGGER_PRESSURE);
    }

    public bool MagEject() {
        return Request(GunSystemRequests.INPUT_EJECT_MAGAZINE);
    }

    public bool PressureOnSlideLock() {
        return Request(GunSystemRequests.APPLY_PRESSURE_ON_SLIDE_LOCK);
    }

    public bool ReleasePressureOnSlideLock() {
        return Request(GunSystemRequests.RELEASE_PRESSURE_ON_SLIDE_LOCK);
    }

    public bool ToggleSafety() {
        return Request(GunSystemRequests.TOGGLE_SAFETY);
    }

    public bool InputStartAim () {
        return Request(GunSystemRequests.INPUT_START_AIM);
    }
    public bool InputStopAim () {
        return Request(GunSystemRequests.INPUT_STOP_AIM);
    }

    public bool ToggleAutoMod() {
        return Request(GunSystemRequests.TOGGLE_FIRE_MODE);
    }

    public bool InputPullSlideBack() {
        return Request(GunSystemRequests.INPUT_PULL_SLIDE_BACK);
    }

    public bool ReleaseSlide() {
        return Request(GunSystemRequests.INPUT_RELEASE_SLIDE);
    }

    public bool PressureOnHammer() {
        return Request(GunSystemRequests.INPUT_PRESSURE_ON_HAMMER);
    }

    public bool ReleaseHammer() {
        return Request(GunSystemRequests.INPUT_RELEASE_HAMMER);
    }

    public bool IsSafetyOn() {
        return Query(GunSystemQueries.IS_SAFETY_ON);
    }

    public bool IsSlideLocked() {
        return Query(GunSystemQueries.IS_SLIDE_LOCKED);
    }

    public bool IsSlidePulledBack() {
        return Query(GunSystemQueries.IS_SLIDE_PULLED_BACK);
    }

    public bool IsThereAMagInGun() {
        return Query(GunSystemQueries.IS_MAGAZINE_IN_GUN);
    }

    public bool IsMagCurrentlyEjecting() {
        return Query(GunSystemQueries.IS_MAGAZINE_EJECTING);
    }

    public bool IsCylinderOpen() {
        return Query(GunSystemQueries.IS_CYLINDER_OPEN);
    }

    public bool AddRoundToCylinder() {
        return Request(GunSystemRequests.INPUT_ADD_ROUND);
    }

    public bool ShouldOpenCylinder() {
        return Query(GunSystemQueries.SHOULD_OPEN_CYLINDER);
    }

    public bool ShouldCloseCylinder() {
        return Query(GunSystemQueries.SHOULD_CLOSE_CYLINDER);
    }

    public bool ShouldExtractCasings() {
        return Query(GunSystemQueries.SHOULD_EXTRACT_CASINGS);
    }

    public bool ShouldInsertBullet() {
        return Query(GunSystemQueries.SHOULD_INSERT_BULLET);
    }

    public bool ShouldToggleAutoMod() {
        return Query(GunSystemQueries.SHOULD_TOGGLE_FIRE_MODE);
    }

    public bool ShouldToggleStance() {
        return Query(GunSystemQueries.SHOULD_TOGGLE_STANCE);
    }

    public bool ShouldToggleBolt() {
        return Query(GunSystemQueries.SHOULD_TOGGLE_BOLT);
    }

    public bool IsHammerCocked() {
        return Query(GunSystemQueries.IS_HAMMER_COCKED);
    }

    public bool IsInAlternativeStance() {
        return Query(GunSystemQueries.IS_IN_ALTERNATIVE_STANCE);
    }

    public bool InputToggleStance() {
        return Request(GunSystemRequests.INPUT_TOGGLE_STANCE);
    }

    public bool ShouldPullBackHammer() {
        return Query(GunSystemQueries.SHOULD_PULL_BACK_HAMMER);
    }

    public bool SwingOutCylinder() {
        return Request(GunSystemRequests.INPUT_SWING_OUT_CYLINDER);
    }

    public bool CloseCylinder() {
        return Request(GunSystemRequests.INPUT_CLOSE_CYLINDER);
    }

    public bool ExtractorRod() {
        return Request(GunSystemRequests.INPUT_USE_EXTRACTOR_ROD);
    }

    public bool IsPressCheck() {
        return Query(GunSystemQueries.IS_PRESS_CHECK);
    }

    public bool IsReadyToRemoveMagazine() {
        return Query(GunSystemQueries.IS_READY_TO_REMOVE_MAGAZINE);
    }
}