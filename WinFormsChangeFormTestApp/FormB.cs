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
    public partial class FormB : Form
    {
        public FormB()
        {
            InitializeComponent();

            AppLogger.Info("FormB コンストラクタが呼ばれました");

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

            AppLogger.Info("FormB OnReceivedEvent受信: " + e.Message);
            MessageBox.Show("FormB OnReceivedEvent を受信");
        }

        private void FormB_Load(object sender, EventArgs e)
        {
            AppLogger.Info("FormB_Load が呼ばれました");
        }

        private void FormB_Shown(object sender, EventArgs e)
        {
            AppLogger.Info("FormB_Shown が呼ばれました");

            //Thread.Sleep(5000);

            AppLogger.Info("FormC への遷移を開始します");
            ScreenManagerForm.Instance.NavigateTo<FormC>();
        }

        private void FormB_FormClosed(object sender, FormClosedEventArgs e)
        {
            AppLogger.Info("FormB_FormClosed が呼ばれました");

            ScreenManagerForm.Instance.ReceivedEvent -= OnReceivedEvent;
        }

        private void btnB_Click(object sender, EventArgs e)
        {

        }
    }
}
