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

    public void ChangeOperationState(bool state)
    {
        OnOperation = state;
        OnPropertyChanged(nameof(OnOperation));
    }

    public void ChangeTabName(string newName)
    {
        Name = newName;
        OnPropertyChanged(nameof(Name));
    }

}
