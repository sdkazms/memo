using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsTestChangeFormApp
{
    public partial class FormC : Form
    {
        public FormC()
        {
            InitializeComponent();

            AppLogger.Info("FormC コンストラクタが呼ばれました");

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

            AppLogger.Info("FormC OnReceivedEvent受信: " + e.Message);
            MessageBox.Show("FormC OnReceivedEvent を受信");
        }

        private void FormC_Load(object sender, EventArgs e)
        {
            AppLogger.Info("FormC_Load が呼ばれました");
        }

        private void FormC_Shown(object sender, EventArgs e)
        {
            AppLogger.Info("FormC_Shown が呼ばれました");
        }

        private void FormC_FormClosed(object sender, FormClosedEventArgs e)
        {
            AppLogger.Info("FormC_FormClosed が呼ばれました");

            ScreenManagerForm.Instance.ReceivedEvent -= OnReceivedEvent;
        }

        private void btnC_Click(object sender, EventArgs e)
        {

        }
    }
}
