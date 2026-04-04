using UnityEngine;

public class SettingsKamcordDisplayer : MonoBehaviour
{
	public GameObject m_kamcordCheckbox;

	public void OnEnable()
	{
		m_kamcordCheckbox.SetActive(VideoRecorder.Supported);
	}
}
