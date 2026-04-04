using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Grid")]
public class UIGrid : UIWidgetContainer
{
	public enum Arrangement
	{
		Horizontal = 0,
		Vertical = 1
	}

	public enum Sorting
	{
		None = 0,
		Alphabetic = 1,
		Horizontal = 2,
		Vertical = 3,
		Custom = 4
	}

	public delegate void OnReposition();

	public Arrangement arrangement;

	public Sorting sorting;

	public UIWidget.Pivot pivot;

	public int maxPerLine;

	public float cellWidth = 200f;

	public float cellHeight = 200f;

	public bool animateSmoothly;

	public bool hideInactive = true;

	public bool keepWithinPanel;

	public OnReposition onReposition;

	public BetterList<Transform>.CompareFunc onCustomSort;

	[SerializeField]
	[HideInInspector]
	private bool sorted;

	protected bool mReposition;

	protected UIPanel mPanel;

	protected bool mInitDone;

	public bool repositionNow
	{
		set
		{
			if (value)
			{
				mReposition = true;
				base.enabled = true;
			}
		}
	}

	public BetterList<Transform> GetChildList()
	{
		Transform transform = base.transform;
		BetterList<Transform> betterList = new BetterList<Transform>();
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (!hideInactive || ((bool)child && NGUITools.GetActive(child.gameObject)))
			{
				betterList.Add(child);
			}
		}
		return betterList;
	}

	public Transform GetChild(int index)
	{
		BetterList<Transform> childList = GetChildList();
		return (index >= childList.size) ? null : childList[index];
	}

	public void AddChild(Transform trans)
	{
		AddChild(trans, true);
	}

	public void AddChild(Transform trans, bool sort)
	{
		if (trans != null)
		{
			BetterList<Transform> childList = GetChildList();
			childList.Add(trans);
			ResetPosition(childList);
		}
	}

	public void AddChild(Transform trans, int index)
	{
		if (trans != null)
		{
			if (sorting != Sorting.None)
			{
				Debug.LogWarning("The Grid has sorting enabled, so AddChild at index may not work as expected.", this);
			}
			BetterList<Transform> childList = GetChildList();
			childList.Insert(index, trans);
			ResetPosition(childList);
		}
	}

	public Transform RemoveChild(int index)
	{
		BetterList<Transform> childList = GetChildList();
		if (index < childList.size)
		{
			Transform result = childList[index];
			childList.RemoveAt(index);
			ResetPosition(childList);
			return result;
		}
		return null;
	}

	public bool RemoveChild(Transform t)
	{
		BetterList<Transform> childList = GetChildList();
		if (childList.Remove(t))
		{
			ResetPosition(childList);
			return true;
		}
		return false;
	}

	protected virtual void Init()
	{
		mInitDone = true;
		mPanel = NGUITools.FindInParents<UIPanel>(base.gameObject);
	}

	protected virtual void Start()
	{
		if (!mInitDone)
		{
			Init();
		}
		bool flag = animateSmoothly;
		animateSmoothly = false;
		Reposition();
		animateSmoothly = flag;
		base.enabled = false;
	}

	protected virtual void Update()
	{
		if (mReposition)
		{
			Reposition();
		}
		base.enabled = false;
	}

	public static int SortByName(Transform a, Transform b)
	{
		return string.Compare(a.name, b.name);
	}

	public static int SortHorizontal(Transform a, Transform b)
	{
		return a.localPosition.x.CompareTo(b.localPosition.x);
	}

	public static int SortVertical(Transform a, Transform b)
	{
		return b.localPosition.y.CompareTo(a.localPosition.y);
	}

	protected virtual void Sort(BetterList<Transform> list)
	{
	}

	[ContextMenu("Execute")]
	public virtual void Reposition()
	{
		if (Application.isPlaying && !mInitDone && NGUITools.GetActive(this))
		{
			mReposition = true;
			return;
		}
		if (sorted)
		{
			sorted = false;
			if (sorting == Sorting.None)
			{
				sorting = Sorting.Alphabetic;
			}
			NGUITools.SetDirty(this);
		}
		if (!mInitDone)
		{
			Init();
		}
		BetterList<Transform> childList = GetChildList();
		if (sorting != Sorting.None)
		{
			if (sorting == Sorting.Alphabetic)
			{
				childList.Sort(SortByName);
			}
			else if (sorting == Sorting.Horizontal)
			{
				childList.Sort(SortHorizontal);
			}
			else if (sorting == Sorting.Vertical)
			{
				childList.Sort(SortVertical);
			}
			else if (onCustomSort != null)
			{
				childList.Sort(onCustomSort);
			}
			else
			{
				Sort(childList);
			}
		}
		ResetPosition(childList);
		if (keepWithinPanel)
		{
			ConstrainWithinPanel();
		}
		if (onReposition != null)
		{
			onReposition();
		}
	}

	public void ConstrainWithinPanel()
	{
		if (mPanel != null)
		{
			mPanel.ConstrainTargetToBounds(base.transform, true);
		}
	}

	protected void ResetPosition(BetterList<Transform> list)
	{
		mReposition = false;
		int i = 0;
		for (int size = list.size; i < size; i++)
		{
			list[i].parent = null;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		Transform transform = base.transform;
		int j = 0;
		for (int size2 = list.size; j < size2; j++)
		{
			Transform transform2 = list[j];
			transform2.parent = transform;
			float z = transform2.localPosition.z;
			Vector3 vector = ((arrangement != Arrangement.Horizontal) ? new Vector3(cellWidth * (float)num2, (0f - cellHeight) * (float)num, z) : new Vector3(cellWidth * (float)num, (0f - cellHeight) * (float)num2, z));
			if (animateSmoothly && Application.isPlaying)
			{
				SpringPosition.Begin(transform2.gameObject, vector, 15f).updateScrollView = true;
			}
			else
			{
				transform2.localPosition = vector;
			}
			num3 = Mathf.Max(num3, num);
			num4 = Mathf.Max(num4, num2);
			if (++num >= maxPerLine && maxPerLine > 0)
			{
				num = 0;
				num2++;
			}
		}
		if (pivot == UIWidget.Pivot.TopLeft)
		{
			return;
		}
		Vector2 pivotOffset = NGUIMath.GetPivotOffset(pivot);
		float num5;
		float num6;
		if (arrangement == Arrangement.Horizontal)
		{
			num5 = Mathf.Lerp(0f, (float)num3 * cellWidth, pivotOffset.x);
			num6 = Mathf.Lerp((float)(-num4) * cellHeight, 0f, pivotOffset.y);
		}
		else
		{
			num5 = Mathf.Lerp(0f, (float)num4 * cellWidth, pivotOffset.x);
			num6 = Mathf.Lerp((float)(-num3) * cellHeight, 0f, pivotOffset.y);
		}
		for (int k = 0; k < transform.childCount; k++)
		{
			Transform child = transform.GetChild(k);
			SpringPosition component = child.GetComponent<SpringPosition>();
			if (component != null)
			{
				component.target.x -= num5;
				component.target.y -= num6;
				continue;
			}
			Vector3 localPosition = child.localPosition;
			localPosition.x -= num5;
			localPosition.y -= num6;
			child.localPosition = localPosition;
		}
	}
}
