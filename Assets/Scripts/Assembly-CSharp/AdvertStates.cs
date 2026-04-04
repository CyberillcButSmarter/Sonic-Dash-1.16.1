using UnityEngine;

public class AdvertStates : MonoBehaviour
{
	private const string StateRoot = "adstate";

	private const string AEnableProperty = "enable";

	private const string MMRateProperty = "mm_rate";

	private const string ResultRateProperty = "result_rate";

	private const string AdsOnFirstInteractionProperty = "ads_on_first_interaction";

	private const string RunsBeforeAdsProperty = "runs_before_ads";

	private const int DefaultLimit = 60;

	private const int DefaultMMRate = 2;

	private const int DefaultResult = 3;

	private const bool DefaultAdsFirstInteraction = true;

	private const int DefaultRunsBeforeAds = 0;

	private static bool s_adsEnabled = true;

	private static int s_adsMainMenuRate;

	private static int s_adsResultScreenRate;

	private static bool s_adsOnFirstInteraction;

	private static int s_runsBeforeAds;

	private static AdvertStates s_state;

	public static bool AdsEnabled
	{
		get
		{
			return s_adsEnabled;
		}
	}

	public static int AdsMainMenuRate
	{
		get
		{
			return s_adsMainMenuRate;
		}
	}

	public static int AdsResultScreenRate
	{
		get
		{
			return s_adsResultScreenRate;
		}
	}

	public static bool AdsOnFirstInteraction
	{
		get
		{
			return s_adsOnFirstInteraction;
		}
	}

	public static int RunsBeforeAds
	{
		get
		{
			return s_runsBeforeAds;
		}
	}

	private void Start()
	{
		s_state = this;
		EventDispatch.RegisterInterest("FeatureStateReady", this);
		if (FeatureState.Ready)
		{
			GetFeatureState();
		}
	}

	private bool GetAdState(LSON.Property thisProperty)
	{
		if (thisProperty == null)
		{
			return true;
		}
		bool boolValue = false;
		return !LSONProperties.AsBool(thisProperty, out boolValue) || boolValue;
	}

	private void Event_FeatureStateReady()
	{
		GetFeatureState();
	}

	private void GetFeatureState()
	{
		LSON.Property stateProperty = FeatureState.GetStateProperty("adstate", "enable");
		s_adsEnabled = GetAdState(stateProperty);
		if (s_adsEnabled)
		{
			bool flag = false;
			LSON.Property stateProperty2 = FeatureState.GetStateProperty("adstate", "mm_rate");
			if (stateProperty2 != null)
			{
				flag = LSONProperties.AsInt(stateProperty2, out s_adsMainMenuRate);
			}
			if (!flag)
			{
				s_adsMainMenuRate = 2;
			}
			flag = false;
			stateProperty2 = FeatureState.GetStateProperty("adstate", "result_rate");
			if (stateProperty2 != null)
			{
				flag = LSONProperties.AsInt(stateProperty2, out s_adsResultScreenRate);
			}
			if (!flag)
			{
				s_adsResultScreenRate = 3;
			}
			flag = false;
			stateProperty2 = FeatureState.GetStateProperty("adstate", "ads_on_first_interaction");
			if (stateProperty2 != null)
			{
				flag = LSONProperties.AsBool(stateProperty2, out s_adsOnFirstInteraction);
			}
			if (!flag)
			{
				s_adsOnFirstInteraction = true;
			}
			flag = false;
			stateProperty2 = FeatureState.GetStateProperty("adstate", "runs_before_ads");
			if (stateProperty2 != null)
			{
				flag = LSONProperties.AsInt(stateProperty2, out s_runsBeforeAds);
			}
			if (!flag)
			{
				s_runsBeforeAds = 0;
			}
		}
	}
}
