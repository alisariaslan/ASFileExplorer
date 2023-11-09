namespace ASFileExplorer;

public partial class SharedView : ContentView
{
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
        await Task.Delay(100);
        vm.OnAppear();
        vm.StopLoad();
    }

    bool clicked = false;
    async void ImageButton_Clicked(System.Object sender, System.EventArgs e)
    {
        if (clicked)
            return;

        clicked = true;
        var sndr = sender as VisualElement;
        while (loadingService.IsLoading)
        {
            await sndr.RelRotateTo(360, 1000);
        }
        clicked = false;
    }
}
