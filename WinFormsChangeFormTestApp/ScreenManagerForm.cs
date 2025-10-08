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
    /// <summary>
    /// 画面管理用フォーム。
    /// アプリ起動時に一度だけ生成しておく想定。
    /// ここに画面遷移の共通メソッドを集約します。
    /// </summary>
    public partial class ScreenManagerForm : Form
    {
        private readonly List<Form> _openForms = new List<Form>();
        public IReadOnlyList<Form> OpenForms => _openForms.AsReadOnly();
        public Form CurrentForm { get; private set; }

        // --- イベント定義（全体通知） ---
        public event EventHandler<ReceivedEventArgs> ReceivedEvent;

        // --- ★アプリ全体で共有できるインスタンスを保持 ---
        private static ScreenManagerForm _instance;
        public static ScreenManagerForm Instance
        {
            get
            {
                // まだ作られていない場合、自動生成も可（任意）
                if (_instance == null)
                {
                    _instance = new ScreenManagerForm();
                }
                return _instance;
            }
        }

        private ScreenManagerForm()
        {
            // 画面管理フォーム自体は表示してもしなくてもOK
            // 必要であれば最小化や不可視にしておく

            InitializeComponent();
            //this.ShowInTaskbar = false;
            //this.WindowState = FormWindowState.Minimized;
        }

        /// <summary>
        /// 任意スレッドからメッセージをRaiseできる（UIスレッドへInvoke）
        /// </summary>
        public void RaiseMessage(object sender, string message)
        {
            if (InvokeRequired)
            {
                // UIスレッドにマーシャリング
                BeginInvoke(new Action<object, string>(RaiseMessage), sender, message);
            }
            else
            {
                if (ReceivedEvent != null)
                {
                    var args = new ReceivedEventArgs(message);
                    ReceivedEvent(sender, args);
                }
            }
        }

        /// <summary>
        /// 指定したフォーム型（T）へ遷移。
        /// 既に開いていればそれを前面化、無ければ新規生成。
        /// </summary>
        public void NavigateTo<T>() where T : Form, new()
        {
            NavigateTo(typeof(T));
        }

        /// <summary>
        /// 実行時に型を決めたい場合はこちら。
        /// </summary>
        public void NavigateTo(Type formType)
        {
            if (formType == null) throw new ArgumentNullException(nameof(formType));
            if (!typeof(Form).IsAssignableFrom(formType))
                throw new ArgumentException("formType は Form を継承している必要があります。", nameof(formType));

            // UIスレッド以外から呼ばれた場合に備える
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<Type>(NavigateTo), formType);
                return;
            }

            //// 既に同型のフォームが開いていれば再利用、無ければ生成
            //var target = Application.OpenForms.Cast<Form>()
            //    .FirstOrDefault(f => f.GetType() == formType);
            Form target = null;

            //if (target == null)
            {
                target = (Form)Activator.CreateInstance(formType);
                // 表示位置など必要に応じて調整
                target.StartPosition = FormStartPosition.CenterScreen;
                // 全面に表示したい（最大化したい）場合はコメントアウトを外す
                // target.WindowState = FormWindowState.Maximized;

                // 管理フォームをオーナーにしておくと一体管理しやすい
                _openForms.Add(target); // add
                target.Show(this);
            }
            //else
            //{
            //    if (target.WindowState == FormWindowState.Minimized)
            //        target.WindowState = FormWindowState.Normal;
            //    target.Show(); // 非表示状態なら再表示
            //}

            // 画面管理フォームとターゲット以外はすべて閉じる
            //var toClose = Application.OpenForms.Cast<Form>()
            //    .Where(f => f != this && f != target)
            //    .ToList(); // 列挙中に Close するので ToList()

            var toClose = _openForms.Where(f => f != target && !f.IsDisposed).ToList();
            foreach (var f in toClose)
            {
                try
                {
                    _openForms.Remove(f);
                    f.Close();
                }
                catch { /* 必要に応じてログ */ }
            }

            // 最前面＆アクティブ化
            target.BringToFront();
            target.Activate();
        }

        /// <summary>
        /// コンストラクタ引数が必要なフォームに遷移したい場合のファクトリ版。
        /// 例: NavigateTo(() => new DetailForm(id));
        /// </summary>
        public void NavigateTo<T>(Func<T> factory) where T : Form
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            if (this.InvokeRequired)
            {
                this.Invoke(new Action<Func<T>>(NavigateTo), factory);
                return;
            }

            var target = Application.OpenForms.Cast<Form>()
                .FirstOrDefault(f => f is T) as T;

            if (target == null)
            {
                target = factory();
                target.StartPosition = FormStartPosition.CenterScreen;
                // target.WindowState = FormWindowState.Maximized;
                target.Show(this);
            }
            else
            {
                if (target.WindowState == FormWindowState.Minimized)
                    target.WindowState = FormWindowState.Normal;
                target.Show();
            }

            var toClose = Application.OpenForms.Cast<Form>()
                .Where(f => f != this && f != target)
                .ToList();

            foreach (var f in toClose)
            {
                try { f.Close(); } catch { }
            }

            target.BringToFront();
            target.Activate();
        }

        private int num = 0;
        private void btnTaskStart_Click(object sender, EventArgs e)
        {
            num++;
            Task workerTask1 = Task.Run(() =>
            {
                Thread.Sleep(3000);
                ScreenManagerForm.Instance.RaiseMessage("Taskスレッド", $"RaiseMessage通知 #{num}");
            });

            num++;
            Task workerTask2 = Task.Run(() =>
            {
                Thread.Sleep(3500);
                ScreenManagerForm.Instance.RaiseMessage("Taskスレッド", $"RaiseMessage通知 #{num}");
            });

            //// --- 別スレッドで通知送信 ---
            //Task.Run(async () =>
            //{
            //    for (int i = 1; i <= 5; i++)
            //    {
            //        ScreenManagerForm.Instance.RaiseMessage("Taskスレッド", $"通知 #{i}");
            //        await Task.Delay(1000);
            //    }

            //    new Thread(() =>
            //    {
            //        ScreenManagerForm.Instance.RaiseMessage(Thread.CurrentThread, "Threadクラスから最終通知");
            //    }).Start();
            //});

        }
    }
}
