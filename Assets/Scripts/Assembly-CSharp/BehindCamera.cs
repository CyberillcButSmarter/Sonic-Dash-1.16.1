using System;
using UnityEngine;

public class BehindCamera : GameCamera
{
	[Flags]
	private enum State
	{
		None = 0,
		ShowingGoal = 1
	}

	private static BehindCamera m_instance;

	[SerializeField]
	private CameraType m_defaultCamera;

	[SerializeField]
	private CameraType m_preRunCamera;

	[SerializeField]
	private CameraTypeSetPiece m_setPieceCamera;

	[SerializeField]
	private CameraType m_deathCamera;

	[SerializeField]
	private CameraType m_attackCamera;

	[SerializeField]
	private CameraType m_springCamera;

	[SerializeField]
	private CameraType m_springBossCamera;

	[SerializeField]
	private float m_toGameTransitionTime = 3f;

	[SerializeField]
	private float m_fromPreRunTransitionTime = 1.5f;

	private State m_state;

	private bool m_attacking;

	public static BehindCamera Instance
	{
		get
		{
			return m_instance;
		}
	}

	public CameraTypeSetPiece SetPieceCamera
	{
		get
		{
			return m_setPieceCamera;
		}
	}

	public CameraType DeathCamera
	{
		get
		{
			return m_deathCamera;
		}
	}

	public void ResetToGameCamera(float transitionTime)
	{
		SetActiveCamera(m_defaultCamera, transitionTime);
	}

	public static CameraTypeMain GetMainCamera()
	{
		return Instance.MainCamera;
	}

	public void ForceAttackEndCamera()
	{
		if (m_attacking)
		{
			m_attacking = false;
			CameraTypeJumpAttack cameraTypeJumpAttack = (CameraTypeJumpAttack)m_attackCamera;
			ResetToGameCamera(cameraTypeJumpAttack.m_transitionOutTime);
		}
	}

	private void Awake()
	{
		if (m_instance == null)
		{
			m_instance = this;
		}
		CacheMainCamera();
	}

	private void Start()
	{
		EventDispatch.RegisterInterest("ResetGameState", this);
		EventDispatch.RegisterInterest("OnNewGameStarted", this);
		EventDispatch.RegisterInterest("OnNewGameHudShown", this);
		EventDispatch.RegisterInterest("EnterMotionRollState", this);
		EventDispatch.RegisterInterest("ExitMotionRollState", this);
		EventDispatch.RegisterInterest("OnSonicDeath", this);
		EventDispatch.RegisterInterest("OnSpringStart", this);
		EventDispatch.RegisterInterest("OnSonicAttack", this);
		EventDispatch.RegisterInterest("OnSonicAttackEnd", this);
		EventDispatch.RegisterInterest("OnSonicResurrection", this);
		base.Camera.enabled = false;
		base.Camera.enabled = true;
	}

	private void ActivateCamera(float time)
	{
		if (HudContent_PlayerGoals.CanShow)
		{
			ActivatePreRunCamera(time);
			m_state |= State.ShowingGoal;
		}
		else
		{
			ActivateDefaultCamera(time);
			m_state &= ~State.ShowingGoal;
		}
	}

	private void ActivateDefaultCamera(float transitionTime)
	{
		SetActiveCamera(m_defaultCamera, transitionTime);
	}

	private void ActivatePreRunCamera(float transitionTime)
	{
		SetActiveCamera(m_preRunCamera, transitionTime);
	}

	private void Event_ResetGameState(GameState.Mode resetState)
	{
		if (resetState == GameState.Mode.Game)
		{
			ActivateCamera(0f);
		}
	}

	private void Event_OnNewGameStarted()
	{
		ActivateCamera(m_toGameTransitionTime);
	}

	private void Event_OnNewGameHudShown()
	{
		if ((m_state & State.ShowingGoal) == State.ShowingGoal)
		{
			ActivateDefaultCamera(m_fromPreRunTransitionTime);
			m_state &= ~State.ShowingGoal;
		}
	}

	private void Event_EnterMotionRollState()
	{
	}

	private void Event_ExitMotionRollState()
	{
		if (base.MainCamera.GetCurrentCameraType() != m_setPieceCamera && (m_state & State.ShowingGoal) == State.ShowingGoal)
		{
			ActivateCamera(0.25f);
		}
	}

	private void Event_OnSonicDeath()
	{
		SetActiveCamera(m_deathCamera, 0.6f);
	}

	private void Event_OnSpringStart(SpringTV.Type springType)
	{
		if (springType == SpringTV.Type.BossEnd)
		{
			SetActiveCamera(m_springBossCamera, 0.25f);
			return;
		}
		CameraTypeSpring cameraTypeSpring = m_springCamera as CameraTypeSpring;
		if (cameraTypeSpring != null)
		{
			cameraTypeSpring.SetSpringType(springType);
		}
		SetActiveCamera(m_springCamera, 0.25f);
	}

	private void Event_OnSonicAttack()
	{
		m_attacking = true;
		CameraTypeJumpAttack cameraTypeJumpAttack = (CameraTypeJumpAttack)m_attackCamera;
		SetActiveCamera(m_attackCamera, cameraTypeJumpAttack.m_transitionInTime);
	}

	private void Event_OnSonicAttackEnd()
	{
		m_attacking = false;
		CameraTypeJumpAttack cameraTypeJumpAttack = (CameraTypeJumpAttack)m_attackCamera;
		ResetToGameCamera(cameraTypeJumpAttack.m_transitionOutTime);
	}

	private void Event_OnSonicResurrection()
	{
		SetActiveCamera(m_defaultCamera, 0.6f);
	}
}
