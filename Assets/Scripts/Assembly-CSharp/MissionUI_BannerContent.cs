using System;
using System.Collections;
using UnityEngine;

public class MissionUI_BannerContent : MonoBehaviour
{
	[Flags]
	private enum State
	{
		Active = 1,
		Swapping = 2,
		TimeOut = 4,
		ClearContent = 8
	}

	private enum Blockers
	{
		Switching = 0,
		TimeOut = 1
	}

	private static int s_swapCount;

	private int m_currentGroup;

	private State m_state;

	private StoreContent.StoreEntry m_pendingPurchase;

	private GuiButtonBlocker[] m_buttonBlockers;

	[SerializeField]
	private UISlider m_progressionSlider;

	[SerializeField]
	private MeshFilter m_mesh;

	[SerializeField]
	private UILabel m_description;

	[SerializeField]
	private UILabel m_missionTitle;

	[SerializeField]
	private UILabel m_cost;

	[SerializeField]
	private UIButton m_skipButton;

	[SerializeField]
	private GameObject m_triggerCompleteStatic;

	[SerializeField]
	private GameObject m_triggerCompleteActive;

	[SerializeField]
	private GameObject m_triggerSwapActive;

	[SerializeField]
	private GameObject m_triggerInProgressStatic;

	[SerializeField]
	private GameObject m_triggerAllDoneStatic;

	[SerializeField]
	private GameObject m_triggerAllDoneActive;

	[SerializeField]
	private float m_sliderCompleteSpeed = 0.5f;

	[SerializeField]
	private GameObject m_skipTextGroup;

	[SerializeField]
	private GameObject m_timeOutGroup;

	[SerializeField]
	private AudioClip m_clipSwapBanner;

	public void PopulateOnStart(int missionGroup, bool inRun)
	{
		m_currentGroup = missionGroup;
		CacheContent();
		if (MissionTracker.AllMissionsComplete())
		{
			m_triggerAllDoneStatic.SendMessage("OnClick", SendMessageOptions.RequireReceiver);
		}
		else
		{
			SetInitialState();
			PopulateBanner();
		}
		UpdateTimeOut(true);
		m_state |= State.Active;
	}

	public void TransitionToMission()
	{
		if (MissionTracker.AllMissionsComplete())
		{
			m_triggerAllDoneActive.SendMessage("OnClick", SendMessageOptions.RequireReceiver);
			m_state |= State.ClearContent;
		}
		else
		{
			m_triggerSwapActive.SendMessage("OnClick", SendMessageOptions.RequireReceiver);
			m_state &= ~State.ClearContent;
		}
		m_state |= State.Swapping;
		Audio.PlayClip(m_clipSwapBanner, false);
	}

	private void Start()
	{
		EventDispatch.RegisterInterest("OnStorePurchaseCompleted", this);
		EventDispatch.RegisterInterest("OnMissionComplete", this);
	}

	private void Update()
	{
		if ((m_state & State.Active) == State.Active)
		{
			UpdateTimeOut(false);
			if ((m_state & State.Swapping) == State.Swapping)
			{
				UpdateSwap();
			}
		}
	}

	private void OnDisable()
	{
		m_state &= ~State.Active;
		s_swapCount = 0;
	}

	private void CacheContent()
	{
		m_buttonBlockers = GetComponentsInChildren<GuiButtonBlocker>();
	}

	private void SetInitialState()
	{
		MissionTracker.Mission thisMission = MissionTracker.GetActiveMission(m_currentGroup);
		float missionProgress = MissionUtils.GetMissionProgress(ref thisMission);
		if (missionProgress == 1f)
		{
			m_triggerCompleteStatic.SendMessage("OnClick", SendMessageOptions.RequireReceiver);
			m_buttonBlockers[0].Blocked = true;
		}
		else
		{
			m_triggerInProgressStatic.SendMessage("OnClick", SendMessageOptions.RequireReceiver);
			m_buttonBlockers[0].Blocked = false;
		}
	}

	private void PopulateBanner()
	{
		MissionTracker.Mission thisMission = MissionTracker.GetActiveMission(m_currentGroup);
		MissionAssets.Assets missionAssets = MissionTracker.GetMissionAssets(m_currentGroup);
		m_mesh.mesh = missionAssets.m_mesh;
		switch (m_currentGroup)
		{
		case 0:
			m_missionTitle.text = LanguageStrings.First.GetString("MISSION_ONE");
			break;
		case 1:
			m_missionTitle.text = LanguageStrings.First.GetString("MISSION_TWO");
			break;
		case 2:
			m_missionTitle.text = LanguageStrings.First.GetString("MISSION_THREE");
			break;
		}
		m_cost.text = missionAssets.m_cost.ToString();
		float missionProgress = MissionUtils.GetMissionProgress(ref thisMission);
		m_progressionSlider.value = missionProgress;
		MissionTracker.GetMissionDescription(m_currentGroup, m_description);
	}

