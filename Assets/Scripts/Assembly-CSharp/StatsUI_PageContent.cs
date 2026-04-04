using System;
using System.Collections;
using UnityEngine;

public class StatsUI_PageContent : MonoBehaviour
{
	[SerializeField]
	private UILabel m_bestScore_Run_Label;

	[SerializeField]
	private UILabel m_bestDistance_Run_Label;

	[SerializeField]
	private UILabel m_bestRingsBanked_Run_Label;

	[SerializeField]
	private UILabel m_bestRingStreaks_Run_Label;

	[SerializeField]
	private UILabel m_bestEnemies_Run_Label;

	[SerializeField]
	private UILabel m_bestEnemyStreaks_Run_Label;

	[SerializeField]
	private UILabel m_timePlayed_Total_Label;

	[SerializeField]
	private UILabel m_numberOfRuns_Total_Label;

	[SerializeField]
	private UILabel m_ringsCollected_Total_Label;

	[SerializeField]
	private UILabel m_distance_Total_Label;

	private void Update()
	{
		SetPlayedTime();
	}

	private void OnEnable()
	{
		StartCoroutine(StartPendingActivation());
	}

	private IEnumerator StartPendingActivation()
	{
		yield return null;
		PlayerStats.Stats currentStats = PlayerStats.GetCurrentStats();
		string localDistance = LanguageStrings.First.GetString("STATS_DISTANCE");
		m_bestScore_Run_Label.text = LanguageUtils.FormatNumber(currentStats.m_trackedLongStats[2]);
		m_bestDistance_Run_Label.text = LanguageUtils.FormatNumber((int)currentStats.m_trackedDistances[4]) + " " + localDistance;
		m_bestRingsBanked_Run_Label.text = LanguageUtils.FormatNumber(currentStats.m_trackedStats[57]);
		m_bestRingStreaks_Run_Label.text = LanguageUtils.FormatNumber(currentStats.m_trackedStats[58]);
		m_bestEnemies_Run_Label.text = LanguageUtils.FormatNumber(currentStats.m_trackedStats[59]);
		m_bestEnemyStreaks_Run_Label.text = LanguageUtils.FormatNumber(currentStats.m_trackedStats[60]);
		SetPlayedTime();
		m_numberOfRuns_Total_Label.text = LanguageUtils.FormatNumber(currentStats.m_trackedStats[0]);
		m_ringsCollected_Total_Label.text = LanguageUtils.FormatNumber(currentStats.m_trackedStats[12]);
		m_distance_Total_Label.text = LanguageUtils.FormatNumber((int)currentStats.m_trackedDistances[0]) + " " + localDistance;
	}

	private void SetPlayedTime()
	{
		string format = LanguageStrings.First.GetString("STATS_TIME_DAYS_HOURS");
		string format2 = LanguageStrings.First.GetString("STATS_TIME_MINUTES_HOURS");
		string format3 = LanguageStrings.First.GetString("STATS_TIME_MINUTES");
		double value = (double)PlayerStats.GetCurrentStats().m_trackedStats[64] * 100.0;
		TimeSpan timeSpan = TimeSpan.FromMilliseconds(value).Add(TimeSpan.FromSeconds((int)PlayerStats.GetSecondsFromLastSaved()));
		if (timeSpan.Days > 0)
		{
			m_timePlayed_Total_Label.text = string.Format(format, timeSpan.Days, timeSpan.Hours);
		}
		else if (timeSpan.Hours > 0)
		{
			m_timePlayed_Total_Label.text = string.Format(format2, timeSpan.Hours, timeSpan.Minutes);
		}
		else
		{
			m_timePlayed_Total_Label.text = string.Format(format3, timeSpan.Minutes);
		}
	}
}
