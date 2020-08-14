using UnityEngine;

[GunData(GunAspect.AIM_SWAY)]
public class AimSwayComponent : GunComponent {

    // Do people really need to be able to edit this?
    public float a = 3;
    public float b = 4;
    public float delta = (float)Mathf.PI / 4f;
    
    public float speed = .5f;
    public float horizontal_strength = 200;
    public float vertical_strength = 200;

}