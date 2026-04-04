using System;
using System.Collections;
using System.Globalization;
using UnityEngine;

public class CrossPromotion : MonoBehaviour
{
	private const int MIN_CROSSPROM_VERSION = 7;

	private const string EventRoot = "crosspromote";

	private const string cPProperty = "crosspromotionevent";

	private const string cPIdProperty = "eventid";

	private const string cPGameNameProperty = "gamename";

	private const string cPGameUrlSchemeProperty = "gameurlscheme";

	private const string cPGameUrlScheme2Property = "gameurlscheme2";

	private const string cPGameIDProperty = "gameid";

	private const string cPGameServerAddrProperty = "serveraddr";

	private const string cPGameIconProperty = "gameicon";

	private const string cPTimeCheckProperty = "timecheck";

	private const string lastCheckedStoreKey = "cPLastChecked";

	private const string lastPlayedStoreKey = "cPLastPlayed";

	private const string serverTimeCheckurl = "http://220.100.128.206:8080/Dash/Query.hl?id=";

	private DateTime cPLastChecked;

	private bool InternetConnection;

	public GameObject content;

	public GameObject m_downloadLabel;

	public GameObject m_playLabel;

	private UILabel m_gameNameLabel;

	private UITexture m_gameIcon;

	private CultureInfo culture = new CultureInfo("en-US");

	public string cPId { get; private set; }

	public string cPGameName { get; private set; }

	public string cPGameUrlScheme { get; private set; }

	public string cPGameUrlScheme2 { get; private set; }

	public string cPGameID { get; private set; }

	public string cPGameServerAddr { get; private set; }

	public string cPGameIconUrl { get; private set; }

	public int cPTimeLimitNotPlayedSecs { get; private set; }

	public DateTime cPTimeLastPlayed { get; private set; }

	public bool cPGameInstalled { get; private set; }

	public bool cPActive { get; private set; }

	public bool cPPlayed { get; private set; }

	private void Awake()
	{
		cPActive = false;
		updateButtonVisibility();
	}

	private void Update()
	{
		if (InternetConnection != (Application.internetReachability != NetworkReachability.NotReachable))
		{
			InternetConnection = Application.internetReachability != NetworkReachability.NotReachable;
			UpdateTapOut();
		}
	}

	private void updateButtonVisibility()
	{
		cPPlayed = cPTimeLastPlayed.AddSeconds(cPTimeLimitNotPlayedSecs) > DateTime.Now;
		InternetConnection = Application.internetReachability != NetworkReachability.NotReachable;
		bool state = false;
		if (cPActive && !cPPlayed && InternetConnection && checkIOSVersion())
		{
			state = true;
		}
		NGUITools.SetActive(m_downloadLabel, !cPGameInstalled);
		NGUITools.SetActive(m_playLabel, cPGameInstalled);
		NGUITools.SetActiveSelf(content, state);
	}

	private bool checkIOSVersion()
	{
		string operatingSystem = SystemInfo.operatingSystem;
		int i = 0;
		int result = -1;
		int result2;
		for (result2 = result; i < operatingSystem.Length && !int.TryParse(operatingSystem.Substring(i, 1), out result2); i++)
		{
		}
		string text = result2.ToString();
		for (i++; i < operatingSystem.Length && !operatingSystem.Substring(i, 1).Equals("."); i++)
		{
			text += operatingSystem.Substring(i, 1);
		}
		int.TryParse(text, out result);
		return result >= 7;
	}

	private void Event_MainMenuActive()
	{
		InternetConnection = Application.internetReachability != NetworkReachability.NotReachable;
		UpdateTapOut();
	}

	private void Start()
	{
		m_gameNameLabel = FindChildByName("Game Name (label)", content).GetComponent<UILabel>();
		m_gameIcon = FindChildByName("button_CrossPromotion_top", content).GetComponent<UITexture>();
	}

