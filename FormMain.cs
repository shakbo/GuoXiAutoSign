using GuoXiAutoSign.Handlers;
using GuoXiAutoSign.Services;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace GuoXiAutoSign
{
    public partial class FormMain : Form
    {
        private CancellationTokenSource _cancellationTokenSource;
        private bool _toogle;
        private string _stuID, _name, _seatNO;
        private IHttpRequestService _requestService;
        private ISignService _signService;
        private NotifyIcon _notifyIcon;
        private ContextMenu _contextMenu;

        public FormMain()
        {
            InitializeComponent();
            _requestService = new HttpRequestService(this);
            _signService = new SignService(this, labelStatus, _requestService);
            _toogle = false;

            _contextMenu = new ContextMenu();
            _contextMenu.MenuItems.Add("離開", (s, e) => Application.Exit());

            _notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Exclamation,
                Text = "等待使用者操作",
                ContextMenu = _contextMenu,
                Visible = false
            };
            _notifyIcon.MouseClick += NotifyIcon_MouseClick;
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                _notifyIcon.Visible = false;
            }
        }

        private async void buttonToggle_Click(object sender, EventArgs e)
        {
            if (!_toogle)
            {
                _stuID = textBoxStuID.Text.ToUpper();
                _name = textBoxName.Text.ToUpper();
                _seatNO = textBoxSeatNO.Text.ToUpper();
                if (!FormInputHandler.ValidateInputs(_stuID, _name, _seatNO, this))
                {
                    return;
                }
                _toogle = true;
                buttonToggle.Text = "停止";
                labelStatus.Text = "檢查簽到伺服器狀態";
                _cancellationTokenSource = new CancellationTokenSource();
                _notifyIcon.Text = "執行中";
                await _signService.StartAsync(_stuID, _name, _seatNO, _cancellationTokenSource.Token);
            }
            else
            {
                _toogle = false;
                buttonToggle.Text = "開始";
                labelStatus.Text = "等待使用者操作";
                _notifyIcon.Text = "等待使用者操作";
                _cancellationTokenSource.Cancel();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                linkLabel1.LinkVisited = true;
                System.Diagnostics.Process.Start("");
            } catch (Exception ex)
            {
                MessageBox.Show("無法開啟此連結！");
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (MessageBox.Show("確認關閉？", "退出", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }

            base.OnFormClosing(e);
            _cancellationTokenSource?.Cancel();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (this.WindowState == FormWindowState.Minimized)
            {
                _notifyIcon.Visible = true;
                this.Hide();
            }
        }
    }
}
