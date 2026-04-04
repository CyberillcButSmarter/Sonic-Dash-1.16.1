using UnityEngine;

public class UserSegment : Segment
{
	public Vector3 startPoint;

	public Vector3 endPoint;

	public UserSegment(SwipeGesture.SwipeDirection directionIn, Vector3 startP, Vector3 endP)
	{
		Debug.Log(string.Concat("UserSegment:UserSegment ", directionIn, " ", startP, " to ", endP));
		direction = directionIn;
		startPoint = startP;
		endPoint = endP;
	}

	public void UpdatePoint(Vector3 pos, bool isStart)
	{
		if (isStart)
		{
			startPoint = pos;
		}
		else
		{
			endPoint = pos;
		}
	}
}
