using System;
using System.Collections.Generic;
using UnityEngine;

public class RecordingGUI : MonoBehaviour
{
	public Font buttonFont;

	private bool firstVideoRecorded;

	private Rect recordingButtonRect;

	private Rect showViewButtonRect;

	private Rect showWatchViewButtonRect;

	private DateTime recordingStartedAt;

	private bool isPaused;

	private void Start()
	{
		firstVideoRecorded = false;
		recordingButtonRect = new Rect(20f, 20f, 200f, 60f);
		showViewButtonRect = new Rect(recordingButtonRect.x, 2f * recordingButtonRect.y + recordingButtonRect.height, recordingButtonRect.width, recordingButtonRect.height);
		showWatchViewButtonRect = new Rect(2f * recordingButtonRect.x + recordingButtonRect.width, recordingButtonRect.y, recordingButtonRect.width, recordingButtonRect.height);
		Kamcord.snapshotReadyAtFilePath += SnapshotReadyAtFilePath;
		Kamcord.kamcordViewDidAppear += KamcordViewDidAppear;
		Kamcord.kamcordViewWillDisappear += KamcordViewWillDisappear;
		Kamcord.kamcordViewDidDisappear += KamcordViewDidDisappear;
		Kamcord.kamcordWatchViewDidAppear += KamcordWatchViewDidAppear;
		Kamcord.kamcordWatchViewWillDisappear += KamcordWatchViewWillDisappear;
		Kamcord.kamcordWatchViewDidDisappear += KamcordWatchViewDidDisappear;
		Kamcord.shareButtonPressed += ShareButtonPressed;
		Kamcord.videoWillBeginUploading += VideoWillBeginUploading;
		Kamcord.videoUploadProgressed += VideoUploadProgressed;
		Kamcord.videoFinishedUploading += VideoFinishedUploading;
		Kamcord.videoSharedTo += VideoSharedTo;
		Kamcord.videoSharedToFacebook += VideoSharedToFacebook;
		Kamcord.videoSharedToTwitter += VideoSharedToTwitter;
		Kamcord.videoSharedToYoutube += VideoSharedToYoutube;
		Kamcord.snapshotReadyAtFilePath += SnapshotReadyAtFilePath;
		Kamcord.pushNotifCallToActionButtonPressed += PushNotifCallToActionButtonPressed;
	}

	private void OnDestroy()
	{
		Kamcord.snapshotReadyAtFilePath -= SnapshotReadyAtFilePath;
		Kamcord.kamcordViewDidAppear -= KamcordViewDidAppear;
		Kamcord.kamcordViewWillDisappear -= KamcordViewWillDisappear;
		Kamcord.kamcordViewDidDisappear -= KamcordViewDidDisappear;
		Kamcord.kamcordWatchViewDidAppear -= KamcordWatchViewDidAppear;
		Kamcord.kamcordWatchViewWillDisappear -= KamcordWatchViewWillDisappear;
		Kamcord.kamcordWatchViewDidDisappear -= KamcordWatchViewDidDisappear;
		Kamcord.videoThumbnailReadyAtFilePath -= VideoThumbnailReadyAtFilePath;
		Kamcord.shareButtonPressed -= ShareButtonPressed;
		Kamcord.videoWillBeginUploading -= VideoWillBeginUploading;
		Kamcord.videoUploadProgressed -= VideoUploadProgressed;
		Kamcord.videoFinishedUploading -= VideoFinishedUploading;
		Kamcord.videoSharedToFacebook -= VideoSharedToFacebook;
		Kamcord.videoSharedToTwitter -= VideoSharedToTwitter;
		Kamcord.snapshotReadyAtFilePath -= SnapshotReadyAtFilePath;
		Kamcord.pushNotifCallToActionButtonPressed -= PushNotifCallToActionButtonPressed;
	}

	private void OnGUI()
	{
		GUI.skin.button.font = buttonFont;
		if (Application.platform == RuntimePlatform.IPhonePlayer && GUI.Button(showWatchViewButtonRect, "Show Watch View"))
		{
			Kamcord.ShowWatchView();
		}
		if (Kamcord.IsRecording())
		{
			firstVideoRecorded = true;
			if (GUI.Button(recordingButtonRect, "Stop Recording"))
			{
				Kamcord.StopRecording();
				double totalSeconds = (DateTime.Now - recordingStartedAt).TotalSeconds;
				Kamcord.SetVideoTitle("AngryBots Gameplay - " + totalSeconds.ToString("F2") + " sec");
				Kamcord.SetLevelAndScore("Level 1", totalSeconds);
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("key1", 1);
				dictionary.Add("key2", 2);
				dictionary.Add("level", "Super Saiyan");
				dictionary.Add("score", 9000);
				Kamcord.SetVideoMetadata(dictionary);
			}
		}
		else if (Kamcord.IsEnabled())
		{
			if (GUI.Button(recordingButtonRect, "Start Recording"))
			{
				Kamcord.StartRecording();
				recordingStartedAt = DateTime.Now;
			}
			if (firstVideoRecorded && GUI.Button(showViewButtonRect, "Show Last Video"))
			{
				Kamcord.ShowView();
			}
		}
	}

	public void KamcordViewDidAppear()
	{
		Debug.Log("KamcordViewDidAppear");
	}

	public void KamcordViewWillDisappear()
	{
		Debug.Log("KamcordViewWillDisappear");
	}

	public void KamcordViewDidDisappear()
	{
		Debug.Log("KamcordViewDidDisappear");
	}

	public void KamcordWatchViewDidAppear()
	{
		Debug.Log("KamcordWatchViewDidAppear");
	}

	public void KamcordWatchViewWillDisappear()
	{
		Debug.Log("KamcordWatchViewWillDisappear");
	}

	public void KamcordWatchViewDidDisappear()
	{
		Debug.Log("KamcordWatchViewDidDisappear");
	}

	private void VideoThumbnailReadyAtFilePath(string filepath)
	{
		Debug.Log("Thumbnail ready at: " + filepath);
	}

	private void ShareButtonPressed()
	{
		Debug.Log("ShareButtonPressed.");
	}

	private void VideoWillBeginUploading(string videoID, string url)
	{
		Debug.Log("Video " + videoID + " will begin uploading: " + url);
	}

	private void VideoUploadProgressed(string videoID, float progress)
	{
		Debug.Log("Video " + videoID + " upload progressed: " + progress);
	}

	private void VideoFinishedUploading(string videoID, bool success)
	{
		Debug.Log("Video " + videoID + " finished uploading: " + success);
	}

	private void VideoSharedTo(string videoID, string network, bool success)
	{
		Debug.Log(videoID + " was shared to " + network + "(" + success + ")");
	}

	private void VideoSharedToFacebook(string videoID, bool success)
	{
		Debug.Log("VideoSharedToFacebook: " + videoID);
	}

	private void VideoSharedToTwitter(string videoID, bool success)
	{
		Debug.Log("VideoSharedToTwitter: " + videoID);
	}

	private void VideoSharedToYoutube(string videoID, bool success)
	{
		Debug.Log("VideoSharedToYoutube: " + videoID);
	}

	public void SnapshotReadyAtFilePath(string filepath)
	{
		Debug.Log("Snapshot ready at filepath: " + filepath);
	}

	private void PushNotifCallToActionButtonPressed()
	{
		Debug.Log("PushNotifCallToActionButtonPressed");
	}
}
