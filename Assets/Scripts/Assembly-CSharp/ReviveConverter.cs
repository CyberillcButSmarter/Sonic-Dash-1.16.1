using System;
using UnityEngine;

public class ReviveConverter : MonoBehaviour
{
	[Flags]
	private enum State
	{
		ABReady = 1,
		StoreReady = 2
	}

	private State m_state;

	private void Start()
	{
		EventDispatch.RegisterInterest("ABTestingReady", this);
		EventDispatch.RegisterInterest("OnStoreInitialised", this);
		EventDispatch.RegisterInterest("MainMenuActive", this);
	}

	private void RemoveReviveStoreEntries()
	{
		if ((m_state & State.ABReady) != 0 && (m_state & State.StoreReady) != 0 && ABTesting.GetTestValueAsInt(ABTesting.Tests.REVIVE_RSRRevive) != 0)
		{
			StoreUtils.HideStoreEntry("Respawn", true);
			StoreUtils.HideStoreEntry("Respawn Multiple", true);
			StoreUtils.HideStoreEntry("Respawn Multiple 25", true);
			StoreUtils.HideStoreEntry("Respawn Multiple 40", true);
		}
	}

	private void Event_ABTestingReady()
	{
		m_state |= State.ABReady;
		if (ABTesting.GetTestValueAsInt(ABTesting.Tests.REVIVE_RSRRevive) != 0)
		{
			DCRewards.ReplaceReward("DailyC Day2", "DailyC Day2 (Revive Replacement)");
			DCRewards.ReplaceReward("DailyC Day5 Reward2", "DailyC Day5 Reward2 (Revive Replacement)");
			WheelOfFortuneRewards.ReplaceReward("WOF Reward1", "WOF Reward1 (Revive Replacement)");
			RemoveReviveStoreEntries();
		}
	}

	private void Event_OnStoreInitialised()
	{
		m_state |= State.StoreReady;
		RemoveReviveStoreEntries();
	}

	private void Event_MainMenuActive()
	{
		if ((m_state & State.ABReady) != 0 && ABTesting.GetTestValueAsInt(ABTesting.Tests.REVIVE_RSRRevive) != 0)
		{
			int powerUpCount = PowerUpsInventory.GetPowerUpCount(PowerUps.Type.Respawn);
			if (powerUpCount != 0)
			{
				PowerUpsInventory.ModifyPowerUpStock(PowerUps.Type.Respawn, -powerUpCount);
				EventDispatch.GenerateEvent("OnStarRingsAwarded", new GameAnalytics.RSRAnalyticsParam(powerUpCount, GameAnalytics.RingsRecievedReason.ReviveConversion));
				DialogContent_GeneralInfo.Display(DialogContent_GeneralInfo.Type.ReviveConversion);
			}
		}
	}
}
