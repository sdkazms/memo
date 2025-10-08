namespace WindowsFormsTestChangeFormApp
{
    partial class FormB
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnB = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnB
            // 
            this.btnB.Location = new System.Drawing.Point(212, 116);
            this.btnB.Name = "btnB";
            this.btnB.Size = new System.Drawing.Size(298, 132);
            this.btnB.TabIndex = 0;
            this.btnB.Text = "buttonB";
            this.btnB.UseVisualStyleBackColor = true;
            this.btnB.Click += new System.EventHandler(this.btnB_Click);
            // 
            // FormB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(708, 406);
            this.Controls.Add(this.btnB);
            this.Name = "FormB";
            this.Text = "FormB";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormB_FormClosed);
            this.Load += new System.EventHandler(this.FormB_Load);
            this.Shown += new System.EventHandler(this.FormB_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnB;
    }
}