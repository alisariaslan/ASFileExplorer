namespace ASFileExplorer
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzE2NjI0OUAzMjM1MmUzMDJlMzBFcC9PWUJiQk9zcnRLb2x1Q2UySks2UXJVbXlmTjJBUnNxSTJidzl4a3hVPQ==");

            MainPage = new AppShell();
        }
    }
}