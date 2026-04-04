using UnityEngine;

public class GuiButtonBlocker : MonoBehaviour
{
	[SerializeField]
	private bool m_blocked;

	private bool m_blockedBack;

	[SerializeField]
	private GameObject m_EnabledBackground;

	[SerializeField]
	private GameObject m_DisabledBackground;

	public bool Blocked
	{
		get
		{
			return m_blocked;
		}
		set
		{
			m_blocked = value;
		}
	}

	public void Update()
	{
		if (m_blocked != m_blockedBack)
		{
			m_blockedBack = m_blocked;
			if (m_EnabledBackground != null && m_DisabledBackground != null)
			{
				Color color = m_EnabledBackground.GetComponent<UISprite>().color;
				color.a = ((!m_blocked) ? 1 : 0);
				m_EnabledBackground.GetComponent<UISprite>().color = color;
				Color color2 = m_DisabledBackground.GetComponent<UISprite>().color;
				color2.a = (m_blocked ? 1 : 0);
				m_DisabledBackground.GetComponent<UISprite>().color = color2;
			}
		}
	}
}
