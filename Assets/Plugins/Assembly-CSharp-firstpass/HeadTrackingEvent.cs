using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class HeadTrackingEvent
{
	public IntPtr nativeHeadTrackingEvent;

	public float x;

	public float y;

	public float z;

	public float headInclinationAngle;

	public ScreenOrientation orientation;

	public bool isTracking;

	public bool isFaceDetected;

	public long timestamp;

	public static bool ROTATE_EVENT_TO_SCREEN_ORIENTATION = true;

	private HeadTrackingEvent(IntPtr nativeEvent)
	{
		nativeHeadTrackingEvent = nativeEvent;
	}

	public HeadTrackingEvent()
	{
		nativeHeadTrackingEvent = IntPtr.Zero;
	}

	[DllImport("headtracking_client")]
	private static extern IntPtr Amazon_HeadTrackingEvent_createInstance();

	[DllImport("headtracking_client")]
	private static extern void Amazon_HeadTrackingEvent_extractHeadTrackingData(IntPtr headTrackingEvent, out float x_mm, out float y_mm, out float z_mm, out float headInclinationAngle_deg, out bool isTracking, out bool isFaceDetected, out long timestamp_nsecs);

	public void UpdateHeadTrackingData()
	{
		Amazon_HeadTrackingEvent_extractHeadTrackingData(nativeHeadTrackingEvent, out x, out y, out z, out headInclinationAngle, out isTracking, out isFaceDetected, out timestamp);
		if (isTracking)
		{
			x /= 1000f;
			y /= 1000f;
			z /= 1000f;
			orientation = ScreenOrientation.Portrait;
			if (ROTATE_EVENT_TO_SCREEN_ORIENTATION)
			{
				RotateEventToScreenOrientation();
			}
		}
	}

	private void RotateEventToScreenOrientation()
	{
		if (Screen.orientation != ScreenOrientation.Portrait && orientation == ScreenOrientation.Portrait)
		{
			if (Screen.orientation == ScreenOrientation.LandscapeLeft)
			{
				float num = x;
				x = 0f - y;
				y = num;
				headInclinationAngle += 270f;
				orientation = ScreenOrientation.LandscapeLeft;
			}
			else if (Screen.orientation == ScreenOrientation.LandscapeRight)
			{
				float num2 = x;
				x = y;
				y = 0f - num2;
				headInclinationAngle += 90f;
				orientation = ScreenOrientation.LandscapeRight;
			}
			else
			{
				y = 0f - y;
				x = 0f - x;
				headInclinationAngle += 180f;
				orientation = ScreenOrientation.PortraitUpsideDown;
			}
			if (headInclinationAngle > 180f)
			{
				headInclinationAngle -= 360f;
			}
		}
	}

	public static HeadTrackingEvent CreateInstance()
	{
		IntPtr intPtr = Amazon_HeadTrackingEvent_createInstance();
		if (intPtr == IntPtr.Zero)
		{
			Debug.LogError("Could not create native HeadTrackingEvent");
			return null;
		}
		return new HeadTrackingEvent(intPtr);
	}
}
