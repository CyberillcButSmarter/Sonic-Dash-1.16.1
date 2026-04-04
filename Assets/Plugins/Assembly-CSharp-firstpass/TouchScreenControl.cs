using UnityEngine;

public class TouchScreenControl : FingerControl
{
	public int fingersUsed = 5;

	private Touch nullTouch = default(Touch);

	private int[] fingerBeingUsed;

	public override int FingerCount()
	{
		return fingersUsed;
	}

	protected override void Start()
	{
		fingerBeingUsed = new int[FingerCount()];
		base.Start();
	}

	public override bool IsFingers()
	{
		return true;
	}

	protected override Finger NewFinger(int index)
	{
		return new TouchScreenFinger(index);
	}

	private bool HasValidTouch(Finger finger)
	{
		return fingerBeingUsed[finger.Index()] != -1;
	}

	private Touch GetTouch(Finger finger)
	{
		int num = fingerBeingUsed[finger.Index()];
		if (num == -1)
		{
			return nullTouch;
		}
		return Input.touches[num];
	}
}
