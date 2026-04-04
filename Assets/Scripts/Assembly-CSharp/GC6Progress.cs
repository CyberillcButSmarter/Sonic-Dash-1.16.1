using System;
using System.Collections;
using UnityEngine;

public class GC6Progress : MonoBehaviour
{
	[Flags]
	private enum State
	{
		Ready = 1
	}

	public const int GC6TiersAmount = 4;

	private const string PropertyGC6PageVisitedFromResult = "GC6PageVisitedFromResult";

	private const string PropertyGC6ActualPointsLocal = "GC6ActualPointsLocal";

	private const string PropertyGC6PreviousPointsLocal = "GC6PreviousPointsLocal";

	private const string PropertyGC6PreviousPointsGlobal = "GC6PreviousPointsGlobal";

	private const string PropertyGC6ActualPointsContributed = "GC6ActualPointsContributed";

	private const string PropertyGC6PreviousPointsContributed = "GC6PreviousPointsContributed";

	private const FileDownloader.Files FileLocation = FileDownloader.Files.GC6;

	private static GC6Progress instance;

	private int m_actualPointsGlobal;

	private int m_actualPointsLocal;

	private int m_actualPointsContributed;

	private int m_lastCheckPointsGlobal;

	private int m_lastCheckPointsLocal;

	private int m_lastCheckPointsContributed;

	private int m_collectedThisRun;

	[SerializeField]
	private int m_totalPointsNeededGlobal = 100000;

	[SerializeField]
	private int m_totalPointsNeededLocal = 10000;

	[SerializeField]
	private string[] m_tierRewards = new string[4];

	[SerializeField]
	private int[] m_tierRewardsAmounts = new int[4];

	private static State s_state;

	public static int ActualPointsLocal
	{
		get
		{
			return instance.m_actualPointsLocal;
		}
	}

	public static int PreviousPointsLocal
	{
		get
		{
			return instance.m_lastCheckPointsLocal;
		}
	}

	public static int ActualPointsGlobal
	{
		get
		{
			return instance.m_actualPointsGlobal;
		}
	}

	public static int PreviousPointsGlobal
	{
		get
		{
			return instance.m_lastCheckPointsGlobal;
		}
	}

	public static int ActualPointsContributed
	{
		get
		{
			return instance.m_actualPointsContributed;
		}
	}

	public static int PreviousPointsContributed
	{
		get
		{
			return instance.m_lastCheckPointsContributed;
		}
	}

	public static string[] TierRewards
	{
		get
		{
			return instance.m_tierRewards;
		}
	}

	public static int CurrentCollectedThisRun
	{
		get
		{
			return instance.m_collectedThisRun;
		}
	}

	public static int GetLocalTierSize
	{
		get
		{
			return instance.m_totalPointsNeededLocal / 4;
		}
	}

	public static bool PageVisitedFromResult { get; set; }

	public static bool Ready
	{
		get
		{
			return (s_state & State.Ready) == State.Ready;
		}
	}

	public static bool ChallengeFullycompleted()
	{
		bool flag = GetGC6LocalTierCurrent() == 4 && GetGC6GlobalTierCurrent() == 4;
		bool flag2 = IsRewardDue();
		return flag && !flag2;
	}

	public static void Restart()
	{
		s_state = (State)0;
		instance.StartCoroutine(instance.DownloadServerFile());
	}

	public static int GetGC6GlobalTierCurrent()
	{
		for (int num = 4; num >= 0; num--)
		{
			if ((float)instance.m_actualPointsGlobal >= (float)instance.m_totalPointsNeededGlobal * ((float)num / 4f))
			{
				return num;
			}
		}
		return 0;
	}

	public static int GetGC6GlobalTierLastCheck()
	{
		for (int num = 4; num >= 0; num--)
		{
			if ((float)instance.m_lastCheckPointsGlobal >= (float)instance.m_totalPointsNeededGlobal * ((float)num / 4f))
			{
				return num;
			}
		}
		return 0;
	}

	public static int GetGC6LocalTierCurrent()
	{
		for (int num = 4; num >= 0; num--)
		{
			if ((float)instance.m_actualPointsLocal >= (float)instance.m_totalPointsNeededLocal * ((float)num / 4f))
			{
				return num;
			}
		}
		return 0;
	}

	public static int GetGC6LocalTierLastCheck()
	{
		for (int num = 4; num >= 0; num--)
		{
			if ((float)instance.m_lastCheckPointsLocal >= (float)instance.m_totalPointsNeededLocal * ((float)num / 4f))
			{
				return num;
			}
		}
		return 0;
	}

	public static float CalculateLocalPercent(int localPoints)
	{
		return (float)localPoints / (float)instance.m_totalPointsNeededLocal;
	}

