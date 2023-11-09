using System.ComponentModel;

namespace ASFileExplorer;

public class BaseViewModel : INotifyPropertyChanged
{
    private bool IsLoading_ { get; set; }
    public bool IsLoading { get { return IsLoading_; } set { IsLoading_ = value; OnPropertyChanged(nameof(IsLoading)); } }

    private double LoadProgress_ { get; set; }
    public double LoadProgress { get { return LoadProgress_; } set { LoadProgress_ = value; OnPropertyChanged(nameof(LoadProgress)); } }

    private string LoadDesc_ { get; set; }
    public string LoadDesc { get { return LoadDesc_; } set { LoadDesc_ = value; OnPropertyChanged(nameof(LoadDesc)); } }

    public event PropertyChangedEventHandler? PropertyChanged;

    public BaseViewModel()
    {
        LoadDesc = "Initializing...";
        //StartLoad(null);
    }

    public virtual void StartLoad(string desc)
    {
        if (string.IsNullOrEmpty(desc) is false)
            LoadDesc = desc;
        LoadProgress = 1;
        IsLoading = true;
    }

    public virtual void StopLoad()
    {
        LoadProgress = 99;
        Task.Run(async () => { await Task.Delay(100); IsLoading = false; });
    }

    public virtual void OnAppear()
    {
        throw new NotImplementedException();
    }

    public void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
