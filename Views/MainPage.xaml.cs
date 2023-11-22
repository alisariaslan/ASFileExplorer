using CommunityToolkit.Mvvm.Messaging;

namespace ASFileExplorer;

public partial class MainPage : ContentPage
{
    private bool initialized;

    private IMessenger messenger;
	private IServiceProvider serviceProvider;

	public MainPage(IServiceProvider serviceProvider, IMessenger messenger)
	{
		InitializeComponent();
		this.serviceProvider = serviceProvider;
		this.messenger = messenger;
	}

	async void ContentPage_Loaded(object sender, EventArgs e)
    {
        if (initialized)
            return;
        initialized = true;

        var vm = this.BindingContext as MainViewModel;
        vm.RegisterMessenger(messenger);
        vm.serviceProvider = this.serviceProvider;
        vm.ScrollTo = new Command<int>(ScrollTo);
#if ANDROID
        await vm.WaitPermissions();
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