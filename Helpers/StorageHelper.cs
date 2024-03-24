namespace ASFileExplorer.Helpers;

public class StorageHelper
{

    public static ulong GetSizeRaw(ItemModel model)
    {
        ulong size = 0;
        try
        {
            if (model.Type == ItemType.FILE)
            {
                FileInfo fileInfo = new FileInfo(model.FullPath);
                size = (ulong)fileInfo.Length;
            }
            else
            {
                foreach (string filePath in Directory.EnumerateFiles(model.FullPath, "*", SearchOption.AllDirectories))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    size += (ulong)fileInfo.Length;
                }
            }
        }
        catch (Exception)
        {

        }
        return size;
    }

    public static string GetSizeAsString(List<ItemModel> models)
    {
        if (models is null || models.Count == 0)
            return null;

        ulong size = 0;
        foreach (var item in models)
        {
            size += GetSizeRaw(item);
        }
        string[] sizeSuffixes = { "B", "KB", "MB", "GB", "TB" };
        int i = 0;
        while (size >= 1024 && i < sizeSuffixes.Length - 1)
        {
            size /= 1024;
            i++;
        }
        return $"{size:n1} {sizeSuffixes[i]}";
    }

    public static string GetDirectoryItemCount(string path)
    {
        if (string.IsNullOrEmpty(path))
            return null;

        return GetFilesAndFolders(path).Count.ToString("n0");
    }

    public static bool IsFolderAccesible(string path)
    {
        try
        {
            return Directory.GetFileSystemEntries(path).Any();
        }
        catch (Exception)
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
            folders.Insert(0, new ItemModel() { FullPath = fullpath, Type = ItemType.FOLDER });
            path = Path.GetDirectoryName(path);
        }
        return folders;
    }


}

