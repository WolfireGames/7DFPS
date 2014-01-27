using UnityEngine;
using System.Collections;

public class SixenseInitializer : MonoBehaviour {

	void Start () {
		SixenseControllerManager.InitializeSixense();
	}
	
	void Update()
	{
	}
}
