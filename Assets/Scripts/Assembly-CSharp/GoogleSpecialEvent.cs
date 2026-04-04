using System;
using System.Globalization;
using UnityEngine;

public class GoogleSpecialEvent : MonoBehaviour
{
	private const int c_RunsRequired = 100;

	private const string EventRoot = "events";

	private const string AndronicEventProperty = "andronicevent";

	private const string AndronicEventIdProperty = "androniceventid";

	private const string AndronicEventRunRequirementProperty = "androniceventnumruns";

	private const string AndronicEventEndProperty = "androniceventend";

	public const string AndronicRunSaveVar = "AndronicRunSaveVar";

	public const string AndronicEventIdSaveVar = "AndronicEventIdSaveVar";

	public const string AndronicEventDialogShownSaveVar = "AndronicEventDialogShownSaveVar";

	public const string AndronicAwardedSaveVar = "AndronicAwarded";

	public const string AndronicEventFailDialogShownSaveVar = "AndronicEventFailDialogShown";

	private static GoogleSpecialEvent s_Instance;

	private bool GotServerSettingsFile;

	public bool AndronicEventActive { get; private set; }

	public DateTime AndronicEventEnd { get; private set; }

	public int AndronicEventId { get; private set; }

	public int AndronicAndroidRuns { get; private set; }

	public int AndronicAndroidTotalRunsRequired { get; private set; }

	public bool AndronicEventIntroDialogShown { get; private set; }

	public bool AndronicEventFailDialogShown { get; private set; }

	public bool AndronicAwarded { get; private set; }

	public static bool IsEventActive()
	{
		if (s_Instance != null)
		{
			return s_Instance.CheckEventStillActive();
		}
		return false;
	}

	public static bool HasEverBeenActive()
	{
		return true;
	}

	public static int GetRunsNeeded()
	{
		if (!IsEventActive())
		{
			return 0;
		}
		int num = s_Instance.AndronicAndroidTotalRunsRequired - s_Instance.AndronicAndroidRuns;
		if (num < 0)
		{
			num = 0;
		}
		return num;
	}

	public static int GetTotalRunsNeeded()
	{
		if (!IsEventActive())
		{
			return 0;
		}
		return s_Instance.AndronicAndroidTotalRunsRequired;
	}

	public static TimeSpan GetTimeRemaining()
	{
		if (IsEventActive())
		{
			TimeSpan timeSpan = s_Instance.AndronicEventEnd - DateTime.UtcNow;
			if (timeSpan < TimeSpan.Zero)
			{
				return TimeSpan.Zero;
			}
			return timeSpan;
		}
		return TimeSpan.Zero;
	}

	public static void ShowProgressDialog()
	{
		Dialog_GoogleSpecialEvent.Display(Dialog_GoogleSpecialEvent.DialogMode.Progress);
	}

	public static void CheckForSuccessOrFailure()
	{
		if (s_Instance.AndronicEventIntroDialogShown && s_Instance.AndronicAndroidRuns >= s_Instance.AndronicAndroidTotalRunsRequired && !s_Instance.AndronicAwarded)
		{
			s_Instance.Win();
		}
		else if (s_Instance.AndronicEventIntroDialogShown && !s_Instance.AndronicAwarded && !s_Instance.AndronicEventActive && s_Instance.AndronicAndroidRuns < s_Instance.AndronicAndroidTotalRunsRequired && !s_Instance.AndronicEventFailDialogShown)
		{
			s_Instance.Fail();
		}
		else if (s_Instance.GotServerSettingsFile && s_Instance.AndronicAndroidRuns >= s_Instance.AndronicAndroidTotalRunsRequired && !Characters.CharacterUnlocked(Characters.Type.Andronic))
		{
			StorePurchases.RequestReward("Character Andronic", 1, 17, StorePurchases.ShowDialog.No);
		}
	}

	private void Win()
	{
		AndronicAwarded = true;
		StorePurchases.RequestReward("Character Andronic", 1, 17, StorePurchases.ShowDialog.No);
		CharacterManager.Singleton.SetPendingCharacterSelection(Characters.Type.Andronic);
		PropertyStore.Save();
		Dialog_GoogleSpecialEvent.Display(Dialog_GoogleSpecialEvent.DialogMode.Success);
		CheckEventStillActive();
	}

	private void Fail()
	{
		CharacterManager.Singleton.SetPendingCharacterSelection(Characters.Type.Andronic);
		AndronicEventFailDialogShown = true;
		PropertyStore.Save();
		Dialog_GoogleSpecialEvent.Display(Dialog_GoogleSpecialEvent.DialogMode.Failure);
	}

