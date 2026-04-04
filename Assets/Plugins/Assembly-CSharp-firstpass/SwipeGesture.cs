using System;
using UnityEngine;

public class SwipeGesture : BaseGesture
{
	public enum SwipeDirection
	{
		Any = 0,
		LeftDiagonal = 1,
		RightDiagonal = 2,
		Vertical = 3,
		Horizontal = 4,
		AnyCross = 5,
		AnyPlus = 6,
		Up = 7,
		Plus45 = 8,
		Right = 9,
		Plus135 = 10,
		Down = 11,
		Minus135 = 12,
		Left = 13,
		Minus45 = 14,
		None = 15
	}

	public delegate void SwipeCallBack(SwipeGesture gesture);

	public SwipeDirection restrictDirection;

	public FingerCountRestriction restrictFingerCount;

	public FingerLocation startsOnObject = FingerLocation.Always;

	public FingerLocation movesOnObject = FingerLocation.Always;

	public FingerLocation endsOnObject = FingerLocation.Always;

	public float minGestureLength = 75f;

	public float maxTime = 1.5f;

	public SwipeDirection swipeDirection;

	public int swipeFingerCount;

	public Vector2 swipePosition;

	public Vector2 startPosition;

	public Vector2 endPosition;

	private bool swiping;

	protected override void EnableGesture()
	{
		base.EnableGesture();
		FingerControl.AddSwipeCallback();
		FingerControl._delegateSwipeInternal += SwipeGestureOnSwipe;
		FingerControl._delegateSwipeMoveInternal += SwipeGestureOnSwipeMove;
		FingerControl._delegateSwipeCancelInternal += SwipeGestureOnSwipeCancel;
	}

	protected override void DisableGesture()
	{
		base.DisableGesture();
		FingerControl.RemoveSwipeCallback();
		FingerControl._delegateSwipeInternal -= SwipeGestureOnSwipe;
		FingerControl._delegateSwipeMoveInternal -= SwipeGestureOnSwipeMove;
		FingerControl._delegateSwipeCancelInternal -= SwipeGestureOnSwipeCancel;
	}

	protected void SwipeGestureOnSwipeCancel()
	{
		if (swiping)
		{
			GestureMessage("GestureSwipeEnd");
		}
	}

	protected void SwipeGestureOnSwipeMove(Finger fingerIn, SwipeDirection direction, int fingerCount)
	{
		swiping = true;
		swipePosition = CalcPosForAllFingers(fingerIn.position);
		if (FingerActivated(movesOnObject, fingerIn.position) && FingerCountGood(fingerCount, restrictFingerCount) && DirectionMatches(direction, restrictDirection) && !base.IsGestureIgnored)
		{
			swipeFingerCount = fingerCount;
			finger = fingerIn;
			swipeDirection = direction;
			startPosition = fingerIn.startPosition;
			GestureMessage("GestureSwipeMove");
		}
	}

