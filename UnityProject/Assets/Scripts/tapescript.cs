using UnityEngine;

public class tapescript:MonoBehaviour{
    public void Update() {
    	transform.Find("light_obj").GetComponent<Light>().intensity = 1.0f + Mathf.Sin(Time.time * 2.0f);
    }
}