using System;
using UnityEngine;

public class VideoRecorder : MonoBehaviour
{
	private enum TitleTypes
	{
		All = 0,
		Multiplier = 1,
		Character = 2
	}

	[Flags]
	private enum State
	{
		Recording = 1,
		Paused = 2,
		Enabled = 4,
		Supported = 8
	}

	[Flags]
	private enum PauseReasons
	{
		AppPaused = 1,
		GamePaused = 2
	}

	private readonly string[] TitleTypeStrings = new string[3] { "VIDEO_RECORDING_TITLE_ALL_INFO", "VIDEO_RECORDING_TITLE_MULTIPLIER_INFO", "VIDEO_RECORDING_TITLE_CHARACTER_INFO" };

	private readonly string YouTubeDescription = "VIDEO_RECORDING_YOUTUBE_DESCRIPTION";

	private readonly string YouTubeTags = "VIDEO_RECORDING_YOUTUBE_TAGS";

	private readonly string FaceBookDescription = "VIDEO_RECORDING_FACEBOOK_DESCRIPTION";

	private readonly string TwitterDescription = "VIDEO_RECORDING_TWITTER_DESCRIPTION";

	private readonly string TwitterTweet = "VIDEO_RECORDING_TWITTER_TWEET";

	private readonly string EmailSubject = "VIDEO_RECORDING_EMAIL_SUBJECT";

	private readonly string EmailBody = "VIDEO_RECORDING_EMAIL_BODY";

	private readonly string AppleAppLink = "http://itunes.apple.com/us/app/id582654048";

	private readonly string GoogleAppLink = "http://play.google.com/store/apps/details?id=com.sega.sonicdash";

	private readonly string VideoRecorderEnabledProperty = "VideoRecorderActive";

	private static VideoRecorder s_recorder;

	private State m_state;

	private PauseReasons m_pauseReasons;

	private Kamcord m_kamcord;

	private int m_revivesUsed;

	public static bool Enabled
	{
		get
		{
			return (s_recorder.m_state & State.Enabled) != 0;
		}
		set
		{
			if (value)
			{
				s_recorder.m_state |= State.Enabled;
			}
			else
			{
				s_recorder.m_state &= ~State.Enabled;
			}
		}
	}

	public static bool Supported
	{
		get
		{
			return (s_recorder.m_state & State.Supported) != 0;
		}
	}

	public static bool Useable
	{
		get
		{
			return Supported && Enabled;
		}
	}

	private void Start()
	{
		s_recorder = this;
		EventDispatch.RegisterInterest("OnNewGameStarted", this);
		EventDispatch.RegisterInterest("OnGameFinished", this, EventDispatch.Priority.Lowest);
		EventDispatch.RegisterInterest("ShareVideoRequest", this);
		EventDispatch.RegisterInterest("DisableGameState", this);
		EventDispatch.RegisterInterest("StartGameState", this);
		EventDispatch.RegisterInterest("PauseModeEnabled", this);
		EventDispatch.RegisterInterest("OnContinueGameOk", this);
		EventDispatch.RegisterInterest("OnGameDataSaveRequest", this);
		EventDispatch.RegisterInterest("OnGameDataLoaded", this);
		EventDispatch.RegisterInterest("FeatureStateReady", this);
		m_kamcord = UnityEngine.Object.FindObjectOfType(typeof(Kamcord)) as Kamcord;
		m_state |= State.Enabled;
		if (!IsDeviceBlacklisted())
		{
			m_state |= State.Supported;
		}
		Kamcord.SetMaximumVideoLength(1800u);
		Kamcord.SetFacebookAppID(HLSocialPlatform.FacebookAppID.ToString());
	}

	public static void LogoutOfSharedFacebookAuth()
	{
		Kamcord.LogoutOfSharedFacebookAuth();
		Debug.Log("**AZ** Kamcord.LogoutOfSharedFacebookAuth() has been called ");
	}

	private void StartRecording()
	{
		if (Useable)
		{
			Kamcord.StartRecording();
			m_state |= State.Recording;
		}
	}

	private void EndRecording()
	{
		if (Useable && (m_state & State.Recording) != 0)
		{
			PauseRecording(false);
			SetVideoProperties(TitleTypes.All);
			Kamcord.StopRecording();
			m_state &= ~State.Recording;
		}
	}

	private void PauseRecording(bool pause)
	{
		if (!Useable || (m_state & State.Recording) == 0)
		{
			return;
		}
		bool flag = (m_state & State.Paused) != 0;
		if (pause != flag)
		{
			if (pause)
			{
				Kamcord.Pause();
				m_state |= State.Paused;
			}
			else
			{
				Kamcord.Resume();
				m_state &= ~State.Paused;
			}
		}
	}

	private void ShareRecording()
	{
		if (Useable && Useable && (m_state & State.Recording) == 0)
		{
			Dialog_VideoRecorderPrompt.LastRunShown = PlayerStats.GetStat(PlayerStats.StatNames.NumberOfRuns_Total);
			Kamcord.ShowView();
		}
	}

