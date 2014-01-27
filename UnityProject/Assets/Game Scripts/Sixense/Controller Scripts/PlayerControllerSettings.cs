using UnityEngine;
using System.Collections;

/*
 * Settings for the player controller including rotation/movement inversions, sensitivity, adjustments,
 * and which hand the controller represents.*/

[System.Serializable]
public class PlayerControllerSettings {
	
	//which hand should this controller pull data from
	public SixenseControllerHand Hand = SixenseControllerHand.LeftHand;
	
	//Automatic rotation adjustment. Example: if the object was facing the camera when your controller
	//was forward, and you want the object facing forward, set the y-value to 180.
	public Vector3 AutomaticRotationAdjust = Vector3.zero; //in degrees
	
	//invert any axis of rotation
	public bool InvertXRotation = false;
	public bool InvertYRotation = false;
	public bool InvertZRotation = false;
	
	//the position values that the controller spits out are pretty large, this well for me as default values.
	public Vector3 MovementSensitivity = new Vector3(0.01f, 0.01f, 0.01f);
	
	//Same as the rotation adjustment, put for the position.
	public Vector3 AutomaticPositionAdjust = Vector3.zero;
	
	//The limit of how far from the starting location (including position adjustment) that the object can move.
	public LocalPositionBox MovementLimitBox = new LocalPositionBox(-10, 10);
	
	//Invert X/Y/Z movements
	public bool InvertXMovement = false;
	public bool InvertYMovement = false;
	public bool InvertZMovement = false;
}
