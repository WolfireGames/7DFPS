#pragma strict

function Start () {
	var renderer = transform.GetComponentInChildren(MeshRenderer);
	var color = renderer.material.GetColor("_TintColor");
	var opac = Random.Range(0.0,1.0);
	color.a = opac;
	renderer.material.SetColor("_TintColor", color);
	transform.localRotation.eulerAngles.z = Random.Range(0.0,360.0);
	transform.localScale.x = Random.Range(0.4,1.0);
	transform.localScale.y = Random.Range(0.4,1.0);
	transform.localScale.z = Random.Range(0.4,1.0);
}

function FixedUpdate() {
	Destroy(gameObject);
}