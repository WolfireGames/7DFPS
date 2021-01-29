using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ShootableTrigger : MonoBehaviour, IShootable {
    public UnityEvent onShot = new UnityEvent();

    public void WasShot(GameObject hit_object, Vector3 hit_pos, Vector3 velocity) {
        onShot.Invoke();
    }
}
