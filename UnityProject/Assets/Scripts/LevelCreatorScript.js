#pragma strict

var level_tiles : GameObject[];

function Start () {
	for(var i=-5; i<5; ++i){
		var level : GameObject = Instantiate(level_tiles[Random.Range(0,level_tiles.Length)], Vector3(0,0,i*20), Quaternion.identity);
		var enemies = level.transform.FindChild("enemies");
		if(enemies){
			for(var child : Transform in enemies){
				if(Random.Range(0.0,1.0) < 0.9){
					GameObject.Destroy(child.gameObject);
				}
			}
		}
		var lights = level.GetComponentsInChildren(Light);
		for(var light : Light in lights){
			light.shadows = LightShadows.None;
		}
		var players = level.transform.FindChild("player_spawn");
		if(players){
			if(i != 0){
				for(var child : Transform in players){
					GameObject.Destroy(child.gameObject);
				}
			} else {
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
		}
	}
}

function Update () {

}