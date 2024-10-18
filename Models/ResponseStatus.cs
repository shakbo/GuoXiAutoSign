namespace GuoXiAutoSign.Models
{
    public class ResponseStatus
    {
        public enum Code : ushort
        {
            PendingConnection = 0,
            Success = 1,
            Checked = 2,
            Failed = 3,
            Unexpected = 4,
        }
    }
}
