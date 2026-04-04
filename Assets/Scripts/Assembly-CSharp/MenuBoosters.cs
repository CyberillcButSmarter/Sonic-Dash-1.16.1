using System.Collections.Generic;
using UnityEngine;

public class MenuBoosters : MonoBehaviour
{
	private class BoosterDesc
	{
		public PowerUps.Type m_type;

		public UILabel m_countLabel;

		public UILabel m_costLabel;

		public UIButton m_removeBtn;

		public MeshFilter m_mesh;

		public GameObject m_sparkles;

		public UIButton m_toggleBtn;

		public GameObject m_btnEnabled;

		public GameObject m_btnDisabled;

		public GameObject m_spinner;

		public UILabel m_nowLabel;

		public UILabel m_wasLabel;

		public UISprite m_ringIcon;

		public UISprite m_RSRIcon;

		public GameObject m_saleNode;

		public GameObject m_offerText;

		public GameObject m_offerBG;

		public GameObject m_removeAdsBG;

		public GameObject m_multiplierSprite;

		public UILabel m_multiplierValue;

		public void SetActive(bool active)
		{
			m_countLabel.gameObject.SetActive(active);
			m_costLabel.transform.parent.gameObject.SetActive(active);
			m_mesh.gameObject.SetActive(active);
			m_sparkles.SetActive(active);
			m_toggleBtn.GetComponent<GuiButtonBlocker>().Blocked = !active;
			m_btnEnabled.SetActive(active);
			m_btnDisabled.SetActive(!active);
			m_spinner.SetActive(!active);
			m_ringIcon.gameObject.SetActive(active);
			m_RSRIcon.gameObject.SetActive(active);
			m_saleNode.SetActive(active);
			m_offerText.SetActive(active);
			m_offerBG.SetActive(active);
			m_removeAdsBG.SetActive(active);
		}
	}

	private class SlotDesc
	{
		public MeshFilter m_mesh;

		public UIButton m_removeBtn;

		public PowerUps.Type? m_type;

		public int m_cost;

		public StoreContent.PaymentMethod m_currency;

		public GameObject m_multiplierSprite;

		public UILabel m_multiplierValue;
	}

	public static bool s_FillBoosters = false;

	[SerializeField]
	private UILabel m_descriptionLabel;

	[SerializeField]
	private GameObject[] m_slot = new GameObject[3];

	[SerializeField]
	private GameObject[] m_booster = new GameObject[5];

	[SerializeField]
	private ParticleSystem m_particleAssigned;

	[SerializeField]
	private UILabel m_ringCountLabel;

	[SerializeField]
	private UILabel m_rsrCountLabel;

	[SerializeField]
	private GameObject m_startGameTrigger;

	[SerializeField]
	private GameObject m_restartGameTrigger;

	[SerializeField]
	private AudioClip m_slotAvailableChoice;

	[SerializeField]
	private AudioClip m_slotUnavailableChoice;

	[SerializeField]
	private AudioClip m_slotRemove;

	[SerializeField]
	private BoostersHandHolder m_handHolder;

	[SerializeField]
	private GuiButtonBlocker[] m_blockers;

	[SerializeField]
	private GuiButtonBlocker m_homeBlocker;

	[SerializeField]
	private MenuTriggers m_menuTriggersRef;

	[SerializeField]
	private GameObject m_multiplierText;

	private bool m_homePressed;

	private bool m_playPressed;

	private int m_boostersAdded;

	private ParticleSystem[] m_particleSelected = new ParticleSystem[3];

	private UILabel m_multiplierLabel;

	private List<BoosterDesc> m_boosterDesc = new List<BoosterDesc>();

	private List<SlotDesc> m_slotDesc = new List<SlotDesc>();

	private bool m_overlayShowing;

	private PowerUps.Type? m_requestedType;

	private int m_requestedRingCost;

	private int m_requestedRSRCost;

	private int m_totalRingCost;

	private int m_totalRSRCost;

	private static string[] StoreEntries = new string[5] { "Booster Spring Bonus", "Booster Enemy Combo", "Booster Ring Streak", "Booster Score Multiplier", "Booster Golden Enemy" };

	private static string[] s_boosterDescs = new string[5] { "BOOSTERS_DESC_SPRING_BOOSTER_SHORT", "BOOSTERS_DESC_COMBO_BOOSTER_SHORT", "BOOSTERS_DESC_STREAK_BOOSTER_SHORT", "BOOSTERS_DESC_SCORE_BOOSTER_SHORT", "BOOSTERS_DESC_GOLDEN_BADNIK_SHORT" };

	private static string s_emptyDesc = "BOOSTERS_INFO";

