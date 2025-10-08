using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsTestChangeFormApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            AppLogger.Info("MainForm コンストラクタが呼ばれました");

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

            AppLogger.Info("MainForm OnReceivedEvent受信: " + e.Message);
            MessageBox.Show("MainForm OnReceivedEvent を受信");
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            AppLogger.Info("FormA への遷移を開始します");
            ScreenManagerForm.Instance.NavigateTo<FormA>();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            AppLogger.Info("MainForm_Load が呼ばれました");
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            AppLogger.Info("MainForm_Shown が呼ばれました");
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            AppLogger.Info("MainForm_FormClosed が呼ばれました");

            ScreenManagerForm.Instance.ReceivedEvent -= OnReceivedEvent;
        }
    }
}
