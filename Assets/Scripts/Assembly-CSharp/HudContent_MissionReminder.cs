using UnityEngine;

public class HudContent_MissionReminder : HudContent_PlayerGoals.Goal
{
	private HudContent_MissionReminder_Banner[] m_banners;

	public HudContent_MissionReminder(UILabel[] labels, UISlider[] progressSliders, GameObject[] completionIcons, GameObject[] incompleteIcons, Color completedColour)
	{
		m_banners = new HudContent_MissionReminder_Banner[3];
		for (int i = 0; i < 3; i++)
		{
			m_banners[i] = new HudContent_MissionReminder_Banner(i, labels[i], progressSliders[i], completionIcons[i], incompleteIcons[i], completedColour);
		}
	}

	public override void Populate()
	{
		for (int i = 0; i < m_banners.Length; i++)
		{
			m_banners[i].PopulateBanner();
		}
	}

	public override void Update()
	{
	}

	public override bool IsValidGoal()
	{
		return !MissionTracker.AllMissionsComplete();
	}
}
