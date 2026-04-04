public class HudContent_GCReminder : HudContent_PlayerGoals.Goal
{
	public override void Populate()
	{
	}

	public override void Update()
	{
	}

	public override bool IsValidGoal()
	{
		return GCState.IsCurrentChallengeActive() && !GC6Progress.ChallengeFullycompleted();
	}
}
