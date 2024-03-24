namespace ASFileExplorer.Helpers;

public class RightPanelHelper
{
    public static List<RightPanelItemModel> GetItems(int hbc, int hfc, int sic)
    {
        var list = new List<RightPanelItemModel>();
        var model = new RightPanelItemModel();

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

        model = new RightPanelItemModel() { Icon = "copy_dark", DeclaredCommandType = CommandType.COPY };
        if (sic == 1)
            list.Add(model);


        return list;
    }
}

