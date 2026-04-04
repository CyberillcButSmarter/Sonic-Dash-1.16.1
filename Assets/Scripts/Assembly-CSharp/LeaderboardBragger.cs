using UnityEngine;

public class LeaderboardBragger : MonoBehaviour
{
	private void Trigger_BragScore()
	{
	}

	private void Trigger_FBInviteFriend(GameObject callerObject)
	{
		UIButtonMessage component = callerObject.GetComponent<UIButtonMessage>();
		GameObject gameObject = Utils.FindTagInChildren(component.target, "HighScore_Name");
		UILabel component2 = gameObject.GetComponent<UILabel>();
		HLUserProfile userProfileFromUserName = ((HLSocialPlatform)Social.Active).GetUserProfileFromUserName(component2.text);
		string id = userProfileFromUserName.id;
		Community.g_instance.InviteFBFriend(id);
	}

	private void Trigger_InviteFriends()
	{
		if (SLSocial.IsAvailable())
		{
			PlayerStats.IncreaseStat(PlayerStats.StatNames.TimesBragged_Total, 1);
			StarRingsRewards.Reward(StarRingsRewards.Reason.Bragging);
			string text = null;
			if (ScoreTracker.HighScore > 0)
			{
				string format = LanguageStrings.First.GetString("SOCIAL_INVITE_WITH_SCORE");
				text = string.Format(format, LanguageUtils.FormatNumber(ScoreTracker.HighScore));
			}
			else
			{
				text = LanguageStrings.First.GetString("SOCIAL_INVITE_NO_SCORE");
			}
			if (text != null)
			{
				SLSocial.ShareMessage(text);
			}
		}
	}
}
