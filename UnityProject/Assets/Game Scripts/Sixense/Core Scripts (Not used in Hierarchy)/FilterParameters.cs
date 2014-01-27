using UnityEngine;
using System.Collections;

public class FilterParameters {

	public float NearRange;
	public float NearValue;
	public float FarRange;
	public float FarValue;
	
	public FilterParameters(float nearRange, float nearValue, float farRange, float farValue)
	{
		NearRange = nearRange;
		NearValue = nearValue;
		FarRange = farRange;
		FarValue = farValue;
	}
}
