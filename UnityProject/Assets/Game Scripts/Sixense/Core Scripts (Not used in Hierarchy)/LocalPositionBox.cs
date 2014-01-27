using UnityEngine;
using System.Collections;

//boundary box for a controller
[System.Serializable]
public class LocalPositionBox {
	
	public float XMin;
	public float XMax;
	public float YMin;
	public float YMax;
	public float ZMin;
	public float ZMax;
	
	public LocalPositionBox(float xmin, float xmax, float ymin, float ymax, float zmin, float zmax)
	{
		XMin = xmin;
		XMax = xmax;
		YMin = ymin;
		YMax = ymax;
		ZMin = zmin;
		ZMax = zmax;
	}
	
	public LocalPositionBox(float min, float max)
	{
		XMin = YMin = ZMin = min;
		XMax = YMax = ZMax = max;
	}
}
