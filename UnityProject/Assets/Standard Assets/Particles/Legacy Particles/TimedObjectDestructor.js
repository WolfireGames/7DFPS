var timeOut = 1.0;
var detachChildren = false;

function Awake ()
{
	Invoke ("DestroyNow", timeOut);
}

function DestroyNow ()
{
	if (detachChildren) {
		transform.DetachChildren ();
	}
	DestroyObject (gameObject);
}