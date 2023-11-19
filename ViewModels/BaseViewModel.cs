﻿namespace ASFileExplorer;

public class BaseViewModel : PropertyNotifier, TabInterface
{
    public List<OperationModel> LocalOperations;
    public TabModel MyTab;

    private bool IsLoading_ { get; set; }
    public bool IsLoading { get { return IsLoading_; } set { IsLoading_ = value; OnPropertyChanged(nameof(IsLoading)); OnPropertyChanged(nameof(IsNotLoading)); } }
    public bool IsNotLoading { get { return !IsLoading_; } }

    //private double LoadProgress_ { get; set; }
    //public double LoadProgress { get { return LoadProgress_; } set { LoadProgress_ = value; OnPropertyChanged(nameof(LoadProgress)); } }

    private string LoadDesc_ { get; set; }
    public string LoadDesc { get { return LoadDesc_; } set { LoadDesc_ = value; OnPropertyChanged(nameof(LoadDesc)); } }

    private long LoadCount_ { get; set; }
    public long LoadCount { get { return LoadCount_; } set { LoadCount_ = value; OnPropertyChanged(nameof(LoadCount)); } }

    private long MaxLoadCount_ { get; set; }
    public long MaxLoadCount { get { return MaxLoadCount_; } set { MaxLoadCount_ = value; OnPropertyChanged(nameof(MaxLoadCount)); } }


    public BaseViewModel()
    {
        LocalOperations = new List<OperationModel>();
        //LoadDesc = "Initializing...";
        //StartLoad(null);
    }

    public async Task CancelLastOperation()
    {
        LocalOperations.LastOrDefault()?.Cancel();
        while (MyTab.GetOperationState())
            await Task.Delay(100);
    }

    public void ChangeOperationState(bool state,string desc)
    {
        MyTab.ChangeOperationState(state,desc);
        IsLoading = state;
        LoadDesc = desc;
    }

    public void ChangeOperationState(long loadCount, long maxLoadCount)
    {
        LoadCount = loadCount;
        MaxLoadCount = maxLoadCount;
    }

    public void ChangeTabName(string newName)
    {
        MyTab.ChangeTabName(newName);
    }

    public CancellationToken GetNewOperationKey()
    {
        LocalOperations.Add(new OperationModel(LocalOperations.Count));
        return LocalOperations.Last().cancellationToken;

    }

    public bool GetOperationState()
    {
        return MyTab.GetOperationState();
    }

    //public virtual void StartLoad(string desc)
    //{
    //    if (string.IsNullOrEmpty(desc) is false)
    //        LoadDesc = desc;
    //    LoadProgress = 1;
    //    IsLoading = true;
    //}

    //public virtual void StopLoad()
    //{
    //    LoadProgress = 99;
    //    Task.Run(async () => { await Task.Delay(100); IsLoading = false; });
    //}

    public virtual void OnAppear()
    {
        throw new NotImplementedException();
    }

}
