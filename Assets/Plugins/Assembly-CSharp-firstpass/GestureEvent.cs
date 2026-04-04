using System;
using System.Collections;
using AmazonCommon;
using UnityEngine;

public class GestureEvent
{
	public enum GestureId
	{
		TILT = 0,
		PEEK = 1
	}

	public enum Direction
	{
		NONE = 0,
		LEFT = 1,
		RIGHT = 2,
		BACK = 4,
		FORWARD = 8
	}

	public enum Action
	{
		DEFAULT = 0,
		ON = 1,
		OFF = 2
	}

	public GestureId gestureId;

	public Direction direction;

	public Action action;

	public ScreenOrientation orientation;

	public long timestamp;

	private GestureEvent(GestureId id, Direction direction, Action action, ScreenOrientation orientation, long timestamp)
	{
		gestureId = id;
		this.direction = direction;
		this.action = action;
		this.orientation = orientation;
		this.timestamp = timestamp;
	}

	public static GestureEvent GetGestureEventFromJSON(string eventJSON)
	{
		Hashtable hashtable = eventJSON.hashtableFromJson();
		if (hashtable == null)
		{
			return null;
		}
		return new GestureEvent((GestureId)Convert.ToInt32(hashtable["gestureId"]), (Direction)Convert.ToInt32(hashtable["direction"]), (Action)Convert.ToInt32(hashtable["action"]), (ScreenOrientation)Convert.ToInt32(hashtable["orientation"]), Convert.ToInt64(hashtable["timestamp_nsecs"]));
	}
}
