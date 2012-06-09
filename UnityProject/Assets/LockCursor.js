#pragma strict

function Start () {

}

function OnMouseDown () {
    // Lock the cursor
    Screen.lockCursor = true;
}

function Update () {
    if (Input.GetKeyDown ("escape"))
        Screen.lockCursor = false;
}