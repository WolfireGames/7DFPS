using UnityEngine;
using System;


public class tapescript:MonoBehaviour{
    
    float life_time = 0.0f;
    Vector3 old_pos;

    Light lightObject;

    Rigidbody rigidBody;
    Collider coll;

    public void Awake() {
    	lightObject = transform.Find("light_obj").GetComponent<Light>();
    }

    public void Start() {
        old_pos = transform.position;
        rigidBody = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
    }
    
    public void Update() {
    	lightObject.intensity = 1.0f + Mathf.Sin(Time.time * 2.0f);
    }
    
    public void FixedUpdate() {
    	if(rigidBody != null && !rigidBody.IsSleeping() && (coll != null) && coll.enabled){
    		life_time += Time.deltaTime;
    		RaycastHit hit = new RaycastHit();
    		if(Physics.Linecast(old_pos, transform.position, out hit, 1)){
    			transform.position = hit.point;
    			rigidBody.velocity *= -0.3f;
    		}
    		if(life_time > 2.0f){
    			rigidBody.Sleep();
    		}
    	}
    	old_pos = transform.position;
    }
}