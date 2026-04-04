using UnityEngine;

public class RingCountDisplay : MonoBehaviour
{
	public enum DisplayType
	{
		TotalBanked = 0,
		RunBanked = 1,
		Held = 2,
		StarRings = 3
	}

	[SerializeField]
	private DisplayType m_displayType;

	private UILabel m_ringLabel;

	private void Start()
	{
		m_ringLabel = GetComponent<UILabel>();
	}

	private void Update()
	{
		int ringCount = GetRingCount();
		m_ringLabel.text = LanguageUtils.FormatNumber(ringCount);
	}

	private int GetRingCount()
	{
		int result = RingStorage.HeldRings;
		if (m_displayType == DisplayType.TotalBanked)
		{
			result = RingStorage.TotalBankedRings;
		}
		else if (m_displayType == DisplayType.RunBanked)
		{
			result = RingStorage.RunBankedRings;
		}
		else if (m_displayType == DisplayType.StarRings)
		{
			result = RingStorage.TotalStarRings;
		}
		return result;
	}
}
