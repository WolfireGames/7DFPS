using UnityEngine;
using System;

public class WeaponHolder : MonoBehaviour {
    public string display_name = "My Gun";

    public GameObject gun_object;
    public GameObject mag_object;
    public GameObject bullet_object;
    public GameObject casing_object;

    [NonSerialized] public Mod mod = null;

    public void Load() {
        if(mod == null)
            return; // Don't need to load anything if it isn't a mod placeholder

        mod.Load();

        WeaponHolder holder = mod.mainAsset.GetComponent<WeaponHolder>();
        this.display_name = holder.display_name;

        this.gun_object = holder.gun_object;
        this.mag_object = holder.mag_object;
        this.bullet_object = holder.bullet_object;
        this.casing_object = holder.casing_object;
    }
}