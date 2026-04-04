using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class HeadTrackingConfiguration
{
	public enum FilterType
	{
		HT_ONLY = 0,
		HT_SENSOR_FUSION = 1
	}

	public enum Fidelity
	{
		LOW_POWER = 1,
		NORMAL = 2
	}

	public IntPtr nativeHeadTrackingConfiguration;

	private FilterType filterType = FilterType.HT_SENSOR_FUSION;

	private Fidelity fidelity = Fidelity.NORMAL;

	private HeadTrackingConfiguration(IntPtr nativeConfig)
	{
		nativeHeadTrackingConfiguration = nativeConfig;
	}

	[DllImport("headtracking_client")]
	private static extern IntPtr Amazon_HeadTrackingConfiguration_createInstance();

	[DllImport("headtracking_client")]
	private static extern void Amazon_HeadTrackingConfiguration_setFidelity(IntPtr headTrackingConfiguration, int fidelity);

	[DllImport("headtracking_client")]
	private static extern void Amazon_HeadTrackingConfiguration_setFilterType(IntPtr headTrackingConfiguration, int filterType);

	public void SetFidelity(Fidelity fidelity)
	{
		if (nativeHeadTrackingConfiguration != IntPtr.Zero)
		{
			Amazon_HeadTrackingConfiguration_setFidelity(nativeHeadTrackingConfiguration, (int)fidelity);
			this.fidelity = fidelity;
		}
	}

	public void SetFilterType(FilterType filterType)
	{
		if (nativeHeadTrackingConfiguration != IntPtr.Zero)
		{
			Amazon_HeadTrackingConfiguration_setFilterType(nativeHeadTrackingConfiguration, (int)filterType);
			this.filterType = filterType;
		}
	}

	public Fidelity GetFidelity()
	{
		return fidelity;
	}

	public FilterType GetFilterType()
	{
		return filterType;
	}

	public static HeadTrackingConfiguration CreateInstance()
	{
		IntPtr intPtr = Amazon_HeadTrackingConfiguration_createInstance();
		if (intPtr == IntPtr.Zero)
		{
			Debug.LogError("Could not create native HeadTrackingConfiguration");
			return null;
		}
		return new HeadTrackingConfiguration(intPtr);
	}
}
