using UnityEngine;

public class GrantooNotificationCounter : MonoBehaviour
{
	private UILabel m_countLabel;

	private bool m_visible = true;

	private int m_previousCount;

	private void Start()
	{
		m_countLabel = Utils.GetComponentInChildren<UILabel>(base.gameObject);
		ShowChildren(false);
	}

	private void Update()
	{
		if (m_countLabel == null)
		{
			return;
		}
		int notificationCount = GrantooManager.GetInstance().GetNotificationCount();
		if (m_previousCount != notificationCount)
		{
			if (notificationCount > 0)
			{
				ShowChildren(true);
				m_countLabel.text = notificationCount.ToString();
			}
			else
			{
				ShowChildren(false);
			}
			m_previousCount = notificationCount;
		}
	}

	private void ShowChildren(bool show)
	{
		UIWidget[] componentsInChildren = GetComponentsInChildren<UIWidget>(true);
		if (componentsInChildren != null && show != m_visible)
		{
			m_visible = show;
			UIWidget[] array = componentsInChildren;
			foreach (UIWidget uIWidget in array)
			{
				uIWidget.gameObject.SetActive(show);
			}
		}
	}
}
