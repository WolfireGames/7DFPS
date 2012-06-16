#pragma strict

var destroy_effect : GameObject;
var light_color = Color(1,1,1);
var destroyed = false;
enum LightType {AIRPLANE_BLINK, NORMAL, FLICKER}
public var light_type = LightType.NORMAL;
private var blink_delay = 0.0;

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
	if(!destroyed){
		switch(light_type){
			case LightType.AIRPLANE_BLINK:
				if(blink_delay <= 0.0){
					blink_delay = 1.0;
					if(light_amount == 1.0){
						light_amount = 0.0;
					} else {
						light_amount = 1.0;
					}
				}
				blink_delay -= Time.deltaTime;
				break;
		}
	}

	var combined_color = Color(light_color.r * light_amount,light_color.g * light_amount,light_color.b * light_amount);
	for(var light : Light in gameObject.GetComponentsInChildren(Light)){
		light.color = combined_color;
	}
	for(var renderer : MeshRenderer in gameObject.GetComponentsInChildren(MeshRenderer)){
		renderer.material.SetColor("_Illum", combined_color);
	}
}