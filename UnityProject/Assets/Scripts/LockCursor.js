#pragma strict

function Start () {	
	Screen.lockCursor = true;
}

function Update () {
    if (Input.GetKeyDown ("escape")){
        Screen.lockCursor = false;
    }
    if (Input.GetMouseButtonDown(0)){
	    Screen.lockCursor = true;
    }
}

function OnApplicationPause() {  
	Screen.lockCursor = false;
}

function OnApplicationFocus() {
	Screen.lockCursor = true;
}