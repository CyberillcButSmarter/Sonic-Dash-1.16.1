using System;
using UnityEngine;

public class PlugInController : MonoBehaviour
{
	[Flags]
	private enum State
	{
		GameSetUpComplete = 1
	}

	private enum Region
	{
		Boot = 0,
		Game = 1
	}

	private State m_state;

	private object[] m_restorePurchasesCompleteParams = new object[1];

	private object[] m_productStateChangedParams = new object[1];

	private object[] m_unverififedParams = new object[1];

	[SerializeField]
	private Region m_runRegion = Region.Game;

	private void Start()
	{
		SetAPITokens();
		if (m_runRegion == Region.Boot)
		{
			InitialiseOnBoot();
			return;
		}
		EventDispatch.RegisterInterest("OnAllAssetsLoaded", this);
		InitialiseOnStart();
	}

	private void Update()
	{
		if (m_runRegion != Region.Boot)
		{
			UpdatePlugIns();
		}
	}

	public void Event3rdPartyActive(string Param)
	{
		EventDispatch.GenerateEvent("3rdPartyActive", null);
	}

	public void Event3rdPartyInactive(string Param)
	{
		EventDispatch.GenerateEvent("3rdPartyInactive", null);
	}

	public void EventAdShown(string Param)
	{
		GameAnalytics.AdvertShown();
	}

	public void EventPlayerFirstSeenOn(string Param)
	{
	}

	public void EventCompletedGoogleSync()
	{
		CloudStorage.EventGoogleSync();
	}

	public void ProductStateChanged(string ProudctID)
	{
		m_productStateChangedParams[0] = ProudctID;
		EventDispatch.GenerateEvent("ProductStateChanged", m_productStateChangedParams);
	}

	public void RestorePurchasesCompleted(string Param)
	{
		m_restorePurchasesCompleteParams[0] = Param;
		EventDispatch.GenerateEvent("RestorePurchasesComplete", m_restorePurchasesCompleteParams);
	}

	public void UnableToVerifyReceipt(string Param)
	{
		m_unverififedParams[0] = Param;
		EventDispatch.GenerateEvent("PurchaseUnableToVerifyReceipt", m_unverififedParams);
	}

	public void DirectPaymentRequested(string productID)
	{
		bool approved = true;
		if (StoreContent.GetStoreEntry(productID, StoreContent.Identifiers.OsStore) == null)
		{
			DialogContent_GeneralInfo.Display(DialogContent_GeneralInfo.Type.UnableToProcessPayment);
			approved = false;
		}
		SLStorePlugin.DirectPurchaseRequestedResult(productID, approved);
	}

	public void AppLeavesBackground(string properties)
	{
		Debug.Log("SLPlugin::AppLeavesBackground");
		EventDispatch.GenerateEvent("SessionStarted");
		EventDispatch.GenerateEvent("OnAppLeavesBackground");
	}

