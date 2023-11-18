namespace ASFileExplorer.Helpers;

public class StorageHelper
{

	public static string GetDirectorySize(string path)
	{
		if (string.IsNullOrEmpty(path))
		return null;
	
		long size = 0;
		try
		{
			foreach (string filePath in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
			{
				FileInfo fileInfo = new FileInfo(filePath);
				size += fileInfo.Length;
			}
		}
		catch (Exception) {}
		string[] sizeSuffixes = { "B", "KB", "MB", "GB", "TB" };
		int i = 0;
		double size1 = size;
		while (size >= 1024 && i < sizeSuffixes.Length - 1)
		{
			size /= 1024;
			i++;
		}
		return $"{size1:n1} {sizeSuffixes[i]}";
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

