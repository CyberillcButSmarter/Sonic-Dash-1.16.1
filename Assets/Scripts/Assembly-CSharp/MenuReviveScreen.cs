using System;
using System.Collections;
using UnityEngine;

public class MenuReviveScreen : MonoBehaviour
{
	[Flags]
	private enum Shown
	{
		Friends = 1,
		Free = 2,
		Paid = 4,
		Ads = 8,
		RingTotal = 0x10
	}

	[Flags]
	private enum State
	{
		AdShown = 1,
		FreeReviveUsed = 2,
		Active = 4,
		Delaying = 8,
		Finished = 0x10,
		ShowingAdRevive = 0x20,
		UsingRSR = 0x40,
		CountdownPaused = 0x80
	}

	private static MenuReviveScreen s_instance;

	private Shown m_shown;

	private State m_state;

	private float m_timeOutTimer;

	private float m_showDelayTimer;

	private bool m_OnContinueOKReceived;

	private bool m_OnContinueOKFree;

	private bool m_VideoReviveRequested;

	[SerializeField]
	private float m_defaultTimeOut = 3f;

	[SerializeField]
	private float m_videoAdTimeOut = 5f;

	[SerializeField]
	private GuiTrigger m_purchasePage;

	[SerializeField]
	private GuiTrigger m_purchasePageSimple;

	[SerializeField]
	private ReviveMenuReviveAd m_reviveAd;

	[SerializeField]
	private ReviveMenuReviveButton m_reviveFree;

	[SerializeField]
	private ReviveMenuReviveButton m_revivePaid;

	[SerializeField]
	private ReviveMenuReviveButton m_reviveRSR;

	[SerializeField]
	private GameObject m_showFriendScore;

	[SerializeField]
	private GameObject m_showReviveAd;

	[SerializeField]
	private GameObject m_showReviveFree;

	[SerializeField]
	private GameObject m_showRevivePaid;

	[SerializeField]
	private GameObject m_showReviveRSR;

	[SerializeField]
	private GameObject m_showRingTotals;

	public static float TimeOut
	{
		get
		{
			if ((s_instance.m_state & State.ShowingAdRevive) == State.ShowingAdRevive)
			{
				return s_instance.m_videoAdTimeOut;
			}
			return s_instance.m_defaultTimeOut;
		}
	}

	public static GuiTrigger PurchasePage
	{
		get
		{
			return (ABTesting.GetTestValueAsInt(ABTesting.Tests.REVIVE_SimpleStore) == 0) ? s_instance.m_purchasePage : s_instance.m_purchasePageSimple;
		}
	}

	public static float CountdownTime
	{
		get
		{
			return s_instance.m_timeOutTimer;
		}
	}

	public static bool Valid
	{
		get
		{
			return s_instance != null;
		}
	}

	public static int RevivesRequired { get; set; }

	public static bool VideoReviveUsed
	{
		get
		{
			return s_instance.m_reviveAd.Used;
		}
	}

	private void Start()
	{
		s_instance = this;
		EventDispatch.RegisterInterest("OnNewGameStarted", this);
		EventDispatch.RegisterInterest("OnContinueGameOk", this, EventDispatch.Priority.Highest);
		EventDispatch.RegisterInterest("OnContinueGameCancel", this, EventDispatch.Priority.Highest);
		EventDispatch.RegisterInterest("OnContinuePurchaseRequired", this, EventDispatch.Priority.Highest);
		EventDispatch.RegisterInterest("3rdPartyInactive", this, EventDispatch.Priority.High);
		EventDispatch.RegisterInterest("VideoReviveRequested", this);
		EventDispatch.RegisterInterest("OnStorePurchaseStarted", this);
		EventDispatch.RegisterInterest("OnStorePurchaseCompleted", this);
		if (ABTesting.GetTestValueAsInt(ABTesting.Tests.REVIVE_RSRRevive) != 0)
		{
			m_state |= State.UsingRSR;
		}
		UpdateABTestingReady();
	}

	private void Update()
	{
		if ((m_state & State.Delaying) == State.Delaying)
		{
			UpdateShowDelay();
		}
		else if (ShouldEnableReviveTimer())
		{
			UpdateTimeOut();
		}
	}

	private void OnEnable()
	{
		StartCoroutine(StartPendingActivation());
	}

	private void UpdateShowDelay()
	{
		m_showDelayTimer -= IndependantTimeDelta.Delta;
		if (m_showDelayTimer <= 0f)
		{
			m_state &= ~State.Delaying;
			ShowScreenContent();
		}
	}

