namespace ASFileExplorer.Helpers;

public class RightPanelHelper
{
    public static List<RightPanelItemModel> GetItems(int hbc, int hfc, int sm, int sic)
    {
        var list = new List<RightPanelItemModel>();
        var model = new RightPanelItemModel();

        model = new RightPanelItemModel() { Icon = "open_dark", DeclaredCommandType = CommandType.OPEN_SELECTION };
        if (sic == 1)
            list.Add(model);

        model = new RightPanelItemModel() { Icon = "left_dark", DeclaredCommandType = CommandType.BACK };
        if (hbc > 0)
            list.Add(model);

        model = new RightPanelItemModel() { Icon = "right_dark", DeclaredCommandType = CommandType.FORWARD };
        if (hfc > 0)
            list.Add(model);

        model = new RightPanelItemModel() { Icon = "refresh_dark", DeclaredCommandType = CommandType.REFRESH };
        list.Add(model);

        model = new RightPanelItemModel() { Icon = "col_dark", DeclaredCommandType = CommandType.SWITCH_DISPLAY };
        list.Add(model);

        var icon = sm == 0 ? "hand_dark" : "select_dark";
        model = new RightPanelItemModel() { Icon = icon, DeclaredCommandType = CommandType.SWITCH_SELECTION };
        list.Add(model);



        return list;
    }
}

