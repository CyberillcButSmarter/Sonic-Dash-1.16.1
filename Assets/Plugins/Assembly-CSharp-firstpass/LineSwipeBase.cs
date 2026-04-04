using System.Collections.Generic;
using UnityEngine;

public class LineSwipeBase
{
	public string name;

	public bool closed;

	public bool startAnywhere = true;

	public bool doCompareLengths = true;

	public bool biDirectional = true;

	public bool maintainAspectRatio = true;

	public static float SegmentMaxDistanceIgnore = 30f;

	public static float matchPositionDiffSloppyMultiplier = 4f;

	public static float matchLengthDiffPercentSloppyMultiplier = 3f;

	protected float matchPositionDiff;

	private float matchLengthDiffPercent;

	private bool firstLastMatch;

	private int extraSegment;

	protected SegmentList sourceSegments;

	private SwipeSegment[] matchedSegments;

	private float overRideUnitSize = -1f;

	public LineGesture.LineIdentification identificationUsed;

	public LineGesture.LineSwipeDirection restrictLineSwipeDirectionUsed;

	public SwipeSegmentList[] compressedSwipeSegmentsList;

	public bool isForwardUsed = true;

	public SwipeSegmentList compressedSwipeSegments;

	public SwipeSegmentList matchedSwipeSegments;

	public static string lastError;

	private int matchScore;

	private int compressedStartIndex;

	private int lookForwardDepth;

	public LineSwipeBase(string nameIn)
	{
		name = nameIn;
	}

	public virtual bool Compare(List<SwipeSegmentList> swipeList, LineGesture.LineSwipeDirection restrictLineSwipeDirection, float positionDiff, float lengthDiffPercent, LineGesture.LineIdentification lineIdentification)
	{
		return false;
	}

	public virtual bool Compare(SwipeSegmentList swipeSegments, LineGesture.LineSwipeDirection restrictLineSwipeDirection, float positionDiff, float lengthDiffPercent, LineGesture.LineIdentification lineIdentification)
	{
		return false;
	}

	public virtual bool IsCompound()
	{
		return false;
	}

	public virtual int Count()
	{
		return 1;
	}

	public virtual int GetMaxSegment()
	{
		return 1;
	}

	public virtual LineSwipeBase GetUsedLineSwipe()
	{
		return this;
	}

	public void SetOverRideUnitSize(float size)
	{
		overRideUnitSize = size;
	}

	protected void SetError(string str, bool force)
	{
		if (force || lastError == null || lastError.Length == 0)
		{
			if (str == null || str.Length == 0)
			{
				lastError = str;
				return;
			}
			lastError = string.Concat(name, " (", identificationUsed, ") - ", str);
		}
	}

	protected float GetMatchPositionDiff(LineGesture.LineIdentification identification, float defaultValue)
	{
		if (matchPositionDiff <= 0f)
		{
			matchPositionDiff = defaultValue;
		}
		if (identification == LineGesture.LineIdentification.Sloppy)
		{
			return matchPositionDiff * matchPositionDiffSloppyMultiplier;
		}
		return matchPositionDiff;
	}

	protected bool DirectionsMatch(LineGesture.LineIdentification identification, SegmentList sourceList, SwipeSegmentList swipeSegments, bool isForward)
	{
		if (sourceList.Count != swipeSegments.Count)
		{
			return false;
		}
		int index = ((!isForward) ? (swipeSegments.Count - 1) : 0);
		for (int i = 0; i < sourceList.Count; i++)
		{
			if (!sourceList[i].CompareDirection(identification, swipeSegments[index].direction, isForward))
			{
				return false;
			}
			index = incIndex(index, 1, isForward, swipeSegments.Count);
		}
		return true;
	}

	public bool CompareDirections(LineGesture.LineIdentification identification, SwipeSegmentList swipeSegments, bool isForward)
	{
		return DirectionsMatch(identification, sourceSegments, swipeSegments, isForward);
	}

	protected bool CloseEnough(float value1, float value2)
	{
		float num = Mathf.Abs(value1 - value2);
		if (num < GetMatchPositionDiff(identificationUsed, 60f))
		{
			return true;
		}
		return false;
	}

