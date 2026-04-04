using UnityEngine;

public struct LightweightTransform
{
	private Vector3 m_location;

	private Quaternion m_orientation;

	public Vector3 Location
	{
		get
		{
			return m_location;
		}
		set
		{
			m_location = value;
		}
	}

	public Quaternion Orientation
	{
		get
		{
			return m_orientation;
		}
		set
		{
			m_orientation = value;
		}
	}

	public Vector3 Forwards
	{
		get
		{
			return Orientation * Vector3.forward;
		}
	}

	public Vector3 Right
	{
		get
		{
			return Orientation * Vector3.right;
		}
	}

	public Vector3 Up
	{
		get
		{
			return Orientation * Vector3.up;
		}
	}

	public LightweightTransform(Transform fromTransform)
	{
		m_location = fromTransform.position;
		m_orientation = fromTransform.rotation;
	}

	public LightweightTransform(Vector3 pos, Quaternion rot)
	{
		m_location = pos;
		m_orientation = rot;
	}

	public static LightweightTransform Lerp(LightweightTransform from, LightweightTransform to, float factor)
	{
		return new LightweightTransform(Vector3.Lerp(from.Location, to.Location, factor), Quaternion.Lerp(from.Orientation, to.Orientation, factor));
	}

	public void ApplyTo(Transform t)
	{
		t.position = Location;
		t.rotation = Orientation;
	}
}
