public class VersionIdentifiers
{
	public enum VersionStatus
	{
		Equal = 0,
		Higher = 1,
		Lower = 2
	}

	public enum Components
	{
		Major = 0,
		Minor = 1,
		Point = 2
	}

	public const string Version = "1.16.1";

	public const string Assets = "1.16.1";

	public const string BundleIdentifier = "com.sega.sonicdash";

	public const string BundleVersion = "1410011602";

	public static VersionStatus CheckVersionNumbers(string versionToCheck, string expectedVersion, Components checkDepth)
	{
		string[] array = expectedVersion.Split('.');
		string[] array2 = versionToCheck.Split('.');
		int[] array3 = new int[3]
		{
			int.Parse(array[0]),
			int.Parse(array[1]),
			int.Parse(array[2])
		};
		int[] array4 = new int[3]
		{
			int.Parse(array2[0]),
			int.Parse(array2[1]),
			int.Parse(array2[2])
		};
		int num = (int)(checkDepth + 1);
		for (int i = 0; i < num; i++)
		{
			if (array4[i] > array3[i])
			{
				return VersionStatus.Higher;
			}
			if (array4[i] < array3[i])
			{
				return VersionStatus.Lower;
			}
		}
		return VersionStatus.Equal;
	}
}