	public static string StoreEntry(PowerUps.Type type)
	{
		return StoreEntries[GetBoosterIndex(type)];
	}

	private void Awake()
	{
		for (int i = 0; i < m_slot.Length; i++)
		{
			GameObject gameObject = m_slot[i];
			SlotDesc slotDesc = new SlotDesc();
			slotDesc.m_mesh = gameObject.transform.Find("Booster Mesh").GetComponent<MeshFilter>();
			slotDesc.m_removeBtn = gameObject.transform.Find("Button (Remove Booster)").GetComponent<UIButton>();
			slotDesc.m_multiplierSprite = gameObject.transform.Find("Multiplier Sprite").gameObject;
			slotDesc.m_multiplierValue = Utils.GetComponentInChildren<UILabel>(slotDesc.m_multiplierSprite);
			m_particleSelected[i] = (ParticleSystem)Object.Instantiate(m_particleAssigned);
			m_slotDesc.Add(slotDesc);
		}
		for (int j = 0; j < m_booster.Length; j++)
		{
			GameObject gameObject2 = m_booster[j];
			BoosterDesc boosterDesc = new BoosterDesc();
			boosterDesc.m_countLabel = gameObject2.transform.Find("Booster Count [label]").GetComponent<UILabel>();
			boosterDesc.m_costLabel = gameObject2.transform.Find("Booster Cost").GetComponentInChildren<UILabel>();
			boosterDesc.m_removeBtn = gameObject2.transform.Find("Button (Remove Booster)").GetComponent<UIButton>();
			boosterDesc.m_mesh = gameObject2.transform.Find("Booster Mesh").GetComponent<MeshFilter>();
			boosterDesc.m_sparkles = gameObject2.transform.Find("Booster Highlight").gameObject;
			boosterDesc.m_spinner = gameObject2.transform.Find("Booster Spinner").gameObject;
			GameObject gameObject3 = gameObject2.transform.Find("Button (Toggle Booster)").gameObject;
			boosterDesc.m_toggleBtn = gameObject3.GetComponent<UIButton>();
			boosterDesc.m_btnEnabled = gameObject3.transform.Find("Background (Enabled)").gameObject;
			boosterDesc.m_btnDisabled = gameObject3.transform.Find("Background (Disabled)").gameObject;
			boosterDesc.m_ringIcon = gameObject2.transform.Find("Booster Cost").Find("Ring Icon").GetComponent<UISprite>();
			boosterDesc.m_RSRIcon = gameObject2.transform.Find("Booster Cost").Find("RSR Icon").GetComponent<UISprite>();
			boosterDesc.m_saleNode = gameObject2.transform.Find("Sale Root").gameObject;
			boosterDesc.m_offerText = gameObject2.transform.Find("Offer Banner").Find("Offer Label (item description)").gameObject;
			boosterDesc.m_offerBG = gameObject2.transform.Find("Offer Banner").Find("Offer_BG (parent)").gameObject;
			boosterDesc.m_removeAdsBG = gameObject2.transform.Find("Offer Banner").Find("RemoveAds_BG (parent)").gameObject;
			boosterDesc.m_multiplierSprite = gameObject2.transform.Find("Multiplier Sprite").gameObject;
			boosterDesc.m_multiplierValue = Utils.GetComponentInChildren<UILabel>(boosterDesc.m_multiplierSprite);
			m_boosterDesc.Add(boosterDesc);
		}
	}

	private void OnEnable()
	{
		Boosters.ClearSelected();
		if (!BoostersBreadcrumb.Instance.BoostersHaveArrivedDialogShown)
		{
			BoostersBreadcrumb.Instance.ShowBoosterAnouncement();
		}
		m_homeBlocker.Blocked = false;
		for (int i = 0; i < m_slotDesc.Count; i++)
		{
			SlotDesc slotDesc = m_slotDesc[i];
			slotDesc.m_mesh.mesh = null;
			slotDesc.m_removeBtn.gameObject.SetActive(false);
			slotDesc.m_type = null;
			slotDesc.m_multiplierSprite.gameObject.SetActive(false);
		}
		m_boostersAdded = 0;
		SetBooster(PowerUps.Type.Booster_SpringBonus, true);
		SetBooster(PowerUps.Type.Booster_ScoreMultiplier, true);
		SetBooster(PowerUps.Type.Booster_GoldenEnemy, true);
		SetBooster(PowerUps.Type.Booster_EnemyComboBonus, true);
		SetBooster(PowerUps.Type.Booster_RingStreakBonus, true);
		BlockAllButtons(false);
		m_totalRingCost = 0;
		m_totalRSRCost = 0;
		UpdateRingCount(0, StoreContent.PaymentMethod.Rings);
		UpdateRingCount(0, StoreContent.PaymentMethod.StarRings);
		SetLocalisedLabel(m_descriptionLabel, -1);
		SetMultiplierLabel(null);
		EventDispatch.RegisterInterest("OnStorePurchaseCompleted", this, EventDispatch.Priority.Lowest);
		m_multiplierText.gameObject.SetActive(false);
		m_handHolder.UpdatePromptButtonText(GetNumberOfFreeSlots());
		m_handHolder.ShowCorrectButton();
		m_homePressed = false;
		m_playPressed = false;
	}

