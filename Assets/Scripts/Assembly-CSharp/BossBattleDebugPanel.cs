using UnityEngine;

public class BossBattleDebugPanel : MonoBehaviour
{
	[SerializeField]
	private bool m_showAlways;

	[SerializeField]
	private UILabel m_descLabel1;

	[SerializeField]
	private UILabel m_descLabel2;

	[SerializeField]
	private UITexture m_background;

	private void Update()
	{
		BossBattleSystem bossBattleSystem = BossBattleSystem.Instance();
		if ((bool)bossBattleSystem && bossBattleSystem.ShowDebug && bossBattleSystem.IsEnabled())
		{
			if (m_descLabel1 != null)
			{
				m_descLabel1.enabled = true;
				m_descLabel1.text = string.Format("{0} - {1}", bossBattleSystem.GetCurrentPhase(), bossBattleSystem.CurrentPhase.Name);
			}
			if (m_descLabel2 != null)
			{
				m_descLabel2.enabled = true;
				if (BossBattleSystem.Instance().CurrentPhase.HasTrack())
				{
					m_descLabel2.text = string.Format("{0:0.0}m / {1:0.0}m - {2:0.0}m", bossBattleSystem.PhaseTrackDistance, bossBattleSystem.CurrentPhase.TrackDistance, bossBattleSystem.TotalTrackDistance);
				}
				else
				{
					m_descLabel2.text = string.Format("{0:0.0}s / {1:0.0}s - {2:0.0}s", bossBattleSystem.PhaseTime, bossBattleSystem.CurrentPhase.Duration, bossBattleSystem.TotalTime);
				}
			}
			if (m_background != null)
			{
				m_background.enabled = true;
			}
		}
		else if (m_showAlways && Sonic.Tracker != null && Sonic.Tracker.Track != null)
		{
			TrackGenerator trackGenerator = Sonic.Tracker.Track as TrackGenerator;
			float bossBattleRand = trackGenerator.BossBattleRand;
			float bossBattleChance = trackGenerator.BossBattleChance;
			if (m_descLabel1 != null)
			{
				m_descLabel1.enabled = true;
				m_descLabel1.text = string.Format("{0:0.00} / {1:0.00}", bossBattleRand, bossBattleChance);
			}
			if (m_descLabel2 != null)
			{
				m_descLabel2.enabled = true;
				m_descLabel2.text = ((!(bossBattleRand < bossBattleChance)) ? "No chance on next springs" : "Boss on next springs!");
			}
			if (m_background != null)
			{
				m_background.enabled = true;
			}
		}
		else
		{
			if (m_descLabel1 != null)
			{
				m_descLabel1.enabled = false;
			}
			if (m_descLabel2 != null)
			{
				m_descLabel2.enabled = false;
			}
			if (m_background != null)
			{
				m_background.enabled = false;
			}
		}
	}
}
