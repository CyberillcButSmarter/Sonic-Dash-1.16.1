using System;
using UnityEngine;

public class HudContent_EspioSpecialEvent
{
	[Flags]
	private enum State
	{
		None = 0,
		Visible = 1,
		Showing = 2
	}

	public GameObject espioToken;

	public GameObject questionToken;

	private State m_state;

	private GameObject m_displayRoot;

	private GameObject m_displayTrigger;

	private float m_displayTimer;

	private float m_displayDuration;

	public HudContent_EspioSpecialEvent(GameObject displayRoot, GameObject displayTrigger)
	{
		m_displayRoot = displayRoot;
		m_displayTrigger = displayTrigger;
		m_displayDuration = 5f;
		EventDispatch.RegisterInterest("OnEspioTokenHuntStart", this);
		EventDispatch.RegisterInterest("OnMotionSpringGesturesStart", this);
	}

	public void Update()
	{
		if ((m_state & State.Visible) == State.Visible)
		{
			UpdateDisplayTimer();
		}
	}

	public void OnResetOnNewGame()
	{
		if ((m_state & State.Showing) == State.Showing)
		{
			Hide();
		}
		m_displayTimer = 0f;
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

	private void Display()
	{
		m_displayTrigger.SendMessage("OnClick", SendMessageOptions.RequireReceiver);
		m_state |= State.Showing;
		m_displayTimer = 0f;
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
		m_displayTimer += Time.deltaTime;
		if (m_displayTimer > m_displayDuration)
		{
			Hide();
		}
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

	private void Event_OnMotionSpringGesturesStart()
	{
		GameObject gameObject = FindChildByName("Label (Title - Special)", m_displayRoot);
		GameObject gameObject2 = FindChildByName("Label (Info - Special)", m_displayRoot);
		if (gameObject != null)
		{
			NGUITools.SetActive(gameObject, true);
		}
		if (gameObject2 != null)
		{
			NGUITools.SetActive(gameObject2, true);
		}
		GameObject gameObject3 = FindChildByName("Label (Title - FindTokens)", m_displayRoot);
		GameObject gameObject4 = FindChildByName("Label (Info - FindTokens)", m_displayRoot);
		if (gameObject3 != null)
		{
			NGUITools.SetActive(gameObject3, false);
		}
		if (gameObject4 != null)
		{
			NGUITools.SetActive(gameObject4, false);
		}
		espioToken = FindChildByName("SlicedSprite (espio token)", m_displayRoot);
		questionToken = FindChildByName("SlicedSprite (question token)", m_displayRoot);
		if (espioToken != null)
		{
			NGUITools.SetActive(espioToken, false);
		}
		if (questionToken != null)
		{
			NGUITools.SetActive(questionToken, false);
		}
		Display();
	}

	private void Event_OnEspioTokenHuntStart()
	{
		GameObject gameObject = FindChildByName("Label (Title - Special)", m_displayRoot);
		GameObject gameObject2 = FindChildByName("Label (Info - Special)", m_displayRoot);
		if (gameObject != null)
		{
			NGUITools.SetActive(gameObject, false);
		}
		if (gameObject2 != null)
		{
			NGUITools.SetActive(gameObject2, false);
		}
		GameObject gameObject3 = FindChildByName("Label (Title - FindTokens)", m_displayRoot);
		GameObject gameObject4 = FindChildByName("Label (Info - FindTokens)", m_displayRoot);
		if (gameObject3 != null)
		{
			NGUITools.SetActive(gameObject3, true);
		}
		if (gameObject4 != null)
		{
			NGUITools.SetActive(gameObject4, true);
		}
		bool flag = !Characters.CharacterUnlocked(Characters.Type.Espio);
		espioToken = FindChildByName("SlicedSprite (espio token)", m_displayRoot);
		questionToken = FindChildByName("SlicedSprite (question token)", m_displayRoot);
		if (espioToken != null)
		{
			NGUITools.SetActive(espioToken, flag);
		}
		if (questionToken != null)
		{
			NGUITools.SetActive(questionToken, !flag);
		}
		Display();
	}
}
