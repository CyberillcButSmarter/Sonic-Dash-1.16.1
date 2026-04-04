using System.Collections.Generic;

public class LineSwipe : LineSwipeBase
{
	public SegmentList segments = new SegmentList();

	public LineSwipe(string nameIn)
		: base(nameIn)
	{
	}

	public override int Count()
	{
		return segments.Count;
	}

	public override int GetMaxSegment()
	{
		return Count();
	}

	public void AddSegment(Segment segment)
	{
		segments.Add(segment);
	}

	public Segment GetSegment(int index)
	{
		if (index < 0 || index >= segments.Count)
		{
			return null;
		}
		return segments[index];
	}

	public override bool Compare(List<SwipeSegmentList> swipeList, LineGesture.LineSwipeDirection restrictLineSwipeDirection, float positionDiff, float lengthDiffPercent, LineGesture.LineIdentification lineIdentification)
	{
		if (swipeList == null || swipeList.Count != 1)
		{
			return false;
		}
		SwipeSegmentList swipeSegments = swipeList[0];
		return Compare(swipeSegments, restrictLineSwipeDirection, positionDiff, lengthDiffPercent, lineIdentification);
	}

	public override bool Compare(SwipeSegmentList swipeSegments, LineGesture.LineSwipeDirection restrictLineSwipeDirection, float positionDiff, float lengthDiffPercent, LineGesture.LineIdentification lineIdentification)
	{
		compressedSwipeSegments = null;
		compressedSwipeSegmentsList = null;
		bool flag = CompareSwipeList(swipeSegments, segments, restrictLineSwipeDirection, positionDiff, lengthDiffPercent, lineIdentification);
		if (!flag)
		{
			compressedSwipeSegments = null;
			compressedSwipeSegmentsList = null;
		}
		return flag;
	}
}