	protected void SwipeGestureOnSwipe(Finger[] fingers, SwipeSegmentList[] segmentsList, int fingersDown)
	{
		swiping = false;
		if (base.IsGestureIgnored)
		{
			ClearIgnoreGesture();
			return;
		}
		float num = Time.time - fingers[0].startTime;
		if (num > maxTime)
		{
			return;
		}
		try
		{
			for (int i = 0; i < fingersDown; i++)
			{
				if (segmentsList[i] == null || segmentsList[i].Count == 0)
				{
					continue;
				}
				SwipeSegmentList swipeSegmentList = segmentsList[i];
				if (swipeSegmentList.Count != 1)
				{
					swipeSegmentList = FingerControl.Factory().TryToMakeOneSegment(swipeSegmentList);
					if (swipeSegmentList.Count != 1)
					{
						return;
					}
				}
				fingers[i].swipeDirection = swipeSegmentList[0].direction;
			}
			swipeDirection = fingers[0].swipeDirection;
			if (fingersDown >= 1)
			{
				Vector2 vector = new Vector2(0f, 0f);
				Vector2 vector2 = new Vector2(0f, 0f);
				int[] array = new int[Enum.GetValues(typeof(SwipeDirection)).Length];
				for (int j = 0; j < array.Length; j++)
				{
					array[j] = 0;
				}
				int num2 = 0;
				for (int k = 0; k < fingersDown; k++)
				{
					if (!FingerActivated(startsOnObject, fingers[k].startPosition) || !FingerActivated(endsOnObject, fingers[k].endPosition))
					{
						return;
					}
					if (swipeDirection != fingers[k].swipeDirection)
					{
						if (!FingerControl.FriendlySwipeDirections(swipeDirection, fingers[k].swipeDirection))
						{
							return;
						}
						array[(int)fingers[k].swipeDirection]++;
						num2++;
					}
					else
					{
						array[(int)swipeDirection]++;
						vector += fingers[k].startPosition;
						vector2 += fingers[k].endPosition;
					}
				}
				if (num2 > 0)
				{
					int num3 = 0;
					for (int l = 1; l < array.Length; l++)
					{
						if (array[num3] < array[l])
						{
							num3 = l;
						}
					}
					swipeDirection = (SwipeDirection)num3;
				}
				startPosition = vector / fingersDown;
				endPosition = vector2 / fingersDown;
			}
			else
			{
				startPosition = fingers[0].startPosition;
				endPosition = fingers[0].endPosition;
			}
			if (!((endPosition - startPosition).magnitude < minGestureLength) && FingerCountGood(fingersDown, restrictFingerCount) && DirectionMatches(swipeDirection, restrictDirection))
			{
				swipeFingerCount = fingersDown;
				SetFingers(fingers, fingersDown);
				GestureMessage("GestureSwipe");
			}
		}
		finally
		{
			GestureMessage("GestureSwipeEnd");
		}
	}

	public static bool DirectionMatches(SwipeDirection direction, SwipeDirection directionRestrict)
	{
		if (directionRestrict == SwipeDirection.Any || direction == directionRestrict || (directionRestrict == SwipeDirection.LeftDiagonal && (direction == SwipeDirection.Minus45 || direction == SwipeDirection.Plus135)) || (directionRestrict == SwipeDirection.RightDiagonal && (direction == SwipeDirection.Plus45 || direction == SwipeDirection.Minus135)) || (directionRestrict == SwipeDirection.Vertical && (direction == SwipeDirection.Up || direction == SwipeDirection.Down)) || (directionRestrict == SwipeDirection.Horizontal && (direction == SwipeDirection.Left || direction == SwipeDirection.Right)) || (directionRestrict == SwipeDirection.AnyCross && (direction == SwipeDirection.Minus45 || direction == SwipeDirection.Plus135 || direction == SwipeDirection.Plus45 || direction == SwipeDirection.Minus135)) || (directionRestrict == SwipeDirection.AnyPlus && (direction == SwipeDirection.Left || direction == SwipeDirection.Right || direction == SwipeDirection.Up || direction == SwipeDirection.Down)))
		{
			return true;
		}
		return false;
	}

	public static SwipeDirection GetDirection(bool isForward, SwipeDirection direction)
	{
		if (isForward)
		{
			return direction;
		}
		return GetReverseDirection(direction);
	}

	public static SwipeDirection GetReverseDirection(SwipeDirection direction)
	{
		switch (direction)
		{
		case SwipeDirection.Up:
			return SwipeDirection.Down;
		case SwipeDirection.Plus45:
			return SwipeDirection.Minus135;
		case SwipeDirection.Right:
			return SwipeDirection.Left;
		case SwipeDirection.Plus135:
			return SwipeDirection.Minus45;
		case SwipeDirection.Down:
			return SwipeDirection.Up;
		case SwipeDirection.Minus135:
			return SwipeDirection.Plus45;
		case SwipeDirection.Left:
			return SwipeDirection.Right;
		case SwipeDirection.Minus45:
			return SwipeDirection.Plus135;
		case SwipeDirection.Any:
		case SwipeDirection.LeftDiagonal:
		case SwipeDirection.RightDiagonal:
		case SwipeDirection.Vertical:
		case SwipeDirection.Horizontal:
		case SwipeDirection.AnyCross:
		case SwipeDirection.AnyPlus:
			return SwipeDirection.Any;
		default:
			return SwipeDirection.Any;
		}
	}
}
