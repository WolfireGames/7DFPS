using UnityEngine;
using System.Collections;
using System.Text;

public class ControllerDebugger : PlayerController {

	void Start () {
	
	}
	
	
	void Update () {
		UpdateController();
		Debug.Log(GetDebugData());
	}
	
	private string GetDebugData()
	{
		StringBuilder debugData = new StringBuilder();
		
		if(Mathf.Abs(ControllerData.Joystick.X) > 0.01)
		{
			debugData.Append(" Joy X: " + ControllerData.Joystick.X);
		}
		if(Mathf.Abs(ControllerData.Joystick.Y) > 0.01)
		{
			debugData.Append(" Joy Y: " + ControllerData.Joystick.Y);
		}
		if(Mathf.Abs(ControllerData.Trigger) > 0.01)
		{
			debugData.Append(" Trigger: " + ControllerData.Trigger);
		}
		debugData.Append(ControllerData.Buttons.One ? " Button 1" : "");
		debugData.Append(ControllerData.Buttons.Two ? " Button 2" : "");
		debugData.Append(ControllerData.Buttons.Three ? " Button 3" : "");
		debugData.Append(ControllerData.Buttons.Four ? " Button 4" : "");
		debugData.Append(ControllerData.Buttons.Start ? " StartButton" : "");
		debugData.Append(ControllerData.Buttons.Bumper ? " BumperButton" : "");
		debugData.Append(ControllerData.Buttons.Joystick ? " JoyButton" : "");
		
		return debugData.ToString();
	}
}
