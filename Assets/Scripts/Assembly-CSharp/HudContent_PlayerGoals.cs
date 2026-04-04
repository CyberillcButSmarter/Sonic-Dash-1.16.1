using System;
using UnityEngine;

public class HudContent_PlayerGoals
{
	public abstract class Goal
	{
		public abstract void Populate();

		public abstract void Update();

		public abstract bool IsValidGoal();
	}

	public class GoalContent
	{
		private Goal m_goal;

		private GameObject m_trigger;

		public int Priority { get; set; }

		public GoalContent(Goal goal, GameObject trigger, int priority)
		{
			m_goal = goal;
			m_trigger = trigger;
			Priority = priority;
		}

		public bool IsValidGoal()
		{
			return m_goal.IsValidGoal();
		}

		public void Update()
		{
			m_goal.Update();
		}

		public void Show()
		{
			m_trigger.SendMessage("OnClick", SendMessageOptions.RequireReceiver);
			m_goal.Populate();
		}

		public void Hide()
		{
			m_trigger.SendMessage("OnClick", SendMessageOptions.RequireReceiver);
		}
	}

	[Flags]
	private enum State
	{
		None = 0,
		Showing = 1
	}

	private AudioClip m_displayAudio;

	private float m_displayTimer;

	private float m_displayDuration;

	private static GoalContent[] s_goals;

	private GoalContent m_currentGoal;

	private State m_state;

	public static bool CanShow
	{
		get
		{
			if (ABTesting.GetTestValueAsInt(ABTesting.Tests.MSG_PreRunDialog) == 0)
			{
				return false;
			}
			if (TutorialSystem.instance().isTrackTutorialEnabled())
			{
				return false;
			}
			return AnyGoals;
		}
	}

	public static bool AnyGoals
	{
		get
		{
			for (int i = 0; i < s_goals.Length; i++)
			{
				if (s_goals[i].IsValidGoal())
				{
					return true;
				}
			}
			return false;
		}
	}

	public HudContent_PlayerGoals(GameObject missionsTrigger, GameObject dcTrigger, GameObject gcTrigger, float displayDuration, AudioClip displayAudio, UILabel[] labels, UISlider[] progressSliders, GameObject[] completionIcons, GameObject[] incompleteIcons, Color completedColour, UILabel dcTimer, UILabel dcCount, MeshRenderer[] jigsawPeices)
	{
		m_displayAudio = displayAudio;
		HudContent_MissionReminder goal = new HudContent_MissionReminder(labels, progressSliders, completionIcons, incompleteIcons, completedColour);
		HudContent_DCReminder goal2 = new HudContent_DCReminder(dcTimer, dcCount, jigsawPeices);
		HudContent_GCReminder goal3 = new HudContent_GCReminder();
		m_displayDuration = displayDuration;
		s_goals = new GoalContent[3]
		{
			new GoalContent(goal, missionsTrigger, 1),
			new GoalContent(goal2, dcTrigger, 2),
			new GoalContent(goal3, gcTrigger, 0)
		};
	}

	public void Update()
	{
		if ((m_state & State.Showing) == State.Showing)
		{
			m_currentGoal.Update();
			UpdateDisplayTimer();
		}
	}

	public void OnResetOnNewGame()
	{
		if (CanShow)
		{
			Display();
		}
	}

	public void OnPauseStateChanged(bool paused)
	{
	}

	public void OnPlayerDeath()
	{
	}

	public void HudVisible(bool visible)
	{
	}

	private void Display()
	{
		if ((m_state & State.Showing) == State.Showing)
		{
			return;
		}
		Audio.PlayClip(m_displayAudio, false);
		m_state |= State.Showing;
		m_displayTimer = 0f;
		Array.Sort(s_goals, (GoalContent a, GoalContent b) => a.Priority - b.Priority);
		for (int num = 0; num < s_goals.Length; num++)
		{
			if (s_goals[num].IsValidGoal())
			{
				m_currentGoal = s_goals[num];
				if (num < s_goals.Length - 1)
				{
					m_currentGoal.Priority = s_goals[s_goals.Length - 1].Priority + 1;
				}
				break;
			}
		}
		m_currentGoal.Show();
	}

	private void Hide()
	{
		if ((m_state & State.Showing) == State.Showing)
		{
			Audio.PlayClip(m_displayAudio, false);
			m_currentGoal.Hide();
			m_state &= ~State.Showing;
			m_displayTimer = 0f;
		}
	}

	private void UpdateDisplayTimer()
	{
		m_displayTimer += Time.deltaTime;
		if (m_displayTimer > m_displayDuration)
		{
			Hide();
		}
	}
}
