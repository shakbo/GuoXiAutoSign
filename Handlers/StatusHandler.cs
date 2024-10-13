using GuoXiAutoSign.Models;
using System.Windows.Forms;

namespace GuoXiAutoSign.Handlers
{
    public class StatusHandler
    {
        public static void UpdateLabelStatus(ResponseStatus.Code response, Form form, Label labelStatus)
        {
            form.Invoke((MethodInvoker)delegate
            {
                switch (response)
                {
                    case ResponseStatus.Code.PendingConnection:
                        labelStatus.Text = "等待連線恢復";
                        break;

                    case ResponseStatus.Code.Success:
                        labelStatus.Text = "等待下次點名";
                        break;

                    case ResponseStatus.Code.Failed:
                    case ResponseStatus.Code.Unexpected:
                        labelStatus.Text = "發生錯誤，等待自動重試";
                        break;

                    default:
                        labelStatus.Text = "發生未知錯誤";
                        break;
                }
            });
        }
    }
}
