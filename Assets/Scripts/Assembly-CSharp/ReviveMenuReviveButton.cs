using System;
using System.Collections;
using UnityEngine;

public class ReviveMenuReviveButton : MonoBehaviour
{
	[Flags]
	private enum State
	{
		Active = 1,
		Delayed = 2,
		UsingRSR = 4,
		WaitingForPurchase = 8
	}

	private State m_state;

	[SerializeField]
	private UILabel m_tokensDisplay;

	[SerializeField]
	private UISlider m_timeOutSlider;

	public void Active(bool active)
	{
		if (active)
		{
			m_state |= State.Active;
		}
		else
		{
			m_state &= ~State.Active;
		}
	}

	private void Start()
	{
		EventDispatch.RegisterInterest("OnStorePurchaseCompleted", this);
		if (ABTesting.GetTestValueAsInt(ABTesting.Tests.REVIVE_RSRRevive) != 0)
		{
			m_state |= State.UsingRSR;
		}
	}

	private void Update()
	{
		if (!MenuReviveScreen.Valid)
		{
			return;
		}
		if (MenuReviveScreen.ShouldEnableReviveTimer())
		{
			if (!m_timeOutSlider.gameObject.activeSelf)
			{
				m_timeOutSlider.gameObject.SetActive(true);
			}
			m_timeOutSlider.value = MenuReviveScreen.CountdownTime / MenuReviveScreen.TimeOut;
		}
		else if (m_timeOutSlider.gameObject.activeSelf)
		{
			m_timeOutSlider.gameObject.SetActive(false);
		}
	}

	private void OnEnable()
	{
		StartCoroutine(StartPendingActivation());
	}

	private IEnumerator StartPendingActivation()
	{
		while (!MenuReviveScreen.Valid)
		{
			yield return null;
		}
		if (m_tokensDisplay != null)
		{
			m_tokensDisplay.text = MenuReviveScreen.RevivesRequired.ToString();
		}
	}

	private void Trigger_ContinueGame_Paid()
	{
		if ((m_state & State.Active) != State.Active || (m_state & State.WaitingForPurchase) == State.WaitingForPurchase)
		{
			return;
		}
		if ((m_state & State.UsingRSR) != 0)
		{
			StoreContent.StoreEntry storeEntry = StoreContent.GetStoreEntry("RSRRevive", StoreContent.Identifiers.Name);
			m_state |= State.WaitingForPurchase;
			storeEntry.m_cost.m_playerCost[0] = MenuReviveScreen.RevivesRequired;
			StorePurchases.RequestPurchase("RSRRevive", StorePurchases.LowCurrencyResponse.PurchaseCurrencyAndItem);
			return;
		}
		int powerUpCount = PowerUpsInventory.GetPowerUpCount(PowerUps.Type.Respawn);
		if (powerUpCount >= MenuReviveScreen.RevivesRequired)
		{
			GameAnalytics.ContinueUsed("NoPurchaseRequired");
			EventDispatch.GenerateEvent("OnContinueGameOk", false);
		}
		else
		{
			EventDispatch.GenerateEvent("OnContinuePurchaseRequired");
		}
	}

	private void Trigger_ContinueGame_Free()
	{
		if ((m_state & State.Active) == State.Active)
		{
			GameAnalytics.ContinueUsed("FreeReviveUsed");
			EventDispatch.GenerateEvent("OnContinueGameOk", true);
			m_state &= ~State.WaitingForPurchase;
		}
	}

	private void Trigger_ContinueGame_Cancel()
	{
		if ((m_state & State.Active) == State.Active)
		{
			GameAnalytics.ContinueCancelled(GameAnalytics.CancelContinueReasons.Skip);
			EventDispatch.GenerateEvent("OnContinueGameCancel");
			m_state &= ~State.WaitingForPurchase;
		}
	}

	private void Event_OnStorePurchaseCompleted(StoreContent.StoreEntry thisEntry, StorePurchases.Result result)
	{
		if ((m_state & State.WaitingForPurchase) != 0)
		{
			m_state &= ~State.WaitingForPurchase;
			if (result == StorePurchases.Result.Success)
			{
				EventDispatch.GenerateEvent("OnContinueGameOk", false);
			}
		}
	}
}
