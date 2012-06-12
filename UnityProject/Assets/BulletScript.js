#pragma strict

var bullet_obj : GameObject;
var bullet_hole_obj : GameObject;
private var old_pos;
private var hit_something = false;
private var line_renderer : LineRenderer; 
private var velocity : Vector3;
private var life_time = 0.0;
private var death_time = 0.0;
private var segment = 1;

function SetVelocity(vel : Vector3){
	this.velocity = vel;
}
		
function Start () {
	line_renderer = GetComponent(LineRenderer);
	line_renderer.SetPosition(0, transform.position);
	line_renderer.SetPosition(1, transform.position);
	old_pos = transform.position;
}

function Update () {
	if(!hit_something){
		life_time += Time.deltaTime;
		if(life_time > 1.5){
			hit_something = true;
		}
		transform.position += velocity * Time.deltaTime;
		velocity += Physics.gravity * Time.deltaTime;
		var hit:RaycastHit;
		var mask = 1<<8;
		mask = ~mask;
		if(Physics.Linecast(old_pos, transform.position, hit, mask)){
			transform.position = hit.point;
			var ricochet_amount = Vector3.Dot(velocity.normalized, hit.normal) * -1.0;
			if(Random.Range(0.0,1.0) > ricochet_amount && Vector3.Magnitude(velocity) * (1.0-ricochet_amount) > 10.0){
				var ricochet = Instantiate(bullet_obj, hit.point, transform.rotation);
				ricochet.GetComponent(BulletScript).SetVelocity(Vector3.Reflect(velocity * 0.3 * (1.0-ricochet_amount), hit.normal));
			}
			if(Vector3.Magnitude(velocity) > 50){
				Instantiate(bullet_hole_obj, hit.point, Quaternion.identity);
			}
			hit_something = true;
		}
		line_renderer.SetVertexCount(segment+1);
		line_renderer.SetPosition(segment, transform.position);
		++segment;
	} else {
		life_time += Time.deltaTime;
		death_time += Time.deltaTime;
		//Destroy(this.gameObject);
	}
	for(var i=0; i<segment; ++i){
		var start_color = Color(1,1,1,(1.0 - life_time * 5.0)*0.05);
		var end_color = Color(1,1,1,(1.0 - death_time * 5.0)*0.05);
		line_renderer.SetColors(start_color, end_color);
	}
}