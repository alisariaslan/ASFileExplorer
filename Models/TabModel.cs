namespace ASFileExplorer;

public class TabModel : PropertyNotifier , TabInterface
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ContentView Content { get; set; }

    public bool OnOperation { get; set; }

    public TabModel(int index, string name,ContentView content)
    {
        this.Id = index;
        this.Name = name;
        this.Content = content;
    }

    public void ChangeTabName(string newName)
    {
        Name = newName;
        OnPropertyChanged(nameof(Name));
    }

    public bool GetOperationState()
    {
        return OnOperation;
    }

    public void ChangeOperationState(bool state, string desc)
    {
        OnOperation = state;
        OnPropertyChanged(nameof(OnOperation));
    }

    public void ChangeOperationState(long loadCount, long maxLoadCount)
    {
        throw new NotImplementedException();
    }
}
