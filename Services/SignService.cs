using GuoXiAutoSign.Handlers;
using GuoXiAutoSign.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GuoXiAutoSign.Services
{
    internal class SignService : ISignService
    {
        private readonly FormMain _formMain;
        private readonly Label _labelStatus;
        private readonly IHttpRequestService _requestService;

        public SignService(FormMain formMain, Label labelStatus, IHttpRequestService requestService)
        {
            _formMain = formMain;
            _labelStatus = labelStatus;
            _requestService = requestService;
        }

        public async Task StartAsync(string stuID, string name, string seatNO, CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var response = await _requestService.SendAsync(stuID, name, seatNO);
                    StatusHandler.UpdateLabelStatus(response, _formMain, _labelStatus);

                    if (response == ResponseStatus.Code.Success)
                    {
                        var nextTargetTime = TimeService.GetNextTarget();
                        var delay = nextTargetTime - DateTime.Now;

                        if (delay > TimeSpan.Zero)
                        {
                            await Task.Delay(delay, cancellationToken);
                        }
                    }
                    else if (response == ResponseStatus.Code.PendingConnection)
                    {
                        while (response == ResponseStatus.Code.PendingConnection && !cancellationToken.IsCancellationRequested)
                        {
                            response = await _requestService.SendAsync(stuID, name, seatNO);
                            StatusHandler.UpdateLabelStatus(response, _formMain, _labelStatus);

                            await Task.Delay(1000, cancellationToken);
                        }
                    }
                    else
                    {
                        await Task.Delay(5000, cancellationToken);
                    }
                }
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception ex)
            {
                _formMain.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show($"發生意料之外的錯誤：{ex.Message}");
                });
            }
        }
    }
}
