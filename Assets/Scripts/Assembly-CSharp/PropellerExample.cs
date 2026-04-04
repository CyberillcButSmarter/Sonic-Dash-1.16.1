using System.Collections.Generic;
using UnityEngine;

public class PropellerExample : MonoBehaviour
{
	private bool m_bInitialized;

	private PropellerSDKListener m_listener;

	private string m_virtualGoodsTransactionId;

	private void OnGUI()
	{
		float left = (float)Screen.width * 0.5f - 160f;
		float num = (float)Screen.height * 0.125f;
		float num2 = num / 8f;
		if (GUI.Button(new Rect(left, num2 + (num + num2) * 0f, 320f, num), "Launch"))
		{
			PropellerSDK.Launch(m_listener);
		}
		else if (GUI.Button(new Rect(left, num2 + (num + num2) * 1f, 320f, num), "Test Login"))
		{
			InvokeLoginDetails();
		}
		else if (GUI.Button(new Rect(left, num2 + (num + num2) * 2f, 320f, num), "SyncChallengeCounts"))
		{
			PropellerSDK.SyncChallengeCounts();
		}
		else if (GUI.Button(new Rect(left, num2 + (num + num2) * 3f, 320f, num), "SyncTournamentInfo"))
		{
			PropellerSDK.SyncTournamentInfo();
		}
		else if (GUI.Button(new Rect(left, num2 + (num + num2) * 4f, 320f, num), "SyncVirtualGoods"))
		{
			PropellerSDK.SyncVirtualGoods();
		}
		else if (GUI.Button(new Rect(left, num2 + (num + num2) * 5f, 320f, num), "EnableNotifications"))
		{
			PropellerSDK.EnableNotification(PropellerSDK.NotificationType.all);
		}
		else if (GUI.Button(new Rect(left, num2 + (num + num2) * 6f, 320f, num), "DisableNotifications"))
		{
			PropellerSDK.DisableNotification(PropellerSDK.NotificationType.all);
		}
	}

	public void OnPropellerSDKChallengeCountUpdated(string count)
	{
		Debug.Log("PropellerExample - OnPropellerSDKChallengeCountUpdated - " + count);
	}

	public void OnPropellerSDKTournamentInfo(Dictionary<string, string> tournamentInfo)
	{
		DisplayTournamentInfo("OnPropellerSDKTournamentInfo", tournamentInfo);
	}

	public void OnPropellerSDKVirtualGoodList(Dictionary<string, object> virtualGoodInfo)
	{
		string text = (string)virtualGoodInfo["transactionID"];
		List<string> list = (List<string>)virtualGoodInfo["virtualGoods"];
		string text2 = string.Join(" - ", list.ToArray());
		Debug.Log("PropellerExample - OnPropellerSDKVirtualGoodList - " + text + " - " + text2);
		m_virtualGoodsTransactionId = text;
		InvokeAcknowledgeVirtualGoods();
	}

	public void OnPropellerSDKVirtualGoodRollback(string transactionId)
	{
		Debug.Log("PropellerExample - OnPropellerSDKVirtualGoodRollback - " + transactionId);
	}

	private void Awake()
	{
		if (!m_bInitialized)
		{
			Object.DontDestroyOnLoad(base.gameObject);
			if (!Application.isEditor)
			{
				m_listener = new PropellerListenerExample(this);
			}
			m_bInitialized = true;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void InvokeMatchDetails()
	{
		Debug.Log("PropellerExample - InvokeMatchDetails");
		Invoke("SendMatchDetails", 0.5f);
	}

	private void SendMatchDetails()
	{
		Debug.Log("PropellerExample - SendMatchDetails");
		PropellerListenerExample propellerListenerExample = (PropellerListenerExample)m_listener;
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("matchID", propellerListenerExample.GetMatchID());
		dictionary.Add("tournamentID", propellerListenerExample.GetTournamentID());
		dictionary.Add("score", "55");
		PropellerSDK.LaunchWithMatchResult(dictionary, m_listener);
	}

	public void InvokeLoginDetails()
	{
		Debug.Log("PropellerExample - InvokeLoginDetails");
		Invoke("SendLoginDetails", 0.5f);
	}

	private void SendLoginDetails()
	{
		Debug.Log("PropellerExample - SendingLoginDetails");
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("provider", "facebook");
		dictionary.Add("email", "testguy@grantoo.org");
		dictionary.Add("id", "testguyid");
		dictionary.Add("nickname", "testguy445");
		dictionary.Add("token", "testguy445");
		PropellerSDK.SdkSocialLoginCompleted(dictionary);
	}

	public void InvokeInviteCompleted()
	{
		Debug.Log("PropellerExample - InvokeInviteCompleted");
		Invoke("SendInviteCompleted", 0.5f);
	}

	private void SendInviteCompleted()
	{
		Debug.Log("PropellerExample - SendInviteCompleted");
		PropellerSDK.SdkSocialInviteCompleted();
	}

	public void InvokeShareCompleted()
	{
		Debug.Log("PropellerExample - InvokeShareCompleted");
		Invoke("SendShareCompleted", 0.5f);
	}

	private void SendShareCompleted()
	{
		Debug.Log("PropellerExample - SendShareCompleted");
		PropellerSDK.SdkSocialShareCompleted();
	}

	public void InvokeAcknowledgeVirtualGoods()
	{
		Debug.Log("PropellerExample - InvokeAcknowledgeVirtualGoods");
		Invoke("SendAcknowledgeVirtualGoods", 0.5f);
	}

	private void SendAcknowledgeVirtualGoods()
	{
		Debug.Log("PropellerExample - SendAcknowledgeVirtualGoods");
		PropellerSDK.AcknowledgeVirtualGoods(m_virtualGoodsTransactionId, true);
	}

	private void DisplayTournamentInfo(string tag, Dictionary<string, string> tournamentInfo)
	{
		if (tournamentInfo == null || tournamentInfo.Count == 0)
		{
			Debug.Log("PropellerExample - " + tag + " - no tournament currently running");
			return;
		}
		string text = tournamentInfo["name"];
		string text2 = tournamentInfo["campaignName"];
		string text3 = tournamentInfo["sponsorName"];
		string text4 = tournamentInfo["startDate"];
		string text5 = tournamentInfo["endDate"];
		string text6 = tournamentInfo["logo"];
		Debug.Log("PropellerExample - " + tag + " - " + text + " - " + text2 + " - " + text3 + " - " + text4 + " - " + text5 + " - " + text6);
	}
}
