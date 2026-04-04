using System;
using System.Runtime.InteropServices;
using UnityEngine;

[AddComponentMenu("Dash/Sonic/Handling")]
public class SonicHandling : MonoBehaviour
{
	[StructLayout((LayoutKind)0, Size = 1)]
	public struct RollStats
	{
		public float Duration { get; private set; }

		public float StartVelocity { get; private set; }

		public float SpinUpDuration { get; private set; }

		public float SpinUpVelocity { get; private set; }

		public float SpinVelocity { get; private set; }

		public float DistanceCovered { get; private set; }

		public RollStats(float duration, float startVelocity, float spinUpDuration, float spinUpVelocity, float spinVelocity, float distanceCovered)
		{
			Duration = duration;
			StartVelocity = startVelocity;
			SpinUpDuration = spinUpDuration;
			SpinUpVelocity = spinUpVelocity;
			SpinVelocity = spinVelocity;
			DistanceCovered = distanceCovered;
		}
	}

	private bool m_preRunInProgress;

	[SerializeField]
	private float m_startSpeed = 30f;

	[SerializeField]
	private float m_preRunSpeed = 15f;

	[SerializeField]
	private float m_speedSmoothing = 3f;

	[SerializeField]
	private float m_preRunSmoothing = 5f;

	[SerializeField]
	private float m_gameplayAccelerationPerMinute = 3f;

	[SerializeField]
	private float m_rewindSpeedMofier = 4f;

	[SerializeField]
	private float m_strafeDuration = 1f;

	[SerializeField]
	private float m_doubleStrafeCutoffTime = 0.6f;

	[SerializeField]
	private float m_jumpDuration = 10f;

	[SerializeField]
	private float m_jumpHeight = 10f;

	[SerializeField]
	private float m_airStrafeDurationMultiplier = 0.5f;

	[SerializeField]
	private float m_airStrafeSmoothness = 5f;

	[SerializeField]
	private float m_attackSpeed = 30f;

	[SerializeField]
	private bool m_attackAccelerationEnabled;

	[SerializeField]
	private float m_attackAcceleration = 1f;

	[SerializeField]
	private float m_postAttackDuration = 1f;

	[SerializeField]
	private float m_diveSpeed = 20f;

	[SerializeField]
	private float m_timeSpeedChangeDuration = 0.3f;

	[SerializeField]
	private float m_slowTimeFactor = 0.33f;

	[SerializeField]
	private bool m_instantDashSpeedGain;

	[SerializeField]
	private float m_timeToDashMaxSpeed = 0.3f;

	[SerializeField]
	private bool m_instantDashSpeedLoss;

	[SerializeField]
	private float m_timeToDashNormalSpeed = 0.3f;

	[SerializeField]
	private float m_dashSpeedMultiplier = 2f;

	[SerializeField]
	private float m_headstartSpeedMultiplier = 2f;

	[SerializeField]
	private float m_superHeadstartSpeedMultiplier = 2f;

	[SerializeField]
	private float m_setPieceDashPadSpeedMultiplier = 2f;

	[SerializeField]
	private float m_splatRestitution = 0.5f;

	[SerializeField]
	private float m_safeUnrollTimeWindow = 0.5f;

	[SerializeField]
	private float m_rollDuration = 4f;

	[SerializeField]
	private float m_rollBoostSpeedModifier = 1.5f;

	[SerializeField]
	private float m_rollBoostTimeIncrementPercent = 0.5f;

	[SerializeField]
	private float m_rollTimeToAllowBoosts = 1f;

	[SerializeField]
	private AnimationCurve m_rollSpeedCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private int m_rollMaxNumBoosts = 3;

	[SerializeField]
	private float m_rollCoolDownDuration = 1f;

	[SerializeField]
	private float m_rollDurationProportionSpinningUp = 0.4f;

	[SerializeField]
	private AnimationCurve m_springHeight;

	[SerializeField]
	private float m_springForwardSpeed = 50f;

	[SerializeField]
	private float m_postDeathPoseDuration = 1f;

	[SerializeField]
	private float m_stumbleDuration = 3f;

	[SerializeField]
	private float m_stumblePenetrationDistance = 0.1f;

