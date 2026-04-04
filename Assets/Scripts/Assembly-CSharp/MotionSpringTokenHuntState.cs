using System;
using UnityEngine;

public class MotionSpringTokenHuntState : MotionSpringState
{
	public enum TrackGenState
	{
		Requested = 0,
		Generated = 1
	}

	private const int c_iNumTokens = 4;

	private const float c_fMinSpawnY = -3f;

	private const float c_fMaxSpawnY = 8f;

	private const float c_fMinSpawnR = 7f;

	private const float c_fMaxSpawnR = 8.5f;

	private const float c_fMinTokenSpacing = 3f;

	private const float c_fTokenTime = 12f;

	private const float c_fTimeBeforeHideTrack = 0.25f;

	private const int c_iDefaultRingReward = 10;

	private const int c_iAllTokensRingReward = 20;

	private const int c_iBetterRingReward = 50;

	private const int c_iMaxRingReward = 100;

	private const float c_fChanceOfBetterReward = 0.5f;

	private const float c_fChanceOfMaxReward = 0.2f;

	private bool m_isTrackGenerated;

	private bool m_stateFinished;

	private bool m_rewardGiven;

	private Transform m_spawnableToken;

	private SpawnPool m_spawnPool;

	private BetterList<SpawnableEspioToken> m_TokenList = new BetterList<SpawnableEspioToken>();

	private GameObject m_camera;

	private GameObject m_hudCamera;

	private TrackGenerator m_trackGen;

	private int m_iNumCollected;

	private int m_iTokenLayerMask;

	private int m_iHUDLayerMask;

	private float m_fHideTrackTimer;

	public MotionSpringTokenHuntState(TrackGenState currentTrackState, SonicHandling handling, SonicPhysics physics)
		: base(physics, handling)
	{
		m_isTrackGenerated = currentTrackState == TrackGenState.Generated;
		m_camera = GameObject.Find("UnityCamera");
		m_hudCamera = GameObject.FindGameObjectWithTag("HudCamera");
		m_trackGen = GameObject.Find("TrackGenerator").GetComponent<TrackGenerator>();
	}

	public override void Enter()
	{
		m_iTokenLayerMask = 1 << LayerMask.NameToLayer("Default");
		m_iHUDLayerMask = 1 << LayerMask.NameToLayer("NGUI (Menu)");
		m_TokenList.Clear();
		m_stateFinished = false;
		m_rewardGiven = false;
		m_iNumCollected = 0;
		m_fHideTrackTimer = 0.25f;
		base.Enter();
		SpawnTokens();
		EventDispatch.GenerateEvent("OnEspioTokenHuntStart");
		m_trackGen.HideTrack(true);
	}

	public override void Exit()
	{
		m_trackGen.HideTrack(false);
		CleanUpTokens();
	}

	private int CalcRingReward()
	{
		int result = 10;
		if (m_iNumCollected >= 4)
		{
			result = 20;
			float num = UnityEngine.Random.Range(0f, 1f);
			if (num < 0.2f)
			{
				result = 100;
			}
			else if (num < 0.5f)
			{
				result = 50;
			}
		}
		return result;
	}

	private void SpawnTokens()
	{
		m_TokenList.Clear();
		m_spawnPool = EspioSpecialEvent.GetTokenSpawnPool();
		if (m_spawnPool != null)
		{
			if (EspioSpecialEvent.GetRewardType() == EspioSpecialEvent.RewardType.Espio)
			{
				m_spawnableToken = m_spawnPool.GetPrefab("EspioToken").transform;
			}
			else
			{
				m_spawnableToken = m_spawnPool.GetPrefab("QuestionMarkToken").transform;
			}
		}
		if (!(m_spawnableToken != null))
		{
			return;
		}
		for (int i = 0; i < 4; i++)
		{
			GameObject gameObject = m_spawnPool.Spawn(m_spawnableToken).gameObject;
			int num = 0;
			bool flag = false;
			Vector3 zero = Vector3.zero;
			do
			{
				float num2 = UnityEngine.Random.Range(7f, 8.5f);
				float f = UnityEngine.Random.Range(30f, 150f) * ((float)Math.PI / 180f);
				zero.y = UnityEngine.Random.Range(-3f, 8f);
				zero.x = num2 * Mathf.Cos(f);
				zero.z = num2 * (0f - Mathf.Sin(f));
				num++;
				flag = false;
				foreach (SpawnableEspioToken token in m_TokenList)
				{
					if ((token.transform.localPosition - zero).magnitude < 3f)
					{
						flag = true;
					}
				}
			}
			while (num < 5 && flag);
			gameObject.transform.parent = Sonic.MeshTransform;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localPosition = zero;
			SpawnableEspioToken component = gameObject.GetComponent<SpawnableEspioToken>();
			float fDelay = (float)i * 0.25f;
			component.Init(fDelay, 12f);
			m_TokenList.Add(component);
		}
	}

