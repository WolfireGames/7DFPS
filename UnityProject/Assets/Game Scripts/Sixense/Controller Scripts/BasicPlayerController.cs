using UnityEngine;
using System.Collections;

public class BasicPlayerController : PlayerController {
	
	//the object to be affected by the Hydra controller
	public GameObject ControllerObject;
	
	//we need to log the start position of the object, so that controller position is relative to the original position
	protected Vector3 StartPosition = Vector3.zero;
	protected bool IsStartPositionSet = false;
	
	
	public void UpdateBasicController () {
		if(!IsStartPositionSet) //set the start position if it has not been set.
		{
			StartPosition = ControllerObject.transform.position;
			IsStartPositionSet = true;
		}
		UpdateController(); //found in PlayerController, gets the newest controller data
		UpdateRotation();
		UpdatePosition();
	}
	
	private void UpdateRotation()
	{
		//updates the rotation of the object, and takes into account inverted rotations and adjustments
		ControllerObject.transform.rotation = new Quaternion(
		                (ControllerSettings.InvertXRotation ? -1 : 1) * ControllerData.RotationQuaternion.x,
		                (ControllerSettings.InvertYRotation ? -1 : 1) * ControllerData.RotationQuaternion.y,
		                (ControllerSettings.InvertZRotation ? -1 : 1) * ControllerData.RotationQuaternion.z,
		                 ControllerData.RotationQuaternion.w);
		ControllerObject.transform.RotateAroundLocal(-Vector3.up, ControllerSettings.AutomaticRotationAdjust.y * Mathf.Deg2Rad);
		ControllerObject.transform.RotateAroundLocal(Vector3.forward, ControllerSettings.AutomaticRotationAdjust.z * Mathf.Deg2Rad);
		ControllerObject.transform.RotateAroundLocal(Vector3.right, ControllerSettings.AutomaticRotationAdjust.x * Mathf.Deg2Rad);
	}
	
	private void UpdatePosition()
	{
		//updates the position of the object, and takes into account inversions, sensitivity, and adjustments
		var positionDelta = new Vector3(Mathf.Clamp((ControllerSettings.InvertXMovement ? -1 : 1) * (ControllerData.Position.x * ControllerSettings.MovementSensitivity.x), ControllerSettings.MovementLimitBox.XMin, ControllerSettings.MovementLimitBox.XMax),
		                                Mathf.Clamp((ControllerSettings.InvertYMovement ? -1 : 1) * (ControllerData.Position.y * ControllerSettings.MovementSensitivity.y), ControllerSettings.MovementLimitBox.YMin, ControllerSettings.MovementLimitBox.YMax),
		                                Mathf.Clamp((ControllerSettings.InvertZMovement ? -1 : 1) * (ControllerData.Position.z * ControllerSettings.MovementSensitivity.z), ControllerSettings.MovementLimitBox.ZMin, ControllerSettings.MovementLimitBox.ZMax));
		ControllerObject.transform.position = new Vector3(StartPosition.x + ControllerSettings.AutomaticPositionAdjust.x + positionDelta.x,
		                                                  StartPosition.y + ControllerSettings.AutomaticPositionAdjust.y + positionDelta.y,
		                                                  StartPosition.z + ControllerSettings.AutomaticPositionAdjust.z + positionDelta.z);
	}
}
