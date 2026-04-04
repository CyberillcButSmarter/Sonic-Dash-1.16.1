using UnityEngine;

public class SliceGesture : BaseGesture
{
	public SwipeGesture.SwipeDirection restrictDirection;

	public SwipeGesture.SwipeDirection sliceDirection;

	public Vector3 sliceStartPosition;

	public Vector3 sliceEndPosition;

	private bool slicing;

	private bool sliced;

	public SwipeSegment swipeSegment;

	protected override void EnableGesture()
	{
		base.EnableGesture();
		FingerControl.AddSwipeCallback();
		FingerControl._delegateSwipeInternal += SliceGestureOnSwipe;
		FingerControl._delegateDownAndMovingBeginInternal += SliceGestureDownAndMovingBegin;
		FingerControl._delegateDownAndMovingMoveInternal += SliceGestureDownAndMovingMove;
	}

	protected override void DisableGesture()
	{
		base.DisableGesture();
		FingerControl.RemoveSwipeCallback();
		FingerControl._delegateSwipeInternal -= SliceGestureOnSwipe;
		FingerControl._delegateDownAndMovingBeginInternal -= SliceGestureDownAndMovingBegin;
		FingerControl._delegateDownAndMovingMoveInternal -= SliceGestureDownAndMovingMove;
	}

	protected void SliceGestureOnSwipe(Finger[] fingersIn, SwipeSegmentList[] segmentsList, int fingerCount)
	{
		if (fingerCount > 1)
		{
			return;
		}
		finger = fingersIn[0];
		SwipeSegmentList swipeSegmentList = segmentsList[0];
		if (swipeSegmentList.Count != 1)
		{
			swipeSegmentList = FingerControl.Factory().TryToMakeOneSegment(swipeSegmentList);
			if (swipeSegmentList.Count != 1)
			{
				return;
			}
		}
		swipeSegment = swipeSegmentList[0];
		if (sliced && SwipeGesture.DirectionMatches(swipeSegment.direction, restrictDirection))
		{
			sliceDirection = swipeSegment.direction;
			GestureMessage("GestureSlice");
		}
		sliced = false;
	}

	protected void SliceGestureDownAndMovingBegin(Finger finger)
	{
		sliced = false;
		slicing = false;
	}

	protected void SliceGestureDownAndMovingMove(Finger fingerIn, Vector2 fingerPos, Vector2 delta)
	{
		if (IsOnObject(fingerPos))
		{
			if (!slicing && !sliced)
			{
				slicing = true;
				finger = fingerIn;
				sliceStartPosition = ScreenToWorldPosition(fingerPos);
			}
		}
		else if (slicing)
		{
			sliceEndPosition = ScreenToWorldPosition(fingerPos);
			slicing = false;
			sliced = true;
		}
	}
}
