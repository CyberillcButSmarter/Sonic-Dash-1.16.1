using UnityEngine;

public class HudContent_DCReminder : HudContent_PlayerGoals.Goal
{
	private const string ChallengePiecesRemaining = "DAILY_CHALLENGE_COLLECT_COUNT";

	private UILabel m_count;

	private UILabel m_timer;

	private MenuMeshMover[] m_peiceMovers;

	private MenuMeshBobble[] m_peiceBobbler;

	private MeshRenderer[] m_jigsawPieces;

	public HudContent_DCReminder(UILabel timer, UILabel count, MeshRenderer[] jigsawPieces)
	{
		m_timer = timer;
		m_count = count;
		m_jigsawPieces = jigsawPieces;
	}

	public override void Populate()
	{
		CacheDisplayProperties();
		SetJigsawState();
		UpdateLabels();
	}

	public override void Update()
	{
		UpdateLabels();
	}

	public override bool IsValidGoal()
	{
		return !DCs.AllPiecesCollected();
	}

	private void UpdateLabels()
	{
		string format = LanguageStrings.First.GetString("DAILY_CHALLENGE_COLLECT_COUNT");
		m_count.text = string.Format(format, DCs.GetNumPiecesRemaining());
		m_timer.text = DCs.GetTimeRemaining();
	}

	private void CacheDisplayProperties()
	{
		if (m_peiceBobbler == null || m_peiceMovers == null)
		{
			m_peiceMovers = new MenuMeshMover[m_jigsawPieces.Length];
			m_peiceBobbler = new MenuMeshBobble[m_jigsawPieces.Length];
			for (int i = 0; i < m_jigsawPieces.Length; i++)
			{
				m_peiceMovers[i] = m_jigsawPieces[i].GetComponent<MenuMeshMover>();
				m_peiceBobbler[i] = m_jigsawPieces[i].GetComponent<MenuMeshBobble>();
			}
		}
	}

	private void SetJigsawState()
	{
		bool flag = DCs.AllPiecesCollected();
		bool[] pieces = DCs.GetPieces();
		for (int i = 0; i < m_jigsawPieces.Length; i++)
		{
			float a = ((!pieces[i]) ? 0.3f : 1f);
			Material material = m_jigsawPieces[i].material;
			Color color = material.color;
			color.a = a;
			material.color = color;
			m_peiceMovers[i].enabled = !flag;
			m_peiceBobbler[i].enabled = !flag;
		}
	}
}
