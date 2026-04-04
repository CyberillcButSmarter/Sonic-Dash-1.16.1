using UnityEngine;

public class PinchGesture : BaseGesture
{
	public enum PinchAction
	{
		Both = 0,
		Close = 1,
		Open = 2
	}

	public enum PinchDirection
	{
		All = 0,
		Vertical = 1,
		LeftDiagonal = 2,
		Horizontal = 3,
		RightDiagonal = 4
	}

	public bool doPinch = true;

	public bool keepAspectRatio;

	public FingerLocation fingerLocation;

	public PinchAction pinchAction;

	public PinchDirection restrictDirection;

	public float pinchScaleFactor = 0.01f;

	public PinchDirection pinchDirection;

	public Finger pinchFinger1;

	public Finger pinchFinger2;

	public float pinchMagnitudeDelta;

	private bool pinching;

	private Vector3 originalScale;

	private Vector3 vector31X = new Vector3(1f, 0f, 0f);

	private Vector3 vector31Y = new Vector3(0f, 1f, 0f);

	private Vector3 vector31Z = new Vector3(0f, 0f, 1f);

	protected override void Start()
	{
		base.Start();
		originalScale = base.transform.localScale;
	}

	protected override void EnableGesture()
	{
		base.EnableGesture();
		FingerControl.AddPinchCallback();
		FingerControl._delegatePinchBeginInternal += PinchGestureDownAndMovingBegin;
		FingerControl._delegatePinchMoveInternal += PinchGestureDownAndMovingMove;
		FingerControl._delegatePinchEndInternal += PinchGestureDownAndMovingEnd;
	}

	protected override void DisableGesture()
	{
		base.DisableGesture();
		FingerControl.RemovePinchCallback();
		FingerControl._delegatePinchBeginInternal -= PinchGestureDownAndMovingBegin;
		FingerControl._delegatePinchMoveInternal -= PinchGestureDownAndMovingMove;
		FingerControl._delegatePinchEndInternal -= PinchGestureDownAndMovingEnd;
	}

	protected void PinchGestureDownAndMovingBegin(Finger finger1, Finger finger2, PinchDirection pinchDirectionIn, float magnitudeDelta)
	{
		if (FingerActivated(fingerLocation, finger1.position) && (pinchAction != PinchAction.Close || !(magnitudeDelta > 0f)) && (pinchAction != PinchAction.Open || !(magnitudeDelta < 0f)))
		{
			pinching = true;
			pinchFinger1 = finger1;
			pinchFinger2 = finger2;
			pinchDirection = pinchDirectionIn;
			GestureMessage("GestureStartPinch");
		}
	}

	protected void PinchGestureDownAndMovingMove(Finger finger1, Finger finger2, PinchDirection pinchDirectionIn, float magnitudeDelta)
	{
		fingerCount = 2;
		if (pinching)
		{
			if (restrictDirection != PinchDirection.All && restrictDirection != pinchDirectionIn)
			{
				PinchGestureDownAndMovingEnd(finger1, finger2);
				return;
			}
			pinchFinger1 = finger1;
			pinchFinger2 = finger2;
			pinchMagnitudeDelta = magnitudeDelta;
			pinchDirection = pinchDirectionIn;
			GestureMessage("GestureMovePinch");
			TargetMovePinch();
		}
	}

	protected void PinchGestureDownAndMovingEnd(Finger finger1, Finger finger2)
	{
		if (pinching)
		{
			pinching = false;
			pinchFinger1 = finger1;
			pinchFinger2 = finger2;
			GestureMessage("GestureEndPinch");
		}
	}

	public static PinchDirection CalculatePinchDirection(Vector2 position1, Vector2 position2)
	{
		if (position1.y > position2.y)
		{
			Vector2 vector = position2;
			position2 = position1;
			position1 = vector;
		}
		Vector2 vector2 = position2 - position1;
		if (Mathf.Abs(vector2.y) < 50f)
		{
			return PinchDirection.Horizontal;
		}
		if (Mathf.Abs(vector2.x) < 50f)
		{
			return PinchDirection.Vertical;
		}
		if (position1.x > position2.x)
		{
			return PinchDirection.LeftDiagonal;
		}
		return PinchDirection.RightDiagonal;
	}

	private void TargetMovePinch()
	{
		if (pinching && doPinch)
		{
			if (keepAspectRatio)
			{
				ScaleTargetXY(pinchMagnitudeDelta, pinchMagnitudeDelta);
			}
			else if (pinchDirection == PinchDirection.Vertical)
			{
				ScaleTargetY(pinchMagnitudeDelta);
			}
			else if (pinchDirection == PinchDirection.Horizontal)
			{
				ScaleTargetX(pinchMagnitudeDelta);
			}
			else
			{
				ScaleTargetXY(pinchMagnitudeDelta, pinchMagnitudeDelta);
			}
		}
	}

	public void RestoreObject()
	{
		if (targetCollider != null)
		{
			targetCollider.gameObject.transform.localScale = originalScale;
		}
	}

	private void ScaleTargetXY(float scaleValueX, float scaleValueY)
	{
		float num = scaleValueX * pinchScaleFactor;
		float num2 = scaleValueY * pinchScaleFactor;
		if (num < 0f && targetCollider.gameObject.transform.localScale.x <= num * -1f)
		{
			num = 0f;
		}
		if (num2 < 0f && targetCollider.gameObject.transform.localScale.y <= num2 * -1f)
		{
			num2 = 0f;
		}
		Vector3 vector = vector31X * num + vector31Y * num2;
		targetCollider.gameObject.transform.localScale += vector;
	}

	private void ScaleTargetX(float scaleValue)
	{
		float num = scaleValue * pinchScaleFactor;
		if (!(scaleValue < 0f) || !(targetCollider.gameObject.transform.localScale.x <= num * -1f))
		{
			targetCollider.gameObject.transform.localScale += vector31X * num;
		}
	}

	private void ScaleTargetY(float scaleValue)
	{
		float num = scaleValue * pinchScaleFactor;
		if (!(scaleValue < 0f) || !(targetCollider.gameObject.transform.localScale.y <= num * -1f))
		{
			targetCollider.gameObject.transform.localScale += vector31Y * num;
		}
	}

	private void ScaleTargetZ(float scaleValue)
	{
		float num = scaleValue * pinchScaleFactor;
		if (!(scaleValue < 0f) || !(targetCollider.gameObject.transform.localScale.z <= num * -1f))
		{
			targetCollider.gameObject.transform.localScale += vector31Z * num;
		}
	}
}
