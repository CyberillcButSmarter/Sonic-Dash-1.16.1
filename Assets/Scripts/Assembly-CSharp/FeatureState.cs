using System;
using System.Collections;
using UnityEngine;

public class FeatureState : MonoBehaviour
{
	[Flags]
	private enum State
	{
		Ready = 1
	}

	private static FeatureState s_featureState;

	private static LSON.Root[] s_defaultRoot;

	private static LSON.Root[] s_abRoot;

	private static State s_state;

	public static bool Ready
	{
		get
		{
			return (s_state & State.Ready) == State.Ready;
		}
	}

	public static bool Valid
	{
		get
		{
			return s_defaultRoot != null || s_abRoot != null;
		}
	}

	public static LSON.Property GetStateProperty(string rootName, string propertyName)
	{
		LSON.Property propertyFromRoot = s_featureState.GetPropertyFromRoot(s_abRoot, rootName, propertyName);
		if (propertyFromRoot == null)
		{
			propertyFromRoot = s_featureState.GetPropertyFromRoot(s_defaultRoot, rootName, propertyName);
		}
		return propertyFromRoot;
	}

	public static void Restart()
	{
		s_defaultRoot = null;
		s_abRoot = null;
		s_state = (State)0;
		s_featureState.StartCoroutine(s_featureState.DownloadServerFile());
	}

	private void Start()
	{
		s_featureState = this;
	}

	private LSON.Property GetPropertyFromRoot(LSON.Root[] root, string rootName, string propertyName)
	{
		if (root == null)
		{
			return null;
		}
		LSON.Property[] properties = LSONProperties.GetProperties(root, rootName);
		if (properties == null || properties.Length == 0)
		{
			return null;
		}
		return LSONProperties.GetProperty(properties, propertyName);
	}

	private IEnumerator DownloadServerFile()
	{
		while (!ABTesting.URLReady)
		{
			yield return null;
		}
		FileDownloader abFile = new FileDownloader(FileDownloader.Files.FeatureStateAB, true);
		yield return abFile.Loading;
		FileDownloader defaultFile = new FileDownloader(FileDownloader.Files.FeatureStateDefault, true);
		yield return defaultFile.Loading;
		if (string.IsNullOrEmpty(abFile.Error) || string.IsNullOrEmpty(defaultFile.Error))
		{
			if (string.IsNullOrEmpty(abFile.Error))
			{
				s_abRoot = LSONReader.Parse(abFile.Text);
			}
			if (string.IsNullOrEmpty(defaultFile.Error))
			{
				s_defaultRoot = LSONReader.Parse(defaultFile.Text);
			}
		}
		s_state |= State.Ready;
		EventDispatch.GenerateEvent("FeatureStateReady");
	}
}
