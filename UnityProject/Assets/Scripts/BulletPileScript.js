#pragma strict

var bullet_obj : GameObject;

function Start () {
	var num_bullets = Random.Range(1,6);
	for(var i=0; i<num_bullets; ++i){
		var bullet : GameObject = Instantiate(bullet_obj);
		bullet.transform.position = transform.position + 
			Vector3(Random.Range(-0.1,0.1),
					Random.Range(0.0,0.2),
					Random.Range(-0.1,0.1));
		bullet.transform.rotation = BulletScript.RandomOrientation();
		bullet.AddComponent(Rigidbody);
		bullet.GetComponent(ShellCasingScript).collided = true;
	}
}

function Update () {

}