	private void UpdateTimeOut()
	{
		if ((m_state & State.Active) == State.Active && (m_state & State.CountdownPaused) != State.CountdownPaused)
		{
			m_timeOutTimer -= IndependantTimeDelta.Delta;
			if (m_timeOutTimer < 0f)
			{
				m_timeOutTimer = 0f;
			}
			if (m_timeOutTimer <= 0f)
			{
				GameAnalytics.ContinueCancelled(GameAnalytics.CancelContinueReasons.Timeout);
				EventDispatch.GenerateEvent("OnContinueGameCancel");
			}
		}
	}

	private void ShowScreenContent()
	{
		Debug.Log("ShowScreenContent - MenuReviveScreen");
		if ((m_state & State.Finished) != State.Finished)
		{
			bool show = ShouldFriendScoreShow();
			ToggleComponentTrigger(show, Shown.Friends, m_showFriendScore);
			bool flag = ShouldFreeReviveShow();
			ToggleComponentTrigger(flag, Shown.Free, m_showReviveFree);
			if (flag)
			{
				m_reviveFree.Active(true);
			}
			bool flag2 = ShouldPaidReviveShow();
			ToggleComponentTrigger(flag2, Shown.Paid, GetReviveScreenTrigger());
			if (flag2)
			{
				GetReviveScreenButton().Active(true);
			}
			bool flag3 = ShouldAdReviveShow(flag);
			ToggleComponentTrigger(flag3, Shown.Ads, m_showReviveAd);
			if (flag3)
			{
				m_reviveAd.Active(true, false);
			}
			bool show2 = ShouldRingTotalsBeShown();
			ToggleComponentTrigger(show2, Shown.RingTotal, m_showRingTotals);
			m_state |= State.Active;
			if (flag3)
			{
				m_state |= State.ShowingAdRevive;
			}
			else
			{
				m_state &= ~State.ShowingAdRevive;
			}
			m_timeOutTimer = TimeOut;
			if (!PaidUser.RemovedAds)
			{
				SLAds.ShowBannerAd(1);
			}
		}
	}

	private void HideScreenContent(bool endingProcess)
	{
		if (!PaidUser.RemovedAds)
		{
			SLAds.CloseBannerAd();
		}
		if ((m_shown & Shown.Friends) == Shown.Friends)
		{
			ToggleComponentTrigger(false, Shown.Friends, m_showFriendScore);
		}
		if ((m_shown & Shown.Free) == Shown.Free)
		{
			ToggleComponentTrigger(false, Shown.Free, m_showReviveFree);
		}
		if ((m_shown & Shown.Paid) == Shown.Paid)
		{
			ToggleComponentTrigger(false, Shown.Paid, GetReviveScreenTrigger());
		}
		if ((m_shown & Shown.Ads) == Shown.Ads)
		{
			ToggleComponentTrigger(false, Shown.Ads, m_showReviveAd);
		}
		if ((m_shown & Shown.RingTotal) == Shown.RingTotal)
		{
			ToggleComponentTrigger(false, Shown.RingTotal, m_showRingTotals);
		}
		m_reviveAd.Active(false, endingProcess);
		m_reviveFree.Active(false);
		GetReviveScreenButton().Active(false);
		m_state &= ~State.Active;
		if (endingProcess)
		{
			m_state |= State.Finished;
		}
	}

	private void ToggleComponentTrigger(bool show, Shown type, GameObject trigger)
	{
		bool flag = (m_shown & type) == type;
		if (flag != show)
		{
			trigger.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			if (show)
			{
				m_shown |= type;
			}
			else
			{
				m_shown &= ~type;
			}
		}
	}

	private bool ShouldFriendScoreShow()
	{
		Leaderboards.Entry entry = HudContent_FriendDisplay.CurrentFriend();
		return entry != null;
	}

	private bool ShouldFreeReviveShow()
	{
		bool flag = PowerUpsInventory.GetPowerUpCount(PowerUps.Type.FreeRevive) > 0;
		bool flag2 = (m_state & State.FreeReviveUsed) == State.FreeReviveUsed;
		return flag && !flag2;
	}

	private bool ShouldPaidReviveShow()
	{
		bool flag = PowerUpsInventory.GetPowerUpCount(PowerUps.Type.FreeRevive) > 0;
		bool flag2 = (m_state & State.FreeReviveUsed) == State.FreeReviveUsed;
		return !flag || flag2;
	}

