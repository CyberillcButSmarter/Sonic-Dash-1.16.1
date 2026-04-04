using UnityEngine;

public class MoveToPageProperties : MonoBehaviour
{
	[SerializeField]
	private GuiTrigger m_destinationPage;

	[SerializeField]
	private iTweenPath m_transitionPath;

	[SerializeField]
	private bool m_transitionPathInReverse;

	[SerializeField]
	private bool m_replaceCurrentPage;

	public GuiTrigger DestinationPage
	{
		get
		{
			return m_destinationPage;
		}
		set
		{
			m_destinationPage = value;
		}
	}

	public iTweenPath TransitionPath
	{
		get
		{
			return m_transitionPath;
		}
		set
		{
			m_transitionPath = value;
		}
	}

	public bool TransitionPathInReverse
	{
		get
		{
			return m_transitionPathInReverse;
		}
	}

	public bool ReplaceCurrentPage
	{
		get
		{
			return m_replaceCurrentPage;
		}
	}
}
