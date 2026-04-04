using System.Collections;
using UnityEngine;

[AddComponentMenu("Dash/Cameras/Spring Cam")]
public class CameraTypeSpring : CameraType
{
	private const float c_fTokenHuntOrbitSpeedScale = 1f;

	private const float c_fTokenHuntOrbitRadiusScale = 1.5f;

	private Vector3 m_velocity;

	[SerializeField]
	private string m_sonicHeadBoneName;

	private bool m_isTokenHunt = true;

	private bool m_isSpringDescentStarted;

	private bool m_gestureFailureEntryLock;

	private float m_headshakeOffset;

	private float m_headTrackingBlend;

	private Vector3 CachedLookAtWithoutHeadTracking = Vector3.zero;

	private Vector3 CachedPositionWithoutHeadTracking = Vector3.zero;

	[SerializeField]
	private AnimationCurve m_climbYOffset;

	[SerializeField]
	private float m_orbitUpOffset = -3f;

	[SerializeField]
	private float m_orbitRadius = 2f;

	[SerializeField]
	private float m_orbitAngularVelocity = 180f;

	[SerializeField]
	private float m_climbSmoothing = 20f;

	[SerializeField]
	private float m_decendSmoothing = 20f;

	[SerializeField]
	private AnimationCurve m_descentYOffset;

	[SerializeField]
	private AnimationCurve m_orbitAngularVelocityMultiplier;

	[SerializeField]
	private AnimationCurve m_descentAngularVelocityMultiplierToEnd;

	[SerializeField]
	private float m_descentStartDeadZoneDuration = 0.5f;

	[SerializeField]
	private float m_descentLookAhead = 15f;

	[SerializeField]
	private float m_descentLookAheadSmoothSpeed = 1f;

	[SerializeField]
	private AnimationCurve m_failureHeadshakeLateralOffsetOverTime;

	[SerializeField]
	private float m_failureHeadshakeLateralOffsetMagnitude = 0.2f;

	public override bool EnableSmoothing
	{
		get
		{
			return false;
		}
	}

	private void Awake()
	{
		EventDispatch.RegisterInterest("OnSpringDescent", this);
		EventDispatch.RegisterInterest("OnSpringEnd", this);
		EventDispatch.RegisterInterest("SpringGestureFailure", this);
		EventDispatch.RegisterInterest("CharacterUnloadStart", this);
		EventDispatch.RegisterInterest("CharacterLoaded", this);
		StartCoroutine(IdleLoop());
	}

	private IEnumerator IdleLoop()
	{
		while (true)
		{
			if (Sonic.Bones != null)
			{
				Transform sonicHeadTransform = Sonic.Bones[m_sonicHeadBoneName];
				if (null != sonicHeadTransform)
				{
					base.CachedLookAt = sonicHeadTransform.position;
				}
				if (null != Camera.main)
				{
					base.transform.position = Camera.main.transform.position;
					base.transform.rotation = Camera.main.transform.rotation;
				}
			}
			yield return null;
		}
	}

	public override void onActive()
	{
		m_isSpringDescentStarted = false;
		m_headTrackingBlend = 0f;
		StopAllCoroutines();
		StartCoroutine(MoveToOrbitStart());
	}

	private void Event_SpringGestureFailure()
	{
		if (!m_gestureFailureEntryLock)
		{
			StartCoroutine(FailureCameraFeedback());
		}
	}

	public void Event_CharacterUnloadStart()
	{
		StopAllCoroutines();
	}

	public void Event_CharacterLoaded()
	{
		StopAllCoroutines();
		StartCoroutine(IdleLoop());
	}

	private IEnumerator FailureCameraFeedback()
	{
		m_gestureFailureEntryLock = true;
		float headshakeDuration = m_failureHeadshakeLateralOffsetOverTime.keys[m_failureHeadshakeLateralOffsetOverTime.length - 1].time;
		float headshakeTimer = 0f;
		while (headshakeTimer < headshakeDuration)
		{
			m_headshakeOffset = m_failureHeadshakeLateralOffsetOverTime.Evaluate(headshakeTimer) * m_failureHeadshakeLateralOffsetMagnitude;
			headshakeTimer += Time.deltaTime;
			yield return null;
		}
		m_headshakeOffset = 0f;
		m_gestureFailureEntryLock = false;
	}

