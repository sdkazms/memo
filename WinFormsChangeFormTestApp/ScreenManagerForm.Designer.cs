namespace WindowsFormsTestChangeFormApp
{
    partial class ScreenManagerForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnTaskStart = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnTaskStart
            // 
            this.btnTaskStart.Location = new System.Drawing.Point(37, 31);
            this.btnTaskStart.Name = "btnTaskStart";
            this.btnTaskStart.Size = new System.Drawing.Size(111, 37);
            this.btnTaskStart.TabIndex = 0;
            this.btnTaskStart.Text = "Task送信";
            this.btnTaskStart.UseVisualStyleBackColor = true;
            this.btnTaskStart.Click += new System.EventHandler(this.btnTaskStart_Click);
            // 
            // ScreenManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(223, 89);
            this.Controls.Add(this.btnTaskStart);
            this.Name = "ScreenManagerForm";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnTaskStart;
    }
}

