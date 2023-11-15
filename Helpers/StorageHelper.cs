namespace ASFileExplorer.Helpers;

public class StorageHelper
{
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

    public static List<ItemModel> GetFilesAndFolders(string path)
    {
        var items = GetFoldersInFolder(path);
        items = items?.OrderBy(f => f.Name).ToList();
        var files = GetFilesInFolder(path);
        files = files?.OrderBy(f => f.Name).ToList();
        foreach (var item in files)
            items.Add(item);
        return items;
    }

    public static List<ItemModel> GetFilesInFolder(string path)
    {
        var items = Directory.GetFiles(path);
        var list = new List<ItemModel>();
        foreach (var fp in items)
        {
            list.Add(new ItemModel() { FullPath = fp, Type = ItemType.FILE });
        }
        return list;
    }

    public static List<ItemModel> GetFoldersInFolder(string path)
    {
        var items = Directory.GetDirectories(path);
        var list = new List<ItemModel>();
        foreach (var fp in items)
        {
            list.Add(new ItemModel() { FullPath = fp, Type = ItemType.FOLDER });
        }
        return list;
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

