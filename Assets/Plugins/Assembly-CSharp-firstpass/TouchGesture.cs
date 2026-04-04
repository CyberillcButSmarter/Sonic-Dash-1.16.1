using UnityEngine;

public class TouchGesture : BaseGesture
{
	public FingerLocation startsOnObject = FingerLocation.Always;

	public FingerLocation movesOnObject = FingerLocation.Always;

	public FingerLocation endsOnObject = FingerLocation.Always;

	public FingerCountRestriction restrictFingerCount;

	public bool averagePoint = true;

	public bool isDown;

	public bool[] isActives = new bool[5];

	public Vector2 touchPositionStart;

	public Vector2 touchPosition;

	public Vector2 touchPositionEnd;

	public float touchMagnitude;

	private bool averageActive;

	private bool sentSomething;

	private bool gestureIsGoing;

	public bool isActive
	{
		get
		{
			if (averageActive)
			{
				return averageActive;
			}
			for (int i = 0; i < fingerCount; i++)
			{
				if (!isActives[i])
				{
					return false;
				}
			}
			return true;
		}
	}

	protected override void Start()
	{
		base.Start();
	}

	protected override void EnableGesture()
	{
		base.EnableGesture();
		FingerControl._delegateFingerDownInternal += TouchGestureDownAndMovingBegin;
		FingerControl._delegateFingerMovingInternal += TouchGestureDownAndMovingMove;
		FingerControl._delegateFingerUpInternal += TouchGestureDownAndMovingEnd;
	}

	protected override void DisableGesture()
	{
		base.DisableGesture();
		FingerControl._delegateFingerDownInternal -= TouchGestureDownAndMovingBegin;
		FingerControl._delegateFingerMovingInternal -= TouchGestureDownAndMovingMove;
		FingerControl._delegateFingerUpInternal -= TouchGestureDownAndMovingEnd;
	}

	protected void TouchGestureIsDown(Finger fingerIn, bool isDownIn)
	{
		if (isDown != isDownIn && !isDownIn && ActiveCount() == 0)
		{
			isDown = false;
		}
	}

	protected void TouchGestureDownAndMovingBegin(Finger fingerIn)
	{
		sentSomething = false;
		finger = fingerIn;
		touchPosition = CalcPosForAllFingers(fingerIn.startPosition);
		touchPositionStart = touchPosition;
		fingerCount = ActiveCount();
		if (FingerActivated(startsOnObject, (!averagePoint) ? fingerIn.startPosition : touchPosition) && FingerCountGood(fingerCount, restrictFingerCount))
		{
			isActives[fingerIn.Index()] = true;
			averageActive = (averagePoint ? true : false);
			if (!isDown)
			{
				GestureMessage("GestureStartTouch");
				sentSomething = true;
				gestureIsGoing = true;
			}
		}
		else
		{
			averageActive = false;
			isActives[fingerIn.Index()] = false;
		}
		isDown = true;
	}

	protected void TouchGestureDownAndMovingMove(Finger fingerIn)
	{
		touchPosition = CalcPosForAllFingers(fingerIn.position);
		fingerCount = ActiveCount();
		if (gestureIsGoing && FingerActivated(movesOnObject, (!averagePoint) ? fingerIn.position : touchPosition) && FingerCountGood(fingerCount, restrictFingerCount))
		{
			isActives[finger.Index()] = true;
			averageActive = (averagePoint ? true : false);
			finger = fingerIn;
			GestureMessage("GestureMoveTouch");
			sentSomething = true;
		}
		else
		{
			averageActive = false;
			isActives[finger.Index()] = false;
		}
	}

	protected void TouchGestureDownAndMovingEnd(Finger fingerIn)
	{
		isActives[fingerIn.Index()] = false;
		fingerCount = ActiveCount();
		if (fingerCount < 2)
		{
			averageActive = false;
		}
		if (fingerCount == 0)
		{
			isDown = false;
			touchPositionEnd = touchPosition;
			touchMagnitude = Mathf.Abs((touchPositionEnd - touchPositionStart).magnitude);
			if (FingerActivated(endsOnObject, fingerIn.endPosition) && sentSomething)
			{
				finger = fingerIn;
				GestureMessage("GestureEndTouch");
			}
			sentSomething = false;
			gestureIsGoing = false;
		}
	}
}
