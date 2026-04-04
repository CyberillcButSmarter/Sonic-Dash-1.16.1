public class Segment
{
	public enum SizeSpec
	{
		Ratio = 0,
		Bigger = 1,
		Biggest = 2,
		Smaller = 3,
		Smallest = 4,
		Ignore = 5
	}

	public SwipeGesture.SwipeDirection direction;

	public SwipeGesture.SwipeDirection optionalDirection;

	public int relativeSize;

	public SizeSpec sizeSpec;

	private float _matchLengthDiffPercent;

	public SwipeGesture.SwipeDirection directionReverse;

	public SwipeGesture.SwipeDirection optionalDirectionReverse;

	public float matchLengthDiffPercent
	{
		set
		{
			_matchLengthDiffPercent = value;
		}
	}

	public Segment()
	{
	}

	public Segment(SwipeGesture.SwipeDirection directionIn, int relativeSizeIn, SizeSpec sizeSpecIn)
	{
		relativeSize = relativeSizeIn;
		relativeSize = relativeSizeIn;
		direction = directionIn;
		sizeSpec = sizeSpecIn;
		optionalDirection = directionIn;
		directionReverse = SwipeGesture.GetReverseDirection(direction);
		optionalDirectionReverse = directionReverse;
	}

	public Segment(SwipeGesture.SwipeDirection directionIn, int relativeSizeIn, SwipeGesture.SwipeDirection optionalDrectionIn, SizeSpec sizeSpecIn)
	{
		direction = directionIn;
		optionalDirection = optionalDrectionIn;
		sizeSpec = sizeSpecIn;
		relativeSize = relativeSizeIn;
		directionReverse = SwipeGesture.GetReverseDirection(direction);
		optionalDirectionReverse = SwipeGesture.GetReverseDirection(optionalDirection);
	}

	public float getMatchLengthDiffPercent(LineGesture.LineIdentification identification, float defaultValue)
	{
		if (_matchLengthDiffPercent <= 0f)
		{
			_matchLengthDiffPercent = defaultValue;
		}
		if (identification == LineGesture.LineIdentification.Sloppy)
		{
			return _matchLengthDiffPercent * LineSwipeBase.matchLengthDiffPercentSloppyMultiplier;
		}
		return _matchLengthDiffPercent;
	}

	public string DebugStringDirections(bool isForward)
	{
		if (direction == optionalDirection)
		{
			if (isForward)
			{
				return direction.ToString();
			}
			return directionReverse.ToString();
		}
		if (isForward)
		{
			return direction.ToString() + " or " + optionalDirection;
		}
		return directionReverse.ToString() + " or " + optionalDirectionReverse;
	}

	public bool CompareDirection(LineGesture.LineIdentification identification, SwipeGesture.SwipeDirection swipeDirection, bool isForward)
	{
		if (isForward)
		{
			return CompareDirectionForward(identification, swipeDirection);
		}
		return CompareDirectionReverse(identification, swipeDirection);
	}

	public bool CompareDirectionForward(LineGesture.LineIdentification identification, SwipeGesture.SwipeDirection swipeDirection)
	{
		if (swipeDirection == direction || swipeDirection == optionalDirection)
		{
			return true;
		}
		if (identification == LineGesture.LineIdentification.Sloppy && (FingerControl.FriendlySwipeDirections(direction, swipeDirection) || FingerControl.FriendlySwipeDirections(optionalDirection, swipeDirection)))
		{
			return true;
		}
		return false;
	}

	public bool CompareDirectionReverse(LineGesture.LineIdentification identification, SwipeGesture.SwipeDirection swipeDirection)
	{
		if (swipeDirection == directionReverse || swipeDirection == optionalDirectionReverse)
		{
			return true;
		}
		if (identification == LineGesture.LineIdentification.Sloppy && (FingerControl.FriendlySwipeDirections(directionReverse, swipeDirection) || FingerControl.FriendlySwipeDirections(optionalDirectionReverse, swipeDirection)))
		{
			return true;
		}
		return false;
	}
}
