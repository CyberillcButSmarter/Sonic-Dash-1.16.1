using UnityEngine;

public class SLAnalytics
{
	private static AndroidJavaClass m_SLAnalytics = new AndroidJavaClass("com.hardlightstudio.dev.sonicdash.plugin.SLAnalytics");

	private static bool m_inSession = false;

	public static void Start()
	{
		m_SLAnalytics.CallStatic("StartSession");
		m_inSession = true;
	}

	public static void OnFocus(bool gainFocus)
	{
		if (m_SLAnalytics != null)
		{
			if (gainFocus)
			{
				m_SLAnalytics.CallStatic("StartSession");
				m_inSession = true;
			}
			else
			{
				m_SLAnalytics.CallStatic("EndSession");
				m_inSession = false;
			}
		}
	}

	public static void Update()
	{
	}

	public static void EnableMetrics(bool enableMetrics)
	{
		string text = ((!enableMetrics) ? "no" : "yes");
		m_SLAnalytics.CallStatic("EnableMetrics", text);
	}

	public static void AddParameter(string Key, string Value)
	{
		m_SLAnalytics.CallStatic("AnalyticsAddParameter", Key, Value);
	}

	public static void LogTrackingEvent(string EventName, string Value)
	{
		m_SLAnalytics.CallStatic("AnalyticsLogTrackEvent", EventName, Value);
	}

	public static void LogEvent(string EventName)
	{
		m_SLAnalytics.CallStatic("AnalyticsLogEvent", EventName);
	}

	public static void LogEventWithParameters(string EventName)
	{
		m_SLAnalytics.CallStatic("AnalyticsLogEventWithParameters", EventName);
	}

	public static void LogPushEventWithParameters(string paramName, string paramValue)
	{
		m_SLAnalytics.CallStatic("AnalyticsLogPushEvent", paramName, paramValue);
	}

	public static void LogPushTagWithParameters(string paramName, string paramValue, string paramType)
	{
		m_SLAnalytics.CallStatic("AnalyticsLogPushTag", paramName, paramValue, paramType);
	}

	public static void LogPushTagWithTimestamp(string paramName)
	{
		m_SLAnalytics.CallStatic("AnalyticsLogPushTagTimestamp", paramName);
	}

	public static void UpdatePushTags()
	{
		LogPushTagWithParameters("missions", PlayerStats.GetStat(PlayerStats.StatNames.MissionsCompleted_Total).ToString(), "double");
		LogPushTagWithParameters("runs", PlayerStats.GetStat(PlayerStats.StatNames.NumberOfRuns_Total).ToString(), "double");
		LogPushTagWithParameters("distance", ((int)(PlayerStats.GetDistance(PlayerStats.DistanceNames.DistanceRun_Total) / 1000f)).ToString(), "double");
		LogPushTagWithParameters("shop_purchases", PlayerStats.GetStat(PlayerStats.StatNames.ShopPurchases_Total).ToString(), "double");
		LogPushTagWithParameters("inapp_purchases", PlayerStats.GetStat(PlayerStats.StatNames.InAppPurchases_Total).ToString(), "double");
		LogPushTagWithParameters("time_played", ((float)PlayerStats.GetStat(PlayerStats.StatNames.TimePlayed_Total) * 1f / 36000f).ToString("N"), "double");
		LogPushTagWithParameters("actual_cohort", ABTesting.Cohort.ToString(), "string");
	}

	public static void DelayEventWithParameters(string eventName)
	{
		m_SLAnalytics.CallStatic("AnalyticsDelayEventWithParameters", eventName);
	}
}
