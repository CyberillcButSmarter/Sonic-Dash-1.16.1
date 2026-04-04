using System;
using UnityEngine;

public class StorePopulator
{
	[Flags]
	public enum Display
	{
		IgnoreTimeOut = 1,
		UpdateDownload = 2
	}

	private enum PurchaseOptions
	{
		Available = 0,
		Unavailable = 1,
		Purchased = 2,
		Pending = 3,
		Downloading = 4
	}

	private enum Content
	{
		Title = 0,
		Description = 1,
		Cost = 2,
		WasCost = 3,
		NowCost = 4,
		WasDescription = 5,
		NowDescription = 6,
		Count = 7,
		Progress = 8,
		StateBackground = 9,
		PriceRoot = 10,
		BuyButton = 11,
		Purchased = 12,
		Unavailable = 13,
		Downloading = 14,
		CurrencyIcon_Ring = 15,
		CurrencyIcon_Star = 16,
		TimeOut = 17,
		ItemMesh = 18,
		OfferText = 19,
		OfferBackground = 20,
		RemoveAdsBackground = 21,
		SaleTag = 22
	}

	private const string DescriptionFormat_Rings_Normal = "STORE_BUNDLE_DESCRIPTION_RINGS";

	private const string DescriptionFormat_Stars_Normal = "STORE_BUNDLE_DESCRIPTION_STARS";

	private const string DescriptionFormat_Both_Normal = "STORE_BUNDLE_DESCRIPTION_RINGS_AND_STARS";

	private const string DescriptionFormat_Rings_Was = "STORE_BUNDLE_DESCRIPTION_RINGS_WAS";

	private const string DescriptionFormat_Stars_Was = "STORE_BUNDLE_DESCRIPTION_STARS_WAS";

	private const string DescriptionFormat_Both_Was = "STORE_BUNDLE_DESCRIPTION_RINGS_AND_STARS_WAS";

	private const string DescriptionFormat_Rings_Now = "STORE_BUNDLE_DESCRIPTION_RINGS_NOW";

	private const string DescriptionFormat_Stars_Now = "STORE_BUNDLE_DESCRIPTION_STARS_NOW";

	private const string DescriptionFormat_Both_Now = "STORE_BUNDLE_DESCRIPTION_RINGS_AND_STARS_NOW";

	private const string RingCostFormat_Was = "STORE_SALE_PRICE_WAS";

	private const string RingCostFormat_Now = "STORE_SALE_PRICE_NOW";

	private const string RingCostFormat_Free = "STORE_FREE";

	private static StoreContent.StoreEntry s_storeEntry;

	private static GameObject[] s_contentObject;

	public static void PopulateEntry(GameObject target, StoreContent.StoreEntry entry)
	{
		PopulateEntry(target, entry, (Display)0);
	}

	public static void PopulateEntry(GameObject target, StoreContent.StoreEntry entry, Display displayFlags)
	{
		CacheEntryContents(target, entry, true);
		DisplayTitle();
		DisplayDescription();
		DisplayEntryCount();
		DisplayUpgradeCount();
		DisplayDownloadCount();
		Display3DMesh();
		DisplaySaleTag();
		DisplayOfferStatement();
		UpdatePurchaseButtons(displayFlags);
		s_storeEntry = null;
	}

	public static void PopulateMinimalEntry(GameObject target, StoreContent.StoreEntry entry, Display displayFlags)
	{
		CacheEntryContents(target, entry, false);
		if ((displayFlags & Display.UpdateDownload) == Display.UpdateDownload)
		{
			DisplayDownloadCount();
		}
		s_storeEntry = null;
	}

	private static void CacheEntryContents(GameObject target, StoreContent.StoreEntry entry, bool disableEverything)
	{
		if (s_contentObject == null)
		{
			s_contentObject = new GameObject[Utils.GetEnumCount<Content>()];
		}
		for (int i = 0; i < s_contentObject.Length; i++)
		{
			Content content = (Content)i;
			string tag = string.Format("StoreEntry_{0}", content.ToString());
			s_contentObject[i] = Utils.FindTagInChildren(target, tag);
			if (s_contentObject[i] != null && disableEverything)
			{
				s_contentObject[i].SetActive(false);
			}
		}
		s_storeEntry = entry;
	}

