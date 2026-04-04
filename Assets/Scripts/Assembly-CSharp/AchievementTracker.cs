using System;
using UnityEngine;

public class AchievementTracker : MonoBehaviour
{
	public struct Achievement
	{
		[Flags]
		public enum State
		{
			Completed = 1,
			SpecialCheck = 2,
			ResetEachRun = 4,
			Pending = 0x10,
			ValidOnPlatform = 0x20
		}

		public Achievements.Types m_id;

		public string m_name;

		public PlayerStats.StatTypes m_trackedType;

		public int m_trackedValue;

		public long m_amountStart;

		public long m_amountCurrent;

		public long m_amountTarget;

		public long m_amountNeeded;

		public State m_state;
	}

	private const string AchievementStatePropertyPrefix = "AchievementState_";

	public static float toFloatDivision = 10000f;

	[SerializeField]
	private int[] m_achivementeNeededValues;

	private Achievement[] m_achievements = new Achievement[Enum.GetNames(typeof(Achievements.Types)).Length];

	public void Initialize()
	{
		int num = Enum.GetNames(typeof(Achievements.Types)).Length;
		if (m_achivementeNeededValues == null)
		{
			m_achivementeNeededValues = new int[num];
		}
		else if (m_achivementeNeededValues.Length != Enum.GetNames(typeof(Achievements.Types)).Length)
		{
			int[] array = new int[num];
			int num2 = m_achivementeNeededValues.Length;
			if (num2 > num)
			{
				num2 = num;
			}
			for (int i = 0; i < num2; i++)
			{
				array[i] = m_achivementeNeededValues[i];
			}
			m_achivementeNeededValues = array;
		}
	}

	public void Awake()
	{
		Initialize();
	}

	private void Start()
	{
		EventDispatch.RegisterInterest("OnNewGameStarted", this);
		EventDispatch.RegisterInterest("OnGameFinished", this, EventDispatch.Priority.Lowest);
		EventDispatch.RegisterInterest("PowerUpLeveledUp", this, EventDispatch.Priority.Lowest);
		EventDispatch.RegisterInterest("OnGameDataLoaded", this);
		EventDispatch.RegisterInterest("OnGameDataSaveRequest", this);
		CreateAchievements();
	}