	private void Update()
	{
		if (s_FillBoosters && ABTesting.GetTestValueAsInt(ABTesting.Tests.BOOSTERS_AutoFill) == 1)
		{
			FillBoosters();
		}
	}

	private void OnDisable()
	{
		EventDispatch.UnregisterInterest("OnStorePurchaseCompleted", this);
	}

	private void Trigger_HomeButtonPressed()
	{
		if (!m_playPressed)
		{
			m_homeBlocker.Blocked = true;
			m_homePressed = true;
			m_menuTriggersRef.SendMessage("Trigger_MoveToPage", base.gameObject, SendMessageOptions.DontRequireReceiver);
			GrantooManager.GetInstance().AbandonMatch();
		}
	}

	private void UseBreadcrumbOfType(PowerUps.Type booster)
	{
		if (BoostersBreadcrumb.Instance.CurrentBreadcrumb < 5)
		{
			BoostersBreadcrumb.Instance.ActivateBreadcrumb(booster);
		}
	}

	private static int GetBoosterIndex(PowerUps.Type boosterType)
	{
		return (int)(boosterType - 11);
	}

	private int SetBooster(PowerUps.Type boosterType, bool enable)
	{
		int boosterIndex = GetBoosterIndex(boosterType);
		int powerUpCount = PowerUpsInventory.GetPowerUpCount(boosterType);
		EnableBooster(boosterType, enable);
		m_boosterDesc[boosterIndex].m_type = boosterType;
		BoosterDesc boosterDesc = m_boosterDesc[boosterIndex];
		if (powerUpCount == 0 && enable)
		{
			boosterDesc.m_countLabel.transform.gameObject.SetActive(false);
			boosterDesc.m_costLabel.transform.parent.gameObject.SetActive(true);
			StoreContent.StoreEntry storeEntry = StoreContent.GetStoreEntry(StoreEntry(boosterType), StoreContent.Identifiers.Name);
			string text = null;
			bool flag = false;
			int itemCost = StoreUtils.GetItemCost(storeEntry, StoreUtils.EntryType.Player);
			if ((storeEntry.m_state & StoreContent.StoreEntry.State.OnSale) == StoreContent.StoreEntry.State.OnSale)
			{
				flag = true;
			}
			text = LanguageUtils.FormatNumber(itemCost);
			if (storeEntry.m_payment == StoreContent.PaymentMethod.Rings)
			{
				boosterDesc.m_ringIcon.enabled = true;
				boosterDesc.m_RSRIcon.enabled = false;
			}
			if (storeEntry.m_payment == StoreContent.PaymentMethod.StarRings)
			{
				boosterDesc.m_ringIcon.enabled = false;
				boosterDesc.m_RSRIcon.enabled = true;
			}
			if (!flag)
			{
				boosterDesc.m_costLabel.text = text;
				boosterDesc.m_costLabel.enabled = true;
				boosterDesc.m_saleNode.SetActive(false);
			}
			else
			{
				boosterDesc.m_saleNode.SetActive(true);
			}
			DisplayOfferStatement(boosterDesc, storeEntry);
		}
		else if (powerUpCount > 0)
		{
			boosterDesc.m_countLabel.transform.gameObject.SetActive(true);
			boosterDesc.m_countLabel.text = "x" + powerUpCount;
			boosterDesc.m_costLabel.transform.parent.gameObject.SetActive(false);
		}
		else
		{
			boosterDesc.m_countLabel.transform.gameObject.SetActive(false);
			boosterDesc.m_costLabel.transform.parent.gameObject.SetActive(true);
		}
		bool active = enable && !BoostersBreadcrumb.Instance.IsBoosterDiscovered(boosterIndex);
		boosterDesc.m_sparkles.SetActive(active);
		int boosterMultiplier = Boosters.GetBoosterMultiplier(boosterType);
		if (boosterMultiplier == 1)
		{
			boosterDesc.m_multiplierSprite.gameObject.SetActive(false);
		}
		else
		{
			boosterDesc.m_multiplierValue.text = string.Format("x{0}", boosterMultiplier);
			boosterDesc.m_multiplierSprite.gameObject.SetActive(true);
		}
		return powerUpCount;
	}

