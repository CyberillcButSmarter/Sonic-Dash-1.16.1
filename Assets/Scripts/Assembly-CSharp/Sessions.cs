using UnityEngine;

public class Sessions : MonoBehaviour
{
	private const string SessionTimeProperty = "EventSessionTime";

	private float m_currentSessionTime;

	private float m_totalPlayTime;

	private void Start()
	{
		EventDispatch.RegisterInterest("OnGameDataLoaded", this);
		EventDispatch.RegisterInterest("OnGameDataSaveRequest", this);
		EventDispatch.RegisterInterest("SessionStarted", this);
		EventDispatch.GenerateEvent("SessionStarted");
	}

	private void Update()
	{
		m_currentSessionTime += IndependantTimeDelta.Delta;
		m_totalPlayTime += IndependantTimeDelta.Delta;
	}

	private void OnSessionStarted()
	{
		m_currentSessionTime = 0f;
	}

	private void OnSessionEnded()
	{
		SLAnalytics.AddParameter("SessionLength", ((int)m_currentSessionTime).ToString());
		SLAnalytics.AddParameter("TotalPlayTime", ((int)m_totalPlayTime).ToString());
		SLAnalytics.AddParameter("PlayerID", UserIdentification.Current);
		SLAnalytics.AddParameter("SessionKey", UserIdentification.Generate);
		string value = string.Empty;
		if (((HLLocalUser)Social.localUser).isFacebookAuthenticated)
		{
			value = ((HLLocalUser)Social.localUser).FacebookUserID;
		}
		SLAnalytics.AddParameter("FacebookId", value);
		SLAnalytics.DelayEventWithParameters("SessionEnded");
		EventDispatch.GenerateEvent("SessionEnded");
		PropertyStore.Save();
	}

	private void OnApplicationPause(bool paused)
	{
		if (paused)
		{
			OnSessionEnded();
		}
	}

	private void Event_SessionStarted()
	{
		OnSessionStarted();
	}

	private void Event_OnGameDataSaveRequest()
	{
		PropertyStore.Store("EventSessionTime", m_totalPlayTime);
	}

	private void Event_OnGameDataLoaded(ActiveProperties activeProperties)
	{
		m_totalPlayTime = activeProperties.GetFloat("EventSessionTime");
	}
}