	[SerializeField]
	private AnimationCurve m_springGestureWindow;

	[SerializeField]
	private float m_postGestureDelay = 1.5f;

	[SerializeField]
	private int m_gesturesPerSpring = 3;

	[SerializeField]
	private float m_invulnerableDuration = 1f;

	public float StartSpeed
	{
		get
		{
			return (!m_preRunInProgress) ? m_startSpeed : m_preRunSpeed;
		}
	}

	public float SpeedSmoothing
	{
		get
		{
			return (!m_preRunInProgress) ? m_speedSmoothing : m_preRunSmoothing;
		}
	}

	public float GameplayAccelerationPerMinute
	{
		get
		{
			return m_gameplayAccelerationPerMinute;
		}
	}

	public float RewindSpeedModifier
	{
		get
		{
			return m_rewindSpeedMofier;
		}
	}

	public float GameplayAcceleration
	{
		get
		{
			return GameplayAccelerationPerMinute / 60f;
		}
	}

	public float StrafeDuration
	{
		get
		{
			return m_strafeDuration;
		}
	}

	public float DoubleStrafeCutoffTime
	{
		get
		{
			return m_doubleStrafeCutoffTime;
		}
	}

	public float JumpDuration
	{
		get
		{
			return m_jumpDuration;
		}
		set
		{
			m_jumpDuration = value;
		}
	}

	public float JumpHeight
	{
		get
		{
			return m_jumpHeight;
		}
		set
		{
			m_jumpHeight = value;
		}
	}

	public float AirStrafeDurationMultiplier
	{
		get
		{
			return m_airStrafeDurationMultiplier;
		}
	}

	public float AirStrafeSmoothness
	{
		get
		{
			return m_airStrafeSmoothness;
		}
	}

	public float AttackSpeed
	{
		get
		{
			return m_attackSpeed;
		}
	}

	public bool AttackAccelerationEnabled
	{
		get
		{
			return m_attackAccelerationEnabled;
		}
	}

	public float AttackAcceleration
	{
		get
		{
			return m_attackAcceleration;
		}
	}

	public float PostAttackDuration
	{
		get
		{
			return m_postAttackDuration;
		}
	}

	public float DiveSpeed
	{
		get
		{
			return m_diveSpeed;
		}
	}

	public float TargetTimeSpeedChangeDuration
	{
		get
		{
			return m_timeSpeedChangeDuration;
		}
	}

	public float TargetSlowTimeFactor
	{
		get
		{
			return m_slowTimeFactor;
		}
	}

	public bool InstantDashSpeedGain
	{
		get
		{
			return m_instantDashSpeedGain;
		}
	}

	public float TimeToDashMaxSpeed
	{
		get
		{
			return m_timeToDashMaxSpeed;
		}
	}

	public bool InstantDashSpeedLoss
	{
		get
		{
			return m_instantDashSpeedLoss;
		}
	}

	public float TimeToDashNormalSpeed
	{
		get
		{
			return m_timeToDashNormalSpeed;
		}
	}

	public float DashSpeedMultiplier
	{
		get
		{
			return m_dashSpeedMultiplier;
		}
	}

	public float HeadstartSpeedMultiplier
	{
		get
		{
			return m_headstartSpeedMultiplier;
		}
	}

	public float SuperHeadstartSpeedMultiplier
	{
		get
		{
			return m_superHeadstartSpeedMultiplier;
		}
	}

	public float SetPieceDashPadSpeedMultiplier
	{
		get
		{
			return m_setPieceDashPadSpeedMultiplier;
		}
	}

	public float SplatRestitution
	{
		get
		{
			return m_splatRestitution;
		}
		set
		{
			m_splatRestitution = value;
		}
	}

	public float SafeUnrollTimeWindow
	{
		get
		{
			return m_safeUnrollTimeWindow;
		}
	}

	public float RollDuration
	{
		get
		{
			return m_rollDuration;
		}
		set
		{
			m_rollDuration = value;
		}
	}

	public float RollBoostSpeedModifier
	{
		get
		{
			return m_rollBoostSpeedModifier;
		}
		set
		{
			m_rollBoostSpeedModifier = value;
		}
	}

