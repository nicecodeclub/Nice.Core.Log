namespace Nice.Core.Log
{
    public interface ILog
    {
        string Content { get; set; }
        string GetFullname(string sign);
    }
}
