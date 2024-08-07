using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using Npgsql;

namespace PostgreSQLManager
{
    public partial class MainWindow : Window
    {
        private const string Host = "localhost";
        private const string User = "postgres";
        private const string Password = "XXXXXXXXXXX";
        private const string Port = "5432";
        private const string PostgresBinPath = @"C:\PostgreSQL\16\bin";
        private string originalPath;

        public MainWindow()
        {
            InitializeComponent();
            AddPostgresBinToPath();
            RefreshDatabases();
        }

        ~MainWindow()
        {
            RestoreOriginalPath();
        }

        private void AddPostgresBinToPath()
        {
            originalPath = Environment.GetEnvironmentVariable("PATH");
            string newPath = $"{PostgresBinPath};{originalPath}";
            Environment.SetEnvironmentVariable("PATH", newPath);
        }

        private void RestoreOriginalPath()
        {
            Environment.SetEnvironmentVariable("PATH", originalPath);
        }

        private void RefreshDatabases()
        {
            try
            {
                using (var conn = new NpgsqlConnection($"Host={Host};Username={User};Password={Password};Port={Port};Database=postgres"))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT datname FROM pg_database WHERE datistemplate = false ORDER BY LOWER(datname) ASC;", conn))
                    {
                        var databases = new List<string>();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                databases.Add(reader.GetString(0));
                            }
                        }
                        DbComboBox.ItemsSource = databases;
                        if (databases.Any())
                        {
                            DbComboBox.SelectedIndex = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshDatabases();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            string newDbName = NewDbTextBox.Text;
            if (string.IsNullOrWhiteSpace(newDbName))
            {
                MessageBox.Show("Please enter a name for the new database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var conn = new NpgsqlConnection($"Host={Host};Username={User};Password={Password};Port={Port};Database=postgres"))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand($"CREATE DATABASE \"{newDbName}\";", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show($"Database \"{newDbName}\" created successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshDatabases();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            string oldName = DbComboBox.SelectedItem as string;
            string newName = NewNameTextBox.Text;
            if (string.IsNullOrWhiteSpace(oldName) || string.IsNullOrWhiteSpace(newName))
            {
                MessageBox.Show("Please select a database and enter a new name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var conn = new NpgsqlConnection($"Host={Host};Username={User};Password={Password};Port={Port};Database=postgres"))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand($"ALTER DATABASE \"{oldName}\" RENAME TO \"{newName}\";", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show($"Database renamed from \"{oldName}\" to \"{newName}\"", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshDatabases();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackupButton_Click(object sender, RoutedEventArgs e)
        {
            string dbName = DbComboBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(dbName))
            {
                MessageBox.Show("Please select a database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                DefaultExt = ".backup",
                Filter = "Backup files (*.backup)|*.backup|All files (*.*)|*.*"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = "pg_dump",
                        Arguments = $"-h {Host} -U {User} -d {dbName} -Fc -f \"{saveFileDialog.FileName}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };
                    startInfo.EnvironmentVariables["PGPASSWORD"] = Password;

                    using (var process = Process.Start(startInfo))
                    {
                        process.WaitForExit();
                        if (process.ExitCode == 0)
                        {
                            MessageBox.Show($"Database \"{dbName}\" backed up to {saveFileDialog.FileName}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            string error = process.StandardError.ReadToEnd();
                            MessageBox.Show($"Error: {error}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            string dbName = DbComboBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(dbName))
            {
                MessageBox.Show("Please select a target database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var openFileDialog = new OpenFileDialog
            {
                Filter = "Backup files (*.backup)|*.backup|SQL files (*.sql)|*.sql|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string fileName = openFileDialog.FileName;
                    string command = Path.GetExtension(fileName).ToLower() == ".sql" ? "psql" : "pg_restore";
                    string arguments = command == "psql"
                        ? $"-h {Host} -U {User} -d {dbName} -f \"{fileName}\""
                        : $"-h {Host} -U {User} -d {dbName} -v \"{fileName}\"";

                    var startInfo = new ProcessStartInfo
                    {
                        FileName = command,
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };
                    startInfo.EnvironmentVariables["PGPASSWORD"] = Password;

                    using (var process = Process.Start(startInfo))
                    {
                        process.WaitForExit();
                        if (process.ExitCode == 0)
                        {
                            MessageBox.Show($"Database restored from {fileName}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            string error = process.StandardError.ReadToEnd();
                            MessageBox.Show($"Error: {error}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            string dbName = DbComboBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(dbName))
            {
                MessageBox.Show("Please select a database to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (MessageBox.Show($"Are you sure you want to delete the database \"{dbName}\"?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var conn = new NpgsqlConnection($"Host={Host};Username={User};Password={Password};Port={Port};Database=postgres"))
                    {
                        conn.Open();
                        using (var cmd = new NpgsqlCommand($"SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = @dbName;", conn))
                        {
                            cmd.Parameters.AddWithValue("dbName", dbName);
                            cmd.ExecuteNonQuery();
                        }
                        using (var cmd = new NpgsqlCommand($"DROP DATABASE \"{dbName}\";", conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show($"Database \"{dbName}\" deleted successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    RefreshDatabases();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RunSqlFilesButton_Click(object sender, RoutedEventArgs e)
        {
            string dbName = DbComboBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(dbName))
            {
                MessageBox.Show("Please select a target database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var openFileDialog = new OpenFileDialog
            {
                Filter = "SQL files (*.sql)|*.sql|All files (*.*)|*.*",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    foreach (string fileName in openFileDialog.FileNames)
                    {
                        var startInfo = new ProcessStartInfo
                        {
                            FileName = "psql",
                            Arguments = $"-h {Host} -U {User} -d {dbName} -f \"{fileName}\"",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true
                        };
                        startInfo.EnvironmentVariables["PGPASSWORD"] = Password;

                        using (var process = Process.Start(startInfo))
                        {
                            process.WaitForExit();
                            if (process.ExitCode != 0)
                            {
                                string error = process.StandardError.ReadToEnd();
                                MessageBox.Show($"Error executing {fileName}: {error}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    MessageBox.Show("Selected SQL files executed successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
