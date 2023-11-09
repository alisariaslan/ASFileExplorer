namespace ASFileExplorer;

public class TabModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ContentView Content { get; set; }

    public TabModel(int index, string name,ContentView content)
    {
        this.Id = index;
        this.Name = name;
        this.Content = content;
    }

}