	private void DisplayOfferStatement(BoosterDesc desc, StoreContent.StoreEntry entry)
	{
		desc.m_offerText.SetActive(false);
		desc.m_offerBG.SetActive(false);
		desc.m_removeAdsBG.SetActive(false);
		bool flag = (entry.m_state & StoreContent.StoreEntry.State.RemoveAds) == StoreContent.StoreEntry.State.RemoveAds && !PaidUser.Paid && !PaidUser.RemovedAds;
		if ((entry.m_offerSellText != null && entry.m_offerSellText.Length != 0) || flag)
		{
			string locId = entry.m_offerSellText;
			GameObject gameObject;
			if (flag)
			{
				gameObject = desc.m_removeAdsBG;
				locId = "STORE_ROSETTE_REMOVE_ADS";
			}
			else
			{
				gameObject = desc.m_offerBG;
			}
			if ((bool)desc.m_offerText)
			{
				desc.m_offerText.SetActive(true);
				LocalisedStringProperties.SetLocalisedString(desc.m_offerText, locId);
			}
			if ((bool)gameObject)
			{
				gameObject.SetActive(true);
			}
		}
	}

	private int GetBoosterFreeSlot()
	{
		int i;
		for (i = 0; i < m_slotDesc.Count; i++)
		{
			PowerUps.Type? type = m_slotDesc[i].m_type;
			if (!type.HasValue)
			{
				break;
			}
		}
		if (i < m_slotDesc.Count)
		{
			return i;
		}
		return -1;
	}

	private int GetNumberOfFreeSlots()
	{
		int num = 0;
		for (int i = 0; i < m_slotDesc.Count; i++)
		{
			PowerUps.Type? type = m_slotDesc[i].m_type;
			if (!type.HasValue)
			{
				num++;
			}
		}
		return num;
	}

	private void AddBoosterToSlot(PowerUps.Type boosterType, int cost, StoreContent.PaymentMethod currency)
	{
		int boosterFreeSlot = GetBoosterFreeSlot();
		if (boosterFreeSlot >= 0)
		{
			int boosterIndex = GetBoosterIndex(boosterType);
			m_slotDesc[boosterFreeSlot].m_removeBtn.gameObject.SetActive(true);
			m_particleSelected[boosterFreeSlot].transform.position = m_slotDesc[boosterFreeSlot].m_mesh.transform.position;
			ParticlePlayer.Play(m_particleSelected[boosterFreeSlot]);
			int num = SetBooster(boosterType, false) - 1;
			m_boosterDesc[boosterIndex].m_countLabel.text = "x" + num;
			m_slotDesc[boosterFreeSlot].m_type = boosterType;
			m_slotDesc[boosterFreeSlot].m_mesh.sharedMesh = m_boosterDesc[boosterIndex].m_mesh.sharedMesh;
			m_slotDesc[boosterFreeSlot].m_cost = cost;
			m_slotDesc[boosterFreeSlot].m_currency = currency;
			int boosterMultiplier = Boosters.GetBoosterMultiplier(boosterType);
			if (boosterMultiplier == 1)
			{
				m_slotDesc[boosterFreeSlot].m_multiplierSprite.gameObject.SetActive(false);
			}
			else
			{
				m_slotDesc[boosterFreeSlot].m_multiplierValue.text = string.Format("x{0}", boosterMultiplier);
				m_slotDesc[boosterFreeSlot].m_multiplierSprite.gameObject.SetActive(true);
			}
			SetLocalisedLabel(m_descriptionLabel, boosterIndex);
			SetMultiplierLabel(boosterType);
			if (cost > 0)
			{
				UpdateRingCount(cost, currency);
			}
			UseBreadcrumbOfType(boosterType);
		}
		m_handHolder.UpdatePromptButtonText(GetNumberOfFreeSlots());
	}

	private void EnableBooster(PowerUps.Type boosterType, bool enable)
	{
		int boosterIndex = GetBoosterIndex(boosterType);
		BoosterDesc boosterDesc = m_boosterDesc[boosterIndex];
		int powerUpCount = PowerUpsInventory.GetPowerUpCount(boosterType);
		boosterDesc.m_countLabel.text = "x" + powerUpCount;
		boosterDesc.m_removeBtn.gameObject.SetActive(false);
		boosterDesc.m_btnEnabled.SetActive(enable);
		boosterDesc.m_btnDisabled.SetActive(!enable);
		boosterDesc.m_sparkles.SetActive(!BoostersBreadcrumb.Instance.IsBoosterDiscovered(boosterIndex));
	}

