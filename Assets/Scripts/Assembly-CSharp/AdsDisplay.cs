using System;
using System.Globalization;
using UnityEngine;

public class AdsDisplay : MonoBehaviour
{
	public enum Regions
	{
		MainMenu = 0,
		ResultScreen = 1
	}

	[Flags]
	private enum State
	{
		Active = 1,
		Initialised = 2,
		Disabled = 4
	}

	private const string AdsPeriodStartProperty = "AdsPeriodStart";

	private const string AdsInPerdiodProperty = "AdsInPeriod";

	private const string AdsCountMMProperty = "AdsCountMM";

	private const string AdsCountResultProperty = "AdsCountResult";

	private State m_state;

	[SerializeField]
	private Regions m_region;

	[SerializeField]
	private bool m_countsForLimit = true;

	private static DateTime s_periodStart;

	private static int s_asdInPeriod;

	private static int s_mmCount = 1;

	private static int s_resultCount = 1;

	public bool Active
	{
		get
		{
			return (m_state & State.Active) == State.Active;
		}
	}

	public bool Disabled
	{
		get
		{
			return (m_state & State.Disabled) == State.Disabled;
		}
		set
		{
			if (value)
			{
				m_state |= State.Disabled;
			}
			else
			{
				m_state &= ~State.Disabled;
			}
		}
	}

	public void Visit()
	{
		if (!Disabled)
		{
			InitialiseRegion();
			bool flag = !PaidUser.Paid && !PaidUser.RemovedAds;
			flag &= AdvertStates.AdsEnabled;
			if (!AdvertStates.AdsOnFirstInteraction && PlayerStats.GetStat(PlayerStats.StatNames.NumberOfSessions_Total) < 2 && PlayerStats.GetStat(PlayerStats.StatNames.NumberOfRuns_Total) < AdvertStates.RunsBeforeAds)
			{
				flag = false;
			}
			if (flag && ShowAdd(m_region))
			{
				m_state |= State.Active;
				OnRegionActivated();
			}
		}
	}

	public void Leave()
	{
		if ((m_state & State.Active) == State.Active)
		{
			EndRegion(m_region);
			m_state &= ~State.Active;
			OnRegionDectivated();
		}
	}

	private void OnEnable()
	{
		Visit();
	}

	private void OnDisable()
	{
		Leave();
	}

	protected virtual void OnRegionActivated()
	{
	}

	protected virtual void OnRegionDectivated()
	{
	}

	private void InitialiseRegion()
	{
		if ((m_state & State.Initialised) != State.Initialised)
		{
			EventDispatch.RegisterInterest("OnGameDataSaveRequest", this);
			EventDispatch.RegisterInterest("OnGameDataLoaded", this);
			m_state |= State.Initialised;
		}
	}

	private bool ShowAdd(Regions region)
	{
		switch (region)
		{
		case Regions.MainMenu:
			if (AdvertStates.AdsMainMenuRate == 0)
			{
				return false;
			}
			if (s_mmCount % AdvertStates.AdsMainMenuRate == 0)
			{
				BeginRegion(region, m_countsForLimit);
			}
			s_mmCount++;
			return true;
		case Regions.ResultScreen:
			if (AdvertStates.AdsResultScreenRate == 0)
			{
				return false;
			}
			if (s_resultCount % AdvertStates.AdsResultScreenRate == 0)
			{
				BeginRegion(region, m_countsForLimit);
			}
			s_resultCount++;
			return true;
		default:
			return false;
		}
	}

	private void BeginRegion(Regions region, bool countForLimit)
	{
		string placementID = "AdSpace" + region;
		SLAds.ShowIntersitialAd(placementID);
		if (countForLimit)
		{
			s_asdInPeriod++;
		}
	}

	private void EndRegion(Regions region)
	{
	}

	private void Event_OnGameDataSaveRequest()
	{
		CultureInfo cultureInfo = new CultureInfo("en-US");
		PropertyStore.Store("AdsPeriodStart", s_periodStart.ToString(cultureInfo.DateTimeFormat));
		PropertyStore.Store("AdsInPeriod", s_asdInPeriod);
		PropertyStore.Store("AdsCountMM", s_mmCount);
		PropertyStore.Store("AdsCountResult", s_resultCount);
	}

	private void Event_OnGameDataLoaded(ActiveProperties activeProperties)
	{
		CultureInfo provider = new CultureInfo("en-US");
		if (!DateTime.TryParse(activeProperties.GetString("AdsPeriodStart"), provider, DateTimeStyles.None, out s_periodStart))
		{
			s_periodStart = DCTime.GetCurrentTime().AddDays(-1.0);
		}
		s_asdInPeriod = activeProperties.GetInt("AdsInPeriod");
		s_mmCount = activeProperties.GetInt("AdsCountMM");
		s_resultCount = activeProperties.GetInt("AdsCountResult");
	}
}
