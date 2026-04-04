using UnityEngine;

public class CameraTransitionProperties : MonoBehaviour
{
	[SerializeField]
	private iTweenPath m_transitionPath;

	[SerializeField]
	private float m_transitionTime = 3f;

	public iTweenPath TransitionPath
	{
		get
		{
			return m_transitionPath;
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
