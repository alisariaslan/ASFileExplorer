namespace ASFileExplorer;

public enum ItemType
{
    FILE,
    FOLDER
}

public enum FileType
{
    UNKNOWN,
    RAWTEXT,
    ARCHIVE
}

public class ItemModel
{
    public int Index { get; set; }
    public string Name { get { return Path.GetFileName(FullPath); } }
    public string FolderPath { get { return Path.GetDirectoryName(FullPath); } }
    public string FullPath { get; set; }
    public string Extension { get { return Path.GetExtension(Name); } }
    public ItemType Type { get; set; }

    public bool StillExists
    {
        get
        {
            return Directory.Exists(FullPath);
        }
    }

    public string ImageSourceString
    {
        get
        {
            if (Type == ItemType.FOLDER)
                return "folder_dark";
            switch (Extension)
            {
                default:
                    return "file_dark";
            }
        }
    }

    public ItemModel() { }

    public ItemModel(string path)
    {
        FullPath = path;
    }
}
