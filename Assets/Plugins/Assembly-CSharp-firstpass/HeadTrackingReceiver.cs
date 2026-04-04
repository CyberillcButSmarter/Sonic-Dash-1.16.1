using UnityEngine;

public class HeadTrackingReceiver : MonoBehaviour
{
	private const float c_fLookLimit = 0.58f;

	private const float c_fTimeUntilStandby = 1f;

	public static bool ReverseRotation;

	public static HeadTrackingEvent lastEvent;

	private HeadTrackingManager headTrackingManager;

	private HeadTrackingPoller poller;

	private static float s_fStandbyTimer;

	private bool m_bStandby;

	private static float s_fLookLimit = 0.58f;

	private void Start()
	{
	}

	private void OnApplicationQuit()
	{
		if (headTrackingManager != null && poller != null)
		{
			headTrackingManager.ReleasePoller(poller);
		}
	}

	private void Update()
	{
		if (!m_bStandby)
		{
			if (s_fStandbyTimer >= 1f)
			{
				m_bStandby = true;
				if (headTrackingManager != null)
				{
					headTrackingManager.RequestStandby(m_bStandby);
				}
			}
		}
		else if (s_fStandbyTimer <= 1f)
		{
			m_bStandby = false;
			if (headTrackingManager != null)
			{
				headTrackingManager.RequestStandby(m_bStandby);
			}
			if (lastEvent != null)
			{
				lastEvent.x = (lastEvent.y = 0f);
				lastEvent.z = 1f;
			}
		}
		s_fStandbyTimer += Time.deltaTime;
		if (!m_bStandby && poller != null && lastEvent != null && !poller.Sample(ref lastEvent))
		{
			Debug.LogWarning("Head Tracking sample failed");
		}
	}

	public static Quaternion GetLookRotation(float fXScale = 1f, float fYScale = 1f, float fLimit = 0.58f, bool bReversed = false)
	{
		Quaternion result = default(Quaternion);
		Vector3 forward = Vector3.forward;
		s_fLookLimit = fLimit;
		if (ReverseRotation)
		{
			bReversed = !bReversed;
		}
		if (lastEvent != null && lastEvent.z > 0f)
		{
			if (bReversed)
			{
				forward.Set(lastEvent.x, lastEvent.y, lastEvent.z);
			}
			else
			{
				forward.Set(0f - lastEvent.x, 0f - lastEvent.y, lastEvent.z);
			}
			forward.Normalize();
			float z = forward.z;
			forward.x = fXScale * Mathf.Clamp(forward.x, (0f - fLimit) * z, fLimit * z);
			forward.y = fYScale * Mathf.Clamp(forward.y, (0f - fLimit) * z, fLimit * z);
			forward.Normalize();
		}
		result.SetLookRotation(forward);
		s_fStandbyTimer = 0f;
		return result;
	}

	public static void WakeUp()
	{
		s_fStandbyTimer = 0f;
	}
}
