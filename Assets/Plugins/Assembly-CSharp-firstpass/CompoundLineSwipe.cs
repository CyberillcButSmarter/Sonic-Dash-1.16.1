using System.Collections.Generic;
using UnityEngine;

public class CompoundLineSwipe : LineSwipeBase
{
	public List<LineSwipe> lineSwipes = new List<LineSwipe>();

	public List<LineRelationship> lineRelationships = new List<LineRelationship>();

	private int maxSegmentCount;

	private float positionDiff;

	public CompoundLineSwipe(string nameIn)
		: base(nameIn)
	{
	}

	public void AddLine(LineSwipe lineSwipe)
	{
		lineSwipes.Add(lineSwipe);
		int num = lineSwipe.Count();
		if (num > maxSegmentCount)
		{
			maxSegmentCount = num;
		}
	}

	public void AddRelationship(LineRelationship lineRelationship)
	{
		lineRelationships.Add(lineRelationship);
	}

	public override int GetMaxSegment()
	{
		return maxSegmentCount;
	}

	public override int Count()
	{
		return lineSwipes.Count;
	}

	public LineSwipe GetLine(int index)
	{
		if (index < 0 || index >= lineSwipes.Count)
		{
			return null;
		}
		return lineSwipes[index];
	}

	public override bool Compare(List<SwipeSegmentList> swipeList, LineGesture.LineSwipeDirection restrictLineSwipeDirection, float positionDiff, float lengthDiffPercent, LineGesture.LineIdentification lineIdentification)
	{
		if (lineSwipes.Count != swipeList.Count)
		{
			return false;
		}
		compressedSwipeSegments = null;
		compressedSwipeSegmentsList = null;
		if (!CompareLocal(swipeList, restrictLineSwipeDirection, positionDiff, lengthDiffPercent, false, lineIdentification))
		{
			compressedSwipeSegments = null;
			compressedSwipeSegmentsList = null;
			return false;
		}
		return true;
	}

	protected bool CompareLocal(List<SwipeSegmentList> swipeList, LineGesture.LineSwipeDirection restrictLineSwipeDirection, float positionDiffIn, float lengthDiffPercent, bool compareLocal, LineGesture.LineIdentification lineIdentification)
	{
		identificationUsed = lineIdentification;
		restrictLineSwipeDirectionUsed = restrictLineSwipeDirection;
		positionDiff = positionDiffIn;
		int[] array = new int[swipeList.Count];
		for (int i = 0; i < swipeList.Count; i++)
		{
			array[i] = -1;
		}
		for (int i = 0; i < swipeList.Count; i++)
		{
			SwipeSegmentList swipeSegments = swipeList[i];
			bool flag = false;
			int j;
			for (j = 0; j < lineSwipes.Count; j++)
			{
				if (array[j] > -1)
				{
					continue;
				}
				lineSwipes[j].doCompareLengths = compareLocal;
				flag = lineSwipes[j].Compare(swipeSegments, restrictLineSwipeDirection, positionDiff, lengthDiffPercent, lineIdentification);
				if (flag)
				{
					if (lineSwipes[j].compressedSwipeSegments != null)
					{
						lineSwipes[j].matchedSwipeSegments = lineSwipes[j].compressedSwipeSegments;
					}
					else
					{
						lineSwipes[j].matchedSwipeSegments = swipeSegments;
					}
					break;
				}
			}
			if (!flag)
			{
				SetError("Swipe " + j + " was not found", false);
				return false;
			}
			array[j] = i;
			if (lineSwipes[j].compressedSwipeSegments != null)
			{
				if (compressedSwipeSegmentsList == null)
				{
					compressedSwipeSegmentsList = new SwipeSegmentList[lineSwipes.Count];
				}
				compressedSwipeSegmentsList[i] = lineSwipes[j].compressedSwipeSegments;
			}
		}
		List<SwipeSegmentList> list = new List<SwipeSegmentList>();
		for (int i = 0; i < lineSwipes.Count; i++)
		{
			int index = array[i];
			if (compressedSwipeSegmentsList != null && compressedSwipeSegmentsList[i] != null)
			{
				list.Add(compressedSwipeSegmentsList[i]);
				continue;
			}
			if (compressedSwipeSegmentsList != null)
			{
			}
			list.Add(swipeList[index]);
		}
		if (!EvaluateRelationships(list))
		{
			return false;
		}
		return true;
	}

