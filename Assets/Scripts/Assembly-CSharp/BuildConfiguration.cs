public class BuildConfiguration
{
	public enum Build
	{
		Development = 1,
		FinalRelease = 2,
		Distribution = 4,
		Review = 8
	}

	public enum Android
	{
		Google = 1,
		AmazonKindle = 2,
		AmazonPhone = 4
	}

	public static Build DefaultBuild = Build.Development;

	public static Android DefaultAndroidPlatform = Android.Google;

	public static Build Current
	{
		get
		{
			return Build.Distribution;
		}
	}

	public static Android AndroidPlatform
	{
		get
		{
			return Android.Google;
		}
	}
}
