namespace ASFileExplorer.Helpers;

public class StartHelper
{
    public static string GetStartFolder()
    {
        var pathList = new List<string>();

#if WINDOWS
         pathList.Add(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
#endif

#if MACCATALYST
        pathList.Add(Path.Combine(Environment.GetEnvironmentVariable("HOME")));
#endif

#if ANDROID
        pathList.Add(Android.OS.Environment.StorageDirectory.AbsolutePath);
        pathList.Add(Android.OS.Environment.GetExternalStoragePublicDirectory(string.Empty).AbsolutePath);
#endif

        foreach (var item in pathList)
        {
            if (StorageHelper.IsFolderAccesible(item))
            {
                //var subPathList = StorageHelper.GetFoldersInFolder(item);
                //foreach (var item2 in subPathList)
                //{
                //    if (StorageHelper.IsFolderAccesible(item2.FullPath))
                //        return item2.FullPath;
                //}
                return item;
            }
        }
        return null;
    }

}

