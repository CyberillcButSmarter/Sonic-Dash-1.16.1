using UnityEngine;

public class SwipeSegment
{
	public SwipeGesture.SwipeDirection direction = SwipeGesture.SwipeDirection.None;

	public float distance;

	public float velocity;

	public float startTime;

	public float endTime;

	public Finger finger;

	public SwipeSegment previous;

	public SwipeSegment next;

	public Vector2 startPosition;

	public Vector2 endPosition;

	public bool initalized;

	public SwipeSegment(SwipeSegment prev)
	{
		startTime = Time.time;
		finger = null;
		previous = prev;
		if (prev != null)
		{
			prev.next = this;
		}
		direction = SwipeGesture.SwipeDirection.None;
	}

	public void Copy(SwipeSegment copy)
	{
		direction = copy.direction;
		distance = copy.distance;
		velocity = copy.velocity;
		startTime = copy.startTime;
		endTime = copy.endTime;
		finger = copy.finger;
		startPosition = copy.startPosition;
		endPosition = copy.endPosition;
		initalized = true;
	}

	public void AddFinger(Finger fingerIn)
	{
		finger = fingerIn;
	}

	public void Initalize(Finger fingerIn, Vector2 startPos, SwipeGesture.SwipeDirection directionIn)
	{
		finger = fingerIn;
		startPosition = startPos;
		direction = directionIn;
		initalized = true;
	}

	public void Set(SwipeGesture.SwipeDirection directionIn, float distanceIn, float velocityIn)
	{
		endTime = Time.time;
		direction = directionIn;
		distance = distanceIn;
		velocity = velocityIn;
	}

	public void Merge(SwipeSegment nextSegment)
	{
		endPosition = nextSegment.endPosition;
		next = nextSegment.next;
		distance += nextSegment.distance;
	}

	public void CombineToSingleAfter(bool isForward, SwipeGesture.SwipeDirection directionIn, SwipeSegment endSegment)
	{
		direction = directionIn;
		previous = null;
		next = null;
		if (isForward)
		{
			endTime = endSegment.endTime;
			endPosition = endSegment.endPosition;
		}
		else
		{
			startPosition = endSegment.startPosition;
			startTime = endSegment.startTime;
		}
		initalized = true;
		distance = ((Vector3)(endPosition - startPosition)).magnitude;
		velocity = distance / (endTime - startTime);
	}
}
