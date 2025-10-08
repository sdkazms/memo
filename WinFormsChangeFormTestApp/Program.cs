using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsTestChangeFormApp
{
    internal static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Instance プロパティから取得（自動生成される）
            var manager = ScreenManagerForm.Instance;

            // 最初の画面へ遷移
            manager.NavigateTo<MainForm>();

            Application.Run(manager);
        }
    }
}