	private IEnumerator MoveToOrbitStart()
	{
		m_velocity = Vector3.zero;
		Vector3 cameraPosition = base.transform.position;
		Vector3 originalPosition = base.transform.position;
		float orbitTime = 0f;
		Transform sonicHeadTransform = Sonic.Bones[m_sonicHeadBoneName];
		while (true)
		{
			float climbYOffset = m_climbYOffset.Evaluate(orbitTime);
			Vector3 orbitPos = CalculateOrbitPosAtTime(orbitTime) + climbYOffset * Sonic.MeshTransform.up;
			cameraPosition = ((!(Mathf.Abs(cameraPosition.x - orbitPos.x) > 25f) && !(Mathf.Abs(cameraPosition.z - orbitPos.z) > 25f)) ? Vector3.SmoothDamp(cameraPosition, orbitPos, ref m_velocity, m_climbSmoothing) : orbitPos);
			if (m_isTokenHunt)
			{
				HeadTrackingReceiver.WakeUp();
			}
			orbitTime += Time.deltaTime;
			float orbitErrorSqd = (orbitPos - cameraPosition).sqrMagnitude;
			if (orbitErrorSqd < 0.06f || (m_isSpringDescentStarted && !m_gestureFailureEntryLock))
			{
				break;
			}
			Vector3 position = cameraPosition;
			position.y = Mathf.Max(originalPosition.y, cameraPosition.y);
			UpdateCamera(position, sonicHeadTransform.position);
			yield return new WaitForEndOfFrame();
		}
		if (m_isSpringDescentStarted && !m_gestureFailureEntryLock)
		{
			yield return StartCoroutine(OrbitDescent(orbitTime, 360f / GetOrbitAngularVelocity()));
		}
		else
		{
			yield return StartCoroutine(OrbitSonic(orbitTime));
		}
	}

	private IEnumerator OrbitSonic(float orbitStartTime)
	{
		float orbitTime = orbitStartTime;
		float orbitPeriod = 360f / GetOrbitAngularVelocity();
		Transform sonicHeadTransform = Sonic.Bones[m_sonicHeadBoneName];
		while (true)
		{
			Vector3 orbitPos = CalculateOrbitPosAtTime(orbitTime);
			float orbitT = Mathf.InverseLerp(0f, orbitPeriod, orbitTime);
			float orbitMul = m_orbitAngularVelocityMultiplier.Evaluate(orbitT);
			orbitTime += Time.deltaTime * orbitMul;
			if (orbitTime > orbitPeriod && !m_isTokenHunt)
			{
				orbitTime -= orbitPeriod;
			}
			if (m_isTokenHunt)
			{
				m_headTrackingBlend = Mathf.Min(1f, m_headTrackingBlend + Time.deltaTime * 4f);
			}
			if (m_isSpringDescentStarted && !m_gestureFailureEntryLock)
			{
				break;
			}
			Vector3 lookatOffset = base.transform.right * m_headshakeOffset;
			Vector3 lookat = sonicHeadTransform.position + lookatOffset;
			UpdateCamera(orbitPos, lookat);
			yield return new WaitForEndOfFrame();
		}
		yield return StartCoroutine(OrbitDescent(orbitTime, orbitPeriod));
	}

	private IEnumerator OrbitDescent(float descentStartTime, float orbitPeriod)
	{
		if (!m_isTokenHunt)
		{
			if (orbitPeriod - descentStartTime < m_descentStartDeadZoneDuration)
			{
				descentStartTime -= orbitPeriod;
			}
		}
		else
		{
			descentStartTime = orbitPeriod * 0.5f;
		}
		float orbitTime = descentStartTime;
		m_velocity = Vector3.zero;
		float lookAhead = 0f;
		float lookAheadVelocity = 0f;
		Transform sonicHeadTransform = Sonic.Bones[m_sonicHeadBoneName];
		while (true)
		{
			if (m_isTokenHunt)
			{
				m_headTrackingBlend = Mathf.Max(0f, m_headTrackingBlend - Time.deltaTime * 1.5f);
			}
			float descentT = Utils.MapValue(orbitTime, descentStartTime, orbitPeriod, 0f, 1f);
			float descentYOffset = m_descentYOffset.Evaluate(descentT);
			Vector3 oldPosition = CachedPositionWithoutHeadTracking;
			Vector3 newPosition = CalculateOrbitPosAtTime(orbitTime) + descentYOffset * Sonic.MeshTransform.up;
			Vector3 idealLookAt = sonicHeadTransform.position + Sonic.MeshTransform.forward * lookAhead * descentT;
			Vector3 previousLookVector = CachedLookAtWithoutHeadTracking - oldPosition;
			Vector3 thisLookVector = idealLookAt - newPosition;
			thisLookVector = Vector3.SmoothDamp(previousLookVector, thisLookVector, ref m_velocity, m_decendSmoothing);
			UpdateCamera(newPosition, newPosition + thisLookVector);
			Debug.DrawLine(base.transform.position, idealLookAt, Color.red);
			Debug.DrawRay(base.CachedLookAt, Vector3.up, Color.red);
			Debug.DrawRay(base.transform.position, base.CachedLookAt - base.transform.position, Color.yellow);
			float descentRotationMultiplier = m_descentAngularVelocityMultiplierToEnd.Evaluate(descentT);
			float orbitT = Mathf.InverseLerp(0f, orbitPeriod, orbitTime);
			float orbitMul = m_orbitAngularVelocityMultiplier.Evaluate(orbitT);
			orbitTime += Time.deltaTime * orbitMul * descentRotationMultiplier;
			if (orbitTime > orbitPeriod)
			{
				orbitTime = orbitPeriod;
				lookAhead = Mathf.SmoothDamp(lookAhead, m_descentLookAhead, ref lookAheadVelocity, m_descentLookAheadSmoothSpeed);
			}
			yield return null;
		}
	}

