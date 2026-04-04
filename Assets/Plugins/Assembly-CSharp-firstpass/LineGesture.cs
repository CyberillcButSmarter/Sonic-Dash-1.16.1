using System;
using System.Collections.Generic;
using UnityEngine;

public class LineGesture : BaseGesture
{
	public enum LineSwipeDirection
	{
		Forward = 0,
		Backward = 1,
		Either = 2,
		Anywhere = 3
	}

	public enum LineIdentification
	{
		Precise = 0,
		Clean = 1,
		Sloppy = 2
	}

	public delegate void LineCallBack(LineGesture gesture);

	public LineSwipeDirection restrictLineSwipeDirection = LineSwipeDirection.Anywhere;

	public LineFactory.LineType[] lineFactoryLineType;

	public FingerLocation startsOnObject = FingerLocation.Always;

	public FingerLocation endsOnObject = FingerLocation.Always;

	public LineIdentification lineIdentification = LineIdentification.Sloppy;

	public bool returnSwipeAlways;

	public float matchPositionDiff = 50f;

	public float matchLengthDiffPercent = 0.5f;

	public float maxTimeBetweenLines = 0.8f;

	public SwipeSegmentList swipeSegments;

	public LineFactory.LineType swipedLineType;

	public List<SwipeSegmentList> swipeList = new List<SwipeSegmentList>();

	public LineIdentification lineIdentificationUsed;

	private bool thereAreCompound;

	private int maxCompoundLines;

	private int maxCompoundLineSegments;

	private Vector2 fingerStartPos;

	private Vector2 fingerEndPos;

	private float endSwipeTime = -1f;

	private Finger lastFinger;

	private bool finishedSwipe = true;

	public bool performingSwipe;

	public SwipeSegmentList compressedSwipeSegments;

	public SwipeSegmentList[] compressedSwipeSegmentsList;

	public LineSwipeBase usedLineSwipe;

	public string errorString;

	protected bool useLineFactory;

	protected LineFactory lineFactory;

	protected event LineCallBack eventHandlers_Line;

	public bool HaveLineFactoryType(LineFactory.LineType lineType)
	{
		if (lineFactoryLineType == null || lineFactoryLineType.Length == 0)
		{
			return false;
		}
		for (int i = 0; i < lineFactoryLineType.Length; i++)
		{
			if (lineFactoryLineType[i] == lineType)
			{
				return true;
			}
		}
		return false;
	}

	public LineSwipeBase GetLineSwipeDef(LineFactory.LineType lineType)
	{
		UseLineFactory();
		return lineFactory.GetLineSwipe(lineType);
	}

	public int GetFactoryTypesCount()
	{
		if (lineFactoryLineType == null)
		{
			return 0;
		}
		return lineFactoryLineType.Length;
	}

	public void AddLineFactoryType(LineFactory.LineType lineType, bool clearList)
	{
		UseLineFactory();
		if (lineType == LineFactory.LineType.None)
		{
			clearList = true;
		}
		if (clearList)
		{
			thereAreCompound = false;
			maxCompoundLines = 0;
			maxCompoundLineSegments = 0;
			if (lineType == LineFactory.LineType.None)
			{
				lineFactoryLineType = null;
				return;
			}
			lineFactoryLineType = new LineFactory.LineType[1];
			lineFactoryLineType[0] = lineType;
		}
		else if (!HaveLineFactoryType(lineType))
		{
			LineFactory.LineType[] array = lineFactoryLineType;
			int num = 0;
			if (array != null)
			{
				num = array.Length;
			}
			lineFactoryLineType = new LineFactory.LineType[num + 1];
			if (array != null && array.Length > 0)
			{
				array.CopyTo(lineFactoryLineType, 0);
			}
			lineFactoryLineType[num] = lineType;
			VerifyType(lineType);
		}
	}

	private void VerifyType(LineFactory.LineType lineType)
	{
		UseLineFactory();
		if (lineFactory.IsCompound(lineType))
		{
			thereAreCompound = true;
			int count = lineFactory.GetCount(lineType);
			if (count > maxCompoundLines)
			{
				maxCompoundLines = count;
			}
			count = lineFactory.GetMaxSegmentCount(lineType);
			if (count > maxCompoundLineSegments)
			{
				maxCompoundLineSegments = count;
			}
		}
	}

	public void RemoveLineFactoryType(LineFactory.LineType lineType)
	{
		if (!useLineFactory || lineFactoryLineType == null || lineFactoryLineType.Length == 0)
		{
			return;
		}
		int num = -1;
		for (int i = 0; i < lineFactoryLineType.Length; i++)
		{
			if (lineFactoryLineType[i] == lineType)
			{
				num = i;
				break;
			}
		}
		if (num < 0)
		{
			return;
		}
		LineFactory.LineType[] array = lineFactoryLineType;
		lineFactoryLineType = new LineFactory.LineType[array.Length - 1];
		int num2 = 0;
		for (int j = 0; j < array.Length; j++)
		{
			if (j != num)
			{
				lineFactoryLineType[num2++] = array[j];
			}
		}
	}

	public void RegisterLineCallback(LineCallBack callback)
	{
		this.eventHandlers_Line = (LineCallBack)Delegate.Combine(this.eventHandlers_Line, callback);
	}

