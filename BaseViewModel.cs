using System.ComponentModel;

namespace ASFileExplorer;

public class BaseViewModel : INotifyPropertyChanged
{
	public virtual Command NavScrollTo { get; set; }

	public event PropertyChangedEventHandler? PropertyChanged;

	public BaseViewModel() {
		NavScrollTo = new Command(() => throw new NotImplementedException());
	}
	public virtual void OnAppear()
	{
		throw new NotImplementedException();
	}

	public void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
