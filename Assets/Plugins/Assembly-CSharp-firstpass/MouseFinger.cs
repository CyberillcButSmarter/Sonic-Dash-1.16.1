using UnityEngine;

public class MouseFinger : Finger
{
	private int _virtualIndex;

	public MouseFinger(int index)
		: base(index)
	{
		usingMouse = true;
	}

	public override int Index()
	{
		return _virtualIndex;
	}

	public override void SetIndex(int index)
	{
		_virtualIndex = index;
	}

	public override bool SetState()
	{
		base.SetState();
		Vector2 vector = Input.mousePosition;
		deltaPosition = vector - position;
		position = vector;
		touchPhase = GetTouchPhase();
		return true;
	}

	private TouchPhase setTouchPhase(TouchPhase touchPhasein)
	{
		touchPhase = touchPhasein;
		return touchPhase;
	}

	public TouchPhase GetTouchPhase()
	{
		bool flag = false;
		for (int i = 0; i < MouseControl.mouseButtons; i++)
		{
			if (Input.GetMouseButton(i))
			{
				flag = true;
				SetIndex(i);
				if (previousTouchPhase != TouchPhase.Began && Input.GetMouseButtonDown(i))
				{
					return setTouchPhase(TouchPhase.Began);
				}
				if (deltaPosition.sqrMagnitude < 1f)
				{
					return setTouchPhase(TouchPhase.Stationary);
				}
				return setTouchPhase(TouchPhase.Moved);
			}
		}
		if (!flag && touchPhase != TouchPhase.Ended && touchPhase != TouchPhase.Canceled)
		{
			return setTouchPhase(TouchPhase.Ended);
		}
		return TouchPhase.Canceled;
	}
}
