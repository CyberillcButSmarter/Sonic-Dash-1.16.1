using UnityEngine;

[AddComponentMenu("Dash/Sonic/Input Controller")]
public class SonicController : MonoBehaviour
{
	private SimpleGestureMonitor m_simpleGestureMonitor;

	private bool m_jump;

	private bool m_roll;

	private bool m_left;

	private bool m_right;

	private bool m_tap;

	private bool m_faketap;

	private bool m_dashInput;

	private Vector2 m_mouseSwipeStart;

	private bool m_mouseSwipeActive;

	private float m_mouseSwipeThreshold;

	private DashMeter m_dashMeter;

	private bool m_jumpPermitted = true;

	private bool m_autoJumpActive;

	private GameObject m_labelGestureOutputDebug;

	public void Awake()
	{
		EventDispatch.RegisterInterest("ResetGameState", this);
		EventDispatch.RegisterInterest("StartGameState", this);
		m_simpleGestureMonitor = new SimpleGestureMonitor();
		m_dashMeter = UnityEngine.Object.FindObjectOfType<DashMeter>();
		m_mouseSwipeThreshold = Mathf.Max(50f, Screen.dpi * 0.05f);
	}

	public void OnDestroy()
	{
		EventDispatch.UnregisterAllInterest(this);
	}

	public void Start()
	{
	}

	private void Event_ResetGameState(GameState.Mode state)
	{
		m_jumpPermitted = true;
	}

	private void Event_StartGameState(GameState.Mode state)
	{
		m_simpleGestureMonitor.reset();
	}

	public void Update()
	{
		resetControls();
		if (isDesktopPlatform())
		{
			updateDesktopControls();
			updateMouseGestures();
		}
		m_simpleGestureMonitor.Update();
		updateGameControls();
		applyControls();
	}

	private void resetControls()
	{
		m_jump = false;
		m_roll = false;
		m_left = false;
		m_right = false;
		m_tap = false;
		m_faketap = false;
		m_dashInput = false;
	}

	private void updateDesktopControls()
	{
		m_jump |= Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
		m_roll |= Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);
		m_left |= Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
		m_right |= Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
		m_faketap |= Input.GetKeyDown(KeyCode.Space);
		m_dashInput |= Input.GetKeyDown(KeyCode.Space);
	}

	private bool isDesktopPlatform()
	{
		RuntimePlatform platform = Application.platform;
		switch (platform)
		{
		case RuntimePlatform.WindowsEditor:
		case RuntimePlatform.WindowsPlayer:
		case RuntimePlatform.OSXEditor:
		case RuntimePlatform.OSXPlayer:
		case RuntimePlatform.LinuxPlayer:
		case RuntimePlatform.WebGLPlayer:
			return true;
		default:
			return false;
		}
	}

	private void updateMouseGestures()
	{
		if (!Input.GetMouseButtonDown(0) && !m_mouseSwipeActive && Input.GetMouseButton(0))
		{
			// still pressed without previous down event, just ignore
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			m_mouseSwipeActive = true;
			m_mouseSwipeStart = Input.mousePosition;
		}
		if (m_mouseSwipeActive && Input.GetMouseButtonUp(0))
		{
			Vector2 delta = (Vector2)Input.mousePosition - m_mouseSwipeStart;
			m_mouseSwipeActive = false;
			if (delta.magnitude > m_mouseSwipeThreshold)
			{
				if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
				{
					if (delta.x > 0f)
					{
						m_right = true;
					}
					else
					{
						m_left = true;
					}
				}
				else
				{
					if (delta.y > 0f)
					{
						m_jump = true;
					}
					else
					{
						m_roll = true;
					}
				}
			}
			else
			{
				m_tap = true;
			}
		}
	}

	private void updateGameControls()
	{
		m_jump |= m_simpleGestureMonitor.swipeUpDetected();
		m_roll |= m_simpleGestureMonitor.swipeDownDetected();
		m_left |= m_simpleGestureMonitor.swipeLeftDetected();
		m_right |= m_simpleGestureMonitor.swipeRightDetected();
		m_tap |= m_simpleGestureMonitor.tapDetected();
	}

	private void applyControls()
	{
		if (m_left)
		{
			SendMessage("Strafe", SideDirection.Left);
			if ((bool)m_labelGestureOutputDebug)
			{
				m_labelGestureOutputDebug.SendMessage("Strafe", SideDirection.Left);
			}
		}
		else if (m_right)
		{
			SendMessage("Strafe", SideDirection.Right);
			if ((bool)m_labelGestureOutputDebug)
			{
				m_labelGestureOutputDebug.SendMessage("Strafe", SideDirection.Right);
			}
		}
		else if (m_roll)
		{
			if (m_jumpPermitted)
			{
				SendMessage("Roll");
				SendMessage("Dive");
				if ((bool)m_labelGestureOutputDebug)
				{
					m_labelGestureOutputDebug.SendMessage("Roll");
				}
			}
		}
		else if (m_jump && m_jumpPermitted)
		{
			SendMessage("Jump");
			if ((bool)m_labelGestureOutputDebug)
			{
				m_labelGestureOutputDebug.SendMessage("Jump");
			}
		}
		if (m_tap)
		{
			float[] value = new float[2]
			{
				m_simpleGestureMonitor.getTapX(),
				m_simpleGestureMonitor.getTapY()
			};
			SendMessage("Tap", value);
			if ((bool)m_labelGestureOutputDebug)
			{
				m_labelGestureOutputDebug.SendMessage("Tap", value);
			}
		}
		if (m_faketap)
		{
			float[] value2 = new float[2]
			{
				Screen.width / 2,
				Screen.height / 2
			};
			SendMessage("Tap", value2);
			if ((bool)m_labelGestureOutputDebug)
			{
				m_labelGestureOutputDebug.SendMessage("Tap", value2);
			}
		}
		if (m_dashInput && m_dashMeter != null)
		{
			m_dashMeter.RequestDash();
		}
	}

	public void activateAutoJump()
	{
		m_autoJumpActive = true;
	}

	public void deactivateAutoJump()
	{
		m_autoJumpActive = false;
	}

	public bool isAutoJumpActive()
	{
		return m_autoJumpActive;
	}

	public void disallowJump()
	{
		m_jumpPermitted = false;
	}

	public void allowJump()
	{
		m_jumpPermitted = true;
	}
}