	private int GetBoosterCost(string storeId, out StoreContent.PaymentMethod currency)
	{
		StoreContent.StoreEntry storeEntry = StoreContent.GetStoreEntry(storeId, StoreContent.Identifiers.Name);
		currency = storeEntry.m_payment;
		return StoreUtils.GetItemCost(storeEntry, StoreUtils.EntryType.Player);
	}

	private void RequestBooster(PowerUps.Type boosterType)
	{
		string identifier = StoreEntry(boosterType);
		int boosterFreeSlot = GetBoosterFreeSlot();
		if (boosterFreeSlot >= 0)
		{
			int powerUpCount = PowerUpsInventory.GetPowerUpCount(boosterType);
			m_requestedType = null;
			m_requestedRingCost = 0;
			m_requestedRSRCost = 0;
			if (powerUpCount > 0)
			{
				AddBoosterToSlot(boosterType, 0, StoreContent.PaymentMethod.Free);
			}
			else
			{
				StoreContent.StoreEntry storeEntry = StoreContent.GetStoreEntry(identifier, StoreContent.Identifiers.Name);
				int num = 0;
				int num2 = 0;
				if (storeEntry.m_payment == StoreContent.PaymentMethod.Rings)
				{
					num = StoreUtils.GetItemCost(storeEntry, StoreUtils.EntryType.Player);
				}
				else if (storeEntry.m_payment == StoreContent.PaymentMethod.StarRings)
				{
					num2 = StoreUtils.GetItemCost(storeEntry, StoreUtils.EntryType.Player);
				}
				int num3 = RingStorage.TotalBankedRings - m_totalRingCost - num;
				int num4 = RingStorage.TotalStarRings - m_totalRSRCost - num2;
				if (num3 < 0 || num4 < 0)
				{
					m_overlayShowing = false;
					m_requestedType = boosterType;
					m_requestedRingCost = num;
					m_requestedRSRCost = num2;
					BlockAllButtons(true);
					if (storeEntry.m_payment == StoreContent.PaymentMethod.Rings)
					{
						StorePurchases.BuyBestRingBundle(storeEntry, -num3);
					}
					else if (storeEntry.m_payment == StoreContent.PaymentMethod.StarRings)
					{
						StorePurchases.BuyBestRingBundle(storeEntry, -num4);
					}
				}
				else
				{
					int cost = ((storeEntry.m_payment != StoreContent.PaymentMethod.Rings) ? num2 : num);
					AddBoosterToSlot(boosterType, cost, storeEntry.m_payment);
				}
			}
			Audio.PlayClip(m_slotAvailableChoice, false);
		}
		else
		{
			Audio.PlayClip(m_slotUnavailableChoice, false);
		}
	}

	private void UpdateRingCount(int ringCost, StoreContent.PaymentMethod currency)
	{
		switch (currency)
		{
		case StoreContent.PaymentMethod.Rings:
			m_totalRingCost += ringCost;
			m_ringCountLabel.text = LanguageUtils.FormatNumber(RingStorage.TotalBankedRings - m_totalRingCost);
			break;
		case StoreContent.PaymentMethod.StarRings:
			m_totalRSRCost += ringCost;
			m_rsrCountLabel.text = LanguageUtils.FormatNumber(RingStorage.TotalStarRings - m_totalRSRCost);
			break;
		}
	}

	private void Event_OnStorePurchaseCompleted(StoreContent.StoreEntry thisEntry, StorePurchases.Result result)
	{
		PowerUps.Type? requestedType = m_requestedType;
		if (requestedType.HasValue)
		{
			if (result == StorePurchases.Result.Success)
			{
				if (m_requestedRingCost != 0)
				{
					AddBoosterToSlot(m_requestedType.GetValueOrDefault(), m_requestedRingCost, StoreContent.PaymentMethod.Rings);
				}
				else if (m_requestedRSRCost != 0)
				{
					AddBoosterToSlot(m_requestedType.GetValueOrDefault(), m_requestedRSRCost, StoreContent.PaymentMethod.StarRings);
				}
			}
			if (m_overlayShowing)
			{
				DialogStack.HideDialog();
			}
			m_requestedType = null;
		}
		BlockAllButtons(false);
	}

	private void Trigger_RequestStartGame()
	{
		if (m_homePressed)
		{
			return;
		}
		m_playPressed = true;
		if (m_handHolder.CanRun())
		{
			if (GCState.IsCurrentChallengeActive())
			{
				if (GCDialogManager.ShouldShowNoConnectionDialog())
				{
					GCDialogManager.ShowNoConnectionDialog();
				}
				else
				{
					StartTheGame();
				}
			}
			else
			{
				StartTheGame();
			}
		}
		else
		{
			Audio.PlayClip(m_slotUnavailableChoice, false);
			m_playPressed = false;
		}
	}

