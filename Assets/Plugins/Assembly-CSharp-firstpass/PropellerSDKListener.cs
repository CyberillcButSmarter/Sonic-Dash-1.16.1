using System.Collections.Generic;
using UnityEngine;

public abstract class PropellerSDKListener
{
	public abstract void SdkCompletedWithExit();

	public abstract void SdkCompletedWithMatch(Dictionary<string, string> matchInfo);

	public abstract void SdkFailed(string reason);

	public virtual void SdkSocialLogin(bool allowCache)
	{
		Debug.Log("SdkSocialLogin default");
	}

	public virtual void SdkSocialInvite(Dictionary<string, string> inviteDetail)
	{
		Debug.Log("SdkSocialInvite default");
	}

	public virtual void SdkSocialShare(Dictionary<string, string> shareDetail)
	{
		Debug.Log("SdkSocialShare default");
	}
}
