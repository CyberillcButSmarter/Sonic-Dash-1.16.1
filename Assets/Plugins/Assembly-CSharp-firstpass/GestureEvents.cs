using UnityEngine;

public class GestureEvents : MonoBehaviour
{
	private void Start()
	{
		LineGesture componentInChildren = base.gameObject.GetComponentInChildren<LineGesture>();
		componentInChildren.AddLineFactoryType(LineFactory.LineType.M, false);
	}

	private void GestureStartDrag(DragGesture gesture)
	{
		Debug.Log("GestureStartDrag  drag started");
	}

	private void GestureMoveDrag(DragGesture gesture)
	{
		Debug.Log("GestureMoveDrag  drag moving " + gesture.dragPosition);
	}

	private void GestureEndDrag(DragGesture gesture)
	{
		Debug.Log("GestureEndDrag  drag ended");
	}

	private void GestureLineSwipe(LineGesture gesture)
	{
		Debug.Log("LineGesture  happened " + gesture.swipedLineType);
	}

	private void GestureLineSwipeFailure(LineGesture gesture)
	{
		Debug.Log("LineGesture  finshed on failure " + gesture.errorString);
	}

	private void GestureLongPress(LongPressGesture gesture)
	{
		Debug.Log("LongPressGesture  happened " + gesture.longPressTime);
	}

	private void GestureStartPinch(PinchGesture gesture)
	{
		Debug.Log("PinchGesture  pinch started");
	}

	private void GestureMovePinch(PinchGesture gesture)
	{
		Debug.Log(string.Concat("PinchGesture  pinching ", gesture.pinchAction, " for ", gesture.pinchDirection));
	}

	private void GestureEndPinch(PinchGesture gesture)
	{
		Debug.Log("PinchGesture  pinch ended");
	}

	private void GestureStartRotate(RotateGesture gesture)
	{
		Debug.Log("GestureStartRotate  rotate started");
	}

	private void GestureMoveRotate(RotateGesture gesture)
	{
		Debug.Log(string.Concat("GestureStartRotate  rotating on axis ", gesture.rotateAxis, " by ", gesture.rotationAngleDelta));
	}

	private void GestureEndRotate(RotateGesture gesture)
	{
		Debug.Log("GestureStartRotate  rotate ended");
	}

	private void GestureSlice(SliceGesture gesture)
	{
		Debug.Log("GestureSlice  slice happened " + gesture.sliceDirection);
	}

	private void GestureSwipe(SwipeGesture gesture)
	{
		Debug.Log(string.Concat("SwipeGesture  swipe happened ", gesture.swipeDirection, " went from ", gesture.startPosition, " to ", gesture.endPosition));
	}

	private void GestureSwipeMove(SwipeGesture gesture)
	{
		Debug.Log(string.Concat("SwipeGesture  move happened ", gesture.swipePosition, " with direction ", gesture.swipeDirection));
	}

	private void GestureSwipeEnd(SwipeGesture gesture)
	{
		Debug.Log("SwipeGesture  end swipe happened " + gesture.endPosition);
	}

	private void GestureTap(TapGesture gesture)
	{
		Debug.Log("TapGesture  tap happened " + gesture.taps);
	}

	private void GestureStartTouch(TouchGesture gesture)
	{
		Debug.Log("GestureStartTouch  start touch " + gesture.finger.startPosition);
	}

	private void GestureMoveTouch(TouchGesture gesture)
	{
		Debug.Log("GestureStartTouch  touch moving " + gesture.finger.position);
	}

	private void GestureEndTouch(TouchGesture gesture)
	{
		Debug.Log("GestureEndTouch  end touch " + gesture.finger.endPosition);
	}
}
