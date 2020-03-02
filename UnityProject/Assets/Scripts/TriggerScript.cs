using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class TriggerScript : MonoBehaviour {
    [Tooltip("Should only gameobjects with the \"Player\" tag be able to trigger?")]
    public bool player_trigger = false;

    [Tooltip("Event invoked when triggered")]
    public UnityEvent trigger_event;

    public void OnTriggerEnter(Collider collider) {
        if(player_trigger && collider.tag != "Player") {
            return;
        }
        trigger_event.Invoke();
    }

    public void OpenScene(string scene_name) {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene_name);
    }
}
