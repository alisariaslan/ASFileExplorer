namespace ASFileExplorer;

public interface TabInterface
{
    public bool GetOperationState();

    public void ChangeTabName(string newName);

    public void ChangeOperationState(bool state, string desc);

    public void ChangeOperationState(long loadCount, long maxLoadCount);
}

