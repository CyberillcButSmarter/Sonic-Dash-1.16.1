using System;
using System.Collections;
using UnityEngine;

public class GCProgressPageDisplay : MonoBehaviour
{
	private const int NumberOfTiers = 4;

	private const float PercentagePerTier = 0.25f;

	[SerializeField]
	private GameObject[] m_tierDisplayers = new GameObject[4];

	[SerializeField]
	private UISlider[] m_localProgressSliders = new UISlider[4];

	[SerializeField]
	private UISlider[] m_communityProgressSliders = new UISlider[4];

	[SerializeField]
	private UIToggle[] m_localTierCheckboxes = new UIToggle[4];

	[SerializeField]
	private UIToggle[] m_communityTierCheckboxes = new UIToggle[4];

	[SerializeField]
	private GameObject[] m_localPadlocks = new GameObject[4];

	[SerializeField]
	private GameObject[] m_communityPadlocks = new GameObject[4];

	[SerializeField]
	private Animation[] m_tierCompleteAnims = new Animation[4];

	[SerializeField]
	private MeshFilter[] m_prizeMeshes = new MeshFilter[4];

	[SerializeField]
	private float m_sliderCompleteSpeed = 0.25f;

	[SerializeField]
	private GameObject m_timer;

	[SerializeField]
	private UILabel m_timerLabel;

	[SerializeField]
	private GameObject m_timerNotConnection;

	[SerializeField]
	private UILabel m_contributionLabel;

	[SerializeField]
	private float m_contributionCountSpeed = 0.01f;

	[SerializeField]
	private float m_contributionCountMaxTime = 5f;

	[SerializeField]
	private GuiButtonBlocker[] m_blockers;

	[SerializeField]
	private GameObject m_homeButtonIcon;

	[SerializeField]
	private GameObject m_homeButtonSpinner;

	private bool m_rewardingFinalTierPrize;

	private string m_daysLeftStringID = "GC_DAYS_LEFT";

	private string m_finalDayStringID = "GC_FINAL_DAY";

	private void Start()
	{
		for (int i = 0; i < 4; i++)
		{
			m_localProgressSliders[i].numberOfSteps = 10000;
			m_communityProgressSliders[i].numberOfSteps = 10000;
		}
	}

	private void OnEnable()
	{
		StartCoroutine(StartPendingActivation());
	}

	private void OnDisable()
	{
		for (int i = 0; i < 4; i++)
		{
			m_localProgressSliders[i].value = 0f;
			m_localPadlocks[i].SetActive(true);
			m_localTierCheckboxes[i].value = false;
			m_communityPadlocks[i].SetActive(true);
			m_communityTierCheckboxes[i].value = false;
			m_communityProgressSliders[i].value = 0f;
		}
		m_rewardingFinalTierPrize = false;
		GC6Progress.GCPageVisited();
	}

	private IEnumerator StartPendingActivation()
	{
		yield return null;
		BlockButtons(true);
		int lastLocalTier = GC6Progress.GetGC6LocalTierLastCheck();
		int lastComTier = GC6Progress.GetGC6GlobalTierLastCheck();
		m_contributionLabel.text = GC6Progress.PreviousPointsContributed.ToString();
		for (int i = 0; i < lastComTier; i++)
		{
			m_communityProgressSliders[i].value = 1f;
			m_communityTierCheckboxes[i].value = true;
			m_communityPadlocks[i].SetActive(false);
		}
		if (lastComTier < 4)
		{
			m_communityProgressSliders[lastComTier].value = (GC6Progress.CalculateGlobalPercent(GC6Progress.PreviousPointsGlobal) - 0.25f * (float)lastComTier) * 4f;
			m_communityPadlocks[lastComTier].SetActive(false);
		}
		for (int j = 0; j < lastLocalTier; j++)
		{
			m_localProgressSliders[j].value = 1f;
			m_localTierCheckboxes[j].value = true;
			if (!m_communityPadlocks[j].activeInHierarchy)
			{
				m_localPadlocks[j].SetActive(false);
			}
		}
		if (lastLocalTier < 4)
		{
			m_localProgressSliders[lastLocalTier].value = (GC6Progress.CalculateLocalPercent(GC6Progress.PreviousPointsLocal) - 0.25f * (float)lastLocalTier) * 4f;
			if (!m_communityPadlocks[lastLocalTier].activeInHierarchy)
			{
				m_localPadlocks[lastLocalTier].SetActive(false);
			}
		}
		for (int k = 0; k < 4; k++)
		{
			StoreContent.StoreEntry entry = StoreContent.GetStoreEntry(GC6Progress.TierRewards[k], StoreContent.Identifiers.Name);
			m_prizeMeshes[k].mesh = entry.m_mesh;
			if (k == Mathf.Min(lastLocalTier, lastComTier))
			{
				m_tierDisplayers[k].SetActive(true);
			}
			else
			{
				m_tierDisplayers[k].SetActive(false);
			}
		}
		if (DCTimeValidation.TrustedTime)
		{
			m_timer.SetActive(true);
			m_timerNotConnection.SetActive(false);
			DateTime now = DCTime.GetCurrentTime();
			TimeSpan span = GCState.GetChallengeDate(GCState.Challenges.gc6).Subtract(now);
			int daysRoundedUp = Mathf.CeilToInt((float)span.TotalDays);
			if (span.TotalHours > 24.0)
			{
				m_timerLabel.text = string.Format(LanguageStrings.First.GetString(m_daysLeftStringID), daysRoundedUp);
			}
			else
			{
				m_timerLabel.text = LanguageStrings.First.GetString(m_finalDayStringID);
			}
		}
		else
		{
			m_timer.SetActive(false);
			m_timerNotConnection.SetActive(true);
		}
	}

