private var motor : CharacterMotor;

private var forward_input_delay = 10.0;
private var old_vert_axis = 0.0;
private var running = false;

// Use this for initialization
function Awake () {
	motor = GetComponent(CharacterMotor);
}

// Update is called once per frame
function Update () {
	// Get the input vector from kayboard or analog stick
	var directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
	
	Debug.Log(Input.GetAxis("Vertical"));
	
	if(old_vert_axis < 0.9 && Input.GetAxis("Vertical") >= 0.9){
		if(forward_input_delay < 0.4){
			motor.SetRunning(Mathf.Clamp(0.2/forward_input_delay,0.5,1.0));
			running = true;			
		}
		forward_input_delay = 0.0;
	}
	forward_input_delay += Time.deltaTime;
	if(forward_input_delay > 0.4){
		motor.SetRunning(0.0);
		running = false;
	}
	if(running){
		directionVector.z = 1.0;
	}
	old_vert_axis = Input.GetAxis("Vertical");
	
	if (directionVector != Vector3.zero) {
		// Get the length of the directon vector and then normalize it
		// Dividing by the length is cheaper than normalizing when we already have the length anyway
		var directionLength = directionVector.magnitude;
		directionVector = directionVector / directionLength;
		
		// Make sure the length is no bigger than 1
		directionLength = Mathf.Min(1, directionLength);
		
		// Make the input vector more sensitive towards the extremes and less sensitive in the middle
		// This makes it easier to control slow speeds when using analog sticks
		directionLength = directionLength * directionLength;
		
		// Multiply the normalized direction vector by the modified length
		directionVector = directionVector * directionLength;
	}
	
	// Apply the direction to the CharacterMotor
	motor.inputMoveDirection = transform.rotation * directionVector;
	motor.inputJump = Input.GetButton("Jump");	
}

function FixedUpdate() {
}

// Require a character controller to be attached to the same game object
@script RequireComponent (CharacterMotor)
@script AddComponentMenu ("Character/FPS Input Controller")