	private void UpdatePauseState()
	{
		bool flag = (m_state & State.Paused) != 0;
		bool flag2 = m_pauseReasons != (PauseReasons)0;
		if ((m_state & State.Recording) != 0)
		{
			if (!flag && flag2)
			{
				PauseRecording(true);
			}
			else
			{
				PauseRecording(false);
			}
		}
	}

	private bool IsDeviceBlacklisted()
	{
		return true;
	}

	private bool IsEnabledByDefault()
	{
		if (IsDeviceBlacklisted())
		{
			return false;
		}
		return false;
	}

	private void SetVideoProperties(TitleTypes titleType)
	{
		uint baseMultiplier = ScoreTracker.BaseMultiplier;
		long currentScore = ScoreTracker.CurrentScore;
		float distanceTravelled = Sonic.Tracker.DistanceTravelled;
		Characters.Type currentCharacter = CharacterManager.Singleton.GetCurrentCharacter();
		string characterName = CharacterManager.Singleton.GetCharacterName(currentCharacter);
		string videoTitle = string.Empty;
		string format = LanguageStrings.First.GetString(TitleTypeStrings[(int)titleType]);
		string text = LanguageUtils.FormatNumber(currentScore);
		switch (titleType)
		{
		case TitleTypes.All:
			videoTitle = string.Format(format, text, characterName, baseMultiplier);
			break;
		case TitleTypes.Multiplier:
			videoTitle = string.Format(format, text, baseMultiplier);
			break;
		case TitleTypes.Character:
			videoTitle = string.Format(format, text, characterName);
			break;
		}
		Kamcord.SetVideoTitle(videoTitle);
		Kamcord.SetDeveloperMetadataWithNumericValue(Kamcord.MetadataType.score, "Score", LanguageUtils.FormatNumber(currentScore), currentScore);
		Kamcord.SetDeveloperMetadataWithNumericValue(Kamcord.MetadataType.other, "Distance", distanceTravelled.ToString(), distanceTravelled);
		Kamcord.SetDeveloperMetadataWithNumericValue(Kamcord.MetadataType.other, "Multiplier", baseMultiplier.ToString(), baseMultiplier);
		Kamcord.SetDeveloperMetadataWithNumericValue(Kamcord.MetadataType.other, "Revives Used", m_revivesUsed.ToString(), m_revivesUsed);
		Kamcord.SetDeveloperMetadataWithNumericValue(Kamcord.MetadataType.other, "Rings Collected", RingStorage.RunBankedRings.ToString(), RingStorage.RunBankedRings);
		Kamcord.SetDeveloperMetadataWithNumericValue(Kamcord.MetadataType.other, "Stars Collected", RingStorage.RunStarRings.ToString(), RingStorage.RunStarRings);
		Kamcord.SetDeveloperMetadata(Kamcord.MetadataType.other, "Character", characterName);
		string text2 = string.Empty;
		if (Boosters.IsBoosterSelected(PowerUps.Type.Booster_SpringBonus))
		{
			text2 += "'Spring Bonus'";
		}
		if (Boosters.IsBoosterSelected(PowerUps.Type.Booster_EnemyComboBonus))
		{
			text2 += "'Enemy Combo'";
		}
		if (Boosters.IsBoosterSelected(PowerUps.Type.Booster_RingStreakBonus))
		{
			text2 += "'Ring Streak'";
		}
		if (Boosters.IsBoosterSelected(PowerUps.Type.Booster_ScoreMultiplier))
		{
			text2 += "'Score Multiplier'";
		}
		if (Boosters.IsBoosterSelected(PowerUps.Type.Booster_GoldenEnemy))
		{
			text2 += "'Golden Enemy'";
		}
		if (!string.IsNullOrEmpty(text2))
		{
			text2 = text2.Replace("''", ", ");
			text2 = text2.Replace("'", string.Empty);
			Kamcord.SetDeveloperMetadata(Kamcord.MetadataType.list, "Boosters", text2);
			Debug.Log(text2);
		}
		string description = string.Format(LanguageStrings.First.GetString(YouTubeDescription), text, characterName, baseMultiplier, AppleAppLink, GoogleAppLink);
		string tags = string.Format(LanguageStrings.First.GetString(YouTubeTags), characterName);
		Kamcord.SetYouTubeSettings(description, tags);
		string facebookDescription = string.Format(LanguageStrings.First.GetString(FaceBookDescription), text, characterName);
		Kamcord.SetFacebookDescription(facebookDescription);
		string twitterDescription = string.Format(LanguageStrings.First.GetString(TwitterDescription), text, characterName, baseMultiplier, AppleAppLink, GoogleAppLink);
		string defaultTweet = string.Format(LanguageStrings.First.GetString(TwitterTweet), text, characterName);
		Kamcord.SetTwitterDescription(twitterDescription);
		Kamcord.SetDefaultTweet(defaultTweet);
		string defaultEmailSubject = string.Format(LanguageStrings.First.GetString(EmailSubject), text, characterName);
		string defaultEmailBody = string.Format(LanguageStrings.First.GetString(EmailBody), text, characterName, "$(KAMCORD_VIDEO_LINK)", AppleAppLink, GoogleAppLink);
		Kamcord.SetDefaultEmailSubject(defaultEmailSubject);
		Kamcord.SetDefaultEmailBody(defaultEmailBody);
	}

