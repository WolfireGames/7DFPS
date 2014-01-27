using UnityEngine;
using System.Collections;

public class RotationAxis {

	public Vector3 X {get{return _x;}}
	public Vector3 Y {get{return _y;}}
	public Vector3 Z {get{return _z;}}
	
	private Vector3 _x;
	private Vector3 _y;
	private Vector3 _z;
	
	public RotationAxis (Vector3 x, Vector3 y, Vector3 z)
	{
		_x = x;
		_y = y;
		_z = z;
	}
}
