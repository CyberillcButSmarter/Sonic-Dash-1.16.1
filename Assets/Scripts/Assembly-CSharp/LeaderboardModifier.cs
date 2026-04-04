using System;
using System.Linq;
using UnityEngine;

public class LeaderboardModifier
{
	private const string PlayerName = "Mr. Test";

	private const int FirstScore = 5000;

	private const int ScoreDifference = 300;

	public static void UpdateLeaderboardData(Leaderboards.Types leaderboard, Leaderboards.Entry[] entries)
	{
		if (!Social.localUser.authenticated)
		{
			return;
		}
		LeaderboardContent.LastPostedData lastPostedData = LeaderboardContent.GetLastPostedData(leaderboard);
		if (!lastPostedData.m_valid)
		{
			return;
		}
		Leaderboards.Entry entry = entries.FirstOrDefault((Leaderboards.Entry thisEntry) => thisEntry != null && thisEntry.m_playersRank);
		if (entry == null || entry.m_user != lastPostedData.m_user || lastPostedData.m_score <= entry.m_score)
		{
			return;
		}
		entry.m_score = lastPostedData.m_score;
		Array.Sort(entries, delegate(Leaderboards.Entry entry3, Leaderboards.Entry entry4)
		{
			if (entry3 == null)
			{
				return int.MaxValue;
			}
			return (entry4 == null) ? int.MinValue : entry4.m_score.CompareTo(entry3.m_score);
		});
		for (int num = 0; num < entries.Length; num++)
		{
			Leaderboards.Entry entry2 = entries[num];
			if (entry2 != null)
			{
				entry2.m_rank = num + 1;
			}
		}
	}
}