	private static void DisplayTitle()
	{
		GameObject gameObject = s_contentObject[0];
		if (!(gameObject == null))
		{
			LocalisedStringProperties.SetLocalisedString(gameObject, s_storeEntry.m_title);
			gameObject.SetActive(true);
		}
	}

	private static void DisplayDescription()
	{
		GameObject gameObject = s_contentObject[1];
		GameObject gameObject2 = s_contentObject[5];
		GameObject gameObject3 = s_contentObject[6];
		if (gameObject == null || gameObject2 == null || gameObject3 == null)
		{
			return;
		}
		if (s_storeEntry.m_description != null && s_storeEntry.m_description.Length > 0)
		{
			LocalisedStringProperties.SetLocalisedString(gameObject, s_storeEntry.m_description);
			gameObject.SetActive(true);
			return;
		}
		int playerRings = s_storeEntry.m_awards.m_playerRings;
		int playerStars = s_storeEntry.m_awards.m_playerStars;
		int baseRings = s_storeEntry.m_awards.m_baseRings;
		int baseStars = s_storeEntry.m_awards.m_baseStars;
		string text = null;
		string text2 = null;
		string text3 = null;
		if (playerRings > 0 && playerStars == 0)
		{
			string format = LanguageStrings.First.GetString("STORE_BUNDLE_DESCRIPTION_RINGS");
			text = string.Format(format, LanguageUtils.FormatNumber(playerRings));
			string format2 = LanguageStrings.First.GetString("STORE_BUNDLE_DESCRIPTION_RINGS_NOW");
			text3 = string.Format(format2, LanguageUtils.FormatNumber(playerRings));
		}
		else if (playerRings == 0 && playerStars > 0)
		{
			string format3 = LanguageStrings.First.GetString("STORE_BUNDLE_DESCRIPTION_STARS");
			text = string.Format(format3, LanguageUtils.FormatNumber(playerStars));
			string format4 = LanguageStrings.First.GetString("STORE_BUNDLE_DESCRIPTION_STARS_NOW");
			text3 = string.Format(format4, LanguageUtils.FormatNumber(playerStars));
		}
		else
		{
			string format5 = LanguageStrings.First.GetString("STORE_BUNDLE_DESCRIPTION_RINGS_AND_STARS");
			text = string.Format(format5, LanguageUtils.FormatNumber(playerRings), LanguageUtils.FormatNumber(playerStars));
			string format6 = LanguageStrings.First.GetString("STORE_BUNDLE_DESCRIPTION_RINGS_AND_STARS_NOW");
			text3 = string.Format(format6, LanguageUtils.FormatNumber(playerRings), LanguageUtils.FormatNumber(playerStars));
		}
		if (baseRings > 0 && baseStars == 0)
		{
			string format7 = LanguageStrings.First.GetString("STORE_BUNDLE_DESCRIPTION_RINGS_WAS");
			text2 = string.Format(format7, LanguageUtils.FormatNumber(baseRings));
		}
		else if (baseRings == 0 && baseStars > 0)
		{
			string format8 = LanguageStrings.First.GetString("STORE_BUNDLE_DESCRIPTION_STARS_WAS");
			text2 = string.Format(format8, LanguageUtils.FormatNumber(baseStars));
		}
		else
		{
			string format9 = LanguageStrings.First.GetString("STORE_BUNDLE_DESCRIPTION_RINGS_AND_STARS_WAS");
			text2 = string.Format(format9, LanguageUtils.FormatNumber(baseRings), LanguageUtils.FormatNumber(baseStars));
		}
		if (baseRings < playerRings || baseStars < playerStars)
		{
			SetLabelString(gameObject2, text2);
			SetLabelString(gameObject3, text3);
			gameObject2.SetActive(true);
			gameObject3.SetActive(true);
		}
		else
		{
			SetLabelString(gameObject, text);
			gameObject.SetActive(true);
		}
	}