	private void CreateAchievements()
	{
		m_achievements[0].m_id = Achievements.Types.SonicRampage;
		m_achievements[0].m_trackedType = PlayerStats.StatTypes.Float;
		m_achievements[0].m_trackedValue = 0;
		m_achievements[0].m_amountNeeded = m_achivementeNeededValues[0];
		m_achievements[0].m_amountTarget = m_achievements[0].m_amountNeeded * (long)toFloatDivision;
		m_achievements[0].m_state = Achievement.State.ValidOnPlatform;
		m_achievements[1].m_id = Achievements.Types.RingHoarder;
		m_achievements[1].m_trackedType = PlayerStats.StatTypes.Int;
		m_achievements[1].m_trackedValue = 12;
		m_achievements[1].m_amountNeeded = m_achivementeNeededValues[1];
		m_achievements[1].m_amountTarget = m_achievements[1].m_amountNeeded;
		m_achievements[1].m_state = Achievement.State.ValidOnPlatform;
		m_achievements[2].m_id = Achievements.Types.PowerOverload;
		m_achievements[2].m_trackedType = PlayerStats.StatTypes.Int;
		m_achievements[2].m_trackedValue = 46;
		m_achievements[2].m_amountNeeded = m_achivementeNeededValues[2];
		m_achievements[2].m_amountTarget = m_achievements[2].m_amountNeeded;
		m_achievements[2].m_state = Achievement.State.ValidOnPlatform;
		m_achievements[3].m_id = Achievements.Types.KnuclesOnTheMove;
		m_achievements[3].m_trackedType = PlayerStats.StatTypes.Float;
		m_achievements[3].m_trackedValue = 5;
		m_achievements[3].m_amountNeeded = m_achivementeNeededValues[3];
		m_achievements[3].m_amountTarget = m_achievements[3].m_amountNeeded * (long)toFloatDivision;
		m_achievements[3].m_state = Achievement.State.ValidOnPlatform;
		m_achievements[4].m_id = Achievements.Types.CaChing;
		m_achievements[4].m_trackedType = PlayerStats.StatTypes.Int;
		m_achievements[4].m_trackedValue = 62;
		m_achievements[4].m_amountNeeded = m_achivementeNeededValues[4];
		m_achievements[4].m_amountTarget = m_achievements[4].m_amountNeeded;
		m_achievements[4].m_state = Achievement.State.ValidOnPlatform;
		m_achievements[5].m_id = Achievements.Types.SkyIsTheLimit;
		m_achievements[5].m_trackedType = PlayerStats.StatTypes.Long;
		m_achievements[5].m_trackedValue = 0;
		m_achievements[5].m_amountNeeded = m_achivementeNeededValues[5];
		m_achievements[5].m_amountTarget = m_achievements[5].m_amountNeeded;
		m_achievements[5].m_state = Achievement.State.ResetEachRun | Achievement.State.ValidOnPlatform;
		m_achievements[6].m_id = Achievements.Types.Ringmaster;
		m_achievements[6].m_trackedType = PlayerStats.StatTypes.Int;
		m_achievements[6].m_trackedValue = 12;
		m_achievements[6].m_amountNeeded = m_achivementeNeededValues[6];
		m_achievements[6].m_amountTarget = m_achievements[6].m_amountNeeded;
		m_achievements[6].m_state = Achievement.State.ResetEachRun | Achievement.State.ValidOnPlatform;
		m_achievements[7].m_id = Achievements.Types.SuperSonic;
		m_achievements[7].m_trackedType = PlayerStats.StatTypes.Float;
		m_achievements[7].m_trackedValue = 0;
		m_achievements[7].m_amountNeeded = m_achivementeNeededValues[7];
		m_achievements[7].m_amountTarget = m_achievements[7].m_amountNeeded * (long)toFloatDivision;
		m_achievements[7].m_state = Achievement.State.ResetEachRun | Achievement.State.ValidOnPlatform;
		m_achievements[8].m_id = Achievements.Types.MissionMaster;
		m_achievements[8].m_trackedType = PlayerStats.StatTypes.Int;
		m_achievements[8].m_trackedValue = 63;
		m_achievements[8].m_amountNeeded = m_achivementeNeededValues[8];
		m_achievements[8].m_amountTarget = m_achievements[8].m_amountNeeded;
		m_achievements[8].m_state = Achievement.State.ValidOnPlatform;
		m_achievements[9].m_id = Achievements.Types.OnARoll;
		m_achievements[9].m_trackedType = PlayerStats.StatTypes.Int;
		m_achievements[9].m_trackedValue = 63;
		m_achievements[9].m_amountNeeded = m_achivementeNeededValues[9];
		m_achievements[9].m_amountTarget = m_achievements[9].m_amountNeeded;
		m_achievements[9].m_state = Achievement.State.ValidOnPlatform;
		m_achievements[10].m_id = Achievements.Types.ActionPacked;
		m_achievements[10].m_trackedType = PlayerStats.StatTypes.Int;
		m_achievements[10].m_trackedValue = 63;
		m_achievements[10].m_amountNeeded = m_achivementeNeededValues[10];
		m_achievements[10].m_amountTarget = m_achievements[10].m_amountNeeded;
		m_achievements[10].m_state = Achievement.State.ValidOnPlatform;
		m_achievements[11].m_id = Achievements.Types.HotHeels;
		m_achievements[11].m_trackedType = PlayerStats.StatTypes.Float;
		m_achievements[11].m_trackedValue = 0;
		m_achievements[11].m_amountNeeded = m_achivementeNeededValues[11];
		m_achievements[11].m_amountTarget = m_achievements[11].m_amountNeeded * (long)toFloatDivision;
		m_achievements[11].m_state = Achievement.State.ResetEachRun | Achievement.State.ValidOnPlatform;
		m_achievements[12].m_id = Achievements.Types.SEGAMember;
		m_achievements[12].m_trackedType = PlayerStats.StatTypes.Int;
		m_achievements[12].m_trackedValue = 61;
		m_achievements[12].m_amountNeeded = m_achivementeNeededValues[12];
		m_achievements[12].m_amountTarget = m_achievements[12].m_amountNeeded;
		m_achievements[12].m_state = Achievement.State.SpecialCheck | Achievement.State.ValidOnPlatform;
		m_achievements[13].m_id = Achievements.Types.EasyTarget;
		m_achievements[13].m_trackedType = PlayerStats.StatTypes.Int;
		m_achievements[13].m_trackedValue = 12;
		m_achievements[13].m_amountNeeded = m_achivementeNeededValues[13];
		m_achievements[13].m_amountTarget = m_achievements[13].m_amountNeeded;
		m_achievements[13].m_state = Achievement.State.ResetEachRun | Achievement.State.ValidOnPlatform;
		m_achievements[14].m_id = Achievements.Types.WarmUp;
		m_achievements[14].m_trackedType = PlayerStats.StatTypes.Float;
		m_achievements[14].m_trackedValue = 0;
		m_achievements[14].m_amountNeeded = m_achivementeNeededValues[14];
		m_achievements[14].m_amountTarget = m_achievements[14].m_amountNeeded * (long)toFloatDivision;
		m_achievements[14].m_state = Achievement.State.ResetEachRun | Achievement.State.ValidOnPlatform;
		m_achievements[15].m_id = Achievements.Types.GoldFish;
		m_achievements[15].m_trackedType = PlayerStats.StatTypes.Int;
		m_achievements[15].m_trackedValue = 11;
		m_achievements[15].m_amountNeeded = m_achivementeNeededValues[15];
		m_achievements[15].m_amountTarget = m_achievements[15].m_amountNeeded;
		m_achievements[15].m_state = Achievement.State.ValidOnPlatform;
		m_achievements[16].m_id = Achievements.Types.ItAintOver;
		m_achievements[16].m_trackedType = PlayerStats.StatTypes.Long;
		m_achievements[16].m_trackedValue = 3;
		m_achievements[16].m_amountNeeded = m_achivementeNeededValues[16];
		m_achievements[16].m_amountTarget = m_achievements[16].m_amountNeeded;
		m_achievements[16].m_state = Achievement.State.ValidOnPlatform;
		m_achievements[17].m_id = Achievements.Types.Springtime;
		m_achievements[17].m_trackedType = PlayerStats.StatTypes.Int;
		m_achievements[17].m_trackedValue = 95;
		m_achievements[17].m_amountNeeded = m_achivementeNeededValues[17];
		m_achievements[17].m_amountTarget = m_achievements[17].m_amountNeeded;
		m_achievements[17].m_state = Achievement.State.ValidOnPlatform;
		m_achievements[18].m_id = Achievements.Types.ThatWasCool;
		m_achievements[18].m_trackedType = PlayerStats.StatTypes.Long;
		m_achievements[18].m_trackedValue = 4;
		m_achievements[18].m_amountNeeded = m_achivementeNeededValues[18];
		m_achievements[18].m_amountTarget = m_achievements[18].m_amountNeeded;
		m_achievements[18].m_state = Achievement.State.ValidOnPlatform;
		m_achievements[19].m_id = Achievements.Types.Streaker;
		m_achievements[19].m_trackedType = PlayerStats.StatTypes.Long;
		m_achievements[19].m_trackedValue = 5;
		m_achievements[19].m_amountNeeded = m_achivementeNeededValues[19];
		m_achievements[19].m_amountTarget = m_achievements[19].m_amountNeeded;
		m_achievements[19].m_state = Achievement.State.ValidOnPlatform;
		m_achievements[20].m_id = Achievements.Types.KillTheLight;
		m_achievements[20].m_trackedType = PlayerStats.StatTypes.Float;
		m_achievements[20].m_trackedValue = 10;
		m_achievements[20].m_amountNeeded = m_achivementeNeededValues[20];
		m_achievements[20].m_amountTarget = m_achievements[20].m_amountNeeded * (long)toFloatDivision;
		m_achievements[20].m_state = Achievement.State.ValidOnPlatform;
		m_achievements[21].m_id = Achievements.Types.ChasingShadows;
		m_achievements[21].m_trackedType = PlayerStats.StatTypes.Long;
		m_achievements[21].m_trackedValue = 6;
		m_achievements[21].m_amountNeeded = m_achivementeNeededValues[21];
		m_achievements[21].m_amountTarget = m_achievements[21].m_amountNeeded;
		m_achievements[21].m_state = Achievement.State.ResetEachRun | Achievement.State.ValidOnPlatform;
		m_achievements[22].m_id = Achievements.Types.RingOfFire;
		m_achievements[22].m_trackedType = PlayerStats.StatTypes.Int;
		m_achievements[22].m_trackedValue = 96;
		m_achievements[22].m_amountNeeded = m_achivementeNeededValues[22];
		m_achievements[22].m_amountTarget = m_achievements[22].m_amountNeeded;
		m_achievements[22].m_state = Achievement.State.ValidOnPlatform;
		m_achievements[23].m_id = Achievements.Types.Trailblazer;
		m_achievements[23].m_trackedType = PlayerStats.StatTypes.Long;
		m_achievements[23].m_trackedValue = 7;
		m_achievements[23].m_amountNeeded = m_achivementeNeededValues[23];
		m_achievements[23].m_amountTarget = m_achievements[23].m_amountNeeded;
		m_achievements[23].m_state = Achievement.State.ResetEachRun | Achievement.State.ValidOnPlatform;
		m_achievements[24].m_id = Achievements.Types.RunningMiles;
		m_achievements[24].m_trackedType = PlayerStats.StatTypes.Float;
		m_achievements[24].m_trackedValue = 11;
		m_achievements[24].m_amountNeeded = m_achivementeNeededValues[24];
		m_achievements[24].m_amountTarget = m_achievements[24].m_amountNeeded * (long)toFloatDivision;
		m_achievements[24].m_state = Achievement.State.ValidOnPlatform;
		m_achievements[25].m_id = Achievements.Types.MilesPerHour;
		m_achievements[25].m_trackedType = PlayerStats.StatTypes.Float;
		m_achievements[25].m_trackedValue = 12;
		m_achievements[25].m_amountNeeded = m_achivementeNeededValues[25];
		m_achievements[25].m_amountTarget = m_achievements[25].m_amountNeeded * (long)toFloatDivision;
		m_achievements[25].m_state = Achievement.State.ResetEachRun | Achievement.State.ValidOnPlatform;
		m_achievements[26].m_id = Achievements.Types.HelpingHand;
		m_achievements[26].m_trackedType = PlayerStats.StatTypes.Int;
		m_achievements[26].m_trackedValue = 97;
		m_achievements[26].m_amountNeeded = m_achivementeNeededValues[26];
		m_achievements[26].m_amountTarget = m_achievements[26].m_amountNeeded;
		m_achievements[26].m_state = Achievement.State.ValidOnPlatform;
		m_achievements[27].m_id = Achievements.Types.FullyLoaded;
		m_achievements[27].m_trackedType = PlayerStats.StatTypes.Int;
		m_achievements[27].m_trackedValue = 97;
		m_achievements[27].m_amountNeeded = m_achivementeNeededValues[27];
		m_achievements[27].m_amountTarget = m_achievements[27].m_amountNeeded;
		m_achievements[27].m_state = Achievement.State.ValidOnPlatform;
		m_achievements[28].m_id = Achievements.Types.MarathonMan;
		m_achievements[28].m_trackedType = PlayerStats.StatTypes.Float;
		m_achievements[28].m_trackedValue = 0;
		m_achievements[28].m_amountNeeded = m_achivementeNeededValues[28];
		m_achievements[28].m_amountTarget = m_achievements[28].m_amountNeeded * (long)toFloatDivision;
		m_achievements[28].m_state = Achievement.State.ValidOnPlatform;
		m_achievements[29].m_id = Achievements.Types.OnlyForTheBrave;
		m_achievements[29].m_trackedType = PlayerStats.StatTypes.Long;
		m_achievements[29].m_trackedValue = 0;
		m_achievements[29].m_amountNeeded = m_achivementeNeededValues[29];
		m_achievements[29].m_amountTarget = m_achievements[29].m_amountNeeded;
		m_achievements[29].m_state = Achievement.State.ResetEachRun | Achievement.State.ValidOnPlatform;
		m_achievements[30].m_id = Achievements.Types.ItsNoSprint;
		m_achievements[30].m_trackedType = PlayerStats.StatTypes.Float;
		m_achievements[30].m_trackedValue = 0;
		m_achievements[30].m_amountNeeded = m_achivementeNeededValues[30];
		m_achievements[30].m_amountTarget = m_achievements[30].m_amountNeeded * (long)toFloatDivision;
		m_achievements[30].m_state = Achievement.State.ValidOnPlatform;
		m_achievements[31].m_id = Achievements.Types.Adventurer;
		m_achievements[31].m_trackedType = PlayerStats.StatTypes.Int;
		m_achievements[31].m_trackedValue = 36;
		m_achievements[31].m_amountNeeded = m_achivementeNeededValues[31];
		m_achievements[31].m_amountTarget = m_achievements[31].m_amountNeeded;
		m_achievements[31].m_state = Achievement.State.ValidOnPlatform;
		m_achievements[32].m_id = Achievements.Types.BeachBum;
		m_achievements[32].m_trackedType = PlayerStats.StatTypes.Int;
		m_achievements[32].m_trackedValue = 38;
		m_achievements[32].m_amountNeeded = m_achivementeNeededValues[32];
		m_achievements[32].m_amountTarget = m_achievements[32].m_amountNeeded;
		m_achievements[32].m_state = Achievement.State.ValidOnPlatform;
		m_achievements[33].m_id = Achievements.Types.Alchemy;
		m_achievements[33].m_trackedType = PlayerStats.StatTypes.Int;
		m_achievements[33].m_trackedValue = 99;
		m_achievements[33].m_amountNeeded = m_achivementeNeededValues[33];
		m_achievements[33].m_amountTarget = m_achievements[33].m_amountNeeded;
		m_achievements[33].m_state = Achievement.State.ValidOnPlatform;
		m_achievements[34].m_id = Achievements.Types.SilverStreak;
		m_achievements[34].m_trackedType = PlayerStats.StatTypes.Float;
		m_achievements[34].m_trackedValue = 13;
		m_achievements[34].m_amountNeeded = m_achivementeNeededValues[34];
		m_achievements[34].m_amountTarget = m_achievements[34].m_amountNeeded * (long)toFloatDivision;
		m_achievements[34].m_state = Achievement.State.ResetEachRun | Achievement.State.ValidOnPlatform;
		m_achievements[35].m_id = Achievements.Types.BattingStreak;
		m_achievements[35].m_trackedType = PlayerStats.StatTypes.Int;
		m_achievements[35].m_trackedValue = 100;
		m_achievements[35].m_amountNeeded = m_achivementeNeededValues[35];
		m_achievements[35].m_amountTarget = m_achievements[35].m_amountNeeded;
		m_achievements[35].m_state = Achievement.State.ValidOnPlatform;
		m_achievements[36].m_id = Achievements.Types.BlingSpring;
		m_achievements[36].m_trackedType = PlayerStats.StatTypes.Int;
		m_achievements[36].m_trackedValue = 101;
		m_achievements[36].m_amountNeeded = m_achivementeNeededValues[36];
		m_achievements[36].m_amountTarget = m_achievements[36].m_amountNeeded;
		m_achievements[36].m_state = Achievement.State.ValidOnPlatform;
		m_achievements[37].m_id = Achievements.Types.CreamOfTheChop;
		m_achievements[37].m_trackedType = PlayerStats.StatTypes.Int;
		m_achievements[37].m_trackedValue = 102;
		m_achievements[37].m_amountNeeded = m_achivementeNeededValues[37];
		m_achievements[37].m_amountTarget = m_achievements[37].m_amountNeeded;
		m_achievements[37].m_state = Achievement.State.ValidOnPlatform;
		m_achievements[38].m_id = Achievements.Types.BoostedBunny;
		m_achievements[38].m_trackedType = PlayerStats.StatTypes.Int;
		m_achievements[38].m_trackedValue = 103;
		m_achievements[38].m_amountNeeded = m_achivementeNeededValues[38];
		m_achievements[38].m_amountTarget = m_achievements[38].m_amountNeeded;
		m_achievements[38].m_state = Achievement.State.ValidOnPlatform;
		m_achievements[39].m_id = Achievements.Types.EspioReady;
		m_achievements[39].m_trackedType = PlayerStats.StatTypes.Float;
		m_achievements[39].m_trackedValue = 14;
		m_achievements[39].m_amountNeeded = m_achivementeNeededValues[39];
		m_achievements[39].m_amountTarget = m_achievements[39].m_amountNeeded * (long)toFloatDivision;
		m_achievements[39].m_state = (Achievement.State)0;
		m_achievements[40].m_id = Achievements.Types.KomboChamelion;
		m_achievements[40].m_trackedType = PlayerStats.StatTypes.Long;
		m_achievements[40].m_trackedValue = 8;
		m_achievements[40].m_amountNeeded = m_achivementeNeededValues[40];
		m_achievements[40].m_amountTarget = m_achievements[40].m_amountNeeded;
		m_achievements[40].m_state = (Achievement.State)0;
	}

