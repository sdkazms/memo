using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsTestChangeFormApp
{
    public partial class FormA : Form
    {
        public FormA()
        {
            InitializeComponent();

            AppLogger.Info("FormA コンストラクタが呼ばれました");

            // --- イベント購読 ---
            ScreenManagerForm.Instance.ReceivedEvent += OnReceivedEvent;
        }

        private void OnReceivedEvent(object sender, ReceivedEventArgs e)
        {
            //// 念のためInvoke対応
            //if (InvokeRequired)
            //{
            //    BeginInvoke(new Action(() => OnReceivedEvent(sender, e)));
            //    return;
            //}

            AppLogger.Info("FormA OnReceivedEvent受信: " + e.Message);
            MessageBox.Show("FormA OnReceivedEvent を受信");
        }

        private void FormA_Load(object sender, EventArgs e)
        {
            AppLogger.Info("FormA_Load が呼ばれました");
        }

        private void FormA_Shown(object sender, EventArgs e)
        {
            AppLogger.Info("FormA_Shown が呼ばれました");

            Thread.Sleep(5000);

            AppLogger.Info("FormB への遷移を開始します");
            ScreenManagerForm.Instance.NavigateTo<FormB>();
        }

        private void FormA_FormClosed(object sender, FormClosedEventArgs e)
        {
            AppLogger.Info("FormA_FormClosed が呼ばれました");

            ScreenManagerForm.Instance.ReceivedEvent -= OnReceivedEvent;
        }
    }
}
