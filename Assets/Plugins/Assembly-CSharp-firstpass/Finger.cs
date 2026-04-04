using UnityEngine;

public class Finger
{
	public enum FingerMotionState
	{
		Stationary = 0,
		Moving = 1,
		Inactive = 2
	}

	private int _index;

	public bool isDown;

	public bool hasMoved;

	public bool longPressSent;

	public bool downAndMoving;

	public FingerMotionState motionState = FingerMotionState.Inactive;

	public float timeSpentStationary;

	public float startTime;

	public float downAndMovingStartTime;

	public bool possibleSwipe = true;

	public bool onlyStationary;

	public SwipeGesture.SwipeDirection swipeDirection = SwipeGesture.SwipeDirection.None;

	public TouchPhase touchPhase = TouchPhase.Ended;

	public Vector2 position = Vector2.zero;

	public Vector2 deltaPosition = Vector2.zero;

	public TouchPhase previousTouchPhase = TouchPhase.Ended;

	public Vector2 previousPosition = Vector2.zero;

	public Vector2 previousDeltaPosition = Vector2.zero;

	public Vector2 stationaryPosition = Vector2.zero;

	public Vector2 startPosition = Vector2.zero;

	public Vector2 endPosition = Vector2.zero;

	public Vector2 tapStartPosition = Vector2.zero;

	public bool usingMouse;

	public Finger(int indexIn)
	{
		SetIndex(indexIn);
	}

	public static Vector2 ScreenVectorToGui(Vector2 screenVector)
	{
		return new Vector2(screenVector.x, (float)Screen.height - screenVector.y);
	}

	public virtual int Index()
	{
		return _index;
	}

	public virtual void SetIndex(int index)
	{
		_index = index;
	}

	public virtual void Down(Vector2 position)
	{
		isDown = true;
		hasMoved = false;
		downAndMoving = false;
		longPressSent = false;
		swipeDirection = SwipeGesture.SwipeDirection.None;
		motionState = FingerMotionState.Inactive;
		startPosition = position;
		stationaryPosition = startPosition;
		endPosition = startPosition;
		startTime = Time.time;
		timeSpentStationary = Time.time;
		downAndMovingStartTime = -1f;
	}

	public virtual void Up()
	{
		isDown = false;
		downAndMoving = false;
	}

	public virtual bool SetState()
	{
		previousTouchPhase = touchPhase;
		previousPosition = position;
		previousDeltaPosition = deltaPosition;
		return true;
	}
}
