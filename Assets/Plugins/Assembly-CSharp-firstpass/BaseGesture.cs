using System.Collections.Generic;
using UnityEngine;

public class BaseGesture : MonoBehaviour
{
	public enum FingerLocation
	{
		Over = 0,
		Always = 1,
		NotOver = 2,
		AtLeastOneOver = 3
	}

	public enum FingerCountRestriction
	{
		Any = 0,
		One = 1,
		Two = 2,
		Three = 3,
		Four = 4,
		Five = 5,
		OneOrTwo = 6,
		OneOrTwoOrThree = 7,
		TwoOrThree = 8
	}

	public enum XYRestriction
	{
		AllDirections = 0,
		XDirecton = 1,
		YDirection = 2
	}

	public GameObject[] targetMessageObjects;

	public Camera alternateCamera;

	public Collider targetCollider;

	public bool topColliderOnly;

	public Finger finger;

	public Finger[] fingers;

	public int fingerCount;

	public bool activeChange = true;

	protected static int baseId;

	private int myId;

	private bool setId = true;

	private Camera theCamera;

	private bool ignoreGesture;

	public static FingerControl fingerControl;

	private bool added;

	private int activeCount;

	public Bounds emptyBounds = new Bounds(Vector3.zero, Vector3.zero);

	protected bool IsGestureIgnored
	{
		get
		{
			return ignoreGesture;
		}
	}

	public void IgnoreGesture()
	{
		ignoreGesture = true;
	}

	protected void ClearIgnoreGesture()
	{
		ignoreGesture = false;
	}

	protected virtual void Start()
	{
		initialize();
		AddGesture();
	}

	protected string getId()
	{
		return base.name + "-" + myId;
	}

	protected void initialize()
	{
		if (setId)
		{
			myId = baseId++;
			setId = false;
		}
		if (fingerControl == null)
		{
			fingerControl = (FingerControl)base.gameObject.AddComponent<TouchScreenControl>();
		}
		setCamera();
		ClearIgnoreGesture();
	}

	private void setCamera()
	{
		if ((bool)theCamera)
		{
			return;
		}
		if ((bool)alternateCamera)
		{
			theCamera = alternateCamera;
			return;
		}
		theCamera = Camera.main;
		if (!theCamera)
		{
			theCamera = Camera.current;
			if (!theCamera && Camera.allCameras.Length > 0)
			{
				theCamera = Camera.allCameras[0];
			}
		}
	}

	private void Awake()
	{
		if (targetMessageObjects != null && targetMessageObjects.Length == 1 && targetMessageObjects[0] == null)
		{
			targetMessageObjects[0] = base.gameObject;
		}
		if (targetMessageObjects == null || targetMessageObjects.Length == 0)
		{
			targetMessageObjects = new GameObject[1];
			targetMessageObjects[0] = base.gameObject;
		}
		if (targetCollider == null)
		{
			targetCollider = base.gameObject.GetComponent<Collider>();
		}
	}

	public void AddGesture()
	{
		if (!added)
		{
			added = true;
			FingerControl.AddGesture(this);
		}
	}

	public void RemoveGesture()
	{
		if (added)
		{
			added = false;
			FingerControl.RemoveGesture(this);
		}
	}

	private void OnDestroy()
	{
		RemoveGesture();
	}

	public void StartGesture()
	{
		EnableGesture();
	}

	public void EndGesture()
	{
		DisableGesture();
	}

	protected virtual void EnableGesture()
	{
		FingerControl._delegateIsDownInternal += GestureIsDown;
		ClearIgnoreGesture();
	}

	protected virtual void DisableGesture()
	{
		FingerControl._delegateIsDownInternal -= GestureIsDown;
	}

	protected void GestureIsDown(Finger fingerIn, bool isDownIn)
	{
		activeChange = true;
	}