	private void Update()
	{
		if ((m_achievements[12].m_state & Achievement.State.Completed) != Achievement.State.Completed)
		{
			CheckAchievement(ref m_achievements[12]);
		}
		if ((m_achievements[2].m_state & Achievement.State.Completed) != Achievement.State.Completed)
		{
			CheckAchievement(ref m_achievements[2]);
		}
		if ((m_achievements[10].m_state & Achievement.State.Completed) != Achievement.State.Completed)
		{
			CheckAchievement(ref m_achievements[10]);
		}
		if ((m_achievements[9].m_state & Achievement.State.Completed) != Achievement.State.Completed)
		{
			CheckAchievement(ref m_achievements[9]);
		}
		if ((m_achievements[8].m_state & Achievement.State.Completed) != Achievement.State.Completed)
		{
			CheckAchievement(ref m_achievements[8]);
		}
		if (GameState.GetMode() == GameState.Mode.Menu)
		{
			return;
		}
		for (int i = 0; i < m_achievements.Length; i++)
		{
			if ((m_achievements[i].m_state & Achievement.State.Completed) != Achievement.State.Completed)
			{
				CheckAchievement(ref m_achievements[i]);
			}
		}
	}

	private void CheckAchievement(ref Achievement currentAchievement)
	{
		PlayerStats.Stats currentStats = PlayerStats.GetCurrentStats();
		if (currentAchievement.m_id == Achievements.Types.MissionMaster && MissionTracker.AllMissionsComplete() && currentStats.m_trackedStats[currentAchievement.m_trackedValue] < currentAchievement.m_amountTarget)
		{
			currentStats.m_trackedStats[currentAchievement.m_trackedValue] = (int)currentAchievement.m_amountTarget;
		}
		switch (currentAchievement.m_trackedType)
		{
		case PlayerStats.StatTypes.Int:
			currentAchievement.m_amountCurrent = currentStats.m_trackedStats[currentAchievement.m_trackedValue];
			break;
		case PlayerStats.StatTypes.Long:
			currentAchievement.m_amountCurrent = currentStats.m_trackedLongStats[currentAchievement.m_trackedValue];
			break;
		case PlayerStats.StatTypes.Float:
			currentAchievement.m_amountCurrent = (long)(currentStats.m_trackedDistances[currentAchievement.m_trackedValue] * toFloatDivision);
			break;
		}
		if ((currentAchievement.m_state &= Achievement.State.ValidOnPlatform) == Achievement.State.ValidOnPlatform && (currentAchievement.m_amountCurrent >= currentAchievement.m_amountTarget || (currentAchievement.m_state & Achievement.State.Pending) == Achievement.State.Pending))
		{
			currentAchievement.m_state |= Achievement.State.Pending;
			Achievements.AwardAchievement(currentAchievement.m_id, 100f, ref currentAchievement);
		}
	}

