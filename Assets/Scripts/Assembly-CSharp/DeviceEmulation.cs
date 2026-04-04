using UnityEngine;

public class DeviceEmulation : MonoBehaviour
{
	[SerializeField]
	private bool m_emulateDevice;

	[SerializeField]
	private SupportedDevices.iOS m_iosDevice;

	public bool Emulate
	{
		get
		{
			return m_emulateDevice;
		}
	}

	public SupportedDevices.iOS iOSDevice
	{
		get
		{
			return m_iosDevice;
		}
	}
}
