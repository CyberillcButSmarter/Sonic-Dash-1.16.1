using System;
using UnityEngine;

[AddComponentMenu("Dash/Cameras/Spline")]
public class CameraTypeSpline : CameraType
{
	public enum Direction
	{
		Forward = 0,
		Backwards = 1
	}

	[Flags]
	private enum State
	{
		None = 0,
		Moving = 1
	}

	[SerializeField]
	private iTweenPath m_positionPath;

	[SerializeField]
	private iTweenPath m_lookAtPath;

	[SerializeField]
	private float m_transitionTime = 30f;

	private GameObject m_lookAtTracker;

	private GameObject m_positionTracker;

	private State m_state;

	public iTweenPath PositionPath
	{
		get
		{
			return m_positionPath;
		}
		set
		{
			m_positionPath = value;
		}
	}

	public iTweenPath LookAtPath
	{
		get
		{
			return m_lookAtPath;
		}
		set
		{
			m_lookAtPath = value;
		}
	}

	public float TransitionTime
	{
		get
		{
			return m_transitionTime;
		}
		set
		{
			m_transitionTime = value;
		}
	}

	public bool InTransition
	{
		get
		{
			return (m_state & State.Moving) == State.Moving;
		}
	}

	public override bool EnableSmoothing
	{
		get
		{
			return true;
		}
	}

	public void PrepareForMovement(Direction direction)
	{
		Vector3[] pathPoints = GetPathPoints(m_positionPath, direction);
		Vector3[] pathPoints2 = GetPathPoints(m_lookAtPath, direction);
		m_positionTracker.transform.position = pathPoints[0];
		m_lookAtTracker.transform.position = pathPoints2[0];
		base.transform.LookAt(m_lookAtTracker.transform.position);
	}

	public void StartMovement(Direction direction, iTween.EaseType camPosEaseType, iTween.EaseType lookAtEasyType)
	{
		Vector3[] pathPoints = GetPathPoints(m_positionPath, direction);
		Vector3[] pathPoints2 = GetPathPoints(m_lookAtPath, direction);
		iTween.MoveTo(m_positionTracker, iTween.Hash("path", pathPoints, "time", m_transitionTime, "easetype", camPosEaseType, "oncomplete", "OnPathFinished", "oncompletetarget", base.gameObject, "oncompleteparams", this));
		iTween.MoveTo(m_lookAtTracker, iTween.Hash("path", pathPoints2, "time", m_transitionTime, "easetype", lookAtEasyType));
		m_state |= State.Moving;
	}

	private void Start()
	{
		m_lookAtTracker = new GameObject(string.Format("{0} Look At Tracker", base.name));
		m_positionTracker = new GameObject(string.Format("{0} Position Tracker", base.name));
	}

	private void Update()
	{
		Vector3 vector = m_positionTracker.transform.position;
		Vector3 vector2 = m_lookAtTracker.transform.position;
		Vector3 normalized = (vector2 - vector).normalized;
		if (BehindCamera.GetMainCamera().IsMenuCameraActive())
		{
			Vector3 vector3 = vector + 5f * normalized;
			Quaternion lookRotation = HeadTrackingReceiver.GetLookRotation(1f, 0.2f);
			Quaternion quaternion = Quaternion.LookRotation(normalized);
			Quaternion quaternion2 = quaternion * lookRotation * Quaternion.Inverse(quaternion);
			vector2 = quaternion2 * (vector2 - vector3) + vector3;
			vector = quaternion2 * (vector - vector3) + vector3;
		}
		base.gameObject.transform.position = vector;
		base.CachedLookAt = vector2;
		base.transform.LookAt(base.CachedLookAt);
	}

	private Vector3[] GetPathPoints(iTweenPath path, Direction direction)
	{
		Vector3[] array = path.nodes.ToArray();
		if (direction == Direction.Backwards)
		{
			Array.Reverse(array);
		}
		return array;
	}

	private void OnPathFinished()
	{
		m_state &= ~State.Moving;
	}
}
