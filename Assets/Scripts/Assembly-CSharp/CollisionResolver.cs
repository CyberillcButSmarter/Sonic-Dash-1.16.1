public abstract class CollisionResolver
{
	public enum ResolutionType
	{
		Nothing = 0,
		SonicDeath = 1,
		EnemyDeath = 2,
		SonicStumble = 3,
		SonicKnockedLeft = 4,
		SonicKnockedRight = 5,
		SonicDieForwards = 6
	}

	public ResolutionType Resolution { get; protected set; }

	protected CollisionResolver(ResolutionType defaultResolution)
	{
		Resolution = defaultResolution;
	}

	public ResolutionType Resolve(MotionState state, bool heldRings, bool ghosted)
	{
		ProcessMotionState(state, heldRings, ghosted);
		return Resolution;
	}

	public virtual void ProcessMotionState(MotionState state, bool heldRings, bool ghosted)
	{
	}
}