	private static void DisplayEntryCount()
	{
		if (s_storeEntry.m_type == StoreContent.EntryType.PowerUp && (s_storeEntry.m_state & StoreContent.StoreEntry.State.OneShot) != StoreContent.StoreEntry.State.OneShot)
		{
			GameObject gameObject = s_contentObject[7];
			if (!(gameObject == null))
			{
				SetLabelString(gameObject, PowerUpsInventory.GetPowerUpCount(s_storeEntry.m_powerUpType).ToString());
				gameObject.SetActive(true);
			}
		}
	}

	private static void DisplayUpgradeCount()
	{
		if (s_storeEntry.m_type != StoreContent.EntryType.Upgrade)
		{
			return;
		}
		GameObject gameObject = s_contentObject[8];
		if (!(gameObject == null))
		{
			int powerUpLevel = PowerUpsInventory.GetPowerUpLevel(s_storeEntry.m_powerUpType);
			SetProgressValue(gameObject, 6, powerUpLevel);
			gameObject.SetActive(true);
			GameObject gameObject2 = s_contentObject[9];
			if (gameObject2 != null)
			{
				gameObject2.SetActive(true);
			}
		}
	}

	private static void DisplayDownloadCount()
	{
		if ((s_storeEntry.m_state & StoreContent.StoreEntry.State.Downloaded) != StoreContent.StoreEntry.State.Downloaded)
		{
			return;
		}
		GameObject gameObject = s_contentObject[8];
		if (!(gameObject == null))
		{
			gameObject.SetActive(true);
			UISlider componentInChildren = Utils.GetComponentInChildren<UISlider>(gameObject);
			componentInChildren.value = s_storeEntry.m_downloadProgress;
			GameObject gameObject2 = s_contentObject[9];
			if (gameObject2 != null)
			{
				gameObject2.SetActive(true);
			}
		}
	}

	private static void DisplayBuyButton()
	{
		GameObject gameObject = s_contentObject[2];
		GameObject gameObject2 = s_contentObject[3];
		GameObject gameObject3 = s_contentObject[4];
		GameObject gameObject4 = s_contentObject[11];
		GameObject gameObject5 = s_contentObject[16];
		GameObject gameObject6 = s_contentObject[15];
		if (gameObject == null)
		{
			return;
		}
		if (gameObject4 != null)
		{
			EnableBuyButton(gameObject4);
		}
		if (s_storeEntry.m_buttonText != null && s_storeEntry.m_buttonText.Length > 0)
		{
			LocalisedStringProperties.SetLocalisedString(gameObject, s_storeEntry.m_buttonText);
			gameObject.SetActive(true);
		}
		else if (s_storeEntry.m_payment == StoreContent.PaymentMethod.Free)
		{
			LocalisedStringProperties.SetLocalisedString(gameObject, "STORE_FREE");
			gameObject.SetActive(true);
		}
		else
		{
			if (s_storeEntry.m_payment != StoreContent.PaymentMethod.Money && s_storeEntry.m_payment != StoreContent.PaymentMethod.Rings && s_storeEntry.m_payment != StoreContent.PaymentMethod.StarRings)
			{
				return;
			}
			string text = null;
			string text2 = null;
			bool flag = false;
			if (s_storeEntry.m_payment == StoreContent.PaymentMethod.Money)
			{
				text = s_storeEntry.m_osStore.m_playerPrice;
				text2 = s_storeEntry.m_osStore.m_basePrice;
				if ((s_storeEntry.m_state & StoreContent.StoreEntry.State.OnSale) == StoreContent.StoreEntry.State.OnSale)
				{
					flag = true;
				}
			}
			else
			{
				int itemCost = StoreUtils.GetItemCost(s_storeEntry, StoreUtils.EntryType.Player);
				int itemCost2 = StoreUtils.GetItemCost(s_storeEntry, StoreUtils.EntryType.Base);
				if ((s_storeEntry.m_state & StoreContent.StoreEntry.State.OnSale) == StoreContent.StoreEntry.State.OnSale)
				{
					flag = true;
				}
				text = LanguageUtils.FormatNumber(itemCost);
				text2 = LanguageUtils.FormatNumber(itemCost2);
				if (s_storeEntry.m_payment == StoreContent.PaymentMethod.Rings && gameObject6 != null)
				{
					gameObject6.SetActive(true);
				}
				if (s_storeEntry.m_payment == StoreContent.PaymentMethod.StarRings && gameObject5 != null)
				{
					gameObject5.SetActive(true);
				}
			}
			if (!flag || gameObject2 == null || gameObject3 == null)
			{
				SetLabelString(gameObject, text);
				gameObject.SetActive(true);
				return;
			}
			string format = LanguageStrings.First.GetString("STORE_SALE_PRICE_WAS");
			string stringToDisplay = string.Format(format, text2);
			string format2 = LanguageStrings.First.GetString("STORE_SALE_PRICE_NOW");
			string stringToDisplay2 = string.Format(format2, text);
			SetLabelString(gameObject3, stringToDisplay2);
			SetLabelString(gameObject2, stringToDisplay);
			gameObject3.SetActive(true);
			gameObject2.SetActive(true);
		}
	}

