namespace Flow.Launcher.Plugin.SimpleContainer.Common;

public class ResultResponse<T>
{
    public bool Success { get; set; }
    public T Result { get; set; }
    public string Message { get; set; }

    public override string ToString()
    {
        return $"ResultResponse: Success={Success}, Result={Result}, Message={Message}";
    }
}