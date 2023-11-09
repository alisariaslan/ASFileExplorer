using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using CommunityToolkit.Mvvm.Messaging;

namespace ASFileExplorer
{
	[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
	public class MainActivity : MauiAppCompatActivity
	{
		public static string[] ManifestPerms = new string[] {
			Manifest.Permission.ReadExternalStorage,
			Manifest.Permission.WriteExternalStorage,
			Manifest.Permission.ManageExternalStorage,
			Manifest.Permission.ReadMediaAudio,
			Manifest.Permission.ReadMediaImages,
			Manifest.Permission.ReadMediaVideo,
			Manifest.Permission.RequestInstallPackages,
			Manifest.Permission.StartViewPermissionUsage
		};

		public MainActivity()
		{
			var messenger = MauiApplication.Current?.Services?.GetService<IMessenger>();
			messenger?.Register<MessageData>(this, (recipient, message) =>
			{
				switch (message.typeOfMessage)
				{
					case MessageType.REQUEST_ALL_PERMS:
						RequestAllPermissions();
						break;
					case MessageType.CHECK_ONE_PERM:
						var result = CheckSpecificPerm((PermModel)message.data);
						messenger?.Send(new MessageData(MessageType.PERM_IS_CHECKED, result));
						break;
				}
			});
		}

		private PermModel CheckSpecificPerm(PermModel model)
		{
			if (ContextCompat.CheckSelfPermission(this, model.Key) is Permission.Granted)
				model.Permitted = true;

			if (model.Perm == PermType.MANAGE_EXTERNAL_STORAGE && model.Permitted is false)
			{
				model.Permitted = Android.OS.Environment.IsExternalStorageManager;
				if (model.Permitted is true)
					return model;
					
				Intent intent = new Intent();
				intent.SetAction(Android.Provider.Settings.ActionManageAllFilesAccessPermission);
				StartActivity(intent);
			}

			return model;
		}

		private void RequestAllPermissions()
		{
			ActivityCompat.RequestPermissions(this, ManifestPerms, 0);
		}

	}


}