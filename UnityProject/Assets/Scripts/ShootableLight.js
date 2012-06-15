#pragma strict

var light_color : Color;

function Start () {
	
}

function Update () {
	var light_amount = Mathf.Sin(Time.time)*0.5 + 0.5;
	for(var light : Light in gameObject.GetComponentsInChildren(Light)){
		light.color = Color(light_amount,light_amount,light_amount);
	}
	for(var renderer : MeshRenderer in gameObject.GetComponentsInChildren(MeshRenderer)){
		renderer.material.SetColor("_Color", Color(light_amount,light_amount,light_amount,light_amount));
	}
}