	private void StartTheGame()
	{
		for (int i = 0; i < m_slotDesc.Count; i++)
		{
			SlotDesc slotDesc = m_slotDesc[i];
			PowerUps.Type? type = slotDesc.m_type;
			if (type.HasValue)
			{
				if (slotDesc.m_cost > 0)
				{
					PowerUps.Type valueOrDefault = slotDesc.m_type.GetValueOrDefault();
					string entryID = StoreEntry(valueOrDefault);
					StorePurchases.RequestPurchase(entryID, StorePurchases.LowCurrencyResponse.PurchaseCurrencyAndItem);
				}
				Boosters.SelectBooster(slotDesc.m_type.GetValueOrDefault());
			}
		}
		bool flag = GrantooManager.GetInstance().IsActive();
		if (GameState.GetMode() == GameState.Mode.PauseMenu || flag)
		{
			m_restartGameTrigger.SendMessage("OnClick");
			m_handHolder.CompleteHandholding();
		}
		else
		{
			m_startGameTrigger.SendMessage("OnClick");
			m_handHolder.CompleteHandholding();
		}
		Audio.PlayClip(m_slotAvailableChoice, false);
	}

	private void RemoveBooster(PowerUps.Type boosterType)
	{
		for (int i = 0; i < m_slotDesc.Count; i++)
		{
			PowerUps.Type? type = m_slotDesc[i].m_type;
			if (type.HasValue && m_slotDesc[i].m_type == boosterType)
			{
				RemoveBoosterSlot(i);
			}
		}
		Audio.PlayClip(m_slotRemove, false);
		m_handHolder.UpdatePromptButtonText(GetNumberOfFreeSlots());
	}

	private void RemoveBoosterSlot(int slot)
	{
		if (slot < m_slotDesc.Count)
		{
			PowerUps.Type? type = m_slotDesc[slot].m_type;
			if (type.HasValue)
			{
				PowerUps.Type valueOrDefault = m_slotDesc[slot].m_type.GetValueOrDefault();
				SetBooster(valueOrDefault, true);
				SlotDesc slotDesc = m_slotDesc[slot];
				if (slotDesc.m_cost > 0)
				{
					UpdateRingCount(-slotDesc.m_cost, slotDesc.m_currency);
				}
				slotDesc.m_mesh.mesh = null;
				slotDesc.m_type = null;
				slotDesc.m_cost = 0;
				slotDesc.m_multiplierSprite.gameObject.SetActive(false);
				slotDesc.m_removeBtn.gameObject.SetActive(false);
				SetLocalisedLabel(m_descriptionLabel, -1);
				SetMultiplierLabel(null);
			}
		}
		Audio.PlayClip(m_slotRemove, false);
		m_handHolder.UpdatePromptButtonText(GetNumberOfFreeSlots());
	}

