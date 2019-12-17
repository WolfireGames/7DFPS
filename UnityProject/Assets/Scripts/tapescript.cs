using UnityEngine;
using System;


public class tapescript:MonoBehaviour{
    
    float life_time = 0.0f;
    Vector3 old_pos;

    Light lightObject;
    
    public void Awake() {
    	lightObject = transform.Find("light_obj").GetComponent<Light>();
    }

    public void Start() {
        old_pos = transform.position;
    }
    
    public void Update() {
    	lightObject.intensity = 1.0f + Mathf.Sin(Time.time * 2.0f);
    }
    
    public void FixedUpdate() {
    	Rigidbody rigidbody = GetComponent<Rigidbody>();
    	if(rigidbody != null && !rigidbody.IsSleeping() && (GetComponent<Collider>() != null) && GetComponent<Collider>().enabled){
    		life_time += Time.deltaTime;
    		RaycastHit hit = new RaycastHit();
    		if(Physics.Linecast(old_pos, transform.position, out hit, 1)){
    			transform.position = hit.point;
    			rigidbody.velocity *= -0.3f;
    		}
    		if(life_time > 2.0f){
    			rigidbody.Sleep();
    		}
    	}
    	old_pos = transform.position;
    }
}