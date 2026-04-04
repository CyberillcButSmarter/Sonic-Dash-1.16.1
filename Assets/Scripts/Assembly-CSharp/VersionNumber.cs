using UnityEngine;

public class VersionNumber : MonoBehaviour
{
	private void Start()
	{
		Object.DontDestroyOnLoad(this);
		string format = LanguageStrings.First.GetString("OPTIONS_VERSION");
		string text = string.Format(format, "1.16.1");
		UILabel component = GetComponent<UILabel>();
		if ((bool)component)
		{
			component.text = text;
		}
	}
}
