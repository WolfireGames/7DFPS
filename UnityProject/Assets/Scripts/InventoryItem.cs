using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour {
    private Vector3 oldPos = Vector3.zero;

    public AudioClip[] soundCollision;

    private static LevelCreatorScript levelCreatorScript;
    new private Rigidbody rigidbody = null;

    private float rigidbodyMass = 0f;
    private float rigidbodyDrag = 0f;
    private float rigidbodyAngularDrag = 0f;

    public void Awake() {
        if(levelCreatorScript == null) {
            levelCreatorScript = GameObject.Find("LevelObject").GetComponent<LevelCreatorScript>();
        }

        // Get rigidbody values
        rigidbody = GetComponent<Rigidbody>();
        rigidbodyMass = rigidbody.mass;
        rigidbodyDrag = rigidbody.drag;
        rigidbodyAngularDrag = rigidbody.angularDrag;
    }

    private void PlaySoundFromGroup(AudioClip[] group, float volume) {
        if(group.Length != 0){
            GetComponent<AudioSource>().PlayOneShot(group[Random.Range(0, group.Length)], volume * PlayerPrefs.GetFloat("sound_volume", 1.0f));
        }
    }

    public void Drop(Vector3 velocity) {
        SetRigidbodyActive(true);
        rigidbody.velocity = velocity;

        if(levelCreatorScript != null) { // This check should be pretty useless unless you're planning on including a playable scene without the Level Creator Script
            transform.parent = levelCreatorScript.GetPositionTileItemParent(transform.position);
        }

        // Notify listeners
        foreach(var component in GetComponents<InventoryItemListener>()) {
            component.OnDrop();
        }
    }

    public void Pickup() {
        if(levelCreatorScript != null) {
            transform.parent = levelCreatorScript.GetPlayerInventoryTransform();
        } else {
            transform.parent = null; //Move item out of tile
        }

        SetRigidbodyActive(false);

        // Notify listeners
        foreach(var component in GetComponents<InventoryItemListener>()) {
            component.OnPickup();
        }
    }

    public void OnCollisionEnter(Collision collision) {
        if(!rigidbody.IsSleeping())
            PlaySoundFromGroup(soundCollision, 0.3f);
    }

    private bool IsRigidbodyActive() {
        if(rigidbody == null)
            return false;
        return !rigidbody.IsSleeping();
    }

    private void SetRigidbodyActive(bool active) {
        if(active) {
            rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.mass = rigidbodyMass;
            rigidbody.drag = rigidbodyDrag;
            rigidbody.angularDrag = rigidbodyAngularDrag;
            rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        } else {
            Destroy(rigidbody);
            rigidbody = null;
        }
    }

    public interface InventoryItemListener {
        void OnDrop();
        void OnPickup();
    }
}
