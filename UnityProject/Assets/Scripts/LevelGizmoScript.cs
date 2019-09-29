using UnityEngine;

public class LevelGizmoScript : MonoBehaviour {
    public Color color = Color.white;
    public Vector3 size = Vector3.one;
    public Vector3 offset = Vector3.zero;

    public void OnDrawGizmos() {
		Gizmos.color = color;
		Gizmos.DrawWireCube(transform.position + offset, size);
	}
}
