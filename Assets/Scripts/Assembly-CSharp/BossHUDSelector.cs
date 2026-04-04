using UnityEngine;

public class BossHUDSelector : MonoBehaviour
{
	private const string ZazzName = "BOSS_NAME_ZAZZ";

	private const string EggmanName = "BOSS_NAME_EGGMAN";

	private const string ZazzLogo = "logo_lostworld";

	private const string EggmanLogo = "logo_sonicdash";

	[SerializeField]
	private UILabel m_bossName;

	[SerializeField]
	private UISprite m_bossLogo;

	private void Start()
	{
		EventDispatch.RegisterInterest("OnBossBattleIntroStart", this);
		EventDispatch.RegisterInterest("OnBossBattleOutroStart", this);
	}

	private void OnEnable()
	{
		UpdateDescription();
	}

	private void UpdateDescription()
	{
		bool flag = m_bossName != null;
		BossLoader.Bosses currentBoss = BossLoader.Instance().m_currentBoss;
		if (!(m_bossLogo != null))
		{
			return;
		}
		switch (currentBoss)
		{
		case BossLoader.Bosses.Eggman:
			m_bossLogo.spriteName = "logo_sonicdash";
			if (flag)
			{
				ChangeName("BOSS_NAME_EGGMAN");
			}
			break;
		case BossLoader.Bosses.Zazz:
			m_bossLogo.spriteName = "logo_lostworld";
			if (flag)
			{
				ChangeName("BOSS_NAME_ZAZZ");
			}
			break;
		}
	}

	private void ChangeName(string name)
	{
		LocalisedStringProperties component = m_bossName.GetComponent<LocalisedStringProperties>();
		component.SetLocalisationID(name);
		m_bossName.text = component.GetLocalisedString();
	}

	private void Event_OnBossBattleIntroStart()
	{
		UpdateDescription();
	}

	private void Event_OnBossBattleOutroStart()
	{
		UpdateDescription();
	}
}
