using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PropellerListenerExample : PropellerSDKListener
{
	private string m_tournamentID;

	private string m_matchID;

	private PropellerExample m_propellerExample;

	public PropellerListenerExample(PropellerExample propellerExample)
	{
		m_tournamentID = string.Empty;
		m_matchID = string.Empty;
		m_propellerExample = propellerExample;
	}

	public string GetTournamentID()
	{
		return m_tournamentID;
	}

	public string GetMatchID()
	{
		return m_matchID;
	}

	public override void SdkCompletedWithMatch(Dictionary<string, string> matchResult)
	{
		string text = matchResult["tournamentID"];
		string text2 = matchResult["matchID"];
		Debug.Log("PropellerExample - SdkCompletedWithMatch - " + text + " - " + text2);
		Debug.Log("Params - " + matchResult["paramsJSON"]);
		m_tournamentID = text;
		m_matchID = text2;
		m_propellerExample.InvokeMatchDetails();
	}

	public override void SdkCompletedWithExit()
	{
		Debug.Log("PropellerExample - SdkCompletedWithExit");
	}

	public override void SdkFailed(string reason)
	{
		Debug.Log("PropellerExample - SdkFailed - " + reason);
	}

	public override void SdkSocialLogin(bool allowCache)
	{
		Debug.Log("PropellerExample - SdkSocialLogin - " + allowCache);
		m_propellerExample.InvokeLoginDetails();
	}

	public override void SdkSocialInvite(Dictionary<string, string> inviteDetail)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = true;
		foreach (KeyValuePair<string, string> item in inviteDetail)
		{
			if (flag)
			{
				flag = false;
			}
			else
			{
				stringBuilder.Append(", ");
			}
			stringBuilder.Append(item.Key);
			stringBuilder.Append("=");
			stringBuilder.Append(item.Value);
		}
		Debug.Log("PropellerExample - SdkSocialInvite - " + stringBuilder.ToString());
		m_propellerExample.InvokeInviteCompleted();
	}

	public override void SdkSocialShare(Dictionary<string, string> shareDetail)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = true;
		foreach (KeyValuePair<string, string> item in shareDetail)
		{
			if (flag)
			{
				flag = false;
			}
			else
			{
				stringBuilder.Append(", ");
			}
			stringBuilder.Append(item.Key);
			stringBuilder.Append("=");
			stringBuilder.Append(item.Value);
		}
		Debug.Log("PropellerExample - SdkSocialShare - " + stringBuilder.ToString());
		m_propellerExample.InvokeShareCompleted();
	}
}
