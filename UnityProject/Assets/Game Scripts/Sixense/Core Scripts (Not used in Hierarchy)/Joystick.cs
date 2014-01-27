using UnityEngine;
using System.Collections;

public class Joystick {
	
	public float X {get{return _x;}}
	public float Y {get{return _y;}}
	
	private float _x;
	private float _y;
	
	public Joystick (float x, float y)
	{
		_x = x;
		_y = y;
	}
}
