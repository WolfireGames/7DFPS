using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public PlayerControllerSettings ControllerSettings;
	protected SixenseControllerData ControllerData;
	
	void Start () {
	
	}
	
	
	public void UpdateController () {
		ControllerData = SixenseControllerManager.GetControllerData(ControllerSettings.Hand);
	}
}
