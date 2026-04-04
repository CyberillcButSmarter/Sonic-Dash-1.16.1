using System;
using UnityEngine;

public class Boosters : MonoBehaviour
{
	public const int NumberOfBoosters = 5;

	public const int BoostersSlots = 3;

	public const int None = -1;

	private const string BoosterTypeStart = "Booster_";

	private const string BoosterMultiplierSpriteName = "BoosterMultiplier_";

	private const int GoldenEnemyArraySize = 100;

	private static Boosters s_singleton;

	private static int[] s_selectedBoosters = new int[3] { -1, -1, -1 };

	private bool[] m_goldenEnemies = new bool[100];

	private int m_goldenEnemyCounter;

	private int[] m_boosterMultiplier = new int[5] { 1, 1, 1, 1, 1 };

	[SerializeField]
	private int m_springBonusScore = 500;

	[SerializeField]
	private Mesh m_meshBooster_SpringBonus;

	[SerializeField]
	private float m_enemyComboScoreMultiplier = 2f;

	[SerializeField]
	private Mesh m_meshBooster_EnemyComboBonus;

	[SerializeField]
	private int m_ringStreakBonusScore = 500;

	[SerializeField]
	private Mesh m_meshBooster_RingStreakBonus;

	[SerializeField]
	private float m_ScoreMultiplier = 2f;

	[SerializeField]
	private Mesh m_meshBooster_ScoreMultiplier;

	[SerializeField]
	private float m_goldenEnemyScoreMultipler = 2f;

	[SerializeField]
	private int m_goldenEnemyPercentage = 20;

	[SerializeField]
	private Mesh m_meshBooster_GoldenEnemy;

	public static uint SpringBonusScore
	{
		get
		{
			return (uint)(s_singleton.m_springBonusScore * GetBoosterMultiplier(PowerUps.Type.Booster_SpringBonus));
		}
	}

	public static float GoldenEnemyScoreMultipler
	{
		get
		{
			return s_singleton.m_goldenEnemyScoreMultipler * (float)GetBoosterMultiplier(PowerUps.Type.Booster_GoldenEnemy);
		}
	}

	public static float EnemyComboScoreMultiplier
	{
		get
		{
			return s_singleton.m_enemyComboScoreMultiplier * (float)GetBoosterMultiplier(PowerUps.Type.Booster_EnemyComboBonus);
		}
	}

	public static float ScoreMultiplier
	{
		get
		{
			return s_singleton.m_ScoreMultiplier * (float)GetBoosterMultiplier(PowerUps.Type.Booster_ScoreMultiplier);
		}
	}

	public static uint RingStreakBonusScore
	{
		get
		{
			return (uint)(s_singleton.m_ringStreakBonusScore * GetBoosterMultiplier(PowerUps.Type.Booster_RingStreakBonus));
		}
	}

	public static bool IsNextEnemyGolden
	{
		get
		{
			return s_singleton.GetNextEnemyIsGolden();
		}
	}

	public static int[] GetBoostersSelected
	{
		get
		{
			return s_selectedBoosters;
		}
	}

	public static int GetNumberOfBoostersSelected()
	{
		int num = 0;
		for (int i = 0; i < s_selectedBoosters.Length; i++)
		{
			if (s_selectedBoosters[i] != -1)
			{
				num++;
			}
		}
		return num;
	}

	public static int GetBoosterMultiplier(PowerUps.Type booster)
	{
		int num = s_singleton.BoosterIndex(booster);
		return s_singleton.m_boosterMultiplier[num];
	}

	public static int SelectBooster(PowerUps.Type booster)
	{
		if (!booster.ToString().StartsWith("Booster_"))
		{
			return -1;
		}
		if (PowerUpsInventory.GetPowerUpCount(booster) < 1)
		{
			return -1;
		}
		int emptySlot = GetEmptySlot();
		if (emptySlot != -1)
		{
			PowerUpsInventory.ModifyPowerUpStock(booster, -1);
			s_selectedBoosters[emptySlot] = (int)booster;
		}
		return emptySlot;
	}

	public static void ClearSelected()
	{
		for (int i = 0; i < 3; i++)
		{
			s_selectedBoosters[i] = -1;
		}
	}

	public static bool IsBoosterSelected(PowerUps.Type booster)
	{
		for (int i = 0; i < 3; i++)
		{
			if (s_selectedBoosters[i] == (int)booster)
			{
				return true;
			}
		}
		return false;
	}

	private static int GetEmptySlot()
	{
		for (int i = 0; i < 3; i++)
		{
			if (s_selectedBoosters[i] < 0)
			{
				return i;
			}
		}
		return -1;
	}

	private bool GetNextEnemyIsGolden()
	{
		if (!IsBoosterSelected(PowerUps.Type.Booster_GoldenEnemy))
		{
			return false;
		}
		m_goldenEnemyCounter++;
		return m_goldenEnemies[m_goldenEnemyCounter % 100];
	}

	private void ResetGoldenEnemies()
	{
		for (int i = 0; i < 100; i++)
		{
			m_goldenEnemies[i] = false;
		}
		int num = 3;
		for (int j = 0; j < Math.Min(100, m_goldenEnemyPercentage); j++)
		{
			m_goldenEnemies[num] = true;
			num = (num + 29) % 100;
			while (m_goldenEnemies[num])
			{
				num = (num + 3) % 100;
			}
		}
	}

	private void Start()
	{
		s_singleton = this;
		EventDispatch.RegisterInterest("OnGameFinished", this);
		EventDispatch.RegisterInterest("FeatureStateReady", this);
		ResetGoldenEnemies();
	}

	private int GetMultiplierFromStateFile(PowerUps.Type booster)
	{
		string propertyName = string.Format("{0}{1}", booster.ToString(), "_Multiplier");
		LSON.Property stateProperty = FeatureState.GetStateProperty("events", propertyName);
		if (stateProperty == null)
		{
			return 1;
		}
		int intValue = 1;
		if (!LSONProperties.AsInt(stateProperty, out intValue))
		{
			return 1;
		}
		return intValue;
	}

	private int BoosterIndex(PowerUps.Type booster)
	{
		return (int)(booster - 11);
	}

	private void Event_OnGameFinished()
	{
		ResetGoldenEnemies();
	}

	private void Event_FeatureStateReady()
	{
		int num = BoosterIndex(PowerUps.Type.Booster_EnemyComboBonus);
		m_boosterMultiplier[num] = GetMultiplierFromStateFile(PowerUps.Type.Booster_EnemyComboBonus);
		num = BoosterIndex(PowerUps.Type.Booster_GoldenEnemy);
		m_boosterMultiplier[num] = GetMultiplierFromStateFile(PowerUps.Type.Booster_GoldenEnemy);
		num = BoosterIndex(PowerUps.Type.Booster_RingStreakBonus);
		m_boosterMultiplier[num] = GetMultiplierFromStateFile(PowerUps.Type.Booster_RingStreakBonus);
		num = BoosterIndex(PowerUps.Type.Booster_ScoreMultiplier);
		m_boosterMultiplier[num] = GetMultiplierFromStateFile(PowerUps.Type.Booster_ScoreMultiplier);
		num = BoosterIndex(PowerUps.Type.Booster_SpringBonus);
		m_boosterMultiplier[num] = GetMultiplierFromStateFile(PowerUps.Type.Booster_SpringBonus);
		for (int i = 0; i < m_boosterMultiplier.Length; i++)
		{
			if (m_boosterMultiplier[i] < 1)
			{
				m_boosterMultiplier[i] = 1;
			}
		}
	}
}
