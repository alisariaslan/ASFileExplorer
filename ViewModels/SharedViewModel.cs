using System.Collections.ObjectModel;
using ASFileExplorer.Helpers;

namespace ASFileExplorer;

public class SharedViewModel : BaseViewModel
{
    public ObservableCollection<ItemModel> Paths { get; set; }
    public List<ItemModel> Items { get; set; }
    public ObservableCollection<ItemModel> VisualItems { get; set; }
    public ObservableCollection<LeftPanelItemModel> LeftPanelItems { get; set; }
    public ObservableCollection<RightPanelItemModel> RightPanelItems { get; set; }
    public List<ItemModel> HistoryBack { get; set; }
    public List<ItemModel> HistoryForward { get; set; }
    public ItemModel SelectedFile_ { get; set; }
    public ItemModel SelectedFile
    {
        get { return SelectedFile_; }
        set
        {
            SelectedFile_ = value;
            OnPropertyChanged(nameof(SelectedFile));
            SelectItem(value);
        }
    }

    private ItemModel SelectedFolder_ { get; set; }
    public ItemModel SelectedFolder
    {
        get { return SelectedFolder_; }
        set
        {
            SelectedFolder_ = value;
            OnPropertyChanged(nameof(SelectedFolder));
            if (firstBlocker)
                firstBlocker = false;
            else
                SwitchFolder(value,null);
        }
    }
    private string SearchText_ { get; set; }
    public string SearchText
    {
        get { return SearchText_; }
        set
        {
            SearchText_ = value; OnPropertyChanged(nameof(SearchText));
            SearchItem(value);
        }
    }

    public Command<object> SwitchFolderPreCommand { get; set; }
    public Command<object> RightPanelCommand { get; set; }

    public Command NavScrollTo { get; set; }
    public LoadingService MyLoadingService { get; set; }

    //private bool TemplateText_ { get; set; } = "VerticalGrid,2";
    //public bool TemplateText { get { return TemplateText_; } set { TemplateText_ = value; OnPropertyChanged(nameof(TemplateText)); } }

    private bool firstBlocker;
    private bool loopBlocker;

    public SharedViewModel()
    {
        Paths = new ObservableCollection<ItemModel>();
        Items = new List<ItemModel>();
        VisualItems = new ObservableCollection<ItemModel>();
        LeftPanelItems = new ObservableCollection<LeftPanelItemModel>();
        RightPanelItems = new ObservableCollection<RightPanelItemModel>();
        SwitchFolderPreCommand = new Command<object>(SwitchFolderPre);
        HistoryBack = new List<ItemModel>();
        HistoryForward = new List<ItemModel>();
        RightPanelCommand = new Command<object>(RightPanelExecute);
    }

    private void RightPanelExecute(object obj)
    {
        var type = (CommandType)obj;
        switch (type)
        {
            case CommandType.BACK:
                var last1 = HistoryBack.LastOrDefault();
                HistoryForward.Add(SelectedFolder);
                HistoryBack.Remove(last1);
                SwitchFolder(last1,"noHistory");
                break;
            case CommandType.FORWARD:
                var forw1 = HistoryForward.LastOrDefault();
                HistoryForward.Remove(forw1);
                SwitchFolder(forw1, null);
                break;
        }
    }

    private void SwitchFolderPre(object path)
    {
        var item = LeftPanelItems.FirstOrDefault(i => i.Path.Equals(path));
        SwitchFolder(new ItemModel(item.Path),null);
    }

    private async Task ClearItems()
    {
        await Task.Run(() =>
        {
            Items.Clear();
            VisualItems.Clear();
        });
    }

    private async Task ClearOnlyVisualItems()
    {
        await Task.Run(() =>
        {
            VisualItems.Clear();
        });
    }

    private async Task AddItems(ItemModel model)
    {
        await Task.Run(() =>
        {
            Items.Add(model);
            VisualItems.Add(model);
        });
    }

    public override void OnAppear()
    {
        UpdateLeftPanel();
        firstBlocker = true;
        SwitchFolder(new ItemModel(StartHelper.GetStartFolder()),"noHistory");
    }

    private void UpdateRightPanel()
    {
        RightPanelItems.Clear();

        var model = new RightPanelItemModel() { Icon = "left_dark", DeclaredCommandType = CommandType.BACK };
        if (HistoryBack.Count > 0)
            RightPanelItems.Add(model);

        model = new RightPanelItemModel() { Icon = "right_dark", DeclaredCommandType = CommandType.FORWARD };
        if (HistoryForward.Count > 0)
            RightPanelItems.Add(model);
    }

    private void UpdateLeftPanel()
    {
        LeftPanelItems.Clear();
        Task.Run(() =>
        {
            var items = LeftPanelHelper.GetItems();
            items.ForEach(LeftPanelItems.Add);
        });
    }

    private async void SearchItem(string name)
    {
        await CancelLastOperation();
        var token = GetNewOperationKey();
        ChangeOperationState(true);

        await ClearOnlyVisualItems();

        await Task.Run(() =>
        {
            if (string.IsNullOrEmpty(name))
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (token.IsCancellationRequested)
                        break;
                    VisualItems.Add(Items[i]);
                }
            }
            else
            {
                var filteredFiles = Items.Where(item => item.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
                for (int i = 0; i < filteredFiles.Count(); i++)
                {
                    if (token.IsCancellationRequested)
                        break;
                    VisualItems.Add(filteredFiles.ElementAt(i));
                }
            }
            ChangeOperationState(false);
        });
    }

    private async Task UpdateNavigation(ItemModel model, object? param)
    {
        loopBlocker = true;
        Paths.Clear();
        await Task.Run(() =>
        {
            var navigationFolders = StorageHelper.GetFoldersToRoot(model.FullPath);
            int index = 0;
            for (int i = 0; i < navigationFolders?.Count(); i++)
            {
                Paths.Add(navigationFolders.ElementAt(i));
                index++;
            }
            if (param is not ("noHistory"))
                HistoryBack.Add(SelectedFolder);
            UpdateRightPanel();
            SelectedFolder = Paths.Last();
            NavScrollTo.Execute(--index);
        });
        loopBlocker = false;
    }

    private void SelectItem(ItemModel item)
    {
        if (item?.Type is ItemType.FOLDER)
            SwitchFolder(item,null);
    }

    private async void SwitchFolder(ItemModel folder,object param)
    {
        if (loopBlocker is true)
            return;

        await CancelLastOperation();
        var cancelToken = GetNewOperationKey();
        ChangeOperationState(true);

        try
        {
            var all = StorageHelper.GetFilesAndFolders(folder.FullPath);
            await UpdateNavigation(folder,param);
            await ClearItems();
            ChangeTabName(folder.Name);
            for (int i = 0; i < all?.Count(); i++)
            {
                if (cancelToken.IsCancellationRequested)
                    break;
                await AddItems(all.ElementAt(i));
            }
        }
        catch (System.UnauthorizedAccessException ex)
        {
            await App.Current.MainPage.DisplayAlert("Error occured", $"Access denied: {folder.FullPath}", "Ok");

        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Error occured", ex.Message, "Ok");
        }
        finally
        {
            ChangeOperationState(false);
            UpdateRightPanel();
        }
    }
}
