using System.Data;

namespace ASFileExplorer;

public enum CommandType
{
    BACK,
    FORWARD,
    CHANGE_SELECTION_MODE,
    SELECT_ALL,
    COPY,
    PASTE
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
    public Command DeclaredCommand { get; set; }

}