	private void SetTarget(ref Achievement currentAchievement)
	{
		if ((currentAchievement.m_state & Achievement.State.Completed) != Achievement.State.Completed)
		{
			PlayerStats.Stats currentStats = PlayerStats.GetCurrentStats();
			switch (currentAchievement.m_trackedType)
			{
			case PlayerStats.StatTypes.Int:
				currentAchievement.m_amountStart = currentStats.m_trackedStats[currentAchievement.m_trackedValue];
				currentAchievement.m_amountTarget = currentAchievement.m_amountStart + currentAchievement.m_amountNeeded;
				break;
			case PlayerStats.StatTypes.Long:
				currentAchievement.m_amountStart = currentStats.m_trackedLongStats[currentAchievement.m_trackedValue];
				currentAchievement.m_amountTarget = currentAchievement.m_amountStart + currentAchievement.m_amountNeeded;
				break;
			case PlayerStats.StatTypes.Float:
				currentAchievement.m_amountStart = (long)(currentStats.m_trackedDistances[currentAchievement.m_trackedValue] * toFloatDivision);
				currentAchievement.m_amountTarget = (long)((float)currentAchievement.m_amountStart + (float)currentAchievement.m_amountNeeded * toFloatDivision);
				break;
			}
			currentAchievement.m_amountCurrent = currentAchievement.m_amountStart;
		}
	}

