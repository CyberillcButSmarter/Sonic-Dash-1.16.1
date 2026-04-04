using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimationController : MonoBehaviour
{
	private float m_timer;

	private List<float> m_firingTimes = new List<float>();

	private BossVisualController m_visualController;

	private BossAudioController m_audioController;

	private BossMissileChase m_missileController;

	private BossMovementController m_movementController;

	[SerializeField]
	private float m_maximumFiringAnimSpeedMultiplier = 1.5f;

	[SerializeField]
	private Animation m_animationCamera;

	[SerializeField]
	private Animation m_animationAttackCamera;

	[SerializeField]
	private Animation m_animationCharacter;

	[SerializeField]
	private AnimationClip[] m_animations = new AnimationClip[Enum.GetNames(typeof(BossAnim)).Length];

	[SerializeField]
	private AnimationClip[] m_cameraAnimations = new AnimationClip[Enum.GetNames(typeof(BossAnim)).Length];

	[SerializeField]
	private AnimationClip[] m_attackCameraAnimations = new AnimationClip[Enum.GetNames(typeof(BossAnim)).Length];

	public bool BlockIdleAnim { get; set; }

	public BossAnim CurrentPreFiringLongAnimation { get; private set; }

	public BossAnim CurrentPreFiringAnimation { get; private set; }

	public BossAnim CurrentFiringAnimation { get; private set; }

	public BossAnim CurrentPostFiringAnimation { get; private set; }

	private void OnEnable()
	{
		StartCoroutine(ManageFiringAnimations());
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private void Awake()
	{
		m_visualController = GetComponent<BossVisualController>();
		m_audioController = GetComponent<BossAudioController>();
		m_missileController = GetComponent<BossMissileChase>();
		m_movementController = GetComponent<BossMovementController>();
	}

	private IEnumerator ManageFiringAnimations()
	{
		float preFiringRocketAnimLength1 = GetCharacterAnimationLength(BossAnim.PreFireRocket1);
		float preFiringRocketAnimLength2 = GetCharacterAnimationLength(BossAnim.PreFireRocket2);
		float preFiringMineAnimLength = GetCharacterAnimationLength(BossAnim.PreFireMine);
		float preFiringRocketAnimLongLength1 = GetCharacterAnimationLength(BossAnim.PreFireLongRocket1);
		float preFiringRocketAnimLongLength2 = GetCharacterAnimationLength(BossAnim.PreFireLongRocket2);
		float preFiringMineAnimLongLength = GetCharacterAnimationLength(BossAnim.PreFireLongMine);
		float firingRocketAnimLength1 = GetCharacterAnimationLength(BossAnim.FireRocket1);
		float firingRocketAnimLength2 = GetCharacterAnimationLength(BossAnim.FireRocket2);
		float firingMineAnimLength = GetCharacterAnimationLength(BossAnim.FireMine);
		float postFiringRocketAnimLength1 = GetCharacterAnimationLength(BossAnim.PostFireRocket1);
		float postFiringRocketAnimLength2 = GetCharacterAnimationLength(BossAnim.PostFireRocket2);
		float postFiringMineAnimLength = GetCharacterAnimationLength(BossAnim.PostFireMine);
		float fullAnimsToFireAgain = 0f;
		float preFiringCurrentAnimLength = 0f;
		float preFiringCurrentLongAnimLength = 0f;
		float firingCurrentAnimLength = 0f;
		float postFiringCurrentAnimLength = 0f;
		while (true)
		{
			if (m_firingTimes.Count > 0)
			{
				if (BossBattleSystem.Instance().CurrentPhase != null && BossBattleSystem.Instance().CurrentPhase.Type == BossBattleSystem.Phase.Types.Attack1)
				{
					int behaveAs = -1;
					if (m_movementController.CurrentLane == Track.Lane.Middle)
					{
						behaveAs = ((!((double)UnityEngine.Random.value < 0.5)) ? 1 : 0);
					}
					if (m_movementController.CurrentLane == Track.Lane.Left || behaveAs == 0)
					{
						m_missileController.MissileLauncher = 0;
						firingCurrentAnimLength = firingRocketAnimLength1;
						CurrentFiringAnimation = BossAnim.FireRocket1;
						postFiringCurrentAnimLength = postFiringRocketAnimLength1;
						CurrentPostFiringAnimation = BossAnim.PostFireRocket1;
						preFiringCurrentAnimLength = preFiringRocketAnimLength1;
						CurrentPreFiringAnimation = BossAnim.PreFireRocket1;
						preFiringCurrentLongAnimLength = preFiringRocketAnimLongLength1;
						CurrentPreFiringLongAnimation = BossAnim.PreFireLongRocket1;
					}
					else if (m_movementController.CurrentLane == Track.Lane.Right || behaveAs == 1)
					{
						m_missileController.MissileLauncher = 1;
						firingCurrentAnimLength = firingRocketAnimLength2;
						CurrentFiringAnimation = BossAnim.FireRocket2;
						postFiringCurrentAnimLength = postFiringRocketAnimLength2;
						CurrentPostFiringAnimation = BossAnim.PostFireRocket2;
						preFiringCurrentAnimLength = preFiringRocketAnimLength2;
						CurrentPreFiringAnimation = BossAnim.PreFireRocket2;
						preFiringCurrentLongAnimLength = preFiringRocketAnimLongLength2;
						CurrentPreFiringLongAnimation = BossAnim.PreFireLongRocket2;
					}
				}
				else
				{
					firingCurrentAnimLength = firingMineAnimLength;
					CurrentFiringAnimation = BossAnim.FireMine;
					postFiringCurrentAnimLength = postFiringMineAnimLength;
					CurrentPostFiringAnimation = BossAnim.PostFireMine;
					preFiringCurrentAnimLength = preFiringMineAnimLength;
					CurrentPreFiringAnimation = BossAnim.PreFireMine;
					preFiringCurrentLongAnimLength = preFiringMineAnimLongLength;
					CurrentPreFiringLongAnimation = BossAnim.PreFireLongMine;
				}
				BossAnim preAnimToUse = CurrentPreFiringAnimation;
				float preAnimLength = preFiringCurrentAnimLength;
				if (m_firingTimes[0] - preFiringCurrentLongAnimLength > m_timer)
				{
					preAnimToUse = CurrentPreFiringLongAnimation;
					preAnimLength = preFiringCurrentLongAnimLength;
				}
				PlayCharacterAnimation(BossAnim.Idle);
				while (m_firingTimes[0] - preAnimLength > m_timer)
				{
					yield return null;
				}
				float availableTimeForPreFire = m_firingTimes[0] - m_timer;
				float preFirePlaySpeed = Mathf.Max(preAnimLength / availableTimeForPreFire, 1f);
				PlayCharacterAnimation(preAnimToUse, preFirePlaySpeed);
				while (m_firingTimes[0] > m_timer)
				{
					yield return null;
				}
				m_firingTimes.RemoveAt(0);
				if (BossBattleSystem.Instance().CurrentPhase.Type == BossBattleSystem.Phase.Types.Attack1)
				{
					m_visualController.PlaySpawnMissileEffect();
				}
				else
				{
					m_visualController.PlaySpawnMineEffect();
				}
				m_audioController.PlayFiringSFX();
				fullAnimsToFireAgain = preFiringCurrentAnimLength + postFiringCurrentAnimLength;
				float animationSpeedRequired = ((m_firingTimes.Count != 0) ? (fullAnimsToFireAgain / (m_firingTimes[0] - m_timer)) : 1f);
				while (m_firingTimes.Count > 0 && animationSpeedRequired > m_maximumFiringAnimSpeedMultiplier)
				{
					if (BossLoader.Instance().m_currentBoss == BossLoader.Bosses.Zazz)
					{
						PlayCharacterAnimation(CurrentPostFiringAnimation, 1f);
						while (m_firingTimes[0] - firingCurrentAnimLength > m_timer)
						{
							yield return null;
						}
					}
					float availableTime = m_firingTimes[0] - m_timer;
					PlayCharacterAnimation(speed: Mathf.Max(firingCurrentAnimLength / availableTime, 1f), animation: CurrentFiringAnimation);
					while (IsCharacterAnimPlaying(CurrentFiringAnimation))
					{
						yield return null;
					}
					m_firingTimes.RemoveAt(0);
					if (BossBattleSystem.Instance().CurrentPhase.Type == BossBattleSystem.Phase.Types.Attack1)
					{
						m_visualController.PlaySpawnMissileEffect();
					}
					else
					{
						m_visualController.PlaySpawnMineEffect();
					}
					m_audioController.PlayFiringSFX();
					animationSpeedRequired = ((m_firingTimes.Count != 0) ? (fullAnimsToFireAgain / (m_firingTimes[0] - m_timer)) : 1f);
				}
				PlayCharacterAnimation(CurrentPostFiringAnimation, animationSpeedRequired);
				while (IsCharacterAnimPlaying(CurrentPostFiringAnimation))
				{
					yield return null;
				}
			}
			else
			{
				if (!BlockIdleAnim && !m_animationCharacter.isPlaying)
				{
					PlayCharacterAnimation(BossAnim.Idle);
				}
				yield return null;
			}
		}
	}

	public void StartFiring(float timeToRelease)
	{
		m_firingTimes.Add(m_timer + timeToRelease);
	}

	public void ClearAllFiring()
	{
		m_firingTimes.Clear();
		StopAllCoroutines();
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(ManageFiringAnimations());
		}
	}

	private void Update()
	{
		m_timer += Time.deltaTime;
	}

	public void PlayAnimation(BossAnim animation)
	{
		PlayAnimation(animation, 1f);
	}

	public void PlayAnimation(BossAnim animation, float speed)
	{
		PlayCharacterAnimation(animation, speed);
		PlayCameraAnimation(animation, speed);
		PlayAttackCameraAnimation(animation, speed);
	}

	public void PlayCharacterAnimation(BossAnim animation)
	{
		PlayCharacterAnimation(animation, 1f);
	}

	public void PlayCharacterAnimation(BossAnim animation, float speed)
	{
		AnimationClip animationClip = m_animations[(int)animation];
		if (animationClip != null)
		{
			m_animationCharacter.Play(animationClip.name);
			AnimationState animationState = m_animationCharacter[animationClip.name];
			animationState.normalizedTime = 0f;
			animationState.speed = speed;
		}
	}

	public void PlayCameraAnimation(BossAnim animation)
	{
		PlayCameraAnimation(animation, 1f);
	}

	public void PlayCameraAnimation(BossAnim animation, float speed)
	{
		AnimationClip animationClip = m_cameraAnimations[(int)animation];
		if (animationClip != null)
		{
			m_animationCamera.Play(animationClip.name);
			AnimationState animationState = m_animationCamera[animationClip.name];
			animationState.normalizedTime = 0f;
			animationState.speed = speed;
		}
	}

	public void PlayAttackCameraAnimation(BossAnim animation)
	{
		PlayAttackCameraAnimation(animation, 1f);
	}

	public void PlayAttackCameraAnimation(BossAnim animation, float speed)
	{
		AnimationClip animationClip = m_attackCameraAnimations[(int)animation];
		if (animationClip != null)
		{
			m_animationAttackCamera.Play(animationClip.name);
			AnimationState animationState = m_animationAttackCamera[animationClip.name];
			animationState.normalizedTime = 0f;
			animationState.speed = speed;
		}
	}

	public float GetCharacterAnimationLength(BossAnim animation)
	{
		AnimationClip animationClip = m_animations[(int)animation];
		if (animationClip != null)
		{
			return animationClip.length;
		}
		return 0f;
	}

	public bool IsCharacterAnimPlaying(BossAnim animation)
	{
		AnimationClip animationClip = m_animations[(int)animation];
		if (animationClip != null)
		{
			return m_animationCharacter.IsPlaying(animationClip.name);
		}
		return false;
	}

	public bool IsCameraAnimPlaying(BossAnim animation)
	{
		AnimationClip animationClip = m_cameraAnimations[(int)animation];
		if (animationClip != null)
		{
			return m_animationCamera.IsPlaying(animationClip.name);
		}
		return false;
	}

	public bool IsAttackCameraAnimPlaying(BossAnim animation)
	{
		AnimationClip animationClip = m_attackCameraAnimations[(int)animation];
		if (animationClip != null)
		{
			return m_animationAttackCamera.IsPlaying(animationClip.name);
		}
		return false;
	}
}
