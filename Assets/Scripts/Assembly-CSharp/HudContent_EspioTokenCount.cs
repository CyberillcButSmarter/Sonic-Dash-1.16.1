using System;
using UnityEngine;

public class HudContent_EspioTokenCount
{
	[Flags]
	private enum State
	{
		None = 0,
		Visible = 1,
		Showing = 2
	}

	private State m_state;

	private GameObject m_displayRoot;

	private GameObject m_displayTrigger;

	private UILabel m_counterLabel;

	private float m_displayTimer;

	private float m_displayDuration;

	private bool m_displayTimerActive;

	private int m_displayedTokenCount = -1;

	public HudContent_EspioTokenCount(GameObject displayRoot, GameObject displayTrigger)
	{
		m_displayRoot = displayRoot;
		m_displayTrigger = displayTrigger;
		m_displayTimerActive = false;
		m_displayDuration = 2.5f;
		EventDispatch.RegisterInterest("OnEspioTokenHuntStart", this);
		EventDispatch.RegisterInterest("OnEspioTokenHuntEnd", this);
		m_counterLabel = FindChildByName("Token Count (label)", m_displayRoot).GetComponent<UILabel>();
	}

	private GameObject FindChildByName(string strName, GameObject objRoot)
	{
		if (objRoot.name == strName)
		{
			return objRoot;
		}
		Transform transform = objRoot.transform;
		int childCount = transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			GameObject gameObject = FindChildByName(strName, transform.GetChild(i).gameObject);
			if (gameObject != null)
			{
				return gameObject;
			}
		}
		return null;
	}

	public void Update()
	{
		if ((m_state & State.Visible) == State.Visible)
		{
			UpdateDisplayTimer();
			UpdateTokenCount();
		}
	}

	public void OnResetOnNewGame()
	{
		if ((m_state & State.Showing) == State.Showing)
		{
			Hide();
		}
		m_displayTimer = 0f;
		m_displayTimerActive = false;
		m_state = State.None;
	}

	public void OnPauseStateChanged(bool paused)
	{
		Hide();
	}

	public void OnPlayerDeath()
	{
		Hide();
	}

	public void HudVisible(bool visible)
	{
		if (visible)
		{
			m_state |= State.Visible;
		}
		else
		{
			m_state &= ~State.Visible;
		}
	}

	private void UpdateTokenCount()
	{
		int tokensNeeded = EspioSpecialEvent.GetTokensNeeded();
		int totalTokensNeeded = EspioSpecialEvent.GetTotalTokensNeeded();
		int num = totalTokensNeeded - tokensNeeded;
		if (num != m_displayedTokenCount)
		{
			m_displayedTokenCount = Mathf.Clamp(num, 0, totalTokensNeeded);
			m_counterLabel.text = string.Format("{0}/{1}", num, totalTokensNeeded);
		}
	}

	private void Display()
	{
		if (EspioSpecialEvent.GetRewardType() == EspioSpecialEvent.RewardType.Espio)
		{
			m_displayTrigger.SendMessage("OnClick", SendMessageOptions.RequireReceiver);
			m_state |= State.Showing;
			m_displayTimer = 0f;
			m_displayTimerActive = false;
			m_displayedTokenCount = -1;
			UpdateTokenCount();
		}
	}

	private void Hide()
	{
		if ((m_state & State.Showing) == State.Showing)
		{
			m_displayTrigger.SendMessage("OnClick", SendMessageOptions.RequireReceiver);
			m_state &= ~State.Showing;
			m_displayTimer = 0f;
		}
	}

	private void UpdateDisplayTimer()
	{
		if (m_displayTimerActive)
		{
			m_displayTimer += Time.deltaTime;
			if (m_displayTimer > m_displayDuration)
			{
				Hide();
			}
		}
	}

	private void Event_OnEspioTokenHuntStart()
	{
		Display();
	}

	private void Event_OnEspioTokenHuntEnd()
	{
		m_displayTimerActive = true;
	}
}
