using UnityEngine;

public interface IShootable {
    void WasShot(GameObject other, Vector3 hit_pos, Vector3 velocity);
}
