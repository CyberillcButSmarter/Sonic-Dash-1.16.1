using System.Reflection;
using UnityEngine;

public class BossAudioController : MonoBehaviour
{
	[SerializeField]
	private LocalisedAudioClip m_introTauntSFX;

	[SerializeField]
	private LocalisedAudioClip m_mineTransitionSFX;

	[SerializeField]
	private LocalisedAudioClip m_flyOffSFX;

	[SerializeField]
	private LocalisedAudioClip m_hitCharacterSFX;

	[SerializeField]
	private AudioClip m_mineFiringSFX;

	[SerializeField]
	private AudioClip m_rocketFiringSFX;

	[SerializeField]
	private AudioClip m_qteHitDamage;

	[SerializeField]
	private LocalisedAudioClip m_qteStartSFX;

	[SerializeField]
	private AudioClip m_qteBossAttackSFX;

	[SerializeField]
	private AudioClip m_qteGestureFailSFX;

	[SerializeField]
	private LocalisedAudioClip m_qteHit1SFX;

	[SerializeField]
	private LocalisedAudioClip m_qteHit2SFX;

	[SerializeField]
	private LocalisedAudioClip m_qteHit3SFX;

	[SerializeField]
	private AudioClip m_qteBossDefeatSFX;

	[SerializeField]
	private LocalisedAudioClip m_qteBossDefeatedVO;

	[SerializeField]
	private LocalisedAudioClip m_qteBossEscape1VO;

	[SerializeField]
	private LocalisedAudioClip m_qteBossEscape2VO;

	[SerializeField]
	private float m_voiceOverVolumeMutliplier;

	private void OnEnable()
	{
		FieldInfo[] fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		int num = fields.Length;
		for (int i = 0; i < num; i++)
		{
			FieldInfo fieldInfo = fields[i];
			if (fieldInfo.FieldType == typeof(LocalisedAudioClip))
			{
				WarmupAudioClip(((LocalisedAudioClip)fieldInfo.GetValue(this)).GetAudioClip());
			}
		}
	}

	private void WarmupAudioClip(AudioClip clip)
	{
		if (clip != null)
		{
			Audio.PlayClipOverrideVolumeModifier(clip, false, 0f);
			Audio.Stop(clip);
		}
	}

	public void PlayIntroTauntSFX()
	{
		Audio.PlayClipOverrideVolumeModifier(m_introTauntSFX.GetAudioClip(), false, m_voiceOverVolumeMutliplier);
	}

	public void PlayMineTransitionSFX()
	{
		Audio.PlayClipOverrideVolumeModifier(m_mineTransitionSFX.GetAudioClip(), false, m_voiceOverVolumeMutliplier);
	}

	public void PlayFlyOffSFX()
	{
		Audio.PlayClipOverrideVolumeModifier(m_flyOffSFX.GetAudioClip(), false, m_voiceOverVolumeMutliplier);
	}

	public void PlayHitCharacterSFX()
	{
		Audio.PlayClipOverrideVolumeModifier(m_hitCharacterSFX.GetAudioClip(), false, m_voiceOverVolumeMutliplier);
	}

	public void PlayFiringSFX()
	{
		if (BossBattleSystem.Instance().CurrentPhase.Type == BossBattleSystem.Phase.Types.Attack1)
		{
			Audio.PlayClip(m_rocketFiringSFX, false);
		}
		else
		{
			Audio.PlayClip(m_mineFiringSFX, false);
		}
	}

	public void PlayQTEStartSFX()
	{
		Audio.PlayClipOverrideVolumeModifier(m_qteStartSFX.GetAudioClip(), false, m_voiceOverVolumeMutliplier);
	}

	public void PlayQTEHit1SFX()
	{
		Audio.PlayClipOverrideVolumeModifier(m_qteHitDamage, false, m_voiceOverVolumeMutliplier);
		Audio.PlayClipOverrideVolumeModifier(m_qteHit1SFX.GetAudioClip(), false, m_voiceOverVolumeMutliplier);
	}

	public void PlayQTEHit2SFX()
	{
		Audio.PlayClipOverrideVolumeModifier(m_qteHitDamage, false, m_voiceOverVolumeMutliplier);
		Audio.PlayClipOverrideVolumeModifier(m_qteHit2SFX.GetAudioClip(), false, m_voiceOverVolumeMutliplier);
	}

	public void PlayQTEHit3SFX()
	{
		Audio.PlayClipOverrideVolumeModifier(m_qteHitDamage, false, m_voiceOverVolumeMutliplier);
		Audio.PlayClipOverrideVolumeModifier(m_qteHit3SFX.GetAudioClip(), false, m_voiceOverVolumeMutliplier);
	}

	public void PlayQTEAttackSFX()
	{
		Audio.PlayClipOverrideVolumeModifier(m_qteBossAttackSFX, false, m_voiceOverVolumeMutliplier);
	}

	public void PlayQTEGestureFailSFX()
	{
		Audio.PlayClipOverrideVolumeModifier(m_qteGestureFailSFX, false, m_voiceOverVolumeMutliplier);
	}

	public void PlayQTEDefeatedSFX()
	{
		Audio.PlayClipOverrideVolumeModifier(m_qteBossDefeatedVO.GetAudioClip(), false, m_voiceOverVolumeMutliplier);
		if (m_qteBossDefeatSFX != null)
		{
			Audio.PlayClip(m_qteBossDefeatSFX, false);
		}
	}

	public void PlayQTEEscape1SFX()
	{
		Audio.PlayClipOverrideVolumeModifier(m_qteBossEscape1VO.GetAudioClip(), false, m_voiceOverVolumeMutliplier);
	}

	public void PlayQTEEscape2SFX()
	{
		Audio.PlayClipOverrideVolumeModifier(m_qteBossEscape2VO.GetAudioClip(), false, m_voiceOverVolumeMutliplier);
	}
}
