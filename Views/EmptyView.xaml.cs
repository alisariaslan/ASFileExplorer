namespace ASFileExplorer;

public partial class EmptyView : ContentView
{

    public EmptyView()
    {
        InitializeComponent();
    }

    public EmptyView(string text)
	{
		InitializeComponent();
		label_text.Text = text;
	}

}