	protected bool CompareSwipeList(SwipeSegmentList swipeSegmentsIn, SegmentList sourceSegmentsIn, LineGesture.LineSwipeDirection restrictLineSwipeDirection, float positionDiff, float lengthDiffPercent, LineGesture.LineIdentification lineIdentification)
	{
		sourceSegments = sourceSegmentsIn;
		identificationUsed = lineIdentification;
		restrictLineSwipeDirectionUsed = restrictLineSwipeDirection;
		compressedSwipeSegments = null;
		matchedSwipeSegments = null;
		SetError(string.Empty, true);
		firstLastMatch = false;
		extraSegment = 0;
		matchPositionDiff = positionDiff;
		matchLengthDiffPercent = lengthDiffPercent;
		if (identificationUsed == LineGesture.LineIdentification.Sloppy)
		{
			matchPositionDiff *= 3f;
			matchLengthDiffPercent = matchPositionDiff * 2f;
		}
		if (swipeSegmentsIn == null || swipeSegmentsIn.Count == 0)
		{
			SetError("Swipe is empty", true);
			return false;
		}
		if (matchedSegments == null)
		{
			matchedSegments = new SwipeSegment[sourceSegments.Count];
		}
		if (closed)
		{
			return CompareClosed(swipeSegmentsIn, restrictLineSwipeDirection);
		}
		if (swipeSegmentsIn.Count != sourceSegments.Count && (!tryHarder() || swipeSegmentsIn.Count <= sourceSegments.Count))
		{
			SetError("Swipe count " + swipeSegmentsIn.Count + " does not match source " + sourceSegments.Count, true);
			return false;
		}
		bool flag = false;
		SwipeSegmentList[] array = null;
		if (CompareFoward(swipeSegmentsIn))
		{
			if (matchScore == 0)
			{
				return true;
			}
			array = compressedSwipeSegmentsList;
			flag = true;
		}
		int num = matchScore;
		if (CompareBackward(swipeSegmentsIn) && (!flag || matchScore < num))
		{
			return true;
		}
		if (flag)
		{
			isForwardUsed = true;
			compressedSwipeSegmentsList = array;
			return true;
		}
		return false;
	}

	private bool CompareFoward(SwipeSegmentList swipeSegments)
	{
		if ((restrictLineSwipeDirectionUsed == LineGesture.LineSwipeDirection.Forward || restrictLineSwipeDirectionUsed == LineGesture.LineSwipeDirection.Either || restrictLineSwipeDirectionUsed == LineGesture.LineSwipeDirection.Anywhere) && CompareDirectionFor(swipeSegments, 0, true))
		{
			return true;
		}
		return false;
	}

	private bool CompareBackward(SwipeSegmentList swipeSegments)
	{
		if (!biDirectional)
		{
			return false;
		}
		if ((restrictLineSwipeDirectionUsed == LineGesture.LineSwipeDirection.Backward || restrictLineSwipeDirectionUsed == LineGesture.LineSwipeDirection.Either || restrictLineSwipeDirectionUsed == LineGesture.LineSwipeDirection.Anywhere) && CompareDirectionFor(swipeSegments, swipeSegments.Count - 1, false))
		{
			return true;
		}
		return false;
	}

