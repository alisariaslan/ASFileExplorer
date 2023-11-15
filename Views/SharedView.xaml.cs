namespace ASFileExplorer;

public partial class SharedView : ContentView
{
    public TabModel tab;
    private LoadingService loadingService;
    private bool initialized;

	public SharedView(LoadingService loadingService)
	{
		InitializeComponent();
		this.loadingService = loadingService;
    }

    private void NavScrollTo(int n)
    {
        if (n < 0)
            return;
        cv_nav.ScrollTo(n);
    }

    async void ContentView_Loaded(System.Object sender, System.EventArgs e)
    {
        if (initialized)
            return;
        initialized = true;

        var vm = this.BindingContext as SharedViewModel;
		vm.NavScrollTo = new Command<int>(NavScrollTo);
        vm.MyLoadingService = loadingService;
        vm.MyTab = tab;
        await Task.Delay(100);
        vm.OnAppear();
    }

    async void ImageButton_Clicked(System.Object sender, System.EventArgs e)
    {
        var btn = sender as VisualElement;
        btn.IsEnabled = false;
        await btn.ScaleTo(0.5f, 125);
        await btn.ScaleTo(1f, 125);
        btn.IsEnabled = true;
    }

	private DataTemplate GetTemplate(BodyDisplayTemplates template)
	{
		return this.Resources[$"template_{template.ToString().ToLower()}"] as DataTemplate;
	}


}
