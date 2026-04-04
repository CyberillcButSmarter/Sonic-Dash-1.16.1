using System.Collections.Generic;

public class MultipleLineSwipe : LineSwipeBase
{
	private List<LineSwipeBase> lineSwipes = new List<LineSwipeBase>();

	private int maxCount;

	private bool isCompound;

	public int swipeUsed = -1;

	public MultipleLineSwipe(string nameIn)
		: base(nameIn)
	{
	}

	public override LineSwipeBase GetUsedLineSwipe()
	{
		if (swipeUsed < 0)
		{
			return null;
		}
		return lineSwipes[swipeUsed];
	}

	public override int GetMaxSegment()
	{
		return -1;
	}

	public override bool IsCompound()
	{
		return isCompound;
	}

	public override int Count()
	{
		return maxCount;
	}

	public void AddLineSwipe(LineSwipeBase lineSwipe)
	{
		lineSwipes.Add(lineSwipe);
		if (lineSwipe.Count() > maxCount)
		{
			maxCount = lineSwipe.Count();
		}
		if (lineSwipe.IsCompound())
		{
			isCompound = true;
		}
	}

	public LineSwipeBase GetLineSwipe(int index)
	{
		if (index < 0 || index >= lineSwipes.Count)
		{
			return null;
		}
		return lineSwipes[index];
	}

	public override bool Compare(List<SwipeSegmentList> swipeList, LineGesture.LineSwipeDirection restrictLineSwipeDirection, float positionDiff, float lengthDiffPercent, LineGesture.LineIdentification lineIdentification)
	{
		swipeUsed = -1;
		for (int i = 0; i < lineSwipes.Count; i++)
		{
			if (lineSwipes[i].Compare(swipeList, restrictLineSwipeDirection, positionDiff, lengthDiffPercent, lineIdentification))
			{
				swipeUsed = i;
				identificationUsed = lineIdentification;
				restrictLineSwipeDirectionUsed = restrictLineSwipeDirection;
				return true;
			}
		}
		return false;
	}
}
