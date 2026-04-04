using System;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Achievements
{
	public enum Types
	{
		SonicRampage = 0,
		RingHoarder = 1,
		PowerOverload = 2,
		KnuclesOnTheMove = 3,
		CaChing = 4,
		SkyIsTheLimit = 5,
		Ringmaster = 6,
		SuperSonic = 7,
		MissionMaster = 8,
		OnARoll = 9,
		ActionPacked = 10,
		HotHeels = 11,
		SEGAMember = 12,
		EasyTarget = 13,
		WarmUp = 14,
		GoldFish = 15,
		ItAintOver = 16,
		Springtime = 17,
		ThatWasCool = 18,
		Streaker = 19,
		KillTheLight = 20,
		ChasingShadows = 21,
		RingOfFire = 22,
		Trailblazer = 23,
		RunningMiles = 24,
		MilesPerHour = 25,
		HelpingHand = 26,
		FullyLoaded = 27,
		MarathonMan = 28,
		OnlyForTheBrave = 29,
		ItsNoSprint = 30,
		Adventurer = 31,
		BeachBum = 32,
		Alchemy = 33,
		SilverStreak = 34,
		BattingStreak = 35,
		BlingSpring = 36,
		CreamOfTheChop = 37,
		BoostedBunny = 38,
		EspioReady = 39,
		KomboChamelion = 40
	}

	[Flags]
	private enum State
	{
		None = 0,
		PendingRequest = 1,
		LoadingAchievements = 2
	}

	public static string[] Names = new string[41]
	{
		"CgkI-9_QnpoeEAIQAQ", "CgkI-9_QnpoeEAIQAg", "CgkI-9_QnpoeEAIQAw", "CgkI-9_QnpoeEAIQBA", "CgkI-9_QnpoeEAIQBQ", "CgkI-9_QnpoeEAIQBg", "CgkI-9_QnpoeEAIQBw", "CgkI-9_QnpoeEAIQCA", "CgkI-9_QnpoeEAIQCQ", "CgkI-9_QnpoeEAIQCg",
		"CgkI-9_QnpoeEAIQCw", "CgkI-9_QnpoeEAIQDA", "CgkI-9_QnpoeEAIQDQ", "CgkI-9_QnpoeEAIQDg", "CgkI-9_QnpoeEAIQDw", "CgkI-9_QnpoeEAIQEg", "CgkI-9_QnpoeEAIQEw", "CgkI-9_QnpoeEAIQFA", "CgkI-9_QnpoeEAIQFQ", "CgkI-9_QnpoeEAIQFg",
		"CgkI-9_QnpoeEAIQFw", "CgkI-9_QnpoeEAIQGA", "CgkI-9_QnpoeEAIQGQ", "CgkI-9_QnpoeEAIQGg", "CgkI-9_QnpoeEAIQHg", "CgkI-9_QnpoeEAIQHw", "CgkI-9_QnpoeEAIQIA", "CgkI-9_QnpoeEAIQIQ", "CgkI-9_QnpoeEAIQGw", "CgkI-9_QnpoeEAIQHA",
		"CgkI-9_QnpoeEAIQHQ", "CgkI-9_QnpoeEAIQIg", "CgkI-9_QnpoeEAIQIw", "CgkI-9_QnpoeEAIQJA", "CgkI-9_QnpoeEAIQJQ", "CgkI-9_QnpoeEAIQJg", "CgkI-9_QnpoeEAIQJw", "CgkI-9_QnpoeEAIQKA", "CgkI-9_QnpoeEAIQKQ", "sdAchievement40",
		"sdAchievement41"
	};

	private static Achievements s_singleton;

	private IAchievement[] m_achievements;

	private State m_state;

	public Achievements()
	{
		CreateAchievementInstances();
		RegisterForEvents();
		s_singleton = this;
	}

	public void LoadAchievements()
	{
		if ((m_state & State.LoadingAchievements) == State.LoadingAchievements)
		{
			m_state |= State.PendingRequest;
			return;
		}
		m_state |= State.LoadingAchievements;
		Social.LoadAchievements(delegate(IAchievement[] result)
		{
			OnAchievementsLoaded(result);
		});
	}

	public static void AwardAchievement(Types achievement, float progress, ref AchievementTracker.Achievement currentAchievement)
	{
		if (!Social.localUser.authenticated || (currentAchievement.m_state &= AchievementTracker.Achievement.State.ValidOnPlatform) != AchievementTracker.Achievement.State.ValidOnPlatform)
		{
			return;
		}
		IAchievement achievement2 = s_singleton.FindAchievement(achievement, s_singleton.m_achievements);
		if (achievement2.percentCompleted == 100.0 || achievement2.completed || achievement2.percentCompleted >= (double)progress)
		{
			return;
		}
		achievement2.percentCompleted = progress;
		bool completed = false;
		achievement2.ReportProgress(delegate(bool result)
		{
			if (result && progress >= 100f)
			{
				completed = true;
			}
		});
		if (completed)
		{
			currentAchievement.m_state |= AchievementTracker.Achievement.State.Completed;
		}
	}

	private void CreateAchievementInstances()
	{
		int enumCount = Utils.GetEnumCount<Types>();
		m_achievements = new IAchievement[enumCount];
		for (int i = 0; i < enumCount; i++)
		{
			string id = Names[i];
			m_achievements[i] = Social.CreateAchievement();
			m_achievements[i].id = id;
			m_achievements[i].percentCompleted = 0.0;
		}
	}

	private void OnAchievementsLoaded(IAchievement[] currentAchievements)
	{
		for (int i = 0; i < m_achievements.Length; i++)
		{
			IAchievement achievement = FindAchievement((Types)i, currentAchievements);
			if (achievement != null)
			{
				m_achievements[i] = achievement;
			}
		}
		m_state &= ~State.LoadingAchievements;
		if ((m_state & State.PendingRequest) == State.PendingRequest)
		{
			m_state &= ~State.PendingRequest;
			LoadAchievements();
		}
	}

	public void Event_OnAndroidAchievementsFetched()
	{
		int enumCount = Utils.GetEnumCount<Types>();
		for (int i = 0; i < enumCount; i++)
		{
			m_achievements[i].percentCompleted = HLSocialPluginAndroid.GetAchievementProgress(m_achievements[i].id);
		}
		m_state &= ~State.LoadingAchievements;
		if ((m_state & State.PendingRequest) == State.PendingRequest)
		{
			m_state &= ~State.PendingRequest;
			LoadAchievements();
		}
	}

	private void RegisterForEvents()
	{
		EventDispatch.RegisterInterest("RequestAchievementDisplay", this, EventDispatch.Priority.Highest);
		EventDispatch.RegisterInterest("OnAndroidAchievementsFetched", this);
	}

	private IAchievement FindAchievement(Types achievement, IAchievement[] achievementList)
	{
		if (achievementList == null)
		{
			return null;
		}
		string text = Names[(int)achievement];
		foreach (IAchievement achievement2 in achievementList)
		{
			if (achievement2.id == text)
			{
				return achievement2;
			}
		}
		return null;
	}

	private void Event_RequestAchievementDisplay()
	{
		Social.ShowAchievementsUI();
	}
}
