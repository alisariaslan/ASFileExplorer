using System;
using System.IO;
using Microsoft.Maui.Controls;

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

    public bool IsImageSourceAvaible
    {
        get
        {
            if (this.ImageSource is null)
                return false;
            else return true;
        }
    }

    public ImageSource ImageSource
    {
        get
        {
            try
            {
                if (Type is not ItemType.FILE && !Extension.Equals("jpg"))
                    throw new Exception(null);
                var bytes = File.ReadAllBytes(FullPath);
                return ImageSource.FromStream(() => new MemoryStream(bytes));
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }
    }

    public string ImageSourceString
    {
        get
        {
            if (Type == ItemType.FOLDER)
                return "folder_dark";
            else
                return "file_dark";
        }
    }

    public ItemModel() { }

    public ItemModel(string path)
    {
        FullPath = path;
    }
}