	public float RollBoostTimeIncrementPercent
	{
		get
		{
			return m_rollBoostTimeIncrementPercent;
		}
		set
		{
			m_rollBoostTimeIncrementPercent = value;
		}
	}

	public float RollTimeToAllowBoosts
	{
		get
		{
			return m_rollTimeToAllowBoosts;
		}
		set
		{
			m_rollTimeToAllowBoosts = value;
		}
	}

	public AnimationCurve RollSpeedCurve
	{
		get
		{
			return m_rollSpeedCurve;
		}
		set
		{
			m_rollSpeedCurve = value;
		}
	}

	public int RollMaxNumBoosts
	{
		get
		{
			return m_rollMaxNumBoosts;
		}
		set
		{
			m_rollMaxNumBoosts = value;
		}
	}

	public float RollCoolDownDuration
	{
		get
		{
			return m_rollCoolDownDuration;
		}
	}

	public float RollDurationProportionSpinningUp
	{
		get
		{
			return m_rollDurationProportionSpinningUp;
		}
		set
		{
			m_rollDurationProportionSpinningUp = value;
		}
	}

	public float SpringForwardSpeed
	{
		get
		{
			return m_springForwardSpeed;
		}
	}

	public float PostDeathPoseDuration
	{
		get
		{
			return m_postDeathPoseDuration;
		}
		set
		{
			m_postDeathPoseDuration = value;
		}
	}

	public float StumbleDuration
	{
		get
		{
			return m_stumbleDuration;
		}
	}

	public float StumblePenetrationDistance
	{
		get
		{
			return m_stumblePenetrationDistance;
		}
	}

	public AnimationCurve SpringGestureWindow
	{
		get
		{
			return m_springGestureWindow;
		}
	}

	public float PostGestureDelay
	{
		get
		{
			return m_postGestureDelay;
		}
	}

	public int GesturesPerSpring
	{
		get
		{
			return m_gesturesPerSpring;
		}
	}

	public float InvulnerableDuration
	{
		get
		{
			return m_invulnerableDuration;
		}
	}

	private void Awake()
	{
		Sonic.Initialise();
		EventDispatch.RegisterInterest("OnNewGameStarted", this);
		EventDispatch.RegisterInterest("OnNewGameHudShown", this);
	}

	private void Event_OnNewGameStarted()
	{
		m_preRunInProgress = true;
	}

	private void Event_OnNewGameHudShown()
	{
		m_preRunInProgress = false;
	}

	public float CalculateVelocityAt(float time)
	{
		return StartSpeed + time * GameplayAcceleration;
	}

	public float CalculatePositionDelta(float windowStartTime, float windowDuration)
	{
		Func<float, float> func = (float time) => StartSpeed * time + 0.5f * GameplayAcceleration * time * time;
		return func(windowStartTime + windowDuration) - func(windowStartTime);
	}

	public float GetJumpLength(float sonicSpeed)
	{
		return m_jumpDuration * sonicSpeed;
	}

	public JumpCurve CreateJumpFrom(float yPos)
	{
		return new JumpCurve(yPos, JumpHeight, JumpDuration);
	}

	public float EvaluateRollSpeed(float fPercentageTime)
	{
		return m_rollSpeedCurve.Evaluate(fPercentageTime);
	}

	public JumpAnimationCurve GetNewSpringJumpCurve(float heightAboveLowGround)
	{
		Keyframe key = new Keyframe(0f, heightAboveLowGround);
		key.outTangent = m_springHeight.keys[0].outTangent;
		m_springHeight.MoveKey(0, key);
		return new JumpAnimationCurve(0f, m_springHeight);
	}

	public RollStats CalculateRollStats(float currentGameTime)
	{
		float num = RollDuration * RollDurationProportionSpinningUp;
		float num2 = CalculateVelocityAt(currentGameTime);
		float num3 = CalculatePositionDelta(currentGameTime, RollDuration);
		float num4 = num2 * num;
		float num5 = num3 - num4;
		float spinVelocity = num5 / (RollDuration - num);
		return new RollStats(RollDuration, num2, num, num2, spinVelocity, num3);
	}
}
