namespace ASFileExplorer;


public class CustomDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate DesktopTemplate { get; set; }
    public DataTemplate MobileTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (DeviceInfo.Platform == DevicePlatform.Android)
            return MobileTemplate;
        if (DeviceInfo.Platform == DevicePlatform.iOS)
            return MobileTemplate;
        if (DeviceInfo.Platform == DevicePlatform.WinUI)
            return DesktopTemplate;
        if (DeviceInfo.Platform == DevicePlatform.macOS)
            return DesktopTemplate;
        if (DeviceInfo.Platform == DevicePlatform.MacCatalyst)
            return DesktopTemplate;
        else
            return MobileTemplate;

    }
}


