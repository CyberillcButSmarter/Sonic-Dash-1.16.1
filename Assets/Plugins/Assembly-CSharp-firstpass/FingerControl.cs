using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class FingerControl : MonoBehaviour
{
	public delegate void FingerIsDownCallBackInternal(Finger finger, bool isDown);

	public delegate void FingerDownCallBackInternal(Finger finger);

	public delegate void FingerMovingCallBackInternal(Finger finger);

	public delegate void FingerUpCallBackInternal(Finger finger);

	public delegate void FingerStationaryBeginCallBackInternal(Finger finger, Vector2 fingerPos);

	public delegate void FingerStationaryCallBackInternal(Finger finger, float elapsedTime);

	public delegate void FingerStationaryEndCallBackInternal(Finger finger, Vector2 fingerPos, float elapsedTime);

	public delegate void FingerMoveBeginCallBackInternal(Finger finger, Vector2 fingerPos);

	public delegate void FingerMoveCallBackInternal(Finger finger, Vector2 fingerPos);

	public delegate void FingerMoveEndCallBackInternal(Finger finger, Vector2 fingerPos);

	public delegate void DownAndMovingBeginCallBackInternal(Finger finger);

	public delegate void DownAndMovingMoveCallBackInternal(Finger finger, Vector2 fingerPos, Vector2 delta);

	public delegate void DownAndMovingEndCallBackInternal(Finger finger);

	public delegate void PinchBeginCallBackInternal(Finger finger1, Finger finger2, PinchGesture.PinchDirection pinchDirection, float magnitudeDelta);

	public delegate void PinchMoveCallBackInternal(Finger finger1, Finger finger2, PinchGesture.PinchDirection pinchDirection, float magnitudeDelta);

	public delegate void PinchEndCallBackInternal(Finger finger1, Finger finger2);

	public delegate void SwipeCallBackInternal(Finger[] fingers, SwipeSegmentList[] segments, int fingersDown);

	public delegate void SwipeCancelCallBackInternal();

	public delegate void SwipeMoveCallBackInternal(Finger finger, SwipeGesture.SwipeDirection direction, int fingersDown);

	public delegate void RotateBeginCallBackInternal(Finger finger1, Finger finger2);

	public delegate void RotateMoveCallBackInternal(Finger finger1, Finger finger2, float rotationAngleDelta);

	public delegate void RotateEndCallBackInternal(Finger finger1, Finger finger2);

	public delegate void SwipeSegmentCreate(SwipeSegment segment);

	protected const int previousPositionsEnd = 15;

	protected const int previousPositionsTurn = 4;

	private static FingerControl _factory;

	private static bool possibleSwipe = false;

	private static int trackPinchCallbacks = 0;

	private static int trackRotateCallbacks = 0;

	private static int trackSwipeCallbacks = 0;

	public static bool trackPinch = false;

	public static bool trackRotate = false;

	public static bool trackSwipe = false;

	public static bool track2FingerGestures = false;

	public static float moveThresholdSquared = 25f;

	public static float swipeChangeMaxDistance = 50f;

	public float swipeChangeSecondaryMaxDistance = 100f;

	public float pinchDeltaScale = 1f;

	public float maxTapTime = 0.8f;

	public float minSwipeDistance = 2f;

	public float swipeDirectionTolerance = 0.2f;

	public float initializeLongPressTime = 1.5f;

	public float vertHorizHalfDegreeThreshold = 15f;

	private int activeFingers;

	protected static List<BaseGesture> gestures = new List<BaseGesture>();

	public Finger[] fingers;

	protected Vector2[,] previousPositions;

	protected bool previousPositionTurn;

	protected static float swpDirRightMin;

	protected static float swpDirRightMax;

	protected static float swpDirUpMin;

	protected static float swpDirUpMax;

	protected static float swpDirLeftMin;

	protected static float swpDirLeftMax;

	protected static float swpDirDownMin;

	protected static float swpDirDownMax;

	protected static SwipeGesture.SwipeDirection[,] friendlySwipeDirections;

	public float pinchDotProductVariance = -0.7f;

	public float pinchMinimumDistance = 0.5f;

	public static float pinchRotateMinimumMoveDifference = 0.02f;

	public static float rotationDotProductVariance = -0.7f;

	public static float minRotateAmount = 0.5f;

	private Finger[] twoFinger = new Finger[2];

	private bool pinching;

	private bool rotating;

	private int swipeFingerCount;

	private int swipeFingersCount;

	private SwipeSegmentList[] segmentLists;

	private static Vector2 toVector2 = new Vector2(1f, 0f);

	public static event FingerIsDownCallBackInternal _delegateIsDownInternal;

	public static event FingerDownCallBackInternal _delegateFingerDownInternal;

	public static event FingerMovingCallBackInternal _delegateFingerMovingInternal;

	public static event FingerUpCallBackInternal _delegateFingerUpInternal;

	public static event FingerStationaryBeginCallBackInternal _delegateFingerStationaryBeginInternal;

	public static event FingerStationaryCallBackInternal _delegateFingerStationaryInternal;

	public static event FingerStationaryEndCallBackInternal _delegateFingerStationaryEndInternal;

	public static event FingerMoveBeginCallBackInternal _delegateFingerMoveBeginInternal;

	public static event FingerMoveCallBackInternal _delegateFingerMoveInternal;

	public static event FingerMoveEndCallBackInternal _delegateFingerMoveEndInternal;

	public static event DownAndMovingBeginCallBackInternal _delegateDownAndMovingBeginInternal;

	public static event DownAndMovingMoveCallBackInternal _delegateDownAndMovingMoveInternal;

	public static event DownAndMovingEndCallBackInternal _delegateDownAndMovingEndInternal;

	public static event PinchBeginCallBackInternal _delegatePinchBeginInternal;

	public static event PinchMoveCallBackInternal _delegatePinchMoveInternal;

	public static event PinchEndCallBackInternal _delegatePinchEndInternal;

	public static event SwipeCallBackInternal _delegateSwipeInternal;

	public static event SwipeCancelCallBackInternal _delegateSwipeCancelInternal;

	public static event SwipeMoveCallBackInternal _delegateSwipeMoveInternal;

	public static event RotateBeginCallBackInternal _delegateRotateBeginInternal;

	public static event RotateMoveCallBackInternal _delegateRotateMoveInternal;

	public static event RotateEndCallBackInternal _delegateRotateEndInternal;

	public static event SwipeSegmentCreate _delegateSwipeSegmentCreate;

	public static FingerControl Factory()
	{
		return _factory;
	}

	public abstract int FingerCount();

	public static void AddPinchCallback()
	{
		trackPinchCallbacks++;
		trackPinch = true;
		track2FingerGestures = true;
	}

	public static void RemovePinchCallback()
	{
		trackPinchCallbacks--;
		if (trackPinchCallbacks <= 0)
		{
			trackPinch = false;
			trackPinchCallbacks = 0;
			if (!trackRotate)
			{
				track2FingerGestures = false;
			}
		}
	}

	public static void AddSwipeCallback()
	{
		trackSwipeCallbacks++;
		trackSwipe = true;
	}

	public static void RemoveSwipeCallback()
	{
		trackSwipeCallbacks--;
		if (trackSwipeCallbacks <= 0)
		{
			trackSwipe = false;
			trackSwipeCallbacks = 0;
		}
	}

	public static void AddRotateCallback()
	{
		trackRotateCallbacks++;
		trackRotate = true;
		track2FingerGestures = true;
	}

	public static void RemoveRotateCallback()
	{
		trackRotateCallbacks--;
		if (trackRotateCallbacks <= 0)
		{
			trackRotate = false;
			trackRotateCallbacks = 0;
			if (!trackPinch)
			{
				track2FingerGestures = false;
			}
		}
	}

	public static void AddGesture(BaseGesture gesture)
	{
		gestures.Add(gesture);
		gesture.StartGesture();
	}

	public static void RemoveGesture(BaseGesture gesture)
	{
		gestures.Remove(gesture);
		gesture.EndGesture();
	}

	public static void RemoveAllGestures()
	{
		for (int i = 0; i < gestures.Count; i++)
		{
			gestures[i].EndGesture();
		}
		gestures.Clear();
	}

	public static bool IsSwiping()
	{
		return possibleSwipe;
	}

	protected abstract Finger NewFinger(int index);

	public abstract bool IsFingers();

	protected virtual void Awake()
	{
		_factory = this;
		fingers = new Finger[FingerCount()];
		for (int i = 0; i < fingers.Length; i++)
		{
			fingers[i] = NewFinger(i);
		}
		friendlySwipeDirections = new SwipeGesture.SwipeDirection[Enum.GetValues(typeof(SwipeGesture.SwipeDirection)).Length, 2];
		friendlySwipeDirections[0, 0] = SwipeGesture.SwipeDirection.Any;
		friendlySwipeDirections[0, 1] = SwipeGesture.SwipeDirection.Any;
		friendlySwipeDirections[15, 0] = SwipeGesture.SwipeDirection.None;
		friendlySwipeDirections[15, 1] = SwipeGesture.SwipeDirection.None;
		friendlySwipeDirections[7, 0] = SwipeGesture.SwipeDirection.Minus45;
		friendlySwipeDirections[7, 1] = SwipeGesture.SwipeDirection.Plus45;
		friendlySwipeDirections[8, 0] = SwipeGesture.SwipeDirection.Up;
		friendlySwipeDirections[8, 1] = SwipeGesture.SwipeDirection.Right;
		friendlySwipeDirections[9, 0] = SwipeGesture.SwipeDirection.Plus45;
		friendlySwipeDirections[9, 1] = SwipeGesture.SwipeDirection.Plus135;
		friendlySwipeDirections[10, 0] = SwipeGesture.SwipeDirection.Right;
		friendlySwipeDirections[10, 1] = SwipeGesture.SwipeDirection.Down;
		friendlySwipeDirections[11, 0] = SwipeGesture.SwipeDirection.Plus135;
		friendlySwipeDirections[11, 1] = SwipeGesture.SwipeDirection.Minus135;
		friendlySwipeDirections[12, 0] = SwipeGesture.SwipeDirection.Down;
		friendlySwipeDirections[12, 1] = SwipeGesture.SwipeDirection.Left;
		friendlySwipeDirections[13, 0] = SwipeGesture.SwipeDirection.Minus135;
		friendlySwipeDirections[13, 1] = SwipeGesture.SwipeDirection.Minus45;
		friendlySwipeDirections[14, 0] = SwipeGesture.SwipeDirection.Left;
		friendlySwipeDirections[14, 1] = SwipeGesture.SwipeDirection.Up;
	}

	protected virtual void Start()
	{
		swpDirRightMin = vertHorizHalfDegreeThreshold;
		swpDirRightMax = 360f - vertHorizHalfDegreeThreshold;
		swpDirUpMin = 90f - vertHorizHalfDegreeThreshold;
		swpDirUpMax = 90f + vertHorizHalfDegreeThreshold;
		swpDirLeftMin = 180f - vertHorizHalfDegreeThreshold;
		swpDirLeftMax = 180f + vertHorizHalfDegreeThreshold;
		swpDirDownMin = 270f - vertHorizHalfDegreeThreshold;
		swpDirDownMax = 270f + vertHorizHalfDegreeThreshold;
		previousPositions = new Vector2[FingerCount(), 16];
	}

	protected virtual void Update()
	{
		if (fingers == null)
		{
			Debug.LogError("FingerControl error, data has disappeared - need to restart");
			return;
		}
		activeFingers = 0;
		for (int i = 0; i < fingers.Length; i++)
		{
			Finger finger = fingers[i];
			if (finger.motionState != Finger.FingerMotionState.Inactive)
			{
				activeFingers++;
			}
		}
		int num = 0;
		for (int j = 0; j < fingers.Length; j++)
		{
			Finger finger2 = fingers[j];
			UpdateFinger(finger2);
			if (finger2.motionState != Finger.FingerMotionState.Inactive)
			{
				num++;
			}
		}
		activeFingers = num;
		if (track2FingerGestures && activeFingers < 3)
		{
			UpdateTwoFingerGestures(activeFingers);
		}
		else if (activeFingers == 0)
		{
			AllFingersInactive();
		}
	}

	protected virtual void UpdateFinger(Finger finger)
	{
		if ((!finger.SetState() || finger.touchPhase == TouchPhase.Canceled) && !finger.isDown)
		{
			return;
		}
		if (finger.touchPhase == TouchPhase.Ended || finger.touchPhase == TouchPhase.Canceled)
		{
			ProcessSwipeEnd(finger);
			if (finger.hasMoved || finger.downAndMoving)
			{
				SendEventDownAndMovingEnd(finger);
			}
			else if (finger.motionState == Finger.FingerMotionState.Stationary)
			{
				StopStationary(finger, finger.position, true);
			}
			else if (finger.motionState == Finger.FingerMotionState.Moving)
			{
				StopMoving(finger, finger.position);
			}
			float timeHeldDown = Time.time - finger.startTime;
			finger.Up();
			SendIsDown(finger, false);
			SendEventFingerUp(finger, timeHeldDown);
		}
		else if (finger.touchPhase == TouchPhase.Began || (!finger.isDown && (finger.touchPhase == TouchPhase.Moved || finger.touchPhase == TouchPhase.Stationary)))
		{
			ProcessSwipeBegin(finger);
			finger.Down(finger.position);
			finger.motionState = Finger.FingerMotionState.Stationary;
			finger.onlyStationary = true;
			finger.touchPhase = TouchPhase.Stationary;
			SendIsDown(finger, true);
			SendEventFingerDown(finger);
		}
		else if (finger.touchPhase == TouchPhase.Moved)
		{
			SendEventFingerMoving(finger);
			if (finger.motionState != Finger.FingerMotionState.Moving)
			{
				bool flag = false;
				Vector2 fingerPos = finger.position;
				if (finger.hasMoved)
				{
					flag = true;
				}
				else if ((finger.position - finger.startPosition).sqrMagnitude > moveThresholdSquared)
				{
					flag = true;
					fingerPos = finger.startPosition;
				}
				if (flag)
				{
					if (finger.motionState == Finger.FingerMotionState.Stationary)
					{
						StopStationary(finger, finger.position, false);
					}
					finger.motionState = Finger.FingerMotionState.Moving;
					SendEventFingerMoveBegin(finger, fingerPos);
					if (!finger.hasMoved)
					{
						finger.downAndMoving = true;
						finger.downAndMovingStartTime = Time.time;
						SendEventDownAndMovingBegin(finger);
						finger.hasMoved = true;
					}
				}
				else
				{
					FingerIsStationary(finger, finger.position);
				}
			}
			if (!finger.hasMoved)
			{
				return;
			}
			for (int i = 1; i < 16; i++)
			{
				previousPositions[SegmentListFingerIndex(finger), i] = previousPositions[SegmentListFingerIndex(finger), i - 1];
			}
			previousPositions[SegmentListFingerIndex(finger), 0] = finger.position;
			if (finger.deltaPosition.sqrMagnitude > 0f)
			{
				ProcessSwipeMove(finger, finger.deltaPosition);
				if (finger.motionState == Finger.FingerMotionState.Moving)
				{
					SendEventFingerMove(finger, finger.position);
				}
				if (finger.downAndMoving)
				{
					SendEventDownAndMoving(finger, finger.position, finger.deltaPosition);
				}
			}
		}
		else if (finger.touchPhase == TouchPhase.Stationary)
		{
			FingerIsStationary(finger, finger.position);
		}
	}

	private void UpdateTwoFingerGestures(int activeFingerCount)
	{
		if (activeFingerCount == 2)
		{
			if (!rotating || !pinching)
			{
				twoFinger[0] = fingers[0];
				twoFinger[1] = fingers[1];
			}
			Finger finger = twoFinger[0];
			Finger finger2 = twoFinger[1];
			if (!finger.hasMoved || !finger2.hasMoved || finger.touchPhase != TouchPhase.Moved || finger2.touchPhase != TouchPhase.Moved)
			{
				return;
			}
			Vector2 vector = finger.previousPosition - finger2.previousPosition;
			Vector2 vector2 = finger.position - finger2.position;
			float num = vector2.magnitude - vector.magnitude;
			if (Mathf.Abs(num) < pinchRotateMinimumMoveDifference)
			{
				return;
			}
			float num2 = Vector2.Dot(finger.deltaPosition.normalized, finger2.deltaPosition.normalized);
			if (trackRotate && num2 < rotationDotProductVariance)
			{
				Vector2 normalized = vector2.normalized;
				if (!rotating)
				{
					Vector2 normalized2 = (finger.startPosition - finger2.startPosition).normalized;
					float num3 = 57.29578f * SignedAngle(normalized2, normalized);
					if (Mathf.Abs(num3) >= minRotateAmount)
					{
						rotating = true;
						SendEventRotateBegin(finger, finger2);
						SendEventRotateMove(finger, finger2, (Mathf.Sign(num3) - num3) * minRotateAmount);
					}
				}
				else
				{
					Vector2 normalized3 = vector.normalized;
					float rotationAngleDelta = 57.29578f * SignedAngle(normalized3, normalized);
					SendEventRotateMove(finger, finger2, rotationAngleDelta);
				}
			}
			if (trackPinch && num2 < pinchDotProductVariance)
			{
				PinchGesture.PinchDirection pinchDirection = PinchGesture.CalculatePinchDirection(finger.position, finger2.position);
				if (!pinching)
				{
					pinching = true;
					SendEventPinchBegin(finger, finger2, pinchDirection, num);
				}
				else
				{
					SendEventPinchMove(finger, finger2, pinchDirection, num);
				}
			}
		}
		else
		{
			if (pinching)
			{
				SendEventPinchEnd(twoFinger[0], twoFinger[1]);
				pinching = false;
			}
			if (rotating)
			{
				SendEventRotateEnd(twoFinger[0], twoFinger[1]);
				rotating = false;
			}
			twoFinger[0] = null;
			twoFinger[1] = null;
		}
	}

	private void StopMoving(Finger finger, Vector2 fingerPos)
	{
		finger.motionState = Finger.FingerMotionState.Inactive;
		SendEventFingerMoveEnd(finger, fingerPos);
	}

	private void StopStationary(Finger finger, Vector2 fingerPos, bool ended)
	{
		finger.motionState = Finger.FingerMotionState.Inactive;
		if (!ended)
		{
			finger.onlyStationary = false;
		}
		SendEventFingerStationaryEnd(finger, fingerPos, Time.time - finger.timeSpentStationary);
	}

	protected void FingerIsStationary(Finger finger, Vector2 fingerPos)
	{
		if (finger.motionState != Finger.FingerMotionState.Stationary)
		{
			if (possibleSwipe || finger.hasMoved)
			{
				return;
			}
			if (finger.motionState == Finger.FingerMotionState.Moving)
			{
				StopMoving(finger, fingerPos);
			}
			finger.motionState = Finger.FingerMotionState.Stationary;
			finger.timeSpentStationary = Time.time;
			finger.stationaryPosition = fingerPos;
			SendEventFingerStationaryBegin(finger, fingerPos);
		}
		float elapsedTime = Time.time - finger.timeSpentStationary;
		SendEventFingerStationary(finger, elapsedTime);
	}

	private void AllFingersInactive()
	{
		possibleSwipe = false;
		swipeFingerCount = 0;
		swipeFingersCount = 0;
		segmentLists = null;
	}

	private void ProcessSwipeBegin(Finger finger)
	{
		if (!trackSwipe)
		{
			return;
		}
		if (swipeFingerCount == 0 || segmentLists == null)
		{
			possibleSwipe = true;
			swipeFingersCount = 0;
			segmentLists = new SwipeSegmentList[FingerCount()];
			segmentLists[SegmentListFingerIndex(finger)] = new SwipeSegmentList();
			segmentLists[SegmentListFingerIndex(finger)].Add(new SwipeSegment(null));
			clearPreviousPositions(-1);
		}
		else
		{
			if (segmentLists[SegmentListFingerIndex(finger)] == null)
			{
				segmentLists[SegmentListFingerIndex(finger)] = new SwipeSegmentList();
			}
			segmentLists[SegmentListFingerIndex(finger)].Add(new SwipeSegment(null));
		}
		SwipeSegment swipeSegment = segmentLists[SegmentListFingerIndex(finger)][0];
		swipeSegment.startPosition = finger.startPosition;
		if (possibleSwipe)
		{
			swipeFingerCount++;
			swipeFingersCount++;
		}
	}

	private int SegmentListFingerIndex(Finger finger)
	{
		if (FingerCount() > 1)
		{
			return finger.Index();
		}
		return 0;
	}

	private void clearPreviousPositions(int index)
	{
		if (index < 0)
		{
			for (int i = 0; i < FingerCount(); i++)
			{
				for (int j = 0; j < 16; j++)
				{
					previousPositions[i, j] = Vector2.zero;
				}
			}
		}
		else
		{
			for (int k = 0; k < 16; k++)
			{
				previousPositions[index, k] = Vector2.zero;
			}
		}
		previousPositionTurn = false;
	}

	private void CancelPossibleSwipe()
	{
		segmentLists = null;
		possibleSwipe = false;
		swipeFingerCount = 0;
		SendEventSwipeCancel();
	}

	private SwipeSegment getLastSegment(Finger finger)
	{
		if (segmentLists == null)
		{
			return null;
		}
		SwipeSegmentList swipeSegmentList = segmentLists[SegmentListFingerIndex(finger)];
		if (swipeSegmentList == null || swipeSegmentList.Count == 0)
		{
			ProcessSwipeBegin(finger);
			return getLastSegment(finger);
		}
		return swipeSegmentList[swipeSegmentList.Count - 1];
	}

	private void ProcessSwipeMove(Finger finger, Vector2 moveDelta)
	{
		if (!possibleSwipe)
		{
			CancelPossibleSwipe();
			return;
		}
		SwipeSegment lastSegment = getLastSegment(finger);
		SwipeGesture.SwipeDirection swipeDirection = GetSwipeDirection(lastSegment.startPosition, finger.position);
		if (previousPositions[SegmentListFingerIndex(finger), 15] != Vector2.zero)
		{
			SwipeGesture.SwipeDirection swipeDirection2 = GetSwipeDirection(previousPositions[SegmentListFingerIndex(finger), 4], previousPositions[SegmentListFingerIndex(finger), 0]);
			if (swipeDirection2 != lastSegment.direction)
			{
				swipeDirection = swipeDirection2;
				previousPositionTurn = true;
			}
		}
		if (lastSegment.direction != swipeDirection || previousPositionTurn)
		{
			if (lastSegment.direction == SwipeGesture.SwipeDirection.None)
			{
				lastSegment.Initalize(finger, finger.startPosition, swipeDirection);
			}
			else
			{
				float magnitude = ((Vector3)(finger.position - lastSegment.startPosition)).magnitude;
				if (magnitude < swipeChangeMaxDistance || (magnitude < swipeChangeSecondaryMaxDistance && FriendlySwipeDirections(lastSegment.direction, swipeDirection)))
				{
					lastSegment.direction = swipeDirection;
				}
				else if (lastSegment.direction != swipeDirection)
				{
					SwipeChangeDirection(finger, swipeDirection);
				}
				previousPositionTurn = false;
			}
		}
		finger.swipeDirection = lastSegment.direction;
		if (lastSegment.direction != SwipeGesture.SwipeDirection.None || lastSegment.startPosition == Vector2.zero)
		{
			SendEventSwipeMove(finger, finger.swipeDirection, swipeFingersCount);
		}
	}

	private void SwipeChangeDirection(Finger finger, SwipeGesture.SwipeDirection direction)
	{
		Vector2 vector = finger.position;
		SwipeSegment lastSegment = getLastSegment(finger);
		if (previousPositionTurn)
		{
			vector = previousPositions[SegmentListFingerIndex(finger), 4];
		}
		lastSegment.endPosition = vector;
		SwipeGesture.SwipeDirection direction2;
		float swipeDistance;
		float velocity;
		if (IsASwipe(lastSegment.startPosition, vector, finger, out direction2, out swipeDistance, out velocity))
		{
			lastSegment.Set(direction2, swipeDistance, velocity);
			SendEventSwipeSegmentCreate(lastSegment);
			lastSegment = new SwipeSegment(lastSegment);
			lastSegment.Initalize(finger, vector, direction);
			segmentLists[SegmentListFingerIndex(finger)].Add(lastSegment);
			clearPreviousPositions(SegmentListFingerIndex(finger));
		}
	}

	private void ProcessSwipeEnd(Finger finger)
	{
		if (segmentLists == null)
		{
			return;
		}
		bool flag = false;
		finger.endPosition = finger.position;
		SwipeSegment lastSegment = getLastSegment(finger);
		if (lastSegment == null)
		{
			return;
		}
		lastSegment.endPosition = finger.position;
		VerifySwipeSegments(finger);
		if (!possibleSwipe || (!finger.hasMoved && !finger.downAndMoving))
		{
			CancelPossibleSwipe();
			return;
		}
		SwipeGesture.SwipeDirection direction;
		float swipeDistance;
		float velocity;
		if (IsASwipe(lastSegment.startPosition, lastSegment.endPosition, finger, out direction, out swipeDistance, out velocity))
		{
			swipeFingerCount--;
			if (swipeFingerCount == 0)
			{
				lastSegment.Set(direction, swipeDistance, velocity);
				SendEventSwipeSegmentCreate(lastSegment);
				SendEventSwipe(fingers, segmentLists, swipeFingersCount);
				flag = true;
			}
		}
		else
		{
			swipeFingerCount--;
			segmentLists[SegmentListFingerIndex(finger)].RemoveAt(segmentLists[SegmentListFingerIndex(finger)].Count - 1);
			if (swipeFingerCount == 0 && segmentLists[SegmentListFingerIndex(finger)].Count > 0)
			{
				SwipeSegment swipeSegment = segmentLists[SegmentListFingerIndex(finger)][segmentLists[SegmentListFingerIndex(finger)].Count - 1];
				finger.endPosition = swipeSegment.endPosition;
				SendEventSwipe(fingers, segmentLists, swipeFingersCount);
				flag = true;
			}
		}
		if (swipeFingerCount == 0)
		{
			CancelPossibleSwipe();
		}
		else if (!flag)
		{
			SendEventSwipeCancel();
		}
	}

	private void VerifySwipeSegments(Finger finger)
	{
		if (segmentLists[SegmentListFingerIndex(finger)].Count >= 2)
		{
			SwipeSegmentList swipeSegmentList = segmentLists[SegmentListFingerIndex(finger)];
			SwipeSegment swipeSegment = swipeSegmentList[swipeSegmentList.Count - 1];
			SwipeSegment swipeSegment2 = swipeSegmentList[swipeSegmentList.Count - 2];
			if (swipeSegment.direction == swipeSegment2.direction)
			{
				swipeSegment2.Merge(swipeSegment);
				swipeSegmentList.RemoveAt(swipeSegmentList.Count - 1);
			}
		}
	}

	public SwipeSegmentList TryToMakeOneSegment(SwipeSegmentList segments)
	{
		if (segments.Count < 2)
		{
			return segments;
		}
		SwipeGesture.SwipeDirection swipeDirection = GetSwipeDirection(segments[0].startPosition, segments[segments.Count - 1].startPosition);
		for (int i = 0; i < segments.Count; i++)
		{
			if (!FriendlySwipeDirections(swipeDirection, segments[i].direction) && segments[i].distance > swipeChangeSecondaryMaxDistance)
			{
				return segments;
			}
		}
		SwipeSegment swipeSegment = segments[0];
		swipeSegment.CombineToSingleAfter(true, swipeDirection, segments[segments.Count - 1]);
		segments = new SwipeSegmentList();
		segments.Add(swipeSegment);
		return segments;
	}

	public static SwipeGesture.SwipeDirection GetSwipeDirection(Vector2 fromPosition, Vector2 toPosition)
	{
		Vector2 vector = toPosition - fromPosition;
		float num = Vector2.Angle(vector, toVector2);
		if (Vector3.Cross(vector, toVector2).z > 0f)
		{
			num = 360f - num;
		}
		if (num >= 0f && num < swpDirRightMin)
		{
			return SwipeGesture.SwipeDirection.Right;
		}
		if (num < swpDirUpMin)
		{
			return SwipeGesture.SwipeDirection.Plus45;
		}
		if (num < swpDirUpMax)
		{
			return SwipeGesture.SwipeDirection.Up;
		}
		if (num < swpDirLeftMin)
		{
			return SwipeGesture.SwipeDirection.Minus45;
		}
		if (num < swpDirLeftMax)
		{
			return SwipeGesture.SwipeDirection.Left;
		}
		if (num < swpDirDownMin)
		{
			return SwipeGesture.SwipeDirection.Minus135;
		}
		if (num < swpDirDownMax)
		{
			return SwipeGesture.SwipeDirection.Down;
		}
		if (num < swpDirRightMax)
		{
			return SwipeGesture.SwipeDirection.Plus135;
		}
		return SwipeGesture.SwipeDirection.Right;
	}

	public static bool FriendlySwipeDirections(SwipeGesture.SwipeDirection direction1, SwipeGesture.SwipeDirection direction2)
	{
		if (direction1 == direction2)
		{
			return true;
		}
		if (direction1 == SwipeGesture.SwipeDirection.None || direction2 == SwipeGesture.SwipeDirection.None)
		{
			return false;
		}
		if (direction1 == SwipeGesture.SwipeDirection.Any || direction2 == SwipeGesture.SwipeDirection.Any)
		{
			return true;
		}
		if (friendlySwipeDirections[(int)direction1, 0] == direction2 || friendlySwipeDirections[(int)direction1, 1] == direction2)
		{
			return true;
		}
		return false;
	}

	public static SwipeGesture.SwipeDirection[] GetFriendlyDirections(SwipeGesture.SwipeDirection direction)
	{
		return new SwipeGesture.SwipeDirection[2]
		{
			friendlySwipeDirections[(int)direction, 0],
			friendlySwipeDirections[(int)direction, 1]
		};
	}

	public static bool IsOppositeSwipeDirection(SwipeGesture.SwipeDirection direction)
	{
		switch (direction)
		{
		case SwipeGesture.SwipeDirection.Any:
			return true;
		case SwipeGesture.SwipeDirection.None:
			return false;
		case SwipeGesture.SwipeDirection.Up:
			if (direction == SwipeGesture.SwipeDirection.Down)
			{
				return true;
			}
			break;
		case SwipeGesture.SwipeDirection.Plus45:
			if (direction == SwipeGesture.SwipeDirection.Minus135)
			{
				return true;
			}
			break;
		case SwipeGesture.SwipeDirection.Right:
			if (direction == SwipeGesture.SwipeDirection.Left)
			{
				return true;
			}
			break;
		case SwipeGesture.SwipeDirection.Plus135:
			if (direction == SwipeGesture.SwipeDirection.Minus45)
			{
				return true;
			}
			break;
		case SwipeGesture.SwipeDirection.Down:
			if (direction == SwipeGesture.SwipeDirection.Up)
			{
				return true;
			}
			break;
		case SwipeGesture.SwipeDirection.Minus135:
			if (direction == SwipeGesture.SwipeDirection.Plus45)
			{
				return true;
			}
			break;
		case SwipeGesture.SwipeDirection.Left:
			if (direction == SwipeGesture.SwipeDirection.Right)
			{
				return true;
			}
			break;
		case SwipeGesture.SwipeDirection.Minus45:
			if (direction == SwipeGesture.SwipeDirection.Plus135)
			{
				return true;
			}
			break;
		}
		return false;
	}

	private bool IsASwipe(Vector2 segmentStart, Vector2 segmentEnd, Finger finger, out SwipeGesture.SwipeDirection direction, out float swipeDistance, out float velocity)
	{
		direction = GetSwipeDirection(segmentStart, segmentEnd);
		velocity = 0f;
		swipeDistance = 0f;
		if (!finger.possibleSwipe)
		{
			return false;
		}
		float magnitude = ((Vector3)(segmentEnd - segmentStart)).magnitude;
		if (magnitude < minSwipeDistance)
		{
			return false;
		}
		float num = Time.time - finger.downAndMovingStartTime;
		velocity = magnitude / num;
		swipeDistance = magnitude;
		return true;
	}

	public static float SignedAngle(Vector2 from, Vector2 to)
	{
		float y = from.x * to.y - from.y * to.x;
		return Mathf.Atan2(y, Vector2.Dot(from, to));
	}

	protected void SendEventFingerDown(Finger finger)
	{
		if (FingerControl._delegateFingerDownInternal != null)
		{
			FingerControl._delegateFingerDownInternal(finger);
		}
	}

	protected void SendEventFingerMoving(Finger finger)
	{
		if (FingerControl._delegateFingerMovingInternal != null)
		{
			FingerControl._delegateFingerMovingInternal(finger);
		}
	}

	protected void SendEventFingerUp(Finger finger)
	{
		if (FingerControl._delegateFingerUpInternal != null)
		{
			FingerControl._delegateFingerUpInternal(finger);
		}
	}

	protected void SendIsDown(Finger finger, bool isDown)
	{
		if (FingerControl._delegateIsDownInternal != null)
		{
			FingerControl._delegateIsDownInternal(finger, isDown);
		}
	}

	protected void SendEventFingerStationaryBegin(Finger finger, Vector2 fingerPos)
	{
		if (FingerControl._delegateFingerStationaryBeginInternal != null)
		{
			FingerControl._delegateFingerStationaryBeginInternal(finger, fingerPos);
		}
	}

	protected void SendEventFingerStationary(Finger finger, float elapsedTime)
	{
		if (FingerControl._delegateFingerStationaryInternal != null)
		{
			FingerControl._delegateFingerStationaryInternal(finger, elapsedTime);
		}
	}

	protected void SendEventFingerStationaryEnd(Finger finger, Vector2 fingerPos, float elapsedTime)
	{
		if (FingerControl._delegateFingerStationaryEndInternal != null)
		{
			FingerControl._delegateFingerStationaryEndInternal(finger, fingerPos, elapsedTime);
		}
	}

	protected void SendEventFingerMoveBegin(Finger finger, Vector2 fingerPos)
	{
		if (FingerControl._delegateFingerMoveBeginInternal != null)
		{
			FingerControl._delegateFingerMoveBeginInternal(finger, fingerPos);
		}
	}

	protected void SendEventFingerMove(Finger finger, Vector2 fingerPos)
	{
		if (FingerControl._delegateFingerMoveInternal != null)
		{
			FingerControl._delegateFingerMoveInternal(finger, fingerPos);
		}
	}

	protected void SendEventFingerMoveEnd(Finger finger, Vector2 fingerPos)
	{
		if (FingerControl._delegateFingerMoveEndInternal != null)
		{
			FingerControl._delegateFingerMoveEndInternal(finger, fingerPos);
		}
	}

	protected void SendEventFingerUp(Finger finger, float timeHeldDown)
	{
		if (FingerControl._delegateFingerUpInternal != null)
		{
			FingerControl._delegateFingerUpInternal(finger);
		}
	}

	protected void SendEventDownAndMovingBegin(Finger finger)
	{
		if (FingerControl._delegateDownAndMovingBeginInternal != null)
		{
			FingerControl._delegateDownAndMovingBeginInternal(finger);
		}
	}

	protected void SendEventDownAndMoving(Finger finger, Vector2 fingerPos, Vector2 delta)
	{
		if (FingerControl._delegateDownAndMovingMoveInternal != null)
		{
			FingerControl._delegateDownAndMovingMoveInternal(finger, fingerPos, delta);
		}
	}

	protected void SendEventDownAndMovingEnd(Finger finger)
	{
		if (FingerControl._delegateDownAndMovingEndInternal != null)
		{
			FingerControl._delegateDownAndMovingEndInternal(finger);
		}
	}

	protected void SendEventPinchBegin(Finger finger1, Finger finger2, PinchGesture.PinchDirection pinchDirection, float magnitudeDelta)
	{
		if (FingerControl._delegatePinchBeginInternal != null)
		{
			FingerControl._delegatePinchBeginInternal(finger1, finger2, pinchDirection, magnitudeDelta);
		}
	}

	protected void SendEventPinchMove(Finger finger1, Finger finger2, PinchGesture.PinchDirection pinchDirection, float magnitudeDelta)
	{
		if (FingerControl._delegatePinchMoveInternal != null)
		{
			FingerControl._delegatePinchMoveInternal(finger1, finger2, pinchDirection, magnitudeDelta);
		}
	}

	protected void SendEventPinchEnd(Finger finger1, Finger finger2)
	{
		if (FingerControl._delegatePinchEndInternal != null)
		{
			FingerControl._delegatePinchEndInternal(finger1, finger2);
		}
	}

	protected void SendEventSwipe(Finger[] fingers, SwipeSegmentList[] segments, int fingersDown)
	{
		if (FingerControl._delegateSwipeInternal != null)
		{
			FingerControl._delegateSwipeInternal(fingers, segments, fingersDown);
		}
	}

	protected void SendEventSwipeMove(Finger finger, SwipeGesture.SwipeDirection direction, int fingersDown)
	{
		if (FingerControl._delegateSwipeMoveInternal != null)
		{
			FingerControl._delegateSwipeMoveInternal(finger, direction, fingersDown);
		}
	}

	protected void SendEventSwipeCancel()
	{
		if (FingerControl._delegateSwipeCancelInternal != null)
		{
			FingerControl._delegateSwipeCancelInternal();
		}
	}

	protected void SendEventSwipeSegmentCreate(SwipeSegment segment)
	{
		if (FingerControl._delegateSwipeSegmentCreate != null)
		{
			FingerControl._delegateSwipeSegmentCreate(segment);
		}
	}

	protected void SendEventRotateBegin(Finger finger1, Finger finger2)
	{
		if (FingerControl._delegateRotateBeginInternal != null)
		{
			FingerControl._delegateRotateBeginInternal(finger1, finger2);
		}
	}

	protected void SendEventRotateMove(Finger finger1, Finger finger2, float rotationAngleDelta)
	{
		if (FingerControl._delegateRotateMoveInternal != null)
		{
			FingerControl._delegateRotateMoveInternal(finger1, finger2, rotationAngleDelta);
		}
	}

	protected void SendEventRotateEnd(Finger finger1, Finger finger2)
	{
		if (FingerControl._delegateRotateEndInternal != null)
		{
			FingerControl._delegateRotateEndInternal(finger1, finger2);
		}
	}
}
