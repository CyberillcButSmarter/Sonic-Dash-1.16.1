using UnityEngine;

public class GuiAndroidBack : IgnoreTimeScale
{
	private const float m_backDisableTime = 0.5f;

	public bool m_killApplication;

	private BoxCollider m_boxCollider;

	private UISprite[] m_renderableChildren;

	private GuiButtonBlocker m_buttonBlocker;

	private float m_timeEnabled;

	private static float m_lastClickTime;

	private void Awake()
	{
		m_renderableChildren = GetComponentsInChildren<UISprite>();
		m_boxCollider = GetComponent<BoxCollider>();
		m_buttonBlocker = GetComponent<GuiButtonBlocker>();
	}

	private bool anyVisible(UISprite[] sprites)
	{
		bool flag = false;
		foreach (UISprite uISprite in sprites)
		{
			flag |= uISprite.isVisible;
			if (flag)
			{
				break;
			}
		}
		return flag;
	}

	private void Update()
	{
		float num = UpdateRealTimeDelta();
		if (m_boxCollider.enabled)
		{
			m_timeEnabled += num;
		}
		else
		{
			m_timeEnabled = 0f;
		}
		if (!Input.GetKeyDown(KeyCode.Escape) || m_timeEnabled < 0.5f)
		{
			return;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (!(m_lastClickTime + 0.5f > realtimeSinceStartup) && (!(m_boxCollider != null) || m_boxCollider.enabled) && (!(m_buttonBlocker != null) || !m_buttonBlocker.Blocked) && (m_renderableChildren == null || anyVisible(m_renderableChildren)))
		{
			m_timeEnabled = 0f;
			m_lastClickTime = Time.realtimeSinceStartup;
			if (m_killApplication)
			{
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.hardlightstudio.dev.sonicdash.plugin.SLGlobal");
				androidJavaClass.CallStatic("ForceMinimize");
			}
			else
			{
				base.gameObject.SendMessage("OnClick");
			}
		}
	}
}
