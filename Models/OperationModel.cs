namespace ASFileExplorer;

public class OperationModel
{
    public int operationId;
    private CancellationTokenSource cancellationTokenSource;
    public CancellationToken cancellationToken;

    public OperationModel(int operationID)
    {
        cancellationTokenSource = new CancellationTokenSource();
        cancellationToken = cancellationTokenSource.Token;
        this.operationId = operationID;
    }

    public void Cancel()
    {
        cancellationTokenSource.Cancel();
    }
}

