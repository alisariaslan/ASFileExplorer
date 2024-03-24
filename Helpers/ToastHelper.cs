using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Alerts;

namespace MobilGarsonNative.Others;

public class ToastHelper
{
    public static async void MakeToastFast(string message)
    {
        ToastDuration duration = ToastDuration.Short;
        string text = message;
        var toast = Toast.Make(text, duration, 16);
        await toast.Show(CancellationToken.None);
    }

}

