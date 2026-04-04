using System;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class GameAnalytics : MonoBehaviour
{
	public class RSRAnalyticsParam
	{
		public int Amount { get; private set; }

		public RingsRecievedReason Reason { get; private set; }

		public RSRAnalyticsParam(int amount, RingsRecievedReason reason)
		{
			Amount = amount;
			Reason = reason;
		}
	}

	public enum PurchaseLocations
	{
		MainMenu = 0,
		ShopMainMenu = 1,
		MissionsMainMenu = 2,
		PauseScreen = 3,
		ShopPauseScreen = 4,
		MissionsPauseScreen = 5,
		ContinueScreen = 6,
		HintScreen = 7,
		ResultScreen = 8,
		ShopResultScreen = 9,
		MissionsResultScreen = 10,
		DoubleRingsScreen = 11,
		MissionsMenuResultScreen = 12,
		LeaderboardMainMenu = 13,
		StatsMenuResultScreen = 14,
		LeaderboardMenuResultScreen = 15,
		FreeRingsButton = 16,
		DCScreen = 17,
		DCScreenResultScreen = 18,
		WOFScreen = 19,
		WOFScreenResultScreen = 20,
		BoosterScreen = 21,
		Plus_MainMenu = 22,
		Plus_ShopMainMenu = 23,
		Plus_MissionsMainMenu = 24,
		Plus_PauseScreen = 25,
		Plus_ShopPauseScreen = 26,
		Plus_MissionsPauseScreen = 27,
		Plus_ContinueScreen = 28,
		Plus_HintScreen = 29,
		Plus_ResultScreen = 30,
		Plus_ShopResultScreen = 31,
		Plus_MissionsResultScreen = 32,
		Plus_DoubleRingsScreen = 33,
		Plus_MissionsMenuResultScreen = 34,
		Plus_LeaderboardMainMenu = 35,
		Plus_StatsMenuResultScreen = 36,
		Plus_LeaderboardMenuResultScreen = 37,
		Plus_FreeRingsButton = 38,
		Plus_DCScreen = 39,
		Plus_DCScreenResultScreen = 40,
		Plus_WOFScreen = 41,
		Plus_WOFScreenResultScreen = 42,
		Plus_BoosterScreen = 43
	}

	public enum FacebookLoginLocations
	{
		Boot = 0,
		Popup = 1,
		LeaderboardMenu = 2,
		LeaderboardResult = 3,
		ReminderPopup = 4
	}

	public enum RateMeButtons
	{
		Never = 0,
		Remember = 1,
		Ok = 2
	}

	public enum AppUpdateChoice
	{
		Update = 0,
		Cancel = 1
	}

	public enum CancelContinueReasons
	{
		Timeout = 0,
		CancelPurchase = 1,
		Skip = 2
	}

	public enum RingsRecievedReason
	{
		Puchased = 0,
		Rewarded = 1,
		MissionSet = 2,
		Highscore = 3,
		Return = 4,
		Leaderboard = 5,
		Brag = 6,
		Tutorial = 7,
		CollectedInRun = 8,
		Restored = 9,
		ReviveConversion = 10
	}

	public enum LoadingStage
	{
		Ignore = 0,
		LoadingScreenHL = 1,
		LoadingScreenSD = 2,
		DownloadFeatureStateAB = 3,
		DownloadFeatureStateDefault = 4,
		DownloadGC6 = 5,
		DownloadOfferState = 6,
		DownloadABConfig = 7,
		DownloadABTestingAB = 8,
		DownloadABTestingDefault = 9,
		DownloadStoreModifierAB = 10,
		DownloadStoreModifierDefault = 11
	}

	private const int m_maxParams = 10;

	private const string FacebookReadAnalyticSentProperty = "FacebookReadAnalyticSent";

	private const string FacebookWriteAnalyticSentProperty = "FacebookWriteAnalyticSent";

	[SerializeField]
	public UILabel m_debugLabel;

	private bool m_skipLoad = true;

	public static int LocationsTop = 22;

	public static bool s_playerDeath = false;

	public static readonly int NoofLoadingStages = Utils.GetEnumCount<LoadingStage>();

	private static LoadingStage[] s_fileToStage = null;

	private static LoadingStage[] s_screenToStage = null;

	private static PlayerStats.StatNames[] m_xAxys = new PlayerStats.StatNames[4]
	{
		PlayerStats.StatNames.TimePlayed_Total,
		PlayerStats.StatNames.NumberOfRuns_Total,
		PlayerStats.StatNames.ShopPurchases_Total,
		PlayerStats.StatNames.MissionsCompleted_Total
	};

	private static PlayerStats.DistanceNames[] m_xAxysDistances = new PlayerStats.DistanceNames[1];

	private static PlayerStats.StatNames[] m_endRun = new PlayerStats.StatNames[8]
	{
		PlayerStats.StatNames.TimePlayed_Run,
		PlayerStats.StatNames.RingsCollected_Run,
		PlayerStats.StatNames.RingsBanked_Run,
		PlayerStats.StatNames.MaxMultiplier_Run,
		PlayerStats.StatNames.DashUses_Run,
		PlayerStats.StatNames.PowerMagnetsPicked_Run,
		PlayerStats.StatNames.Enemies_Run,
		PlayerStats.StatNames.RevivesUsed_Run
	};

	private static PlayerStats.DistanceNames[] m_endRunDistances = new PlayerStats.DistanceNames[1] { PlayerStats.DistanceNames.DistanceRun_Run };

	private static PlayerStats.StatNames[] m_continue = new PlayerStats.StatNames[5]
	{
		PlayerStats.StatNames.TimePlayed_Run,
		PlayerStats.StatNames.RingsCollected_Run,
		PlayerStats.StatNames.RingsBanked_Run,
		PlayerStats.StatNames.Enemies_Run,
		PlayerStats.StatNames.RevivesUsed_Run
	};

	private static PurchaseLocations s_currentLocation = PurchaseLocations.MainMenu;

	private static FacebookLoginLocations s_facebookLoginLocation = FacebookLoginLocations.Boot;

	private static bool s_facebookReadAnalyticSent = false;

	private static bool s_facebookWriteAnalyticSent = false;

	private static bool s_friendsAnalyticSent;

	private static PlayerStats.Stats s_stats;

	private void Update()
	{
		if (m_debugLabel != null)
		{
			m_debugLabel.text = GetPurchaseLocation().ToString();
		}
	}

	public static void SetPurchaseLocation(PurchaseLocations location)
	{
		s_currentLocation = location;
	}

	public static PurchaseLocations GetPurchaseLocation()
	{
		return s_currentLocation;
	}

	public static void SetFacebookLocation(FacebookLoginLocations location)
	{
		s_facebookLoginLocation = location;
	}

	private static LoadingStage FileDownloaderToLoadingStage(FileDownloader.Files file)
	{
		if (s_fileToStage == null)
		{
			s_fileToStage = new LoadingStage[Utils.GetEnumCount<FileDownloader.Files>()];
			s_fileToStage[4] = LoadingStage.DownloadFeatureStateAB;
			s_fileToStage[5] = LoadingStage.DownloadFeatureStateDefault;
			s_fileToStage[0] = LoadingStage.Ignore;
			s_fileToStage[1] = LoadingStage.DownloadGC6;
			s_fileToStage[2] = LoadingStage.DownloadOfferState;
			s_fileToStage[3] = LoadingStage.DownloadABConfig;
			s_fileToStage[6] = LoadingStage.DownloadABTestingAB;
			s_fileToStage[7] = LoadingStage.DownloadABTestingDefault;
			s_fileToStage[8] = LoadingStage.DownloadStoreModifierAB;
			s_fileToStage[9] = LoadingStage.DownloadStoreModifierDefault;
		}
		if (file > FileDownloader.Files.SegaTime && (int)file < s_fileToStage.Length)
		{
			return s_fileToStage[(int)file];
		}
		return LoadingStage.Ignore;
	}

	private static LoadingStage LoadingScreenToLoadingStage(LoadingScreenProperties.ScreenType screenType)
	{
		if (s_screenToStage == null)
		{
			s_screenToStage = new LoadingStage[3];
			s_screenToStage[0] = LoadingStage.Ignore;
			s_screenToStage[1] = LoadingStage.LoadingScreenHL;
			s_screenToStage[2] = LoadingStage.LoadingScreenSD;
		}
		if (screenType > LoadingScreenProperties.ScreenType.SegaLogo && (int)screenType < s_screenToStage.Length)
		{
			return s_screenToStage[(int)screenType];
		}
		return LoadingStage.Ignore;
	}

	public static void NotifyFileDownload(FileDownloader.Files file, float seconds)
	{
		LoadingStage loadingStage = FileDownloaderToLoadingStage(file);
		if (loadingStage != LoadingStage.Ignore)
		{
			SLAnalytics.AddParameter("File", loadingStage.ToString());
			SLAnalytics.AddParameter("RawTime", seconds.ToString());
			SLAnalytics.AddParameter("QuantizedTime", QuantizeValue(file, seconds));
			SLAnalytics.LogEventWithParameters("FileDownloadComplete");
		}
	}

	public static void NotifyLoadScreen(LoadingScreenProperties screen, float seconds, bool initialLoad)
	{
		LoadingStage loadingStage = LoadingScreenToLoadingStage(screen.m_screenType);
		if (loadingStage != LoadingStage.Ignore)
		{
			SLAnalytics.AddParameter("Page", loadingStage.ToString());
			SLAnalytics.AddParameter("InitialLoad", initialLoad.ToString());
			SLAnalytics.AddParameter("RawTime", seconds.ToString());
			SLAnalytics.AddParameter("QuantizedTime", QuantizeValue(screen.m_screenType, seconds));
			SLAnalytics.LogEventWithParameters("LoadingScreenComplete");
		}
	}

	public static void SessionInfo()
	{
		SLAnalytics.AddParameter("ID", UserIdentification.Current);
		SLAnalytics.AddParameter("SessionNumber", PlayerStats.GetStat(PlayerStats.StatNames.NumberOfSessions_Total).ToString());
		string deviceModel = SystemInfo.deviceModel;
		SLAnalytics.AddParameter("Device", deviceModel);
		SLAnalytics.LogEventWithParameters("SessionInfo");
	}

	public static void FirstGame()
	{
		string value = Language.GetLanguage().ToString();
		string value2 = Language.GetLocale().ToString();
		SLAnalytics.AddParameter("Language", value);
		SLAnalytics.AddParameter("Locale", value2);
		string value3 = ((!Application.genuineCheckAvailable) ? "Unknown" : ((!Application.genuine) ? "Not Genuine" : "Genuine"));
		SLAnalytics.AddParameter("GenuineApp", value3);
		string value4 = DateTime.Today.Date.ToString("dd.MM");
		SLAnalytics.AddParameter("Date", value4);
		string deviceModel = SystemInfo.deviceModel;
		SLAnalytics.AddParameter("Device", deviceModel);
		SLAnalytics.AddParameter("OSVersion", SystemInfo.operatingSystem);
		SLAnalytics.LogEventWithParameters("InitialBoot");
		GameUpdated(true);
	}

	public static void GameUpdated(bool firstBoot)
	{
		SLAnalytics.AddParameter("Version", "1.16.1");
		SLAnalytics.AddParameter("FirstBoot", firstBoot.ToString());
		SLAnalytics.LogEventWithParameters("Update");
	}

	public static void EnterShop()
	{
		SLAnalytics.AddParameter("PurchaseLocation", s_currentLocation.ToString());
		XAxysProperties();
		SLAnalytics.LogEventWithParameters("EnterShop");
	}

	public static void ShopPurchaseCompleted(string itemID)
	{
		PlayerStats.IncreaseStat(PlayerStats.StatNames.ShopPurchases_Total, 1);
		PlayerStats.IncreaseStat(PlayerStats.StatNames.ShopPurchases_Session, 1);
		PlayerStats.IncreaseStat(PlayerStats.StatNames.ShopPurchases_Run, 1);
		SLAnalytics.AddParameter("ItemID", itemID);
		SLAnalytics.AddParameter("PurchaseLocation", s_currentLocation.ToString());
		XAxysProperties();
		SLAnalytics.LogEventWithParameters("ShopPurchaseCompleted");
		SLAnalytics.AddParameter("PurchaseLocation", s_currentLocation.ToString());
		XAxysProperties();
		SLAnalytics.LogEventWithParameters("ShopPurchaseCompleted_" + itemID);
		if (itemID.EndsWith("_upgrade"))
		{
			SLAnalytics.AddParameter("PurchaseLocation", s_currentLocation.ToString());
			XAxysProperties();
			switch (itemID)
			{
			case "magnet_upgrade":
				SLAnalytics.LogEventWithParameters("ShopPurchaseCompleted_" + itemID + PowerUpsInventory.GetPowerUpLevel(PowerUps.Type.Magnet));
				break;
			case "headstart_upgrade":
				SLAnalytics.LogEventWithParameters("ShopPurchaseCompleted_" + itemID + PowerUpsInventory.GetPowerUpLevel(PowerUps.Type.HeadStart));
				break;
			case "dash_burndown_upgrade":
				SLAnalytics.LogEventWithParameters("ShopPurchaseCompleted_" + itemID + PowerUpsInventory.GetPowerUpLevel(PowerUps.Type.DashLength));
				break;
			case "dash_fillrate_upgrade":
				SLAnalytics.LogEventWithParameters("ShopPurchaseCompleted_" + itemID + PowerUpsInventory.GetPowerUpLevel(PowerUps.Type.DashIncrease));
				break;
			case "shield_upgrade":
				SLAnalytics.LogEventWithParameters("ShopPurchaseCompleted_" + itemID + PowerUpsInventory.GetPowerUpLevel(PowerUps.Type.Shield));
				break;
			default:
				SLAnalytics.LogEventWithParameters("ShopPurchaseCompleted_UNKNOWN_LEVEL");
				break;
			}
		}
		if (itemID.StartsWith("character_"))
		{
			SLAnalytics.AddParameter("Character", itemID.Replace("character_", string.Empty));
			XAxysProperties();
			SLAnalytics.LogEventWithParameters("CharacterPurchased");
		}
	}

	public static void ShopPurchaseFailed(string itemID)
	{
		SLAnalytics.AddParameter("ItemID", itemID);
		SLAnalytics.AddParameter("PurchaseLocation", s_currentLocation.ToString());
		XAxysProperties();
		SLAnalytics.LogEventWithParameters("ShopPurchaseFailed");
	}

	public static void InAppPurchaseCompleted(StoreContent.StoreEntry storeEntry, string transactionId, bool internalPurchase)
	{
		PlayerStats.IncreaseStat(PlayerStats.StatNames.InAppPurchases_Total, 1);
		PlayerStats.IncreaseStat(PlayerStats.StatNames.InAppPurchases_Session, 1);
		PlayerStats.IncreaseStat(PlayerStats.StatNames.InAppPurchases_Run, 1);
		SLAnalytics.AddParameter("ItemID", storeEntry.m_osStore.m_playeriTunesId);
		SLAnalytics.AddParameter("PurchaseLocation", s_currentLocation.ToString());
		SLAnalytics.AddParameter("AutoIAP", internalPurchase.ToString());
		SLAnalytics.AddParameter("Price", storeEntry.m_osStore.m_playerNumericalPrice);
		SLAnalytics.AddParameter("Currency", storeEntry.m_osStore.m_playerCurrencyCode);
		SLAnalytics.AddParameter("TransactionId", transactionId);
		SLAnalytics.AddParameter("Amount", "1");
		XAxysProperties();
		SLAnalytics.LogEventWithParameters("InAppPurchaseCompleted");
		SLAnalytics.AddParameter("PurchaseLocation", s_currentLocation.ToString());
		SLAnalytics.AddParameter("AutoIAP", internalPurchase.ToString());
		XAxysProperties();
		SLAnalytics.LogEventWithParameters("InAppPurchaseCompleted_" + storeEntry.m_osStore.m_playeriTunesId);
	}

	public static void InAppPurchaseFailed(string itemID, bool internalPurchase, PaymentErrorCode errorCode)
	{
		SLAnalytics.AddParameter("ItemID", itemID);
		SLAnalytics.AddParameter("PurchaseLocation", s_currentLocation.ToString());
		SLAnalytics.AddParameter("AutoIAP", internalPurchase.ToString());
		SLAnalytics.AddParameter("PurchaseFailReason", errorCode.ToString());
		XAxysProperties();
		SLAnalytics.LogEventWithParameters("InAppPurchaseFailed");
	}

	public static void RemoveAdsPurchased(string itemID)
	{
		SLAnalytics.AddParameter("ItemID", itemID);
		SLAnalytics.LogEventWithParameters("RemoveAdsPurchased");
	}

	public static void RSRGiven(int amountGiven, int amountHad, RingsRecievedReason reason)
	{
		int num = amountHad + amountGiven;
		int num2 = PlayerStats.GetCurrentStats().m_trackedStats[64];
		SLAnalytics.AddParameter("RSRAmountGiven", amountGiven.ToString());
		SLAnalytics.AddParameter("RSRAmountTotalNow", num.ToString());
		SLAnalytics.AddParameter("TotalTimePlayedFor", num2.ToString());
		SLAnalytics.AddParameter("ReasonForGiving", reason.ToString("F"));
		SLAnalytics.LogEventWithParameters("RedStarRingsGiven");
	}

	public static void RSRTaken(int amountTaken, int amountHad)
	{
		int num = amountHad - amountTaken;
		int num2 = PlayerStats.GetCurrentStats().m_trackedStats[64];
		SLAnalytics.AddParameter("RSRAmountTaken", amountTaken.ToString());
		SLAnalytics.AddParameter("RSRAmountTotalNow", num.ToString());
		SLAnalytics.AddParameter("TotalTimePlayedFor", num2.ToString());
		SLAnalytics.LogEventWithParameters("RedStarRingsTaken");
	}

	public static void RingsGiven(int amountGiven, int amountHad, RingsRecievedReason reason)
	{
		int num = amountHad + amountGiven;
		int num2 = PlayerStats.GetCurrentStats().m_trackedStats[64];
		SLAnalytics.AddParameter("RingsAmountGiven", amountGiven.ToString());
		SLAnalytics.AddParameter("RingsAmountTotalNow", num.ToString());
		SLAnalytics.AddParameter("TotalTimePlayedFor", num2.ToString());
		SLAnalytics.AddParameter("ReasonForGiving", reason.ToString("F"));
		SLAnalytics.LogEventWithParameters("GoldRingsGiven");
	}

	public static void RingsTaken(int amountTaken, int amountHad)
	{
		int num = amountHad - amountTaken;
		int num2 = PlayerStats.GetCurrentStats().m_trackedStats[64];
		SLAnalytics.AddParameter("RingsAmountGiven", amountTaken.ToString());
		SLAnalytics.AddParameter("RingsAmountTotalNow", num.ToString());
		SLAnalytics.AddParameter("TotalTimePlayedFor", num2.ToString());
		SLAnalytics.LogEventWithParameters("GoldRingsTaken");
	}

	public static void PlayerDeath(string reason)
	{
		if (!s_playerDeath)
		{
			SLAnalytics.AddParameter("Reason", reason);
			SLAnalytics.AddParameter("Template", GetCurrentTemplate());
			XAxysProperties();
			SLAnalytics.LogEventWithParameters("PlayerDeath");
			s_playerDeath = true;
		}
	}

	public static void ContinueUsed(string itemID)
	{
		SLAnalytics.AddParameter("ItemID", itemID);
		PlayerStats.StatNames[] array = m_continue;
		foreach (PlayerStats.StatNames statNames in array)
		{
			SLAnalytics.AddParameter(statNames.ToString(), QuantizeValue(statNames, s_stats.m_trackedStats[(int)statNames]));
		}
		PlayerStats.DistanceNames[] endRunDistances = m_endRunDistances;
		foreach (PlayerStats.DistanceNames distanceNames in endRunDistances)
		{
			SLAnalytics.AddParameter(distanceNames.ToString(), QuantizeValue(distanceNames, s_stats.m_trackedDistances[(int)distanceNames]));
		}
		SLAnalytics.LogEventWithParameters("ContinueUsed");
		s_playerDeath = false;
	}

	public static void ContinueCancelled(CancelContinueReasons reason)
	{
		SLAnalytics.AddParameter("Reason", reason.ToString());
		PlayerStats.StatNames statNames = PlayerStats.StatNames.RevivesUsed_Run;
		SLAnalytics.AddParameter(statNames.ToString(), QuantizeValue(statNames, s_stats.m_trackedStats[(int)statNames]));
		XAxysProperties();
		SLAnalytics.LogEventWithParameters("ContinueCancelled");
		s_playerDeath = false;
	}

	public static void RunStart(Characters.Type character)
	{
		SLAnalytics.AddParameter("Character", Enum.GetName(typeof(Characters.Type), character));
		SLAnalytics.LogEventWithParameters("RunStart");
		s_playerDeath = false;
	}

	public static void EndRun()
	{
		SLAnalytics.AddParameter("Score", QuantizeScore(ScoreTracker.CurrentScore));
		PlayerStats.StatNames[] endRun = m_endRun;
		foreach (PlayerStats.StatNames statNames in endRun)
		{
			SLAnalytics.AddParameter(statNames.ToString(), QuantizeValue(statNames, s_stats.m_trackedStats[(int)statNames]));
		}
		PlayerStats.DistanceNames[] endRunDistances = m_endRunDistances;
		foreach (PlayerStats.DistanceNames distanceNames in endRunDistances)
		{
			SLAnalytics.AddParameter(distanceNames.ToString(), QuantizeValue(distanceNames, s_stats.m_trackedDistances[(int)distanceNames]));
		}
		SLAnalytics.LogEventWithParameters("EndRun");
		s_playerDeath = false;
	}

	public static void EndRunBoosters()
	{
		SLAnalytics.AddParameter("Score", QuantizeScore(ScoreTracker.CurrentScore));
		SLAnalytics.AddParameter("ScoreMultiplier", ScoreTracker.BaseMultiplier.ToString());
		for (int i = 0; i < Boosters.GetBoostersSelected.Length; i++)
		{
			SLAnalytics.AddParameter(Value: (Boosters.GetBoostersSelected[i] < 0) ? "None" : ((PowerUps.Type)Boosters.GetBoostersSelected[i]).ToString(), Key: "BoosterSlot" + (i + 1));
		}
		SLAnalytics.AddParameter(PlayerStats.DistanceNames.DistanceRun_Run.ToString(), QuantizeValue(PlayerStats.DistanceNames.DistanceRun_Run, s_stats.m_trackedDistances[7]));
		SLAnalytics.LogEventWithParameters("EndRunBoosters");
	}

	public static void DCCompleted()
	{
		SLAnalytics.AddParameter("DayNumber", (DCs.GetInternalDayNumber() + 1).ToString());
		SLAnalytics.LogEventWithParameters("DCCompleted");
	}

	public static void DCReset()
	{
		SLAnalytics.AddParameter("DayNumber", (DCs.GetInternalDayNumber() + 1).ToString());
		SLAnalytics.LogEventWithParameters("DCReset");
	}

	public static void DCDay5Rewarded(int amount, string reward)
	{
		SLAnalytics.AddParameter("DayNumber", (DCs.GetInternalDayNumber() + 1).ToString());
		SLAnalytics.AddParameter("DCReward", amount + "x" + reward);
		SLAnalytics.LogEventWithParameters("DCDay5Rewarded");
	}

	public static void HintShown(string hint)
	{
		SLAnalytics.AddParameter("Hint", hint);
		SLAnalytics.LogEventWithParameters("HintShown");
	}

	public static void MissionComplete(string missionName, bool purchased)
	{
		SLAnalytics.AddParameter("MissionCompleted", missionName);
		SLAnalytics.AddParameter("Skipped", purchased.ToString());
		XAxysProperties();
		SLAnalytics.LogEventWithParameters("MissionComplete");
	}

	public static void FacebookLike()
	{
		SLAnalytics.AddParameter("Location", s_currentLocation.ToString());
		XAxysProperties();
		SLAnalytics.LogEventWithParameters("FacebookLike");
	}

	public static void TwitterFollow()
	{
		SLAnalytics.AddParameter("Location", s_currentLocation.ToString());
		XAxysProperties();
		SLAnalytics.LogEventWithParameters("TwitterFollow");
	}

	public static void WatchVideo(bool result, string rewardId)
	{
		SLAnalytics.AddParameter("Location", s_currentLocation.ToString());
		SLAnalytics.AddParameter("VideoViewed", result.ToString());
		SLAnalytics.AddParameter("Reward", rewardId);
		XAxysProperties();
		SLAnalytics.LogEventWithParameters("WatchVideo");
	}

	public static void AdvertShown()
	{
		SLAnalytics.LogEvent("AdvertShown");
	}

	public static void CharacterRewarded(Characters.Type character)
	{
		string eventName = "CharacterRewarded_" + character;
		SLAnalytics.LogEvent(eventName);
	}

	public static void RateMeDialogShownFirstTime(RateMeButtons option)
	{
		SLAnalytics.AddParameter("Option", option.ToString());
		SLAnalytics.LogEventWithParameters("RateMeDialogShownFirstTime");
	}

	public static void RateMeDialogShown(RateMeButtons option, int times)
	{
		SLAnalytics.AddParameter("Option", option.ToString());
		SLAnalytics.AddParameter("TimesShown", times.ToString());
		SLAnalytics.LogEventWithParameters("RateMeDialogShown");
	}

	public static void AppUpdateDialogShown(AppUpdateChoice choice, int timesShown)
	{
		SLAnalytics.AddParameter("Choice", choice.ToString("g"));
		SLAnalytics.AddParameter("TimesShown", timesShown.ToString());
		SLAnalytics.LogEventWithParameters("AppUpdateDialogShown");
	}

	public static void GCButtonPressed(bool internet, GCState.Challenges challenge)
	{
		string value = DCTime.GetCurrentTime().ToString("yy.MM.dd");
		SLAnalytics.AddParameter("Date", value);
		SLAnalytics.AddParameter("Participated", GCState.IsChallengeParticipated(challenge).ToString());
		SLAnalytics.AddParameter("HadInternet", internet.ToString());
		SLAnalytics.LogEventWithParameters("GCButtonPressed");
	}

	public static void GCButtonTimesPreseed(int timesPressed, DateTime first, DateTime last, GCState.Challenges challenge)
	{
		SLAnalytics.AddParameter("TimesPressed", timesPressed.ToString());
		string value = first.ToString("yy.MM.dd");
		SLAnalytics.AddParameter("FirstTime", value);
		value = last.ToString("yy.MM.dd");
		SLAnalytics.AddParameter("LastTime", value);
		SLAnalytics.AddParameter("Participated", GCState.IsChallengeParticipated(challenge).ToString());
		SLAnalytics.LogEventWithParameters("GCButtonTimesPressed");
	}

	public static void TutorialProgress(int[] tutorialCounts)
	{
		SLAnalytics.AddParameter("Left", tutorialCounts[0].ToString());
		SLAnalytics.AddParameter("Jump", tutorialCounts[1].ToString());
		SLAnalytics.AddParameter("Roll", tutorialCounts[2].ToString());
		SLAnalytics.AddParameter("Attack", tutorialCounts[3].ToString());
		SLAnalytics.AddParameter("Chopper", tutorialCounts[4].ToString());
		SLAnalytics.AddParameter("Dash", tutorialCounts[6].ToString());
		SLAnalytics.DelayEventWithParameters("TutorialProgress");
	}

	public static void TutorialCompleted(int[] tutorialCounts)
	{
		SLAnalytics.AddParameter("Left", tutorialCounts[0].ToString());
		SLAnalytics.AddParameter("Jump", tutorialCounts[1].ToString());
		SLAnalytics.AddParameter("Roll", tutorialCounts[2].ToString());
		SLAnalytics.AddParameter("Attack", tutorialCounts[3].ToString());
		SLAnalytics.AddParameter("Chopper", tutorialCounts[4].ToString());
		SLAnalytics.AddParameter("Dash", tutorialCounts[6].ToString());
		SLAnalytics.LogEventWithParameters("TutorialCompleted");
	}

	public static void WOFDailyStatsLogged(DateTime today, int normalPrizesWonToday, int jackpotsWonToday, int paidSpinsTakenToday, int normalPrizesWonTotal, int jackpotsWonTotal, int paidSpinsTakenTotal, int freeSpinsTakenTotal, int freeSpinsMissed)
	{
		CultureInfo cultureInfo = new CultureInfo("en-US");
		SLAnalytics.AddParameter("TodaysDate", today.Date.ToString(cultureInfo));
		SLAnalytics.AddParameter("NormalPrizesWonToday", normalPrizesWonToday.ToString());
		SLAnalytics.AddParameter("JackpotsWonToday", jackpotsWonToday.ToString());
		SLAnalytics.AddParameter("PaidSpinsToday", paidSpinsTakenToday.ToString());
		SLAnalytics.AddParameter("NormalPrizesWonTotal", normalPrizesWonTotal.ToString());
		SLAnalytics.AddParameter("JackpotsWonTotal", jackpotsWonTotal.ToString());
		SLAnalytics.AddParameter("PaidSpinsTotal", paidSpinsTakenTotal.ToString());
		SLAnalytics.AddParameter("FreeSpinsTotal", freeSpinsTakenTotal.ToString());
		SLAnalytics.AddParameter("FreeSpinsMissed", freeSpinsMissed.ToString());
		SLAnalytics.LogEventWithParameters("WOFDailySpinStats");
	}

	public static void WOFRedStarRingBought(string lastPrize, string lastPrizeType, string currentJackpot, string currentJackpotType)
	{
		SLAnalytics.AddParameter("LastPrize", lastPrize);
		SLAnalytics.AddParameter("LastPrizeType", lastPrizeType);
		SLAnalytics.AddParameter("CurrentJackpot", currentJackpot);
		SLAnalytics.AddParameter("CurrentJackpotType", currentJackpotType);
		SLAnalytics.LogEventWithParameters("WOFRedStarRingBought");
	}

	public static void WOFFirstDecission(string action, int visits)
	{
		SLAnalytics.AddParameter("Action", action);
		SLAnalytics.AddParameter("HasFreeSpin", WheelOfFortuneSettings.Instance.HasFreeSpin.ToString());
		SLAnalytics.AddParameter("VisitsThisSession", visits.ToString());
		SLAnalytics.LogEventWithParameters("WOFFirstDecission");
	}

	public static void GC6ChallengePoints(int amount)
	{
		SLAnalytics.AddParameter("GC6Points", amount.ToString());
		SLAnalytics.LogEventWithParameters("GC2ChallengePoints");
	}

	public static void ABTestingCohortChange(string oldC, string newC)
	{
		SLAnalytics.AddParameter("OldCohort", oldC);
		SLAnalytics.AddParameter("NewCohort", newC);
		SLAnalytics.LogEventWithParameters("ABTestingCohortChange");
	}

	public static void LeaderboardFriends(int amount)
	{
		if (!s_friendsAnalyticSent)
		{
			s_friendsAnalyticSent = true;
			SLAnalytics.AddParameter("NumberOfFriendsOnLeaderboad", amount.ToString());
			SLAnalytics.LogEventWithParameters("FriendsOnLeaderboard");
		}
	}

	private void Start()
	{
		EventDispatch.RegisterInterest("OnGameFinished", this, EventDispatch.Priority.Lowest);
		EventDispatch.RegisterInterest("ResetGameState", this);
		EventDispatch.RegisterInterest("OnSonicFall", this);
		EventDispatch.RegisterInterest("OnGameDataLoaded", this, EventDispatch.Priority.Lowest);
		EventDispatch.RegisterInterest("OnGameDataSaveRequest", this);
		EventDispatch.RegisterInterest("OnFacebookAuthenticateComplete", this);
		EventDispatch.RegisterInterest("FeatureStateReady", this);
		s_stats = PlayerStats.GetCurrentStats();
	}

	private void Event_OnGameFinished()
	{
		EndRun();
		EndRunBoosters();
	}

	private void Event_ResetGameState(GameState.Mode mode)
	{
	}

	public void Event_OnSonicFall()
	{
		PlayerDeath("Fall");
	}

	private void Event_OnFacebookAuthenticateComplete(bool[] bools)
	{
		if ((bools[0] && !s_facebookReadAnalyticSent) || (bools[0] && bools[1] && !s_facebookWriteAnalyticSent))
		{
			SLAnalytics.AddParameter("Location", s_facebookLoginLocation.ToString());
			XAxysProperties();
			if (!bools[1])
			{
				SLAnalytics.LogEventWithParameters("FacebookLoginReadOnly");
				s_facebookReadAnalyticSent = true;
			}
			else
			{
				SLAnalytics.LogEventWithParameters("FacebookLoginWrite");
				s_facebookWriteAnalyticSent = true;
			}
		}
	}

	private static void XAxysProperties()
	{
		int num = m_xAxys.Length;
		for (int i = 0; i < num; i++)
		{
			PlayerStats.StatNames statNames = m_xAxys[i];
			SLAnalytics.AddParameter(statNames.ToString(), QuantizeValue(statNames, s_stats.m_trackedStats[(int)statNames]));
		}
		num = m_xAxysDistances.Length;
		for (int j = 0; j < num; j++)
		{
			PlayerStats.DistanceNames distanceNames = m_xAxysDistances[j];
			SLAnalytics.AddParameter(distanceNames.ToString(), QuantizeValue(distanceNames, s_stats.m_trackedDistances[(int)distanceNames]));
		}
	}

	private static string QuantizeScore(long val)
	{
		long num = (long)Math.Ceiling((double)val / 1000.0);
		if (num <= 1)
		{
			return "< 1000 Points";
		}
		if (num <= 10)
		{
			return string.Format("{0} - {1} Points", (num - 1) * 1000, num * 1000);
		}
		if (num <= 50)
		{
			long num2 = (long)Math.Ceiling((double)num / 5.0);
			return string.Format("{0} - {1} Points", (num2 - 1) * 5000, num2 * 5000);
		}
		if (num <= 250)
		{
			long num3 = (long)Math.Ceiling((double)num / 10.0);
			return string.Format("{0} - {1} Points", (num3 - 1) * 10000, num3 * 10000);
		}
		if (num <= 500)
		{
			long num4 = (long)Math.Ceiling((double)num / 50.0);
			return string.Format("{0} - {1} Points", (num4 - 1) * 50000, num4 * 50000);
		}
		if (num <= 1000)
		{
			long num5 = (long)Math.Ceiling((double)num / 100.0);
			return string.Format("{0} - {1} Points", (num5 - 1) * 100000, num5 * 100000);
		}
		if (num <= 5000)
		{
			long num6 = (long)Math.Ceiling((double)num / 250.0);
			return string.Format("{0} - {1} Points", (num6 - 1) * 250000, num6 * 250000);
		}
		long num7 = (long)Math.Ceiling((double)num / 500.0);
		return string.Format("{0} - {1} Points", (num7 - 1) * 500000, num7 * 500000);
	}

	private static string QuantizeValue(PlayerStats.StatNames valueName, int val)
	{
		switch (valueName)
		{
		case PlayerStats.StatNames.NumberOfRuns_Total:
		{
			double num21 = Math.Ceiling((double)val / 10.0);
			if (num21 <= 1.0)
			{
				return "< 10";
			}
			if (num21 <= 5.0)
			{
				return string.Format("{0} - {1}", (num21 - 1.0) * 10.0, num21 * 10.0);
			}
			if (num21 <= 20.0)
			{
				double num22 = Math.Ceiling(num21 / 2.5);
				return string.Format("{0} - {1}", (num22 - 1.0) * 25.0, num22 * 25.0);
			}
			if (num21 <= 100.0)
			{
				double num23 = Math.Ceiling(num21 / 5.0);
				return string.Format("{0} - {1}", (num23 - 1.0) * 50.0, num23 * 50.0);
			}
			if (num21 <= 200.0)
			{
				double num24 = Math.Ceiling(num21 / 50.0);
				return string.Format("{0} - {1}", (num24 - 1.0) * 500.0, num24 * 500.0);
			}
			return string.Format("> 2000");
		}
		case PlayerStats.StatNames.ShopPurchases_Total:
		{
			double num15 = Math.Ceiling((double)val / 5.0);
			if (num15 <= 1.0)
			{
				return "< 5";
			}
			if (num15 <= 10.0)
			{
				return string.Format("{0} - {1}", (num15 - 1.0) * 5.0, num15 * 5.0);
			}
			if (num15 <= 20.0)
			{
				double num16 = Math.Ceiling(num15 / 2.0);
				return string.Format("{0} - {1}", (num16 - 1.0) * 10.0, num16 * 10.0);
			}
			double num17 = Math.Ceiling(num15 / 10.0);
			return string.Format("{0} - {1}", (num17 - 1.0) * 50.0, num17 * 50.0);
		}
		case PlayerStats.StatNames.TimePlayed_Total:
		{
			int num9 = val / 600;
			double num10 = Math.Ceiling((double)num9 / 15.0);
			if (num10 <= 1.0)
			{
				return "< 0.25 Hours";
			}
			if (num10 <= 16.0)
			{
				return string.Format("{0} - {1} Hours", (num10 - 1.0) * 0.25, num10 * 0.25);
			}
			if (num10 <= 40.0)
			{
				double num11 = Math.Ceiling(num10 / 2.0);
				return string.Format("{0} - {1} Hours", (num11 - 1.0) * 0.5, num11 * 0.5);
			}
			if (num10 <= 80.0)
			{
				double num12 = Math.Ceiling(num10 / 4.0);
				return string.Format("{0} - {1} Hours", num12 - 1.0, num12);
			}
			if (num10 <= 240.0)
			{
				double num13 = Math.Ceiling(num10 / 20.0);
				return string.Format("{0} - {1} Hours", (num13 - 1.0) * 5.0, num13 * 5.0);
			}
			double num14 = Math.Ceiling(num10 / 40.0);
			return string.Format("{0} - {1} Hours", (num14 - 1.0) * 10.0, num14 * 10.0);
		}
		case PlayerStats.StatNames.TimePlayed_Run:
		{
			int num5 = val / 10;
			double num6 = Math.Ceiling((double)num5 / 30.0);
			if (num6 <= 1.0)
			{
				return "< 0.5 Minutes";
			}
			if (num6 <= 10.0)
			{
				return string.Format("{0} - {1} Minutes", (num6 - 1.0) * 0.5, num6 * 0.5);
			}
			if (num6 <= 20.0)
			{
				double num7 = Math.Ceiling(num6 / 2.0);
				return string.Format("{0} - {1} Minutes", num7 - 1.0, num7);
			}
			double num8 = Math.Ceiling(num6 / 10.0);
			return string.Format("{0} - {1} Minutes", (num8 - 1.0) * 5.0, num8 * 5.0);
		}
		case PlayerStats.StatNames.Enemies_Run:
		{
			double num3 = Math.Ceiling((double)val / 5.0);
			if (num3 <= 1.0)
			{
				return "< 5";
			}
			if (num3 <= 6.0)
			{
				return string.Format("{0} - {1}", (num3 - 1.0) * 5.0, num3 * 5.0);
			}
			double num4 = Math.Ceiling(num3 / 2.0);
			return string.Format("{0} - {1}", (num4 - 1.0) * 10.0, num4 * 10.0);
		}
		case PlayerStats.StatNames.RingsCollected_Run:
		case PlayerStats.StatNames.RingsBanked_Run:
		{
			double num18 = Math.Ceiling((double)val / 25.0);
			if (num18 <= 1.0)
			{
				return "< 25";
			}
			if (num18 <= 12.0)
			{
				return string.Format("{0} - {1}", (num18 - 1.0) * 25.0, num18 * 25.0);
			}
			if (num18 <= 20.0)
			{
				double num19 = Math.Ceiling(num18 / 2.0);
				return string.Format("{0} - {1}", (num19 - 1.0) * 50.0, num19 * 50.0);
			}
			double num20 = Math.Ceiling(num18 / 4.0);
			return string.Format("{0} - {1}", (num20 - 1.0) * 100.0, num20 * 100.0);
		}
		case PlayerStats.StatNames.DashUses_Run:
		{
			if (val <= 4)
			{
				return string.Format("{0}", val);
			}
			if (val <= 10)
			{
				double num = Math.Ceiling((double)val / 2.0);
				return string.Format("{0} - {1}", (num - 1.0) * 2.0, num * 2.0);
			}
			double num2 = Math.Ceiling((double)val / 5.0);
			return string.Format("{0} - {1}", (num2 - 1.0) * 5.0, num2 * 5.0);
		}
		default:
			return val.ToString();
		}
	}

	private static string QuantizeValue(PlayerStats.DistanceNames valueName, float val)
	{
		switch (valueName)
		{
		case PlayerStats.DistanceNames.DistanceRun_Run:
		{
			double num6 = Math.Ceiling((double)val / 500.0);
			if (num6 <= 1.0)
			{
				return "< 500 m";
			}
			if (num6 <= 20.0)
			{
				return string.Format("{0} - {1} m", (num6 - 1.0) * 500.0, num6 * 500.0);
			}
			double num7 = Math.Ceiling(num6 / 2.0);
			return string.Format("{0} - {1} Km", (num7 - 1.0) * 1.0, num7 * 1.0);
		}
		case PlayerStats.DistanceNames.DistanceRun_Total:
		{
			double num = Math.Ceiling((double)val / 10000.0);
			if (num <= 1.0)
			{
				return "< 10 Km";
			}
			if (num <= 5.0)
			{
				return string.Format("{0} - {1} Km", (num - 1.0) * 10.0, num * 10.0);
			}
			if (num <= 10.0)
			{
				double num2 = Math.Ceiling(num / 2.5);
				return string.Format("{0} - {1} Km", (num2 - 1.0) * 25.0, num2 * 25.0);
			}
			if (num <= 100.0)
			{
				double num3 = Math.Ceiling(num / 10.0);
				return string.Format("{0} - {1} Km", (num3 - 1.0) * 100.0, num3 * 100.0);
			}
			if (num <= 2000.0)
			{
				double num4 = Math.Ceiling(num / 25.0);
				return string.Format("{0} - {1} Km", (num4 - 1.0) * 250.0, num4 * 250.0);
			}
			double num5 = Math.Ceiling(num / 50.0);
			return string.Format("{0} - {1} Km", (num5 - 1.0) * 500.0, num5 * 500.0);
		}
		default:
			return ((int)val).ToString();
		}
	}

	private static string QuantizeValue(FileDownloader.Files fileName, float val)
	{
		if (val <= 1f)
		{
			return "< 1s";
		}
		return string.Format("{0} - {1}s", val - 1f, val);
	}

	private static string QuantizeValue(LoadingScreenProperties.ScreenType screenName, float val)
	{
		if (val <= 1f)
		{
			return "< 1s";
		}
		if (val <= 5f)
		{
			return string.Format("{0} - {1}s", val - 1f, val);
		}
		if (val <= 15f)
		{
			float num = Mathf.Ceil(val / 5f);
			return string.Format("{0} - {1}s", (num - 1f) * 5f, num * 5f);
		}
		if (val <= 30f)
		{
			float num2 = Mathf.Ceil(val / 15f);
			return string.Format("{0} - {1}s", (num2 - 1f) * 15f, num2 * 15f);
		}
		if (val <= 300f)
		{
			float num3 = Mathf.Ceil(val / 30f);
			return string.Format("{0} - {1}m", (num3 - 1f) * 0.5f, num3 * 0.5f);
		}
		float num4 = Mathf.Ceil(val / 300f);
		return string.Format("{0} - {1}m", (num4 - 1f) * 5f, num4 * 5f);
	}

	private static string GetCurrentTemplate()
	{
		TrackSegment trackSegment = ((!(Sonic.Tracker.CurrentSpline == null)) ? TrackSegment.GetSegmentOfSpline(Sonic.Tracker.CurrentSpline) : null);
		if (trackSegment == null)
		{
			return "NULL";
		}
		string text = string.Join(",", trackSegment.TemplateContainers.Select((Transform container) => container.gameObject.name).ToArray());
		string text2 = ((!trackSegment.TemplateContainers.Any()) ? "(none)" : text);
		return text2.ToLower().Replace("template", string.Empty);
	}

	private void Event_OnGameDataSaveRequest()
	{
		PropertyStore.Store("FacebookReadAnalyticSent", s_facebookReadAnalyticSent);
		PropertyStore.Store("FacebookWriteAnalyticSent", s_facebookWriteAnalyticSent);
	}

	private void Event_OnGameDataLoaded(ActiveProperties activeProperties)
	{
		s_facebookReadAnalyticSent = activeProperties.GetBool("FacebookReadAnalyticSent");
		s_facebookWriteAnalyticSent = activeProperties.GetBool("FacebookWriteAnalyticSent");
		if (m_skipLoad)
		{
			m_skipLoad = false;
		}
		else
		{
			SessionInfo();
		}
	}

	private void Event_FeatureStateReady()
	{
		LSON.Property stateProperty = FeatureState.GetStateProperty("State", "hlmetrics");
		if (stateProperty != null)
		{
			bool boolValue = false;
			if (LSONProperties.AsBool(stateProperty, out boolValue))
			{
				SLAnalytics.EnableMetrics(boolValue);
			}
		}
	}
}
