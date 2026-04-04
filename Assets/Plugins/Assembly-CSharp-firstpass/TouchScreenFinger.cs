using UnityEngine;

public class TouchScreenFinger : Finger
{
	public TouchScreenFinger(int index)
		: base(index)
	{
	}

	public override bool SetState()
	{
		base.SetState();
		if (Input.touchCount > Index())
		{
			Touch touch = Input.touches[Index()];
			touchPhase = touch.phase;
			position = touch.position;
			deltaPosition = touch.deltaPosition;
			return true;
		}
		touchPhase = TouchPhase.Canceled;
		position = Vector2.zero;
		deltaPosition = Vector2.zero;
		motionState = FingerMotionState.Inactive;
		return false;
	}
}
