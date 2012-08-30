#pragma strict

var battery_curve : AnimationCurve;
private var switch_on = false;
private var max_battery_life = 60*60*5.5;
private var battery_life_remaining = max_battery_life;

private var initial_pointlight_intensity : float;
private var initial_spotlight_intensity : float;

function Awake() {
	switch_on = Random.Range(0.0,1.0) < 0.5;
}

function Start () {
	initial_pointlight_intensity = transform.FindChild("Pointlight").gameObject.GetComponent(Light).intensity;
	initial_spotlight_intensity = transform.FindChild("Spotlight").gameObject.GetComponent(Light).intensity;
	battery_life_remaining = Random.Range(max_battery_life*0.2, max_battery_life);
}

function TurnOn(){
	switch_on = true;
}

function TurnOff(){
	switch_on = false;
}

function Update () {
	if(switch_on){
		battery_life_remaining -= Time.deltaTime;
		if(battery_life_remaining <= 0.0){
			battery_life_remaining = 0.0;
		}
		var battery_curve_eval = battery_curve.Evaluate(1.0-battery_life_remaining/max_battery_life);
		transform.FindChild("Pointlight").gameObject.GetComponent(Light).intensity = initial_pointlight_intensity * battery_curve_eval * 4.0;
		transform.FindChild("Spotlight").gameObject.GetComponent(Light).intensity = initial_spotlight_intensity * battery_curve_eval;
	} else {
		transform.FindChild("Pointlight").gameObject.GetComponent(Light).intensity = 0.0;
		transform.FindChild("Spotlight").gameObject.GetComponent(Light).intensity = 0.0;
	}
	if(rigidbody){
		transform.FindChild("Pointlight").light.intensity = 1.0 + Mathf.Sin(Time.time * 2.0);
		transform.FindChild("Pointlight").light.range = 1.0;
	} else {
		transform.FindChild("Pointlight").light.range = 10.0;
	}
}