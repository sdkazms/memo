using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace WindowsFormsTestChangeFormApp
{
    /// <summary>
    /// シンプルなアプリ共通ログクラス
    /// </summary>
    public static class AppLogger
    {
        // ロックオブジェクト（スレッドセーフ用）
        private static readonly object _lockObj = new object();

        // ログディレクトリ（アプリの実行フォルダ直下に "logs"）
        private static readonly string _logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

        /// <summary>
        /// 通常ログを出力
        /// </summary>
        public static void Info(string message)
        {
            WriteLog("INFO", message);
        }

        /// <summary>
        /// 警告ログを出力
        /// </summary>
        public static void Warn(string message)
        {
            WriteLog("WARN", message);
        }

        /// <summary>
        /// エラーログを出力
        /// </summary>
        public static void Error(string message)
        {
            WriteLog("ERROR", message);
        }

        /// <summary>
        /// 例外を含めたエラーログを出力
        /// </summary>
        public static void Error(Exception ex, string message = null)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(message))
                sb.AppendLine(message);

            sb.AppendLine($"Exception: {ex.GetType().FullName}");
            sb.AppendLine($"Message  : {ex.Message}");
            sb.AppendLine($"StackTrace:");
            sb.AppendLine(ex.StackTrace);

            WriteLog("ERROR", sb.ToString());
        }

        /// <summary>
        /// 実際の書き込み処理（共通）
        /// </summary>
        private static void WriteLog(string level, string message)
        {
            try
            {
                lock (_lockObj)
                {
                    // ディレクトリがなければ作成
                    if (!Directory.Exists(_logDir))
                    {
                        Directory.CreateDirectory(_logDir);
                    }

                    // 日付ごとにファイル分割
                    string logFile = Path.Combine(_logDir, $"log_{DateTime.Now:yyyy-MM-dd}.txt");

                    string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level}] {message}{Environment.NewLine}";

                    File.AppendAllText(logFile, line, Encoding.UTF8);
                }
            }
            catch
            {
                // ログ出力に失敗してもアプリを止めない
            }
        }
    }
}