	private static void DisplayPending()
	{
		GameObject gameObject = s_contentObject[17];
		if (!(gameObject == null))
		{
			gameObject.SetActive(true);
			for (int i = 0; i < gameObject.transform.childCount; i++)
			{
				gameObject.transform.GetChild(i).gameObject.SetActive(true);
			}
		}
	}

	private static void DisplayPurchasedInfo()
	{
		GameObject gameObject = s_contentObject[12];
		if (gameObject == null || s_storeEntry.m_purchasedString == null || s_storeEntry.m_purchasedString.Length == 0)
		{
			return;
		}
		LocalisedStringProperties.SetLocalisedString(gameObject, s_storeEntry.m_purchasedString);
		if (s_storeEntry.m_purchasedString == "BUTTON_DOWNLOAD")
		{
			GameObject gameObject2 = s_contentObject[11];
			if (gameObject2 != null)
			{
				EnableBuyButton(gameObject2);
			}
		}
		gameObject.SetActive(true);
	}

	private static void DisplayDownloadingInfo()
	{
		GameObject gameObject = s_contentObject[14];
		if (!(gameObject == null))
		{
			gameObject.SetActive(true);
		}
	}

	private static void DisplayUnavailableInfo()
	{
		GameObject gameObject = s_contentObject[13];
		if (!(gameObject == null))
		{
			gameObject.SetActive(true);
		}
	}

	private static void Display3DMesh()
	{
		GameObject gameObject = s_contentObject[18];
		if (!(gameObject == null))
		{
			MeshFilter[] componentsInChildren = gameObject.GetComponentsInChildren<MeshFilter>(true);
			if (componentsInChildren != null && componentsInChildren.Length != 0)
			{
				componentsInChildren[0].mesh = s_storeEntry.m_mesh;
				gameObject.SetActive(true);
			}
		}
	}

	private static void DisplaySaleTag()
	{
		GameObject gameObject = s_contentObject[22];
		if (!(gameObject == null) && (s_storeEntry.m_state & StoreContent.StoreEntry.State.OnSale) == StoreContent.StoreEntry.State.OnSale)
		{
			gameObject.SetActive(true);
		}
	}

	private static void DisplayOfferStatement()
	{
		bool flag = (s_storeEntry.m_state & StoreContent.StoreEntry.State.RemoveAds) == StoreContent.StoreEntry.State.RemoveAds && !PaidUser.Paid && !PaidUser.RemovedAds;
		if (s_storeEntry.m_offerSellText != null && s_storeEntry.m_offerSellText.Length > 0)
		{
			GameObject gameObject = s_contentObject[19];
			GameObject gameObject2 = s_contentObject[20];
			if ((bool)gameObject)
			{
				LocalisedStringProperties.SetLocalisedString(gameObject, s_storeEntry.m_offerSellText);
				gameObject.SetActive(true);
			}
			if ((bool)gameObject2)
			{
				gameObject2.SetActive(true);
			}
		}
		if (flag)
		{
			GameObject gameObject3 = s_contentObject[21];
			if ((bool)gameObject3)
			{
				gameObject3.SetActive(true);
			}
		}
	}

