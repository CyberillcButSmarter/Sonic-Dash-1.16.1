using System.Collections.Generic;

public class GrantooListener : PropellerSDKListener
{
	public override void SdkCompletedWithMatch(Dictionary<string, string> matchInfo)
	{
		GrantooManager.GetInstance().OnSDKCompletedWithMatch(matchInfo);
	}

	public override void SdkCompletedWithExit()
	{
		GrantooManager.GetInstance().OnSDKCompletedWithExit();
	}

	public override void SdkFailed(string strReason)
	{
		GrantooManager.GetInstance().OnSDKCompletedWithError(strReason);
	}

	public override void SdkSocialLogin(bool bAllowCachedLogin)
	{
		GrantooManager.GetInstance().OnSDKSocialLogin(bAllowCachedLogin);
	}
}
