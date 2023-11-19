namespace ASFileExplorer;

public enum CommandType
{
    REFRESH,
    BACK,
    FORWARD,
    SWITCH_DISPLAY,
    SWITCH_SELECTION
}

public class LeftPanelItemModel
{
	public string Title { get; set; }
	public string Path { get; set; }
	public string Icon { get; set; }

}

public class RightPanelItemModel : PropertyNotifier
{
    public string Icon { get; set; }
    public CommandType DeclaredCommandType { get; set; }

}

