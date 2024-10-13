using System.Threading;
using System.Threading.Tasks;

namespace GuoXiAutoSign.Services
{
    internal interface ISignService
    {
        Task StartAsync(string stuID, string name, string seatNO, CancellationToken cancellationToken);
    }
}
