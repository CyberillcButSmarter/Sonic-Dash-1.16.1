using System.Collections;
using UnityEngine;

public class PoolFrameTimeSentinal : MonoBehaviour
{
	private static float s_realTimeAtEndOfLastFrame;

	public static bool IsFramerateImportant
	{
		get
		{
			bool isPlaying = Application.isPlaying;
			bool flag = FrameDurationSoFar > 0.025000002f;
			return isPlaying && flag;
		}
	}

	public static float RealTimeAtEndOfLastFrame
	{
		get
		{
			return s_realTimeAtEndOfLastFrame;
		}
		private set
		{
			s_realTimeAtEndOfLastFrame = value;
		}
	}

	public static float FrameDurationSoFar
	{
		get
		{
			return Time.realtimeSinceStartup - RealTimeAtEndOfLastFrame;
		}
	}

	private void Awake()
	{
		StartCoroutine(RealTimeAtEndOfLastFrameUpdater());
	}

	private IEnumerator RealTimeAtEndOfLastFrameUpdater()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			RealTimeAtEndOfLastFrame = Time.realtimeSinceStartup;
		}
	}
}
