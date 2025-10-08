namespace WindowsFormsTestChangeFormApp
{
    partial class FormA
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
            this.btnA = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnA
            // 
            this.btnA.Location = new System.Drawing.Point(193, 103);
            this.btnA.Name = "btnA";
            this.btnA.Size = new System.Drawing.Size(251, 109);
            this.btnA.TabIndex = 0;
            this.btnA.Text = "buttonA";
            this.btnA.UseVisualStyleBackColor = true;
            // 
            // FormA
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(662, 351);
            this.Controls.Add(this.btnA);
            this.Name = "FormA";
            this.Text = "FormA";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormA_FormClosed);
            this.Load += new System.EventHandler(this.FormA_Load);
            this.Shown += new System.EventHandler(this.FormA_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnA;
    }
}