	public static float CalculateGlobalPercent(int globalPoints)
	{
		return (float)globalPoints / (float)instance.m_totalPointsNeededGlobal;
	}

	public static void GCPageVisited()
	{
		instance.m_lastCheckPointsGlobal = instance.m_actualPointsGlobal;
		instance.m_lastCheckPointsLocal = instance.m_actualPointsLocal;
		instance.m_lastCheckPointsContributed = instance.m_actualPointsContributed;
		PropertyStore.Save();
	}

	public static bool IsRewardDue()
	{
		int gC6LocalTierCurrent = GetGC6LocalTierCurrent();
		int gC6LocalTierLastCheck = GetGC6LocalTierLastCheck();
		int gC6GlobalTierCurrent = GetGC6GlobalTierCurrent();
		int gC6GlobalTierLastCheck = GetGC6GlobalTierLastCheck();
		return (gC6LocalTierCurrent > gC6LocalTierLastCheck && gC6GlobalTierCurrent >= gC6LocalTierCurrent) || (gC6GlobalTierCurrent > gC6GlobalTierLastCheck && gC6LocalTierCurrent > gC6GlobalTierLastCheck);
	}

	public static string GetRewardDue(out int amount, out bool finalReward)
	{
		if (!IsRewardDue())
		{
			amount = 0;
			finalReward = false;
			return null;
		}
		int num = GetGC6LocalTierCurrent() - 1;
		amount = instance.m_tierRewardsAmounts[num];
		finalReward = num + 1 == 4;
		return instance.m_tierRewards[num];
	}

	public static void GCCollectableCollected()
	{
		instance.m_collectedThisRun++;
	}

	public static int CollectedThisRun()
	{
		return instance.m_collectedThisRun;
	}

	public static void ContributeToChallenge()
	{
		int gC6GlobalTierCurrent = GetGC6GlobalTierCurrent();
		int gC6LocalTierCurrent = GetGC6LocalTierCurrent();
		if (!GCState.IsCurrentChallengeActive())
		{
			return;
		}
		instance.m_actualPointsContributed += instance.m_collectedThisRun;
		if (gC6LocalTierCurrent <= gC6GlobalTierCurrent)
		{
			instance.m_actualPointsLocal += instance.m_collectedThisRun;
			if (instance.m_actualPointsLocal > instance.m_totalPointsNeededLocal / 4 * (gC6LocalTierCurrent + 1))
			{
				instance.m_actualPointsLocal = instance.m_totalPointsNeededLocal / 4 * (gC6LocalTierCurrent + 1);
			}
		}
		GameAnalytics.GC6ChallengePoints(instance.m_collectedThisRun);
		PropertyStore.Save();
	}

	private void Start()
	{
		instance = this;
		EventDispatch.RegisterInterest("OnGameDataLoaded", this);
		EventDispatch.RegisterInterest("OnGameDataSaveRequest", this);
		EventDispatch.RegisterInterest("OnNewGameStarted", this);
	}

	private IEnumerator DownloadServerFile()
	{
		FileDownloader fDownloader = new FileDownloader(FileDownloader.Files.GC6, true);
		yield return fDownloader.Loading;
		if (fDownloader.Error == null && !int.TryParse(fDownloader.Text, out m_actualPointsGlobal))
		{
			m_actualPointsGlobal = 0;
		}
		s_state |= State.Ready;
	}

	private void Event_OnNewGameStarted()
	{
		m_collectedThisRun = 0;
	}

	private void Event_OnGameDataSaveRequest()
	{
		PropertyStore.Store("GC6PageVisitedFromResult", PageVisitedFromResult);
		PropertyStore.Store("GC6ActualPointsLocal", m_actualPointsLocal);
		PropertyStore.Store("GC6PreviousPointsLocal", m_lastCheckPointsLocal);
		PropertyStore.Store("GC6PreviousPointsGlobal", m_lastCheckPointsGlobal);
		PropertyStore.Store("GC6ActualPointsContributed", m_actualPointsContributed);
		PropertyStore.Store("GC6PreviousPointsContributed", m_lastCheckPointsContributed);
	}

	private void Event_OnGameDataLoaded(ActiveProperties activeProperties)
	{
		PageVisitedFromResult = activeProperties.GetBool("GC6PageVisitedFromResult");
		m_actualPointsLocal = activeProperties.GetInt("GC6ActualPointsLocal");
		m_lastCheckPointsLocal = activeProperties.GetInt("GC6PreviousPointsLocal");
		m_actualPointsContributed = activeProperties.GetInt("GC6ActualPointsContributed");
		m_lastCheckPointsContributed = activeProperties.GetInt("GC6PreviousPointsContributed");
		if (m_actualPointsGlobal == 0)
		{
			m_lastCheckPointsGlobal = activeProperties.GetInt("GC6PreviousPointsGlobal");
		}
	}
}
