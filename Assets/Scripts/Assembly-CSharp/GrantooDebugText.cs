using UnityEngine;

public class GrantooDebugText : MonoBehaviour
{
	private UILabel m_uiLabel;

	private void Start()
	{
		m_uiLabel = GetComponent<UILabel>();
	}

	private void Update()
	{
		if (!m_uiLabel)
		{
			return;
		}
		GrantooManager instance = GrantooManager.GetInstance();
		if (!(instance != null))
		{
			return;
		}
		GrantooState state = instance.GetState();
		string error = instance.GetError();
		if (state == GrantooState.Inactive && error == null)
		{
			m_uiLabel.enabled = false;
			return;
		}
		m_uiLabel.enabled = true;
		if (error == null)
		{
			m_uiLabel.text = state.ToString();
		}
		else
		{
			m_uiLabel.text = error;
		}
	}
}
