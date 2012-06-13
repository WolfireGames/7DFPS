#pragma strict

var opac = 0.0;

function UpdateColor() {
	var renderers = transform.GetComponentsInChildren(MeshRenderer);
	var color = Vector4(opac,opac,opac,opac);
	for(var renderer : MeshRenderer in renderers){
		renderer.material.SetColor("_TintColor", color);
	}
	var lights = transform.GetComponentsInChildren(Light);
	for(var light : Light in lights){
		light.intensity = opac * 2.0;
	}
}

function Start () {
	opac = Random.Range(0.4,1.0);
	UpdateColor();
	transform.localRotation.eulerAngles.z = Random.Range(0.0,360.0);
	transform.localScale.x = Random.Range(0.8,2.0);
	transform.localScale.y = Random.Range(0.8,2.0);
	transform.localScale.z = Random.Range(0.8,2.0);
}

function Update() {
	UpdateColor();
	opac -= Time.deltaTime * 5.0;
	transform.localScale += Vector3(1,1,1)*Time.deltaTime*30.0;
	if(opac <= 0.0){
		Destroy(gameObject);
	}
}