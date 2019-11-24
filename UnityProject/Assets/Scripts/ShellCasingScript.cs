using UnityEngine;

public class ShellCasingScript:MonoBehaviour{
    public float life_time = 0.0f;
    public float glint_delay = 0.0f;
    public float glint_progress = 0.0f;
    Light glint_light;
    
    public void Awake() {
    	if(transform.Find("light_pos") != null){
    		glint_light = transform.Find("light_pos").GetComponent<Light>();
    		glint_light.enabled = false;
    	}
    }
    
    public void FixedUpdate() {
    	if((GetComponent<Rigidbody>() != null) && GetComponent<Rigidbody>().IsSleeping() && (glint_light != null)){
    		if(glint_delay == 0.0f){
    			glint_delay = UnityEngine.Random.Range(1.0f,5.0f);
    		}
    		glint_delay = Mathf.Max(0.0f, glint_delay - Time.deltaTime);
    		if(glint_delay == 0.0f){
    			glint_progress = 1.0f;
    		}
    		if(glint_progress > 0.0f){
    			glint_light.enabled = true;
    			glint_light.intensity = Mathf.Sin(glint_progress * Mathf.PI);
    			glint_progress = Mathf.Max(0.0f, glint_progress - Time.deltaTime * 2.0f);
    		} else {
    			glint_light.enabled = false;
    		}
    	}
    }
}