using System;
using UnityEngine;

public class ReviveMenuReviveAd : MonoBehaviour
{
	[Flags]
	private enum State
	{
		Active = 1,
		WaitingForConfirmation = 2,
		UsedRevive = 4,
		WaitingToTrigger = 8
	}

	private readonly string FreeReviveEvent = StoreContent.FormatIdentifier("VideosForRevives");

	private State m_state;

	public bool Used
	{
		get
		{
			return (m_state & State.UsedRevive) == State.UsedRevive;
		}
	}

	public void Active(bool active, bool endingProcess)
	{
		if (active)
		{
			m_state |= State.Active;
			m_state &= ~State.WaitingForConfirmation;
			return;
		}
		m_state &= ~State.Active;
		if (endingProcess)
		{
			m_state &= ~State.WaitingForConfirmation;
		}
	}

	private void Start()
	{
		EventDispatch.RegisterInterest("OnEventAwarded", this, EventDispatch.Priority.Highest);
		EventDispatch.RegisterInterest("OnNewGameStarted", this);
	}

	private void Trigger_StartRevive()
	{
		if ((m_state & State.Active) == State.Active)
		{
			m_state |= State.WaitingForConfirmation;
			m_state |= State.WaitingToTrigger;
			EventDispatch.GenerateEvent("VideoReviveRequested", false);
		}
	}

	private void Trigger_TransitionFinished()
	{
		Trigger_TransitionFinished(null);
	}

	private void Trigger_TransitionFinished(object[] par)
	{
		if ((m_state & State.WaitingToTrigger) == 0)
		{
			return;
		}
		m_state &= ~State.WaitingToTrigger;
		if (SLAds.IsVideoAvailable())
		{
			if (!SLAds.IsVideoReady("VIDEO_REWARD_AD"))
			{
				SLAds.PrepareVideoAd("VIDEO_REWARD_AD");
				EventDispatch.GenerateEvent("VideoReviveRequested", true);
			}
		}
		else
		{
			EventDispatch.GenerateEvent("VideoReviveRequested", true);
		}
		StorePurchases.RequestPurchase(FreeReviveEvent, StorePurchases.LowCurrencyResponse.Fail);
	}

	private void Event_OnEventAwarded(StoreContent.StoreEntry awardedEntry)
	{
		if ((m_state & State.WaitingForConfirmation) == State.WaitingForConfirmation && !(awardedEntry.m_identifier != FreeReviveEvent))
		{
			m_state &= ~State.WaitingForConfirmation;
			GameAnalytics.ContinueUsed("VideoWatched");
			EventDispatch.GenerateEvent("OnContinueGameOk", true);
			m_state |= State.UsedRevive;
		}
	}

	private void Event_OnNewGameStarted()
	{
		m_state &= ~State.UsedRevive;
	}
}
