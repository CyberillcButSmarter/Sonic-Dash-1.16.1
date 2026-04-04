using System;
using UnityEngine;

public class SpawnableEspioToken : SpawnableObject
{
	private enum State
	{
		Waiting = 0,
		Active = 1,
		Collecting = 2,
		Finished = 3
	}

	private const float c_fAnimInDistance = 20f;

	private const float c_fAnimOutDistance = 20f;

	[SerializeField]
	private GameObject m_MeshObject;

	[SerializeField]
	private Collider m_Collider;

	[SerializeField]
	private float m_fSpinRate = 200f;

	[SerializeField]
	private ParticleSystem[] m_ActiveParticles;

	private float m_fDelay;

	private float m_fActiveTime;

	private float m_fTotalActiveTime;

	private State m_eState;

	private Vector3 m_vSpawnPos = Vector3.zero;

	private float m_fSpin;

	public void Init(float fDelay = 0f, float fActiveTime = 5f)
	{
		m_fDelay = fDelay;
		m_fTotalActiveTime = (m_fActiveTime = fActiveTime);
		m_eState = State.Waiting;
		m_vSpawnPos = base.transform.localPosition;
		m_fSpin = UnityEngine.Random.Range(0f, 360f);
		m_MeshObject.SetActive(false);
		m_Collider.enabled = false;
		ParticleSystem[] activeParticles = m_ActiveParticles;
		foreach (ParticleSystem particleSystem in activeParticles)
		{
			particleSystem.Stop();
			particleSystem.Clear();
		}
	}

	public bool IsFinished()
	{
		return m_eState == State.Finished;
	}

	public bool CanBeCollected()
	{
		return m_eState == State.Active;
	}

	public void Collect()
	{
		m_fActiveTime = 1f;
		m_eState = State.Collecting;
		m_Collider.enabled = false;
		m_vSpawnPos = base.transform.localPosition;
		ParticleSystem[] activeParticles = m_ActiveParticles;
		foreach (ParticleSystem particleSystem in activeParticles)
		{
			particleSystem.Stop();
		}
		if (EspioSpecialEvent.GetRewardType() == EspioSpecialEvent.RewardType.Espio)
		{
			EspioSpecialEvent.AwardEspioToken();
		}
	}

	private void Update()
	{
		m_fSpin += m_fSpinRate * Time.deltaTime;
		switch (m_eState)
		{
		case State.Waiting:
			m_fDelay -= Time.deltaTime;
			if (m_fDelay <= 0f)
			{
				m_MeshObject.SetActive(true);
				m_Collider.enabled = true;
				m_eState = State.Active;
				UpdateTransform();
				ParticleSystem[] activeParticles = m_ActiveParticles;
				foreach (ParticleSystem particleSystem in activeParticles)
				{
					particleSystem.Play();
				}
			}
			break;
		case State.Active:
			m_fActiveTime = Mathf.Max(0f, m_fActiveTime - Time.deltaTime);
			UpdateTransform();
			if (m_fActiveTime <= 0f)
			{
				m_MeshObject.SetActive(false);
				m_Collider.enabled = false;
				m_eState = State.Finished;
			}
			break;
		case State.Collecting:
			m_fSpin += m_fSpinRate * Time.deltaTime * 3f;
			m_fActiveTime = Mathf.Max(0f, m_fActiveTime - Time.deltaTime * 2f);
			UpdateCollectAnimTransform();
			if (m_fActiveTime <= 0f)
			{
				m_MeshObject.SetActive(false);
				m_Collider.enabled = false;
				m_eState = State.Finished;
			}
			break;
		}
	}

	private float EaseIn(float fIn)
	{
		float num = Mathf.Clamp(fIn, 0f, 1f);
		return Mathf.Sin(num * 0.5f * (float)Math.PI);
	}

	private void UpdateTransform()
	{
		float num = 0.35f;
		float num2 = m_vSpawnPos.y;
		if (m_fActiveTime > m_fTotalActiveTime - num)
		{
			float num3 = (m_fActiveTime - (m_fTotalActiveTime - num)) / num;
			num2 -= (1f - EaseIn(1f - num3)) * 20f;
		}
		if (m_fActiveTime < num)
		{
			float num4 = 1f - m_fActiveTime / num;
			num2 += (1f - EaseIn(1f - num4)) * 20f;
		}
		Vector3 vSpawnPos = m_vSpawnPos;
		vSpawnPos.y = num2;
		base.transform.localPosition = vSpawnPos;
		m_MeshObject.transform.localRotation = Quaternion.Euler(0f, m_fSpin, 0f);
		m_MeshObject.transform.localScale = Vector3.one;
	}

	private void UpdateCollectAnimTransform()
	{
		Vector3 vector = new Vector3(0f, 1f, 0f);
		Vector3 localPosition = Vector3.Lerp(vector, m_vSpawnPos, Mathf.SmoothStep(0f, 1f, m_fActiveTime));
		float num = Mathf.Sin(m_fActiveTime * (float)Math.PI);
		localPosition.y += num;
		base.transform.localPosition = localPosition;
		float num2 = 0.25f + 0.75f * m_fActiveTime;
		m_MeshObject.transform.localRotation = Quaternion.Euler(0f, m_fSpin, 0f);
		m_MeshObject.transform.localScale = Vector3.one * num2;
	}
}
