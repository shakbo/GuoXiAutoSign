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
        private bool _toggle;
        private string _stuID, _name, _seatNO;
        private IHttpRequestService _requestService;
        private ISignService _signService;
        private NotifyIcon _notifyIcon;
        private ContextMenu _contextMenu;

        private static string urlOwnerProfile = "https://github.com/shakbo/";
        private static string urlRepository = "https://github.com/shakbo/GuoXiAutoSign/";
        private static string urlCC = "https://creativecommons.org/licenses/by-nc-sa/4.0/";

        public FormMain()
        {
            InitializeComponent();
            InitializeServices();
            InitializeNotifyIcon();
            _toggle = false;
        }

        private void InitializeServices()
        {
            _requestService = new HttpRequestService(this);
            _signService = new SignService(this, labelStatus, _requestService);
        }

        private void InitializeNotifyIcon()
        {
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

        private void buttonToggle_Click(object sender, EventArgs e)
        {
            if (!_toggle)
            {
                SetUserData();
                if (!FormInputHandler.ValidateInputs(_stuID, _name, _seatNO, this))
                    return;
                StartSigningProcess();
            }
            else
            {
                StopSigningProcess();
            }
        }

        private void SetUserData()
        {
            _stuID = textBoxStuID.Text.ToUpper();
            _name = textBoxName.Text.ToUpper();
            _seatNO = textBoxSeatNO.Text.ToUpper();
        }

        private async void StartSigningProcess()
        {
            _toggle = true;
            textBoxStuID.Enabled = false;
            textBoxName.Enabled = false;
            textBoxSeatNO.Enabled = false;
            buttonToggle.Text = "停止";
            labelStatus.Text = "檢查簽到伺服器狀態";
            _cancellationTokenSource = new CancellationTokenSource();
            _notifyIcon.Text = "執行中";
            await _signService.StartAsync(_stuID, _name, _seatNO, _cancellationTokenSource.Token);
        }

        private void StopSigningProcess()
        {
            _toggle = false;
            textBoxStuID.Enabled = true;
            textBoxName.Enabled = true;
            textBoxSeatNO.Enabled = true;
            buttonToggle.Text = "開始";
            labelStatus.Text = "等待使用者操作";
            _notifyIcon.Text = "等待使用者操作";
            _cancellationTokenSource.Cancel();
        }

        private void OpenLink(string url)
        {
            try
            {
                System.Diagnostics.Process.Start(url);
            }
            catch (Exception)
            {
                MessageBox.Show("無法開啟該連結！");
            }
        }

        private void linkLabelSourceCode_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) =>
            OpenLink(urlRepository);

        private void linkLabelCopyrightSourceCode_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) =>
            OpenLink(urlRepository);

        private void linkLabelCopyrightOwner_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) =>
            OpenLink(urlOwnerProfile);

        private void linkLabelCopyrightCC_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) =>
            OpenLink(urlCC);

        private void pictureBoxCC_Click(object sender, EventArgs e) =>
            OpenLink(urlCC);

        private void pictureBoxBY_Click(object sender, EventArgs e) =>
            OpenLink(urlCC);

        private void pictureBoxNC_Click(object sender, EventArgs e) =>
            OpenLink(urlCC);

        private void pictureBoxSA_Click(object sender, EventArgs e) =>
            OpenLink(urlCC);

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