	private bool CheckEventStillActive()
	{
		if (Characters.CharacterUnlocked(Characters.Type.Andronic))
		{
			AndronicEventActive = false;
			AndronicEventEnd = DateTime.Now;
		}
		else if (!AndronicEventActive)
		{
			AndronicEventEnd = DateTime.Now;
		}
		return AndronicEventActive;
	}

	private void Awake()
	{
		AndronicEventActive = false;
		AndronicEventEnd = DateTime.Now;
		AndronicEventId = 0;
		AndronicAndroidRuns = 0;
		AndronicAndroidTotalRunsRequired = 0;
		AndronicEventIntroDialogShown = false;
		AndronicEventFailDialogShown = false;
		AndronicAwarded = false;
		s_Instance = this;
		EventDispatch.RegisterInterest("MainMenuActive", this, EventDispatch.Priority.Highest);
		EventDispatch.RegisterInterest("FeatureStateReady", this);
		EventDispatch.RegisterInterest("OnGameDataSaveRequest", this);
		EventDispatch.RegisterInterest("OnGameDataLoaded", this);
		EventDispatch.RegisterInterest("OnGameFinished", this);
	}

	private void Event_MainMenuActive()
	{
		if (AndronicEventActive && !AndronicEventIntroDialogShown)
		{
			Dialog_GoogleSpecialEvent.Display(Dialog_GoogleSpecialEvent.DialogMode.Progress);
			AndronicEventIntroDialogShown = true;
		}
		else
		{
			CheckForSuccessOrFailure();
		}
	}

	private void Event_OnGameFinished()
	{
		Characters.Type currentCharacterSelection = CharacterManager.Singleton.GetCurrentCharacterSelection();
		if (AndronicEventActive && currentCharacterSelection == Characters.Type.Android)
		{
			AndronicAndroidRuns++;
			Debug.Log("Andronic Android runs = " + AndronicAndroidRuns);
		}
	}

	private void Event_OnGameDataSaveRequest()
	{
		PropertyStore.Store("AndronicRunSaveVar", AndronicAndroidRuns);
		PropertyStore.Store("AndronicEventIdSaveVar", AndronicEventId);
		PropertyStore.Store("AndronicEventDialogShownSaveVar", AndronicEventIntroDialogShown);
		PropertyStore.Store("AndronicEventFailDialogShown", AndronicEventFailDialogShown);
		PropertyStore.Store("AndronicAwarded", AndronicAwarded);
	}

	private void Event_OnGameDataLoaded(ActiveProperties activeProperties)
	{
		AndronicAndroidRuns = Mathf.Max(0, activeProperties.GetInt("AndronicRunSaveVar"));
		AndronicEventId = activeProperties.GetInt("AndronicEventIdSaveVar");
		AndronicEventIntroDialogShown = activeProperties.GetBool("AndronicEventDialogShownSaveVar");
		AndronicEventFailDialogShown = activeProperties.GetBool("AndronicEventDialogShownSaveVar");
		AndronicAwarded = activeProperties.GetBool("AndronicAwarded");
	}

	private void Event_FeatureStateReady()
	{
		if (!FeatureState.Valid)
		{
			return;
		}
		LSON.Property stateProperty = FeatureState.GetStateProperty("events", "andronicevent");
		if (stateProperty != null)
		{
			bool boolValue = false;
			if (LSONProperties.AsBool(stateProperty, out boolValue))
			{
				AndronicEventActive = boolValue;
			}
		}
		stateProperty = FeatureState.GetStateProperty("events", "androniceventnumruns");
		if (stateProperty != null)
		{
			int intValue = 0;
			if (LSONProperties.AsInt(stateProperty, out intValue))
			{
				AndronicAndroidTotalRunsRequired = intValue;
			}
		}
		stateProperty = FeatureState.GetStateProperty("events", "androniceventid");
		if (stateProperty != null)
		{
			int intValue2 = 0;
			if (LSONProperties.AsInt(stateProperty, out intValue2))
			{
				if (AndronicEventId != intValue2)
				{
					AndronicAndroidRuns = 0;
					if (!AndronicAwarded)
					{
						AndronicEventIntroDialogShown = false;
						AndronicEventFailDialogShown = false;
					}
				}
				AndronicEventId = intValue2;
			}
		}
		stateProperty = FeatureState.GetStateProperty("events", "androniceventend");
		string stringValue;
		if (stateProperty != null && LSONProperties.AsString(stateProperty, out stringValue))
		{
			CultureInfo provider = new CultureInfo("en-US");
			stringValue = stringValue.Replace("_", " ");
			stringValue = stringValue.Replace(".", ":");
			DateTime result = DateTime.Now;
			if (DateTime.TryParse(stringValue, provider, DateTimeStyles.None, out result))
			{
				AndronicEventEnd = result;
			}
		}
		CheckEventStillActive();
		GotServerSettingsFile = true;
	}
}