	private bool CompareClosed(SwipeSegmentList swipeSegmentsIn, LineGesture.LineSwipeDirection restrictLineSwipeDirection)
	{
		compressedSwipeSegments = null;
		if (swipeSegmentsIn.Count != sourceSegments.Count)
		{
			if ((!startAnywhere || swipeSegmentsIn.Count != sourceSegments.Count + 1) && !tryHarder())
			{
				return false;
			}
			if ((swipeSegmentsIn.Count == sourceSegments.Count + 1 || (tryHarder() && swipeSegmentsIn.Count > sourceSegments.Count + 1)) && swipeSegmentsIn[0].direction == swipeSegmentsIn[swipeSegmentsIn.Count - 1].direction)
			{
				firstLastMatch = true;
				extraSegment = 1;
			}
			else if (!tryHarder() && swipeSegmentsIn.Count < sourceSegments.Count)
			{
				return false;
			}
		}
		Vector2 vector = swipeSegmentsIn[0].startPosition - swipeSegmentsIn[swipeSegmentsIn.Count - 1].endPosition;
		if (Mathf.Abs(vector.x) > matchPositionDiff || Mathf.Abs(vector.y) > matchPositionDiff)
		{
			SetError("Close line end points to not match ", true);
			return false;
		}
		bool flag = false;
		int num = swipeSegmentsIn.Count;
		if (!startAnywhere || restrictLineSwipeDirection != LineGesture.LineSwipeDirection.Anywhere)
		{
			num = 1;
		}
		for (int i = 0; i < num; i++)
		{
			if (restrictLineSwipeDirection == LineGesture.LineSwipeDirection.Forward || restrictLineSwipeDirection == LineGesture.LineSwipeDirection.Either || restrictLineSwipeDirection == LineGesture.LineSwipeDirection.Anywhere)
			{
				flag = CompareDirectionFor(swipeSegmentsIn, i, true);
				if (flag)
				{
					break;
				}
			}
			if (biDirectional && (restrictLineSwipeDirection == LineGesture.LineSwipeDirection.Backward || restrictLineSwipeDirection == LineGesture.LineSwipeDirection.Either || restrictLineSwipeDirection == LineGesture.LineSwipeDirection.Anywhere))
			{
				flag = CompareDirectionFor(swipeSegmentsIn, i, false);
				if (flag)
				{
					break;
				}
			}
		}
		if (flag)
		{
			return true;
		}
		return false;
	}

	private bool CompareDirectionFor(SwipeSegmentList swipeSegmentsIn, int startIndex, bool isForward)
	{
		SwipeSegmentList swipeSegmentList = swipeSegmentsIn;
		int num = startIndex;
		int currentIndex = num;
		isForwardUsed = isForward;
		swipeSegmentList.isForwardUsed = isForwardUsed;
		matchScore = 0;
		for (int i = 0; i < sourceSegments.Count; i++)
		{
			Segment segment = sourceSegments[i];
			if (!segment.CompareDirection(identificationUsed, swipeSegmentList[num].direction, isForward) || (i == sourceSegments.Count - 1 && swipeSegmentList.Count > sourceSegments.Count + extraSegment))
			{
				if (swipeSegmentList.Count - extraSegment <= sourceSegments.Count)
				{
					return false;
				}
				if (!TryToCompessSegments(i - 1, swipeSegmentList, startIndex, currentIndex, isForward) && !TryToCompessSegments(i, swipeSegmentList, startIndex, num, isForward))
				{
					return false;
				}
				swipeSegmentList = compressedSwipeSegments;
				startIndex = compressedStartIndex;
				num = startIndex;
				currentIndex = num;
				matchScore = 0;
				i = -1;
			}
			else
			{
				if (!segment.CompareDirection(LineGesture.LineIdentification.Clean, swipeSegmentList[num].direction, isForward))
				{
					matchScore += 3;
				}
				currentIndex = num;
				num = incIndex(num, 1, isForward, swipeSegmentList.Count);
			}
		}
		if (swipeSegmentList.Count != sourceSegments.Count + extraSegment)
		{
			return false;
		}
		num = startIndex;
		for (int j = 0; j < sourceSegments.Count; j++)
		{
			matchedSegments[j] = swipeSegmentList[num];
			num = incIndex(num, 1, isForward, swipeSegmentList.Count);
		}
		if (CheckLengths(matchedSegments, swipeSegmentList, startIndex))
		{
			return true;
		}
		return false;
	}

