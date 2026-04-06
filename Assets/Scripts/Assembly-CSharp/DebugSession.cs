using System;
using UnityEngine;

public static class DebugSession
{
	private static bool s_enabled;
	private static DebugSessionDelayedApply s_delayedApply;

	public static bool Enabled
	{
		get
		{
			return s_enabled;
		}
	}

	public static void EnableForSession()
	{
		if (s_enabled)
		{
			return;
		}
		s_enabled = true;
		ApplyToScene();
		ScheduleDelayedApply();
	}

	public static void ApplyToScene()
	{
		if (!s_enabled)
		{
			return;
		}
		if (!IsMenuOrPauseMode())
		{
			return;
		}
		EnableDebugObjects();
	}

	private static void ScheduleDelayedApply()
	{
		if (s_delayedApply != null)
		{
			s_delayedApply.Restart();
			return;
		}
		GameObject applyObject = new GameObject("DebugSessionDelayedApply");
		UnityEngine.Object.DontDestroyOnLoad(applyObject);
		s_delayedApply = applyObject.AddComponent<DebugSessionDelayedApply>();
		s_delayedApply.Restart();
	}

	private static void EnableDebugObjects()
	{
		Transform[] transforms = Resources.FindObjectsOfTypeAll<Transform>();
		for (int i = 0; i < transforms.Length; i++)
		{
			Transform transform = transforms[i];
			if (transform == null)
			{
				continue;
			}
			if (transform.hideFlags != HideFlags.None)
			{
				continue;
			}
			GameObject gameObject = transform.gameObject;
			if (gameObject == null || gameObject.hideFlags != HideFlags.None)
			{
				continue;
			}
			if (IsDebugObjectName(gameObject.name) && ShouldEnableDebugObject(gameObject))
			{
				EnableWithParents(gameObject);
			}
		}
	}

	private static bool ShouldEnableDebugObject(GameObject gameObject)
	{
		if (Debug.isDebugBuild)
		{
			return true;
		}
		if (string.Equals(gameObject.name, "Button (Debug)", StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}
		return true;
	}

	private static void EnableWithParents(GameObject gameObject)
	{
		if (gameObject == null)
		{
			return;
		}
		Transform current = gameObject.transform;
		while (current != null)
		{
			if (!current.gameObject.activeSelf)
			{
				current.gameObject.SetActive(true);
			}
			current = current.parent;
		}
		SetActiveRecursively(gameObject.transform, true);
	}

	private static void SetActiveRecursively(Transform root, bool active)
	{
		if (root == null)
		{
			return;
		}
		for (int i = 0; i < root.childCount; i++)
		{
			Transform child = root.GetChild(i);
			if (child != null && !child.gameObject.activeSelf)
			{
				child.gameObject.SetActive(active);
			}
			SetActiveRecursively(child, active);
		}
	}

	private static bool IsMenuOrPauseMode()
	{
		try
		{
			GameState.Mode mode = GameState.GetMode();
			return mode == GameState.Mode.Menu || mode == GameState.Mode.PauseMenu;
		}
		catch
		{
			return true;
		}
	}

	private class DebugSessionDelayedApply : MonoBehaviour
	{
		private int m_stepsRemaining;

		private float m_nextApplyTime;

		public void Restart()
		{
			m_stepsRemaining = 3;
			m_nextApplyTime = 0f;
		}

		private void Update()
		{
			if (!s_enabled)
			{
				return;
			}
			if (m_stepsRemaining <= 0)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				s_delayedApply = null;
				return;
			}
			float now = Time.realtimeSinceStartup;
			if (m_nextApplyTime > 0f && now < m_nextApplyTime)
			{
				return;
			}
			m_stepsRemaining--;
			ApplyToScene();
			m_nextApplyTime = now + 0.6f;
		}
	}

	private static bool IsDebugObjectName(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return false;
		}
		if (name == "Debug (parent)")
		{
			return true;
		}
		if (name.IndexOf("[Debug]", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			return true;
		}
		if (name.IndexOf("Debug Menu", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			return true;
		}
		if (name.EndsWith("(Debug)", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		return false;
	}
}