	protected bool IsOnObject(Vector2 pos)
	{
		if (targetCollider == null)
		{
			return false;
		}
		if (topColliderOnly)
		{
			RaycastHit hitInfo;
			if (Physics.Raycast(theCamera.ScreenPointToRay(pos), out hitInfo) && hitInfo.collider.gameObject == targetCollider.gameObject)
			{
				return true;
			}
		}
		else
		{
			RaycastHit[] array = Physics.RaycastAll(theCamera.ScreenPointToRay(pos));
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].collider.gameObject == targetCollider.gameObject)
				{
					return true;
				}
			}
		}
		return false;
	}

	protected Vector3 ScreenToWorldPosition(Vector2 screenPos)
	{
		return ScreenToWorldPosition(screenPos, float.MinValue);
	}

	protected Vector3 ScreenToWorldPosition(Vector2 screenPos, float dist)
	{
		Ray ray = theCamera.ScreenPointToRay(screenPos);
		if (dist == float.MinValue)
		{
			if (targetCollider != null)
			{
				dist = (ray.origin - targetCollider.gameObject.transform.position).magnitude;
				return ray.GetPoint(dist);
			}
			return ray.GetPoint((0f - ray.origin.z) / ray.direction.z);
		}
		return ray.GetPoint(dist);
	}

	protected void SetFingers(Finger[] fingersIn, int count)
	{
		finger = fingersIn[0];
		fingers = new Finger[count];
		for (int i = 0; i < count; i++)
		{
			fingers[i] = fingersIn[i];
		}
	}

	public int ActiveCount()
	{
		if (activeChange)
		{
			Finger[] array = FingerControl.Factory().fingers;
			activeCount = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].isDown)
				{
					activeCount++;
				}
			}
			activeChange = false;
		}
		return activeCount;
	}

	protected void GestureMessage(string gestureName)
	{
		fingerCount = ActiveCount();
		for (int i = 0; i < targetMessageObjects.Length; i++)
		{
			targetMessageObjects[i].SendMessage(gestureName, this, SendMessageOptions.DontRequireReceiver);
		}
	}

	protected bool FingerActivated(FingerLocation fLocation)
	{
		if (fLocation == FingerLocation.Always)
		{
			return true;
		}
		Finger[] array = FingerControl.Factory().fingers;
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].isDown)
			{
				bool flag = FingerActivated(fLocation, array[i].position);
				if (flag)
				{
					num++;
				}
				if ((fLocation == FingerLocation.Over || fLocation == FingerLocation.NotOver) && !flag)
				{
					return false;
				}
			}
		}
		if (fLocation == FingerLocation.AtLeastOneOver && num == 0)
		{
			return false;
		}
		return true;
	}

	protected bool FingerActivated(FingerLocation fLocation, Vector2 position)
	{
		switch (fLocation)
		{
		case FingerLocation.Always:
			return true;
		case FingerLocation.Over:
		case FingerLocation.AtLeastOneOver:
			return IsOnObject(position);
		case FingerLocation.NotOver:
			return !IsOnObject(position);
		default:
			return false;
		}
	}

	protected bool FingerCountGood(int fingerCount, FingerCountRestriction restrictFingerCount)
	{
		if (restrictFingerCount == FingerCountRestriction.Any)
		{
			return true;
		}
		switch (fingerCount)
		{
		case 1:
			if (restrictFingerCount == FingerCountRestriction.One || restrictFingerCount == FingerCountRestriction.OneOrTwo || restrictFingerCount == FingerCountRestriction.OneOrTwoOrThree)
			{
				return true;
			}
			break;
		case 2:
			if (restrictFingerCount == FingerCountRestriction.Two || restrictFingerCount == FingerCountRestriction.OneOrTwo || restrictFingerCount == FingerCountRestriction.OneOrTwoOrThree || restrictFingerCount == FingerCountRestriction.TwoOrThree)
			{
				return true;
			}
			break;
		case 3:
			if (restrictFingerCount == FingerCountRestriction.Three || restrictFingerCount == FingerCountRestriction.OneOrTwoOrThree || restrictFingerCount == FingerCountRestriction.TwoOrThree)
			{
				return true;
			}
			break;
		case 4:
			if (restrictFingerCount == FingerCountRestriction.Four)
			{
				return true;
			}
			break;
		case 5:
			if (restrictFingerCount == FingerCountRestriction.Five)
			{
				return true;
			}
			break;
		}
		return false;
	}

	protected void CleanFingers(List<Finger> fingers)
	{
		if (fingers == null || fingers.Count == 0)
		{
			return;
		}
		for (int i = 0; i < fingers.Count; i++)
		{
			Finger finger = fingers[i];
			if (!finger.isDown || !finger.SetState())
			{
				fingers.RemoveAt(i);
				i--;
			}
		}
	}

	protected Vector3 CalcWorldPosForAllFingers(Vector2 fingerPos)
	{
		Vector3 result = ScreenToWorldPosition(CalcPosForAllFingers(fingerPos));
		result.z = targetCollider.gameObject.transform.position.z;
		return result;
	}

	protected Vector2 CalcPosForAllFingers(Vector2 fingerPos)
	{
		int num = ActiveCount();
		if (num < 2)
		{
			return fingerPos;
		}
		Finger[] array = FingerControl.Factory().fingers;
		float num2 = 0f;
		float num3 = 0f;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].isDown)
			{
				num2 += array[i].position.x;
				num3 += array[i].position.y;
			}
		}
		num2 /= (float)num;
		num3 /= (float)num;
		return new Vector2(num2, num3);
	}

	protected Bounds GetBounds()
	{
		if (base.gameObject == null)
		{
			return emptyBounds;
		}
		Collider[] componentsInChildren = base.gameObject.GetComponentsInChildren<Collider>();
		if (componentsInChildren != null && componentsInChildren.Length > 0)
		{
			Bounds bounds = componentsInChildren[0].bounds;
			for (int i = 1; i < componentsInChildren.Length; i++)
			{
				bounds.Encapsulate(componentsInChildren[i].bounds);
			}
			return bounds;
		}
		Renderer[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<Renderer>();
		Bounds bounds2 = componentsInChildren2[0].bounds;
		if (componentsInChildren2 != null && componentsInChildren2.Length > 0)
		{
			for (int j = 1; j < componentsInChildren.Length; j++)
			{
				bounds2.Encapsulate(componentsInChildren2[j].bounds);
			}
			return bounds2;
		}
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		if (mesh != null)
		{
			return mesh.bounds;
		}
		return emptyBounds;
	}
}
