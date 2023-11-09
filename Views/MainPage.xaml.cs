using CommunityToolkit.Mvvm.Messaging;
#if ANDROID
using Android.OS;
#endif

namespace ASFileExplorer;

public partial class MainPage : ContentPage
{
	private List<PermModel> permList;
    private bool initialized;

    private IMessenger messenger;
	private IServiceProvider serviceProvider;

	public MainPage(IServiceProvider serviceProvider, IMessenger messenger)
	{
		InitializeComponent();
		this.serviceProvider = serviceProvider;
		this.messenger = messenger;
		permList = new List<PermModel>();
		messenger.Register<MessageData>(this, (recipient, message) =>
		{
			switch (message.typeOfMessage)
			{
				case MessageType.PERM_IS_CHECKED:
					var result = (PermModel)message.data;
					for (int i = 0; i < permList.Count; i++)
					{
						if (permList[i].Perm.Equals(result.Perm))
							permList[i].Permitted = result.Permitted;
					}
					break;
			}
		});
	}

	async void ContentPage_Loaded(object sender, EventArgs e)
    {
        if (initialized)
            return;
        initialized = true;

        var vm = this.BindingContext as MainViewModel;
		vm.serviceProvider = this.serviceProvider;
        vm.ScrollTo = new Command<int>(ScrollTo);
#if ANDROID
        await vm.WaitPermissions(messenger,permList);
#endif
		vm.OnAppear();

	}

    private void ScrollTo(int n)
    {
        if (n < 0)
            return;
        tabs_collection.ScrollTo(n,0,ScrollToPosition.End,true);
    }

}