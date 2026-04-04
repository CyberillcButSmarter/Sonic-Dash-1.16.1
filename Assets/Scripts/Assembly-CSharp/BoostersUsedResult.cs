using UnityEngine;

public class BoostersUsedResult : MonoBehaviour
{
	private int[] m_selectedBoosters;

	private bool m_getNewBoosters = true;

	[SerializeField]
	private UISprite m_booster1;

	[SerializeField]
	private UISprite m_booster2;

	[SerializeField]
	private UISprite m_booster3;

	private static string[] s_boosterSpriteNames = new string[5] { "icon-boostershud-enemycombo", "icon-boostershud-ringstreak", "icon-boostershud-springbonus", "icon-boostershud-goldnik", "icon-boostershud-scorebonus" };

	private void Start()
	{
		EventDispatch.RegisterInterest("OnNewGameStarted", this);
	}

	private void Event_OnNewGameStarted()
	{
		m_getNewBoosters = true;
	}

	private void OnEnable()
	{
		string empty = string.Empty;
		if (m_getNewBoosters)
		{
			m_selectedBoosters = (int[])Boosters.GetBoostersSelected.Clone();
			m_getNewBoosters = false;
		}
		empty = GetSpriteNameForBooster((PowerUps.Type)m_selectedBoosters[0]);
		if (empty != string.Empty)
		{
			m_booster1.enabled = true;
			m_booster1.spriteName = empty;
			m_booster1.MakePixelPerfect();
		}
		else
		{
			m_booster1.enabled = false;
		}
		empty = GetSpriteNameForBooster((PowerUps.Type)m_selectedBoosters[1]);
		if (empty != string.Empty)
		{
			m_booster2.enabled = true;
			m_booster2.spriteName = empty;
			m_booster2.MakePixelPerfect();
		}
		else
		{
			m_booster2.enabled = false;
		}
		empty = GetSpriteNameForBooster((PowerUps.Type)m_selectedBoosters[2]);
		if (empty != string.Empty)
		{
			m_booster3.enabled = true;
			m_booster3.spriteName = empty;
			m_booster3.MakePixelPerfect();
		}
		else
		{
			m_booster3.enabled = false;
		}
	}

	private int BoosterSlot(PowerUps.Type booster)
	{
		for (int i = 0; i < m_selectedBoosters.Length; i++)
		{
			if (m_selectedBoosters[i] == (int)booster)
			{
				return i;
			}
		}
		return -1;
	}

	private string GetSpriteNameForBooster(PowerUps.Type booster)
	{
		switch (booster)
		{
		case PowerUps.Type.Booster_EnemyComboBonus:
			return s_boosterSpriteNames[0];
		case PowerUps.Type.Booster_RingStreakBonus:
			return s_boosterSpriteNames[1];
		case PowerUps.Type.Booster_SpringBonus:
			return s_boosterSpriteNames[2];
		case PowerUps.Type.Booster_GoldenEnemy:
			return s_boosterSpriteNames[3];
		case PowerUps.Type.Booster_ScoreMultiplier:
			return s_boosterSpriteNames[4];
		default:
			return string.Empty;
		}
	}
}
