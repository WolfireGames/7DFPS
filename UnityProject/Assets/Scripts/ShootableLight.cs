using UnityEngine;
using System;

public enum LightType {AIRPLANE_BLINK, NORMAL, FLICKER}

public class ShootableLight:MonoBehaviour{
    
    public GameObject destroy_effect;
    public Color light_color = new Color(1.0f,1.0f,1.0f);
    public bool destroyed = false;

    public LightType light_type = LightType.NORMAL;
    float blink_delay = 0.0f;
    
    float light_amount = 1.0f;
    
    public void WasShot(GameObject obj,Vector3 pos,Vector3 vel) {
    	if(!destroyed){
    		destroyed = true;
    		light_amount = 0.0f;
    		Instantiate(destroy_effect, transform.Find("bulb").position, Quaternion.identity);
    		UpdateLightColors();
    	}
    	if((obj != null) && (obj.GetComponent<Collider>() != null) && obj.GetComponent<Collider>().material.name == "glass (Instance)"){
    		GameObject.Destroy(obj);
    	}
    }
    
    public void UpdateLightColors() {
    	Color combined_color = new Color(light_color.r * light_amount,light_color.g * light_amount,light_color.b * light_amount);
    	foreach(Light light in gameObject.GetComponentsInChildren<Light>()){
    		light.color = combined_color;
    	}
    	foreach(MeshRenderer renderer in gameObject.GetComponentsInChildren<MeshRenderer>()){
    		renderer.material.SetColor("_Illum", combined_color);
    		if(renderer.gameObject.name == "shade"){
    			renderer.material.SetColor("_Illum", combined_color * 0.5f);
    		}
    	}
    }
    
    public void Start() {
    	UpdateLightColors();
    }
    
    public void Update() {
    	if(!destroyed){
    		switch(light_type){
    			case LightType.AIRPLANE_BLINK:
    				if(blink_delay <= 0.0f){
    					blink_delay = 1.0f;
    					if(light_amount == 1.0f){
    						light_amount = 0.0f;
    					} else {
    						light_amount = 1.0f;
    					}
    					UpdateLightColors();
    				}
    				blink_delay -= Time.deltaTime;
    				break;
    		}
    	}
    
    }
}