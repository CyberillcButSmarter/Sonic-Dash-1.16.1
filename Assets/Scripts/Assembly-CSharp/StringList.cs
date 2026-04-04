using System.Collections;
using UnityEngine;

public class StringList : MonoBehaviour
{
	public UIDragScrollView m_largeHeadingEntry;

	public UIDragScrollView m_smallHeadingEntry;

	public UIDragScrollView m_descriptionEntry;

	private UIGrid m_displayGrid;

	private UIScrollView m_draggablePanel;

	private void Start()
	{
		m_displayGrid = Utils.FindBehaviourInTree(this, m_displayGrid);
		m_draggablePanel = Utils.FindBehaviourInTree(this, m_draggablePanel);
		m_displayGrid.sorting = UIGrid.Sorting.Alphabetic;
	}

	private void OnEnable()
	{
		StartCoroutine(StartPendingActivation());
	}

	private IEnumerator StartPendingActivation()
	{
		yield return null;
		m_draggablePanel.ResetPosition();
		PopulatePanelEntries();
		m_displayGrid.Reposition();
		m_draggablePanel.ResetPosition();
	}

	private void PopulatePanelEntries()
	{
		LanguageStrings first = LanguageStrings.First;
		string[] allStrings = first.GetAllStrings();
		int num = 0;
		string[] array = allStrings;
		foreach (string text in array)
		{
			UIDragScrollView templatePanel = GetTemplatePanel();
			if (!(templatePanel == null))
			{
				GameObject gameObject = NGUITools.AddChild(base.gameObject, templatePanel.gameObject);
				SetObjectLabel(gameObject, "DebugEntry_StringList_Id", num.ToString());
				SetObjectLabel(gameObject, "DebugEntry_StringList_String", text);
				gameObject.name = string.Format("{0} String Entry", num.ToString("D3"));
				num++;
			}
		}
	}

	private void SetObjectLabel(GameObject source, string tag, string text)
	{
		GameObject gameObject = Utils.FindTagInChildren(source, tag);
		if (!(gameObject == null))
		{
			UILabel component = gameObject.GetComponent<UILabel>();
			if (!(component == null))
			{
				component.text = text;
			}
		}
	}

	private UIDragScrollView GetTemplatePanel()
	{
		return m_smallHeadingEntry;
	}
}
