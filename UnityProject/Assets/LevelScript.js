#pragma strict

var level_tiles : GameObject[];

function Start () {
	for(var i=-5; i<5; ++i){
		Instantiate(level_tiles[Random.Range(0,level_tiles.Length)], Vector3(0,0,i*20), Quaternion.identity);
	}
}

function Update () {

}