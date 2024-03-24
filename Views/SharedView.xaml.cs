using CommunityToolkit.Mvvm.Messaging;
using Syncfusion.Maui.ListView;

namespace ASFileExplorer;

public partial class SharedView : ContentView
{
    public TabModel tab;
    private bool initialized;
    private SharedViewModel ViewModel { get; set; }

    public SharedView(LoadingService MyLoadingService, IMessenger MyMessenger)
    {
        InitializeComponent();
        this.ViewModel = this.BindingContext as SharedViewModel;
        this.ViewModel.View = this;
        this.ViewModel.RegisterMessenger(MyMessenger);
        this.ViewModel.bodyTemplateSelector = GetObjectFromResources("customDataTemplateSelector") as BodyTemplateSelector;
        this.ViewModel.MyLoadingService = MyLoadingService;
    }

    public void NavScrollTo(int n)
    {
        if (n < 0)
            return;
        cv_nav.ScrollTo(n);
    }

    void ContentView_Loaded(System.Object sender, System.EventArgs e)
    {
        if (initialized)
            return;
        initialized = true;
        this.ViewModel.MyTab = tab;
        this.ViewModel.OnAppear();
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

    public void ClearItemSelections()
    {
        cview_body.SelectedItems.Clear();
    }

    void cview_body_SelectionChanged(System.Object sender, Syncfusion.Maui.ListView.ItemSelectionChangedEventArgs e)
    {
        var items = (sender as SfListView).SelectedItems.ToList();
        foreach (var item in items)
        {
            if (item is ItemModel model)
            {
                ViewModel.SelectedItems.Add(model);
            }
        }
        ViewModel.SelectedItemsChanged();
    }


}
