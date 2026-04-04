public class DeepLink
{
	private string deeplinkURL;

	private string pageName;

	private static DeepLink m_deepLink;

	public bool HasBeenUsed;

	public DeepLink(string _deeplinkURL)
	{
		deeplinkURL = _deeplinkURL;
	}

	public static DeepLink GetDeepLink()
	{
		if (m_deepLink == null || m_deepLink.IsEmpty() || m_deepLink.HasBeenUsed)
		{
			m_deepLink = new DeepLink(SLPlugin.GetStoredURL());
		}
		return m_deepLink;
	}

	public bool HasPageName(out string pageName)
	{
		if (deeplinkURL == null || deeplinkURL == string.Empty)
		{
			pageName = string.Empty;
			return false;
		}
		string text = "Page:";
		if (deeplinkURL.Length > text.Length)
		{
			int num = deeplinkURL.IndexOf(text);
			if (num >= 0)
			{
				int num2 = num + text.Length;
				pageName = deeplinkURL.Substring(num2, deeplinkURL.Length - num2);
				return true;
			}
		}
		pageName = string.Empty;
		return false;
	}

	public bool IsEmpty()
	{
		if (deeplinkURL == null || deeplinkURL == string.Empty)
		{
			return true;
		}
		return false;
	}
}