	private GameObject FindChildByName(string strName, GameObject objRoot)
	{
		if (objRoot.name == strName)
		{
			return objRoot;
		}
		Transform transform = objRoot.transform;
		int childCount = transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			GameObject gameObject = FindChildByName(strName, transform.GetChild(i).gameObject);
			if (gameObject != null)
			{
				return gameObject;
			}
		}
		return null;
	}

	private void Event_FeatureStateReady()
	{
		if (!FeatureState.Valid)
		{
			return;
		}
		cPActive = false;
		LSON.Property stateProperty = FeatureState.GetStateProperty("crosspromote", "crosspromotionevent");
		stateProperty = FeatureState.GetStateProperty("crosspromote", "eventid");
		if (stateProperty != null)
		{
			string stringValue = "0";
			if (LSONProperties.AsString(stateProperty, out stringValue))
			{
				if (cPId != stringValue)
				{
				}
				cPId = stringValue;
				cPActive = true;
			}
		}
		stateProperty = FeatureState.GetStateProperty("crosspromote", "gamename");
		if (stateProperty != null)
		{
			string stringValue2 = string.Empty;
			if (LSONProperties.AsString(stateProperty, out stringValue2))
			{
				cPGameName = stringValue2.Replace("\"", string.Empty).Trim();
				m_gameNameLabel.text = cPGameName;
			}
		}
		else
		{
			cPActive = false;
		}
		stateProperty = FeatureState.GetStateProperty("crosspromote", "gameid");
		if (stateProperty != null)
		{
			string stringValue3 = string.Empty;
			if (LSONProperties.AsString(stateProperty, out stringValue3))
			{
				cPGameID = stringValue3;
			}
		}
		else
		{
			cPActive = false;
		}
		stateProperty = FeatureState.GetStateProperty("crosspromote", "serveraddr");
		if (stateProperty != null)
		{
			string stringValue4 = string.Empty;
			if (LSONProperties.AsString(stateProperty, out stringValue4))
			{
				cPGameServerAddr = stringValue4.Replace("^", ":").Trim();
			}
		}
		else
		{
			cPActive = false;
		}
		stateProperty = FeatureState.GetStateProperty("crosspromote", "gameurlscheme");
		if (stateProperty != null)
		{
			string stringValue5 = string.Empty;
			if (LSONProperties.AsString(stateProperty, out stringValue5))
			{
				cPGameUrlScheme = stringValue5.Replace("^", ":").Trim() + "://";
			}
		}
		else
		{
			cPActive = false;
		}
		stateProperty = FeatureState.GetStateProperty("crosspromote", "gameurlscheme2");
		if (stateProperty != null)
		{
			string stringValue6 = string.Empty;
			if (LSONProperties.AsString(stateProperty, out stringValue6))
			{
				cPGameUrlScheme2 = stringValue6.Replace("^", ":").Trim() + "://";
			}
		}
		stateProperty = FeatureState.GetStateProperty("crosspromote", "gameicon");
		if (stateProperty != null)
		{
			string stringValue7 = string.Empty;
			if (LSONProperties.AsString(stateProperty, out stringValue7))
			{
				cPGameIconUrl = stringValue7.Replace("^", ":").Trim();
				StartCoroutine(setButtonGameIcon(cPGameIconUrl));
			}
		}
		else
		{
			cPActive = false;
		}
		stateProperty = FeatureState.GetStateProperty("crosspromote", "timecheck");
		if (stateProperty != null)
		{
			int intValue = 0;
			if (LSONProperties.AsInt(stateProperty, out intValue))
			{
				cPTimeLimitNotPlayedSecs = intValue;
			}
		}
		else
		{
			cPActive = false;
		}
		UpdateTapOut();
	}

	private void UpdateTapOut()
	{
	}

	public DateTime FromUnixTime(long unixTime)
	{
		return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTime);
	}

	private void Event_OnGameDataLoaded(ActiveProperties activeProperties)
	{
		if (!DateTime.TryParse(activeProperties.GetString("cPLastChecked"), culture, DateTimeStyles.None, out cPLastChecked))
		{
			cPLastChecked = DateTime.Parse("1970/01/01 00:00:00");
		}
		DateTime result;
		if (!DateTime.TryParse(activeProperties.GetString("cPLastPlayed"), culture, DateTimeStyles.None, out result))
		{
			cPTimeLastPlayed = DateTime.Parse("1970/01/01 00:00:00");
		}
		cPTimeLastPlayed = result;
	}

	private void Event_OnGameDataSaveRequest()
	{
		if (cPLastChecked.Ticks > 0)
		{
			PropertyStore.Store("cPLastChecked", cPLastChecked.ToString(culture.DateTimeFormat));
		}
		PropertyStore.Store("cPLastPlayed", cPTimeLastPlayed.ToString(culture.DateTimeFormat));
	}

	private IEnumerator setSecondsSinceLastPlay(string queryURL)
	{
		WWW www = new WWW(queryURL);
		yield return www;
		try
		{
			DateTime serverTimeLastPlayed = FromUnixTime(long.Parse(www.text) / 1000);
			if (serverTimeLastPlayed > cPTimeLastPlayed)
			{
				cPTimeLastPlayed = serverTimeLastPlayed;
			}
			Debug.Log("Cross Promotion cPTimeLastPlayed " + cPTimeLastPlayed.ToString("yyyy/MM/dd hh:mm:ss"));
			updateButtonVisibility();
		}
		catch (Exception ex)
		{
			Exception e = ex;
			string error = e.Message;
			Debug.Log("Cross Promotion cPTimeLastPlayed " + error);
			cPTimeLastPlayed = DateTime.Parse("1970/01/01 00:00:00");
		}
	}

	private IEnumerator setButtonGameIcon(string cPGameIconUrl)
	{
		WWW www = new WWW(cPGameIconUrl);
		yield return www;
		try
		{
			if (www.size > 0)
			{
				m_gameIcon.mainTexture = www.texture;
				yield break;
			}
			cPActive = false;
			updateButtonVisibility();
		}
		catch (Exception)
		{
			cPActive = false;
		}
	}

	public void launchStore()
	{
		Application.OpenURL("http://play.google.com/store/apps/details?id=com.sega.sonicdash");
	}
}
