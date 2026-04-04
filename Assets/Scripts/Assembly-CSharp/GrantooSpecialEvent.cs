using System;
using System.Globalization;
using UnityEngine;

public class GrantooSpecialEvent : MonoBehaviour
{
	private enum GrantooEventID
	{
		NOTLOADED = -1,
		NOTSPECIFIED = 0
	}

	private class MultiPlayerEvent
	{
		public DateTime endTime;

		public int runsNeeded;

		public bool show;

		public bool shownOnce;

		public MultiPlayerEvent(DateTime endTime, int runsNeeded)
		{
			this.endTime = endTime;
			this.runsNeeded = runsNeeded;
		}
	}

	private const string EventRoot = "events";

	private const string GrantooEventProperty = "grantooevent";

	private const string GrantooEventIdProperty = "grantooeventid";

	private const string GrantooEventEndProperty = "grantooeventend";

	private const string GrantooEventRunsNeededProperty = "grantoorunsneeded";

	private bool display;

	private int lastEvent = -1;

	private int newEvent = -1;

	private static GrantooSpecialEvent s_Instance;

	private MultiPlayerEvent m_event;

	private static string LastEventId
	{
		get
		{
			return "GrantooLastEventId";
		}
	}

	public static GrantooSpecialEvent GetInstance()
	{
		return s_Instance;
	}

	public void Awake()
	{
		EventDispatch.RegisterInterest("OnGameDataLoaded", this);
		EventDispatch.RegisterInterest("OnGameDataSaveRequest", this);
	}

	public void Start()
	{
		s_Instance = this;
		EventDispatch.RegisterInterest("MainMenuActive", this, EventDispatch.Priority.Highest);
		EventDispatch.RegisterInterest("FeatureStateReady", this);
	}

	public void Update()
	{
		if (display && Dialog_GrantooSpecialEvent.Display())
		{
			display = false;
			m_event.show = false;
			m_event.shownOnce = true;
		}
	}

	public bool IsEventActive()
	{
		if (s_Instance != null)
		{
			return s_Instance.m_event != null;
		}
		return false;
	}

	public void Set(DateTime endTime, int runsNeeded)
	{
		m_event = new MultiPlayerEvent(endTime, runsNeeded);
		if (GrantooManager.IsGrantooAvailable() && lastEvent != -1 && newEvent != -1 && ((lastEvent == 0 && newEvent == 0 && !m_event.shownOnce) || lastEvent != newEvent))
		{
			m_event.show = true;
			lastEvent = newEvent;
		}
	}

	public void UnSet()
	{
		m_event = null;
	}

	public TimeSpan GetRemainingTime()
	{
		if (m_event != null)
		{
			TimeSpan timeSpan = m_event.endTime.Subtract(DCTime.GetCurrentTime());
			if (timeSpan > TimeSpan.Zero)
			{
				return timeSpan;
			}
		}
		return TimeSpan.Zero;
	}

	public int GetRunsNeeded()
	{
		if (m_event != null)
		{
			return m_event.runsNeeded;
		}
		return 0;
	}

	public void ShowIfShould()
	{
		display = GrantooManager.IsGrantooAvailable() && IsEventActive() && m_event.show;
	}

	private void Event_MainMenuActive()
	{
		ShowIfShould();
	}

	private void Event_FeatureStateReady()
	{
		if (!FeatureState.Valid)
		{
			return;
		}
		LSON.Property stateProperty = FeatureState.GetStateProperty("events", "grantooevent");
		if (stateProperty == null)
		{
			return;
		}
		bool boolValue = false;
		if (!LSONProperties.AsBool(stateProperty, out boolValue))
		{
			return;
		}
		if (boolValue)
		{
			stateProperty = FeatureState.GetStateProperty("events", "grantooeventend");
			if (stateProperty == null)
			{
				return;
			}
			string stringValue = null;
			if (!LSONProperties.AsString(stateProperty, out stringValue))
			{
				return;
			}
			CultureInfo provider = new CultureInfo("en-US");
			stringValue = stringValue.Replace("_", " ");
			stringValue = stringValue.Replace(".", ":");
			DateTime result = DateTime.Now;
			if (DateTime.TryParse(stringValue, provider, DateTimeStyles.None, out result))
			{
				int intValue = 0;
				stateProperty = FeatureState.GetStateProperty("events", "grantoorunsneeded");
				if (stateProperty == null || LSONProperties.AsInt(stateProperty, out intValue))
				{
				}
				newEvent = 0;
				stateProperty = FeatureState.GetStateProperty("events", "grantooeventid");
				if (stateProperty == null || LSONProperties.AsInt(stateProperty, out newEvent))
				{
				}
				Set(result, intValue);
			}
		}
		else
		{
			UnSet();
		}
	}

	private void Event_OnGameDataSaveRequest()
	{
		if (lastEvent != -1)
		{
			PropertyStore.Store(LastEventId, lastEvent);
		}
	}

	private void Event_OnGameDataLoaded(ActiveProperties activeProperties)
	{
		lastEvent = activeProperties.GetInt(LastEventId);
	}
}
