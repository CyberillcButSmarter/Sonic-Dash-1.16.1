using System;
using UnityEngine;

public class OfferRegion_AppOpen : MonoBehaviour
{
	[Flags]
	private enum State
	{
		Active = 1,
		Valid = 2,
		Initialised = 4,
		RegionRequired = 8
	}

	private State m_state;

	private OfferRegion_Timed m_pushOfferRegion;

	private OfferRegion_Timed m_manualOfferRegion;

	private void Start()
	{
		EventDispatch.RegisterInterest("StartGameState", this);
		EventDispatch.RegisterInterest("OnNewGameAboutToStart", this);
		EventDispatch.RegisterInterest("OnTransitionStarted", this);
		EventDispatch.RegisterInterest("OnGameFinished", this);
		m_pushOfferRegion = base.gameObject.AddComponent<OfferRegion_Timed>();
		m_pushOfferRegion.RegionName = "app_opened_push";
		m_manualOfferRegion = base.gameObject.AddComponent<OfferRegion_Timed>();
		m_manualOfferRegion.RegionName = "app_opened_manual";
	}

	private void Update()
	{
		if (NotificationServices.remoteNotificationCount != 0)
		{
			m_state |= State.RegionRequired;
		}
		if ((m_state & State.Active) == State.Active && (m_state & State.Valid) == State.Valid && (m_state & State.RegionRequired) == State.RegionRequired)
		{
			OfferRegion_Timed offerRegion_Timed = m_manualOfferRegion;
			if (NotificationServices.remoteNotificationCount != 0)
			{
				offerRegion_Timed = m_pushOfferRegion;
			}
			OfferRegion.EndAll();
			offerRegion_Timed.Visit();
			m_state &= ~State.RegionRequired;
			NotificationServices.ClearRemoteNotifications();
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			m_state |= State.RegionRequired;
		}
	}

	private void Event_OnNewGameAboutToStart()
	{
		m_state &= ~State.Valid;
		SLAds.EnableRemotePushNotifications(false);
	}

	private void Event_OnTransitionStarted()
	{
		m_state &= ~State.Valid;
		SLAds.EnableRemotePushNotifications(false);
	}

	private void Event_OnGameFinished()
	{
		m_state |= State.Valid;
		SLAds.EnableRemotePushNotifications(true);
	}

	private void Event_StartGameState(GameState.Mode nextState)
	{
		if (nextState == GameState.Mode.Menu)
		{
			m_state |= State.Active;
			m_state |= State.Valid;
			if ((m_state & State.Initialised) != State.Initialised)
			{
				m_state |= State.RegionRequired;
			}
			m_state |= State.Initialised;
			SLAds.EnableRemotePushNotifications(true);
		}
	}
}
