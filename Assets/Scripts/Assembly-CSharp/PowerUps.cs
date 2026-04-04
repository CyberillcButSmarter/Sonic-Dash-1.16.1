public class PowerUps
{
	public enum Type
	{
		Respawn = 0,
		Magnet = 1,
		HeadStart = 2,
		RollBoost = 3,
		IncreasedAttackRange = 4,
		DashLength = 5,
		DashIncrease = 6,
		DoubleRing = 7,
		SuperHeadStart = 8,
		Shield = 9,
		FreeRevive = 10,
		Booster_SpringBonus = 11,
		Booster_EnemyComboBonus = 12,
		Booster_RingStreakBonus = 13,
		Booster_ScoreMultiplier = 14,
		Booster_GoldenEnemy = 15
	}

	public const int NumberOfPowerUps = 16;

	public const int NumberOfUpgradables = 5;

	public const int UpgradeSlots = 7;

	public static int GetRingsForRingsPickup()
	{
		float current = RingPerMinute.Current;
		return (current < 50f) ? 100 : ((current < 100f) ? 50 : ((!(current < 130f)) ? 10 : 20));
	}

	public static void DoRingPowerupAction(int ringCount)
	{
		RingPickupMonitor.instance().PickupRings(ringCount);
		Sonic.RenderManager.playRingPickupParticles(ringCount);
	}

	public static bool CanPowerUpBeCollected(Type powerUp)
	{
		return powerUp != Type.RollBoost && powerUp != Type.DashIncrease && powerUp != Type.DashLength && powerUp != Type.IncreasedAttackRange;
	}

	public static bool CanPowerUpBeUpgraded(Type powerUp)
	{
		return powerUp != Type.DoubleRing && powerUp != Type.FreeRevive && powerUp != Type.Respawn && powerUp != Type.SuperHeadStart && powerUp != Type.Booster_SpringBonus && powerUp != Type.Booster_ScoreMultiplier && powerUp != Type.Booster_GoldenEnemy && powerUp != Type.Booster_EnemyComboBonus && powerUp != Type.Booster_RingStreakBonus;
	}

	public static bool CanPowerUpBeHinted(Type powerUp)
	{
		return powerUp != Type.DoubleRing && powerUp != Type.FreeRevive && powerUp != Type.Respawn && powerUp != Type.SuperHeadStart && powerUp != Type.IncreasedAttackRange && powerUp != Type.RollBoost && powerUp != Type.Booster_SpringBonus && powerUp != Type.Booster_ScoreMultiplier && powerUp != Type.Booster_GoldenEnemy && powerUp != Type.Booster_EnemyComboBonus && powerUp != Type.Booster_RingStreakBonus;
	}
}
