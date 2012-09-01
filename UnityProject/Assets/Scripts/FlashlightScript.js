#pragma strict

var battery_curve : AnimationCurve;
var sound_turn_on : AudioClip;
var sound_turn_off : AudioClip;
private var kSoundVolume = 0.3;
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
	if(!switch_on){
		switch_on = true;
		audio.PlayOneShot(sound_turn_on, kSoundVolume * PlayerPrefs.GetFloat("sound_volume", 1.0));
	}
}

function TurnOff(){
	if(switch_on){
		switch_on = false;
		audio.PlayOneShot(sound_turn_off, kSoundVolume * PlayerPrefs.GetFloat("sound_volume", 1.0));
	}
}

function Update () {
	if(switch_on){
		battery_life_remaining -= Time.deltaTime;
		if(battery_life_remaining <= 0.0){
			battery_life_remaining = 0.0;
		}
		var battery_curve_eval = battery_curve.Evaluate(1.0-battery_life_remaining/max_battery_life);
		transform.FindChild("Pointlight").gameObject.GetComponent(Light).intensity = initial_pointlight_intensity * battery_curve_eval * 8.0;
		transform.FindChild("Spotlight").gameObject.GetComponent(Light).intensity = initial_spotlight_intensity * battery_curve_eval * 3.0;
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