using System.Windows.Forms;

namespace GuoXiAutoSign.Handlers
{
    public class FormInputHandler
    {
        public static bool ValidateInputs(string stdID, string name, string seatNO, Form form)
        {
            if (!(string.IsNullOrEmpty(stdID) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(seatNO)))
            {
                return true;
            }

            form.Invoke((MethodInvoker)delegate
            {
                MessageBox.Show("上方欄位不可有空！");
            });
            return false;
        }
    }
}
