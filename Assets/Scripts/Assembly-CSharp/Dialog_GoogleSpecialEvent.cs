using System;
using System.Collections;
using UnityEngine;

public class Dialog_GoogleSpecialEvent : MonoBehaviour
{
	public enum DialogMode
	{
		Progress = 0,
		Success = 1,
		Failure = 2
	}

	private static DialogMode s_eDialogMode;

	[SerializeField]
	private GameObject m_contentProgressParent;

	[SerializeField]
	private GameObject m_contentResultParent;

	[SerializeField]
	private UILabel m_timeRemainingLabel;

	[SerializeField]
	private UILabel m_runsNeededLabel;

	[SerializeField]
	private UILabel m_resultsInfoLabel;

	[SerializeField]
	private GameObject m_congratsObject;

	[SerializeField]
	private GameObject m_badLuckObject;

	public static void Display(DialogMode eMode)
	{
		s_eDialogMode = eMode;
		DialogStack.ShowDialog("Google Special Event Dialog");
	}

	private void OnEnable()
	{
		StartCoroutine(StartPendingActivation());
	}

	private IEnumerator StartPendingActivation()
	{
		yield return null;
		UpdateContent();
	}

	private void UpdateContent()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (s_eDialogMode == DialogMode.Progress)
		{
			NGUITools.SetActive(m_contentProgressParent, true);
			NGUITools.SetActive(m_contentResultParent, false);
			string text = ((GoogleSpecialEvent.GetRunsNeeded() != GoogleSpecialEvent.GetTotalRunsNeeded()) ? string.Format(LanguageStrings.First.GetString("GOOGLE_EVENT_PROGRESS_BODY_LOWER"), GoogleSpecialEvent.GetRunsNeeded()) : string.Format(LanguageStrings.First.GetString("GOOGLE_EVENT_INTRO_BODY_LOWER"), GoogleSpecialEvent.GetRunsNeeded()));
			m_runsNeededLabel.text = text;
			return;
		}
		NGUITools.SetActive(m_contentProgressParent, false);
		NGUITools.SetActive(m_contentResultParent, true);
		if (s_eDialogMode == DialogMode.Success)
		{
			NGUITools.SetActive(m_congratsObject, true);
			NGUITools.SetActive(m_badLuckObject, false);
			m_resultsInfoLabel.text = LanguageStrings.First.GetString("GOOGLE_EVENT_SUCCESS_BODY");
		}
		else
		{
			NGUITools.SetActive(m_congratsObject, false);
			NGUITools.SetActive(m_badLuckObject, true);
			m_resultsInfoLabel.text = LanguageStrings.First.GetString("GOOGLE_EVENT_FAIL_BODY");
		}
	}

	private void Update()
	{
		if (s_eDialogMode == DialogMode.Progress)
		{
			TimeSpan timeRemaining = GoogleSpecialEvent.GetTimeRemaining();
			int num = Mathf.CeilToInt((float)timeRemaining.TotalDays);
			if (timeRemaining.TotalHours > 24.0)
			{
				m_timeRemainingLabel.text = string.Format(LanguageStrings.First.GetString("GC_DAYS_LEFT"), num);
			}
			else
			{
				m_timeRemainingLabel.text = LanguageStrings.First.GetString("GC_FINAL_DAY");
			}
		}
	}
}
