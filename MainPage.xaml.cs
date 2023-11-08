using CommunityToolkit.Mvvm.Messaging;

namespace ASFileExplorer;

public partial class MainPage : ContentPage
{
	private List<PermModel> Permissions;

	private IMessenger messenger;

	public MainPage(IMessenger messenger)
	{
		InitializeComponent();
		this.messenger = messenger;
		Permissions = new List<PermModel>();
		messenger.Register<MessageData>(this, (recipient, message) =>
		{
			switch (message.typeOfMessage)
			{
				case MessageType.PERM_IS_CHECKED:
					var result = (PermModel)message.data;
					for (int i = 0; i < Permissions.Count; i++)
					{
						if (Permissions[i].Perm.Equals(result.Perm))
							Permissions[i].Permitted = result.Permitted;
					}
					break;
			}
		});
	}

	private async void ContentPage_Loaded(object sender, EventArgs e)
	{
		var vm = this.BindingContext as BaseViewModel;
		vm.NavScrollTo = new Command<int>(NavScrollTo);
#if ANDROID
		await WaitPermissions();
#endif
		vm.OnAppear();
	}

	private void NavScrollTo(int n)
	{
		if (n < 0)
			return;
		cv_nav.ScrollTo(n);
	}

	private async Task WaitPermissions()
	{
		messenger?.Send(new MessageData(MessageType.REQUEST_ALL_PERMS, this));
#if ANDROID33_0_OR_GREATER
		Permissions.Add(new PermModel(PermType.READ_MEDIA_IMAGES));
		Permissions.Add(new PermModel(PermType.READ_MEDIA_VIDEO));
		Permissions.Add(new PermModel(PermType.READ_MEDIA_AUDIO));
#endif
#if ANDROID30_0_OR_GREATER
		Permissions.Add(new PermModel(PermType.MANAGE_EXTERNAL_STORAGE));
#else
		Permissions.Add(new PermModel(PermType.READ_EXTERNAL_STORAGE));
		Permissions.Add(new PermModel(PermType.WRITE_EXTERNAL_STORAGE));
#endif

		while (Permissions.Any(p => p.Permitted is false))
		{
			foreach (var item in Permissions)
				messenger?.Send(new MessageData(MessageType.CHECK_ONE_PERM, item));
			await Task.Delay(1000);
			var perm = Permissions.FirstOrDefault(p => p.Permitted is false);
			bool answer = await DisplayAlert("Warning", $"{perm.Key} is not permitted. This app needs all necessary permissions to work properly.", "Retry", "Open App Settings",FlowDirection.LeftToRight);
			if (answer is false)
				AppInfo.ShowSettingsUI();
		}
	}
}