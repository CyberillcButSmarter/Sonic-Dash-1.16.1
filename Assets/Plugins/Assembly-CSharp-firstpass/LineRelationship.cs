public class LineRelationship
{
	public enum LinePosition
	{
		Percent = 0,
		Start = 1,
		End = 2,
		Upper = 3,
		Lower = 4,
		Left = 5,
		Right = 6,
		BetweenTopBottom = 7,
		BetweenLeftRight = 8
	}

	public LineSwipe targetLine;

	public int targetSegmentNum;

	public LinePosition targetPosition;

	public float targetPercentPosition;

	public LineSwipe relativeLine;

	public int relativeSegmentNum;

	public LinePosition relativePosition;

	public float relativePercentPosition;

	public LineRelationship(LineSwipe targetLineIn, int targetSegmentNumIn, LinePosition targetPositionIn, LineSwipe relativeLineIn, int relativeSegmentNumIn, LinePosition relativePositionIn)
	{
		targetLine = targetLineIn;
		targetSegmentNum = targetSegmentNumIn;
		targetPosition = targetPositionIn;
		relativeLine = relativeLineIn;
		relativeSegmentNum = relativeSegmentNumIn;
		relativePosition = relativePositionIn;
	}

	public LineRelationship(LineSwipe targetLineIn, int targetSegmentNumIn, float targetPercentPositionIn, LineSwipe relativeLineIn, int relativeSegmentNumIn, LinePosition relativePositionIn)
	{
		targetLine = targetLineIn;
		targetSegmentNum = targetSegmentNumIn;
		targetPercentPosition = targetPercentPositionIn;
		relativeLine = relativeLineIn;
		relativeSegmentNum = relativeSegmentNumIn;
		relativePosition = relativePositionIn;
	}

	public LineRelationship(LineSwipe targetLineIn, int targetSegmentNumIn, float targetPercentPositionIn, LineSwipe relativeLineIn, int relativeSegmentNumIn, float relativePercentPositionIn)
	{
		targetLine = targetLineIn;
		targetSegmentNum = targetSegmentNumIn;
		targetPercentPosition = targetPercentPositionIn;
		relativeLine = relativeLineIn;
		relativeSegmentNum = relativeSegmentNumIn;
		relativePercentPosition = relativePercentPositionIn;
	}

	public LineRelationship(LineSwipe targetLineIn, int targetSegmentNumIn, LinePosition targetPositionIn, LineSwipe relativeLineIn, int relativeSegmentNumIn, float relativePercentPositionIn)
	{
		targetLine = targetLineIn;
		targetSegmentNum = targetSegmentNumIn;
		targetPosition = targetPositionIn;
		relativeLine = relativeLineIn;
		relativeSegmentNum = relativeSegmentNumIn;
		relativePercentPosition = relativePercentPositionIn;
	}
}
