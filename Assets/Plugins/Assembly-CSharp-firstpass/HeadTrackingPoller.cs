using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class HeadTrackingPoller
{
	public IntPtr nativeHeadTrackingPoller;

	public HeadTrackingPoller(IntPtr poller)
	{
		nativeHeadTrackingPoller = poller;
	}

	[DllImport("headtracking_client")]
	private static extern bool Amazon_HeadTrackingPoller_sample(IntPtr headTrackingPoller, IntPtr headTrackingEvent);

	[DllImport("headtracking_client")]
	private static extern bool Amazon_HeadTrackingPoller_updateConfiguration(IntPtr headTrackingPoller, IntPtr headTrackingConfig);

	public bool Sample(ref HeadTrackingEvent htEvent)
	{
		if (htEvent == null)
		{
			Debug.LogWarning("The provided HeadTrackingEvent is not initialized. Unable to sample.");
			return false;
		}
		if (nativeHeadTrackingPoller == IntPtr.Zero)
		{
			Debug.LogWarning("The native HeadTrackingPoller is not initialized. Unable to sample.");
			return false;
		}
		if (!Amazon_HeadTrackingPoller_sample(nativeHeadTrackingPoller, htEvent.nativeHeadTrackingEvent))
		{
			return false;
		}
		htEvent.UpdateHeadTrackingData();
		return true;
	}

	public void UpdateConfiguration(HeadTrackingConfiguration config)
	{
		if (config == null)
		{
			Debug.LogWarning("Unable to update HeadTrackingConfiguration.");
		}
		else
		{
			Amazon_HeadTrackingPoller_updateConfiguration(nativeHeadTrackingPoller, config.nativeHeadTrackingConfiguration);
		}
	}
}
