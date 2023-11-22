using CommunityToolkit.Mvvm.Messaging;

namespace ASFileExplorer;

public partial class SharedView : ContentView
{
    public TabModel tab;
    private LoadingService loadingService;
    private bool initialized;
    private IMessenger messenger;

    public SharedView(LoadingService loadingService, IMessenger messenger)
    {
        InitializeComponent();
        this.loadingService = loadingService;
        this.messenger = messenger;
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
        vm.RegisterMessenger(messenger);
        vm.bodyTemplateSelector = GetObjectFromResources("customDataTemplateSelector") as BodyTemplateSelector;
        vm.NavScrollTo = new Command<int>(NavScrollTo);
        vm.MyLoadingService = loadingService;
        vm.MyTab = tab;
        vm.ClearSelectedItemsCommand = new Command(() => cview_body.SelectedItems.Clear());
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

    private object GetObjectFromResources(string obj)
    {
        return this.Resources[obj];
    }

    void CollectionView_SelectionChanged(System.Object sender, Microsoft.Maui.Controls.SelectionChangedEventArgs e)
    {
        var vm = this.BindingContext as SharedViewModel;
        if (vm.SelectionMode is SharedViewModel.SelectionModeTypes.Multi)
        {
            vm.SelectedFileList.Clear();
            foreach (var item in e.CurrentSelection)
                vm.SelectedFileList.Add(item as ItemModel);
            vm.InvokeSelectedFileList();
        }
    }

}
