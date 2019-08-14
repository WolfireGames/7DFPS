using UnityEngine;
using System;


public class electricsparkscript:MonoBehaviour{
    
    public float opac = 0.0f;
    
    public void UpdateColor() {
    	MeshRenderer[] renderers = transform.GetComponentsInChildren<MeshRenderer>();
    	Vector4 color = new Vector4(opac,opac,opac,opac);
    	foreach(MeshRenderer renderer in renderers){
    		renderer.material.SetColor("_TintColor", (Color)color);
    	}
    	Light[] lights = transform.GetComponentsInChildren<Light>();
    	foreach(Light light in lights){
    		light.intensity = opac * 2.0f;
    	}
    }
    
    public void Start() {
    	opac = UnityEngine.Random.Range(0.4f,1.0f);
    	UpdateColor();
    	var tmp_cs1 = transform.localRotation;
        var tmp_cs2 = tmp_cs1.eulerAngles;
        tmp_cs2.z = UnityEngine.Random.Range(0.0f,360.0f);
        tmp_cs1.eulerAngles = tmp_cs2;
        transform.localRotation = tmp_cs1;
    	var tmp_cs3 = transform.localScale;
        tmp_cs3.x = UnityEngine.Random.Range(0.8f,2.0f);
        tmp_cs3.y = UnityEngine.Random.Range(0.8f,2.0f);
        tmp_cs3.z = UnityEngine.Random.Range(0.8f,2.0f);
        transform.localScale = tmp_cs3;
    }
    
    public void Update() {
    	UpdateColor();
    	opac -= Time.deltaTime * 5.0f;
    	transform.localScale += new Vector3(1.0f,1.0f,1.0f)*Time.deltaTime*30.0f;
    	if(opac <= 0.0f){
    		Destroy(gameObject);
    	}
    }
}