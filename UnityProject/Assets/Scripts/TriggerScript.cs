using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class TriggerScript : MonoBehaviour {
    [Tooltip("Should only gameobjects with the \"Player\" tag be able to trigger?")]
    public bool player_trigger = false;

    [Tooltip("Event invoked when triggered")]
    public UnityEvent trigger_event;

    [Tooltip("Event invoked when the trigger was exited")]
    public UnityEvent trigger_exit_event;

    [Tooltip("Event invoked while a collider is inside the trigger")]
    public UnityEvent trigger_stay_event;

    public void OnTriggerEnter(Collider collider) {
        if(ShouldTrigger(collider)) {
            trigger_event.Invoke();
        }
    }

    public void OnTriggerExit(Collider collider) {
        if(ShouldTrigger(collider)) {
            trigger_exit_event.Invoke();
        }
    }

    public void OnTriggerStay(Collider collider) {
        if(ShouldTrigger(collider)) {
            trigger_stay_event.Invoke();
        }
    }

    /// <summary> Return false if this collider should only be triggered by the player and was triggered by a non player collider </summary>
    private bool ShouldTrigger(Collider collider) {
        return !player_trigger || collider.tag == "Player";
    }

    public void OpenScene(string scene_name) {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene_name);
    }
}