	private void Trigger_UpdateScores()
	{
		StartCoroutine(UpdateScoreVisuals());
		if (GC6Progress.PreviousPointsContributed < GC6Progress.ActualPointsContributed)
		{
			StartCoroutine(CountUpContribution());
		}
	}

	private IEnumerator CountUpContribution()
	{
		int total = GC6Progress.ActualPointsContributed;
		double count = GC6Progress.PreviousPointsContributed;
		int increment = total - (int)count;
		float estimeTime = (float)increment / m_contributionCountSpeed;
		float speedUsed = ((!(estimeTime > m_contributionCountMaxTime)) ? m_contributionCountSpeed : ((float)increment / m_contributionCountMaxTime));
		m_contributionLabel.text = count.ToString();
		do
		{
			count += (double)(IndependantTimeDelta.Delta * speedUsed);
			m_contributionLabel.text = ((int)count).ToString();
			yield return null;
		}
		while (count < (double)total);
	}

	private IEnumerator UpdateScoreVisuals()
	{
		int lastLocalTier = GC6Progress.GetGC6LocalTierLastCheck();
		int lastComTier = GC6Progress.GetGC6GlobalTierLastCheck();
		int curLocalTier = GC6Progress.GetGC6LocalTierCurrent();
		int curComTier = GC6Progress.GetGC6GlobalTierCurrent();
		for (int i = lastComTier; i <= curComTier; i++)
		{
			if (i >= m_communityProgressSliders.Length)
			{
				continue;
			}
			float progress = m_communityProgressSliders[i].value;
			float totalGlobalProgress = GC6Progress.CalculateGlobalPercent(GC6Progress.ActualPointsGlobal);
			float desiredTierProgress = 0f;
			desiredTierProgress = ((!(totalGlobalProgress > ((float)i + 1f) * 0.25f)) ? ((totalGlobalProgress - 0.25f * (float)i) * 4f) : 1f);
			while (progress < desiredTierProgress)
			{
				progress += m_sliderCompleteSpeed * IndependantTimeDelta.Delta;
				progress = Mathf.Min(progress, desiredTierProgress);
				m_communityProgressSliders[i].value = progress;
				yield return null;
			}
			if (m_communityProgressSliders[i].value == 1f)
			{
				m_communityPadlocks[i].SetActive(false);
				m_communityTierCheckboxes[i].value = true;
				if (i < 3)
				{
					m_communityPadlocks[i + 1].SetActive(false);
				}
				yield return null;
			}
		}
		for (int j = lastLocalTier; j <= curLocalTier; j++)
		{
			if (j >= m_localProgressSliders.Length)
			{
				continue;
			}
			float locProgress = m_localProgressSliders[j].value;
			float totalLocalProgress = GC6Progress.CalculateLocalPercent(GC6Progress.ActualPointsLocal);
			float desiredLocTierProgress = 0f;
			desiredLocTierProgress = ((!(totalLocalProgress > ((float)j + 1f) * 0.25f)) ? ((totalLocalProgress - 0.25f * (float)j) * 4f) : 1f);
			while (locProgress < desiredLocTierProgress)
			{
				locProgress += 1f / m_sliderCompleteSpeed * IndependantTimeDelta.Delta;
				locProgress = Mathf.Min(locProgress, desiredLocTierProgress);
				m_localProgressSliders[j].value = locProgress;
				yield return null;
			}
			if (m_localProgressSliders[j].value == 1f)
			{
				m_localPadlocks[j].SetActive(false);
				m_localTierCheckboxes[j].value = true;
				if (j < 3 && !m_communityPadlocks[j + 1].activeInHierarchy)
				{
					m_localPadlocks[j + 1].SetActive(false);
				}
				yield return null;
			}
		}
		if (GC6Progress.IsRewardDue())
		{
			if (curLocalTier < 4)
			{
				m_tierDisplayers[curLocalTier - 1].SetActive(false);
				m_tierDisplayers[curLocalTier].SetActive(true);
				m_localPadlocks[curLocalTier].SetActive(false);
				if (curComTier < 4)
				{
					m_communityPadlocks[curComTier].SetActive(false);
				}
			}
			int animIndex = ((lastLocalTier >= lastComTier) ? lastComTier : lastLocalTier);
			if (animIndex > 3)
			{
				animIndex = 3;
			}
			m_tierCompleteAnims[animIndex].Play();
			yield return new WaitForSeconds(1.5f);
			int amount = 0;
			string reward = GC6Progress.GetRewardDue(out amount, out m_rewardingFinalTierPrize);
			if (m_rewardingFinalTierPrize)
			{
				StorePurchases.RequestReward(reward, amount, 16, StorePurchases.ShowDialog.Yes);
			}
			else
			{
				StorePurchases.RequestReward(reward, amount, 15, StorePurchases.ShowDialog.Yes);
			}
			GC6Progress.GCPageVisited();
		}
		BlockButtons(false);
	}

	private void Trigger_HelpDialogShow()
	{
		Dialog_GCHelp.Display();
	}

	private void BlockButtons(bool block)
	{
		for (int i = 0; i < m_blockers.Length; i++)
		{
			m_blockers[i].Blocked = block;
		}
		m_homeButtonIcon.SetActive(!block);
		m_homeButtonSpinner.SetActive(block);
	}
}
