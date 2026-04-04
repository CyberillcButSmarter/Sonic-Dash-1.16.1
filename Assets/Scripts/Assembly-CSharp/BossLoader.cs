using System.Collections;
using UnityEngine;

public class BossLoader : MonoBehaviour
{
	public enum Bosses
	{
		Eggman = 0,
		Zazz = 1
	}

	private const string NextBossProperty = "NextBossProperty";

	public Bosses m_nextBoss;

	public Bosses m_currentBoss;

	private static BossLoader s_instance;

	public static bool ZazzBossActive { get; set; }

	public static bool EggmanBossActive { get; set; }

	public static BossLoader Instance()
	{
		return s_instance;
	}

	public static void ConfirmNextBoss()
	{
		if (s_instance.m_nextBoss == Bosses.Eggman)
		{
			if (!EggmanBossActive && ZazzBossActive)
			{
				s_instance.m_nextBoss = Bosses.Zazz;
			}
		}
		else if (s_instance.m_nextBoss == Bosses.Zazz && !ZazzBossActive && EggmanBossActive)
		{
			s_instance.m_nextBoss = Bosses.Eggman;
		}
	}

	private void Awake()
	{
		s_instance = this;
		EventDispatch.RegisterInterest("OnBossBattleEnd", this);
		EventDispatch.RegisterInterest("ResetGameState", this);
		EventDispatch.RegisterInterest("OnGameDataLoaded", this);
		EventDispatch.RegisterInterest("OnGameDataSaveRequest", this);
	}

	public IEnumerator LoadBoss()
	{
		if (m_nextBoss == Bosses.Zazz)
		{
			yield return Application.LoadLevelAdditiveAsync("s_boss_zazz");
			m_currentBoss = Bosses.Zazz;
			if (EggmanBossActive)
			{
				m_nextBoss = Bosses.Eggman;
			}
		}
		else if (m_nextBoss == Bosses.Eggman)
		{
			yield return Application.LoadLevelAdditiveAsync("s_boss_eggman");
			m_currentBoss = Bosses.Eggman;
			if (ZazzBossActive)
			{
				m_nextBoss = Bosses.Zazz;
			}
		}
	}

	public void DestroyBoss()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("BossRoot");
		if ((bool)gameObject)
		{
			Object.Destroy(gameObject);
		}
	}

	private void Event_OnBossBattleEnd()
	{
		DestroyBoss();
	}

	private void Event_ResetGameState(GameState.Mode resetState)
	{
		DestroyBoss();
	}

	private void Event_OnGameDataSaveRequest()
	{
		PropertyStore.Store("NextBossProperty", (int)m_nextBoss);
	}

	private void Event_OnGameDataLoaded(ActiveProperties activeProperties)
	{
		if (activeProperties.DoesPropertyExist("NextBossProperty"))
		{
			m_nextBoss = (Bosses)activeProperties.GetInt("NextBossProperty");
		}
		else
		{
			m_nextBoss = Bosses.Eggman;
		}
		ConfirmNextBoss();
	}
}
