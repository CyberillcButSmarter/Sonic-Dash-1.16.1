using UnityEngine;

public class MainMenuPlayButtonMover : MonoBehaviour
{
	private enum EButtons
	{
		Play = 0,
		Grantoo = 1
	}

	[SerializeField]
	private GameObject[] m_Buttons;

	[SerializeField]
	private int m_ButtonSpacing = 295;

	[SerializeField]
	private int m_Margin = 20;

	[SerializeField]
	private GameObject m_SinglePlayerBox;

	private UIRoot m_UIRoot;

	private int m_iContentWidth;

	private int m_iWidth;

	private int m_iHeight;

	private int m_iButtonFlags = -1;

	private Vector3 m_vInitialScale;

	private void Awake()
	{
		m_UIRoot = FindUIRoot(base.gameObject);
		m_vInitialScale = base.transform.localScale;
		Update();
	}

	protected UIRoot FindUIRoot(GameObject obj)
	{
		UIRoot component = obj.transform.GetComponent<UIRoot>();
		if ((bool)component)
		{
			return component;
		}
		if (obj.transform.parent == null)
		{
			return null;
		}
		return FindUIRoot(obj.transform.parent.gameObject);
	}

	private void Update()
	{
		bool flag = UpdateButtons();
		int width = Screen.width;
		int height = Screen.height;
		if (m_iWidth != width || m_iHeight != height || flag)
		{
			RescaleButtonBar();
		}
	}

	private bool UpdateButtons()
	{
		Characters.Type currentCharacterSelection = CharacterManager.Singleton.GetCurrentCharacterSelection();
		bool flag = Characters.CharacterUnlocked(currentCharacterSelection);
		bool active = GrantooManager.IsGrantooAvailable() && flag;
		m_Buttons[1].SetActive(active);
		m_SinglePlayerBox.SetActive(active);
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < m_Buttons.Length; i++)
		{
			if (m_Buttons[i].activeSelf)
			{
				num++;
				m_iButtonFlags += 1 << num;
			}
		}
		if (num2 != m_iButtonFlags)
		{
			m_iButtonFlags = num2;
			m_iContentWidth = m_Margin * 2;
			float num3 = (0f - (float)((num - 1) * m_ButtonSpacing)) / 2f;
			for (int j = 0; j < m_Buttons.Length; j++)
			{
				if (m_Buttons[j].activeSelf)
				{
					Vector3 localPosition = m_Buttons[j].transform.localPosition;
					localPosition.x = num3 + (float)(j * m_ButtonSpacing);
					m_Buttons[j].transform.localPosition = localPosition;
					m_iContentWidth += m_ButtonSpacing;
				}
			}
			return true;
		}
		return false;
	}

	private void RescaleButtonBar()
	{
		int width = Screen.width;
		int height = Screen.height;
		m_iWidth = width;
		m_iHeight = height;
		float num = m_UIRoot.manualHeight;
		float num2 = (float)width / (float)height;
		float num3 = num * num2;
		float num4 = 1f;
		if (num3 < (float)m_iContentWidth)
		{
			num4 = num3 / (float)m_iContentWidth;
		}
		base.transform.localScale = num4 * m_vInitialScale;
	}
}
