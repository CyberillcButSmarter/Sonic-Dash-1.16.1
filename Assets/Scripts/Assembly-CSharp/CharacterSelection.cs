using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
	private enum EButtons
	{
		Play = 0,
		Grantoo = 1
	}

	private enum EButtonState
	{
		Enabled = 0,
		ShowStore = 1,
		StoreActive = 2
	}

	private const string RingCostFormat_Was = "STORE_SALE_PRICE_WAS";

	private const string RingCostFormat_Now = "STORE_SALE_PRICE_NOW";

	private const string c_strSpritePlayButtonDisabled = "button_disabled";

	private const string c_strSpritePlayButtonEnabled = "button_orange_med";

	public static string[] m_storeEntries = new string[12]
	{
		string.Empty,
		"Character Tails",
		"Character Knuckles",
		"Character Amy",
		"Character Shadow",
		"Character Blaze",
		"Character Silver",
		"Character Rouge",
		"Character Cream",
		"Character Espio",
		"Character Andronic",
		"Character Android"
	};

	private object[] m_eventParams = new object[1];

	private StoreContent.StoreEntry m_pendingPurchase;

	private Characters.Type m_currentDisplayedCharacter;

	private Characters.Type m_characterPendingConfirmation;

	[SerializeField]
	private GameObject[] m_playGroups;

	[SerializeField]
	private GameObject[] m_costGroups;

	[SerializeField]
	private GameObject[] m_activeGroups;

	[SerializeField]
	private GuiButtonBlocker[] m_buttonBlockers;

	[SerializeField]
	private GameObject m_startGameTrigger;

	[SerializeField]
	private GameObject m_nextMenuTrigger;

	[SerializeField]
	private UILabel[] m_costLabels;

	[SerializeField]
	private UILabel[] m_nowLabels;

	[SerializeField]
	private UILabel[] m_wasLabels;

	[SerializeField]
	private UISprite[] m_ringIcons;

	[SerializeField]
	private UISprite[] m_RSRIcons;

	[SerializeField]
	private GameObject m_saleNode;

	[SerializeField]
	private GameObject m_offerText;

	[SerializeField]
	private GameObject m_offerBG;

	[SerializeField]
	private GameObject m_removeAdsBG;

	private SimpleGestureMonitor m_simpleGestureMonitor;

	private bool m_starting;

	private void Start()
	{
		m_simpleGestureMonitor = new SimpleGestureMonitor();
	}

	private void OnEnable()
	{
		m_starting = false;
		EventDispatch.GenerateEvent("OnCharacterSelectEnabled");
		if (m_simpleGestureMonitor != null)
		{
			m_simpleGestureMonitor.reset();
		}
		EventDispatch.RegisterInterest("OnStorePurchaseCompleted", this);
	}

	private void OnDisable()
	{
		m_pendingPurchase = null;
		EventDispatch.UnregisterInterest("OnStorePurchaseCompleted", this);
	}

	private void Update()
	{
		UpdateCharacterCost();
		bool buttonBlocked = UpdateButtonDisplay();
		UpdateButtonState(buttonBlocked);
		m_simpleGestureMonitor.Update();
		if (!m_starting && !DialogStack.IsDialogShown)
		{
			if (GrantooManager.GetInstance().ShouldMatchBegin())
			{
				GrantooManager.GetInstance().BeginMatch();
				MoveToNextMenu();
			}
			else if (m_simpleGestureMonitor.swipeLeftDetected() && m_simpleGestureMonitor.GestureStartPosition.x > (float)Screen.width * 0.5f)
			{
				Trigger_CharacterNext();
			}
			else if (m_simpleGestureMonitor.swipeRightDetected() && m_simpleGestureMonitor.GestureStartPosition.x < (float)Screen.width * 0.5f)
			{
				Trigger_CharacterPrev();
			}
		}
	}

	private void UpdateCharacterCost()
	{
		Characters.Type currentCharacterSelection = CharacterManager.Singleton.GetCurrentCharacterSelection();
		bool flag = Characters.CharacterUnlocked(currentCharacterSelection);
		if (currentCharacterSelection != m_currentDisplayedCharacter && !flag)
		{
			StoreContent.StoreEntry storeEntry = StoreContent.GetStoreEntry(m_storeEntries[(int)currentCharacterSelection], StoreContent.Identifiers.Name);
			if (currentCharacterSelection == Characters.Type.Espio)
			{
				m_saleNode.SetActive(false);
				m_offerText.SetActive(false);
				m_offerBG.SetActive(false);
				m_removeAdsBG.SetActive(false);
			}
			else
			{
				string text = null;
				string text2 = null;
				bool flag2 = false;
				if (storeEntry.m_payment == StoreContent.PaymentMethod.Money)
				{
					text = storeEntry.m_osStore.m_playerPrice;
					text2 = storeEntry.m_osStore.m_basePrice;
					if ((storeEntry.m_state & StoreContent.StoreEntry.State.OnSale) == StoreContent.StoreEntry.State.OnSale)
					{
						flag2 = true;
					}
					m_ringIcons[0].enabled = false;
					m_ringIcons[1].enabled = false;
					m_RSRIcons[0].enabled = false;
					m_RSRIcons[1].enabled = false;
				}
				else
				{
					int itemCost = StoreUtils.GetItemCost(storeEntry, StoreUtils.EntryType.Player);
					int itemCost2 = StoreUtils.GetItemCost(storeEntry, StoreUtils.EntryType.Base);
					if ((storeEntry.m_state & StoreContent.StoreEntry.State.OnSale) == StoreContent.StoreEntry.State.OnSale)
					{
						flag2 = true;
					}
					text = LanguageUtils.FormatNumber(itemCost);
					text2 = LanguageUtils.FormatNumber(itemCost2);
					if (storeEntry.m_payment == StoreContent.PaymentMethod.Rings)
					{
						m_ringIcons[0].enabled = true;
						m_ringIcons[1].enabled = true;
						m_RSRIcons[0].enabled = false;
						m_RSRIcons[1].enabled = false;
					}
					if (storeEntry.m_payment == StoreContent.PaymentMethod.StarRings)
					{
						m_ringIcons[0].enabled = false;
						m_ringIcons[1].enabled = false;
						m_RSRIcons[0].enabled = true;
						m_RSRIcons[1].enabled = true;
					}
				}
				if (!flag2)
				{
					for (int i = 0; i < 2; i++)
					{
						m_costLabels[i].text = text;
						m_nowLabels[i].enabled = false;
						m_wasLabels[i].enabled = false;
						m_costLabels[i].enabled = true;
					}
				}
				else
				{
					string format = LanguageStrings.First.GetString("STORE_SALE_PRICE_WAS");
					string text3 = string.Format(format, text2);
					string format2 = LanguageStrings.First.GetString("STORE_SALE_PRICE_NOW");
					string text4 = string.Format(format2, text);
					for (int j = 0; j < 2; j++)
					{
						m_nowLabels[j].text = text4;
						m_wasLabels[j].text = text3;
						m_nowLabels[j].enabled = true;
						m_wasLabels[j].enabled = true;
						m_costLabels[j].enabled = false;
					}
				}
				DisplaySaleTag();
				DisplayOfferStatement();
			}
		}
		else if (flag)
		{
			m_saleNode.SetActive(false);
			m_offerText.SetActive(false);
			m_offerBG.SetActive(false);
			m_removeAdsBG.SetActive(false);
		}
		m_currentDisplayedCharacter = currentCharacterSelection;
	}

	private void DisplaySaleTag()
	{
		Characters.Type currentCharacterSelection = CharacterManager.Singleton.GetCurrentCharacterSelection();
		bool flag = Characters.CharacterUnlocked(currentCharacterSelection);
		if (currentCharacterSelection != m_currentDisplayedCharacter && !flag)
		{
			m_saleNode.SetActive(false);
			StoreContent.StoreEntry storeEntry = StoreContent.GetStoreEntry(m_storeEntries[(int)currentCharacterSelection], StoreContent.Identifiers.Name);
			if ((storeEntry.m_state & StoreContent.StoreEntry.State.OnSale) == StoreContent.StoreEntry.State.OnSale)
			{
				m_saleNode.SetActive(true);
			}
		}
	}

	private void DisplayOfferStatement()
	{
		Characters.Type currentCharacterSelection = CharacterManager.Singleton.GetCurrentCharacterSelection();
		bool flag = Characters.CharacterUnlocked(currentCharacterSelection);
		if (currentCharacterSelection == m_currentDisplayedCharacter || flag)
		{
			return;
		}
		StoreContent.StoreEntry storeEntry = StoreContent.GetStoreEntry(m_storeEntries[(int)currentCharacterSelection], StoreContent.Identifiers.Name);
		m_offerText.SetActive(false);
		m_offerBG.SetActive(false);
		m_removeAdsBG.SetActive(false);
		bool flag2 = (storeEntry.m_state & StoreContent.StoreEntry.State.RemoveAds) == StoreContent.StoreEntry.State.RemoveAds && !PaidUser.Paid && !PaidUser.RemovedAds;
		if ((storeEntry.m_offerSellText != null && storeEntry.m_offerSellText.Length != 0) || flag2)
		{
			string locId = storeEntry.m_offerSellText;
			GameObject gameObject;
			if (flag2)
			{
				gameObject = m_removeAdsBG;
				locId = "STORE_ROSETTE_REMOVE_ADS";
			}
			else
			{
				gameObject = m_offerBG;
			}
			if ((bool)m_offerText)
			{
				m_offerText.SetActive(true);
				LocalisedStringProperties.SetLocalisedString(m_offerText, locId);
			}
			if ((bool)gameObject)
			{
				gameObject.SetActive(true);
			}
		}
	}

	private void SetButtonState(EButtons button, EButtonState state)
	{
		switch (state)
		{
		case EButtonState.Enabled:
			m_playGroups[(int)button].SetActive(true);
			m_costGroups[(int)button].SetActive(false);
			m_activeGroups[(int)button].SetActive(false);
			break;
		case EButtonState.ShowStore:
			m_playGroups[(int)button].SetActive(false);
			m_costGroups[(int)button].SetActive(true);
			m_activeGroups[(int)button].SetActive(false);
			break;
		case EButtonState.StoreActive:
			m_playGroups[(int)button].SetActive(false);
			m_costGroups[(int)button].SetActive(false);
			m_activeGroups[(int)button].SetActive(true);
			break;
		}
	}

	private bool UpdateButtonDisplay()
	{
		bool result = false;
		bool flag = false;
		Characters.Type currentCharacterSelection = CharacterManager.Singleton.GetCurrentCharacterSelection();
		if (Characters.CharacterUnlocked(currentCharacterSelection))
		{
			SetButtonState(EButtons.Play, EButtonState.Enabled);
			SetButtonState(EButtons.Grantoo, EButtonState.Enabled);
		}
		else if (currentCharacterSelection == Characters.Type.Espio)
		{
			SetButtonState(EButtons.Play, EButtonState.Enabled);
			SetButtonState(EButtons.Grantoo, EButtonState.Enabled);
			result = true;
			flag = true;
		}
		else if (StoreUtils.IsStoreActive())
		{
			SetButtonState(EButtons.Play, EButtonState.StoreActive);
			SetButtonState(EButtons.Grantoo, EButtonState.StoreActive);
			result = true;
		}
		else
		{
			SetButtonState(EButtons.Play, EButtonState.ShowStore);
			SetButtonState(EButtons.Grantoo, EButtonState.ShowStore);
		}
		UISprite componentInChildren = m_playGroups[0].transform.parent.GetComponentInChildren<UISprite>();
		if (componentInChildren != null)
		{
			componentInChildren.spriteName = ((!flag) ? "button_orange_med" : "button_disabled");
		}
		return result;
	}

	private void UpdateButtonState(bool buttonBlocked)
	{
		for (int i = 0; i < m_buttonBlockers.Length; i++)
		{
			m_buttonBlockers[i].Blocked = buttonBlocked;
		}
	}

	private void StartCharacterPurchase(Characters.Type currentCharacter)
	{
		m_pendingPurchase = StoreContent.GetStoreEntry(m_storeEntries[(int)currentCharacter], StoreContent.Identifiers.Name);
		StorePurchases.RequestPurchase(m_storeEntries[(int)currentCharacter], StorePurchases.LowCurrencyResponse.PurchaseCurrencyAndItem);
	}

	private void MoveToNextMenu()
	{
		bool flag = TutorialSystem.instance().isTrackTutorialEnabled();
		m_starting = true;
		int testValueAsInt = ABTesting.GetTestValueAsInt(ABTesting.Tests.BOOSTERS_RunsBeforeUse);
		if (flag || (PlayerStats.GetStat(PlayerStats.StatNames.NumberOfSessions_Total) < 2 && PlayerStats.GetStat(PlayerStats.StatNames.NumberOfRuns_Total) < testValueAsInt))
		{
			m_startGameTrigger.SendMessage("OnClick");
		}
		else
		{
			m_nextMenuTrigger.SendMessage("OnClick");
		}
		m_pendingPurchase = null;
	}

	private void Trigger_CharacterScrolling()
	{
		m_eventParams[0] = true;
		EventDispatch.GenerateEvent("OnCharacterSelection", m_eventParams);
		m_pendingPurchase = null;
	}

	private void Trigger_CharacterNext()
	{
		CharacterManager.Singleton.Next();
		Trigger_CharacterIdle();
	}

	private void Trigger_CharacterPrev()
	{
		CharacterManager.Singleton.Previous();
		Trigger_CharacterIdle();
	}

	private void Trigger_CharacterIdle()
	{
		m_eventParams[0] = false;
		EventDispatch.GenerateEvent("OnCharacterSelection", m_eventParams);
	}

	private void Trigger_OnPlaySelected()
	{
		Characters.Type currentCharacterSelection = CharacterManager.Singleton.GetCurrentCharacterSelection();
		if (!Characters.CharacterUnlocked(currentCharacterSelection))
		{
			Dialog_CharacterConfirmation.Display(currentCharacterSelection);
			m_characterPendingConfirmation = currentCharacterSelection;
		}
		else
		{
			MoveToNextMenu();
		}
	}

	private void Trigger_OnGrantooSelected()
	{
		if (!m_buttonBlockers[1].Blocked)
		{
			Characters.Type currentCharacterSelection = CharacterManager.Singleton.GetCurrentCharacterSelection();
			if (!Characters.CharacterUnlocked(currentCharacterSelection))
			{
				Dialog_CharacterConfirmation.Display(currentCharacterSelection);
				m_characterPendingConfirmation = currentCharacterSelection;
			}
			else
			{
				GrantooManager.GetInstance().Launch();
			}
		}
	}

	private void Trigger_OnShowGoogleEventProgress()
	{
		GoogleSpecialEvent.ShowProgressDialog();
	}

	private void Trigger_OnShowEspioEventProgress()
	{
		EspioSpecialEvent.ShowProgressDialog();
	}

	private void Event_OnStorePurchaseCompleted(StoreContent.StoreEntry thisEntry, StorePurchases.Result result)
	{
		if (thisEntry == m_pendingPurchase && thisEntry != null)
		{
			m_pendingPurchase = null;
			if (result == StorePurchases.Result.Success)
			{
				MoveToNextMenu();
			}
		}
	}

	private void Trigger_AcceptPurchase()
	{
		StartCharacterPurchase(m_characterPendingConfirmation);
	}
}
