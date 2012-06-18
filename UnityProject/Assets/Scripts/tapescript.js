#pragma strict

private var life_time = 0.0;
private var old_pos : Vector3;

function Start () {
	old_pos = transform.position;
}

function Update () {
	transform.FindChild("light_obj").light.intensity = 1.0 + Mathf.Sin(Time.time * 2.0);
}

function FixedUpdate() {
	if(rigidbody && !rigidbody.IsSleeping() && collider && collider.enabled){
		life_time += Time.deltaTime;
		var hit : RaycastHit;
		if(Physics.Linecast(old_pos, transform.position, hit, 1)){
			transform.position = hit.point;
			transform.rigidbody.velocity *= -0.3;
		}
		if(life_time > 2.0){
			rigidbody.Sleep();
		}
	}
	old_pos = transform.position;
}