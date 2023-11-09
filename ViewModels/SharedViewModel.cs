using System.Collections.ObjectModel;

namespace ASFileExplorer;

public class SharedViewModel : BaseViewModel
{
    public ObservableCollection<ItemModel> Paths { get; set; }
    public List<ItemModel> Items { get; set; }
    public ObservableCollection<ItemModel> VisualItems { get; set; }
    public ObservableCollection<PanelItemModel> LeftPanelItems { get; set; }
    public ObservableCollection<PanelItemModel> RightPanelItems { get; set; }

    public ItemModel SelectedFile_ { get; set; }
    public ItemModel SelectedFile { get { return SelectedFile_; } set { SelectedFile_ = value; OnPropertyChanged(nameof(SelectedFile)); SelectItem(value); } }

    private ItemModel SelectedFolder_ { get; set; }
    public ItemModel SelectedFolder { get { return SelectedFolder_; } set { SelectedFolder_ = value; OnPropertyChanged(nameof(SelectedFolder)); SwitchFolder(value); } }
    private string SearchText_ { get; set; }
    public string SearchText { get { return SearchText_; } set { SearchText_ = value; OnPropertyChanged(nameof(SearchText)); SearchItem(value); } }

    public Command<object> SwitchFolderPreCommand { get; set; }

    public Command NavScrollTo { get; set; }
    public LoadingService MyLoadingService { get; set; }

    public SharedViewModel()
    {
        Paths = new ObservableCollection<ItemModel>();
        Items = new List<ItemModel>();
        VisualItems = new ObservableCollection<ItemModel>();
        LeftPanelItems = new ObservableCollection<PanelItemModel>();
        RightPanelItems = new ObservableCollection<PanelItemModel>();
        SwitchFolderPreCommand = new Command<object>(SwitchFolderPre);
    }

    private void SwitchFolderPre(object path)
    {
        if (MyLoadingService.IsLoading)
            return;

        MyLoadingService.IsLoading = true;
        var item = LeftPanelItems.FirstOrDefault(i => i.Path.Equals(path));
        Task.Run(() => SwitchFolder(new ItemModel(item.Path)));
    }

    private void ClearItems()
    {
        Items.Clear();
        VisualItems.Clear();
    }

    private void AddItems(ItemModel model)
    {
        Items.Add(model);
        VisualItems.Add(model);
    }

    public override void OnAppear()
    {
        string startPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
        //startupFolder = Path.Combine(Environment.GetEnvironmentVariable("EXTERNAL_STORAGE"));
        //startupFolder = "/sdk_gphone64_arm64";
#if ANDROID
        startPath = Android.OS.Environment.GetExternalStoragePublicDirectory(string.Empty).AbsolutePath;
#endif

        SwitchFolder(new ItemModel(startPath));

        GetAllLeftPanelItems();
    }

    private async void GetAllLeftPanelItems()
    {
#if ANDROID
        var internalPath = Android.OS.Environment.GetExternalStoragePublicDirectory(string.Empty).AbsolutePath;
        LeftPanelItems.Add(new PanelItemModel() { Icon = "disk_dark", Path = internalPath });

        string externalStoragePath = null;

        var drives = DriveInfo.GetDrives();


        var downloadsPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
        LeftPanelItems.Add(new PanelItemModel() { Icon = "download2_dark",Path = downloadsPath });

        var imagesPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).AbsolutePath;
        LeftPanelItems.Add(new PanelItemModel() { Icon = "images_dark", Path = imagesPath });

        var filmPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMovies).AbsolutePath;
        LeftPanelItems.Add(new PanelItemModel() { Icon = "film_dark", Path = filmPath });
#endif
    }


    #region SEARCH
    private int searcher;
    private async void SearchItem(string name)
    {
        searcher++;
        if (searcher > 1)
            return;

        _ = Task.Run(async () => {
            if(string.IsNullOrEmpty(name) is false)
            {
                var all = GetFilesAndFolders(SelectedFolder.FullPath);
                if (all is null)
                    return;

                VisualItems.Clear();
                var filteredFiles = all.Where(item => item.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
                for (int i = 0; i < filteredFiles.Count(); i++)
                    VisualItems.Add(filteredFiles.ElementAt(i));
            } else if(VisualItems.Count != Items.Count)
            {
                VisualItems.Clear();
                for (int i = 0; i < Items.Count; i++)
                    VisualItems.Add(Items[i]);
            }

            while (searcher > 0)
                searcher--;
        });
    }
    #endregion

    private void UpdateNavigation(string path)
    {
        Paths.Clear();
        var navigationFolders = GetFoldersToRoot(path);
        int index = 0;
        for (int i = 0; i < navigationFolders?.Count(); i++)
        {
            Paths.Add(navigationFolders.ElementAt(i));
            index++;
        }
        SelectedFolder = Paths.Last();
        NavScrollTo.Execute(--index);
    }

    private void SelectItem(ItemModel item)
    {
        if (item?.Type is ItemType.FOLDER)
            SwitchFolder(item);
    }

    #region SWITCH FOLDER
    private int switcher;
    private async void SwitchFolder(ItemModel folder)
    {
        switcher++;
        if (switcher > 1)
            return;
        await Task.Delay(10);

        try
        {
            var all = GetFilesAndFolders(folder.FullPath);
            ClearItems();
            for (int i = 0; i < all?.Count(); i++)
                AddItems(all.ElementAt(i));
            UpdateNavigation(folder.FullPath);
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
            while (switcher > 0)
                switcher--;
            MyLoadingService.IsLoading = false;
        }
    }
    #endregion

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
