using System.Collections.Generic;
using UnityEngine;

public class LongPressGesture : BaseGesture
{
	public FingerLocation pressLocation;

	public float longPressTime = 1.5f;

	public float maxPressDistance = 20f;

	public FingerCountRestriction restrictFingerCount;

	public float timeDifference;

	private List<Finger> fingersUsed = new List<Finger>();

	private bool possiblePress;

	private float lastStartTime;

	protected override void EnableGesture()
	{
		base.EnableGesture();
		possiblePress = false;
		FingerControl._delegateFingerDownInternal += GestureDownAndMovingBegin;
		FingerControl._delegateFingerMovingInternal += GestureDownAndMovingMove;
		FingerControl._delegateFingerUpInternal += GestureDownAndMovingEnd;
	}

	protected override void DisableGesture()
	{
		base.DisableGesture();
		possiblePress = false;
		FingerControl._delegateFingerDownInternal -= GestureDownAndMovingBegin;
		FingerControl._delegateFingerMovingInternal -= GestureDownAndMovingMove;
		FingerControl._delegateFingerUpInternal -= GestureDownAndMovingEnd;
	}

	protected void GestureDownAndMovingBegin(Finger fingerIn)
	{
		CleanFingers(fingersUsed);
		if (fingersUsed.Count == 0)
		{
			possiblePress = true;
			timeDifference = 0f;
		}
		if (!fingersUsed.Contains(fingerIn))
		{
			fingersUsed.Add(fingerIn);
			lastStartTime = Time.time;
			if (!FingerActivated(pressLocation, fingerIn.position))
			{
				possiblePress = false;
			}
		}
	}

	protected void GestureDownAndMovingMove(Finger fingerIn)
	{
		if (possiblePress && distanceGood(finger.position, finger.startPosition))
		{
			CheckLongPress();
		}
	}

	protected void GestureDownAndMovingEnd(Finger fingerIn)
	{
		if (fingersUsed.Contains(fingerIn))
		{
			fingersUsed.Remove(fingerIn);
		}
		CleanFingers(fingersUsed);
		distanceGood(fingerIn.endPosition, fingerIn.startPosition);
		possiblePress = false;
	}

	private void Update()
	{
		if (possiblePress)
		{
			float num = Time.time - lastStartTime;
			if (num >= longPressTime)
			{
				CheckLongPress();
			}
		}
	}

	protected void CheckLongPress()
	{
		if (!possiblePress)
		{
			return;
		}
		float num = Time.time - lastStartTime;
		if (num >= longPressTime)
		{
			if (!FingerCountGood(fingersUsed.Count, restrictFingerCount))
			{
				possiblePress = false;
				return;
			}
			finger = fingersUsed[0];
			fingerCount = fingersUsed.Count;
			timeDifference = num;
			GestureMessage("GestureLongPress");
			possiblePress = false;
		}
	}

	private bool distanceGood(Vector2 point1, Vector2 point2)
	{
		if ((point1 - point2).magnitude > maxPressDistance)
		{
			possiblePress = false;
			return false;
		}
		return true;
	}

	public void LongPressDoneCallBack(Finger finger)
	{
	}
}