	private float GetOrbitRadius()
	{
		if (m_isTokenHunt)
		{
			return m_orbitRadius * 1.5f;
		}
		return m_orbitRadius;
	}

	private float GetOrbitAngularVelocity()
	{
		if (m_isTokenHunt)
		{
			return m_orbitAngularVelocity;
		}
		return m_orbitAngularVelocity * 1f;
	}

	private Vector3 CalculateOrbitPosAtTime(float time)
	{
		float orbitRadius = GetOrbitRadius();
		float orbitAngularVelocity = GetOrbitAngularVelocity();
		float num = time * orbitAngularVelocity;
		float num2 = m_orbitUpOffset;
		if (m_isTokenHunt)
		{
			num2 += 1.5f;
			if (!m_isSpringDescentStarted || m_gestureFailureEntryLock)
			{
				num = Mathf.Clamp(time * orbitAngularVelocity, 0f, 180f);
				num = Mathf.SmoothStep(0f, 180f, num / 180f);
			}
			else
			{
				num = time * orbitAngularVelocity;
			}
		}
		else
		{
			num = time * orbitAngularVelocity;
		}
		Vector3 vector = -Sonic.MeshTransform.forward;
		Quaternion quaternion = Quaternion.AngleAxis(num, Sonic.MeshTransform.up);
		Vector3 vector2 = quaternion * vector;
		return Sonic.MeshTransform.position + vector2 * orbitRadius + Sonic.MeshTransform.up * num2;
	}

	private void Event_OnSpringDescent(float springDescentTime)
	{
		m_isSpringDescentStarted = true;
	}

	private void Event_OnSpringEnd()
	{
		StopAllCoroutines();
		BehindCamera.Instance.ResetToGameCamera(0.5f);
		StartCoroutine(IdleLoop());
	}

	private void UpdateCamera(Vector3 camPos, Vector3 lookAt)
	{
		CachedLookAtWithoutHeadTracking = lookAt;
		CachedPositionWithoutHeadTracking = camPos;
		if (m_headTrackingBlend > 0f)
		{
			Vector3 normalized = (lookAt - camPos).normalized;
			Vector3 vector = camPos + normalized * (GetOrbitRadius() + 0.25f);
			Quaternion lookRotation = HeadTrackingReceiver.GetLookRotation(1.2f, 1.2f, 1.5f);
			lookRotation = Quaternion.Slerp(Quaternion.identity, lookRotation, m_headTrackingBlend);
			Quaternion quaternion = Quaternion.LookRotation(normalized);
			Quaternion quaternion2 = quaternion * lookRotation * Quaternion.Inverse(quaternion);
			lookAt = quaternion2 * (lookAt - vector) + vector;
			camPos = quaternion2 * (camPos - vector) + vector;
		}
		base.CachedLookAt = lookAt;
		base.transform.position = camPos;
		base.transform.LookAt(lookAt, Vector3.up);
	}

	private void LateUpdate()
	{
	}

	public void SetSpringType(SpringTV.Type springType)
	{
		m_isTokenHunt = springType == SpringTV.Type.TokenHunt;
	}
}
