using System.Collections.ObjectModel;
using Android.Content;
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
    public List<ItemModel> SelectedFileList { get; set; }

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
                SwitchFolder(value, SwitchProperties.SaveHistory);
        }
    }
    public string SelectedFolderSize { get { if (SelectedFolder is null) return "0"; else return StorageHelper.GetSizeAsString(new List<ItemModel>() { SelectedFolder }); } }
    public string SelectedFolderItemCount { get { if (SelectedFolder is null) return "0"; else return StorageHelper.GetDirectoryItemCount(SelectedFolder?.FullPath); } }
    public string SelectedFileNameOrCount
    {
        get
        {
            if (SelectedFileList?.Count > 0)
                return $"{SelectedFileList.Count} item selected.";
            else if (SelectedFile is null)
                return "No file selected.";
            else return SelectedFile.Name;
        }
    }
    public string SelectedFileSize
    {
        get
        {
            if (SelectedFileList?.Count > 0)
                return StorageHelper.GetSizeAsString(SelectedFileList);
            else if (SelectedFile is null)
                return "0 KB";
            else return StorageHelper.GetSizeAsString(new List<ItemModel>() { SelectedFile });
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

    public Command ClearSelectedItemsCommand { get; set; }
    public Command<object> SwitchFolderPreCommand { get; set; }
    public Command<object> RightPanelCommand { get; set; }
    public Command NavScrollTo { get; set; }
    public LoadingService MyLoadingService { get; set; }

    private bool firstBlocker;
    private bool loopBlocker;
    public BodyTemplateSelector bodyTemplateSelector { get; set; }
    public int BodyLayoutType
    {
        get
        {
            if (bodyTemplateSelector is null)
                return 0;
            else
                return (int)bodyTemplateSelector.SelectedTemplate;
        }
    }

    public enum SelectionModeTypes
    {
        Single,
        Multi
    }

    public SelectionModeTypes SelectionMode { get; set; }

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
        SelectionMode = SelectionModeTypes.Single;
        SelectedFileList = new List<ItemModel>();
    }

    public void InvokeSelectedFileList()
    {
        OnPropertyChanged(nameof(SelectedFileList));
        OnPropertyChanged(nameof(SelectedFileNameOrCount));
        OnPropertyChanged(nameof(SelectedFileSize));
        UpdateRightPanel();
    }

    private void SetBodyTemplate(BodyDisplayTemplates target)
    {
        if (bodyTemplateSelector is not null)
            bodyTemplateSelector.SelectedTemplate = target;
        OnPropertyChanged(nameof(BodyLayoutType));
        SwitchFolder(SelectedFolder, SwitchProperties.Empty);
    }

    private async void SwitchBodyTemplate()
    {
        await CancelLastOperation();
        VisualItems.Clear();
        BodyDisplayTemplates template = bodyTemplateSelector is not null ? bodyTemplateSelector.SelectedTemplate : BodyDisplayTemplates.SIMPLEROW;
        int index = ((int)template + 1) % Enum.GetValues(typeof(BodyDisplayTemplates)).Length;
        SetBodyTemplate((BodyDisplayTemplates)index);
    }

    private void RightPanelExecute(object obj)
    {
        var type = (CommandType)obj;
        switch (type)
        {
            case CommandType.REFRESH:
                SwitchFolder(SelectedFolder, SwitchProperties.Empty);
                break;
            case CommandType.BACK:
                var last1 = HistoryBack.LastOrDefault();
                HistoryForward.Add(SelectedFolder);
                HistoryBack.Remove(last1);
                SwitchFolder(last1, SwitchProperties.Empty);
                break;
            case CommandType.FORWARD:
                var forw1 = HistoryForward.LastOrDefault();
                HistoryForward.Remove(forw1);
                SwitchFolder(forw1, SwitchProperties.SaveHistory);
                break;
            case CommandType.SWITCH_DISPLAY:
                SwitchBodyTemplate();
                break;
            case CommandType.SWITCH_SELECTION:
                ClearSelectedItemsCommand.Execute(null);
                SelectedFileList.Clear();
                SelectedFile = null;
                int index = ((int)SelectionMode + 1) % Enum.GetValues(typeof(SelectionModeTypes)).Length;
                SelectionMode = (SelectionModeTypes)index;
                OnPropertyChanged(nameof(SelectionMode));
                UpdateRightPanel();
                break;
            case CommandType.OPEN_SELECTION:
                if (SelectionMode is SelectionModeTypes.Multi)
                {
                }
                else
                {
                  
                }
                break;
        }
    }

    private void SwitchFolderPre(object path)
    {
        var item = LeftPanelItems.FirstOrDefault(i => i.Path.Equals(path));
        SwitchFolder(new ItemModel(item.Path), SwitchProperties.SaveHistory);
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

    public override void OnAppear()
    {
        UpdateLeftPanel();
        firstBlocker = true;
        SwitchFolder(new ItemModel(StartHelper.GetStartFolder()), SwitchProperties.Empty);
    }

    private void UpdateRightPanel()
    {
        RightPanelItems.Clear();

        var sic = 0;
        if (SelectionMode is SelectionModeTypes.Multi)
            sic = SelectedFileList.Count;
        else sic = SelectedFile is not null ? 1 : 0;

        var hbc = HistoryBack.Count;
        var hfc = HistoryForward.Count;
        var sm = (int)SelectionMode;

        var items = RightPanelHelper.GetItems(hbc, hfc, sm, sic);
        items.ForEach(RightPanelItems.Add);
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
        ChangeOperationState(true, "Searching...");

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
            ChangeOperationState(false, string.Empty);
        });
    }

    private async Task UpdateNavigation(ItemModel model, SwitchProperties properties)
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
            if (properties is not SwitchProperties.Empty)
                HistoryBack.Add(SelectedFolder);
            SelectedFileList.Clear();
            ClearSelectedItemsCommand.Execute(this);
            SelectedFolder = Paths.Last();
            UpdateRightPanel();
            OnPropertyChanged(nameof(SelectedFolderSize));
            OnPropertyChanged(nameof(SelectedFolderItemCount));
            SelectedFile = null;
            NavScrollTo.Execute(--index);
        });
        loopBlocker = false;
    }

    private void SelectItem(ItemModel item)
    {
        if (item?.Type is ItemType.FOLDER)
            SwitchFolder(item, SwitchProperties.SaveHistory);
        OnPropertyChanged(nameof(SelectedFileNameOrCount));
        OnPropertyChanged(nameof(SelectedFileSize));
        UpdateRightPanel();
    }

    private enum SwitchProperties
    {
        Empty,
        SaveHistory
    }

    private async void SwitchFolder(ItemModel folder, SwitchProperties properties)
    {
        if (loopBlocker is true)
            return;

        await CancelLastOperation();
        var cancelToken = GetNewOperationKey();
        ChangeOperationState(true, "Directory constructing...");

        try
        {
            var all = StorageHelper.GetFilesAndFolders(folder.FullPath);
            await UpdateNavigation(folder, properties);
            await ClearItems();
            ChangeTabName(folder.Name);

            for (int i = 0; i < all?.Count(); i++)
            {
                if (cancelToken.IsCancellationRequested)
                    break;
                await Task.Run(() =>
                {
                    Items.Add(all.ElementAt(i));
                    VisualItems.Add(all.ElementAt(i));
                    ChangeOperationState(VisualItems.Count, all.Count);
                });
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
            ChangeOperationState(false, string.Empty);
            UpdateRightPanel();
        }
    }

}
