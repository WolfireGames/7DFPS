#pragma strict

var muzzle_flash : GameObject;
var bullet_obj : GameObject;
private var gun_delay = 0.0;

function Start () {

}

function Update () {
	var gun_pivot = transform.FindChild("gun pivot");
	if(gun_pivot){
		gun_pivot.RotateAround(
			transform.FindChild("point_pivot").position, 
			transform.FindChild("point_pivot").rotation * Vector3(0,1,0),
			Time.deltaTime * 100.0);
		gun_pivot.RotateAround(
			transform.FindChild("point_pivot").position, 
			transform.FindChild("point_pivot").rotation * Vector3(1,0,0),
			Time.deltaTime * Mathf.Sin(Time.time * 10) * 100.0);
	}
	if(gun_delay <= 0.0){
		gun_delay += 0.1;
		var point_muzzle_flash = gun_pivot.FindChild("point_muzzleflash");
		Instantiate(muzzle_flash, point_muzzle_flash.position, point_muzzle_flash.rotation);
		var bullet = Instantiate(bullet_obj, point_muzzle_flash.position, point_muzzle_flash.rotation);
		bullet.GetComponent(BulletScript).SetVelocity(point_muzzle_flash.forward * 300.0);
	}
	gun_delay -= Time.deltaTime;
}