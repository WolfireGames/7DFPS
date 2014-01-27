using UnityEngine;
using System.Collections;

/*
 This class is a wrapper class of sixenseControllerRawData, which I don't consider to be very
 'Unity-user-friendly' (for example, rather than an 'int' for all button data, I've added a 
 button class to separate each one out), so this takes care of putting the data into objects that
 are easier to use. 
 All data is set to be read-only for good-practice purposes. This is data you're receiving, if adjustments
 need to be made, that's the job of creating settings as I've done in PlayerControllerSettings
 */

public class SixenseControllerData {
	
	private Vector3 _position;
	private RotationAxis _rotationAxis;
	private Joystick _joystick;
	private float _trigger;
	private Buttons _buttons;
	private Quaternion _rotationQuaternion;
	private bool _isEnabled;
	private int _controllerIndex;
	private bool _isDocked;
	private SixenseControllerHand _hand;
	
	public Vector3 Position {get{return _position;}}
	public RotationAxis RotationAxis {get{return _rotationAxis;}}
	public Joystick Joystick {get{return _joystick;}}
	public float Trigger {get{return _trigger;}}
	public Buttons Buttons {get{return _buttons;}}
	public Quaternion RotationQuaternion {get{return _rotationQuaternion;}}
	public bool IsEnabled {get{return _isEnabled;}}
	public int ControllerIndex {get{return _controllerIndex;}}
	public bool IsDocked {get{return _isDocked;}}
	public SixenseControllerHand Hand {get{return _hand;}}
	
	public SixenseControllerData(sixenseControllerRawData data)
	{
		_position = data.Position;
		_rotationAxis = new RotationAxis(data.RotationX, data.RotationY, data.RotationZ);
		_joystick = new Joystick(data.JoystickX, data.JoystickY);
		_trigger = data.Trigger;
		_buttons = new Buttons(data.Buttons);
		_rotationQuaternion = data.RotationQuaternion;
		_isEnabled = data.Enabled == 1 ? true : false;
		_controllerIndex = data.ControllerIndex;
		_isDocked = data.IsDocked == 1 ? true : false;
		_hand = data.WhichHand == 0 ? SixenseControllerHand.LeftHand : SixenseControllerHand.RightHand;
	}
}
