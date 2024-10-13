using GuoXiAutoSign.Models;
using System.Threading.Tasks;

namespace GuoXiAutoSign.Services
{
    internal interface IHttpRequestService
    {
        Task<ResponseStatus.Code> SendAsync(string stuID, string name, string seatNO);
    }
}
