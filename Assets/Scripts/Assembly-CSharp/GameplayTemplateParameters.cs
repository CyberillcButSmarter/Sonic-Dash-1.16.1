using System;
using UnityEngine;

[AddComponentMenu("Dash/Gameplay Templates/Template Parameters")]
public class GameplayTemplateParameters : MonoBehaviour
{
	[SerializeField]
	private float m_rowLength = 2f;

	[SerializeField]
	private float m_ringJumpCurveFactor = 0.9f;

	[SerializeField]
	private int m_minTemplateRowCount = 16;

	[SerializeField]
	private int m_maxTemplateRowCount = 80;

	private GameplayTemplateDebugParameters m_debugParameters;

	[SerializeField]
	private float m_chopperSeperationJumpProportion = 7.5f;

	[SerializeField]
	private AnimationCurve m_gapChopperCountByDifficulty;

	[SerializeField]
	private AnimationCurve m_jumpGapJumpProportionByDifficulty;

	[SerializeField]
	private int m_emptyRowsBeforeSetPieceDashPads = 1;

	[SerializeField]
	private int m_ringSkipCellCount = 16;

	[SerializeField]
	private float m_templateBiasTowardsEndOfSetPiece = 2f;

	[SerializeField]
	private float m_minDistanceBetweenSetPieceSprings = 2000f;

	[SerializeField]
	private float m_setPieceSpringAChance = 0.5f;

	[SerializeField]
	private float m_setPieceSpringBChance = 0.5f;

	[SerializeField]
	private AnimationCurve m_chanceToSpawnPowerUp;

	[SerializeField]
	private AnimationCurve m_minSegmentsBetweenPowerUps;

	[SerializeField]
	private float m_changeSubzoneSpringAChanceGrass = 0.5f;

	[SerializeField]
	private float m_changeSubzoneSpringAChanceTemple = 0.5f;

	[SerializeField]
	private float m_changeSubzoneSpringAChanceBeach = 0.5f;

	[SerializeField]
	private float m_changeSubzoneSpringBChanceGrass = 0.5f;

	[SerializeField]
	private float m_changeSubzoneSpringBChanceTemple = 0.5f;

	[SerializeField]
	private float m_changeSubzoneSpringBChanceBeach = 0.5f;

	public float RowLength
	{
		get
		{
			return m_rowLength;
		}
	}

	public float RingJumpCurveFactor
	{
		get
		{
			return m_ringJumpCurveFactor;
		}
	}

	public int MinTemplateRowCount
	{
		get
		{
			return m_minTemplateRowCount;
		}
	}

	public int MaxTemplateRowCount
	{
		get
		{
			return m_maxTemplateRowCount;
		}
	}

	public int EmptyRowsBeforeSetPieceDashPads
	{
		get
		{
			return m_emptyRowsBeforeSetPieceDashPads;
		}
	}

	public int RingSkipCellCount
	{
		get
		{
			return m_ringSkipCellCount;
		}
	}

	public float TemplateBiasTowardsEndOfSetPiece
	{
		get
		{
			return m_templateBiasTowardsEndOfSetPiece;
		}
	}

	public float MinDistanceBetweenSetPieceSprings
	{
		get
		{
			return m_minDistanceBetweenSetPieceSprings;
		}
	}

	private void Awake()
	{
		m_debugParameters = GetComponent<GameplayTemplateDebugParameters>();
	}

	public bool IsValidRowCount(int rowCount)
	{
		return rowCount <= MaxTemplateRowCount && (rowCount < MinTemplateRowCount || rowCount / MinTemplateRowCount * MinTemplateRowCount == rowCount);
	}

	public float GetChopperSeperation(float sonicJumpLength)
	{
		return sonicJumpLength * m_chopperSeperationJumpProportion;
	}

	public int GetGapChopperCountAtDifficulty(float difficulty)
	{
		if (m_debugParameters != null && m_debugParameters.m_debugChoppers)
		{
			return m_debugParameters.m_debugChopperCount;
		}
		return (int)m_gapChopperCountByDifficulty.Evaluate(difficulty);
	}

	public float GetJumpGapLength(float sonicJumpLength, float difficulty)
	{
		return m_jumpGapJumpProportionByDifficulty.Evaluate(difficulty) * sonicJumpLength;
	}

	public float SetPieceSpringChance(int i)
	{
		return (i != 0) ? m_setPieceSpringBChance : m_setPieceSpringAChance;
	}

	public float GetSpawnChance(float distance)
	{
		float time = distance / 10000f;
		return m_chanceToSpawnPowerUp.Evaluate(time);
	}

	public int GetSegmentsWithoutPowerups(float distance)
	{
		float time = distance / 10000f;
		return (int)Math.Floor(m_minSegmentsBetweenPowerUps.Evaluate(time));
	}

	public float ChangeSubzoneSpringChance(int spring, SpringTV.Destination subzone)
	{
		float result = 0f;
		if (spring == 0)
		{
			switch (subzone)
			{
			case SpringTV.Destination.Grass:
				result = m_changeSubzoneSpringAChanceGrass;
				break;
			case SpringTV.Destination.Temple:
				result = m_changeSubzoneSpringAChanceTemple;
				break;
			case SpringTV.Destination.Beach:
				result = m_changeSubzoneSpringAChanceBeach;
				break;
			}
		}
		else
		{
			switch (subzone)
			{
			case SpringTV.Destination.Grass:
				result = m_changeSubzoneSpringBChanceGrass;
				break;
			case SpringTV.Destination.Temple:
				result = m_changeSubzoneSpringBChanceTemple;
				break;
			case SpringTV.Destination.Beach:
				result = m_changeSubzoneSpringBChanceBeach;
				break;
			}
		}
		return result;
	}

	public float RandomSpringChance(int spring)
	{
		float num = SetPieceSpringChance(spring);
		for (int i = 0; i < 3; i++)
		{
			num += ChangeSubzoneSpringChance(spring, (SpringTV.Destination)i);
		}
		return 1f - num;
	}
}
