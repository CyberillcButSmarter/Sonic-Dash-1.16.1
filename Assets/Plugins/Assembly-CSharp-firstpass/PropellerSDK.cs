using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class PropellerSDK : MonoBehaviour
{
	public enum ContentOrientation
	{
		landscape = 0,
		portrait = 1,
		auto = 2
	}

	public enum NotificationType
	{
		none = 0,
		all = 1,
		push = 2,
		local = 3
	}

	private enum DataType
	{
		intType = 0,
		longType = 1,
		floatType = 2,
		doubleType = 3,
		boolType = 4,
		stringType = 5
	}

	public static bool AllowEnablePropeller;

	public string GameKey;

	public string GameSecret;

	public bool UseTestServers;

	public ContentOrientation Orientation;

	public string HostGameObjectName;

	public bool iOSGameHandleLogin;

	public bool iOSGameHandleInvite;

	public bool iOSGameHandleShare;

	public string AndroidGCMSenderID;

	public bool AndroidGameHandleLogin;

	public bool AndroidGameHandleInvite;

	public bool AndroidGameHandleShare;

	private static bool m_bInitialized;

	private static PropellerSDKListener m_listener;

	private static GameObject m_hostGameObject;

	private static AndroidJavaClass m_jniPropellerUnity;

	static PropellerSDK()
	{
		m_bInitialized = false;
	}

	public static bool Launch(PropellerSDKListener listener)
	{
		Debug.Log("Launch - start");
		m_listener = listener;
		bool result = false;
		if (!Application.isEditor && m_jniPropellerUnity != null)
		{
			result = m_jniPropellerUnity.CallStatic<bool>("Launch", new object[0]);
		}
		Debug.Log("Launch - end");
		return result;
	}

	public static bool LaunchWithMatchResult(Dictionary<string, object> matchResult, PropellerSDKListener listener)
	{
		Debug.Log("LaunchWithMatchResult - start");
		m_listener = listener;
		bool result = false;
		if (!Application.isEditor)
		{
			JSONClass jSONClass = toJSONClass(matchResult);
			if (jSONClass == null)
			{
				Debug.Log("LaunchWithMatchResult - match result parse error");
			}
			else
			{
				using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("java.lang.String", jSONClass.ToString()))
				{
					if (m_jniPropellerUnity != null)
					{
						result = m_jniPropellerUnity.CallStatic<bool>("LaunchWithMatchResult", new object[1] { androidJavaObject });
					}
				}
			}
		}
		Debug.Log("LaunchWithMatchResult - end");
		return result;
	}

	public static void SyncChallengeCounts()
	{
		Debug.Log("SyncChallengeCounts - start");
		if (!Application.isEditor && m_jniPropellerUnity != null)
		{
			m_jniPropellerUnity.CallStatic("SyncChallengeCounts");
		}
		Debug.Log("SyncChallengeCounts - end");
	}

	public static void SyncTournamentInfo()
	{
		Debug.Log("SyncTournamentInfo - start");
		if (!Application.isEditor && m_jniPropellerUnity != null)
		{
			m_jniPropellerUnity.CallStatic("SyncTournamentInfo");
		}
		Debug.Log("SyncTournamentInfo - end");
	}

	public static void SyncVirtualGoods()
	{
		Debug.Log("SyncVirtualGoods - start");
		if (!Application.isEditor && m_jniPropellerUnity != null)
		{
			m_jniPropellerUnity.CallStatic("SyncVirtualGoods");
		}
		Debug.Log("SyncVirtualGoods - end");
	}

	public static void AcknowledgeVirtualGoods(string transactionId, bool consumed)
	{
		Debug.Log("AcknowledgeVirtualGoods - start");
		if (!Application.isEditor && m_jniPropellerUnity != null)
		{
			m_jniPropellerUnity.CallStatic("AcknowledgeVirtualGoods", transactionId, consumed);
		}
		Debug.Log("AcknowledgeVirtualGoods - end");
	}

	public static void EnableNotification(NotificationType notificationType)
	{
		Debug.Log("EnableNotification - start");
		if (!Application.isEditor)
		{
			AndroidJavaObject propellerSDKNotificationType = GetPropellerSDKNotificationType(notificationType);
			if (propellerSDKNotificationType == null)
			{
				Debug.Log("EnableNotification - invalid notification type");
				return;
			}
			if (m_jniPropellerUnity != null)
			{
				m_jniPropellerUnity.CallStatic<bool>("EnableNotification", new object[1] { propellerSDKNotificationType });
			}
		}
		Debug.Log("EnableNotification - end");
	}

	public static void DisableNotification(NotificationType notificationType)
	{
		Debug.Log("DisableNotification - start");
		if (!Application.isEditor)
		{
			AndroidJavaObject propellerSDKNotificationType = GetPropellerSDKNotificationType(notificationType);
			if (propellerSDKNotificationType == null)
			{
				Debug.Log("DisableNotification - invalid notification type");
				return;
			}
			if (m_jniPropellerUnity != null)
			{
				m_jniPropellerUnity.CallStatic<bool>("DisableNotification", new object[1] { propellerSDKNotificationType });
			}
		}
		Debug.Log("DisableNotification - end");
	}

	public static bool IsNotificationEnabled(NotificationType notificationType)
	{
		Debug.Log("IsNotificationEnabled - start");
		bool result = false;
		if (!Application.isEditor)
		{
			AndroidJavaObject propellerSDKNotificationType = GetPropellerSDKNotificationType(notificationType);
			if (propellerSDKNotificationType == null)
			{
				Debug.Log("IsNotificationEnabled - invalid notification type");
				return false;
			}
			if (m_jniPropellerUnity != null)
			{
				result = m_jniPropellerUnity.CallStatic<bool>("IsNotificationEnabled", new object[1] { propellerSDKNotificationType });
			}
		}
		Debug.Log("IsNotificationEnabled - end");
		return result;
	}

	private static AndroidJavaObject GetPropellerSDKNotificationType(NotificationType notificationType)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("org.grantoo.lib.propeller.PropellerSDKNotificationType");
		AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("values", new object[0]);
		IntPtr rawObject = androidJavaObject.GetRawObject();
		if (rawObject.ToInt32() == 0)
		{
			Debug.Log("PropellerSDKNotificationType values array is null");
			return null;
		}
		AndroidJavaObject[] array = AndroidJNIHelper.ConvertFromJNIArray<AndroidJavaObject[]>(rawObject);
		if ((int)notificationType >= array.Length)
		{
			Debug.Log("NotificationType ordinal index is out-of-bounds");
			return null;
		}
		return array[(int)notificationType];
	}

	public static void SdkSocialLoginCompleted(Dictionary<string, string> loginInfo)
	{
		Debug.Log("SdkSocialLoginCompleted - start");
		if (!Application.isEditor)
		{
			using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("java.util.HashMap"))
			{
				if (loginInfo != null)
				{
					IntPtr methodID = AndroidJNI.GetMethodID(androidJavaObject.GetRawClass(), "put", "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
					foreach (string key in loginInfo.Keys)
					{
						using (AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("java.lang.String", key))
						{
							using (AndroidJavaObject androidJavaObject3 = new AndroidJavaObject("java.lang.String", loginInfo[key]))
							{
								jvalue[] args = AndroidJNIHelper.CreateJNIArgArray(new object[2] { androidJavaObject2, androidJavaObject3 });
								AndroidJNI.CallObjectMethod(androidJavaObject.GetRawObject(), methodID, args);
							}
						}
					}
				}
				if (m_jniPropellerUnity != null)
				{
					m_jniPropellerUnity.CallStatic("SdkSocialLoginCompleted", androidJavaObject);
				}
			}
		}
		Debug.Log("SdkSocialLoginCompleted - end");
	}

	public static void SdkSocialInviteCompleted()
	{
		Debug.Log("SdkSocialInviteCompleted - start");
		if (!Application.isEditor && m_jniPropellerUnity != null)
		{
			m_jniPropellerUnity.CallStatic("SdkSocialInviteCompleted");
		}
		Debug.Log("SdkSocialInviteCompleted - end");
	}

	public static void SdkSocialShareCompleted()
	{
		Debug.Log("SdkSocialShareCompleted - start");
		if (!Application.isEditor && m_jniPropellerUnity != null)
		{
			m_jniPropellerUnity.CallStatic("SdkSocialShareCompleted");
		}
		Debug.Log("SdkSocialShareCompleted - end");
	}

	public static void RestoreAllLocalNotifications()
	{
		Debug.Log("RestoreAllLocalNotifications - start");
		if (!Application.isEditor)
		{
			Debug.Log("RestoreAllLocalNotifications - unused by Android");
		}
		Debug.Log("RestoreAllLocalNotifications - end");
	}

	private void Awake()
	{
		if (!m_bInitialized)
		{
			if (!AllowEnablePropeller)
			{
				return;
			}
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			if (!Application.isEditor)
			{
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				m_jniPropellerUnity = new AndroidJavaClass("org.grantoo.propellersdkunity.PropellerSDKUnitySingleton");
				flag = AndroidGameHandleLogin;
				flag2 = AndroidGameHandleInvite;
				flag3 = AndroidGameHandleShare;
				Initialize(GameKey, GameSecret, Orientation.ToString(), UseTestServers, flag, flag2, flag3);
				if (m_jniPropellerUnity != null)
				{
					m_jniPropellerUnity.CallStatic("InitializeGCM", AndroidGCMSenderID);
				}
				if (!string.IsNullOrEmpty(HostGameObjectName))
				{
					m_hostGameObject = GameObject.Find(HostGameObjectName);
				}
			}
			m_bInitialized = true;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (Application.isEditor)
		{
			return;
		}
		if (paused)
		{
			if (m_jniPropellerUnity != null)
			{
				m_jniPropellerUnity.CallStatic("OnPause");
			}
		}
		else if (m_jniPropellerUnity != null)
		{
			m_jniPropellerUnity.CallStatic("OnResume");
		}
	}

	private void OnApplicationQuit()
	{
		if (!Application.isEditor && m_jniPropellerUnity != null)
		{
			m_jniPropellerUnity.CallStatic("OnQuit");
		}
	}

	private static void Initialize(string key, string secret, string screenOrientation, bool useTestServers, bool gameHasLogin, bool gameHasInvite, bool gameHasShare)
	{
		Debug.Log("Initialize - start");
		if (!Application.isEditor && m_jniPropellerUnity != null)
		{
			m_jniPropellerUnity.CallStatic("Initialize", key, secret, screenOrientation, useTestServers, gameHasLogin, gameHasInvite, gameHasShare);
		}
		Debug.Log("Initialize - end");
	}

	private void PropellerOnSdkCompletedWithExit(string message)
	{
		Debug.Log("PropellerOnSdkCompletedWithExit");
		if (!string.IsNullOrEmpty(message))
		{
			Debug.Log("PropellerOnSdkCompletedWithExit - " + message);
		}
		if (m_listener == null)
		{
			Debug.Log("PropellerOnSdkCompletedWithExit - undefined listener");
		}
		else
		{
			m_listener.SdkCompletedWithExit();
		}
	}

	private void PropellerOnSdkCompletedWithMatch(string message)
	{
		Debug.Log("PropellerOnSdkCompletedWithMatch");
		if (string.IsNullOrEmpty(message))
		{
			Debug.LogError("PropellerOnSdkCompletedWithMatch - null or empty message");
			return;
		}
		Debug.Log("PropellerOnSdkCompletedWithMatch - " + message);
		string[] array = message.Split('&');
		if (array.Length != 3)
		{
			Debug.LogError("PropellerOnSdkCompletedWithMatch - Invalid response from PropellerUnitySDK");
			return;
		}
		string value = array[0];
		string value2 = array[1];
		string value3 = WWW.UnEscapeURL(array[2]);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("tournamentID", value);
		dictionary.Add("matchID", value2);
		dictionary.Add("paramsJSON", value3);
		if (m_listener == null)
		{
			Debug.Log("PropellerOnSdkCompletedWithExit - undefined listener");
		}
		else
		{
			m_listener.SdkCompletedWithMatch(dictionary);
		}
	}

	private void PropellerOnSdkFailed(string message)
	{
		Debug.Log("PropellerOnSdkFailed");
		if (!string.IsNullOrEmpty(message))
		{
			Debug.Log("PropellerOnSdkFailed - " + message);
		}
		if (m_listener == null)
		{
			Debug.Log("PropellerOnSdkFailed - undefined listener");
		}
		else
		{
			m_listener.SdkFailed(message);
		}
	}

	private void PropellerOnChallengeCountChanged(string message)
	{
		Debug.Log("PropellerOnChallengeCountChanged");
		if (string.IsNullOrEmpty(message))
		{
			Debug.Log("PropellerOnChallengeCountChanged - null or empty message");
			return;
		}
		Debug.Log("PropellerOnChallengeCountChanged - " + message);
		if (m_hostGameObject == null)
		{
			Debug.Log("PropellerOnChallengeCountChanged - undefined host game object");
		}
		else
		{
			m_hostGameObject.SendMessage("OnPropellerSDKChallengeCountUpdated", message);
		}
	}

	private void PropellerOnTournamentInfo(string message)
	{
		Debug.Log("PropellerOnTournamentInfo");
		if (string.IsNullOrEmpty(message))
		{
			Debug.Log("PropellerOnTournamentInfo - null or empty message");
			return;
		}
		Debug.Log("PropellerOnTournamentInfo - " + message);
		string[] array = message.Split('&');
		if (array.Length != 6)
		{
			Debug.LogError("PropellerOnTournamentInfo - Invalid response from PropellerUnitySDK");
			return;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		if (!string.IsNullOrEmpty(array[0]))
		{
			dictionary.Add("name", WWW.UnEscapeURL(array[0]));
		}
		if (!string.IsNullOrEmpty(array[1]))
		{
			dictionary.Add("campaignName", WWW.UnEscapeURL(array[1]));
		}
		if (!string.IsNullOrEmpty(array[2]))
		{
			dictionary.Add("sponsorName", WWW.UnEscapeURL(array[2]));
		}
		if (!string.IsNullOrEmpty(array[3]))
		{
			dictionary.Add("startDate", WWW.UnEscapeURL(array[3]));
		}
		if (!string.IsNullOrEmpty(array[4]))
		{
			dictionary.Add("endDate", WWW.UnEscapeURL(array[4]));
		}
		if (!string.IsNullOrEmpty(array[5]))
		{
			dictionary.Add("logo", WWW.UnEscapeURL(array[5]));
		}
		if (m_hostGameObject == null)
		{
			Debug.Log("PropellerOnTournamentInfo - undefined host game object");
		}
		else
		{
			m_hostGameObject.SendMessage("OnPropellerSDKTournamentInfo", dictionary);
		}
	}

	private void PropellerOnVirtualGoodList(string message)
	{
		Debug.Log("PropellerOnVirtualGoodList");
		if (string.IsNullOrEmpty(message))
		{
			Debug.Log("PropellerOnVirtualGoodList - null or empty message");
			return;
		}
		Debug.Log("PropellerOnVirtualGoodList - " + message);
		string[] array = message.Split('&');
		if (array.Length == 0)
		{
			Debug.LogError("PropellerOnVirtualGoodList - Invalid response from PropellerUnitySDK");
			return;
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("transactionID", array[0]);
		List<string> list = new List<string>();
		for (int i = 1; i < array.Length; i++)
		{
			list.Add(array[i]);
		}
		dictionary.Add("virtualGoods", list);
		if (m_hostGameObject == null)
		{
			Debug.Log("PropellerOnVirtualGoodList - undefined host game object");
		}
		else
		{
			m_hostGameObject.SendMessage("OnPropellerSDKVirtualGoodList", dictionary);
		}
	}

	private void PropellerOnVirtualGoodRollback(string message)
	{
		Debug.Log("PropellerOnVirtualGoodRollback");
		if (string.IsNullOrEmpty(message))
		{
			Debug.Log("PropellerOnVirtualGoodRollback - null or empty message");
			return;
		}
		Debug.Log("PropellerOnVirtualGoodRollback - " + message);
		if (m_hostGameObject == null)
		{
			Debug.Log("PropellerOnVirtualGoodRollback - undefined host game object");
		}
		else
		{
			m_hostGameObject.SendMessage("OnPropellerSDKVirtualGoodRollback", message);
		}
	}

	private void PropellerOnSdkSocialLogin(string message)
	{
		Debug.Log("PropellerOnSdkSocialLogin");
		if (string.IsNullOrEmpty(message))
		{
			Debug.Log("PropellerOnSdkSocialLogin - null or empty message");
			return;
		}
		Debug.Log("PropellerOnSdkSocialLogin - " + message);
		if (m_listener == null)
		{
			Debug.Log("PropellerOnSdkSocialLogin - undefined listener");
			return;
		}
		bool allowCache = Convert.ToBoolean(message);
		m_listener.SdkSocialLogin(allowCache);
	}

	private void PropellerOnSdkSocialInvite(string message)
	{
		Debug.Log("PropellerOnSdkSocialInvite");
		if (string.IsNullOrEmpty(message))
		{
			Debug.Log("PropellerOnSdkSocialInvite - null or empty message");
			return;
		}
		Debug.Log("PropellerOnSdkSocialInvite - " + message);
		if (m_listener == null)
		{
			Debug.Log("PropellerOnSdkSocialInvite - undefined listener");
			return;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		string[] array = message.Split('&');
		string[] array2 = array;
		foreach (string text in array2)
		{
			string[] array3 = text.Split('=');
			string key = WWW.UnEscapeURL(array3[0]);
			string value = WWW.UnEscapeURL(array3[1]);
			dictionary.Add(key, value);
		}
		m_listener.SdkSocialInvite(dictionary);
	}

	private void PropellerOnSdkSocialShare(string message)
	{
		Debug.Log("PropellerOnSdkSocialShare");
		if (string.IsNullOrEmpty(message))
		{
			Debug.Log("PropellerOnSdkSocialShare - null or empty message");
			return;
		}
		Debug.Log("PropellerOnSdkSocialShare - " + message);
		if (m_listener == null)
		{
			Debug.Log("PropellerOnSdkSocialShare - undefined listener");
			return;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		string[] array = message.Split('&');
		string[] array2 = array;
		foreach (string text in array2)
		{
			string[] array3 = text.Split('=');
			string key = WWW.UnEscapeURL(array3[0]);
			string value = WWW.UnEscapeURL(array3[1]);
			dictionary.Add(key, value);
		}
		m_listener.SdkSocialShare(dictionary);
	}

	private static JSONClass toJSONClass(Dictionary<string, object> dictionary)
	{
		if (dictionary == null)
		{
			return null;
		}
		JSONClass jSONClass = new JSONClass();
		foreach (KeyValuePair<string, object> item in dictionary)
		{
			JSONNode jSONNode = ((item.Value is List<object>) ? ((JSONNode)toJSONArray((List<object>)item.Value)) : ((JSONNode)((!(item.Value is Dictionary<string, object>)) ? toJSONValue(item.Value) : toJSONClass((Dictionary<string, object>)item.Value))));
			if (!(jSONNode == null))
			{
				jSONClass.Add(item.Key, jSONNode);
			}
		}
		return jSONClass;
	}

	private static JSONArray toJSONArray(List<object> list)
	{
		if (list == null)
		{
			return null;
		}
		JSONArray jSONArray = new JSONArray();
		foreach (object item in list)
		{
			if (item != null)
			{
				JSONNode jSONNode = ((item is List<object>) ? ((JSONNode)toJSONArray((List<object>)item)) : ((JSONNode)((!(item is Dictionary<string, object>)) ? toJSONValue(item) : toJSONClass((Dictionary<string, object>)item))));
				if (!(jSONNode == null))
				{
					jSONArray.Add(jSONNode);
				}
			}
		}
		return jSONArray;
	}

	private static JSONClass toJSONValue(object data)
	{
		if (data == null)
		{
			return null;
		}
		DataType dataType;
		if (data is int)
		{
			dataType = DataType.intType;
		}
		else if (data is long)
		{
			dataType = DataType.longType;
		}
		else if (data is float)
		{
			dataType = DataType.floatType;
		}
		else if (data is double)
		{
			dataType = DataType.doubleType;
		}
		else if (data is bool)
		{
			dataType = DataType.boolType;
		}
		else
		{
			if (!(data is string))
			{
				return null;
			}
			dataType = DataType.stringType;
		}
		JSONData aItem = new JSONData("faddface");
		int num = (int)dataType;
		JSONData aItem2 = new JSONData(num.ToString());
		JSONData aItem3 = new JSONData(data.ToString());
		JSONClass jSONClass = new JSONClass();
		jSONClass.Add("checksum", aItem);
		jSONClass.Add("type", aItem2);
		jSONClass.Add("value", aItem3);
		return jSONClass;
	}
}
