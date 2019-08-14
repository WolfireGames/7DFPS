using UnityEngine;
using System;


public class tapescript:MonoBehaviour{
    
    float life_time = 0.0f;
    Vector3 old_pos;
    
    public void Start() {
    	old_pos = transform.position;
    }
    
    public void Update() {
    	transform.Find("light_obj").GetComponent<Light>().intensity = 1.0f + Mathf.Sin(Time.time * 2.0f);
    }
    
    public void FixedUpdate() {
    	if((GetComponent<Rigidbody>() != null) && !GetComponent<Rigidbody>().IsSleeping() && (GetComponent<Collider>() != null) && GetComponent<Collider>().enabled){
    		life_time += Time.deltaTime;
    		RaycastHit hit = new RaycastHit();
    		if(Physics.Linecast(old_pos, transform.position, out hit, 1)){
    			transform.position = hit.point;
    			transform.GetComponent<Rigidbody>().velocity *= -0.3f;
    		}
    		if(life_time > 2.0f){
    			GetComponent<Rigidbody>().Sleep();
    		}
    	}
    	old_pos = transform.position;
    }
}