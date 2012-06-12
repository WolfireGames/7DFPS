#pragma strict

var opac = 0.0;

function Start () {
	opac = Random.Range(0.0,1.0);
	transform.localRotation.eulerAngles.z = Random.Range(0.0,360.0);
	transform.localScale.x = Random.Range(0.8,2.0);
	transform.localScale.y = Random.Range(0.8,2.0);
	transform.localScale.z = Random.Range(0.8,2.0);
}

function Update() {
	var renderers = transform.GetComponentsInChildren(MeshRenderer);
	var color = Vector4(opac,opac,opac,opac);
	for(var renderer : MeshRenderer in renderers){
		renderer.material.SetColor("_TintColor", color);
	}
	opac -= Time.deltaTime * 50.0;
	if(opac <= 0.0){
		Destroy(gameObject);
	}
}