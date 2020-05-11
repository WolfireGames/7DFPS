using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCircleColor : MonoBehaviour
{
    public Renderer rend;
    void Start()
    {
        rend.sharedMaterial.color = Color.green;
    }

    public void Teleportable(bool canTeleport) {
        if (canTeleport) {
            rend.sharedMaterial.color = Color.green;
        }
        else {
            rend.sharedMaterial.color = Color.red;
        }
    }

    void Update()
    {
        
    }
}
