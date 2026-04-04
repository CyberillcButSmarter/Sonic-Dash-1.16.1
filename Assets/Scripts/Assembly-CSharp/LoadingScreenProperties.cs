using UnityEngine;

public class LoadingScreenProperties : MonoBehaviour
{
	public enum ScreenType
	{
		SegaLogo = 0,
		HLLogo = 1,
		SonicLoading = 2
	}

	[SerializeField]
	public int m_screenOrder;

	[SerializeField]
	public float m_displayTime = 1f;

	[SerializeField]
	public float m_transitionTime = 0.5f;

	[SerializeField]
	public ScreenType m_screenType;

	public int ScreenOrder
	{
		get
		{
			return m_screenOrder;
		}
	}

	public float DisplayTime
	{
		get
		{
			return m_displayTime;
		}
	}

	public float TransitionTime
	{
		get
		{
			return m_transitionTime;
		}
	}
}
