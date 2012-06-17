#pragma strict

function Start () {
	var enemies = transform.FindChild("enemies");
	if(enemies){
		for(var child : Transform in enemies){
			if(Random.Range(0.0,1.0) < 0.9){
				GameObject.Destroy(child.gameObject);
			}
		}
	}
	var players = transform.FindChild("player_spawn");
	if(players){
		var num = 0;
		for(var child : Transform in players){
			++num;
		}
		var save = Random.Range(0,num);
		var j=0;
		for(var child : Transform in players){
			if(j != save){
				GameObject.Destroy(child.gameObject);
			}
			++j;
		}
	}
	var items = transform.FindChild("items");
	if(items){
		for(var child : Transform in items){
			if(Random.Range(0.0,1.0) < 0.9){
				GameObject.Destroy(child.gameObject);
			}
		}
	}
}

function Update () {

}