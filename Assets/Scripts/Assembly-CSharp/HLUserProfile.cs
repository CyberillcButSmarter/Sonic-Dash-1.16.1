using UnityEngine.SocialPlatforms.Impl;

public class HLUserProfile : UserProfile
{
	public enum ProfileSource
	{
		Facebook = 0,
		GameCenter = 1,
		GooglePlay = 2,
		Multiple = 3,
		Max = 4
	}

	public ProfileSource Source { get; private set; }

	public HLUserProfile()
	{
		Source = ProfileSource.Max;
	}

	public void SetSource(ProfileSource src)
	{
		Source = src;
	}
}
