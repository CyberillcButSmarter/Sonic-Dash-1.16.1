using UnityEngine;

public class HudContent_MissionReminder_Banner
{
	private int m_currentGroup;

	private UISlider m_progressionSlider;

	private GameObject m_completionIcon;

	private GameObject m_incompleteIcon;

	private UILabel m_description;

	private Color m_completedColour = Color.white;

	public HudContent_MissionReminder_Banner(int missionGroup, UILabel descriptionLabel, UISlider progressSlider, GameObject completionIcon, GameObject incompleteIcon, Color completedColour)
	{
		m_currentGroup = missionGroup;
		m_progressionSlider = progressSlider;
		m_description = descriptionLabel;
		m_completionIcon = completionIcon;
		m_incompleteIcon = incompleteIcon;
		m_completedColour = completedColour;
	}

	public void PopulateBanner()
	{
		MissionTracker.Mission thisMission = MissionTracker.GetActiveMission(m_currentGroup);
		bool flag = (thisMission.m_state & MissionTracker.Mission.State.Completed) == MissionTracker.Mission.State.Completed;
		bool flag2 = (thisMission.m_state & MissionTracker.Mission.State.ResetEachRun) == MissionTracker.Mission.State.ResetEachRun;
		m_completionIcon.SetActive(flag);
		m_incompleteIcon.SetActive(flag2 && !flag);
		m_progressionSlider.gameObject.SetActive(!flag2 && !flag);
		Color color = ((!flag) ? Color.white : m_completedColour);
		UIWidget[] componentsInChildren = m_completionIcon.transform.parent.GetComponentsInChildren<UIWidget>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].color = color;
		}
		float missionProgress = MissionUtils.GetMissionProgress(ref thisMission);
		m_progressionSlider.value = missionProgress;
		MissionTracker.GetMissionDescription(m_currentGroup, m_description);
	}
}
