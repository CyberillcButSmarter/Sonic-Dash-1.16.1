using UnityEngine;

public class DialogContent_PlayerReward : MonoBehaviour
{
	public enum Reason
	{
		None = 0,
		BeatHighScore = 1,
		Ran5000Meters = 2,
		PostedToSocial = 3,
		AllAchievements = 4,
		FacebookLogin = 5,
		FacebookLike = 6,
		TwitterFollow = 7,
		RateMe = 8,
		FinalDay = 9,
		DailyChallengeComplete = 10,
		WebPurchase = 11,
		ShadowUnlocked = 12,
		WatchVideo = 13,
		TutorialComplete = 14,
		GlobalChallengeMilestone = 15,
		GlobalChallengeComplete = 16,
		GoogleSpecialEvent = 17,
		EspioSpecialEvent = 18,
		WheelOfFortuneNormalPrize = 19,
		GPlusLogin = 20,
		GrantooWinner = 21
	}

	public class Content
	{
		public readonly string m_titleLoc;

		public readonly string m_infoLoc;

		public Content(string title, string info)
		{
			m_titleLoc = title;
			m_infoLoc = info;
		}
	}

	private static Content[] s_dialogContent;

	static DialogContent_PlayerReward()
	{
		s_dialogContent = new Content[22]
		{
			new Content("DIALOG_CONGRATS", string.Empty),
			new Content("DIALOG_CONGRATS", "REWARD_REASON_BEAT_HIGH_SCORE"),
			new Content("DIALOG_CONGRATS", "REWARD_REASON_TRAVELLED_5000_METERS"),
			new Content("DIALOG_CONGRATS", "REWARD_REASON_POSTED_TO_SOCIAL"),
			new Content("DIALOG_CONGRATS", "REWARD_REASON_ALL_ACHIEVEMENTS"),
			new Content("FACEBOOK_LOGIN_COMPLETE_TITLE", "FACEBOOK_LOGIN_COMPLETE_BODY"),
			new Content("FACEBOOK_LIKE_TITLE", "FACEBOOK_LIKE_BODY"),
			new Content("TWITTER_FOLLOW_TITLE", "TWITTER_FOLLOW_BODY"),
			new Content("RATE_ME_TITLE", "RATE_ME_BODY"),
			new Content("DIALOG_CONGRATS", string.Empty),
			new Content("COMPLETE_DAILY_CHALLENGE_TITLE", "COMPLETE_DAILY_CHALLENGE_BODY"),
			new Content("DIALOG_CONGRATS", string.Empty),
			new Content("DIALOG_CONGRATS", string.Empty),
			new Content("WATCH_VIDEO_TITLE", "WATCH_VIDEO_BODY"),
			new Content("COMPLETE_TUTORIAL_TITLE", "COMPLETE_TUTORIAL_BODY"),
			new Content("GLOBAL_CHALLENGE_MILESTONE_TITLE", "GLOBAL_CHALLENGE_MILESTONE_BODY"),
			new Content("GLOBAL_CHALLENGE_COMPLETE_TITLE", "GLOBAL_CHALLENGE_COMPLETE_BODY"),
			new Content("DIALOG_CONGRATS", string.Empty),
			new Content("DIALOG_CONGRATS", string.Empty),
			new Content("DIALOG_WHEEL_NORMAL_PRIZE_TITLE", "DIALOG_WHEEL_NORMAL_PRIZE_BODY"),
			new Content("GOOGLEPLUS_LOGIN_COMPLETE_TITLE", "GOOGLEPLUS_LOGIN_COMPLETE_BODY"),
			new Content("DIALOG_CONGRATS", "REWARD_REASON_GRANTOOWINNER")
		};
	}

	public static Content GetContent(Reason reasonType)
	{
		return s_dialogContent[(int)reasonType];
	}

	public static void Display(Reason reason, StoreContent.StoreEntry entry, int quantity)
	{
		GuiTrigger guiTrigger = DialogStack.ShowDialog("Rewards Dialog");
		Dialog_PlayerReward componentInChildren = Utils.GetComponentInChildren<Dialog_PlayerReward>(guiTrigger.Trigger.gameObject);
		componentInChildren.SetContent(reason, entry, quantity);
	}
}
