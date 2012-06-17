#pragma strict

function Start () {

}

function Update () {
	transform.FindChild("light_obj").light.intensity = 1.0 + Mathf.Sin(Time.time * 2.0);
}