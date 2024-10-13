using GuoXiAutoSign.Models;
using System;
using System.Net.Http;
using System.Net.Sockets;

namespace GuoXiAutoSign.Handlers
{
    public class HttpErrorHandler
    {
        public static ResponseStatus.Code HandleException(Exception ex)
        {
            if (ex is HttpRequestException)
                return ResponseStatus.Code.PendingConnection;

            if (ex is SocketException socketEx && socketEx.Message.Contains("not properly respond after a period of time"))
                return ResponseStatus.Code.PendingConnection;

            return ResponseStatus.Code.Failed;
        }
    }
}