	private bool TryToCompessSegments(int sourceSegmentsIndex, SwipeSegmentList swipeSegments, int startIndex, int currentIndex, bool isForward)
	{
		int extras = swipeSegments.Count - sourceSegments.Count - extraSegment;
		int num = LookForwardForCompress(sourceSegmentsIndex, swipeSegments, startIndex, currentIndex, isForward, extras);
		if (num < 2)
		{
			return false;
		}
		int index = incIndex(currentIndex, num - 1, isForward, swipeSegments.Count);
		Segment segment = sourceSegments[sourceSegmentsIndex];
		SwipeSegment swipeSegment = swipeSegments[currentIndex];
		SwipeSegment swipeSegment2 = swipeSegments[index];
		SwipeGesture.SwipeDirection swipeDirection = ((!isForward) ? FingerControl.GetSwipeDirection(swipeSegment.endPosition, swipeSegment2.startPosition) : FingerControl.GetSwipeDirection(swipeSegment.startPosition, swipeSegment2.endPosition));
		if (swipeDirection == SwipeGesture.SwipeDirection.None)
		{
			return false;
		}
		SwipeGesture.SwipeDirection direction = swipeDirection;
		if (!segment.CompareDirection(identificationUsed, swipeDirection, true))
		{
			return false;
		}
		direction = SwipeGesture.GetDirection(isForward, direction);
		int num2;
		int num3;
		int num4;
		int num5;
		if (isForward)
		{
			num2 = currentIndex + 1;
			num3 = currentIndex + num - 1;
			compressedStartIndex = startIndex;
			if (num3 > swipeSegments.Count - 1)
			{
				num4 = 0;
				num5 = num3 - swipeSegments.Count - 1;
				num3 = swipeSegments.Count - 1;
				compressedStartIndex = startIndex - num5;
			}
			else
			{
				num4 = num2;
				num5 = num3;
			}
		}
		else
		{
			num2 = currentIndex - num + 1;
			num3 = currentIndex - 1;
			if (startIndex < num3)
			{
				compressedStartIndex = startIndex;
			}
			else
			{
				compressedStartIndex = startIndex - (num3 - num2 + 1);
			}
			if (num2 < 0)
			{
				num4 = swipeSegments.Count + num2;
				num5 = swipeSegments.Count - 1;
				num2 = 0;
				compressedStartIndex = 0;
			}
			else
			{
				num4 = num2;
				num5 = num3;
			}
		}
		if (compressedStartIndex < 0)
		{
			compressedStartIndex = 0;
		}
		SwipeSegment prev = null;
		SwipeSegment swipeSegment3 = null;
		compressedSwipeSegments = new SwipeSegmentList();
		compressedSwipeSegments.isForwardUsed = isForward;
		for (int i = 0; i < swipeSegments.Count; i++)
		{
			if (i == currentIndex)
			{
				swipeSegment3 = new SwipeSegment(prev);
				swipeSegment3.Copy(swipeSegment);
				swipeSegment3.CombineToSingleAfter(isForward, direction, swipeSegment2);
				compressedSwipeSegments.Add(swipeSegment3);
				prev = swipeSegment3;
			}
			else if ((i < num2 || i > num3) && (i < num4 || i > num5))
			{
				swipeSegment3 = new SwipeSegment(prev);
				swipeSegment3.Copy(swipeSegments[i]);
				compressedSwipeSegments.Add(swipeSegment3);
				prev = swipeSegment3;
			}
		}
		if (compressedStartIndex >= compressedSwipeSegments.Count)
		{
			compressedStartIndex = compressedSwipeSegments.Count - 1;
		}
		return true;
	}

