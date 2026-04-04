using System;
using System.Collections;
using System.Collections.Generic;
using Prime31;
using UnityEngine;

public class CloudStorage : MonoBehaviour
{
	private enum State
	{
		Idle = 0,
		Saving = 1,
		Loading = 2,
		CloudDidChange = 3,
		GameDidChange = 4
	}

	private const string FeatureStateRoot = "state";

	private const string ICloudStateProperty = "icloud";

	private const int SaveVersion = 1;

	private const string PropertyKeyDesc = "SonicDashProperties";

	private const string PropertyTimeStampDesc = "TimeStamp";

	private const string PropertyDeviceDesc = "Device";

	private static CloudStorage s_instance;

	private static bool s_active = true;

	private static bool s_connected;

	private bool m_firstTime = true;

	private static State s_state;

	private static Hashtable s_properties;

	public static CloudStorage Instance
	{
		get
		{
			return s_instance;
		}
	}

	public static bool CloudDidChange
	{
		get
		{
			return s_state == State.CloudDidChange;
		}
	}

	public static string DeviceDesc
	{
		get
		{
			return SystemInfo.deviceModel;
		}
	}

	private void Awake()
	{
		s_instance = this;
		s_properties = new Hashtable();
		s_state = State.Idle;
		EventDispatch.RegisterInterest("OnAllAssetsLoaded", this);
		EventDispatch.RegisterInterest("FeatureStateReady", this);
		Log("Constructor");
	}

	private void OnEnable()
	{
		PropertyStore.AddLoadDelegateEvent(CloudLoad);
		Log("Enabled");
	}

	private void OnDisable()
	{
		PropertyStore.RemoveLoadDelegateEvent(CloudLoad);
		Log("Disabled");
	}

	private void Event_OnAllAssetsLoaded()
	{
		if (HLSocialPluginAndroid.IsGooglePlusSignedIn())
		{
			EventGoogleSync();
		}
	}

	private bool Init()
	{
		if (m_firstTime)
		{
			m_firstTime = false;
			UpdateServerActiveFlag();
			if (!s_active)
			{
				return false;
			}
			if (s_state != State.CloudDidChange)
			{
				string cloudProperty = GetCloudProperty("TimeStamp", false);
				if (cloudProperty == null)
				{
					return false;
				}
				string text = PropertyStore.ActiveProperties().GetString("TimeStamp");
				if (cloudProperty != text)
				{
					s_state = State.CloudDidChange;
				}
			}
			if (CloudDidChange)
			{
				if (PlayerStats.GetStat(PlayerStats.StatNames.NumberOfSessions_Total) == 1)
				{
					Load();
					return true;
				}
				return Sync();
			}
		}
		return false;
	}

	public static bool Sync()
	{
		if (s_connected && !P31Prefs.isCloudConnected())
		{
			s_connected = false;
		}
		if (s_active && s_connected)
		{
			bool flag = SyncCloudData();
			if (flag && CloudDidChange)
			{
				Dialog_CloudConflict.Display();
			}
			else
			{
				Save(false);
			}
			return flag;
		}
		return false;
	}

	public static void Save(bool overrideCloud)
	{
		if (s_active && s_connected)
		{
			s_state = State.Saving;
			UpdateCloudData();
			if (overrideCloud || s_state == State.GameDidChange)
			{
				SaveCloudData();
				Log("CloudStorageSaved");
				SetPropertyStore("TimeStamp");
				PropertyStore.Save();
			}
			s_state = State.Idle;
		}
	}

	public static void Load()
	{
		if (s_active && s_connected && CloudDidChange)
		{
			PropertyStore.Load(true);
			PropertyStore.Save();
		}
	}

	private static void CloudLoad()
	{
		if (CloudDidChange)
		{
			s_state = State.Loading;
			Log("CloudStorage Load called.");
			if (LoadCloudData())
			{
				UpdatePropertyStore();
				Log("CloudStorageLoaded");
			}
		}
		s_state = State.Idle;
	}

	private static void UpdatePropertyStore()
	{
		SetPropertyStore("TimeStamp");
		foreach (string key in s_properties.Keys)
		{
			if (key != "CharacterSelection")
			{
				SetPropertyStore(key);
			}
		}
	}

