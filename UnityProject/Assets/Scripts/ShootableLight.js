#pragma strict

var destroy_effect : GameObject;
var light_color = Color(1,1,1);
var destroyed = false;

private var light_amount = 1.0;

function WasShot(obj : GameObject, pos : Vector3, vel : Vector3) {
	if(!destroyed){
		destroyed = true;
		light_amount = 0.0;
		Instantiate(destroy_effect, transform.FindChild("bulb").position, Quaternion.identity);
	}
	if(obj && obj.collider && obj.collider.material.name == "glass (Instance)"){
		GameObject.Destroy(obj);
	}
}

function Start () {
	
}

function Update () {
	var combined_color = Color(light_color.r * light_amount,light_color.g * light_amount,light_color.b * light_amount);
	for(var light : Light in gameObject.GetComponentsInChildren(Light)){
		light.color = combined_color;
	}
	for(var renderer : MeshRenderer in gameObject.GetComponentsInChildren(MeshRenderer)){
		renderer.material.SetColor("_Illum", combined_color);
	}
}