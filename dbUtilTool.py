import tkinter as tk
from tkinter import ttk, messagebox, filedialog
import psycopg2
import subprocess
import os

class PostgreSQLManager:
    def __init__(self, master):
        self.master = master
        master.title("PostgreSQL Database Manager")
        master.geometry("400x450")

        # データベース接続情報
        self.host = "localhost"
        self.user = "postgres"
        self.password = "XXXXXXXXXXXXXXXXX"
        self.port = "5432"

        # PostgreSQLのバイナリディレクトリをPATHに追加
        self.postgres_bin_path = "C:\\PostgreSQL\\16\\bin"
        os.environ["PATH"] += os.pathsep + self.postgres_bin_path

        # UI要素
        self.create_widgets()

    def create_widgets(self):
        # データベース選択
        ttk.Label(self.master, text="Database:").grid(row=0, column=0, padx=5, pady=5)
        self.db_combo = ttk.Combobox(self.master, width=30)
        self.db_combo.grid(row=0, column=1, padx=5, pady=5)
        self.refresh_button = ttk.Button(self.master, text="Refresh", command=self.refresh_databases)
        self.refresh_button.grid(row=0, column=2, padx=5, pady=5)

        # 新規データベース作成
        ttk.Label(self.master, text="New DB:").grid(row=1, column=0, padx=5, pady=5)
        self.new_db_entry = ttk.Entry(self.master, width=30)
        self.new_db_entry.grid(row=1, column=1, padx=5, pady=5)
        self.create_button = ttk.Button(self.master, text="Create", command=self.create_database)
        self.create_button.grid(row=1, column=2, padx=5, pady=5)

        # リネーム機能
        ttk.Label(self.master, text="New name:").grid(row=2, column=0, padx=5, pady=5)
        self.new_name_entry = ttk.Entry(self.master, width=30)
        self.new_name_entry.grid(row=2, column=1, padx=5, pady=5)
        self.rename_button = ttk.Button(self.master, text="Rename", command=self.rename_database)
        self.rename_button.grid(row=2, column=2, padx=5, pady=5)

        # バックアップ機能
        self.backup_button = ttk.Button(self.master, text="Backup", command=self.backup_database)
        self.backup_button.grid(row=3, column=1, padx=5, pady=5)

        # リストア機能
        self.restore_button = ttk.Button(self.master, text="Restore", command=self.restore_database)
        self.restore_button.grid(row=4, column=1, padx=5, pady=5)

        # 削除機能
        self.delete_button = ttk.Button(self.master, text="Delete", command=self.delete_database)
        self.delete_button.grid(row=5, column=1, padx=5, pady=5)

        # SQLファイル実行機能
        self.run_sql_files_button = ttk.Button(self.master, text="Run SQL Files", command=self.run_sql_files)
        self.run_sql_files_button.grid(row=6, column=1, padx=5, pady=5)

        self.refresh_databases()

    def refresh_databases(self):
        try:
            conn = psycopg2.connect(
                host=self.host,
                user=self.user,
                password=self.password,
                port=self.port,
                database="postgres"
            )
            cur = conn.cursor()
            cur.execute("SELECT datname FROM pg_database WHERE datistemplate = false ORDER BY LOWER(datname) ASC;")
            databases = [row[0] for row in cur.fetchall()]
            self.db_combo['values'] = databases
            if databases:
                self.db_combo.set(databases[0])
            cur.close()
            conn.close()
        except Exception as e:
            messagebox.showerror("Error", str(e))

    def create_database(self):
        new_db_name = self.new_db_entry.get()
        if not new_db_name:
            messagebox.showerror("Error", "Please enter a name for the new database.")
            return
        try:
            conn = psycopg2.connect(
                host=self.host,
                user=self.user,
                password=self.password,
                port=self.port,
                database="postgres"
            )
            conn.autocommit = True
            cur = conn.cursor()
            cur.execute(f'CREATE DATABASE "{new_db_name}";')
            cur.close()
            conn.close()
            messagebox.showinfo("Success", f'Database "{new_db_name}" created successfully')
            self.refresh_databases()
        except Exception as e:
            messagebox.showerror("Error", str(e))

    def rename_database(self):
        old_name = self.db_combo.get()
        new_name = self.new_name_entry.get()
        if not old_name or not new_name:
            messagebox.showerror("Error", "Please select a database and enter a new name.")
            return
        try:
            conn = psycopg2.connect(
                host=self.host,
                user=self.user,
                password=self.password,
                port=self.port,
                database="postgres"
            )
            conn.autocommit = True
            cur = conn.cursor()
            cur.execute(f'ALTER DATABASE "{old_name}" RENAME TO "{new_name}";')
            cur.close()
            conn.close()
            messagebox.showinfo("Success", f'Database renamed from "{old_name}" to "{new_name}"')
            self.refresh_databases()
        except Exception as e:
            messagebox.showerror("Error", str(e))

    def backup_database(self):
        db_name = self.db_combo.get()
        if not db_name:
            messagebox.showerror("Error", "Please select a database.")
            return
        file_path = filedialog.asksaveasfilename(
            defaultextension=".backup",
            filetypes=[("Backup files", "*.backup"), ("All files", "*.*")]
        )
        if file_path:
            try:
                subprocess.run([
                    "pg_dump",
                    "-h", self.host,
                    "-U", self.user,
                    "-d", db_name,
                    "-Fc",
                    "-f", file_path
                ], check=True, env=dict(os.environ, PGPASSWORD=self.password))
                messagebox.showinfo("Success", f'Database "{db_name}" backed up to {file_path}')
            except subprocess.CalledProcessError as e:
                messagebox.showerror("Error", str(e))

    def restore_database(self):
        file_path = filedialog.askopenfilename(
            title="Select Backup File to Restore",
            filetypes=[
                ("Backup files", "*.backup"),
                ("SQL files", "*.sql"),
                ("All files", "*.*")
            ]
        )
        if file_path:
            db_name = self.db_combo.get()
            if not db_name:
                messagebox.showerror("Error", "Please select a target database.")
                return
            try:
                if file_path.endswith('.sql'):
                    # SQLファイルの場合
                    subprocess.run([
                        "psql",
                        "-h", self.host,
                        "-U", self.user,
                        "-d", db_name,
                        "-f", file_path
                    ], check=True, env=dict(os.environ, PGPASSWORD=self.password))
                else:
                    # カスタム形式のバックアップファイルの場合
                    subprocess.run([
                        "pg_restore",
                        "-h", self.host,
                        "-U", self.user,
                        "-d", db_name,
                        "-v",
                        file_path
                    ], check=True, env=dict(os.environ, PGPASSWORD=self.password))
                messagebox.showinfo("Success", f'Database restored from {file_path}')
            except subprocess.CalledProcessError as e:
                messagebox.showerror("Error", str(e))

    def delete_database(self):
        db_name = self.db_combo.get()
        if not db_name:
            messagebox.showerror("Error", "Please select a database to delete.")
            return
        
        # 確認ダイアログを表示
        if messagebox.askyesno("Confirm Delete", f'Are you sure you want to delete the database "{db_name}"?'):
            try:
                conn = psycopg2.connect(
                    host=self.host,
                    user=self.user,
                    password=self.password,
                    port=self.port,
                    database="postgres"
                )
                conn.autocommit = True
                cur = conn.cursor()
                
                # 接続を切断
                cur.execute(f"""
                    SELECT pg_terminate_backend(pid)
                    FROM pg_stat_activity
                    WHERE datname = %s;
                """, (db_name,))
                
                # データベースを削除
                cur.execute(f'DROP DATABASE "{db_name}";')
                cur.close()
                conn.close()
                messagebox.showinfo("Success", f'Database "{db_name}" deleted successfully')
                self.refresh_databases()
            except Exception as e:
                messagebox.showerror("Error", str(e))

    def run_sql_files(self):
        db_name = self.db_combo.get()
        if not db_name:
            messagebox.showerror("Error", "Please select a target database.")
            return

        file_paths = filedialog.askopenfilenames(
            title="Select SQL Files",
            filetypes=[("SQL Files", "*.sql"), ("All files", "*.*")]
        )

        if not file_paths:
            messagebox.showinfo("Info", "No files selected.")
            return

        try:
            for file_path in file_paths:
                subprocess.run([
                    "psql",
                    "-h", self.host,
                    "-U", self.user,
                    "-d", db_name,
                    "-f", file_path
                ], check=True, env=dict(os.environ, PGPASSWORD=self.password))
            messagebox.showinfo("Success", f"Selected SQL files executed successfully")
        except subprocess.CalledProcessError as e:
            messagebox.showerror("Error", str(e))

root = tk.Tk()
app = PostgreSQLManager(root)
root.mainloop()
