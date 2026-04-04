using UnityEngine;

public class GrantooRetryButtonBlocker : MonoBehaviour
{
	[SerializeField]
	private UISprite m_ButtonSprite;

	protected bool m_bEnabled = true;

	private void Update()
	{
		bool flag = !GrantooManager.GetInstance().IsActive();
		if (flag != m_bEnabled)
		{
			m_bEnabled = flag;
			m_ButtonSprite.spriteName = ((!flag) ? "button_disabled" : "button_orange_med");
			GetComponent<GuiButtonBlocker>().Blocked = !flag;
		}
	}
}
