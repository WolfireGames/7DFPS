#pragma strict

private var num_rounds = 8;
private var max_rounds = 8;

function RemoveRound() {
	var round_obj = transform.FindChild("round_"+num_rounds);
	round_obj.renderer.enabled = false;
	--num_rounds;
}

function AddRound() {
	if(num_rounds >= max_rounds){
		return;
	}
	++num_rounds;
	var round_obj = transform.FindChild("round_"+num_rounds);
	round_obj.renderer.enabled = true;
}

function NumRounds() : int {
	return num_rounds;
}

function Start () {

}

function Update () {

}