	private void Event_OnNewGameStarted()
	{
		for (int i = 0; i < m_achievements.Length; i++)
		{
			if ((m_achievements[i].m_state & Achievement.State.Completed) != Achievement.State.Completed && (m_achievements[i].m_state & Achievement.State.ResetEachRun) == Achievement.State.ResetEachRun)
			{
				SetTarget(ref m_achievements[i]);
			}
		}
	}

	private void Event_OnGameFinished()
	{
		for (int i = 0; i < m_achievements.Length; i++)
		{
			CheckAchievement(ref m_achievements[i]);
			float progress = (float)(m_achievements[i].m_amountCurrent - m_achievements[i].m_amountStart) / (float)(m_achievements[i].m_amountTarget - m_achievements[i].m_amountStart) * 100f;
			Achievements.AwardAchievement(m_achievements[i].m_id, progress, ref m_achievements[i]);
		}
		UpdateAmazonPhoneWidget();
	}

	public void UpdateAmazonPhoneWidget()
	{
	}

	private void Event_PowerUpLeveledUp(PowerUps.Type pUp)
	{
		CheckAchievement(ref m_achievements[2]);
		float progress = (float)(m_achievements[2].m_amountCurrent - m_achievements[2].m_amountStart) / (float)(m_achievements[2].m_amountTarget - m_achievements[2].m_amountStart) * 100f;
		Achievements.AwardAchievement(m_achievements[2].m_id, progress, ref m_achievements[2]);
	}

	private void Event_OnGameDataSaveRequest()
	{
		string[] names = Enum.GetNames(typeof(Achievements.Types));
		for (int i = 0; i < names.Length; i++)
		{
			PropertyStore.Store("AchievementState_" + names[i], (int)m_achievements[i].m_state);
		}
	}

	private void Event_OnGameDataLoaded(ActiveProperties activeProperties)
	{
		string[] names = Enum.GetNames(typeof(Achievements.Types));
		for (int i = 0; i < names.Length; i++)
		{
			m_achievements[i].m_state |= (Achievement.State)activeProperties.GetInt("AchievementState_" + names[i]);
		}
	}
}
