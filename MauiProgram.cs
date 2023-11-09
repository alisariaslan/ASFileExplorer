using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Maui;

#if DEBUG
    using DotNet.Meteor.HotReload.Plugin;
#endif

namespace ASFileExplorer
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
#if DEBUG
                .EnableHotReload()
#endif
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Poppins-Bold.ttf", "PoppinsBold");
                    fonts.AddFont("Poppins-Regular.ttf", "PoppinsRegular");
                    fonts.AddFont("Poppins-SemiBold.ttf", "PoppinsSemiBold");
                    fonts.AddFont("Poppins-Medium.ttf", "PoppinsMedium");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<IMessenger, WeakReferenceMessenger>();

            builder.Services.AddSingleton<LoadingService>();

            builder.Services.AddSingleton<MainPage>();

            builder.Services.AddTransient<SharedView>();

            return builder.Build();
        }
    }
}