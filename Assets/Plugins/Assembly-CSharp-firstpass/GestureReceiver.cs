using UnityEngine;

public class GestureReceiver : MonoBehaviour, GestureListener
{
	public static GestureEvent lastEvent;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnApplicationQuit()
	{
	}

	public void OnGestureEvent(string eventJSON)
	{
		lastEvent = GestureEvent.GetGestureEventFromJSON(eventJSON);
	}

	public static GestureEvent GetLastEvent()
	{
		return lastEvent;
	}

	public static void ClearLastEvent()
	{
		lastEvent = null;
	}
}
