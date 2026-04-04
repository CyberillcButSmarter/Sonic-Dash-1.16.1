using UnityEngine;

public class StarRingsRewards : MonoBehaviour
{
	public enum Reason
	{
		Bragging = 0,
		Returning = 1,
		FirstLeaderboard = 2,
		HighScore = 3
	}

	public static bool Reward(Reason reason)
	{
		bool result = false;
		int amount = 1;
		PlayerStats.Stats currentStats = PlayerStats.GetCurrentStats();
		switch (reason)
		{
		case Reason.Bragging:
			if (ABTesting.GetTestValueAsInt(ABTesting.Tests.RSR_Brag) > 0 && currentStats.m_trackedStats[78] == 1)
			{
				Dialog_StarRingReward.Display(Reason.Bragging);
				PlayerStats.IncreaseStat(PlayerStats.StatNames.StarRingsEarned_Total, 1);
				EventDispatch.GenerateEvent("OnStarRingsAwarded", new GameAnalytics.RSRAnalyticsParam(amount, GameAnalytics.RingsRecievedReason.Brag));
				result = true;
			}
			break;
		case Reason.Returning:
			if (ABTesting.GetTestValueAsInt(ABTesting.Tests.RSR_Return) > 0 && currentStats.m_trackedStats[80] == 2)
			{
				Dialog_StarRingReward.Display(Reason.Returning);
				PlayerStats.IncreaseStat(PlayerStats.StatNames.StarRingsEarned_Total, 1);
				EventDispatch.GenerateEvent("OnStarRingsAwarded", new GameAnalytics.RSRAnalyticsParam(amount, GameAnalytics.RingsRecievedReason.Return));
				result = true;
			}
			break;
		case Reason.FirstLeaderboard:
			if (ABTesting.GetTestValueAsInt(ABTesting.Tests.RSR_Leader) > 0 && currentStats.m_trackedStats[79] == 0)
			{
				currentStats.m_trackedStats[79] = 1;
				Dialog_StarRingReward.Display(Reason.FirstLeaderboard);
				PlayerStats.IncreaseStat(PlayerStats.StatNames.StarRingsEarned_Total, 1);
				EventDispatch.GenerateEvent("OnStarRingsAwarded", new GameAnalytics.RSRAnalyticsParam(amount, GameAnalytics.RingsRecievedReason.Leaderboard));
				result = true;
			}
			break;
		case Reason.HighScore:
			if (ABTesting.GetTestValueAsInt(ABTesting.Tests.RSR_Highscore) > 0 && currentStats.m_trackedStats[81] == 0)
			{
				currentStats.m_trackedStats[81] = 1;
				Dialog_StarRingReward.Display(Reason.HighScore);
				PlayerStats.IncreaseStat(PlayerStats.StatNames.StarRingsEarned_Total, 1);
				EventDispatch.GenerateEvent("OnStarRingsAwarded", new GameAnalytics.RSRAnalyticsParam(amount, GameAnalytics.RingsRecievedReason.Highscore));
				result = true;
			}
			break;
		}
		return result;
	}
}