	private void SetAPITokens()
	{
		SLPlugin.SetToken("AppVersion", "1.16.1");
		SLPlugin.SetToken("FlurryID", "KZRMMZ2DCJDGPM92P9K4");
		SLPlugin.SetToken("Release", "FINAL");
		SLPlugin.SetToken("VungleAppID", "525bb61fd5fcfa5236000011");
		SLPlugin.SetToken("BurstlyAppID", "4GS2DIy1TkSF24Je_65YMg");
		SLPlugin.SetToken("BurstlyInterstitialID", "0751987889181224792");
		if (Screen.width >= 768)
		{
			SLPlugin.SetToken("MoPubAdUnitID", "6da28757634c4992b9dfa9721118fe2a");
		}
		else
		{
			SLPlugin.SetToken("MoPubAdUnitID", "78c42f9c9d914593835d14bec86b6bda");
		}
		SLPlugin.SetToken("INTERSTITIAL_AD", "0751987889181224792");
		SLPlugin.SetToken("VIDEO_REWARD_AD", "6677ec6a694a4abc944e6afbf1d19bef");
		SLPlugin.SetToken("AdSpaceMainMenu", "0751987789181224792");
		SLPlugin.SetToken("AdSpaceResultScreen", "0851987789181224792");
		SLPlugin.SetToken("AdSpaceAndroidTest", "0359987989181214792");
		SLPlugin.SetToken("PlayhavenToken", "6c71f06b5a754857a11a80d52102d687");
		SLPlugin.SetToken("PlayhavenSecret", "12c85d4217184277adccfdc918a4453d");
		SLPlugin.SetToken("ApplifierGameID", "11742");
		SLPlugin.SetToken("ApplifierID", "lgcagpimjimjmipklbgggggh");
		SLPlugin.SetToken("ChartboostAppID", "52778d3816ba47722f000018");
		SLPlugin.SetToken("ChartboostAppSig", "6d765f6d2a68d24f1bf0073e44115757bb7a6e5f");
		SLPlugin.SetToken("NativeXAppID", "14764");
		SLPlugin.SetToken("GCMProjectNumber", "1037835710459");
		SLPlugin.SetToken("TapJoyAppID", "70e0a697-a1c3-4407-98b4-06184a668a32");
		SLPlugin.SetToken("TapJoySecret", "7TTdcIaC6KEQ5y2Don9j");
	}

	private void InitialiseOnBoot()
	{
		if (!GameUnloader.HasReloaded)
		{
			SLPlugin.Init();
			SLAnalytics.Start();
		}
		EventDispatch.GenerateEvent("SoftLightPlugIns_InitialisedOnBoot");
	}

	private void InitialiseOnStart()
	{
		if (!GameUnloader.HasReloaded)
		{
		}
		EventDispatch.GenerateEvent("SoftLightPlugIns_InitialisedOnStart");
	}

	private void InitialiseOnGame()
	{
		if (!GameUnloader.HasReloaded)
		{
			SLStorePlugin.Start();
			int interstitalProvider = 0;
			AdProvider moreGamesProvider = AdProvider.Playhaven;
			AdProvider offersProvider = AdProvider.None;
			AdProvider gameOffersProvider = AdProvider.None;
			AdProvider pushProvider = AdProvider.None;
			AdProvider videoProvider = AdProvider.None;
			AdProvider loginProvider = AdProvider.None;
			bool flag = Language.GetLanguage() == Language.ID.Korean || Language.GetLanguage() == Language.ID.Chinese || Language.GetLocale() == Language.Locale.China || Language.GetLocale() == Language.Locale.Korea;
			if (Language.GetLanguage() != Language.ID.Japanese && Language.GetLocale() != Language.Locale.Japan && !flag)
			{
				moreGamesProvider = AdProvider.Playhaven;
				offersProvider = AdProvider.Playhaven;
				videoProvider = AdProvider.Vungle;
				interstitalProvider = 865;
				videoProvider = AdProvider.MoPub;
				interstitalProvider |= 2;
			}
			SLAds.Init(interstitalProvider, moreGamesProvider, offersProvider, gameOffersProvider, pushProvider, videoProvider, loginProvider);
			SLAds.PrepareVideoAd("INTERSTITIAL_AD");
			if (SLPlugin.IsCupCakeTasty())
			{
				SLAds.PrepareA4P(true);
			}
			SLAds.PrepareA4P(false);
		}
		EventDispatch.GenerateEvent("SoftLightPlugIns_InitialisedOnGame");
	}

	private void UpdatePlugIns()
	{
		SLStorePlugin.Update();
		SLAds.Update();
	}

	private void Event_OnAllAssetsLoaded()
	{
		if ((m_state & State.GameSetUpComplete) != State.GameSetUpComplete)
		{
			InitialiseOnGame();
			m_state |= State.GameSetUpComplete;
		}
	}
}