	public override void Execute()
	{
		bool flag = false;
		foreach (SpawnableEspioToken token in m_TokenList)
		{
			flag |= !token.IsFinished();
		}
		if (!flag)
		{
			m_stateFinished = true;
			if (m_iNumCollected == 0)
			{
				EventDispatch.GenerateEvent("SpringGestureFailure");
			}
		}
		if (m_fHideTrackTimer > 0f)
		{
			m_fHideTrackTimer -= Time.deltaTime;
			if (m_fHideTrackTimer <= 0f)
			{
				m_trackGen.HideTrack(true);
			}
		}
		CheckForTokenTaps();
	}

	public void CheckForTokenTaps()
	{
		if (!Input.GetMouseButtonDown(0))
		{
			return;
		}
		Ray ray = m_hudCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;
		if (UnityEngine.Physics.Raycast(ray, out hitInfo, 100f, m_iHUDLayerMask))
		{
			return;
		}
		Ray ray2 = m_camera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
		if (!UnityEngine.Physics.Raycast(ray2, out hitInfo, 100f, m_iTokenLayerMask))
		{
			return;
		}
		SpawnableEspioToken component = hitInfo.collider.gameObject.GetComponent<SpawnableEspioToken>();
		if (component != null && component.CanBeCollected())
		{
			component.Collect();
			int num = m_iNumCollected & 3;
			CartesianDir cartesianDir = CartesianDir.Down;
			if (num == 1)
			{
				cartesianDir = CartesianDir.Left;
			}
			if (num == 2)
			{
				cartesianDir = CartesianDir.Right;
			}
			if (num == 3)
			{
				cartesianDir = CartesianDir.Up;
			}
			EventDispatch.GenerateEvent("OnSingleSpringGestureSuccess", cartesianDir);
			m_iNumCollected++;
		}
	}

	public override LightweightTransform CalculateNewTransform(TransformParameters tParams)
	{
		if (m_stateFinished)
		{
			if (!m_rewardGiven && m_iNumCollected > 0)
			{
				if (EspioSpecialEvent.GetRewardType() == EspioSpecialEvent.RewardType.Rings)
				{
					int ringCount = CalcRingReward();
					PowerUps.DoRingPowerupAction(ringCount);
					EventDispatch.GenerateEvent("SpringTokenHuntSuccess");
				}
				m_rewardGiven = true;
			}
			if (m_isTrackGenerated)
			{
				m_trackGen.HideTrack(false);
				EventDispatch.GenerateEvent("OnEspioTokenHuntEnd");
				MoveToNextState(tParams.StateMachine);
			}
		}
		return tParams.CurrentTransform;
	}

	public void CleanUpTokens()
	{
		foreach (SpawnableEspioToken token in m_TokenList)
		{
			m_spawnPool.Despawn(token.transform);
		}
		m_TokenList.Clear();
	}

	private void MoveToNextState(MotionStateMachine stateMachine)
	{
		CleanUpTokens();
		stateMachine.RequestState(new MotionSpringDescentState(MotionSpringDescentState.TrackState.Ready, Sonic.Handling, base.Physics));
	}

	private void Event_OnTrackGenerationComplete()
	{
		m_isTrackGenerated = true;
	}
}