	private void BlockAllButtons(bool blockVal)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < m_boosterDesc.Count; i++)
		{
			m_boosterDesc[i].m_spinner.SetActive(blockVal);
			if (blockVal)
			{
				m_boosterDesc[i].m_countLabel.transform.gameObject.SetActive(false);
				m_boosterDesc[i].m_costLabel.transform.parent.gameObject.SetActive(false);
				continue;
			}
			StoreContent.PaymentMethod currency;
			num = GetBoosterCost(StoreEntry(m_boosterDesc[i].m_type), out currency);
			num3 = (num2 = PowerUpsInventory.GetPowerUpCount(m_boosterDesc[i].m_type));
			if (IsBoosterSelectedInMenu(m_boosterDesc[i].m_type))
			{
				num2 = -1;
			}
			if (num2 == 0)
			{
				m_boosterDesc[i].m_countLabel.transform.gameObject.SetActive(false);
				m_boosterDesc[i].m_costLabel.transform.parent.gameObject.SetActive(true);
				m_boosterDesc[i].m_costLabel.text = num.ToString();
			}
			else if (num2 > 0)
			{
				m_boosterDesc[i].m_countLabel.transform.gameObject.SetActive(true);
				m_boosterDesc[i].m_countLabel.text = "x" + num2;
				m_boosterDesc[i].m_costLabel.transform.parent.gameObject.SetActive(false);
			}
			else if (num3 == 0)
			{
				m_boosterDesc[i].m_countLabel.transform.gameObject.SetActive(false);
				m_boosterDesc[i].m_costLabel.transform.parent.gameObject.SetActive(true);
			}
			else
			{
				m_boosterDesc[i].m_countLabel.text = "x" + ((num3 != 0) ? (num3 - 1) : num3);
				m_boosterDesc[i].m_countLabel.transform.gameObject.SetActive(true);
				m_boosterDesc[i].m_costLabel.transform.parent.gameObject.SetActive(false);
			}
		}
		for (int j = 0; j < m_blockers.Length; j++)
		{
			m_blockers[j].Blocked = blockVal;
		}
	}

	private bool IsBoosterSelectedInMenu(PowerUps.Type booster)
	{
		for (int i = 0; i < m_slotDesc.Count; i++)
		{
			if (m_slotDesc[i].m_type == booster)
			{
				return true;
			}
		}
		return false;
	}

	private void Trigger_AddSpringBonus()
	{
		if (m_boosterDesc[GetBoosterIndex(PowerUps.Type.Booster_SpringBonus)].m_btnEnabled.activeSelf)
		{
			RequestBooster(PowerUps.Type.Booster_SpringBonus);
		}
		else
		{
			Trigger_RemoveSpringBonus();
		}
	}

	private void Trigger_AddRingStreakCombo()
	{
		if (m_boosterDesc[GetBoosterIndex(PowerUps.Type.Booster_RingStreakBonus)].m_btnEnabled.activeSelf)
		{
			RequestBooster(PowerUps.Type.Booster_RingStreakBonus);
		}
		else
		{
			Trigger_RemoveRingStreakCombo();
		}
	}

	private void Trigger_AddEnemyCombo()
	{
		if (m_boosterDesc[GetBoosterIndex(PowerUps.Type.Booster_EnemyComboBonus)].m_btnEnabled.activeSelf)
		{
			RequestBooster(PowerUps.Type.Booster_EnemyComboBonus);
		}
		else
		{
			Trigger_RemoveEnemyCombo();
		}
	}

	private void Trigger_AddScoreMultiplier()
	{
		if (m_boosterDesc[GetBoosterIndex(PowerUps.Type.Booster_ScoreMultiplier)].m_btnEnabled.activeSelf)
		{
			RequestBooster(PowerUps.Type.Booster_ScoreMultiplier);
		}
		else
		{
			Trigger_RemoveScoreMultiplier();
		}
	}

	private void Trigger_AddGoldenEnemy()
	{
		if (m_boosterDesc[GetBoosterIndex(PowerUps.Type.Booster_GoldenEnemy)].m_btnEnabled.activeSelf)
		{
			RequestBooster(PowerUps.Type.Booster_GoldenEnemy);
		}
		else
		{
			Trigger_RemoveGoldenEnemy();
		}
	}

	private void FillBoosters()
	{
		if (m_boostersAdded >= 3)
		{
			s_FillBoosters = false;
		}
		while (s_FillBoosters)
		{
			int nextFillerBooster = GetNextFillerBooster();
			if (nextFillerBooster == -1)
			{
				s_FillBoosters = false;
				continue;
			}
			PowerUps.Type boosterType = (PowerUps.Type)nextFillerBooster;
			if (!BoostersBreadcrumb.Instance.IsBoosterDiscovered(GetBoosterIndex(boosterType)))
			{
				s_FillBoosters = false;
			}
			RequestBooster(boosterType);
			m_boostersAdded++;
			if (m_boostersAdded >= 3)
			{
				s_FillBoosters = false;
			}
		}
		SetLocalisedLabel(m_descriptionLabel, -1);
		SetMultiplierLabel(null);
	}

	private int GetNextFillerBooster()
	{
		List<PowerUps.Type> list = new List<PowerUps.Type>(5);
		List<PowerUps.Type> list2 = new List<PowerUps.Type>(5);
		List<PowerUps.Type> list3 = new List<PowerUps.Type>(5);
		for (int i = 0; i < m_boosterDesc.Count; i++)
		{
			PowerUps.Type type = m_boosterDesc[i].m_type;
			if (!BoostersBreadcrumb.Instance.IsBoosterDiscovered(GetBoosterIndex(type)))
			{
				list3.Add(type);
			}
			if (!IsBoosterSelectedInMenu(type))
			{
				if (PowerUpsInventory.GetPowerUpCount(type) > 0)
				{
					list2.Add(type);
				}
				else if (CanAffordBooster(type))
				{
					list.Add(type);
				}
			}
		}
		if (list.Count == 0 && list2.Count == 0)
		{
			return -1;
		}
		if (list3.Count > 0)
		{
			return (int)list3[Random.Range(0, list3.Count)];
		}
		if (list2.Count > 0)
		{
			return (int)list2[Random.Range(0, list2.Count)];
		}
		return (int)list[Random.Range(0, list.Count)];
	}

	private bool CanAffordBooster(PowerUps.Type booster)
	{
		StoreContent.PaymentMethod currency;
		int boosterCost = GetBoosterCost(StoreEntry(booster), out currency);
		switch (currency)
		{
		case StoreContent.PaymentMethod.Rings:
			return boosterCost <= RingStorage.TotalBankedRings - m_totalRingCost;
		case StoreContent.PaymentMethod.StarRings:
			return boosterCost <= RingStorage.TotalStarRings - m_totalRSRCost;
		default:
			return false;
		}
	}

	private void Trigger_RemoveSpringBonus()
	{
		RemoveBooster(PowerUps.Type.Booster_SpringBonus);
	}

	private void Trigger_RemoveRingStreakCombo()
	{
		RemoveBooster(PowerUps.Type.Booster_RingStreakBonus);
	}

	private void Trigger_RemoveEnemyCombo()
	{
		RemoveBooster(PowerUps.Type.Booster_EnemyComboBonus);
	}

	private void Trigger_RemoveScoreMultiplier()
	{
		RemoveBooster(PowerUps.Type.Booster_ScoreMultiplier);
	}

	private void Trigger_RemoveGoldenEnemy()
	{
		RemoveBooster(PowerUps.Type.Booster_GoldenEnemy);
	}

	private void Trigger_RemoveSlot1()
	{
		RemoveBoosterSlot(0);
	}

	private void Trigger_RemoveSlot2()
	{
		RemoveBoosterSlot(1);
	}

	private void Trigger_RemoveSlot3()
	{
		RemoveBoosterSlot(2);
	}

	private void Trigger_ShowInfoSlot1()
	{
		ShowSelectedSlotInfo(0);
	}

	private void Trigger_ShowInfoSlot2()
	{
		ShowSelectedSlotInfo(1);
	}

	private void Trigger_ShowInfoSlot3()
	{
		ShowSelectedSlotInfo(2);
	}

	private void ShowSelectedSlotInfo(int slot)
	{
		PowerUps.Type? type = m_slotDesc[slot].m_type;
		if (type.HasValue)
		{
			PowerUps.Type? type2 = m_slotDesc[slot].m_type;
			PowerUps.Type value = type2.Value;
			int boosterIndex = GetBoosterIndex(value);
			SetLocalisedLabel(m_descriptionLabel, boosterIndex);
			SetMultiplierLabel(value);
		}
		else
		{
			SetLocalisedLabel(m_descriptionLabel, -1);
			SetMultiplierLabel(null);
		}
	}

	private void Trigger_DisplayHelpDialog()
	{
		DialogStack.ShowDialog("Boosters Help");
	}

	private void SetMultiplierLabel(PowerUps.Type? booster)
	{
		if (!booster.HasValue)
		{
			m_multiplierText.gameObject.SetActive(false);
			return;
		}
		if (m_multiplierLabel == null)
		{
			m_multiplierLabel = Utils.GetComponentInChildren<UILabel>(m_multiplierText);
		}
		int boosterMultiplier = Boosters.GetBoosterMultiplier(booster.Value);
		if (boosterMultiplier == 1)
		{
			m_multiplierText.gameObject.SetActive(false);
		}
		else if (m_multiplierLabel != null)
		{
			string format = LanguageStrings.First.GetString("BOOSTERS_MULTIPLIER_TEXT");
			string text = string.Format(format, boosterMultiplier);
			m_multiplierLabel.text = text;
			m_multiplierText.gameObject.SetActive(true);
		}
	}

	private void SetLocalisedLabel(UILabel descriptionLabel, int boosterIndex)
	{
		LocalisedStringProperties component = descriptionLabel.GetComponent<LocalisedStringProperties>();
		if (boosterIndex == -1)
		{
			component.SetLocalisationID(s_emptyDesc);
		}
		else if (boosterIndex == GetBoosterIndex(PowerUps.Type.Booster_ScoreMultiplier))
		{
			string format = LanguageStrings.First.GetString(s_boosterDescs[boosterIndex]);
			string text = string.Format(format, Boosters.ScoreMultiplier * 100f);
			descriptionLabel.text = text;
			component.SetLocalisationID(null);
		}
		else if (boosterIndex == GetBoosterIndex(PowerUps.Type.Booster_GoldenEnemy))
		{
			string format2 = LanguageStrings.First.GetString(s_boosterDescs[boosterIndex]);
			string text2 = string.Format(format2, Boosters.GoldenEnemyScoreMultipler);
			descriptionLabel.text = text2;
			component.SetLocalisationID(null);
		}
		else
		{
			component.SetLocalisationID(s_boosterDescs[boosterIndex]);
		}
		LocalisedStringStatic component2 = descriptionLabel.GetComponent<LocalisedStringStatic>();
		component2.ForceStringUpdate();
	}
}
