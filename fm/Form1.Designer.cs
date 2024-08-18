using CustomButtonControl;
using System.Drawing;
using System.Windows.Forms;

namespace SimpleAppForm
{
    partial class Form1
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
            this.transparentPanel = new SimpleAppForm.TransparentPanel();
            this.transparentPanel1 = new SimpleAppForm.TransparentPanel();
            this.button2 = new CustomButtonControl.FontButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.NoFontOverrideButton = new CustomButtonControl.NoFontButton();
            this.fontButton1 = new CustomButtonControl.FontButton();
            this.transparentPanel.SuspendLayout();
            this.transparentPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // transparentPanel
            // 
            this.transparentPanel.BackgroundImage = global::SimpleAppForm.Resource1.blue_base;
            this.transparentPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.transparentPanel.Controls.Add(this.transparentPanel1);
            this.transparentPanel.Location = new System.Drawing.Point(12, 12);
            this.transparentPanel.Name = "transparentPanel";
            this.transparentPanel.Size = new System.Drawing.Size(1021, 330);
            this.transparentPanel.TabIndex = 0;
            // 
            // transparentPanel1
            // 
            this.transparentPanel1.BackgroundImage = global::SimpleAppForm.Resource1.blue_base;
            this.transparentPanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.transparentPanel1.Controls.Add(this.button2);
            this.transparentPanel1.Controls.Add(this.panel1);
            this.transparentPanel1.Location = new System.Drawing.Point(27, 16);
            this.transparentPanel1.Name = "transparentPanel1";
            this.transparentPanel1.Size = new System.Drawing.Size(975, 291);
            this.transparentPanel1.TabIndex = 2;
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("MS UI Gothic", 10F);
            this.button2.Location = new System.Drawing.Point(629, 48);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(292, 188);
            this.button2.TabIndex = 1;
            this.button2.Text = "FontOverrideButton";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.panel1.Controls.Add(this.NoFontOverrideButton);
            this.panel1.Location = new System.Drawing.Point(74, 29);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(500, 239);
            this.panel1.TabIndex = 0;
            // 
            // NoFontOverrideButton
            // 
            this.NoFontOverrideButton.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.NoFontOverrideButton.Location = new System.Drawing.Point(68, 49);
            this.NoFontOverrideButton.Margin = new System.Windows.Forms.Padding(4);
            this.NoFontOverrideButton.Name = "NoFontOverrideButton";
            this.NoFontOverrideButton.Size = new System.Drawing.Size(371, 170);
            this.NoFontOverrideButton.TabIndex = 0;
            this.NoFontOverrideButton.Text = "NoFontOverrideButton";
            this.NoFontOverrideButton.UseVisualStyleBackColor = false;
            this.NoFontOverrideButton.Click += new System.EventHandler(this.panelButton_Click);
            // 
            // fontButton1
            // 
            this.fontButton1.Font = new System.Drawing.Font("MS UI Gothic", 10F);
            this.fontButton1.Location = new System.Drawing.Point(12, 214);
            this.fontButton1.Margin = new System.Windows.Forms.Padding(4);
            this.fontButton1.Name = "fontButton1";
            this.fontButton1.Size = new System.Drawing.Size(292, 188);
            this.fontButton1.TabIndex = 2;
            this.fontButton1.Text = "FontOverrideButton";
            this.fontButton1.UseVisualStyleBackColor = false; // 背景色を有効にする
            this.fontButton1.BackColor = Color.FromArgb(128, Color.White); // 半透明の白色に設定
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SimpleAppForm.Resource1.background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1067, 562);
            //this.Controls.Add(this.fontButton1);
            //this.Controls.Add(this.transparentPanel);
            // コントロールの追加順序を変更
            this.Controls.Add(this.transparentPanel);
            this.Controls.Add(this.fontButton1);

            // fontButton1を最前面に配置
            this.fontButton1.BringToFront();

            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.transparentPanel.ResumeLayout(false);
            this.transparentPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private TransparentPanel transparentPanel;
        private NoFontButton NoFontOverrideButton;
        private FontButton button2;
        private TransparentPanel transparentPanel1;
        private FontButton fontButton1;
    }
}