	private bool EvaluateRelationships(List<SwipeSegmentList> swipeList)
	{
		for (int i = 0; i < lineRelationships.Count; i++)
		{
			if (!EvaluateRelationship(lineRelationships[i], swipeList))
			{
				return false;
			}
		}
		return true;
	}

	private bool EvaluateRelationship(LineRelationship lineRelationship, List<SwipeSegmentList> swipeList)
	{
		SwipeSegmentList swipeSegmentList = lineRelationship.targetLine.matchedSwipeSegments;
		SwipeSegmentList swipeSegmentList2 = lineRelationship.relativeLine.matchedSwipeSegments;
		if (swipeSegmentList == null || swipeSegmentList2 == null)
		{
			return false;
		}
		for (int i = 0; i < swipeList.Count; i++)
		{
			swipeSegmentList = swipeList[i];
			int num = FindIsForward(lineRelationship.targetLine, swipeList[i]);
			if (num == -1)
			{
				continue;
			}
			bool flag = num == 1;
			if (!DirectionsMatch(identificationUsed, lineRelationship.targetLine.segments, swipeSegmentList, flag))
			{
				continue;
			}
			for (int j = 0; j < swipeList.Count; j++)
			{
				if (i == j)
				{
					continue;
				}
				swipeSegmentList2 = swipeList[j];
				num = FindIsForward(lineRelationship.relativeLine, swipeList[j]);
				if (num != -1)
				{
					bool flag2 = num == 1;
					if (DirectionsMatch(identificationUsed, lineRelationship.relativeLine.segments, swipeList[j], flag2) && TestRelationShipPoints(lineRelationship, swipeSegmentList, flag, swipeSegmentList2, flag2))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	private bool TestRelationShipPoints(LineRelationship lineRelationship, SwipeSegmentList targetSegments, bool targetIsForward, SwipeSegmentList relativeSegments, bool relativeIsForward)
	{
		Vector2 vector = FindRelationShipPoint(targetSegments, targetIsForward, lineRelationship.targetLine, lineRelationship.targetSegmentNum, lineRelationship.targetPosition, lineRelationship.targetPercentPosition);
		Vector2 vector2 = FindRelationShipPoint(relativeSegments, relativeIsForward, lineRelationship.relativeLine, lineRelationship.relativeSegmentNum, lineRelationship.relativePosition, lineRelationship.relativePercentPosition);
		if (vector == Vector2.zero || vector2 == Vector2.zero)
		{
			SetError("Line relationship failed on error - an end point is zero", false);
			return false;
		}
		if (lineRelationship.targetPosition == LineRelationship.LinePosition.BetweenTopBottom)
		{
			if (vector2.y >= vector.x && vector2.y <= vector.y)
			{
				return true;
			}
			SetError(string.Concat("Line is not between top and bottom  with ", lineRelationship.relativePosition, " using range ", vector, " with ", vector2), true);
			return false;
		}
		if (lineRelationship.targetPosition == LineRelationship.LinePosition.BetweenLeftRight)
		{
			if (vector2.x >= vector.x && vector2.x <= vector.y)
			{
				return true;
			}
			SetError(string.Concat("Line is not between left and righ with ", lineRelationship.relativePosition, " using range ", vector, " with ", vector2), true);
			return false;
		}
		Vector2 vector3 = vector - vector2;
		float num = GetMatchPositionDiff(identificationUsed, positionDiff);
		if (Mathf.Abs(vector3.x) > num || Mathf.Abs(vector3.y) > num)
		{
			SetError(string.Concat("Line relationship points are too far apart ", lineRelationship.targetPosition, " with ", lineRelationship.relativePosition, " using ", num, " with ", vector3), true);
			return false;
		}
		if (!VerifyRepationShip(lineRelationship.targetPosition, vector, vector2))
		{
			SetError(string.Concat("Target line relationship ", lineRelationship.targetPosition, " not close enough"), true);
			return false;
		}
		if (!VerifyRepationShip(lineRelationship.relativePosition, vector2, vector))
		{
			SetError(string.Concat("Relative line relationship ", lineRelationship.relativePosition, "  not close enough"), true);
			return false;
		}
		return true;
	}

	private bool VerifyRepationShip(LineRelationship.LinePosition position, Vector2 positionPoint, Vector2 otherPoint)
	{
		switch (position)
		{
		case LineRelationship.LinePosition.Left:
			if (positionPoint.x < otherPoint.x)
			{
				return CloseEnough(positionPoint.x, otherPoint.x);
			}
			break;
		case LineRelationship.LinePosition.Right:
			if (positionPoint.x > otherPoint.x)
			{
				return CloseEnough(positionPoint.x, otherPoint.x);
			}
			break;
		case LineRelationship.LinePosition.Upper:
			if (positionPoint.y < otherPoint.y)
			{
				return CloseEnough(positionPoint.y, otherPoint.y);
			}
			break;
		case LineRelationship.LinePosition.Lower:
			if (positionPoint.y > otherPoint.y)
			{
				return CloseEnough(positionPoint.y, otherPoint.y);
			}
			break;
		}
		return true;
	}

	private Vector2 FindRelationShipPoint(SwipeSegmentList swipeSegments, bool isForward, LineSwipe lineSwipe, int segmentNum, LineRelationship.LinePosition position, float percentPosition)
	{
		if (swipeSegments.Count != lineSwipe.Count() || segmentNum >= swipeSegments.Count)
		{
			SetError("Line segment counts do not match, " + swipeSegments.Count + " needing " + lineSwipe.Count(), false);
			return Vector2.zero;
		}
		SwipeSegment swipeSegment = ((swipeSegments.Count == 1) ? swipeSegments[0] : ((!isForward) ? swipeSegments[swipeSegments.Count - segmentNum - 1] : swipeSegments[segmentNum]));
		Vector2 vector;
		Vector2 vector2;
		if (isForward)
		{
			vector = swipeSegment.startPosition;
			vector2 = swipeSegment.endPosition;
		}
		else
		{
			vector = swipeSegment.endPosition;
			vector2 = swipeSegment.startPosition;
		}
		switch (position)
		{
		case LineRelationship.LinePosition.Start:
			return vector;
		case LineRelationship.LinePosition.End:
			return vector2;
		case LineRelationship.LinePosition.Left:
			if (vector.x > vector2.x)
			{
				return vector2;
			}
			return vector;
		case LineRelationship.LinePosition.Right:
			if (vector.x > vector2.x)
			{
				return vector;
			}
			return vector2;
		case LineRelationship.LinePosition.Upper:
			if (vector.y > vector2.y)
			{
				return vector;
			}
			return vector2;
		case LineRelationship.LinePosition.Lower:
			if (vector.y > vector2.y)
			{
				return vector2;
			}
			return vector;
		case LineRelationship.LinePosition.Percent:
		{
			Vector2 vector3 = vector2 - vector;
			vector3 *= percentPosition;
			return vector + vector3;
		}
		case LineRelationship.LinePosition.BetweenTopBottom:
			if (vector.y > vector2.y)
			{
				return new Vector2(vector2.y, vector.y);
			}
			return new Vector2(vector.y, vector2.y);
		case LineRelationship.LinePosition.BetweenLeftRight:
			if (vector.x > vector2.x)
			{
				return new Vector2(vector2.x, vector.x);
			}
			return new Vector2(vector.x, vector2.x);
		default:
			SetError("Unable to find relationship point, " + position, false);
			return Vector2.zero;
		}
	}

	private int FindIsForward(LineSwipe lineSwipe, SwipeSegmentList segmentList)
	{
		if (restrictLineSwipeDirectionUsed == LineGesture.LineSwipeDirection.Backward)
		{
			if (!segmentList.isForwardUsed)
			{
				return 0;
			}
			return -1;
		}
		if (restrictLineSwipeDirectionUsed == LineGesture.LineSwipeDirection.Forward)
		{
			if (segmentList.isForwardUsed)
			{
				return 1;
			}
			return -1;
		}
		return segmentList.isForwardUsed ? 1 : 0;
	}

	public override bool IsCompound()
	{
		return true;
	}
}
