using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class TapGesture : BaseGesture
{
	public int taps;

	public float maxTimeBetweensTaps = 0.4f;

	public float maxTapDistance = 25f;

	public int tapRateTapsCount = 2;

	public FingerLocation startsOnObject;

	public FingerLocation movesOnObject;

	public FingerLocation endsOnObject;

	public bool enforceStationary;

	public float tapsPerMinute;

	private int tapsReceived;

	private float lastTime;

	private float tapStartTime;

	private List<float> tapTimes;

	private Timer tapTimer = new Timer();

	private float _maxTimeBetweensTaps;

	private bool gestureIsGoing;

	protected override void EnableGesture()
	{
		base.EnableGesture();
		tapTimer.AutoReset = false;
		tapTimer.Elapsed += OnTimedEvent;
		tapTimes = new List<float>();
		FingerControl._delegateFingerUpInternal += TapGestureFingerUp;
		FingerControl._delegateFingerMovingInternal += TapGestureDownAndMovingMove;
		FingerControl._delegateFingerDownInternal += TapGestureFingerDown;
	}

	protected override void DisableGesture()
	{
		base.DisableGesture();
		FingerControl._delegateFingerUpInternal -= TapGestureFingerUp;
		FingerControl._delegateFingerMovingInternal -= TapGestureDownAndMovingMove;
		FingerControl._delegateFingerDownInternal -= TapGestureFingerDown;
	}

	protected void TapGestureFingerDown(Finger fingerIn)
	{
		_maxTimeBetweensTaps = maxTimeBetweensTaps;
		if (_maxTimeBetweensTaps < 0.0001f)
		{
			_maxTimeBetweensTaps = 0.0001f;
		}
		if (FingerActivated(startsOnObject, fingerIn.position))
		{
			gestureIsGoing = true;
		}
	}

	protected void TapGestureFingerUp(Finger fingerIn)
	{
		if (gestureIsGoing && FingerActivated(endsOnObject, fingerIn.position) && (!enforceStationary || fingerIn.onlyStationary))
		{
			gestureIsGoing = false;
			if (lastTime > 0f && Time.time - lastTime > _maxTimeBetweensTaps)
			{
				clearTaps();
			}
			if (tapsReceived == 0)
			{
				tapTimes.Clear();
			}
			if ((fingerIn.position - fingerIn.startPosition).magnitude > maxTapDistance)
			{
				tapTimes.Clear();
				return;
			}
			tapsReceived++;
			tapTimes.Add(Time.time);
			if (tapsReceived == 1)
			{
				tapsPerMinute = 0f;
			}
			else if (tapsReceived < tapRateTapsCount)
			{
				tapsPerMinute = 1f / ((Time.time - tapTimes[0]) / (float)tapsReceived) * 60f;
			}
			else
			{
				tapsPerMinute = 1f / ((Time.time - tapTimes[0]) / (float)tapRateTapsCount) * 60f;
				tapTimes.RemoveAt(0);
			}
			lastTime = Time.time;
			if (tapsReceived == taps || taps == 0)
			{
				finger = fingerIn;
				GestureMessage("GestureTap");
			}
			tapTimer.Stop();
			tapTimer.Interval = _maxTimeBetweensTaps * 1000f;
			tapTimer.Start();
		}
		else
		{
			clearTaps();
		}
	}

	protected void TapGestureDownAndMovingMove(Finger fingerIn)
	{
		if (!enforceStationary && gestureIsGoing && !FingerActivated(movesOnObject, fingerIn.position))
		{
			gestureIsGoing = false;
		}
	}

	private void OnTimedEvent(object source, ElapsedEventArgs e)
	{
		clearTaps();
	}

	private void clearTaps()
	{
		tapTimer.Stop();
		tapTimes.Clear();
		tapsPerMinute = 0f;
		tapsReceived = 0;
		lastTime = 0f;
	}
}
