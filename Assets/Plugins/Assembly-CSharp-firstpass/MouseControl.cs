using UnityEngine;

public class MouseControl : FingerControl
{
	public static int mouseButtons = 5;

	public float wheelPinchThreshold = 0.01f;

	private bool pinchingMouseWheel;

	protected override void Start()
	{
		base.Start();
	}

	protected override Finger NewFinger(int index)
	{
		return new MouseFinger(index);
	}

	public override int FingerCount()
	{
		return 1;
	}

	public override bool IsFingers()
	{
		return false;
	}

	protected override void Update()
	{
		base.Update();
		if (!FingerControl.trackPinch)
		{
			return;
		}
		float axis = Input.GetAxis("Mouse ScrollWheel");
		if (Mathf.Abs(axis) > wheelPinchThreshold)
		{
			float magnitudeDelta = axis * 200f;
			if (!pinchingMouseWheel)
			{
				SendEventPinchBegin(fingers[0], fingers[0], PinchGesture.PinchDirection.Vertical, magnitudeDelta);
				SendEventPinchMove(fingers[0], fingers[0], PinchGesture.PinchDirection.Vertical, magnitudeDelta);
				pinchingMouseWheel = true;
			}
			else
			{
				SendEventPinchMove(fingers[0], fingers[0], PinchGesture.PinchDirection.Vertical, magnitudeDelta);
			}
		}
		else if (pinchingMouseWheel)
		{
			SendEventPinchEnd(fingers[0], fingers[0]);
			pinchingMouseWheel = false;
		}
	}
}