	private void OnApplicationPause(bool paused)
	{
		if (paused)
		{
			m_pauseReasons |= PauseReasons.AppPaused;
			m_pauseReasons |= PauseReasons.GamePaused;
		}
		else
		{
			m_pauseReasons &= ~PauseReasons.AppPaused;
		}
		UpdatePauseState();
	}

	private void Event_OnNewGameStarted()
	{
		m_revivesUsed = 0;
		StartRecording();
	}

	private void Event_OnGameFinished()
	{
		EndRecording();
	}

	private void Event_ShareVideoRequest()
	{
		ShareRecording();
	}

	private void Event_DisableGameState(GameState.Mode nextState)
	{
		EndRecording();
	}

	private void Event_StartGameState(GameState.Mode startState)
	{
		if (startState == GameState.Mode.Game)
		{
			m_pauseReasons &= ~PauseReasons.GamePaused;
			UpdatePauseState();
		}
	}

	private void Event_PauseModeEnabled()
	{
		m_pauseReasons |= PauseReasons.GamePaused;
		UpdatePauseState();
	}

	private void Event_OnContinueGameOk(bool freeRevive)
	{
		m_revivesUsed++;
	}

	private void Event_OnGameDataSaveRequest()
	{
		PropertyStore.Store(VideoRecorderEnabledProperty, Enabled);
	}

	private void Event_OnGameDataLoaded(ActiveProperties activeProperties)
	{
		if (!activeProperties.DoesPropertyExist(VideoRecorderEnabledProperty))
		{
			Enabled = IsEnabledByDefault();
		}
		else
		{
			Enabled = activeProperties.GetBool(VideoRecorderEnabledProperty);
		}
	}

	private void Event_FeatureStateReady()
	{
		LSON.Property stateProperty = FeatureState.GetStateProperty("videorecorder", "blacklist_ipad1");
		bool boolValue;
		if (LSONProperties.AsBool(stateProperty, out boolValue))
		{
			m_kamcord.deviceBlacklist.ipad1 = boolValue;
		}
		stateProperty = FeatureState.GetStateProperty("videorecorder", "blacklist_ipad2");
		if (LSONProperties.AsBool(stateProperty, out boolValue))
		{
			m_kamcord.deviceBlacklist.ipad2 = boolValue;
		}
		stateProperty = FeatureState.GetStateProperty("videorecorder", "blacklist_ipad3");
		if (LSONProperties.AsBool(stateProperty, out boolValue))
		{
			m_kamcord.deviceBlacklist.ipad3 = boolValue;
		}
		stateProperty = FeatureState.GetStateProperty("videorecorder", "blacklist_ipadMini");
		if (LSONProperties.AsBool(stateProperty, out boolValue))
		{
			m_kamcord.deviceBlacklist.ipadMini = boolValue;
		}
		stateProperty = FeatureState.GetStateProperty("videorecorder", "blacklist_ipod4Gen");
		if (LSONProperties.AsBool(stateProperty, out boolValue))
		{
			m_kamcord.deviceBlacklist.ipod4Gen = boolValue;
		}
		stateProperty = FeatureState.GetStateProperty("videorecorder", "blacklist_ipod5Gen");
		if (LSONProperties.AsBool(stateProperty, out boolValue))
		{
			m_kamcord.deviceBlacklist.ipod5Gen = boolValue;
		}
		stateProperty = FeatureState.GetStateProperty("videorecorder", "blacklist_iphone3GS");
		if (LSONProperties.AsBool(stateProperty, out boolValue))
		{
			m_kamcord.deviceBlacklist.iphone3GS = boolValue;
		}
		stateProperty = FeatureState.GetStateProperty("videorecorder", "blacklist_iphone4");
		if (LSONProperties.AsBool(stateProperty, out boolValue))
		{
			m_kamcord.deviceBlacklist.iphone4 = boolValue;
		}
		stateProperty = FeatureState.GetStateProperty("videorecorder", "blacklist_iphone4S");
		if (LSONProperties.AsBool(stateProperty, out boolValue))
		{
			m_kamcord.deviceBlacklist.iphone4S = boolValue;
		}
		stateProperty = FeatureState.GetStateProperty("videorecorder", "forceDisable");
		bool flag = LSONProperties.AsBool(stateProperty, out boolValue);
		if (flag && boolValue)
		{
			m_state &= ~State.Supported;
		}
	}
}
