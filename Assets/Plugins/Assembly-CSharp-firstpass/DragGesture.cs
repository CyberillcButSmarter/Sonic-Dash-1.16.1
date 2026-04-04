using UnityEngine;

public class DragGesture : BaseGesture
{
	public enum DragPosition
	{
		Relative = 0,
		Centred = 1,
		NoDrag = 2
	}

	private DragGesture dragGesture;

	public DragPosition dragPosition;

	public FingerLocation fingerLocation;

	public FingerCountRestriction restrictFingerCount;

	public bool doDrag = true;

	public XYRestriction restrictDirection;

	public float restrictScreenMin;

	public float restrictScreenMax = 1f;

	public int dragFingerCount;

	public Vector3 startPoint = Vector3.zero;

	public Vector3 endPoint = Vector3.zero;

	public Vector3 targetPoint;

	private Vector3 worldDelta;

	private Vector3 originalPosition;

	private bool draggingObject;

	protected override void Start()
	{
		base.Start();
		originalPosition = base.transform.position;
	}

	protected override void EnableGesture()
	{
		base.EnableGesture();
		FingerControl._delegateDownAndMovingBeginInternal += DragGestureDownAndMovingBegin;
		FingerControl._delegateDownAndMovingMoveInternal += DragGestureDownAndMovingMove;
		FingerControl._delegateDownAndMovingEndInternal += DragGestureDownAndMovingEnd;
		if (targetCollider == null)
		{
			doDrag = false;
		}
		else
		{
			targetPoint = targetCollider.gameObject.transform.position;
		}
	}

	protected override void DisableGesture()
	{
		base.DisableGesture();
		FingerControl._delegateDownAndMovingBeginInternal -= DragGestureDownAndMovingBegin;
		FingerControl._delegateDownAndMovingMoveInternal -= DragGestureDownAndMovingMove;
		FingerControl._delegateDownAndMovingEndInternal -= DragGestureDownAndMovingEnd;
	}

	protected void DragGestureDownAndMovingBegin(Finger fingerIn)
	{
		if (FingerCountGood(ActiveCount(), restrictFingerCount) && FingerActivated(fingerLocation, fingerIn.position) && targetCollider != null && dragPosition != DragPosition.NoDrag)
		{
			draggingObject = true;
			startPoint = targetCollider.gameObject.transform.localPosition;
			dragFingerCount = 0;
			if (fingerIn.Index() == 0)
			{
				finger = fingerIn;
			}
			GestureMessage("GestureStartDrag");
		}
	}

	protected void DragGestureDownAndMovingMove(Finger fingerIn, Vector2 fingerPos, Vector2 delta)
	{
		int num = ActiveCount();
		if (!draggingObject || finger != fingerIn || !FingerCountGood(num, restrictFingerCount) || dragPosition == DragPosition.NoDrag)
		{
			return;
		}
		if (doDrag)
		{
			if ((restrictDirection == XYRestriction.XDirecton || restrictDirection == XYRestriction.YDirection) && (restrictScreenMin > 0f || restrictScreenMax != 1f))
			{
				float num2 = restrictScreenMin;
				float num3 = restrictScreenMax;
				if (restrictDirection == XYRestriction.XDirecton)
				{
					if (num2 <= 1f)
					{
						num2 = (float)Screen.width * num2;
					}
					if (num3 <= 1f)
					{
						num3 = (float)Screen.width * num3;
					}
					if (fingerPos.x < num2)
					{
						fingerPos.x = num2;
					}
					if (fingerPos.x > num3)
					{
						fingerPos.x = num3;
					}
				}
				else if (restrictDirection == XYRestriction.YDirection)
				{
					if (num2 <= 1f)
					{
						num2 = (float)Screen.height * num2;
					}
					if (num3 <= 1f)
					{
						num3 = (float)Screen.height * num3;
					}
					if (fingerPos.y < num2)
					{
						fingerPos.y = num2;
					}
					if (fingerPos.y > num3)
					{
						fingerPos.y = num3;
					}
				}
			}
			Vector3 vector = CalcWorldPosForAllFingers(fingerPos);
			if (restrictDirection == XYRestriction.XDirecton)
			{
				vector.y = targetCollider.gameObject.transform.position.y;
			}
			else if (restrictDirection == XYRestriction.YDirection)
			{
				vector.x = targetCollider.gameObject.transform.position.x;
			}
			if (dragPosition == DragPosition.Relative)
			{
				if (dragFingerCount != num)
				{
					dragFingerCount = num;
					worldDelta = new Vector3(targetCollider.gameObject.transform.position.x - vector.x, targetCollider.gameObject.transform.position.y - vector.y, targetCollider.gameObject.transform.position.z);
				}
				targetPoint = new Vector3(vector.x + worldDelta.x, vector.y + worldDelta.y, worldDelta.z);
			}
			else
			{
				targetPoint = vector;
			}
		}
		GestureMessage("GestureMoveDrag");
	}

	protected void DragGestureDownAndMovingEnd(Finger fingerIn)
	{
		if (draggingObject && finger == fingerIn)
		{
			draggingObject = false;
			endPoint = targetCollider.gameObject.transform.localPosition;
			GestureMessage("GestureEndDrag");
		}
		dragFingerCount = 0;
	}

	private void Update()
	{
		if (doDrag && targetPoint != targetCollider.gameObject.transform.position)
		{
			targetCollider.gameObject.transform.position = Vector3.Lerp(targetCollider.gameObject.transform.position, targetPoint, Time.deltaTime * 25f);
		}
	}

	public void RestoreObject()
	{
		base.transform.position = originalPosition;
		targetPoint = targetCollider.gameObject.transform.position;
	}
}
