using GuoXiAutoSign.Handlers;
using GuoXiAutoSign.Models;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GuoXiAutoSign.Services
{
    internal class HttpRequestService : IHttpRequestService
    {
        private readonly FormMain _formMain;
        private readonly HttpClient _httpClient;

        public HttpRequestService(FormMain formMain)
        {
            _formMain = formMain;
            _httpClient = new HttpClient();
        }

        public async Task<ResponseStatus.Code> SendAsync(string stuID, string name, string seatNO)
        {
            string formData = $"stuID={WebUtility.UrlEncode(stuID)}&name={WebUtility.UrlEncode(name)}&seatNO={WebUtility.UrlEncode(seatNO)}";
            string url = "http://192.168.0.171/att/process.php";

            try
            {
                var response = await _httpClient.PostAsync(url, new StringContent(formData, Encoding.UTF8, "application/x-www-form-urlencoded"));
                var responseString = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<RequestResponse>(responseString);

                if (!responseObject.Success)
                {
                    _formMain.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show("請求成功送出，但發生預期外的錯誤：" + responseObject.Message);
                    });
                    return ResponseStatus.Code.Unexpected;
                }

                if (responseObject.Message.Contains("You've checked!"))
                {
                    return ResponseStatus.Code.Checked;
                }

                _formMain.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show("該節課簽到成功！");
                });
                return ResponseStatus.Code.Success;
            }
            catch (Exception ex)
            {
                return HttpErrorHandler.HandleException(ex);
            }
        }
    }
}
