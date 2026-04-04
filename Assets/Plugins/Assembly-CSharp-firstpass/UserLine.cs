using System.Collections.Generic;

public class UserLine
{
	public List<UserSegmentList> segmentLists = new List<UserSegmentList>();

	public void AddSegmentList(UserSegmentList list)
	{
		segmentLists.Add(list);
	}
}
