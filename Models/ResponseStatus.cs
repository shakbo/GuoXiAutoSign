namespace GuoXiAutoSign.Models
{
    public class ResponseStatus
    {
        public enum Code : ushort
        {
            PendingConnection = 0,
            Success = 1,
            Failed = 2,
            Unexpected = 3,
        }
    }
}