	private int LookForwardForCompress(int sourceSegmentsIndex, SwipeSegmentList swipeSegments, int startIndex, int currentIndex, bool isForward, int extras)
	{
		lookForwardDepth++;
		int num = 0;
		try
		{
			if (extras == 0 || sourceSegmentsIndex < 0)
			{
				return 0;
			}
			if (sourceSegmentsIndex < 0)
			{
				sourceSegmentsIndex = 0;
			}
			Segment segment = sourceSegments[sourceSegmentsIndex];
			SwipeGesture.SwipeDirection direction = segment.direction;
			SwipeGesture.SwipeDirection optionalDirection = segment.optionalDirection;
			SwipeGesture.SwipeDirection[] array = null;
			SwipeGesture.SwipeDirection[] array2 = null;
			Segment segment2 = null;
			int num2 = -1;
			if (sourceSegmentsIndex < sourceSegments.Count - 1)
			{
				num2 = sourceSegmentsIndex + 1;
			}
			else if (closed)
			{
				num2 = 0;
			}
			if (num2 >= 0)
			{
				segment2 = sourceSegments[num2];
			}
			SwipeSegment swipeSegment = swipeSegments[currentIndex];
			SwipeSegment swipeSegment2 = null;
			SwipeGesture.SwipeDirection swipeDirection = SwipeGesture.SwipeDirection.None;
			if (!segment.CompareDirection(identificationUsed, swipeSegment.direction, isForward))
			{
				return 0;
			}
			num++;
			bool flag = false;
			bool flag2 = false;
			for (int num3 = incIndex(currentIndex, 1, isForward, swipeSegments.Count); num < extras + 1 && num3 != startIndex; num3 = incIndex(num3, 1, isForward, swipeSegments.Count))
			{
				flag = true;
				swipeSegment2 = swipeSegments[num3];
				SwipeGesture.SwipeDirection direction2 = SwipeGesture.GetDirection(isForward, swipeSegment2.direction);
				if (num3 != currentIndex && segment2 != null && segment2.CompareDirection(identificationUsed, direction2, true))
				{
					int num4 = LookForwardForCompress(num2, swipeSegments, startIndex, num3, isForward, extras - num + 1);
					if (num4 > 0)
					{
						flag2 = true;
						break;
					}
					if (num4 >= 0)
					{
					}
				}
				if (!segment.CompareDirection(identificationUsed, direction2, true))
				{
					bool flag3 = false;
					if (identificationUsed == LineGesture.LineIdentification.Sloppy)
					{
						if (array == null)
						{
							array = FingerControl.GetFriendlyDirections(direction);
							if (direction != optionalDirection)
							{
								array2 = FingerControl.GetFriendlyDirections(optionalDirection);
							}
						}
						int num5 = 0;
						while (!flag3 && num5 < array.Length)
						{
							if (FingerControl.FriendlySwipeDirections(array[num5], direction2))
							{
								flag3 = true;
							}
							if (direction != optionalDirection && FingerControl.FriendlySwipeDirections(array2[num5], direction2))
							{
								flag3 = true;
							}
							num5++;
						}
					}
					if (!flag3)
					{
						if (swipeSegment2.distance > SegmentMaxDistanceIgnore)
						{
							SetError(string.Concat("Parsing source direction failure ", direction2, " does not match ", segment.DebugStringDirections(true)), true);
							break;
						}
						num++;
						flag = false;
						continue;
					}
				}
				swipeDirection = ((!isForward) ? FingerControl.GetSwipeDirection(swipeSegment.endPosition, swipeSegment2.startPosition) : FingerControl.GetSwipeDirection(swipeSegment.startPosition, swipeSegment2.endPosition));
				if (!segment.CompareDirection(identificationUsed, swipeDirection, true) && swipeSegment2.distance > SegmentMaxDistanceIgnore)
				{
					SetError(string.Concat("Parsing calulated direction failure ", swipeDirection, " does not match ", segment.DebugStringDirections(true)), true);
					num++;
					flag = false;
					break;
				}
				if (num == extras && segment2 != null && !segment2.CompareDirection(identificationUsed, swipeSegments[incIndex(num3, 1, isForward, swipeSegments.Count)].direction, isForward))
				{
					break;
				}
				num++;
				flag = false;
			}
			if (sourceSegmentsIndex == sourceSegments.Count - 1 && num != extras + 1)
			{
				SetError("Unable to match source segments on segment " + startIndex, false);
				return 0;
			}
			if (num == extras && segment2 != null && !segment2.CompareDirection(identificationUsed, swipeSegments[incIndex(currentIndex, num, isForward, swipeSegments.Count)].direction, isForward))
			{
				SetError("Unable to match source segments on segment " + startIndex + " ending with matching end", true);
				return 0;
			}
			if (num < extras && segment2 == null)
			{
				SetError("Unable to match remaining source segments on segments " + startIndex + " on ending segment", true);
				return 0;
			}
			if (flag && !flag2)
			{
				num *= -1;
			}
		}
		finally
		{
			lookForwardDepth--;
		}
		return num;
	}

	private int incIndex(int index, int numberOfTimes, bool isForward, int indexEnd)
	{
		int num = (isForward ? 1 : (-1));
		for (int i = 0; i < numberOfTimes; i++)
		{
			index += num;
			if (isForward)
			{
				if (index >= indexEnd)
				{
					index = 0;
				}
			}
			else if (index < 0)
			{
				index = indexEnd - 1;
			}
		}
		return index;
	}