	private static void UpdatePurchaseButtons(Display displayFlags)
	{
		switch (DeterminePurchaseOption(displayFlags))
		{
		case PurchaseOptions.Available:
			DisplayBuyButton();
			break;
		case PurchaseOptions.Unavailable:
			DisplayUnavailableInfo();
			break;
		case PurchaseOptions.Downloading:
			DisplayDownloadingInfo();
			break;
		case PurchaseOptions.Pending:
			DisplayPending();
			break;
		case PurchaseOptions.Purchased:
			DisplayPurchasedInfo();
			break;
		}
	}

	private static PurchaseOptions DeterminePurchaseOption(Display displayFlags)
	{
		if ((s_storeEntry.m_state & StoreContent.StoreEntry.State.Downloading) == StoreContent.StoreEntry.State.Downloading)
		{
			return PurchaseOptions.Downloading;
		}
		if (s_storeEntry.m_payment == StoreContent.PaymentMethod.Money && ((s_storeEntry.m_osStore.m_baseState != ProductStateCode.ProductInfoDone && s_storeEntry.m_osStore.m_baseState != ProductStateCode.ProductInfoDoneRrefershing) || (s_storeEntry.m_osStore.m_playerState != ProductStateCode.ProductInfoDone && s_storeEntry.m_osStore.m_playerState != ProductStateCode.ProductInfoDoneRrefershing)))
		{
			return PurchaseOptions.Unavailable;
		}
		if (StoreUtils.IsStoreActive() && (displayFlags & Display.IgnoreTimeOut) != Display.IgnoreTimeOut)
		{
			return PurchaseOptions.Pending;
		}
		if ((s_storeEntry.m_state & StoreContent.StoreEntry.State.Purchased) == StoreContent.StoreEntry.State.Purchased)
		{
			return PurchaseOptions.Purchased;
		}
		if (s_storeEntry.m_type == StoreContent.EntryType.Upgrade)
		{
			int powerUpLevel = PowerUpsInventory.GetPowerUpLevel(s_storeEntry.m_powerUpType);
			if (powerUpLevel == 6)
			{
				return PurchaseOptions.Purchased;
			}
		}
		return PurchaseOptions.Available;
	}

	private static void EnableBuyButton(GameObject buttonNode)
	{
		buttonNode.SetActive(true);
		if (s_contentObject[10] != null)
		{
			Transform[] componentsInChildren = s_contentObject[10].GetComponentsInChildren<Transform>(true);
			Transform[] array = componentsInChildren;
			foreach (Transform transform in array)
			{
				transform.gameObject.SetActive(false);
			}
			s_contentObject[10].SetActive(true);
		}
		if (s_contentObject[17] != null)
		{
			Transform[] componentsInChildren2 = s_contentObject[17].GetComponentsInChildren<Transform>(true);
			Transform[] array2 = componentsInChildren2;
			foreach (Transform transform2 in array2)
			{
				transform2.gameObject.SetActive(false);
			}
			s_contentObject[17].SetActive(true);
		}
	}

	private static void SetLabelString(GameObject target, string stringToDisplay)
	{
		UILabel componentInChildren = Utils.GetComponentInChildren<UILabel>(target);
		componentInChildren.text = stringToDisplay;
		LocalisedStringProperties componentInChildren2 = Utils.GetComponentInChildren<LocalisedStringProperties>(target);
		if (componentInChildren2 != null)
		{
			componentInChildren2.SetLocalisationID(null);
		}
	}

	private static void SetProgressValue(GameObject progressBar, int maxProgress, int currentProgress)
	{
		UISlider componentInChildren = Utils.GetComponentInChildren<UISlider>(progressBar);
		componentInChildren.value = (float)currentProgress / (float)maxProgress;
	}
}
