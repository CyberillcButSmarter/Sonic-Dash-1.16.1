using System;
using UnityEngine;

public class MenuRevivePurchase : MonoBehaviour
{
	private bool m_updateReviveCount;

	private int m_lastRevivesNeeded = int.MaxValue;

	[SerializeField]
	private GuiTrigger m_continuePage;

	[SerializeField]
	private UILabel m_reviveTokensNeeded;

	public static int RevivesRequired { private get; set; }

	private void Start()
	{
		m_updateReviveCount = ABTesting.GetTestValueAsInt(ABTesting.Tests.REVIVE_SimpleStore) == 1;
	}

	private void Update()
	{
		if (m_updateReviveCount)
		{
			SetRequiredTokenDisplay();
		}
	}

	private void OnEnable()
	{
		m_lastRevivesNeeded = int.MaxValue;
		GameAnalytics.SetPurchaseLocation(GameAnalytics.PurchaseLocations.ContinueScreen);
		SetRequiredTokenDisplay();
		EventDispatch.RegisterInterest("OnStorePurchaseCompleted", this, EventDispatch.Priority.Lowest);
	}

	private void OnDisable()
	{
		EventDispatch.UnregisterInterest("OnStorePurchaseCompleted", this);
	}

	private void SetRequiredTokenDisplay()
	{
		if (ABTesting.GetTestValueAsInt(ABTesting.Tests.REVIVE_SimpleStore) != 0)
		{
			int powerUpCount = PowerUpsInventory.GetPowerUpCount(PowerUps.Type.Respawn);
			int val = RevivesRequired - powerUpCount;
			val = Math.Min(m_lastRevivesNeeded, val);
			val = Math.Max(0, val);
			m_reviveTokensNeeded.text = val.ToString();
			m_lastRevivesNeeded = val;
		}
		else
		{
			string format = LanguageStrings.First.GetString("DIALOG_CONTINUE_REVIVES_NEEDED_SINGLE");
			if (RevivesRequired > 1)
			{
				format = LanguageStrings.First.GetString("DIALOG_CONTINUE_REVIVES_NEEDED_MULTIPLE");
			}
			string text = string.Format(format, RevivesRequired);
			m_reviveTokensNeeded.text = text;
		}
	}

	private void Trigger_CancelPurchase()
	{
		GameAnalytics.ContinueCancelled(GameAnalytics.CancelContinueReasons.CancelPurchase);
		EventDispatch.GenerateEvent("OnContinueGameCancel");
	}

	private void Event_OnStorePurchaseCompleted(StoreContent.StoreEntry thisEntry, StorePurchases.Result result)
	{
		if (thisEntry != null)
		{
			int powerUpCount = PowerUpsInventory.GetPowerUpCount(PowerUps.Type.Respawn);
			if (powerUpCount >= RevivesRequired)
			{
				GameAnalytics.ContinueUsed(thisEntry.m_identifier);
				EventDispatch.GenerateEvent("OnContinueGameOk", false);
				m_lastRevivesNeeded = 0;
			}
		}
	}
}