	private static void SetPropertyStore(string name)
	{
		string cloudProperty = GetCloudProperty(name, true);
		if (cloudProperty != null)
		{
			PropertyStore.Store(name, cloudProperty);
		}
	}

	public static void Reset()
	{
		s_state = State.Loading;
		s_properties.Clear();
		s_state = State.Idle;
		Log("Reset");
	}

	private static void UpdateCloudData()
	{
		List<PropertyStore.Property> propertyList = PropertyStore.ActiveProperties().PropertyList;
		for (int i = 0; i < propertyList.Count; i++)
		{
			PropertyStore.Property property = propertyList[i];
			SetCloudProperty(property.m_name, property.m_value);
		}
	}

	private static void SetCloudPlayerStatsProperty<T1, T2>(T1 nameEnum, T2[] stats)
	{
		SetCloudProperty(nameEnum.ToString(), stats[Convert.ToInt32(nameEnum)]);
	}

	public static string GetCloudProperty(string name, bool fromLocal)
	{
		object obj = null;
		if (fromLocal)
		{
			obj = s_properties[name];
		}
		else
		{
			IDictionary dictionary = P31Prefs.getDictionary("SonicDashProperties");
			if (dictionary != null)
			{
				obj = dictionary[name];
			}
		}
		if (obj != null)
		{
			return Convert.ToString(obj);
		}
		return null;
	}

	public static string GetCloudPropertyDeviceDesc(bool fromLocal)
	{
		return GetCloudProperty("Device", fromLocal);
	}

	private static void SetCloudProperty<T>(string name, T value)
	{
		string text = value.ToString();
		bool flag = s_properties.Contains(name);
		string cloudProperty = GetCloudProperty(name, true);
		if (!flag || cloudProperty != text)
		{
			s_properties[name] = text;
			s_state = State.GameDidChange;
		}
	}

	private static void Event_KeyValueStoreDidChange(List<object> keys)
	{
		Log("********************************************************");
		Log("***** Event_KeyValueStoreDidChange.  changed keys: *****");
		foreach (object key in keys)
		{
			Log(string.Empty + key.ToString());
			if (key.ToString() == "SonicDashProperties")
			{
				IDictionary dictionary = P31Prefs.getDictionary("SonicDashProperties");
				Prime31.Utils.logObject(dictionary);
				if (s_properties["TimeStamp"] != dictionary["TimeStamp"])
				{
					EventDispatch.GenerateEvent("OnCloudStorageChanged");
					s_state = State.CloudDidChange;
				}
			}
		}
		Log("********************************************************");
	}

	private static void Event_UbiquityIdentityDidChange()
	{
		Log("Event_UbiquityIdentityDidChange");
	}

	private static void Event_EntitlementsMissing()
	{
		Log("Event_EntitlementsMissing");
	}

	private static void SaveCloudData()
	{
		Log("SaveCloudData");
		SetCloudProperty("TimeStamp", DCTime.GetCurrentTime().Ticks);
		SetCloudProperty("Device", DeviceDesc);
		P31Prefs.setDictionary("SonicDashProperties", s_properties);
	}

	private static bool LoadCloudData()
	{
		Log("LoadCloudData");
		IDictionary dictionary = P31Prefs.getDictionary("SonicDashProperties");
		if (dictionary != null)
		{
			Prime31.Utils.logObject(dictionary);
			s_properties = new Hashtable(dictionary);
			return true;
		}
		Log("No dictionary found");
		return false;
	}

	private static bool SyncCloudData()
	{
		bool flag = false;
		return P31Prefs.synchronize();
	}

	private void Event_FeatureStateReady()
	{
		UpdateServerActiveFlag();
	}

	private void UpdateServerActiveFlag()
	{
		s_active = true;
		LSON.Property stateProperty = FeatureState.GetStateProperty("state", "icloud");
		if (stateProperty != null)
		{
			bool boolValue = true;
			if (LSONProperties.AsBool(stateProperty, out boolValue) && !boolValue)
			{
				s_active = false;
			}
		}
	}

	public static void EventGoogleSync()
	{
		if (!s_connected)
		{
			s_connected = true;
			P31Prefs.rebuildDictionary();
			Instance.Init();
		}
	}

	private static void Log(string msg)
	{
	}
}
