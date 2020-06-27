using UnityEngine;
using System.Collections.Generic;
using System;
using GunSystemInterfaces;

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
        if(gun_systems.disconnectMagazine != null)
            return gun_systems.disconnectMagazine();
        return null;
    }

    /// <summary> Connect a magazine to the GunSystems and notifies the Systems to handle it from now on </summary>
    public bool InsertMag(GameObject mag) {
        if(gun_systems.connectMagazine != null)
            return gun_systems.connectMagazine(mag);
        return false;
    }

    public void RotateCylinder(int how_many) {
        if(gun_systems.spinCylinder != null)
            gun_systems.spinCylinder(how_many);
    }

    public Vector2 GetRecoilTransfer() {
        if(gun_systems.getRecoilTransfer != null)
            return gun_systems.getRecoilTransfer();
        return Vector2.zero;
    }
    public Vector2 GetRecoilRotation() {
        if(gun_systems.getRecoilRotation != null)
            return gun_systems.getRecoilRotation();
        return Vector2.zero;
    }
    public bool AddHeadRecoil() {
        if(gun_systems.addHeadRecoil != null)
            return gun_systems.addHeadRecoil();
        return false;
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

    public bool ResetRecoil() {
        return Request(GunSystemRequests.RESET_RECOIL);
    }
}