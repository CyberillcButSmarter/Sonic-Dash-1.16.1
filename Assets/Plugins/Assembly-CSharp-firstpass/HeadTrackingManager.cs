using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class HeadTrackingManager
{
	private IntPtr nativeHeadTrackingManager;

	private static HeadTrackingManager headTrackingManager;

	private HeadTrackingManager()
	{
		nativeHeadTrackingManager = Amazon_HeadTrackingManager_getInstance();
	}

	[DllImport("headtracking_client")]
	private static extern IntPtr Amazon_HeadTrackingManager_getInstance();

	[DllImport("headtracking_client")]
	private static extern IntPtr Amazon_HeadTrackingManager_createPoller(IntPtr headtrackingManager);

	[DllImport("headtracking_client")]
	private static extern IntPtr Amazon_HeadTrackingManager_createPollerWithConfig(IntPtr headtrackingManager, IntPtr headtrackingConfiguration);

	[DllImport("headtracking_client")]
	private static extern void Amazon_HeadTrackingManager_releasePoller(IntPtr headtrackingManager, IntPtr headtrackingPoller);

	[DllImport("headtracking_client")]
	private static extern void Amazon_HeadTrackingManager_requestStandby(IntPtr headtrackingManager, bool standby);

	public static HeadTrackingManager CreateInstance()
	{
		if (headTrackingManager == null)
		{
			headTrackingManager = new HeadTrackingManager();
			if (headTrackingManager.nativeHeadTrackingManager == IntPtr.Zero)
			{
				Debug.LogError("Could not obtain the native HeadTrackingManager");
				headTrackingManager = null;
			}
		}
		return headTrackingManager;
	}

	public HeadTrackingPoller CreatePoller()
	{
		if (nativeHeadTrackingManager == IntPtr.Zero)
		{
			Debug.LogWarning("The native HeadTrackingManager is not initialized. Unable to create poller.");
			return null;
		}
		IntPtr intPtr = Amazon_HeadTrackingManager_createPoller(nativeHeadTrackingManager);
		if (intPtr == IntPtr.Zero)
		{
			Debug.LogWarning("Unable to obtain native HeadTrackingPoller.");
			return null;
		}
		return new HeadTrackingPoller(intPtr);
	}

	public HeadTrackingPoller CreatePoller(HeadTrackingConfiguration config)
	{
		if (nativeHeadTrackingManager == IntPtr.Zero)
		{
			Debug.LogWarning("The native HeadTrackingManager is not initialized. Unable to create poller.");
			return null;
		}
		IntPtr intPtr;
		if (config == null)
		{
			Debug.LogWarning("The provided HeadTrackingConfiguration is not initialized properly. Using the default HeadTrackingConfiguration instead.");
			intPtr = Amazon_HeadTrackingManager_createPoller(nativeHeadTrackingManager);
		}
		else
		{
			intPtr = Amazon_HeadTrackingManager_createPollerWithConfig(nativeHeadTrackingManager, config.nativeHeadTrackingConfiguration);
		}
		if (intPtr == IntPtr.Zero)
		{
			Debug.LogWarning("Unable to obtain native HeadTrackingPoller.");
			return null;
		}
		return new HeadTrackingPoller(intPtr);
	}

	public void ReleasePoller(HeadTrackingPoller poller)
	{
		if (nativeHeadTrackingManager == IntPtr.Zero)
		{
			Debug.LogWarning("The native HeadTrackingManager is not initialized. Unable to release poller.");
		}
		else if (poller == null)
		{
			Debug.LogWarning("The provided poller is not initialized. Unable to release poller.");
		}
		else
		{
			Amazon_HeadTrackingManager_releasePoller(nativeHeadTrackingManager, poller.nativeHeadTrackingPoller);
		}
	}

	public void RequestStandby(bool standby)
	{
		if (nativeHeadTrackingManager == IntPtr.Zero)
		{
			Debug.LogWarning("The native HeadTrackingManager is not initialized. Cancelling standby request.");
		}
		else
		{
			Amazon_HeadTrackingManager_requestStandby(nativeHeadTrackingManager, standby);
		}
	}
}
