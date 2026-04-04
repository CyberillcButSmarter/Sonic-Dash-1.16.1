using System;
using UnityEngine;

public class OfferRegion_MainMenu : MonoBehaviour
{
	[Flags]
	private enum State
	{
		InitialDone = 1
	}

	private State m_state;

	[SerializeField]
	private OfferRegion_Timed m_onGameFlow;

	private void OnEnable()
	{
		if ((m_state & State.InitialDone) == State.InitialDone && m_onGameFlow != null)
		{
			m_onGameFlow.Visit();
		}
		m_state |= State.InitialDone;
	}
}
