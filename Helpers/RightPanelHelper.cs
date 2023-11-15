using System;
namespace ASFileExplorer.Helpers;

public class RightPanelHelper
{
    public static List<RightPanelItemModel> GetItems(int hbc, int hfc)
    {
        var list = new List<RightPanelItemModel>();

		var model = new RightPanelItemModel() { Icon = "left_dark", DeclaredCommandType = CommandType.BACK };
		if (hbc > 0)
			list.Add(model);

		model = new RightPanelItemModel() { Icon = "right_dark", DeclaredCommandType = CommandType.FORWARD };
		if (hfc > 0)
			list.Add(model);

        return list;
    }
}

