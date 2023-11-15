namespace ASFileExplorer;

public interface TabInterface
{
    public bool GetOperationState();

    public void ChangeOperationState(bool state);

    public void ChangeTabName(string newName);
}

