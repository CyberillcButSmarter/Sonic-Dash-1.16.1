public class GameOverState
{
	private GuiTrigger m_gameOverScreen;

	private GuiTrigger m_continueScreen;

	private int m_currentContinueCost;

	private int[] m_continueCost;

	private bool m_OnContinueOKReceived;

	private bool m_OnContinueOKFree;

	private bool m_VideoReviveRequested;

	public GameOverState(GuiTrigger gameOverScreen, GuiTrigger continueScreen, int[] continueCost)
	{
		m_gameOverScreen = gameOverScreen;
		m_continueScreen = continueScreen;
		m_continueCost = continueCost;
		EventDispatch.RegisterInterest("OnNewGameStarted", this);
		EventDispatch.RegisterInterest("OnContinueGameCancel", this);
		EventDispatch.RegisterInterest("OnContinueGameOk", this);
		EventDispatch.RegisterInterest("3rdPartyInactive", this);
		EventDispatch.RegisterInterest("VideoReviveRequested", this);
	}

	public void TriggerGameOver(bool bAllowRespawn)
	{
		if (!bAllowRespawn)
		{
			EndGame(true);
		}
		else
		{
			PromptToContinue();
		}
	}

	private void ContinueGame(bool freeRevive)
	{
		GameState.RequestMode(GameState.Mode.Game);
		Sonic.Tracker.Resurrect(freeRevive);
		if (!freeRevive)
		{
			int currentContinueCost = GetCurrentContinueCost();
			PowerUpsInventory.ModifyPowerUpStock(PowerUps.Type.Respawn, -currentContinueCost);
			PlayerStats.IncreaseStat(PlayerStats.StatNames.RevivesUsed_Total, currentContinueCost);
			PlayerStats.IncreaseStat(PlayerStats.StatNames.RevivesUsed_Session, currentContinueCost);
			PlayerStats.IncreaseStat(PlayerStats.StatNames.RevivesUsed_Run, currentContinueCost);
			m_currentContinueCost++;
		}
	}

	private void EndGame(bool instantEnd)
	{
		if (instantEnd)
		{
			MenuStack.RequestPage = m_gameOverScreen;
			GameState.RequestMode(GameState.Mode.PauseMenu);
		}
		else
		{
			MenuStack.MoveToPage(m_gameOverScreen, true);
		}
		EventDispatch.GenerateEvent("OnGameFinished");
		PropertyStore.Save();
	}

	private void PromptToContinue()
	{
		int currentContinueCost = GetCurrentContinueCost();
		m_VideoReviveRequested = false;
		m_OnContinueOKReceived = false;
		MenuReviveScreen.RevivesRequired = currentContinueCost;
		MenuRevivePurchase.RevivesRequired = currentContinueCost;
		MenuStack.RequestPage = m_continueScreen;
		GameState.RequestMode(GameState.Mode.PauseMenu);
	}

	private int GetCurrentContinueCost()
	{
		if (m_currentContinueCost >= m_continueCost.Length)
		{
			return m_continueCost[m_continueCost.Length - 1];
		}
		return m_continueCost[m_currentContinueCost];
	}

	private void Event_OnNewGameStarted()
	{
		m_currentContinueCost = 0;
		m_VideoReviveRequested = false;
		m_OnContinueOKReceived = false;
	}

	private void Event_OnContinueGameCancel()
	{
		m_VideoReviveRequested = false;
		m_OnContinueOKReceived = false;
		EndGame(false);
	}

	private void Event_VideoReviveRequested(bool failed)
	{
		if (!failed)
		{
			m_VideoReviveRequested = true;
		}
	}

	private void Event_OnContinueGameOk(bool freeRevive)
	{
		if (m_VideoReviveRequested)
		{
			m_OnContinueOKReceived = true;
			m_OnContinueOKFree = freeRevive;
			m_VideoReviveRequested = false;
		}
		else
		{
			ContinueGame(freeRevive);
		}
	}

	private void Event_3rdPartyInactive()
	{
		if (m_OnContinueOKReceived)
		{
			m_OnContinueOKReceived = false;
			ContinueGame(m_OnContinueOKFree);
		}
	}
}
