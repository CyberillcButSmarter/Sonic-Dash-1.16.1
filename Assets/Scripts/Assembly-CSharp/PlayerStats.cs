using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
	public class Stats
	{
		public int[] m_trackedStats = new int[Enum.GetNames(typeof(StatNames)).Length];

		public long[] m_trackedLongStats = new long[Enum.GetNames(typeof(LongStatNames)).Length];

		public float[] m_trackedDistances = new float[Enum.GetNames(typeof(DistanceNames)).Length];

		public DateTime[] m_trackedDates = new DateTime[Enum.GetNames(typeof(DateNames)).Length];
	}

	public enum StatTypes
	{
		Int = 0,
		Long = 1,
		Float = 2,
		Date = 3
	}

	public enum StatNames
	{
		NumberOfRuns_Total = 0,
		NumberOfRuns_Session = 1,
		Enemies_Total = 2,
		Enemies_Session = 3,
		Enemies_Run = 4,
		EnemiesHoming_Total = 5,
		EnemiesRolling_Total = 6,
		EnemiesDiving_Total = 7,
		EnemiesAir_Total = 8,
		EnemyStreaks_Total = 9,
		EnemyStreaks_Run = 10,
		GoldenChoppersKilled_Total = 11,
		RingsCollected_Total = 12,
		RingsCollected_Session = 13,
		RingsCollected_Run = 14,
		RingsBanked_Total = 15,
		RingsBanked_Session = 16,
		RingsBanked_Run = 17,
		RingsHeld = 18,
		RingStreaks_Total = 19,
		RingStreaks_Run = 20,
		RingsPurchased_Total = 21,
		RingsSpent_Total = 22,
		TimesDropRings_Total = 23,
		RingsDropped_Total = 24,
		VasesDestroyed_Total = 25,
		MinesTriped_Total = 26,
		MinesTriped_Run = 27,
		RollsMiddle_Total = 28,
		BridgesRolled_Total = 29,
		BridgesRolled_Run = 30,
		CorkscrewsRan_Total = 31,
		LoopsBoosted_Total = 32,
		DashUses_Total = 33,
		DashUses_Session = 34,
		DashUses_Run = 35,
		TempleVisits_Total = 36,
		GrassVisits_Total = 37,
		BeachVisits_Total = 38,
		PowerMagnetsPicked_Total = 39,
		PowerMagnetsPicked_Session = 40,
		PowerMagnetsPicked_Run = 41,
		PowerShieldPicked_Run = 42,
		PowerupsPicked_Total = 43,
		PowerupsPicked_Session = 44,
		PowerupsPicked_Run = 45,
		MaxedPowerUps_Total = 46,
		Level5PowerUps_Total = 47,
		RevivesUsed_Total = 48,
		RevivesUsed_Session = 49,
		RevivesUsed_Run = 50,
		HeadstartsUsed_Total = 51,
		HeadstartsUsed_Session = 52,
		HeadstartsUsed_Run = 53,
		SuperHeadstartsUsed_Total = 54,
		SuperHeadstartsUsed_Session = 55,
		SuperHeadstartsUsed_Run = 56,
		RingsBanked_Run_Best = 57,
		RingStreaks_Run_Best = 58,
		Enemies_Run_Best = 59,
		EnemyStreaks_Run_Best = 60,
		RegisteredFacebook = 61,
		RingsAsAmy_Total = 62,
		MissionsCompleted_Total = 63,
		TimePlayed_Total = 64,
		TimePlayed_Session = 65,
		TimePlayed_Run = 66,
		MaxMultiplier_Total = 67,
		MaxMultiplier_Session = 68,
		MaxMultiplier_Run = 69,
		StarRingsEarned_Total = 70,
		ShopPurchases_Total = 71,
		ShopPurchases_Session = 72,
		ShopPurchases_Run = 73,
		InAppPurchases_Total = 74,
		InAppPurchases_Session = 75,
		InAppPurchases_Run = 76,
		NumberOfGamingSessions_Total = 77,
		TimesBragged_Total = 78,
		FirstLeaderboardRewarded = 79,
		NumberOfSessions_Total = 80,
		HighScoreRewarded = 81,
		DCsCompleted_Total = 82,
		DCsCompletedConsecutive_Total = 83,
		CrabmeatJumpedOver_Run = 84,
		SpikysJumpedOver_Total = 85,
		PlantPotsJumpedOver_Total = 86,
		TotemsDashedThrough_Total = 87,
		TotemsDashedThrough_Run = 88,
		TimeAirbourne_Total = 89,
		TimeAirbourne_Run = 90,
		SpringsPassed_Run = 91,
		BossBattles_Total = 92,
		BossBattlesEasy_Total = 93,
		BossBattlesHard_Total = 94,
		BoosterSpringSprings_Total = 95,
		RingsAsBlaze_Total = 96,
		BoostersUsed_Total = 97,
		RegisteredGPlus = 98,
		GoldenEnemiesKilledAsSilver_Total = 99,
		RingStreaksBoosterBonusesAsRouge_Total = 100,
		SpringsBoosterBonusesAsRouge_Total = 101,
		GoldenChoppersKilledAsCream_Total = 102,
		BoostersUsedAsCream_Total = 103
	}

	public enum LongStatNames
	{
		Score = 0,
		ScoreLastDroppedRings = 1,
		Score_Run_Best = 2,
		Score_BoosterEndRunBonus_Total = 3,
		Score_BoosterEnemyCombos_Total = 4,
		Score_BoosterRingStreaks_Total = 5,
		ScoreAsShadow_Total = 6,
		ScoreAsBlaze_Run = 7,
		Score_BoosterEnemyCombosAsEspio_Total = 8
	}

	public enum DistanceNames
	{
		DistanceRun_Total = 0,
		DistanceLastMissedRing = 1,
		DistanceLastPickedRing = 2,
		DistanceLastBanked = 3,
		Distance_Run_Best = 4,
		DistanceAsKnuckles_Total = 5,
		DistanceRun_Session = 6,
		DistanceRun_Run = 7,
		DistanceChangedLane = 8,
		DistanceDashMeterFilled = 9,
		DistanceAsShadow_Total = 10,
		DistanceAsTails_Total = 11,
		DistanceAsTails_Run = 12,
		DistanceAsSilver_Run = 13,
		DistanceAsEspio_Total = 14
	}

	public enum DateNames
	{
		LastDayPlayed = 0,
		LastDayNotPlayed = 1
	}

	[SerializeField]
	private List<string> m_plantpotObjectNames;

	[SerializeField]
	private List<string> m_totemObjectNames;

	private static PlayerStats m_singleton;

	private static Stats s_currentStats = new Stats();

	private CharacterManager m_characterManager;

	private int m_previousRingsBanked;

	private float m_previousDistance;

	private long m_previousScore;

	private int m_lastSessionTimeSaved;

	private float m_sessionTime;

	private float m_runTime;

	private Track.Lane m_previousLane;

	private float m_partialTime;

	private bool m_needsSave;

	private int m_loadCount;

	private bool m_displayFacebookReward;

	private bool m_displayGPlusReward;

	public static bool DashMeterFilled { get; set; }

	public static bool Airbourne { get; set; }

	public static Stats GetCurrentStats()
	{
		return s_currentStats;
	}

	public static int GetStat(StatNames statName)
	{
		return GetCurrentStats().m_trackedStats[(int)statName];
	}

	public static float GetDistance(DistanceNames distanceName)
	{
		return GetCurrentStats().m_trackedDistances[(int)distanceName];
	}

	public static void IncreaseStat(StatNames name, int amount)
	{
		s_currentStats.m_trackedStats[(int)name] += amount;
		if (m_singleton != null)
		{
			if (m_singleton.m_characterManager.GetCurrentCharacter() == Characters.Type.Amy && name == StatNames.RingsCollected_Total)
			{
				s_currentStats.m_trackedStats[62] += amount;
			}
			if (m_singleton.m_characterManager.GetCurrentCharacter() == Characters.Type.Blaze && name == StatNames.RingsCollected_Total)
			{
				s_currentStats.m_trackedStats[96] += amount;
			}
		}
	}

	public static void IncreaseLongStat(LongStatNames name, long amount)
	{
		s_currentStats.m_trackedLongStats[(int)name] += amount;
	}

	public static void IncreaseDistance(DistanceNames name, float amount)
	{
		if (!(amount < 0f))
		{
			s_currentStats.m_trackedDistances[(int)name] += amount;
		}
	}

	public static void UpdateDistanceToCurrent(DistanceNames name)
	{
		s_currentStats.m_trackedDistances[(int)name] = s_currentStats.m_trackedDistances[0];
	}

	public static float GetSecondsFromLastSaved()
	{
		return (m_singleton.m_sessionTime * 10f - (float)m_singleton.m_lastSessionTimeSaved) / 10f;
	}

	public static void UpdateMultiplier(uint multiplierValue)
	{
		if (multiplierValue > s_currentStats.m_trackedStats[69])
		{
			s_currentStats.m_trackedStats[69] = (int)multiplierValue;
		}
		if (multiplierValue > s_currentStats.m_trackedStats[68])
		{
			s_currentStats.m_trackedStats[68] = (int)multiplierValue;
		}
		if (multiplierValue > s_currentStats.m_trackedStats[67])
		{
			s_currentStats.m_trackedStats[67] = (int)multiplierValue;
		}
	}

	private static string ScoreToTrackEvent(long score)
	{
		if (score >= 1000000)
		{
			return "Score_1m";
		}
		if (score >= 500000)
		{
			return "Score_500K";
		}
		if (score >= 200000)
		{
			return "Score_200K";
		}
		if (score >= 100000)
		{
			return "Score_100K";
		}
		if (score >= 50000)
		{
			return "Score_50K";
		}
		if (score >= 20000)
		{
			return "Score_20K";
		}
		if (score > 10000)
		{
			return "Score_10K";
		}
		return string.Empty;
	}

	public static void UpdateFinalScore(long score)
	{
		s_currentStats.m_trackedLongStats[0] = score;
		if (s_currentStats.m_trackedLongStats[0] > s_currentStats.m_trackedLongStats[2])
		{
			string text = ScoreToTrackEvent(s_currentStats.m_trackedLongStats[2]);
			s_currentStats.m_trackedLongStats[2] = s_currentStats.m_trackedLongStats[0];
			string text2 = ScoreToTrackEvent(s_currentStats.m_trackedLongStats[2]);
			if (text2 != text)
			{
				SLAnalytics.LogTrackingEvent(text2, string.Empty);
			}
		}
	}

	public static void EnterSetPiece(TrackDatabase.PieceType type)
	{
		switch (type)
		{
		case TrackDatabase.PieceType.SetPieceLoop:
			IncreaseStat(StatNames.LoopsBoosted_Total, 1);
			break;
		case TrackDatabase.PieceType.SetPieceCorkscrew:
			IncreaseStat(StatNames.CorkscrewsRan_Total, 1);
			break;
		}
	}

	public static void JumpOverObstacle(UnityEngine.Object obstacle)
	{
		for (int i = 0; i < m_singleton.m_plantpotObjectNames.Count; i++)
		{
			if (obstacle.name.StartsWith(m_singleton.m_plantpotObjectNames[i]))
			{
				IncreaseStat(StatNames.PlantPotsJumpedOver_Total, 1);
				break;
			}
		}
	}

	public static void DashThroughObstacle(UnityEngine.Object obstacle)
	{
		for (int i = 0; i < m_singleton.m_totemObjectNames.Count; i++)
		{
			if (obstacle.name.StartsWith(m_singleton.m_totemObjectNames[i]))
			{
				IncreaseStat(StatNames.TotemsDashedThrough_Run, 1);
				IncreaseStat(StatNames.TotemsDashedThrough_Total, 1);
				break;
			}
		}
	}

	public void OnGooglePlusLogin()
	{
		if (s_currentStats.m_trackedStats[98] == 0)
		{
			m_displayGPlusReward = true;
			s_currentStats.m_trackedStats[98] = 1;
		}
	}

	public static PlayerStats instance()
	{
		return m_singleton;
	}

	private void Start()
	{
		m_singleton = this;
		EventDispatch.RegisterInterest("OnNewGameAboutToStart", this, EventDispatch.Priority.High);
		EventDispatch.RegisterInterest("OnGameFinished", this, EventDispatch.Priority.Low);
		EventDispatch.RegisterInterest("OnGameDataLoaded", this);
		EventDispatch.RegisterInterest("OnGameDataSaveRequest", this);
		EventDispatch.RegisterInterest("MainMenuActive", this);
		EventDispatch.RegisterInterest("OnSonicStumble", this, EventDispatch.Priority.Highest);
		EventDispatch.RegisterInterest("OnRingStreakCompleted", this);
		EventDispatch.RegisterInterest("OnRingBankRequest", this, EventDispatch.Priority.Lowest);
		EventDispatch.RegisterInterest("OnRingsAwarded", this);
		EventDispatch.RegisterInterest("OnDashMeterFilled", this);
		EventDispatch.RegisterInterest("OnEnemyKilled", this);
		EventDispatch.RegisterInterest("PowerUpLeveledUp", this);
		EventDispatch.RegisterInterest("OnFacebookLogin", this);
		DashMeterFilled = false;
		m_characterManager = UnityEngine.Object.FindObjectOfType(typeof(CharacterManager)) as CharacterManager;
		m_needsSave = false;
	}

	private void OnApplicationQuit()
	{
	}

	private void Event_OnGameDataSaveRequest()
	{
		s_currentStats.m_trackedStats[64] += (int)(m_sessionTime * 10f) - m_lastSessionTimeSaved;
		s_currentStats.m_trackedStats[65] = (int)(m_sessionTime * 10f);
		m_lastSessionTimeSaved = (int)(m_sessionTime * 10f);
		string[] names = Enum.GetNames(typeof(StatNames));
		for (int i = 0; i < names.Length; i++)
		{
			if (!names[i].EndsWith("_Run"))
			{
				PropertyStore.Store(names[i], s_currentStats.m_trackedStats[i]);
			}
		}
		names = Enum.GetNames(typeof(LongStatNames));
		for (int j = 0; j < names.Length; j++)
		{
			PropertyStore.Store(names[j], s_currentStats.m_trackedLongStats[j]);
		}
		names = Enum.GetNames(typeof(DistanceNames));
		for (int k = 0; k < names.Length; k++)
		{
			if (!names[k].EndsWith("_Run"))
			{
				PropertyStore.Store(names[k], s_currentStats.m_trackedDistances[k]);
			}
		}
		CultureInfo cultureInfo = new CultureInfo("en-US");
		names = Enum.GetNames(typeof(DateNames));
		for (int l = 0; l < names.Length; l++)
		{
			PropertyStore.Store(names[l], s_currentStats.m_trackedDates[l].Date.ToString(cultureInfo.DateTimeFormat));
		}
		PropertyStore.Store("VersionID", "1.16.1");
		m_needsSave = false;
	}

	private void Event_OnGameDataLoaded(ActiveProperties activeProperties)
	{
		string[] names = Enum.GetNames(typeof(StatNames));
		for (int i = 0; i < names.Length; i++)
		{
			if (!names[i].EndsWith("_Run"))
			{
				s_currentStats.m_trackedStats[i] = activeProperties.GetInt(names[i]);
			}
		}
		if (s_currentStats.m_trackedStats[47] < s_currentStats.m_trackedStats[46])
		{
			s_currentStats.m_trackedStats[47] = s_currentStats.m_trackedStats[46];
		}
		names = Enum.GetNames(typeof(LongStatNames));
		for (int j = 0; j < names.Length; j++)
		{
			s_currentStats.m_trackedLongStats[j] = activeProperties.GetLong(names[j]);
		}
		names = Enum.GetNames(typeof(DistanceNames));
		for (int k = 0; k < names.Length; k++)
		{
			if (!names[k].EndsWith("_Run"))
			{
				s_currentStats.m_trackedDistances[k] = activeProperties.GetFloat(names[k]);
			}
		}
		CultureInfo provider = new CultureInfo("en-US");
		names = Enum.GetNames(typeof(DateNames));
		for (int l = 0; l < names.Length; l++)
		{
			if (!DateTime.TryParse(activeProperties.GetString(names[l]), provider, DateTimeStyles.None, out s_currentStats.m_trackedDates[l]))
			{
				s_currentStats.m_trackedDates[l] = DateTime.Now.AddDays(-1.0).Date;
			}
		}
		names = Enum.GetNames(typeof(StatNames));
		for (int m = 0; m < names.Length; m++)
		{
			if (names[m].EndsWith("_Session"))
			{
				s_currentStats.m_trackedStats[m] = 0;
			}
		}
		names = Enum.GetNames(typeof(DistanceNames));
		for (int n = 0; n < names.Length; n++)
		{
			if (names[n].EndsWith("_Session"))
			{
				s_currentStats.m_trackedDistances[n] = 0f;
			}
		}
		IncreaseStat(StatNames.NumberOfSessions_Total, 1);
		m_loadCount++;
		if (m_loadCount != 2)
		{
			return;
		}
		if (activeProperties.GetPropertyCount() > 0)
		{
			string text = activeProperties.GetString("VersionID");
			if ("1.16.1" != text)
			{
				GameAnalytics.GameUpdated(false);
				m_needsSave = true;
			}
		}
		else
		{
			SLAnalytics.LogTrackingEvent("Open", "Install");
			m_needsSave = true;
		}
		int num = s_currentStats.m_trackedStats[80];
		if (num >= 2)
		{
			SLAnalytics.LogTrackingEvent("RepeatOpen", num.ToString());
		}
	}

	private void Event_MainMenuActive()
	{
		if (m_needsSave)
		{
			m_needsSave = false;
			PropertyStore.Save();
		}
	}

	private void Event_OnNewGameAboutToStart()
	{
		IncreaseStat(StatNames.NumberOfRuns_Total, 1);
		IncreaseStat(StatNames.NumberOfRuns_Session, 1);
		int num = s_currentStats.m_trackedStats[1];
		if (num >= 2)
		{
			SLAnalytics.LogTrackingEvent("RepeatPlay", num.ToString());
		}
		if (!s_currentStats.m_trackedDates[0].Date.Equals(DateTime.Now.Date))
		{
			if (!s_currentStats.m_trackedDates[0].Date.Equals(DateTime.Now.AddDays(-1.0).Date))
			{
				s_currentStats.m_trackedDates[1] = DateTime.Now.AddDays(-1.0).Date;
			}
			s_currentStats.m_trackedDates[0] = DateTime.Now.Date;
		}
		string[] names = Enum.GetNames(typeof(StatNames));
		for (int i = 0; i < names.Length; i++)
		{
			if (names[i].EndsWith("_Run"))
			{
				s_currentStats.m_trackedStats[i] = 0;
			}
		}
		names = Enum.GetNames(typeof(DistanceNames));
		for (int j = 0; j < names.Length; j++)
		{
			if (names[j].EndsWith("_Run"))
			{
				s_currentStats.m_trackedDistances[j] = 0f;
			}
		}
		s_currentStats.m_trackedLongStats[0] = 0L;
		s_currentStats.m_trackedLongStats[1] = 0L;
		UpdateDistanceToCurrent(DistanceNames.DistanceLastMissedRing);
		UpdateDistanceToCurrent(DistanceNames.DistanceLastPickedRing);
		UpdateDistanceToCurrent(DistanceNames.DistanceChangedLane);
		UpdateDistanceToCurrent(DistanceNames.DistanceLastBanked);
		m_previousRingsBanked = 0;
		m_previousDistance = 0f;
		m_previousScore = 0L;
		DashMeterFilled = false;
		Airbourne = false;
		m_partialTime = 0f;
		m_runTime = 0f;
		int numberOfBoostersSelected = Boosters.GetNumberOfBoostersSelected();
		IncreaseStat(StatNames.BoostersUsed_Total, numberOfBoostersSelected);
		if (m_singleton.m_characterManager.GetCurrentCharacter() == Characters.Type.Cream)
		{
			IncreaseStat(StatNames.BoostersUsedAsCream_Total, numberOfBoostersSelected);
		}
		GameAnalytics.RunStart(m_singleton.m_characterManager.GetCurrentCharacter());
	}

	private void Event_OnGameFinished()
	{
		s_currentStats.m_trackedStats[18] = RingStorage.HeldRings;
		s_currentStats.m_trackedStats[15] = RingStorage.TotalBankedRings;
		IncreaseStat(StatNames.RingsBanked_Session, RingStorage.RunBankedRings - m_previousRingsBanked);
		s_currentStats.m_trackedStats[17] = RingStorage.RunBankedRings;
		s_currentStats.m_trackedStats[66] = (int)(m_runTime * 10f);
		if (s_currentStats.m_trackedDistances[7] > s_currentStats.m_trackedDistances[4])
		{
			s_currentStats.m_trackedDistances[4] = s_currentStats.m_trackedDistances[7];
		}
		if (s_currentStats.m_trackedLongStats[0] > s_currentStats.m_trackedLongStats[2])
		{
			string text = ScoreToTrackEvent(s_currentStats.m_trackedLongStats[2]);
			s_currentStats.m_trackedLongStats[2] = s_currentStats.m_trackedLongStats[0];
			string text2 = ScoreToTrackEvent(s_currentStats.m_trackedLongStats[2]);
			if (text2 != text)
			{
				SLAnalytics.LogTrackingEvent(text2, string.Empty);
			}
		}
		if (s_currentStats.m_trackedStats[17] > s_currentStats.m_trackedStats[57])
		{
			s_currentStats.m_trackedStats[57] = s_currentStats.m_trackedStats[17];
		}
		if (s_currentStats.m_trackedStats[20] > s_currentStats.m_trackedStats[58])
		{
			s_currentStats.m_trackedStats[58] = s_currentStats.m_trackedStats[20];
		}
		if (s_currentStats.m_trackedStats[4] > s_currentStats.m_trackedStats[59])
		{
			s_currentStats.m_trackedStats[59] = s_currentStats.m_trackedStats[4];
		}
		if (s_currentStats.m_trackedStats[10] > s_currentStats.m_trackedStats[60])
		{
			s_currentStats.m_trackedStats[60] = s_currentStats.m_trackedStats[10];
		}
	}

	private void Event_OnFacebookLogin()
	{
		if (s_currentStats.m_trackedStats[61] == 0)
		{
			s_currentStats.m_trackedStats[61] = 1;
			if (ABTesting.GetTestValueAsInt(ABTesting.Tests.RSR_Facebook) > 0)
			{
				m_displayFacebookReward = true;
			}
		}
	}

	private void Update()
	{
		if (m_displayFacebookReward)
		{
			StorePurchases.RequestReward("single_star_reward", 1, 5, StorePurchases.ShowDialog.Yes);
			m_displayFacebookReward = false;
		}
		if (m_displayGPlusReward)
		{
			StorePurchases.RequestReward("single_star_reward", 1, 20, StorePurchases.ShowDialog.Yes);
			m_displayGPlusReward = false;
		}
		m_sessionTime += IndependantTimeDelta.Delta;
		if (GameState.GetMode() == GameState.Mode.Menu)
		{
			return;
		}
		m_runTime += Time.deltaTime;
		if (Sonic.Tracker.InternalMotionState != null && Sonic.Tracker.InternalMotionState.HasActiveState && (Sonic.Tracker.InternalMotionState.CurrentState.GetType() == typeof(MotionDiveState) || Sonic.Tracker.InternalMotionState.CurrentState.GetType() == typeof(MotionFallState) || Sonic.Tracker.InternalMotionState.CurrentState.GetType() == typeof(MotionGlideState) || Sonic.Tracker.InternalMotionState.CurrentState.GetType() == typeof(MotionJumpState) || Sonic.Tracker.InternalMotionState.CurrentState.GetType() == typeof(MotionSpringState) || Sonic.Tracker.InternalMotionState.CurrentState.GetType() == typeof(MotionSpringAscentState) || Sonic.Tracker.InternalMotionState.CurrentState.GetType() == typeof(MotionSpringDescentState) || Sonic.Tracker.InternalMotionState.CurrentState.GetType() == typeof(MotionSpringGesturesState) || (Sonic.Tracker.InternalMotionState.CurrentState.GetType() == typeof(MotionAttackState) && Sonic.Tracker.JumpHeight > 0f)))
		{
			m_partialTime += Time.deltaTime;
			int num = (int)Mathf.Floor(m_partialTime * 10f);
			m_partialTime -= (float)num / 10f;
			IncreaseStat(StatNames.TimeAirbourne_Total, num);
			IncreaseStat(StatNames.TimeAirbourne_Run, num);
		}
		float amount = Sonic.Tracker.DistanceTravelled - m_previousDistance;
		IncreaseDistance(DistanceNames.DistanceRun_Total, amount);
		IncreaseDistance(DistanceNames.DistanceRun_Session, amount);
		IncreaseDistance(DistanceNames.DistanceRun_Run, amount);
		if (DashMeterFilled)
		{
			IncreaseDistance(DistanceNames.DistanceDashMeterFilled, amount);
		}
		if (Sonic.Tracker.IsTrackerAvailable)
		{
			if (m_previousLane != Sonic.Tracker.getLane())
			{
				UpdateDistanceToCurrent(DistanceNames.DistanceChangedLane);
			}
			m_previousLane = Sonic.Tracker.getLane();
		}
		if (m_singleton.m_characterManager.GetCurrentCharacter() == Characters.Type.Knuckles)
		{
			IncreaseDistance(DistanceNames.DistanceAsKnuckles_Total, amount);
		}
		if (m_singleton.m_characterManager.GetCurrentCharacter() == Characters.Type.Shadow)
		{
			IncreaseDistance(DistanceNames.DistanceAsShadow_Total, amount);
		}
		if (m_singleton.m_characterManager.GetCurrentCharacter() == Characters.Type.Tails)
		{
			IncreaseDistance(DistanceNames.DistanceAsTails_Total, amount);
			IncreaseDistance(DistanceNames.DistanceAsTails_Run, amount);
		}
		if (m_singleton.m_characterManager.GetCurrentCharacter() == Characters.Type.Silver)
		{
			IncreaseDistance(DistanceNames.DistanceAsSilver_Run, amount);
		}
		if (m_singleton.m_characterManager.GetCurrentCharacter() == Characters.Type.Espio)
		{
			IncreaseDistance(DistanceNames.DistanceAsEspio_Total, amount);
		}
		m_previousDistance = Sonic.Tracker.DistanceTravelled;
		s_currentStats.m_trackedStats[18] = RingStorage.HeldRings;
		s_currentStats.m_trackedLongStats[0] = ScoreTracker.CurrentScore;
		long amount2 = ScoreTracker.CurrentScore - m_previousScore;
		if (m_singleton.m_characterManager.GetCurrentCharacter() == Characters.Type.Shadow)
		{
			IncreaseLongStat(LongStatNames.ScoreAsShadow_Total, amount2);
		}
		if (m_singleton.m_characterManager.GetCurrentCharacter() == Characters.Type.Blaze)
		{
			IncreaseLongStat(LongStatNames.ScoreAsBlaze_Run, amount2);
		}
		m_previousScore = ScoreTracker.CurrentScore;
	}

	private void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
		}
	}

	private void Event_OnSonicStumble()
	{
		IncreaseStat(StatNames.TimesDropRings_Total, 1);
		IncreaseStat(StatNames.RingsDropped_Total, RingStorage.HeldRings);
		s_currentStats.m_trackedLongStats[1] = ScoreTracker.CurrentScore;
	}

	private void Event_OnRingBankRequest()
	{
		UpdateDistanceToCurrent(DistanceNames.DistanceLastBanked);
		IncreaseStat(StatNames.RingsBanked_Total, RingStorage.RunBankedRings - m_previousRingsBanked);
		IncreaseStat(StatNames.RingsBanked_Session, RingStorage.RunBankedRings - m_previousRingsBanked);
		s_currentStats.m_trackedStats[17] = RingStorage.RunBankedRings;
		m_previousRingsBanked = RingStorage.RunBankedRings;
	}

	private void Event_OnRingStreakCompleted(int lenght, float firstRingTrackPosition, float lastRingTrackPosition)
	{
		IncreaseStat(StatNames.RingStreaks_Total, 1);
		IncreaseStat(StatNames.RingStreaks_Run, 1);
		if (m_singleton.m_characterManager.GetCurrentCharacter() == Characters.Type.Rouge && Boosters.IsBoosterSelected(PowerUps.Type.Booster_RingStreakBonus))
		{
			IncreaseStat(StatNames.RingStreaksBoosterBonusesAsRouge_Total, 1);
		}
	}

	private void Event_OnRingsAwarded(int ringCount)
	{
		if (ringCount > 0)
		{
			IncreaseStat(StatNames.RingsPurchased_Total, ringCount);
		}
		else
		{
			IncreaseStat(StatNames.RingsSpent_Total, -ringCount);
		}
	}

	private void Event_OnEnemyKilled(Enemy enemy, Enemy.Kill killType)
	{
		switch (killType)
		{
		case Enemy.Kill.Homing:
			IncreaseStat(StatNames.EnemiesHoming_Total, 1);
			IncreaseStat(StatNames.Enemies_Total, 1);
			IncreaseStat(StatNames.Enemies_Session, 1);
			IncreaseStat(StatNames.Enemies_Run, 1);
			break;
		case Enemy.Kill.Rolling:
			IncreaseStat(StatNames.EnemiesRolling_Total, 1);
			IncreaseStat(StatNames.Enemies_Total, 1);
			IncreaseStat(StatNames.Enemies_Session, 1);
			IncreaseStat(StatNames.Enemies_Run, 1);
			break;
		case Enemy.Kill.Diving:
			IncreaseStat(StatNames.EnemiesDiving_Total, 1);
			IncreaseStat(StatNames.Enemies_Total, 1);
			IncreaseStat(StatNames.Enemies_Session, 1);
			IncreaseStat(StatNames.Enemies_Run, 1);
			IncreaseStat(StatNames.EnemiesAir_Total, 1);
			break;
		}
		if (enemy.isChopper())
		{
			IncreaseStat(StatNames.EnemiesAir_Total, 1);
			if (enemy.Golden)
			{
				IncreaseStat(StatNames.GoldenChoppersKilled_Total, 1);
				if (m_singleton.m_characterManager.GetCurrentCharacter() == Characters.Type.Cream)
				{
					IncreaseStat(StatNames.GoldenChoppersKilledAsCream_Total, 1);
				}
			}
		}
		if (enemy.Golden && m_singleton.m_characterManager.GetCurrentCharacter() == Characters.Type.Silver)
		{
			IncreaseStat(StatNames.GoldenEnemiesKilledAsSilver_Total, 1);
		}
	}

	private void Event_PowerUpLeveledUp(PowerUps.Type pUp)
	{
		if (PowerUpsInventory.GetPowerUpLevel(pUp) == 6)
		{
			IncreaseStat(StatNames.MaxedPowerUps_Total, 1);
		}
		if (PowerUpsInventory.GetPowerUpLevel(pUp) == 5)
		{
			IncreaseStat(StatNames.Level5PowerUps_Total, 1);
		}
	}

	private void Event_OnDashMeterFilled()
	{
		DashMeterFilled = true;
	}
}
