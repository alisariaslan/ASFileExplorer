using System;
namespace ASFileExplorer.Helpers;

public class LeftPanelHelper
{
    public static List<LeftPanelItemModel> GetItems()
    {
        var list = new List<LeftPanelItemModel>();

        var drives = DriveInfo.GetDrives();

#if ANDROID

        var path = Android.OS.Environment.RootDirectory.AbsolutePath;
        if (StorageHelper.IsFolderAccesible(path))
            list.Add(new LeftPanelItemModel() { Title = "Root", Icon = "drive2_dark", Path = path });

        path = Android.OS.Environment.StorageDirectory?.AbsolutePath;
        if (StorageHelper.IsFolderAccesible(path))
            list.Add(new LeftPanelItemModel() { Title = "Storage", Icon = "drive1_dark", Path = path });

        path = Android.OS.Environment.GetExternalStoragePublicDirectory(string.Empty).AbsolutePath;
        if (StorageHelper.IsFolderAccesible(path))
            list.Add(new LeftPanelItemModel() { Title = "Public Storage", Icon = "disk_dark", Path = path });

        path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
        if (StorageHelper.IsFolderAccesible(path))
            list.Add(new LeftPanelItemModel() { Title = "Downloads", Icon = "download2_dark", Path = path });

        path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim).AbsolutePath;
        if (StorageHelper.IsFolderAccesible(path))
            list.Add(new LeftPanelItemModel() { Title = "Camera", Icon = "cam_dark", Path = path });

        path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).AbsolutePath;
        if (StorageHelper.IsFolderAccesible(path))
            list.Add(new LeftPanelItemModel() { Title = "Pictures", Icon = "image_dark", Path = path });

        path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryScreenshots).AbsolutePath;
        if (StorageHelper.IsFolderAccesible(path))
            list.Add(new LeftPanelItemModel() { Title = "Screenshots", Icon = "ss_dark", Path = path });

        path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments).AbsolutePath;
        if (StorageHelper.IsFolderAccesible(path))
            list.Add(new LeftPanelItemModel() { Title = "Documents", Icon = "docs_dark", Path = path });

        path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMusic).AbsolutePath;
        if (StorageHelper.IsFolderAccesible(path))
            list.Add(new LeftPanelItemModel() { Title = "Music", Icon = "music_dark", Path = path });

        path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMovies).AbsolutePath;
        if (StorageHelper.IsFolderAccesible(path))
            list.Add(new LeftPanelItemModel() { Title = "Movies", Icon = "film_dark", Path = path });
#endif

        return list;
    }
}

