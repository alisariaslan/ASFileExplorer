using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;

namespace ASFileExplorer;

public class MainViewModel : BaseViewModel
{
    public int LastTabId;
    public int ActiveTabId;

    public TabModel SelectedTabModel_ { get; set; }
    public TabModel SelectedTabModel
    {
        get { return SelectedTabModel_; }
        set
        {
            SelectedTabModel_ = value;
            OnPropertyChanged(nameof(SelectedTabModel));
            if (firstBlocker)
                firstBlocker = false;
            else
                SwitchTab(value);
        }
    }

    public ContentView SelectedTabContent_ { get; set; }
    public ContentView SelectedTabContent { get { return SelectedTabContent_; } set { SelectedTabContent_ = value; OnPropertyChanged(nameof(SelectedTabContent)); } }

    public ObservableCollection<TabModel> TabList { get; set; }
    public IServiceProvider serviceProvider;

    public Command DelTabCommand { get; set; }
    public Command NewTabCommand { get; set; }
    public Command ScrollTo { get; set; }

    private bool firstBlocker;

    public MainViewModel()
    {
        TabList = new ObservableCollection<TabModel>();
        NewTabCommand = new Command(CreateNewTab);
        DelTabCommand = new Command(DelTab);
        SelectedTabContent = new EmptyView("New tab creating...");
    }

    private void DelTab(object id_)
    {
        var id = (int)id_;
        TabList.Remove(TabList.FirstOrDefault(t => t.Id == id));
        SelectedTabModel = TabList.Count > 0 ? TabList.Last() : null;
    }

    public async void CreateNewTab()
    {
        await Task.Run(() =>
        {
            var newtab = serviceProvider.GetRequiredService<SharedView>();
            var tabmodel = new TabModel(++LastTabId, $"New tab {TabList.Count + 1}", newtab);
            newtab.tab = tabmodel;
            TabList.Add(tabmodel);
            SelectedTabModel = tabmodel;
        });
    }

    public override void OnAppear()
    {
        firstBlocker = true;
        CreateNewTab();
    }

    private void SwitchTab(TabModel tabModel)
    {
        if (tabModel is null)
        {
            SelectedTabContent = new EmptyView();
            return;
        }
        else
        {
            var tab = TabList.FirstOrDefault(t => t.Id == tabModel.Id);
            SelectedTabContent = tab.Content;
            ActiveTabId = tab.Id;
            ScrollTo.Execute(tabModel.Id);
        }
    }

    public async Task WaitPermissions(IMessenger messenger, List<PermModel> permList)
    {
        messenger?.Send(new MessageData(MessageType.REQUEST_ALL_PERMS, this));

        var major = DeviceInfo.Version.Major;
        var build = DeviceInfo.Version.Build;

        if (major > 10)
            permList.Add(new PermModel(PermType.MANAGE_EXTERNAL_STORAGE));
        else
        {
            permList.Add(new PermModel(PermType.READ_EXTERNAL_STORAGE));
            permList.Add(new PermModel(PermType.WRITE_EXTERNAL_STORAGE));
        }
        if (major > 12)
        {
            permList.Add(new PermModel(PermType.READ_MEDIA_IMAGES));
            permList.Add(new PermModel(PermType.READ_MEDIA_VIDEO));
            permList.Add(new PermModel(PermType.READ_MEDIA_AUDIO));
        }
        while (permList.Any(p => p.Permitted is false))
        {
            foreach (var item in permList)
                messenger?.Send(new MessageData(MessageType.CHECK_ONE_PERM, item));
            await Task.Delay(1000);
            var perm = permList.FirstOrDefault(p => p.Permitted is false);
            if (perm is null)
                break;
            bool answer = await App.Current.MainPage.DisplayAlert("Warning", $"{perm.Key} is not permitted. This app needs all necessary permissions to work properly.", "Retry", "Open App Settings", FlowDirection.LeftToRight);
            if (answer is false)
                AppInfo.ShowSettingsUI();
        }
    }
}
