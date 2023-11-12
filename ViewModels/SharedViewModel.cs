using System.Collections.ObjectModel;

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
                SwitchFolder(value);

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
    public Command<object> RightpanelCommand { get; set; }

    public Command NavScrollTo { get; set; }
    public LoadingService MyLoadingService { get; set; }

    private string TemplateText_ { get; set; } = "VerticalGrid,2";
    public string TemplateText { get { return TemplateText_; } set { TemplateText_ = value; OnPropertyChanged(nameof(TemplateText)); } }

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
        RightpanelCommand = new Command<object>(RightPanelExecute);
    }

    private void RightPanelExecute(object obj)
    {

    }

    private void AddToBackHistory(ItemModel model)
    {
        HistoryBack.Add(model);
    }

    private void SwitchFolderPre(object path)
    {
        var item = LeftPanelItems.FirstOrDefault(i => i.Path.Equals(path));
        SwitchFolder(new ItemModel(item.Path));
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
        string startPath = string.Empty;

#if WINDOWS
         startPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
#endif

#if MACCATALYST
        startPath = Path.Combine(Environment.GetEnvironmentVariable("HOME"));
#endif

#if ANDROID
#if DEBUG
        startPath = Android.OS.Environment.StorageDirectory.AbsolutePath;
#else
        startPath = Android.OS.Environment.GetExternalStoragePublicDirectory.AbsolutePath;
#endif
#endif

        UpdateLeftPanel();

        firstBlocker = true;
        SwitchFolder(new ItemModel(startPath));
    }

    public static bool IsFolderAccesible(string path)
    {
        try
        {
            return Directory.GetFileSystemEntries(path).Any();
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    private void UpdateRightPanel()
    {
        RightPanelItems.Clear();

        var command = new Command(() =>
        {

        });
        var model = new RightPanelItemModel() { DeclaredCommandType = CommandType.BACK, DeclaredCommand = command };
        RightPanelItems.Add(model);
    }

    private void UpdateLeftPanel()
    {
        LeftPanelItems.Clear();

#if ANDROID
        _ = Task.Run(async () =>
        {
            var drives = DriveInfo.GetDrives();

            var path = Android.OS.Environment.RootDirectory.AbsolutePath;
            if (IsFolderAccesible(path))
                LeftPanelItems.Add(new LeftPanelItemModel() { Title = "Root", Icon = "drive2_dark", Path = path });

            path = Android.OS.Environment.StorageDirectory?.AbsolutePath;
            if (IsFolderAccesible(path))
                LeftPanelItems.Add(new LeftPanelItemModel() { Title = "Storage", Icon = "drive1_dark", Path = path });

            path = Android.OS.Environment.GetExternalStoragePublicDirectory(string.Empty).AbsolutePath;
            if (IsFolderAccesible(path))
                LeftPanelItems.Add(new LeftPanelItemModel() { Title = "Public Storage", Icon = "disk_dark", Path = path });

            path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
            if (IsFolderAccesible(path))
                LeftPanelItems.Add(new LeftPanelItemModel() { Title="Downloads", Icon = "download2_dark", Path = path });

            path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim).AbsolutePath;
            if (IsFolderAccesible(path))
                LeftPanelItems.Add(new LeftPanelItemModel() { Title = "Camera", Icon = "cam_dark", Path = path });

            path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).AbsolutePath;
            if (IsFolderAccesible(path))
                LeftPanelItems.Add(new LeftPanelItemModel() { Title = "Pictures", Icon = "image_dark", Path = path });

            path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryScreenshots).AbsolutePath;
            if (IsFolderAccesible(path))
                LeftPanelItems.Add(new LeftPanelItemModel() { Title = "Screenshots", Icon = "ss_dark", Path = path });

            path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments).AbsolutePath;
            if (IsFolderAccesible(path))
                LeftPanelItems.Add(new LeftPanelItemModel() { Title = "Documents", Icon = "docs_dark", Path = path });

            path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMusic).AbsolutePath;
            if (IsFolderAccesible(path))
                LeftPanelItems.Add(new LeftPanelItemModel() { Title = "Music", Icon = "music_dark", Path = path });

            path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMovies).AbsolutePath;
            if (IsFolderAccesible(path))
                LeftPanelItems.Add(new LeftPanelItemModel() { Title = "Movies", Icon = "film_dark", Path = path });
        });
#endif
    }

    private void UpdateRightPanelItems()
    {

    }

    private async void SearchItem(string name)
    {
        CancelLastOperation();
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

    private async Task UpdateNavigation(ItemModel model)
    {
        loopBlocker = true;
        Paths.Clear();
        await Task.Run(() =>
        {
            var navigationFolders = GetFoldersToRoot(model.FullPath);
            int index = 0;
            for (int i = 0; i < navigationFolders?.Count(); i++)
            {
                Paths.Add(navigationFolders.ElementAt(i));
                index++;
            }
            AddToBackHistory(SelectedFolder);
            SelectedFolder = Paths.Last();
            NavScrollTo.Execute(--index);
        });
        loopBlocker = false;
    }

    private void SelectItem(ItemModel item)
    {
        if (item?.Type is ItemType.FOLDER)
            SwitchFolder(item);
    }

    private async void SwitchFolder(ItemModel folder)
    {
        if (loopBlocker is true)
            return;

        CancelLastOperation();
        var cancelToken = GetNewOperationKey();
        ChangeOperationState(true);

        try
        {
            var all = GetFilesAndFolders(folder.FullPath);
            await UpdateNavigation(folder);
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




    public static IEnumerable<ItemModel> GetFilesAndFolders(string path)
    {
        var folders = GetFoldersInFolder(path);
        folders = folders?.OrderBy(f => f.Name).ToList();
        var files = GetFilesInFolder(path);
        files = files?.OrderBy(f => f.Name).ToList();
        return folders.Concat(files);
    }

    public static IEnumerable<ItemModel> GetFilesInFolder(string path)
    {
        if (string.IsNullOrEmpty(path))
            return null;
        return from fullpath in Directory.GetFiles(path)
               select new ItemModel()
               {
                   FullPath = fullpath,
                   Type = ItemType.FILE
               };
    }

    public static IEnumerable<ItemModel> GetFoldersInFolder(string path)
    {
        if (string.IsNullOrEmpty(path))
            return null;
        return from title in Directory.GetDirectories(path)
               select new ItemModel()
               {
                   FullPath = title,
                   Type = ItemType.FOLDER
               };
    }

    public static List<ItemModel> GetFoldersToRoot(string path)
    {
        var folders = new List<ItemModel>();
        while (path != null)
        {
            string fullpath = Path.GetFullPath(path);
            folders.Insert(0, new ItemModel() { FullPath = fullpath });
            path = Path.GetDirectoryName(path);
        }
        return folders;
    }
}
