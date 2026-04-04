using UnityEngine;

public class MenuOptionsPanel : MonoBehaviour
{
	public UIToggle checkboxMusic;

	public UIToggle checkboxSfx;

	public UIToggle checkboxVideoRecorder;

	public UIToggle m_checkboxTutorial;

	private void Start()
	{
		EventDispatch.RegisterInterest("OnGameDataLoaded", this, EventDispatch.Priority.Lowest);
	}

	private void Update()
	{
	}

	private void Event_OnGameDataLoaded(ActiveProperties activeProperties)
	{
		if ((bool)checkboxMusic)
		{
			if (activeProperties.DoesPropertyExist("MusicVolume") && activeProperties.GetFloat("MusicVolume") == 0f)
			{
				checkboxMusic.startsActive = false;
			}
			else
			{
				checkboxMusic.startsActive = true;
			}
		}
		if ((bool)checkboxSfx)
		{
			if (activeProperties.DoesPropertyExist("SfxVolume") && activeProperties.GetFloat("SfxVolume") == 0f)
			{
				checkboxSfx.startsActive = false;
			}
			else
			{
				checkboxSfx.startsActive = true;
			}
		}
		if ((bool)checkboxVideoRecorder)
		{
			checkboxVideoRecorder.startsActive = VideoRecorder.Enabled;
		}
		if (activeProperties.DoesPropertyExist("ShowTutorial") && (bool)m_checkboxTutorial)
		{
			UIToggle uIToggle = m_checkboxTutorial.GetComponent("UIToggle") as UIToggle;
			if (activeProperties.GetInt("ShowTutorial") > 0)
			{
				uIToggle.startsActive = true;
			}
			else
			{
				uIToggle.startsActive = false;
			}
		}
	}
}
