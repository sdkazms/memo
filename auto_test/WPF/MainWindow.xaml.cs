using System;
using System.Windows;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoTestWPF
{
    public partial class MainWindow : Window
    {
        private ExeRunner exeRunner;
        private InputSimulator inputSimulator;
        private XMLHandler xmlHandler;
        private MultiMonitorInputSimulator multiMonitorInputSimulator;

        public MainWindow()
        {
            InitializeComponent();
            exeRunner = new ExeRunner();
            inputSimulator = new InputSimulator();
            xmlHandler = new XMLHandler();
            multiMonitorInputSimulator = new MultiMonitorInputSimulator();
        }

        private async void RunExe_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = @"C:\Path\To\Your\Folder";
            string exeName = "YourExe.exe";
            await Task.Run(() => exeRunner.RunExe(folderPath, exeName));
        }

        private void SimulateTouch_Click(object sender, RoutedEventArgs e)
        {
            inputSimulator.SimulateTouch(100, 100);
        }

        private void SimulateDoubleTouch_Click(object sender, RoutedEventArgs e)
        {
            inputSimulator.SimulateDoubleTouch(200, 200);
        }

        private async void ShowMousePosition_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => MousePositionTracker.ShowRealTimePosition(Screen.PrimaryScreen.Bounds));
        }

        private async void ReadXML_Click(object sender, RoutedEventArgs e)
        {
            string xmlPath = @"C:\Path\To\Your\File.xml";
            var result = await Task.Run(() => xmlHandler.ReadXML(xmlPath));
            System.Windows.MessageBox.Show(string.Join("\n", result));
        }

        private async void WriteXML_Click(object sender, RoutedEventArgs e)
        {
            string xmlPath = @"C:\Path\To\Your\File.xml";
            var updates = new System.Collections.Generic.Dictionary<string, string>
            {
                { "SomeElement", "新しい値" }
            };
            await Task.Run(() => xmlHandler.WriteXML(xmlPath, updates));
        }

        private async void MultiMonitorTest_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                var tester = new MultiMonitorInputTester(multiMonitorInputSimulator);
                tester.TestMultiMonitorClicks();
            });
        }
    }


    public class ExeRunner
    {
        public void RunExe(string folderPath, string exeName)
        {
            string fullPath = System.IO.Path.Combine(folderPath, exeName);
            if (System.IO.File.Exists(fullPath))
            {
                try
                {
                    Process.Start(fullPath);
                    System.Threading.Thread.Sleep(5000); // 5秒待機
                    System.Windows.MessageBox.Show($"{exeName}が起動しました。");
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"エラー: {ex.Message}");
                }
            }
            else
            {
                System.Windows.MessageBox.Show($"エラー: {fullPath}が見つかりません。");
            }
        }
    }

    public class InputSimulator
    {
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;

        public void SimulateTouch(int x, int y)
        {
            SetCursorPos(x, y);
            mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
            System.Threading.Thread.Sleep(100);
            mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, 0);
            System.Windows.MessageBox.Show($"タッチシミュレーション: ({x}, {y})");
        }

        public void SimulateDoubleTouch(int x, int y)
        {
            SimulateTouch(x, y);
            System.Threading.Thread.Sleep(100);
            SimulateTouch(x, y);
            System.Windows.MessageBox.Show($"ダブルタッチシミュレーション: ({x}, {y})");
        }
    }

    public class XMLHandler
    {
        public System.Collections.Generic.List<Tuple<string, string>> ReadXML(string filePath)
        {
            var result = new System.Collections.Generic.List<Tuple<string, string>>();
            XDocument doc = XDocument.Load(filePath);
            foreach (var element in doc.Root.Elements())
            {
                result.Add(new Tuple<string, string>(element.Name.LocalName, element.Value));
            }
            return result;
        }

        public void WriteXML(string filePath, System.Collections.Generic.Dictionary<string, string> updates)
        {
            XDocument doc = XDocument.Load(filePath);
            foreach (var update in updates)
            {
                var element = doc.Root.Element(update.Key);
                if (element != null)
                {
                    element.Value = update.Value;
                }
            }
            doc.Save(filePath);
        }
    }

    public class MultiMonitorInputSimulator : InputSimulator
    {
        public void ClickOnMonitor(Screen monitor, int x, int y)
        {
            int absoluteX = monitor.Bounds.Left + x;
            int absoluteY = monitor.Bounds.Top + y;
            SimulateTouch(absoluteX, absoluteY);
        }
    }

    public static class MousePositionTracker
    {
        public static void ShowRealTimePosition(System.Drawing.Rectangle monitorBounds)
        {
            Console.WriteLine("マウス位置をリアルタイムで表示します。終了するには何かキーを押してください。");
            while (!Console.KeyAvailable)
            {
                System.Drawing.Point cursorPosition = System.Windows.Forms.Cursor.Position;
                int relativeX = cursorPosition.X - monitorBounds.Left;
                int relativeY = cursorPosition.Y - monitorBounds.Top;
                double xFraction = (double)relativeX / monitorBounds.Width;
                double yFraction = (double)relativeY / monitorBounds.Height;
                Console.Write($"\r現在のマウス位置: 幅の{xFraction:F3}, 高さの{yFraction:F3}");
                System.Threading.Thread.Sleep(100);
            }
        }
    }

    public class MultiMonitorInputTester
    {
        private MultiMonitorInputSimulator simulator;

        public MultiMonitorInputTester(MultiMonitorInputSimulator simulator)
        {
            this.simulator = simulator;
        }

        public void TestMultiMonitorClicks()
        {
            Screen[] screens = Screen.AllScreens;
            if (screens.Length < 2)
            {
                System.Windows.MessageBox.Show("複数のモニターが検出されませんでした。");
                return;
            }

            Screen mainMonitor = Screen.PrimaryScreen;
            Screen subMonitor = screens[1];

            System.Windows.MessageBox.Show("5秒後にクリック操作を開始します...");
            System.Threading.Thread.Sleep(5000);

            simulator.ClickOnMonitor(mainMonitor, mainMonitor.Bounds.Width / 2, mainMonitor.Bounds.Height / 2);
            System.Threading.Thread.Sleep(2000);
            simulator.ClickOnMonitor(subMonitor, subMonitor.Bounds.Width / 2, subMonitor.Bounds.Height / 2);
        }
    }
}