	private void ClearBanner()
	{
		m_mesh.mesh = null;
	}

	private IEnumerator UpdateProgressBar()
	{
		float initialProgress = m_progressionSlider.value;
		float currentTime = 0f;
		do
		{
			currentTime += 1f / m_sliderCompleteSpeed * IndependantTimeDelta.Delta;
			float currentProgress = EaseInOutExpo(initialProgress, 1f, currentTime);
			if (currentProgress > 1f)
			{
				currentProgress = 1f;
			}
			m_progressionSlider.value = currentProgress;
			yield return null;
		}
		while (currentTime < 1f);
	}

	private void UpdateTimeOut(bool force)
	{
		bool flag = StoreUtils.IsStoreActive() && !m_buttonBlockers[0].Blocked;
		bool flag2 = (m_state & State.TimeOut) == State.TimeOut;
		if (flag2 != flag || force)
		{
			if (flag)
			{
				m_skipTextGroup.SetActive(false);
				m_timeOutGroup.SetActive(true);
				m_state |= State.TimeOut;
				m_buttonBlockers[1].Blocked = true;
			}
			else
			{
				m_skipTextGroup.SetActive(true);
				m_timeOutGroup.SetActive(false);
				m_state &= ~State.TimeOut;
				m_buttonBlockers[1].Blocked = false;
			}
		}
	}

	private void UpdateSwap()
	{
		float y = base.transform.localScale.y;
		if (y <= 0.2f)
		{
			if ((m_state & State.ClearContent) == State.ClearContent)
			{
				ClearBanner();
			}
			else
			{
				PopulateBanner();
				UpdateTimeOut(true);
				m_buttonBlockers[0].Blocked = false;
			}
			m_state &= ~State.Swapping;
		}
	}

	private float EaseInOutExpo(float start, float end, float value)
	{
		value /= 0.5f;
		end -= start;
		if (value < 1f)
		{
			return end / 2f * Mathf.Pow(2f, 10f * (value - 1f)) + start;
		}
		value -= 1f;
		return end / 2f * (0f - Mathf.Pow(2f, -10f * value) + 2f) + start;
	}

	private void Trigger_SkipMission()
	{
		MissionTracker.Mission thisMission = MissionTracker.GetActiveMission(m_currentGroup);
		float missionProgress = MissionUtils.GetMissionProgress(ref thisMission);
		if (missionProgress != 1f && !StoreUtils.IsStoreActive())
		{
			m_pendingPurchase = StoreContent.GetStoreEntry("Skip Mission", StoreContent.Identifiers.Name);
			MissionAssets.Assets missionAssets = MissionTracker.GetMissionAssets(m_currentGroup);
			m_pendingPurchase.m_cost.m_baseCost[0] = missionAssets.m_cost;
			m_pendingPurchase.m_cost.m_playerCost[0] = missionAssets.m_cost;
			StorePurchases.RequestPurchase("Skip Mission", StorePurchases.LowCurrencyResponse.PurchaseCurrencyAndItem);
		}
	}

	private void Trigger_SwapOutFinished()
	{
		s_swapCount++;
		if (s_swapCount == 3)
		{
			MissionTracker.Track(true);
			s_swapCount = 0;
		}
	}

	private void Event_OnMissionComplete(int missionGroup, bool setComplete)
	{
		if ((m_state & State.Active) == State.Active && m_currentGroup == missionGroup)
		{
			StartCoroutine(UpdateProgressBar());
			m_triggerCompleteActive.SendMessage("OnClick", SendMessageOptions.RequireReceiver);
			m_buttonBlockers[0].Blocked = true;
		}
	}

	private void Event_OnStorePurchaseCompleted(StoreContent.StoreEntry thisEntry, StorePurchases.Result result)
	{
		if (m_pendingPurchase == thisEntry && thisEntry != null)
		{
			if (result == StorePurchases.Result.Success)
			{
				MissionTracker.CompleteActiveMission(m_currentGroup);
			}
			m_pendingPurchase = null;
		}
	}
}
