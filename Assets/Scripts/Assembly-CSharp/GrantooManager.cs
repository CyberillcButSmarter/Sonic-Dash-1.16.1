using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrantooManager : MonoBehaviour
{
	private static GrantooManager s_Instance;

	private bool checkGrantooRewards = true;

	private GrantooListener m_Listener;

	private GrantooState m_eState;

	private string m_strError;

	private int m_iNotificationCount;

	private DateTime m_LastChallengeCountSync;

	private TimeSpan m_SyncInterval = new TimeSpan(0, 5, 0);

	private string m_strCurrentMatchID = string.Empty;

	private string m_strCurrentTournamentID = string.Empty;

	private string m_strCurrentParamsJSON = string.Empty;

	private int m_iCurrentSeed;

	private bool m_bCurrentAdsAllowed = true;

	private void Start()
	{
		s_Instance = this;
		m_Listener = new GrantooListener();
		if (PropellerSDK.AllowEnablePropeller)
		{
			RefreshNotificationCount();
			SLAnalytics.LogEvent("GrantooInitialise");
			EventDispatch.RegisterInterest("OnFacebookAuthenticateComplete", this);
			EventDispatch.RegisterInterest("OnGrantooFacebookAuthenticateComplete", this);
			EventDispatch.RegisterInterest("MainMenuActive", this);
		}
	}

	private void Update()
	{
		if (PropellerSDK.AllowEnablePropeller && DateTime.Now > m_LastChallengeCountSync + m_SyncInterval)
		{
			m_LastChallengeCountSync = DateTime.Now;
			PropellerSDK.SyncChallengeCounts();
		}
	}

	public static GrantooManager GetInstance()
	{
		return s_Instance;
	}

	public void Launch()
	{
		if (PropellerSDK.AllowEnablePropeller)
		{
			m_eState = GrantooState.InGrantooUI;
			SLAnalytics.LogEvent("GrantooLaunch");
			PropellerSDK.Launch(m_Listener);
		}
	}

	public int GetNotificationCount()
	{
		return m_iNotificationCount;
	}

	public void RefreshNotificationCount()
	{
		PropellerSDK.SyncChallengeCounts();
		m_LastChallengeCountSync = DateTime.Now;
	}

	public void OnPropellerSDKChallengeCountUpdated(string count)
	{
		int.TryParse(count, out m_iNotificationCount);
	}

	public void AbandonMatch()
	{
		m_eState = GrantooState.Inactive;
	}

	public void BeginMatch()
	{
		if (m_eState == GrantooState.RequestStartGame)
		{
			m_eState = GrantooState.ChallengeActive;
		}
	}

	public bool ShouldMatchBegin()
	{
		return m_eState == GrantooState.RequestStartGame;
	}

	public bool IsActive()
	{
		return m_eState == GrantooState.ChallengeActive || m_eState == GrantooState.TournamentActive;
	}

	public bool IsInGrantooUI()
	{
		return m_eState == GrantooState.InGrantooUI;
	}

	public int GetCurrentSeed()
	{
		return m_iCurrentSeed;
	}

	public void OnRoundCompleted()
	{
		float distanceTravelled = Sonic.Tracker.DistanceTravelled;
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("matchID", m_strCurrentMatchID);
		dictionary.Add("tournamentID", m_strCurrentTournamentID);
		dictionary.Add("score", ((int)distanceTravelled).ToString());
		m_eState = GrantooState.InGrantooUI;
		SLAnalytics.AddParameter("Match", m_strCurrentMatchID);
		SLAnalytics.AddParameter("Tournament", m_strCurrentTournamentID);
		SLAnalytics.LogEventWithParameters("GrantooRoundCompleted");
		PropellerSDK.LaunchWithMatchResult(dictionary, m_Listener);
	}

	public void OnSDKCompletedWithMatch(Dictionary<string, string> matchInfo)
	{
		bool flag = true;
		flag &= matchInfo.TryGetValue("matchID", out m_strCurrentMatchID);
		flag &= matchInfo.TryGetValue("tournamentID", out m_strCurrentTournamentID);
		flag &= matchInfo.TryGetValue("paramsJSON", out m_strCurrentParamsJSON);
		if (flag)
		{
			JsonParser jsonParser = new JsonParser(m_strCurrentParamsJSON);
			Dictionary<string, object> dictionary = jsonParser.Parse();
			object value = null;
			object value2 = null;
			flag &= dictionary.TryGetValue("seed", out value);
			flag &= dictionary.TryGetValue("adsAllowed", out value2);
			if (flag)
			{
				long result = 0L;
				flag |= long.TryParse(value.ToString(), out result);
				m_iCurrentSeed = (int)(result % 1000000);
				flag |= bool.TryParse(value2.ToString(), out m_bCurrentAdsAllowed);
			}
		}
		if (!flag)
		{
			m_eState = GrantooState.Error;
			m_strError = "invalid matchInfo contents!";
		}
		else
		{
			GetInstance().RefreshNotificationCount();
			m_eState = GrantooState.RequestStartGame;
			m_strError = null;
		}
		SLAnalytics.AddParameter("Match", m_strCurrentMatchID);
		SLAnalytics.AddParameter("Tournament", m_strCurrentTournamentID);
		SLAnalytics.LogEventWithParameters("GrantooCompletedWithMatch");
		Debug.Log("Grantoo UI exited with match!");
	}

	public void OnSDKCompletedWithExit()
	{
		SLAnalytics.LogEvent("GrantooExit");
		GetInstance().RefreshNotificationCount();
		PropellerSDK.SyncVirtualGoods();
		m_eState = GrantooState.Inactive;
		m_strError = null;
		Debug.Log("Grantoo UI exited without starting a game");
	}

	public void OnSDKCompletedWithError(string strReason)
	{
		SLAnalytics.AddParameter("Error", strReason);
		SLAnalytics.LogEventWithParameters("GrantooCompletedWithError");
		GetInstance().RefreshNotificationCount();
		m_eState = GrantooState.Error;
		m_strError = strReason;
		Debug.Log("Grantoo UI exited with error: " + strReason);
	}

	public GrantooState GetState()
	{
		return m_eState;
	}

	public string GetError()
	{
		return m_strError;
	}

	public void OnSDKSocialLogin(bool bAllowCachedLogin)
	{
		if (bAllowCachedLogin)
		{
			Debug.Log("OnSDKSocialLogin - bAllowCachedLogin=true");
			if (Social.localUser.authenticated)
			{
				Event_OnGrantooFacebookAuthenticateComplete(new bool[1] { true });
				return;
			}
		}
		Debug.Log("OnSDKSocialLogin - bAllowCachedLogin=false");
		GameObject gameObject = GameObject.Find("Community");
		if (gameObject != null)
		{
			Debug.Log("OnSDKSocialLogin - Community obj found");
		}
		Debug.Log("OnSDKSocialLogin - Calling Trigger_FBLogin");
		gameObject.SendMessage("Trigger_FBLogin", base.gameObject, SendMessageOptions.DontRequireReceiver);
	}

	private void Event_OnFacebookAuthenticateComplete(bool[] bools)
	{
		Event_OnGrantooFacebookAuthenticateComplete(bools);
	}

	private void Event_OnGrantooFacebookAuthenticateComplete(bool[] bools)
	{
		Debug.Log("GRANTOO MANAGER: RECEIVING EVENT OnFacebookAuthenticateComplete");
		if (!bools[0])
		{
			Debug.Log("Event_OnGrantooFacebookAuthenticateComplete - Login failed");
			PropellerSDK.SdkSocialLoginCompleted(null);
			return;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		string facebookUserName = ((HLLocalUser)Social.localUser).FacebookUserName;
		string facebookUserID = ((HLLocalUser)Social.localUser).FacebookUserID;
		string facebookAccessToken = ((HLLocalUser)Social.localUser).FacebookAccessToken;
		string facebookMailAddress = ((HLLocalUser)Social.localUser).FacebookMailAddress;
		Debug.Log("GRANTOO FACEBOOK USERNAME: " + facebookUserName);
		Debug.Log("GRANTOO FACEBOOK USERID:   " + facebookUserID);
		Debug.Log("GRANTOO FACEBOOK TOKEN:    " + facebookAccessToken);
		Debug.Log("GRANTOO FACEBOOK MAIL:     " + facebookMailAddress);
		dictionary.Add("provider", "facebook");
		dictionary.Add("email", facebookMailAddress);
		dictionary.Add("name", facebookUserName);
		dictionary.Add("id", facebookUserID);
		dictionary.Add("token", facebookAccessToken);
		PropellerSDK.SdkSocialLoginCompleted(dictionary);
	}

	public void OnPropellerSDKVirtualGoodList(Dictionary<string, object> virtualGoodInfo)
	{
		string transactionId = (string)virtualGoodInfo["transactionID"];
		List<string> virtualGoods = (List<string>)virtualGoodInfo["virtualGoods"];
		StartCoroutine(GrantRewards(transactionId, virtualGoods));
	}

	private IEnumerator GrantRewards(string transactionId, List<string> virtualGoods)
	{
		yield return new WaitForSeconds(0.1f);
		foreach (string good in virtualGoods)
		{
			Debug.Log("Virtual good : " + good + "\n");
			switch (good)
			{
			case "rings_50":
			{
				string entry8 = "Single Ring Reward";
				int quantity8 = 50;
				StorePurchases.RequestReward(entry8, quantity8, 21, StorePurchases.ShowDialog.Yes);
				break;
			}
			case "rings_100":
			{
				string entry7 = "Single Ring Reward";
				int quantity7 = 100;
				StorePurchases.RequestReward(entry7, quantity7, 21, StorePurchases.ShowDialog.Yes);
				break;
			}
			case "rings_200":
			{
				string entry6 = "Single Ring Reward";
				int quantity6 = 200;
				StorePurchases.RequestReward(entry6, quantity6, 21, StorePurchases.ShowDialog.Yes);
				break;
			}
			case "rings_red_star_10":
			{
				string entry5 = "Single Star Reward";
				int quantity5 = 10;
				StorePurchases.RequestReward(entry5, quantity5, 21, StorePurchases.ShowDialog.Yes);
				break;
			}
			case "rings_red_star_30":
			{
				string entry4 = "Single Star Reward";
				int quantity4 = 30;
				StorePurchases.RequestReward(entry4, quantity4, 21, StorePurchases.ShowDialog.Yes);
				break;
			}
			case "rings_red_star_50":
			{
				string entry3 = "Single Star Reward";
				int quantity3 = 50;
				StorePurchases.RequestReward(entry3, quantity3, 21, StorePurchases.ShowDialog.Yes);
				break;
			}
			case "rings_red_star_75":
			{
				string entry2 = "Single Star Reward";
				int quantity2 = 75;
				StorePurchases.RequestReward(entry2, quantity2, 21, StorePurchases.ShowDialog.Yes);
				break;
			}
			case "rings_red_star_100":
			{
				string entry = "Single Star Reward";
				int quantity = 100;
				StorePurchases.RequestReward(entry, quantity, 21, StorePurchases.ShowDialog.Yes);
				break;
			}
			default:
				DialogContent_GeneralInfo.Display(DialogContent_GeneralInfo.Type.NewGameUpdate);
				break;
			}
		}
		PropellerSDK.AcknowledgeVirtualGoods(transactionId, true);
	}

	public bool DeviceLocked()
	{
		return false;
	}

	public void Event_MainMenuActive()
	{
		GetInstance().RefreshNotificationCount();
		if (checkGrantooRewards)
		{
			checkGrantooRewards = false;
			PropellerSDK.SyncVirtualGoods();
		}
	}

	public static bool IsGrantooAvailable()
	{
		if (!PropellerSDK.AllowEnablePropeller)
		{
			return false;
		}
		bool flag = Application.internetReachability != NetworkReachability.NotReachable && ABTesting.GetTestValueAsInt(ABTesting.Tests.GRANTOO_Enable) != 0;
		Language.ID language = Language.GetLanguage();
		flag = flag && (language == Language.ID.English_UK || language == Language.ID.English_US) && !GetInstance().DeviceLocked() && GrantooSpecialEvent.GetInstance().IsEventActive();
		bool flag2 = TutorialSystem.instance().isTrackTutorialEnabled();
		int testValueAsInt = ABTesting.GetTestValueAsInt(ABTesting.Tests.BOOSTERS_RunsBeforeUse);
		bool flag3 = PlayerStats.GetStat(PlayerStats.StatNames.NumberOfSessions_Total) < 2 && PlayerStats.GetStat(PlayerStats.StatNames.NumberOfRuns_Total) < testValueAsInt;
		int runsNeeded = GrantooSpecialEvent.GetInstance().GetRunsNeeded();
		bool flag4 = PlayerStats.GetStat(PlayerStats.StatNames.NumberOfRuns_Total) >= runsNeeded;
		return flag && !flag2 && !flag3 && flag4;
	}
}
