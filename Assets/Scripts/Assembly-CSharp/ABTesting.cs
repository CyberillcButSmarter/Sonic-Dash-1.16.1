using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABTesting : MonoBehaviour
{
	public enum Tests
	{
		RSR_MissionSetAmount = 0,
		RSR_Brag = 1,
		RSR_Return = 2,
		RSR_Leader = 3,
		RSR_Highscore = 4,
		RSR_Facebook = 5,
		RSR_RunsBefore1 = 6,
		RSR_Spread1 = 7,
		RSR_RunsBeforeNext = 8,
		RSR_SpreadNext = 9,
		RSR_MaxDaily = 10,
		HINT_ImportantInterval = 11,
		HINT_SkipHints = 12,
		MSG_MissionBenefits = 13,
		MSG_PreRunDialog = 14,
		ADS_Revive = 15,
		PLAY_MagnetDash = 16,
		PLAY_ReviveTimeNormal = 17,
		PLAY_ReviveTimeAds = 18,
		STORE_MaximumBundleOffers = 19,
		REVIVE_SimpleStore = 20,
		REVIVE_RSRRevive = 21,
		REVIVE_TimerEnabled = 22,
		BOOSTERS_AutoFill = 23,
		BOOSTERS_RunsBeforeUse = 24,
		HINT_OnlyStore = 25,
		PUSH_CustomSound = 26,
		PUSH_RunsBeforeAskMainMenu = 27,
		PUSH_AskOnPersonalBest = 28,
		GRANTOO_Enable = 29
	}

	private const string PopertyActualCohort = "ActualCohort";

	private const string ABGroupName = "ab";

	private static ABTesting instance;

	private LSON.Root[] m_abRoots;

	private LSON.Root[] m_defaultRoots;

	private int m_actualCohort = -1;

	private float[] m_defaults;

	public static bool Ready { get; private set; }

	public static bool URLReady { get; set; }

	public static int Cohort
	{
		get
		{
			return instance.m_actualCohort;
		}
	}

	public static int GetTestValueAsInt(Tests test)
	{
		float testValueAsFloat = GetTestValueAsFloat(test);
		return (int)testValueAsFloat;
	}

	public static float GetTestValueAsFloat(Tests test)
	{
		float testValue = instance.m_defaults[(int)test];
		if (!instance.FindValueInRoot(instance.m_abRoots, test, ref testValue))
		{
			instance.FindValueInRoot(instance.m_defaultRoots, test, ref testValue);
		}
		return testValue;
	}

	public static void Restart()
	{
		Ready = false;
		instance.SetDefaults();
		instance.StartCoroutine(instance.DownloadConfigFile());
		instance.StartCoroutine(instance.DownloadServerFile());
	}

	private void Start()
	{
		EventDispatch.RegisterInterest("OnGameDataLoaded", this);
		EventDispatch.RegisterInterest("OnGameDataSaveRequest", this);
		instance = this;
	}

	private bool FindValueInRoot(LSON.Root[] rootList, Tests test, ref float testValue)
	{
		if (rootList == null)
		{
			return false;
		}
		LSON.Property[] properties = LSONProperties.GetProperties(rootList, "ab");
		if (properties == null || properties.Length == 0)
		{
			return false;
		}
		string propertyName = test.ToString();
		LSON.Property property = LSONProperties.GetProperty(properties, propertyName);
		if (property == null)
		{
			return false;
		}
		return float.TryParse(property.m_value, out testValue);
	}

	private IEnumerator DownloadConfigFile()
	{
		FileDownloader fDownloader = new FileDownloader(FileDownloader.Files.ABConfig, true);
		yield return fDownloader.Loading;
		if (fDownloader.Error == null)
		{
			JsonParser parser = new JsonParser(fDownloader.Text);
			Dictionary<string, object> jsonObject = parser.Parse();
			int status = -1;
			object valStatus = null;
			if (jsonObject.TryGetValue("status", out valStatus) && (int)((double)valStatus + 0.1) == 0)
			{
				object param = null;
				if (jsonObject.TryGetValue("storeconfig", out param))
				{
					string storeconfig = (string)param;
					if (storeconfig != null)
					{
						FileDownloader.TweakABTestingURLs(FileDownloader.Files.StoreModifierAB, storeconfig);
					}
				}
				if (jsonObject.TryGetValue("abconfig", out param))
				{
					string abconfig = (string)param;
					if (abconfig != null)
					{
						FileDownloader.TweakABTestingURLs(FileDownloader.Files.ABTestingAB, abconfig);
					}
				}
				if (jsonObject.TryGetValue("stateconfig", out param))
				{
					string stateConfig = (string)param;
					if (stateConfig != null)
					{
						FileDownloader.TweakABTestingURLs(FileDownloader.Files.FeatureStateAB, stateConfig);
					}
				}
				if (jsonObject.TryGetValue("cohort", out param))
				{
					int cohort = (int)((double)param + 0.1);
					if (m_actualCohort != cohort)
					{
						GameAnalytics.ABTestingCohortChange(m_actualCohort.ToString(), cohort.ToString());
						m_actualCohort = cohort;
					}
				}
			}
		}
		URLReady = true;
	}

	private IEnumerator DownloadServerFile()
	{
		while (!URLReady)
		{
			yield return null;
		}
		FileDownloader abFile = new FileDownloader(FileDownloader.Files.ABTestingAB, true);
		yield return abFile.Loading;
		FileDownloader defaultFile = new FileDownloader(FileDownloader.Files.ABTestingDefault, true);
		yield return defaultFile.Loading;
		if (string.IsNullOrEmpty(abFile.Error) || string.IsNullOrEmpty(defaultFile.Error))
		{
			if (string.IsNullOrEmpty(abFile.Error))
			{
				m_abRoots = LSONReader.Parse(abFile.Text);
			}
			if (string.IsNullOrEmpty(defaultFile.Error))
			{
				m_defaultRoots = LSONReader.Parse(defaultFile.Text);
			}
			if (m_abRoots != null || m_defaultRoots != null)
			{
				EventDispatch.GenerateEvent("ABTestingReady");
			}
		}
		SetPushSoundTestValue();
		Ready = true;
	}

	private void SetDefaults()
	{
		m_defaults = new float[Utils.GetEnumCount<Tests>()];
		m_defaults[1] = 1f;
		m_defaults[2] = 1f;
		m_defaults[3] = 1f;
		m_defaults[4] = 1f;
		m_defaults[5] = 1f;
		m_defaults[12] = 0f;
		m_defaults[13] = 1f;
		m_defaults[14] = 1f;
		m_defaults[6] = -1f;
		m_defaults[7] = -1f;
		m_defaults[8] = -1f;
		m_defaults[9] = -1f;
		m_defaults[11] = -1f;
		m_defaults[19] = 1f;
		m_defaults[10] = -1f;
		m_defaults[0] = -1f;
		m_defaults[17] = -1f;
		m_defaults[18] = -1f;
		m_defaults[23] = 0f;
		m_defaults[24] = 1f;
		m_defaults[25] = 0f;
		m_defaults[26] = 0f;
		m_defaults[27] = 0f;
		m_defaults[28] = 0f;
		m_defaults[29] = 1f;
		m_defaults[22] = 1f;
	}

	private void Event_OnGameDataSaveRequest()
	{
		PropertyStore.Store("ActualCohort", m_actualCohort);
	}

	private void Event_OnGameDataLoaded(ActiveProperties activeProperties)
	{
		if (activeProperties.DoesPropertyExist("ActualCohort"))
		{
			m_actualCohort = activeProperties.GetInt("ActualCohort");
		}
	}

	private void SetPushSoundTestValue()
	{
		bool flag = GetTestValueAsInt(Tests.PUSH_CustomSound) > 0;
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.hardlightstudio.dev.sonicdash.plugin.SLGlobal");
		androidJavaClass.CallStatic("SetPushSoundTestValue", flag);
	}
}