	private void DebugSwipeListDirection(string title, SwipeSegmentList swipeSegments, bool isForward, int index1, string text1, int index2, string text2)
	{
		if (isForward)
		{
			for (int i = 0; i < swipeSegments.Count; i++)
			{
				Debug.Log(string.Concat(title, " -FORWARD- ", name, " segment ", i, "  ", swipeSegments[i].direction, " ", (index1 != i) ? string.Empty : text1, " ", (index2 != i) ? string.Empty : text2));
			}
			return;
		}
		for (int num = swipeSegments.Count - 1; num >= 0; num--)
		{
			Debug.Log(string.Concat(title, " -BACKWARD- ", name, " segment ", num, "  ", SwipeGesture.GetReverseDirection(swipeSegments[num].direction), " ", (index1 != num) ? string.Empty : text1, " ", (index2 != num) ? string.Empty : text2));
		}
	}

	private bool CheckLengths(SwipeSegment[] matchedSegments, SwipeSegmentList swipeSegments, int startIndex)
	{
		if (!doCompareLengths)
		{
			return true;
		}
		float[] array = new float[matchedSegments.Length];
		float num = float.MaxValue;
		int num2 = int.MaxValue;
		float num3 = float.MinValue;
		for (int i = 0; i < matchedSegments.Length; i++)
		{
			array[i] = getDistance(matchedSegments[i]);
			if (i != startIndex || !closed || !firstLastMatch || extraSegment <= 0)
			{
				if (array[i] < num)
				{
					num = array[i];
				}
				if (array[i] > num3)
				{
					num3 = array[i];
				}
			}
			if (sourceSegments[i].relativeSize > 0 && sourceSegments[i].relativeSize < num2)
			{
				num2 = sourceSegments[i].relativeSize;
			}
		}
		if (closed && extraSegment > 0)
		{
			array[startIndex] += getDistance(swipeSegments[swipeSegments.Count - 1]);
			if (array[startIndex] < num)
			{
				num = array[startIndex];
			}
		}
		if (num2 == int.MaxValue)
		{
			num2 = 1;
		}
		float num4 = overRideUnitSize;
		if (num4 <= 0f)
		{
			num4 = num / (float)num2;
		}
		for (int j = 0; j < matchedSegments.Length; j++)
		{
			if (sourceSegments[j].sizeSpec == Segment.SizeSpec.Ratio)
			{
				if (sourceSegments[j].relativeSize > 0)
				{
					float num5 = num4 * (float)sourceSegments[j].relativeSize;
					float f = Mathf.Abs(array[j] - num5);
					float num6 = num4 * sourceSegments[j].getMatchLengthDiffPercent(identificationUsed, matchLengthDiffPercent);
					if (Mathf.Abs(f) > num6)
					{
						SetError("Segment length ratio " + sourceSegments[j].relativeSize + " check on " + j, true);
						return false;
					}
				}
			}
			else if (sourceSegments[j].sizeSpec == Segment.SizeSpec.Bigger)
			{
				float num5 = num4 * (float)sourceSegments[j].relativeSize;
				if (array[j] < num5)
				{
					SetError("Bigger segmentthan " + sourceSegments[j].relativeSize + " failed check on " + j, true);
					return false;
				}
			}
			else if (sourceSegments[j].sizeSpec == Segment.SizeSpec.Smaller)
			{
				float num5 = num4 * (float)sourceSegments[j].relativeSize;
				if (array[j] <= num5)
				{
					SetError("Smaller segment than " + sourceSegments[j].relativeSize + " failed check on " + j, true);
					return false;
				}
			}
			else if (sourceSegments[j].sizeSpec == Segment.SizeSpec.Smallest)
			{
				if (array[j] > num)
				{
					SetError("Smallest segment check failed  failed on " + j, true);
					return false;
				}
			}
			else if (sourceSegments[j].sizeSpec == Segment.SizeSpec.Biggest)
			{
				if (array[j] > num3)
				{
					SetError("Biggest segment check failed  failed on " + j, true);
					return false;
				}
			}
			else if (sourceSegments[j].sizeSpec != Segment.SizeSpec.Ignore)
			{
			}
		}
		return true;
	}

	protected float getDistance(SwipeSegment swipeSegment)
	{
		return (swipeSegment.startPosition - swipeSegment.endPosition).magnitude;
	}

	private bool tryHarder()
	{
		if (identificationUsed == LineGesture.LineIdentification.Precise)
		{
			return false;
		}
		return true;
	}
}
