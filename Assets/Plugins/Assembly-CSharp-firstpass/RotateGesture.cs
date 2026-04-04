using UnityEngine;

public class RotateGesture : BaseGesture
{
	public enum RotateAxis
	{
		X = 0,
		Y = 1,
		Z = 2
	}

	public bool doRotate = true;

	public float minSqrDistanceToCenter = 5f;

	public FingerLocation fingerLocation;

	public RotateAxis rotateAxis = RotateAxis.Z;

	public FingerCountRestriction restrictFingerCount;

	public Finger rotateFinger1;

	public Finger rotateFinger2;

	public float rotationAngleDelta;

	private bool rotating2Finger;

	private Quaternion originalRotation;

	public Vector2 touchPosition;

	public bool isDown;

	private Vector3 startDir;

	private Vector3 prevCenter;

	private Vector3 prevPosition;

	private bool rotatingAround;

	protected override void Start()
	{
		base.Start();
		originalRotation = base.transform.rotation;
	}

	protected override void EnableGesture()
	{
		base.EnableGesture();
		FingerControl.AddRotateCallback();
		FingerControl._delegateRotateBeginInternal += RotateGestureDownAndMovingBegin;
		FingerControl._delegateRotateMoveInternal += RotateGestureDownAndMovingMove;
		FingerControl._delegateRotateEndInternal += RotateGestureDownAndMovingEnd;
		FingerControl._delegateFingerDownInternal += GestureDownAndMovingBegin;
		FingerControl._delegateFingerMovingInternal += GestureDownAndMovingMove;
		FingerControl._delegateFingerUpInternal += GestureDownAndMovingEnd;
	}

	protected override void DisableGesture()
	{
		base.DisableGesture();
		FingerControl.RemoveRotateCallback();
		FingerControl._delegateRotateBeginInternal -= RotateGestureDownAndMovingBegin;
		FingerControl._delegateRotateMoveInternal -= RotateGestureDownAndMovingMove;
		FingerControl._delegateRotateEndInternal -= RotateGestureDownAndMovingEnd;
		FingerControl._delegateFingerDownInternal -= GestureDownAndMovingBegin;
		FingerControl._delegateFingerMovingInternal -= GestureDownAndMovingMove;
		FingerControl._delegateFingerUpInternal -= GestureDownAndMovingEnd;
	}

	protected void RotateGestureDownAndMovingBegin(Finger finger1, Finger finger2)
	{
		if (FingerActivated(fingerLocation, finger1.position))
		{
			rotating2Finger = true;
			rotateFinger1 = finger1;
			rotateFinger2 = finger2;
			GestureMessage("GestureStartRotate");
		}
	}

	protected void RotateGestureDownAndMovingMove(Finger finger1, Finger finger2, float rotationAngleDeltaIn)
	{
		if (rotating2Finger)
		{
			fingerCount = 2;
			if (FingerCountGood(fingerCount, restrictFingerCount))
			{
				rotateFinger1 = finger1;
				rotateFinger2 = finger2;
				rotationAngleDelta = rotationAngleDeltaIn;
				Rotate(rotationAngleDelta);
			}
		}
	}

	private void Rotate(float angle)
	{
		if (doRotate && targetCollider != null)
		{
			switch (rotateAxis)
			{
			case RotateAxis.X:
				targetCollider.gameObject.transform.Rotate(angle, 0f, 0f);
				break;
			case RotateAxis.Y:
				targetCollider.gameObject.transform.Rotate(0f, angle, 0f);
				break;
			case RotateAxis.Z:
				targetCollider.gameObject.transform.Rotate(0f, 0f, angle);
				break;
			}
			GestureMessage("GestureMoveRotate");
		}
	}

	protected void RotateGestureDownAndMovingEnd(Finger finger1, Finger finger2)
	{
		if (rotating2Finger)
		{
			rotating2Finger = false;
			rotateFinger1 = finger1;
			rotateFinger2 = finger2;
			GestureMessage("GestureEndRotate");
		}
	}

	protected void GestureDownAndMovingBegin(Finger fingerIn)
	{
		Bounds bounds = GetBounds();
		Vector3 vector = Camera.main.transform.position - bounds.center;
		Vector3 vector2 = ScreenToWorldPosition(CalcPosForAllFingers(fingerIn.startPosition), Mathf.Abs(vector.magnitude));
		startDir = (bounds.center - vector2).normalized;
		fingerCount = ActiveCount();
		isDown = true;
	}

	protected void GestureDownAndMovingMove(Finger fingerIn)
	{
		fingerCount = ActiveCount();
		if (!FingerCountGood(fingerCount, restrictFingerCount) || !FingerActivated(fingerLocation) || (fingerCount == 2 && rotating2Finger) || !(targetCollider != null))
		{
			return;
		}
		Vector2 screenPos = CalcPosForAllFingers(fingerIn.position);
		Bounds bounds = GetBounds();
		if (bounds == emptyBounds)
		{
			return;
		}
		Vector3 vector = ScreenToWorldPosition(screenPos, Mathf.Abs((Camera.main.transform.position - bounds.center).magnitude));
		if ((vector - bounds.center).sqrMagnitude < minSqrDistanceToCenter)
		{
			return;
		}
		Vector3 vector2 = prevPosition - prevCenter;
		Vector3 vector3 = vector - bounds.center;
		float f = vector3.magnitude - vector2.magnitude;
		if (Mathf.Abs(f) < FingerControl.pinchRotateMinimumMoveDifference)
		{
			return;
		}
		Vector3 normalized = vector3.normalized;
		float angle;
		if (!rotatingAround)
		{
			rotatingAround = true;
			angle = 57.29578f * FingerControl.SignedAngle(startDir, normalized);
			if (!rotating2Finger)
			{
				GestureMessage("GestureStartRotate");
			}
		}
		else
		{
			angle = 57.29578f * FingerControl.SignedAngle(vector2.normalized, normalized);
			Rotate(angle);
		}
		rotationAngleDelta = angle;
		rotateFinger1 = fingerIn;
		rotateFinger2 = null;
		prevPosition = vector;
		prevCenter = bounds.center;
	}

	protected void GestureDownAndMovingEnd(Finger fingerIn)
	{
		fingerCount = ActiveCount();
		if (fingerCount == 0)
		{
			isDown = false;
			rotating2Finger = false;
			rotatingAround = false;
			rotateFinger1 = fingerIn;
			rotateFinger2 = null;
			GestureMessage("GestureEndRotate");
		}
	}

	public void RestoreObject()
	{
		base.transform.rotation = originalRotation;
	}
}
