#pragma strict

var battery_curve : AnimationCurve;
private var max_battery_life = 60*60*5.5;
private var battery_life_remaining = max_battery_life;

private var initial_pointlight_intensity : float;
private var initial_spotlight_intensity : float;

function Start () {
	initial_pointlight_intensity = transform.FindChild("Pointlight").gameObject.GetComponent(Light).intensity;
	initial_spotlight_intensity = transform.FindChild("Spotlight").gameObject.GetComponent(Light).intensity;
	battery_life_remaining = Random.Range(0.0, max_battery_life);
}

function Update () {
	battery_life_remaining -= Time.deltaTime;
	if(battery_life_remaining <= 0.0){
		battery_life_remaining = 0.0;
	}
	var battery_curve_eval = battery_curve.Evaluate(1.0-battery_life_remaining/max_battery_life);
	transform.FindChild("Pointlight").gameObject.GetComponent(Light).intensity = initial_pointlight_intensity * battery_curve_eval;
	transform.FindChild("Spotlight").gameObject.GetComponent(Light).intensity = initial_spotlight_intensity * battery_curve_eval;
}