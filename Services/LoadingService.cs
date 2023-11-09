using System.ComponentModel;

namespace ASFileExplorer
{
    public class LoadingService : INotifyPropertyChanged
    {
        private bool IsLoading_ { get; set; }
        public bool IsLoading { get { return IsLoading_; } set { IsLoading_ = value; OnPropertyChanged(nameof(IsLoading)); } }


        public event PropertyChangedEventHandler? PropertyChanged;
       
        public void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}

