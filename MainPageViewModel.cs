using System.Collections.ObjectModel;
using Bumptech.Glide.Load.Engine;

namespace ASFileExplorer;

public class MainPageViewModel : BaseViewModel
{
    public ObservableCollection<ItemModel> Paths { get; set; }
    public List<ItemModel> Items { get; set; }
    public ObservableCollection<ItemModel> VisualItems { get; set; }

    public ItemModel SelectedFile_ { get; set; }
    public ItemModel SelectedFile { get { return SelectedFile_; } set { SelectedFile_ = value; OnPropertyChanged(nameof(SelectedFile)); SelectItem(value); } }

    private ItemModel SelectedFolder_ { get; set; }
    public ItemModel SelectedFolder { get { return SelectedFolder_; } set { SelectedFolder_ = value; OnPropertyChanged(nameof(SelectedFolder)); SwitchFolder(value,true); } }
    private string SearchText_ { get; set; }
    public string SearchText { get { return SearchText_; } set { SearchText_ = value; OnPropertyChanged(nameof(SearchText)); SearchItem(value); } }

    bool fileRecursiveBlocker = false;
    bool folderRecursiveBlocker = true;

    public MainPageViewModel()
    {
        Paths = new ObservableCollection<ItemModel>();
        Items = new List<ItemModel>();
        VisualItems = new ObservableCollection<ItemModel>();
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
        string startupFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
        //startupFolder = Path.Combine(Environment.GetEnvironmentVariable("EXTERNAL_STORAGE"));
        //startupFolder = "/sdk_gphone64_arm64";
        startupFolder = Android.OS.Environment.GetExternalStoragePublicDirectory(string.Empty).AbsolutePath;

        UpdatePath(startupFolder,false);
    }

    private void SearchItem(string name)
    {
        VisualItems.Clear();
        if (name is null)
        {
            for (int i = 0; i < Items.Count; i++)
                VisualItems.Add(Items[i]);
        }
        else
        {
            var all = GetFilesAndFolders(SelectedFolder.FullPath);
            if (all is null)
                return;

            var filteredFiles = all.Where(item => item.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            for (int i = 0; i < filteredFiles.Count(); i++)
                VisualItems.Add(filteredFiles.ElementAt(i));
        }
        OnPropertyChanged(nameof(VisualItems));
    }

    private async void UpdatePath(string path,bool recursiveBlock)
    {
        folderRecursiveBlocker = true;
        Paths.Clear();
        var navigationFolders = GetFoldersToRoot(path);
        int index = 0;
        for (int i = 0; i < navigationFolders?.Count(); i++)
        {
            Paths.Add(navigationFolders.ElementAt(i));
            index++;
        }
        SelectedFolder = Paths.Last();
        SwitchFolder(SelectedFolder,false);
        NavScrollTo.Execute(--index);
        OnPropertyChanged(nameof(Paths));
        await Task.Delay(100);
        folderRecursiveBlocker = false;
    }

    private void SelectItem(ItemModel item)
    {
        if (item?.Type is ItemType.FOLDER)
            SwitchFolder(item,false);
    }


    private void SwitchFolder(ItemModel folder,bool blockerCheck)
    {
        if(folderRecursiveBlocker && blockerCheck)
            return;

        _ = Task.Run(async () =>
        {
            try
            {
                var all = GetFilesAndFolders(folder.FullPath);
                ClearItems();
                for (int i = 0; i < all?.Count(); i++)
                    AddItems(all.ElementAt(i));
            }
            catch (System.UnauthorizedAccessException ex)
            {
                App.Current.Dispatcher.Dispatch(() =>
                {
                    App.Current.MainPage.DisplayAlert("Error occured", $"Access denied: {folder.FullPath}", "Ok");
                });
            }
            catch (Exception ex)
            {
                App.Current.Dispatcher.Dispatch(() =>
                {
                    App.Current.MainPage.DisplayAlert("Error occured", ex.Message, "Ok");
                });
            }
            finally
            {
                UpdatePath(folder.FullPath,true);
                OnPropertyChanged(nameof(Items));
                OnPropertyChanged(nameof(VisualItems));
            }
        });
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