	private bool ShouldAdReviveShow(bool freeReviveShown)
	{
		if (FeatureSupport.IsLowEndDevice())
		{
			return false;
		}
		if (freeReviveShown)
		{
			return false;
		}
		if ((m_state & State.AdShown) == State.AdShown)
		{
			return false;
		}
		m_state |= State.AdShown;
		if (ABTesting.GetTestValueAsInt(ABTesting.Tests.ADS_Revive) == 0)
		{
			return false;
		}
		if (!SLAds.IsVideoAvailable())
		{
			return false;
		}
		if (!SLAds.IsVideoReady("VIDEO_REWARD_AD"))
		{
			SLAds.PrepareVideoAd("VIDEO_REWARD_AD");
			return false;
		}
		if (StoreUtils.IsStoreActive())
		{
			return false;
		}
		return true;
	}

	private bool ShouldRingTotalsBeShown()
	{
		return ABTesting.GetTestValueAsInt(ABTesting.Tests.REVIVE_RSRRevive) != 0;
	}

	public static bool ShouldEnableReviveTimer()
	{
		bool flag = true;
		if (ABTesting.GetTestValueAsInt(ABTesting.Tests.REVIVE_TimerEnabled) == 1)
		{
			return true;
		}
		return false;
	}

	private void UpdateABTestingReady()
	{
		if (ABTesting.Ready)
		{
			float testValueAsFloat = ABTesting.GetTestValueAsFloat(ABTesting.Tests.PLAY_ReviveTimeNormal);
			float testValueAsFloat2 = ABTesting.GetTestValueAsFloat(ABTesting.Tests.PLAY_ReviveTimeAds);
			if (testValueAsFloat != -1f)
			{
				m_defaultTimeOut = testValueAsFloat;
			}
			if (testValueAsFloat2 != -1f)
			{
				m_videoAdTimeOut = testValueAsFloat2;
			}
		}
	}

	private IEnumerator StartPendingActivation()
	{
		yield return null;
		m_state &= ~State.Delaying;
		m_state &= ~State.Finished;
		m_VideoReviveRequested = false;
		m_OnContinueOKReceived = false;
		ShowScreenContent();
	}

	private GameObject GetReviveScreenTrigger()
	{
		return ((m_state & State.UsingRSR) == 0) ? m_showRevivePaid : m_showReviveRSR;
	}

	private ReviveMenuReviveButton GetReviveScreenButton()
	{
		return ((m_state & State.UsingRSR) == 0) ? m_revivePaid : m_reviveRSR;
	}

	private void Event_OnNewGameStarted()
	{
		m_state &= ~State.AdShown;
		m_state &= ~State.FreeReviveUsed;
	}

	private void Event_OnContinueGameCancel()
	{
		HideScreenContent(true);
	}

	private void Event_OnContinueGameOk(bool freeRevive)
	{
		if (m_VideoReviveRequested)
		{
			m_OnContinueOKReceived = true;
			m_OnContinueOKFree = freeRevive;
			m_VideoReviveRequested = false;
			return;
		}
		HideScreenContent(true);
		if (freeRevive)
		{
			m_state |= State.FreeReviveUsed;
		}
	}

	private void Event_OnContinuePurchaseRequired()
	{
		MenuStack.MoveToPage(PurchasePage, true);
		HideScreenContent(true);
	}

	private void Event_3rdPartyInactive()
	{
		if (m_OnContinueOKReceived)
		{
			m_OnContinueOKReceived = false;
			HideScreenContent(true);
			if (m_OnContinueOKFree)
			{
				m_state |= State.FreeReviveUsed;
			}
		}
		if (base.gameObject.activeInHierarchy)
		{
			m_showDelayTimer = 2f;
			m_state |= State.Delaying;
		}
	}

	private void Event_VideoReviveRequested(bool failed)
	{
		if (!failed)
		{
			HideScreenContent(false);
			m_VideoReviveRequested = true;
		}
		else
		{
			ShowScreenContent();
		}
	}

	private void Event_OnStorePurchaseStarted(StoreContent.StoreEntry entry)
	{
		m_state |= State.CountdownPaused;
	}

	private void Event_OnStorePurchaseCompleted(StoreContent.StoreEntry thisEntry, StorePurchases.Result result)
	{
		m_state &= ~State.CountdownPaused;
	}
}
