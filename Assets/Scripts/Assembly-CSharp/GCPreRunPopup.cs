using System;
using UnityEngine;

public class GCPreRunPopup : MonoBehaviour
{
	private string m_daysLeftStringID = "GC_DAYS_LEFT";

	private string m_finalDayStringID = "GC_FINAL_DAY";

	[SerializeField]
	private UILabel m_timeLeftLabel;

	[SerializeField]
	private UILabel m_points;

	private void OnEnable()
	{
		if (DCTimeValidation.TrustedTime)
		{
			m_timeLeftLabel.enabled = true;
			DateTime currentTime = DCTime.GetCurrentTime();
			TimeSpan timeSpan = GCState.GetChallengeDate(GCState.Challenges.gc6).Subtract(currentTime);
			int num = Mathf.CeilToInt((float)timeSpan.TotalDays);
			if (timeSpan.TotalHours > 24.0)
			{
				m_timeLeftLabel.text = string.Format(LanguageStrings.First.GetString(m_daysLeftStringID), num);
			}
			else
			{
				m_timeLeftLabel.text = LanguageStrings.First.GetString(m_finalDayStringID);
			}
		}
		else
		{
			m_timeLeftLabel.enabled = false;
		}
		m_points.text = GC6Progress.ActualPointsContributed.ToString();
	}
}