	protected override void EnableGesture()
	{
		base.EnableGesture();
		if (useLineFactory)
		{
			UseLineFactory();
		}
		FingerControl.AddSwipeCallback();
		FingerControl._delegateSwipeInternal += SwipeGestureOnSwipe;
		if (lineFactoryLineType != null)
		{
			for (int i = 0; i < lineFactoryLineType.Length; i++)
			{
				VerifyType(lineFactoryLineType[i]);
			}
		}
	}

	protected override void DisableGesture()
	{
		base.DisableGesture();
		FingerControl.RemoveSwipeCallback();
		FingerControl._delegateSwipeInternal -= SwipeGestureOnSwipe;
	}

	protected void SwipeGestureOnSwipe(Finger[] fingers, SwipeSegmentList[] segmentsList, int fingerCount)
	{
		if (finishedSwipe)
		{
			ClearSwipe();
			swipeList.Clear();
		}
		if (fingerCount > 1 || segmentsList == null || segmentsList.Length == 0)
		{
			return;
		}
		SwipeSegmentList item = segmentsList[0];
		Finger finger = fingers[0];
		if (swipeList.Count == 0)
		{
			fingerStartPos = finger.startPosition;
		}
		fingerEndPos = finger.endPosition;
		performingSwipe = true;
		swipeList.Add(item);
		lastFinger = finger;
		if (thereAreCompound)
		{
			if (swipeList.Count < maxCompoundLines)
			{
				endSwipeTime = Time.time + maxTimeBetweenLines;
				finishedSwipe = false;
				return;
			}
			ClearSwipeTime();
		}
		EvaluateSwipe();
	}

	protected void EvaluateSwipe()
	{
		finishedSwipe = true;
		if (swipeList == null || swipeList.Count == 0)
		{
			return;
		}
		usedLineSwipe = null;
		performingSwipe = false;
		if (FingerActivated(startsOnObject, fingerStartPos) && FingerActivated(endsOnObject, fingerEndPos))
		{
			if (swipeList.Count == 1)
			{
				swipeSegments = swipeList[0];
			}
			else
			{
				swipeSegments = null;
			}
			swipedLineType = FindLineType(swipeList);
			if (swipedLineType == LineFactory.LineType.None && !returnSwipeAlways)
			{
				GestureMessage("GestureLineSwipeFailure");
				return;
			}
			finger = lastFinger;
			if (this.eventHandlers_Line != null)
			{
				this.eventHandlers_Line(this);
			}
			if (lineFactory != null)
			{
				if (swipedLineType == LineFactory.LineType.None)
				{
					if (GetFactoryTypesCount() != 1)
					{
						usedLineSwipe = lineFactory.GetLineSwipe(lineFactoryLineType[0]).GetUsedLineSwipe();
					}
				}
				else
				{
					usedLineSwipe = lineFactory.GetLineSwipe(swipedLineType).GetUsedLineSwipe();
				}
			}
			if (usedLineSwipe != null)
			{
				compressedSwipeSegments = usedLineSwipe.compressedSwipeSegments;
				compressedSwipeSegmentsList = usedLineSwipe.compressedSwipeSegmentsList;
			}
			if (swipedLineType == LineFactory.LineType.None)
			{
				errorString = LineSwipeBase.lastError;
				if (GetFactoryTypesCount() != 1)
				{
					usedLineSwipe = null;
					compressedSwipeSegments = null;
					compressedSwipeSegmentsList = null;
				}
			}
			else
			{
				errorString = string.Empty;
			}
			GestureMessage("GestureLineSwipe");
		}
		else
		{
			swipedLineType = LineFactory.LineType.None;
			swipeSegments = null;
			GestureMessage("GestureLineSwipeFailure");
		}
	}

	protected void ClearSwipe()
	{
		swipeList.Clear();
		finishedSwipe = true;
		ClearSwipeTime();
	}

	protected void ClearSwipeTime()
	{
		endSwipeTime = -1f;
	}

	private void Update()
	{
		if (thereAreCompound && !(endSwipeTime <= 0f) && endSwipeTime < Time.time)
		{
			if (!FingerControl.IsSwiping())
			{
				EvaluateSwipe();
			}
			endSwipeTime = -1f;
		}
	}

	private LineFactory.LineType FindLineType(List<SwipeSegmentList> listOfSwipes)
	{
		if (lineFactory == null || lineFactoryLineType == null || lineFactoryLineType.Length == 0)
		{
			return LineFactory.LineType.None;
		}
		LineFactory.LineType lineType = lineFactory.FindLineType(lineFactoryLineType, listOfSwipes, lineIdentification, restrictLineSwipeDirection, matchPositionDiff, matchLengthDiffPercent);
		if (lineType != LineFactory.LineType.None)
		{
			LineSwipeBase lineSwipeDef = GetLineSwipeDef(lineType);
			lineIdentificationUsed = lineSwipeDef.identificationUsed;
		}
		return lineType;
	}

	private void UseLineFactory()
	{
		useLineFactory = true;
		if (lineFactory == null)
		{
			lineFactory = LineFactory.Factory();
		}
	}
}
